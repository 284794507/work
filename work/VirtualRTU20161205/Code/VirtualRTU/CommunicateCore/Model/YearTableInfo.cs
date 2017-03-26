using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Model
{
    [Serializable]
    public class YearTableInfo
    {
        public byte StartMonth { get; set; }

        public byte StartDay { get; set; }

        public byte[] DayNum { get; set; }

        public OpenAndCloseTime[] PlanTime { get; set; }

        public YearTableInfo()
        {
            this.DayNum = new byte[2];
        }

        public void BuildYearTableInfo(byte[] data)
        {
            int curIndex = 0;
            this.StartMonth = data[curIndex++];
            this.StartDay = data[curIndex++];
            Buffer.BlockCopy(data, curIndex, this.DayNum, 0, 2);
            curIndex += 2;
            short num = BitConverter.ToInt16(this.DayNum, 0);
            this.PlanTime = new OpenAndCloseTime[num];
            for(int i=0;i<num;i++)
            {
                Buffer.BlockCopy(data, curIndex, this.PlanTime[i].OpenTime, 0, 2);
                curIndex += 2;
                Buffer.BlockCopy(data, curIndex, this.PlanTime[i].CloseTime, 0, 2);
                curIndex += 2;
            }
        }

        public byte[] ToSetBytes()
        {
            byte[] result = new byte[1+1+2+BitConverter.ToInt16(this.DayNum,0)*2];
            int curIndex = 0;
            result[curIndex++] = this.StartMonth;
            result[curIndex++] = this.StartDay;
            Buffer.BlockCopy(this.DayNum, 0, result, curIndex, 2);
            curIndex += 2;
            short num = BitConverter.ToInt16(this.DayNum, 0);
            for (int i = 0; i < num; i++)
            {
                Buffer.BlockCopy(this.PlanTime[i].OpenTime, curIndex, result, 0, 2);
                curIndex += 2;
                Buffer.BlockCopy(this.PlanTime[i].CloseTime, curIndex, result, 0, 2);
                curIndex += 2;
            }
            return result;
        }

        public byte[] ToQueryBytes()
        {
            byte[] result = new byte[1 + 1 + 2];
            int curIndex = 0;
            result[curIndex++] = this.StartMonth;
            result[curIndex++] = this.StartDay;
            Buffer.BlockCopy(this.DayNum, 0, result, curIndex, 2);
            return result;
        }
    }

    [Serializable]
    public class OpenAndCloseTime
    {
        public byte[] OpenTime { get; set; }

        public byte[] CloseTime { get; set; }

        public OpenAndCloseTime()
        {
            this.OpenTime = new byte[2];

            this.CloseTime = new byte[2];
        }
    }
}
