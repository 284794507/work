using CommunicateCore.Model;
using CommunicateCore.Terminal;
using CommunicateCore.Terminal.TerminalBusiness;
using CommunicateCore.Utility;
using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicateCore.RTUSvr.RTUSvrBusiness
{
    public class UpgradeFromRTUSvrHandler
    {
        private static UpgradeFromRTUSvrHandler _UpgradeFromRTUSvrHandler;
        public static UpgradeFromRTUSvrHandler GetHandler
        {
            get
            {
                if(_UpgradeFromRTUSvrHandler==null)
                {
                    _UpgradeFromRTUSvrHandler = new UpgradeFromRTUSvrHandler();
                }
                return _UpgradeFromRTUSvrHandler;
            }
        }

        private BrokerMessage BMsg;

        private static byte Stage;

        private static DateTime  sendTime = DateTime.MinValue;

        private readonly int Size = 512;

        private int LampNo;

        private int ResendNo = 0;

        public void SendUpgradeFile()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    //string jsonStr = ((Newtonsoft.Json.Linq.JToken)BMsg.MsgBody).Root.ToString();
                    //UpgradeInfo ReceviceInfo = JsonSerializeHelper.GetHelper.Deserialize<UpgradeInfo>(jsonStr);
                    UpgradeInfo ReceviceInfo = BMsg.MsgBody as UpgradeInfo;
                    UpgradeInfo curInfo = new UpgradeInfo();
                    curInfo.UpgradeStatus = Stage;
                    sendTime = DateTime.Now;
                    bool isSend = false;
                    ResendNo++;
                    if (ResendNo == 5)//重试5次，仍然失败，则结束本次升级
                    {
                        return;
                    }
                    switch (Stage)
                    {
                        case 1:
                            sendTime = DateTime.MinValue;
                            curInfo.FileName = ReceviceInfo.FileName;
                            curInfo.FileLength = ReceviceInfo.FileLength;
                            curInfo.FileCRC = ReceviceInfo.FileCRC;
                            Stage++;
                            isSend = true;
                            MonitorDevUpgrade();
                            break;
                        case 2:
                            if (ReceviceInfo.TotalNum>= ReceviceInfo.SendNo)
                            {
                                int curSize = Size;
                                if (ReceviceInfo.TotalNum == ReceviceInfo.SendNo)
                                {
                                    curSize = ReceviceInfo.FileData.Length - (ReceviceInfo.SendNo - 1) * Size;
                                }
                                byte[] len = BitConverter.GetBytes((short)curSize);
                                curInfo.FileLength = new byte[2];
                                curInfo.FileLength[0] = len[1];
                                curInfo.FileLength[1] = len[0];
                                curInfo.TotalNum = ReceviceInfo.TotalNum;
                                curInfo.SendNo = ReceviceInfo.SendNo;
                                //Buffer.BlockCopy(len, 0, curInfo.FileLength, 0, 3);
                                curInfo.FileData = new byte[curSize];
                                Buffer.BlockCopy(ReceviceInfo.FileData, (ReceviceInfo.SendNo - 1) * Size, curInfo.FileData, 0, curSize);
                            }
                            isSend = true;
                            break;
                        case 3:
                            isSend = true;
                            break;
                    }
                    if(isSend)
                    {
                        byte[] ctuAddr = TerminalShare.GetShare.GetCtuAddrByPtuAddr(BMsg.TerminalAddress);
                        string addrStr = ByteHelper.bytesToCtuAddr(ctuAddr, true); //BitConverter.ToInt16(ctuAddr, 0).ToString();
                        UpgradeHandler.GetHandler.SendUpgradeInfo(addrStr, LampNo, curInfo);
                    }
                });
        }

        public void HandlerUpgradePackage(LH_PackageData package)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte[] lampNo = new byte[2];
                    int curIndex = 0;
                    byte[] addr = new byte[2];
                    Buffer.BlockCopy(package.OnlyData, curIndex, addr, 0, 2);
                    curIndex += 2;
                    Buffer.BlockCopy(package.OnlyData, curIndex, lampNo, 0, 2);
                    curIndex += 2;

                    int no = BitConverter.ToInt16(lampNo, 0);
                    LampNo = no;
                    string ctuAddr = ByteHelper.bytesToCtuAddr(addr, true);
                    vLampInfo lampInfo = TerminalInitBusiness.GetInit.GetLampInfoByLampNo(ctuAddr, no);
                    if (!string.IsNullOrEmpty(lampInfo.PtuID))
                    {
                        if (TerminalShare.GetShare.checkDevIsLoginOrNot(lampInfo.PtuID.Trim()))
                        {
                            byte[] address = ByteHelper.HexStrToByteArrayWithDelimiter(lampInfo.PtuID.Trim(), " ");

                            int len = package.OnlyData[curIndex++];
                            int dataLen = package.OnlyData.Length -2- 2 -1- len;//CTU地址，灯号，文件名称长度，文件名称
                            
                            UpgradeInfo curInfo = new UpgradeInfo();
                            curInfo.TotalNum =(byte)( dataLen / Size);
                            if (dataLen % Size > 0) curInfo.TotalNum++;
                            curInfo.SendNo = 0;
                            curInfo.FileName = new byte[len];
                            Buffer.BlockCopy(package.OnlyData, curIndex, curInfo.FileName, 0, len);
                            curIndex += len;
                            curInfo.FileData = new byte[dataLen];
                            Buffer.BlockCopy(package.OnlyData, curIndex, curInfo.FileData, 0, dataLen);
                            byte[] bLen = new byte[4];
                            bLen = BitConverter.GetBytes(dataLen);
                            curInfo.FileLength = new byte[3];
                            //Buffer.BlockCopy(bLen, 0, curInfo.FileLength, 0, 3);
                            curInfo.FileLength[0] = bLen[2];
                            curInfo.FileLength[1] = bLen[1];
                            curInfo.FileLength[2] = bLen[0];
                            byte[] crc = BitConverter.GetBytes(ByteHelper.GetCrc16(curInfo.FileData));
                            curInfo.FileCRC = new byte[2];
                            //Buffer.BlockCopy(crc, 0, curInfo.FileCRC, 0, 2);
                            curInfo.FileCRC[0] = crc[1];
                            curInfo.FileCRC[1] = crc[0];

                            BMsg = new BrokerMessage(MessageType.upgrade, 0, address, curInfo);
                            Stage = 1;
                            SendUpgradeFile();
                        }
                    }
                    
                });
        }

        public void UpgradeSuccess(BrokerMessage backMsg,int lampNo)
        {
            AspectF.Define.Retry()
                .Log(RTUSvrShare.GetShare.WriterLog,"",ResendNo==5?"已经重试5次，发送失败，中断本次升级！":"")
                .Do(() =>
                {
                    if (backMsg.MsgBody.ToString() == "1")
                    {
                        UpgradeInfo ReceviceInfo = BMsg.MsgBody as UpgradeInfo;
                        ReceviceInfo.SendNo++;
                        ResendNo = 0;
                        if (ReceviceInfo.TotalNum < ReceviceInfo.SendNo)
                        {
                            Stage++;
                        }
                    }
                    else
                    {
                        //ResendNo++;
                        //if (ResendNo == 5)//重试5次，仍然失败，则结束本次升级
                        //{
                        //    return;
                        //}
                    }
                    Thread.Sleep(1000);
                    SendUpgradeFile(); 
                });
        }


        /// <summary>
        /// 监测升级是否超时
        /// </summary>
        private void MonitorDevUpgrade()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            if (sendTime != DateTime.MinValue)
                            {
                                TimeSpan ts = DateTime.Now - sendTime;
                                if (ts.TotalMilliseconds >= 1000 * 5 && ResendNo<5 && Stage!=3)
                                {
                                    Console.WriteLine("发送超时！");
                                    Thread.Sleep(1000);
                                    SendUpgradeFile();
                                }
                            }
                        }
                    }
                    );
            }
            catch (Exception ex)
            {
                MonitorDevUpgrade();
            }
        }
    }
}
