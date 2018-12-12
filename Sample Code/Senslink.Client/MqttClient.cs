using System;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Senslink.Client.Mqtt
{
    /// <summary>
    /// MQTT client 連接水利署水資源物聯網 Mqtt Borker
    /// </summary>
    public class MqttClient
    {
        #region Data Structure

        public class ObservationsPacket
        {
            [JsonProperty(PropertyName = "phenomenonTime")]
            public string phenomenonTime { get; set; }

            [JsonProperty(PropertyName = "result")]
            public double result { get; set; }
        }

        public struct ConnectConfig
        {
            public IPAddress IpAddress;
            public int Port;
            public string UserName;
            public string Password;
        }

        #endregion

        #region Private Fields        

        /// <summary>
        /// The MQTT client which connects to our internal MQTT broker.
        /// </summary>
        private MQTTnet.Client.MqttClient _client = null;
        /// <summary>
        /// Queueing msg waiting for publishing
        /// </summary>
        private ConcurrentDictionary<ushort, DateTime> _publishedMessage = new ConcurrentDictionary<ushort, DateTime>();

        /// <summary>
        /// Monitor mqtt connection, manage connecting/disconnecting
        /// </summary>
        private Thread _mqttClientRunningThread;

        /// <summary>
        /// This MQTT client is closed by user or not. (Different from the connection status) 
        /// </summary>
        private bool _isRunningThread = false;

        /// <summary>
        /// MQTT client runtime connection status. More detail then only connected/disconnected
        /// </summary>
        private MqttBrokerConnectionStatus _mqttStatus = MqttBrokerConnectionStatus.DisConnected;

        /// <summary>
        /// Save data which fails to publish
        /// </summary>
        private ConcurrentQueue<MqttApplicationMessage> _waitForPublishMessages = new ConcurrentQueue<MqttApplicationMessage>();

        private ConnectConfig _config;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of client and serializer
        /// </summary>
        /// <param name="InternalClientInfo"></param>
        public MqttClient(ConnectConfig config)
        {
            _config = config;
            //建立MqttClient物件
            _client = new MqttFactory().CreateMqttClient() as MQTTnet.Client.MqttClient;
            _client.Disconnected += MqttClient_Disconnected;
            _mqttStatus = MqttBrokerConnectionStatus.DisConnected;

            _mqttClientRunningThread = new Thread(new ThreadStart(mqttClientRunningProcess));
        }

        #endregion

        #region Public Method

        public void Open()
        {
            if (!_isRunningThread)
                _mqttClientRunningThread.Start();
        }

        public void Close()
        {
            _isRunningThread = false;
            _client.DisconnectAsync();
            _client.Dispose();
        }

        public bool IsConnected
        {
            get
            {
                if (_mqttStatus == MqttBrokerConnectionStatus.Connected)
                    return true;
                else
                    return false;
            }
        }

        public void Publish(Guid dataStreamId, ObservationsPacket packet)
        {
            string topic = $"datastream({dataStreamId.ToString()})/Observation";
            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                            .WithTopic(topic)
                            .WithPayload(JsonConvert.SerializeObject(packet))
                            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                            .WithRetainFlag()
                            .Build();
            _waitForPublishMessages.Enqueue(message);
        }

        #endregion

        #region Private Method - Event Callback

        /// <summary>
        /// Create new connection
        /// </summary>
        private void mqttClientRunningProcess()
        {
            _isRunningThread = true;
            Task connectTask = null;
            Task publishTask = null;
            Task disConnectTask = null;

            int maxPublishFailTimes = 15;
            int maxReconnectTimes = 10;

            int numOfReconnectTimes = 0;
            int numOfPublishFailTimes = 0;

            //MQTT Connection Options
            var options = new MqttClientOptionsBuilder()
                .WithClientId($"{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}")
                .WithTcpServer(_config.IpAddress.ToString(), _config.Port)
                .WithCredentials(_config.UserName, _config.Password)
                .WithKeepAlivePeriod(new TimeSpan(200000000))
                .WithCommunicationTimeout(TimeSpan.FromSeconds(3000))
                .WithCleanSession()
                .WithProtocolVersion(MQTTnet.Serializer.MqttProtocolVersion.V311)
                .Build();

            MqttApplicationMessage[] publishingMessages = null;

            while (_isRunningThread)
            {
                Thread.Sleep(0);

                if (_client == null)
                    continue;

                switch (_mqttStatus)
                {
                    case MqttBrokerConnectionStatus.DisConnected:

                        #region DisConnected

                        if (numOfReconnectTimes++ > maxReconnectTimes)
                        {
                            numOfReconnectTimes = 0;
                            Thread.Sleep(10000);
                        }

                        // Create TCP based options using the builder.
                        _mqttStatus = MqttBrokerConnectionStatus.Connecting;
                        Console.WriteLine($"Start Connecting");

                        try
                        {
                            connectTask = _client.ConnectAsync(options);
                            connectTask.ConfigureAwait(false);
                        }
                        catch
                        {
                            _mqttStatus = MqttBrokerConnectionStatus.DisConnecting;
                        }
                        #endregion

                        break;

                    case MqttBrokerConnectionStatus.Connecting:

                        #region Connecting

                        switch (connectTask.Status)
                        {
                            //工作成功完成
                            case TaskStatus.RanToCompletion:
                                Console.WriteLine("Connected");
                                _mqttStatus = MqttBrokerConnectionStatus.Connected;
                                break;

                            //工作失敗
                            case TaskStatus.Faulted:
                                Console.WriteLine($"Connecting Fail.");
                                _mqttStatus = MqttBrokerConnectionStatus.DisConnecting;
                                break;

                            //工作取消
                            case TaskStatus.Canceled:
                                Console.WriteLine($"Connecting canceled");
                                _mqttStatus = MqttBrokerConnectionStatus.DisConnecting;
                                break;
                        }
                        break;

                    #endregion

                    //已經連線
                    case MqttBrokerConnectionStatus.Connected:

                        #region Connected

                        if (publishTask == null) //沒有 publishTask 在運行，發送新的Publish Task
                        {
                            if (!_client.IsConnected)
                            {
                                _mqttStatus = MqttBrokerConnectionStatus.DisConnecting;
                                continue;
                            }

                            if (_waitForPublishMessages.Any())
                            {
                                if (_waitForPublishMessages.Count() < 100)
                                    publishingMessages = _waitForPublishMessages.ToArray();
                                else
                                    publishingMessages = _waitForPublishMessages.Take(100).ToArray();

                                try
                                {
                                    publishTask = _client.PublishAsync(publishingMessages);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Publishing Fail.");
                                    _mqttStatus = MqttBrokerConnectionStatus.DisConnecting;
                                }
                            }
                            else
                                Thread.Sleep(1);
                        }
                        else
                        {
                            switch (publishTask.Status)
                            {
                                case TaskStatus.RanToCompletion:

                                    MqttApplicationMessage deQueueMsg;
                                    for (int index = 0; index < publishingMessages.Length; index++)
                                        _waitForPublishMessages.TryDequeue(out deQueueMsg);

                                    numOfPublishFailTimes = 0;
                                    //清空工作
                                    publishTask.Dispose();
                                    publishTask = null;
                                    break;

                                case TaskStatus.Faulted:

                                    numOfPublishFailTimes++;
                                    Console.WriteLine($"Publishing Fail.");

                                    if (publishTask.Exception != null)
                                    {
                                        _mqttStatus = MqttBrokerConnectionStatus.DisConnecting;
                                    }

                                    //清空工作
                                    publishTask = null;

                                    if (numOfPublishFailTimes > maxPublishFailTimes)
                                    {
                                        numOfPublishFailTimes = 0;
                                        _mqttStatus = MqttBrokerConnectionStatus.DisConnecting;
                                    }
                                    break;

                                case TaskStatus.Canceled:

                                    numOfPublishFailTimes++;
                                    //清空工作
                                    publishTask = null;

                                    if (numOfPublishFailTimes > maxPublishFailTimes)
                                    {
                                        numOfPublishFailTimes = 0;
                                        _mqttStatus = MqttBrokerConnectionStatus.DisConnecting;
                                    }
                                    break;
                            }
                        }
                        break;

                    #endregion

                    case MqttBrokerConnectionStatus.DisConnecting:

                        if (disConnectTask == null)
                        {
                            Console.WriteLine($"DisConnecting.");
                            try
                            {
                                disConnectTask = _client.DisconnectAsync();
                            }
                            catch
                            {
                                Console.WriteLine($"DisConnected.");
                                disConnectTask = null;
                                _mqttStatus = MqttBrokerConnectionStatus.DisConnected;
                            }
                        }
                        else
                        {
                            if (disConnectTask.IsCompleted)
                            {
                                Console.WriteLine($"DisConnected.");
                                disConnectTask = null;
                                _mqttStatus = MqttBrokerConnectionStatus.DisConnected;
                            }
                        }
                        break;
                }
            }
        }

        private void MqttClient_Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected");
        }
        #endregion
    }
}
