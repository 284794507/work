using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr.RTUSvrBusiness
{
    public class QueryTimeHandler
    {//时间查询
        private static QueryTimeHandler _QueryTimeHandler;
        public static QueryTimeHandler GetHandler
        {
            get
            {
                if(_QueryTimeHandler==null)
                {
                    _QueryTimeHandler = new QueryTimeHandler();
                }
                return _QueryTimeHandler;
            }
        }

        public void HandlerQueryTimePackage(LH_PackageData package)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte[] data = new byte[7];
                    DateTime curTime = DateTime.Now;
                    data[0] = (byte)((byte)(curTime.Second / 10) << 4 | (byte)(curTime.Second % 10));
                    data[1] = (byte)((byte)(curTime.Minute / 10) << 4 | (byte)(curTime.Minute % 10));
                    data[2] = (byte)((byte)(curTime.Hour / 10) << 4 | (byte)(curTime.Hour % 10));
                    data[3] = (byte)((byte)(curTime.Day / 10) << 4 | (byte)(curTime.Day % 10));
                    data[4] = (byte)((byte)(curTime.Month / 10) << 4 | (byte)(curTime.Month % 10));
                    int year = curTime.Year % 100;
                    data[5] = (byte)((byte)(year / 10) << 4 | (byte)(year % 10));
                    year = curTime.Year / 100;
                    data[6] = (byte)((byte)(year / 10) << 4 | (byte)(year % 10));
                    LH_PackageData back = new LH_PackageData(package.CtuAddr, data, LHCmdWordConst.RecvQueryCTUTime);
                    RTUSvrShare.GetShare.SendToRTUSvr(back);
                });
        }
    }
}
