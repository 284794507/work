using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Model
{
    [Serializable]
    public class RealTimeCtrlLamp
    {
        public byte ChNo { get; set; }

        public byte OptValue { get; set; }

        public byte IsLock { get; set; }

        public void BuildRealTimeCtrlInfo(byte[] data)
        {
            this.ChNo = data[0];
            this.OptValue = data[1];
            this.IsLock = data[2];
        }

        public byte[] ToBytes()
        {
            byte[] data = new byte[4];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    data[0] = 0;//SEQ
                    data[1] = this.ChNo;
                    data[2] = this.OptValue;
                    data[3] = this.IsLock;
                });

            return data;
        }
    }
}
