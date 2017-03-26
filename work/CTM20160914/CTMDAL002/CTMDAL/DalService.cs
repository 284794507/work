using CTMDAL.CTMDAL_Utility;
using CTMDAL.Utility;
using CTMDAL.WcfServer;
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

namespace CTMDAL
{
    public partial class DalService : ServiceBase
    {
        private static ServiceHost wcfHost = null;//wcf寄宿对象
        public DalService()
        {
            InitializeComponent();
            this.ServiceName = "CTMDalService";
            //UtilityShare.Instance.ShowLog += UtilityShare.Instance.WriteLog;
        }

        protected override void OnStart(string[] args)
        {
            AspectF.Define.Retry()
                .Log(UtilityShare.Instance.WriteLog, "OnStart", "启动成功！")
                .Do(() =>
                {
                    if(wcfHost!=null)
                    {
                        wcfHost.Close();
                    }
                    wcfHost = new ServiceHost(typeof(CTMDal));
                    if(wcfHost.State!=CommunicationState.Opened)
                    {
                        wcfHost.Open();
                    }
                });
        }

        protected override void OnStop()
        {
            AspectF.Define.Retry()
                .Log(UtilityShare.Instance.WriteLog, "", "关闭成功！")
                .Do(() =>
                {
                    wcfHost.Close();
                    wcfHost = null;
                });
        }
    }
}
