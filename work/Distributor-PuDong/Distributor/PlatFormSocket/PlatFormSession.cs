using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace PlatFormSocket
{
    public class PlatFormSession:AppSession<PlatFormSession, BinaryRequestInfo>
    {
        public string PlatFormID { get; set; }

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnSessionStarted()
        {
            string msg = "New PlatFormServer Connected "+this.RemoteEndPoint.Address+":"+this.RemoteEndPoint.Port;
            Share.Instance.WriteMsg(msg, 2);
            this.PlatFormID = this.RemoteEndPoint.Port.ToString();
            base.OnSessionStarted();
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            string msg = "PlatFormServer DisConnected " + this.RemoteEndPoint.Address + ":" + this.RemoteEndPoint.Port;
            Share.Instance.WriteMsg(msg, 2);
            base.OnSessionClosed(reason);
        }

        protected override void HandleUnknownRequest(BinaryRequestInfo requestInfo)
        {
            Share.Instance.WriteMsg(string.Format("Invalid Request，ID：{0}", requestInfo.Key),2);
            base.HandleUnknownRequest(requestInfo);
        }

        public new PlatFormServer AppServer
        {
            get
            {
                return (PlatFormServer)base.AppServer;
            }
        }
    }
}
