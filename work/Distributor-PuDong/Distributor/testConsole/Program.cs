using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace testConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();
            Share.Instance.WriteMsg += ShowLog;
            Share.Instance.WriteMsg += Share.Instance.WriteLog;
            // TODO: 在此处添加代码以启动服务。
            if (!bootstrap.Initialize())
            {
                Share.Instance.WriteMsg("Failed to initialize!", 2);
                Console.ReadKey();
                return;
            }

            var result = bootstrap.Start();
            Share.Instance.WriteMsg(string.Format("Start result: {0}!", result), 2);

            if (result == StartResult.Failed)
            {
                Share.Instance.WriteMsg("Failed to start!", 2);
                Console.ReadKey();
                return;
            }

            Console.ReadLine();
            bootstrap.Stop();
        }
        private static void ShowLog(string msg, int lvl)
        {
            if (string.IsNullOrEmpty(msg)) return;
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine(msg);
        }
    }
}
