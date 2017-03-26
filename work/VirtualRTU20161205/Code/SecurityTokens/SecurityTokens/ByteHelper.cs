using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityTokens
{
    public class ByteHelper
    {
        public static byte[] StringTOAsciiBytes(string value)
        {
            byte[] result = Encoding.UTF8.GetBytes(value);

            return result;
        }

        public static string AsciiBytesToString(byte[] value)
        {
            string result = Encoding.UTF8.GetString(value);

            return result;
        }
    }
}
