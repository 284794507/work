using OSRLamp.PackageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRLamp.BusinessHandler
{
    public class DateTimeHandler
    {
        private static DateTimeHandler _DateTimeHandler;
        public static DateTimeHandler Instance
        {
            get
            {
                if (_DateTimeHandler == null)
                {
                    _DateTimeHandler = new DateTimeHandler();
                }
                return _DateTimeHandler;
            }
        }


        /// <summary>
        /// 设置日历时钟
        /// </summary>
        /// <param name="terminalID"></param>
        /// <param name="mode"></param>
        public void SetDateTime(byte[] terminalID)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    byte[] data = new byte[6];
                    data = ByteHelper.DateTimeToBytes6(DateTime.Now);
                    NJPackageData sendPkg = new NJPackageData(CmdWord.Set_Datetime, terminalID, data);

                    Utility.SendDataToTerminal(sendPkg);
                });
        }
    }
}
