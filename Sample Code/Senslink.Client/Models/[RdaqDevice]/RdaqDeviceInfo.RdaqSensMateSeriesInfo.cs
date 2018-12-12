using System;
using System.Runtime.Serialization;
using Senslink.Client.Enum;

namespace Senslink.Client.Models
{
    //Last Review 2016.9.7 Abi, Brian, Richard

    /// <summary>
    /// SensMateSeriesInfo 包含所有 SensMate 所有衍生產品的型號，例如SensMini, SensMini M4, SensMate, SensMateCE 等   
    /// 其參數共用，僅某些參數在某些型號不使用
    /// 使用此Info可產生的物件實體包括SensMiniTcpClient, SensMiniTcpServer, SensMiniGemTekMqttLoRaClient
    /// </summary>
    public class RdaqSensMateSeriesInfo : RdaqDeviceBaseInfo
    {
        /// <summary>
        /// [非必要] 如果是SensMiniTcpServer則為必要,是SensMini的Guid識別碼
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// [非必要] MACAddress,如果有需要LORA模組通訊則為必要
        /// </summary>
        public string LoRaMAC { get; set; }

        /// <summary>
        /// [非必要] 實體地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// [非必要] SIM Card Number，僅SensMini使用行動通訊時使用
        /// </summary>
        public string SimCardNumber { get; set; }

        /// <summary>
        /// [非必要] 行動電話號碼，僅SensMini使用行動通訊時使用
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// [必要] WGS84緯度
        /// </summary>
        public double LatitudeWGS84 { get; set; }

        /// <summary>
        /// [必要] WGS84經度
        /// </summary>
        public double LongitudeWGS84 { get; set; }

        /// <summary>
        /// [必要] 歷史數據同步間隔，Senslink端多久自動下達一次SensTalk命令取得歷史資料(Sec)，僅SensMiniTCPClient/Server物件使用，預設值：600
        /// </summary>
        public int HistoricalDataRetrieveInterval { get; set; }

        /// <summary>
        /// [必要] 即時數據同步間隔，Senslink端多久自動下達一次SensTalk命令取得即時資料(Sec)，僅SensMiniTCPClient/Server物件使用，預設值：300
        /// </summary>
        public int RealTimeDataRetrieveInterval { get; set; }

        /// <summary>
        /// [必要] Senslink下達自動資料補遺時間，例如午夜3點，單位(O'Clock)，僅SensMiniTCPClient/Server物件使用，預設值：3
        /// </summary>
        public int AutoDataAddendumCheckTriggerTime { get; set; }

        /// <summary>
        /// [必要] 資料補遺檢查最大時間長度，例如往前檢查七日是否有資料遺失，單位(Day)，僅SensMiniTCPClient/Server物件使用，預設值：7
        /// </summary>
        public int AutoDataAddendumCheckTimeSpan { get; set; }

        /// <summary>
        /// [必要] 發送自動時間校正至RdaqDevice時間間隔，單位(Sec)，僅SensMiniTCPClient/Server物件使用，預設值：21600
        /// </summary>
        public int AutoClockSyncInterval { get; set; }

        /// <summary>
        /// [必要] 傳輸接收是否啟用CRC檢查碼，預設值：True
        /// </summary>
        public bool IsCRCActive { get; set; }

        /// <summary>
        /// [非必要] 如果有啟用UDP連線，最後一次連線IP
        /// </summary>
        public string UdpLastConnectIP { get; set; }

        /// <summary>
        /// [非必要] 如果有啟用UDP連線，連線的遠端監聽Port
        /// </summary>
        public int? UdpLastConnectPort { get; set; }


        ////以下為遠端紀錄器參數

        /// <summary>
        /// [必要] 遠端紀錄器參數: 紀錄器是否是以ClientMode運作，即由現場發動Socket連線,若為True，則RdaqDevice實體為SensMiniTcpServer
        /// </summary>
        public bool IsRunningInClientMode { get; set; }

        /// <summary>
        /// [必要] 遠端紀錄器參數: SensMate設備內部資料紀錄時距(Sec)，不是Senslink端收到資料寫入資料庫的Interval，預設值：600
        /// </summary>
        public int RecordInterval { get; set; }

        /// <summary>
        /// [非必要] 遠端紀錄器參數: 是否開啟省電功能
        /// </summary>
        public bool? IsOpenPowerSaving { get; set; }

        /// <summary>
        /// [非必要] 遠端紀錄器參數: ex:senslink ,只有在Rdaq是Client主動連線時有意義
        /// </summary>
        public string APN { get; set; }

        /// <summary>
        /// [非必要] 遠端紀錄器參數: 有線or無線網路優先, 只有在Rdaq是Client主動連線時有意義
        /// </summary>
        public TelecomTypes? PriorityConnectType { get; set; }

        /// <summary>
        /// [非必要] 遠端紀錄器參數: Senslink server IP, 只有在Rdaq是Client主動連線時有意義
        /// </summary>
        public string ServerAddressOnClientMode { get; set; }

        /// <summary>
        /// [非必要] 遠端紀錄器參數: Senslink server Port, 只有在Rdaq是Client主動連線時有意義
        /// </summary>
        public int? ServerPortOnClientMode { get; set; }
    }
}
