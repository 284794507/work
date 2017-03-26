using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class UtilityHelper
    {
        private static UtilityHelper _UtilityHelper;
        public static UtilityHelper GetHelper
        {
            get
            {
                if (_UtilityHelper == null)
                {
                    _UtilityHelper = new UtilityHelper();
                }
                return _UtilityHelper;
            }
        }

        public delegate void WriteMsg_RTUSvr(string msg);
        public WriteMsg_RTUSvr WriteLog_RTUSvr;
        //public Action<string> WriterLog;
        public void WriterLog(string msg)
        {
            WriteLog_RTUSvr(msg);
        }

        public void CatchExpection(Exception ex)
        {
            WriterLog(ex.Message);
        }
    }
}
