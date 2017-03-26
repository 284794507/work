using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace TerminalSocket
{
    public class TerminalSession:AppSession<TerminalSession,BinaryRequestInfo>
    {
        public string TerminalID { get; set; }

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnSessionStarted()
        {
            string msg = "New Terminal Connected: " + this.RemoteEndPoint.Address + ":" + this.RemoteEndPoint.Port;
            Logger.Info(msg);
            //UtilityHelper.GetHelper.WriteLog_RTUSvr(DateTime.Now.ToString());
            UtilityHelper.GetHelper.WriteLog_RTUSvr(msg);
            base.OnSessionStarted();
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            string msg = "Terminal DisConnected: " + this.RemoteEndPoint.Address + ":" + this.RemoteEndPoint.Port;
            Logger.Info(msg);
            //UtilityHelper.GetHelper.WriteLog_RTUSvr(DateTime.Now.ToString());
            UtilityHelper.GetHelper.WriteLog_RTUSvr(msg);
            
            base.OnSessionClosed(reason);
        }

        protected override void HandleUnknownRequest(BinaryRequestInfo requestInfo)
        {
            Logger.DebugFormat("无效的通信请求，消息ID：{0}", requestInfo.Key);
            base.HandleUnknownRequest(requestInfo);
        }

        public new TerminalServer AppServer
        {
            get
            {
                return (TerminalServer)base.AppServer;
            }
        }
    }
}
