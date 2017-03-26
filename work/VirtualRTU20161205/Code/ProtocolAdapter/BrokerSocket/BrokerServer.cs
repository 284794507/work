using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Interface;

namespace BrokerSocket
{
    public class BrokerServer:AppServer<BrokerSession,BinaryRequestInfo>,IBrokerSvr
    {
        public ITerminalSvr TerminalSvr { get; set; }
        public BrokerServer()
            :base(new DefaultReceiveFilterFactory<BrokerReceiveFilter,BinaryRequestInfo>())
        {

        }

        protected override void OnStarted()
        {
            TerminalSvr = this.Bootstrap.GetServerByName("TerminalSocketServer") as ITerminalSvr;
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

        public void SendDataToBroker(SocketEventsArgs args)
        {
            AspectF.Define.Retry(UtilityHelper.GetHelper.CatchExpection)
                .Do(() => {
                    var sessions = GetAllSessions();
                    if (sessions.Count() == 0) return;

                    var list = new List<BrokerSession>(sessions);
                    this.AsyncRun(() =>
                    {
                        list.ForEach(s => s.Send(args.Buffer, 0, args.Buffer.Length));
                    });
                });
        }

        public void BroadcastDataToTerminal(SocketEventsArgs args)
        {
            AspectF.Define.Retry(UtilityHelper.GetHelper.CatchExpection)
                .Do(() =>
                {
                    if (string.IsNullOrEmpty(args.TerminalID))
                    {
                        TerminalSvr.BroadcastDataToTerminal(args);
                    }
                    else
                    {
                        TerminalSvr.SendDataToTerminal(args);
                    }
                });
        }
    }
}
