using Communicate_Core.Model;
using Communicate_Core.PackageHandler;
using Communicate_Core.Utility;
using CTMDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Communicate_Core.Business
{
    public class SetRoute
    {
        private static SetRoute _SetRoute;
        public static SetRoute Instance
        {
            get
            {
                if(_SetRoute==null)
                {
                    _SetRoute = new SetRoute();
                }
                return _SetRoute;
            }
        }

        public void SetRoutePackageHandler(Terminal_PackageData package)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "Receivce Route！")
                .Do(() =>
                {
                    string key = ByteHelper.ByteToHexStrWithDelimiter(package.DevAddr, "-");
                    if (Share.Instance.ClientList.ContainsKey(key))
                    {
                        DevClient curClient = Share.Instance.ClientList[key] as DevClient;
                        curClient.HeartBeatTime = DateTime.Now;

                        byte[] data = new byte[8];
                        Buffer.BlockCopy(package.OnlyData, 0, data, 0, 8);
                        string desAddr = ByteHelper.ByteToHexStrWithDelimiter(data, "-", false);

                        Buffer.BlockCopy(package.OnlyData, 0, data, 8, 8);
                        string macAddr = ByteHelper.ByteToHexStrWithDelimiter(data, "-", false);


                        TPLCollectorStaticRoutes info = new TPLCollectorStaticRoutes();
                        info.DevID =new Guid(Share.Instance.GetDevIDByAddr(desAddr));
                        info.StaticRouteNode = new Guid(Share.Instance.GetDevIDByAddr(macAddr));

                        DBHandler.Instance.AddRoute(info);

                        Terminal_PackageData SendPkg = new Terminal_PackageData(package.DevAddr, new byte[] { 0x01 }, TerminalCmdWordConst.SetRouteBack, curClient.GetSendNo, TerminalCmdWordConst.SetRoute_FN);
                        Share.Instance.SendOnlyDataToTerminal(SendPkg);
                    }
                });
        }

        /// <summary>
        /// 监测设置路由
        /// </summary>
        public void MonitorSetRoute()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        TPLCollectorStaticRoutes[] routeList = DBHandler.Instance.GetRouteList();
                        if (routeList == null || routeList.Count() == 0) return;

                        Dictionary<Guid, int> dictDevLvl = new Dictionary<Guid, int>();
                        Dictionary<int, int> dictAllLvl = new Dictionary<int, int>();
                        Dictionary<Guid, Guid> dictAllRoute = new Dictionary<Guid, Guid>();
                        foreach (TPLCollectorStaticRoutes info in routeList)
                        {
                            Guid devID = info.DevID;
                            int num = 0;
                            GetDevLvl(routeList, info.StaticRouteNode, ref num);
                            if (!dictDevLvl.ContainsKey(devID))
                            {
                                dictDevLvl.Add(devID, num);
                                dictAllRoute.Add(devID, info.StaticRouteNode);
                            }
                            if (!dictAllLvl.ContainsKey(num))
                            {
                                dictAllLvl.Add(num, num);
                            }

                        }
                        foreach (KeyValuePair<int, int> kvp in dictAllLvl.OrderBy(p => p.Key))
                        {
                            foreach (KeyValuePair<Guid, int> sKvp in dictDevLvl)
                            {
                                if (sKvp.Value == kvp.Key)
                                {
                                    //bool isOk = GetRouteByID(routeList, sKvp.Key);
                                    //if(isOk)
                                    //{
                                    if (dictAllRoute.ContainsKey(sKvp.Key))
                                    {
                                        string desID = sKvp.Key.ToString();
                                        string desAddr = Share.Instance.GetAddrByDevID(desID);
                                        byte[] bDesAddr = ByteHelper.HexStrToByteArrayWithDelimiter(desAddr, "-");

                                        string routeID = dictAllRoute[sKvp.Key].ToString();
                                        string macAddr = Share.Instance.GetAddrByDevID(routeID);
                                        byte[] bMacAddr = ByteHelper.HexStrToByteArrayWithDelimiter(macAddr, "-");

                                        byte[] gprsAddr = Share.Instance.GetGPRSAddrByAddr(macAddr);
                                        string key = ByteHelper.ByteToHexStrWithDelimiter(gprsAddr, "-");
                                        if (Share.Instance.ClientList.ContainsKey(key))
                                        {
                                            DevClient curClient = Share.Instance.ClientList[key] as DevClient;

                                            byte[] data = new byte[16];
                                            Buffer.BlockCopy(bDesAddr, 0, data, 0, 8);
                                            Buffer.BlockCopy(bMacAddr, 0, data, 8, 8);
                                            Terminal_PackageData SendPkg = new Terminal_PackageData(gprsAddr, data, TerminalCmdWordConst.SetRoute, curClient.GetSendNo, TerminalCmdWordConst.SetRoute_FN);
                                            Share.Instance.SendDataToTerminal(bDesAddr, SendPkg);
                                        }
                                        //byte seq = Share.Instance.GetNewSeq(0x80);
                                    }
                                    //}
                                }
                            }
                        }
                    }
                    );
            }
            catch (Exception ex)
            {
                Share.Instance.WriteLog(ex.Message, 4);
                MonitorSetRoute();
            }
        }

        public void GetDevLvl(TPLCollectorStaticRoutes[] routeList,Guid devID,ref int num)
        {
            int no = 0;
            int len = routeList.Length;
            Guid UpID = new Guid();
            foreach (TPLCollectorStaticRoutes info2 in routeList)
            {
                if (info2.DevID == devID)
                {
                    num++;
                    UpID = info2.StaticRouteNode;
                    break;
                }
                else
                {
                    no++;
                }
            }
            if(no!=len)
            {
                GetDevLvl(routeList, UpID, ref num);
            }
        }

        public bool GetRouteByID(TPLCollectorStaticRoutes[] routeList, Guid devID)
        {
            bool result = false;
            Guid routeID = new Guid();
            foreach(TPLCollectorStaticRoutes info in routeList)
            {
                if(info.DevID==devID)
                {
                    string macAddr = Share.Instance.GetAddrByDevID(info.StaticRouteNode.ToString());
                    routeID = info.StaticRouteNode;
                    if (Share.Instance.dictIsUpload.ContainsKey(macAddr))
                    {
                        result = true;
                        break;
                    }
                }
            }
            if(!result)
            {
                result = GetRouteByID(routeList, routeID);
            }

            return result;
        }
    }
}
