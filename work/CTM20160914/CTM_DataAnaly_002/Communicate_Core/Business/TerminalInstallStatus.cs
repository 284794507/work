using Communicate_Core.Model;
using Communicate_Core.PackageHandler;
using Communicate_Core.Utility;
using DL_LMS_Server.Default.Shared;
using DL_LMS_Server.Service.DataModel.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.Business
{
    public class TerminalInstallStatus
    {//安装状态
        private static TerminalInstallStatus _TerminalInstallStatus;
        public static TerminalInstallStatus Instance
        {
            get
            {
                if (_TerminalInstallStatus == null)
                {
                    _TerminalInstallStatus = new TerminalInstallStatus();
                }
                return _TerminalInstallStatus;
            }
        }

        /// <summary>
        /// 安装状态查询
        /// </summary>
        /// <param name="client"></param>
        /// <param name="info"></param>
        public void SendQueryInstallStatusPackage(DevClient client, string devAddr)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    byte[] bDevAddr = ByteHelper.HexStrToByteArrayWithDelimiter(devAddr, "-");
                    Terminal_PackageData SendPkg = new Terminal_PackageData(client.bAddr, bDevAddr, TerminalCmdWordConst.QueryCmd, client.GetSendNo, TerminalCmdWordConst.QueryInstallStatus_FN);
                    Share.Instance.SendDataToTerminal(bDevAddr, SendPkg);
                });
        }

        /// <summary>
        /// 安装状态查询返回
        /// </summary>
        /// <param name="package"></param>
        public void InstallStatusPackageHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "Recevice InstallStatus Info！")
                .Do(() =>
                {
                    string key = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, "-");
                    if (Share.Instance.ClientList.ContainsKey(key))
                    {
                        DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                        curClient.HeartBeatTime = DateTime.Now;

                        int curIndex = 0;
                        byte[] bAddr = new byte[8];
                        Buffer.BlockCopy(package.OnlyData, curIndex, bAddr, 0, 8);
                        curIndex += 8;
                        int status = package.OnlyData[curIndex];

                        CommonBusinessBackResult optResult = new CommonBusinessBackResult();
                        optResult.ExecuteResult = 1;
                        optResult.BusinessType = CommonBusinessType.PLDevInstallationStatus_Query;
                        optResult.BusinessObject= ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-");
                        optResult.BusinessReturnValue = status+1;//界面与设备参加相差1
                        SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
                        //
                        if (msg.ExcuResult)
                        {
                            Share.Instance.WriteLog(" 安装状态查询返回成功！");
                        }
                    }
                });
        }


        /// <summary>
        /// 安装状态信息设置
        /// </summary>
        /// <param name="client"></param>
        /// <param name="info"></param>
        public void SendSetInstallStatusPackage(DevClient client, string devAddr,int status)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "SendSetInstallStatusPackage", "")
                .Do(() =>
                {
                    byte[] data = new byte[9];
                    byte[] bDevAddr = ByteHelper.HexStrToByteArrayWithDelimiter(devAddr, "-");
                    Buffer.BlockCopy(bDevAddr, 0, data, 0, 8);
                    data[8] = (byte)(status-1);//0 – 正常运行状态  1 - 安装中…

                    Terminal_PackageData SendPkg = new Terminal_PackageData(client.bAddr, data, TerminalCmdWordConst.SetCmd, client.GetSendNo, TerminalCmdWordConst.SetInstallStatus_FN);
                    Share.Instance.SendDataToTerminal(bDevAddr, SendPkg);
                });
        }

        /// <summary>
        /// 安装状态信息设置回复
        /// </summary>
        /// <param name="package"></param>
        public void InstallStatusSetPackageBackHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "InstallStatusSetPackageBackHandler", "")
                .Do(() =>
                {
                    CommonBusinessBackResult optResult = new CommonBusinessBackResult();
                    optResult.ExecuteResult = 1;
                    optResult.BusinessType = CommonBusinessType.PLDevInstallationStatus_Set;
                    optResult.BusinessReturnValue = 1;
                    SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
                    //
                    if (msg.ExcuResult)
                    {
                        Share.Instance.WriteLog(" 安装状态设置返回成功！");
                    }
                });
        }
    }
}
