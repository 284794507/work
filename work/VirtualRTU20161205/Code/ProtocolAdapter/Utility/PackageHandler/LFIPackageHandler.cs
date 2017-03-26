using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.PackageHandler
{
    public class LFIPackageHandler
    {
        public byte SynWord1 { get; set; }

        public byte[] DataLength { get; set; }

        /// <summary>
        /// 8字节
        /// </summary>
        public byte[] DevAddr { get; set; }

        public byte SynWord2 { get; set; }

        public byte[] CmdWord { get; set; }

        public byte[] OnlyData { get; set; }

        /// <summary>
        /// 包括验证码的所以信息
        /// </summary>
        public byte[] AllData { get; set; }

        public byte[] CRC { get; set; }

        public byte EndTag { get; set; }

        public byte[] afterPackage { get; set; }

        public PackageErr curError { get; set; }

        public bool AnalySuccess { get; set; }

        private readonly byte StartMark = 0x68;
        private readonly byte EndMark = 0x16;

        private readonly int LLLen = 2;
        private readonly int AddrLen = 8;
        private readonly int SynWordLen = 1;
        private readonly int CmdLen = 2;
        private readonly int CrcLen = 1;
        private readonly int EndTagLen = 1;

        public LFIPackageHandler()
        {
            this.SynWord1 = StartMark;
            this.SynWord2 = StartMark;
            this.EndTag = EndMark;

            afterPackage = null;
            curError = PackageErr.Normal;
            AnalySuccess = true;
        }

        public LFIPackageHandler(byte[] address, byte[] data, byte[] cmd) : this()
        {
            int len = 0;
            AspectF.Define.Retry().Do(() =>
            {
                if (data != null)
                {
                    len = data.Length;
                }
                this.DataLength = BitConverter.GetBytes((short)(2 + len));
                this.DevAddr = address;
                this.CmdWord = cmd;
                this.OnlyData = data;
                this.AllData = new byte[LLLen + AddrLen + SynWordLen + CmdLen + len];
                this.AllData = GetAllData();
                this.CRC = GetCRC(this.AllData);
            });            
        }

        private byte[] GetAllData()
        {
            byte[] result = null;
            AspectF.Define.Retry().Do(() =>
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(mem);
                    writer.Write(this.DataLength);
                    writer.Write(this.DevAddr);
                    writer.Write(this.SynWord2);
                    writer.Write(this.CmdWord);
                    if (this.OnlyData != null)
                    {
                        writer.Write(this.OnlyData);
                    }
                    result = mem.ToArray();
                    writer.Close();
                }
            });
            
            return result;
        }

        public void BuildPackageFromBytes(byte[] data)
        {
            AnalySuccess = false;
            AspectF.Define.Retry(()=>
            {
                this.afterPackage = null;
                this.curError = PackageErr.ExceptionError;
            }).Do(() =>
            {
                using (MemoryStream mem = new MemoryStream(data))
                {
                    BinaryReader reader = new BinaryReader(mem);

                    this.SynWord1 = reader.ReadByte();
                    this.DataLength = reader.ReadBytes(LLLen);
                    int len = BitConverter.ToUInt16(this.DataLength, 0);
                    long realLen = reader.BaseStream.Position + AddrLen + SynWordLen + len + CrcLen + EndTagLen + 1;
                    this.afterPackage = null;
                    if (data.Length > realLen)
                    {
                        long afterLen = data.Length - realLen;
                        this.afterPackage = new byte[afterLen];
                        Buffer.BlockCopy(data, (int)realLen, this.afterPackage, 0, (int)afterLen);
                    }
                    else if (data.Length < realLen)
                    {
                        this.curError = PackageErr.PackageLengthError;
                        return;
                    }

                    this.DevAddr = reader.ReadBytes(AddrLen);
                    this.SynWord2 = reader.ReadByte();
                    this.CmdWord = reader.ReadBytes(CmdLen);
                    if (len > CmdLen)
                    {
                        this.OnlyData = reader.ReadBytes(len - CmdLen);
                    }
                    else
                    {
                        this.OnlyData = null;
                    }
                    this.AllData = GetAllData();
                    this.CRC = reader.ReadBytes(2);
                    this.EndTag = reader.ReadByte();
                    reader.Close();

                    this.AnalySuccess = CheckPackageIsNotError();
                }
            });
        }

        private bool CheckPackageIsNotError()
        {
            bool flag = true;

            if (this.SynWord1 != StartMark)
            {
                this.curError = PackageErr.SynWordError;
                flag = false;
            }

            if (this.SynWord1 != this.SynWord2)
            {
                this.curError = PackageErr.SynWordMatchError;
                flag = false;
            }

            if (BitConverter.ToUInt16(this.DataLength, 0) < CmdLen)
            {
                this.curError = PackageErr.DataLengthError;
                flag = false;
            }

            if (this.EndTag != EndMark)
            {
                this.curError = PackageErr.EndTagError;
                flag = false;
            }

            if (!ByteHelper.ByteArryEquals(this.CRC, GetCRC(this.AllData)))
            {
                this.curError = PackageErr.CRCError;
                flag = false;
            }
            
            return flag;
        }

        public byte[] ToBytes()
        {
            byte[] result = new byte[0];

            AspectF.Define.Retry().Do(() =>
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(mem);
                    writer.Write(SynWord1);
                    writer.Write(DataLength);
                    writer.Write(DevAddr);
                    writer.Write(SynWord2);
                    writer.Write(CmdWord);
                    if (OnlyData != null && OnlyData.Length != 0)
                    {
                        writer.Write(OnlyData);
                    }
                    writer.Write(CRC);
                    writer.Write(EndTag);

                    result = mem.ToArray();
                    writer.Close();
                }
            });

            return result;
        }
        
        private byte[] GetCRC(byte[] data)
        {
            ushort result=ByteHelper.GetCrc16(data);
            
            return BitConverter.GetBytes(result);            
        }
    }
}
