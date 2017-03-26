using System;
using System.Collections.Generic;
using System.Linq;
using System.Logger;
using System.Text;
using System.Threading.Tasks;

namespace CTM_Route_Utility
{
    public class Route_Utility
    {
        private static Route_Utility _Route_Utility;
        public static Route_Utility Instance
        {
            get
            {
                if(_Route_Utility==null)
                {
                    _Route_Utility = new Route_Utility();
                }
                return _Route_Utility;
            }
        }

        static SimpleLogger Route_Log = SimpleLogger.GetInstance();
        
        public delegate void WriteMsg_Route(string msg,int lvl);
        public WriteMsg_Route WriteLog_Route;
        public void WriterLog(string msg, int lvl)
        {
            switch(lvl)
            {
                case 1:
                    Route_Log.Debug(msg);
                    break;
                case 2:
                    Route_Log.Info(msg);
                    break;
                case 3:
                    Route_Log.Warn(msg);
                    break;
                case 4:
                    Route_Log.Error(msg);
                    break;
                case 5:
                    Route_Log.Fatal(msg);
                    break;
            }
        }

        public void CatchExpection(Exception ex)
        {
            WriterLog(ex.Message,4);
        }
    }
}
