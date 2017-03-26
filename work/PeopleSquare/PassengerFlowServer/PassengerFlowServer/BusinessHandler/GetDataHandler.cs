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
    public class GetDataHandler
    {
        private static GetDataHandler _GetDataHandler;
        public static GetDataHandler Instance
        {
            get
            {
                if (_GetDataHandler == null)
                {
                    _GetDataHandler = new GetDataHandler();
                }
                return _GetDataHandler;
            }
        }

        public static long deltaT = 0;

        public void GetCurDataPkgHandler(SubPackage pkg, TabCurData info)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    int curIndex = 0;
                    byte channel = pkg.OnlyData[curIndex++];
                    byte rssi = pkg.OnlyData[curIndex++];
                    byte[] val = new byte[0];
                    val = new byte[6];
                    Buffer.BlockCopy(pkg.OnlyData, curIndex, val, 0, 6);
                    curIndex += 6;
                    string macAddr = ByteHelper.ByteToHexStrWithDelimiter(val, " ", false);
                    Share.Instance.WriteLog(macAddr);
                    val = new byte[4];
                    Buffer.BlockCopy(pkg.OnlyData, curIndex, val, 0, 4);
                    curIndex += 4;
                    uint rtc = BitConverter.ToUInt32(val, 0);
                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                    //DateTime curTime = DateTime.Now;
                    //TimeSpan ts = DateTime.Now - startTime;
                    //long millisecond = (long)ts.TotalMilliseconds;
                    long addTime = CheckTimeHandler.deltaT[info.ModuleNo - 1] + rtc;
                    DateTime realTime = startTime.AddMilliseconds(addTime);
                    if(CheckTimeHandler.deltaT[info.ModuleNo - 1] == 0)
                    {
                        realTime = DateTime.Now;
                    }

                    info.DevTime = rtc;
                    info.RealTime = realTime;
                    info.Channel = channel;
                    info.RSSI = rssi;
                    info.PhoneMac = macAddr;
                    DBHandler.Instance.AddCurData(info);
                });
        }
    }
}
