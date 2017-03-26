using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Model
{
    public class TerminalClient
    {
        /// <summary>
        /// 通信地址
        /// </summary>
        private string Addr { get; set; }

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
    }
}
