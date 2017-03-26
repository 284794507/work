using Communicate_Core;
using Communicate_Core.Business;
using Communicate_Core.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace CTM_DataAnaly
{
    class Program
    {
        private static HttpSelfHostConfiguration _config = null;
        private static HttpSelfHostServer _host = null;

        static void Main(string[] args)
        {
            //异常捕获
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

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
            Share.Instance.Write_CTM_log += ShowLog;

            TerminalClient.Instance.InitClient();

            _config = new HttpSelfHostConfiguration("http://192.168.1.245:60190");//("http://192.168.1.6:61190");//
            _config.MapHttpAttributeRoutes();
            _config.MaxReceivedMessageSize = long.MaxValue;
            _config.TransferMode = TransferMode.Streamed;
            _config.Routes.MapHttpRoute("Route1", "api/{controller}/{value}", new { value = RouteParameter.Optional });
            _host = new HttpSelfHostServer(_config);
            _host.OpenAsync().Wait();

            while (true)
            {
                Console.WriteLine("");

                string cmd = Console.ReadLine();
                byte[] devID = new byte[] { 0x02, 0x11, 0x00, 0x01, 0x20, 0x14 };

                //退出测试程序
                if (cmd.ToLower() == "exit")
                {
                    break;
                }

                //查询设备列表
                if (cmd.ToLower() == "devlist")
                {
                    Share.Instance.WriteLog("查询设备列表");
                    TerminalListHandler.Instance.QueryTerminalList();
                    continue;
                }
                
                continue;
            }

            //while (Console.ReadKey().KeyChar != 'q')
            //{
            //    continue;
            //}
        }

        public static void ShowLog(string msg)
        {
            Console.WriteLine(DateTime.Now + " : " + msg);
        }

        /// <summary>
        /// 顶级异常的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception error = (Exception)e.ExceptionObject;

            Console.WriteLine(DateTime.Now + " : " + error.Message);
        }
    }
}
