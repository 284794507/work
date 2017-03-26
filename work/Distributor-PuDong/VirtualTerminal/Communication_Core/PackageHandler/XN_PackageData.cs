using Communication_Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication_Core.PackageHandler
{
    public static class PackageConst
    {
        /// <summary>
        /// 同步字
        /// </summary>
        public const byte SynWord = 0x68;

        /// <summary>
        /// 结束字
        /// </summary>
        public const byte EndTag = 0x16;
    }

    public class XNPackageData
    {
        #region 属性

        /// <summary>
        /// 地址--8个字节
        /// </summary>
        public byte[] Address
        {
            get;
            set;
        }

        /// <summary>
        /// AFN(DIR PRM ACD FC)
        /// </summary>
        public byte AFN
        {
            get;
            set;
        }

        /// <summary>
        /// 功能码
        /// </summary>
        public byte AFN_FC
        {
            get
            {
                //例如登陆  0xC2; //11000010
                byte _temp = (byte)(AFN << 3);//左移动3位得00010000
                return (byte)(_temp >> 3);//右移动3位得到00000010
            }
        }

        /// <summary>
        /// 附加信息域（包括EC和TP）
        /// </summary>
        public byte[] AUX
        {
            get;
            set;
        }

        /// <summary>
        /// EC事件计数器2个字节（EC1和EC2）
        /// </summary>
        public byte[] AUX_EC
        {
            get;
            set;
        }

        /// <summary>
        /// Tp--6个字节
        /// 启动帧帧序号计数器PFC(1)
        /// 启动帧发送时标(4)
        /// 允许发送传输延时时间(1)
        /// </summary>
        public byte[] AUX_TP
        {
            get;
            set;
        }

        /// <summary>
        /// 启动帧帧序号计数器PFC(1)
        /// </summary>
        public byte[] AUX_EC_PFC
        {
            get
            {
                if (AUX_EC != null && AUX_EC.Length == 6)
                {
                    return new byte[] { AUX_EC[0], AUX_EC[1] };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// CRC校验码（2个字节）
        /// 校验包括地址域、链路用户数据（应用层）二部分
        /// </summary>
        public byte[] CRC
        {
            get;
            set;
        }

        /// <summary>
        /// 2个字节长度域（地址域+应用层长度）（ 包里2个字节，双倍4个字节组成）
        /// </summary>
        public byte[] DataLength
        {
            get;
            set;
        }

        /// <summary>
        /// 数据单元_报文数据
        /// </summary>
        public byte[] DataUnit_Customer
        {
            get;
            set;
        }

        /// <summary>
        /// 数据单元标示PnFn--4四个字节
        /// </summary>
        public byte[] DataUnit_PnFN
        {
            get;
            set;
        }

        /// <summary>
        /// 数据标识字符串
        /// </summary>
        public string PnFn
        {
            get;
            set;
        }

        /// <summary>
        /// DA
        /// </summary>
        public string Pn
        {
            get;
            set;
        }

        /// <summary>
        /// DT
        /// </summary>
        public string Fn
        {
            get;
            set;
        }

        /// <summary>
        /// 是否启用Tp
        /// </summary>
        public bool EnableTP
        {
            get;
            set;
        }

        /// <summary>
        /// ACD表示是否带事件记数器EC
        /// </summary>
        public bool HasEC
        {
            get;
            set;
        }

        /// <summary>
        /// 结束码
        /// </summary>
        public byte EndTag
        {
            get;
            set;
        }

        /// <summary>
        /// SEQ(TPV FIR FIN CON P/RSEQ)
        /// </summary>
        public byte SEQ
        {
            get;
            set;
        }

        /// <summary>
        /// 同步字1
        /// </summary>
        public byte SynWord1
        {
            get;
            set;
        }

        /// <summary>
        /// 同步字2
        /// </summary>
        public byte SynWord2
        {
            get;
            set;
        }

        /// <summary>
        /// 链路用户数据
        /// </summary>
        public byte[] UserDataAll
        {
            get;
            set;
        }

        public byte[] DataUnit
        {
            get;
            set;
        }

        #endregion 属性

        /// <summary>
        /// 欣能协议构造函数(DIR=0都是由主站发出的下行报文)
        /// </summary>
        /// <param name="clientAddress">终端地址</param>
        /// <param name="afn">AFN</param>
        /// <param name="pnfn">PNFN</param>
        /// <param name="dataUnit">用户数据体</param>
        /// <param name="enableTP">是否带时间标签</param>
        /// <param name="enablePRM">是否启动站</param>
        /// <param name="pfc">启动帧帧序号计数器PFC.</param>
        /// <param name="date">用于tp</param>
        public XNPackageData(byte[] clientAddress, byte afn, string pnfn, byte[] dataUnit, bool enableTP, bool enablePRM, byte pfc, DateTime date, byte delayNum, int fir, int fin)
        {
            SynWord1 = PackageConst.SynWord;
            SynWord2 = PackageConst.SynWord;
            EndTag = PackageConst.EndTag;
            Address = clientAddress;
            SEQ = 0;
            SEQ = ByteHelper.SetBit(SEQ, 4);//CON赋值1
            if (fin == 1)
            {
                SEQ = ByteHelper.SetBit(SEQ, 5);//fin赋值1
            }
            if (fir == 1)
            {
                SEQ = ByteHelper.SetBit(SEQ, 6);//fir赋值1
            }

            if (enableTP)
            {
                SEQ = ByteHelper.SetBit(SEQ, 7);//TPV赋值1
                byte[] _tp = new byte[6];
                _tp[0] = ByteHelper.GetLow(pfc);//启动帧帧序号计数器PFC
                _tp[1] = Convert.ToByte(date.Second.ToString(), 16);
                _tp[2] = Convert.ToByte(date.Minute.ToString(), 16);
                _tp[3] = Convert.ToByte(date.Hour.ToString(), 16);
                _tp[4] = Convert.ToByte(date.Day.ToString(), 16);
                _tp[5] = delayNum;//允许发送传输延时
                AUX = _tp;
                //Array.Copy(_tp, 0, AUX, 0, _tp.Length);
            }

            DataUnit_PnFN = XNPkgDataHandler.GetInstance.SetPnFnBytes(pnfn);
            int _dataUnitLenght = dataUnit == null ? 0 : dataUnit.Length;
            int dataAllLenght = enableTP ? 1 + 1 + 4 + _dataUnitLenght + 6 : 1 + 1 + 4 + _dataUnitLenght;
            UserDataAll = new byte[dataAllLenght];//afn + seq + pnfn+dataUnit+tp
            //UserDataAll = new byte[1 + 1 + 4 + _dataUnitLenght];//afn + seq + pnfn
            UserDataAll[0] = afn;

            if (enablePRM) //是否是启动站
            {
                UserDataAll[0] = ByteHelper.SetBit(UserDataAll[0], 6);
            }
            //add 20160608 从动站不需要回应
            else
            {
                SEQ = ByteHelper.ClearBit(SEQ, 4);//CON赋值0
            }

            AFN = UserDataAll[0];
            UserDataAll[1] = SEQ;
            Array.Copy(DataUnit_PnFN, 0, UserDataAll, 2, 4);

            if (dataUnit != null)
            {
                DataUnit_Customer = dataUnit;
                //Array.Copy(dataUnit, 0, DataUnit_Customer, 0, _dataUnitLenght);
                Array.Copy(dataUnit, 0, UserDataAll, 6, _dataUnitLenght);
            }

            if (AUX != null)
            {
                Array.Copy(AUX, 0, UserDataAll, 6 + _dataUnitLenght, 6);
            }

            DataLength = BitConverter.GetBytes(Convert.ToUInt16(Address.Length + UserDataAll.Length));
            CRC = new byte[2];
            CRC = ByteHelper.GetCrc16_Bytes(Address, UserDataAll);
        }

        /// <summary>
        /// 构造函数
        /// 不带时间标签，
        /// 默认启动站，
        /// 自定义数据单元为NULL
        /// </summary>
        /// <param name="clientAddress">终端地址</param>
        /// <param name="afn">AFN</param>
        /// <param name="pnfn">PNFN</param>
        /// 时间：2016/6/12 20:00
        /// 备注：
        public XNPackageData(byte[] clientAddress, byte afn, string pnfn) : this(clientAddress, afn, pnfn, null, false, true, 0x00, DateTime.Now, 0x00, 1, 1)
        {
        }

        /// <summary>
        /// 构造函数，可指定主动从动站
        /// 默认不带时间标签，默认数据单元空值
        /// </summary>
        /// <param name="clientAddress">终端地址</param>
        /// <param name="afn">AFN</param>
        /// <param name="pnfn">PNFN</param>
        /// <param name="enablePRM">主动从动</param>
        public XNPackageData(byte[] clientAddress, byte afn, string pnfn, bool enablePRM)
        : this(clientAddress, afn, pnfn, null, false, enablePRM, 0x00, DateTime.Now, 0x00, 1, 1)
        {
        }

        /// <summary>
        /// 构造函数
        /// 带时间标签==》当前时间，
        /// PFC==>0
        /// 默认启动站，
        /// </summary>
        /// <param name="clientAddress">终端地址</param>
        /// <param name="afn">AFN</param>
        /// <param name="pnfn">PNFN</param>
        /// <param name="dataUnit">自定义数据单元</param>
        /// 时间：2016/6/12 20:03
        /// 备注：
        public XNPackageData(byte[] clientAddress, byte afn, string pnfn, byte[] dataUnit) : this(clientAddress, afn, pnfn, dataUnit, true, true, 0x00, DateTime.Now, 0x00, 1, 1)
        {
        }

        /// <summary>
        /// 欣能协议构造函数
        /// </summary>
        /// <param name="clientAddress">终端地址</param>
        /// <param name="afn">AFN</param>
        /// <param name="pnfn">PNFN</param>
        /// <param name="dataUnit">用户数据体</param>
        /// <param name="enableTP">是否带时间标签</param>
        /// <param name="enablePRM">是否启动站</param>
        /// <param name="pfc">启动帧帧序号计数器PFC.</param>
        /// <param name="date">用于tp</param>
        /// <param name="fir">置“1”，报文的第一帧</param>
        /// <param name="fin">置“1”，报文的最后一帧</param>
        /// 时间：2016/6/12 20:31
        /// 备注：
        public XNPackageData(byte[] clientAddress, byte afn, string pnfn, byte[] dataUnit, bool enableTP, bool enablePRM, int fir, int fin) : this(clientAddress, afn, pnfn, dataUnit, enableTP, enablePRM, 0x00, DateTime.Now, 0x00, fir, fin)
        {
        }

        /// <summary>
        /// 欣能协议构造函数
        /// </summary>
        /// <param name="clientAddress">终端地址</param>
        /// <param name="afn">AFN</param>
        /// <param name="pnfn">PNFN</param>
        /// <param name="dataUnit">用户数据体</param>
        /// <param name="enableTP">是否带时间标签</param>
        /// <param name="enablePRM">是否启动站</param>
        /// <param name="pfc">启动帧帧序号计数器PFC.</param>
        /// <param name="date">用于tp</param>
        /// 时间：2016/6/12 20:31
        /// 备注：
        public XNPackageData(byte[] clientAddress, byte afn, string pnfn, byte[] dataUnit, bool enableTP, bool enablePRM) : this(clientAddress, afn, pnfn, dataUnit, enableTP, enablePRM, 0x00, DateTime.Now, 0x00, 1, 1)
        {
        }

        /// <summary>
        /// 默认欣能构造函数
        /// </summary>
        /// 时间：2016/6/1 9:28
        /// 备注：
        public XNPackageData()
        {
        }

        /// <summary>
        /// 根据字节数组进行分析合法性判断，并在返回参数中生成通讯包对象和合法性判断的信息
        /// </summary>
        /// <param name="Buffer">字节数组</param>
        /// <param name="package">CTU通讯包对象</param>
        /// <param name="error">包对象生成合法性与否的信息</param>
        /// <returns></returns>
        public static bool BuileObjFromBytes(byte[] Buffer, out XNPackageData package, out byte[] afterPackage, out XNPackageErrEnum error)
        {
            //68
            //1A 00 1A 00  //长度
            //68
            //66 26 00 00 00 00 00 00 //地址
            //05 //AFN
            //F2 //SEQ
            //00 00 40 03 //FNPN
            //15 05 14 23 04 15 //用户数据
            //12 50 31 13 27 00 //TP
            //9D 0C 16
            bool flag = false;
            //初始化进函数赋予空值
            afterPackage = null;
            package = new XNPackageData();
            error = XNPackageErrEnum.Normal;

            try
            {
                using (MemoryStream mem = new MemoryStream(Buffer))
                {
                    BinaryReader reader = new BinaryReader(mem);
                    //读取第一个同步字
                    package.SynWord1 = reader.ReadByte();

                    //验证第一个同步字合法性
                    if (package.SynWord1 != PackageConst.SynWord)
                    {
                        error = XNPackageErrEnum.SynWordError;
                        return false;
                    }

                    //跳过4个字节，读取1字节同步字
                    reader.BaseStream.Position += 4;
                    package.SynWord2 = reader.ReadByte();

                    //验证2个同步字是否一致
                    if (package.SynWord1 != package.SynWord2)
                    {
                        error = XNPackageErrEnum.SynWordMatchError;
                        return false;
                    }

                    //返回5个字节读取数据长度和地址
                    reader.BaseStream.Position -= 5;
                    package.DataLength = reader.ReadBytes(2);
                    ushort datalength = BitConverter.ToUInt16(package.DataLength, 0);
                    //继续读取下2个字节表示的长度
                    byte[] nextLength = reader.ReadBytes(2);

                    //前2个字节和后2个字节表示的长度不一致
                    if (!ByteHelper.ByteArryEquals(package.DataLength, nextLength))
                    {
                        error = XNPackageErrEnum.DoubleDataLengthMatchError;
                        return false;
                    }

                    //验证整个包的长度合法性
                    //包的长度 = 当前位置 +同步字（1）+ datalength  + CRC（2） + ENDTAG（1）
                    long tmpPackLengh = reader.BaseStream.Position + 1 + datalength + 2 + 1;

                    if (Buffer.Length != tmpPackLengh)
                    {
                        //实际收到的包比包格式定义的要大
                        if (Buffer.Length > tmpPackLengh)
                        {
                            //Console.WriteLine("可能有粘包！");
                            //20130322 粘包长度
                            long afterPackageLength = Buffer.Length - tmpPackLengh;
                            afterPackage = new Byte[afterPackageLength];
                            Array.Copy(Buffer, tmpPackLengh, afterPackage, 0, afterPackageLength);
                        }
                        else
                        {
                            //可能有丢包
                            //log记录
                            //Console.WriteLine("可能有丢包！");
                            error = XNPackageErrEnum.PackageLengthError;
                            return false;
                        }

                        //粘包没有报错，因为只取第一个包了
                    }

                    //跳过已经读取的第二个同步字
                    reader.BaseStream.Position += 1;
                    package.Address = reader.ReadBytes(8);
                    //空余待开发；CTUAddr合法性检查
                    //do sth
                    ///所有链路用户数据
                    package.UserDataAll = reader.ReadBytes(datalength - 8);
                    //赋值 AFN
                    package.AFN = package.UserDataAll[0];
                    //赋值seq
                    package.SEQ = package.UserDataAll[1];
                    //赋值TPV/EnableTP
                    package.EnableTP = ByteHelper.GetBit(package.SEQ, 7) == 1 ? true : false;
                    //赋值TP
                    package.AUX_TP = new byte[0];

                    if (package.EnableTP)
                    {
                        package.AUX_TP = new byte[6];
                        System.Buffer.BlockCopy(package.UserDataAll, datalength - 8 - 6, package.AUX_TP, 0, 6);
                    }

                    //赋值ACD/HasEC
                    package.HasEC = ByteHelper.GetBit(package.AFN, 5) == 1 ? true : false;
                    //赋值EC
                    package.AUX_EC = new byte[0];

                    if (package.HasEC)
                    {
                        package.AUX_EC = new byte[2];
                        int eclen = 2;

                        if (package.EnableTP) eclen += 6;

                        System.Buffer.BlockCopy(package.UserDataAll, datalength - 8 - eclen, package.AUX_EC, 0, 2);
                    }

                    package.DataUnit = new byte[0];
                    int dataLen = package.UserDataAll.Length - (6 + package.AUX_TP.Length + package.AUX_EC.Length);

                    if (dataLen > 0)
                    {
                        package.DataUnit = new byte[dataLen];
                        System.Buffer.BlockCopy(package.UserDataAll, 6, package.DataUnit, 0, dataLen);
                    }
                    package.DataUnit_Customer = package.DataUnit;
                    package.DataUnit_PnFN = new byte[4];
                    //赋值PnFn
                    Array.Copy(package.UserDataAll, 2, package.DataUnit_PnFN, 0, 4);
                    package.PnFn = package.GetPnFnString();
                    package.Pn = package.GetPnString();
                    package.Fn = package.GetFnString();
                    //CRC验证码
                    package.CRC = reader.ReadBytes(2);
                    //所有用户数据区(需要CRC16)
                    byte[] addr_Userdata = new byte[package.Address.Length + package.UserDataAll.Length];
                    Array.Copy(package.Address, 0, addr_Userdata, 0, package.Address.Length);
                    Array.Copy(package.UserDataAll, 0, addr_Userdata, package.Address.Length, package.UserDataAll.Length);

                    if (!ByteHelper.CheckCrc16(addr_Userdata, package.CRC))
                    {
                        error = XNPackageErrEnum.CRCError;
                        return false;
                    }

                    //结束码
                    package.EndTag = reader.ReadByte();

                    //验证包尾
                    if (package.EndTag != PackageConst.EndTag)
                    {
                        error = XNPackageErrEnum.EndTagError;
                        return false;
                    }

                    reader.Close();
                    //最后标志置为true
                    flag = true;
                }
            }
            catch
            {
                error = XNPackageErrEnum.ExceptionError;
                return false;
            }

            return flag;
        }

        /// <summary>
        /// 初始化数据包对象（同步字1,2；结束码)
        /// </summary>
        /// <returns></returns>
        public static XNPackageData InitPackage()
        {
            XNPackageData package = new XNPackageData();
            package.SynWord1 = PackageConst.SynWord;
            package.SynWord2 = PackageConst.SynWord;
            package.EndTag = PackageConst.EndTag;
            //DTDataMarkers = CreateDTDUMarker();
            //DADataMakers = CreateDADUMarker();
            //FnStringList = InitDTStringArray();
            //PnStringList = InitDAStringArray();
            return package;
        }

        /// <summary>
        /// 获取PNFN字符串码
        /// </summary>
        /// <param name="pnfn"></param>
        /// <returns></returns>
        public string GetPnFnString()
        {
            return XNPkgDataHandler.GetInstance.GetPnFnString(this.DataUnit_PnFN);
        }

        /// <summary>
        /// 获取PN字符串码
        /// </summary>
        /// <param name="pn"></param>
        /// <returns></returns>
        public string GetPnString()
        {
            return XNPkgDataHandler.GetInstance.GetPnString(this.DataUnit_PnFN.Take(2).ToArray());
        }

        /// <summary>
        /// 获取FN字符串码
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public string GetFnString()
        {
            return XNPkgDataHandler.GetInstance.GetFnString(this.DataUnit_PnFN.Skip(2).ToArray());
        }

        #region 已被注释

        //#region  DTDAMarker
        //public static string[,] DTDataMarkers = null;
        //public static string[,] DADataMakers = null;

        //#endregion 变量

        //#region 创建DT数据表示

        ///// <summary>
        ///// 创建DT数据表示
        ///// </summary>
        //public static string[,] CreateDTDUMarker()
        //{
        //    string[,] _dt = new string[201, 8];
        //    for (int i = 0; i < 200; i++)
        //    {
        //        for (int j = 0; j < 8; j++)
        //        {
        //            string _ff = string.Format("F{0}", 8 * (i + 1) - j);
        //            _dt[i, j] = _ff;
        //            Debug.Write(_dt[i, j] + " ");
        //        }
        //        Debug.WriteLine(Environment.NewLine);
        //    }
        //    return _dt;
        //}

        //#endregion 创建DT数据表示

        //#region 创建DA数据标识

        ///// <summary>
        ///// 创建DA数据标识
        ///// </summary>
        ///// <returns></returns>
        //public static string[,] CreateDADUMarker()
        //{
        //    string[,] _da = new string[201, 9];
        //    for (int i = 1; i < 200; i++)
        //    {
        //        int k = 0;
        //        for (int j = 0; j < 9; j++)
        //        {
        //            string _ff = string.Format("P{0}", 8 * i - j);
        //            _da[i, j] = _ff;
        //            Debug.Write(_da[i, j] + " ");
        //            k++;
        //        }
        //        Debug.WriteLine(Environment.NewLine);
        //    }
        //    return _da;
        //}

        //#endregion 创建DA数据标识

        //#region 数据单元标示_按列索引

        ///// <summary>
        ///// 数据单元标示_按列索引
        ///// </summary>
        ///// <param name="dadt"></param>
        ///// <returns></returns>
        //private static int GetPnFnIndex_Col(byte dadt)
        //{
        //    int _index = 0;
        //    if (dadt != 0x00)
        //    {
        //        string _dadtBinaryString = ByteHelper.ByteToBinaryString(dadt);
        //        _index = _dadtBinaryString.IndexOf('1');
        //    }
        //    return _index;
        //}

        //#endregion 数据单元标示_按列索引

        //#region 数据单元标示_按行索引

        ///// <summary>
        ///// 数据单元标示_按行索引
        ///// </summary>
        //private static int GetPnFnIndex_Row(byte dadt)
        //{
        //    int _index = 0;
        //    if (dadt != 0x00)
        //    {
        //        string _dadtBinaryString = ByteHelper.ByteToBinaryString(dadt).Reverse();
        //        _index = _dadtBinaryString.IndexOf('1') + 1;
        //    }
        //    return _index;
        //}

        //#endregion 数据单元标示_按行索引

        #endregion 已被注释

        public byte[] ToBytes()
        {
            byte[] _byte;

            try
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(mem);
                    writer.Write(SynWord1);
                    writer.Write(DataLength);
                    writer.Write(DataLength);
                    writer.Write(SynWord2);
                    writer.Write(Address);
                    writer.Write(AFN);
                    writer.Write(SEQ);
                    writer.Write(DataUnit_PnFN);

                    ////用户数据可能为空
                    if (DataUnit_Customer != null)
                    {
                        writer.Write(DataUnit_Customer);
                    }

                    //附加信息域可能为空
                    if (AUX != null && AUX.Length>0)
                    {
                        writer.Write(AUX);
                    }
                    else 
                    {
                        if (AUX_EC != null && AUX_EC.Length > 0)
                        {
                            writer.Write(AUX_EC);
                        }
                        else if (AUX_TP != null && AUX_TP.Length > 0)
                        {
                            writer.Write(AUX_TP);
                        }
                    }

                    writer.Write(CRC);
                    writer.Write(EndTag);
                    _byte = mem.ToArray();
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return _byte;
        }
    }

    public class XNPkgDataHandler
    {
        #region 静态初始化实例

        private static XNPkgDataHandler _xnPackageDataCfg;

        public static XNPkgDataHandler GetInstance
        {
            get
            {
                if (_xnPackageDataCfg == null)
                    _xnPackageDataCfg = new XNPkgDataHandler();

                return _xnPackageDataCfg;
            }
        }

        #endregion 静态初始化实例

        #region 构造函数

        public XNPkgDataHandler()
        {
            FnStringList = InitDTStringArray();
            PnStringList = InitDAStringArray();
        }

        #endregion 构造函数

        public static string[] FnStringList = null;

        public static string[] PnStringList = null;

        /// <summary>
        /// 获取FN值
        /// </summary>
        /// <param name="da"></param>
        /// <returns></returns>
        public string GetFnString(byte[] dt)
        {
            if (dt[0] == 0 && dt[1] == 0)
            {
                return "F0";
            }

            string retString = "";
            int cNum_hex = Convert.ToInt16(dt[0]);
            // 底数函数求指数 例如： 2的3次方= 8 ，返回的是指数3 就是列序号
            int cNum = (int)Math.Log(cNum_hex, 2);
            int rNum = Convert.ToInt16(dt[1]);
            //F的行号从0开始
            int startRIndex = rNum * 8;
            //P的列号
            int startCindex = cNum + 1;
            retString = FnStringList[startRIndex + cNum];
            return retString;
        }

        /// <summary>
        /// 获取PNFN字符串
        /// </summary>
        /// <param name="pnfn"></param>
        /// <returns></returns>
        public string GetPnFnString(byte[] pnfn)
        {
            if (pnfn == null)
            {
                throw new Exception("pnfn的字节数组为空！");
            }

            byte[] pnBytes = new byte[2];
            Array.Copy(pnfn, 0, pnBytes, 0, 2);
            string PnStr = GetPnString(pnBytes);
            byte[] fnBytes = new byte[2];
            Array.Copy(pnfn, 2, fnBytes, 0, 2);
            string FnStr = GetFnString(fnBytes);
            return PnStr + ":" + FnStr;
        }

        /// <summary>
        /// 获取PN值
        /// </summary>
        /// <param name="da"></param>
        /// <returns></returns>
        public string GetPnString(byte[] da)
        {
            if (da[0] == 0 && da[1] == 0)
            {
                return "P0";
            }

            string retString = "";
            int cNum_hex = Convert.ToInt16(da[0]);
            // 底数函数求指数 例如： 2的3次方= 8 ，返回的是指数3 就是列序号
            int cNum = (int)Math.Log(cNum_hex, 2);
            int rNum = Convert.ToInt16(da[1]);
            //P的行号从1开始;数组里-1
            int startRIndex = (rNum - 1) * 8;
            //P的列号
            int startCindex = cNum + 1;
            retString = PnStringList[startRIndex + cNum];
            return retString;
        }

        //信息类组DT2   信息类元D T1
        //D7～D0   D7  D6  D5  D4  D3  D2  D1  D0
        //0         F8  F7  F6  F5  F4  F3  F2  F1
        //1         F16 F15 F14 F13 F12 F11 F10 F9
        //2         F24 F23 F22 F21 F20 F19 F18 F17
        //……    ……  ……  ……  ……  ……  ……  ……  ……
        //30    F248    F247    F246    F245    F244    F243    F242    F241
        //……    未定义
        //255
        /// <summary>
        /// 初始化DA列表（Pn)；行号从0开始
        /// </summary>
        /// <returns></returns>
        public string[] InitDAStringArray()
        {
            string[] daString = new string[255 * 8];

            for (int i = 0; i < daString.Length; i++)
            {
                daString[i] = string.Format("P{0}", i + 1);
            }

            return daString;
        }

        // 信息点组DA2  信息点元DA1
        //D7～D0   D7  D6  D5  D4  D3  D2  D1  D0
        //1         p8  p7  p6  p5  p4  p3  p2  p1
        //2         p16 p15 p14 p13 p12 p11 p10 p9
        //3         p24 p23 p22 p21 p20 p19 p18 p17
        //……    ……  ……  ……  ……  ……  ……  ……  ……
        //255   P2040   P2039   P2038   P2037   P2036   P2035   P2034   P2033
        /// <summary>
        /// 初始化DT列表（Fn);行号从1开始
        /// </summary>
        /// <returns></returns>
        public string[] InitDTStringArray()
        {
            string[] dtString = new string[255 * 8];

            for (int i = 0; i < dtString.Length; i++)
            {
                dtString[i] = string.Format("F{0}", i + 1);
            }

            return dtString;
        }

        /// <summary>
        /// 根据pnfn字符串得到bytes数组（输入P0：F1得到 00 00 01 00)
        /// 函数算法 例：F12 取到12/8 得到 第二行，第四个值
        /// </summary>
        /// <param name="pnfnString"></param>
        /// <returns></returns>
        public byte[] SetPnFnBytes(string pnfnString)
        {
            if (string.IsNullOrEmpty(pnfnString))
            {
                throw new Exception("pnfnString输入字符串为空");
            }

            byte[] pnbytes = new byte[2];
            byte[] fnbytes = new byte[2];
            byte[] pnfnbytes = new Byte[4];
            string[] pnfnlist = pnfnString.Split(':');
            string pnStr = pnfnlist[0];
            string fnStr = pnfnlist[1];

            if (pnStr == "P0")
            {
            }
            else
            {
                int pnIndex = Convert.ToInt16(pnStr.Substring(1));
                int pnNum = pnIndex / 8;
                int pnNum_remain = pnIndex % 8;
                //申明行数据
                byte pnRbyte = 0;
                //申明列数据
                byte pnCbyte = 0;

                //余数是0; pnNum是行数据得到后 赋值da2
                if (pnNum_remain == 0)
                {
                    //pn行号从1开始
                    pnRbyte = Convert.ToByte(pnNum);
                    //pnNum_remain余数是列数据 赋值给da1
                    pnCbyte = ByteHelper.SetBit(pnCbyte, 7);
                }
                else
                {
                    pnRbyte = Convert.ToByte(pnNum + 1);
                    //pnNum_remain余数是列数据 赋值给da1
                    pnCbyte = ByteHelper.SetBit(pnCbyte, pnNum_remain - 1);
                }

                pnbytes[0] = pnCbyte;
                pnbytes[1] = pnRbyte;
            }

            if (fnStr == "F0")
            {
            }
            else
            {
                int fnIndex = Convert.ToInt16(fnStr.Substring(1));
                int fnNum = fnIndex / 8;
                int fnNum_remain = fnIndex % 8;
                //申明行数据
                byte fnRbyte = 0;
                //申明列数据
                byte fnCbyte = 0;

                if (fnNum_remain == 0)
                {
                    //fn 行号从0开始
                    fnRbyte = Convert.ToByte(fnNum - 1);
                    fnCbyte = ByteHelper.SetBit(fnCbyte, 7);
                }
                else
                {
                    fnRbyte = Convert.ToByte(fnNum);
                    fnCbyte = ByteHelper.SetBit(fnCbyte, fnNum_remain - 1);
                }

                fnbytes[0] = fnCbyte;
                fnbytes[1] = fnRbyte;
            }

            Array.Copy(pnbytes, 0, pnfnbytes, 0, 2);
            Array.Copy(fnbytes, 0, pnfnbytes, 2, 2);
            return pnfnbytes;
        }
    }
}
