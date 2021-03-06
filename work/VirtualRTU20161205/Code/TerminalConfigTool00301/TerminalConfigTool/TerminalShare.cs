﻿using CommunicateCore.Model;
using CommunicateCore.Utility;
using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Logger;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerminalConfigTool.model;
using Utility;
using Utility.Model;

namespace CommunicateCore.Terminal
{
    public class TerminalShare
    {
        private static TerminalShare _TerminalShare = null;

        private static readonly object initLock = new object();
        public static TerminalShare GetShare
        {
            get
            {
                if (_TerminalShare == null)
                {
                    lock (initLock)
                    {
                        if (_TerminalShare == null)
                        {
                            _TerminalShare = new TerminalShare();
                        }
                    }
                }
                return _TerminalShare;
            }
        }

        static SimpleLogger Terminal_Log = SimpleLogger.GetInstance();

        public int ReSendNum = 50;

        public bool StartUpgrade = true;
        public DateTime StartTime = DateTime.Now;
        
        public delegate void SendDataToTerminal(BrokerMessage bMsg, int no = -1);
        public SendDataToTerminal SendToTerminal;

        public delegate void WriteMsg_Terminal(string msg);
        public WriteMsg_Terminal WriteLog_Terminal;
        public void WriterLog(string msg)
        {
            Terminal_Log.Info(msg);
            Console.WriteLine(DateTime.Now);
            Console.WriteLine(msg);
        }

        public delegate void SendDataToRTUSvr(byte[] data);
        public SendDataToRTUSvr SendToRTUSvr;

        public IPAddress LocalIP;

        public int LocalPort;

        public IPAddress TO_RTUSvr_IP;

        public int TO_RTUSvr_Port;
        
        public int HeartBeatInterval;

        public TerminalClient CurClient = new TerminalClient();

        //保存连接的所有客户端;采用线程安全的HashTable(对于索引特别优化);注意枚举时一定要加锁(lock (clientList.SyncRoot) )
        public System.Collections.Hashtable ClientList = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

        public Dictionary<string, TerminalClient> UpgradeList = new Dictionary<string, TerminalClient>();
        
        public void InitConfig()
        {
            string ip = ConfigurationManager.AppSettings["IP"];
            LocalIP = IPAddress.Parse(ip);

            string port = ConfigurationManager.AppSettings["Port"];
            LocalPort = int.Parse(port);

        }
                
        /// <summary>
        /// 从电参数消息对象中获取电参数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="curIndex"></param>
        /// <returns></returns>
        public tLampHisDataRec GetElecDataFromBytes(ElecData info)
        {
            tLampHisDataRec val = new tLampHisDataRec();

            val.LampStatus = info.OptValue;
            val.LampU = BitConverter.ToInt16(info.Voltage, 0) / 100.0;
            val.LampI = BitConverter.ToInt16(info.Current, 0);
            val.LampAP = BitConverter.ToInt16(info.ActivePower, 0) / 100.0;
            double SP = BitConverter.ToInt16(info.AppPower, 0) / 100.0;
            val.LampVP = Math.Round(Math.Pow(SP * SP - val.LampAP * val.LampAP, 0.5), 2);
            if (SP == 0)
            {
                val.LampPF = 0;
            }
            else
            {
                val.LampPF = val.LampAP / SP;
                if (val.LampPF > 1) val.LampPF = 1;
            }
            return val;
        }

        public bool IsIP(string ip)
        {
            //判断是否为IP
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
    
    }
}
