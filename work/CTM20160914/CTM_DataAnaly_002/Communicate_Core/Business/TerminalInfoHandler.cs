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
    public class TerminalInfoHandler
    {
        private static TerminalInfoHandler _TerminalInfoHandler;
        public static TerminalInfoHandler Instance
        {
            get
            {
                if(_TerminalInfoHandler==null)
                {
                    _TerminalInfoHandler = new TerminalInfoHandler();
                }
                return _TerminalInfoHandler;
            }
        }

        public void SendQueryTerminalInfo(string[] addr)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                   .Log(Share.Instance.WriteLog, "", "Query Terminal Info！")
                   .Do(() =>
                   {
                       foreach (string macAddr in addr)
                       {
                           byte[] devAddr = ByteHelper.HexStrToByteArrayWithDelimiter(macAddr, "-");
                           byte[] gprsAddr = Share.Instance.GetGPRSAddrByAddr(macAddr);
                           string key = ByteHelper.ByteToHexStrWithDelimiter(gprsAddr, "-");
                           if (Share.Instance.ClientList.ContainsKey(key))
                           {
                               DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                               Terminal_PackageData SendPkg = new Terminal_PackageData(gprsAddr, devAddr, TerminalCmdWordConst.QueryTerminalBasicInfo, curClient.GetSendNo, TerminalCmdWordConst.QueryTerminalInfo_FN);
                               Share.Instance.SendDataToTerminal(devAddr, SendPkg);
                           }
                           else
                           {
                               Share.Instance.GPRSOffLine(key, CommonBusinessType.PLDevInfo_Query);
                           }
                           //byte seq = Share.Instance.GetNewSeq(0x80);
                       }
                   });
        }

        public void TerminalInfoPackageHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "Recevice Terminal Info！")
                .Do(() =>
                {
                    string key = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, "-");
                    if (Share.Instance.ClientList.ContainsKey(key))
                    {
                        DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                        curClient.HeartBeatTime = DateTime.Now;

                        int curIndex = 0;
                        TPLCollectorInfo collector = new TPLCollectorInfo();

                        byte devType = package.OnlyData[curIndex++];
                        collector.DevType = Share.Instance.GetDevType(devType);
                        byte[] bAddr = new byte[8];
                        Buffer.BlockCopy(package.OnlyData, curIndex, bAddr, 0, 8);
                        curIndex += 8;
                        collector.MacAddr = ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-");
                        string id = Share.Instance.GetDevIDByAddr(collector.MacAddr);
                        if(string.IsNullOrEmpty(id))
                        {
                            collector.ObjID = Guid.NewGuid();
                        }
                        else
                        {
                            collector.ObjID = new Guid(id);
                        }
                        collector.DevStatus = (byte)CollectorStatus.OnLine;
                        collector.SNCode = Encoding.UTF8.GetString(package.OnlyData, curIndex, 15);
                        curIndex += 15;

                        curIndex += 9;//时间 不解析
                        collector.ChannelNo = package.OnlyData[curIndex++];
                        int pcbNo = BitConverter.ToInt16(package.OnlyData, curIndex);
                        curIndex += 2;
                        string bVersion = Encoding.UTF8.GetString(package.OnlyData, curIndex++, 1);
                        byte bSubVersion = package.OnlyData[curIndex++];
                        collector.HVer = pcbNo.ToString() + "-" + bVersion + "-" + bSubVersion.ToString();
                        collector.SVer = ByteHelper.GetVersion(package.OnlyData, curIndex);
                        curIndex += 2;
                        //if(package.OnlyData.Length-curIndex>=8)
                        //{
                        //    bAddr = new byte[8];
                        //    Buffer.BlockCopy(package.OnlyData, curIndex, bAddr, 0, 8);
                        //    curIndex += 8;
                        //    string routeMac = ByteHelper.ByteToHexStrWithDelimiter(bAddr, "-");
                        //    string routeID = Share.Instance.GetDevIDByAddr(routeMac);
                        //    TPLCollectorStaticRoutes route = new TPLCollectorStaticRoutes();
                        //    route.DevID = new Guid(id);
                        //    route.StaticRouteNode = new Guid(routeID);
                        //    DBHandler.Instance.AddRoute(route);
                        //}
                        
                        Guid loarID = new Guid(Share.Instance.GetDevIDByAddr(key));
                        collector.GprsID = loarID;

                        if (Share.Instance.dictIsUpload.ContainsKey(collector.MacAddr))
                        {
                            Share.Instance.dictIsUpload[collector.MacAddr] = true;
                        }
                        else
                        {
                            Share.Instance.dictIsUpload.Add(collector.MacAddr, true);
                        }
                        DBHandler.Instance.UpdateCollector(collector);

                        //上报与查询回复内容一致，共用解析方法，如果是上报则回复,查询则回复给平台
                        if (ByteHelper.ByteArryEquals(package.CmdWord,TerminalCmdWordConst.UploadTerminalBasicInfo))
                        {
                            UploadBack(curClient, package.DevAddr);
                        }
                        else
                        {
                            QueryBackToLmsSvr(collector);
                        }
                    }
                });
        }
                
        /// <summary>
        /// 上报设备信息返回
        /// </summary>
        private void UploadBack(DevClient curClient,byte[] address)
        {
            Terminal_PackageData SendPkg = new Terminal_PackageData(address, new byte[] { 0x01 }, TerminalCmdWordConst.UploadTerminalBasicInfoBack, curClient.GetSendNo, TerminalCmdWordConst.UploadTerminalInfo_FN);
            Share.Instance.SendOnlyDataToTerminal(SendPkg);
        }

        public void QueryBackToLmsSvr(TPLCollectorInfo collector)
        {
            PLDevInfo info = new PLDevInfo();
            info.DevID = collector.ObjID.ToString();
            info.DevMac = collector.MacAddr;
            info.DevType = collector.DevType;
            info.DevSN = collector.SNCode;
            info.GetDataTime = DateTime.Now;
            info.DevHardWareVer = collector.HVer;
            info.DevSoftWareVer = collector.SVer;

            PLDevInfo[] arr = new PLDevInfo[1];
            arr[0] = info;
            CommonBusinessBackResult optResult = new CommonBusinessBackResult();
            optResult.ExecuteResult = 1;
            optResult.BusinessType = CommonBusinessType.PLDevInfo_Query;            
            optResult.BusinessObject = arr;
            SvrRetMessage msg = Share.Instance.proxy.CommonBusinessDeviceBack(Share.Instance.MacNameToLmsSvr, optResult);
            //
            if (msg.ExcuResult)
            {
                Share.Instance.WriteLog(" 设备基本信息查询返回成功！");
            }
        }
    }
}
