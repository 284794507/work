using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Model
{
    [Serializable]
    public class EthernetInterface
    {
        public byte CfgNum { get; set; }

        public ConfigDetail[] CfgDetail { get; set; }

        public byte[] ToBytes()
        {
            byte[] result = new byte[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    int len = 2;
                    foreach (ConfigDetail detail in CfgDetail)
                    {
                        len += 1 +1+ detail.Detail.Length;
                    }
                    result = new byte[len];
                    int curIndex = 0;
                    result[curIndex++] = 0;//SEQ
                    result[curIndex++] = CfgNum;
                    foreach (ConfigDetail detail in CfgDetail)
                    {
                        result[curIndex++] = detail.CfgID;
                        byte curLen = (byte)detail.Detail.Length;
                        result[curIndex++] = curLen;
                        Buffer.BlockCopy(detail.Detail, 0, result, curIndex, curLen);
                        curIndex += curLen;
                    }
                });

            return result;
        }

        public void BuildEthernetInterfaceInfo(byte[] data)
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
            this.CfgNum = data[curIndex++];
            this.CfgDetail = new ConfigDetail[this.CfgNum];
            int num = this.CfgNum;
            for (int i = 0; i < num; i++)
            {
                ConfigDetail info = new ConfigDetail();
                info.CfgID = data[curIndex++];
                int len = data[curIndex++];
                info.Detail = new byte[len];
                Buffer.BlockCopy(data, curIndex, info.Detail, 0, len);
                curIndex += len;

                this.CfgDetail[i] = info;
            }
        }

    }

    public class ConfigDetail
    {
        public byte CfgID { get; set; }
        
        public byte[] Detail { get; set; }
    }
}
