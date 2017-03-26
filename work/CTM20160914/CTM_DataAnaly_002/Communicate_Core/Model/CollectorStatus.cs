using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.Model
{
    public enum CollectorStatus
    {
        /// <summary>
        /// 不在线
        /// </summary>
        OffLine = 0,

        /// <summary>
        /// 在线
        /// </summary>
        OnLine = 1,

        /// <summary>
        /// 断电
        /// </summary>
        PowerOff = 2,

        /// <summary>
        /// 通信中断
        /// </summary>
        CommunicationInterrupt = 3
    }
}
