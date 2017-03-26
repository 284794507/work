using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace BackEndSocket
{
    public class BackEndReceiveFilter: SubFixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        private readonly static byte BeginMark = 0x86;
        private readonly static byte EndMark = 0x16;
        private readonly static int HeadLen = 3;

        public BackEndReceiveFilter():base(HeadLen)
        {

        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            if (header[offset] != BeginMark)
            {
                Share.Instance.WriteLog("BeginMark is error!", 4);
                return -1;
            }
            else
            {
                //LL=业务数据长度+2
                //帧头(1)+LL(2)+Addr(8)+sync(1)+seq(1)+usart(1)+data+crc(2)+帧尾(1)
                return (BitConverter.ToUInt16(header, offset + 1) + 8 + 1 + 1 + 1 + 1);
            }
        }    

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            return AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Return<BinaryRequestInfo>(() =>
                {
                    if(length==-1)
                    {
                        return NullRequestInfo;
                    }
                    byte[] data = new byte[0];

                    data = new byte[length + HeadLen];
                    Buffer.BlockCopy(header.Array, header.Offset, data, 0, HeadLen);
                    Buffer.BlockCopy(bodyBuffer, offset, data, HeadLen, length);
                    Share.Instance.WriteLog("Receive from BackEnd : " + ByteHelper.ByteToHexStrWithDelimiter(data, " ", false));
                    if (data[length + HeadLen - 1] != EndMark)
                    {
                        Share.Instance.WriteLog("EndMark is error!", 4);
                        return NullRequestInfo;
                    }

                    return new BinaryRequestInfo("Flow", data);

                });
        }
    }
}
