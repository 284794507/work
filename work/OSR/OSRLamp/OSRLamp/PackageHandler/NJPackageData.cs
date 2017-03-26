using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSRLamp.PackageHandler
{
    public class NJPackageData
    {
        public byte StartMark { get; set; }

        //CmdWord + BodyProperty + TerminalID + SerialNumber + PackageInfo + Tp + OnlyData
        public byte[] Datalen { get; set; }

        public byte SyncWord2 { get; set; }

        public byte[] CmdWord { get; set; }

        public byte[] BodyProperty { get; set; }

        public byte[] TerminalID { get; set; }

        public byte[] SerialNumber { get; set; }

        public byte[] PackageInfo { get; set; }

        public byte[] Tp { get; set; }

        public byte[] OnlyData { get; set; }

        public byte[] Crc16 { get; set; }

        public byte EndTag { get; set; }

        public byte[] AfterData { get; set; }

        public PackageErr CurError { get; set; }

        public bool AnalySuccess { get; set; }

        public NJPackageData()
        {
            this.StartMark = SyncWord.StartTag;
            this.SyncWord2 = SyncWord.StartTag;
            this.EndTag = SyncWord.EndTag;

            this.AfterData = new byte[0];
            this.CurError = PackageErr.Normal;
            this.AnalySuccess = true;
        }

        public NJPackageData(byte[] cmdWord,byte[] bodyProperty, byte[] terminalID,byte[] serialNo,byte[] packageInfo,byte[] tp,byte[] data):this()
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {

                    //CmdWord + BodyProperty + TerminalID + SerialNumber + PackageInfo + Tp + OnlyData
                    int len = cmdWord.Length + bodyProperty.Length + terminalID.Length + serialNo.Length + packageInfo.Length + tp.Length + data.Length;
                    this.Datalen = BitConverter.GetBytes((short)len);
                    this.CmdWord = new byte[2];
                    this.CmdWord[0] = cmdWord[1];
                    this.CmdWord[1] = cmdWord[0];
                    //Array.Reverse(this.CmdWord);
                    this.BodyProperty = bodyProperty;
                    this.TerminalID = terminalID;
                    this.SerialNumber = serialNo;
                    this.PackageInfo = packageInfo;
                    this.Tp = tp;
                    this.OnlyData = data;
                    this.Crc16 = GetCrc16();
                });
        }

        //new NJPackageData(CmdWord.Lamp_RealCtrl, new byte[] { 0x00, 0x00 }, terminalID, Utility.GetSerialNo(), new byte[] { 0x01, 0x00, 0x01, 0x00 }, new byte[0], data);
        public NJPackageData(byte[] cmdWord, byte[] terminalID,byte[] data) : this(cmdWord, new byte[] { 0x00, 0x00 }, terminalID, Utility.GetSerialNo(), new byte[0] , new byte[0], data)//{ 0x01, 0x00, 0x01, 0x00 }
        {

        }

        public void BuildPackageFromBytes(byte[] para)
        {
            AspectF.Define.Retry(Utility.CatchExpection, () =>
            {
                this.AfterData = new byte[0];
                this.CurError = PackageErr.ExceptionError;
                this.AnalySuccess = false;
            })
            .Log(Utility.WriteLog, "", "")
            .Do(() =>
            {
                this.AnalySuccess = false;
                using (MemoryStream mem = new MemoryStream(para))
                {
                    BinaryReader br = new BinaryReader(mem);

                    this.StartMark = br.ReadByte();
                    this.Datalen = br.ReadBytes(2);
                    int len = BitConverter.ToUInt16(this.Datalen, 0);
                    int realLen = 1 + 2 + 1 + len + 2 + 1;
                    int paraLen = para.Length;
                    if(paraLen>realLen)
                    {
                        int afterLen = paraLen - realLen;
                        this.AfterData = new byte[afterLen];
                        Buffer.BlockCopy(para, realLen, this.AfterData, 0, afterLen);
                    }
                    else if(paraLen<realLen)
                    {
                        this.CurError = PackageErr.PackageLengthError;
                        br.Close();
                        return;
                    }

                    this.SyncWord2 = br.ReadByte();
                    this.CmdWord = br.ReadBytes(2);
                    this.BodyProperty = br.ReadBytes(2);
                    this.TerminalID = br.ReadBytes(6);
                    this.SerialNumber = br.ReadBytes(2);
                    int otherLen = 2 + 2 + 6 + 2; 
                    int isLongPkg = ByteHelper.GetBit(this.BodyProperty[1], 6);
                    if(isLongPkg==1)
                    {
                        this.PackageInfo = br.ReadBytes(4);
                        otherLen += 4;
                    }
                    int isTp = ByteHelper.GetBit(this.BodyProperty[1], 5);
                    if(isTp==1)
                    {
                        this.Tp = br.ReadBytes(6);
                        otherLen += 6;
                    }
                    this.OnlyData = br.ReadBytes(len - otherLen);
                    this.Crc16 = br.ReadBytes(2);
                    this.EndTag = br.ReadByte();

                    br.Close();

                    this.AnalySuccess = CheckPackageIsNotError();
                }
            });
        }

        private bool CheckPackageIsNotError()
        {
            bool flag = true;

            if (this.StartMark != SyncWord.StartTag)
            {
                this.CurError = PackageErr.SynWordError;
                flag = false;
            }

            if (this.StartMark != this.SyncWord2)
            {
                this.CurError = PackageErr.SynWordMatchError;
                flag = false;
            }
            
            if (this.EndTag != SyncWord.EndTag)
            {
                this.CurError = PackageErr.EndTagError;
                flag = false;
            }

            if (!ByteHelper.ByteArryEquals(this.Crc16, GetCrc16()))
            {
                this.CurError = PackageErr.CRCError;
                flag = false;
            }

            return flag;
        }

        public byte[] ToBytes()
        {
            return AspectF.Define.Retry(Utility.CatchExpection)
                .Return<byte[]>(() =>
                {
                    byte[] result = new byte[0];
                    using (MemoryStream mem = new MemoryStream())
                    {//CmdWord + BodyProperty + TerminalID + SerialNumber + PackageInfo + Tp + OnlyData
                        BinaryWriter writer = new BinaryWriter(mem);
                        writer.Write(this.StartMark);
                        writer.Write(this.Datalen);
                        writer.Write(this.SyncWord2);
                        writer.Write(this.CmdWord);
                        writer.Write(this.BodyProperty);
                        writer.Write(this.TerminalID);
                        writer.Write(this.SerialNumber);
                        //writer.Write(this.PackageInfo);
                        if (this.PackageInfo != null)
                        {
                            writer.Write(this.PackageInfo);
                        }
                        if (this.Tp != null)
                        {
                            writer.Write(this.Tp);
                        }
                        if (this.OnlyData != null)
                        {
                            writer.Write(this.OnlyData);
                        }
                        writer.Write(this.Crc16);
                        writer.Write(this.EndTag);
                        result = mem.ToArray();
                        writer.Close();
                    }
                    return result;
                });
        }

        private byte[] GetCrc16()
        {
            return AspectF.Define.Retry(Utility.CatchExpection)
                .Return< byte[]>(() =>
                {
                    byte[] result = new byte[2];
                    using (MemoryStream mem = new MemoryStream())
                    {//CmdWord + BodyProperty + TerminalID + SerialNumber + PackageInfo + Tp + OnlyData
                        BinaryWriter writer = new BinaryWriter(mem);
                        writer.Write(this.CmdWord);
                        writer.Write(this.BodyProperty);
                        writer.Write(this.TerminalID);
                        writer.Write(this.SerialNumber);
                        //writer.Write(this.PackageInfo);
                        if (this.PackageInfo != null)
                        {
                            writer.Write(this.PackageInfo);
                        }
                        if (this.Tp != null)
                        {
                            writer.Write(this.Tp);
                        }
                        if (this.OnlyData != null)
                        {
                            writer.Write(this.OnlyData);
                        }
                        result =ByteHelper.GetCrc16_Bytes(mem.ToArray());
                        writer.Close();
                    }
                    return result;
                });
        }
    }
}
