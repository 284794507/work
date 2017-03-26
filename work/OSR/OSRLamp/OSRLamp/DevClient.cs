using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRLamp
{
    public class DevClient
    {
        public string StrID { get; set; }

        public byte[] TerminalID { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime HeartBeatTime { get; set; }
    }
}
