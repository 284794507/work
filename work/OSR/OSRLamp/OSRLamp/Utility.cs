using OSRLamp.BusinessHandler;
using OSRLamp.PackageHandler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Logger;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OSRLamp
{
    public class Utility
    {
        public static int SerialNo;

        public static byte[] GetSerialNo()
        {
            byte[] result = new byte[2];
            int newNo = SerialNo + 1;
            if(newNo > 65535)
            {
                newNo = 0;
                SerialNo = 0;
            }
            result = BitConverter.GetBytes((short)newNo);
            return result;
        }

        private static SimpleLogger SimpleLog = SimpleLogger.GetInstance();

        public static void WriteLog(string msg, int lvl = 2)
        {
            switch (lvl)
            {
                case 1:
                    SimpleLog.Debug(msg);
                    break;
                case 2:
                    SimpleLog.Info(msg);
                    break;
                case 3:
                    SimpleLog.Warn(msg);
                    break;
                case 4:
                    SimpleLog.Error(msg);
                    break;
                case 5:
                    SimpleLog.Fatal(msg);
                    break;
            }
            Console.WriteLine(DateTime.Now.ToString() + " : " + msg);
        }

        public static void CatchExpection(Exception ex)
        {
            WriteLog(ex.Message, 4);
        }

        public static IPAddress Adapter_IP;
        public static int Adapter_Port;
        public static int HeartBeatTime;

        public static void InitConfig()
        {
            string val = "";

            val = ConfigurationManager.AppSettings["Adapter_IP"];
            Adapter_IP = IPAddress.Parse(val);

            val = ConfigurationManager.AppSettings["Adapter_Port"];
            Adapter_Port = int.Parse(val);

            val = ConfigurationManager.AppSettings["HeartBeatTime"];
            HeartBeatTime = int.Parse(val);
        }

        public delegate void SendData(NJPackageData package);
        public static　SendData SendDataToTerminal;

        public delegate void HandlerFunction(NJPackageData package);
        public static Dictionary<string, HandlerFunction> dictAllHandlerFun;
        
        public static　void InitHandlerFunction()
        {
            dictAllHandlerFun = new Dictionary<string, HandlerFunction>();
            string key = ByteHelper.ByteToHexStrWithDelimiter(CmdWord.Terminal_Login);//登录
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, LoginHandler.Instance.LoginPackageHandler);
            }

            key = ByteHelper.ByteToHexStrWithDelimiter(CmdWord.Terminal_HeartBeat);//心跳
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, HeartBeatHandler.Instance.HeartBeatPackageHandler);
            }

            key = ByteHelper.ByteToHexStrWithDelimiter(CmdWord.Query_LightCtrlBack);//查询光感控制回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, LightCtrlHandler.Instance.QueryLightCrtlBackPackageHandler);
            }

            key = ByteHelper.ByteToHexStrWithDelimiter(CmdWord.Terminal_Response);//通用回复
            if (!dictAllHandlerFun.ContainsKey(key))
            {
                dictAllHandlerFun.Add(key, TerminalResponseHandler.Instance.TerminalResponsePackageHandler);
            }
        }

        private void NoPackageHandler(NJPackageData package)
        {

        }

        public static void MasterResponse(NJPackageData pkg)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Log(Utility.WriteLog, "", "MasterResponse！")
                .Do(() =>
                {
                    byte[] data = new byte[5];
                    Buffer.BlockCopy(pkg.SerialNumber, 0, data, 0, 2);
                    Buffer.BlockCopy(pkg.CmdWord, 0, data, 2, 2);
                    data[4] = 0;//0：成功/确认；1：失败；2：消息有误；3：不支持
                    //NJPackageData sendPkg = new NJPackageData(CmdWord.Master_Response, new byte[] { 0x00, 0x00 }, package.TerminalID, Utility.GetSerialNo(), new byte[] { 0x01, 0x00, 0x01, 0x00 }, new byte[0], data);
                    NJPackageData sendPkg = new NJPackageData(CmdWord.Master_Response, pkg.TerminalID, data);
                    Utility.SendDataToTerminal(sendPkg);
                });
        }

        //保存连接的所有客户端;采用线程安全的HashTable(对于索引特别优化);注意枚举时一定要加锁(lock (clientList.SyncRoot) )
        public static System.Collections.Hashtable ClientList = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

    }
}
