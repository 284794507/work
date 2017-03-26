using CTMSvr.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace CTMSvr
{
    class Program
    {
        public static HttpSelfHostConfiguration _config = null;
        public static HttpSelfHostServer _host = null;

        static void Main(string[] args)
        {
            Share.Instance.InitConfig();

            _config = new HttpSelfHostConfiguration("http://192.168.1.6:60190");
            _config.MapHttpAttributeRoutes();
            _config.Routes.MapHttpRoute("Route1", "api/{controller}/{value}", new { value = RouteParameter.Optional });
            //_config.Routes.MapHttpRoute("Route2", "api/{controller}/{action}/{value}", new { value = RouteParameter.Optional });
            //_config.Routes.MapHttpRoute("Route3", "api/{controller}/{action}/{value}", new { action = RouteParameter.Optional, value = RouteParameter.Optional });

            _config.MaxReceivedMessageSize = long.MaxValue;// int.MaxValue;
            _config.MaxBufferSize = int.MaxValue;

            _config.TransferMode = TransferMode.Streamed;
            _host = new HttpSelfHostServer(_config);
            _host.OpenAsync().Wait();

            Console.ReadLine();
            _host.CloseAsync().Wait();
        }
    }
}
