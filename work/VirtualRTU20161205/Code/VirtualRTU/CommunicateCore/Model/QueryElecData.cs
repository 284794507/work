using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Model
{
    [Serializable]
    public class QueryElecData
    {
        public byte StartChNo { get; set; }

        public byte ChNum { get; set; }

        public DateTime curTime { get; set; }

        public ElecData[] ArrElecData { get; set; }

        public void BuildElecDataInfo(byte[] data)
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

                this.StartChNo = data[curIndex++];
                this.ChNum = data[curIndex++];
                this.ArrElecData = new ElecData[this.ChNum];
                for (int i = 0; i < this.ChNum; i++)
                {
                    ElecData elec = new ElecData();
                    elec.OptValue = data[curIndex++];
                    Buffer.BlockCopy(data, curIndex, elec.Voltage, 0, 2);
                    curIndex += 2;
                    Buffer.BlockCopy(data, curIndex, elec.Current, 0, 2);
                    curIndex += 2;
                    Buffer.BlockCopy(data, curIndex, elec.ActivePower, 0, 2);
                    curIndex += 2;
                    Buffer.BlockCopy(data, curIndex, elec.AppPower, 0, 2);
                    curIndex += 2;
                    Buffer.BlockCopy(data, curIndex, elec.LeakVoltage, 0, 2);
                    curIndex += 2;
                    Buffer.BlockCopy(data, curIndex, elec.LeakCurrent, 0, 2);
                    curIndex += 2;

                    this.ArrElecData[i] = elec;
                }
            });
        }
    }

    [Serializable]
    public class ElecData
    {
        public byte OptValue { get; set; }

        public byte[] Voltage { get; set; }

        public byte[] Current { get; set; }

        public byte[] ActivePower { get; set; }

        public byte[] AppPower { get; set; }

        public byte[] LeakVoltage { get; set; }

        public byte[] LeakCurrent { get; set; }
    }
}
