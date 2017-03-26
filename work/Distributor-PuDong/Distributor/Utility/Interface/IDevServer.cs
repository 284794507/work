using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interface
{
    public interface IDevServer
    {
        IPlatFormServer PlatFormSvr { get; set; }

        void SendDataToDev(SocketEventsArgs args);

        void BroadcastDataToDev(SocketEventsArgs args);
    }
}
