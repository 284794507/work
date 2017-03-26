using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace BrokerSocket
{
    public class BrokerSession:AppSession<BrokerSession,BinaryRequestInfo>
    {
        public string BrokerID { get; set; }

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnSessionStarted()
        {
            string msg = "New Broker Connected: " + this.RemoteEndPoint.Address + ":" + this.RemoteEndPoint.Port;
            UtilityHelper.GetHelper.WriterLog(msg);
            //Logger.Info(msg);
            //Console.WriteLine(DateTime.Now.ToString());
            //Console.WriteLine(msg);
            base.OnSessionStarted();
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            string msg = "Broker DisConnected: " + this.RemoteEndPoint.Address + ":" + this.RemoteEndPoint.Port;
            UtilityHelper.GetHelper.WriterLog(msg);
            //Logger.Info(msg);
            //Console.WriteLine(DateTime.Now.ToString());
            //Console.WriteLine(msg);
            base.OnSessionClosed(reason);
        }

        protected override void HandleUnknownRequest(BinaryRequestInfo requestInfo)
        {
            Logger.DebugFormat("无效的通信请求，消息ID：{0}", requestInfo.Key);
            base.HandleUnknownRequest(requestInfo);
        }

        public new BrokerServer AppServer
        {
            get
            {
                return (BrokerServer)base.AppServer;
            }
        }
    }
}
