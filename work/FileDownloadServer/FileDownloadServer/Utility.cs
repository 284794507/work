using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Database;
using System.Logger;

namespace FileDownloadServer
{
    public class Utility
    {
        static SimpleLogger SimpleLog = SimpleLogger.GetInstance();

        public static void WriteLog(string msg, int lvl = 2)
        {
            switch (lvl)
            {
                case 1:
                    SimpleLog.Debug(msg);
                    break;
                case 2:
                    SimpleLog.Info(msg);
                    break;
                case 3:
                    SimpleLog.Warn(msg);
                    break;
                case 4:
                    SimpleLog.Error(msg);
                    break;
                case 5:
                    SimpleLog.Fatal(msg);
                    break;
            }
            Console.WriteLine(DateTime.Now.ToString() + " : " + msg);
        }

        public static void CatchExpection(Exception ex)
        {
            WriteLog(ex.Message, 4);
        }

        public static string BaseAddr;
        public static string Sver_IntelligentSingleLamp;
        public static string FilePath_IntelligentSingleLamp;
        public static string FilePath_mvn_smartswitch;
        public static string FilePath_CTM_App;

        public static void InitConfig()
        {
            AspectF.Define.Retry(CatchExpection)
                .Do(()=>{
                string val = "";

                val = ConfigurationManager.AppSettings["BaseAddr"];
                BaseAddr = val;

                val = ConfigurationManager.AppSettings["Sver_IntelligentSingleLamp"];
                Sver_IntelligentSingleLamp = val;

                val = ConfigurationManager.AppSettings["FilePath_IntelligentSingleLamp"];
                FilePath_IntelligentSingleLamp = val;

                val = ConfigurationManager.AppSettings["FilePath_mvn_smartswitch"];
                FilePath_mvn_smartswitch = val;

                val = ConfigurationManager.AppSettings["FilePath_CTM_App"];
                FilePath_CTM_App = val;

            });
        }
    }
}
