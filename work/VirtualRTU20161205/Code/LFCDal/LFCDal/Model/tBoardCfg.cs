using System;
using System.Collections.Generic;

namespace LFCDal.Model
{
    public class tBoardCfg
    {
        public byte BoardNo { get; set; } // tinyint, not null

        public string BoardName { get; set; } // varchar(100), null

        public int BoardCommAddr { get; set; } // int, not null

        public byte BoardType { get; set; } // tinyint, not null

        public byte? CtrlInNum { get; set; } // tinyint, null

        public string CtrlChByte { get; set; } // char(8), null

        public byte? CtrlChNum { get; set; } // tinyint, null

        public byte? CtrlStartNo { get; set; } // tinyint, null

        public byte? CT1 { get; set; } // tinyint, null

        public byte? CT2 { get; set; } // tinyint, null

        public string Memo { get; set; } // nvarchar(200), null
    }
}
