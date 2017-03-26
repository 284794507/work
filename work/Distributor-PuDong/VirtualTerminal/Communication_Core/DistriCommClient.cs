using Communication_Core.PackageHandler;
using Communication_Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communication_Core
{
    public partial class DistriCommClient
    {
        private static DistriCommClient _DistriCommClient;
        public static DistriCommClient Instance
        {
            get
            {
                if(_DistriCommClient == null)
                {
                    _DistriCommClient = new DistriCommClient();
                }
                return _DistriCommClient;
            }
        }

        private TcpClient DistriClient;
        private byte[] ReceviceBuffer;
        private DateTime DistriAliveTime;

        public void InitClient()
        {
            Share.Instance.InitConfig();

            DistriClient = new TcpClient();
            ReceviceBuffer = new byte[DistriClient.ReceiveBufferSize];

            Share.Instance.WriteMsg+= Share.Instance.WriteLog;
            Share.Instance.SendToDistriSvr += SendData;
            Share.Instance.MonitorHeartBeat += new Share.MonitorHeartBeatAliveTime(MonitorHearBeatTime);
            Share.Instance.MonitorTcp += new Share.MonitorTcpToDistri(MonitorDevClient);
            TcpConnect();
            Share.Instance.MonitorHeartBeat.BeginInvoke(null, null);
            Share.Instance.MonitorTcp.BeginInvoke(null, null);
        }

        private static Object sendLock = new object();
        public void SendData(XNPackageData package)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("SendData");
                    }))
                   .Do(() =>
                   {
                    lock (sendLock)
                    {
                        if (DistriClient.Connected)
                        {
                            byte[] data = new byte[0];
                            data = package.ToBytes();
                            //DistriClient.GetStream().Write(data, 0, data.Length);
                            DistriClient.GetStream().BeginWrite(data, 0, data.Length, new AsyncCallback(DistriWriteCallBack), DistriClient);
                               DistriClient.GetStream().Flush();
                            DistriAliveTime = DateTime.Now;
                            Share.Instance.WriteMsg("发送数据包成功！" +　 ByteHelper.ByteToHexStrWithDelimiter(data," ",false), 2);
                            //Thread.Sleep(1000);//发送太频繁，设备来不及处理
                        }
                    }
                });
        }

        public void SendData(byte[] data)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("SendData");
                    }))
                   .Do(() =>
                   {
                       lock (sendLock)
                       {
                           if (DistriClient.Connected)
                           {
                               DistriClient.GetStream().BeginWrite(data, 0, data.Length, new AsyncCallback(DistriWriteCallBack), DistriClient);
                               DistriClient.GetStream().Flush();
                               DistriAliveTime = DateTime.Now;
                               Share.Instance.WriteMsg("发送字节数组成功！" +  ByteHelper.ByteToHexStrWithDelimiter(data, " ", false), 2);
                           }
                       }
                   });
        }

        private void MonitorHearBeatTime()
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection,(() => {
                Share.Instance.LogInfo("MonitorHearBeatTime");
                    }))
                   .Do(() =>
                   {
                    while (true)
                    {
                        Thread.Sleep(5000);
                        MonitorHeartBeat();
                    }
                });
        }

        private void MonitorHeartBeat()
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("MonitorHeartBeat");
                    }))
                   .Do(() =>
                   {
                       List<string> ExpiredKeyList = new List<string>();

                       lock (Share.ClientList.SyncRoot)
                       {
                           foreach (string key in Share.ClientList.Keys)
                           {
                               TimeSpan ts = DateTime.Now - ((TerminalClient)Share.ClientList[key]).lastAliveTimeToTerminal;
                               if (ts.TotalMinutes > Share.Instance.TerminalHearBeatTime)
                               {
                                   string delKey = key;
                                   ExpiredKeyList.Add(delKey);
                               }
                           }
                       }

                       foreach (string key in ExpiredKeyList)
                       {
                           Share.Instance.WriteMsg("心跳过期：" + key + " 被删除！", 2);
                           TerminalClient Duplicate_Client = (TerminalClient)Share.ClientList[key];
                           Duplicate_Client.Close();

                           lock (Share.ClientList.SyncRoot)
                           {
                               Share.ClientList.Remove(key);
                               Duplicate_Client = null;
                           }
                       }
                   });
        }

        public void TcpConnect()
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("TcpConnect");
                }))
                .Do(() =>
                {
                    DistriClient = new TcpClient();
                   // DistriClient.ReceiveTimeout = 60;
                    DistriClient.Connect(Share.Instance.DistributorEndPoint);
                    DistriAliveTime = DateTime.Now;
                    Share.Instance.WriteMsg("连接成功", 2);
                    DistriClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, DistriClientRecevice, ReceviceBuffer);
                });
        }

        private void MonitorDevClient()
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("MonitorDevClient");
                }))
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(5000);
                            if (!DistriClient.Connected)
                            {
                                TcpConnect();
                            }
                        }
                    }
                    );
                });
        }


        private void DistriWriteCallBack(IAsyncResult ar)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("DistriWriteCallBack");
                }))
                .Do(() =>
                {
                    TcpClient DistriClient = ar.AsyncState as TcpClient;
                    DistriClient.GetStream().EndWrite(ar);
                });
        }

        public void DistriClientRecevice(IAsyncResult ar)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("DistriClientRecevice");
                }))
                .Do(() =>
                {
                    if (DistriClient.Connected)
                    {
                        NetworkStream ns = DistriClient.GetStream();

                        //已经关闭的流就退出
                        if (!DistriClient.GetStream().CanRead)
                        {
                            return;
                        }
                        int len = 0;
                        try
                        {
                            len = ns.EndRead(ar);
                        }
                        catch
                        {
                            len = 0;
                        }
                        if (len == 0) return;

                        byte[] buffer = (byte[])ar.AsyncState;
                        byte[] receviceData = new byte[len];
                        Buffer.BlockCopy(buffer, 0, receviceData, 0, len);
                        DistriAliveTime = DateTime.Now;
                        AnalyProtocol_Distri(receviceData);

                        DistriClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, DistriClientRecevice, ReceviceBuffer);
                    }
                });
        }

        public void AnalyProtocol_Distri(byte[] data)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("AnalyProtocol_Distri");
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
                        {
                            string key = BitConverter.ToString(package.Address);
                            if (Share.Instance.AllowDevList == null || Share.Instance.AllowDevList.Count() == 0 || Share.Instance.AllowDevList.Contains(key))
                            {//只转发被允许的箱
                                Share.Instance.WriteMsg("Recevice data from Distri:" + ByteHelper.byteToHexStr_Blank(data), 2);
                                DistriPackageHandler(package,data);
                            }
                        }
                        else
                        {
                            Share.Instance.WriteMsg("Analy data fail:" + ByteHelper.byteToHexStr_Blank(data) + msg.ToString(), 4);
                        }
                    //}
                    //while (afterPackage != null);
                });
        }

        public void DistriPackageHandler(XNPackageData package, byte[] data)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("DistriPackageHandler");
                    }))
                   .Do(() =>
                   {

                       string key = BitConverter.ToString(package.Address);
                       if (Share.Instance.AllowDevList==null || Share.Instance.AllowDevList.Count()==0 || Share.Instance.AllowDevList.Contains(key))
                       {//只转发被允许的箱
                           TerminalClient TClient = null;
                           switch (package.AFN_FC)
                           {
                               case XNCmdWordConst.AFN_LinkDetection_FN:
                                   string pnfn = package.PnFn;
                                   if (pnfn == XNCmdWordConst.PnFn_LinkDetection_Login)
                                   {
                                       if (Share.ClientList.ContainsKey(key))
                                       {
                                           //关闭原来的client，并删除列表后，再添加。
                                           TerminalClient Duplicate_Client = (TerminalClient)Share.ClientList[key];
                                           Duplicate_Client.Close();

                                           lock (Share.ClientList.SyncRoot)
                                           {
                                               Share.ClientList.Remove(key);
                                               Duplicate_Client = null; 
                                           }
                                           Share.Instance.LogInfo("设备重复登录！ID：" + key);
                                       }
                                       TClient = new TerminalClient();
                                       TClient.loginTime = TClient.lastAliveTimeToTerminal = DateTime.Now;
                                       Share.ClientList.Add(key, TClient);
                                       Share.Instance.LogInfo("设备登录成功！ID：" + key);
                                       TClient.netStream.BeginRead(TClient.readBytes, 0, TClient.readBytes.Length, PlatFormRecevice, TClient);
                                   }
                                   break;
                           }
                           AnalyDevPackage(package);

                           if (Share.ClientList.ContainsKey(key))
                           {
                               //关闭原来的client，并删除列表后，再添加。
                               TClient = (TerminalClient)Share.ClientList[key];
                               TClient.lastAliveTimeToTerminal = DateTime.Now;
                           }
                           if (TClient != null)
                           {
                               SendDataToPlatForm(TClient, data);
                           }
                       }
                   });
        }

        //发送消息到平台
        private void SendDataToPlatForm(TerminalClient client,byte[] data)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                
                AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                    Share.Instance.LogInfo("DistriWriteCallBack");
                    }))
                     .Do(() =>
                     {
                         client.ReConnectPlatForm();
                         SendToPlatForm(client,data);
                     });
                }))
                .Log(Share.Instance.LogInfo, "SendDataToPlatForm", "")
                .Do(() =>
                {
                    SendToPlatForm(client, data);
                });
        }

        private void SendToPlatForm(TerminalClient client, byte[] data)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("SendToPlatForm");
                    }))
                     .Do(() =>
                     {
                         int len = data.Length;
                         client.writeBytes = new byte[len];
                         Buffer.BlockCopy(data, 0, client.writeBytes, 0, len);
                         client.netStream.BeginWrite(client.writeBytes, 0, len, new AsyncCallback(WriteCallBack), client);
                         Share.Instance.WriteMsg("Send data to PlatForm:" + ByteHelper.byteToHexStr_Blank(client.writeBytes), 4);
                         client.netStream.Flush();
                     });
        }

        private void WriteCallBack(IAsyncResult ar)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("WriteCallBack");
                }))
                .Do(() =>
                {
                    TerminalClient TClient = ar.AsyncState as TerminalClient;
                    TClient.netStream.EndWrite(ar);
                });
        }

        private void AnalyDevPackage(XNPackageData package)
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
                #region AFN=02

                case XNCmdWordConst.AFN_LinkDetection_FN://链路接口检测
                    switch (package.PnFn)
                    {
                        case XNCmdWordConst.PnFn_LinkDetection_Login://登录
                            Share.Instance.LogInfo("登录！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_LinkDetection_HeartBeat://心跳
                            Share.Instance.LogInfo("心跳！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_LinkDetection_Logout://登出
                            Share.Instance.LogInfo("登出！ID：" + key);
                            break;
                    }
                    break;
                    #endregion
                    #region AFN=09
                    
                case XNCmdWordConst.AFN_RequestCTUConfigAndInfo_FN://终端配置及信息
                    Share.Instance.LogInfo("终端配置及信息查询返回！ID：" + key);
                    break;
                #endregion
                #region AFN=0A

                case XNCmdWordConst.AFN_QueryParameter_FN://参数查询指令
                    switch (package.PnFn)
                    {
                        case XNCmdWordConst.PnFn_QueryParameter_SetCTUUpCommParam://查询终端上行通信口通信参数设置
                            Share.Instance.LogInfo("查询终端上行通信口通信参数设置返回！ID：" + key);
                            break;
                        case XNCmdWordConst.PnFn_QueryParameter_SetCTUEventLogConfig://终端事件记录配置查询
                            Share.Instance.LogInfo("终端事件记录配置查询返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_EquipmentBasicInfo://基本设备信息查询
                            Share.Instance.LogInfo("基本设备信息查询返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_MasterStationIPAndPort://主站IP和端口查询
                            Share.Instance.LogInfo("主站IP和端口查询返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_CTUEventDetectConfigParam://查询终端事件检测配置参数
                            Share.Instance.LogInfo("/查询终端事件检测配置参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_LampEventDetectConfigParam://查询单灯事件检测配置参数
                            Share.Instance.LogInfo("查询单灯事件检测配置参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_CTUCascadeCommParam://查询终端级联通信参数
                            Share.Instance.LogInfo("查询终端级联通信参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_ControllerLampOnOffTimeParam://年表
                            Share.Instance.LogInfo("年表查询返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_PowerMeterParam://查询电能表参数
                            Share.Instance.LogInfo("查询电能表参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_DetectInfoIntervalTimeParam://查询信息量检测间隔时间参数
                            Share.Instance.LogInfo("查询信息量检测间隔时间参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_ControlLoopParam://查询控制回路参数
                            Share.Instance.LogInfo("查询控制回路参数返回！ID：" + key);
                            break;
                        
                        case XNCmdWordConst.PnFn_QueryParameter_SetCollectorAndLampCtrlRelationParam://查询集中器与单灯控制器关系参数
                            Share.Instance.LogInfo("查询集中器与单灯控制器关系参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_RTUAndLampCtrlRelationParam://查询单灯控件器与灯具关系参数
                            Share.Instance.LogInfo("查询单灯控件器与灯具关系参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_RemoteSignalSortParam://查询遥信量分类参数
                            Share.Instance.LogInfo("查询遥信量分类参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_AnalogQuantitySortLimitParam://查询模拟量分类限值参数
                            Share.Instance.LogInfo("查询模拟量分类限值参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_LuminometerParam://查询照度计运行参数
                            Share.Instance.LogInfo("查询照度计运行参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_QueryParameter_MasterStationDomainAndPort://主站IP和端口查询
                            Share.Instance.LogInfo("主站IP和端口查询返回！ID：" + key);
                            break;
                    }

                    switch (package.Fn)
                    {
                        //由于两个怎么控制参数使用同一种类型，且内容无法区分，因为在数据数组前添加一位用来区别//0单灯，１单灯组
                        case XNCmdWordConst.Fn_QueryParameter_CTULampGrpAutoCtl:// 查询控制器、单灯组自动控制参数
                            Share.Instance.LogInfo("查询控制器、单灯组自动控制参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_QueryParameter_LampAutoCtlParm://单灯自动控制参数
                            Share.Instance.LogInfo("单灯自动控制参数查询返回！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_QueryParameter_LampGrpParam://单灯分组参数
                            Share.Instance.LogInfo("单灯分组参数返回！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_QueryParameter_RTUAndLampCtrlRelationParam://查询单灯控件器与灯具关系参数
                            Share.Instance.LogInfo("查询单灯控件器与灯具关系参数返回！ID：" + key);
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
                            Share.Instance.LogInfo("电压回复！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_RequestRealTimeData_Current://电流
                            Share.Instance.LogInfo("电流回复！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_RequestRealTimeData_ActivePower://有功功率
                            Share.Instance.LogInfo("有功功率回复！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_RequestRealTimeData_PowerFact://功率因素
                            Share.Instance.LogInfo("功率因素回复！ID：" + key);
                            break;

                        case XNCmdWordConst.Fn_RequestRealTimeData_AppPower://视在功率
                            Share.Instance.LogInfo("视在功率回复！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestRealTimeData_ReactivePower://无功功率
                            Share.Instance.LogInfo("无功功率回复！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestRealTimeData_QueryLampPlusStatus://单灯运行状态
                            Share.Instance.LogInfo("单灯运行状态回复！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestRealTimeData_SmartMeterElectricity://电能量查询
                            Share.Instance.LogInfo("电能量查询回复！ID：" + key);
                            break;
                    }
                    #endregion
                    #region PNFN
                    switch (package.PnFn)
                    {
                        case XNCmdWordConst.PnFn_RequestRealTimeData_EnergySavingSwitchGear://节能开关档位
                            Share.Instance.LogInfo("节能开关档位回复！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_RequestRealTimeData_QueryPollLamp://单灯轮询查询
                            Share.Instance.LogInfo("单灯轮询查询回复！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_RequestRealTimeData_QueryNewPTU://发现新单灯控制器查询
                            Share.Instance.LogInfo("发现新单灯控制器查询回复！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_RequestRealTimeData_CTUCalendarClock://CTU时钟 
                            Share.Instance.LogInfo("CTU时钟回复！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_RequestRealTimeData_BatchQueryAnalogQuantity:
                            Share.Instance.LogInfo("模拟量批量查询回复！ID：" + key);
                            break;

                        case XNCmdWordConst.PnFn_RequestRealTimeData_BatchQueryRemoteSignal:
                            Share.Instance.LogInfo("遥信量批量查询回复！ID：" + key);
                            break;
                    }
                    #endregion
                    break;
                #endregion
                #region AFN=0E

                case XNCmdWordConst.AFN_RequestEventData_FN://请求事件
                    Share.Instance.LogInfo("请求事件回复！ID：" + key);
                    break;
                #endregion
                #region AFN=0D

                case XNCmdWordConst.AFN_RequestHistoryData_FN://请求历史数据
                    #region Fn
                    switch (package.Fn)
                    {
                        case XNCmdWordConst.Fn_RequestHistoryData_CTUAnalogQuantityHisDataChart://控制器模拟量历史数据曲线
                            Share.Instance.LogInfo("控制器模拟量历史数据曲线回复！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_DayFrozenSmartMeter://日电冻结智能电表电量数据
                            Share.Instance.LogInfo("日电冻结智能电表电量数据回复！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampVoltageHisDataChart://单灯电压历史数据曲线
                            Share.Instance.LogInfo("单灯电压历史数据曲线回复！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampActivePowerHisDataChart://单灯有功功率历史数据曲线
                            Share.Instance.LogInfo("单灯有功功率历史数据曲线回复！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampPowerFactHisDataChart://单灯功率因数历史数据曲线
                            Share.Instance.LogInfo("单灯功率因数历史数据曲线回复！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampAppPowerHisDataChart://单灯视在功率历史数据曲线
                            Share.Instance.LogInfo("单灯视在功率历史数据曲线回复！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampReactivePowerHisDataChart://单灯无功功率历史数据曲线
                            Share.Instance.LogInfo("单灯无功功率历史数据曲线回复！ID：" + key);
                            break;
                        case XNCmdWordConst.Fn_RequestHistoryData_LampCurrentHisDataChart://单灯电流历史数据曲线
                            Share.Instance.LogInfo("单灯电流历史数据曲线回复！ID：" + key);
                            break;
                    }
                    #endregion
                    #region PnFn
                    switch (package.PnFn)
                    {
                        case XNCmdWordConst.PnFn_RequestHistoryData_CTUOpenDayLog://控制器日开关灯记录

                            Share.Instance.LogInfo("控制器日开关灯记录回复！ID：" + key);
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
