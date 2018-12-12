using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Models
{
    public class RdaqSenSmartWlsInfo : RdaqSensMateSeriesInfo
    {
        /// <summary>
        /// [必要] 離地高度 (零點校正值)
        /// </summary>
        /// <value>The zero point offset.</value>
        public int GroundClearance { get; set; }

        /// <summary>
        /// [必要] 警戒水位 (自動喚醒水位)(mm)
        /// </summary>
        public int AlertThreshold { get; set; }

    }
}
