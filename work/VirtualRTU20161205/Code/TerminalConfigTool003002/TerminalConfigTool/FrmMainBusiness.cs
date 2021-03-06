﻿using CommunicateCore.Model;
using CommunicateCore.Terminal;
using CommunicateCore.Utility;
using LFCDal.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TerminalConfigTool.model;
using TerminalConfigTool.packageHandler;
using Utility;

namespace TerminalConfigTool
{
    partial class FrmMain
    {

        private void RecevicePackageHandler(LFIPackageHandler package, TerminalClient terminal)
        {
            int curIndex = 0;
            AspectF.Define.Retry()
                .Do(() =>
                {
                    byte seq = package.OnlyData[curIndex++];
                    byte[] time = new byte[7];
                    DateTime curTime = DateTime.MinValue;
                    if (ByteHelper.GetBit(seq, 0) == 1)
                    {
                        Buffer.BlockCopy(package.OnlyData, curIndex, time, 0, 7);
                        curTime = ByteHelper.Bytes7ToDateTime(time, false);
                        curIndex += 7;
                    }
                    if (ByteHelper.GetBit(seq, 1) == 1)
                    {
                        curIndex += 2;
                    }
                    if (ByteHelper.ByteArryEquals(package.CmdWord, LFICmdWordConst.Login))//登录
                    {
                        LoginPackageHandler(package, terminal, curIndex);
                    }
                    else if (ByteHelper.ByteArryEquals(package.CmdWord, LFICmdWordConst.HeartBeat))//心跳
                    {
                        HeartBeatPackageHandler(package, terminal);
                    }
                    else if (ByteHelper.ByteArryEquals(package.CmdWord, LFICmdWordConst.RealTimeControl))//实时控制
                    {
                        
                    }
                    else if (ByteHelper.ByteArryEquals(package.CmdWord, LFICmdWordConst.Alarm))//报警
                    {
                        
                    }
                    else if (ByteHelper.ByteArryEquals(package.CmdWord, LFICmdWordConst.QueryElecDataBack))//电参数
                    {
                        ElecDataPackageHandler(package, terminal);
                    }
                    else if (ByteHelper.ByteArryEquals(package.CmdWord, LFICmdWordConst.SetYearTableBack))//设置年表
                    {
                        
                    }
                    else if (ByteHelper.ByteArryEquals(package.CmdWord, LFICmdWordConst.QueryYearTableBack))//查询年表
                    {
                        
                    }
                    else if (ByteHelper.ByteArryEquals(package.CmdWord, LFICmdWordConst.UpgradeBack))//远程升级
                    {
                        //string addr = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, " ",false);
                        //if (TerminalShare.GetShare.UpgradeList.ContainsKey(addr))
                        //{
                        //    TerminalClient curClient = TerminalShare.GetShare.UpgradeList[addr];
                            UpgradePackageHandler(package.OnlyData[curIndex++], terminal);
                        //}
                    }
                    else if (ByteHelper.ByteArryEquals(package.CmdWord, LFICmdWordConst.QueryNetWorkParaBack))//查询网络参数
                    {
                        NetWorkPackageHandler(package, terminal);
                    }
                    else if (ByteHelper.ByteArryEquals(package.CmdWord, LFICmdWordConst.QueryEthernetInterfaceBack))//查询网口参数
                    {
                        EthernetInterfacePackageHandler(package, terminal);
                    }

                });
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="package"></param>
        private void LoginPackageHandler(LFIPackageHandler package, TerminalClient terminal,int curIndex)
        {
            LoginInfo info = new LoginInfo();
            info.BuildLoginInfo(package.OnlyData);
            string addr = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr," ",false);
            int len = package.DevAddr.Length;
            terminal.TerminalAddr = new byte[len];
            Buffer.BlockCopy(package.DevAddr, 0, terminal.TerminalAddr, 0, len);
            terminal.Addr = addr;

            terminal.HeartBeatTime = DateTime.Now;
            terminal.LoginTime = DateTime.Now;
            terminal.SVer = GetVersion(package.OnlyData, curIndex+41);//49

            if (TerminalShare.GetShare.ClientList.ContainsKey(addr))
            {
                //关闭原来的client，并删除列表后，再添加。
                TerminalClient Duplicate_Client = (TerminalClient)TerminalShare.GetShare.ClientList[addr];
                Duplicate_Client.Close();

                lock (TerminalShare.GetShare.ClientList.SyncRoot)
                {
                    TerminalShare.GetShare.ClientList.Remove(addr);
                    Duplicate_Client = null; 
                }
            }
            //else
            //{
                ShowTitle(addr , terminal.SVer);
            //}
            string strFilePath = Path.Combine(Environment.CurrentDirectory, terminal.SVer+".txt");
            if (TerminalShare.CheckLogFile(addr + " --- " + terminal.SVer, strFilePath))
            {
                TerminalShare.OpenLogFile(strFilePath);
                TerminalShare.WriteLogFile(addr + " --- " + terminal.SVer);
                TerminalShare.CloseLogFile();
            }
            TerminalShare.GetShare.ClientList.Add(addr, terminal);
            if(TerminalShare.GetShare.UpgradeList.ContainsKey(addr) && terminal.SVer != TerminalShare.GetShare.NewSVer)
            {
                terminal.CurUpgradeInfo = null;
                TerminalShare.GetShare.UpgradeList[addr]= terminal;
            }

            LoginBackInfo backInfo = new LoginBackInfo();
            backInfo.LoginReuslt = 1;

            LFIPackageHandler sendPkg = new LFIPackageHandler(package.DevAddr, backInfo.ToBytes(), LFICmdWordConst.LoginBack);
            SendData(sendPkg.ToBytes(), terminal);
        }


        public static string GetVersion(byte[] data, int startIndex)
        {
            //版本号只有两个字节
            if (data.Length < 2) return "";

            return "V" + data[startIndex].ToString("X2") + "." + data[startIndex + 1].ToString("X2");
        }

        private delegate void SetTitle(string addr,string sVer);//代理

        private void ShowTitle(string addr,string sVer)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    if (this.InvokeRequired)
                    {
                        SetTitle setpos = new SetTitle(ShowTitle);
                        this.Invoke(setpos, addr,sVer);
                    }
                    else
                    {
                        //this.txtTerminalAddr.Text = addr;
                        string val = addr + " - " + sVer;
                        int num = chkListDevAddr.Items.Count;                        
                        for (int i = 0; i < num; i++)
                        {
                            string value = chkListDevAddr.Items[i].ToString();
                            int no = value.IndexOf('-');

                            string newAddr = value.Substring(0, no - 1).Trim();
                            if (newAddr == addr)
                            {
                                chkListDevAddr.Items.RemoveAt(i);
                                break;
                            }
                        }
                        chkListDevAddr.Items.Add(val);
                        chkSelectAll.Text = "全选(" + (num+1).ToString() + ")";
                    }
                });
        }

        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="package"></param>
        public void HeartBeatPackageHandler(LFIPackageHandler package, TerminalClient terminal)
        {
            byte []data = new byte[1];
            data[0] = 0; 

            if(terminal != null && terminal.TerminalAddr !=null && terminal.TerminalAddr.Length>0)
            {
                //if (ByteHelper.ByteArryEquals(terminal.TerminalAddr, package.DevAddr) && TerminalShare.GetShare.UpgradeList!=null && TerminalShare.GetShare.UpgradeList.Count >0)
                //{
                //    TerminalShare.GetShare.WriterLog("升级数量："+TerminalShare.GetShare.UpgradeList.Count.ToString());
                //    string[] keys = TerminalShare.GetShare.UpgradeList.Keys.ToArray();
                //    foreach (string key in keys)
                //    {
                //        if(key==terminal.Addr)
                //        {
                //            if (TerminalShare.GetShare.ClientList.ContainsKey(key))
                //            {
                //                TerminalShare.GetShare.WriterLog("设备列表已存在：" + terminal.Addr);
                //                TerminalClient newClient = TerminalShare.GetShare.ClientList[key] as TerminalClient;
                //                TerminalShare.GetShare.WriterLog("旧地址：" + newClient.Addr);
                //                if (newClient.Addr == terminal.Addr)
                //                {
                //                    TerminalShare.GetShare.UpgradeList[key] = newClient;
                //                    if (newClient.CurUpgradeInfo == null)
                //                    {
                //                        newClient.CurUpgradeInfo = new UpgradeInfo();
                //                        readyUpgrade(terminal, upgradeData, upgradeName);
                //                    }
                //                    //readyUpgrade(newClient, upgradeData, upgradeName);
                //                    SendUpgradeFile(newClient);

                //                    break;
                //                }
                //            }
                //            else
                //            {
                //                TerminalShare.GetShare.WriterLog("设备列表已不存在");
                //            }
                //        }
                //    }
                //}
                LFIPackageHandler sendPkg = new LFIPackageHandler(package.DevAddr, data, LFICmdWordConst.HeartBeatBack);
                SendData(sendPkg.ToBytes(), terminal);
            }
        }

        //实时控制
        private void RTCtrlLamp(byte optValue, TerminalClient terminal)
        {
            AspectF.Define.Retry()
                .Do(() =>
                {
                    string chNo = txtChNo.Text.Trim();
                    bool isLock = chkLock.Checked;
                    byte bIsLock = (byte)(isLock ? 1 : 0);
                    if (string.IsNullOrEmpty(chNo))
                    {
                        MessageBox.Show("IP或域名不能为空！请重新输入！", "提示");
                        return;
                    }
                    byte[] data = new byte[4];
                    data[0] = 0;//SEQ
                    data[1] = byte.Parse(chNo);
                    data[2] = optValue;
                    data[3] = bIsLock;
                    LFIPackageHandler sendPkg = new LFIPackageHandler(terminal.TerminalAddr, data, LFICmdWordConst.RealTimeControl);
                    SendData(sendPkg.ToBytes(), terminal);
                });
        }

        //单灯电参数
        private void ElecDataPackageHandler(LFIPackageHandler package, TerminalClient terminal)
        {
            //string jsonStr = ((Newtonsoft.Json.Linq.JToken)bMsg.MsgBody).Root.ToString();
            //QueryElecData info = JsonSerializeHelper.GetHelper.Deserialize<QueryElecData>(jsonStr);
            QueryElecData info = new QueryElecData();
            info.BuildElecDataInfo(package.OnlyData);
            List<tLampHisDataRec> listDataRec = new List<tLampHisDataRec>();
            string result = "";
            for (int i = 0; i < info.ChNum; i++)
            {
                int chNo = i + 1;
                tLampHisDataRec elecData = TerminalShare.GetShare.GetElecDataFromBytes(info.ArrElecData[i]);
                listDataRec.Add(elecData);
                result += "第 " + chNo + " 回路电参数为：电压：" + elecData.LampU + "，电流：" + elecData.LampI + "，有功：" + elecData.LampAP + "，功率因数：" + elecData.LampPF;
                result += "\r\n";
            }
            tLampHisDataRec[] ArrElecData = listDataRec.ToArray();
            ShowElecData(result);
        }

        private delegate void SetElecData(string curValue);//代理

        private void ShowElecData(string curValue = "")
        {
            if (this.InvokeRequired)
            {
                SetElecData setpos = new SetElecData(ShowElecData);
                this.Invoke(setpos, curValue);
            }
            else
            {
                txtElecData.Text = curValue;
            }
        }

        /// <summary>
        /// 网络参数
        /// </summary>
        /// <param name="package"></param>
        private void NetWorkPackageHandler(LFIPackageHandler package, TerminalClient terminal)
        {
            NetWorkInfo info = new NetWorkInfo();
            info.BuildNetWorkInfo(package.OnlyData);
            NetWorkPara curNetWorkPara = new NetWorkPara();

            for (int i = 0; i < info.SetNum; i++)
            {
                SetInfo sInfo = info.SetInfo[i];
                if (sInfo.ID == 1)
                {
                    curNetWorkPara.IPOrDomain = Encoding.UTF8.GetString(sInfo.data);// txtIPOrDomain.Text = Encoding.UTF8.GetString(sInfo.data);
                }
                else if (sInfo.ID == 2)
                {
                    int len = sInfo.data.Length;
                    byte[] bPort = new byte[len];
                    for (int j = 0; j < len; j++)
                    {
                        bPort[j] = sInfo.data[len - 1 - j];
                    }
                    curNetWorkPara.Port = BitConverter.ToUInt16(bPort, 0).ToString();// txtPort.Text = BitConverter.ToInt16(sInfo.data,0).ToString();
                }
                ShowNetWorkPara(curNetWorkPara);
            }
        }

        private class NetWorkPara
        {
            public string IPOrDomain { get; set; }

            public string Port { get; set; }
        }

        private delegate void SetNetWorkPara(NetWorkPara curValue);//代理

        private void ShowNetWorkPara(NetWorkPara curValue)
        {
            if (this.InvokeRequired)
            {
                SetNetWorkPara setpos = new SetNetWorkPara(ShowNetWorkPara);
                this.Invoke(setpos, curValue);
            }
            else
            {
                txtIPOrDomain.Text = curValue.IPOrDomain;
                txtPort.Text = curValue.Port;
            }
        }

        /// <summary>
        /// 网口参数
        /// </summary>
        /// <param name="package"></param>
        private void EthernetInterfacePackageHandler(LFIPackageHandler package, TerminalClient terminal)
        {
            EthernetInterface info = new EthernetInterface();
            info.BuildEthernetInterfaceInfo(package.OnlyData);
            ShowEthernetInterface(info);
        }

        private delegate void SetEthernetInterface(EthernetInterface info);//代理

        private void ShowEthernetInterface(EthernetInterface info)
        {
            if (this.InvokeRequired)
            {
                SetEthernetInterface setpos = new SetEthernetInterface(ShowEthernetInterface);
                this.Invoke(setpos, info);
            }
            else
            {
                for (int i = 0; i < info.CfgNum; i++)
                {
                    ConfigDetail sInfo = info.CfgDetail[i];
                    switch (sInfo.CfgID)
                    {
                        case 0x01://IP地址类型
                            if (sInfo.Detail.Length == 1)
                            {
                                byte type = sInfo.Detail[0];
                                if (type == 0)
                                {
                                    combType.SelectedIndex = 1;
                                }
                                else
                                {
                                    combType.SelectedIndex = 0;
                                }
                            }
                            break;
                        case 0x02://静态IP地址
                            txtStaticIP.Text = sInfo.Detail[0].ToString() + "." + sInfo.Detail[1].ToString() + "." + sInfo.Detail[2].ToString() + "." + sInfo.Detail[3].ToString();

                            break;
                        case 0x03://子网掩码
                            txtMaskCode.Text = sInfo.Detail[0].ToString() + "." + sInfo.Detail[1].ToString() + "." + sInfo.Detail[2].ToString() + "." + sInfo.Detail[3].ToString();

                            break;
                        case 0x04://网关
                            txtGateway.Text = sInfo.Detail[0].ToString() + "." + sInfo.Detail[1].ToString() + "." + sInfo.Detail[2].ToString() + "." + sInfo.Detail[3].ToString();

                            break;
                        case 0x05://MAC地址
                            txtMac.Text = sInfo.Detail[0].ToString("X2") + ":" + sInfo.Detail[1].ToString("X2") + ":" + sInfo.Detail[2].ToString("X2") + ":" + sInfo.Detail[3].ToString("X2") + ":" + sInfo.Detail[4].ToString("X2") + ":" + sInfo.Detail[5].ToString("X2");

                            break;
                    }
                }
            }
        }

        private void UpgradePackageHandler(byte status, TerminalClient terminal)
        {
            AspectF.Define.Retry()
                .Log(TerminalShare.GetShare.WriterLog, "", terminal.CurUpgradeInfo.ResendNo > TerminalShare.GetShare.ReSendNum ? "已经重试" + TerminalShare.GetShare.ReSendNum + "次，发送失败，中断本次升级:" + terminal.Addr : "")
                .Do(() =>
                {
                    if (status==1)
                    {
                        //UpgradeInfo ReceviceInfo = BMsg.MsgBody as UpgradeInfo;
                        //terminal.CurUpgradeInfo.SendNo++;
                        terminal.CurUpgradeInfo.ResendNo = 0;


                        if (terminal.CurUpgradeInfo.Stage == 2) terminal.CurUpgradeInfo.SendNo++;
                        if (terminal.CurUpgradeInfo.Stage == 1 || (terminal.CurUpgradeInfo.TotalNum < terminal.CurUpgradeInfo.SendNo))
                        {
                            terminal.CurUpgradeInfo.Stage++;
                        }
                        SendUpgradeFile(terminal);
                    }
                    else
                    {
                        //ResendNo++;
                        //if (ResendNo == 5)//重试5次，仍然失败，则结束本次升级
                        //{
                        //    return;
                        //}
                    }
                    //Thread.Sleep(1000);
                });
        }

        private bool chkDevList()
        {
            bool result = true;

            int num = chkListDevAddr.Items.Count;
            int selNum = 0;
            int curIndex = 0;
            for(int i=0;i<num;i++)
            {
                if(chkListDevAddr.GetItemChecked(i))
                {
                    selNum++;
                    curIndex = i;
                }
            }
            if (selNum != 1)
            {
                MessageBox.Show("只能选择一个设备！", "提示");
                result = false;
            }
            else
            {
                string value = chkListDevAddr.Items[curIndex].ToString();
                int no = value.IndexOf('-');

                string addr = value.Substring(0,no-1).Trim();
                if (TerminalShare.GetShare.ClientList.ContainsKey(addr))
                {
                    TerminalShare.GetShare.CurClient = TerminalShare.GetShare.ClientList[addr] as TerminalClient;
                }
                else
                {
                    MessageBox.Show("设备未登录："+addr, "提示");
                    result = false;
                }
            }

            return result;
        }
    }
}
