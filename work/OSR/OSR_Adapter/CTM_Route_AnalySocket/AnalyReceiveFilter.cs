using CTM_Route_Utility;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace CTM_Route_AnalySocket
{
    public class AnalyReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        private readonly static byte BeginMark = 0x68;
        private readonly static int headLen = 3;
        public AnalyReceiveFilter() : base(headLen)
        {

        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return (1 + BitConverter.ToInt16(header, offset + 1) + 2 + 1);
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            byte head = header.Array[header.Offset];
            if (head != BeginMark)
            {
                return null;
            }

            byte[] data = new byte[length + headLen];
            Buffer.BlockCopy(header.Array, header.Offset, data, 0, headLen);
            Buffer.BlockCopy(bodyBuffer, offset, data, headLen, length);
            Route_Utility.Instance.WriteLog_Route(bodyBuffer.Length.ToString() + " # " + offset.ToString() + " # " + length.ToString(), 2);

            Route_Utility.Instance.WriteLog_Route("接收Analy报文，ID：" + PackageHelper.GetPackageHelper.GetAddressFromCTMProtocol(data), 2);
            Route_Utility.Instance.WriteLog_Route(ByteHelper.ByteToHexStrWithDelimiter(data, " ", false), 2);
            return new BinaryRequestInfo("CTM", data);
        }
    }

    //public class AnalyReceiveFilter:BeginEndMarkReceiveFilter<BinaryRequestInfo>
    //{
    //    private readonly static byte[] BeginMark = new byte[] { 0x68 };
    //    private readonly static byte[] EndMark = new byte[] { 0x16 };
    //    public AnalyReceiveFilter():base(BeginMark,EndMark)
    //    {

    //    }

    //    protected override BinaryRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
    //    {
    //        //byte[] data = new byte[length + 2];
    //        //data[0] = BeginMark[0];
    //        //Buffer.BlockCopy(readBuffer, offset, data, 0, length);
    //        //data[length + 1] = EndMark[0];

    //        Console.WriteLine("接收Analy报文，ID：" + PackageHelper.GetPackageHelper.GetAddressFromCTMProtocol(readBuffer));
    //        Console.WriteLine(ByteHelper.ByteToHexStrWithDelimiter(readBuffer, " ", false));
    //        return new BinaryRequestInfo("CTM", readBuffer);
    //    }
    //}
}
