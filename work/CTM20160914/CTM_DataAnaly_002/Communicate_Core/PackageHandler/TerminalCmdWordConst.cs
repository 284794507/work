using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.PackageHandler
{
    public static class TerminalCmdWordConst
    {
        /// <summary>
        /// 复位指令
        /// </summary>
        public static readonly byte[] Reset=new byte[] { 0x30, 0x01 };

        /// <summary>
        /// 终端复位
        /// </summary>
        public static readonly byte TerminalReset_FN = 0xF1;

        /// <summary>
        /// GPRS重连指令
        /// </summary>
        public static readonly byte GPRSReset_FN = 0xF2;

        /// <summary>
        /// 复位回复指令
        /// </summary>
        public static readonly byte[] ResetBack = new byte[] { 0xB0, 0x01 };

        /// <summary>
        /// 登录指令
        /// </summary>
        public static readonly byte[] GetLoginOrHeartBeat = new byte[] { 0xB0, 0x02 };

        /// <summary>
        /// 登录
        /// </summary>
        public static readonly byte LoginON_FN = 0xF1;

        /// <summary>
        /// 退出登录
        /// </summary>
        public static readonly byte LoginOFF_FN = 0xF2;

        /// <summary>
        /// 心跳
        /// </summary>
        public static readonly byte HeartBeat_FN = 0xF3;

        /// <summary>
        /// 登录回复指令
        /// </summary>
        public static readonly byte[] GetLoginOrHeartBeatBack = new byte[] { 0x30, 0x02 };
        
        /// <summary>
        /// 设置指令
        /// </summary>
        public static readonly byte[] SetCmd = new byte[] { 0x30, 0x04 };

        /// <summary>
        /// 时间同步
        /// </summary>
        public static readonly byte SetTime_FN = 0xF1;

        /// <summary>
        /// 设置台区信息
        /// </summary>
        public static readonly byte SetPlatForm_FN = 0xF2;

        /// <summary>
        /// 配置LORA模组
        /// </summary>
        public static readonly byte SetLora_FN = 0xF3;

        /// <summary>
        /// 恢复默认电系数
        /// </summary>
        public static readonly byte ResetElecFact_FN = 0xF4;

        /// <summary>
        /// 校正电系数
        /// </summary>
        public static readonly byte SetElecFact_FN = 0xF5;

        /// <summary>
        /// 设置SN号
        /// </summary>
        public static readonly byte SetSNCode_FN = 0xF6;

        /// <summary>
        /// 设置IP和端口号
        /// </summary>
        public static readonly byte SetIPAndPort_FN = 0xF7;

        /// <summary>
        /// 设置安装状态
        /// </summary>
        public static readonly byte SetInstallStatus_FN = 0xF8;


        /// <summary>
        /// 设置回复指令
        /// </summary>
        public static readonly byte[] SetCmdBack = new byte[] { 0xB0, 0x04 };

        /// <summary>
        /// 查询指令
        /// </summary>
        public static readonly byte[] QueryCmd = new byte[] { 0x30, 0x05 };

        /// <summary>
        /// 查询时间
        /// </summary>
        public static readonly byte QueryTime_FN = 0xF1;

        /// <summary>
        /// 查询台区信息
        /// </summary>
        public static readonly byte QueryPlatForm_FN = 0xF2;

        /// <summary>
        /// 查询LORA模组
        /// </summary>
        public static readonly byte QueryLora_FN = 0xF3;

        /// <summary>
        /// 查询电系数
        /// </summary>
        public static readonly byte QueryElecFact_FN = 0xF4;

        /// <summary>
        /// 查询IP和端口号
        /// </summary>
        public static readonly byte QueryIPAndPort_FN = 0xF5;

        /// <summary>
        /// 查询SN号
        /// </summary>
        public static readonly byte QuerySNCode_FN = 0xF6;

        /// <summary>
        /// 查询安装状态
        /// </summary>
        public static readonly byte QueryInstallStatus_FN = 0xF8;

        /// <summary>
        /// 查询回复指令
        /// </summary>
        public static readonly byte[] QueryCmdBack = new byte[] { 0xB0, 0x05 };

        /// <summary>
        /// 查询设备信息
        /// </summary>
        public static readonly byte[] QueryTerminalBasicInfo = new byte[] { 0x30, 0x06 };

        /// <summary>
        /// 查询设备信息
        /// </summary>
        public static readonly byte QueryTerminalInfo_FN = 0xF1;

        /// <summary>
        /// 查询设备列表
        /// </summary>
        public static readonly byte QueryTerminalList_FN = 0xF2;

        /// <summary>
        /// 查询设备信息回复指令
        /// </summary>
        public static readonly byte[] QueryTerminalBasicInfoBack = new byte[] { 0xB0, 0x06 };

        /// <summary>
        /// 查询数据
        /// </summary>
        public static readonly byte[] QueryData = new byte[] { 0x30, 0x07 };

        /// <summary>
        /// 查询历史数据
        /// </summary>
        public static readonly byte QueryHisData_FN = 0xF1;

        /// <summary>
        /// 查询故障数据
        /// </summary>
        public static readonly byte QueryErrorData = 0xF2;

        /// <summary>
        /// 查询数据回复指令
        /// </summary>
        public static readonly byte[] QueryDataBack = new byte[] { 0xB0, 0x07 };

        /// <summary>
        /// 查询通信信息
        /// </summary>
        public static readonly byte[] QueryCommData = new byte[] { 0x30, 0x08 };

        /// <summary>
        /// 查询通信信息
        /// </summary>
        public static readonly byte QueryComm_FN = 0xF1;

        /// <summary>
        /// 查询通信信息回复指令
        /// </summary>
        public static readonly byte[] QueryCommDataBack = new byte[] { 0xB0, 0x08 };

        /// <summary>
        /// 上报设备信息
        /// </summary>
        public static readonly byte[] UploadTerminalBasicInfo = new byte[] { 0xB0, 0x0C };

        /// <summary>
        /// 上报设备信息
        /// </summary>
        public static readonly byte UploadTerminalInfo_FN = 0xF1;

        /// <summary>
        /// 上报设备信息回复指令
        /// </summary>
        public static readonly byte[] UploadTerminalBasicInfoBack = new byte[] { 0x30, 0x0C };

        /// <summary>
        /// 上报数据
        /// </summary>
        public static readonly byte[] UploadData = new byte[] { 0xB0, 0x0D };

        /// <summary>
        /// 上报历史数据
        /// </summary>
        public static readonly byte UploadHisData_FN = 0xF1;

        /// <summary>
        /// 上报故障数据
        /// </summary>
        public static readonly byte UploadErrorData_FN = 0xF2;

        /// <summary>
        /// 上报数据回复指令
        /// </summary>
        public static readonly byte[] UploadDataBack = new byte[] { 0x30, 0x0D };

        /// <summary>
        /// 上报通信信息
        /// </summary>
        public static readonly byte[] UploadCommData = new byte[] { 0xB0, 0x0E };

        /// <summary>
        /// 上报通信信息
        /// </summary>
        public static readonly byte UploadComm_FN = 0xF1;

        /// <summary>
        /// 上报通信信息回复指令
        /// </summary>
        public static readonly byte[] UploadCommDataBack = new byte[] { 0x30, 0x0E };

        /// <summary>
        /// 远程升级
        /// </summary>
        public static readonly byte[] RemoteUpgrade = new byte[] { 0x30, 0x0F };

        /// <summary>
        /// 准备升级
        /// </summary>
        public static readonly byte Ready_FN = 0xF1;

        /// <summary>
        /// 升级中
        /// </summary>
        public static readonly byte Upgrading_FN = 0xF2;

        /// <summary>
        /// 升级确认
        /// </summary>
        public static readonly byte Confirm_FN = 0xF3;

        /// <summary>
        /// 取消升级
        /// </summary>
        public static readonly byte Cancel_FN = 0xF4;

        /// <summary>
        /// 远程升级回复
        /// </summary>
        public static readonly byte[] RemoteUpgradeBack = new byte[] { 0xB0, 0x0F };

        /// <summary>
        /// 重发此包
        /// </summary>
        public static readonly byte ReSendBag_FN = 0xF1;

        /// <summary>
        /// 接收成功
        /// </summary>
        public static readonly byte ReceviceSuccess_FN = 0xF2;

        /// <summary>
        /// 不允许升级
        /// </summary>
        public static readonly byte NotAllow_FN = 0xF3;

        /// <summary>
        /// 回应续传
        /// </summary>
        public static readonly byte SendAgain_FN = 0xF4;

        /// <summary>
        /// 数据转发
        /// </summary>
        public static readonly byte[] Retransmission = new byte[] { 0x30, 0x10 };

        /// <summary>
        /// 数据转发
        /// </summary>
        public static readonly byte Retransmission_FN = 0xF1;

        /// <summary>
        /// 路由设置
        /// </summary>
        public static readonly byte[] SetRoute = new byte[] { 0x30, 0x0A };

        /// <summary>
        /// 数据转发
        /// </summary>
        public static readonly byte SetRoute_FN = 0xF1;

        /// <summary>
        /// 路由设置
        /// </summary>
        public static readonly byte[] SetRouteBack = new byte[] { 0xB0, 0x0A };        
    }
}
