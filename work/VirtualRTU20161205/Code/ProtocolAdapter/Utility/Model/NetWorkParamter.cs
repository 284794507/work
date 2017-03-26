using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Model
{
    //[Serializable]
    //public class NetWorkParamter
    //{
    //    public byte ParaNum { get; set; }

    //    public ParamterDetail[] ArrPara { get; set; }

    //    public byte[]ToBytes()
    //    {
    //        byte[] result=new byte[0];
    //        AspectF.Define.Retry()
    //            .Do(() =>
    //            {
    //                int len = 1;
    //                foreach (ParamterDetail detail in ArrPara)
    //                {
    //                    len += 1 + detail.Detail.Length;
    //                }
    //                result = new byte[len];
    //                int curIndex = 0;
    //                result[curIndex++] = ParaNum;
    //                foreach (ParamterDetail detail in ArrPara)
    //                {
    //                    result[curIndex++] = detail.ID;
    //                    int curLen = detail.Detail.Length;
    //                    Buffer.BlockCopy(detail.Detail, 0, result, curIndex, curLen);
    //                    curIndex += curLen;
    //                }
    //            });

    //        return result;
    //    }
    //}

    ////0X01	参数长度描述 主服务器IP或域名，ASCII
    ////0X02	参数长度描述 主服务器端口
    ////0X03	参数长度描述 APN, ASCII
    ////0X04	参数长度描述 副服务器IP或域名，ASCII
    ////0X05	参数长度描述 副服务器端口
    //[Serializable]
    //public class ParamterDetail
    //{
    //    public byte ID { get; set; }

    //    public byte[] Detail { get; set; }
    //}
}
