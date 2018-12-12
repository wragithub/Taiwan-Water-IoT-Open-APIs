using System;
using System.Collections.Generic;
using Senslink.Client.Enum;


namespace Senslink.Client.Models
{
    /// <summary>
    /// 資料擷取設備共通參數
    /// </summary>
    public class RdaqDeviceBaseInfo : InfoBase
    {
        /// <summary>
        /// [必要] 傳輸紀錄器Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// [必要] 紀錄器名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// [非必要] 規格型號
        /// </summary>
        public string SpecInfo { get; set; }

        /// <summary>
        /// [非必要] 設備描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// [必要] 時區，以分鐘定義，必要(台灣480)
        /// </summary>
        public int DeviceTimeZone { get; set; }

        /// <summary>
        /// [非必要] 設定現場設備使用的時間標記，是使用當地時間還是UTC時間，local:0 或 utc:1 
        /// </summary>
        /// <value><c>true</c> if [adjust received data time zone]; otherwise, <c>false</c>.</value>
        public DeviceTimeStampTypes DeviceTimeStampType { get; set; }

        /// <summary>
        /// [非必要] 安裝日期
        /// </summary>
        public DateTimeOffset? InstallOn { get; set; }

        /// <summary>
        /// [非必要] 最近一次維修時間
        /// </summary>
        public DateTimeOffset? LastMaintenance { get; set; }

        /// <summary>
        /// [必要] 是否啟用
        /// </summary>
        public bool IsEnable { get; set; } 

        /// <summary>
        /// [必要] 紀錄器是否目前連線中
        /// </summary>
        public bool IsConnected { get; set; } 

        /// <summary>
        /// [必要] RdaqDevice型態，若不清楚型態，預設可使用 RdaqGeneric
        /// </summary>
        /// <value>The type of the rdaq device.</value>
        public RdaqDeviceTypes RdaqDeviceType { get; set; }

        /// <summary>
        /// [必要] 被哪一個 Transceiver 管理,一律給 Guid.Empty,由底層系統指定
        /// </summary>
        public Guid Transceiver_Id { get; set; }

        /// <summary>
        /// [非必要] 設定遠端設備DO狀態，選填
        /// </summary>
        public string DOSwitchStatus { get; set; }

        /// <summary>
        /// [非必要] 使用T/N表示 (T)Toggle 命令已經被下達，準備傳送 (N)已經傳送出去，恢復原狀
        /// </summary>
        public string DOToggleStatus { get; set; }

        /// <summary>
        /// [非必要] 遠端參數設定狀態
        /// </summary>
        public RemoteConfigStatuses RemoteConfigStatus { get; set; }

        public IEnumerable<TelecomBaseInfo> LinkedTelecomBaseInfos { get; set; }
    }
}
