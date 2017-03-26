using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTM_Route_Utility
{
    public interface IAnalyServer
    {
        IDevServer DevSvr { get; set; }

        void SendMessageToAnaly(SocketEventsArgs args);
    }
}
