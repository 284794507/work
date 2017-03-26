using CommunicateCore.RTUSvr;
using CommunicateCore.Terminal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Logger;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace VirtualRTU
{
    partial class VirtualRTUService : ServiceBase
    {
        static SimpleLogger LFI_Log = SimpleLogger.GetInstance();

        public VirtualRTUService()
        {
            InitializeComponent();
            RTUSvrShare.GetShare.WriteLog_RTUSvr += ShowLog;

            TerminalShare.GetShare.WriteLog_Terminal += ShowLog;
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。

            RTUSvr_TCPClient.GetClient.InitClient();
            Terminal_TCPClient.GetClient.InitClient();
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
        }

        public static void ShowLog(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                LFI_Log.Info(msg);
            }
        }
    }
}
