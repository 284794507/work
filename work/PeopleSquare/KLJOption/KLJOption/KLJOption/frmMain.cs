using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions;           //正则校验
using System.Net.Sockets;
using Utility;
using System.Threading;

namespace KLJOption
{
    public partial class frmMain : Form
    {
        TcpClient mainClient;
        private static int ManTcpNum;
        private byte[] ReceviceBuffer;
        private byte[] MacAddr;
        private Dictionary<string, DateTime> dictMacAddr = new Dictionary<string, DateTime>();

        public frmMain()
        {
            InitializeComponent();
            Utility.InitConfig();
            MacAddr = new byte []{ 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            ConnectTcp();
            MonitorDevClient();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
/*
1.this.Close();   只是关闭当前窗口，若不是主窗体的话，是无法退出程序的，另外若有托管线程（非主线程），也无法干净地退出；
2.Application.Exit();  强制所有消息中止，退出所有的窗体，但是若有托管线程（非主线程），也无法干净地退出；
3.Application.ExitThread(); 强制中止调用线程上的所有消息，同样面临其它线程无法正确退出的问题；
4.System.Environment.Exit(0);   这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。
*/
            System.Environment.Exit(0);
        }

        private void ConnectTcp()
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    mainClient = new TcpClient();
                    
                    ReceviceBuffer = new byte[mainClient.ReceiveBufferSize];
                    mainClient.Connect(Utility.RemotePoint);
                    Utility.WriteLog("接入服务器连接成功！");
                    mainClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, ReceviceData, ReceviceBuffer);
                });
        }


        private void MonitorDevClient()
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        while (true)
                        {
                            Thread.Sleep(5000);
                            if (!mainClient.Connected || !mainClient.Client.Connected)
                            {
                                ConnectTcp();
                            }
                        }
                    }
                    );
                });
        }


        private void ReceviceData(IAsyncResult ar)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    if (mainClient.Connected)
                    {
                        NetworkStream NetStream = mainClient.GetStream();
                        int len = 0;
                        try
                        {
                            len = NetStream.EndRead(ar);
                        }
                        catch
                        {
                            len = 0;
                        }
                        if (len == 0) return;

                        byte[] buffer = (byte[])ar.AsyncState;
                        byte[] receviceBytes = new byte[len];
                        Buffer.BlockCopy(buffer, 0, receviceBytes, 0, len);
                        Utility.WriteLog("接收数据包成功：" + ByteHelper.ByteToHexStrWithDelimiter(receviceBytes, " ", false));

                        AnalyzeProtocol(receviceBytes);

                        mainClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, ReceviceData, ReceviceBuffer);
                    }
                });
        }
        
        public void AnalyzeProtocol(byte[] data)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    byte []afterPkg = new byte[0];
                    do
                    {
                        PackageData curPkg = new PackageData();
                        if(afterPkg==null || afterPkg.Length==0)
                        {
                            afterPkg = curPkg.BuildPackgeFromBytes(data);
                        }
                        else
                        {
                            afterPkg = curPkg.BuildPackgeFromBytes(afterPkg);
                        }
                        if(curPkg.AnalySuccess)
                        {
                            AnalyzeSubPackage(curPkg.OnlyData,curPkg);
                        }
                    }
                    while (afterPkg != null && afterPkg.Length > 0);

                });
        }

        private void AnalyzeSubPackage(byte[] data, PackageData outPkg)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {

                    SubPackage subPkg = new SubPackage();
                    subPkg.BuildPackgeFromBytes(data);
                    if (subPkg.AnalySuccess)
                    {
                        PackageData sendPkg = new PackageData();
                        SubPackage sendSubPkg = new SubPackage();
                        byte[] sendData = new byte[0];
                        byte[] subData = new byte[0];
                        if (subPkg.CmdWord == PackageCmdWord.Login)
                        {
                            //登录回复
                            sendSubPkg = new SubPackage(PackageCmdWord.LoginBack, new byte[0]);
                            subData = sendSubPkg.ToBytes();

                            sendPkg = new PackageData(outPkg.Addr, outPkg.Seq, outPkg.USart, subData);
                            sendData = sendPkg.ToBytes();
                            SendDataToDev(sendData);
                            //Thread.Sleep(1000);
                            //查询MAC ADDR
                            sendSubPkg = new SubPackage(PackageCmdWord.QueryMacAddr, new byte[0]);
                            subData = sendSubPkg.ToBytes();

                            sendPkg =new PackageData(outPkg.Addr, outPkg.Seq, (byte)USART.uMcu, subData);
                            sendData = sendPkg.ToBytes();
                            SendDataToDev(sendData);
                        }
                        else if(subPkg.CmdWord == PackageCmdWord.QueryMacAddrBack)
                        {                            
                            //Buffer.BlockCopy(subPkg.OnlyData, 0, MacAddr, 0, 8);
                            string mac= ByteHelper.ByteToHexStrWithDelimiter(subPkg.OnlyData, " ", false);
                            ShowItem(mac);
                        }
                        else
                        {
                            string mac = ByteHelper.ByteToHexStrWithDelimiter(outPkg.Addr, " ", false);
                            ShowItem(mac);
                        }
                    }

                });
        }

        private delegate void AddItem(string addr);//代理

        private void ShowItem(string addr)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    if (this.InvokeRequired)
                    {
                        AddItem setpos = new AddItem(ShowItem);
                        this.Invoke(setpos, addr);
                    }
                    else
                    {
                        if (!dictMacAddr.ContainsKey(addr))
                        {
                            dictMacAddr.Add(addr, DateTime.Now);
                            ListViewItem item = new ListViewItem();
                            item.Text = addr;
                            //item.SubItems[1].Text = DateTime.Now.ToString();
                            item.SubItems.Add(DateTime.Now.ToString());
                            lvwDev.Items.Add(item);
                        }
                        else
                        {
                            foreach (ListViewItem val in lvwDev.Items)
                            {
                                if (val.Text == addr)
                                {
                                    //lvwDev.Items.Remove(val);
                                    val.SubItems[1].Text = DateTime.Now.ToString();
                                }
                            }
                        }
                        //ListViewItem item = new ListViewItem();
                        //item.Text = addr;
                        //item.SubItems.Add(DateTime.Now.ToString());
                        //lvwDev.Items.Add(item);
                    }
                });
        }

        private void SendDataToDev(byte[] data)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    mainClient.GetStream().BeginWrite(data, 0, data.Length, EndWrite, data);
                    mainClient.GetStream().Flush();
                });
        }

        private void EndWrite(IAsyncResult ar)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    byte[] buffer = (byte[])ar.AsyncState;
                    mainClient.GetStream().EndWrite(ar);
                    Utility.WriteLog("发送数据包成功：" + ByteHelper.ByteToHexStrWithDelimiter(buffer, " ", false));
                });
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cboModuleNo.Text = "1";
            cboReportCycle.Text = "1";
            cboReportMode.Text = "1";
            cboSecretWay.Text = "WPA/WPA2";
            cboWifiChannel.Text = "1";
        }

        private void btnApplyAP_Click(object sender, EventArgs e)
        {
            if (txtAPName.Text == "")
            {
                MessageBox.Show("没有AP名称！"); 
                return;
            }
            if (txtPassword.Text == "" )
            {
                MessageBox.Show("没有密码！");
                return;
            }
            if (txtPassword.Text.Length<6)
            {
                MessageBox.Show("密码长度小于11个字符，不符合规定");
                return;
            }

            //if (txtDevMAC.Text.Length!=16)
            //{
            //    MessageBox.Show("MAC长度不对，应该为8个字节");
            //    return;
            //}
            //else
            //{
            //    if (!Regex.IsMatch(txtDevMAC.Text, "^[0-9A-Fa-f]+$"))
            //    {
            //        MessageBox.Show("MAC格式不对!");
            //        return;
            //    }
            //}

            byte []subData = GetSubApData();
            PackageData sendData = new PackageData(MacAddr, 0, 0, subData);
            SendDataToDev(sendData.ToBytes());
            return;

            byte[] KernalBuffer = new byte[100];
            int KernalLength=MakeAPOption(KernalBuffer);
            byte[] SendFrameBuffer = new byte[200];
            int SendFrameLength = 0;
            int iCount = 0;
            int iMaccount = 0;
            int iPos = 0;
            UInt16 iCRC = 0;

            SendFrameBuffer[0] = 0x86;
            byte[] mac=GetBytes(txtDevMAC.Text,out iMaccount);
            for (iCount=0;iCount< iMaccount;iCount++)
            {
                SendFrameBuffer[3 + iCount] = mac[iCount];
            }
            SendFrameBuffer[3 + iCount++] = 0x86;
            SendFrameBuffer[3 + iCount++] = 0;          //包序号
            SendFrameBuffer[3 + iCount++] = 0;          //模块号
            SendFrameLength = 3 + iCount;
            for (iPos=0;iPos< KernalLength;iPos++)
            {
                SendFrameBuffer[SendFrameLength++] = KernalBuffer[iPos];
            }
            SendFrameBuffer[1] = (byte)((KernalLength + 2)&0xFF);       //长度
            SendFrameBuffer[2] = 0x0;
            //计算CRC


            //填充道信息区，可以供人复制
            //lblmsg

            //发送帧数据
            return;


        }

        public byte[] GetSubApData()
        {
            byte []result = new byte[0];

            byte []data = new byte[41];
            int i = 0,j = 0;
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(txtAPName.Text);
            int Length = bytes.Length;
            byte[] bytes1 = System.Text.Encoding.ASCII.GetBytes(txtPassword.Text);
            int Length1 = bytes1.Length;
            for (i = 0; i < Length; i++)
            {
                data[i] = bytes[i];
            }
            for (; i < 20; i++)
            {
                data[i] = 0xFF;
            }
            if (cboReportMode.Text == "1")
                data[i] = 1;
            else
                data[i] = 0;
            i++;
            for (j = 0; j < Length1; j++)
            {
                data[i + j] = bytes1[j];
            }
            for (; j < 20; j++)
            {
                data[i + j] = 0xFF;
            }

            SubPackage pkg = new SubPackage(PackageCmdWord.SetAp, data);
            result = pkg.ToBytes();

            return result;
        }

        private void lblMsg_Click(object sender, EventArgs e)
        {
            lblMsg.Refresh();
        }

        private int MakeAPOption(byte[] KernalBuffer)
        {  /*配置AP参数*/
           // byte[] KernalBuffer = new byte[200];
            int KernalLength = 0;
            int i,j;
            UInt16 iSum=0;

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(txtAPName.Text);
            int Length = bytes.Length;
            byte[] bytes1 = System.Text.Encoding.ASCII.GetBytes(txtPassword.Text);
            int Length1 = bytes1.Length;

            KernalBuffer[0] = 0x02;
            KernalBuffer[1] = 43; KernalBuffer[2] = 0x00;
            KernalBuffer[3] = 0xE2;
            for (i=0;i<txtAPName.Text.Length;i++)
            {
                KernalBuffer[4 + i] = bytes[i];
            }
            for (; i < 20; i++)
            {
                KernalBuffer[4 + i] =0xFF;
            }
            if (cboReportMode.Text == "1")
                KernalBuffer[4 + i] = 1;
            else
                KernalBuffer[4 + i] = 0;
            i++;
            for ( j= 0; j < txtPassword.Text.Length; j++)
            {
                KernalBuffer[4 + i+j] = bytes1[j];
            }
            for (; j < 20; j++)
            {
                KernalBuffer[4 + i+j] = 0xFF;
            }
            KernalLength = 4 + i + j;
            for (i=1;i< KernalLength;i++)
            {
                iSum += KernalBuffer[i];
            }
            KernalBuffer[i++] = (byte)(iSum & 0xFF);
            KernalBuffer[i++] = (byte)(iSum >>8);
            KernalLength += 2;
            

            return(KernalLength);
        }

        //byte[] 转16进制格式string：new byte[]{ 0x30, 0x31}转成"3031":

        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;

            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }



        //16进制格式string 转byte[]：
        public static byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            string newString = "";
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (Uri.IsHexDigit(c))
                    newString += c;
                else
                    discarded++;
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = newString.Substring(0, newString.Length - 1);
            }
            
            return HexToByte(newString);
        }


        public static byte[] HexToByte(string hexString)
         {
             if (string.IsNullOrEmpty(hexString))
             {
                 hexString = "00";
             }
             byte[] returnBytes = new byte[hexString.Length / 2];
             for (int i = 0; i<returnBytes.Length; i++)
                 returnBytes[i] = Convert.ToByte(hexString.Substring(i*2, 2), 16);
             return returnBytes;
         }

        private void btnApplyModule_Click(object sender, EventArgs e)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    byte usart = byte.Parse(cboModuleNo.Text);
                    byte channel = byte.Parse(cboWifiChannel.Text);
                    byte minute = byte.Parse(cboReportCycle.Text);
                    byte mode = byte.Parse(cboReportMode.Text);
                    byte[] subData = new byte[3];
                    subData[0] = mode;
                    subData[1] = channel;
                    subData[2] = minute;
                    SubPackage pkg = new SubPackage(PackageCmdWord.SetMode, subData);

                    PackageData sendPkg = new PackageData(MacAddr, 0, usart, pkg.ToBytes());
                    SendDataToDev(sendPkg.ToBytes());                    
                });

        }

        private void btnServer_Click(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    byte[] subData = new byte[0];
                    SubPackage pkg = new SubPackage(PackageCmdWord.Restart, subData);

                    PackageData sendPkg = new PackageData(MacAddr, 0, 0xFF, pkg.ToBytes());
                    SendDataToDev(sendPkg.ToBytes());
                });
        }

        private void lvwDev_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            AspectF.Define.Retry(Utility.CatchExpection)
                .Do(() =>
                {
                    string mac = e.Item.Text;
                    MacAddr = ByteHelper.HexStrToByteArrayWithDelimiter(mac, " ", false);
                    txtDevMAC.Text = e.Item.Text;
                });
        }
    }
}
