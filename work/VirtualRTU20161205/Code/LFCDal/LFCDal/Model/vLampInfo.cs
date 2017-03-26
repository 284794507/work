using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LFCDal.Model
{
    [DataContract]
    public class vLampInfo
    {
        [DataMember]
        public string CtuID { get; set; }

        [DataMember]
        public string CTUCommAddr { get; set; }

        [DataMember]
        public string PtuID { get; set; }

        [DataMember]
        public string PtuVer { get; set; }

        [DataMember]
        public byte PtuChNo { get; set; }

        [DataMember]
        public int LampNo { get; set; }

        [DataMember]
        public byte LampType { get; set; }

        [DataMember]
        public byte LampStatus { get; set; }

        [DataMember]
        public byte GrpNo { get; set; }
    }
}
