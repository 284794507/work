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
using Utility;

namespace Distributor
{
    partial class DistributorService : ServiceBase
    {
        public DistributorService()
        {
            InitializeComponent();
            Share.Instance.WriteMsg += Share.Instance.WriteLog;
        }

        IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("OnStart");
                }))
                .Do(() =>
                {
                    if (!bootstrap.Initialize())
                    {
                        Share.Instance.WriteMsg("Failed to initialize!", 2);
                        return;
                    }

                    var result = bootstrap.Start();
                    Share.Instance.WriteMsg(string.Format("Start result: {0}!", result), 2);

                    if (result == StartResult.Failed)
                    {
                        Share.Instance.WriteMsg("Failed to start!", 2);
                        return;
                    }
                });
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("OnStop");
                }))
                .Do(() =>
                {
                    bootstrap.Stop();
                });
        }
    }
}
