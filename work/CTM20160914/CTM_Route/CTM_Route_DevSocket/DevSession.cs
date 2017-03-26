using CTM_Route_Utility;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTM_Route_DevSocket
{
    public class DevSession:AppSession<DevSession,BinaryRequestInfo>
    {
        public string DeviceID { get; set; }

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnSessionStarted()
        {
            Route_Utility.Instance.WriteLog_Route("新设备连接成功：" + this.RemoteEndPoint.Address + " : " + this.RemoteEndPoint.Port,2);
            base.OnSessionStarted();
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            Route_Utility.Instance.WriteLog_Route("设备连接断开！",2);
            base.OnSessionClosed(reason);
        }

        protected override void HandleUnknownRequest(BinaryRequestInfo requestInfo)
        {
            Route_Utility.Instance.WriteLog_Route(string.Format("无效的通信请求，消息ID：{0}", requestInfo.Key),2);
            //base.HandleUnknownRequest(requestInfo);
            //this.Close();
        }

        public new DevServer AppServer
        {
            get
            {
                return (DevServer)base.AppServer;
            }
        }
    }
}
