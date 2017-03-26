using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Model
{
    [Serializable]
    public class NetWorkParamter
    {
        public byte ParaNum { get; set; }

        public ParamterDetail[] ArrPara { get; set; }
    }

    //0X01	参数长度描述 主服务器IP或域名，ASCII
    //0X02	参数长度描述 主服务器端口
    //0X03	参数长度描述 APN, ASCII
    //0X04	参数长度描述 副服务器IP或域名，ASCII
    //0X05	参数长度描述 副服务器端口
    [Serializable]
    public class ParamterDetail
    {
        public byte ID { get; set; }

        public byte[] Detail { get; set; }
    }
}
