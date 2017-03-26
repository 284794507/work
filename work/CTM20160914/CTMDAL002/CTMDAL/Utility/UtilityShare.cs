using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Logger;
using System.Text;
using System.Threading.Tasks;

namespace CTMDAL.CTMDAL_Utility
{
    public class UtilityShare
    {
        private static UtilityShare _UtilityShare;
        public static UtilityShare Instance
        {
            get
            {
                if(_UtilityShare==null)
                {
                    _UtilityShare = new UtilityShare();
                }
                return _UtilityShare;
            }
        }

        private static string SqlServerConnStr;
        public static string GetSqlConnStr
        {
            get
            {
                if (String.IsNullOrEmpty(SqlServerConnStr))
                {
                    SqlServerConnStr = ConfigurationManager.ConnectionStrings["SqlServerConnStr"].ToString();
                }
                return SqlServerConnStr;
            }
        }


        static SimpleLogger CTM_Log = SimpleLogger.GetInstance();

        //public delegate void ShowLog_CTM(string msg,int lvl);
        //public ShowLog_CTM ShowLog;
        public void WriteLog(string msg,int lvl)
        {
            if (string.IsNullOrEmpty(msg)) return;

            switch(lvl)
            {
                case 1:
                    CTM_Log.Debug(msg);
                    break;
                case 2:
                    CTM_Log.Info(msg);
                    break;
                case 3:
                    CTM_Log.Warn(msg);
                    break;
                case 4:
                    CTM_Log.Error(msg);
                    break;
                case 5:
                    CTM_Log.Fatal(msg);
                    break;
            }
        }

        public void CatchExption(Exception ex)
        {
            WriteLog(ex.Message, 4);
        }        
    }
}
