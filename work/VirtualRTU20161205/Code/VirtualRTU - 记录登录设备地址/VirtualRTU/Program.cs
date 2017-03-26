using CommunicateCore.RTUSvr;
using CommunicateCore.Terminal;
using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Logger;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualRTU
{
    class Program
    {
        static SimpleLogger LFI_Log = SimpleLogger.GetInstance();

        static void Main(string[] args)
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new VirtualRTUService()
            //};
            //ServiceBase.Run(ServicesToRun);

            //return;
            //ShowLog("正在申请验证，请稍等......");
            //ValidateFromServer.Instance.InitClient();
            //int requestNum = 0;
            //while (!(UtilityHelper.GetHelper.IsAllowed || requestNum>=30))
            //{
            //    if (requestNum > 0 && !UtilityHelper.GetHelper.IsAllowed)
            //    {
            //        Thread.Sleep(1000);
            //    }
            //    ValidateFromServer.Instance.RequestAllow();
            //    requestNum++;                
            //}
            //ValidateFromServer.Instance.Close();

            //if (UtilityHelper.GetHelper.IsAllowed)
            //{
            //    ShowLog("申请验证通过！");
            RTUSvrShare.GetShare.WriteLog_RTUSvr += ShowLog;
            //RTUSvrShare.GetShare.WriteLog_RTUSvr += RTUSvrShare.GetShare.WriterLog;

            TerminalShare.GetShare.WriteLog_Terminal += ShowLog;
            //TerminalShare.GetShare.WriteLog_Terminal += TerminalShare.GetShare.WriterLog;

            RTUSvr_TCPClient.GetClient.InitClient();
            Terminal_TCPClient.GetClient.InitClient();
            //}
            //else
            //{
            //    ShowLog("申请验证不通过，请与软件厂商联系！");
            //}

            Console.ReadLine();
        }
        
        public static void ShowLog(string msg)
        {
            if(!string.IsNullOrEmpty(msg))
            {
                LFI_Log.Info(msg);
                Console.WriteLine(msg);
            }
        }
    }
}
