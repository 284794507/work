using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRLamp.PackageHandler
{
    public class CmdWord
    {
        /// <summary>
        /// 终端通用应答new byte[] { 0x00, 0x00 };
        /// </summary>
        public static readonly byte[] Terminal_Response = new byte[] { 0x00, 0x00 };

        /// <summary>
        /// 主站能用应答 new byte[] { 0x80, 0x00 };
        /// </summary>
        public static readonly byte[] Master_Response = new byte[] { 0x80, 0x00 };

        /// <summary>
        /// 终端登录new byte[] { 0x00, 0x01 };
        /// </summary>
        public static readonly byte[] Terminal_Login = new byte[] { 0x00, 0x01 };

        /// <summary>
        /// 终端登录应答new byte[] { 0x80, 0x01 };
        /// </summary>
        public static readonly byte[] Terminal_LoginResponse = new byte[] { 0x80, 0x01 };

        /// <summary>
        /// 终端心跳new byte[] { 0x00, 0x02 };
        /// </summary>
        public static readonly byte[] Terminal_HeartBeat = new byte[] { 0x00, 0x02 };

        /// <summary>
        /// 单灯遥控操作new byte[] { 0xB1, 0x10 };
        /// </summary>
        public static readonly byte[] Lamp_RealCtrl = new byte[] { 0xB1, 0x10 };

        /// <summary>
        /// 设置光感开关阈值new byte[] { 0x82, 0x0a };
        /// </summary>
        public static readonly byte[] Set_LightCtrl = new byte[] { 0x82, 0x0a };

        /// <summary>
        /// 查询感开关阈值new byte[] { 0x81, 0x0a };
        /// </summary>
        public static readonly byte[] Query_LightCtrl = new byte[] { 0x81, 0x0a };

        /// <summary>
        /// 查询光感开关阈值返回new byte[] { 0x01, 0x0a };
        /// </summary>
        public static readonly byte[] Query_LightCtrlBack = new byte[] { 0x01, 0x0a };

        /// <summary>
        /// 设置单灯自动、手动模式new byte[] { 0xb1, 0x06 };
        /// </summary>
        public static readonly byte[] Set_CtrlMode = new byte[] { 0xb1, 0x06 };

        /// <summary>
        /// 设置日历时钟new byte[] { 0x82, 0x02 };
        /// </summary>
        public static readonly byte[] Set_Datetime = new byte[] { 0x82, 0x02 };
    }
}
