using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr
{
    public static class LHCmdWordConst
    {
        #region 故障指令

        /// <summary>
        /// CTU回复的erro指令
        /// [Para1] 1 字节，
        ///01h表示 格式不符  （正在超过8字节时，才会响应），
        ///02表示 地址不符错误，
        ///03表示 指令不对，
        ///04表示 指令中的参数错
        ///xx----其他需要扩展
        /// </summary>
        public static readonly byte[] RecvErrorCmd = new Byte[] { 0x84, 0x01 };

        public static readonly byte[] ReplyErrorCmd = new Byte[] { 0x04, 0x01 };

        #endregion 故障指令

        /// <summary>
        /// 收到登入指令　0x81, 0x00
        /// </summary>
        public static readonly byte[] GetLogin = new Byte[] { 0x81, 0x00 };

        /// <summary>
        /// 回复登录指令 0x01, 0x00
        /// </summary>
        public static readonly byte[] ReplyLogin = new Byte[] { 0x01, 0x00 };

        /// <summary>
        /// 收到的心跳包指令 0x82, 0x00
        /// </summary>
        public static readonly byte[] GetHeartBeat = new Byte[] { 0x82, 0x00 };

        /// <summary>
        /// 回复的心跳包指令 0x02, 0x00
        /// </summary>
        public static readonly byte[] ReplyHeartBeat = new Byte[] { 0x02, 0x00 };

        /// <summary>
        /// 发送的实时查询CTU时间指令
        /// </summary>
        public static readonly byte[] SendQueryCTUTime = new Byte[] { 0x08, 0x01 };

        /// <summary>
        /// 收到的实时查询CTU时间指令
        /// </summary>
        public static readonly byte[] RecvQueryCTUTime = new Byte[] { 0x88, 0x01 };

        /// <summary>
        /// 发送的修改CTU时间的指令
        /// </summary>
        public static readonly byte[] SendUpdateCTUTime = new Byte[] { 0x07, 0x01 };

        /// <summary>
        /// 收到的修改CTU时间的指令
        /// </summary>
        public static readonly byte[] RecvUpdateCTUTime = new Byte[] { 0x87, 0x01 };
        /// <summary>
        /// 发送的实时PTU单灯控制指令 0x08, 0x02
        /// </summary>
        public static readonly byte[] SendRTPtuChCtrl = new Byte[] { 0x08, 0x02 };

        /// <summary>
        /// 确认收到实时PTU单灯控制指令 0xC8, 0x02
        /// </summary>
        public static readonly byte[] ConfirmRTPtuChCtrl = new Byte[] { 0xC8, 0x02 };

        /// <summary>
        /// 收到的实时PTU单灯控制指令 0x88, 0x02
        /// </summary>
        public static readonly byte[] RecvRTPtuChCtrl = new Byte[] { 0x88, 0x02 };

        /// <summary>
        /// 发送的实时控制CTU回路操作指令
        /// </summary>
        public static readonly byte[] SendRTCtuChCtrl = new Byte[] { 0x08, 0x11 };

        /// <summary>
        /// 收到的实时控制CTU回路操作指令
        /// </summary>
        public static readonly byte[] RecvRTCtuChCtrl = new Byte[] { 0x88, 0x11 };

        /// <summary>
        /// 在CTU无法向LTU执行操作（无返回时）CTU回复 0x84, 0x02
        /// </summary>
        public static readonly byte[] GetPtuErrorCmd = new Byte[] { 0x84, 0x02 };

        /// <summary>
        /// 发送的实时PTU单灯组控指令 0x08, 0x03
        /// </summary>
        public static readonly byte[] SendRTPtuChCtrlByGroup = new Byte[] { 0x08, 0x03 };

        /// <summary>
        /// 收到的实时PTU单灯组控指令 0x88, 0x03
        /// </summary>
        public static readonly byte[] RecvRTPtuChCtrlByGroup = new Byte[] { 0x88, 0x03 };

        #region 单灯自主运行设置和查询

        /// <summary>
        /// 发送的单灯自主运行设置 0x07, 0x06
        /// </summary>
        public static readonly byte[] SendRTLampSelfRunCfg = new Byte[] { 0x07, 0x06 };

        /// <summary>
        /// 收到的单灯自主运行设置（ctu迅速返回） 0xC7, 0x06
        /// </summary>
        public static readonly byte[] RecvRTLampSelfRunCfgFromCtu = new Byte[] { 0xC7, 0x06 };

        /// <summary>
        /// 收到的单灯自主运行设置（Ptu返回） 0x87, 0x06
        /// </summary>
        public static readonly byte[] RecvRTLampSelfRunCfgFromPtu = new Byte[] { 0x87, 0x06 };

        //查询单个Lamp自主运行的指令

        /// <summary>
        /// 发送的查询单个Lamp自主运行的指令 0x15, 0x06
        /// </summary>
        public static readonly byte[] SendQueryLampSelfRunCfg = new Byte[] { 0x15, 0x06 };

        /// <summary>
        /// 收到的查询单个Lamp自主运行的指令(查询ptu的话，ctu迅速返回) 0xd5, 0x06
        /// </summary>
        public static readonly byte[] RecvQueryLampSelfRunCfgRapidly = new Byte[] { 0xd5, 0x06 };

        /// <summary>
        /// 收到的查询单个Lamp自主运行的指令(查询ptu的话，ptu返回；如果只查ctu也是此指令返回，内含参数标示getway) 0x95, 0x06 
        /// </summary>
        public static readonly byte[] RecvQueryLampSelfRunCfg = new Byte[] { 0x95, 0x06 };

        #endregion 单灯自主运行设置和查询

        #region 单灯组设置和查询

        /// <summary>
        /// 发送的单灯组设置指令 0x07, 0x09
        /// </summary>
        public static readonly byte[] SendRTLampGroupCfg = new Byte[] { 0x07, 0x09 };

        /// <summary>
        /// 收到的单灯组设置指令（ctu迅速返回） 0xC7, 0x09
        /// </summary>
        public static readonly byte[] RecvRTLampGroupCfgFromCtu = new Byte[] { 0xC7, 0x09 };

        /// <summary>
        /// 收到的单灯组设置指令（Ptu返回） 0x87, 0x09
        /// </summary>
        public static readonly byte[] RecvRTLampGroupCfgFromPtu = new Byte[] { 0x87, 0x09 };

        //查询单个Lamp分组参数的指令

        /// <summary>
        /// 发送的查询单个Lamp分组参数的指令 0x15, 0x09
        /// </summary>
        public static readonly byte[] SendQueryRTLampGroupCfg = new Byte[] { 0x15, 0x09 };

        /// <summary>
        /// 收到的查询单个Lamp分组参数的指令(查询ptu的话，ctu迅速返回) 0xd5, 0x09
        /// </summary>
        public static readonly byte[] RecvQueryRTLampGroupCfgRapidly = new Byte[] { 0xd5, 0x09 };

        /// <summary>
        /// 收到的查询单个Lamp分组参数的指令(查询ptu的话，ptu返回；如果只查ctu也是此指令返回，内含参数标示getway) 0x95, 0x09
        /// </summary>
        public static readonly byte[] RecvQueryRTLampGroupCfg = new Byte[] { 0x95, 0x09 };

        #endregion 单灯组设置和查询
        
        /// <summary>
        /// 发送的实时查询PTU的详细状态指令（查LTU）
        /// </summary>
        public static readonly byte[] SendRTQueryLampDetailStatus = new Byte[] { 0x12, 0x02 };

        /// <summary>
        /// 迅速收到的实时查询PTU的详细状态指令（查LTU）
        /// </summary>
        public static readonly byte[] RecvRTQueryLampDetailStatus_Quick = new Byte[] { 0xd2, 0x02 };

        /// <summary>
        /// 收到的实时查询PTU的详细状态指令（查LTU）
        /// </summary>
        public static readonly byte[] RecvRTQueryLampDetailStatus = new Byte[] { 0x92, 0x02 };

        /// <summary>
        /// 发送的实时查询PTU的状态指令（查RTU的DB） 0x12, 0x03
        /// </summary>
        public static readonly byte[] SendRTQueryLampStatusFromDB = new Byte[] { 0x12, 0x03 };

        /// <summary>
        /// 收到的实时查询PTU的状态指令（查RTU的DB） 0x92, 0x03
        /// </summary>
        public static readonly byte[] RecvRTQueryLampStatusFromDB = new Byte[] { 0x92, 0x03 };

        /// <summary>
        /// 发送的实时查询PTU的详细状态指令（查RTU的DB） 0x12, 0x04
        /// </summary>
        public static readonly byte[] SendRTQueryLampStatusDetailFromDB = new Byte[] { 0x12, 0x04 };

        /// <summary>
        /// 收到的实时查询PTU的详细状态指令（查RTU的DB） 0x92, 0x04
        /// </summary>
        public static readonly byte[] RecvRTQueryLampStatusDetailFromDB = new Byte[] { 0x92, 0x04 };

        /// <summary>
        /// 收到灯故障报警指令 0x89, 0x09 
        /// </summary>
        public static readonly byte[] GetLampStatusChangedCmd = new Byte[] { 0x89, 0x09 };

        /// <summary>
        /// 回复灯故障报警指令 0x09, 0x09
        /// </summary>
        public static readonly byte[] ReplyLampStatusChangedCmd = new Byte[] { 0x09, 0x09 };

        #region 单灯组预约设置相关

        /// <summary>
        /// 发送的单灯组临时预约设置指令 0x06, 0x09 
        /// </summary>
        public static readonly byte[] SendRTLampGroupCfgOfTmpCtrl = new Byte[] { 0x06, 0x09 };

        /// <summary>
        /// 收到的单灯组临时预约设置指令 0x86, 0x09 
        /// </summary>
        public static readonly byte[] RecvRTLampGroupCfgOfTmpCtrl = new Byte[] { 0x86, 0x09 };

        /// <summary>
        /// 发送的查询单灯组临时预约的指令 0x14, 0x09
        /// </summary>
        public static readonly byte[] SendQueryRTLampGroupCfgOfTmpCtrl = new Byte[] { 0x14, 0x09 };

        /// <summary>
        /// 收到的查询单灯组临时预约的指令 0x94, 0x09
        /// </summary>
        public static readonly byte[] RecvQueryRTLampGroupCfgOfTmpCtrl = new Byte[] { 0x94, 0x09 };

        /// <summary>
        /// 发送的单灯组周期性预约设置指令 0x06, 0x0A
        /// </summary>
        public static readonly byte[] SendLampCyclicTmpCfg = new Byte[] { 0x06, 0x0A };

        /// <summary>
        /// 收到的单灯组周期性预约设置指令 0x86, 0x0A
        /// </summary>
        public static readonly byte[] RecvLampCyclicTmpCfg = new Byte[] { 0x86, 0x0A };

        /// <summary>
        /// 发送的查询单灯组周期性预约设置指令 0x14, 0x0A
        /// </summary>
        public static readonly byte[] SendQueryLampCyclicTmpCfg = new Byte[] { 0x14, 0x0A };

        /// <summary>
        /// 收到的查询单灯组周期性预约设置指令 0x94, 0x0A 
        /// </summary>
        public static readonly byte[] RecvQueryLampCyclicTmpCfg = new Byte[] { 0x94, 0x0A };

        /// <summary>
        /// 发送的单灯基本设置指令 0x07, 0x05
        /// </summary>
        public static readonly byte[] SendLampBasicCfg_Set = new Byte[] { 0x07, 0x05 };

        /// <summary>
        /// 收到的单灯基本设置指令 0x87, 0x05
        /// </summary>
        public static readonly byte[] RecvLampBasicCfg_Set = new Byte[] { 0x87, 0x05 };

        /// <summary>
        /// 收到的单灯基本设置指令_CTU快速回复 0xC7, 0x05
        /// </summary>
        public static readonly byte[] RecvLampBasicCfg_Set_Quick = new Byte[] { 0xC7, 0x05 };

        /// <summary>
        /// 发送的查询单灯基本设置指令 0x15, 0x05
        /// </summary>
        public static readonly byte[] SendLampBasicCfg_Query = new Byte[] { 0x15, 0x05 };

        /// <summary>
        /// 收到的查询单灯基本设置指令 0x95, 0x05
        /// </summary>
        public static readonly byte[] RecvLampBasicCfg_Query = new Byte[] { 0x95, 0x05 };

        /// <summary>
        /// 发送的查询单灯用电量指令 0x12, 0x09
        /// </summary>
        public static readonly byte[] SendLampPowerConsumption_Query = new Byte[] { 0x12, 0x09 };

        /// <summary>
        /// 收到的查询单灯用电量指令 0x92, 0x09
        /// </summary>
        public static readonly byte[] RecvLampPowerConsumption_Query = new Byte[] { 0x92, 0x09 };

        #endregion


        /// <summary>
        /// 发送的设定CTU中年表数据指令
        /// </summary>
        public static readonly byte[] SendYearTableCFG = new Byte[] { 0x06, 0x02 };

        /// <summary>
        /// 收到的设定CTU中年表数据指令
        /// </summary>
        public static readonly byte[] RecvYearTableCFG = new Byte[] { 0x86, 0x02 };

        /// <summary>
        /// 发送的查询CTU中年表数据指令
        /// </summary>
        public static readonly byte[] SendQueryYearTableInfo = new Byte[] { 0x14, 0x02 };

        /// <summary>
        /// 收到的查询CTU中年表数据指令
        /// </summary>
        public static readonly byte[] RecvQueryYearTableInfo = new Byte[] { 0x94, 0x02 };


        /// <summary>
        /// 发送的升级包升级指令 (0x2306)
        /// </summary>
        public static readonly byte[] SendUpdateZipFileCmd = new Byte[] { 0x23, 0x06 };

        /// <summary>
        /// 收到的升级包升级指令 (0xA306)
        /// </summary>
        public static readonly byte[] RecvUpdateZipFileCmd = new Byte[] { 0xA3, 0x06 };
    }
}
