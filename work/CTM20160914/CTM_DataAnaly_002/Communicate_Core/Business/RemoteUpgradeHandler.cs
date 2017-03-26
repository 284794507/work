using Communicate_Core.Model;
using Communicate_Core.PackageHandler;
using Communicate_Core.Utility;
using CTMDAL.Model;
using DL_LMS_Server.Default.Shared;
using DL_LMS_Server.Service.DataModel.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communicate_Core.Business
{
    public class RemoteUpgradeHandler
    {
        private static RemoteUpgradeHandler _RemoteUpgradeHandler;
        public static RemoteUpgradeHandler Instance
        {
            get
            {
                if (_RemoteUpgradeHandler == null)
                {
                    _RemoteUpgradeHandler = new RemoteUpgradeHandler();
                }
                return _RemoteUpgradeHandler;
            }
        }

        /// <summary>
        /// 准备升级
        /// </summary>
        /// <param name="curFile"></param>
        /// <param name="curClient"></param>
        public void ReadyToUpgrade(TPLUpgradeFileInfo curFile,string terminalAddr, DevClient curClient)
        {
            int fileLen = curFile.FileContent.Length;
            int bagNum = fileLen / curFile.FilePerSize;
            if (fileLen % curFile.FilePerSize > 0) bagNum++;
            curClient.UpgradeInfo = new UpgradeStatus();
            curClient.UpgradeInfo.AllBagNum = bagNum;
            curClient.UpgradeInfo.CurBagNo = 1;
            curClient.UpgradeInfo.IsUpgradeSuccess = false;
            curClient.UpgradeInfo.TerminalID = curFile.PLCollectorInfoID.ToString();// Share.Instance.GetDevIDByAddr(curClient.CommAddr);

            byte[] data = new byte[8 + 33];
            int curIndex = 0;
            byte[] devAddr = ByteHelper.HexStrToByteArrayWithDelimiter(terminalAddr, "-");
            Buffer.BlockCopy(devAddr, 0, data, curIndex, 8);
            curIndex += 8;
            int no = curFile.FileName.LastIndexOf(".");
            string realName = curFile.FileName.Substring(0, no);
            byte[] fileName= Encoding.UTF8.GetBytes(realName);
            Buffer.BlockCopy(fileName, 0, data, curIndex, 11);
            curIndex += 11;

            string[] HVer = curFile.FileHardWareVer.Split('-');
            if (HVer.Length == 3)
            {
                byte[] pcba1 = BitConverter.GetBytes(short.Parse(HVer[0]));
                data[curIndex++] = pcba1[0];
                data[curIndex++] = pcba1[1];
                data[curIndex++] = (byte)(Encoding.ASCII.GetBytes(HVer[1].ToUpper())[0]);
                data[curIndex++] = byte.Parse(HVer[2]);
            }
            else
            {
                UpgradeFial("硬件版本号格式错误，请重新升级！");
                return;
            }

            string[] SVer = curFile.FileSoftWareVer.Split('.');
            if (SVer.Length == 2)
            {
                data[curIndex++] = Convert.ToByte(SVer[0], 16);
                data[curIndex++] = Convert.ToByte(SVer[1], 16);
            }
            else
            {
                UpgradeFial("软件版本号格式错误，请重新升级！");
                return;
            }
            byte []bFileLen = BitConverter.GetBytes(fileLen);
            Buffer.BlockCopy(bFileLen, 0, data, curIndex, 3);
            curIndex += 3;

            byte[] bCRC = ByteHelper.GetCrc16_Bytes(curFile.FileContent);
            data[curIndex++] = bCRC[0];
            data[curIndex++] = bCRC[1];

            bagNum++;//与测试工具兼容，总包数加1
            byte[] bNum = BitConverter.GetBytes((ushort)bagNum);
            data[curIndex++] = bNum[0];
            data[curIndex++] = bNum[1];

            byte[] bTime = ByteHelper.DateTimeToBytes9(DateTime.Now);
            Buffer.BlockCopy(bTime, 0, data, curIndex, 9);
            curIndex += 9;            

            Terminal_PackageData SendPkg = new Terminal_PackageData(curClient.bAddr, data, TerminalCmdWordConst.RemoteUpgrade, curClient.GetSendNo, TerminalCmdWordConst.Ready_FN);
            Share.Instance.SendOnlyDataToTerminal(SendPkg);
        }

        /// <summary>
        /// 升级回复
        /// </summary>
        /// <param name="package"></param>
        public void RemoteUpgradePackageHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    int receviceBagNo = 0;
                    string key = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, "-");
                    if (Share.Instance.ClientList.ContainsKey(key))
                    {
                        DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                        curClient.HeartBeatTime = DateTime.Now;

                        switch(package.FN)
                        {
                            case 0xF1://重发此包
                                receviceBagNo = BitConverter.ToInt16(package.OnlyData, 8);//前面加8个字节的地址
                                ReSendReceviceBag(curClient, receviceBagNo);
                                break;
                            case 0xF2://接受成功
                                receviceBagNo = BitConverter.ToInt16(package.OnlyData, 8);//前面加8个字节的地址
                                ReceviceSuccessful(curClient, receviceBagNo);
                                break;
                            case 0xF3://不允许升级
                                curClient.UpgradeInfo = new UpgradeStatus();
                                byte noAllow = package.OnlyData[8];//前面加8个字节的地址
                                NoAllowUpgrade(noAllow);
                                break;
                            case 0xF4://回应续传
                                RetryUpgrade(curClient, package.OnlyData);
                                break;
                        }
                    }
                });
        }

        /// <summary>
        /// 生发指定包
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bagNo"></param>
        private void ReSendReceviceBag(DevClient client, int bagNo)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                   .Do(() =>
                   {
                       if (client.UpgradeInfo.SendNum >= Share.Instance.UpgradeReSendNum)
                       {
                           Share.Instance.WriteLog("重发超过指定次数，请重新升级！");
                           UpgradeFial("重发超过指定次数，请重新升级！");
                           client.UpgradeInfo.IsUpgradeSuccess = true;
                       }
                       else
                       {
                           client.UpgradeInfo.CurBagNo = bagNo;
                           SendBag(client);
                       }
                   });

        }

        /// <summary>
        /// 升级包接收成功
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bagNo"></param>
        private void ReceviceSuccessful(DevClient client, int bagNo)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                   .Do(() =>
                   {
                       if (client.UpgradeInfo.IsUpgradeSuccess)
                       {
                           Share.Instance.WriteLog("Upgrade Successed!");
                           UpgradeSuccess();
                       }
                       else
                       {
                           if (client.UpgradeInfo.CurBagNo == client.UpgradeInfo.AllBagNum)
                           {
                               client.UpgradeInfo.IsUpgradeSuccess = true;
                               string addr = Share.Instance.GetDevAddrByID(client.UpgradeInfo.TerminalID);
                               SendConfirmBag(client, addr);
                           }
                           else
                           {
                               client.UpgradeInfo.CurBagNo = bagNo + 1;
                               client.UpgradeInfo.SendNum = 0;
                               SendBag(client);
                           }
                           if (client.UpgradeInfo.IsNewUpgrade)
                           {
                               client.UpgradeInfo.IsNewUpgrade = false;
                               MonitorDevUpgrade();
                           }
                       }
                   });
        }

        /// <summary>
        /// 回应续传
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        private void RetryUpgrade(DevClient client,byte[] data)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    int isUpgrade = DBHandler.Instance.GetUpgradeStatus(client.UpgradeInfo.TerminalID, data);
                    int curIndex = 0;
                    byte[] addr = new byte[8];
                    Buffer.BlockCopy(data, 0, addr, 0, 8);
                    string sAddr = ByteHelper.ByteToHexStrWithDelimiter(addr, "-", false);
                    curIndex += 8;
                    int curBag = BitConverter.ToInt16(data, curIndex);
                    curIndex += 2;
                    int bagNum = BitConverter.ToInt16(data, curIndex);
                    curIndex += 2;

                    if (isUpgrade == 0)
                    {
                        UpgradeFial("设备与服务端记录不服，请重新升级！");
                        client.UpgradeInfo.IsUpgradeSuccess = true;
                    }
                    else
                    {
                        client.UpgradeInfo = new UpgradeStatus();
                        client.UpgradeInfo.AllBagNum = curBag + bagNum;
                        client.UpgradeInfo.CurBagNo = curBag + 1;
                        client.UpgradeInfo.IsUpgradeSuccess = false;
                        client.UpgradeInfo.TerminalID = Share.Instance.GetDevIDByAddr(sAddr);
                        client.UpgradeInfo.SendNum = 0;
                        SendBag(client);
                        client.UpgradeInfo.IsUpgradeSuccess = false;
                    }
                });
        }

        /// <summary>
        /// 发送升级包
        /// </summary>
        /// <param name="client"></param>
        public void SendBag(DevClient client)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    if (client.UpgradeInfo.IsUpgradeSuccess == false)
                    {
                        if (client.UpgradeInfo.SendNum >= Share.Instance.UpgradeReSendNum)
                        {
                            Share.Instance.WriteLog("重发超过指定次数，请重新升级！");
                            UpgradeFial("重发超过指定次数，请重新升级！");
                            client.UpgradeInfo.IsUpgradeSuccess = true;
                        }
                        else
                        {
                            TPLUpgradeFileInfoDetail detail = DBHandler.Instance.GetNextUpgradeData(client.UpgradeInfo.TerminalID, client.UpgradeInfo.CurBagNo);
                            int len = 8 + 2 + detail.FileDataContent.Length;
                            byte[] data = new byte[len];
                            string addr = Share.Instance.GetAddrByDevID(client.UpgradeInfo.TerminalID);
                            int curIndex = 0;
                            byte[] devAddr = ByteHelper.HexStrToByteArrayWithDelimiter(addr, "-");
                            Buffer.BlockCopy(devAddr, 0, data, curIndex, 8);
                            curIndex += 8;

                            byte[] bagNo = BitConverter.GetBytes((ushort)detail.FileDataNo);
                            data[curIndex++] = bagNo[0];
                            data[curIndex++] = bagNo[1];

                            Buffer.BlockCopy(detail.FileDataContent, 0, data, curIndex, detail.FileDataContent.Length);

                            client.UpgradeInfo.SendTime = DateTime.Now;
                            Terminal_PackageData SendPkg = new Terminal_PackageData();
                            if (client.UpgradeInfo.SendNum > 0)
                            {
                                SendPkg = new Terminal_PackageData(client.bAddr, data, TerminalCmdWordConst.RemoteUpgrade, client.GetSendNo, Share.Instance.GetLastSeq(0x80), TerminalCmdWordConst.Upgrading_FN);
                            }
                            else
                            {
                                SendPkg = new Terminal_PackageData(client.bAddr, data, TerminalCmdWordConst.RemoteUpgrade, client.GetSendNo, TerminalCmdWordConst.Upgrading_FN);
                            }
                            Share.Instance.SendOnlyDataToTerminal(SendPkg);

                            client.UpgradeInfo.SendNum++;
                        }
                    }                        
                });
        }

        public void ReSendBag(DevClient client)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                });
        }

        /// <summary>
        /// 发送升级完成确认包
        /// </summary>
        /// <param name="client"></param>
        public void SendConfirmBag(DevClient client,string addr)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    //string addr = Share.Instance.GetAddrByDevID(client.UpgradeInfo.TerminalID);
                    byte[] devAddr = ByteHelper.HexStrToByteArrayWithDelimiter(addr, "-");
                    Terminal_PackageData SendPkg = new Terminal_PackageData(client.bAddr, devAddr, TerminalCmdWordConst.RemoteUpgrade, client.GetSendNo, TerminalCmdWordConst.Confirm_FN);
                    Share.Instance.SendOnlyDataToTerminal(SendPkg);
                });
        }

        /// <summary>
        /// 续传
        /// </summary>
        /// <param name="client"></param>
        //public void RetryUpgrade(DevClient client)
        //{
        //    AspectF.Define.Retry(Share.Instance.ExceptionHandler)
        //        .Do(() =>
        //        {
        //            client.UpgradeInfo.TerminalID = Share.Instance.GetDevIDByAddr(client.CommAddr);

        //            Terminal_PackageData SendPkg = new Terminal_PackageData(client.bAddr, new byte[0], TerminalCmdWordConst.RemoteUpgrade, Share.Instance.GetBSendNo(), TerminalCmdWordConst.Confirm_FN);
        //            Share.Instance.SendOnlyDataToTerminal(SendPkg);
        //        });
        //}

        /// <summary>
        /// 取消升级
        /// </summary>
        /// <param name="client"></param>
        public void CancelUpgrade(DevClient client)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    string addr = Share.Instance.GetAddrByDevID(client.UpgradeInfo.TerminalID);
                    byte[] devAddr = ByteHelper.HexStrToByteArrayWithDelimiter(addr, "-");
                    Share.Instance.WriteLog(" 取消升级！");
                    client.UpgradeInfo.IsUpgradeSuccess = true;
                    Terminal_PackageData SendPkg = new Terminal_PackageData(client.bAddr, devAddr, TerminalCmdWordConst.RemoteUpgrade, client.GetSendNo, TerminalCmdWordConst.Cancel_FN);
                    Share.Instance.SendOnlyDataToTerminal(SendPkg);
                });
        }

        /// <summary>
        /// 监测升级是否超时
        /// </summary>
        private void MonitorDevUpgrade()
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler,()=> { MonitorDevUpgrade(); })
                .Log(Share.Instance.WriteLog, "MonitorDevUpgrade", "")
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            foreach (string macAddr in Share.Instance.ClientList.Keys)
                            {
                                DevClient dev = Share.Instance.ClientList[macAddr] as DevClient;
                                if (dev.UpgradeInfo.IsUpgradeSuccess == false && dev.UpgradeInfo.SendTime != DateTime.MinValue)
                                {
                                    TimeSpan ts = DateTime.Now - dev.UpgradeInfo.SendTime;
                                    if (ts.TotalMilliseconds >= 1000 * Share.Instance.UpgradeInterval)
                                    {
                                        Share.Instance.WriteLog(dev.UpgradeInfo.SendTime.ToString());
                                        Share.Instance.WriteLog(" 发送超时,重新发送！");
                                        SendBag(dev);

                                        Thread.Sleep(3000);
                                    }
                                }
                            }
                        }
                    }
                    );
                });
        }

        /// <summary>
        /// 不允许升级
        /// </summary>
        /// <param name="errNo"></param>
        private void NoAllowUpgrade(int errNo)
        {
            string msg = "";
            switch (errNo)
            {
                case 1:
                    msg = "硬件版本不对";
                    break;
                case 2:
                    msg = "软件版本过低";
                    break;
                case 3:
                    msg = "文件名不对";
                    break;
                case 4:
                    msg = "升级包过大";
                    break;
                case 5:
                    msg = "升级中";
                    break;
                case 6:
                    msg = "未准备好/原升级已经超时，每次升级等待时间10min";
                    break;
                case 7:
                    msg = "Flash读写故障，停止升级，显示报警";
                    break;
            }
            UpgradeFial(msg);
        }

        /// <summary>
        /// 升级失败
        /// </summary>
        /// <param name="errMsg"></param>
        private void UpgradeFial(string errMsg)
        {
            CommonBusinessBackResult optResult = new CommonBusinessBackResult();
            optResult.ExecuteResult = 1;
            optResult.BusinessType = CommonBusinessType.PLDevUpgrade_ResultInfo;
            optResult.BusinessReturnValue = 0;
            optResult.BusinessObject = errMsg;
            SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
            //
            if (msg.ExcuResult)
            {
                Share.Instance.WriteLog(" 升级结果返回成功！");
            }
        }

        /// <summary>
        /// 升级成功
        /// </summary>
        private void UpgradeSuccess()
        {
            CommonBusinessBackResult optResult = new CommonBusinessBackResult();
            optResult.ExecuteResult = 1;
            optResult.BusinessType = CommonBusinessType.PLDevUpgrade_ResultInfo;
            optResult.BusinessReturnValue = 1;
            optResult.BusinessObject = "升级完成！";
            SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
            //
            if (msg.ExcuResult)
            {
                Share.Instance.WriteLog(" 升级结果返回成功！");
            }
        }
    }
}
