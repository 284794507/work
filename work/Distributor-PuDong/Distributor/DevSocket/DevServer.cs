using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Interface;

namespace DevSocket
{
    public class DevServer:AppServer<DevSession,BinaryRequestInfo>,IDevServer
    {
        public IPlatFormServer PlatFormSvr { get; set; }
        public DevServer()
            :base(new DefaultReceiveFilterFactory<DevReceiveFilter,BinaryRequestInfo>())
        {

        }

        protected override void OnStarted()
        {
            PlatFormSvr = this.Bootstrap.GetServerByName("PlatFormSocketServer") as IPlatFormServer;
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

        public void SendDataToDev(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("SendDataToDev");
            }))
                .Do(() =>
                {
                    var sessions = GetAllSessions();
                    if (sessions.Count() == 0)
                    {
                        Share.Instance.WriteMsg("No Dev is online！", 3);
                        return;
                    }
                    Share.Instance.WriteMsg(sessions.Count().ToString(), 2);
                    var list = new List<DevSession>(sessions);
                    this.AsyncRun(() =>
                    {
                        list.ForEach(s =>
                        {
                            if(s.DevID==args.DeviceID)
                            {
                                var str = ByteHelper.byteToHexStr(args.Buffer);
                                Share.Instance.WriteMsg(string.Format("SendDataToDev：{0}－{1}", args.DeviceID, str), 2);
                                s.Send(args.Buffer, 0, args.Buffer.Length);
                            }
                        });
                    }
                    );

                    //var session = sessions.Where(p => p.DevID == args.DeviceID).FirstOrDefault();//.SingleOrDefault();
                    //if (session != null)
                    //{
                    //    var str = ByteHelper.byteToHexStr(args.Buffer);
                    //    Share.Instance.WriteMsg(string.Format("SendDataToDev：{0}－{1}", args.DeviceID, str), 2);
                    //    session.Send(args.Buffer, 0, args.Buffer.Length);
                    //}
                    //else
                    //{
                    //    Share.Instance.WriteMsg("Dev is offline！ID:" + args.DeviceID, 3);
                    //}
                });
        }

        public void BroadcastDataToDev(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("BroadcastDataToDev");
                }))
                .Do(() =>
                {
                    var sessions = GetAllSessions();
                    var list = new List<DevSession>(sessions);
                    this.AsyncRun(() =>
                    {
                        list.ForEach(s => s.Send(args.Buffer, 0, args.Buffer.Length));
                    }
                    );
                });
        }

        public void BroadcastPlatFormSvr(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("BroadcastPlatFormSvr");
            }))
                .Do(() =>
                {
                    PlatFormSvr.BroadcastDataToPlatForm(args);
                });
        }
    }
}
