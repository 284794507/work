using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Model
{
    [Serializable]
    public class BrokerMessage
    {
        public MessageType MsgType { get; set; }

        public MessageLevel MsgLvl { get; set; }

        public byte[] TerminalAddress { get; set; }

        public Object MsgBody { get; set; }

        public BrokerMessage()
        {
            this.MsgLvl = 0;
        }

        public byte[] ToBytes()
        {
            byte[] data = new byte[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    BrokerMessage bMsg = new BrokerMessage();
                    string jsonStr = JsonConvert.SerializeObject(this);
                    data = Encoding.UTF8.GetBytes('!' + jsonStr + '$');
                });
            return data;
        }
    }
}
