using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senslink.Client.Enum;

namespace Senslink.Client.Models
{
    //2016/9/12 Review Abi, Hanny, Richard

    /// <summary>
    /// 自動計算機器人設定參數
    /// </summary>
    public abstract class RdaqCalculateRobotInfo : RdaqDeviceBaseInfo
    {
        /// <summary>
        /// [必要] 自動計算啟動時間區間 (sec)，必須是86400的因數
        /// </summary>
        public int RecordInterval { get; set; }

        /// <summary>
        /// [必要] 自動計算機器人型態
        /// </summary>
        public RdaqCalculateRobotTypes CalculateRobotType { get; set; }
    }
}
