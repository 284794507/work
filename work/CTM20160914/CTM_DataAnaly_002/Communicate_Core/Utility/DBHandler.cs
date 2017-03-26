using Communicate_Core.PackageHandler;
using CTMDAL.Model;
using CTMDAL.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Communicate_Core.Utility
{
    public class DBHandler
    {
        private static DBHandler _DBHandler;
        public static DBHandler Instance
        {
            get
            {
                if(_DBHandler==null)
                {
                    _DBHandler = new DBHandler();
                }
                return _DBHandler;
            }
        }


        private static ICTMDal DalProxy;

        public static ICTMDal GetProxy
        {
            get
            {
                if (DalProxy == null)
                {
                    CreateChannel();
                }
                return DalProxy;
            }
        }

        private static void CreateChannel()
        {
            string dbEndPointName = ConfigurationManager.AppSettings["DBEndPoint"];
            ChannelFactory<ICTMDal> channelFactory = new ChannelFactory<ICTMDal>(dbEndPointName);
            DalProxy = channelFactory.CreateChannel();
        }

        private void MonitorChannel()
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler,()=> { CreateChannel(); })
                .Log(Share.Instance.WriteLog, "MonitorChannel", "")
                .Do(() =>
                {
                    if (((IClientChannel)DalProxy).State == CommunicationState.Closed)
                    {
                        DalProxy = null;
                    }
                });
        }

        public void InitData()
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler, () => { CreateChannel(); })
                .Log(Share.Instance.WriteLog, "InitData", "")
                .Do(() =>
                {
                    GetPlatList();
                    GetCollectorList();
                    GetSupplyInfo();

                    if (Share.Instance.dictIsUpload == null) Share.Instance.dictIsUpload = new Dictionary<string, bool>();
                    foreach (TPLCollectorInfo info in Share.Instance.listCollector)
                    {
                        if (!Share.Instance.dictIsUpload.ContainsKey(info.MacAddr))
                        {
                            Share.Instance.dictIsUpload.Add(info.MacAddr, false);
                        }
                    }
                });
        }

        public void GetPlatList()
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog,"","加载台区数据成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.GetPlatFormInfo;
                    CTMDalParameter para = GetProxy.GetInfo(sendPara);
                    Share.Instance.ListPlat = (para.BusinessObject as TPlatFormInfo[]).ToList<TPlatFormInfo>();
                });
        }

        public void GetCollectorList()
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "加载节点数据成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.GetCollectorInfo;
                    CTMDalParameter para = GetProxy.GetInfo(sendPara);
                    Share.Instance.listCollector = (para.BusinessObject as TPLCollectorInfo[]).ToList<TPLCollectorInfo>();
                });
        }

        /// <summary>
        /// 获取当前升级状态
        /// </summary>
        /// <param name="devID"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int GetUpgradeStatus(string devID,byte[]data)
        {
            int result = 0;
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "获取设备升级状态成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.GetUpgradeStatus;
                    sendPara.TerminalID = devID;
                    sendPara.BusinessObject = data;
                    CTMDalParameter para = GetProxy.GetInfo(sendPara);
                    result = int.Parse(para.BusinessObject.ToString());
                });
            return result;
        }

        /// <summary>
        /// 获取指定升级包的内容
        /// </summary>
        /// <param name="devID"></param>
        /// <param name="bagNo"></param>
        /// <returns></returns>
        public TPLUpgradeFileInfoDetail GetNextUpgradeData(string devID,int bagNo)
        {
            TPLUpgradeFileInfoDetail detail = new TPLUpgradeFileInfoDetail();
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "加载节点数据成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.GetNextUpgradeInfoByID;
                    sendPara.TerminalID = devID;
                    sendPara.BusinessObject = bagNo;
                    CTMDalParameter para = GetProxy.GetInfo(sendPara);
                    detail = para.BusinessObject as TPLUpgradeFileInfoDetail;
                });
            return detail;
        }

        public string [] GetRouteByAddr(byte[] addr)
        {
            string[] result = new string[0];
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "获取节点路由成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.GetRouteByAddr;
                    sendPara.BusinessObject = ByteHelper.ByteToHexStrWithDelimiter(addr,"-");
                    CTMDalParameter para = GetProxy.GetInfo(sendPara);
                    result = para.BusinessObject as string[];
                });
            return result;
        }

        public TPLCollectorStaticRoutes[] GetRouteList()
        {
            TPLCollectorStaticRoutes[] result = new TPLCollectorStaticRoutes[0];
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "获取所有路由成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.GetRouteList;
                    CTMDalParameter para = GetProxy.GetInfo(sendPara);
                    result = para.BusinessObject as TPLCollectorStaticRoutes[];
                });
            return result;
        }

        public void  GetSupplyInfo()
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "获取进户点信息成功！")
                .Do(() =>
                {
                    TPLBasicInfo[] result = new TPLBasicInfo[0];

                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.GetSupplyInfo;
                    CTMDalParameter para = GetProxy.GetInfo(sendPara);
                    Share.Instance.listSupply = (para.BusinessObject as TPLBasicInfo[]).ToList<TPLBasicInfo>();                    
                });
        }

        /// <summary>
        /// 插入节点信息，带经纬度，由APP上报
        /// </summary>
        /// <param name="info"></param>
        public void InsertCollectorInfo(TPLCollectorInfo info)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "插入设备信息成功！")
                .Do(() =>
                {
                    //更新本地设备信息缓存
                    Share.Instance.UpdateCollectorInfo_Local(info);

                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.InsertCollectorInfo;
                    sendPara.BusinessObject = info;
                    GetProxy.AddInfo(sendPara);
                });
        }

        /// <summary>
        /// 更新节点信息，没有则插入
        /// </summary>
        /// <param name="collector"></param>
        public void UpdateCollector(TPLCollectorInfo collector)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "更新设备信息成功！")
                .Do(() =>
                {
                    //更新本地设备信息缓存
                    Share.Instance.UpdateCollectorInfo_Local(collector);

                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateCollectorInfo;
                    sendPara.BusinessObject = collector;
                    GetProxy.UpdateInfo(sendPara);
                });
        }

        /// <summary>
        /// 更新节点所属plc,台区信息
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="devID"></param>
        /// <param name="plcID"></param>
        /// <param name="platID"></param>
        /// <param name="platCode"></param>
        public void UpdateCollectorUp(Guid devID,Guid plcID, Guid platID, string platCode)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "更新设备所属plc,台区信息成功！")
                .Do(() =>
                {
                    //更新本地设备信息缓存
                    TPLCollectorInfo newCollector = Share.Instance.UpdateCollectorUp(devID, plcID, platID, platCode);

                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateCollectorUp;
                    sendPara.BusinessObject = newCollector;
                    GetProxy.UpdateInfo(sendPara);
                });
        }

        /// <summary>
        /// 更新节点所属gprs
        /// </summary>
        /// <param name="devID"></param>
        /// <param name="gprsID"></param>
        public void UpdateCollectorGPRS(Guid devID, Guid gprsID)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "更新设备所属gprs信息成功！")
                .Do(() =>
                {
                    //更新本地设备信息缓存
                    TPLCollectorInfo newCollector = Share.Instance.UpdateCollectorGPRS(devID, gprsID);

                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateCollectorUp;
                    sendPara.BusinessObject = newCollector;
                    GetProxy.UpdateInfo(sendPara);
                });
        }

        /// <summary>
        /// 更改GPRS通信状态
        /// </summary>
        /// <param name="masterComm"></param>
        public void UpdateGPRSCommStatus(TPLCollectorMasterCommStatus_Cur masterComm)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "更新GPRS通信状态成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateCollectorMasterComm;
                    sendPara.BusinessObject = masterComm;
                    GetProxy.UpdateInfo(sendPara);                    
                });
        }

        public void UpdateCollectorStatus(TPLCollectorInfo collector)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "更新设备状态成功！")
                .Do(() =>
                {                    
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateCollectorStatus;
                    sendPara.BusinessObject = collector;
                    GetProxy.UpdateInfo(sendPara);

                    Share.Instance.UpdateCollectorStatus_Local(collector);
                });
        }

        public void UpdateCollectorPhase(Guid devID, string aPhase, string bPhase, string cPhase)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "更新设备相位成功！")
                .Do(() =>
                {
                    //更新本地设备信息缓存
                    TPLCollectorInfo collector = Share.Instance.UpdateCollectorPhase(devID, aPhase, bPhase, cPhase);
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateCollectorPhase;
                    sendPara.BusinessObject = collector;
                    GetProxy.UpdateInfo(sendPara);

                    Share.Instance.UpdateCollectorStatus_Local(collector);
                });
        }

        public void UpdateElecData(TPLDataRecRTM elecData)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "更新设备电参数成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateDataRecRTM;
                    sendPara.BusinessObject = elecData;
                    GetProxy.UpdateInfo(sendPara);
                });
        }

        /// <summary>
        /// 更改GPRS通信状态、时间、次数
        /// </summary>
        /// <param name="masterComm"></param>
        public void UpdateGPRSStatus(TPLCollectorMasterCommStatus_Cur masterComm)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "更新GPRS通信状态成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateCollectorMasterCommStatus;
                    sendPara.BusinessObject = masterComm;
                    GetProxy.UpdateInfo(sendPara);
                });
        }

        /// <summary>
        /// 更改无线通信状态、时间、次数
        /// </summary>
        /// <param name="wireLess"></param>
        public void UpdateWireLessStatus(TPLCollectorWireLessCommStatus_Cur wireLess)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "更新设备无线通信状态成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateCollectorWireLessStatus;
                    sendPara.BusinessObject = wireLess;
                    GetProxy.UpdateInfo(sendPara);
                });
        }

        /// <summary>
        /// 更改PLC通信状态、时间、次数
        /// </summary>
        /// <param name="plc"></param>
        public void UpdatePlcStatus(TPLCollectorPLCCommStatus_Cur plc)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "更新设备PLC通信状态成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateCollectorPLCCommStatus;
                    sendPara.BusinessObject = plc;
                    GetProxy.UpdateInfo(sendPara);
                });
        }

        public void AddUpgradeFile(TPLUpgradeFileInfo file)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "添加升级文件成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.AddUpgradeFile;
                    sendPara.BusinessObject = file;
                    GetProxy.AddInfo(sendPara);
                });
        }

        public void AddNewPlatForm(TPlatFormInfo info)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "添加新台区成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.InsertIntoPlatForm;
                    sendPara.BusinessObject = info;
                    GetProxy.AddInfo(sendPara);
                });
        }

        public TPLCollectorMasterCommStatus_Cur GetGprsCommStatus(string devID)
        {
            TPLCollectorMasterCommStatus_Cur result = null;
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "获取GPRS设备通信信息成功！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.GetGprsCommStatus;
                    sendPara.BusinessObject = devID;
                    CTMDalParameter para = GetProxy.GetInfo(sendPara);
                    result = para.BusinessObject as TPLCollectorMasterCommStatus_Cur;
                });
            return result;
        }

        /// <summary>
        /// 更新设备的单个无线通信状态
        /// </summary>
        /// <param name="info"></param>
        public void UpdateCollectorWireLess(TPLCollectorWireLessCommStatus_Cur info)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "更新设备的单个无线通信状态！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.UpdateCollectorWireLess;
                    sendPara.BusinessObject = info;
                    GetProxy.UpdateInfo(sendPara);
                });
        }

        /// <summary>
        /// 新增路由
        /// </summary>
        /// <param name="DevID"></param>
        /// <param name="RouteID"></param>
        public void AddRoute(TPLCollectorStaticRoutes info)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "新增路由信息！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.AddRoute;
                    sendPara.BusinessObject = info;
                    GetProxy.AddInfo(sendPara);
                });
        }

        /// <summary>
        /// 绑定进户点
        /// </summary>
        /// <param name="info"></param>
        public void BindSupplyCode(TPLCollectorAndMeter info)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "绑定进户点信息！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.BindSupplyCode;
                    sendPara.BusinessObject = info;
                    GetProxy.AddInfo(sendPara);
                });
        }

        /// <summary>
        /// 新增电表信息
        /// </summary>
        /// <param name="info"></param>
        public void InsertIntoMeter(TPLCollectorAndMeter info)
        {
            AspectF.Define.Retry(Share.Instance.ExceptionHandler)
                .Log(Share.Instance.WriteLog, "", "新增电表信息！")
                .Do(() =>
                {
                    CTMDalParameter sendPara = new CTMDalParameter();
                    sendPara.BusinessType = BusinessType.InsertIntoMeter;
                    sendPara.BusinessObject = info;
                    GetProxy.AddInfo(sendPara);
                });
        }
    }
}
