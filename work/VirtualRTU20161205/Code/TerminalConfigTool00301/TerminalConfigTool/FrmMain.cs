using CommunicateCore.Model;
using CommunicateCore.Terminal;
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
        private int TCPNum;
        private readonly int MaxTcpNum=100;
        //private byte[] ReceviceBuffer;
        //TcpClient CurClient;


        //用于线程同步，初始状态设为非终止状态，使用手动重置方式
        private EventWaitHandle allDone = new EventWaitHandle(false, EventResetMode.ManualReset);

        byte[] devAddr = new byte[8];
        
        public void InitFrm()
        {
            TerminalShare.GetShare.InitConfig();
            
            //IPAddress[] ip = Dns.GetHostAddresses(Dns.GetHostName());
            this.TCPNum = 0;
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
                    
                    TcpServer.Start(this.MaxTcpNum);
                    //while (true)
                    {
                        //allDone.Reset();
                        AsyncCallback callback = new AsyncCallback(AcceptTcpClient);
                        TcpServer.BeginAcceptTcpClient(callback, TcpServer);
                        //allDone.WaitOne();//阻塞当前线程，直到收到信号
                    }
                });
        }

        private void AcceptTcpClient(IAsyncResult ar)
        {
            AspectF.Define.Retry().
                Do(() =>
                {
                    //allDone.Set();
                    TcpListener myListener = ar.AsyncState as TcpListener;
                    TcpClient tcpClient = myListener.EndAcceptTcpClient(ar);
                    TCPNum++;
                    if (TCPNum <= this.MaxTcpNum)
                    {
                        TerminalClient curClient = new TerminalClient(tcpClient);

                        curClient.NetStream.BeginRead(curClient.ReceviceBuffer, 0, curClient.ReceviceBuffer.Length, ReadCallBack, curClient);
                    }
                    AsyncCallback callback = new AsyncCallback(AcceptTcpClient);
                    TcpServer.BeginAcceptTcpClient(callback, TcpServer);
                });
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            AspectF.Define.Retry().
                Do(() =>
                {
                    TerminalClient terminal = ar.AsyncState as TerminalClient;

                    if (terminal.CurTcpClient.Connected)
                    {
                        terminal.HeartBeatTime = DateTime.Now;
                        NetworkStream NetStream = terminal.NetStream;
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

                        //byte[] buffer = (byte[])ar.AsyncState;
                        byte[] receviceBytes = new byte[len];
                        Buffer.BlockCopy(terminal.ReceviceBuffer, 0, receviceBytes, 0, len);
                        TerminalShare.GetShare.WriterLog(ByteHelper.ByteToHexStrWithDelimiter(receviceBytes, " ", false));
                        AnalyzeProtocol_LFI(receviceBytes, terminal);

                        NetStream.BeginRead(terminal.ReceviceBuffer, 0, terminal.ReceviceBuffer.Length, ReadCallBack, terminal);
                    }
                });
        }

        /// <summary>
        /// 解析报文
        /// </summary>
        /// <param name="data"></param>
        /// <param name="session"></param>
        private void AnalyzeProtocol_LFI(byte[] data, TerminalClient terminal)
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

                RecevicePackageHandler(package, terminal);
            }
            while (afterPackge != null);
        }

        private void SendData(byte[] data, TerminalClient terminal)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    TerminalShare.GetShare.WriterLog(ByteHelper.ByteToHexStrWithDelimiter(data, " ", false));
                    terminal.NetStream.Write(data, 0, data.Length);
                });
        }

        /// <summary>
        /// 设备网络参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSet_Click(object sender, EventArgs e)
        {
            if (!chkDevList()) return;
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
            SendData(sendPkg.ToBytes(), TerminalShare.GetShare.CurClient);
        }

        private void btnCkeckTime_Click(object sender, EventArgs e)
        {
            if (!chkDevList()) return;
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
                SendData(sendPkg.ToBytes(), TerminalShare.GetShare.CurClient);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            if (!chkDevList()) return;
            this.txtIPOrDomain.Text = "";
            this.txtPort.Text = "";
            devAddr = TerminalShare.GetShare.CurClient.TerminalAddr;
            BrokerMessage sendMsg = new BrokerMessage();
            sendMsg.MsgType = MessageType.queryNetWorkParamter;
            sendMsg.TerminalAddress = devAddr;

            byte[] data = new byte[1];
            data[0] = 0;
            
            LFIPackageHandler sendPkg = new LFIPackageHandler(TerminalShare.GetShare.CurClient.TerminalAddr, data, LFICmdWordConst.QueryNetWorkPara);
            SendData(sendPkg.ToBytes(), TerminalShare.GetShare.CurClient);
        }

        private void btnOpenLamp_Click(object sender, EventArgs e)
        {
            if (!chkDevList()) return;
            RTCtrlLamp(0xFF, TerminalShare.GetShare.CurClient);
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
            if (!chkDevList()) return;
            RTCtrlLamp(0x00, TerminalShare.GetShare.CurClient);
        }
        
        private void btnQueryElecData_Click(object sender, EventArgs e)
        {
            if (!chkDevList()) return;
            this.txtElecData.Text = "";

            QueryElecData info = new QueryElecData();
            info.StartChNo = 1;
            info.ChNum = 2;

            LFIPackageHandler sendPkg = new LFIPackageHandler(TerminalShare.GetShare.CurClient.TerminalAddr, info.ToBytes(), LFICmdWordConst.QueryElecData);
            SendData(sendPkg.ToBytes(), TerminalShare.GetShare.CurClient);
            
        }

        private void btnSetGateway_Click(object sender, EventArgs e)
        {
            if (!chkDevList()) return;
            string staticIP = txtStaticIP.Text.Trim();
            string maskCode = txtMaskCode.Text.Trim();
            string gateWay = txtGateway.Text.Trim();

            AspectF.Define.Retry()
                .StringMustBeNonNull(new string[] { staticIP, maskCode, gateWay })
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
                    
                    LFIPackageHandler sendPkg = new LFIPackageHandler(TerminalShare.GetShare.CurClient.TerminalAddr, info.ToBytes(), LFICmdWordConst.SetEthernetInterface);
                    SendData(sendPkg.ToBytes(), TerminalShare.GetShare.CurClient);
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

        private void btnQueryGateway_Click(object sender, EventArgs e)
        {
            if (!chkDevList()) return;
            this.combType.SelectedIndex = 0;
            this.txtStaticIP.Text = "";
            this.txtMaskCode.Text = "";
            this.txtGateway.Text = "";

            byte[] data = new byte[1];
            data[0] = 0;

            LFIPackageHandler sendPkg = new LFIPackageHandler(TerminalShare.GetShare.CurClient.TerminalAddr, data, LFICmdWordConst.QueryEthernetInterface);
            SendData(sendPkg.ToBytes(), TerminalShare.GetShare.CurClient);
        }

        private readonly int Size = 512;
        private static byte[] upgradeData;
        private static string upgradeName;

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog oDialog = new OpenFileDialog();
            oDialog.InitialDirectory = System.Environment.CurrentDirectory; //"c:\\";
            oDialog.Filter = "升级包|*.bin";
            oDialog.RestoreDirectory = true;
            oDialog.FilterIndex = 1;
            if (oDialog.ShowDialog() == DialogResult.OK)
            {
                string name = oDialog.FileName;
                txtFileName.Text = name;
                FileStream fs = new FileStream(name, FileMode.Open, FileAccess.Read);
                long fileLen = fs.Length;
                upgradeData = new byte[fileLen];
                fs.Read(upgradeData, 0, (int)fileLen);
                upgradeName = oDialog.SafeFileName;
                //readyUpgrade(buffur,oDialog.SafeFileName);
            }
        }

        private void readyUpgrade(TerminalClient curClient, byte[] data,string fileName)
        {
            byte[] lampNo = new byte[2];
            long fileLen = data.Length;
            curClient.CurUpgradeInfo = new UpgradeInfo();
            curClient.CurUpgradeInfo.TotalNum = (byte)(fileLen / Size);
            if (fileLen % Size > 0) curClient.CurUpgradeInfo.TotalNum++;
            
            int nameLen = fileName.Length;
            curClient.CurUpgradeInfo.FileName = Encoding.UTF8.GetBytes(fileName);
            curClient.CurUpgradeInfo.FileData = new byte[fileLen];
            Buffer.BlockCopy(data, 0, curClient.CurUpgradeInfo.FileData, 0, (int)fileLen);
            byte[] bLen = new byte[4];
            bLen = BitConverter.GetBytes(fileLen);
            curClient.CurUpgradeInfo.FileLength = new byte[3];
            curClient.CurUpgradeInfo.FileLength[0] = bLen[2];
            curClient.CurUpgradeInfo.FileLength[1] = bLen[1];
            curClient.CurUpgradeInfo.FileLength[2] = bLen[0];
            byte[] crc = BitConverter.GetBytes(ByteHelper.GetCrc16(curClient.CurUpgradeInfo.FileData));
            curClient.CurUpgradeInfo.FileCRC = new byte[2];
            curClient.CurUpgradeInfo.FileCRC[0] = crc[1];
            curClient.CurUpgradeInfo.FileCRC[1] = crc[0];
        }

        private void btnStartUpgrade_Click(object sender, EventArgs e)
        {
            int num = chkListDevAddr.Items.Count;
            int selNum = 0;
            for (int i = 0; i < num; i++)
            {
                if (chkListDevAddr.GetItemChecked(i))
                {
                    selNum++;
                    string addr = chkListDevAddr.Items[i].ToString();
                    TerminalShare.GetShare.UpgradeList.Add(addr, null);
                    //if (TerminalShare.GetShare.ClientList.ContainsKey(addr))
                    //{
                    //    TerminalClient curClient = TerminalShare.GetShare.ClientList[addr] as TerminalClient;
                    //    if(TerminalShare.GetShare.UpgradeList.ContainsKey(addr))
                    //    {
                    //        TerminalShare.GetShare.UpgradeList.Remove(addr);
                    //    }
                    //    curClient.CurUpgradeInfo = new UpgradeInfo();
                    //    TerminalShare.GetShare.UpgradeList.Add(addr, curClient);
                    //}
                }
            }
            if(MessageBox.Show("是否对已选择的"+selNum+"个设备进行升级？","询问",MessageBoxButtons.OKCancel)==DialogResult.OK)
            {
                TerminalShare.GetShare.StartUpgrade = false;
                //foreach(KeyValuePair<string,TerminalClient> kvp in TerminalShare.GetShare.UpgradeList)
                //{
                //    readyUpgrade(kvp.Value,upgradeData,upgradeName);
                //    SendUpgradeFile(kvp.Value);
                //    break;//一个一个顺序升级
                //}
            }
            else
            {
                TerminalShare.GetShare.StartUpgrade = true;
                TerminalShare.GetShare.UpgradeList.Clear();
            }
        }

        public void SendUpgradeFile(TerminalClient curClient)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {                    
                    UpgradeInfo curInfo = new UpgradeInfo();
                    curInfo.UpgradeStatus = curClient.CurUpgradeInfo.Stage;
                    curClient.CurUpgradeInfo.sendTime = DateTime.Now;
                    bool isSend = false;
                    curClient.CurUpgradeInfo.ResendNo++;
                    if (curClient.CurUpgradeInfo.ResendNo > TerminalShare.GetShare.ReSendNum)//重试5次，仍然失败，则结束本次升级
                    {
                        curClient.CurUpgradeInfo.IsOK = true;
                        return;
                    }
                    switch (curClient.CurUpgradeInfo.Stage)
                    {
                        case 1:
                            curClient.CurUpgradeInfo.sendTime = DateTime.MinValue;
                            curInfo.FileName = curClient.CurUpgradeInfo.FileName;
                            curInfo.FileLength = curClient.CurUpgradeInfo.FileLength;
                            curInfo.FileCRC = curClient.CurUpgradeInfo.FileCRC;
                            //curClient.CurUpgradeInfo.Stage++;
                            isSend = true;
                            MonitorDevUpgrade();
                            break;
                        case 2:
                            if (curClient.CurUpgradeInfo.TotalNum >= curClient.CurUpgradeInfo.SendNo)
                            {
                                int curSize = Size;
                                if (curClient.CurUpgradeInfo.TotalNum == curClient.CurUpgradeInfo.SendNo)
                                {
                                    curSize = curClient.CurUpgradeInfo.FileData.Length - (curClient.CurUpgradeInfo.SendNo - 1) * Size;
                                }
                                byte[] len = BitConverter.GetBytes((short)curSize);
                                curInfo.FileLength = new byte[2];
                                curInfo.FileLength[0] = len[1];
                                curInfo.FileLength[1] = len[0];
                                curInfo.TotalNum = curClient.CurUpgradeInfo.TotalNum;
                                curInfo.SendNo = curClient.CurUpgradeInfo.SendNo;
                                
                                curInfo.FileData = new byte[curSize];
                                Buffer.BlockCopy(curClient.CurUpgradeInfo.FileData, (curClient.CurUpgradeInfo.SendNo - 1) * Size, curInfo.FileData, 0, curSize);

                            }
                            isSend = true;
                            break;
                        case 3:
                            isSend = true;
                            break;
                    }
                    if (isSend)
                    {
                        LFIPackageHandler sendPkg = new LFIPackageHandler(curClient.TerminalAddr, curInfo.ToBytes(), LFICmdWordConst.Upgrade);
                        SendData(sendPkg.ToBytes(), curClient);
                        if (curClient.CurUpgradeInfo.Stage == 3)
                        {
                            curClient.CurUpgradeInfo.IsOK = true;
                            if(TerminalShare.GetShare.UpgradeList.ContainsKey(curClient.Addr))
                            {
                                TerminalShare.GetShare.UpgradeList.Remove(curClient.Addr);
                                TerminalShare.GetShare.StartUpgrade = false;
                            }
                            if(TerminalShare.GetShare.UpgradeList.Count ==0)
                            {
                                MessageBox.Show("升级结束！", "提示");
                            }
                            //else
                            //{
                            //    foreach (KeyValuePair<string, TerminalClient> kvp in TerminalShare.GetShare.UpgradeList)
                            //    {
                            //        if (TerminalShare.GetShare.ClientList.ContainsKey(kvp.Key))
                            //        {
                            //            TerminalClient newClient = TerminalShare.GetShare.ClientList[kvp.Key] as TerminalClient;

                            //            TerminalShare.GetShare.UpgradeList[kvp.Key] = newClient;
                            //        }
                            //        readyUpgrade(kvp.Value, upgradeData, upgradeName);
                            //        SendUpgradeFile(kvp.Value);
                            //        break;//一个一个顺序升级
                            //    }
                            //}
                        }
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
                            string []keys = TerminalShare.GetShare.UpgradeList.Keys.ToArray();
                            foreach(string key in keys)
                            {
                                if(TerminalShare.GetShare.UpgradeList.ContainsKey(key))
                                {
                                    TerminalClient value = TerminalShare.GetShare.UpgradeList[key];
                                    if (value != null && value.CurUpgradeInfo != null && value.CurUpgradeInfo.IsOK == false && value.CurUpgradeInfo.sendTime != DateTime.MinValue)
                                    {
                                        TimeSpan ts = DateTime.Now - value.CurUpgradeInfo.sendTime;
                                        if (ts.TotalMilliseconds >= 1000 * 5 && value.CurUpgradeInfo.ResendNo < TerminalShare.GetShare.ReSendNum && value.CurUpgradeInfo.Stage != 3)
                                        {
                                            Console.WriteLine("发送超时！");
                                            Thread.Sleep(1000);
                                            SendUpgradeFile(value);
                                        }
                                    }
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

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            int num = chkListDevAddr.Items.Count;
            if(chkSelectAll.Checked)
            {
                for(int i=0;i<num;i++)
                {
                    chkListDevAddr.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < num; i++)
                {
                    chkListDevAddr.SetItemChecked(i, false);
                }
            }
        }
        
    }
}
