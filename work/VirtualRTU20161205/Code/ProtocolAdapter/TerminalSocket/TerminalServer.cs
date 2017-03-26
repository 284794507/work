using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Interface;

namespace TerminalSocket
{
    public class TerminalServer:AppServer<TerminalSession,BinaryRequestInfo>, ITerminalSvr
    {
        public IBrokerSvr BrokerSvr { get; set; }
        public TerminalServer()
            :base(new DefaultReceiveFilterFactory<TerminalReceiveFilter,BinaryRequestInfo>())
        {

        }

        protected override void OnStarted()
        {
            BrokerSvr = this.Bootstrap.GetServerByName("BrokerSocketServer") as IBrokerSvr;
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

        public void SendDataToTerminal(SocketEventsArgs args)
        {
            try
            {
                var sessions = GetAllSessions();
                if (sessions.Count() == 0) return;

                var session = sessions.Where(p => p.TerminalID == args.TerminalID).FirstOrDefault();
                if(session !=null)
                {
                    var str = ByteHelper.ByteToHexStrWithDelimiter(args.Buffer," ",false);
                    Logger.DebugFormat("SendDataToTerminal：{0}－{1}", args.TerminalID, str);
                    UtilityHelper.GetHelper.WriteLog_RTUSvr(DateTime.Now.ToString());
                    UtilityHelper.GetHelper.WriteLog_RTUSvr(string.Format("SendDataToTerminal：{0}－{1}", args.TerminalID, str));
                    session.Send(args.Buffer, 0, args.Buffer.Length);
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }            
        }

        public void BroadcastDataToTerminal(SocketEventsArgs args)
        {
            try
            {
                var sessions = GetAllSessions();
                var list = new List<TerminalSession>(sessions);
                this.AsyncRun(() =>
                {
                    list.ForEach(s => s.Send(args.Buffer, 0, args.Buffer.Length));
                }
                    );
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void SendBrokerSvr(SocketEventsArgs args)
        {
            try
            {
                BrokerSvr.SendDataToBroker(args);
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
