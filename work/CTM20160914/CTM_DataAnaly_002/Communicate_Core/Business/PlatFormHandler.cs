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
    public class PlatFormHandler
    {
        private static PlatFormHandler _PlatFormHandler;
        public static PlatFormHandler Instance
        {
            get
            {
                if(_PlatFormHandler==null)
                {
                    _PlatFormHandler = new PlatFormHandler();
                }
                return _PlatFormHandler;
            }
        }

        /// <summary>
        /// 台区信息查询
        /// </summary>
        /// <param name="client"></param>
        /// <param name="info"></param>
        public void SendQueryPlatFormPackage(DevClient client, PL_PLInfo info)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    byte[] bDevAddr = ByteHelper.HexStrToByteArrayWithDelimiter(info.PlcDevAddr, "-");
                    Terminal_PackageData SendPkg = new Terminal_PackageData(client.bAddr, bDevAddr, TerminalCmdWordConst.QueryCmd, client.GetSendNo, TerminalCmdWordConst.QueryPlatForm_FN);
                    Share.Instance.SendDataToTerminal(bDevAddr, SendPkg);
                });
        }

        /// <summary>
        /// 台区信息查询返回
        /// </summary>
        /// <param name="package"></param>
        public void PlatFormPackageHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "Recevice PlatForm Info！")
                .Do(() =>
                {
                    string key = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, "-");
                    if (Share.Instance.ClientList.ContainsKey(key))
                    {
                        DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                        curClient.HeartBeatTime = DateTime.Now;

                        PL_PLInfo info = new PL_PLInfo();
                        int curIndex = 0;
                        byte[] bAddr = new byte[8];
                        Buffer.BlockCopy(package.OnlyData, curIndex, bAddr, 0, 8);
                        curIndex += 8;
                        info.PlcDevAddr = ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-");
                        info.ContryCode = package.OnlyData[curIndex++].ToString();
                        info.CityCode = BitConverter.ToInt16(package.OnlyData, curIndex).ToString();
                        curIndex += 2;
                        byte[] bArr = new byte[4];
                        Buffer.BlockCopy(package.OnlyData, curIndex, bArr,0, 3);
                        info.PlfCode = BitConverter.ToInt32(bArr, 0).ToString();
                        
                        CommonBusinessBackResult optResult = new CommonBusinessBackResult();
                        optResult.ExecuteResult = 1;
                        optResult.BusinessType = CommonBusinessType.PLCodeQuery;
                        optResult.BusinessObject = info;
                        optResult.BusinessReturnValue = 1;
                        SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
                        //
                        if (msg.ExcuResult)
                        {
                            Share.Instance.WriteLog(" 台区信息查询返回成功！");
                        }
                    }
                });
        }
        

        /// <summary>
        /// 设备台区信息设置
        /// </summary>
        /// <param name="client"></param>
        /// <param name="info"></param>
        public void SendSetPlatFormPackage(DevClient client,PL_PLInfo info)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    byte[] data = new byte[14];
                    byte[] bDevAddr = ByteHelper.HexStrToByteArrayWithDelimiter(info.PlcDevAddr, "-");
                    Buffer.BlockCopy(bDevAddr, 0, data, 0, 8);
                    data[8] = byte.Parse(info.ContryCode);
                    byte[] b1 = new byte[2];
                    b1 = BitConverter.GetBytes(short.Parse(info.CityCode));
                    Buffer.BlockCopy(b1, 0, data, 9, 2);
                    b1 = new byte[4];
                    b1 = BitConverter.GetBytes(Int32.Parse(info.PlfCode));
                    Buffer.BlockCopy(b1, 0, data, 11, 3);

                   // byte seq = Share.Instance.GetNewSeq(0x80);
                    Terminal_PackageData SendPkg = new Terminal_PackageData(client.bAddr, data, TerminalCmdWordConst.SetCmd, client.GetSendNo, TerminalCmdWordConst.SetPlatForm_FN);
                    Share.Instance.SendDataToTerminal(bDevAddr, SendPkg);
                });
        }

        /// <summary>
        /// 设备台区信息设置回复
        /// </summary>
        /// <param name="package"></param>
        public void PlatFormSetPackageBackHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "PlatFormSetPackageBackHandler", "")
                .Do(() =>
                {
                    CommonBusinessBackResult optResult = new CommonBusinessBackResult();
                    optResult.ExecuteResult = 1;
                    optResult.BusinessType = CommonBusinessType.PLCodeModify;
                    optResult.BusinessReturnValue = 1;
                    SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
                    //
                    if (msg.ExcuResult)
                    {
                        Share.Instance.WriteLog(" 台区信息设置返回成功！");
                    }
                });
        }
    }
}
