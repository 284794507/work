using Communicate_Core.Model;
using Communicate_Core.PackageHandler;
using Communicate_Core.Utility;
using CTMDAL.Model;
using DL_LMS_Server.Service.DataModel.Config;
using DL_LMS_Server.Service.DataModel.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communicate_Core.Business
{
    public class LmsSvrHandler
    {
        private static LmsSvrHandler _LmsSvrHandler;
        public static LmsSvrHandler Instance
        {
            get
            {
                if(_LmsSvrHandler==null)
                {
                    _LmsSvrHandler = new LmsSvrHandler();
                }
                return _LmsSvrHandler;
            }
        }


        public void LmsSvrQueryElecData(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    string addr = para.BusinessObject.ToString();
                    ElecDataHandler.Instance.SendQueryElecDataPackage(addr);
                });
        }

        private string[] GetNewDevOrder(string[] arr)
        {
            int len = arr.Length;
            if (len <= 1) return arr;
            string[] result = new string[len];

            int startIndex = 0;
            int endIndex = len - 1;
            for (int i = 0; i < len; i++)
            {
                foreach (DevClient dev in Share.Instance.ClientList.Values)
                {
                    byte devType = Share.Instance.GetDevTypeByAddr(arr[i]);
                    if (devType == 0x01 || devType == 0x03)
                    {
                        result[endIndex--] = arr[i];
                    }
                    else
                    {
                        result[startIndex++] = arr[i];
                    }
                }
            }
            return result;
        }
        
        /// <summary>
        /// 测试有没有相同的指令，如果没有则发查询指令
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="devAddr"></param>
        /// <returns></returns>
        private bool CheckRepeatCmd(CommonBusinessParameter para)
        {
            if (Share.Instance.dictQueryFromLMSSvr == null)
            {
                Share.Instance.dictQueryFromLMSSvr = new Dictionary<int, CommonBusinessParameter>();
            }
            bool isRepeat = false;
            foreach (CommonBusinessParameter p in Share.Instance.dictQueryFromLMSSvr.Values)
            {
                if(para.BusinessType==p.BusinessType && para.CabID==p.CabID && para.BusinessSendValue==p.BusinessSendValue)
                {
                    switch (para.BusinessType)
                    {
                        case CommonBusinessType.PLDevRestart://重启设备
                            if(para.BusinessSendValue==1)
                            {
                                isRepeat = true;
                            }
                            else
                            {
                                string[] paraArr = para.BusinessObject as string[];
                                string[] pArr = p.BusinessObject as string[];
                                isRepeat = stringArrarEqual(paraArr, pArr);
                            }
                            break;
                        case CommonBusinessType.PLCodeModify:
                            break;
                    }
                }
            }
            return true;
        }

        public bool stringArrarEqual(string []arr1,string []arr2)
        {
            bool result = false;
            int len1 = arr1.Length;
            int len2 = arr2.Length;
            if(len1 == len2)
            {
                for(int i=0;i< len1;i++)
                {
                    if(arr1[i]!=arr2[i])
                    {
                        break;
                    }
                }
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 发送请求到设备
        /// </summary>
        /// <param name="connAddr"></param>
        /// <param name="devAddr"></param>
        /// <param name="businessType"></param>
        private void SendPackgeToDev(byte[] connAddr, byte[] data, CommonBusinessParameter param)
        {
            //switch (param.BusinessType)
            //{
            //    case CommonBusinessType.PLDevElecQuery://电参数
            //        QueryDevDataRecHandler.GetHandler.SendQueryDevDataRec(connAddr, data);
            //        break;
            //    case CommonBusinessType.PLDevRestart://重启设备
            //        ResetDevHandler.GetHandler.SendResetDevPackage(connAddr, data);
            //        break;
            //    case CommonBusinessType.PLListReGet://重新获取台区列表
            //        string addr = ByteHelper.ByteToHexStrWithDelimiter(data, "-");
            //        string plcAddr = DBHandler.GetHandler.GetPlcAddrByDevAddr(addr);
            //        if (plcAddr == "")
            //        {
            //            plcAddr = "FF-FF-FF-FF-FF-FF-FF-FF";
            //        }
            //        data = ByteHelper.HexStrToByteArrayWithDelimiter(plcAddr, "-");
            //        ReacquirePlatFormHandler.GetHandler.SendReacquirePlatFormToDev(connAddr, data);
            //        break;
            //    case CommonBusinessType.PLCodeModify:// 台区号设定
            //        SetPlatFormCodeHandler.GetHandler.SendSetPlatFormCode(connAddr, data);
            //        break;
            //    case CommonBusinessType.PLDevUpgrade_InitInfo:// 初始化远程升级
            //        RemoteUpgradeHandler.GetHandler.SendRemoteUpgradePackage(connAddr, data);
            //        Share.Instance.SendReceviceMsgToLmsSvr(param.CabID, param.BusinessType);
            //        break;
            //    case CommonBusinessType.PLDevUpgrade_Cancel:// 取消远程升级

            //        RemoteUpgradeHandler.GetHandler.SendRemoteUpgradePackage(connAddr, data);
            //        Share.Instance.SendReceviceMsgToLmsSvr(param.CabID, param.BusinessType);
            //        //RemoteUpgradeHandler.GetHandler.CancelUpgrade(param.CtuID);
            //        break;
            //}
        }

        public void LmsSvrRestartHandler(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    byte[] devAddr = new byte[8];
                    if (para.BusinessSendValue == 1)//复位所有
                    {
                        string platFormID = para.CabID;
                        string[] gprsAddr = Share.Instance.GetAllGprsByPlatFormID(platFormID);
                        for (int i = 0; i < gprsAddr.Length; i++)
                        {
                            if (Share.Instance.checkDevIsLoginOrNot(gprsAddr[i]))
                            {
                                DevClient curClient = Share.Instance.ClientList[gprsAddr[i]] as DevClient;

                                Terminal_PackageData SendPkg = new Terminal_PackageData(curClient.bAddr, Share.Instance.BroadcastAddr, TerminalCmdWordConst.Reset, curClient.GetSendNo, TerminalCmdWordConst.TerminalReset_FN);
                                Share.Instance.SendOnlyDataToTerminal(SendPkg);
                            }
                        }
                    }
                    else
                    {
                        string[] tempAddr = para.BusinessObject as string[];
                        string[] addr = GetNewDevOrder(tempAddr);
                        for (int i = 0; i < addr.Length; i++)
                        {
                            devAddr = ByteHelper.HexStrToByteArrayWithDelimiter(addr[i], "-", false);
                            byte[] bAddr = Share.Instance.GetGPRSAddrByAddr(addr[i]);
                            string gprsAddr = ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-");
                            if (Share.Instance.checkDevIsLoginOrNot(gprsAddr))
                            {
                                DevClient curClient = Share.Instance.ClientList[gprsAddr] as DevClient;

                                Terminal_PackageData SendPkg = new Terminal_PackageData(curClient.bAddr, devAddr, TerminalCmdWordConst.Reset, curClient.GetSendNo, TerminalCmdWordConst.TerminalReset_FN);
                                Share.Instance.SendDataToTerminal(devAddr, SendPkg);
                            }
                        }
                    }
                });
        }

        /// <summary>
        /// 台区号设定
        /// </summary>
        /// <param name="parameter"></param>
        public void PLCodeModifyHandler(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    PL_PLInfo info = para.BusinessObject as PL_PLInfo;
                   
                    byte[] addr = Share.Instance.GetGPRSAddrByAddr(info.PlcDevAddr);
                    string gprsAddr = ByteHelper.ByteToHexStrWithDelimiter(addr, "-");
                    if (Share.Instance.checkDevIsLoginOrNot(gprsAddr))
                    {
                        DevClient curClient = Share.Instance.ClientList[gprsAddr] as DevClient;
                        
                        PlatFormHandler.Instance.SendSetPlatFormPackage(curClient, info);
                    }
                });
        }

        /// <summary>
        /// 台区号查询
        /// </summary>
        /// <param name="para"></param>
        public void PLCodeQueryHandler(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    PL_PLInfo info = para.BusinessObject as PL_PLInfo;

                    byte[] addr = Share.Instance.GetGPRSAddrByAddr(info.PlcDevAddr);
                    string gprsAddr = ByteHelper.ByteToHexStrWithDelimiter(addr, "-");
                    if (Share.Instance.checkDevIsLoginOrNot(gprsAddr))
                    {
                        DevClient curClient = Share.Instance.ClientList[gprsAddr] as DevClient;

                        PlatFormHandler.Instance.SendQueryPlatFormPackage(curClient, info);
                    }
                });
        }

        /// <summary>
        /// 安装状态设定
        /// </summary>
        /// <param name="parameter"></param>
        public void PLSetInstallStatusHandler(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    string devAddr = para.BusinessObject.ToString();
                    byte[] addr = Share.Instance.GetGPRSAddrByAddr(devAddr);
                    string gprsAddr = ByteHelper.ByteToHexStrWithDelimiter(addr, "-");
                    if (Share.Instance.checkDevIsLoginOrNot(gprsAddr))
                    {
                        DevClient curClient = Share.Instance.ClientList[gprsAddr] as DevClient;

                        TerminalInstallStatus.Instance.SendSetInstallStatusPackage(curClient, devAddr,para.BusinessSendValue);
                    }
                });
        }

        /// <summary>
        /// 安装状态查询
        /// </summary>
        /// <param name="para"></param>
        public void PLQueryInstallStatusHandler(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    string devAddr = para.BusinessObject.ToString();
                    byte[] addr = Share.Instance.GetGPRSAddrByAddr(devAddr);
                    string gprsAddr = ByteHelper.ByteToHexStrWithDelimiter(addr, "-");
                    if (Share.Instance.checkDevIsLoginOrNot(gprsAddr))
                    {
                        DevClient curClient = Share.Instance.ClientList[gprsAddr] as DevClient;

                        TerminalInstallStatus.Instance.SendQueryInstallStatusPackage(curClient, devAddr);
                    }
                });
        }

        /// <summary>
        /// LORA模组信息设置
        /// </summary>
        /// <param name="para"></param>
        public void SetLoraCfg(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    PLLoraCfgInfo[] loraArr=para.BusinessObject as PLLoraCfgInfo[];
                    LoraHandler.Instance.SendSetLoraPackage(loraArr);
                });
        }

        /// <summary>
        /// LORA模组信息查询
        /// </summary>
        /// <param name="para"></param>
        public void QueryLoraCfg(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    string[] loraArr = para.BusinessObject as string[];
                    LoraHandler.Instance.SendQueryLoraPackage(loraArr);
                });
        }

        /// <summary>
        /// 设备信息查询
        /// </summary>
        /// <param name="para"></param>
        public void QueryDevInfo(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    string[] addr = para.BusinessObject as string[];
                    TerminalInfoHandler.Instance.SendQueryTerminalInfo(addr);
                });
        }

        /// <summary>
        /// 设备通信信息查询
        /// </summary>
        /// <param name="para"></param>
        public void QueryCommInfo(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    string[] addr = para.BusinessObject as string[];
                    CommStatusHandler.Instance.SendQueryCommStatus(addr);
                });
        }

        /// <summary>
        /// 初始化远程升级
        /// </summary>
        /// <param name="para"></param>
        public void PLDevUpgrade_InitInfoHandler(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "准备升级！")
                .Do(() =>
                {
                    byte[] bGprsAddr = Share.Instance.GetGPRSAddrByAddr(para.CtuID);
                    string gprsID = ByteHelper.ByteToHexStrWithDelimiter(bGprsAddr, "-", false);
                    if (Share.Instance.checkDevIsLoginOrNot(gprsID))
                    {
                        DevClient curClient = Share.Instance.ClientList[gprsID] as DevClient;

                        TPLUpgradeFileInfo curFile = new TPLUpgradeFileInfo();

                        curFile.PLCollectorInfoID = new Guid(Share.Instance.GetDevIDByAddr(para.CtuID));
                        FileMessageParameter fMsg = para.BusinessObject as FileMessageParameter;
                        curFile.FileName = fMsg.FileName;
                        curFile.FileContent = fMsg.FileData;
                        if(para.CtuID==gprsID)
                        {
                            curFile.FilePerSize = Share.Instance.UpgradeGPRSPerSize;
                        }
                        else
                        {
                            curFile.FilePerSize = Share.Instance.UpgradeOtherPerSize;
                        }
                        curFile.FileSoftWareVer = fMsg.FileSoftWareVer;
                        curFile.FileHardWareVer = fMsg.FileHardWareVer;
                        curFile.FileUpLoadTime = fMsg.FileUpLoadTime;

                        DBHandler.Instance.AddUpgradeFile(curFile);

                        RemoteUpgradeHandler.Instance.ReadyToUpgrade(curFile,para.CtuID,curClient);
                    }
                });
        }

        /// <summary>
        /// 继续升级
        /// </summary>
        /// <param name="para"></param>
        public void PLDevUpgrade_ConfirmAndRetryHandler(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    byte[] bGprsAddr = Share.Instance.GetGPRSAddrByAddr(para.CtuID);
                    string gprsID = ByteHelper.ByteToHexStrWithDelimiter(bGprsAddr, "-", false);
                    if (Share.Instance.checkDevIsLoginOrNot(gprsID))
                    {
                        DevClient curClient = Share.Instance.ClientList[gprsID] as DevClient;
                        Share.Instance.WriteLog(" 续传！");
                        //继续升级发确认升级命令
                        RemoteUpgradeHandler.Instance.SendConfirmBag(curClient,para.CtuID);
                    }
                });
        }

        /// <summary>
        /// 取消远程升级
        /// </summary>
        /// <param name="para"></param>
        public void PLDevUpgrade_CancelHandler(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    byte[] bGprsAddr = Share.Instance.GetGPRSAddrByAddr(para.CtuID);
                    string gprsID = ByteHelper.ByteToHexStrWithDelimiter(bGprsAddr, "-", false);
                    if (Share.Instance.checkDevIsLoginOrNot(gprsID))
                    {
                        DevClient curClient = Share.Instance.ClientList[gprsID] as DevClient;
                        RemoteUpgradeHandler.Instance.CancelUpgrade(curClient);
                    }
                });
        }

        public void PLRouterCFG_SetHandler(CommonBusinessParameter para)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    TPLCollectorStaticRoutes route = new TPLCollectorStaticRoutes();
                    PL_DevRouterCfgInfo info = para.BusinessObject as PL_DevRouterCfgInfo;
                    route.DevID = new Guid(info.DevID);
                    route.StaticRouteNode = new Guid(info.RouterDevID);

                    DBHandler.Instance.AddRoute(route); 
                });
        }
    }
}
