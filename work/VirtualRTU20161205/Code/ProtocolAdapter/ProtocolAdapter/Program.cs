using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Logger;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace ProtocolAdapter
{
    class Program
    {
        static SimpleLogger Adapter_Log = SimpleLogger.GetInstance();

        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ProtocolAdapterService()
            };
            ServiceBase.Run(ServicesToRun);

            //UtilityHelper.GetHelper.WriteLog_RTUSvr += ShowLog;

            //Guid ownGuid = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute))).Value);
            //int ownPID = Process.GetCurrentProcess().Id;
            //foreach (Process p in Process.GetProcesses())
            //{
            //    try
            //    {
            //        Guid proGuid = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(Assembly.Load(File.ReadAllBytes(p.MainModule.FileName)), typeof(GuidAttribute))).Value);
            //        if (ownGuid == proGuid && ownPID != p.Id)
            //        {
            //            UtilityHelper.GetHelper.WriterLog("程序已经运行！");
            //            return;
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}

            //var bootstrap = BootstrapFactory.CreateBootstrap();

            //if (!bootstrap.Initialize())
            //{
            //    //UtilityHelper.GetHelper.WriteLog_RTUSvr(DateTime.Now.ToString());
            //    UtilityHelper.GetHelper.WriteLog_RTUSvr("Failed to initialize!");
            //    Console.ReadKey();
            //    return;
            //}

            //var result = bootstrap.Start();
            ////UtilityHelper.GetHelper.WriteLog_RTUSvr(DateTime.Now.ToString());
            ////UtilityHelper.GetHelper.WriteLog_RTUSvr("Start result: {0}!", result);
            //UtilityHelper.GetHelper.WriteLog_RTUSvr(string.Format("Start result: {0}!", result));

            //if (result == StartResult.Failed)
            //{
            //    //UtilityHelper.GetHelper.WriteLog_RTUSvr(DateTime.Now.ToString());
            //    UtilityHelper.GetHelper.WriteLog_RTUSvr("Failed to start!");
            //    Console.ReadKey();
            //    return;
            //}

            //while (Console.ReadKey().KeyChar != 'q')
            //{
            //    Console.ReadLine();
            //    continue;
            //}
            //bootstrap.Stop();
        }

        public static void ShowLog(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                Adapter_Log.Info(msg);
                Console.WriteLine(msg);
            }
        }
    }
}
