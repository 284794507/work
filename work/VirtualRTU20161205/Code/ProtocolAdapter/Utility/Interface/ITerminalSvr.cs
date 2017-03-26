using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interface
{
    public interface ITerminalSvr
    {
        IBrokerSvr BrokerSvr { get; set; }

        void SendDataToTerminal(SocketEventsArgs args);

        void BroadcastDataToTerminal(SocketEventsArgs args);
    }
}
