using CTM_Route_Utility;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace CTM_Route_AnalySocket
{
    public class AnalyServer :AppServer<AnalySession,BinaryRequestInfo>,IAnalyServer
    {
        public IDevServer DevSvr { get; set; }
        public AnalyServer()
            :base(new DefaultReceiveFilterFactory<AnalyReceiveFilter,BinaryRequestInfo>())
        {

        }

        protected override void OnStarted()
        {
            DevSvr = this.Bootstrap.GetServerByName("DevSocketServer") as IDevServer;
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

        public void SendMessageToAnaly(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Route_Utility.Instance.CatchExpection)
                   .Do(() =>
                   {
                       var sessions = GetAllSessions();
                       if (sessions.Count() == 0) return;

                       var list = new List<AnalySession>(sessions);
                       this.AsyncRun(() =>
                       {
                           list.ForEach(s => s.Send(args.Buffer, 0, args.Buffer.Length));
                       });
                   });
        }

        public void BroadcastMessageToDev(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Route_Utility.Instance.CatchExpection)
                   .Do(() =>
                   {
                       if (string.IsNullOrEmpty(args.DeviceID))
                       {
                           DevSvr.BroadcastMessageToDev(args);
                       }
                       else
                       {
                           DevSvr.SendMessageToDev(args);
                       }
                   });
        }
    }
}
