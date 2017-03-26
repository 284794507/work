using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Interface;

namespace PlatFormSocket
{
    public class PlatFormServer:AppServer<PlatFormSession, BinaryRequestInfo>,IPlatFormServer
    {
        public IDevServer DevSvr { get; set; }
        public PlatFormServer ()
            :base(new DefaultReceiveFilterFactory<PlatFormReceiveFilter,BinaryRequestInfo>())
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

        public void SendDataToPlatForm(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("SendDataToPlatForm");
            }))
                .Do(() =>
                {
                    var sessions = GetAllSessions();
                    if(sessions.Count()==0)
                    {
                        Share.Instance.WriteMsg("No PlatForm is online！", 3);
                        return;
                    }

                    var session = sessions.Where(p => p.PlatFormID == args.DeviceID).FirstOrDefault();//.SingleOrDefault();
                    if (session != null)
                    {
                        var str = ByteHelper.byteToHexStr(args.Buffer);
                        Share.Instance.WriteMsg(string.Format("SendDataToPlatForm：{0}－{1}", args.DeviceID, str), 2);
                        session.Send(args.Buffer, 0, args.Buffer.Length);
                    }
                    else
                    {
                        Share.Instance.WriteMsg("PlatForm is offline！ID:" + args.DeviceID, 3);
                    }
                });
        }

        public void BroadcastDataToPlatForm(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("BroadcastDataToPlatForm");
            }))
                .Do(() =>
                {
                    var sessions = GetAllSessions();
                    if (sessions.Count() == 0)
                    {
                        Share.Instance.WriteMsg("No PlatForm is online！", 3);
                        return;
                    }
                    Share.Instance.LogInfo("BroadcastDataToPlatForm:"+ sessions.Count());
                    Share.Instance.LogInfo(ByteHelper.byteToHexStr(args.Buffer));
                    var list = new List<PlatFormSession>(sessions);
                    
                    this.AsyncRun(() =>
                    {
                        list.ForEach(s => s.Send(args.Buffer, 0, args.Buffer.Length));
                    });
                });
        }

        public void BroadcastDataToDev(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("BroadcastDataToDev");
                }))
                .Do(() =>
                {
                    Share.Instance.LogInfo(ByteHelper.byteToHexStr(args.Buffer));
                    if (string.IsNullOrEmpty(args.DeviceID))
                    {
                        DevSvr.BroadcastDataToDev(args);
                    }
                    else
                    {
                        Share.Instance.LogInfo(args.DeviceID);
                        DevSvr.SendDataToDev(args);
                    }
                });
                
        }
    }
}
