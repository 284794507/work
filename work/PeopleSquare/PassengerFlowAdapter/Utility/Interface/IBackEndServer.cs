using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interface
{
    public interface IBackEndServer
    {
        IDevServer DevSvr { get; set; }

        void SendDataToBackEnd(SocketEventsArgs args);

        void BroadcastDataToBackEnd(SocketEventsArgs args);
    }
}
