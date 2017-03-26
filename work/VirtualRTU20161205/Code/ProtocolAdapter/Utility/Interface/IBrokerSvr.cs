using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interface
{
    public interface IBrokerSvr
    {
        ITerminalSvr TerminalSvr { get; set; }

        void SendDataToBroker(SocketEventsArgs args);
    }
}
