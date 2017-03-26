using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace CommunicateCore.Model
{
    [Serializable]
    public class LoginBackInfo
    {
        /// <summary>
        /// 1登录成功,2 不能登录,3须登录到以下地址
        /// </summary>
        public byte? LoginReuslt { get; set; }

        /// <summary>
        /// IP或域名
        /// </summary>
        public byte?[] IPOrDomain { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public byte?[] Port { get; set; }

        public byte?[] APN { get; set; }

        public LoginBackInfo()
        {
            this.LoginReuslt = 1;
            this.IPOrDomain = new byte?[1];
            this.IPOrDomain[0] = 1;
            this.Port = new byte?[1];
            this.Port[0] = 1;
            this.APN = new byte?[1];
            this.APN[0] = 1;
        }


        public byte[] ToBytes()
        {
            byte[] data = new byte[0];
            AspectF.Define.Retry()
                .Do(() =>
                {
                    if (this.LoginReuslt == 3)
                    {
                        int len1 = this.IPOrDomain.Length;
                        int len2 = this.APN.Length;
                        int curIndex = 0;
                        data = new byte[1 + 1 + len1 + 2 + len2];
                        data[curIndex++] = 0;//SEQ
                        data[curIndex++] = this.LoginReuslt.Value;
                        data[curIndex++] = (byte)len1;
                        Buffer.BlockCopy(this.IPOrDomain, 0, data, curIndex, len1);
                        curIndex += len1;
                        Buffer.BlockCopy(this.Port, 0, data, curIndex, 2);
                        curIndex += 2;
                        data[curIndex++] = (byte)len2;
                        Buffer.BlockCopy(this.APN, 0, data, 2 + curIndex, len2);
                    }
                    else
                    {
                        data = new byte[2] { 0x00, this.LoginReuslt.Value };
                    }
                });
            return data;
        }
    }
}
