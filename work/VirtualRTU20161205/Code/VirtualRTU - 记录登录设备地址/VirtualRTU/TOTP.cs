using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VirtualRTU
{
    public class TOTP
    {
        private static TOTP _TOTP;
        public static TOTP Instance
        {
            get
            {
                if(_TOTP==null)
                {
                    _TOTP = new TOTP();
                }
                return _TOTP;
            }
        }

        public TOTP()
        {

        }

        /// <summary>
        /// HMAC-SHA1加密算法
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="input">要加密的串</param>
        /// <returns></returns>
        public static string HmacSha1(string key, string input)
        {
            byte[] keyBytes = ASCIIEncoding.ASCII.GetBytes(key);
            byte[] inputBytes = ASCIIEncoding.ASCII.GetBytes(input);
            return HmacSha1(keyBytes, inputBytes);
            //HMACSHA1 hmac = new HMACSHA1(keyBytes);            
            //byte[] hashBytes = hmac.ComputeHash(inputBytes);
            //return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// HMAC-SHA1加密算法
        /// </summary>
        /// <param name="keyBytes">密钥</param>
        /// <param name="inputBytes">要加密的串</param>
        /// <returns></returns>
        public static string HmacSha1(byte[] keyBytes, byte[] inputBytes)
        {
            HMACSHA1 hmac = new HMACSHA1(keyBytes);
            byte[] hashBytes = hmac.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashBytes);
        }

        private static readonly int[] DIGITS_POWER = { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000 };

        public static string GenerateTOTP(string key, string time, string input)
        {
            int codeDigits = int.Parse(input);
            string result = "";

            while (time.Length < 16)
            {
                time = "0" + time;
            }

            string hash = HmacSha1(key, time);
            byte[] bHash = ASCIIEncoding.ASCII.GetBytes(hash);

            int offset = bHash[bHash.Length - 1] & 0x0F;
            int binary = ((bHash[offset] & 0x7F) << 24) | ((bHash[offset + 1] & 0xFF) << 16)
                        | ((bHash[offset + 2] & 0xFF) << 8) | (bHash[offset + 3] & 0xFF);
            int otp = binary % DIGITS_POWER[codeDigits];
            result = otp.ToString();
            while (result.Length < codeDigits)
            {
                result = "0" + result;
            }

            return result;
        }
    }

    public class Base64
    {
        //默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
    }
}
