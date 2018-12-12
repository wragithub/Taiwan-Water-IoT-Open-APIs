using System;

namespace Senslink.Client.Enum
{
    /// <summary>
    /// 時間序列趨勢圖型態
    /// </summary>
    [Flags]
    public enum TimeSeriesChartTypes
    {
        /// <summary>
        /// SeriesChartTypeLine
        /// </summary>
        Line = 1,

        /// <summary>
        /// SeriesChartTypeColumn
        /// </summary>
        Bar = 3
    }
}
