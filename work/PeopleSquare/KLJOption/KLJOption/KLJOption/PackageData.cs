
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace KLJOption
{
    public static class Mark
    {
        public const byte HeadMark = 0x86;

        public const byte EndMark = 0x16;

        public const byte SubHeadMark=0x02;
    }

    public class PackageData
    {
        public byte Head { get; set; }

        public byte []Len { get; set; }

        public byte[] Addr { get; set; }

        public byte SyncWord { get; set; }

        public byte Seq { get; set; }

        public byte USart { get; set; }

        public byte[] OnlyData { get; set; }

        public byte[] AllData { get; set; }

        public byte[] Crc16 { get; set; }

        public byte Tail { get; set; }

        public bool AnalySuccess { get; set; }

        public PackageData()
        {

        }

        public PackageData(byte[] addr,byte seq,byte uStart,byte[] data)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    if(addr.Length!=8)
                    {
                        Utility.WriteLog("地址长度错误！", 4);
                        return;
                    }
                    this.Head = Mark.HeadMark;
                    this.Len = BitConverter.GetBytes((short)(data.Length + 2));
                    this.Addr = new byte[addr.Length];
                    Buffer.BlockCopy(addr, 0, this.Addr, 0, addr.Length);
                    this.Seq = seq;
                    this.SyncWord = Mark.HeadMark;
                    this.USart = uStart;
                    this.OnlyData = new byte[data.Length];
                    Buffer.BlockCopy(data, 0, this.OnlyData, 0, data.Length);
                    this.AllData = new byte[2 + 8 + 1 + 1 + 1 + this.OnlyData.Length];
                    this.AllData = GetAllData();
                    this.Crc16 = ByteHelper.GetCrc16_Bytes(this.AllData);
                    this.Tail = Mark.EndMark;
                });
        }

        public byte[] GetAllData()
        {
            return AspectF.Define.Retry(Utility.CatchExpection)
                .Return<byte[]>(() =>
                {
                    byte[] result;
                    using (MemoryStream mem = new MemoryStream())
                    {
                        BinaryWriter bw = new BinaryWriter(mem);
                        bw.Write(this.Len);
                        bw.Write(this.Addr);
                        bw.Write(this.SyncWord);
                        bw.Write(this.Seq);
                        bw.Write(this.USart);
                        if (this.OnlyData != null && this.OnlyData.Length > 0)
                        {
                            bw.Write(this.OnlyData);
                        }
                        result = mem.ToArray();
                        bw.Close();
                    }
                    return result;
                });
        }

        public byte[] BuildPackgeFromBytes(byte[] data)
        {
            return AspectF.Define.Retry(Utility.CatchExpection)
                .Return<byte[]>(() =>
                {
                    byte[] result=new byte[0];
                    PackageErrEnum err = PackageErrEnum.Normal;
                    using (MemoryStream mem = new MemoryStream(data))
                    {
                        BinaryReader br = new BinaryReader(mem);
                        this.Head = br.ReadByte();
                        if(this.Head!=Mark.HeadMark)
                        {
                            err = PackageErrEnum.SynWordError;
                        }
                        this.Len = br.ReadBytes(2);
                        this.Addr = br.ReadBytes(8);
                        this.SyncWord = br.ReadByte();
                        if(this.Head != this.SyncWord)
                        {
                            err = PackageErrEnum.SynWordMatchError;
                        }
                        ushort dataLength = BitConverter.ToUInt16(this.Len, 0);
                        int pkgLen = (int)br.BaseStream.Position+1+1+dataLength+1;
                        if(data.Length < pkgLen)
                        {
                            err = PackageErrEnum.PackageLengthError;
                        }
                        else if(data.Length>pkgLen)
                        {
                            int afterInt = data.Length - pkgLen;
                            result = new byte[afterInt];
                            Buffer.BlockCopy(data, pkgLen, result, 0, afterInt);
                        }
                        
                        this.Seq = br.ReadByte();
                        this.USart = br.ReadByte();
                        this.OnlyData = br.ReadBytes(dataLength-2);
                        this.AllData= GetAllData();
                        this.Crc16 = br.ReadBytes(2);
                        if(!ByteHelper.ByteArryEquals(this.Crc16,ByteHelper.GetCrc16_Bytes(this.AllData)))
                        {
                            err = PackageErrEnum.CRCError;
                        }
                        this.Tail = br.ReadByte();
                        if(this.Tail!=Mark.EndMark)
                        {
                            err = PackageErrEnum.EndTagError;
                        }
                        this.AnalySuccess = true;
                        if (err!=PackageErrEnum.Normal)
                        {
                            this.AnalySuccess = false;
                            Utility.WriteLog("解析报文出错："+err, 4);
                        }
                    }

                    return result;
                });
        }

        public byte[] ToBytes()
        {
            return AspectF.Define.Retry(Utility.CatchExpection)
                .Return<byte[]>(() =>
                {
                    byte[] result;
                    using (MemoryStream mem = new MemoryStream())
                    {
                        BinaryWriter bw = new BinaryWriter(mem);
                        bw.Write(this.Head);
                        bw.Write(this.Len);
                        bw.Write(this.Addr);
                        bw.Write(this.SyncWord);
                        bw.Write(this.Seq);
                        bw.Write(this.USart);
                        if(this.OnlyData!=null && this.OnlyData.Length>0)
                        {
                            bw.Write(this.OnlyData);
                        }
                        bw.Write(this.Crc16);
                        bw.Write(this.Tail);
                        result = mem.ToArray();
                        bw.Close();
                    }

                    return result;
                });
        }
    }

    public class SubPackage
    {
        public byte Head { get; set; }

        public byte Len { get; set; }        

        public byte CmdWord { get; set; }

        public byte[] OnlyData { get; set; }

        public byte []AllData { get; set; }

        public byte []Crc { get; set; }

        public bool AnalySuccess { get; set; }

        public SubPackage()
        {

        }

        public SubPackage(byte cmdWord, byte[] data)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    this.Head = Mark.SubHeadMark;
                    this.Len =(byte)(1+data.Length + 2);
                    this.CmdWord = cmdWord;
                    this.OnlyData = new byte[data.Length];
                    Buffer.BlockCopy(data, 0, this.OnlyData, 0, data.Length);
                    this.AllData = GetAllData();                   
                    this.Crc = GetCRC(this.AllData);
                });
        }

        public byte[] GetAllData()
        {
            return AspectF.Define.Retry(Utility.CatchExpection)
                .Return<byte[]>(() =>
                {
                    byte[] result;
                    using (MemoryStream mem = new MemoryStream())
                    {
                        BinaryWriter bw = new BinaryWriter(mem);
                        bw.Write(this.Len);
                        bw.Write(this.CmdWord);
                        if (this.OnlyData != null && this.OnlyData.Length > 0)
                        {
                            bw.Write(this.OnlyData);
                        }
                        result = mem.ToArray();
                        bw.Close();
                    }
                    return result;
                });
        }

        public void BuildPackgeFromBytes(byte[] data)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    PackageErrEnum err = PackageErrEnum.Normal;
                    using (MemoryStream mem = new MemoryStream(data))
                    {
                        BinaryReader br = new BinaryReader(mem);
                        this.Head = br.ReadByte();
                        if (this.Head != Mark.SubHeadMark)
                        {
                            err = PackageErrEnum.SynWordError;
                        }
                        this.Len = br.ReadByte();
                        int pkgLen = (int)br.BaseStream.Position + 1+1;
                        if (data.Length < pkgLen)
                        {
                            err = PackageErrEnum.PackageLengthError;
                        }
                        this.CmdWord = br.ReadByte();
                                 
                        this.OnlyData = br.ReadBytes(this.Len-1-2);
                        this.AllData = GetAllData();
                        this.Crc = br.ReadBytes(2);
                        if (!ByteHelper.ByteArryEquals(this.Crc, GetCRC(this.AllData)))
                        {
                            err = PackageErrEnum.CRCError;
                        }
                        
                        this.AnalySuccess = true;
                        if (err != PackageErrEnum.Normal)
                        {
                            this.AnalySuccess = false;
                            Utility.WriteLog("子解析报文出错：" + err, 4);
                        }
                    }
                    
                });
        }

        public byte[] ToBytes()
        {
            return AspectF.Define.Retry(Utility.CatchExpection)
                .Return<byte[]>(() =>
                {
                    byte[] result;
                    using (MemoryStream mem = new MemoryStream())
                    {
                        BinaryWriter bw = new BinaryWriter(mem);
                        bw.Write(this.Head);
                        bw.Write(this.Len);
                        bw.Write(this.CmdWord);
                        if (this.OnlyData != null && this.OnlyData.Length > 0)
                        {
                            bw.Write(this.OnlyData);
                        }
                        bw.Write(this.Crc);
                        result = mem.ToArray();
                        bw.Close();
                    }

                    return result;
                });
        }

        private byte[] GetCRC(byte[] data)
        {
            byte []result = new byte[2];

            ushort crc = 0;
            foreach (byte b in data)
            {
                crc += b;
            }
            result = BitConverter.GetBytes(crc);

            return result;
        }
    }
}
