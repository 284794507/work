using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace PlatFormSocket
{
    public class PlatFormReceiveFilter:FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        private readonly static byte BeginMark = 0x68;
        private readonly static int headLen = 3;

        public PlatFormReceiveFilter():base(headLen)
        {

        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return (BitConverter.ToInt16(header, offset + 1)+9- headLen);//欣能协议包长度为数据长度+9
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            byte[] data = new byte[0];
            AspectF.Define.Retry(Share.Instance.CatchExpection, (() => {
                Share.Instance.LogInfo("PlatForm:ResolveRequestInfo");
            }))
                .Do(() =>
                {
                    byte head = header.Array[header.Offset];
                    if (head != BeginMark)
                    {
                        Share.Instance.WriteMsg("BeginMark is error!", 3);
                    }

                    data = new byte[length + headLen];
                    Buffer.BlockCopy(header.Array, header.Offset, data, 0, headLen);
                    Buffer.BlockCopy(bodyBuffer, offset, data, headLen, length);
                    
                    Share.Instance.WriteMsg("Receive from Platform，ID：" + PackageHelper.GetPackageHelper.GetAddressFromXNProtocol(data), 2);
                    Share.Instance.WriteMsg(ByteHelper.ByteToHexStrWithDelimiter(data, " ", false), 2);
                });
            return new BinaryRequestInfo("XN", data);
        }
    }
}
