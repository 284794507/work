using CTM_Route_Utility;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CTM_Route
{
    partial class CTMRouteService : ServiceBase
    {
        IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();

        public CTMRouteService()
        {
            InitializeComponent();

            Route_Utility.Instance.WriteLog_Route += Route_Utility.Instance.WriterLog;
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            if (!bootstrap.Initialize())
            {
                Route_Utility.Instance.WriteLog_Route("Failed to initialize!", 2);
                return;
            }

            var result = bootstrap.Start();
            Route_Utility.Instance.WriteLog_Route(string.Format("Start result: {0}!", result), 2);

            if (result == StartResult.Failed)
            {
                Route_Utility.Instance.WriteLog_Route("Failed to start!", 2);
                return;
            }
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。            
            bootstrap.Stop();
        }
        
    }
}
