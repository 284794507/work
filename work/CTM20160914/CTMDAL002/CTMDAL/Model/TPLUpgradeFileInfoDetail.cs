using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CTMDAL.Model
{
    [DataContract]
    public class TPLUpgradeFileInfoDetail
    {
        public TPLUpgradeFileInfoDetail()
        {
            this.ObjID = Guid.NewGuid();
            this.TheDate = DateTime.Now;
            this.IsDownLoadToDev = false;
            this.DownloadToDevTime = null;
            this.Status = 1;
            this.Memo = "";
        }

        [DataMember]
        public Guid ObjID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public DateTime TheDate { get; set; } // datetime, not null

        [DataMember]
        public Guid FileInfoID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public int FileDataNo { get; set; } // int, not null

        [DataMember]
        public int FileDataLength { get; set; } // int, not null

        [DataMember]
        public byte[] FileDataContent { get; set; } // varbinary(max), not null

        [DataMember]
        public bool IsDownLoadToDev { get; set; } // bit, not null

        [DataMember]
        public DateTime? DownloadToDevTime { get; set; } // datetime, null

        [DataMember]
        public byte Status { get; set; } // tinyint, not null

        [DataMember]
        public string Memo { get; set; } // varchar(50), null
    }
}
