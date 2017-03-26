using PassengerFlowServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PassengerFlowServer
{
    class Program
    {
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
            MainClient.Instance.CloseClient();
            return false;
        }

        [STAThread]
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PFSvrService()
            };
            ServiceBase.Run(ServicesToRun);

            //bool bRet = SetConsoleCtrlHandler(cancelHandler, true);

            //MainClient.Instance.InitClient();

            //Console.ReadLine();
        }
    }
}
