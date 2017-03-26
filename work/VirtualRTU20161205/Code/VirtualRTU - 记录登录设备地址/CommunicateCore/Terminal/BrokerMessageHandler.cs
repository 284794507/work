using CommunicateCore.Model;
using CommunicateCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Terminal
{
    public class BrokerMessageHandler
    {
        private static BrokerMessageHandler _BrokerMessageHandler;
        public static BrokerMessageHandler GetHandler
        {
            get
            {
                if(_BrokerMessageHandler==null)
                {
                    _BrokerMessageHandler = new BrokerMessageHandler();
                }
                return _BrokerMessageHandler;
            }
        }

        private readonly static byte[] begeinMark = new byte[] { (byte)'!' };
        private readonly static byte[] endMark = new byte[] { (byte)'$' };

        public BrokerMessage BuildMessage(byte[] data)
        {
            BrokerMessage bMsg = null;
            AspectF.Define.Retry()
                .MustBeNonNull(data)
                .Do(() =>
                {
                    int len = data.Length;
                    if(len>2 && data[0]==begeinMark[0] && data[len-1]==endMark[0])
                    {
                        string jsonStr = Encoding.UTF8.GetString(data, 1, len-2);

                        JsonSerializer serializer = new JsonSerializer();
                        StringReader sr = new StringReader(jsonStr);
                        object obj = serializer.Deserialize(new JsonTextReader(sr), typeof(BrokerMessage));
                        bMsg = obj as BrokerMessage;
                    }
                });

            return bMsg;
        }
    }
}
