using Communicate_Core.PackageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.Model
{
    public class SendToTerminalInfo
    {
        public Terminal_PackageData pkg { get; set; }

        /// <summary>
        /// 最多重试3次
        /// </summary>
        public int SendNum { get; set; }

        public DateTime SendTime { get; set; }

        /// <summary>
        /// 是否转发，转发10秒超时，不转发3秒超时
        /// </summary>
        public bool isRetransmission { get; set; }
    }
}
