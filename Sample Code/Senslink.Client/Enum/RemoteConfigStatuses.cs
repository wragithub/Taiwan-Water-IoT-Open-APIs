using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Enum
{
    public enum RemoteConfigStatuses
    {
        Normal,
        RemoteDeviceConfigValueChanged, //資料庫中遠端設定參數改變，通常需要下達重新啟動命令
        RemoteDeviceDOSwitchChanged,    //設定遠端DO開關狀態
        ExecuteDataAddendum             //啟動資料補遺
    }
}
