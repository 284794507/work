using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Model
{
    public class CmdWordTime_Terminal
    {
        public BrokerMessage BMsg { get; set; }

        public DateTime SendTime { get; set; }

        public int Num { get; set; }

        public int No { get; set; }

        public CmdWordTime_Terminal()
        {

        }

        public CmdWordTime_Terminal(BrokerMessage bMsg)
        {
            BMsg = bMsg;
            SendTime = DateTime.Now;
            Num = 1;
        }
    }
}
