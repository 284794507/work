using Communicate_Core.Model;
using Communicate_Core.PackageHandler;
using Communicate_Core.Utility;
using DL_LMS_Server.Default.Shared;
using DL_LMS_Server.Service.DataModel.Config;
using DL_LMS_Server.Service.DataModel.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.Business
{
    public class LoraHandler
    {
        private static LoraHandler _LoraHandler;
        public static LoraHandler Instance
        {
            get
            {
                if(_LoraHandler==null)
                {
                    _LoraHandler = new LoraHandler();
                }
                return _LoraHandler;
            }
        }

        /// <summary>
        /// lora模组信息查询
        /// </summary>
        /// <param name="loraArr"></param>
        public void SendQueryLoraPackage(string[] loraArr)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                   .Log(Share.Instance.WriteLog, "", "Query LORA Info！")
                   .Do(() =>
                   {
                       foreach (string macAddr in loraArr)
                       {
                           byte[] devAddr = ByteHelper.HexStrToByteArrayWithDelimiter(macAddr, "-");
                           byte[] gprsAddr = Share.Instance.GetGPRSAddrByAddr(macAddr);
                           string key = ByteHelper.ByteToHexStrWithDelimiter(gprsAddr, "-");
                           if (Share.Instance.ClientList.ContainsKey(key))
                           {
                               DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                               Terminal_PackageData SendPkg = new Terminal_PackageData(gprsAddr, devAddr, TerminalCmdWordConst.QueryCmd, curClient.GetSendNo, TerminalCmdWordConst.QueryLora_FN);
                               Share.Instance.SendDataToTerminal(devAddr, SendPkg);
                           }
                           else
                           {
                               Share.Instance.GPRSOffLine(key, CommonBusinessType.PLDevLoraCfg_Qurey);
                           }
                               //byte seq = Share.Instance.GetNewSeq(0x80);
                       }
                   });
        }

        /// <summary>
        /// LORA模组信息查询回复
        /// </summary>
        /// <param name="package"></param>
        public void LoraQueryPackageBackHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "Recevice LORA Info！")
                .Do(() =>
                {
                    string key = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, "-");
                    if (Share.Instance.ClientList.ContainsKey(key))
                    {
                        DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                        curClient.HeartBeatTime = DateTime.Now;

                        PLLoraCfgInfo info = new PLLoraCfgInfo();
                        int curIndex = 0;
                        byte[] bAddr = new byte[8];
                        Buffer.BlockCopy(package.OnlyData, curIndex, bAddr, 0, 8);
                        curIndex += 8;
                        string macAddr = ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-");
                        info.DevID = Share.Instance.GetDevIDByAddr(macAddr);
                        info.DevMac = macAddr;
                        info.DevChennel = package.OnlyData[curIndex++];
                        info.TransmittingPower = package.OnlyData[curIndex++];
                        info.TransmittingRate = package.OnlyData[curIndex++];

                        List<PLLoraCfgInfo> list = new List<PLLoraCfgInfo>();
                        list.Add(info);

                        CommonBusinessBackResult optResult = new CommonBusinessBackResult();
                        optResult.ExecuteResult = 1;
                        optResult.BusinessType = CommonBusinessType.PLDevLoraCfg_Qurey;                        
                        optResult.BusinessObject = list.ToArray();
                        SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
                        //
                        if (msg.ExcuResult)
                        {
                            Share.Instance.WriteLog(" LORA模组信息查询返回成功！");
                        }
                    }
                });
        }

        /// <summary>
        /// lora模组信息设置
        /// </summary>
        /// <param name="loraArr"></param>
        public void SendSetLoraPackage(PLLoraCfgInfo[] loraArr)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                   .Log(Share.Instance.WriteLog, "", "Set LORA Info！")
                   .Do(() =>
                   {
                       foreach (PLLoraCfgInfo info in loraArr)
                       {
                           byte[] devAddr = ByteHelper.HexStrToByteArrayWithDelimiter(info.DevMac, "-");
                           byte[] gprsAddr = Share.Instance.GetGPRSAddrByAddr(info.DevMac);
                           string key = ByteHelper.ByteToHexStrWithDelimiter(gprsAddr, "-");
                           if (Share.Instance.ClientList.ContainsKey(key))
                           {
                               DevClient curClient = Share.Instance.ClientList[key] as DevClient;

                               int curIndex = 0;
                               byte[] data = new byte[11];
                               Buffer.BlockCopy(devAddr, 0, data, curIndex, 8);
                               curIndex += 8;
                               data[curIndex++] = (byte)info.DevChennel;
                               data[curIndex++] = (byte)info.TransmittingPower;
                               data[curIndex++] = (byte)info.TransmittingRate;
                               Terminal_PackageData SendPkg = new Terminal_PackageData(gprsAddr, data, TerminalCmdWordConst.SetCmd, curClient.GetSendNo, TerminalCmdWordConst.SetLora_FN);
                               Share.Instance.SendDataToTerminal(devAddr, SendPkg);
                           }
                           else
                           {
                               Share.Instance.GPRSOffLine(key, CommonBusinessType.PLDevLoraCfg_Set);
                           }

                           //byte seq = Share.Instance.GetNewSeq(0x80);
                           
                       }
                   });
        }

        /// <summary>
        /// LORA模组信息设置回复
        /// </summary>
        /// <param name="package"></param>
        public void LoraSetPackageBackHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "Recevice LORA Info！")
                .Do(() =>
                {
                    CommonBusinessBackResult optResult = new CommonBusinessBackResult();
                    optResult.ExecuteResult = 1;
                    optResult.BusinessType = CommonBusinessType.PLDevLoraCfg_Set;
                    optResult.BusinessReturnValue = 1;
                    SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
                    //
                    if (msg.ExcuResult)
                    {
                        Share.Instance.WriteLog(" LORA模组信息设置返回成功！");
                    }
                });
        }
    }
}
