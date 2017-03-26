using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LFCDal.Model
{
    [DataContract]
    public class pLampTmpCtrlCfg
    {
        [DataMember]
        public int OptType { get; set; }

        [DataMember]
        public int No { get; set; }

        [DataMember]
        public int CmdType { get; set; }//1~7：分别描述周一（1）~周日（7）， 8~9,8代表工作日，9代表周末（周六和周日）, 0：所有周天为0

    }
}
