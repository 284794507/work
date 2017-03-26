using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication_Core.PackageHandler
{
    public class XNCmdWordConst
    {
        /// <summary>
        /// 终端上行通信口通信参数设置
        /// </summary>
        public const string SetParamter_F1 = "终端上行通信口通信参数设置";

        public const byte SetParamter_F1_AFN = 0x04;

        /// <summary>
        /// 主站IP地址和端口
        /// </summary>
        public const string SetParamter_F2 = "主站IP地址和端口";

        /// <summary>
        /// 终端事件记录配置设置
        /// </summary>
        public const string SetParamter_F3 = "终端事件记录配置设置";

        /// <summary>
        /// 设备基本信息
        /// </summary>
        public const string SetParamter_F4 = "设备基本信息";

        /// <summary>
        /// 终端事件检测配置参数
        /// </summary>
        public const string SetParamter_F5 = "终端事件检测配置参数";

        /// <summary>
        /// 单灯事件检测配置参数
        /// </summary>
        public const string SetParamter_F6 = "单灯事件检测配置参数";

        /// <summary>
        /// 终端级联通信参数
        /// </summary>
        public const string SetParamter_F7 = "终端级联通信参数";

        /// <summary>
        /// 控制器开关灯时间参数
        /// </summary>
        public const string SetParamter_F9 = "控制器开关灯时间参数";

        /// <summary>
        /// 电能表参数
        /// </summary>
        public const string SetParamter_F10 = "电能表参数";

        /// <summary>
        /// 信息量检测间隔时间参数
        /// </summary>
        public const string SetParamter_F11 = "信息量检测间隔时间参数";

        /// <summary>
        /// 控制回路参数
        /// </summary>
        public const string SetParamter_F12 = "控制回路参数";

        /// <summary>
        /// 集中器与单灯控制器关系参数设置
        /// </summary>
        public const string SetParamter_F17 = "集中器与单灯控制器关系参数设置";

        /// <summary>
        /// 单灯控制器与灯具关系参数
        /// </summary>
        public const string SetParamter_F18 = "单灯控制器与灯具关系参数";

        /// <summary>
        /// 单灯自动控制参数
        /// </summary>
        public const string SetParamter_F19 = "单灯自动控制参数";

        /// <summary>
        /// 单灯分组参数
        /// </summary>
        public const string SetParamter_F20 = "单灯分组参数";

        /// <summary>
        /// 遥信量分类参数
        /// </summary>
        public const string SetParamter_F25 = "遥信量分类参数";

        /// <summary>
        /// 模拟量分类限值参数
        /// </summary>
        public const string SetParamter_F26 = "模拟量分类限值参数";

        /// <summary>
        /// 照度计运行参数
        /// </summary>
        public const string SetParamter_F33 = "照度计运行参数";

        //-----------------------------------------------------------
        /// <summary>
        /// 控制器遥控操作
        /// </summary>
        public const string CtrlParamter_F1 = "控制器遥控操作";

        public const byte CtrlParamter_F1_AFN = 0x05;

        /// <summary>
        /// F31：对时命令
        /// </summary>
        public const string CtrlParamter_F31 = "F31：对时命令";

        public const byte CtrlParamter_F31_AFN = 0x05;

        #region AFN指令

        #region 链路检测0x02（登录；心跳）

        /// <summary>
        /// 链路检测0x02（登录；心跳）
        /// </summary>
        public const byte AFN_LinkDetection_FN = 0x02;

        /// <summary>
        /// 登录
        /// </summary>
        public const string PnFn_LinkDetection_Login = "P0:F1";

        /// <summary>
        /// 登出
        /// </summary>
        public const string PnFn_LinkDetection_Logout = "P0:F2";

        /// <summary>
        /// 心跳
        /// </summary>
        public const string PnFn_LinkDetection_HeartBeat = "P0:F3";

        #endregion 链路检测0x02（登录；心跳）

        #region 确认/否认指令0x00

        /// <summary>
        /// 确认/否认指令0x00
        /// </summary>
        public const byte AFN_ConfirmOrDeny_FN = 0x00;

        /// <summary>
        /// 全部确认
        /// </summary>
        public const string PnFn_ConfirmOrDeny_Confirm = "P0:F1";

        /// <summary>
        /// 全部否认
        /// </summary>
        public const string PnFn_ConfirmOrDeny_Deny = "P0:F2";

        #endregion 确认/否认指令0x00

        #region 复位指令0x01

        /// <summary>
        /// 复位指令
        /// </summary>
        public const byte AFN_Reset_FN = 0x01;

        /// <summary>
        /// 终端复位
        /// </summary>
        public const string PnFn_Reset_CTUReset = "P0:F1";

        /// <summary>
        /// GPRS重连指令
        /// </summary>
        public const string PnFn_Reset_GPRSReconnect = "P0:F2";

        #endregion 复位指令0x01

        #region 设置参数指令0x04

        /// <summary>
        /// 设置参数指令
        /// </summary>
        public const byte AFN_SetParameter_FN = 0x04;

        /// <summary>
        /// 终端上行通信口通信参数设置
        /// </summary>
        public const string PnFn_SetParameter_SetCTUUpCommParam = "P0:F1";

        /// <summary>
        /// 主站IP地址和端口
        /// </summary>
        public const string PnFn_SetParameter_MasterStationIPAndPort = "P0:F2";

        /// <summary>
        /// 主站域名和端口
        /// </summary>
        /// 时间：2016/9/5 10:53
        /// 备注：
        public const string PnFn_SetParameter_MasterStationDomainAndPort = "P0:F1007";

        /// <summary>
        /// 终端事件记录配置设置
        /// </summary>
        public const string PnFn_SetParameter_SetCTUEventLogConfig = "P0:F3";

        /// <summary>
        /// 设置基本信息
        /// </summary>
        public const string PnFn_SetParameter_EquipmentBasicInfo = "P0:F4";

        /// <summary>
        /// 终端事件检测配置参数
        /// </summary>
        public const string PnFn_SetParameter_CTUEventDetectConfigParam = "P0:F5";

        /// <summary>
        /// 单灯事件检测配置参数
        /// </summary>
        public const string PnFn_SetParameter_LampEventDetectConfigParam = "P0:F6";

        /// <summary>
        /// 终端级联通信参数
        /// </summary>
        public const string PnFn_SetParameter_CTUCascadeCommParam = "P0:F7";

        /// <summary>
        /// 控制器开关灯时间参数
        /// </summary>
        public const string PnFn_SetParameter_ControllerLampOnOffTimeParam = "P0:F9";

        /// <summary>
        /// 电能表参数
        /// </summary>
        public const string PnFn_SetParameter_PowerMeterParam = "P0:F10";

        /// <summary>
        /// 信息量检测间隔时间参数
        /// </summary>
        public const string PnFn_SetParameter_DetectInfoIntervalTimeParam = "P0:F11";

        /// <summary>
        /// 控制回路参数
        /// </summary>
        public const string PnFn_SetParameter_ControlLoopParam = "P0:F12";

        /// <summary>
        /// 控制器、单灯组自动控制参数
        /// </summary>
        public const string Fn_SetParameter_CTULampGrpAutoCtl = "F13";

        /// <summary>
        /// 集中器与单灯控制器关系参数设置
        /// </summary>
        public const string PnFn_SetParameter_SetCollectorAndLampCtrlRelationParam = "P0:F17";

        /// <summary>
        /// 单灯控件器与灯具关系参数
        /// </summary>
        public const string PnFn_SetParameter_RTUAndLampCtrlRelationParam = "P0:F18";
        public const string Fn_SetParameter_RTUAndLampCtrlRelationParam = "F18";

        /// <summary>
        /// 单灯自动控制参数
        /// </summary>
        public const string Fn_SetParameter_LampAutoCtlParm = "F19";

        /// <summary>
        /// 单灯分组参数
        /// </summary>
        public const string Fn_SetParameter_LampGrpParam = "F20";

        /// <summary>
        /// 遥信量分类参数
        /// </summary>
        public const string PnFn_SetParameter_RemoteSignalSortParam = "P0:F25";

        /// <summary>
        /// 模拟量分类限值参数
        /// </summary>
        public const string PnFn_SetParameter_AnalogQuantitySortLimitParam = "P0:F26";

        /// <summary>
        /// 照度计运行参数
        /// </summary>
        public const string PnFn_SetParameter_LuminometerParam = "P0:F33";

        #endregion 设置参数指令0x04

        #region 控制指令0x05

        /// <summary>
        /// 控制指令0x05
        /// </summary>
        public const byte AFN_Control_FN = 0x05;

        /// <summary>
        /// 控制器遥控操作
        /// </summary>
        public const string Fn_Control_CTUCtlOpt = "F1";

        /// <summary>
        /// 单灯遥控操作
        /// </summary>
        public const string Fn_Control_LampCtlOpt = "F2";

        /// <summary>
        /// 单灯组遥控操作
        /// </summary>
        public const string Fn_Control_LampGrpCtlOpt = "F3";

        /// <summary>
        /// 删除报警数据
        /// </summary>
        public const string PnFn_Control_DeleteAlarmInfo = "P0:F4";

        /// <summary>
        /// 单灯控制轮询状态切换
        /// </summary>
        public const string PnFn_Control_RTULampPoll = "P0:F9";

        /// <summary>
        /// 单灯控制器复位
        /// </summary>
        public const string Fn_Control_RTUReset = "F10";

        /// <summary>
        /// 单灯控制器组复位
        /// </summary>
        public const string Fn_Control_RTUGrpReset = "F11";

        /// <summary>
        /// 新增单灯预约控制
        /// </summary>
        public const string Fn_Control_AddLampCtlTmp = "F12";

        /// <summary>
        /// 新增单灯组预约控制
        /// </summary>
        public const string Fn_Control_AddLampGrpCtlTmp = "F13";

        /// <summary>
        /// 删除单灯预约控制
        /// </summary>
        public const string Fn_Control_DelLampCtlTmp = "F14";

        /// <summary>
        /// 删除单灯组预约控制
        /// </summary>
        public const string Fn_Control_DelLampGrpCtlTmp = "F15";

        /// <summary>
        /// 新增控制器预约控制
        /// </summary>
        public const string Fn_Control_AddCTUCtlTmp = "F17";

        /// <summary>
        /// 删除控制器预约控制
        /// </summary>
        public const string Fn_Control_DelCTUCtlTmp = "F18";

        /// <summary>
        /// 对时命令
        /// </summary>
        public const string PnFn_CtrlParameter_UpdateCtuTimeParam = "P0:F31";

        #endregion 控制指令0x05

        #region 请求终端配置及信息指令0x09

        /// <summary>
        /// 请求终端配置及信息指令0x09
        /// </summary>
        public const byte AFN_RequestCTUConfigAndInfo_FN = 0x09;

        /// <summary>
        /// 终端信息
        /// </summary>
        public const string PnFn_RequestCTUConfigAndInfo_CTUInfo = "P0:F1";

        #endregion 请求终端配置及信息指令0x09

        #region 查询参数指令0x0A

        /// <summary>
        /// 查询参数指令0x0A
        /// </summary>
        public const byte AFN_QueryParameter_FN = 0x0A;

        /// <summary>
        /// 查询终端上行通信口通信参数设置
        /// </summary>
        public const string PnFn_QueryParameter_SetCTUUpCommParam = "P0:F1";

        /// <summary>
        /// 查询主站IP地址和端口
        /// </summary>
        public const string PnFn_QueryParameter_MasterStationIPAndPort = "P0:F2";

        /// <summary>
        /// 查询主站域名和端口
        /// </summary>
        /// 时间：2016/9/5 10:53
        /// 备注：
        public const string PnFn_QueryParameter_MasterStationDomainAndPort = "P0:F1007";

        /// <summary>
        /// 查询终端事件记录配置设置
        /// </summary>
        public const string PnFn_QueryParameter_SetCTUEventLogConfig = "P0:F3";

        /// <summary>
        /// 查询设置基本信息
        /// </summary>
        public const string PnFn_QueryParameter_EquipmentBasicInfo = "P0:F4";

        /// <summary>
        /// 查询终端事件检测配置参数
        /// </summary>
        public const string PnFn_QueryParameter_CTUEventDetectConfigParam = "P0:F5";

        /// <summary>
        /// 查询单灯事件检测配置参数
        /// </summary>
        public const string PnFn_QueryParameter_LampEventDetectConfigParam = "P0:F6";

        /// <summary>
        /// 查询终端级联通信参数
        /// </summary>
        public const string PnFn_QueryParameter_CTUCascadeCommParam = "P0:F7";

        /// <summary>
        /// 查询控制器开关灯时间参数
        /// </summary>
        public const string PnFn_QueryParameter_ControllerLampOnOffTimeParam = "P0:F9";

        /// <summary>
        /// 查询电能表参数
        /// </summary>
        public const string PnFn_QueryParameter_PowerMeterParam = "P0:F10";

        /// <summary>
        /// 查询信息量检测间隔时间参数
        /// </summary>
        public const string PnFn_QueryParameter_DetectInfoIntervalTimeParam = "P0:F11";

        /// <summary>
        /// 查询控制回路参数
        /// </summary>
        public const string PnFn_QueryParameter_ControlLoopParam = "P0:F12";

        /// <summary>
        /// 查询控制器、单灯组自动控制参数
        /// </summary>
        public const string Fn_QueryParameter_CTULampGrpAutoCtl = "F13";

        /// <summary>
        /// 查询集中器与单灯控制器关系参数设置
        /// </summary>
        public const string PnFn_QueryParameter_SetCollectorAndLampCtrlRelationParam = "P0:F17";

        /// <summary>
        /// 查询单灯控件器与灯具关系参数
        /// </summary>
        public const string PnFn_QueryParameter_RTUAndLampCtrlRelationParam = "P0:F18";

        public const string Fn_QueryParameter_RTUAndLampCtrlRelationParam = "F18";

        /// <summary>
        /// 查询单灯自动控制参数
        /// </summary>
        public const string Fn_QueryParameter_LampAutoCtlParm = "F19";

        /// <summary>
        /// 查询单灯分组参数
        /// </summary>
        public const string Fn_QueryParameter_LampGrpParam = "F20";

        /// <summary>
        /// 查询遥信量分类参数
        /// </summary>
        public const string PnFn_QueryParameter_RemoteSignalSortParam = "P0:F25";

        /// <summary>
        /// 查询模拟量分类限值参数
        /// </summary>
        public const string PnFn_QueryParameter_AnalogQuantitySortLimitParam = "P0:F26";

        /// <summary>
        /// 查询照度计运行参数
        /// </summary>
        public const string PnFn_QueryParameter_LuminometerParam = "P0:F33";

        #endregion 查询参数指令0x0A

        #region 请求实时数据指令0x0C

        /// <summary>
        /// 请求实时数据指令0x0C
        /// </summary>
        public const byte AFN_RequestRealTimeData_FN = 0x0C;

        /// <summary>
        /// 终端日历时钟
        /// </summary>
        public const string PnFn_RequestRealTimeData_CTUCalendarClock = "P0:F2";

        /// <summary>
        /// 节能开关档位
        /// </summary>
        public const string PnFn_RequestRealTimeData_EnergySavingSwitchGear = "P0:F3";

        /// <summary>
        /// 单灯轮询查询
        /// </summary>
        public const string PnFn_RequestRealTimeData_QueryPollLamp = "P0:F4";

        /// <summary>
        /// 模拟量查询
        /// </summary>
        public const string Fn_RequestRealTimeData_QueryAnalogQuantity = "F9";

        /// <summary>
        /// 遥信量查询
        /// </summary>
        public const string Fn_RequestRealTimeData_QueryRemoteSignal = "F10";

        /// <summary>
        /// 模拟量批量查询
        /// </summary>
        public const string PnFn_RequestRealTimeData_BatchQueryAnalogQuantity = "P0:F11";

        /// <summary>
        /// 遥信量批量查询
        /// </summary>
        public const string PnFn_RequestRealTimeData_BatchQueryRemoteSignal = "P0:F12";

        /// <summary>
        /// 智能电表电量查询
        /// </summary>
        public const string Fn_RequestRealTimeData_SmartMeterElectricity = "F17";

        /// <summary>
        /// 发现新单灯控制器查询
        /// </summary>
        public const string PnFn_RequestRealTimeData_QueryNewPTU = "P0:F25";

        /// <summary>
        /// 单灯运行状态查询
        /// </summary>
        public const string Fn_RequestRealTimeData_QueryLampPlusStatus = "F33";

        /// <summary>
        /// 单灯电压查询，电参数查询
        /// </summary>
        public const string Fn_RequestRealTimeData_Voltage = "F34";

        /// <summary>
        /// 单灯电流查询
        /// </summary>
        public const string Fn_RequestRealTimeData_Current = "F35";

        /// <summary>
        /// 单灯有功功率查询
        /// </summary>
        public const string Fn_RequestRealTimeData_ActivePower = "F36";

        /// <summary>
        /// 单灯功率因素查询
        /// </summary>
        public const string Fn_RequestRealTimeData_PowerFact = "F37";

        /// <summary>
        /// 单灯视在功率查询
        /// </summary>
        public const string Fn_RequestRealTimeData_AppPower = "F38";

        /// <summary>
        /// 单灯无功功率查询
        /// </summary>
        public const string Fn_RequestRealTimeData_ReactivePower = "F39";

        #endregion 请求实时数据指令0x0C

        #region 请求历史数据指令0x0D

        /// <summary>
        /// 请求历史数据指令
        /// </summary>
        public const byte AFN_RequestHistoryData_FN = 0x0D;

        /// <summary>
        /// 控制器模拟量历史数据曲线
        /// </summary>
        public const string Fn_RequestHistoryData_CTUAnalogQuantityHisDataChart = "F1";

        /// <summary>
        /// 控制器日开关灯记录
        /// </summary>
        public const string PnFn_RequestHistoryData_CTUOpenDayLog = "P0:F2";

        /// <summary>
        /// 日电冻结智能电表电量数据
        /// </summary>
        public const string Fn_RequestHistoryData_DayFrozenSmartMeter = "F9";

        /// <summary>
        /// 单灯电压历史数据曲线
        /// </summary>
        public const string Fn_RequestHistoryData_LampVoltageHisDataChart = "F17";

        /// <summary>
        /// 单灯电流历史数据曲线
        /// </summary>
        public const string Fn_RequestHistoryData_LampCurrentHisDataChart = "F18";

        /// <summary>
        /// 单灯有功功率历史数据曲线
        /// </summary>
        public const string Fn_RequestHistoryData_LampActivePowerHisDataChart = "F19";

        /// <summary>
        /// 单灯功率因素历史数据曲线
        /// </summary>
        public const string Fn_RequestHistoryData_LampPowerFactHisDataChart = "F20";

        /// <summary>
        /// 单灯视在功率历史数据曲线
        /// </summary>
        public const string Fn_RequestHistoryData_LampAppPowerHisDataChart = "F21";

        /// <summary>
        /// 单灯无功功率历史数据曲线
        /// </summary>
        public const string Fn_RequestHistoryData_LampReactivePowerHisDataChart = "F22";

        #endregion 请求历史数据指令0x0D

        #region 请求事件数据指令0x0E

        /// <summary>
        /// 请求事件数据指令
        /// </summary>
        public const byte AFN_RequestEventData_FN = 0x0E;

        /// <summary>
        /// 请求重要事件
        /// </summary>
        public const string PnFn_RequestEventData_Important = "P0:F1";

        /// <summary>
        /// 请求一般事件
        /// </summary>
        public const string PnFn_RequestEventData_Common = "P0:F2";

        #endregion 请求事件数据指令0x0E

        #region 数据转发指令0x10

        /// <summary>
        /// 数据转发指令
        /// </summary>
        public const byte AFN_ForwardData_FN = 0x10;

        /// <summary>
        /// 透明转发
        /// </summary>
        public const string PnFn_ForwardData_TransparentForward = "P0:F1";

        /// <summary>
        /// FTP升级命令
        /// </summary>
        public const string PnFn_ForwardData_FTPUpdateCmd = "P0:F9";

        /// <summary>
        /// FTP升级命令(域名)
        /// </summary>
        public const string PnFn_ForwardData_FTPUpdateCmd_domain = "P0:F100";

        /// <summary>
        /// FTP升级结果
        /// </summary>
        public const string PnFn_ForwardData_FTPUpdateResult = "P0:F10";

        #endregion 数据转发指令0x10

        #endregion AFN指令
    }
}
