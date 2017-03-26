using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Model
{
    [Serializable]
    public class TerminalBasicInfo
    {
        public byte[] SoftVersion { get; set; }

        public byte[] HardVersion { get; set; }

        public byte[] ProtocolVersion { get; set; }

        public byte CommType { get; set; }

        public byte ChNum { get; set; }

        public SetTimeInfo DateOfProduction { get; set; }

        public TerminalBasicInfo()
        {
            this.SoftVersion = new byte[2];
            this.HardVersion = new byte[2];
            this.ProtocolVersion = new byte[2];
        }

        public byte[] ToBytes()
        {
            byte[] result = new byte[15];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    int curIndex = 0;
                    Buffer.BlockCopy(this.SoftVersion, 0, result, curIndex, 2);
                    curIndex += 2;
                    Buffer.BlockCopy(this.HardVersion, 0, result, curIndex, 2);
                    curIndex += 2;
                    Buffer.BlockCopy(this.ProtocolVersion, 0, result, curIndex, 2);
                    curIndex += 2;
                    result[curIndex++] = this.CommType;
                    result[curIndex++] = this.ChNum;
                    Buffer.BlockCopy(this.DateOfProduction.ToBytes(), 0, result, curIndex, 7);
                });

            return result;
        }
    }
}
