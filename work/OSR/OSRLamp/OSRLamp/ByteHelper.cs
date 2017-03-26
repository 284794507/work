using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OSRLamp
{
    public sealed class ByteHelper
    {
        /// <summary>
        /// 获取高位
        /// <para>eg: 0x0A==>10101101==>1010</para>
        /// <para>eg: Assert.AreEqual(0x0A, ByteHelper.GetHigh(0xAD));</para>
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte GetHigh(byte data)
        {
            return (byte)(data >> 4);
        }

        /// <summary>
        /// 获取低位
        /// <para>eg: 0x0A==>10101101==>1101</para>
        /// <para>eg: Assert.AreEqual(0x0D, ByteHelper.GetLow(0xAD));</para>
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte GetLow(byte data)
        {
            return (byte)(data & 0x0F);
        }

        /// <summary>
        /// 比较两个字节数组是否相等
        /// </summary>
        /// <param name="b1">byte数组1</param>
        /// <param name="b2">byte数组2</param>
        /// <returns>是否相等</returns>
        public static bool ByteArryEquals(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null)
                return false;
            if (b1.Length != b2.Length)
                return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }

        ///<summary>将16进制字符串(ex: E4 CA B2) 转换成字节数组;也可不留空格保证2的倍数字符串  </summary>
        ///<param name="s">The string containing the hex digits (with or without spaces).</param>
        ///<returns></returns>
        public static byte[] HexStrToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            }
            return buffer;
        }

        /// <summary>
        /// 将16进制字符串(ex: E4 CA B2) 转换成字节数组;可字定义分隔符"-" 例如00-03-00-B2-04-E6-CF
        /// </summary>
        /// <param name="s"></param>
        /// <param name="Delimiter"></param>
        /// <returns></returns>
        public static byte[] HexStrToByteArrayWithDelimiter(string s, string delimiter=" ", bool needReverse = false)
        {
            s = s.Replace(delimiter, "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            if (needReverse)
            {
                Array.Reverse(buffer);
            }
            return buffer;
        }

        /// <summary>
        /// 字节数组转换为16进制字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="separator">分隔符（为空不需要分隔符）</param>
        /// <param name="needReverse">需要反转</param>
        /// <returns></returns>
        public static string ByteToHexStrWithDelimiter(byte[] bytes, string separator=" ", bool needReverse=false)
        {
            string returnStr = "";
            if (bytes != null)
            {
                if (needReverse)
                {
                    Array.Reverse(bytes);
                }

                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");

                    if (!string.IsNullOrEmpty(separator))
                    {
                        //最后一个字节
                        if (i != bytes.Length - 1)
                        {
                            returnStr += separator;
                        }
                    }
                }
            }
            return returnStr;
        }

        /// <summary>
        /// 字节数组转16进制字符串(加空格)
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr_Blank(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                    //字节空一格
                    if (i != bytes.Length - 1)
                    {
                        returnStr += " ";
                    }
                }
            }
            return returnStr;
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /// <summary>
        /// CRC16验证
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckCrc16(byte[] data, byte[] crcData)
        {
            bool flag = false;

            if (ByteArryEquals(crcData, GetCrc16_Bytes(data)))
            {
                flag = true;
            }

            return flag;
        }

        /// <summary>
        /// 异或运算获取CRC16(返回2个字节的bytes);
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] GetCrc16_Bytes(byte[] data)
        {
            byte[] uIntCrc = BitConverter.GetBytes(GetCrc16(data));

            return uIntCrc;
        }

        /// <summary>
        /// 异或运算获取CRC16(返回2个字节的bytes);
        /// </summary>
        /// <param name="addBytes"></param>
        /// <param name="userdata"></param>
        /// <returns></returns>
        public static byte[] GetCrc16_Bytes(byte[] addBytes, byte[] userdata)
        {
            byte[] addr_Userdata = new byte[addBytes.Length + userdata.Length];

            Array.Copy(addBytes, 0, addr_Userdata, 0, addBytes.Length);
            Array.Copy(userdata, 0, addr_Userdata, addBytes.Length, userdata.Length);
            return GetCrc16_Bytes(addr_Userdata);
        }

        /// <summary>
        /// 异或运算获取CRC16(UInt16);
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static UInt16 GetCrc16(byte[] data)
        {
            //WORD CProtocol::GetCRC16(BYTE *data,int len)
            //{
            //WORD ax,lsb;
            //int i,j;
            //ax=0xFFFF;
            //for(i=0;i<len;i++) {
            //ax ^= data[i];
            //for(j=0;j<8;j++) {
            //lsb=ax&0x0001;
            //ax = ax>>1;
            //if(lsb!=0)
            //ax ^= 0xA001;
            //}
            //}
            //return ax;
            //}

            UInt16 ax = 0xFFFF; UInt16 lsb = 0;

            for (int i = 0; i < data.Length; i++)
            {
                ax ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    lsb = Convert.ToUInt16(ax & 0x0001);
                    ax = Convert.ToUInt16(ax >> 1);
                    if (lsb != 0)
                        ax ^= 0xA001;
                }
            }

            return ax;
        }

        /// <summary>
        /// CRC_V3验证码判断(长度域 + 68 + 命令字 + 命令参数)
        /// </summary>
        /// <param name="cmdword">命令字</param>
        /// <param name="cmdparms">数据参数</param>
        /// <param name="crc">对比的crc值</param>
        /// <returns></returns>
        public static byte GetCRC_V3(Byte[] datalength, Byte[] ctuAddr, Byte synword2, Byte[] cmdword, Byte[] cmdparms)
        {
            Byte cal = 0;
            uint totol = 0;
            foreach (Byte b in datalength)
            {
                totol += b;
            }
            foreach (Byte b in ctuAddr)
            {
                totol += b;
            }
            totol += synword2;

            foreach (Byte b in cmdword)
            {
                totol += b;
            }
            if (cmdparms != null)
            {
                foreach (Byte b in cmdparms)
                {
                    totol += b;
                }
            }
            //溢出不抛出异常，取1个byte
            unchecked
            {
                cal = (Byte)totol;
            }

            return cal;
        }

        /// <summary>
        /// 取命令字和命令字参数相加的得到的验证码（1个byte）
        /// </summary>
        /// <param name="cmdword">命令字</param>
        /// <param name="cmdparms">命令参数</param>
        /// <returns></returns>
        public static Byte GetCRC(Byte[] cmdword, Byte[] cmdparms)
        {
            Byte cal = 0;
            uint totol = 0;
            foreach (Byte b in cmdword)
            {
                totol += b;
            }
            //用户数据有可能为空（例如心跳包返回）
            if (cmdparms != null)
            {
                foreach (Byte b in cmdparms)
                {
                    totol += b;
                }
            }
            //溢出不抛出异常，取1个byte
            unchecked
            {
                cal = (Byte)totol;
            }

            return cal;
        }

        public static bool CheckCRC(Byte[] cmdword, Byte[] cmdparms, Byte crc)
        {
            bool flag = false;
            Byte cal = 0;
            uint totol = 0;
            foreach (Byte b in cmdword)
            {
                totol += b;
            }
            if (cmdparms != null)
            {
                foreach (Byte b in cmdparms)
                {
                    totol += b;
                }
            }
            //溢出不抛出异常
            unchecked
            {
                cal = (Byte)totol;
            }
            if (cal == crc)
            {
                return true;
            }
            return flag;
        }

        /// <summary>
        ///根据包里的用户数据(6字节或者7字节)解析时间，返回DateTime
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static DateTime BytesToDateTime(Byte[] bytes)
        {
            DateTime dt = DateTime.MinValue;
            if (bytes == null)
            {
                throw new Exception("需转换的时间字节为空！");
            }
            //按照格式，时间数据应为6个字节
            if (bytes.Length == 6 || bytes.Length == 7)
            {
                //十六机制转换
                //Byte[0]=秒，Byte[1]=分，Byte[2]=时，Byte[3]=日，Byte[4]=月，Byte[5]=年 为秒、分、时、日、月、年的BCD码
                //如：42 05 15 20 05 09=2009年05月20日15时05分42秒
                //注意42 05 15 20 05 09为16进制字符串

                //ToString("X2")
                //X     十六进制
                //X是大写,x是小写
                //2     每次都是两位数
                //比如   0x0A   如果没有2,就只会输出0xA
                //如果两个数10和26，正常情况十六进制显示0xA、0x1A，这样看起来不整齐，为了好看，我们可以指定X2，这样显示出来就是：0x0A、0x1A。

                try
                {
                    int second = Convert.ToUInt16(bytes[0].ToString("X2"));
                    int minute = Convert.ToUInt16(bytes[1].ToString("X2"));
                    int hour = Convert.ToUInt16(bytes[2].ToString("X2"));
                    int day = Convert.ToUInt16(bytes[3].ToString("X2"));
                    int month = Convert.ToUInt16(bytes[4].ToString("X2"));
                    //老协议 42 05 15 20 05 09 新协议 42 05 15 20 05 09 20 取年份其实都是20xx年
                    int year = 2000 + Convert.ToUInt16(bytes[5].ToString("X2"));
                    dt = new DateTime(year, month, day, hour, minute, second);
                }
                catch (Exception e)
                {
                    throw new Exception("时间bytes转换datetime出错：" + e.Message);
                }
            }
            else
            {
                throw new Exception("时间格式应该6个字节或者7个字节！错误:" + bytes.Length.ToString());
            }

            return dt;
        }

        public static DateTime Bytes6ToDateTime(Byte[] bytes)
        {
            DateTime dt = DateTime.MinValue;
            if (bytes == null)
            {
                throw new Exception("需转换的时间字节为空！");
            }
            //按照格式，时间数据应为6个字节
            if (bytes.Length == 6 )
            {

                try
                {
                    int second = Convert.ToUInt16(bytes[5].ToString("X2"));
                    int minute = Convert.ToUInt16(bytes[4].ToString("X2"));
                    int hour = Convert.ToUInt16(bytes[3].ToString("X2"));
                    int day = Convert.ToUInt16(bytes[2].ToString("X2"));
                    int month = Convert.ToUInt16(bytes[1].ToString("X2"));
                    //老协议 42 05 15 20 05 09 新协议 42 05 15 20 05 09 20 取年份其实都是20xx年
                    int year = 2000 + Convert.ToUInt16(bytes[0].ToString("X2"));
                    dt = new DateTime(year, month, day, hour, minute, second);
                }
                catch (Exception e)
                {
                    throw new Exception("时间bytes转换datetime出错：" + e.Message);
                }
            }
            else
            {
                throw new Exception("时间格式应该6个字节！错误:" + bytes.Length.ToString());
            }

            return dt;
        }

        /// <summary>
        ///根据包里的用户数据(7字节)解析时间(转换错误返回MinValue)
        /// </summary>
        /// <param name="bytes">字节数据</param>
        /// <param name="dateType">byte根据ss mm hh DD MM YY YY为true，反之false</param>
        /// <returns></returns>
        public static DateTime Bytes7ToDateTime_Plus(Byte[] bytes, bool dateType)
        {
            DateTime dt = DateTime.MinValue;
            if (bytes == null)
            {
                Console.WriteLine("需转换的时间字节为空！");
            }
            //按照格式，时间数据应为7个字节
            if (bytes.Length == 7)
            {
                //十六机制转换
                try
                {
                    if (dateType)
                    {
                        // Eg：00 12 03 15 06 12 20 表示2012-6-14 03:12:00时间
                        int second = Convert.ToUInt16(bytes[0].ToString("X2"));
                        int minute = Convert.ToUInt16(bytes[1].ToString("X2"));
                        int hour = Convert.ToUInt16(bytes[2].ToString("X2"));
                        int day = Convert.ToUInt16(bytes[3].ToString("X2"));
                        int month = Convert.ToUInt16(bytes[4].ToString("X2"));
                        int year = 2000 + Convert.ToUInt16(bytes[5].ToString("X2"));
                        dt = new DateTime(year, month, day, hour, minute, second);
                    }
                    else
                    {
                        int second = Convert.ToUInt16(bytes[6].ToString("X2"));
                        int minute = Convert.ToUInt16(bytes[5].ToString("X2"));
                        int hour = Convert.ToUInt16(bytes[4].ToString("X2"));
                        int day = Convert.ToUInt16(bytes[3].ToString("X2"));
                        int month = Convert.ToUInt16(bytes[2].ToString("X2"));
                        int year = 2000 + Convert.ToUInt16(bytes[1].ToString("X2"));
                        dt = new DateTime(year, month, day, hour, minute, second);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("时间bytes转换datetime出错：" + e.Message);
                }
            }
            else
            {
                Console.WriteLine("时间格式应该7个字节！错误:" + bytes.Length.ToString());
            }

            return dt;
        }

        /// <summary>
        ///根据包里的用户数据(7字节)解析时间，返回DateTime
        /// </summary>
        /// <param name="bytes">字节数据</param>
        /// <param name="dateType">byte根据ss mm hh DD MM YY YY为true，反之false</param>
        /// <returns></returns>
        public static DateTime Bytes7ToDateTime(Byte[] bytes, bool dateType)
        {
            DateTime dt = DateTime.MinValue;
            if (bytes == null)
            {
                throw new Exception("需转换的时间字节为空！");
            }
            //按照格式，时间数据应为7个字节
            if (bytes.Length == 7)
            {
                //十六机制转换
                try
                {
                    if (dateType)
                    {
                        // Eg：00 12 03 15 06 12 20 表示2012-6-14 03:12:00时间
                        int second = Convert.ToUInt16(bytes[0].ToString("X2"));
                        int minute = Convert.ToUInt16(bytes[1].ToString("X2"));
                        int hour = Convert.ToUInt16(bytes[2].ToString("X2"));
                        int day = Convert.ToUInt16(bytes[3].ToString("X2"));
                        int month = Convert.ToUInt16(bytes[4].ToString("X2"));
                        int year = 2000 + Convert.ToUInt16(bytes[5].ToString("X2"));
                        dt = new DateTime(year, month, day, hour, minute, second);
                    }
                    else
                    {
                        int second = Convert.ToUInt16(bytes[6].ToString("X2"));
                        int minute = Convert.ToUInt16(bytes[5].ToString("X2"));
                        int hour = Convert.ToUInt16(bytes[4].ToString("X2"));
                        int day = Convert.ToUInt16(bytes[3].ToString("X2"));
                        int month = Convert.ToUInt16(bytes[2].ToString("X2"));
                        int year = 2000 + Convert.ToUInt16(bytes[1].ToString("X2"));
                        dt = new DateTime(year, month, day, hour, minute, second);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Bytes7ToDateTime抛出错误！时间bytes转换datetime出错：" + e.Message);
                }
            }
            else
            {
                throw new Exception("时间格式应该7个字节！错误:" + bytes.Length.ToString());
            }

            return dt;
        }

        /// <summary>
        /// 2016 08 18 13 58 20 400---》20 16 08 18 13 58 20 04 00
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DateTime Bytes9ToDateTime(byte[]data)
        {
            int year = Convert.ToUInt16(data[0].ToString("X2"))*100+ Convert.ToUInt16(data[1].ToString("X2"));
            int month = Convert.ToUInt16(data[2].ToString("X2"));
            int day = Convert.ToUInt16(data[3].ToString("X2"));
            int hour = Convert.ToUInt16(data[4].ToString("X2"));
            int minute = Convert.ToUInt16(data[5].ToString("X2"));
            int second = Convert.ToUInt16(data[6].ToString("X2"));
            int milliSecond = Convert.ToUInt16(data[7].ToString("X2")) * 100 + Convert.ToUInt16(data[8].ToString("X2"));

            DateTime curTime = new DateTime(year, month, day, hour, minute, second, milliSecond);

            return curTime;
        }

        /// <summary>
        /// 将时间转换为9位的字节数组
        /// </summary>
        /// <param name="curTime"></param>
        /// <returns></returns>
        public static byte[] DateTimeToBytes6(DateTime curTime)
        {
            byte[] result = new byte[9];
            
            result[0] = Convert.ToByte((curTime.Year % 100).ToString(), 16);
            result[1] = Convert.ToByte(curTime.Month.ToString(), 16);
            result[2] = Convert.ToByte(curTime.Day.ToString(), 16);
            result[3] = Convert.ToByte(curTime.Hour.ToString(), 16);
            result[4] = Convert.ToByte(curTime.Minute.ToString(), 16);
            result[5] = Convert.ToByte(curTime.Second.ToString(), 16);

            return result;
        }

        /// <summary>
        /// 将时间转换为9位的字节数组
        /// </summary>
        /// <param name="curTime"></param>
        /// <returns></returns>
        public static byte[] DateTimeToBytes9(DateTime curTime)
        {
            byte[] result = new byte[9];

            result[0] = Convert.ToByte((curTime.Year / 100).ToString(), 16);
            result[1] = Convert.ToByte((curTime.Year % 100).ToString(), 16);
            result[2] = Convert.ToByte(curTime.Month.ToString(), 16);
            result[3] = Convert.ToByte(curTime.Day.ToString(), 16);
            result[4] = Convert.ToByte(curTime.Hour.ToString(), 16);
            result[5] = Convert.ToByte(curTime.Minute.ToString(), 16);
            result[6] = Convert.ToByte(curTime.Second.ToString(), 16);
            result[7] = Convert.ToByte((curTime.Millisecond / 100).ToString(), 16);
            result[8] = Convert.ToByte((curTime.Millisecond % 100).ToString(), 16);
            
            return result;
        }

        /// <summary>
        /// 将时间转换为TP
        /// </summary>
        /// <param name="curTime"></param>
        /// <returns></returns>
        public static byte[] DateTimeToTP(DateTime curTime)
        {
            byte[] result = new byte[6];

            result[0] = Convert.ToByte(curTime.Hour.ToString(), 16);
            result[1] = Convert.ToByte(curTime.Minute.ToString(), 16);
            result[2] = Convert.ToByte(curTime.Second.ToString(), 16);
            result[3] = Convert.ToByte((curTime.Millisecond / 100).ToString(), 16);
            result[4] = Convert.ToByte((curTime.Millisecond % 100).ToString(), 16);
            result[5] =0;           

            return result;
        }

        /// <summary>
        /// 根据DateTime 返回6个字节的数组(ssmmHHMMYY)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Byte[] DateTimeToBytes(DateTime dt)
        {
            Byte[] bytes = new Byte[6];

            try
            {
                bytes[0] = Convert.ToByte(dt.Second.ToString(), 16);
                bytes[1] = Convert.ToByte(dt.Minute.ToString(), 16);
                bytes[2] = Convert.ToByte(dt.Hour.ToString(), 16);
                bytes[3] = Convert.ToByte(dt.Day.ToString(), 16);
                bytes[4] = Convert.ToByte(dt.Month.ToString(), 16);
                bytes[5] = Convert.ToByte((dt.Year - 2000).ToString(), 16);
            }
            catch (Exception e)
            {
                throw new Exception("DateTime转换成Bytes出错：" + e.Message);
            }

            return bytes;
        }

        /// <summary>
        /// 根据DateTime 返回7个字节的数组(ssmmHHMMYY)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Byte[] DateTimeToBytes_7(DateTime dt)
        {
            Byte[] bytes = new Byte[7];

            try
            {
                bytes[0] = Convert.ToByte(dt.Second.ToString(), 16);
                bytes[1] = Convert.ToByte(dt.Minute.ToString(), 16);
                bytes[2] = Convert.ToByte(dt.Hour.ToString(), 16);
                bytes[3] = Convert.ToByte(dt.Day.ToString(), 16);
                bytes[4] = Convert.ToByte(dt.Month.ToString(), 16);
                bytes[5] = Convert.ToByte((dt.Year - 2000).ToString(), 16);
                bytes[6] = Convert.ToByte("20", 16);
            }
            catch (Exception e)
            {
                throw new Exception("DateTime转换成Bytes出错：" + e.Message);
            }

            return bytes;
        }

        /// <summary>
        /// string 类型CTUAddr 转换为 bytes数组（双字节）
        /// 高低位互换形如：431-> 01AF ->AF01
        /// </summary>
        /// <param name="ctuid"></param>
        /// <returns></returns>
        public static Byte[] CtuAddrToBytes(string ctuAdd)
        {
            Byte[] bytes = new Byte[2];

            try
            {
                int value = Convert.ToUInt16(ctuAdd);
                int hValue = (value & 0xFF00) >> 8;
                int lValue = value & 0xFF;
                byte[] arr = new byte[] { (byte)hValue, (byte)lValue };

                bytes[0] = (byte)lValue;
                bytes[1] = (byte)hValue;
            }
            catch (Exception e)
            {
                throw new Exception("CtuIdToBytes出错：" + e.Message);
            }

            return bytes;
        }

        /// <summary>
        /// byte数组(双字节)转化为string 类型 ctuAddr 16进制
        /// 高低位互换形如：AF01->01AF
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string bytesToCtuAddr_Hex(Byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                if (bytes.Length != 2)
                {
                    throw new Exception("转换CtuID的字节数长度只能为2！");
                }
                returnStr = bytes[1].ToString("X2") + bytes[0].ToString("X2");
            }
            return returnStr;
        }

        /// <summary>
        /// byte数组(双字节)转化为string 类型 ctuAddr 10进制
        /// 高低位互换形如：AF01->01AF ->431
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string bytesToCtuAddr(Byte[] bytes, bool NeedRevers)
        {
            string returnStr = "";
            if (bytes != null)
            {
                if (bytes.Length != 2)
                {
                    throw new Exception("转换CtuAddr的字节数长度只能为2！");
                }
                Byte[] xbytes = new Byte[2];
                ////数组倒转
                //Array.Reverse(bytes);
                if (NeedRevers)
                {
                    xbytes[0] = bytes[1];
                    xbytes[1] = bytes[0];
                }
                else
                {
                    xbytes[0] = bytes[0];
                    xbytes[1] = bytes[1];
                }
                //得到16进制字符串
                string HexStr = BitConverter.ToString(xbytes).Replace("-", null);

                returnStr = Convert.ToUInt16(HexStr, 16).ToString();
            }

            return returnStr;
        }

        /// <summary>
        /// byte数组(八字节)转化为string 类型 ctuAddr 10进制
        /// 高低位互换形如：AF01->01AF ->431
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string bytesToCtuAddr8(Byte[] bytes, bool NeedRevers)
        {
            string result = "";
            if (bytes == null || bytes.Length != 8)
            {
                throw new Exception("转换CtuAddr必须传入8字节的byte数组！");
            }

            byte[] newBytes = new byte[8];
            if (NeedRevers)
            {
                newBytes[0] = bytes[7];
                newBytes[1] = bytes[6];
                newBytes[2] = bytes[5];
                newBytes[3] = bytes[4];
                newBytes[4] = bytes[3];
                newBytes[5] = bytes[2];
                newBytes[6] = bytes[1];
                newBytes[7] = bytes[0];
            }
            else
            {
                Buffer.BlockCopy(bytes, 0, newBytes, 0, 8);
            }
            //得到16进制字符串
            string HexStr = BitConverter.ToString(newBytes).Replace("-", null);
            result = Convert.ToUInt16(HexStr, 16).ToString();

            return result;
        }

        /// <summary>
        /// 欣能地址转换；字节数组转化为字符串（大端模式，低位在前）
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string bytesToCtuAddrString_8(Byte[] bytes)
        {
            string result = "";

            if (bytes == null || bytes.Length != 8)
            {
                throw new Exception("转换CtuAddr必须传入8字节的byte数组！");
            }

            result = BitConverter.ToUInt64(bytes, 0).ToString();

            return result;

        }

        /// <summary>
        /// 欣能地址转换，按BCD码
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string bytesToCtuAddrXN(byte[] bytes)
        {
            string result = "";
            int len = 8;

            if (bytes == null || bytes.Length != len)
            {
                throw new Exception("转换CtuAddr必须传入8字节的byte数组！");
            }

            double add = 0;
            for (int i = 0; i < len; i++)
            {
                add += int.Parse(bytes[i].ToString("X2")) * Math.Pow(10, 2 * i);
            }
            result = add.ToString();
            return result;
        }

        /// <summary>
        /// 把Uint(2个字节)类型转化为字节数组（可高低位反转）
        /// </summary>
        /// <param name="value">UShort类型</param>
        /// <param name="reverse">是不是需要把得到的字节数组反转</param>
        /// <returns></returns>
        public static byte[] ConvertUIntToBytes(ushort value, bool reverse)
        {
            byte[] ret = BitConverter.GetBytes(value);
            if (reverse)
            {
                Array.Reverse(ret);
            }
            return ret;
        }

        /// <summary>
        /// 获取形如1111格式的BYTES：例如 byte: 3f = 111111组成;全1111格式
        /// 例：0x0000003F =0000 0000 0000 0000 0000 0000 0011 1111 （4字节,加反转）
        /// </summary>
        /// <param name="dataCount">输入数据位数总数</param>
        /// <param name="byteCount">需要生成的字节总数</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static Byte[] Get1111AllBytes(int dataCount, int byteCount, bool reverse)
        {
            //算出输入位数所需要的总字节数
            int dataByteNum = dataCount / 8;

            int dataMod = dataCount % 8;

            //余数不为0，字节count +1
            if (dataMod != 0)
            {
                dataByteNum += 1;
            }
            byte[] resultBytes = new Byte[byteCount];
            for (int i = 0; i < byteCount; i++)
            {
                if (i < dataByteNum)
                {
                    //有余数的话，最后一个字节按位赋值
                    if (dataMod != 0 && i == dataByteNum - 1)
                    {
                        for (int j = 0; j < dataMod; j++)
                        {
                            resultBytes[i] = ByteHelper.SetBit(resultBytes[i], j);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            resultBytes[i] = ByteHelper.SetBit(resultBytes[i], j);
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            if (reverse)
            {
                Array.Reverse(resultBytes);
            }

            return resultBytes;
        }

        /// <summary>
        /// 单字节负数转成数值类型（补码取反-1方式);只能负数
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int ByteToNegative_BuMa(byte x)
        {
            int num = 0;

            //负数的补码是:符号位（1）,原码各位求反,末位加1
            //即 原码 =   （ 补码 - 1） 求反
            num = ~(x - 1);
            return num;
        }

        /// <summary>
        /// 单字节负数转成数值类型(首位符号位方式)；只能负数
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int ByteToNegative(byte x)
        {
            int num = 0;

            x = ClearBit(x, 7);

            num = 0 - x;

            return num;
        }

        ///// <summary>
        ///// 2字节补码转成原码（支持正数和负数）
        ///// </summary>
        ///// <param name="x"></param>
        ///// <returns></returns>
        //public static int BuMaBytesToIntNum(byte[] bytes)
        //{
        //    int iTmp =  BitConverter.ToInt16(bytes, 0);
        //    //if (iTmp >= 0x8000)
        //    //{ iTmp = ~(iTmp - 1); }
        //    return iTmp;
        //}

        /// <summary>
        /// index从0开始
        /// 获取取第index位的值
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetBit(byte b, int index)
        {
            return ((b & (1 << index)) > 0) ? 1 : 0;
        }

        /// <summary>
        /// 将第index位设为1
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static byte SetBit(byte b, int index)
        {
            return (byte)(b | (1 << index));
        }

        /// <summary>
        /// 将第index位设为0
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static byte ClearBit(byte b, int index)
        {
            return (byte)(b & (byte.MaxValue - (1 << index)));
        }

        /// <summary>
        /// 将第index位取反
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>

        public static byte ReverseBit(byte b, int index)
        {
            return (byte)(b ^ (byte)(1 << index));
        }

        /// <summary>
        /// 将byte转换成对应的8421BCD码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string To8421BCDString(byte data)
        {
            byte _bcdByte = data;
            int _idHigh = _bcdByte >> 4;
            int _idLow = _bcdByte & 0x0F;

            return string.Format("{0}{1}", _idHigh, _idLow);
        }

        #region 将byte数组转为BCD字符串描述

        /// <summary>
        /// 将byte数组转为BCD字符串描述
        /// <para>eg: Assert.AreEqual("1001", ByteHelper.ToBinaryCodedDecimal(new byte[2] { 0x01, 0x10 }, true));</para>
        /// <para>eg: Assert.AreEqual("0110", ByteHelper.ToBinaryCodedDecimal(new byte[2] { 0x01, 0x10 }, false));</para>
        /// </summary>
        /// <param name="data">Byte数组</param>
        /// <param name="isLittleEndian">是否低位在前高位在后</param>
        /// <returns>BCD描述</returns>
        public static string To8421BCDString(byte[] data, bool isLittleEndian)
        {
            StringBuilder _builder = new StringBuilder(data.Length * 2);
            if (isLittleEndian)
            {
                for (int i = data.Length - 1; i >= 0; i--)
                {
                    byte _bcdByte = data[i];
                    int _idHigh = _bcdByte >> 4;
                    int _idLow = _bcdByte & 0x0F;
                    _builder.Append(string.Format("{0}{1}", _idHigh, _idLow));
                }
            }
            else
            {
                for (int i = 0; i < data.Length; i++)
                {
                    byte _bcdByte = data[i];
                    int _idHigh = _bcdByte >> 4;
                    int _idLow = _bcdByte & 0x0F;
                    _builder.Append(string.Format("{0}{1}", _idHigh, _idLow));
                }
            }
            return _builder.ToString();
        }

        #endregion 将byte数组转为BCD字符串描述

        #region 转为bcd码Byte数组描述

        /// <summary>
        /// 字符串转为bcd码Byte数组描述
        /// <para>eg:CollectionAssert.AreEqual(new byte[2] { 0x01, 0x10 }, BCDHelper.ToBinaryCodedDecimal("0110", false));</para>
        /// <para>eg:CollectionAssert.AreEqual(new byte[2] { 0x10, 0x01 }, BCDHelper.ToBinaryCodedDecimal("0110", true));</para>
        /// </summary>
        /// <param name="bcdString">bcd字符串</param>
        /// <param name="isLittleEndian">是否低位在前高位在后</param>
        /// <returns>Byte数组</returns>
        public static byte[] From8421BCDToBytes(string bcdString, bool isLittleEndian)
        {
            byte[] _bytes = null;
            //if (bcdString.IsBinaryCodedDecimal())
            //{
            char[] _chars = bcdString.ToCharArray();
            int _len = _chars.Length / 2;
            _bytes = new byte[_len];
            if (isLittleEndian)
            {
                for (int i = 0; i < _len; i++)
                {
                    byte _highNibble = byte.Parse(_chars[2 * (_len - 1) - 2 * i].ToString());
                    byte _lowNibble = byte.Parse(_chars[2 * (_len - 1) - 2 * i + 1].ToString());
                    _bytes[i] = (byte)((byte)(_highNibble << 4) | _lowNibble);
                }
            }
            else
            {
                for (int i = 0; i < _len; i++)
                {
                    byte _highNibble = byte.Parse(_chars[2 * i].ToString());
                    byte _lowNibble = byte.Parse(_chars[2 * i + 1].ToString());
                    _bytes[i] = (byte)((byte)(_highNibble << 4) | _lowNibble);
                }
            }
            //}
            return _bytes;
        }

        /// <summary>
        /// Int转为bcd码Byte数组描述
        /// </summary>
        /// <param name="bcdNumber"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static byte[] From8421BCDToBytes(int bcdNumber, bool isLittleEndian)
        {
            string _bcdString = bcdNumber.ToString();
            if (!IsBinaryCodedDecimal(_bcdString))
                _bcdString = _bcdString.PadLeft(_bcdString.Length + 1, '0');
            return From8421BCDToBytes(_bcdString, isLittleEndian);
        }

        #endregion 转为bcd码Byte数组描述

        #region 转为bcd码Byte描述

        /// <summary>
        /// 转为bcd码Byte描述
        /// 其中高四位存放十位数字，低四位存放个位数字。
        /// </summary>
        /// <param name="bcdNumber">数字</param>
        /// <returns>Byte描述</returns>
        public static byte From8421BCDToByte(int bcdNumber)
        {
            byte _bcd = (byte)(bcdNumber % 10);
            bcdNumber /= 10;
            _bcd |= (byte)((bcdNumber % 10) << 4);
            bcdNumber /= 10;
            return _bcd;
        }

        #endregion 转为bcd码Byte描述

        /// <summary>  
        /// 验证Bcd码 e.g. "01" or "3456"
        /// </summary>
        public const string BinaryCodedDecimal = @"^([0-9]{2})+$";

        /// <summary>
        /// 判断是否是BCD字符串
        /// </summary>
        /// <param name="data">验证字符串</param>
        /// <returns>否是BCD字符串</returns>
        public static bool IsBinaryCodedDecimal(string data)
        {
            return IsMatch(data, BinaryCodedDecimal);
        }

        /// <summary>
        /// 正则表达式匹配，匹配返回true
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">模式字符串</param>
        public static bool IsMatch(string input, string pattern)
        {
            return IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 正则表达式匹配，匹配返回true
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="pattern">模式字符串</param>
        /// <param name="options">筛选条件</param>
        public static bool IsMatch(string input, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(input, pattern, options);
        }

        /// <summary>
        /// 将指定字符串转换成ASCII码表示的特定长度的字符数组，位数不足则后面补0
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static byte[] StrToASCII(string str, int len)
        {
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            byte[] bAscii = ascii.GetBytes(str);
            int asciiLen = bAscii.Length;
            if (asciiLen == len)
            {
                return bAscii;
            }
            else
            {
                byte[] bApn = new byte[len];
                Buffer.BlockCopy(bAscii, 0, bApn, 0, asciiLen);
                if (asciiLen < len)
                {
                    for (int i = asciiLen; i < len; i++)
                    {
                        bApn[i] = 0;
                    }
                }
                return bApn;
            }
        }

        /// <summary>
        /// 将字节数组表示的ＡＳＣＩＩ码转换成字符串
        /// </summary>
        /// <param name="dataUnit"></param>
        /// <param name="len"></param>
        /// <param name="curIndex"></param>
        /// <returns></returns>
        public static string GetStringFromBytes(byte[] dataUnit, int len, ref int curIndex)
        {
            byte[] data = new byte[len];
            Buffer.BlockCopy(dataUnit, curIndex, data, 0, len);
            curIndex += len;

            return System.Text.Encoding.ASCII.GetString(data).Replace("\0", "");
        }

        /// <summary>
        /// 将BCD码转换成字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        public static string ToStrFromBCD(byte[] data, bool reverse)
        {
            string result = "";

            int len = data.Length;
            if (reverse)
            {
                for (int i = len - 1; i >= 0; i--)
                {
                    result += data[i].ToString("X2");
                }
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    result += data[i].ToString("X2");
                }
            }

            return result;
        }

        /// <summary>
        /// 将十进制数转换成BCD码表示的字节数组
        /// </summary>
        /// <param name="val"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static byte[] ToBCDBytesFromUlong(ulong val, int len)
        {
            byte[] result = new byte[len];
            int curIndex = 0;
            ulong curVal = val;
            while (true)
            {
                ulong remainder = 0;
                ulong quotient = 0;
                quotient = curVal / 100;
                remainder = curVal % 100;

                result[curIndex++] = Convert.ToByte(remainder.ToString(), 16);
                if (quotient == 0) break;
                curVal = quotient;
            }
            for (int i = curIndex; i < len; i++)
            {
                result[i] = 0;
            }

            return result;
        }

        public static string GetVersion(byte[] data,int startIndex)
        {
            //版本号只有两个字节
            if (data.Length < 2) return "";

            return "V" + data[startIndex].ToString("X2") +"."+ data[startIndex+1].ToString("X2");
        }
    }
}
