using OSRLamp.BusinessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OSRLamp
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
                    Utility.WriteLog("Ctrl+C");
                    break;
                case 2:
                    Utility.WriteLog("按键");
                    break;
            }
            TcpServer.Instance.TcpClose();
            return false;
        }

        [STAThread]
        static void Main(string[] args)
        {
            TcpServer.Instance.InitClient();
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

                //开灯
                if (cmd.ToLower() == "open")
                {
                    Utility.WriteLog("开灯");
                    RealCtrlLamp.Instance.SendRealCtrlLamp(devID, 1);
                    continue;
                }

                //关灯
                if (cmd.ToLower() == "close")
                {
                    Utility.WriteLog("关灯");
                    RealCtrlLamp.Instance.SendRealCtrlLamp(devID, 0);
                    continue;
                }

                //调光
                if (cmd.ToLower() == "open2")
                {
                    Utility.WriteLog("调光");
                    RealCtrlLamp.Instance.SendRealCtrlLamp(devID, 50);
                    continue;
                }

                //开启光感
                if (cmd.ToLower() == "setlight")
                {
                    Utility.WriteLog("开启光感");
                    LightCtrlHandler.Instance.SetLightCtrlHandler(devID, 1, BitConverter.GetBytes((short)15), BitConverter.GetBytes((short)600));
                    continue;
                }

                //禁用光感
                if (cmd.ToLower() == "setlight2")
                {
                    Utility.WriteLog("禁用光感");
                    LightCtrlHandler.Instance.SetLightCtrlHandler(devID, 0, BitConverter.GetBytes((short)15), BitConverter.GetBytes((short)600));
                    continue;
                }

                //查询光感
                if (cmd.ToLower() == "querylight")
                {
                    Utility.WriteLog("查询光感");
                    LightCtrlHandler.Instance.QueryLightCtrlHandler(devID);
                    continue;
                }

                //设置为手动模式
                if (cmd.ToLower() == "sethand")
                {
                    Utility.WriteLog("设置为手动模式");
                    ControlType.Instance.SetCtrlType(devID,1);
                    continue;
                }

                //设置为自动模式
                if (cmd.ToLower() == "setauto")
                {
                    Utility.WriteLog("设置为自动模式");
                    ControlType.Instance.SetCtrlType(devID, 0);
                    continue;
                }
                continue;
            }
        }
    }
}
