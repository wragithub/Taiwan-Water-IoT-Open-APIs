using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Senslink.Client.Models
{
    /// <summary>
    ///     This API interacts with the Spark Service API provided by ITRII.
    /// </summary>
    public class SparkApi
    {
        #region Private Fields

        private readonly byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private readonly byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private SubmitPredefinedJobParams userInput;
        private string sparkWebServiceIp;

        private string NodeUrl { get; }
        private string DeleteUrl { get; }
        private string JobId { get; set; }
        private string FileName { get; set; }
        private SparkPredefined SparkPre { get; set; }
        private SparkGeneral SparkGen { get; set; }
        private Model ModelInfo { get; set; }
        private S3PredefinedJob S3Pre { get; set; }
        private S3GerneralJob S3Gen { get; set; }
        #endregion

        #region Private Structures

        private struct DeleteResult
        {
            [JsonProperty(PropertyName = "Result")]
            public string Message { get; set; }
        }

        private struct DeleteRequest
        {
            public string role { get; set; }
            public string id { get; set; }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes the spark api provider.
        /// </summary>
        /// <param name="webServiceIp">Spark web service IP address</param>
        public SparkApi(string webServiceIp)
        {
            NodeUrl = $"http://{webServiceIp}/sparkapi/v1.0/sparkapp";
            DeleteUrl = $"http://{webServiceIp}/sparkapi/v1.0/delsparkapp";
        }

        /// <summary>
        ///     Initializes the spark api provider.
        /// </summary>
        public SparkApi(SubmitGeneralJobParams instance, string webServiceIp)
        {
            SparkGen = instance.Spark;
            S3Gen = instance.S3;

            NodeUrl = $"http://{webServiceIp}/sparkapi/v1.0/sparkapp";
            DeleteUrl = $"http://{webServiceIp}/sparkapi/v1.0/delsparkapp";
        }

        public SparkApi(SubmitPredefinedJobParams instance, string webServiceIp)
        {
            Model.ModelNames mainFile;
            System.Enum.TryParse(instance.Model.Name, out mainFile);

            SparkPre = instance.Spark;
            SparkPre.MainFile = instance.Model.GetMainFileName(mainFile);

            S3Pre = instance.S3;
            ModelInfo = instance.Model;

            NodeUrl = $"http://{webServiceIp}/sparkapi/v1.0/sparkapp";
            DeleteUrl = $"http://{webServiceIp}/sparkapi/v1.0/delsparkapp";

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Submits a job to the spark server.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string SubmitJob(Stream file, string fileName)
        {
            if (file == null)
                return "Error: could not download file from S3";

            FileName = fileName;
            return Upload(file);
        }

        /// <summary>
        /// Submits a job to the spark server.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string SubmitGeneralJob(Stream file, string fileName)
        {
            if (file == null)
                return "Error: could not download file from S3";      
         
            FileName = fileName;
            return UploadGen(file);
        }

        /// <summary>
        ///     Return the current status of the job on the spark server.
        /// </summary>
        /// <param name="jobId">Job id of the submitted job.</param>
        /// <param name="roleName">Role name on the spark server.</param>
        /// <returns></returns>
        public string GetJobStatus(long jobId, string roleName)
        {
            string status = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string queryUrl = $"{NodeUrl}?id={jobId}&role={roleName}";
                    using (Task<HttpResponseMessage> message = client.GetAsync(queryUrl))
                    {
                        status = message.Result.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

            return status;
        }

        /// <summary>
        ///     Returns all the current jobs for a role.
        /// </summary>
        /// <param name="roleName"></param>
        public string GetRoleJobs(string roleName)
        {
            using (HttpClient client = new HttpClient())
            {
                string queryUrl = $"{NodeUrl}s?role={roleName}";
                using (Task<HttpResponseMessage> message = client.GetAsync(queryUrl))
                {
                    string response = message.Result.Content.ReadAsStringAsync().Result;
                    return response;
                }
            }
        }

        /// <summary>
        ///     Deletes a job from the spark server.
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public string DeleteJob(long jobId, string roleName)
        {
            using (HttpClient client = new HttpClient())
            {
                DeleteRequest queryData = new DeleteRequest() { role = roleName, id = jobId.ToString() };
                using (Task<HttpResponseMessage> message = client.PutAsJsonAsync(DeleteUrl, queryData))
                {
                    string response = message.Result.Content.ReadAsStringAsync().Result;
                    return response;
                }
            }
        }

        #endregion

        #region Private Methods

        private string Upload(Stream fileStream)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //multipart/form-data contains boundary to separate name/value pairs. 
                    using (MultipartFormDataContent content =
                        new MultipartFormDataContent($"Upload----{DateTime.Now.ToString(CultureInfo.InvariantCulture)}")
                    )
                    {
                        content.Add(new StreamContent(fileStream), "file", FileName);
                        content.Add(new StringContent($"com.anasystem.spark.{SparkPre.MainFile}"), "mainfile");
                        content.Add(new StringContent(SparkPre.Account), "role"); //"20180606"
                        content.Add(new StringContent(SparkPre.Priority ?? ""), "priority");
                        //content.Add(new StringContent(SparkConfig.Arguments ?? ""), "argument");
                        content.Add(new StringContent(ArgumentConfig() ?? ""), "argument");

                        content.Add(new StringContent(JsonConvert.SerializeObject(SparkPre.Workers)), "worker");
                        content.Add(new StringContent(SparkPre.Driver), "driver");

                        //  new StringContent(stringPayload, Encoding.UTF8, "application/json");
                        using (Task<HttpResponseMessage> message = client.PostAsync(NodeUrl, content))
                        {
                            JobId = message.Result.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                
            }

            return JobId;
        }


        private string UploadGen(Stream fileStream)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //multipart/form-data contains boundary to separate name/value pairs. 
                    using (MultipartFormDataContent content =
                        new MultipartFormDataContent($"Upload----{DateTime.Now.ToString(CultureInfo.InvariantCulture)}")
                    )
                    {
                        content.Add(new StreamContent(fileStream), "file", FileName);
                        content.Add(new StringContent(SparkGen.MainFile), "mainfile");
                        content.Add(new StringContent(SparkGen.Account), "role"); //"20180606"
                        content.Add(new StringContent(SparkGen.Priority ?? ""), "priority");
                        //content.Add(new StringContent(SparkConfig.Arguments ?? ""), "argument");
                        content.Add(new StringContent(SparkGen.Arguments ?? ""), "argument");

                        content.Add(new StringContent(JsonConvert.SerializeObject(SparkGen.Workers)), "worker");
                        content.Add(new StringContent(SparkGen.Driver), "driver");

                        //  new StringContent(stringPayload, Encoding.UTF8, "application/json");
                        using (Task<HttpResponseMessage> message = client.PostAsync(NodeUrl, content))
                        {
                            JobId = message.Result.Content.ReadAsStringAsync().Result;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                
            }

            return JobId;
        }

        private string ArgumentConfig()
        {
            string s3 = $"10.57.225.25:7480   {S3Pre.AccessKey}    {S3Pre.SecretKey}";
            string generalArgs = s3 + $"  {Bucket(S3Pre.TrainingData)}  {Key(S3Pre.TrainingData)}  {Bucket(S3Pre.OutputPath)} {Key(S3Pre.OutputPath)} output {SparkPre.MainFile}Out.txt /root/nfsshare/ {ModelInfo.Parameters}";
            #region tested 
            /*
             * 
            //string arg1 = s3 + "   data sample_associationrules.csv    models    AssociationrulesTrain output ARTrainOut.txt /root/nfsshare/ 0.8";

            //string arg2 = s3 + " data   iris.csv    models DtTrain output DtTrainOut.txt /root/nfsshare/ 4 5 30";

            //string arg3 = s3 + " data sample_fpgrowth.csv models FPgrowthTrain output FPgrowthOut.txt /root/nfsshare/ 0.2 10";

            //string arg4 = s3 + " data irisTwoClass.csv models GradientBoostedTrain output GradientBoostedTrain.txt /root/nfsshare/ 4 5 30 10";


            //string arg5 = s3 + " data lpsa.csv models LRWithLassoTrain output LRWithLassoTrain.txt /root/nfsshare/ 100 1.0  0.01 1.0";

            //string arg6 = s3 + " data lpsa.csv models LRWithRidgeTrain output LRWithRidgeTrain.txt /root/nfsshare/ 100 1.0  0.01 1.0";

            //string arg7 = s3 + " data lpsa.csv models LRWithSGDTrain output LRWithSGDTrain.txt /root/nfsshare/ 100 1.0  1.0";

            //string arg8 = s3 + " data irisTwoClass.csv models LinearSVMWithSGDTrain output LinearSVMWithSGDTrain.txt /root/nfsshare/ 100 1.0  0.01  1.0";

            //string arg9 = s3 + " data iris.csv models LogisticRegressionTrain output LogisticRegressionTrain.txt /root/nfsshare/ 10";
            //string arg10 = s3 + " data sample_prefixspan.csv models PrefixspanTrain output PrefixspanTrain.txt /root/nfsshare/ 0.5  5";
            //string arg11 = s3 + " data iris.csv models RandomForestTrain output RandomForestTrain.txt /root/nfsshare/ 4 5 30 3";
            */

            //PrefixspanTrain RandomForestTrain 
            #endregion

            string val = Encrypt(generalArgs);

            return val;
        }

        private string Encrypt(string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = ASCIIEncoding.ASCII.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        private string Bucket(string path)
        {
            //bucket name and key name are separated by '/'
            string[] s3Input = path.Split('/');
            return s3Input[0];

        }
        private string Key(string path)
        {
            //bucket name and key name are separated by '/'
            string[] s3Input = path.Split('/');
            string dataBucketName = s3Input[0];
            //note that the key name could contain multiple slashes /folder/folder/key-name
            string fileName = string.Join("/", s3Input.Skip(1).ToArray());

            return fileName;
        }

        #endregion


    }
}