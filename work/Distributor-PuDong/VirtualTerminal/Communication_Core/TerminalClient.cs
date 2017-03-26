using Communication_Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Communication_Core
{
    public class TerminalClient
    {
        public TcpClient tcpClient;
        public NetworkStream netStream;
        public byte[] readBytes;
        public byte[] writeBytes;
        public bool Connected;

        public DateTime lastAliveTimeToPlatForm;//与平台通信时间
        public DateTime lastAliveTimeToTerminal;//与设备通信时间
        public DateTime loginTime;//设备登录时间
        

        public TerminalClient()
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("TerminalClient");
                    }))
                   .Do(() =>
                   {
                       ConnectPlatForm();
                       
                   });
        }

        public void ReConnectPlatForm()
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("ReConnectPlatForm");
                    }))
                .Do(() =>
                {
                    Close();
                    ConnectPlatForm();
                });
        }

        public void ConnectPlatForm()
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("ConnectPlatForm");
                    }))
                .Do(() =>
                {
                    Connected = false;
                    tcpClient = new TcpClient();
                    tcpClient.Connect(Share.Instance.PlatFormEndPoint);
                    netStream = tcpClient.GetStream();
                    Share.Instance.WriteMsg("连接平台成功！", 2);
                    readBytes = new byte[tcpClient.ReceiveBufferSize];
                    writeBytes = new byte[tcpClient.SendBufferSize];
                    Connected = true;
                });
        }

        public void Close()
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("Close");
                    }))
                   .Do(() =>
                   {
                       netStream.Close();
                       tcpClient.Close();
                       tcpClient.Client.Close();
                   });
        }
    }
}
