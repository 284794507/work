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
    public class HeartBeatHandler
    {
        private static HeartBeatHandler _HeartBeatHandler;
        public static HeartBeatHandler Instance
        {
            get
            {
                if(_HeartBeatHandler==null)
                {
                    _HeartBeatHandler = new HeartBeatHandler();
                }
                return _HeartBeatHandler;
            }
        }

        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="package"></param>
        public void HeartBeatPackageHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "Recevice HeartBeat！")
                .Do(() =>
                {
                    string key = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, "-");
                    if (Share.Instance.ClientList.ContainsKey(key))
                    {
                        DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                        curClient.HeartBeatTime = DateTime.Now;

                        byte[] time = new byte[9];
                        Buffer.BlockCopy(package.OnlyData, 0, time, 0, 9);
                        TimeSpan ts = DateTime.Now - ByteHelper.Bytes9ToDateTime(time);
                        if (ts.TotalMilliseconds > 30 * 1000)//时间相关超过30秒，发校时指令
                        {
                            SendCheckTime(curClient,package.DevAddr);
                        }
                        
                        //回复心跳
                        Terminal_PackageData SendPkg = new Terminal_PackageData(package.DevAddr, new byte[] { 0x01 }, TerminalCmdWordConst.GetLoginOrHeartBeatBack, curClient.GetSendNo, TerminalCmdWordConst.HeartBeat_FN);
                        Share.Instance.SendOnlyDataToTerminal(SendPkg);
                    }
                });
        }

        private void UpdateGprsStatus(DevClient dev,string addr,byte status)
        {
            string id = Share.Instance.GetDevIDByAddr(addr);
            TPLCollectorMasterCommStatus_Cur masterComm = new TPLCollectorMasterCommStatus_Cur();
            masterComm.PLCollectorInfoID = new Guid(id);
            masterComm.ChkDataTime = DateTime.Now;
            masterComm.UpdateTime = DateTime.Now;
            masterComm.CommStatus = status;
            
            masterComm.TotalCommTimes = dev.SendNo;
            masterComm.SuccessfulCommTimes = dev.ReceviceNo;
            masterComm.LostRate = (masterComm.TotalCommTimes - masterComm.SuccessfulCommTimes) / masterComm.TotalCommTimes;
            DBHandler.Instance.UpdateGPRSCommStatus(masterComm);
        }

        /// <summary>
        /// 校时指令
        /// </summary>
        /// <param name="addr"></param>
        private void SendCheckTime(DevClient curClient,byte[] addr)
        {
            byte[] data = new byte[17];
            Buffer.BlockCopy(Share.Instance.BroadcastAddr, 0, data, 0, 8);
            byte[] bTime = ByteHelper.DateTimeToBytes9(DateTime.Now);
            Buffer.BlockCopy(bTime, 0, data, 8, 9);
            //byte seq = Share.Instance.GetNewSeq(0x80);
            Terminal_PackageData CheckPkg = new Terminal_PackageData(Share.Instance.BroadcastAddr, data, TerminalCmdWordConst.QueryCmd, curClient.GetSendNo,TerminalCmdWordConst.SetTime_FN);
            Share.Instance.SendDataToTerminal(addr,CheckPkg);
        }

        /// <summary>
        /// 监测心跳包
        /// </summary>
        public void MonitorRTUSvrHeartBeat()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(60000);
                            List<string> delClient = new List<string>();
                            foreach (string macAddr in Share.Instance.ClientList.Keys)
                            {
                                DevClient dev = Share.Instance.ClientList[macAddr] as DevClient;
                                TimeSpan ts = DateTime.Now - dev.HeartBeatTime;
                                if (ts.Minutes >= Share.Instance.HeartBeatTime *3)
                                {
                                    UpdateGprsStatus(dev, dev.CommAddr, 0x03);//连接断开，算主通信状态异常中断

                                    TPLCollectorInfo collector = new TPLCollectorInfo();
                                    collector.MacAddr = dev.CommAddr;
                                    collector.UpdateTime = DateTime.Now;
                                    collector.DevStatus = 0;//通讯中断，则设备状态未知
                                    DBHandler.Instance.UpdateCollectorStatus(collector);
                                    delClient.Add(macAddr);
                                }

                            }
                            foreach (string addr in delClient)//心跳过期则删除
                            {
                                Share.Instance.ClientList.Remove(addr);
                            }
                        }
                    }
                    );
            }
            catch (Exception ex)
            {
                Share.Instance.WriteLog(ex.Message, 4);
                MonitorRTUSvrHeartBeat();
            }
        }
    }
}
