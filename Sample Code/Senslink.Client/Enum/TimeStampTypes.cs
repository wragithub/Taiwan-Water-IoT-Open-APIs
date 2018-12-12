using System;

namespace Senslink.Client.Enum
{    
    [Flags]
    /// <summary>
    /// The time stamp type used by RdaqDevice logger. It will be transmitted to senslink colud
    /// </summary>
    public enum DeviceTimeStampTypes
    {
        /// <summary>
        /// The device use the local time as time stamp
        /// </summary>
        Local,

        /// <summary>
        /// The device use the UTC time as time stamp
        /// </summary>
        Utc
    }
}
