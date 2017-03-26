using PassengerFlowDal.Model;
using PassengerFlowServer.PackageHandler;
using PassengerFlowServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerFlowServer.BusinessHandler
{
    public class CheckTimeHandler
    {
        private static CheckTimeHandler _CheckTimeHandler;
        public static CheckTimeHandler Instance
        {
            get
            {
                if(_CheckTimeHandler==null)
                {
                    _CheckTimeHandler = new CheckTimeHandler();
                }
                return _CheckTimeHandler;
            }
        }

        public static long[] deltaT=new long[] { 0, 0, 0 };

        public void CheckTimePkgHandler(SubPackage pkg, TabTimeSync info)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    uint rtc = BitConverter.ToUInt32(pkg.OnlyData, 0);
                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                    DateTime curTime = DateTime.Now;
                    TimeSpan ts = curTime - startTime;
                    long millisecond = (long)ts.TotalMilliseconds;
                    deltaT[info.ModuleNo - 1] = millisecond - rtc;
                    
                    info.DevTime = rtc;
                    info.ServerTime = curTime;
                    info.DeltaTime = deltaT[info.ModuleNo - 1];
                                
                });
        }
    }
}
