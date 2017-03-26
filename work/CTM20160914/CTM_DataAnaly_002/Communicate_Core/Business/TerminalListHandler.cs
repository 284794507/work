using Communicate_Core.Model;
using Communicate_Core.PackageHandler;
using Communicate_Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.Business
{
    public class TerminalListHandler
    {
        private static TerminalListHandler _TerminalListHandler;
        public static TerminalListHandler Instance
        {
            get
            {
                if (_TerminalListHandler == null)
                {
                    _TerminalListHandler = new TerminalListHandler();
                }
                return _TerminalListHandler;
            }
        }

        /// <summary>
        /// 查询设备列表
        /// </summary>
        public void QueryTerminalList()
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                   .Log(Share.Instance.WriteLog, "", "Query TerminalList Info！")
                   .Do(() =>
                   {
                       string gprsAddr = "47-53-30-34-40-57-14-43";
                       byte[] devAddr = ByteHelper.HexStrToByteArrayWithDelimiter(gprsAddr, "-");

                       if (Share.Instance.ClientList.ContainsKey(gprsAddr))
                       {
                           DevClient curClient = Share.Instance.ClientList[gprsAddr] as DevClient;
                           Terminal_PackageData SendPkg = new Terminal_PackageData(devAddr, devAddr, TerminalCmdWordConst.QueryTerminalBasicInfo, curClient.GetSendNo, TerminalCmdWordConst.QueryTerminalList_FN);
                           Share.Instance.SendDataToTerminal(devAddr, SendPkg);
                       }
                       else
                       {
                           Share.Instance.GPRSOffLine(gprsAddr, CommonBusinessType.PLDevInfo_Query);
                       }
                   });
        }

        public void QueryTerminalListBackPackageHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "Recevice TerminalList Info！")
                .Do(() =>
                {
                    string key = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, "-");
                    if (Share.Instance.ClientList.ContainsKey(key))
                    {
                        DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                        curClient.HeartBeatTime = DateTime.Now;

                        int curIndex = 0;
                        curIndex += 3;//前３字节是台区号
                        int dataLen = package.OnlyData.Length;
                        byte devNum=package.OnlyData[curIndex++];
                        for(int i=0;i<devNum;i++)
                        {
                            if(curIndex+8>dataLen)
                            {
                                break;
                            }
                            byte[] addr = new byte[8];
                            Buffer.BlockCopy(package.OnlyData, curIndex, addr, 0, 8);
                            string strAddr = ByteHelper.ByteToHexStrWithDelimiter(addr, "-");
                            Share.Instance.WriteLog(strAddr);
                            curIndex += 8;
                        }                        
                    }
                });
        }
    }
}
