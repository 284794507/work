using Communicate_Core.PackageHandler;
using CTMDAL.Model;
using DL_LMS_Server.Service.DataModel.Result;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Logger;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Communicate_Core.Business;
using DL_LMS_Server.Service.DataModel.Parameter;
using Communicate_Core.Model;
using DL_LMS_Server.Default.Shared;
using System.IO;

namespace Communicate_Core.Utility
{
    public class Share
    {
        private static Share _Share;
        public static Share Instance
        {
            get
            {
                if(_Share==null)
                {
                    _Share = new Share();
                }
                return _Share;
            }
        }
        public byte PSeq;//0~63循环，下发1,回知复2,再下发3,再回复4
        public Dictionary<byte, SendToTerminalInfo> dictSendToTerminal = new Dictionary<byte, SendToTerminalInfo>();

        static SimpleLogger CTM_Log = SimpleLogger.GetInstance();

        public delegate void ShowLog(string msg);
        public ShowLog Write_CTM_log;
        public void WriteLog(string msg,int logType=2)
        {
            if (string.IsNullOrEmpty(msg)) return;
            Write_CTM_log(msg);
            //msg += "\r\n" + msg;
            switch (logType)
            {
                case 1:
                    CTM_Log.Debug(msg);
                    break;
                case 2:
                    CTM_Log.Info(msg);
                    break;
                case 3:
                    CTM_Log.Warn(msg);
                    break;
                case 4:
                    CTM_Log.Error(msg);
                    break;
                case 5:
                    CTM_Log.Fatal(msg);
                    break;
            }
        }

        public void ExceptionHandler(Exception ex)
        {
            WriteLog(ex.Message, 4);
        }

        public delegate void SendData(byte[] devAddr,Terminal_PackageData package);
        public SendData SendDataToTerminal;

        public delegate void SendOnlyData(Terminal_PackageData package);
        public SendOnlyData SendOnlyDataToTerminal;

        public delegate void HandlerFunction(Terminal_PackageData package);
        public Dictionary<string, HandlerFunction> dictAllHandlerFun;

        public byte[] BroadcastAddr = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        public IPAddress CTM_Route_IP;
        public int CTM_Route_Port;

        public List<TPlatFormInfo> ListPlat;
        public List<TPLCollectorInfo> listCollector;
        public List<TPLBasicInfo> listSupply;

        public Dictionary<string, bool> dictIsUpload = new Dictionary<string, bool>();

        public Guid BatchID;//电参数批次ID
        public DateTime LastElecDataTime;//上次电参数传输时间

        //保存连接的所有客户端;采用线程安全的HashTable(对于索引特别优化);注意枚举时一定要加锁(lock (clientList.SyncRoot) )
        public System.Collections.Hashtable ClientList = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

        public AppComServiceClient proxy;

        //记录台区号为0的设备与对应的plc设备地址的关系
        public Dictionary<string, string> dictTempPlc = new Dictionary<string, string>();

        /// <summary>
        /// 获得通信状态
        /// </summary>
        public ICommunicationObject GetObjetcStatus
        {
            get
            {
                return proxy as ICommunicationObject;
            }
        }

        /// <summary>
        /// 标识是否是平台发送的命令请求
        /// </summary>
        public Dictionary<int, CommonBusinessParameter> dictQueryFromLMSSvr;
        public DateTime ReceviceTime = DateTime.MinValue;
        //public Dictionary<string, DateTime> dictQueryFromLMSSvr = new Dictionary<string, DateTime>();
        public Dictionary<string, CommonBusinessType> dictQueryTypeFromLMSSvr = new Dictionary<string, CommonBusinessType>();

        public int HeartBeatTime;
        public int UpgradeInterval;
        public int UpgradeReSendNum;
        public int UpgradeGPRSPerSize;
        public int UpgradeOtherPerSize;
        public int LmsSvrInterval;

        public void InitConfig()
        {
            string ip = ConfigurationManager.AppSettings["CTM_Route_IP"];
            CTM_Route_IP = IPAddress.Parse(ip);

            string port = ConfigurationManager.AppSettings["CTM_Route_Port"];
            CTM_Route_Port = int.Parse(port);

            string time = ConfigurationManager.AppSettings["HeartBeatTime"];
            HeartBeatTime = int.Parse(time);

            string interval = ConfigurationManager.AppSettings["UpgradeInterval"];
            UpgradeInterval = int.Parse(interval); 

            string reSendNum = ConfigurationManager.AppSettings["UpgradeReSendNum"];
            UpgradeReSendNum = int.Parse(reSendNum);

            string perSize = ConfigurationManager.AppSettings["UpgradeGPRSPerSize"];
            UpgradeGPRSPerSize = int.Parse(perSize);

            perSize = ConfigurationManager.AppSettings["UpgradeOtherPerSize"];
            UpgradeOtherPerSize = int.Parse(perSize);

            string lInterval = ConfigurationManager.AppSettings["LmsSvrInterval"];
            LmsSvrInterval = int.Parse(lInterval);
        }

        public bool checkDevIsLoginOrNot(string gprsAddr)
        {            
            bool flag = false;
            if (ClientList.ContainsKey(gprsAddr))
            {
                flag = true;
            }
            else
            {
                WriteLog("设备未登录：" + gprsAddr);
            }
            return flag;
        }

        public byte GetNewSeq(byte seq)
        {
            //byte seq = package.SEQ;
            byte result = ++PSeq;
            if (ByteHelper.GetBit(seq, 7) == 1)
            {
                result = ByteHelper.SetBit(result, 7);
            }
            if (ByteHelper.GetBit(seq, 6) == 1)
            {
                result = ByteHelper.SetBit(result, 6);
            }

            return result;
        }

        public byte GetLastSeq(byte seq)
        {
            //byte seq = package.SEQ;
            byte result = PSeq;
            if (ByteHelper.GetBit(seq, 7) == 1)
            {
                result = ByteHelper.SetBit(result, 7);
            }
            if (ByteHelper.GetBit(seq, 6) == 1)
            {
                result = ByteHelper.SetBit(result, 6);
            }

            return result;
        }
        /// <summary>
        /// 记录下发到设备的报文，方便重试
        /// </summary>
        /// <param name="package"></param>
        public void AddSendToTerminal(Terminal_PackageData package,bool isRetransmission)
        {
            if (dictSendToTerminal == null)
            {
                dictSendToTerminal = new Dictionary<byte, SendToTerminalInfo>();
            }
            byte seq = package.SEQ;
            seq = ByteHelper.ClearBit(seq, 7);
            seq = ByteHelper.ClearBit(seq, 6);
            if (dictSendToTerminal.ContainsKey(seq))
            {
                SendToTerminalInfo info = dictSendToTerminal[seq];
                info.pkg = package;
                info.SendTime = DateTime.Now;
                //info.SendNum = 1;
                info.isRetransmission = isRetransmission;
            }
            else
            {
                SendToTerminalInfo info = new SendToTerminalInfo();
                info.pkg = package;
                info.SendTime = DateTime.Now;
                info.SendNum = 1;
                info.isRetransmission = isRetransmission;
                dictSendToTerminal.Add(seq, info);
            }
        }

        public void CheckSendToTerminal(byte rSeq)
        {
            byte seq =0;
            if (rSeq == 0)
            {
                seq = 63;
            }
            else
            {
                seq = (byte)(rSeq - 1);
            }
            if (dictSendToTerminal.ContainsKey(seq))
            {
                dictSendToTerminal.Remove(seq);
            }
        }


        /// <summary>
        /// 获取本机的Mac地址
        /// </summary>
        /// <returns></returns>
        public string GetMacAddress()
        {
            string mac = null;

            try
            {
                mac = GetLoaclMac(NetworkInterfaceType.Ethernet);
                if (String.IsNullOrEmpty(mac))
                {
                    mac = GetLoaclMac(NetworkInterfaceType.Wireless80211);
                }
                //ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
                //ManagementObjectCollection queryCollection = query.Get();
                //foreach (ManagementObject mo in queryCollection)
                //{
                //    if (mo["IPEnabled"].ToString() == "True")
                //        mac = mo["MacAddress"].ToString();
                //}
            }
            catch
            {

            }
            return mac;
        }
        
        public string GetLoaclMac(NetworkInterfaceType macType)
        {
            string mac = "";
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.NetworkInterfaceType == macType && ni.OperationalStatus == OperationalStatus.Up)
                {
                    if (macType == NetworkInterfaceType.Ethernet)
                    {
                        if (ni.Name.IndexOf("本地连接") >= 0)
                        {
                            mac = ni.GetPhysicalAddress().ToString();
                        }
                    }
                    else
                    {
                        mac = ni.GetPhysicalAddress().ToString();
                    }
                    break;
                }
            }
            return FormatMacAddress(mac);
        }

        public string FormatMacAddress(string mac)
        {
            string result = mac;

            if (mac.Length == 12)
            {
                result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", mac.Substring(0, 2), mac.Substring(2, 2), mac.Substring(4, 2), mac.Substring(6, 2), mac.Substring(8, 2), mac.Substring(10, 2));
            }

            return result;
        }

        //返回描述本地计算机上的网络接口的对象(网络接口也称为网络适配器)。
        public static NetworkInterface[] NetCardInfo()
        {
            return NetworkInterface.GetAllNetworkInterfaces();
        }

        ///<summary>
        /// 通过NetworkInterface读取网卡Mac
        ///</summary>
        ///<returns></returns>
        public static List<string> GetMacByNetworkInterface()
        {
            List<string> macs = new List<string>();
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                macs.Add(ni.GetPhysicalAddress().ToString());
            }
            return macs;
        }

        /// <summary>
        /// 从data的startindex开始，取4字节转换成经纬度
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public decimal GetLonLatFromData(byte[] data, int startIndex)
        {
            //0是北纬 1是南纬 北纬为正数，南纬为负数。
            //0是东经 1是西经 东经为正数，西经为负数。 
            int direction = (data[startIndex] >> 7) == 0 ? 1 : -1;
            byte[] lonLat = new byte[4];
            lonLat[3] = (byte)(data[startIndex] & 0x7F);
            lonLat[2] = data[startIndex + 1];
            lonLat[1] = data[startIndex + 2];
            lonLat[0] = data[startIndex + 3];
            decimal dLonLat = (decimal)(BitConverter.ToUInt32(lonLat, 0) * direction / 10000000.0);//经纬度保留５位小数

            return dLonLat;
        }

        /// <summary>
        /// 将设备的类型转换成平台显示类型
        /// </summary>
        /// <param name="devType"></param>
        /// <returns></returns>
        public byte GetDevType(byte devType)
        {
            byte result = 2;
            //0x10 – PLC协调器, 0x11 – LORA协调器,0x12 – 普通节点协调器,0x13 – 全功能协调器
            //0x10->集中器，0x11->节点+GPRS+GPS，0x12->普通节点；
            //0x02->普通节点，0x00->集中器，0x01->节点+GPRS+GPS，0x03->全功能协调器
            switch (devType)
            {
                case 0x10:
                    result = 0;
                    break;
                case 0x11:
                    result = 1;
                    break;
                case 0x12:
                    result = 2;
                    break;
                case 0x13:
                    result = 3;
                    break;
            }

            return result;
        }

        public string GetKey(byte[] cmd,byte fn)
        {
            return ByteHelper.ByteToHexStrWithDelimiter(cmd) + fn.ToString("X2");
        }

        public void InitHandlerFunction()
        {
            dictAllHandlerFun = new Dictionary<string, HandlerFunction>();
            string key = GetKey(TerminalCmdWordConst.GetLoginOrHeartBeat,TerminalCmdWordConst.LoginON_FN);//登录
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, LoginHandler.Instance.LoginPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.GetLoginOrHeartBeat, TerminalCmdWordConst.HeartBeat_FN);//心跳
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, HeartBeatHandler.Instance.HeartBeatPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.ResetBack, TerminalCmdWordConst.TerminalReset_FN);//重启
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, NoPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.QueryCmdBack, TerminalCmdWordConst.QueryPlatForm_FN);//查询台区信息回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, PlatFormHandler.Instance.PlatFormPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.QueryCmdBack, TerminalCmdWordConst.QueryInstallStatus_FN);//查询安装状态信息回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, TerminalInstallStatus.Instance.InstallStatusPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.QueryCmdBack, TerminalCmdWordConst.QueryLora_FN);//查询LORA模组信息回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, LoraHandler.Instance.LoraQueryPackageBackHandler);
            }

            key = GetKey(TerminalCmdWordConst.UploadTerminalBasicInfo, TerminalCmdWordConst.UploadTerminalInfo_FN);//上报设备信息
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, TerminalInfoHandler.Instance.TerminalInfoPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.QueryTerminalBasicInfoBack, TerminalCmdWordConst.QueryTerminalInfo_FN);//查询设备信息回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, TerminalInfoHandler.Instance.TerminalInfoPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.UploadData, TerminalCmdWordConst.UploadHisData_FN);//上报电参数
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, ElecDataHandler.Instance.ElecDataPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.QueryDataBack, TerminalCmdWordConst.QueryHisData_FN);//查询电参数回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, ElecDataHandler.Instance.ElecDataPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.UploadCommData, TerminalCmdWordConst.UploadComm_FN);//上报通信状态
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, CommStatusHandler.Instance.CommStatusPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.QueryCommDataBack, TerminalCmdWordConst.QueryComm_FN);//查询通信状态回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, CommStatusHandler.Instance.CommStatusPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.SetCmdBack, TerminalCmdWordConst.SetPlatForm_FN);//设置台区信息回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, PlatFormHandler.Instance.PlatFormSetPackageBackHandler);
            }
            
            key = GetKey(TerminalCmdWordConst.SetCmdBack, TerminalCmdWordConst.SetInstallStatus_FN);//设置安装状态信息回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, TerminalInstallStatus.Instance.InstallStatusSetPackageBackHandler);
            }

            key = GetKey(TerminalCmdWordConst.RemoteUpgradeBack, TerminalCmdWordConst.ReSendBag_FN);//升级回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, RemoteUpgradeHandler.Instance.RemoteUpgradePackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.RemoteUpgradeBack, TerminalCmdWordConst.ReceviceSuccess_FN);//升级回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, RemoteUpgradeHandler.Instance.RemoteUpgradePackageHandler);
            }
            
            key = GetKey(TerminalCmdWordConst.RemoteUpgradeBack, TerminalCmdWordConst.NotAllow_FN);//升级回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, RemoteUpgradeHandler.Instance.RemoteUpgradePackageHandler);
            }
            
            key = GetKey(TerminalCmdWordConst.RemoteUpgradeBack, TerminalCmdWordConst.SendAgain_FN);//升级回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, RemoteUpgradeHandler.Instance.RemoteUpgradePackageHandler);
            }
            
            key = GetKey(TerminalCmdWordConst.SetRouteBack, TerminalCmdWordConst.SetRoute_FN);//路由设置回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, NoPackageHandler);
            }

            key = GetKey(TerminalCmdWordConst.QueryTerminalBasicInfoBack, TerminalCmdWordConst.QueryTerminalList_FN);//查询设备列表回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, TerminalListHandler.Instance.QueryTerminalListBackPackageHandler);
            }
        }

        private void  NoPackageHandler(Terminal_PackageData package)
        {

        }

        public string MacNameToLmsSvr;

        /// <summary>
        /// 返回确认信息
        /// </summary>
        /// <param name="platFormID"></param>
        /// <param name="CommonType"></param>
        public void SendReceviceMsgToLmsSvr(string platFormID, CommonBusinessType CommonType)
        {
            CommonBusinessBackResult optResult = new CommonBusinessBackResult();
            optResult.BusinessType = CommonType;

            optResult.CabID = platFormID;
            optResult.ExecuteResult = 1;
            proxy.CommonBusinessDeviceBack(MacNameToLmsSvr, optResult);
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        /// <param name="platFormID"></param>
        /// <param name="devID"></param>
        /// <param name="devAddr"></param>
        /// <param name="CommonType"></param>
        /// <param name="errMsg"></param>
        public void SendErrorMsgToLmsSvr(string platFormID, string devID, string devAddr, CommonBusinessType CommonType, string errMsg)
        {
            CommonBusinessBackResult optResult = new CommonBusinessBackResult();
            optResult.BusinessType = CommonType;

            optResult.CabID = platFormID;
            //List<PLDevElecStatus> list = new List<PLDevElecStatus>();
            //PLDevElecStatus status = new PLDevElecStatus();
            //status.DevID = devID;// InitBusiness.GetInit.GetCollectorIDByAddr(addr).ToLower();
            //status.DevMac = devAddr;
            //status.DevStatus = (byte)CollectorStatus.PowerOff;
            //status.UpdateTime = DateTime.Now;
            //list.Add(status);
            //optResult.BusinessObject = list.ToArray();
            //optResult.ExecuteResult = 0;
            //optResult.ReturnMessage = errMsg;
            //proxy.CommonBusinessDeviceBack(MacNameToLmsSvr, optResult);
        }

        /// <summary>
        /// 检查设备是否存在
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public bool CheckDevExist(string addr)
        {
            bool result = false;
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.MacAddr == addr)
                        {
                            result = true;
                            break;
                        }
                    }

                });

            return result;
        }

        /// <summary>
        /// 根据节点地址获取设备ID
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public string GetDevIDByAddr(string addr)
        {
            string id = "";
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if(info.MacAddr==addr)
                        {
                            id = info.ObjID.ToString();
                            break;
                        }
                    }

                });

            return id;
        }

        /// <summary>
        /// 根据节点ID获取设备地址
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public string GetDevAddrByID(string ID)
        {
            string addr = "";
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.ObjID.ToString() == ID)
                        {
                            addr = info.MacAddr.ToString();
                            break;
                        }
                    }

                });

            return addr;
        }

        /// <summary>
        /// 根据节点地址获取设备名称
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public string GetDevNameByAddr(string addr)
        {
            string name = "";
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.MacAddr == addr)
                        {
                            name = info.CollectorName;
                            break;
                        }
                    }

                });

            return name;
        }

        /// <summary>
        /// 根据节点地址获取设备类型
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public byte GetDevTypeByAddr(string addr)
        {
            byte type = 0x12;
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.MacAddr == addr)
                        {
                            type = info.DevType;
                            break;
                        }
                    }

                });

            return type;
        }

        /// <summary>
        /// 根据节点地址获取所属GPRS设备地址
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public byte[] GetGPRSAddrByAddr(string addr)
        {
            byte[] result = new byte[0];
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    Guid gprsAddr=new Guid();
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.MacAddr == addr)
                        {
                            gprsAddr = info.GprsID.Value;
                            break;
                        }
                    }
                    
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.ObjID == gprsAddr)
                        {
                            result = ByteHelper.HexStrToByteArrayWithDelimiter(info.MacAddr, "-");
                            break;
                        }
                    }
                });

            return result;
        }

        /// <summary>
        /// 根据节点地址获取所属GPRS设备地址
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public string GetGPRSAddrByDevID(string devID)
        {
            byte[] result = new byte[0];
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    Guid gprsAddr = new Guid();
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.ObjID.ToString() == devID)
                        {
                            gprsAddr = info.GprsID.Value;
                            break;
                        }
                    }

                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.ObjID == gprsAddr)
                        {
                            result = ByteHelper.HexStrToByteArrayWithDelimiter(info.MacAddr, "-");
                            break;
                        }
                    }
                });

            string gprsID= ByteHelper.ByteToHexStrWithDelimiter(result, "-");
            return gprsID;
        }

        /// <summary>
        /// 根据设备ID获取节点地址
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public string GetAddrByDevID(string id)
        {
            string addr = "";
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.ObjID.ToString() == id)
                        {
                            addr = info.MacAddr;
                            break;
                        }
                    }

                });

            return addr;
        }

        /// <summary>
        /// 更新本地设备信息
        /// </summary>
        /// <param name="collector"></param>
        public void UpdateCollectorInfo_Local(TPLCollectorInfo collector)
        {
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    bool isInsert = true;
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.MacAddr == collector.MacAddr)//(info.SNCode == collector.SNCode)
                        {
                            info.SNCode = collector.SNCode;
                            info.DevType = collector.DevType;
                            info.DevStatus = collector.DevStatus;
                            info.ChannelNo = collector.ChannelNo;
                            info.HVer = collector.HVer;
                            info.SVer = collector.SVer;
                            isInsert = false;
                            break;
                        }
                    }

                    if(isInsert)
                    {
                        listCollector.Add(collector);
                    }

                });
        }

        /// <summary>
        /// 更新本地设备状态
        /// </summary>
        /// <param name="collector"></param>
        public void UpdateCollectorStatus_Local(TPLCollectorInfo collector)
        {
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.MacAddr == collector.MacAddr)
                        {
                            info.DevStatus = collector.DevStatus;
                            break;
                        }
                    }
                });
        }

        /// <summary>
        /// 更新设备的所属GPRS设备、PLC设备信息、台区信息
        /// </summary>
        /// <param name="devID"></param>
        /// <param name="gprsID"></param>
        /// <param name="plcID"></param>
        //public void UpdateCollectorUp(Guid devID, Guid gprsID, Guid plcID,Guid platID,string platCode)
        //{
        //    AspectF.Define.Retry(ExceptionHandler)
        //        .Do(() =>
        //        {
        //            foreach (TPLCollectorInfo info in listCollector)
        //            {
        //                if (info.ObjID == devID)
        //                {
        //                    info.GprsID = gprsID;
        //                    info.PlcID = plcID;
        //                    info.TPlatFormID = platID;
        //                    info.TPlatFormCode = platCode;
        //                    break;
        //                }
        //            }
        //        });
        //}

        //更新设备的所属PLC设备信息、台区信息
        public TPLCollectorInfo UpdateCollectorUp(Guid devID, Guid plcID, Guid platID, string platCode)
        {
            TPLCollectorInfo collector = new TPLCollectorInfo();
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.ObjID == devID)
                        {
                            info.PlcID = plcID;
                            info.TPlatFormID = platID;
                            info.TPlatFormCode = platCode;

                            collector = info;
                            break;
                        }
                    }
                });
            return collector;
        }

        /// <summary>
        /// 更新设备的所属GPRS设备
        /// </summary>
        /// <param name="devID"></param>
        /// <param name="gprsID"></param>
        public TPLCollectorInfo UpdateCollectorGPRS(Guid devID, Guid gprsID)
        {
            TPLCollectorInfo collector = new TPLCollectorInfo();
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.ObjID == devID)
                        {
                            info.GprsID = gprsID;

                            collector = info;
                            break;
                        }
                    }
                });
            return collector;
        }

        /// <summary>
        /// 更新设备的相位
        /// </summary>
        /// <param name="devID"></param>
        /// <param name="gprsID"></param>
        public TPLCollectorInfo UpdateCollectorPhase(Guid devID, string aPhase, string bPhase, string cPhase)
        {
            TPLCollectorInfo collector = new TPLCollectorInfo();
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.ObjID == devID)
                        {
                            info.APhase = aPhase;
                            info.BPhase = bPhase;
                            info.CPhase = cPhase;

                            collector = info;
                            break;
                        }
                    }
                });
            return collector;
        }

        /// <summary>
        /// 更新台区编号获取台区ID
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="cityCode"></param>
        /// <param name="platCode"></param>
        public Guid GetPlatID(string countryCode, string cityCode,string plcMacAddr,ref string platCode)
        {
            Guid result = Guid.Empty;
            try
            {
                bool isFind = false;
                string newPlatCode = platCode;
                foreach (TPlatFormInfo info in ListPlat)
                {
                    if(info.CountryCode==countryCode && info.CityCode==cityCode && info.PlatFormCode==platCode)
                    {
                        result = info.ObjID;
                        isFind = true;
                        break;
                    }
                }
                if(!isFind)
                {
                    if (platCode == "0" && dictTempPlc.ContainsKey(plcMacAddr))
                    {//如果设备台区号为0，但是已经归过组，则不再新增台区
                        result = new Guid(dictTempPlc[plcMacAddr]);
                        return result;
                    }
                    TPlatFormInfo newInfo = new TPlatFormInfo();
                    newInfo.ObjID = Guid.NewGuid();
                    newInfo.CountryCode = countryCode;
                    newInfo.CityCode = cityCode;
                    if(platCode =="0")
                    {
                        int maxCode = ListPlat.Max(p => int.Parse(p.PlatFormCode));
                        for (int i = 1; i < maxCode + 2; i++)//找一个没用过，且不重复的台区号
                        {
                            string newCode = i.ToString();
                            TPlatFormInfo info = ListPlat.Find(p => p.PlatFormCode == newCode);
                            if (info == null)
                            {
                                newInfo.PlatFormName = newCode;
                                newInfo.PlatFormCode = newCode;
                                newPlatCode = newCode;
                                break;
                            }
                        }
                    }
                    else
                    {
                        newInfo.PlatFormName = platCode;
                        newInfo.PlatFormCode = platCode;
                    }
                    //newInfo.PlatFormName = platCode;

                    DBHandler.Instance.AddNewPlatForm(newInfo);
                    if (platCode == "0")
                    {
                        if (!dictTempPlc.ContainsKey(plcMacAddr))
                        {
                            dictTempPlc.Add(plcMacAddr, newInfo.ObjID.ToString());
                        }
                    }
                    ListPlat.Add(newInfo);
                    platCode = newPlatCode;
                    result = newInfo.ObjID;
                }
                return result;
            }
            catch(Exception ex)
            {
                Share.Instance.WriteLog(ex.Message);
                return result;
            }
        }

        /// <summary>
        /// GPRS设备未登录
        /// </summary>
        /// <param name="package"></param>
        public void GPRSOffLine(string addr,CommonBusinessType curType)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    CommonBusinessBackResult optResult = new CommonBusinessBackResult();
                    optResult.ExecuteResult = 0;
                    optResult.BusinessType = curType;
                    string name = GetDevNameByAddr(addr);
                    optResult.ReturnMessage = name + " 未登录！";
                    SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
                    //
                    if (msg.ExcuResult)
                    {
                        Share.Instance.WriteLog(" GPRS设备未登录！");
                    }
                });
        }

        /// <summary>
        /// 获取台区下所有GPRS设备地址
        /// </summary>
        /// <param name="platFormID"></param>
        public string[] GetAllGprsByPlatFormID(string platFormID)
        {
            List<string> result = new List<string>();
            TPLCollectorInfo collector = new TPLCollectorInfo();
            AspectF.Define.Retry(ExceptionHandler)
                .Do(() =>
                {
                    foreach (TPLCollectorInfo info in listCollector)
                    {
                        if (info.TPlatFormID.ToString() == platFormID)
                        {
                            result.Add(info.MacAddr);
                        }
                    }
                });
            return result.ToArray();
        }

        /// <summary>
        /// 获取相应的文件夹，如果不存在，则新建
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPath(string path)
        {
            string tPath = path;
            if (!Directory.Exists(tPath))
            {
                Directory.CreateDirectory(tPath);
            }
            return tPath;
        }
    }
}
