
using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace CommunicateCore.Model
{
    public class SetTimeInfo
    {
        public byte HYear { get; set; }

        public byte LYear { get; set; }

        public byte Month { get; set; }

        public byte Day { get; set; }

        public byte Hour { get; set; }

        public byte Minute { get; set; }

        public byte Second { get; set; }

        public byte[] ToBytes()
        {
            byte[] result = new byte[8];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    result[0] = 0;//SEQ
                    result[1] = this.HYear;
                    result[2] = this.LYear;
                    result[3] = this.Month;
                    result[4] = this.Day;
                    result[5] = this.Hour;
                    result[6] = this.Minute;
                    result[7] = this.Second;
                });

            return result;
        }

        public void BuildTime(DateTime curTime)
        {
            byte[] bTime= ByteHelper.DateTimeToBytes_7(curTime);
            this.HYear = bTime[0];
            this.LYear = bTime[1];
            this.Month = bTime[2];
            this.Day = bTime[3];
            this.Hour = bTime[4];
            this.Minute = bTime[5];
            this.Second = bTime[6];
        }
    }
}
