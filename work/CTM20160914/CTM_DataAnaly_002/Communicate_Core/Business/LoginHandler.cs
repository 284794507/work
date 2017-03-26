using Communicate_Core.Model;
using Communicate_Core.PackageHandler;
using Communicate_Core.Utility;
using CTMDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communicate_Core.Business
{
    public class LoginHandler
    {
        private static LoginHandler _LoginHandler;
        public static LoginHandler Instance
        {
            get
            {
                if (_LoginHandler == null)
                {
                    _LoginHandler = new LoginHandler();
                }
                return _LoginHandler;
            }
        }

        public void LoginPackageHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "Login Successed！")
                .Do(() =>
                {
                    DevClient curClient = new DevClient();

                    string key = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, "-");
                    if (Share.Instance.ClientList.ContainsKey(key))
                    {
                        curClient = Share.Instance.ClientList[key] as DevClient;
                    }

                    int curIndex = 0;
                    TPLCollectorInfo collector = new TPLCollectorInfo();
                    collector.ObjID = Guid.NewGuid();
                    curClient.CommAddr = key;
                    curClient.bAddr = package.DevAddr;

                    byte devType = package.OnlyData[curIndex++];
                    collector.MacAddr = key;
                    collector.DevType = Share.Instance.GetDevType(devType);
                    collector.DevStatus = (byte)CollectorStatus.OnLine;
                    collector.SNCode = Encoding.UTF8.GetString(package.OnlyData, curIndex, 15);
                    curIndex += 15;

                    byte[] time = new byte[9];
                    Buffer.BlockCopy(package.OnlyData, curIndex, time, 0, 9);
                    curIndex += 9;
                    curClient.LoginTime = ByteHelper.Bytes9ToDateTime(time);
                    collector.ChannelNo = package.OnlyData[curIndex++];
                    int pcbNo = BitConverter.ToInt16(package.OnlyData, curIndex);
                    curIndex += 2;
                    string bVersion = Encoding.UTF8.GetString(package.OnlyData, curIndex++, 1);
                    byte bSubVersion = package.OnlyData[curIndex++];
                    collector.HVer = pcbNo.ToString() + "-" + bVersion + "-" + bSubVersion.ToString();
                    collector.SVer = ByteHelper.GetVersion(package.OnlyData, curIndex);
                    curIndex += 2;
                    collector.Lat = Share.Instance.GetLonLatFromData(package.OnlyData, curIndex);
                    curIndex += 4;
                    collector.Lon = Share.Instance.GetLonLatFromData(package.OnlyData, curIndex);
                    curIndex += 4;
                    string gprsDevID = Share.Instance.GetDevIDByAddr(key);
                    if(string.IsNullOrEmpty(gprsDevID))
                    {
                        collector.GprsID = collector.ObjID;
                    }
                    else
                    {
                        collector.GprsID = new Guid(gprsDevID);
                    }
                    
                    Share.Instance.WriteLog(collector.GprsID.ToString(),1);
                    curClient.HeartBeatTime = DateTime.Now;
                    curClient.IsOnLine = true;
                    curClient.batchTime = DateTime.MinValue;
                    curClient.UpgradeInfo = new UpgradeStatus();

                    string devid = Share.Instance.GetDevIDByAddr(key);

                    TPLCollectorMasterCommStatus_Cur masterComm = DBHandler.Instance.GetGprsCommStatus(devid);
                    if(masterComm!=null)
                    {
                        curClient.SendNo = (ushort)masterComm.TotalCommTimes;
                        int frameNum = BitConverter.ToInt16(package.FrameNum, 0);
                        if (frameNum == 0)
                        {
                            curClient.LastReceviceNo = (ushort)masterComm.SuccessfulCommTimes;
                        }
                        else
                        {
                            curClient.LastReceviceNo = 0;
                        }
                    }

                    if (!Share.Instance.ClientList.ContainsKey(key))
                    {
                        Share.Instance.ClientList.Add(key, curClient);
                    }
                    
                    LoginSuccess(curClient, package.DevAddr);

                    DBHandler.Instance.UpdateCollector(collector);
                });
        }

        /// <summary>
        /// 登录成功
        /// </summary>
        private void LoginSuccess(DevClient curClient,byte[] address)
        {
            Thread.Sleep(2000);
            Terminal_PackageData SendPkg = new Terminal_PackageData(address, new byte[] { 0x01 }, TerminalCmdWordConst.GetLoginOrHeartBeatBack, curClient.GetSendNo, TerminalCmdWordConst.LoginON_FN);
            Share.Instance.SendOnlyDataToTerminal(SendPkg);

            HeartBeatHandler.Instance.MonitorRTUSvrHeartBeat();
            SetRoute.Instance.MonitorSetRoute();
        }
    }
}
