using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace PassengerFlowAdapter
{
    class Program
    {
        static IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();

        public delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    Share.Instance.WriteLog("Ctrl+C");
                    break;
                case 2:
                    Share.Instance.WriteLog("按键");
                    break;
            }
            bootstrap.Stop();
            return false;
        }

        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PassengerFlowService()
            };
            ServiceBase.Run(ServicesToRun);

            //AspectF.Define.Retry(Share.Instance.CatchExpection)
            //    .Do(() =>
            //    {

            //        if (!bootstrap.Initialize())
            //        {
            //            Share.Instance.WriteLog("Failed to initialize!", 2);
            //            return;
            //        }

            //        var result = bootstrap.Start();
            //        Share.Instance.WriteLog(string.Format("Start result: {0}!", result), 2);

            //        if (result == StartResult.Failed)
            //        {
            //            Share.Instance.WriteLog("Failed to start!", 2);
            //            return;
            //        }

            //        Console.ReadLine();
            //    });
        }
    }
}
