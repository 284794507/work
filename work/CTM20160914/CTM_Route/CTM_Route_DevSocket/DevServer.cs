using CTM_Route_Utility;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace CTM_Route_DevSocket
{
    public class DevServer:AppServer<DevSession,BinaryRequestInfo>,IDevServer
    {
        public IAnalyServer AnalySvr { get; set; }
        public DevServer()
            :base(new DefaultReceiveFilterFactory<DevReceiveFilter,BinaryRequestInfo>())
        {

        }

        protected override void OnStarted()
        {
            AnalySvr = this.Bootstrap.GetServerByName("AnalySocketServer") as IAnalyServer;
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

        public void SendMessageToDev(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Route_Utility.Instance.CatchExpection)
                      .Do(() =>
                      {
                          var sessions = GetAllSessions();
                          if (sessions.Count() == 0) return;

                          var session = sessions.Where(p => p.DeviceID == args.DeviceID).FirstOrDefault();//.SingleOrDefault();
                          if (session != null)
                          {
                              var str = ByteHelper.byteToHexStr(args.Buffer);
                              Route_Utility.Instance.WriteLog_Route(string.Format("发送消息给设备：{0}－{1}", args.DeviceID, str),2);
                              session.Send(args.Buffer, 0, args.Buffer.Length);
                          }
                      });
        }

        public void BroadcastMessageToDev(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Route_Utility.Instance.CatchExpection)
                      .Do(() =>
                      {
                          var sessions = GetAllSessions();
                          if (sessions.Count() == 0) return;

                          var list = new List<DevSession>(sessions);
                          this.AsyncRun(() =>
                          {
                              list.ForEach(s => s.Send(args.Buffer, 0, args.Buffer.Length));
                          });
                      });
        }

        public void SendAnalySvr(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Route_Utility.Instance.CatchExpection)
                      .Do(() =>
                      {
                          AnalySvr.SendMessageToAnaly(args);
                      });
        }
    }
}
