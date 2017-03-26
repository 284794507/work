using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KLJOption
{
    public class PackageCmdWord
    {
        public static readonly byte Login = 0xE8;

        public static readonly byte LoginBack = 0xE6;

        public static readonly byte QueryMacAddr = 0xE4;

        public static readonly byte QueryMacAddrBack = 0xE5;

        public static readonly byte SetAp = 0xE2;

        public static readonly byte SetMode = 0xF7;

        public static readonly byte Restart = 0xE3;
    }

    public enum USART
    {
        uTcp=0,
        u1=1,
        u2=2,
        u3=3,
        uMcu=5
    }
}
