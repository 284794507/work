using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tCTUTaskLog
    {
        public int TaskNo { get; set; } // int, not null

        public byte TaskFrom { get; set; } // tinyint, not null

        public DateTime TaskAddTime { get; set; } // datetime, not null

        public byte CMDLevel { get; set; } // tinyint, not null

        public string CMDValue { get; set; } // char(8), not null

        public byte isNeedUpload { get; set; } // tinyint, not null

        public string TaskData { get; set; } // nvarchar(100), null

        public byte ResultStatus { get; set; } // tinyint, not null

        public short RunTimes { get; set; } // smallint, not null

        public DateTime ResultDTime { get; set; } // datetime, not null

        public string RetCMDValue { get; set; } // char(8), not null

        public string RetTaskData { get; set; } // nvarchar(100), null

        public byte UpLoadStatus { get; set; } // tinyint, not null

        public DateTime UpLoadDTime { get; set; } // datetime, not null

        public string Memo { get; set; } // nvarchar(200), null

        public string CTUID { get; set; } // char(40), not null
    }
}
