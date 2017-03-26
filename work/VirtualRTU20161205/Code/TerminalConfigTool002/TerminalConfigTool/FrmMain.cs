using CommunicateCore.Model;
using CommunicateCore.Terminal;
using CommunicateCore.Terminal.TerminalBusiness;
using CommunicateCore.Utility;
using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TerminalConfigTool.model;
using TerminalConfigTool.packageHandler;
using Utility;
using Utility.Model;

namespace TerminalConfigTool
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            InitFrm();
        }

        TcpListener TcpServer;
        private static int TCPNum;
        private byte[] ReceviceBuffer;
        TcpClient CurClient;

        //用于线程同步，初始状态设为非终止状态，使用手动重置方式
        private EventWaitHandle allDone = new EventWaitHandle(false, EventResetMode.ManualReset);

        byte[] devAddr = new byte[8];
        
        public void InitFrm()
        {
            TerminalShare.GetShare.InitConfig();

            //IPAddress[] ip = Dns.GetHostAddresses(Dns.GetHostName());

            //TcpServer = new TcpListener(ip[0], TerminalShare.GetShare.LocalPort);
            TcpServer = new TcpListener(TerminalShare.GetShare.LocalIP, TerminalShare.GetShare.LocalPort);
            combType.SelectedIndex = 0;
            ThreadStart s = new ThreadStart(AcceptConnect);
            Thread tcpThread = new Thread(s);
            tcpThread.Start();
        }


        private void AcceptConnect()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    //让其他需要等待的线程阻塞
                    //将事件的状态设为非终止
                    
                    TcpServer.Start(3);
                    while (true)
                    {
                        allDone.Reset();
                        AsyncCallback callback = new AsyncCallback(AcceptTcpClient);
                        TcpServer.BeginAcceptTcpClient(callback, TcpServer);
                        allDone.WaitOne();//阻塞当前线程，直到收到信号
                    }
                });
        }

        private void AcceptTcpClient(IAsyncResult ar)
        {
            AspectF.Define.Retry().
                Do(() =>
                {
                    allDone.Set();
                    TcpListener myListener = ar.AsyncState as TcpListener;
                    TcpClient tcpClient = myListener.EndAcceptTcpClient(ar);
                    TCPNum++;
                    if (TCPNum > 1)
                    {
                        if (CurClient != null)
                        {
                            CurClient.Close();
                        }
                    }
                    CurClient = tcpClient;
                    ReceviceBuffer = new byte[tcpClient.ReceiveBufferSize];
                    tcpClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, ReadCallBack, ReceviceBuffer);
                });
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            AspectF.Define.Retry().
                Do(() =>
                {
                    if (CurClient.Connected)
                    {
                        NetworkStream NetStream = CurClient.GetStream();
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
                        TerminalShare.GetShare.WriterLog(ByteHelper.ByteToHexStrWithDelimiter(receviceBytes, " ", false));
                        AnalyzeProtocol_LFI(receviceBytes);

                        CurClient.GetStream().BeginRead(ReceviceBuffer, 0, ReceviceBuffer.Length, ReadCallBack, ReceviceBuffer);
                    }
                });
        }

        /// <summary>
        /// 解析报文
        /// </summary>
        /// <param name="data"></param>
        /// <param name="session"></param>
        private void AnalyzeProtocol_LFI(byte[] data)
        {
            byte[] afterPackge = null;
            LFIPackageHandler package = null;

            do
            {
                package = new LFIPackageHandler();
                if (afterPackge == null)
                {
                    package.BuildPackageFromBytes(data);
                }
                else
                {
                    package.BuildPackageFromBytes(afterPackge);
                }
                afterPackge = package.afterPackage;

                RecevicePackageHandler(package);
            }
            while (afterPackge != null);
        }

        private void SendData(byte[] data)
        {
            TerminalShare.GetShare.WriterLog(ByteHelper.ByteToHexStrWithDelimiter(data, " ", false));
            CurClient.GetStream().Write(data, 0, data.Length);
        }


        private void btnSet_Click(object sender, EventArgs e)
        {
            string ipOrDomain = txtIPOrDomain.Text.Trim();
            string port = txtPort.Text.Trim();

            if (string.IsNullOrEmpty(ipOrDomain))
            {
                MessageBox.Show("IP或域名不能为空！请重新输入！", "提示");
                return;
            }

            if (string.IsNullOrEmpty(port))
            {
                MessageBox.Show("端口不能为空！请重新输入！", "提示");
                return;
            }
            devAddr = TerminalShare.GetShare.CurClient.TerminalAddr;
            NetWorkInfo info = new NetWorkInfo();
            info.SetNum = 2;
            info.SetInfo = new SetInfo[2];
            SetInfo IpInfo = new SetInfo();
            IpInfo.ID = 1;
            IpInfo.data = Encoding.UTF8.GetBytes(ipOrDomain);
            info.SetInfo[0] = IpInfo;
            SetInfo portInfo = new SetInfo();
            portInfo.ID = 2;
            byte[] bPort = BitConverter.GetBytes(ushort.Parse(port));
            portInfo.data = new byte[2];
            portInfo.data[0] = bPort[1];
            portInfo.data[1] = bPort[0];
            info.SetInfo[1] = portInfo;
            
            LFIPackageHandler sendPkg = new LFIPackageHandler(TerminalShare.GetShare.CurClient.TerminalAddr, info.ToBytes(), LFICmdWordConst.SetNetWorkPara);
            SendData(sendPkg.ToBytes());
        }

        private void btnCkeckTime_Click(object sender, EventArgs e)
        {
            DateTime curTime = dtPick.Value;
            string str = txtTime.Text.Trim();
            try
            {
                devAddr = TerminalShare.GetShare.CurClient.TerminalAddr;
                if (chkHand.Checked)
                {
                    curTime = DateTime.Parse(str);
                }
                SetTimeInfo info = new SetTimeInfo();
                info.BuildTime(curTime);
                LFIPackageHandler sendPkg = new LFIPackageHandler(TerminalShare.GetShare.CurClient.TerminalAddr, info.ToBytes(), LFICmdWordConst.SetTime);
                SendData(sendPkg.ToBytes());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            this.txtIPOrDomain.Text = "";
            this.txtPort.Text = "";
            devAddr = TerminalShare.GetShare.CurClient.TerminalAddr;
            BrokerMessage sendMsg = new BrokerMessage();
            sendMsg.MsgType = MessageType.queryNetWorkParamter;
            sendMsg.TerminalAddress = devAddr;

            byte[] data = new byte[1];
            data[0] = 0;
            
            LFIPackageHandler sendPkg = new LFIPackageHandler(TerminalShare.GetShare.CurClient.TerminalAddr, data, LFICmdWordConst.QueryNetWorkPara);
            SendData(sendPkg.ToBytes());
        }

        private void btnOpenLamp_Click(object sender, EventArgs e)
        {
            RTCtrlLamp(0xFF);
        }

        private void txtPort_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtChNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void btnCloseLamp_Click(object sender, EventArgs e)
        {
            RTCtrlLamp(0x00);
        }
        
        private void btnQueryElecData_Click(object sender, EventArgs e)
        {
            this.txtElecData.Text = "";

            QueryElecData info = new QueryElecData();
            info.StartChNo = 1;
            info.ChNum = 2;

            LFIPackageHandler sendPkg = new LFIPackageHandler(TerminalShare.GetShare.CurClient.TerminalAddr, info.ToBytes(), LFICmdWordConst.QueryElecData);
            SendData(sendPkg.ToBytes());
            
        }

        private void btnSetGateway_Click(object sender, EventArgs e)
        {
            string staticIP = txtStaticIP.Text.Trim();
            string maskCode = txtMaskCode.Text.Trim();
            string gateWay = txtGateway.Text.Trim();
            string mac = txtMac.Text.Trim();

            AspectF.Define.Retry()
                .StringMustBeNonNull(new string[] { staticIP, maskCode, gateWay, mac })
                .Do(() =>
                {
                    if (!TerminalShare.GetShare.IsIP(staticIP))
                    {
                        TerminalShare.GetShare.WriterLog("静态IP格式错误！");
                        MessageBox.Show("静态IP格式错误！请重新输入！", "提示");
                        return;
                    }
                    if (!TerminalShare.GetShare.IsIP(maskCode))
                    {
                        TerminalShare.GetShare.WriterLog("子网掩码格式错误！");
                        MessageBox.Show("子网掩码格式错误！请重新输入！", "提示");
                        return;
                    }
                    if (!TerminalShare.GetShare.IsIP(gateWay))
                    {
                        TerminalShare.GetShare.WriterLog("网关格式错误！");
                        MessageBox.Show("网关格式错误！请重新输入！", "提示");
                        return;
                    }

                    devAddr = TerminalShare.GetShare.CurClient.TerminalAddr;
                    EthernetInterface info = new EthernetInterface();
                    info.CfgNum = 4;
                    info.CfgDetail = new ConfigDetail[4];
                    ConfigDetail infoType = new ConfigDetail();
                    infoType.CfgID = 1;
                    byte type = 0;
                    if (combType.SelectedIndex == 0)
                    {
                        type = 1;
                    }
                    infoType.Detail = new byte[] { type };
                    info.CfgDetail[0] = infoType;

                    ConfigDetail infoStaticIP = new ConfigDetail();
                    infoStaticIP.CfgID = 2;
                    infoStaticIP.Detail = GetIPsByString(staticIP);
                    info.CfgDetail[1] = infoStaticIP;

                    ConfigDetail infoMaskCode = new ConfigDetail();
                    infoMaskCode.CfgID = 3;
                    infoMaskCode.Detail = GetIPsByString(maskCode);
                    info.CfgDetail[2] = infoMaskCode;

                    ConfigDetail infoGateWay = new ConfigDetail();
                    infoGateWay.CfgID = 4;
                    infoGateWay.Detail = GetIPsByString(gateWay);
                    info.CfgDetail[3] = infoGateWay;

                    ConfigDetail infoMac = new ConfigDetail();
                    infoMac.CfgID = 5;
                    infoMac.Detail = GetMacByString(mac);
                    info.CfgDetail[4] = infoMac;

                    LFIPackageHandler sendPkg = new LFIPackageHandler(TerminalShare.GetShare.CurClient.TerminalAddr, info.ToBytes(), LFICmdWordConst.SetEthernetInterface);
                    SendData(sendPkg.ToBytes());
                });
        }

        private byte[] GetIPsByString(string ip)
        {
            byte[] result = new byte[4];

            string[] arr = ip.Split('.');
            if (arr.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    result[i] = byte.Parse(arr[i]);
                }
            }

            return result;
        }

        private byte[] GetMacByString(string mac)
        {
            byte[] result = new byte[6];

            string[] arr = mac.Split(':');
            if (arr.Length == 6)
            {
                for (int i = 0; i < 6; i++)
                {
                    result[i] = byte.Parse(arr[i]);
                }
            }

            return result;
        }

        private void btnQueryGateway_Click(object sender, EventArgs e)
        {
            this.combType.SelectedIndex = 0;
            this.txtStaticIP.Text = "";
            this.txtMaskCode.Text = "";
            this.txtGateway.Text = "";
            this.txtMac.Text = "";

            byte[] data = new byte[1];
            data[0] = 0;

            LFIPackageHandler sendPkg = new LFIPackageHandler(TerminalShare.GetShare.CurClient.TerminalAddr, data, LFICmdWordConst.QueryEthernetInterface);
            SendData(sendPkg.ToBytes());
        }

        private readonly int Size = 512;
        private static byte Stage;
        private static DateTime sendTime = DateTime.MinValue;
        private int ResendNo = 0;

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog oDialog = new OpenFileDialog();
            oDialog.InitialDirectory = "c:\\";
            oDialog.Filter = "升级包|*.bin";
            oDialog.RestoreDirectory = true;
            oDialog.FilterIndex = 1;
            if (oDialog.ShowDialog() == DialogResult.OK)
            {
                string name = oDialog.FileName;
                txtFileName.Text = name;
                FileStream fs = new FileStream(name, FileMode.Open, FileAccess.Read);
                long fileLen = fs.Length;
                byte[] buffur = new byte[fileLen];
                fs.Read(buffur, 0, (int)fileLen);

                readyUpgrade(buffur,oDialog.SafeFileName);
            }
        }

        private void readyUpgrade(byte[] data,string fileName)
        {
            byte[] lampNo = new byte[2];
            long fileLen = data.Length;
            TerminalShare.GetShare.curUpgradeInfo = new UpgradeInfo();
            TerminalShare.GetShare.curUpgradeInfo.TotalNum = (byte)(fileLen / Size);
            if (fileLen % Size > 0) TerminalShare.GetShare.curUpgradeInfo.TotalNum++;
            txtTotal.Text= TerminalShare.GetShare.curUpgradeInfo.TotalNum.ToString();
            int nameLen = fileName.Length;
            TerminalShare.GetShare.curUpgradeInfo.FileName = Encoding.UTF8.GetBytes(fileName);
            TerminalShare.GetShare.curUpgradeInfo.FileData = new byte[fileLen];
            Buffer.BlockCopy(data, 0, TerminalShare.GetShare.curUpgradeInfo.FileData, 0, (int)fileLen);
            byte[] bLen = new byte[4];
            bLen = BitConverter.GetBytes(fileLen);
            TerminalShare.GetShare.curUpgradeInfo.FileLength = new byte[3];
            TerminalShare.GetShare.curUpgradeInfo.FileLength[0] = bLen[2];
            TerminalShare.GetShare.curUpgradeInfo.FileLength[1] = bLen[1];
            TerminalShare.GetShare.curUpgradeInfo.FileLength[2] = bLen[0];
            byte[] crc = BitConverter.GetBytes(ByteHelper.GetCrc16(TerminalShare.GetShare.curUpgradeInfo.FileData));
            TerminalShare.GetShare.curUpgradeInfo.FileCRC = new byte[2];
            TerminalShare.GetShare.curUpgradeInfo.FileCRC[0] = crc[1];
            TerminalShare.GetShare.curUpgradeInfo.FileCRC[1] = crc[0];

        }

        private void btnStartUpgrade_Click(object sender, EventArgs e)
        {

            Stage = 1;
            SendUpgradeFile();
        }

        public void SendUpgradeFile()
        {
            AspectF.Define.Retry()
                .Do(() =>
                {                    
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
                            curInfo.FileName = TerminalShare.GetShare.curUpgradeInfo.FileName;
                            curInfo.FileLength = TerminalShare.GetShare.curUpgradeInfo.FileLength;
                            curInfo.FileCRC = TerminalShare.GetShare.curUpgradeInfo.FileCRC;
                            Stage++;
                            isSend = true;
                            MonitorDevUpgrade();
                            break;
                        case 2:
                            if (TerminalShare.GetShare.curUpgradeInfo.TotalNum >= TerminalShare.GetShare.curUpgradeInfo.SendNo)
                            {
                                int curSize = Size;
                                if (TerminalShare.GetShare.curUpgradeInfo.TotalNum == TerminalShare.GetShare.curUpgradeInfo.SendNo)
                                {
                                    curSize = TerminalShare.GetShare.curUpgradeInfo.FileData.Length - (TerminalShare.GetShare.curUpgradeInfo.SendNo - 1) * Size;
                                }
                                byte[] len = BitConverter.GetBytes((short)curSize);
                                curInfo.FileLength = new byte[2];
                                curInfo.FileLength[0] = len[1];
                                curInfo.FileLength[1] = len[0];
                                curInfo.TotalNum = TerminalShare.GetShare.curUpgradeInfo.TotalNum;
                                curInfo.SendNo = TerminalShare.GetShare.curUpgradeInfo.SendNo;

                                ShowSendNo(TerminalShare.GetShare.curUpgradeInfo.SendNo.ToString());
                                curInfo.FileData = new byte[curSize];
                                Buffer.BlockCopy(TerminalShare.GetShare.curUpgradeInfo.FileData, (TerminalShare.GetShare.curUpgradeInfo.SendNo - 1) * Size, curInfo.FileData, 0, curSize);
                            }
                            isSend = true;
                            break;
                        case 3:
                            isSend = true;
                            break;
                    }
                    if (isSend)
                    {
                        LFIPackageHandler sendPkg = new LFIPackageHandler(TerminalShare.GetShare.CurClient.TerminalAddr, curInfo.ToBytes(), LFICmdWordConst.Upgrade);
                        SendData(sendPkg.ToBytes());
                        if(Stage==3)
                        {
                            MessageBox.Show("升级结束！", "提示");
                        }
                    }
                });
        }

        private delegate void SetSendNo(string no);//代理

        private void ShowSendNo(string no)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    if (this.InvokeRequired)
                    {
                        SetSendNo setpos = new SetSendNo(ShowSendNo);
                        this.Invoke(setpos, no);
                    }
                    else
                    {
                        this.txtSendNo.Text = no;
                    }
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
                                if (ts.TotalMilliseconds >= 1000 * 5 && ResendNo < 5 && Stage != 3)
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
                TerminalShare.GetShare.WriterLog(ex.Message);
                MonitorDevUpgrade();
            }
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            TcpServer.Stop();
        }        
    }
}
