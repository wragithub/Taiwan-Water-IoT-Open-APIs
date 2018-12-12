using System;

namespace Senslink.Client.Enum
{
    //注意，此處 ChannelType 的型態名稱，必須與 {Senlink.LogicLayer.RdaqDevice} 中的可實體化物件名稱一致

    /// <summary>
    /// RdaqDevice型態，型態中所描述的Client/Server，是以Senslink為觀點，而非遠端設備
    /// </summary>
    [Flags]
    public enum RdaqDeviceTypes
    {
        None = 0,

        /// <summary>
        /// 遠端設備為 SensMini 電路版，包括SensMini M4 或 四合一水位計，
        /// <para>使用有線或3G/4G以Ethernet TCP為基礎的通訊方式，應用層協定為SensTalkFull</para>
        /// </summary>
        RdaqSensMini = 1,

        /// <summary>
        /// [不再使用] 遠端設備為 SensMini 電路版，包括SensMini M4 或 四合一水位計，
        /// <para>搭配正文(GemTek)公網LoRa通訊，透過正文Mqtt Gateway接收資料的客戶端，</para>
        /// <para>應用層協定為SensTalkMicro </para>
        /// </summary>
        RdaqSensMiniMqttLoRa = 2,

        /// <summary>
        /// 遠端設備為 OpcUaServer
        /// </summary>
        RdaqOpcUaClient = 4,

        /// <summary>
        /// 自動計算機器人 - 估計常態分佈參數
        /// </summary>
        RdaqRobotEstNormalPdfParams = 8,
        
        /// <summary>
        /// 自動計算機器人 - 多參數運算
        /// </summary>
        RdaqRobotMultiIVariables = 16,
        
        /// <summary>
        /// 自動計算機器人 - 累計與積分
        /// </summary>
        RdaqRobotSumAndIntegral = 32,
        
        RdaqModbusMaster = 64,

        /// <summary>
        /// 四合一水位計TCP
        /// </summary>
        RdaqSenSmartWls = 128,

        /// <summary>
        /// SensMateCe
        /// </summary>
        RdaqSensMateCe = 256,

        RdaqGeneric = 512
    }

}
