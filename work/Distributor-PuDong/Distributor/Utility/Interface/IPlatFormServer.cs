using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interface
{
    public interface IPlatFormServer
    {
        IDevServer DevSvr { get; set; }

        void SendDataToPlatForm(SocketEventsArgs args);

        void BroadcastDataToPlatForm(SocketEventsArgs args);
    }
}
