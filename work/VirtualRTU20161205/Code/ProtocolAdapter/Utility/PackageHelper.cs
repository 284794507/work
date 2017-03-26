using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class PackageHelper
    {
        private static PackageHelper _PackageHelper;
        public static PackageHelper GetHelper
        {
            get
            {
                if (_PackageHelper == null)
                {
                    _PackageHelper = new PackageHelper();
                }
                return _PackageHelper;
            }
        }

        public string GetTerminalIDFromData(byte[] data, int startNo, int len, ProtocolType pType=0)
        {
            byte[] addr = new byte[len];
            Buffer.BlockCopy(data, startNo, addr, 0, len);
            string TerminalID = ByteHelper.byteToHexStr(addr);

            return TerminalID;
        }
    }
}
