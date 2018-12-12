using System;

namespace Senslink.Client.Enum
{
    /// <summary>
    /// Calculate Robot Types
    /// </summary>
    [Flags]
    public enum RdaqCalculateRobotTypes
    {
        EstNormalPdfParams = 0,
        MultiIVariables = 1,
        SumAndIntegral = 2
    }
}
