using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerFlowServer.PackageHandler
{
    public class PackageCmdWord
    {
        public const byte Login = 0xE8;

        public const byte LoginBack = 0xE6;

        public const byte QueryMacAddr = 0xE4;

        public const byte QueryMacAddrBack = 0xE5;

        public const byte SetAp = 0xE2;

        public const byte SetMode = 0xF7;

        public const byte Restart = 0xE3;

        public const byte GetTime = 0xF8;

        public const byte GetData = 0xF6;
    }

    public enum USART
    {
        uTcp = 0,
        u1 = 1,
        u2 = 2,
        u3 = 3,
        uMcu = 5
    }
}
