using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interface
{
    public interface IDevServer
    {
        IBackEndServer BackEndSvr { get; set; }

        void SendDataToDev(SocketEventsArgs args);

        void BroadcastDataToDev(SocketEventsArgs args);
    }
}
