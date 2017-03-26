using Communication_Core;
using Communication_Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTerminal
{
    class Program
    {
        static void Main(string[] args)
        {
            Share.Instance.WriteMsg += ShowLog;

            DistriCommClient.Instance.InitClient();

            Console.ReadLine();
        }

        private static void  ShowLog(string msg,int lvl)
        {
            if (string.IsNullOrEmpty(msg)) return;
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine(msg);
        }
    }
}
