using Communication_Core.PackageHandler;
using Communication_Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication_Core
{
    class PlatFormCommClient
    {
    }

    public partial class DistriCommClient
    {

        public void PlatFormRecevice(IAsyncResult ar)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("PlatFormRecevice");
                }))
                .Do(() =>
                {
                    if (DistriClient.Connected)
                    {
                        TerminalClient TClient = ar.AsyncState as TerminalClient;

                        //已经关闭的流就退出
                        if (!TClient.netStream.CanRead)
                        {
                            return;
                        }
                        int len = 0;
                        try
                        {
                            len = TClient.netStream.EndRead(ar);
                        }
                        catch
                        {
                            len = 0;
                        }
                        if (len == 0)
                        {
                            Share.Instance.WriteMsg("平台数据读取失败！", 4);
                            return;
                        }
                        TClient.lastAliveTimeToPlatForm = DateTime.Now;

                        //byte[] buffer = (byte[])ar.AsyncState;
                        byte[] receviceData = new byte[len];
                        Buffer.BlockCopy(TClient.readBytes, 0, receviceData, 0, len);
                        Share.Instance.WriteMsg("Recevice data from PlatForm:" + ByteHelper.byteToHexStr_Blank(receviceData), 4);
                        AnalyProtocol_PlatForm(receviceData,TClient);

                        TClient.netStream.BeginRead(TClient.readBytes, 0, TClient.readBytes.Length, PlatFormRecevice, TClient);
                    }
                });
        }

        public void AnalyProtocol_PlatForm(byte[] data, TerminalClient TClient)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("AnalyProtocol_PlatForm");
                }))
                .Do(() =>
                {
                    //包解析成功与否
                    bool success = false;
                    //包解析后的粘包
                    byte[] afterPackage = null;
                    XNPackageData package = null;
                    XNPackageErrEnum msg = XNPackageErrEnum.Normal;

                    //do
                    //{
                        if (afterPackage == null)
                        {
                            //无粘包，解析欣能协议
                            success = XNPackageData.BuileObjFromBytes(data, out package, out afterPackage, out msg);
                        }
                        else
                        {
                            //拿出粘包，解析欣能协议
                            success = XNPackageData.BuileObjFromBytes(afterPackage, out package, out afterPackage, out msg);
                        }

                        if (success)
                        {//如果解析成功，只转发符合条件的报文到设备
                            AnalyPlatFormPackage(package);
                            if (PlatFormPackageHandler(package,TClient))
                            {
                                SendData(data);
                            }
                        }
                        else
                        {
                            //SendData(data);//平台下发的报文如果解析失败，原样转发到设备
                            Share.Instance.WriteMsg("Analy data fail:" + ByteHelper.byteToHexStr_Blank(data) , 4);
                            Share.Instance.WriteMsg(msg.ToString(), 4);
                        }
                    //}
                    //while (afterPackage != null);
                });
        }

        public bool  PlatFormPackageHandler(XNPackageData package, TerminalClient TClient)
        {
            bool result = true;
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("PlatFormPackageHandler");
                }))
                .Do(() =>
                {
                    //bool isSend = true;
                    switch (package.AFN_FC)
                    {
                        case XNCmdWordConst.AFN_QueryParameter_FN:
                            //string pnfn = package.PnFn;
                            if (package.PnFn == XNCmdWordConst.PnFn_QueryParameter_MasterStationIPAndPort)
                            {
                                result = false;
                                string[] ip = Share.Instance.PlatFormEndPoint.Address.ToString().Split('.');
                                if(ip.Length==4)
                                {
                                    using (MemoryStream mem = new MemoryStream())
                                    {
                                        BinaryWriter writer = new BinaryWriter(mem);
                                        writer.Write(Convert.ToByte(ip[0]));
                                        writer.Write(Convert.ToByte(ip[1]));
                                        writer.Write(Convert.ToByte(ip[2]));
                                        writer.Write(Convert.ToByte(ip[3]));
                                        byte[] port = ByteHelper.ConvertUIntToBytes((ushort)Share.Instance.PlatFormEndPoint.Port, false);
                                        writer.Write(port);
                                        byte[]apn = ByteHelper.StrToASCII(Share.Instance.PlatFormAPN, 16);
                                        writer.Write(apn);
                                        byte[] dataAll = mem.ToArray();
                                        writer.Close();

                                        XNPackageData Pkg = new XNPackageData(package.Address, XNCmdWordConst.AFN_QueryParameter_FN, XNCmdWordConst.PnFn_QueryParameter_MasterStationIPAndPort, dataAll, true, true);
                                        SendDataToPlatForm(TClient, Pkg.ToBytes());
                                    }
                                }
                            }
                            break;
                        //case XNCmdWordConst.AFN_ConfirmOrDeny_FN:
                        //    result = true;
                        //    break;
                        default:
                            string key = BitConverter.ToString(package.Address);
                            if (Share.Instance.OnlyReceviceDevList != null && Share.Instance.OnlyReceviceDevList.Count() > 0 && Share.Instance.OnlyReceviceDevList.Contains(key))
                            {//只能接收的箱不转发平台下发报文
                                result = false;
                            }
                            break;
                    }
                });
            return result;
        }

        private void AnalyPlatFormPackage(XNPackageData package)
        {
            string key = BitConverter.ToString(package.Address);
            switch (package.AFN_FC)
            {
                #region AFN=00
                //欣能协议中的复位命令、设置参数、控制命令　的回复报文都是此指令，无法区分，所以采用统一回复的方法，到平台再处理
                case XNCmdWordConst.AFN_ConfirmOrDeny_FN://确认否认指令
                    Share.Instance.LogInfo("通用回复！ID：" + key);
                    break;
                #endregion
                #region AFN=09

                case XNCmdWordConst.AFN_RequestCTUConfigAndInfo_FN://查询终端配置及信息
                    Share.Instance.LogInfo("终端配置及信息查询！ID：" + key);
                    break;
                #endregion
                #region AFN=0A

                case XNCmdWordConst.AFN_SetParameter_FN://参数查询指令
                    switch (package.PnFn)
                    {
                        case XNCmdWordConst.PnFn_SetParameter_SetCTUUpCommParam://查询终端上行通信口通信参数设置
                            Share.Instance.LogInfo("查询终端上行通信口通信参数设置！ID：" + key);
                            break;
                        case XNCmdWordConst.PnFn_SetParameter_SetCTUEventLogConfig://终端事件记录配置查询
                            Share.Instance.LogInfo("终端事件记录配置查询！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_EquipmentBasicInfo://基本设备信息查询
                            Share.Instance.LogInfo("基本设备信息查询！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_MasterStationIPAndPort://主站IP和端口查询
                            Share.Instance.LogInfo("主站IP和端口查询！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_CTUEventDetectConfigParam://查询终端事件检测配置参数
                            Share.Instance.LogInfo("/查询终端事件检测配置参数！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_LampEventDetectConfigParam://查询单灯事件检测配置参数
                            Share.Instance.LogInfo("查询单灯事件检测配置参数！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_CTUCascadeCommParam://查询终端级联通信参数
                            Share.Instance.LogInfo("查询终端级联通信参数！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_ControllerLampOnOffTimeParam://年表
                            Share.Instance.LogInfo("年表查询！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_PowerMeterParam://查询电能表参数
                            Share.Instance.LogInfo("查询电能表参数！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_DetectInfoIntervalTimeParam://查询信息量检测间隔时间参数
                            Share.Instance.LogInfo("查询信息量检测间隔时间参数！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_ControlLoopParam://查询控制回路参数
                            Share.Instance.LogInfo("查询控制回路参数！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_SetCollectorAndLampCtrlRelationParam://查询集中器与单灯控制器关系参数
                            Share.Instance.LogInfo("查询集中器与单灯控制器关系参数！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_RTUAndLampCtrlRelationParam://查询单灯控件器与灯具关系参数
                            Share.Instance.LogInfo("查询单灯控件器与灯具关系参数！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_RemoteSignalSortParam://查询遥信量分类参数
                            Share.Instance.LogInfo("查询遥信量分类参数！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_AnalogQuantitySortLimitParam://查询模拟量分类限值参数
                            Share.Instance.LogInfo("查询模拟量分类限值参数！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_LuminometerParam://查询照度计运行参数
                            Share.Instance.LogInfo("查询照度计运行参数！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_SetParameter_MasterStationDomainAndPort://主站IP和端口查询
                            Share.Instance.LogInfo("主站IP和端口查询！ID：" + key);
                            break;
                    }

                    switch (package.Fn)
                    {
                        //由于两个怎么控制参数使用同一种类型，且内容无法区分，因为在数据数组前添加一位用来区别//0单灯，１单灯组
                        case XNCmdWordConst.Fn_SetParameter_CTULampGrpAutoCtl:// 查询控制器、单灯组自动控制参数
                            Share.Instance.LogInfo("查询控制器、单灯组自动控制参数！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_SetParameter_LampAutoCtlParm://单灯自动控制参数
                            Share.Instance.LogInfo("单灯自动控制参数查询！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_SetParameter_LampGrpParam://单灯分组参数
                            Share.Instance.LogInfo("单灯分组参数！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_SetParameter_RTUAndLampCtrlRelationParam://查询单灯控件器与灯具关系参数
                            Share.Instance.LogInfo("查询单灯控件器与灯具关系参数！ID：" + key);
                            break;
                    }
                    break;
                #endregion
                #region AFN=0C

                case XNCmdWordConst.AFN_RequestRealTimeData_FN://实时数据查询，由于一个数据包内部可以包含多个指令，采取统一循环解析的方式
                    #region FN
                    switch (package.Fn)
                    {
                        case XNCmdWordConst.Fn_RequestRealTimeData_Voltage://电压
                            Share.Instance.LogInfo("电压查询！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_RequestRealTimeData_Current://电流
                            Share.Instance.LogInfo("电流查询！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_RequestRealTimeData_ActivePower://有功功率
                            Share.Instance.LogInfo("有功功率查询！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_RequestRealTimeData_PowerFact://功率因素
                            Share.Instance.LogInfo("功率因素查询！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_RequestRealTimeData_AppPower://视在功率
                            Share.Instance.LogInfo("视在功率查询！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestRealTimeData_ReactivePower://无功功率
                            Share.Instance.LogInfo("无功功率查询！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestRealTimeData_QueryLampPlusStatus://单灯运行状态
                            Share.Instance.LogInfo("单灯运行状态查询！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestRealTimeData_SmartMeterElectricity://电能量查询
                            Share.Instance.LogInfo("电能量查询查询！ID：" + key);
                            break;
                    }
                    #endregion
                    #region PNFN
                    switch (package.PnFn)
                    {
                        case XNCmdWordConst.PnFn_RequestRealTimeData_EnergySavingSwitchGear://节能开关档位
                            Share.Instance.LogInfo("节能开关档位查询！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_RequestRealTimeData_QueryPollLamp://单灯轮询查询
                            Share.Instance.LogInfo("单灯轮询查询查询！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_RequestRealTimeData_QueryNewPTU://发现新单灯控制器查询
                            Share.Instance.LogInfo("发现新单灯控制器查询查询！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_RequestRealTimeData_CTUCalendarClock://CTU时钟 
                            Share.Instance.LogInfo("CTU时钟查询！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_RequestRealTimeData_BatchQueryAnalogQuantity:
                            Share.Instance.LogInfo("模拟量批量查询！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_RequestRealTimeData_BatchQueryRemoteSignal:
                            Share.Instance.LogInfo("遥信量批量查询！ID：" + key);
                            break;
                    }
                    #endregion
                    break;
                #endregion
                #region AFN=0E

                case XNCmdWordConst.AFN_RequestEventData_FN://请求事件
                    Share.Instance.LogInfo("请求事件！ID：" + key);
                    break;
                #endregion
                #region AFN=0D

                case XNCmdWordConst.AFN_RequestHistoryData_FN://请求历史数据
                    #region Fn
                    switch (package.Fn)
                    {
                        case XNCmdWordConst.Fn_RequestHistoryData_CTUAnalogQuantityHisDataChart://控制器模拟量历史数据曲线
                            Share.Instance.LogInfo("控制器模拟量历史数据曲线查询！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_DayFrozenSmartMeter://日电冻结智能电表电量数据
                            Share.Instance.LogInfo("日电冻结智能电表电量数据查询！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampVoltageHisDataChart://单灯电压历史数据曲线
                            Share.Instance.LogInfo("单灯电压历史数据曲线查询！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampActivePowerHisDataChart://单灯有功功率历史数据曲线
                            Share.Instance.LogInfo("单灯有功功率历史数据曲线查询！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampPowerFactHisDataChart://单灯功率因数历史数据曲线
                            Share.Instance.LogInfo("单灯功率因数历史数据曲线查询！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampAppPowerHisDataChart://单灯视在功率历史数据曲线
                            Share.Instance.LogInfo("单灯视在功率历史数据曲线查询！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampReactivePowerHisDataChart://单灯无功功率历史数据曲线
                            Share.Instance.LogInfo("单灯无功功率历史数据曲线查询！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampCurrentHisDataChart://单灯电流历史数据曲线
                            Share.Instance.LogInfo("单灯电流历史数据曲线查询！ID：" + key);
                            break;
                    }
                    #endregion
                    #region PnFn
                    switch (package.PnFn)
                    {
                        case XNCmdWordConst.PnFn_RequestHistoryData_CTUOpenDayLog://控制器日开关灯记录

                            Share.Instance.LogInfo("控制器日开关灯记录查询！ID：" + key);
                            break;
                    }
                    #endregion
                    break;
                #endregion
                #region AFN=10

                case XNCmdWordConst.AFN_ForwardData_FN://数据转发
                    Share.Instance.LogInfo("数据转发！ID：" + key);
                    break;
                    #endregion
            }
        }
    }
}
