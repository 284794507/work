using LFCDal.WCFSvr;
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

namespace LFCDal
{
    public partial class DalService : ServiceBase
    {
        private static ServiceHost wcfHost = null;//wcf寄宿服务对象

        public DalService()
        {
            InitializeComponent();
            this.ServiceName = "LFCDalService";
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if(wcfHost!=null)
                {
                    wcfHost.Close();
                }
                wcfHost = new ServiceHost(typeof(DBDal));
                if (wcfHost.State!=CommunicationState.Opened)
                {
                    wcfHost.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("启动成功！");
        }

        protected override void OnStop()
        {
            if(wcfHost!=null)
            {
                wcfHost.Close();
                wcfHost = null;
            }

            Console.WriteLine("关闭成功！");
        }
    }
}
