using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Model
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
            byte seq = data[curIndex++];
            if (ByteHelper.GetBit(seq, 0) == 1)
            {
                curIndex += 7;
            }

            if (ByteHelper.GetBit(seq, 1) == 1)
            {
                curIndex += 2;
            }

            this.StartMonth = data[curIndex++];
            this.StartDay = data[curIndex++];
            this.DayNum[1]= data[curIndex++];
            this.DayNum[0] = data[curIndex++];
            short num = BitConverter.ToInt16(this.DayNum, 0);
            this.PlanTime = new OpenAndCloseTime[num];
            for(int i=0;i<num;i++)
            {
                OpenAndCloseTime newTime = new OpenAndCloseTime();
                Buffer.BlockCopy(data, curIndex, newTime.OpenTime, 0, 2);
                curIndex += 2;
                Buffer.BlockCopy(data, curIndex, newTime.CloseTime, 0, 2);
                this.PlanTime[i]=newTime;
                curIndex += 2;
            }
        }

        public byte[] ToSetBytes()
        {
            short num = BitConverter.ToInt16(this.DayNum, 0);
            int startNo = 0;
            if(num>200)
            {
                if(DivPackagePara.total> DivPackagePara.curNo)
                {
                    num = 200;
                }
                else
                {
                    num = (short)(num - (DivPackagePara.curNo - 1) * 200);
                }
                this.DayNum = BitConverter.GetBytes(num);
                startNo = (DivPackagePara.curNo - 1) * 200;
                DateTime curTime = new DateTime(DateTime.Now.Year, this.StartMonth, this.StartDay);
                curTime = curTime.AddDays(startNo);
                this.StartMonth = (byte)curTime.Month;
                this.StartDay = (byte)curTime.Day;
            }
            byte[] result = new byte[1 + 1 + 1 + 2 + num * 4];
            int curIndex = 0;
            result[curIndex++] = 0;//SEQ
            result[curIndex++] = Convert.ToByte(this.StartMonth.ToString(), 16); //this.StartMonth;
            result[curIndex++] = Convert.ToByte(this.StartDay.ToString(), 16); //this.StartDay;
            result[curIndex++] = this.DayNum[1];
            result[curIndex++] = this.DayNum[0];
            
            for (int i = 0; i < num; i++)
            {
                result[curIndex++] = this.PlanTime[startNo + i].OpenTime[1];
                result[curIndex++] = this.PlanTime[startNo + i].OpenTime[0];

                result[curIndex++] = this.PlanTime[startNo + i].CloseTime[1];
                result[curIndex++] = this.PlanTime[startNo + i].CloseTime[0];
                //Buffer.BlockCopy(this.PlanTime[startNo+i].OpenTime, 0, result, curIndex, 2);
                //curIndex += 2;
                //Buffer.BlockCopy(this.PlanTime[startNo+i].CloseTime, 0, result, curIndex, 2);
                //curIndex += 2;
            }
            return result;
        }

        public byte[] ToQueryBytes()
        {
            byte[] result = new byte[1 + 1 + 1 + 2];
            int curIndex = 0;
            result[curIndex++] = 0;//SEQ
            result[curIndex++] = Convert.ToByte(this.StartMonth.ToString(), 16); //this.StartMonth;
            result[curIndex++] = Convert.ToByte(this.StartDay.ToString(), 16); //this.StartDay;
            result[curIndex++] = this.DayNum[1];
            result[curIndex++] = this.DayNum[0];
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
