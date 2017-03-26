using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using PassengerFlowDal.Model;

namespace PassengerFlowDal.Utility
{
    [DataContract]
    [KnownType(typeof(int))]
    [KnownType(typeof(int[]))]
    [KnownType(typeof(byte))]
    [KnownType(typeof(byte[]))]
    [KnownType(typeof(string))]
    [KnownType(typeof(string[]))]
    [KnownType(typeof(TabTimeSync))]
    [KnownType(typeof(TabTimeSync[]))]
    [KnownType(typeof(TabCurData))]
    [KnownType(typeof(TabCurData[]))]
    [KnownType(typeof(TabHistoryData))]
    [KnownType(typeof(TabHistoryData[]))]
    [KnownType(typeof(TabArea))]
    [KnownType(typeof(TabArea[]))]
    public class DalParameter
    {
        [DataMember]
        public Object BusinessObject { get; set; }
        
        [DataMember]
        public BusinessType BusinessType { get; set; }

        [DataMember]
        public string TerminalID { get; set; }
    }
}
