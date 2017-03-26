using Communicate_Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.PackageHandler
{
    public class Terminal_PackageData
    {
        public byte SynWord1 { get; set; }

        public byte[] DataLength { get; set; }

        /// <summary>
        /// 8字节
        /// </summary>
        public byte[] DevAddr { get; set; }

        public byte SynWord2 { get; set; }

        public byte[] CmdWord { get; set; }

        /// <summary>
        /// 2字节
        /// </summary>
        public byte[] FrameNum { get; set; }

        public byte SEQ { get; set; }

        public byte FN { get; set; }
               
        public byte[] OnlyData { get; set; }

        public byte[] EC { get; set; }

        /// <summary>
        /// 六字节
        /// </summary>
        public byte[] TP { get; set; }

        /// <summary>
        /// 包括验证码的所以信息
        /// </summary>
        public byte[] AllData { get; set; }

        public byte[] CRC { get; set; }

        public byte EndTag { get; set; }

        public byte[] afterPackage { get; set; }

        public PackageErr_CTM curError { get; set; }

        public bool AnalySuccess { get; set; }

        private readonly byte StartMark = 0x68;
        private readonly byte EndMark = 0x16;

        private readonly int LLLen = 2;
        private readonly int AddrLen = 8;
        private readonly int SynWordLen = 1;
        private readonly int CmdLen = 2;
        private readonly int FrameNumLen = 2;
        private readonly int SEQLen = 1;
        private readonly int FNLen = 1;
        private readonly int ECLen = 0;
        private readonly int TPLen = 0;
        private readonly int CrcLen = 2;
        private readonly int EndTagLen = 1;

        public Terminal_PackageData()
        {
            this.SynWord1 = StartMark;
            this.SynWord2 = StartMark;
            this.EndTag = EndMark;

            afterPackage = null;
            curError = PackageErr_CTM.Normal;
            AnalySuccess = true;
        }


        public Terminal_PackageData(byte[] address, byte[] data, byte[] cmd, byte[] frameNum, byte fn) : this(address, data, cmd, frameNum, Share.Instance.GetNewSeq(0x80), fn, ByteHelper.DateTimeToTP(DateTime.Now))
        {
            
        }

        public Terminal_PackageData(byte[] address, byte[] data, byte[] cmd, byte[] frameNum, byte seq, byte fn) : this(address, data, cmd, frameNum, seq, fn, ByteHelper.DateTimeToTP(DateTime.Now))
        {
            
        }

        public Terminal_PackageData(byte[] address, byte[] data, byte[] cmd,byte[] frameNum, byte seq, byte fn, byte[] tp) : this()
        {
            int len = 0;
            if (data != null)
            {
                len = data.Length;
            }
            this.DataLength = BitConverter.GetBytes((short)(2 + len + 2 + 1 + 1 + 6));
            this.DevAddr = address;
            this.CmdWord = cmd;
            this.FrameNum = frameNum;
            //seq = Share.Instance.GetNewSeq(seq);
            this.SEQ = seq;
            this.FN = fn;
            this.OnlyData = data;
            this.TP = tp;
            this.AllData = new byte[LLLen + AddrLen + SynWordLen + CmdLen + FrameNumLen+ SEQLen + FNLen + len + TPLen];
            this.AllData = GetAllData();
            this.CRC = ByteHelper.GetCrc16_Bytes(this.AllData);
        }

        private byte[] GetAllData()
        {
            byte[] result = null;
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    using (MemoryStream mem = new MemoryStream())
                    {
                        BinaryWriter writer = new BinaryWriter(mem);
                        writer.Write(this.DataLength);
                        writer.Write(this.DevAddr);
                        writer.Write(this.SynWord2);
                        writer.Write(this.CmdWord);
                        writer.Write(this.FrameNum);
                        writer.Write(this.SEQ);
                        writer.Write(this.FN);
                        if (this.OnlyData != null)
                        {
                            writer.Write(this.OnlyData);
                        }
                        if (this.TP != null)
                        {
                            writer.Write(this.TP);
                        }
                        result = mem.ToArray();
                        writer.Close();
                    }
                });
            return result;
        }

        public void BuildPackageFromBytes(byte[] data)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler,()=> {
                this.afterPackage = null;
                this.curError = PackageErr_CTM.ExceptionError;
            })
                .Log(Share.Instance.WriteLog, "BuildPackageFromBytes", "")
                .Do(() =>
                {
                    AnalySuccess = false;
                    using (MemoryStream mem = new MemoryStream(data))
                    {
                        BinaryReader reader = new BinaryReader(mem);

                        this.SynWord1 = reader.ReadByte();
                        this.DataLength = reader.ReadBytes(LLLen);
                        int len = BitConverter.ToUInt16(this.DataLength, 0);
                        long realLen = reader.BaseStream.Position + AddrLen + SynWordLen + len + CrcLen + EndTagLen;
                        this.afterPackage = null;
                        if (data.Length > realLen)
                        {
                            long afterLen = data.Length - realLen;
                            this.afterPackage = new byte[afterLen];
                            Buffer.BlockCopy(data, (int)realLen, this.afterPackage, 0, (int)afterLen);
                        }
                        else if (data.Length < realLen)
                        {
                            this.curError = PackageErr_CTM.PackageLengthError;
                            return;
                        }

                        this.DevAddr = reader.ReadBytes(AddrLen);
                        this.SynWord2 = reader.ReadByte();
                        this.CmdWord = reader.ReadBytes(CmdLen);
                        this.FrameNum = reader.ReadBytes(FrameNumLen);
                        this.SEQ = reader.ReadByte();
                        this.FN = reader.ReadByte();
                        int dataLen = CmdLen + FrameNumLen + SEQLen + FNLen;
                        int isTP = ByteHelper.GetBit(this.SEQ, 7);
                        if (isTP == 1) dataLen += TPLen;
                        if (len > dataLen)
                        {
                            this.OnlyData = reader.ReadBytes(len - dataLen);
                        }
                        else
                        {
                            this.OnlyData = null;
                        }
                        if (isTP == 1)
                        {
                            this.TP = reader.ReadBytes(TPLen);
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
                this.curError = PackageErr_CTM.SynWordError;
                flag = false;
            }

            if (this.SynWord1 != this.SynWord2)
            {
                this.curError = PackageErr_CTM.SynWordMatchError;
                flag = false;
            }

            if (BitConverter.ToUInt16(this.DataLength, 0) < CmdLen)
            {
                this.curError = PackageErr_CTM.DataLengthError;
                flag = false;
            }

            if (this.EndTag != EndMark)
            {
                this.curError = PackageErr_CTM.EndTagError;
                flag = false;
            }

            if (!ByteHelper.ByteArryEquals(this.CRC, ByteHelper.GetCrc16_Bytes(this.AllData)))
            {
                this.curError = PackageErr_CTM.CRCError;
                flag = false;
            }

            //if (this.CRC != GetCRC(this.AllData))
            //{
            //    this.curError = PackageErr_CTM.CRCError;
            //    flag = false;
            //}

            return flag;
        }

        public byte[] ToBytes()
        {
            byte[] result = new byte[0];
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Do(() =>
                {
                    using (MemoryStream mem = new MemoryStream())
                    {
                        BinaryWriter writer = new BinaryWriter(mem);
                        writer.Write(this.SynWord1);
                        writer.Write(this.DataLength);
                        writer.Write(this.DevAddr);
                        writer.Write(this.SynWord2);
                        writer.Write(this.CmdWord);
                        writer.Write(this.FrameNum);
                        writer.Write(this.SEQ);
                        writer.Write(this.FN);
                        if (this.OnlyData != null && this.OnlyData.Length != 0)
                        {
                            writer.Write(this.OnlyData);
                        }
                        if (this.TP != null && this.TP.Length != 0)
                        {
                            writer.Write(this.TP);
                        }
                        writer.Write(this.CRC);
                        writer.Write(this.EndTag);

                        result = mem.ToArray();
                        writer.Close();
                    }
                });

            return result;
        }

        private static byte[] aucCRCHi = {
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40
        };

        private static byte[] aucCRCLo = {
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7,
            0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E,
            0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9,
            0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC,
            0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
            0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32,
            0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D,
            0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38,
            0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF,
            0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
            0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1,
            0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4,
            0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB,
            0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA,
            0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
            0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,
            0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97,
            0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E,
            0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89,
            0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83,
            0x41, 0x81, 0x80, 0x40
        };

        private byte[] GetCRC(byte[] data)
        {
            byte[] result = new byte[2];
            byte crcHi = 0xFF;
            byte crcLo = 0xFF;
            int index = 0;

            foreach (byte b in data)
            {
                index = crcLo ^ b;
                crcLo = (byte)(crcHi ^ aucCRCHi[index]);
                crcHi = aucCRCLo[index];
            }

            result[0] = crcLo;
            result[1] = crcHi;

            return result;

            //foreach (byte b in data)
            //{
            //    result += b;
            //}

            //return result;
        }
    }
}
