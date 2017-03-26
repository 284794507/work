using Communication_Core.PackageHandler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Logger;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Communication_Core.Utility
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

        private static SimpleLogger Simple_Log = SimpleLogger.GetInstance();

        public delegate void PrintMsg(string msg, int lvl);
        public PrintMsg WriteMsg;
        public void WriteLog(string msg,int lvl)
        {
            if (string.IsNullOrEmpty(msg)) return;
            switch (lvl)
            {
                case 1:
                    Simple_Log.Debug(msg);
                    break;
                case 2:
                    Simple_Log.Info(msg);
                    break;
                case 3:
                    Simple_Log.Warn(msg);
                    break;
                case 4:
                    Simple_Log.Error(msg);
                    break;
                case 5:
                    Simple_Log.Fatal(msg);
                    break;
            }
        }

        public void LogInfo(string msg)
        {
            WriteMsg(msg, 2);
        }

        public void CatchExpection(Exception ex)
        {
            WriteMsg(ex.Message, 4);
        }

        public IPEndPoint DistributorEndPoint;
        public IPEndPoint PlatFormEndPoint;
        public string PlatFormAPN;

        public int TerminalHearBeatTime;

        public List<string> AllowDevList;
        public List<string> OnlyReceviceDevList;

        public void InitConfig()
        {
            string value = "";
            IPAddress ipAddr;
            int port;

            value = ConfigurationManager.AppSettings["DistributorIP"];
            ipAddr = IPAddress.Parse(value);

            value = ConfigurationManager.AppSettings["DistributorPort"];
            port = int.Parse(value);

            DistributorEndPoint = new IPEndPoint(ipAddr, port);

            value = ConfigurationManager.AppSettings["PlatFormIP"];
            ipAddr = IPAddress.Parse(value);

            value = ConfigurationManager.AppSettings["PlatFormPort"];
            port = int.Parse(value);

            PlatFormEndPoint = new IPEndPoint(ipAddr, port);

            value= ConfigurationManager.AppSettings["PlatFormAPN"];
            PlatFormAPN = value;

            value = ConfigurationManager.AppSettings["TerminalHearBeatTime"];
            TerminalHearBeatTime = int.Parse(value);

            value = ConfigurationManager.AppSettings["AllowDevID"];
            AllowDevList = new List<string>();
            if (!string.IsNullOrEmpty(value))
            {
                AllowDevList=GetDevList(value);
            }

            value= ConfigurationManager.AppSettings["OnlyReceviceDevID"];
            OnlyReceviceDevList = new List<string>();
            if (!string.IsNullOrEmpty(value))
            {
                OnlyReceviceDevList=GetDevList(value);
            }

        }

        private List<string> GetDevList(string val)
        {
            List<string> devList=new List<string>();
            
            AspectF.Define.Retry(CatchExpection)
                .Do(() =>
                {
                    string[] arr = val.Split(',');
                    int len = arr.Length;
                    if (len > 0)
                    {
                        foreach (string addr in arr)
                        {
                            byte[] bAddr = new byte[8];
                            long id = long.Parse(addr);
                            long a = id / 100;
                            long b = id % 100;
                            int i = 0;
                            while (b > 0)
                            {
                                bAddr[i++] = Convert.ToByte(b.ToString(), 16);
                                id = a;
                                a = id / 100;
                                b = id % 100;
                            }

                            string str = BitConverter.ToString(bAddr);// ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-", false);
                            if (!devList.Contains(str))
                            {
                                devList.Add(str);
                            }
                        }
                    }
                });

            return devList;
        }

        public delegate void SendPackage(XNPackageData package);
        public SendPackage SendToDistriSvr;

        //保存连接的所有客户端;采用线程安全的HashTable(对于索引特别优化);注意枚举时一定要加锁(lock (clientList.SyncRoot) )
        public static System.Collections.Hashtable ClientList = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

        public delegate void MonitorHeartBeatAliveTime();

        public MonitorHeartBeatAliveTime MonitorHeartBeat = null;

        public delegate void MonitorTcpToDistri();

        public MonitorTcpToDistri MonitorTcp = null;

    }
}
