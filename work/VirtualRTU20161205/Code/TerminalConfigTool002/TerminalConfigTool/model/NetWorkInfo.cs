using CommunicateCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalConfigTool
{
    [Serializable]
    public class NetWorkInfo
    {
        public byte SetNum { get; set; }

        public SetInfo[] SetInfo {get;set;}

        public byte[] ToBytes()
        {
            byte[] result = new byte[0];
            int curIndex = 0;
            int len = 2;
            for (int i = 0; i < SetNum; i++)
            {
                len += 1 + 1 + (byte)this.SetInfo[i].data.Length;
            }
            result = new byte[len];
            result[curIndex++] = 0;//SEQ
            result[curIndex++] = SetNum;
            for (int i = 0; i < SetNum; i++)
            {
                result[curIndex++] = this.SetInfo[i].ID;
                byte curLen = (byte)this.SetInfo[i].data.Length;
                result[curIndex++] = curLen;
                Buffer.BlockCopy(this.SetInfo[i].data, 0, result, curIndex, curLen);
                curIndex += curLen;
            }
            return result;
        }

        public void BuildNetWorkInfo(byte[] data)
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
            this.SetNum = data[curIndex++];
            this.SetInfo = new SetInfo[this.SetNum];
            int num = this.SetNum;
            for (int i = 0; i < num; i++)
            {
                SetInfo info = new SetInfo();
                info.ID = data[curIndex++];
                int len = data[curIndex++];
                info.data = new byte[len];
                Buffer.BlockCopy(data, curIndex, info.data, 0, len);
                curIndex += len;

                this.SetInfo[i] = info;
            }
        }
    }

    [Serializable]
    public class SetInfo
    {
        public byte ID { get; set; }

        public byte[] data { get; set; }
    }
}
