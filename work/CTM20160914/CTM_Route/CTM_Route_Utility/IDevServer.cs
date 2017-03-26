using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTM_Route_Utility
{
    public interface IDevServer
    {
        IAnalyServer AnalySvr { get; set; }

        void SendMessageToDev(SocketEventsArgs args);

        void BroadcastMessageToDev(SocketEventsArgs args);
    }
}
