﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Logger;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PassengerFlowServer.Utility
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

        private static SimpleLogger SimpleLog = SimpleLogger.GetInstance();
        public void WriteLog(string msg,int lvl=2)
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
            Console.WriteLine(msg);
        }
        
        public void CatchExpection(Exception ex)
        {
            WriteLog(ex.Message, 4);
        }

        public static IPEndPoint RemotePoint;

        public void InitConfig()
        {
            string val = "";

            val = ConfigurationManager.AppSettings["AdapterIP"];
            IPAddress addr = IPAddress.Parse(val);

            val = ConfigurationManager.AppSettings["AdapterPort"];
            int port = int.Parse(val);

            RemotePoint = new IPEndPoint(addr, port);
        }
    }
}
