using Communicate_Core.Model;
using Communicate_Core.PackageHandler;
using Communicate_Core.Utility;
using CTMDAL.Model;
using DL_LMS_Server.Default.Shared;
using DL_LMS_Server.Service.DataModel.Result;
using DL_LMS_Server.Service.DataModel.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.Business
{
    public class ElecDataHandler
    {
        private static ElecDataHandler _ElecDataHandler;
        public static ElecDataHandler Instance
        {
            get
            {
                if(_ElecDataHandler==null)
                {
                    _ElecDataHandler = new ElecDataHandler();
                }
                return _ElecDataHandler;
            }
        }

        public void SendQueryElecDataPackage(string addr)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                   .Log(Share.Instance.WriteLog, "", "Query ElecData Info！")
                   .Do(() =>
                   {
                       byte[] devAddr = ByteHelper.HexStrToByteArrayWithDelimiter(addr, "-");
                       byte[] gprsAddr = Share.Instance.GetGPRSAddrByAddr(addr);
                       string key = ByteHelper.ByteToHexStrWithDelimiter(gprsAddr, "-");
                       if (Share.Instance.ClientList.ContainsKey(key))
                       {
                           DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                           Terminal_PackageData SendPkg = new Terminal_PackageData(gprsAddr, devAddr, TerminalCmdWordConst.QueryData, curClient.GetSendNo, TerminalCmdWordConst.QueryHisData_FN);
                           Share.Instance.SendDataToTerminal(devAddr, SendPkg);
                       }
                       else
                       {
                           Share.Instance.GPRSOffLine(key, CommonBusinessType.PLDevElecQuery);
                       }
                   });
        }

        /// <summary>
        /// 电参数
        /// </summary>
        /// <param name="package"></param>
        public void ElecDataPackageHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "Recevice ElecData！")
                .Do(() =>
                {
                    string key = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, "-");
                    if (Share.Instance.ClientList.ContainsKey(key))
                    {
                        DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                        curClient.HeartBeatTime = DateTime.Now;
                        TimeSpan ts = DateTime.Now - curClient.batchTime;
                        if(ts.Minutes>1)
                        {
                            curClient.batchID = Guid.NewGuid();
                            curClient.batchTime = DateTime.Now;
                        }

                        TPLCollectorInfo collector = new TPLCollectorInfo();
                        TPLDataRecRTM elecData = GetElecData(package.OnlyData);
                        if(elecData==null)
                        {//电参数解析失败！
                            Share.Instance.WriteLog("电参数解析失败！",3);
                            return;
                        }

                        int curIndex = 0;
                        byte devType = package.OnlyData[curIndex++];                        
                        byte[] bAddr = new byte[8];
                        Buffer.BlockCopy(package.OnlyData, curIndex, bAddr, 0, 8);
                        curIndex += 8;
                        collector.MacAddr = ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-");

                        if (Share.Instance.CheckDevExist(collector.MacAddr))
                        {
                            byte phase = package.OnlyData[curIndex++];
                            bool phaseVaild = ((phase >> 6) & 0x03) == 1 ? true : false;
                            //if (phaseVaild)
                            //{
                                collector.APhase = "L" + (phase & 0x03).ToString();
                                collector.BPhase = "L" + ((byte)((phase >> 2) & 0x03)).ToString();
                                collector.CPhase = "L" + ((byte)((phase >> 4) & 0x03)).ToString();
                            //}
                            collector.DevStatus = elecData.DevStatus.Value;
                            //if (phaseVaild)//有相位更新相位，无相位更新状态,
                            //{
                                //根据相位调整电参数
                                GetElecdataByPhase(collector.APhase, collector.BPhase, collector.CPhase, ref elecData);
                            //}
                            //else 
                            if (collector.DevStatus == (byte)CollectorStatus.PowerOff)
                            {
                                DBHandler.Instance.UpdateCollectorStatus(collector);
                            }

                            TimeSpan tsElec = DateTime.Now - Share.Instance.LastElecDataTime;
                            if (tsElec.Minutes > 1)
                            {
                                Share.Instance.BatchID = Guid.NewGuid();
                                Share.Instance.LastElecDataTime = DateTime.Now;
                            }
                            elecData.BatchID = Share.Instance.BatchID;
                            if(elecData.DevStatus != (byte)CollectorStatus.PowerOff)//如果通信中断则不更新电参数数据
                            {
                                DBHandler.Instance.UpdateElecData(elecData);
                            }

                            //上报与查询回复内容一致，共用解析方法，如果是上报则回复
                            if (ByteHelper.ByteArryEquals(package.CmdWord, TerminalCmdWordConst.UploadData))
                            {
                                UploadBack(curClient,package.DevAddr);
                            }
                            else
                            {
                                QueryBackToLmsSvr(collector, elecData);
                            }
                        }
                        else//如果以上的设备不存在，则查询
                        {
                            string[] addr = new string[1];
                            addr[0] = collector.MacAddr;
                            TerminalInfoHandler.Instance.SendQueryTerminalInfo(addr);
                        }
                    }
                });
        }

        private TPLDataRecRTM GetElecData(byte[] data)
        {
            TPLDataRecRTM result = new TPLDataRecRTM();
            int curIndex = 0;
            byte devType = data[curIndex++];
            //collector.DevType = Share.Instance.GetDevType(devType);
            byte[] bAddr = new byte[8];
            Buffer.BlockCopy(data, curIndex, bAddr, 0, 8);
            curIndex += 8;
            string macAddr = ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-");
            byte phase = data[curIndex++];
            uint milliSecond = BitConverter.ToUInt32(data, curIndex);
            curIndex += 4;
            DateTime curT = DateTime.Now;
            DateTime startTime = new DateTime(curT.Year, curT.Month, curT.Day, 0, 0, 0, 0);
            result.GetDataTime = startTime.AddMilliseconds(milliSecond);
            result.DevStatus = (byte)CollectorStatus.OnLine;
            string id = Share.Instance.GetDevIDByAddr(macAddr);
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            else
            {
                result.DevID = new Guid(id);
            }
            result.LampVoltageA = new decimal(BitConverter.ToUInt16(data, curIndex) / 100.0);
            result.LampCurrentA = new decimal(BitConverter.ToUInt16(data, curIndex + 2) / 100.0);

            result.LampVoltageB = new decimal(BitConverter.ToUInt16(data, curIndex + 4) / 100.0);
            result.LampCurrentB = new decimal(BitConverter.ToUInt16(data, curIndex + 6) / 100.0);

            result.LampVoltageC = new decimal(BitConverter.ToUInt16(data, curIndex + 8) / 100.0);
            result.LampCurrentC = new decimal(BitConverter.ToUInt16(data, curIndex + 10) / 100.0);

            if (result.LampVoltageA == 0 && result.LampCurrentA == 0 && result.LampVoltageB == 0 && result.LampCurrentB == 0
                            && result.LampVoltageC == 0 && result.LampCurrentC == 0)
            {
                result.DevStatus = (byte)CollectorStatus.PowerOff;
            }
            else if (result.LampVoltageA.ToString() == "655.35" && result.LampCurrentA.ToString() == "655.35" && result.LampVoltageB.ToString() == "655.35"
                             && result.LampCurrentB.ToString() == "655.35" && result.LampVoltageC.ToString() == "655.35" && result.LampCurrentC.ToString() == "655.35")
            {
                result.DevStatus = (byte)CollectorStatus.PowerOff;
                UpdateGprsStatus(macAddr, 0);//电参数FF表示无线通信中断
            }

            return result;
        }
        
        private void UpdateGprsStatus(string addr, byte status)
        {
            string id = Share.Instance.GetDevIDByAddr(addr);
            TPLCollectorWireLessCommStatus_Cur wireLess = new TPLCollectorWireLessCommStatus_Cur();
            wireLess.PLCollectorInfoID = new Guid(id);
            wireLess.ChkDataTime = DateTime.Now;
            wireLess.UpdateTime = DateTime.Now;
            wireLess.WireLessStatus = status;

            DBHandler.Instance.UpdateCollectorWireLess(wireLess);
        }

        /// <summary>
        /// 根据相位调整电参数的位置
        /// </summary>
        /// <param name="aPhase"></param>
        /// <param name="bPhase"></param>
        /// <param name="cPhase"></param>
        /// <param name="tempStatus"></param>
        /// <returns></returns>
        private void GetElecdataByPhase(string aPhase, string bPhase, string cPhase, ref TPLDataRecRTM elecData)
        {
            TPLDataRecRTM result = new TPLDataRecRTM();

            result.LampVoltageA = elecData.LampVoltageA;
            result.LampCurrentA = elecData.LampCurrentA;
            result.LampVoltageB = elecData.LampVoltageB;
            result.LampCurrentB = elecData.LampCurrentB;
            result.LampVoltageC = elecData.LampVoltageC;
            result.LampCurrentC = elecData.LampCurrentC;

            elecData.LampVoltageA = 0;
            elecData.LampCurrentA = 0;
            elecData.LampVoltageB = 0;
            elecData.LampCurrentB = 0;
            elecData.LampVoltageC = 0;
            elecData.LampCurrentC = 0;

            if (aPhase == "L1")
            {
                elecData.LampVoltageA = result.LampVoltageA;
                elecData.LampCurrentA = result.LampCurrentA;
            }
            else if (bPhase == "L1")
            {
                elecData.LampVoltageA = result.LampVoltageB;
                elecData.LampCurrentA = result.LampCurrentB;
            }
            else if (cPhase == "L1")
            {
                elecData.LampVoltageA = result.LampVoltageC;
                elecData.LampCurrentA = result.LampCurrentC;
            }
            else if (aPhase == "L0")
            {
                elecData.LampVoltageA = result.LampVoltageA;
                elecData.LampCurrentA = result.LampCurrentA;
            }

            if (aPhase == "L2")
            {
                elecData.LampVoltageB = result.LampVoltageA;
                elecData.LampCurrentB = result.LampCurrentA;
            }
            else if (bPhase == "L2")
            {
                elecData.LampVoltageB = result.LampVoltageB;
                elecData.LampCurrentB = result.LampCurrentB;
            }
            else if (cPhase == "L2")
            {
                elecData.LampVoltageB = result.LampVoltageC;
                elecData.LampCurrentB = result.LampCurrentC;
            }
            else if (bPhase == "L0")
            {
                elecData.LampVoltageB = result.LampVoltageB;
                elecData.LampCurrentB = result.LampCurrentB;
            }

            if (aPhase == "L3")
            {
                elecData.LampVoltageC = result.LampVoltageA;
                elecData.LampCurrentC = result.LampCurrentA;
            }
            else if (bPhase == "L3")
            {
                elecData.LampVoltageC = result.LampVoltageB;
                elecData.LampCurrentC = result.LampCurrentB;
            }
            else if (cPhase == "L3")
            {
                elecData.LampVoltageC = result.LampVoltageC;
                elecData.LampCurrentC = result.LampCurrentC;
            }
            else if (cPhase == "L0")
            {
                elecData.LampVoltageC = result.LampVoltageC;
                elecData.LampCurrentC = result.LampCurrentC;
            }
        }

        /// <summary>
        /// 上报设备电参数返回
        /// </summary>
        private void UploadBack(DevClient curClient,byte[] address)
        {
            Terminal_PackageData SendPkg = new Terminal_PackageData(address, new byte[] { 0x01 }, TerminalCmdWordConst.UploadDataBack, curClient.GetSendNo, TerminalCmdWordConst.UploadHisData_FN);
            Share.Instance.SendOnlyDataToTerminal(SendPkg);
        }

        public void QueryBackToLmsSvr(TPLCollectorInfo collector, TPLDataRecRTM elecData)
        {
            List<PLDevElecStatus> list = new List<PLDevElecStatus>();
            PLDevElecStatus status = new PLDevElecStatus();
            status.DevID = elecData.DevID.ToString() ;
            status.DevMac = collector.MacAddr;
            //具体电参数等等
            status.CurrentA = elecData.LampCurrentA.ToString();
            status.VoltageA = elecData.LampVoltageA.ToString();
            status.PowerFactA = elecData.LampPowerFactA.ToString();
            status.ActivePowerA = elecData.LampActivePowerA.ToString();

            status.CurrentB = elecData.LampCurrentB.ToString();
            status.VoltageB = elecData.LampVoltageB.ToString();
            status.PowerFactB = elecData.LampPowerFactB.ToString();
            status.ActivePowerB = elecData.LampActivePowerB.ToString();

            status.CurrentC = elecData.LampCurrentC.ToString();
            status.VoltageC = elecData.LampVoltageC.ToString();
            status.PowerFactC = elecData.LampPowerFactC.ToString();
            status.ActivePowerC = elecData.LampActivePowerC.ToString();

            status.GetDataTime = elecData.GetDataTime;
            status.UpdateTime = DateTime.Now;
            list.Add(status);
            
            CommonBusinessBackResult optResult = new CommonBusinessBackResult();
            optResult.ExecuteResult = 1;
            optResult.BusinessType = CommonBusinessType.PLDevElecQuery;
            optResult.BusinessObject = list.ToArray();
            SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
            //
            if (msg.ExcuResult)
            {
                Share.Instance.WriteLog(" 设备电参数查询返回成功！");
            }
        }
    }
}
