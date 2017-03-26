using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
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
using Utility;

namespace ProtocolAdapter
{
    partial class ProtocolAdapterService : ServiceBase
    {
        static SimpleLogger Adapter_Log = SimpleLogger.GetInstance();
        IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();

        public ProtocolAdapterService()
        {
            InitializeComponent();
            UtilityHelper.GetHelper.WriteLog_RTUSvr += ShowLog;
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            if (!bootstrap.Initialize())
            {
                UtilityHelper.GetHelper.WriteLog_RTUSvr("Failed to initialize!");
                return;
            }

            var result = bootstrap.Start();
            UtilityHelper.GetHelper.WriteLog_RTUSvr(string.Format("Start result: {0}!", result));

            if (result == StartResult.Failed)
            {
                UtilityHelper.GetHelper.WriteLog_RTUSvr("Failed to start!");
                return;
            }
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            bootstrap.Stop();
        }

        public static void ShowLog(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                Adapter_Log.Info(msg);
            }
        }
    }
}
