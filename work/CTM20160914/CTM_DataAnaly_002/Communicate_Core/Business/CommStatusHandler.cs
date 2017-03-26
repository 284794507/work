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
    public class CommStatusHandler
    {
        private static CommStatusHandler _CommStatusHandler;
        public static CommStatusHandler Instance
        {
            get
            {
                if(_CommStatusHandler==null)
                {
                    _CommStatusHandler = new CommStatusHandler();
                }
                return _CommStatusHandler;
            }
        }

        public void SendQueryCommStatus(string[] addr)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                   .Log(Share.Instance.WriteLog, "", "Query Terminal CommStatus！")
                   .Do(() =>
                   {
                       foreach (string macAddr in addr)
                       {
                           byte[] devAddr = ByteHelper.HexStrToByteArrayWithDelimiter(macAddr, "-");
                           byte[] gprsAddr = Share.Instance.GetGPRSAddrByAddr(macAddr);
                           string key= ByteHelper.ByteToHexStrWithDelimiter(gprsAddr, "-");
                           if (Share.Instance.ClientList.ContainsKey(key))
                           {
                               DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                               Terminal_PackageData SendPkg = new Terminal_PackageData(gprsAddr, devAddr, TerminalCmdWordConst.QueryCommData, curClient.GetSendNo, TerminalCmdWordConst.QueryComm_FN);
                               Share.Instance.SendDataToTerminal(devAddr, SendPkg);
                           }
                               //byte seq = Share.Instance.GetNewSeq(0x80);
                               
                           //Share.Instance.SendOnlyDataToTerminal(SendPkg);
                       }
                   });
        }

        /// <summary>
        /// 通信状态
        /// </summary>
        /// <param name="package"></param>
        public void CommStatusPackageHandler(Terminal_PackageData package)
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

                        PLDevCommInfo info=UpdateCommStatus(package.OnlyData);                        

                        //上报与查询回复内容一致，共用解析方法，如果是上报则回复
                        if (ByteHelper.ByteArryEquals(package.CmdWord, TerminalCmdWordConst.UploadCommData))
                        {
                            UploadBack(curClient,package.DevAddr);
                        }
                        else
                        {
                            QueryBackToLmsSvr(info);
                        }
                    }
                });
        }

        private PLDevCommInfo UpdateCommStatus(byte[] data)
        {
            PLDevCommInfo info = new PLDevCommInfo();
            TPLCollectorPLCCommStatus_Cur plc = new TPLCollectorPLCCommStatus_Cur();
            TPLCollectorWireLessCommStatus_Cur wireLess = new TPLCollectorWireLessCommStatus_Cur();
            TPLCollectorMasterCommStatus_Cur masterComm = new TPLCollectorMasterCommStatus_Cur();
            int curIndex = 0;

            byte devType = data[curIndex++];
            byte[] bAddr = new byte[8];
            Buffer.BlockCopy(data, curIndex, bAddr, 0, 8);
            curIndex += 8;
            string macAddr = ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-");//设备地址
            if (Share.Instance.CheckDevExist(macAddr))
            {
                Guid devID = new Guid(Share.Instance.GetDevIDByAddr(macAddr));
                plc.PLCollectorInfoID = devID;
                wireLess.PLCollectorInfoID = devID;

                Buffer.BlockCopy(data, curIndex, bAddr, 0, 8);
                curIndex += 8;
                string plcMac = ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-");//PLC协调器地址
                string plcObjID = Share.Instance.GetDevIDByAddr(plcMac);
                Guid plcID = Guid.Empty;
                if (!string.IsNullOrEmpty(plcObjID))
                {
                    plcID = new Guid(plcObjID);
                }

                Buffer.BlockCopy(data, curIndex, bAddr, 0, 8);
                curIndex += 8;
                string loarMac = ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-");//LORA协调器地址
                //Guid loarID = new Guid(Share.Instance.GetDevIDByAddr(loarMac));
                string loarObjID = Share.Instance.GetDevIDByAddr(loarMac);
                Guid loarID = Guid.Empty;
                if (!string.IsNullOrEmpty(loarObjID))
                {
                    loarID = new Guid(loarObjID);
                }
                //byte[] bPlat = new byte[7];//台区
                //Buffer.BlockCopy(data, curIndex, bPlat, 0, 7);
                //curIndex += 7;
                //台区
                byte countryCode = data[curIndex++];
                int cityCode = BitConverter.ToInt16(data, curIndex);
                curIndex += 2;
                byte[] bArr = new byte[4];
                Buffer.BlockCopy(data, curIndex, bArr, 0, 3);
                curIndex += 3;
                int platFormCode = BitConverter.ToInt32(bArr, 0);
                string pFCode = platFormCode.ToString();
                Guid platID = Share.Instance.GetPlatID(countryCode.ToString(), cityCode.ToString(), plcMac,ref pFCode);
                //更新设备的所属PLC、台区信息
                
                DBHandler.Instance.UpdateCollectorUp(devID, plcID, platID, pFCode);

                byte phase = data[curIndex++];//相位

                byte bStatus = data[curIndex++];
                plc.PLCL1Status = (byte)(bStatus & 0x03);
                plc.PLCL2Status = (byte)((bStatus >> 2) & 0x03);
                plc.PLCL3Status = (byte)((bStatus >> 4) & 0x03);
                //if (devType == 0x11 || devType == 0x13)
                //{
                //    masterComm.CommStatus = (byte)((bStatus >> 6) & 0x03);
                //}
                //else
                //{
                    wireLess.WireLessStatus = (byte)((bStatus >> 6) & 0x03);
                //}

                plc.L1TotalCommTimes = BitConverter.ToInt16(data, curIndex);
                curIndex += 2;
                plc.L1SuccessfulCommTimes = BitConverter.ToInt16(data, curIndex);
                curIndex += 2;
                if (plc.L1TotalCommTimes == 0)
                {
                    plc.L1LostRate = 0;
                }
                else
                {
                    plc.L1LostRate = GetRate(plc.L1TotalCommTimes.Value, plc.L1SuccessfulCommTimes.Value);
                }
                uint milliSecond = BitConverter.ToUInt32(data, curIndex);
                curIndex += 4;
                DateTime curT = DateTime.Now;
                DateTime startTime = new DateTime(curT.Year, curT.Month, curT.Day, 0, 0, 0, 0);
                plc.ChkDataTime1 = startTime.AddMilliseconds(milliSecond);

                plc.L2TotalCommTimes = BitConverter.ToInt16(data, curIndex);
                curIndex += 2;
                plc.L2SuccessfulCommTimes = BitConverter.ToInt16(data, curIndex);
                curIndex += 2;
                if (plc.L2TotalCommTimes == 0)
                {
                    plc.L2LostRate = 0;
                }
                else
                {
                    plc.L2LostRate = GetRate(plc.L2TotalCommTimes.Value,plc.L2SuccessfulCommTimes.Value);
                }

                milliSecond = BitConverter.ToUInt32(data, curIndex);
                curIndex += 4;
                plc.ChkDataTime2 = startTime.AddMilliseconds(milliSecond);

                plc.L3TotalCommTimes = BitConverter.ToInt16(data, curIndex);
                curIndex += 2;
                plc.L3SuccessfulCommTimes = BitConverter.ToInt16(data, curIndex);
                curIndex += 2;
                if (plc.L3TotalCommTimes == 0)
                {
                    plc.L3LostRate = 0;
                }
                else
                {
                    plc.L3LostRate = GetRate(plc.L3TotalCommTimes.Value,plc.L3SuccessfulCommTimes.Value);
                }
                milliSecond = BitConverter.ToUInt32(data, curIndex);
                curIndex += 4;
                plc.ChkDataTime3 = startTime.AddMilliseconds(milliSecond);

                bool phaseVaild = ((phase >> 6) & 0x03) == 0 ? true : false;
                //if (phaseVaild)//有相位更新相位，无相位更新状态
                //{
                    string aPhase = "L" + (phase & 0x03).ToString();
                    string bPhase = "L" + ((byte)((phase >> 2) & 0x03)).ToString();
                    string cPhase = "L" + ((byte)((phase >> 4) & 0x03)).ToString();
                    DBHandler.Instance.UpdateCollectorPhase(devID,aPhase,bPhase,cPhase);

                    GetElecdataByPhase(aPhase, bPhase, cPhase, ref plc);//根据相位调整通信状态
                //}

                if (devType == 0x11 || devType == 0x13)
                {
                    masterComm.PLCollectorInfoID = wireLess.PLCollectorInfoID;
                    masterComm.CommStatus = wireLess.WireLessStatus;
                    masterComm.TotalCommTimes = BitConverter.ToInt16(data, curIndex);
                    curIndex += 2;
                    masterComm.SuccessfulCommTimes = BitConverter.ToInt16(data, curIndex);
                    if (Share.Instance.ClientList.ContainsKey(loarMac))
                    {
                        DevClient curClient = Share.Instance.ClientList[loarMac] as DevClient;
                        masterComm.SuccessfulCommTimes += curClient.LastReceviceNo;
                        curClient.ReceviceNo = (ushort)masterComm.SuccessfulCommTimes;
                        curIndex += 2;
                        if (masterComm.TotalCommTimes == 0)
                        {
                            masterComm.LostRate = 0;
                        }
                        else
                        {
                            masterComm.LostRate = GetRate(masterComm.TotalCommTimes.Value, masterComm.SuccessfulCommTimes.Value);
                        }
                        milliSecond = BitConverter.ToUInt32(data, curIndex);
                        curIndex += 4;
                        masterComm.ChkDataTime = startTime.AddMilliseconds(milliSecond);
                        DBHandler.Instance.UpdateGPRSStatus(masterComm);

                        wireLess.ChkDataTime = masterComm.ChkDataTime;//GPRS设备也有无线通信状态
                        DBHandler.Instance.UpdateWireLessStatus(wireLess);
                    }
                }
                else
                {
                    wireLess.TotalCommTimes = BitConverter.ToInt16(data, curIndex);
                    curIndex += 2;
                    wireLess.SuccessfulCommTimes = BitConverter.ToInt16(data, curIndex);
                    curIndex += 2;
                    if (wireLess.TotalCommTimes == 0)
                    {
                        wireLess.LostRate = 0;
                    }
                    else
                    {
                        wireLess.LostRate = GetRate(wireLess.TotalCommTimes.Value,wireLess.SuccessfulCommTimes.Value);
                    }
                    milliSecond = BitConverter.ToUInt32(data, curIndex);
                    curIndex += 4;
                    wireLess.ChkDataTime = startTime.AddMilliseconds(milliSecond);
                    DBHandler.Instance.UpdateWireLessStatus(wireLess);
                }

                DBHandler.Instance.UpdatePlcStatus(plc);

                info.DevID = plc.PLCollectorInfoID.ToString();
                info.DevType = Share.Instance.GetDevType(devType);
                info.DevMac = macAddr;
                info.DevPLCMac = plcMac;
                info.DevLORAMac = loarMac;
                info.PLInfo_Contry = countryCode.ToString();
                info.PLInfo_City = cityCode.ToString();
                info.PLInfo_Code = platFormCode.ToString();
                info.P1_CommStatus = plc.PLCL1Status;
                info.P1_CommTotolNum = plc.L1TotalCommTimes.Value;
                info.P1_CommSuccessNum = plc.L1SuccessfulCommTimes.Value;
                info.P1_CommTime = plc.ChkDataTime1.Value;
                info.P1_LostRate = (double)plc.L1LostRate;

                info.P2_CommStatus = plc.PLCL2Status;
                info.P2_CommTotolNum = plc.L2TotalCommTimes.Value;
                info.P2_CommSuccessNum = plc.L2SuccessfulCommTimes.Value;
                info.P2_CommTime = plc.ChkDataTime2.Value;
                info.P2_LostRate = (double)plc.L2LostRate;

                info.P3_CommStatus = plc.PLCL3Status;
                info.P3_CommTotolNum = plc.L3TotalCommTimes.Value;
                info.P3_CommSuccessNum = plc.L3SuccessfulCommTimes.Value;
                info.P3_CommTime = plc.ChkDataTime3.Value;
                info.P3_LostRate = (double)plc.L3LostRate;
                if (devType == 0x11 || devType == 0x13)
                {
                    info.LORA_CommStatus = masterComm.CommStatus;
                    info.LORA_CommTotolNum = masterComm.TotalCommTimes.Value;
                    info.LORA_CommSuccessNum = masterComm.SuccessfulCommTimes.Value;
                    info.LORA_CommTime = masterComm.ChkDataTime;
                    info.LORA_LostRate = (double)masterComm.LostRate;
                }
                else
                {
                    info.LORA_CommStatus = wireLess.WireLessStatus;
                    info.LORA_CommTotolNum = wireLess.TotalCommTimes.Value;
                    info.LORA_CommSuccessNum = wireLess.SuccessfulCommTimes.Value;
                    info.LORA_CommTime = wireLess.ChkDataTime;
                    info.LORA_LostRate = (double)wireLess.LostRate;

                }
            }
            else
            {
                string[] addr = new string[1];
                addr[0] = macAddr;
                TerminalInfoHandler.Instance.SendQueryTerminalInfo(addr);
            }

            return info;
        }

        private decimal GetRate(int totalNum,int successNum)
        {
            double? rate = (totalNum - successNum) / (totalNum * 1.0);
            if (rate < 0) rate = 0;
            if (rate > 1) rate = 1;
            return  new decimal(rate.Value);
        }

        /// <summary>
        /// 根据相位调整通信状态位置
        /// </summary>
        /// <param name="aPhase"></param>
        /// <param name="bPhase"></param>
        /// <param name="cPhase"></param>
        /// <param name="tempStatus"></param>
        /// <returns></returns>
        private void GetElecdataByPhase(string aPhase, string bPhase, string cPhase, ref TPLCollectorPLCCommStatus_Cur plc)
        {
            TPLCollectorPLCCommStatus_Cur result = new TPLCollectorPLCCommStatus_Cur();

            result.L1TotalCommTimes = plc.L1TotalCommTimes;
            result.L1SuccessfulCommTimes = plc.L1SuccessfulCommTimes;
            result.L1LostRate = plc.L1LostRate;
            result.ChkDataTime1 = plc.ChkDataTime1;

            result.L2TotalCommTimes = plc.L2TotalCommTimes;
            result.L2SuccessfulCommTimes = plc.L2SuccessfulCommTimes;
            result.L2LostRate = plc.L2LostRate;
            result.ChkDataTime2 = plc.ChkDataTime2;

            result.L3TotalCommTimes = plc.L3TotalCommTimes;
            result.L3SuccessfulCommTimes = plc.L3SuccessfulCommTimes;
            result.L3LostRate = plc.L3LostRate;
            result.ChkDataTime3 = plc.ChkDataTime3;

            if (aPhase == "L1")
            {
            }
            else if (bPhase == "L1")
            {
                plc.L1TotalCommTimes = result.L2TotalCommTimes;
                plc.L1SuccessfulCommTimes = result.L2SuccessfulCommTimes;
                plc.L1LostRate = result.L2LostRate;
                plc.ChkDataTime1 = result.ChkDataTime2;
            }
            else if (cPhase == "L1")
            {
                plc.L1TotalCommTimes = result.L3TotalCommTimes;
                plc.L1SuccessfulCommTimes = result.L3SuccessfulCommTimes;
                plc.L1LostRate = result.L3LostRate;
                plc.ChkDataTime1 = result.ChkDataTime3;
            }
            else if (aPhase == "L0")
            {
            }

            if (aPhase == "L2")
            {
                plc.L2TotalCommTimes = result.L1TotalCommTimes;
                plc.L2SuccessfulCommTimes = result.L1SuccessfulCommTimes;
                plc.L2LostRate = result.L1LostRate;
                plc.ChkDataTime2 = result.ChkDataTime1;
            }
            else if (bPhase == "L2")
            {
            }
            else if (cPhase == "L2")
            {
                plc.L2TotalCommTimes = result.L3TotalCommTimes;
                plc.L2SuccessfulCommTimes = result.L3SuccessfulCommTimes;
                plc.L2LostRate = result.L3LostRate;
                plc.ChkDataTime2 = result.ChkDataTime3;
            }
            else if (bPhase == "L0")
            {
            }

            if (aPhase == "L3")
            {
                plc.L3TotalCommTimes = result.L1TotalCommTimes;
                plc.L3SuccessfulCommTimes = result.L1SuccessfulCommTimes;
                plc.L3LostRate = result.L1LostRate;
                plc.ChkDataTime3 = result.ChkDataTime1;
            }
            else if (bPhase == "L3")
            {
                plc.L3TotalCommTimes = result.L2TotalCommTimes;
                plc.L3SuccessfulCommTimes = result.L2SuccessfulCommTimes;
                plc.L3LostRate = result.L2LostRate;
                plc.ChkDataTime3 = result.ChkDataTime2;
            }
            else if (cPhase == "L3")
            {
            }
            else if (cPhase == "L0")
            {
            }
        }

        /// <summary>
        /// 上报通信状态返回
        /// </summary>
        private void UploadBack(DevClient curClient,byte[] address)
        {
            Terminal_PackageData SendPkg = new Terminal_PackageData(address, new byte[] { 0x01 }, TerminalCmdWordConst.UploadCommDataBack, curClient.GetSendNo, TerminalCmdWordConst.UploadComm_FN);
            Share.Instance.SendOnlyDataToTerminal(SendPkg);
        }

        public void QueryBackToLmsSvr(PLDevCommInfo info)
        {
            PLDevCommInfo[] arr = new PLDevCommInfo[1];
            arr[0] = info;
            CommonBusinessBackResult optResult = new CommonBusinessBackResult();
            optResult.ExecuteResult = 1;
            optResult.BusinessType = CommonBusinessType.PLDevCommInfo_Query;
            optResult.BusinessObject = arr;
            SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
            //
            if (msg.ExcuResult)
            {
                Share.Instance.WriteLog(" 设备基本信息设置返回成功！");
            }
        }
    }
}
