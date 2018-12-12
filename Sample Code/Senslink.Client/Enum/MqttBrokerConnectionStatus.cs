using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senslink.Client.Mqtt
{
    public enum MqttBrokerConnectionStatus
    {
        DisConnected = 0,
        Connecting = 1,
        Connected = 2,
        Closed = 3,
        WaitToClose = 4,
        DisConnecting = 6
    }
}
