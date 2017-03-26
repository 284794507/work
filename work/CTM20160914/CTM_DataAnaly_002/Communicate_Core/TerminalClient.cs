using Communicate_Core.Business;
using Communicate_Core.Model;
using Communicate_Core.PackageHandler;
using Communicate_Core.Utility;
using CTMDAL.Model;
using DL_LMS_Server.Default.Shared;
using DL_LMS_Server.Service.DataModel.Parameter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communicate_Core
{
    public class TerminalClient: IAppComServiceCallback
    {
        private static TerminalClient _TerminalClient;
        public static TerminalClient Instance
        {
            get
            {
                if(_TerminalClient==null)
                {
                    _TerminalClient = new TerminalClient();
                }
                return _TerminalClient;
            }
        }

        private TcpClient DevClient;
        byte[] ReceviceBuffer;

        public void InitClient()
        {
            Share.Instance.InitConfig();
            DBHandler.Instance.InitData();
            
            DevClient = new TcpClient();
            ReceviceBuffer = new byte[DevClient.ReceiveBufferSize];
            Share.Instance.SendDataToTerminal += SendData;

            Share.Instance.SendOnlyDataToTerminal += SendOnlyData;

            Share.Instance.MacNameToLmsSvr = Share.Instance.GetMacAddress();
            ConnectLmsSvr();

            Share.Instance.InitHandlerFunction();
            Share.Instance.LastElecDataTime = DateTime.MinValue;
            Share.Instance.BatchID = Guid.NewGuid();

            TcpConnect();
            MonitorDevClient();
            MonitorSendToTerminal();
        }

        private static Object sendLock = new object();

        private void SendData(byte[] devAddr,Terminal_PackageData package)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    lock (sendLock)
                    {
                        if (DevClient.Connected)
                        {
                            //byte[] data = new byte[0];
                            //string []routes = DBHandler.Instance.GetRouteByAddr(devAddr);
                            //if(routes.Count()==0)
                            //{
                                //package.SEQ = GetNewSeq(package.SEQ);
                                SendOnlyData(package);
                                Share.Instance.AddSendToTerminal(package,false);
                            //}
                            //else
                            //{//添加静态路由
                            //    byte[] subData= GetRouteData(devAddr, routes, package);
                            //    Terminal_PackageData routePackage = new Terminal_PackageData(package.DevAddr, subData, TerminalCmdWordConst.Retransmission, Share.Instance.GetBSendNo(), TerminalCmdWordConst.Retransmission_FN);
                            //    routePackage.SEQ = GetNewSeq(routePackage.SEQ);
                            //    SendOnlyData(routePackage);
                            //    Share.Instance.AddSendToTerminal(package,true);
                            //}
                        }
                    }
                });
        }

        public void SendOnlyData(Terminal_PackageData package)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    lock (sendLock)
                    {
                        if (DevClient.Connected)
                        {
                            byte[] data = new byte[0];
                            data = package.ToBytes();
                            DevClient.GetStream().Write(data, 0, data.Length);
                            Share.Instance.WriteLog("发送成功！" + "\r\n" + ByteHelper.ByteToHexStrWithDelimiter(data));
                            //Share.Instance.WriteLog(ByteHelper.ByteToHexStrWithDelimiter(data), 2);
                            Thread.Sleep(1000);//发送太频繁，设备来不及处理
                        }
                    }
                });
        }

        private byte[] GetRouteData(byte[] devAddr, string[] routes, Terminal_PackageData subPkg)
        {
            byte[] result = new byte[0];
            AspectF.Define.Retry(Share.Instance.ExceptionHandler, () => { Thread.Sleep(5000); ConnectLmsSvr(); })
                .Log(Share.Instance.WriteLog, "GetRouteData", "")
                      .Do(() =>
                      {
                          using (MemoryStream mem = new MemoryStream())
                          {
                              BinaryWriter writer = new BinaryWriter(mem);
                              writer.Write(devAddr);
                              writer.Write(subPkg.DevAddr);
                              byte len = (byte)routes.Length;
                              writer.Write(len);
                              writer.Write(0x00);
                              for(byte i=0;i<len;i++)
                              {
                                  byte[] routeAddr = ByteHelper.HexStrToByteArrayWithDelimiter(routes[i], "-");
                                  writer.Write(routeAddr);
                              }
                              writer.Write(subPkg.CmdWord);
                              writer.Write(subPkg.FrameNum);
                              writer.Write(subPkg.SEQ);
                              writer.Write(subPkg.FN);
                              if (subPkg.OnlyData != null && subPkg.OnlyData.Length != 0)
                              {
                                  writer.Write(subPkg.OnlyData);
                              }
                              if (subPkg.TP != null && subPkg.TP.Length != 0)
                              {
                                  writer.Write(subPkg.TP);
                              }

                              result = mem.ToArray();
                              writer.Close();
                          }
                      });

            return result;
        }

        private void MonitorSendToTerminal()
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler, () => { MonitorSendToTerminal(); })
                .Log(Share.Instance.WriteLog, "MonitorSendToTerminal", "")
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(1000);
                            byte[] bSeq = Share.Instance.dictSendToTerminal.Keys.ToArray();
                            int len = bSeq.Length;
                            for(int i=0;i<len;i++)
                            {
                                byte seq = bSeq[i];
                                SendToTerminalInfo info = Share.Instance.dictSendToTerminal[seq];
                                TimeSpan ts =DateTime.Now- info.SendTime;
                                if((ts.Seconds>4 && info.isRetransmission==false) || (info.isRetransmission==true && ts.Seconds>10))
                                {
                                    if (info.SendNum >= 3)
                                    {
                                        Share.Instance.dictSendToTerminal.Remove(seq);
                                    }
                                    else
                                    {
                                        info.SendNum++;
                                        info.SendTime = DateTime.Now;
                                        string key = ByteHelper.ByteToHexStrWithDelimiter(info.pkg.DevAddr, "-");
                                        if (Share.Instance.ClientList.ContainsKey(key))
                                        {
                                            DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                                            Terminal_PackageData SendPkg = new Terminal_PackageData(info.pkg.DevAddr, info.pkg.OnlyData, info.pkg.CmdWord, curClient.GetSendNo, info.pkg.SEQ, info.pkg.FN);
                                            SendData(info.pkg.DevAddr, SendPkg);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    );
                });
        }
        /// <summary>
        /// 连接到服务端
        /// </summary>
        public void ConnectLmsSvr()
        {
            ThreadPool.QueueUserWorkItem
              (
                  delegate
                  {
                      AspectF.Define.Retry(Share.Instance.ExceptionHandler,()=> { Thread.Sleep(5000); ConnectLmsSvr(); })
                        .Log(Share.Instance.WriteLog, "ConnectLmsSvr", "")
                      .Do(() =>
                      {
                          CreateSvrChannel();
                          SvrRetMessage _connectResult = Share.Instance.proxy.Connect(Share.Instance.MacNameToLmsSvr);

                          if (_connectResult.ExcuResult)
                          {
                              Share.Instance.WriteLog("Connected to LMSSvr！");
                              MonitorClientChanel();
                          }
                      });
                  }
              );
        }
        
        /// <summary>
        /// 建立实例通道
        /// </summary>
        private void CreateSvrChannel()
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    InstanceContext site = new InstanceContext(this);
                    Share.Instance.proxy = new AppComServiceClient(site);
                });
        }

        /// <summary>
        /// 初始配置获得通道实例
        /// </summary>
        public IAppComService SvrObject
        {
            get
            {
                if (Share.Instance.proxy == null)
                    CreateSvrChannel();

                return Share.Instance.proxy;
            }
        }

        #region 心跳包

        /// <summary>
        /// 维持和服务端的心跳；断线重连
        /// </summary>
        private void MonitorClientChanel()
        {
            ThreadPool.QueueUserWorkItem
            (
                delegate
                {
                    while (true)
                    {
                        Thread.Sleep(5000);
                        AspectF.Define.Retry(Share.Instance.ExceptionHandler,()=> { CreateSvrChannel(); })
                            .Do(() =>
                            {
                                if (Share.Instance.GetObjetcStatus.State == CommunicationState.Faulted)
                                {
                                    Share.Instance.GetObjetcStatus.Abort();
                                    CreateSvrChannel();
                                    Share.Instance.WriteLog(" 心跳连接抛出异常CommunicationState.Faulted！", 4);
                                }

                                SvrObject.ReConnect(Share.Instance.MacNameToLmsSvr, "", "");
                            });
                    }
                }
            );
        }

        private delegate void HandlerLmsSvrSendInfo(CommonBusinessParameter parameter);

        #endregion 心跳包

        public void CommBusinessSendToEquip(CommonBusinessParameter parameter)
        {
            HandlerLmsSvrSendInfo handler = new HandlerLmsSvrSendInfo(HandlerLmsSvrMsg);
            handler.BeginInvoke(parameter, null, null);
        }

        /// <summary>
        /// 处理WCF请求
        /// </summary>
        /// <param name="parameter"></param>
        private void HandlerLmsSvrMsg(CommonBusinessParameter parameter)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                   .Do(() =>
                   {
                       if (CheckLmsSvrCmd())
                       {
                           switch (parameter.BusinessType)
                           {
                               case CommonBusinessType.PLDevElecQuery:
                               //case CommonBusinessType.PLListReGet:
                                   LmsSvrHandler.Instance.LmsSvrQueryElecData(parameter);
                                   break;
                               case CommonBusinessType.PLDevRestart://重启
                                   LmsSvrHandler.Instance.LmsSvrRestartHandler(parameter);
                                   break;                               
                               case CommonBusinessType.PLCodeModify://台区号修改
                                   LmsSvrHandler.Instance.PLCodeModifyHandler(parameter);
                                   break;
                               case CommonBusinessType.PLCodeQuery://台区号查询
                                   LmsSvrHandler.Instance.PLCodeQueryHandler(parameter);
                                   break;
                               case CommonBusinessType.PLDevInstallationStatus_Set://安装信息修改
                                   LmsSvrHandler.Instance.PLSetInstallStatusHandler(parameter);
                                   break;
                               case CommonBusinessType.PLDevInstallationStatus_Query://安装信息查询
                                   LmsSvrHandler.Instance.PLQueryInstallStatusHandler(parameter);
                                   break;
                               case CommonBusinessType.PLDevLoraCfg_Set://lora模组设置
                                   LmsSvrHandler.Instance.SetLoraCfg(parameter);
                                   break;
                               case CommonBusinessType.PLDevLoraCfg_Qurey://lora模组查询
                                   LmsSvrHandler.Instance.QueryLoraCfg(parameter);
                                   break;
                               case CommonBusinessType.PLDevInfo_Query://设备信息查询
                                   LmsSvrHandler.Instance.QueryDevInfo(parameter);
                                   break;
                               case CommonBusinessType.PLDevCommInfo_Query://通信信息查询
                                   LmsSvrHandler.Instance.QueryCommInfo(parameter);
                                   break;
                               case CommonBusinessType.PLDevUpgrade_InitInfo://初始化升级
                                   LmsSvrHandler.Instance.PLDevUpgrade_InitInfoHandler(parameter);
                                   break;
                               case CommonBusinessType.PLDevUpgrade_ConfirmAndRetry://续传
                                   LmsSvrHandler.Instance.PLDevUpgrade_ConfirmAndRetryHandler(parameter);
                                   break;
                               case CommonBusinessType.PLDevUpgrade_Cancel://取消升级
                                   LmsSvrHandler.Instance.PLDevUpgrade_CancelHandler(parameter);
                                   break;
                               case CommonBusinessType.PLRouterCFG_Set://静态路由设置
                                   LmsSvrHandler.Instance.PLRouterCFG_SetHandler(parameter);
                                   break;
                           }
                       }
                   });
        }

        /// <summary>
        /// 接收平台指令时间间隔有要求
        /// </summary>
        /// <returns></returns>
        private bool CheckLmsSvrCmd()
        {
            bool result = false;

            TimeSpan ts =DateTime.Now -Share.Instance.ReceviceTime;
            if(ts.Seconds>Share.Instance.LmsSvrInterval)
            {
                result = true;
                Share.Instance.ReceviceTime = DateTime.Now;
            }

            return result;
        }

        public void HandlerMessage(string message)
        {
            //throw new NotImplementedException();
        }

        public void TcpConnect()
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    DevClient = new TcpClient();
                    DevClient.Connect(Share.Instance.CTM_Route_IP, Share.Instance.CTM_Route_Port);
                    Share.Instance.WriteLog("连接成功");
                    DevClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, DevClientRecevice, ReceviceBuffer);
                });
        }

        /// <summary>
        /// 接收TCP数据
        /// </summary>
        /// <param name="ar"></param>
        public void DevClientRecevice(IAsyncResult ar)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    if (DevClient.Connected)
                    {
                        NetworkStream ns = DevClient.GetStream();
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

                        AnalyProtocol_Dev(receviceData);

                        DevClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, DevClientRecevice, ReceviceBuffer);
                    }
                });
        }

        /// <summary>
        /// 解析报文
        /// </summary>
        /// <param name="data"></param>
        public void AnalyProtocol_Dev(byte[] data)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    byte[] afterData = null;
                    Terminal_PackageData package = new Terminal_PackageData();

                    do
                    {
                        if (afterData == null)
                        {
                            package.BuildPackageFromBytes(data);
                        }
                        else
                        {
                            package.BuildPackageFromBytes(afterData);
                        }

                        string msg = "";
                        if (package.AnalySuccess)
                        {
                            msg = "接收正确的报文" + "\r\n";
                            msg += "命令字：" + BitConverter.ToString(package.CmdWord) + "\r\n";
                        }
                        else
                        {
                            msg = "接收错误的报文" + "\r\n";
                        }
                        if (afterData == null)
                        {
                            msg += ByteHelper.ByteToHexStrWithDelimiter(package.ToBytes());
                        }
                        else
                        {
                            msg += ByteHelper.ByteToHexStrWithDelimiter(afterData);
                        }
                        Share.Instance.WriteLog(msg);
                        if (package.AnalySuccess)
                        {
                            string key = ByteHelper.ByteToHexStrWithDelimiter(package.CmdWord);
                            key = Share.Instance.GetKey(package.CmdWord, package.FN);
                            string addr = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, "-");
                            if (Share.Instance.dictAllHandlerFun.ContainsKey(key))
                            {
                                string keyLogin = Share.Instance.GetKey(TerminalCmdWordConst.GetLoginOrHeartBeat, TerminalCmdWordConst.LoginON_FN);
                                if (key == keyLogin || Share.Instance.ClientList.ContainsKey(addr))
                                {
                                    byte seq = package.SEQ;
                                    seq = ByteHelper.ClearBit(seq, 7);
                                    seq = ByteHelper.ClearBit(seq, 6);
                                    Share.Instance.CheckSendToTerminal(seq);//收到回复，则删除发送记录
                                    Share.Instance.dictAllHandlerFun[key](package);
                                }
                            }
                        }
                        afterData = package.afterPackage;
                    }
                    while (afterData != null);
                });
        }

        private void MonitorDevClient()
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler,()=> { MonitorDevClient(); })
                .Log(Share.Instance.WriteLog, "MonitorDevClient", "")
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(5000);
                            if (!DevClient.Connected)
                            {
                                TcpConnect();
                            }
                        }
                    }
                    );
                });
        }
    }
}
