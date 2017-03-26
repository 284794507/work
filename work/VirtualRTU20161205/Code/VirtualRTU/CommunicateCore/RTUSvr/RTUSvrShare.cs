using CommunicateCore.Model;
using CommunicateCore.Utility;
using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr
{
    public class RTUSvrShare
    {
        private static RTUSvrShare rtuSvrShare;

        public static RTUSvrShare GetShare
        {
            get
            {
                if (rtuSvrShare == null)
                {
                    rtuSvrShare = new RTUSvrShare();
                }
                return rtuSvrShare;
            }
        }

        public DateTime HeartBeatTime;
        public int HeartBeatNumber;
        public bool LoginSuccess = false;

        public delegate void HandlerPackage(LH_PackageData package);
        public Dictionary<string, HandlerPackage> dictAllHandlerFunction = new Dictionary<string, HandlerPackage>();

        public delegate void SendPackage();
        public Dictionary<string, SendPackage> dictAllSendFunction = new Dictionary<string, SendPackage>();

        public delegate void SendDataToRTUSvr(LH_PackageData package);
        public SendDataToRTUSvr SendToRTUSvr;

        public delegate void SendNoDataPackageToRTUSvr(byte[] ctuAddr, byte[] cmdWord);
        public SendNoDataPackageToRTUSvr SendToRTUSvrWithNoData;

        public delegate void WriteMsg_RTUSvr(string msg);
        public WriteMsg_RTUSvr WriteLog_RTUSvr;
        //public Action<string> WriterLog;
        public void WriterLog(string msg)
        {
            WriteLog_RTUSvr(msg);
        }

        public delegate void SendDataToLFI(byte[] data);
        public SendDataToLFI SendToLFI;

        public IPAddress RTUSvr_IP;
        public int RTUSvr_Port;

        public IPAddress TO_LFI_IP;
        public int TO_LFI_Port;

        public int HeartBeatInterval;
        
        public Dictionary<tLampGrpCfg, string> dictLampGrpCfg;//记录本次单灯分组配置信息

        public void InitConfig()
        {
            string ip = ConfigurationManager.AppSettings["RTUSvr_IP"];
            RTUSvr_IP = IPAddress.Parse(ip);

            string port = ConfigurationManager.AppSettings["RTUSvr_Port"];
            RTUSvr_Port = int.Parse(port);

            ip = ConfigurationManager.AppSettings["TO_LFI_IP"];
            TO_LFI_IP = IPAddress.Parse(ip);

            port = ConfigurationManager.AppSettings["TO_LFI_Port"];
            TO_LFI_Port = int.Parse(port);

            string interval = ConfigurationManager.AppSettings["HeartBeatInterval"];
            HeartBeatInterval = int.Parse(interval);
        }

        public void RefreshHeartBeatTime()
        {
            HeartBeatTime = DateTime.Now;
        }

        public Dictionary<string, DateTime> dictReceviceCmdTimeByLFC = new Dictionary<string, DateTime>();
        /// <summary>
        /// 记录是否是平台发送请求
        /// </summary>
        public Dictionary<string, string> dictSendByRTUSvr = new Dictionary<string, string>();

        public tCTUInfo[] CtuList;
        public tSysRunStatus[] curRunStatus;

        //记录单灯（组）同一指令发送的时间，同一设备同一指令，5秒内重复发送，直接过滤，不同指令，提示不要发送太频繁
        public Dictionary<int, CmdWordTime_LH> dictLampTimeOfSameCmdWord = new Dictionary<int, CmdWordTime_LH>();
        public Dictionary<int, CmdWordTime_LH> dictLampGrpTimeOfSameCmdWord = new Dictionary<int, CmdWordTime_LH>();

        public delegate void DRTCtrlByLampNo(string ctuAddr, int grpNo, int lampNo, byte opt, byte isLock);

        public DRTCtrlByLampNo RTCtrlByLampNo;

        public string GetCtuIDByCtuAddr(byte[] addr)
        {
            string result = "";
            AspectF.Define.Retry()
                .Do(() =>
                {
                    string ctuAddr = ByteHelper.bytesToCtuAddr(addr, true);
                    foreach (tCTUInfo info in CtuList)
                    {
                        if (info.CTUCommAddr == ctuAddr)
                        {
                            result = info.CTUID;
                        }
                    }
                });
            return result;
        }

        public byte[] GetCtuAddrByCtuID(string id)
        {
            byte[] ctuAddr = new byte[2];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    foreach (tCTUInfo info in CtuList)
                    {
                        if (info.CTUID == id)
                        {
                            ctuAddr = ByteHelper.CtuAddrToBytes(info.CTUCommAddr);
                        }
                    }
                });
            return ctuAddr;
        }

        public tSysRunStatus GetRunStatusByCtuID(string id)
        {
            tSysRunStatus result = new tSysRunStatus();
            AspectF.Define.Retry()
                .Do(() =>
                {
                    foreach (tSysRunStatus info in curRunStatus)
                    {
                        if (info.CTUID == id)
                        {
                            result = info;
                        }
                    }
                });
            return result;
        }

        /// <summary>
        /// 检测重复指令
        /// </summary>
        /// <param name="no"></param>
        /// <param name="package"></param>
        /// <returns></returns>
        public bool CheckRepeatCmdWord(int no, LH_PackageData package)
        {
            bool result = true;
            AspectF.Define.Retry()
                .Do(() =>
                {
                    CmdWordTime_LH cwt = new CmdWordTime_LH();
                    if (RTUSvrShare.GetShare.dictLampTimeOfSameCmdWord.ContainsKey(no))
                    {
                        cwt = RTUSvrShare.GetShare.dictLampTimeOfSameCmdWord[no];
                        TimeSpan ts = DateTime.Now - cwt.SendTime;
                        if (ts.Seconds < 5)
                        {
                            if (cwt.CmdWord == package.CmdWord)
                            {
                                result= false;//5秒内同一个设备同一具指令，忽略
                            }
                            else
                            {
                                RTUSvrShare.GetShare.WriteLog_RTUSvr("CheckRepeatCmdWord：" + "操作太频繁!");
                                result= false;//5秒内同一个设备不同指令，提示操作太频繁
                            }
                        }
                        else//超过5秒，则更新
                        {
                            cwt.CmdWord = package.CmdWord;
                            cwt.SendTime = DateTime.Now;
                            RTUSvrShare.GetShare.dictLampTimeOfSameCmdWord[no] = cwt;
                        }
                    }
                    else
                    {
                        cwt.CmdWord = package.CmdWord;
                        cwt.SendTime = DateTime.Now;
                        RTUSvrShare.GetShare.dictLampTimeOfSameCmdWord.Add(no, cwt);
                    }
                });
            return result;
        }

        /// <summary>
        /// 发送返回成功
        /// </summary>
        /// <param name="cmdWord"></param>
        /// <param name="lampNo"></param>
        /// <param name="opt"></param>
        public void SendBackSuccess(byte[] ctuAddr, byte[] cmdWord, int lampNo, byte opt)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte[] no = new byte[2];
                    no = BitConverter.GetBytes((short)lampNo);

                    byte[] data = new byte[3];
                    Buffer.BlockCopy(no, 0, data, 0, 2);
                    data[2] = opt;

                    LH_PackageData package = new LH_PackageData(ctuAddr, data, cmdWord);
                    RTUSvrShare.GetShare.SendToRTUSvr(package);
                });
        }

        /// <summary>
        /// 发送返回失败
        /// </summary>
        /// <param name="macAddr"></param>
        /// <param name="plcCmd"></param>
        public void SendBackFail(byte[] cutAddr, byte[] macAddr, byte plcCmd)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte[] data = new byte[8];
                    Buffer.BlockCopy(macAddr, 0, data, 0, 7);
                    data[7] = plcCmd;

                    LH_PackageData package = new LH_PackageData(cutAddr, data, LHCmdWordConst.GetPtuErrorCmd);
                    RTUSvrShare.GetShare.SendToRTUSvr(package);
                });
        }

    }
}
