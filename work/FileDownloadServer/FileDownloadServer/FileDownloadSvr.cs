using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace FileDownloadServer
{
    public partial class FileDownloadSvr : ServiceBase
    {
        HttpSelfHostServer SelfHost;
        HttpSelfHostConfiguration SelfConfig;
        public FileDownloadSvr()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Utility.InitConfig();

            SelfConfig = new HttpSelfHostConfiguration(Utility.BaseAddr);
            SelfConfig.MaxBufferSize = int.MaxValue;
            SelfConfig.MaxReceivedMessageSize = int.MaxValue;
            SelfConfig.MapHttpAttributeRoutes();
            SelfConfig.Routes.MapHttpRoute("Route", "Api/{controller}/{value}", new { value = RouteParameter.Optional });
            SelfConfig.TransferMode = System.ServiceModel.TransferMode.Streamed;

            SelfHost = new HttpSelfHostServer(SelfConfig);
            SelfHost.OpenAsync().Wait();
        }

        protected override void OnStop()
        {
        }
    }
}
