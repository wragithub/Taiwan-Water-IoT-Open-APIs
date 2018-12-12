using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using Senslink.Client.Models;
using Senslink.Client.Enum;
using Senslink.Client.UserApi;
using Senslink.Client.OAuth2;
using Senslink.Client.Mqtt;
using Newtonsoft.Json.Linq;

namespace ClientDemo
{
    class Program
    {
        #region Private Filelds

        //OAuth2 認證網址
        static Uri _wraOAuth2UriRoot = new Uri("https://iapi.wra.gov.tw/v3/oauth2/");

        //WRA User API Root 網址
        static Uri _wraApiUriRoot = new Uri("https://iapi.wra.gov.tw/v3/api/");

        static OAuth2Client _oAuth2Client;
        static UserApiClient _userApiClient;

        //OAuth2 認證所需之金鑰ID與密碼
        static string _clientId = "";
        static string _clientSecret = "";

        #endregion

        static void Main(string[] args)
        {
            
            #region MQTT Client 資料上傳範例
                      
            IPHostEntry wraIP = Dns.GetHostEntry("iapi.wra.gov.tw");
            
            MqttClient.ConnectConfig cc = new MqttClient.ConnectConfig()
            {

                IpAddress = wraIP.AddressList[0],
                Port = 1883,
                UserName = "",
                Password = ""
            };

            MqttClient dps = new MqttClient(cc);

            dps.Open();
            Console.WriteLine("啟動MQTT連線");
            Console.ReadKey();

            if (!dps.IsConnected)
            {
                Console.WriteLine("連線失敗");
                Console.ReadKey();
            }

            //產生隨機數值
            Random rnd = new Random();

            //寫入物理量 Id
            Guid pqId = Guid.Parse("");
            Console.WriteLine("Publish Data..");

            for(int index = 0; index < 20; index++)
            {
                //Mqtt Payload 格式
                MqttClient.ObservationsPacket op = new MqttClient.ObservationsPacket()
                {
                    phenomenonTime = DateTime.Now.AddMinutes(-index).ToString("yyyyMMddTHHmmss+08"),
                    result = 4 + rnd.NextDouble()
                };

                Console.WriteLine($"發送資料 {op.result.ToString()}");

                dps.Publish(pqId, op);
                Thread.Sleep(1000);
            }
            Console.WriteLine("Publish Data Finish.");
            Console.ReadKey();
            dps.Close();

            #endregion
            
            
            #region 初始化 OAuth2 Client and User API Client

            //初始化 OAuth2 Client
            _oAuth2Client = new OAuth2Client(_wraOAuth2UriRoot, _clientId, _clientSecret);
            
            //初始化 UserApi Client
            _userApiClient = new UserApiClient(_wraApiUriRoot, _oAuth2Client);

            #endregion

            
            #region ETL API 使用範例

            Console.WriteLine("\r\n-----讀取全部監測站群組------");
            IEnumerable<StationGroupInfo> stationGroupInfos = StationGroupGetAll();
            Console.ReadKey();

            //取得單一監測站群組
            Console.WriteLine($"\r\n-----使用監測站群組ID讀取單一監測站群組 {stationGroupInfos.First().Id.ToString() }------");
            StationGroupGet(stationGroupInfos.First().Id);
            Console.ReadKey();

            //取得全部監測站
            Console.WriteLine($"\r\n-----取得全部監測站------");
            IEnumerable<StationInfo> stationInfos = StationGetAll();
            Console.ReadKey();

            //取得單一監測站
            Console.WriteLine($"\r\n-----取得單一監測站 {stationInfos.First().Id.ToString() }------");
            StationGet(stationInfos.First().Id);
            Console.ReadKey();

            //取得全部物理量
            Console.WriteLine($"\r\n-----取得全部物理量------");
            IEnumerable<PhysicalQuantityInfo> physicalQuantityInfos = PhysicalQuantityGetAll();
            Console.ReadKey();

            //取得單一物理量
            Console.WriteLine($"\r\n-----取得單一物理量 {physicalQuantityInfos.First().Id.ToString() }------");
            PhysicalQuantityGet(physicalQuantityInfos.First().Id);
            Console.ReadKey();

            //讀取全部物理量即時資料
            Console.WriteLine($"\r\n-----讀取全部物理量即時資料------");
            LastestDataReadAll();
            Console.ReadKey();

            //讀取單一物理量即時資料
            Console.WriteLine($"\r\n-----讀取單一物理量即時資料 {physicalQuantityInfos.First().Id.ToString()}------");
            LatestDataReadPhysicalQuantity(physicalQuantityInfos.First().Id);
            Console.ReadKey();

            Console.WriteLine($"\r\n-----讀取單一監測站下所有物理量資料 Station Id: {stationInfos.First().Id.ToString()}");
            LastestDataReadStation(stationInfos.First().Id);
            Console.ReadKey();

            Console.WriteLine($"\r\n-----讀取單一監測站群組下所有物理量資料 StationGroup Id: {stationGroupInfos.First().ChildStationGroups.First().Id.ToString()}");
            LastestDataReadStationGroup(stationGroupInfos.First().ChildStationGroups.First().Id);
            Console.ReadKey();

            Console.WriteLine($"\r\n-----讀取單一物理量之歷史原始資料-------");
            TimeSeriesDataReadRawData(physicalQuantityInfos.First().Id);
            Console.ReadKey();
 
            Console.WriteLine($"\r\n-----讀取單一物理量之歷史彙總資料-------");
            DataSeriesEs dataSeries = TimeSeriesDataReadAggregratedData(Guid.Parse("38a666f9-dcc4-4259-b454-9313f95a9d0c"));
            Console.ReadKey();

            #endregion

            #region 可視化 API 使用範例

            Console.WriteLine($"\r\n----產生可嵌入圖表--------");
            ChartGenerate(dataSeries);
            Console.ReadKey();

            #endregion
            
            #region 雲端運算 API 使用範例

            int jobId = 40;
            Console.WriteLine($"\r\n----從 S3 上傳 Java 檔案至雲端運算資源--------");
            //jobId = SparkJobSubmit();
            Console.ReadKey();
            
            Console.WriteLine($"\r\n----取得工作列表-------");
            SparkJobList();
            Console.ReadKey();

            Console.WriteLine("\r\n----取得工作狀態-----");
            SparkJobStatus(jobId);
            Console.ReadKey();

            Console.WriteLine("\r\n----取得工作狀態-----");
            SparkJobStatus(jobId);
            Console.ReadKey();
            
            Console.WriteLine("r\n----取得工作紀錄-----");
            SparkJobLog(35);
            Console.ReadKey();

            #endregion
    
            #region 地理圖資 API 

            Console.WriteLine($"\r\n----上傳檔案至地理圖資資料庫--------");
            RasterMapFileUpload();
            Console.ReadKey();

            Console.WriteLine($"\r\n----讀取地理圖資資料庫檔案列表--------");
            IEnumerable<FileDetails> fileDetails = RasterMapFileList();
            Console.ReadKey();

            Console.WriteLine($"\r\n----下載地理圖資檔案-------");
            RasterMapFileDownload(fileDetails.First().FileName);
            Console.ReadKey();

            Console.WriteLine($"\r\n----讀取地理圖資檔案 NetCDF ASCII--------");
            RasterMapNetCDFGet(fileDetails.First().FileName);
            Console.ReadKey();

            #endregion
        }

        #region 監測站群組

        private static IEnumerable<StationGroupInfo> StationGroupGetAll()
        {
            IEnumerable<StationGroupInfo> stationGroupInfos;
            if (_userApiClient.StationGroupGetAll(out stationGroupInfos, SenslinkInfoTypes.None) == System.Net.HttpStatusCode.OK)
            {
                foreach (StationGroupInfo cunStationGroupInfo in stationGroupInfos)
                {
                    Console.WriteLine($"監測站群組 {cunStationGroupInfo.Name} - {cunStationGroupInfo.Id.ToString()}");
                    


                }
            }
            return stationGroupInfos;
        }

        private static StationGroupInfo StationGroupGet(Guid id)
        {
            StationGroupInfo stationGroupInfo;
            if(_userApiClient.StationGroupGet(out stationGroupInfo, id, SenslinkInfoTypes.Station) == System.Net.HttpStatusCode.OK)
            {
                Console.Write($"監測站群組 {stationGroupInfo.Name}");
                return stationGroupInfo;
            }
            return null;
        }

        #endregion 

        #region 監測站

        private static IEnumerable<StationInfo> StationGetAll()
        {
            IEnumerable<StationInfo> stationInfos;
            if(_userApiClient.StationGetAll(out stationInfos, SenslinkInfoTypes.PhysicalQuantity) == System.Net.HttpStatusCode.OK)
            {
                foreach(StationInfo cunInfo in stationInfos)
                {
                    Console.WriteLine($"\r\n監測站 {cunInfo.Name}");

                    if (cunInfo.LinkedPhysicalQuantities != null)
                    {
                        foreach (PhysicalQuantityInfo cunPqInfo in cunInfo.LinkedPhysicalQuantities)
                        {
                            Console.WriteLine($"-{cunPqInfo.Name}");
                        }
                    }
                }
            }
            return stationInfos;
        }

        private static StationInfo StationGet(Guid id)
        {
            StationInfo stationInfo;
            if(_userApiClient.StationGet(out stationInfo, id, SenslinkInfoTypes.PhysicalQuantity) == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"監測站 {stationInfo.Name}");
                if (stationInfo.LinkedPhysicalQuantities != null)
                {
                    foreach (PhysicalQuantityInfo cunPqInfo in stationInfo.LinkedPhysicalQuantities)
                    {
                        Console.WriteLine($"-{cunPqInfo.Name}");
                    }
                }
            }
            return stationInfo;
        }

        #endregion

        #region 物理量

        /// <summary>
        /// 6.7.1
        /// </summary>
        /// <returns></returns>
        static public IEnumerable<PhysicalQuantityInfo> PhysicalQuantityGetAll()
        {
            IEnumerable<PhysicalQuantityInfo> physicalQuantityInfos;
            if (_userApiClient.PhysicalQuantityGetAll(out physicalQuantityInfos, SenslinkInfoTypes.None) == System.Net.HttpStatusCode.OK)
            {
                foreach(PhysicalQuantityInfo pqInfo in physicalQuantityInfos)
                {
                    Console.WriteLine($"物理量 {pqInfo.Name}");
                }
            }
            return physicalQuantityInfos;
        }

        static public PhysicalQuantityInfo PhysicalQuantityGet(Guid id)
        {
            PhysicalQuantityInfo pqInfo;
            if(_userApiClient.PhysicalQuantityGet(out pqInfo, id, SenslinkInfoTypes.None) == System.Net.HttpStatusCode.OK)
                Console.WriteLine($"物理量 {pqInfo.Name}");
            return pqInfo;
        }

        #endregion

        #region 即時資料讀取

        static public IEnumerable<DataPoint> LastestDataReadAll()
        {
            IEnumerable<DataPoint> dataPoints = null;
            if(_userApiClient.LatestDataReadAll(out dataPoints) == System.Net.HttpStatusCode.OK)
            {
                foreach(DataPoint cunDp in dataPoints)
                {
                    Console.WriteLine($"{cunDp.Id.ToString()} {cunDp.TimeStamp.ToString()} {cunDp.Value.ToString()}");
                }
            }
            return dataPoints;
        }

        static public DataPoint LatestDataReadPhysicalQuantity(Guid id)
        {
            DataPoint dataPoint;
            if(_userApiClient.LastestDataReadPhysicalQuantity(out dataPoint, id) == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"{id.ToString()} {dataPoint.TimeStamp.ToString()} {dataPoint.Value.ToString()}");
            }
            return dataPoint;
        }

        static public IEnumerable<DataPoint> LastestDataReadStation(Guid id)
        {
            IEnumerable<DataPoint> dataPoints;
            if(_userApiClient.LastestDataReadStation(out dataPoints, id) == System.Net.HttpStatusCode.OK)
            {
                foreach (DataPoint cunDp in dataPoints)
                {
                    Console.WriteLine($"{cunDp.Id.ToString()} {cunDp.TimeStamp.ToString()} {cunDp.Value.ToString()}");
                }
            }
            return dataPoints;
        }

        static public IEnumerable<DataPoint> LastestDataReadStationGroup(Guid id)
        {
            IEnumerable<DataPoint> dataPoints;
            if (_userApiClient.LatestDataReadStationGroup(out dataPoints, id) == System.Net.HttpStatusCode.OK)
            {
                foreach (DataPoint cunDp in dataPoints)
                {
                    Console.WriteLine($"{cunDp.Id.ToString()} {cunDp.TimeStamp.ToString()} {cunDp.Value.ToString()}");
                }
            }
            return dataPoints;
        }

        static public DataSeriesEs TimeSeriesDataReadAggregratedData(Guid id)
        {
            DataSeriesEs dataSeriesEs;
            DateTimeOffset eDt = DateTimeOffset.Now;
            DateTimeOffset sDt = eDt.AddDays(-7);
            if (_userApiClient.TimeSeriesDataReadAggregateData(out dataSeriesEs, id, sDt, eDt, AggregateCalculationMethods.avg, 600, 480) == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"{dataSeriesEs.Id.ToString()}");
                Console.WriteLine($"{dataSeriesEs.StartTimeStamp.ToString()}");
                
                foreach(DataPoint dp in dataSeriesEs.ToDataPointArray(true))
                {
                    Console.WriteLine($"{dp.TimeStamp.ToString()} {dp.Value.ToString()}");
                }
            }
            return dataSeriesEs;
        }

        static public DataSeriesUs TimeSeriesDataReadRawData(Guid id)
        {
            DataSeriesUs dataSeriesUs;
            DateTimeOffset eDt = DateTimeOffset.Now;
            DateTimeOffset sDt = eDt.AddDays(-7);
            if (_userApiClient.TimeSeriesDataReadRawData(out dataSeriesUs, id, sDt, eDt, true, 480) == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"{dataSeriesUs.Id.ToString()}");
                foreach (DataPoint dp in dataSeriesUs.DataPoints)
                {
                    Console.WriteLine($"{dp.TimeStamp.ToString()} {dp.Value.ToString()}");
                }
            }
            return dataSeriesUs;
        }

        #endregion

        #region 可視化

        static public void ChartGenerate(DataSeriesEs dataSeries)
        {
            string chartUrl = string.Empty;

            JObject data = new JObject();
            JArray dataSets = new JArray();
            JArray labels = new JArray();
            JArray dataPoints = new JArray();
            JObject dataSet = new JObject();

            DateTimeOffset cunTimeStamp = dataSeries.StartTimeStamp;
            int numOfValues = dataSeries.Values.Length;
            for(int index = 0; index < numOfValues; index++)
            {
                dataPoints.Add(dataSeries.Values[index]);
                labels.Add(cunTimeStamp.ToString("MM/dd HH:mm"));
                cunTimeStamp = cunTimeStamp.AddSeconds(dataSeries.TimeInterval);
            }
            dataSet.Add("label", "水位");
            dataSet.Add("backgroundColor", "rgba(0, 119, 204, 0.3)");
            dataSet.Add("data", dataPoints);
            dataSets.Add(dataSet);

            data.Add("labels", labels);
            data.Add("datasets", dataSets);

            JObject chartBody = new JObject();
            chartBody.Add("type", "line");
            chartBody.Add("data", data);

            string chartBodyStr = chartBody.ToString();
            _userApiClient.ChartGenerate(out chartUrl, ChartProvider.ChartJS, chartBodyStr);
            System.Diagnostics.Process.Start($"{chartUrl.ToString()}");
            Console.WriteLine($"{chartUrl.ToString()}");
        }

        #endregion

        #region 地理圖資

        static public void RasterMapFileUpload()
        {
            string fileName = "TainanFloodZone4_201609271100.nc";
            string filePath = Directory.GetCurrentDirectory();

            FileMetaData fmd = new FileMetaData()
            {
                name = "TainanFlood",
                description = "MyTest",
                shareLevel = "private",
            };

            if (_userApiClient.RasterMapFileUpload($"{filePath}\\{fileName}", true, fmd) == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"{fileName}圖資檔案上傳成功");
            }
        }
        
        static public IEnumerable<FileDetails> RasterMapFileList()
        {
            IEnumerable<FileDetails> fileDetails;
            if(_userApiClient.RasterMapFileList(out fileDetails) == System.Net.HttpStatusCode.OK)
            {
                foreach(FileDetails cunFd in fileDetails)
                {
                    Console.WriteLine($"{cunFd.FileName}");
                }
            }
            return fileDetails;
        }

        static public void RasterMapFileDownload(string fileName)
        {
            string filePath = Directory.GetCurrentDirectory();
            
            if(_userApiClient.RasterMapFileDownload(filePath, fileName) == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"{fileName}圖資檔案下載成功");
            }
        }

        static public void RasterMapNetCDFGet(string fileName)
        {
            string ASCIIContent = null;
            if( _userApiClient.RasterMapNetCDFGet("taoyuansystem", fileName, out ASCIIContent) == System.Net.HttpStatusCode.OK )
            {
                Console.WriteLine(ASCIIContent);
            }
        }

        #endregion

        #region Spark

        static public int SparkJobSubmit()
        {
            SubmitGeneralJobParams jobParams = new SubmitGeneralJobParams()
            {
                Spark = new SparkGeneral()
                {
                    MainFile = "com.anasystem.spark.AssociationrulesTrain",
                    Workers = new string[] { "10.57.234.249", "10.57.234.250", "10.57.234.251" },
                    Driver = "10.57.234.251",
                    Priority = "normal",
                    Arguments = "sCTphzCqB5wDxFSO6d1ELX9Md2X5fi+jC1sKtKt9cxpvmUAv1y0UCav5xYQdvaq1p/ZYE09NYaF8rBXIp3PxbXiirIeSzJZf05bNgde/sfWThv72d9Ena8cp7o3pgOQuUugWay/VkyV+AwA9XJsIThydzXBTldQE96svz7HolOFPmdNOWtKv/VVVUcUpmTmj7TAto1oQr42g7+dzV0Z8S+hTkdK07KZc1bnZRfZLjsx0RkGgnx2SEZ+Rs+SMadTR98MdKvdqs+dDN4IgRRk9vQ=="
                },
                S3 = new S3GerneralJob()
                {
                    InputPath = "AlgorithmsPool/spark-training-phase.jar",
                    AccessKey = "6MWMHIE0NMV060E59S3I",
                    SecretKey = "c95vzcMgLq33Bx2KfBWZZhyZkWYpwlgnMC3kWsy2",
                }
            };

            int jobId = 0;
            if( _userApiClient.SparkJobGeneralSubmit(jobParams, out jobId) == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"工作上傳成功，取得工作Id {jobId}");
            }
            return jobId;
        }

        static public void SparkJobList()
        {
            JobInfo[] jobInfos;
            if( _userApiClient.SparkJobList(out jobInfos) == System.Net.HttpStatusCode.OK )
            {
                foreach (JobInfo cunJob in jobInfos)
                {
                    Console.WriteLine($"{cunJob.submittime} {cunJob.jobname}");
                }
            }
        }

        static public void SparkJobStatus(int jobId)
        {
            JobInfo jobInfo;
            if (_userApiClient.SparkJobStatus(jobId, out jobInfo) == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"{jobInfo.submittime} {jobInfo.jobname}");
            }
        }

        static public void SparkJobLog(int jobId)
        {
            string responseString = null;
            if (_userApiClient.SparkJobLog(jobId, out responseString) == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(responseString);
            }
        }
        #endregion
    }
}
