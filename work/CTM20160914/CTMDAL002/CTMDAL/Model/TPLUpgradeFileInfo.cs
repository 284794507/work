using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CTMDAL.Model
{
    [DataContract]
    public class TPLUpgradeFileInfo
    {
        public TPLUpgradeFileInfo()
        {
            this.ObjID = Guid.NewGuid();
            this.TheDate = DateTime.Now;
            this.FileUpLoadTime = DateTime.Now;
            this.FileType = 1;
            this.IsDownLoadToDev = false;
            this.Status = 1;
        }

        [DataMember]
        public Guid ObjID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public DateTime TheDate { get; set; } // datetime, not null

        [DataMember]
        public Guid PLCollectorInfoID { get; set; } // uniqueidentifier, not null

        [DataMember]
        public DateTime FileUpLoadTime { get; set; } // datetime, not null

        [DataMember]
        public string FileName { get; set; } // varchar(50), not null

        [DataMember]
        public byte FileType { get; set; } // tinyint, not null

        [DataMember]
        public int FilePerSize { get; set; } // int, not null

        [DataMember]
        public byte[] FileContent { get; set; } // varbinary(max), not null

        [DataMember]
        public string FileSoftWareVer { get; set; } // varchar(30), null

        [DataMember]
        public string FileHardWareVer { get; set; } // varchar(30), null

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
