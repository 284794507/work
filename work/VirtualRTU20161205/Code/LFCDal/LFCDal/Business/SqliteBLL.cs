using Dapper;
using LFCDal.Interface;
using LFCDal.Model;
using LFCDal.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFCDal.Business
{
    public class SqliteBLL : IDBBLL
    {
        private static SqliteBLL _SqliteBLL;
        public static SqliteBLL GetBll
        {
            get
            {
                if (_SqliteBLL == null)
                {
                    _SqliteBLL = new SqliteBLL();
                }
                return _SqliteBLL;
            }
        }

        public SQLiteConnection GetSqlConn(string SqliteStr)
        {
            SQLiteConnection conn = new SQLiteConnection(SqliteStr);
            conn.Open();
            return conn;
        }

        public SQLiteConnection GetSqlConn()
        {
            SQLiteConnection conn = new SQLiteConnection(Share.GetSqlitenStr);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 获取CTU基本信息
        /// </summary>
        /// <returns></returns>
        public LFCDalParameter GetALLCtuInfo()
        {
            using (IDbConnection conn = GetSqlConn())
            {
                LFCDalParameter para = new LFCDalParameter();
                para.BusinessType = BusinessType.GetCtuInfo;

                string sql = "Select * from tCTUInfo";
                tCTUInfo[] arr = conn.Query<tCTUInfo>(sql).ToArray();
                foreach (tCTUInfo info in arr)
                {
                    info.CTUCommAddr = int.Parse(info.CTUCommAddr).ToString();
                }
                para.BusinessObject = arr;

                return para;
            }
        }

        /// <summary>
        /// 获取系统基本信息
        /// </summary>
        /// <returns></returns>
        public LFCDalParameter GetSysRunStatus()
        {
            using (IDbConnection conn = GetSqlConn())
            {
                LFCDalParameter para = new LFCDalParameter();
                para.BusinessType = BusinessType.GetSysRunStatus;

                string sql = "Select * from tSysRunStatus";
                tSysRunStatus[] arr = conn.Query<tSysRunStatus>(sql).ToArray();
                para.BusinessObject = arr;

                return para;
            }
        }

        /// <summary>
        /// 获取单灯基本信息
        /// </summary>
        /// <returns></returns>
        public LFCDalParameter GetLampList()
        {
            using (IDbConnection conn = GetSqlConn())
            {
                LFCDalParameter para = new LFCDalParameter();
                para.BusinessType = BusinessType.GetLampList;

                string sql = "Select ctu.ctuid, ctu.CTUCommAddr,ptu.ptuid,ptu.ptuver,ptuch.ptuchno,lamp.lampno,lamp.lamptype,lamp.lampstatus,grp.grpno ";
                sql += " from tptuinfo ptu inner join tptuchlampcfg ptuch on ptu.ptuno=ptuch.ptuno ";
                sql += " inner join tlampinfo lamp on lamp.lampno=ptuch.lampno  ";
                sql += " left join (select lampno,grpno from tlampgrpcfg where DownloadTick=1 and optType=1)  grp on lamp.lampno=grp.lampno  ";
                sql += " inner join tCTUInfo ctu on ctu.ctuid=ptu.ctuid  ";
                vLampInfo[] arr = conn.Query<vLampInfo>(sql).ToArray();
                foreach (vLampInfo info in arr)
                {
                    info.CTUCommAddr = int.Parse(info.CTUCommAddr).ToString();
                }
                para.BusinessObject = arr;

                return para;
            }
        }

        /// <summary>
        /// 根据单灯号获取单灯状态
        /// </summary>
        /// <param name="lampNo"></param>
        /// <returns></returns>
        public LFCDalParameter GetLampStatusByLampNo(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                int lampNo = int.Parse(para.BusinessObject.ToString());
                LFCDalParameter result = new LFCDalParameter();
                result.BusinessType = BusinessType.GetLampStatusByLampNo;

                string sql = "Select * from tLampNewStatus where LampNo = @lampNo and CTUID = @CtuID";
                tLampNewStatus info = conn.Query<tLampNewStatus>(sql, new { lampNo = lampNo, CtuID = para.CTUID }).FirstOrDefault();
                result.BusinessObject = info;

                return result;
            }
        }

        /// <summary>
        /// 根据单灯号获取单灯电参数
        /// </summary>
        /// <param name="lampNo"></param>
        /// <returns></returns>
        public LFCDalParameter GetLampDataRecByLampNo(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                int lampNo = int.Parse(para.BusinessObject.ToString());
                LFCDalParameter result = new LFCDalParameter();
                result.BusinessType = BusinessType.GetLampDataRecByLampNo;

                string sql = "Select * from tPTUCurDataRec where LampNo = @lampNo and CTUID = @CtuID";
                tPTUCurDataRec info = conn.Query<tPTUCurDataRec>(sql, new { lampNo = lampNo, CtuID = para.CTUID }).FirstOrDefault();
                result.BusinessObject = info;

                return result;
            }
        }

        /// <summary>
        /// 获取最近需要执行的单灯临时预约
        /// </summary>
        /// <returns></returns>
        public LFCDalParameter GetLampTempCtrlCfg()
        {
            using (IDbConnection conn = GetSqlConn())
            {
                LFCDalParameter result = new LFCDalParameter();
                result.BusinessType = BusinessType.GetLampTempCtrlCfg;

                string sql = "Select * from tLampTmpCtrlCfg where  PlanOptDateTime>datetime('now', 'localtime') ";//"Select * from tLampTmpCtrlCfg where strftime('%Y-%m-%d', OptDateTime)=date('NOW')";
                tLampTmpCtrlCfg[] arr = conn.Query<tLampTmpCtrlCfg>(sql).ToArray();
                result.BusinessObject = arr;

                return result;
            }
        }

        /// <summary>
        /// 根据类型与灯（组）号查询对应的临时预约
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public LFCDalParameter QueryLampTempCtrlCfg(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                LFCDalParameter result = new LFCDalParameter();
                result.BusinessType = BusinessType.QueryLampTempCtrlCfg;

                pLampTmpCtrlCfg data = (pLampTmpCtrlCfg)para.BusinessObject;
                string sql = "Select * from tLampTmpCtrlCfg where OptType=@optType and LampObjNo=@lampObjNo and CtuID=@ctuID";
                tLampTmpCtrlCfg[] arr = conn.Query<tLampTmpCtrlCfg>(sql, new { optType = data.OptType, lampObjNo = data.No, CtuID = para.CTUID }).ToArray();
                result.BusinessObject = arr;

                return result;
            }
        }

        /// <summary>
        /// 根据配置号删除单灯(组)临时预约
        /// </summary>
        /// <param name="data"></param>
        public void DelLampTmpCtrlCfgByCfgNo(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                pLampTmpCtrlCfg data = (pLampTmpCtrlCfg)para.BusinessObject;
                string sql = "Delete from tLampTmpCtrlCfg where OptType = @optType and  cfgNo = @cfgNo and CTUID = @CtuID";
                conn.Execute(sql, new { optType = data.OptType, cfgNo = data.No, CtuID = para.CTUID });
            }
        }

        /// <summary>
        /// 根据灯(组)号删除单灯(组)临时预约
        /// </summary>
        /// <param name="data"></param>
        public void DelLampTmpCtrlCfgByNo(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                pLampTmpCtrlCfg data = (pLampTmpCtrlCfg)para.BusinessObject;
                string sql = "Delete from tLampTmpCtrlCfg where OptType = @optType and  LampObjNo = @lampObjNo and CTUID = @CtuID";
                conn.Execute(sql, new { optType = data.OptType, lampObjNo = data.No, CtuID = para.CTUID });
            }
        }

        /// <summary>
        /// 删除所有单灯(组)临时预约
        /// </summary>
        public void DelAllLampTmpCtrlCfg(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                string sql = "Delete from tLampTmpCtrlCfg  where  CTUID = @CtuID";
                conn.Execute(sql, new { CtuID = para.CTUID });
            }
        }

        /// <summary>
        /// 新增单灯临时预约
        /// </summary>
        /// <param name="data"></param>
        public void AddLampTmpCtrlCfg(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                tLampTmpCtrlCfg data = para.BusinessObject as tLampTmpCtrlCfg;
                data.OptDateTime = DateTime.Now;
                string sql = "insert into tLampTmpCtrlCfg(CfgNo,PlanOptDateTime,OptType,LampObjNo,OptValue,CtrlStatus,OptDateTime,Memo,CtuID) VALUES";
                sql += " (@CfgNo,@PlanOptDateTime,@OptType,@LampObjNo,@OptValue,@CtrlStatus,@OptDateTime,@Memo,@CtuID)";
                int result = conn.Execute(sql, data);
                if (result > 0)
                {
                    Console.WriteLine("插入成功！");
                }
                else
                {
                    Console.WriteLine("插入失败！");
                }
            }
        }

        /// <summary>
        /// 根据CTUID获取当前临时预约最大编号
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public LFCDalParameter GetMaxLampTmpCtrlCfgNo(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                LFCDalParameter result = new LFCDalParameter();
                result.BusinessType = BusinessType.GetMaxLampTmpCtrlCfgNo;

                string sql = "select max(CfgNo) maxNo from tLampTmpCtrlCfg where CTUID = @CtuID";
                var arr = conn.Query(sql, new { CtuID = para.CTUID }).ToArray();
                if(arr[0].maxNo == null)
                {
                    result.BusinessObject = 1;
                }
                else
                {
                    result.BusinessObject = (int)(arr[0].maxNo + 1);
                }

                return result;
            }
        }

        /// <summary>
        /// 根据周天参数删除单灯(组)周期性临时预约
        /// </summary>
        /// <param name="data"></param>
        public void DelLampWeekCtrlCfgByWeekDay(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                int data = int.Parse(para.BusinessObject.ToString());
                string sql = "Delete from tLampWeekCtrlCfg where CmdType = @cmdType   and CTUID = @CtuID";
                conn.Execute(sql, new { cmdType = data, CtuID = para.CTUID });
            }
        }

        /// <summary>
        /// 删除所有周期性临时预约
        /// </summary>
        public void DelAllLampWeekCtrlCfg(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                string sql = "Delete from tLampWeekCtrlCfg  where  CTUID = @CtuID";
                conn.Execute(sql, new { CtuID = para.CTUID });
            }
        }

        /// <summary>
        /// 新增单灯(组)周期性临时预约
        /// </summary>
        /// <param name="data"></param>
        public void AddLampWeekCtrlCfg(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                tLampWeekCtrlCfg[] data= para.BusinessObject as tLampWeekCtrlCfg[];

                string sql = "replace into tLampWeekCtrlCfg(CmdType,GrpNo,TimeType,OptTime,OptType,LampOrGrpNo,OptValue,CtrlStatus,OptDateTime,CtuID) VALUES";
                sql += " (@CmdType,@GrpNo,@TimeType,@OptTime,@OptType,@LampOrGrpNo,@OptValue,@CtrlStatus,@OptDateTime,@CtuID)";
                int result = conn.Execute(sql, data);
                if (result > 0)
                {
                    Console.WriteLine("插入成功！");
                }
                else
                {
                    Console.WriteLine("插入失败！");
                }
            }
        }

        /// <summary>
        /// 获取年表当天开关灯时间
        /// </summary>
        /// <returns></returns>
        public LFCDalParameter GetChronologyOfToday(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                LFCDalParameter result = new LFCDalParameter();
                result.BusinessType = BusinessType.GetChronologyOfToday;

                string sql = "Select * from tCTUOCDayCfg where TMonth = @tMonth and TDay = @tDay";
                tCTUOCDayCfg[] arr = conn.Query<tCTUOCDayCfg>(sql, new { tMonth = DateTime.Now.Month, tDay = DateTime.Now.Day}).ToArray();
                result.BusinessObject = arr;

                return result;
            }
        }

        /// <summary>
        /// 获取当天临时预约计划
        /// </summary>
        /// <returns></returns>
        public LFCDalParameter GetPlanFromLampTmpCtrlCfg(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                LFCDalParameter result = new LFCDalParameter();
                result.BusinessType = BusinessType.GetPlanFromLampTmpCtrlCfg;

                string sql = "Select * from tLampTmpCtrlCfg where   PlanOptDateTime>datetime('now', 'localtime')  and CtrlStatus = 0 ";
                tLampTmpCtrlCfg[] arr = conn.Query<tLampTmpCtrlCfg>(sql).ToArray();
                result.BusinessObject = arr;

                return result;
            }
        }

        /// <summary>
        /// 获取当天周期性预约计划
        /// </summary>
        /// <returns></returns>
        public LFCDalParameter GetPlanFromLampWeekCtrlCfg(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                LFCDalParameter result = new LFCDalParameter();
                result.BusinessType = BusinessType.GetPlanFromLampWeekCtrlCfg;

                int week = (int)DateTime.Now.DayOfWeek;
                if (week == 0) week = 7;
                string sql = "Select * from tLampWeekCtrlCfg where (CmdType=@week or CmdType=0) and CtrlStatus = 0";
                tLampWeekCtrlCfg[] arr = conn.Query<tLampWeekCtrlCfg>(sql, new { week = week}).ToArray();
                result.BusinessObject = arr;

                return result;
            }
        }

        /// <summary>
        /// 查询周期性预约计划
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public LFCDalParameter QueryPlanFromLampWeekCtrlCfg(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                LFCDalParameter result = new LFCDalParameter();
                result.BusinessType = BusinessType.QueryPlanFromLampWeekCtrlCfg;

                pLampTmpCtrlCfg data = (pLampTmpCtrlCfg)para.BusinessObject;
                string sql = "";
                tLampWeekCtrlCfg[] arr = new tLampWeekCtrlCfg[0];
                if (data.OptType == -1)
                {
                    if(data.CmdType==0x0a)
                    {
                        sql = "Select * from tLampWeekCtrlCfg where CtuID=@ctuID";
                        arr = conn.Query<tLampWeekCtrlCfg>(sql, new { CtuID = para.CTUID }).ToArray();
                    }
                    else
                    {
                        sql = "Select * from tLampWeekCtrlCfg where CmdType=@cmdType and CtuID=@ctuID";
                        arr = conn.Query<tLampWeekCtrlCfg>(sql, new { cmdType = data.CmdType, CtuID = para.CTUID }).ToArray();
                    }
                }
                else
                {
                    if (data.CmdType == 0x0a)
                    {
                        sql = "Select * from tLampWeekCtrlCfg where OptType=@optType and LampOrGrpNo=@lampOrGrpNo and CtuID=@ctuID";
                        arr = conn.Query<tLampWeekCtrlCfg>(sql, new { optType = data.OptType, lampOrGrpNo = data.No, CtuID = para.CTUID }).ToArray();
                    }
                    else
                    {
                        sql = "Select * from tLampWeekCtrlCfg where CmdType=@cmdType and OptType=@optType and LampOrGrpNo=@lampOrGrpNo and CtuID=@ctuID";
                        arr = conn.Query<tLampWeekCtrlCfg>(sql, new { cmdType = data.CmdType, optType = data.OptType, lampOrGrpNo = data.No, CtuID = para.CTUID }).ToArray();
                    }
                }
                result.BusinessObject = arr;

                return result;
            }
        }

        //public LFCDalParameter GetMaxNoFromElecData(LFCDalParameter para)
        //{
        //    using (IDbConnection conn = GetSqlConn())
        //    {
        //        LFCDalParameter result = new LFCDalParameter();
        //        result.BusinessType = BusinessType.GetMaxNoFromElecData;

        //        string sql = "Select max(LampDataRecNo) from tLampHisDataRec where CTUID = @CtuID";
        //        tLampHisDataRec[] arr = conn.Query<tLampHisDataRec>(sql, new { CtuID = para.CTUID }).ToArray();
        //        result.BusinessObject = arr[0].LampDataRecNo;

        //        return result;
        //    }
        //}

        private int GetMaxNoFromElecData(string ctuID)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                int result = 0;

                string sql = "Select * from tLampHisDataRec where CTUID = @CtuID and LampDataRecNo=(Select max(LampDataRecNo) from tLampHisDataRec where CTUID = @CtuID)";
                tLampHisDataRec[] arr = conn.Query<tLampHisDataRec>(sql, new { CtuID = ctuID }).ToArray();

                //string sql = "Select max(LampDataRecNo) from tLampHisDataRec ";
                //tLampHisDataRec[] arr = conn.Query<tLampHisDataRec>(sql).ToArray();
                result = arr[0].LampDataRecNo;

                return result;
            }
        }

        public void SaveElecData(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                tLampHisDataRec[] data =para.BusinessObject as tLampHisDataRec[];
                if (data.Length>0)
                {
                    int maxNo = GetMaxNoFromElecData(data[0].CTUID)+1;
                    foreach(tLampHisDataRec dataRec in data)
                    {
                        dataRec.LampDataRecNo = maxNo++;
                    }
                    string sql = "insert into tLampHisDataRec(LampDataRecNo,LampNo,LampU,LampI,LampAP,LampVP,LampPF,LampStatus,GetDateTime,CommValue,isUpLoaded,UpLoadDTime,Memo,CtuID) VALUES";
                    sql += " (@LampDataRecNo,@LampNo,@LampU,@LampI,@LampAP,@LampVP,@LampPF,@LampStatus,@GetDateTime,@CommValue,@isUpLoaded,@UpLoadDTime,@Memo,@CtuID)";
                    int result = conn.Execute(sql, data);
                    if (result > 0)
                    {
                        Console.WriteLine("插入成功！");
                    }
                    else
                    {
                        Console.WriteLine("插入失败！");
                    }
                }
            }
        }

        /// <summary>
        /// 将临时预约的状态修改成完成态
        /// </summary>
        /// <param name="para"></param>
        public void UpdateLampTmpCtrlStatus(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                pLampTmpCtrlCfg data = (pLampTmpCtrlCfg)para.BusinessObject;
                string sql = "Update tLampTmpCtrlCfg set CtrlStatus=1  where OptType = @optType and  cfgNo = @cfgNo and CTUID = @CtuID";
                int result = conn.Execute(sql, new { optType = data.OptType, cfgNo = data.No, CtuID = para.CTUID }); if (result > 0)
                {
                    Console.WriteLine("更新成功！");
                }
                else
                {
                    Console.WriteLine("更新失败！");
                }
            }
        }

        /// <summary>
        /// 新增单灯分组设置功能
        /// </summary>
        /// <param name="data"></param>
        public void SaveLampGroupCfg(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                string sql = "";
                tLampGrpCfg[] data = para.BusinessObject as tLampGrpCfg[];
                Dictionary<string, string> dictKey = new Dictionary<string, string>();
                foreach(tLampGrpCfg cfg in data)//单灯组配置采用替换的方式，每次配置都删除上次配置
                {
                    string key = cfg.CTUID + "#" + cfg.LampNo;
                    if(!dictKey.ContainsKey(key))
                    {
                        dictKey.Add(key, key);
                        sql = " delete from tLampGrpCfg where ctuID=@ctuID and lampNo=@lampNo";
                        conn.Execute(sql, new { ctuID = cfg.CTUID, lampNo = cfg.LampNo });
                    }
                }
                sql = "replace into tLampGrpCfg(GrpNo,LampNo,OptDatetime,OptType,DownloadTick,DLFailedTimes,DLDateTime,CtuID) VALUES";
                sql += " (@GrpNo,@LampNo,@OptDatetime,@OptType,@DownloadTick,@DLFailedTimes,@DLDateTime,@CtuID)";
                int result = conn.Execute(sql, data);
                if (result > 0)
                {
                    Console.WriteLine("插入成功！");
                }
                else
                {
                    Console.WriteLine("插入失败！");
                }
            }
        }

        /// <summary>
        /// 修改单灯配置的下发标志
        /// </summary>
        /// <param name="para"></param>
        public void UpdateLampGrpCfgStatus(LFCDalParameter para)
        {
            using (IDbConnection conn = GetSqlConn())
            {
                tLampGrpCfg data = para.BusinessObject as tLampGrpCfg;

                string sql = "update tLampGrpCfg set downloadTick=@downloadTick where lampNo=@lampNo and ctuID=@ctuID";
                int result = conn.Execute(sql, new { data.DownloadTick, data.LampNo, data.CTUID });
                if (result > 0)
                {
                    Console.WriteLine("更新成功！");
                }
                else
                {
                    Console.WriteLine("更新失败！");
                }
            }
        }
    }
}
