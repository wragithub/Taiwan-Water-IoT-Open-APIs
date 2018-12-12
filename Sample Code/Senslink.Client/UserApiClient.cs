using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Senslink.Client.Enum;
using Senslink.Client.Models;
using Senslink.Client.OAuth2;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Senslink.Client.UserApi
{
    public class UserApiClient
    {
        #region Private 

        private Uri _senslinkUri;
        private OAuth2Client _oauth2Client;

        #endregion

        #region Private Methods

        private HttpStatusCode HttpPost(string additionalUri, string body, out string responseString)
        {
            OAuth2Client.AccessToken ac = _oauth2Client.GetAccessToken(false);
            if (ac == null)
            {
                responseString = null;
                return HttpStatusCode.BadRequest;
            }

            responseString = string.Empty;
            HttpClient client = new HttpClient();

            client.BaseAddress = _senslinkUri;

            // We want the response to be JSON.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _oauth2Client.GetAccessToken(false).access_token);

            // Build up the data to POST.
            StringContent content = new StringContent(body);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            Task<HttpResponseMessage> postTask = null;

            try
            {
                postTask = client.PostAsync(additionalUri, content);
                postTask.Wait(50000);
            }
            catch { }

            HttpStatusCode responseCode = HttpStatusCode.BadRequest;

            if (postTask.IsCompleted && !postTask.IsFaulted && !postTask.IsCanceled)
            {
                HttpResponseMessage message = postTask.Result;
                if (message.IsSuccessStatusCode)
                {
                    Task<string> responseContent = message.Content.ReadAsStringAsync();
                    responseContent.Wait(5000);

                    if (responseContent.IsCompleted && !postTask.IsFaulted && !postTask.IsCanceled)
                    {
                        if (postTask.Result != null)
                        {
                            responseString = responseContent.Result;
                        }
                    }
                }
                responseCode = postTask.Result.StatusCode;
            }
            return responseCode;
        }

        private HttpStatusCode HttpGet(string additionalUri, out string responseString)
        {
            OAuth2Client.AccessToken ac = _oauth2Client.GetAccessToken(false);
            if (ac == null)
            {
                responseString = null;
                return HttpStatusCode.BadRequest;
            }

            responseString = string.Empty;
            HttpClient client = new HttpClient();

            client.BaseAddress = _senslinkUri;

            // We want the response to be JSON.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ac.access_token);

            // Build up the data to POST.
            Task<HttpResponseMessage> getTask = client.GetAsync(additionalUri);
            getTask.Wait(50000);

            if (getTask.IsCompleted && !getTask.IsFaulted && !getTask.IsCanceled)
            {
                HttpResponseMessage message = getTask.Result;
                if (message.IsSuccessStatusCode)
                {
                    Task<string> readTask = null;
                    try
                    {
                        readTask = message.Content.ReadAsStringAsync();
                    }
                    catch { }
                    readTask.Wait(5000);

                    if (readTask.IsCompleted && !readTask.IsFaulted && !readTask.IsCanceled)
                    {
                        if (getTask.Result != null)
                        {
                            responseString = readTask.Result;
                        }
                    }
                }
            }
            return getTask.Result.StatusCode;
        }

        private HttpStatusCode HttpDelete(string additionalUri, out string responseString)
        {
            OAuth2Client.AccessToken ac = _oauth2Client.GetAccessToken(false);
            if (ac == null)
            {
                responseString = null;
                return HttpStatusCode.BadRequest;
            }

            responseString = string.Empty;
            HttpClient client = new HttpClient();

            client.BaseAddress = _senslinkUri;

            // We want the response to be JSON.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ac.access_token);

            // Build up the data to POST.
            Task<HttpResponseMessage> deleteTask = client.DeleteAsync(additionalUri);
            deleteTask.Wait(50000);

            if (deleteTask.IsCompleted && !deleteTask.IsFaulted && !deleteTask.IsCanceled)
            {
                HttpResponseMessage message = deleteTask.Result;
                if (message.IsSuccessStatusCode)
                {
                    Task<string> readTask = null;
                    try
                    {
                        readTask = message.Content.ReadAsStringAsync();
                    }
                    catch { }
                    readTask.Wait(5000);

                    if (readTask.IsCompleted && !readTask.IsFaulted && !readTask.IsCanceled)
                    {
                        if (deleteTask.Result != null)
                        {
                            responseString = readTask.Result;
                        }
                    }
                }
            }
            return deleteTask.Result.StatusCode;
        }

        private HttpStatusCode HttpGetFile(string additionalUrl, out byte[] downloadedFile)
        {
            downloadedFile = null;
            OAuth2Client.AccessToken ac = _oauth2Client.GetAccessToken(false);
            if (ac == null)
            {
                return HttpStatusCode.BadRequest;
            }

            HttpClient client = new HttpClient();

            client.BaseAddress = _senslinkUri;

            // We want the response to be JSON.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _oauth2Client.GetAccessToken(false).access_token);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            Task<HttpResponseMessage> getTask = client.GetAsync(additionalUrl);

            Console.WriteLine("Downloading");
            while(!getTask.IsCompleted)
            {
                Console.Write(".");
                System.Threading.Thread.Sleep(1000);
            }

            HttpStatusCode responseCode = getTask.Result.StatusCode;
            if(getTask.Status == TaskStatus.RanToCompletion && getTask.Status != TaskStatus.Faulted && getTask.Status != TaskStatus.Canceled)
            {
                HttpContent content = getTask.Result.Content;
                Task<byte[]> readTask = content.ReadAsByteArrayAsync();

                while(!readTask.IsCompleted)
                {
                    Console.Write(".");
                    System.Threading.Thread.Sleep(1000);
                }
                downloadedFile = readTask.Result;
            }
            return responseCode;
        }

        private HttpStatusCode HttpPostMultiPart(out string responseString, string additionalUri, string filePath, bool overWrite, FileMetaData metaData)
        {
            OAuth2Client.AccessToken ac = _oauth2Client.GetAccessToken(false);
            if (ac == null)
            {
                responseString = null;
                return HttpStatusCode.BadRequest;
            }

            responseString = string.Empty;
            HttpClient client = new HttpClient();

            client.BaseAddress = _senslinkUri;

            // We want the response to be JSON.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _oauth2Client.GetAccessToken(false).access_token);

            FileInfo fi = new FileInfo(filePath);
            byte[] fileBytes = File.ReadAllBytes(filePath);
            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            //First Part - File
            ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.Remove("Content-Type");
            fileContent.Headers.Add("Content-Disposition", $"form-data; name=\"\"; filename=\"{fi.Name}\"");
            requestContent.Add(fileContent);

            //Second Part - metaData in JSON format
            string jsonContent = JsonConvert.SerializeObject(metaData);
            StringContent metaDataContent = new StringContent(jsonContent);
            metaDataContent.Headers.Remove("Content-Type");
            metaDataContent.Headers.Add("Content-Disposition", "form-data; name=\"metaData\"");
            requestContent.Add(metaDataContent);

            //Third Part - metaData in JSON format
            StringContent overWriteContent = new StringContent(overWrite.ToString());
            overWriteContent.Headers.Remove("Content-Type");
            overWriteContent.Headers.Add("Content-Disposition", "form-data; name=\"overWrite\"");
            requestContent.Add(overWriteContent);

            //Forth Part - metaData in JSON format
            StringContent sharedWithUsersContent = new StringContent(metaData.shareLevel.ToString());
            sharedWithUsersContent.Headers.Remove("Content-Type");
            sharedWithUsersContent.Headers.Add("Content-Disposition", "form-data; name=\"sharedWithUsers\"");
            requestContent.Add(sharedWithUsersContent);

            // Build up the data to Put

            Task<HttpResponseMessage> putTask = null;
            try
            {
                putTask = client.PostAsync(additionalUri, requestContent);
                putTask.Wait(50000);
            }
            catch { }

            HttpStatusCode responseCode = HttpStatusCode.BadRequest;

            if (putTask.IsCompleted && !putTask.IsFaulted && !putTask.IsCanceled)
            {
                HttpResponseMessage message = putTask.Result;
                if (message.IsSuccessStatusCode)
                {
                    Task<string> responseContent = message.Content.ReadAsStringAsync();
                    responseContent.Wait(5000);

                    if (responseContent.IsCompleted && !putTask.IsFaulted && !putTask.IsCanceled)
                    {
                        if (putTask.Result != null)
                        {
                            responseString = responseContent.Result;
                        }
                    }
                }
                responseCode = putTask.Result.StatusCode;
            }
            return responseCode;
        }


        private string AppendIncludedChildInfoString(string baseUrlStr, SenslinkInfoTypes includedChildInfos)
        {
            if (includedChildInfos == SenslinkInfoTypes.None)
                return baseUrlStr;

            IEnumerable<SenslinkInfoTypes> types = System.Enum.GetValues(typeof(SenslinkInfoTypes)).OfType<SenslinkInfoTypes>();
            StringBuilder sb = new StringBuilder($"{baseUrlStr}/{{");
            foreach (SenslinkInfoTypes cunType in types)
            {
                if (cunType == SenslinkInfoTypes.None)
                    continue;

                if (includedChildInfos.HasFlag(cunType))
                {
                    sb.Append($"{cunType.ToString()}|");
                }
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }


        #endregion

        #region Constructor

        public UserApiClient(Uri senslinkApiRootUri, OAuth2Client oauth2Client)
        {
            _senslinkUri = senslinkApiRootUri;
            _oauth2Client = oauth2Client;
        }

        #endregion

        #region ETL API - 監測站群組

        /// <summary>
        /// 6.5.1 ~ 6.5.2
        /// </summary>
        /// <param name="includedChildInfos"></param>
        /// <returns></returns>
        public HttpStatusCode StationGroupGetAll(out IEnumerable<StationGroupInfo> stationGroupInfos, SenslinkInfoTypes includedChildInfos)
        {
            string resposeString = string.Empty;
            StringBuilder sb = new StringBuilder("StationGroup/Get/All");

            if (includedChildInfos != SenslinkInfoTypes.None)
            {
                bool isFirstChild = true;
                sb.Append("/{");

                if (includedChildInfos.HasFlag(SenslinkInfoTypes.PhysicalQuantity))
                {
                    sb.Append("PhysicalQuantity");
                    isFirstChild = false;
                }

                if (includedChildInfos.HasFlag(SenslinkInfoTypes.Category))
                {
                    if (!isFirstChild)
                        sb.Append("|");
                    sb.Append("Category");
                    isFirstChild = false;
                }
                sb.Append("}");
            }

            HttpStatusCode resultCode = HttpGet(sb.ToString(), out resposeString);
            stationGroupInfos = null;

            if (resultCode == HttpStatusCode.OK)
            {
                JArray stationGroupJArray = JArray.Parse(resposeString);
                JToken[] stationGroupTokens = stationGroupJArray.ToArray();
                List<StationGroupInfo> parsedStationGroupInfos = new List<StationGroupInfo>();
                foreach (JToken stationGroupToken in stationGroupTokens)
                {
                    StationGroupInfo cunInfo = stationGroupToken.ToObject<StationGroupInfo>();
                    parsedStationGroupInfos.Add(cunInfo);
                }
                stationGroupInfos = parsedStationGroupInfos.ToList();
            }
            return resultCode;
        }

        public HttpStatusCode StationGroupGet(out StationGroupInfo stationGroupInfo, Guid id, SenslinkInfoTypes includedChildInfos)
        {
            string resposeString = string.Empty;
            StringBuilder sb = new StringBuilder($"StationGroup/Get/{id.ToString()}");

            if (includedChildInfos != SenslinkInfoTypes.None)
            {
                bool isFirstChild = true;
                sb.Append("/{");

                if (includedChildInfos.HasFlag(SenslinkInfoTypes.PhysicalQuantity))
                {
                    sb.Append("PhysicalQuantity");
                    isFirstChild = false;
                }

                if (includedChildInfos.HasFlag(SenslinkInfoTypes.Category))
                {
                    if (!isFirstChild)
                        sb.Append("|");
                    sb.Append("Category");
                    isFirstChild = false;
                }
                sb.Append("}");
            }

            HttpStatusCode resultCode = HttpGet(sb.ToString(), out resposeString);
            stationGroupInfo = null;

            if (resultCode == HttpStatusCode.OK)
            {
                JToken stationGroupToken = JToken.Parse(resposeString);
                stationGroupInfo = stationGroupToken.ToObject<StationGroupInfo>();
            }
            return resultCode;
        }

        #endregion

        #region ETL API - 監測站

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stationInfos"></param>
        /// <param name="includedChildInfos"></param>
        /// <returns></returns>
        public HttpStatusCode StationGetAll(out IEnumerable<StationInfo> stationInfos, SenslinkInfoTypes includedChildInfos)
        {
            string resposeString = string.Empty;
            string requestUrl = AppendIncludedChildInfoString("Station/Get/All", includedChildInfos);

            HttpStatusCode resultCode = HttpGet(requestUrl, out resposeString);
            stationInfos = null;
            if (resultCode == HttpStatusCode.OK)
            {
                JArray stationJArray = null;
                try
                {
                    stationJArray = JArray.Parse(resposeString);
                }
                catch
                {
                    Console.WriteLine("Get StationInfos Fail");
                    return HttpStatusCode.BadRequest;
                }
                JToken[] stationTokens = stationJArray.ToArray();
                List<StationInfo> parsedStationInfos = new List<StationInfo>();
                foreach (JToken stationToken in stationTokens)
                {
                    StationInfo cunInfo = stationToken.ToObject<StationInfo>();
                    parsedStationInfos.Add(cunInfo);
                }
                stationInfos = parsedStationInfos.ToList();
            }
            return resultCode;
        }

        public HttpStatusCode StationGet(out StationInfo stationInfo, Guid id, SenslinkInfoTypes includedChildInfos)
        {
            string responseString = string.Empty;
            string requestUrl = AppendIncludedChildInfoString($"Station/Get/{id.ToString()}", includedChildInfos);
            HttpStatusCode resultCode = HttpGet(requestUrl, out responseString);
            stationInfo = null;
            if (resultCode == HttpStatusCode.OK && responseString != string.Empty)
            {
                JToken stationToken = JToken.Parse(responseString);
                stationInfo = stationToken.ToObject<StationInfo>();
            }
            return resultCode;
        }


        #endregion

        #region ETL API - 物理量

        public HttpStatusCode PhysicalQuantityGetAll(out IEnumerable<PhysicalQuantityInfo> physicalQuantityInfos, SenslinkInfoTypes includedChildInfos)
        {
            string resposeString = string.Empty;
            string requestUrlStr = AppendIncludedChildInfoString("PhysicalQuantity/Get/All", includedChildInfos);
            HttpStatusCode resultCode = HttpGet(requestUrlStr, out resposeString);

            JArray physicalQuantityJArray = JArray.Parse(resposeString);
            JToken[] physicalQuantityTokens = physicalQuantityJArray.ToArray();
            List<PhysicalQuantityInfo> parsedPqInfos = new List<PhysicalQuantityInfo>();

            foreach (JToken physicalQuantityToken in physicalQuantityTokens)
                parsedPqInfos.Add(physicalQuantityToken.ToObject<PhysicalQuantityInfo>());
            
            physicalQuantityInfos = parsedPqInfos.ToList();
            return resultCode;
        }

        /// <summary>
        /// 6.7.2 ~ 6.7.4
        /// </summary>
        /// <param name="physicalQuantityInfo"></param>
        /// <param name="id"></param>
        /// <param name="includedChildInfos"></param>
        /// <returns></returns>
        public HttpStatusCode PhysicalQuantityGet(out PhysicalQuantityInfo physicalQuantityInfo, Guid id, SenslinkInfoTypes includedChildInfos)
        {
            string responseString = string.Empty;
            string requestUrlStr = AppendIncludedChildInfoString($"PhysicalQuantity/Get/{id.ToString()}", includedChildInfos);

            HttpStatusCode resultCode = HttpGet(requestUrlStr, out responseString);
            physicalQuantityInfo = JToken.Parse(responseString).ToObject<PhysicalQuantityInfo>();
            return resultCode;
        }

        #endregion

        #region ETL API - 即時資料


        public HttpStatusCode LatestDataReadAll(out IEnumerable<DataPoint> dataPoints)
        {
            string responseString = string.Empty;
            HttpStatusCode resultCode = HttpGet("LatestData/Read/All", out responseString);

            JArray dataPointArray = JArray.Parse(responseString);
            dataPoints = dataPointArray.ToObject<DataPoint[]>();
            return resultCode;
        }

        public HttpStatusCode LastestDataReadPhysicalQuantity(out DataPoint dataPoint, Guid id)
        {
            string responseString = string.Empty;
            HttpStatusCode resultCode = HttpGet($"LatestData/Read/PhysicalQuantity/{id.ToString()}", out responseString);

            dataPoint = JToken.Parse(responseString).ToObject<DataPoint>();
            return resultCode;

        }

        public HttpStatusCode LastestDataReadStation(out IEnumerable<DataPoint> dataPoints, Guid id)
        {
            string responseString = string.Empty;
            HttpStatusCode resultCode = HttpGet($"LatestData/Read/Station/{id.ToString()}", out responseString);

            JArray dataPointArray = JArray.Parse(responseString);
            dataPoints = dataPointArray.ToObject<DataPoint[]>();
            return resultCode;
        }

        public HttpStatusCode LatestDataReadStationGroup(out IEnumerable<DataPoint> dataPoints, Guid id)
        {
            string responseString = string.Empty;
            HttpStatusCode resultCode = HttpGet($"LatestData/Read/StationGroup/{id.ToString()}", out responseString);

            JArray dataPointArray = JArray.Parse(responseString);
            dataPoints = dataPointArray.ToObject<DataPoint[]>();
            return resultCode;
        }

        #endregion

        #region ETL API - 歷史資料

        public HttpStatusCode TimeSeriesDataReadAggregateData(out DataSeriesEs dataSeries, Guid id, DateTimeOffset sDt, DateTimeOffset eDt, AggregateCalculationMethods agMethod, int agInterval, int timeZone)
        {
            string responseString = string.Empty;
            HttpStatusCode resultCode = HttpGet($"TimeSeriesData/ReadAggregatedData/{id.ToString()}/{sDt.ToString("yyyy-MM-ddTHH.mm.ss")}/{eDt.ToString("yyyy-MM-ddTHH.mm.ss")}/{((int)agMethod).ToString()}/{((int)agInterval).ToString()}/{((int)timeZone).ToString()}", out responseString);
            JToken token = JToken.Parse(responseString);
            dataSeries = token.ToObject<DataSeriesEs>();
            return resultCode;
        }

        public HttpStatusCode TimeSeriesDataReadRawData(out DataSeriesUs dataSeries, Guid id, DateTimeOffset sDt, DateTimeOffset eDt, bool ignoreNaN, int timeZone)
        {
            string responseString = string.Empty;
            HttpStatusCode resultCode = HttpGet($"TimeSeriesData/ReadRawData/{id.ToString()}/{sDt.ToString("yyyy-MM-ddTHH.mm.ss")}/{eDt.ToString("yyyy-MM-ddTHH.mm.ss")}/{((bool)ignoreNaN).ToString()}/{((int)timeZone).ToString()}", out responseString);
            JToken token = JToken.Parse(responseString);
            dataSeries = token.ToObject<DataSeriesUs>();
            return resultCode;
        }

        public HttpStatusCode DeleteData(Guid physicalQuantityId, DateTimeOffset sDt, DateTimeOffset eDt, bool isDeleteNaNOnly)
        {
            string resposneString = string.Empty;
            string uriStr = string.Format("TimeSeriesData/DeleteData/{0:s}/{1:yyyy-MM-dd}T{1:HH.mm.ss}/{2:yyyy-MM-dd}T{2:HH.mm.ss}/{3:s}", physicalQuantityId.ToString(), sDt, eDt, isDeleteNaNOnly.ToString());
            return HttpGet(uriStr, out resposneString);
        }

        public HttpStatusCode Write(DataPoint[] dataPoints)
        {
            string resposneString = string.Empty;
            return HttpPost("TimeSeriesData/Write", JsonConvert.SerializeObject(dataPoints), out resposneString);
        }

        public HttpStatusCode WriteFormulaTransferred(DataPoint[] dataPoints)
        {
            string resposneString = string.Empty;
            return HttpPost("TimeSeriesData/Write/FormulaTransferred", JsonConvert.SerializeObject(dataPoints), out resposneString);
        }

        #endregion

        #region 可視化元件 API

        public HttpStatusCode ChartGenerate(out string chartUrl, ChartProvider chartProvider, string chartBody)
        {
            HttpStatusCode resultCode = HttpPost($"Chart/Generate/{chartProvider.ToString()}", chartBody, out chartUrl);
            return resultCode;
        }

        #endregion

        #region 地理圖資 API - 檔案管理

        public HttpStatusCode RasterMapFileList(out IEnumerable<FileDetails> fileDetails)
        {
            string responseString = string.Empty;
            fileDetails = null;
            HttpStatusCode resultCode = HttpGet("RasterMap/File/List", out responseString);
            if ( resultCode == HttpStatusCode.OK )
            {
                if(responseString != null)
                {
                    JArray tokens = JArray.Parse(responseString);
                    fileDetails = tokens.ToObject<FileDetails[]>();
                }
            }
            return resultCode;
        }

        
        public HttpStatusCode RasterMapFileUpload(string filePath, bool overWrite, FileMetaData metaData)
        {
            string responseString = string.Empty;
            HttpStatusCode resultCode = HttpPostMultiPart(out responseString, "RasterMap/File/Upload", filePath, overWrite, metaData);
            return resultCode;
        }
       

        public HttpStatusCode RasterMapFileDownload(string saveFilePath, string fileName)
        {
            byte[] downloadedFile = null;
            HttpStatusCode resultCode = HttpGetFile($"RasterMap/File/Download/{fileName}/", out downloadedFile);
            if(resultCode == HttpStatusCode.OK)
            {
                FileStream fs = new FileStream($"{saveFilePath}\\downloaded\\{fileName}", FileMode.Create);
                fs.Write(downloadedFile, 0, downloadedFile.Length);
                fs.Close();
            }
            return resultCode;
        }

        public HttpStatusCode RasterMapFileDelete(string fileName, string responseString)
        {
            responseString = null;
            HttpStatusCode resultCode = HttpGet($"RasterMap/File/Delete/{fileName}", out responseString);
            return resultCode;
        }

        #endregion

        #region 地理圖資 API - NetCDF 操作

        public HttpStatusCode RasterMapNetCDFGet(string ownerName, string fileName, out string ASCIIContent)
        {
            ASCIIContent = null;
            HttpStatusCode resultCode = HttpGet($"RasterMap/NetCDF/Get/{ownerName}/{fileName}/", out ASCIIContent);
            return resultCode;
        }

        public HttpStatusCode RasterMapNetCDFGetGrid(string fileName, string variableName, int unixtimeInMins, float leftUpCornerX, float leftUpCornerY, float rightDownCornerX, float rightDownCornerY, out string ASCIIContent)
        {
            ASCIIContent = null;
            HttpStatusCode resultCode = HttpGet($"RasterMap/NetCDF/Get/{fileName}/{variableName}/{unixtimeInMins}/{leftUpCornerX}/{leftUpCornerY}/{rightDownCornerX}/{rightDownCornerY}", out ASCIIContent);
            return resultCode;
        }

        public HttpStatusCode RasterMapNetCDFExecute(string fileName, string query, out string responseString)
        {
            responseString = null;
            JObject jobject = new JObject(
                new JProperty("fileName", fileName),
                new JProperty("query", query)
                );
            return HttpPost($"RasterMap/NetCDF/Execute", jobject.ToString(), out responseString);
        }

        #endregion

        #region 地理圖資 API - WMS 操作

        public HttpStatusCode WMSGetCapabilites(string fileName, out string capabilitiesUrl)
        {
            capabilitiesUrl = null;
            return HttpGet($"WMS/Get/Capabilities/{fileName}", out capabilitiesUrl);
        }

        public HttpStatusCode WMSGetImageInfos(string fileName, out string responseString)
        {
            responseString = null;
            return HttpGet($"WMS/Get/ImageInfos/{fileName}", out responseString);
        }

        #endregion

        #region 雲端運算 API - Job 操作

        public HttpStatusCode SparkJobStatus(int jobId, out JobInfo jobInfo)
        {
            string responseString = null;
            HttpStatusCode resultCode = HttpGet($"Spark/Job/Status/{jobId}", out responseString);
            jobInfo = null;
            if (resultCode == HttpStatusCode.OK)
            {
                JObject jobject = JObject.Parse(responseString);
                JToken jtoken = jobject["result"];
                jobInfo = jtoken.ToObject<JobInfo>();
            }
            return resultCode;
        }

        public HttpStatusCode SparkJobLog(int jobId, out string reasponseStirng)
        {
            reasponseStirng = null;
            return HttpGet($"Spark/Job/Log/{jobId}", out reasponseStirng);
        }

        public HttpStatusCode SparkJobList(out JobInfo[] jobInfos)
        {
            string responseString = null;
            HttpStatusCode resultCode = HttpGet($"Spark/Job/List", out responseString);
            jobInfos = null;
            if (resultCode == HttpStatusCode.OK)
            {
                JObject jobject = JObject.Parse(responseString);
                JArray jarray = (JArray)jobject["results"];
                JToken[] jtokens = jarray.ToArray();
                JobInfo cunJobInfo = null;
                List<JobInfo> jobInfosList = new List<Models.JobInfo>();
                foreach (JToken token in jtokens)
                {
                    try
                    {
                        cunJobInfo = token.ToObject<JobInfo>();
                    }
                    catch { continue; }
                    jobInfosList.Add(cunJobInfo);
                }
                jobInfos = jobInfosList.ToArray();
            }
            return resultCode;
        }

        public HttpStatusCode SparkJobGeneralSubmit(SubmitGeneralJobParams submitJobParams, out int jobId)
        {
            jobId = 0;
            string responseString = null;
            string jsonString = JsonConvert.SerializeObject(submitJobParams);
            HttpStatusCode resultCode = HttpPost($"Spark/Job/General/Submit", jsonString, out responseString);
            if(resultCode == HttpStatusCode.OK)
            {
                JObject jobject = JObject.Parse(responseString);
                jobId = jobject["JobId"].ToObject<int>();
            }
            return resultCode;
        }

        public HttpStatusCode SparkJobDelete(int jobId, out string responsString)
        {
            responsString = string.Empty;
            HttpStatusCode resultCode = HttpDelete($"Spark/Job/Delete/{jobId}", out responsString);
            return resultCode;
        }

        #endregion
    }
}
