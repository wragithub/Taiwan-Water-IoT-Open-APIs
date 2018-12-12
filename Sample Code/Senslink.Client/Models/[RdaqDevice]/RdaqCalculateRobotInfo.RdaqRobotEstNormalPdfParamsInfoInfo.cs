using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Models
{
    /// <summary>
    /// 常態分佈參數估計自動計算機器人設定參數
    /// </summary>
    public class RdaqRobotEstNormalPdfParamsInfo : RdaqCalculateRobotInfo
    {
        /// <summary>
        /// [必要] 資料取樣區間，單位為Sec
        /// </summary>
        public int SamplingInterval { get; set; }

        /// <summary>
        /// [必要] 以紀錄觸發時間點計算，往前推 SampleStart 倍的 SamplingInterval 時間區間，作為資料擷取起始點
        /// </summary>
        public int SampleStart { get; set; }

        /// <summary>
        /// [必要] 以紀錄觸發時間點計算，往前推 SampleEnd 倍的 SamplingInterval 時間區間，作為資料擷取結束點
        /// </summary>
        public int SampleEnd { get; set; }

        /// <summary>
        /// [必要] 來源資料(自變數)所連結的物理量
        /// </summary>
        public Guid SourcePhysicalQuantity { get; set; }
    }
}

