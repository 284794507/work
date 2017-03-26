using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Model
{
    [Serializable]
    public class RealTimeCtrlLamp
    {
        public byte ChNo { get; set; }

        public byte OptValue { get; set; }

        public byte IsLock { get; set; }
    }
}
