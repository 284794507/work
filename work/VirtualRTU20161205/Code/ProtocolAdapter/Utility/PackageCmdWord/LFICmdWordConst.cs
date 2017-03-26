using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.PackageCmdWord
{
    public class LFICmdWordConst
    {
        /// <summary>
        /// 登录指令
        /// </summary>
        public static readonly byte[] Login = new Byte[] { 0xC0, 0x01 };

        /// <summary>
        /// 登录回复指令
        /// </summary>
        public static readonly byte[] LoginBack = new Byte[] { 0x40, 0x01 };

        /// <summary>
        /// 心跳指令
        /// </summary>
        public static readonly byte[] HeartBeat = new Byte[] { 0xC0, 0x02 };

        /// <summary>
        /// 心跳回复指令
        /// </summary>
        public static readonly byte[] HeartBeatBack = new Byte[] { 0x40, 0x02 };

        /// <summary>
        /// 报警
        /// </summary>
        public static readonly byte[] Alarm = new byte[] { 0xC0, 0x05 };

        /// <summary>
        /// 报警回复
        /// </summary>
        public static readonly byte[] AlarmBack = new Byte[] { 0x40, 0x05 };

        /// <summary>
        /// 查询终端信息
        /// </summary>
        public static readonly byte[] QueryTerminalInfo = new byte[] { 0x40, 0x20 };

        /// <summary>
        /// 查询终端信息回复
        /// </summary>
        public static readonly byte[] QueryTerminalInfoBack = new Byte[] { 0xC0, 0x20 };

        /// <summary>
        /// 重启指令
        /// </summary>
        public static readonly byte[] Reset = new Byte[] { 0x40, 0x21 };

        /// <summary>
        /// 重启回复指令
        /// </summary>
        public static readonly byte[] ResetBack = new Byte[] { 0xC0, 0x21 };

        /// <summary>
        /// 实时控制指令
        /// </summary>
        public static readonly byte[] RealTimeControl = new Byte[] { 0x40, 0x30 };

        /// <summary>
        /// 实时控制回复指令
        /// </summary>
        public static readonly byte[] RealTimeControlBack = new Byte[] { 0xC0, 0x30 };

        /// <summary>
        /// 查询电参数
        /// </summary>
        public static readonly byte[] QueryElecData = new byte[] { 0x40, 0x60 };

        /// <summary>
        /// 查询电参数回复
        /// </summary>
        public static readonly byte[] QueryElecDataBack = new Byte[] { 0xC0, 0x60 };

        /// <summary>
        /// 查询其它参数
        /// </summary>
        public static readonly byte[] QueryOtherPara = new byte[] { 0x40, 0x61 };

        /// <summary>
        /// 查询其它参数回复
        /// </summary>
        public static readonly byte[] QueryOtherParaBack = new Byte[] { 0xC0, 0x61 };

        /// <summary>
        /// 设置时间
        /// </summary>
        public static readonly byte[] SetTime = new byte[] { 0x41, 0x00 };

        /// <summary>
        /// 设置时间回复
        /// </summary>
        public static readonly byte[] SetTimeBack = new Byte[] { 0xC1, 0x00 };

        /// <summary>
        /// 设置基本参数
        /// </summary>
        public static readonly byte[] SetBasicPara = new Byte[] { 0x41, 0x01 };

        /// <summary>
        /// 设置基本参数回复指令
        /// </summary>
        public static readonly byte[] SetBasicParaBack = new Byte[] { 0xC1, 0x01 };

        /// <summary>
        /// 设置通信参数指令
        /// </summary>
        public static readonly byte[] SetCommPara = new Byte[] { 0x41, 0x02 };

        /// <summary>
        /// 设置通信参数回复指令
        /// </summary>
        public static readonly byte[] SetCommParaBack = new Byte[] { 0xC1, 0x02 };

        /// <summary>
        /// 设置网络参数
        /// </summary>
        public static readonly byte[] SetNetWorkPara = new byte[] { 0x41, 0x03 };

        /// <summary>
        /// 设置网络参数回复
        /// </summary>
        public static readonly byte[] SetNetWorkParaBack = new Byte[] { 0xC1, 0x03 };

        /// <summary>
        /// 设置灯参数
        /// </summary>
        public static readonly byte[] SetLampPara = new byte[] { 0x41, 0x04 };

        /// <summary>
        /// 设置灯参数回复
        /// </summary>
        public static readonly byte[] SetLampParaBack = new Byte[] { 0xC1, 0x04 };

        /// <summary>
        /// 设备报警阀值参数
        /// </summary>
        public static readonly byte[] SetAlarmLimit = new byte[] { 0x41, 0x05 };

        /// <summary>
        /// 设备报警阀值参数回复
        /// </summary>
        public static readonly byte[] SetAlarmLimitBack = new Byte[] { 0xC1, 0x05 };

        /// <summary>
        /// 设置经纬度开关灯时间指令
        /// </summary>
        public static readonly byte[] SetYearTable = new Byte[] { 0x41, 0x06 };

        /// <summary>
        /// 设置经纬度开关灯时间指令
        /// </summary>
        public static readonly byte[] SetYearTableBack = new Byte[] { 0xC1, 0x06 };

        /// <summary>
        /// 设置预约
        /// </summary>
        public static readonly byte[] SetPlan = new Byte[] { 0x41, 0x07 };

        /// <summary>
        /// 设置预约回复指令
        /// </summary>
        public static readonly byte[] SetPlanBack = new Byte[] { 0xC1, 0x07 };

        /// <summary>
        /// 设置自主运行
        /// </summary>
        public static readonly byte[] SetSefRun = new byte[] { 0x41, 0x08 };

        /// <summary>
        /// 设置自主运行回复
        /// </summary>
        public static readonly byte[] SetSefRunBack = new Byte[] { 0xC1, 0x08 };

        /// <summary>
        /// 设置光照度开关阀值
        /// </summary>
        public static readonly byte[] SetLightInfo = new byte[] { 0x41, 0x09 };

        /// <summary>
        /// 设置光照度开关阀值回复
        /// </summary>
        public static readonly byte[] SetLightInfoBack = new Byte[] { 0xC1, 0x09 };

        /// <summary>
        /// 设置漏电阀值操作参数
        /// </summary>
        public static readonly byte[] SetLeakCurrentLimit = new byte[] { 0x41, 0x0A };

        /// <summary>
        /// 设置漏电阀值操作参数回复
        /// </summary>
        public static readonly byte[] SetLeakCurrentLimitBack = new Byte[] { 0xC1, 0x0A };

        /// <summary>
        /// 设置网口模块参数
        /// </summary>
        public static readonly byte[] SetEthernetInterface = new byte[] { 0x41, 0x0B };

        /// <summary>
        /// 设置网口模块参数回复
        /// </summary>
        public static readonly byte[] SetEthernetInterfaceBack = new Byte[] { 0xC1, 0x0B };

        /// <summary>
        /// 查询时间
        /// </summary>
        public static readonly byte[] QueryTime = new Byte[] { 0x42, 0x00 };

        /// <summary>
        /// 查询时间回复指令
        /// </summary>
        public static readonly byte[] QueryTimeBack = new Byte[] { 0xC2, 0x00 };

        /// <summary>
        /// 查询基本参数
        /// </summary>
        public static readonly byte[] QueryBasicPara = new Byte[] { 0x42, 0x01 };

        /// <summary>
        ///查询基本参数回复指令
        /// </summary>
        public static readonly byte[] QueryBasicParaBack = new Byte[] { 0xC2, 0x01 };

        /// <summary>
        /// 查询通信参数指令
        /// </summary>
        public static readonly byte[] QueryCommPara = new Byte[] { 0x42, 0x02 };

        /// <summary>
        /// 查询通信参数回复指令
        /// </summary>
        public static readonly byte[] QueryCommParaBack = new Byte[] { 0xC2, 0x02 };

        /// <summary>
        /// 查询网络参数
        /// </summary>
        public static readonly byte[] QueryNetWorkPara = new byte[] { 0x42, 0x03 };

        /// <summary>
        /// 查询网络参数回复
        /// </summary>
        public static readonly byte[] QueryNetWorkParaBack = new Byte[] { 0xC2, 0x03 };

        /// <summary>
        /// 查询灯参数
        /// </summary>
        public static readonly byte[] QueryLampPara = new byte[] { 0x42, 0x04 };

        /// <summary>
        /// 查询灯参数回复
        /// </summary>
        public static readonly byte[] QueryLampParaBack = new Byte[] { 0xC2, 0x04 };

        /// <summary>
        /// 查询报警阀值参数
        /// </summary>
        public static readonly byte[] QueryAlarmLimit = new byte[] { 0x42, 0x05 };

        /// <summary>
        /// 查询报警阀值参数回复
        /// </summary>
        public static readonly byte[] QueryAlarmLimitBack = new Byte[] { 0xC2, 0x05 };

        /// <summary>
        /// 查询经纬度开关灯时间指令
        /// </summary>
        public static readonly byte[] QueryYearTable = new Byte[] { 0x42, 0x06 };

        /// <summary>
        /// 查询经纬度开关灯时间指令
        /// </summary>
        public static readonly byte[] QueryYearTableBack = new Byte[] { 0xC2, 0x06 };

        /// <summary>
        /// 查询预约
        /// </summary>
        public static readonly byte[] QueryPlan = new Byte[] { 0x42, 0x07 };

        /// <summary>
        /// 查询预约回复指令
        /// </summary>
        public static readonly byte[] QueryPlanBack = new Byte[] { 0xC2, 0x07 };

        /// <summary>
        /// 查询自主运行
        /// </summary>
        public static readonly byte[] QuerySefRun = new byte[] { 0x42, 0x08 };

        /// <summary>
        /// 查询自主运行回复
        /// </summary>
        public static readonly byte[] QuerySefRunBack = new Byte[] { 0xC2, 0x08 };

        /// <summary>
        /// 查询光照度开关阀值
        /// </summary>
        public static readonly byte[] QueryLightInfo = new byte[] { 0x42, 0x09 };

        /// <summary>
        /// 查询光照度开关阀值回复
        /// </summary>
        public static readonly byte[] QueryLightInfoBack = new Byte[] { 0xC2, 0x09 };

        /// <summary>
        /// 查询漏电阀值操作参数
        /// </summary>
        public static readonly byte[] QueryLeakCurrentLimit = new byte[] { 0x42, 0x0A };

        /// <summary>
        /// 查询漏电阀值操作参数回复
        /// </summary>
        public static readonly byte[] QueryLeakCurrentLimitBack = new Byte[] { 0xC2, 0x0A };
        
        /// <summary>
        /// 设置网口模块参数
        /// </summary>
        public static readonly byte[] QueryEthernetInterface = new byte[] { 0x42, 0x0B };

        /// <summary>
        /// 设置网口模块参数回复
        /// </summary>
        public static readonly byte[] QueryEthernetInterfaceBack = new Byte[] { 0xC2, 0x0B };
        
        /// <summary>
        /// 数据透传
        /// </summary>
        public static readonly byte[] Transparent = new Byte[] { 0x43, 0x00 };

        /// <summary>
        /// 数据透传回复指令
        /// </summary>
        public static readonly byte[] TransparentBack = new Byte[] { 0xC3, 0x00 };

        /// <summary>
        /// 升级指令
        /// </summary>
        public static readonly byte[] Upgrade = new Byte[] { 0x43, 0x01 };

        /// <summary>
        /// 升级回复指令
        /// </summary>
        public static readonly byte[] UpgradeBack = new Byte[] { 0xC3, 0x01 };
    }
}
