﻿using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DevSocket
{
    public class DevSession:AppSession<DevSession,BinaryRequestInfo>
    {
        public string DevID { get; set; }

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnSessionStarted()
        {
            string msg = "New Dev Connected : " + this.RemoteEndPoint.Address + " - " + this.RemoteEndPoint.Port;
            Share.Instance.WriteLog(msg);
            base.OnSessionStarted();
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            string msg="Dev DisConnected : "+this.DevID+" - "+ this.RemoteEndPoint.Address + " - " + this.RemoteEndPoint.Port;
            Share.Instance.WriteLog(msg);
            this.Close(reason);
            this.SocketSession.Client.Close();
            base.OnSessionClosed(reason);
        }

        protected override void HandleUnknownRequest(BinaryRequestInfo requestInfo)
        {
            Share.Instance.WriteLog(string.Format("Invalid  Request, ID:{0}",requestInfo.Key));
            base.HandleUnknownRequest(requestInfo);
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
