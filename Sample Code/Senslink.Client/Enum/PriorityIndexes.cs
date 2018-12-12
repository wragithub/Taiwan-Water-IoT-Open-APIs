using System;

namespace Senslink.Client.Enum
{
    /// <summary>
    /// 優先順序
    /// </summary>
    [Flags]
    public enum PriorityIndexes
    {
        /// <summary>
        /// Priority Low
        /// </summary>
        Low = 0,

        /// <summary>
        /// Priority medium
        /// </summary>
        Medium = 1,

        /// <summary>
        /// Priority High
        /// </summary>
        High = 2
    }
}
