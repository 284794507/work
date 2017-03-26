using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Model
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

        public BrokerMessage(MessageType mType,MessageLevel mLvl,byte[] addr,object mBody)
        {
            this.MsgType = mType;
            this.MsgLvl = mLvl;
            this.TerminalAddress = addr;
            this.MsgBody = mBody;
        }

        public byte[] ToBytes()
        {
            byte[] data = new byte[0];
            BrokerMessage bMsg = new BrokerMessage();
            string jsonStr = JsonConvert.SerializeObject(this);
            data = Encoding.UTF8.GetBytes('!' + jsonStr + '$');

            return data;
        }

        //public bool BuildSuccessed { get; set; }
    }
}
