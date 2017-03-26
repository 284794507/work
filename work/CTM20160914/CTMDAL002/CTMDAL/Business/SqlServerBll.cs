using CTMDAL.CTMDAL_Utility;
using CTMDAL.Interface;
using CTMDAL.Model;
using CTMDAL.Utility;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMDAL.Business
{
    public class SqlServerBll: ICRUD
    {

        private static SqlServerBll _SqlServerBll;
        public static SqlServerBll Instance
        {
            get
            {
                if(_SqlServerBll==null)
                {
                    _SqlServerBll = new SqlServerBll();
                }
                return _SqlServerBll;
            }
        }

        public SqlConnection GetSqlConn(string SqlConnStr)
        {
            SqlConnection conn = new SqlConnection(SqlConnStr);
            conn.Open();
            return conn;
        }

        public SqlConnection GetSqlConn()
        {
            SqlConnection conn = new SqlConnection(UtilityShare.GetSqlConnStr);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 获取台区基本信息
        /// </summary>
        /// <returns></returns>
        public CTMDalParameter GetPlatFormInfo()
        {
            CTMDalParameter result = new CTMDalParameter();
            result.BusinessType = BusinessType.GetPlatFormInfo;

            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "GetPlatFormInfo", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string sql = "select objid,thedate,countrycode,citycode,platformcode,platformname,updatetime,status,memo from tplatforminfo";
                        TPlatFormInfo []arr = conn.Query<TPlatFormInfo>(sql).ToArray();
                        result.BusinessObject = arr;
                    }
                });

            return result;
        }
        
        /// <summary>
        /// 获取节点基本信息
        /// </summary>
        /// <returns></returns>
        public CTMDalParameter GetCollectorInfo()
        {
            CTMDalParameter result = new CTMDalParameter();
            result.BusinessType = BusinessType.GetCollectorInfo;

            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "GetCollectorInfo", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string sql = "select ObjID,TheDate,MacAddr,HVer,SVer,TPlatFormID,TPlatFormCode,DevType,DevStatus,ValidStatus,APhase,BPhase,CPhase";
                               sql+=" ,UpdateTime,Status,Memo,GprsID,PlcID,SNCode,Address,ChannelNo,Lon,Lat from TPLCollectorInfo";
                        TPLCollectorInfo []arr = conn.Query<TPLCollectorInfo>(sql).ToArray();
                        result.BusinessObject = arr;
                    }
                });
            return result;
        }
        
        /// <summary>
        /// 获取指定设备特定包号的升级包内容，并将以前的报的发送状态置成true
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CTMDalParameter GetNextUpgradeInfoByID(CTMDalParameter data)
        {
            CTMDalParameter result = new CTMDalParameter();
            result.BusinessType = BusinessType.GetNextUpgradeInfoByID;

            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "GetNextUpgradeInfoByID", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        int bagNo = int.Parse(data.BusinessObject.ToString());
                        string sql = "select objid from tplUpgradeFileInfo where plCollectorInfoID=@devID  order by TheDate desc";
                        TPLUpgradeFileInfo info = conn.Query<TPLUpgradeFileInfo>(sql, new { devID = data.TerminalID}).FirstOrDefault();
                        if(bagNo>0)
                        {
                            sql = "update tplUpgradeFileInfoDetail set IsDownLoadToDev=1 where fileDataNo<@no";
                            conn.Execute(sql, new { no = bagNo });
                        }
                        sql = "select ObjID,TheDate,FileInfoID,FileDataNo,FileDataLength,FileDataContent,IsDownLoadToDev,DownloadToDevTime,Status,Memo";
                            sql+= " from tplUpgradeFileInfoDetail where fileDataNo=@no and FileInfoID=@id";
                        TPLUpgradeFileInfoDetail detail= conn.Query<TPLUpgradeFileInfoDetail>(sql, new { no = bagNo,id=info.ObjID }).FirstOrDefault();
                        result.BusinessObject = detail;
                    }
                });

            return result;
        }

        /// <summary>
        /// 获取指定设备特定包号的升级包内容
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CTMDalParameter GetNextUpgradeInfoByIDAndNo(CTMDalParameter data)
        {
            CTMDalParameter result = new CTMDalParameter();
            result.BusinessType = BusinessType.GetNextUpgradeInfoByIDAndNo;

            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "GetNextUpgradeInfoByIDAndNo", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        int bagNo = int.Parse(data.BusinessObject.ToString());
                        string sql = "select objid from tplUpgradeFileInfo where plCollectorInfoID=@devID  order by TheDate desc";
                        TPLUpgradeFileInfo info = conn.Query<TPLUpgradeFileInfo>(sql, new { devID = data.TerminalID }).FirstOrDefault();
                        sql = "select ObjID,TheDate,FileInfoID,FileDataNo,FileDataLength,FileDataContent,IsDownLoadToDev,DownloadToDevTime,Status,Memo";
                        sql += " from tplUpgradeFileInfoDetail where fileDataNo=@no and FileInfoID=@id";
                        TPLUpgradeFileInfoDetail detail = conn.Query<TPLUpgradeFileInfoDetail>(sql, new { no = bagNo, id = info.ObjID }).FirstOrDefault();
                        result.BusinessObject = detail;
                    }
                });

            return result;
        }

        /// <summary>
        /// 根据当前包号及剩余包数量确定与设备的升级状态是否一致
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CTMDalParameter GetUpgradeStatus(CTMDalParameter data)
        {
            CTMDalParameter result = new CTMDalParameter();
            result.BusinessType = BusinessType.GetUpgradeStatus;

            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "GetUpgradeStatus", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        byte[] arr = data.BusinessObject as byte[];
                        int curIndex = 8;//前面有8位的地址
                        int bagNo = BitConverter.ToInt16(arr, curIndex);
                        curIndex += 2;
                        int allNum = BitConverter.ToInt16(arr, curIndex)-1;
                        string sql = "select objid from tplUpgradeFileInfo where plCollectorInfoID=@devID  order by TheDate desc";
                        TPLUpgradeFileInfo info = conn.Query<TPLUpgradeFileInfo>(sql, new { devID = data.TerminalID }).FirstOrDefault();
                        sql = "select ObjID,TheDate,FileInfoID,FileDataNo,FileDataLength,FileDataContent,IsDownLoadToDev,DownloadToDevTime,Status,Memo";
                        sql += " from tplUpgradeFileInfoDetail where fileDataNo>@no and FileInfoID=@id";
                        TPLUpgradeFileInfoDetail []detail = conn.Query<TPLUpgradeFileInfoDetail>(sql, new { no = bagNo, id = info.ObjID }).ToArray();
                        if(allNum==detail.Count())
                        {
                            result.BusinessObject =1;
                        }
                        else
                        {
                            result.BusinessObject = 0;
                        }
                    }
                });

            return result;
        }

        /// <summary>
        /// 根据设备地址获取静态路由信息
        /// </summary>
        /// <param name="data"></param>
        public CTMDalParameter GetRouteByAddr(CTMDalParameter data)
        {
            CTMDalParameter result = new CTMDalParameter();
            result.BusinessType = BusinessType.GetRouteByAddr;

            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "GetRouteByAddr", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string addr = data.BusinessObject.ToString();
                        string sql = "select MacAddr  from TPLCollectorInfo a";
                        sql += " inner join TPLCollectorStaticRoutes b on a.ObjID=b.DevID";
                        sql += " inner join TPLCollectorInfo c on b.StaticRouteNode=c.ObjID";
                        sql += " where c.MacAddr=@addr";

                        string[] detail = conn.Query<string>(sql, new { addr = addr}).ToArray();
                        result.BusinessObject = detail;
                    }
                });

            return result;
        }

        /// <summary>
        /// 获取所有的路由信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CTMDalParameter GetRouteList(CTMDalParameter data)
        {
            CTMDalParameter result = new CTMDalParameter();
            result.BusinessType = BusinessType.GetRouteByAddr;

            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "GetRouteList", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string sql = "select ObjID,TheDate,DevID,StaticRouteNode,ValidStatus,Status,Memo from TPLCollectorStaticRoutes";
                        sql += " where ValidStatus=1";

                        TPLCollectorStaticRoutes[] detail = conn.Query<TPLCollectorStaticRoutes>(sql).ToArray();
                        result.BusinessObject = detail;
                    }
                });

            return result;
        }

        /// <summary>
        /// 获取GPRS通信状态
        /// </summary>
        /// <param name="data"></param>
        public CTMDalParameter GetGprsCommStatus(CTMDalParameter data)
        {
            CTMDalParameter result = new CTMDalParameter();
            result.BusinessType = BusinessType.GetGprsCommStatus;

            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "GetGprsCommStatus", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string devid = data.BusinessObject.ToString();
                        string sql = "select ObjID,TheDate,PLCollectorInfoID,CommStatus,ChkDataTime,UpdateTime,TotalCommTimes,SuccessfulCommTimes";
                        sql+=" ,LostRate,Status from TPLCollectorMasterCommStatus_Cur";
                        sql += " where PLCollectorInfoID=@devid";

                        TPLCollectorStaticRoutes[] detail = conn.Query<TPLCollectorStaticRoutes>(sql, new { devid = devid }).ToArray();
                        result.BusinessObject = detail;
                    }
                });

            return result;
        }

        /// <summary>
        /// 获取进户点基本信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CTMDalParameter GetSupplyInfo(CTMDalParameter data)
        {
            return AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Return<CTMDalParameter>(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        CTMDalParameter result = new CTMDalParameter();
                        result.BusinessType = BusinessType.GetSupplyInfo;

                        string sql = "";
                        sql = " select   distinct   MeterName,SupplyCode,PlatFormCode,UserName,UserAddress from TPLBasicInfo";
                        TPLBasicInfo[] arrInfo = conn.Query<TPLBasicInfo>(sql).ToArray();

                        result.BusinessObject = arrInfo;

                        return result;
                    }
                });
        }

        /// <summary>
        /// 绑定采集器与进户点
        /// </summary>
        /// <param name="data"></param>
        public void BindSupplyCode(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorAndMeter info = data.BusinessObject as TPLCollectorAndMeter;
                        string sql = "";

                        info = GetMeterInfo(info);
                        ///绑定前先删除以前的绑定关系
                        sql = " delete from TPLCollectorAndMeter where CollectorID=@CollectorID";
                        conn.Execute(sql,info);
                        sql = "insert into TPLCollectorAndMeter(CollectorID,collectorName,collectorcode,supplycode,metercode,username,useraddress)";
                        sql += " values(@CollectorID,@collectorName,@collectorcode,@supplycode,@metercode,@username,@useraddress)";
                        //sql = " merge into TPLCollectorAndMeter a";
                        //sql += " using(select    '" + info.CollectorID + "' DevID ) b on a.devid=b.CollectorID";
                        //sql += " update set a.collectorName=b.collectorName,a.collectorcode=b.collectorcode,a.supplycode=b.supplycode";
                        //sql += " ,a.metercode=b.metercode,a.username=b.username,a.useraddress=b.useraddress";
                        //sql += " when not matched then";
                        //sql += " insert (CollectorID,collectorName,collectorcode,supplycode,metercode,username,useraddress)";
                        //sql += " values(@CollectorID,@collectorName,@collectorcode,@supplycode,@metercode,@username,@useraddress);";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("采集器与进户点绑定成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("采集器与进户点绑定失败！", 2);
                        }
                    }

                });
        }

        private TPLCollectorAndMeter GetMeterInfo(TPLCollectorAndMeter info)
        {
            return AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Return<TPLCollectorAndMeter>(() =>
                {
                    TPLCollectorAndMeter result = info;
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string sql = "";

                        sql = "select SNCode,CollectorName  from TPLCollectorInfo";
                        sql += " where objid=@CollectorID";
                        TPLCollectorInfo collector = conn.Query<TPLCollectorInfo>(sql, info).FirstOrDefault();
                        if (collector != null)
                        {
                            result.CollectorCode = collector.SNCode;
                            result.CollectorName = collector.CollectorName;

                            sql = "select PlatFormCode,PlatFormName from TPlatFormInfo where objid=@TPlatFormID";
                            TPlatFormInfo platForm = conn.Query<TPlatFormInfo>(sql, collector).FirstOrDefault();
                            if (platForm != null)
                            {
                                result.PlatFormCode = platForm.PlatFormCode;
                                result.PlatFormName = platForm.PlatFormName;
                            }
                        }

                        sql = "select  supplycode,username,useraddress  from  TPLCollectorAndMeter where CollectorID=@ObjID";
                        TPLCollectorAndMeter meter = conn.Query<TPLCollectorAndMeter>(sql, info).FirstOrDefault();
                        if (meter != null)
                        {
                            result.SupplyCode = meter.SupplyCode;
                            result.UserName = meter.UserName;
                            result.UserAddress = meter.UserAddress;
                        }
                    }                        

                    return result;
                });
        }
        
        /// <summary>
        /// 新增采集器与电表关系信息
        /// </summary>
        /// <param name="data"></param>
        public void InsertIntoMeter(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorAndMeter info = data.BusinessObject as TPLCollectorAndMeter;
                        string sql = "";

                        info = GetMeterInfo(info);

                        //sql = "insert into TPLCollectorAndMeter(CollectorID,collectorName,collectorcode,supplycode,metercode,username,useraddress)";
                        //sql += " values(@CollectorID,@collectorName,@collectorcode,@supplycode,@metercode,@username,@useraddress)";

                        sql = " merge into TPLCollectorAndMeter a";
                        sql += " using(select    '" + info.CollectorID + "' DevID,'" + info.MeterCode +"' MeterName ) b on a.devid=b.CollectorID and a.MeterCode=b.MeterName ";
                        sql += " update set a.collectorName=b.collectorName,a.collectorcode=b.collectorcode,a.supplycode=b.supplycode";
                        sql += " ,a.metercode=b.metercode,a.username=b.username,a.useraddress=b.useraddress";
                        sql += " when not matched then";
                        sql += " insert (CollectorID,collectorName,collectorcode,supplycode,metercode,username,useraddress)";
                        sql += " values(@CollectorID,@collectorName,@collectorcode,@supplycode,@metercode,@username,@useraddress);";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("电表信息插入成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("电表信息插入失败！", 2);
                        }
                    }                        

                });
        }

        /// <summary>
        /// 新增台区信息
        /// </summary>
        /// <param name="data"></param>
        public void InsertIntoPlatForm(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "InsertIntoPlatForm", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPlatFormInfo info = data.BusinessObject as TPlatFormInfo;
                        
                        string sql = "insert into TPlatFormInfo(ObjID,countrycode,citycode,platformcode,platformname) VALUES";
                        sql += " (@ObjID,@countrycode,@citycode,@platformcode,@platformname)";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("台区信息插入成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("台区信息插入失败！", 2);
                        }
                    }
                });
        }

        /// <summary>
        /// 插入升级数据
        /// </summary>
        /// <param name="data"></param>
        public void InsertUpdateFileToDB(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "InsertUpdateFileToDB", " 升级数据插入成功！")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLUpgradeFileInfo info = data.BusinessObject as TPLUpgradeFileInfo;
                        TPLUpgradeFileInfoDetail detail = new TPLUpgradeFileInfoDetail();
                        int curIndex = 0;
                        int no = 1;
                        int curPos = 0;
                        string sql = "";
                        while(curPos < info.FileContent.Length)
                        {
                            int len = info.FileContent.Length - curPos;
                            if(len>info.FilePerSize)
                            {
                                len = info.FilePerSize;
                            }
                            detail = new TPLUpgradeFileInfoDetail();
                            detail.FileInfoID = info.ObjID;
                            detail.FileDataNo = no++;
                            detail.FileDataLength = len;
                            detail.FileDataContent = new byte[detail.FileDataLength];
                            Buffer.BlockCopy(info.FileContent, curIndex * detail.FileDataLength, detail.FileDataContent, 0, detail.FileDataLength);
                            detail.IsDownLoadToDev = false;

                            curIndex++;
                            curPos += len;

                            sql = "insert into TPLUpgradeFileInfoDetail(FileInfoID,FileDataNo,FileDataLength,FileDataContent,IsDownLoadToDev,DownloadToDevTime) VALUES";
                            sql += " (@FileInfoID,@FileDataNo,@FileDataLength,@FileDataContent,@IsDownLoadToDev,@DownloadToDevTime)";
                            int result = conn.Execute(sql, detail);
                        }
                    }
                });
        }
        
        /// <summary>
        /// 将升级文件添加到数据库
        /// </summary>
        /// <param name="data"></param>
        public void AddUpgradeFile(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "AddUpgradeFile", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLUpgradeFileInfo info = data.BusinessObject as TPLUpgradeFileInfo;
                        info.ObjID = Guid.NewGuid();
                        string sql = "insert into TPLUpgradeFileInfo (ObjID,PLCollectorInfoID,FileUpLoadTime,FileName,FileType,FilePerSize,FileContent";
                        sql += " ,FileSoftWareVer,FileHardWareVer,IsDownLoadToDev,DownloadToDevTime) values (@ObjID,@PLCollectorInfoID,@FileUpLoadTime,@FileName";
                        sql += " ,@FileType,@FilePerSize,@FileContent,@FileSoftWareVer,@FileHardWareVer,@IsDownLoadToDev,@DownloadToDevTime)";

                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("升级文件添加成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("升级文件添加失败！", 2);
                        }
                        //插入升级文件详细信息
                        TPLUpgradeFileInfoDetail detail = new TPLUpgradeFileInfoDetail();
                        int bagNo = 1;
                        int curPos = 0;
                        int fileLen = info.FileContent.Length;
                        while (curPos < fileLen)
                        {
                            int len = fileLen - curPos;
                            if (len > 0)
                            {
                                if (len > info.FilePerSize)
                                {
                                    len = info.FilePerSize;
                                }
                                detail = new TPLUpgradeFileInfoDetail();
                                detail.FileInfoID = info.ObjID;
                                detail.FileDataNo = bagNo++;
                                detail.FileDataLength = len;
                                detail.FileDataContent = new byte[len];
                                Buffer.BlockCopy(info.FileContent, curPos, detail.FileDataContent, 0, len);
                                detail.IsDownLoadToDev = false;

                                curPos += len;
                                AddUpgradeFileDetail(detail);
                            }
                        }
                    }
                });
        }

        private void AddUpgradeFileDetail(TPLUpgradeFileInfoDetail detail)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "AddUpgradeFileDetail", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string sql = "insert into TPLUpgradeFileInfoDetail (FileInfoID,FileDataNo,FileDataLength,FileDataContent,IsDownLoadToDev)";
                        sql += "  values (@FileInfoID,@FileDataNo,@FileDataLength ,@FileDataContent,@IsDownLoadToDev)";

                        int result = conn.Execute(sql, detail);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("升级文件详细添加成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("升级文件详细添加失败！", 2);
                        }
                    }
                });
        }

        public void AddRoute(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "AddRoute", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorStaticRoutes info = data.BusinessObject as TPLCollectorStaticRoutes;

                        string sql = "insert into TPLCollectorStaticRoutes(DevID,StaticRouteNode) VALUES";
                        sql += " (@DevID,@StaticRouteNode)";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("路由信息插入成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("路由信息插入失败！", 2);
                        }
                    }
                });
        }

        /// <summary>
        /// 新增台区信息
        /// </summary>
        /// <param name="data"></param>
        //public void AddNewPlatForm(CTMDalParameter data)
        //{
        //    AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
        //        .Log(UtilityShare.Instance.WriteLog, "AddNewPlatForm", "")
        //        .Do(() =>
        //        {
        //            using (IDbConnection conn = GetSqlConn())
        //            {
        //                TPlatFormInfo info = data.BusinessObject as TPlatFormInfo;

        //                string sql = "insert into TPlatFormInfo (ObjID,CountryCode,CityCode,PlatFormCode,PlatFormName";
        //                sql += " ) values (@ObjID,@CountryCode,@CityCode,@PlatFormCode,@PlatFormName)";

        //                int result = conn.Execute(sql, info);
        //                if (result > 0)
        //                {
        //                    UtilityShare.Instance.WriteLog("台区信息添加成功！", 2);
        //                }
        //                else
        //                {
        //                    UtilityShare.Instance.WriteLog("台区信息添加失败！", 2);
        //                }
        //            }
        //        });
        //}

        /// <summary>
        /// 更新实时电参数
        /// </summary>
        /// <param name="data"></param>
        public void UpdateDataRecRTM(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateDataRecRTM", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {

                        TPLDataRecRTM info = data.BusinessObject as TPLDataRecRTM;
                        InsertIntoBatchTask(info.BatchID.ToString());

                        info.UpdateTime = DateTime.Now;
                        string sql = "merge into TPLDataRecRTM a";
                        sql += " using(select    '" + info.DevID + "' DevID ) b on a.devid=b.devid";
                        sql += " when matched then";
                        sql += " update set a.LampVoltageA=@LampVoltageA,a.LampCurrentA=@LampCurrentA,a.LampActivePowerA=@LampActivePowerA";
                        sql += " ,a.LampPowerFactA=@LampPowerFactA,a.LampVoltageB=@LampVoltageB,a.LampCurrentB=@LampCurrentB,a.LampActivePowerB=@LampActivePowerB";
                        sql += " ,a.LampPowerFactB=@LampPowerFactB,a.LampVoltageC=@LampVoltageC,a.LampCurrentC=@LampCurrentC,a.LampActivePowerC=@LampActivePowerC";
                        sql += " ,a.LampPowerFactC=@LampPowerFactC,a.GetDataTime=@GetDataTime,a.UpdateTime=@UpdateTime,a.DevStatus=@DevStatus";
                        sql += " ,a.BatchID=@BatchID";
                        sql += " when not matched then";
                        sql += " insert (devid,LampVoltageA,LampCurrentA,LampActivePowerA,LampPowerFactA,LampVoltageB ,LampCurrentB,LampActivePowerB";
                        sql += " ,LampPowerFactB,LampVoltageC,LampCurrentC,LampActivePowerC,LampPowerFactC,GetDataTime,UpdateTime,DevStatus,BatchID)";
                        sql += " VALUES (@devid,@LampVoltageA,@LampCurrentA,@LampActivePowerA,@LampPowerFactA,@LampVoltageB ,@LampCurrentB,@LampActivePowerB";
                        sql += " ,@LampPowerFactB,@LampVoltageC,@LampCurrentC,@LampActivePowerC,@LampPowerFactC,@GetDataTime,@UpdateTime,@DevStatus,@BatchID);";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("实时电参数更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("实时电参数更新失败！", 2);
                        }
                        //插入实时电参数的同时插入历史电参数
                        sql = "insert into TPLDataRecHIS(objid,thedate,devid,LampVoltageA,LampCurrentA,LampActivePowerA,LampPowerFactA,LampVoltageB";
                        sql += " ,LampCurrentB,LampActivePowerB,LampPowerFactB,LampVoltageC,LampCurrentC,LampActivePowerC,LampPowerFactC,GetDataTime";
                        sql += " ,UpdateTime,DevStatus,BatchID) ";
                        sql += " (select newid(),thedate,devid,LampVoltageA,LampCurrentA,LampActivePowerA,LampPowerFactA,LampVoltageB,LampCurrentB";
                        sql += " ,LampActivePowerB,LampPowerFactB,LampVoltageC,LampCurrentC,LampActivePowerC,LampPowerFactC,GetDataTime";
                        sql += " ,UpdateTime,DevStatus,BatchID from TPLDataRecRTM where DevID=@DevID)";
                        result = conn.Execute(sql, new { DevID = info.DevID });
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("历史电参数插入成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("历史电参数插入失败！", 2);
                        }
                    }
                });
        }

        private void InsertIntoBatchTask(string batchID)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "InsertIntoBatchTask", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string sql = "select ObjID,TheDate,BatchNum,BatchDateTime,Memo from TPLBatchTask where ObjID=@objID";
                        TPLBatchTask info= null;
                        if(string.IsNullOrEmpty(batchID))
                        {
                            info = null;
                        }
                        else
                        {
                            info = conn.Query<TPLBatchTask>(sql, new { objID = batchID }).SingleOrDefault();
                        }
                        if(info==null)
                        {
                            sql = "select ObjID,TheDate,BatchNum,BatchDateTime,Memo from TPLBatchTask where BatchNum=(select max(BatchNum) from TPLBatchTask)";
                            TPLBatchTask maxInfo = conn.Query<TPLBatchTask>(sql, new { objID = batchID }).SingleOrDefault();

                            info = new TPLBatchTask();
                            info.ObjID = new Guid(batchID);
                            info.BatchNum = maxInfo.BatchNum + 1;

                            sql = "insert into TPLBatchTask(ObjID,TheDate,BatchDateTime) values";
                            sql += " (@ObjID,@TheDate,@BatchDateTime)";
                            int result = conn.Execute(sql, info);
                            if (result > 0)
                            {
                                UtilityShare.Instance.WriteLog("电参数批次ID增加成功！", 2);
                            }
                            else
                            {
                                UtilityShare.Instance.WriteLog("电参数批次ID增加失败！", 2);
                            }
                        }
                    }
                });
        }

        class collectorNum
        {
            public int maxNum { get; set; }
        }

        /// <summary>
        /// 插入采集器数据，带经纬度，一般用于手机APP上报
        /// </summary>
        /// <param name="data"></param>
        public void InsertCollectorInfo(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorInfo", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string sql = "";
                        TPLCollectorInfo info = data.BusinessObject as TPLCollectorInfo;
                        if (string.IsNullOrEmpty(info.CollectorName))
                        {   //设备名称依次取设备1、设备2、
                            sql = "select count(*) maxNum from TPLCollectorInfo ";
                            collectorNum cNum = conn.Query<collectorNum>(sql).FirstOrDefault();
                            //var cNum = conn.Query(sql).FirstOrDefault();
                            info.CollectorName = "设备" + (cNum.maxNum + 1);

                        }
                        //根据MAC地址确定一个设备
                        info.UpdateTime = DateTime.Now;
                        sql = "merge into TPLCollectorInfo a";
                        sql += " using(select   '" + info.MacAddr + "' MacAddr ) b on a.MacAddr=b.MacAddr";// and a.SNCode=@SNCode
                        sql += " when matched then";
                        sql += " update set a.HVer=@HVer,a.SVer=@SVer,a.DevType=@DevType,a.DevStatus=@DevStatus,a.SNCode=@SNCode";
                        sql += " ,a.UpdateTime=@UpdateTime,a.ChannelNo=@ChannelNo,a.Lon=@Lon,a.Lat=@Lat,a.GPRSID=@GPRSID";//
                        sql += " when not matched then";
                        sql += " insert (ObjID,MacAddr,HVer,SVer,DevType,DevStatus , UpdateTime,SNCode,ChannelNo,CollectorName,GPRSID,Lon,Lat)VALUES";//,Lon,Lat
                        sql += " (@ObjID,@MacAddr,@HVer,@SVer,@DevType,@DevStatus,@UpdateTime,@SNCode,@ChannelNo,@CollectorName,@GPRSID,@Lon,@Lat);";//,@Lon,@Lat
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("节点数据更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("节点数据更新失败！", 2);
                        }
                    }
                });
        }

        /// <summary>
        /// 更新节点数据
        /// </summary>
        /// <param name="data"></param>
        public void UpdateCollectorInfo(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorInfo", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string sql = "";
                        TPLCollectorInfo info = data.BusinessObject as TPLCollectorInfo;
                        if (string.IsNullOrEmpty(info.CollectorName))
                        {   //设备名称依次取设备1、设备2、
                            sql = "select count(*) maxNum from TPLCollectorInfo ";
                            collectorNum cNum =conn.Query<collectorNum>(sql).FirstOrDefault();
                            //var cNum = conn.Query(sql).FirstOrDefault();
                            info.CollectorName = "设备"+(cNum.maxNum+1);

                        }
                        //根据MAC地址确定一个设备
                        info.UpdateTime = DateTime.Now;
                        sql = "merge into TPLCollectorInfo a";
                        sql += " using(select   '" + info.MacAddr + "' MacAddr ) b on a.MacAddr=b.MacAddr";// and a.SNCode=@SNCode
                        sql += " when matched then";
                        sql += " update set a.HVer=@HVer,a.SVer=@SVer,a.DevType=@DevType,a.DevStatus=@DevStatus,a.SNCode=@SNCode";
                        sql += " ,a.UpdateTime=@UpdateTime,a.ChannelNo=@ChannelNo,a.GPRSID=@GPRSID";//a.Lon=@Lon,a.Lat=@Lat,
                        sql += " when not matched then";
                        sql += " insert (ObjID,MacAddr,HVer,SVer,DevType,DevStatus , UpdateTime,SNCode,ChannelNo,CollectorName,GPRSID)VALUES";//,Lon,Lat
                        sql += " (@ObjID,@MacAddr,@HVer,@SVer,@DevType,@DevStatus,@UpdateTime,@SNCode,@ChannelNo,@CollectorName,@GPRSID);";//,@Lon,@Lat
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("节点数据更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("节点数据更新失败！", 2);
                        }
                    }
                });
        }

        /// <summary>
        /// 更新节点状态
        /// </summary>
        /// <param name="data"></param>
        public void UpdateCollectorStatus(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorStatus", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorInfo info = data.BusinessObject as TPLCollectorInfo;
                        info.UpdateTime = DateTime.Now;
                        string sql = "update TPLCollectorInfo set DevStatus=@DevStatus , UpdateTime=@UpdateTime where MacAddr=@MacAddr";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("节点状态更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("节点状态更新失败！", 2);
                        }
                    }
                });
        }

        /// <summary>
        /// 更新设备所属PLC，台区设备信息
        /// </summary>
        /// <param name="data"></param>
        public void UpdateCollectorUp(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorUp", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorInfo info = data.BusinessObject as TPLCollectorInfo;
                        info.UpdateTime = DateTime.Now;
                        string sql = "update TPLCollectorInfo set TPlatFormID=@TPlatFormID , UpdateTime=@UpdateTime , TPlatFormCode=@TPlatFormCode";
                        sql+= " ,  PlcID=@PlcID where MacAddr=@MacAddr";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("节点所属PLC，台区设备信息更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("节点所属PLC，台区设备信息更新失败！", 2);
                        }
                    }
                });
        }

        /// <summary>
        /// 更新设备所属GPRS信息
        /// </summary>
        /// <param name="data"></param>
        public void UpdateCollectorGPRS(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorGPRS", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorInfo info = data.BusinessObject as TPLCollectorInfo;
                        info.UpdateTime = DateTime.Now;
                        string sql = "update TPLCollectorInfo set  UpdateTime=@UpdateTime ";
                        sql += " , GprsID=@GprsID where MacAddr=@MacAddr";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("节点所属GPRS信息更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("节点所属GPRS信息更新失败！", 2);
                        }
                    }
                });
        }

        /// <summary>
        /// 更新设备相位及状态
        /// </summary>
        /// <param name="data"></param>
        public void UpdateCollectorPhase(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorPhase", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorInfo info = data.BusinessObject as TPLCollectorInfo;
                        info.UpdateTime = DateTime.Now;
                        string sql = "update TPLCollectorInfo set DevStatus=@DevStatus , UpdateTime=@UpdateTime , APhase=@aPhase , BPhase=@bPhase";
                        sql+= " , CPhase=@cPhase where MacAddr=@MacAddr";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("节点相位更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("节点相位更新失败！", 2);
                        }
                    }
                });
        }

        /// <summary>
        /// 更新无线通信时间及状态
        /// </summary>
        /// <param name="data"></param>
        public void UpdateCollectorWireLessStatus(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorWireLessStatus", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorWireLessCommStatus_Cur info = data.BusinessObject as TPLCollectorWireLessCommStatus_Cur;
                        info.UpdateTime = DateTime.Now;
                        string sql = "merge into TPLCollectorWireLessCommStatus_Cur a";
                        sql += " using(select   '" + info.PLCollectorInfoID + "' PLCollectorInfoID ) b";
                        sql += "  on a.PLCollectorInfoID=b.PLCollectorInfoID when matched then";
                        sql += " update set a.PLCollectorInfoID=@PLCollectorInfoID,a.WireLessStatus=@WireLessStatus,a.ChkDataTime=@ChkDataTime";
                        sql += " ,a.TotalCommTimes=@TotalCommTimes,a.SuccessfulCommTimes=@SuccessfulCommTimes,a.LostRate=@LostRate,a.UpdateTime=@UpdateTime";
                        sql += " when not matched then";
                        sql += " insert (PLCollectorInfoID,WireLessStatus,ChkDataTime,TotalCommTimes,SuccessfulCommTimes,LostRate,UpdateTime) VALUES";
                        sql += " (@PLCollectorInfoID,@WireLessStatus,@ChkDataTime,@TotalCommTimes,@SuccessfulCommTimes,@LostRate,@UpdateTime);";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("无线通信时间及状态更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("无线通信时间及状态更新失败！", 2);
                        }
                        //插入实时无线通信信息的同时插入历史电参数
                        UpdateCollectorWireLessHis(info);
                    }
                });
        }

        /// <summary>
        /// 更新某个设备的无线通信状态
        /// </summary>
        /// <param name="data"></param>
        public void UpdateCollectorWireLess(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorWireLess", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorWireLessCommStatus_Cur info = data.BusinessObject as TPLCollectorWireLessCommStatus_Cur;
                        info.UpdateTime = DateTime.Now;
                        string sql = "update TPLCollectorWireLessCommStatus_Cur set WireLessStatus=@WireLessStatus , ChkDataTime=@ChkDataTime , UpdateTime=@UpdateTime";
                        sql += " where PLCollectorInfoID=@PLCollectorInfoID";
                        
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("单设备无线通信时间及状态更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("单设备无线通信时间及状态更新失败！", 2);
                        }
                        //插入实时无线通信通信信息的同时插入历史电参数
                        UpdateCollectorWireLessHis(info);
                    }
                });
        }

        private void UpdateCollectorWireLessHis(TPLCollectorWireLessCommStatus_Cur info)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorWireLessHis", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string sql = "insert into TPLCollectorWireLessCommStatus_HIS(objid,thedate,PLCollectorInfoID,WireLessStatus,ChkDataTime,TotalCommTimes";
                        sql += " ,SuccessfulCommTimes,LostRate,UpdateTime) (select newid(),thedate,PLCollectorInfoID,WireLessStatus,ChkDataTime,TotalCommTimes";
                        sql += "  ,SuccessfulCommTimes,LostRate,UpdateTime from TPLCollectorWireLessCommStatus_Cur where PLCollectorInfoID=@PLCollectorInfoID)";
                        int result = conn.Execute(sql,new { PLCollectorInfoID = info.PLCollectorInfoID });
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("历史无线通信时间及状态插入成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("历史无线通信时间及状态插入失败！", 2);
                        }
                    }
                });
        }

        /// <summary>
        /// 更新PLC通信时间及状态
        /// </summary>
        /// <param name="data"></param>
        public void UpdateCollectorPLCCommStatus(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorPLCCommStatus", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorPLCCommStatus_Cur info = data.BusinessObject as TPLCollectorPLCCommStatus_Cur;
                        info.UpdateTime = DateTime.Now;
                        string sql = "merge into TPLCollectorPLCCommStatus_Cur a";
                        sql += " using(select    '" + info.PLCollectorInfoID + "' PLCollectorInfoID ) b on a.PLCollectorInfoID=b.PLCollectorInfoID";
                        sql += " when matched then";
                        sql += " update set a.PLCollectorInfoID=@PLCollectorInfoID,a.PLCL1Status=@PLCL1Status,a.PLCL2Status=@PLCL2Status,a.PLCL3Status=@PLCL3Status";
                        sql += " ,a.ChkDataTime1=@ChkDataTime1,a.ChkDataTime2=@ChkDataTime2,a.ChkDataTime3=@ChkDataTime3,a.UpdateTime=@UpdateTime";
                        sql += " ,a.L1TotalCommTimes=@L1TotalCommTimes,a.L2TotalCommTimes=@L2TotalCommTimes,a.L3TotalCommTimes=@L3TotalCommTimes";
                        sql += " ,a.L1SuccessfulCommTimes=@L1SuccessfulCommTimes,a.L2SuccessfulCommTimes=@L2SuccessfulCommTimes,a.L3SuccessfulCommTimes=@L3SuccessfulCommTimes";
                        sql += " ,a.L1LostRate=@L1LostRate,a.L2LostRate=@L2LostRate,a.L3LostRate=@L3LostRate  when not matched then";
                        sql += " insert  (PLCollectorInfoID,PLCL1Status,PLCL2Status,PLCL3Status,ChkDataTime1,ChkDataTime2,ChkDataTime3,UpdateTime,L1TotalCommTimes";
                        sql += " ,L2TotalCommTimes,L3TotalCommTimes,L1SuccessfulCommTimes,L2SuccessfulCommTimes,L3SuccessfulCommTimes,L1LostRate,L2LostRate,L3LostRate)";
                        sql += " VALUES (@PLCollectorInfoID,@PLCL1Status,@PLCL2Status,@PLCL3Status,@ChkDataTime1,@ChkDataTime2,@ChkDataTime3,@UpdateTime,@L1TotalCommTimes";
                        sql += " ,@L2TotalCommTimes,@L3TotalCommTimes,@L1SuccessfulCommTimes,@L2SuccessfulCommTimes,@L3SuccessfulCommTimes,@L1LostRate,@L2LostRate,@L3LostRate);";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("PLC通信时间及状态更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("PLC通信时间及状态更新失败！", 2);
                        }
                        //插入实时PLC通信信息的同时插入历史电参数
                        UpdateCollectorPLCCommHis(info);
                    }
                });
        }

        private void UpdateCollectorPLCCommHis(TPLCollectorPLCCommStatus_Cur info)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorPLCCommHis", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string sql = "insert into TPLCollectorPLCCommStatus_HIS(objid,thedate,PLCollectorInfoID,PLCL1Status,PLCL2Status,PLCL3Status,ChkDataTime1";
                        sql += " ,ChkDataTime2,ChkDataTime3,UpdateTime,L1TotalCommTimes,L2TotalCommTimes,L3TotalCommTimes,L1SuccessfulCommTimes";
                        sql += " ,L2SuccessfulCommTimes,L3SuccessfulCommTimes,L1LostRate,L2LostRate,L3LostRate) (select newid(),thedate,PLCollectorInfoID";
                        sql += " ,PLCL1Status,PLCL2Status,PLCL3Status,ChkDataTime1,ChkDataTime2,ChkDataTime3,UpdateTime,L1TotalCommTimes,L2TotalCommTimes,L3TotalCommTimes";
                        sql += " ,L1SuccessfulCommTimes,L2SuccessfulCommTimes,L3SuccessfulCommTimes,L1LostRate,L2LostRate,L3LostRate from TPLCollectorPLCCommStatus_Cur";
                        sql += " where PLCollectorInfoID=@PLCollectorInfoID)";
                        int result = conn.Execute(sql, new { PLCollectorInfoID = info.PLCollectorInfoID });
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("历史PLC通信时间及状态插入成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("历史PLC通信时间及状态插入失败！", 2);
                        }
                    }
                });
        }

        /// <summary>
        /// 更新GPRS通信时间及状态表
        /// </summary>
        /// <param name="data"></param>
        public void UpdateCollectorMasterCommStatus(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorMasterComm", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorMasterCommStatus_Cur info = data.BusinessObject as TPLCollectorMasterCommStatus_Cur;
                        info.UpdateTime = DateTime.Now;
                        string sql = "merge into TPLCollectorMasterCommStatus_Cur a";
                        sql += " using(select   '" + info.PLCollectorInfoID + "' PLCollectorInfoID ) b";
                        sql += "  on a.PLCollectorInfoID=b.PLCollectorInfoID when matched then";
                        sql += " update set a.PLCollectorInfoID=@PLCollectorInfoID,a.CommStatus=@CommStatus,a.ChkDataTime=@ChkDataTime";
                        sql += " ,a.TotalCommTimes=@TotalCommTimes,a.SuccessfulCommTimes=@SuccessfulCommTimes,a.LostRate=@LostRate, a.UpdateTime=@UpdateTime";
                        sql += " when not matched then";
                        sql += " insert ( PLCollectorInfoID,CommStatus,ChkDataTime,TotalCommTimes,SuccessfulCommTimes,LostRate,UpdateTime) VALUES";
                        sql += " ( @PLCollectorInfoID,@CommStatus,@ChkDataTime,@TotalCommTimes,@SuccessfulCommTimes,@LostRate,@UpdateTime);";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("GPRS通信时间及状态更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("GPRS通信时间及状态更新失败！", 2);
                        }
                        //插入实时GPRS通信信息的同时插入历史电参数
                        AddCollectorMasterCommHis(info);
                    }
                });
        }

        /// <summary>
        /// 更新某个设备的GPRS通信状态
        /// </summary>
        /// <param name="data"></param>
        public void UpdateCollectorMasterComm(CTMDalParameter data)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateCollectorMasterCommStatus", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TPLCollectorMasterCommStatus_Cur info = data.BusinessObject as TPLCollectorMasterCommStatus_Cur;
                        info.UpdateTime = DateTime.Now;
                        string sql = "update TPLCollectorMasterCommStatus_Cur set CommStatus=@CommStatus , ChkDataTime=@ChkDataTime , UpdateTime=@UpdateTime";
                        sql += " where PLCollectorInfoID=@PLCollectorInfoID";
                        
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("单设备GPRS通信时间及状态更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("单设备GPRS通信时间及状态更新失败！", 2);
                        }
                        //插入实时GPRS通信信息的同时插入历史电参数
                        AddCollectorMasterCommHis(info);
                        //如果GPRS通信中断，则所属的设备无线全部通信中断
                        if(info.CommStatus== 0x03)
                        {
                            UpdateWirelessByGprs(info);
                        }
                    }
                });
        }

        /// <summary>
        /// 如果GPRS通信中断，则所属的设备无线全部通信中断
        /// </summary>
        /// <param name="info"></param>
        private void UpdateWirelessByGprs(TPLCollectorMasterCommStatus_Cur info)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "UpdateWirelessByGprs", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {

                        TPLCollectorWireLessCommStatus_Cur wireLess = new TPLCollectorWireLessCommStatus_Cur();
                        wireLess.PLCollectorInfoID = info.PLCollectorInfoID;
                        wireLess.WireLessStatus = 0;
                        wireLess.UpdateTime = info.UpdateTime;
                        wireLess.ChkDataTime = info.ChkDataTime;
                        string sql = "update TPLCollectorWireLessCommStatus_Cur  set WireLessStatus=@WireLessStatus , ChkDataTime=@ChkDataTime , UpdateTime=@UpdateTime";
                        sql += " where PLCollectorInfoID in (select ObjID from TPLCollectorInfo where GprsID=@PLCollectorInfoID) ";

                        int result = conn.Execute(sql, wireLess);
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("单设备无线通信时间及状态更新成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("单设备无线通信时间及状态更新失败！", 2);
                        }
                        //插入实时无线通信信息的同时插入历史电参数
                        sql = "insert into TPLCollectorWireLessCommStatus_HIS(objid,thedate,PLCollectorInfoID,WireLessStatus,ChkDataTime,TotalCommTimes";
                        sql += " ,SuccessfulCommTimes,LostRate,UpdateTime) (select newid(),thedate,PLCollectorInfoID,WireLessStatus,ChkDataTime,TotalCommTimes";
                        sql += "  ,SuccessfulCommTimes,LostRate,UpdateTime from TPLCollectorWireLessCommStatus_Cur where ";
                        sql += " PLCollectorInfoID in (select ObjID from TPLCollectorInfo where GprsID=@PLCollectorInfoID))";
                        result = conn.Execute(sql, new { PLCollectorInfoID = info.PLCollectorInfoID });
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("历史无线通信时间及状态插入成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("历史无线通信时间及状态插入失败！", 2);
                        }
                    }
                });
        }

        private void AddCollectorMasterCommHis(TPLCollectorMasterCommStatus_Cur info)
        {
            AspectF.Define.Retry(UtilityShare.Instance.CatchExption)
                .Log(UtilityShare.Instance.WriteLog, "AddCollectorMasterCommHis", "")
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string sql = "insert into TPLCollectorMasterCommStatus_HIS(objid,thedate,PLCollectorInfoID,CommStatus,ChkDataTime,TotalCommTimes";
                        sql += " ,SuccessfulCommTimes,LostRate,UpdateTime)";
                        sql += "  (select newid(),thedate,PLCollectorInfoID,CommStatus,ChkDataTime,TotalCommTimes,SuccessfulCommTimes,LostRate,UpdateTime";
                        sql += " from TPLCollectorMasterCommStatus_Cur where PLCollectorInfoID=@PLCollectorInfoID)";
                        int result = conn.Execute(sql, new { PLCollectorInfoID = info.PLCollectorInfoID });
                        if (result > 0)
                        {
                            UtilityShare.Instance.WriteLog("历史GPRS通信时间及状态插入成功！", 2);
                        }
                        else
                        {
                            UtilityShare.Instance.WriteLog("历史GPRS通信时间及状态插入失败！", 2);
                        }
                    }
                });
        }
   
    }
}
