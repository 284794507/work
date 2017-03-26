using System;
using System.Collections.Generic;
using System.Linq;
using System.Logger;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class Share
    {
        private static Share _Share;
        public static Share Instance
        {
            get
            {
                if(_Share==null)
                {
                    _Share = new Share();
                }
                return _Share;
            }
        }

        private static SimpleLogger Simple_Log = SimpleLogger.GetInstance();

        public delegate void PrintMsg(string msg, int lvl);
        public PrintMsg WriteMsg;
        public void WriteLog(string msg,int lvl)
        {
            switch (lvl)
            {
                case 1:
                    Simple_Log.Debug(msg);
                    break;
                case 2:
                    Simple_Log.Info(msg);
                    break;
                case 3:
                    Simple_Log.Warn(msg);
                    break;
                case 4:
                    Simple_Log.Error(msg);
                    break;
                case 5:
                    Simple_Log.Fatal(msg);
                    break;
            }
        }

        public void LogInfo(string msg)
        {
            WriteMsg(msg, 2);
        }

        public void CatchExpection(Exception ex)
        {
            WriteMsg(ex.Message, 4);
        }
    }
}
