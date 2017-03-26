using BrokerSocket.Command;
using Newtonsoft.Json;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Model;

namespace BrokerSocket
{
    public class BrokerReceiveFilter: BeginEndMarkReceiveFilter<BinaryRequestInfo>
    {
        private readonly static byte[] begeinMark = new byte[] { (byte)'!' };
        private readonly static byte[] endMark = new byte[] { (byte)'$' };

        public BrokerReceiveFilter():base(begeinMark,endMark)
        {

        }

        protected override BinaryRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            string flag = "";
            byte[] data = new byte[length - 2];
            AspectF.Define.Retry()
                .Log(UtilityHelper.GetHelper.WriterLog,"", data.Length>0?"Recevice Message from Broker!":"")
                .Do(() =>
                {
                    Buffer.BlockCopy(readBuffer, offset + 1, data, 0, length - 2);
                    flag = ProtocolType.LFI.ToString();
                });
            return new BinaryRequestInfo(flag, data);
            //if(length>2000)
            //{
            //    data = new byte[200];
            //    Buffer.BlockCopy(readBuffer, offset + 1, data, 0, 200);
            //}
            //else
            //{

            //}

            //string jsonStr = Encoding.UTF8.GetString(readBuffer, offset + 1, length - 2);
            //BrokerMessage bMsg = JsonSerializeHelper.GetHelper.Deserialize<BrokerMessage>(jsonStr);
            //data = bMsg.ToBytes();
            //ToLFIProtocol.curMsg = bMsg;
            //return new BinaryRequestInfo(flag, new byte[1] { 1 });
        }
    }
    //public class BrokerReceiveFilter:FixedHeaderReceiveFilter<BinaryRequestInfo>
    //{
    //    private readonly static byte beginMark = 0x68;
    //    private readonly static int headLen = 3;
    //    public BrokerReceiveFilter():base(headLen)
    //    {

    //    }

    //    protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
    //    {
    //        return (BitConverter.ToInt16(header, offset + 1) + 8 + 1 + 2 + 1);
    //    }

    //    protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
    //    {
    //        byte head = header.Array[header.Offset];
    //        if (head != beginMark)
    //        {
    //            Console.WriteLine("Broker Sync Mark Error！");
    //            return null;
    //        }

    //        byte[] data = new byte[length + headLen];
    //        Buffer.BlockCopy(header.Array, header.Offset, data, 0, headLen);
    //        Buffer.BlockCopy(bodyBuffer, offset, data, headLen, length);
    //        Console.WriteLine(DateTime.Now.ToString());
    //        Console.WriteLine("Recevice From Broker:");
    //        Console.WriteLine(ByteHelper.ByteToHexStrWithDelimiter(data, "", false));
    //        string flag = "";
    //        flag = "Virtual";
    //        return new BinaryRequestInfo(flag, data);
    //    }
    //}
}
