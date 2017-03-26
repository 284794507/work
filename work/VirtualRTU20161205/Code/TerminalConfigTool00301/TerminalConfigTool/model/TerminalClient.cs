using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TerminalConfigTool.model;
using Utility;

namespace CommunicateCore.Model
{
    public class TerminalClient
    {
        public TcpClient CurTcpClient;

        public NetworkStream NetStream;

        public byte[] ReceviceBuffer;

        public EndPoint curEndPoint;

        public TerminalClient(TcpClient client):this()
        {
            this.CurTcpClient = client;
            this.NetStream = client.GetStream();
            ReceviceBuffer = new byte[client.ReceiveBufferSize];
            this.curEndPoint = client.Client.RemoteEndPoint;
        }

        /// <summary>
        /// 通信地址
        /// </summary>
        public string Addr { get; set; }

        public byte[] TerminalAddr { get; set; }

        /// <summary>
        /// 单灯所属虚拟RTU ID
        /// </summary>
        public string CtuID { get; set; }

        private DateTime heartBeatTime;

        public DateTime LoginTime { get; set; }

        public int LoginNum { get; set; }

        public int DevNum { get; set; }

        /// <summary>
        /// 设备列表
        /// </summary>
        public Dictionary<string, int> DictDev { get; set; }

        /// <summary>
        /// 心跳时间
        /// </summary>
        public DateTime HeartBeatTime
        {
            get
            {
                return heartBeatTime;
            }
            set
            {
                this.heartBeatTime = value;
                this.isHasHeartBeat = true;
            }
        }

        public bool isHasHeartBeat { get; set; }

        public TerminalClient()
        {
            this.DictDev = new Dictionary<string, int>();
            this.isHasHeartBeat = true;
        }

        public UpgradeInfo CurUpgradeInfo { get; set; }
        
        public void Close()
        {
            AspectF.Define.Retry()
                   .Do(() =>
                   {
                       NetStream.Close();
                       CurTcpClient.Close();
                       CurTcpClient.Client.Close();
                   });
        }
    }
}
