using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace TerminalSocket
{
    public class TerminalReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        private readonly static byte beginMark = 0x68;
        private readonly static int headLen = 3;

        public TerminalReceiveFilter():base(headLen)
        {

        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return (BitConverter.ToInt16(header, offset + 1) + 8 + 1 + 2 + 1);
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            byte head = header.Array[header.Offset];
            if(head != beginMark)
            {
                UtilityHelper.GetHelper.WriteLog_RTUSvr("终端报文同步字错误！");
                return null;
            }

            byte[] data = new byte[length + headLen];
            Buffer.BlockCopy(header.Array, header.Offset, data, 0, headLen);
            Buffer.BlockCopy(bodyBuffer, offset, data, headLen, length);
            //UtilityHelper.GetHelper.WriteLog_RTUSvr(DateTime.Now.ToString());
            //Console.WriteLine("Recevice From Terminal:");
            //Console.WriteLine(ByteHelper.ByteToHexStrWithDelimiter(data,"",false));
            string flag = "";
            flag= ProtocolType.LFI.ToString(); 
            return new BinaryRequestInfo(flag, data);
        }
    }
}
