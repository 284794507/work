using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Model
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
    }
}
