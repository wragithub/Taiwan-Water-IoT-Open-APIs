using System;

namespace Senslink.Client.Enum
{
    [Flags]
    public enum TelecomTypes
    {
        /// <summary>
        /// 有線
        /// </summary>
        Wired = 0,

        /// <summary>
        /// 無線
        /// </summary>
        Wireless = 1
    }
}
