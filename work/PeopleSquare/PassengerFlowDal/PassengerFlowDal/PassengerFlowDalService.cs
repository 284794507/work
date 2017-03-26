using PassengerFlowDal.Utility;
using PassengerFlowDal.WcfServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PassengerFlowDal
{
    partial class PassengerFlowDalService : ServiceBase
    {
        private static ServiceHost wcfHost = null;
        public PassengerFlowDalService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    if(wcfHost!=null)
                    {
                        wcfHost.Close();
                    }
                    wcfHost = new ServiceHost(typeof(DalSvr));
                    if(wcfHost.State!=CommunicationState.Opened)
                    {
                        wcfHost.Open();
                    }
                });
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    wcfHost.Close();
                    wcfHost = null;
                });
        }
    }
}
