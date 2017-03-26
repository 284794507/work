using LampCensus.Utility;
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
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace LampCensus
{
    partial class LampCensusService : ServiceBase
    {
        public static HttpSelfHostConfiguration _config = null;
        public static HttpSelfHostServer _host = null;

        public LampCensusService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            Share.Instance.InitConfig();

            _config = new HttpSelfHostConfiguration("http://192.168.1.6:60180");
            _config.MapHttpAttributeRoutes();
            _config.Routes.MapHttpRoute("Route1", "api/{controller}/{value}", new { value = RouteParameter.Optional });
            //_config.Routes.MapHttpRoute("Route2", "api/{controller}/{action}/{value}", new { value = RouteParameter.Optional });
            //_config.Routes.MapHttpRoute("Route3", "api/{controller}/{action}/{value}", new { action = RouteParameter.Optional, value = RouteParameter.Optional });

            _config.MaxReceivedMessageSize = int.MaxValue;
            _config.TransferMode = TransferMode.Streamed;
            _host = new HttpSelfHostServer(_config);
            _host.OpenAsync().Wait();
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            _host.CloseAsync().Wait();
        }
    }
}
