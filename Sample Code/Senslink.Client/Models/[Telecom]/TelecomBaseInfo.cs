using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Senslink.Client.Enum;

namespace Senslink.Client.Models
{
    //使用於 Mqtt, Tcp client, tcp server 與 Udp 連線
  
    public class TelecomBaseInfo : InfoBase
    {
        /// <summary>
        /// [必要] 連線方法Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// [必要] IPv4地址,沒有的話請設0.0.0.0
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// [必要] Port 通訊埠,沒有的話請設0或80
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// [必要] 連線優先順序，例如，有兩個可用連線，數字較小者優先使用 Enum PriorityIndexes
        /// </summary>
        public PriorityIndexes PriorityIndex { get; set; }

        /// <summary>
        /// [必要] 此連線方式的RdaqDeviceBase_Id
        /// </summary>
        public Guid RdaqDevice_Id { get; set; }

        /// <summary>
        /// [非必要] The MQTT topic, only for Mqtt
        /// </summary>
        public string MqttTopic { get; set; }

        /// <summary>
        /// [非必要] UserName For MQTT or OPCUAServer
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// [非必要] Password For MQTT or OPCUAServer
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// [必要] 預設值是 NoMqttPacketTypes = 0
        /// </summary>
        public ExternalMqttPacketTypes PacketType { get; set; }

        /// <summary>
        /// [必要] 啟用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// [必要] 連線方式
        /// </summary>
        public TransportLayerTypes TransportLayerType { get; set; }


    }
}
