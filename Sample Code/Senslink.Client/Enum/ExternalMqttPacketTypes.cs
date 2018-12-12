namespace Senslink.Client.Enum
{
    public enum ExternalMqttPacketTypes
    {
        /// <summary>
        /// 通訊方式非使用MQTT
        /// </summary>
        NoMqttPacketTypes = 0,

        /// <summary>
        /// 正文的Broker和AP
        /// </summary>
        GemTekPublic = 1,

        /// <summary>
        /// 安研的Broker、正文的AP(outdoor/indoor)
        /// </summary>
        GemTekPrivate = 2,

        /// <summary>
        /// 亞太的
        /// </summary>
        GtIoT = 3,

        /// <summary>
        /// 安研的Broker、正文的AP(indoor有特殊packet)
        /// </summary>
        GemTekPrivateIndoor = 4,

        IoTDataPoint = 5
    }
}
