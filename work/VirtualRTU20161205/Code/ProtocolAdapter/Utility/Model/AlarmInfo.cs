using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Model
{
    [Serializable]
    public class AlarmInfo
    {
        //Bit0 防雷器故障
        //Bit1 门磁
        //Bit2 光源故障
        //Bit3 继电器故障
        //Bit4 电容故障
        //Bit5 电流过大报警
        //Bit6 功率过大报警
        //Bit7 漏电压报警
        //Bit8 漏电流报警
        //Bit9 ~Bit15  保留
        public byte[,] ChAlarmInfo { get; set; }    
        
        public DateTime curTime { get; set; } 
        
        public AlarmInfo()
        {
            int len1 = 16, len2 = 2;
            this.ChAlarmInfo = new byte[len1, len2];
            for(int i=0;i< len1; i++)
            {
                for(int j=0;j< len2; j++)
                {
                    this.ChAlarmInfo[i, j] = 0x00;
                }
            }             
        }  
        
        public void BuildAlarmInfo(byte[]data)
        {
            AspectF.Define.Retry().Do(() =>
            {
                int curIndex = 0;

                byte seq = data[curIndex++];
                byte[] time = new byte[7];
                if (ByteHelper.GetBit(seq, 0) == 1)
                {
                    Buffer.BlockCopy(data, 1, time, 0, 7);
                    this.curTime = ByteHelper.Bytes7ToDateTime(time, false);
                    curIndex += 7;
                }
                else
                {
                    this.curTime = DateTime.Now;
                }

                if (ByteHelper.GetBit(seq, 1) == 1)
                {
                    curIndex += 2;
                }

                for (int i = 0; i < 16; i++)
                {
                    this.ChAlarmInfo[i, 0] = data[curIndex++];
                    this.ChAlarmInfo[i, 1] = data[curIndex++];
                }
            });
        }
    }
}
