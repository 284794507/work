using CTM_Route_Utility;
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

namespace CTM_Route
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CTMRouteService()
            };
            ServiceBase.Run(ServicesToRun);
            //Route_Utility.Instance.WriteLog_Route += ShowLog;
            //Route_Utility.Instance.WriteLog_Route += Route_Utility.Instance.WriterLog;

            //Guid ownGuid = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute))).Value);
            //int ownPID = Process.GetCurrentProcess().Id;
            //foreach (Process p in Process.GetProcesses())
            //{
            //    try
            //    {
            //        Guid proGuid = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(Assembly.Load(File.ReadAllBytes(p.MainModule.FileName)), typeof(GuidAttribute))).Value);
            //        if (ownGuid == proGuid && ownPID != p.Id)
            //        {
            //            Console.WriteLine("程序已经运行！");
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
            //    Route_Utility.Instance.WriteLog_Route("Failed to initialize!",2);
            //    Console.ReadKey();
            //    return;
            //}

            //var result = bootstrap.Start();
            //Route_Utility.Instance.WriteLog_Route(string.Format("Start result: {0}!", result),2);

            //if (result == StartResult.Failed)
            //{
            //    Route_Utility.Instance.WriteLog_Route("Failed to start!", 2);
            //    Console.ReadKey();
            //    return;
            //}

            //while (Console.ReadKey().KeyChar != 'q')
            //{
            //    Console.WriteLine();
            //    continue;
            //}
            //bootstrap.Stop();
        }

        public static void ShowLog(string msg,int lvl)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(msg);
            }
        }
    }
}
