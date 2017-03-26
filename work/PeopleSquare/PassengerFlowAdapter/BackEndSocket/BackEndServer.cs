using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Interface;

namespace BackEndSocket
{
    public class BackEndServer : AppServer<BackEndSession, BinaryRequestInfo>, IBackEndServer
    {
        public IDevServer DevSvr { get; set; }
        public BackEndServer()
            :base(new DefaultReceiveFilterFactory<BackEndReceiveFilter,BinaryRequestInfo>())
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

        public void SendDataToBackEnd(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    var sessions = GetAllSessions();
                    if (sessions.Count() == 0)
                    {
                        Share.Instance.WriteLog("No BackEnd is online！", 3);
                        return;
                    }

                    var session = sessions.Where(p => p.BackEndID == args.ObjID).FirstOrDefault();
                    if (session != null)
                    {
                        var str = ByteHelper.byteToHexStr(args.Buffer);
                        Share.Instance.WriteLog(string.Format("SendDataToBackEnd：{0}－{1}", args.ObjID, str), 2);
                        session.Send(args.Buffer, 0, args.Buffer.Length);
                    }
                    else
                    {
                        Share.Instance.WriteLog("BackEnd is offline！ID:" + args.ObjID, 3);
                    }
                });
        }

        public void BroadcastDataToBackEnd(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    var sessions = GetAllSessions();
                    if (sessions.Count() == 0)
                    {
                        Share.Instance.WriteLog("No BackEnd is online！", 3);
                        return;
                    }
                    Share.Instance.WriteLog("BroadcastDataToBackEnd:" + sessions.Count());
                    
                    var list = new List<BackEndSession>(sessions);

                    this.AsyncRun(() =>
                    {
                        list.ForEach(s => s.Send(args.Buffer, 0, args.Buffer.Length));
                    });
                });
        }

        public void BroadcastDataToDev(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    if (string.IsNullOrEmpty(args.ObjID))
                    {
                        DevSvr.BroadcastDataToDev(args);
                    }
                    else
                    {
                        DevSvr.SendDataToDev(args);
                    }
                });
        }
    }
}
