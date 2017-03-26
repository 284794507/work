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
        public IBackEndServer BackEndSvr { get; set; }
        public DevServer():
            base(new DefaultReceiveFilterFactory<DevReceiveFilter,BinaryRequestInfo>())
        {

        }

        protected override void OnStarted()
        {
            BackEndSvr = this.Bootstrap.GetServerByName("BackEndSocketServer") as IBackEndServer;
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

        public void SendDataToDev(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    var sessions = GetAllSessions();
                    if(sessions.Count()==0)
                    {
                        Share.Instance.WriteLog("No Dev is online!",3);
                        return;
                    }
                    var list = new List<DevSession>(sessions);
                    this.AsyncRun(() =>
                    {
                        list.ForEach(s =>
                        {
                            if(s.DevID==args.ObjID)
                            {
                                var str = ByteHelper.ByteToHexStrWithDelimiter(args.Buffer, " ", false);
                                Share.Instance.WriteLog(string.Format("SendDataToDev:{0}", args.ObjID));
                                s.Send(args.Buffer, 0, args.Buffer.Length);
                            }
                        });
                    });
                });
        }

        public void BroadcastDataToDev(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    var sessions = GetAllSessions();
                    int count = sessions.Count();
                    if (count > 0)
                    {
                        var str = ByteHelper.ByteToHexStrWithDelimiter(args.Buffer, " ", false);
                        Share.Instance.WriteLog(string.Format("BroadcastDataToDev : "+ count));
                    }
                    else
                    {
                        return;
                    }
                    var list = new List<DevSession>(sessions);
                    this.AsyncRun(() =>
                    {
                        list.ForEach(s =>
                        {
                            s.Send(args.Buffer, 0, args.Buffer.Length);
                        });
                    });
                });
        }

        public void BroadcastBackEndSvr(SocketEventsArgs args)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    BackEndSvr.BroadcastDataToBackEnd(args);
                });
        }
    }
}
