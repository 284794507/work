using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr
{
    public class LH_PackageData //: IPackageData
    {
        public byte SynWord1 { get; set; }

        public byte[] DataLength { get; set; }

        public byte[] CtuAddr { get; set; }

        public byte SynWord2 { get; set; }

        public byte[] CmdWord { get; set; }

        public byte[] OnlyData { get; set; }

        /// <summary>
        /// 包括验证码的所以信息
        /// </summary>
        public byte[] AllData { get; set; }

        public byte CRC { get; set; }

        public byte EndTag { get; set; }

        public byte[] afterPackage { get; set; }

        public PackageErr_LH curError { get; set; }

        public bool AnalySuccess { get; set; }

        private readonly byte StartMark = 0x68;
        private readonly byte EndMark = 0x16;

        public  LH_PackageData()
        {
            this.SynWord1 = StartMark;
            this.SynWord2 = StartMark;
            this.EndTag = EndMark;

            afterPackage = null;
            curError = PackageErr_LH.Normal;
            AnalySuccess = true;
        }

        public LH_PackageData(byte[] address,byte[] data,byte[] cmd) : this()
        {
            int len = 0;
            if(data !=null)
            {
                len = data.Length;
            }
            this.DataLength =BitConverter.GetBytes((short)(2 + len));
            this.CtuAddr = address;
            this.CmdWord = cmd;
            this.OnlyData = data;
            this.AllData = new byte[2 + 2 + 1 + 2 + len];
            this.AllData = GetAllData();
            this.CRC = GetCRC(this.AllData);
        }

        private byte[] GetAllData()
        {
            byte[] result = null;
            using (MemoryStream mem = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(mem);
                writer.Write(this.DataLength);
                writer.Write(this.CtuAddr);
                writer.Write(this.SynWord2);
                writer.Write(this.CmdWord);
                if(this.OnlyData!=null) writer.Write(this.OnlyData);
                result = mem.ToArray();
                writer.Close();
            }
            return result;
        }

        private byte GetCRC(byte []data)
        {
            byte result=0;

            uint a = 0;
            foreach(byte b in data)
            {
                a += b;
            }
            result = (byte)a;
            return result;
        }

        public void BuildPackageFromBytes(byte[] data)
        {
            AnalySuccess = false;

            try
            {
                using (MemoryStream mem = new MemoryStream(data))
                {
                    BinaryReader reader = new BinaryReader(mem);

                    this.SynWord1 = reader.ReadByte();
                    this.DataLength = reader.ReadBytes(2);
                    int len = BitConverter.ToInt16(this.DataLength, 0);
                    long realLen = reader.BaseStream.Position + 2 + 1 + len + 1 + 1;
                    if(data.Length>realLen)
                    {
                        long afterLen = data.Length - realLen;
                        this.afterPackage = new byte[afterLen];
                        Buffer.BlockCopy(data, (int)realLen, this.afterPackage, 0, (int)afterLen);
                    }
                    else if(data.Length<realLen)
                    {
                        this.curError = PackageErr_LH.PackageLengthError;
                        return;
                    }

                    this.CtuAddr = reader.ReadBytes(2);
                    this.SynWord2 = reader.ReadByte();
                    this.CmdWord = reader.ReadBytes(2);
                    if(len > 2)
                    {
                        this.OnlyData = reader.ReadBytes(len - 2);
                    }
                    else
                    {
                        this.OnlyData = null;
                    }
                    this.AllData = GetAllData();
                    this.CRC = reader.ReadByte();
                    this.EndTag = reader.ReadByte();
                    reader.Close();

                    this.AnalySuccess = CheckPackageIsNotError();
                }
            }
            catch
            {
                this.curError = PackageErr_LH.ExceptionError;                
            }
        }

        private bool CheckPackageIsNotError()
        {
            bool flag = true;

            if(this.SynWord1!=StartMark)
            {
                this.curError = PackageErr_LH.SynWordError;
                flag = false;
            }

            if(this.SynWord1!=this.SynWord2)
            {
                this.curError = PackageErr_LH.SynWordMatchError;
                flag = false;
            }

            if(BitConverter.ToInt16(this.DataLength, 0)<2)
            {
                this.curError = PackageErr_LH.DataLengthError;
                flag = false;
            }

            if(this.EndTag!=EndMark)
            {
                this.curError = PackageErr_LH.EndTagError;
                flag = false;
            }
            
            if(this.CRC!=GetCRC(this.AllData))
            {
                this.curError = PackageErr_LH.CRCError;
                flag = false;
            }

            return flag;
        }

        public byte[] ToBytes()
        {
            byte[] result=new byte[0];

            try
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(mem);
                    writer.Write(SynWord1);
                    writer.Write(DataLength);
                    writer.Write(CtuAddr);
                    writer.Write(SynWord2);
                    writer.Write(CmdWord);
                    if(OnlyData!=null && OnlyData.Length!=0)
                    {
                        writer.Write(OnlyData);
                    }
                    writer.Write(CRC);
                    writer.Write(EndTag);

                    result = mem.ToArray();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}
