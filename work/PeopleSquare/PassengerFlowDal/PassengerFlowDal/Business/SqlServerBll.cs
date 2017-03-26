using Dapper;
using PassengerFlowDal.Interface;
using PassengerFlowDal.Model;
using PassengerFlowDal.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerFlowDal.Business
{
    public class SqlServerBll: ICRUD
    {
        private static SqlServerBll _SqlServerBll;

        private static SqlServerBll Instance
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

        internal SqlConnection GetSqlConn(string connStr)
        {
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            return conn;
        }

        internal SqlConnection GetSqlConn()
        {
            string connStr = ConfigurationManager.ConnectionStrings["SqlServerConnStr"].ToString();
            return GetSqlConn(connStr);
        }

        /// <summary>
        /// 获取区域信息
        /// </summary>
        /// <param name="para"></param>
        public DalParameter GetAreaInfo(DalParameter para)
        {
            return AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Return<DalParameter>(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        string name = para.BusinessObject.ToString();
                        string sql = "";

                        sql = "with  allArea as (";
                        sql += " select objid,parentid,areaname,origin_lon,origin_lat,width,height,center_lon,center_lat,zoomlevel";
                        sql += " from tabarea where areaname='" + name + "'";
                        sql += " Union all ";
                        sql += " (select a.objid,a.parentid,a.areaname,a.origin_lon,a.origin_lat,a.width,a.height,a.center_lon,a.center_lat,a.zoomlevel ";
                        sql += " from tabarea as a inner join allArea as b on a.parentid=b.objid)";
                        sql += " )Select objid,parentid,areaname,origin_lon,origin_lat,width,height,center_lon,center_lat,zoomlevel";
                        sql += " from allArea";

                        TabArea[] arrInfo = conn.Query<TabArea>(sql).ToArray();

                        DalParameter result = new DalParameter();
                        result.BusinessObject = arrInfo;
                        result.BusinessType = BusinessType.GetAreaInfo;
                        
                        return result;
                    }
                });
        }

        /// <summary>
        /// 新增同步时间
        /// </summary>
        /// <param name="para"></param>
        public void AddCheckTime(DalParameter para)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TabTimeSync info = para.BusinessObject as TabTimeSync;
                        string sql = "";
                        sql = "update TabTimeSync set Valid=0 where DevMac=@Devmac";
                        int result = conn.Execute(sql, info);

                        sql = "insert into TabTimeSync(DevMac,ServerTime,DevTime,ModuleNo,Valid,DeltaTime) Values";
                        sql += " (@DevMac,@ServerTime,@DevTime,@ModuleNo,@Valid,@DeltaTime)";
                        result = conn.Execute(sql, info);
                        if(result>0)
                        {
                            Share.Instance.WriteLog("同步时间新增成功！");
                        }
                        else
                        {
                            Share.Instance.WriteLog("同步时间新增失败！");
                        }
                    }
                });
        }

        /// <summary>
        /// 新增采集数据
        /// </summary>
        /// <param name="para"></param>
        public void AddData(DalParameter para)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    using (IDbConnection conn = GetSqlConn())
                    {
                        TabCurData info = para.BusinessObject as TabCurData;
                        string sql = "";
                        sql = "merge into TabCurData a";
                        sql += " using (select '" + info.DevMac + "' DevMac ,'" + info.PhoneMac + "' PhoneMac ,'" + info.ModuleNo + "' ModuleNo ) b ";
                        sql += " on a.DevMac=b.DevMac and a.ModuleNo=b.ModuleNo and a.PhoneMac=b.PhoneMac";
                        sql += " when matched then";
                        sql += " update set a.DevTime=@DevTime,a.RealTime=@RealTime,a.Channel=@Channel,a.RSSI=@RSSI";
                        sql += " when not matched then";
                        sql += " insert(DevMac,PhoneMac,ModuleNo,DevTime,RealTime,Channel,RSSI) values";
                        sql += " (@DevMac,@PhoneMac,@ModuleNo,@DevTime,@RealTime,@Channel,@RSSI);";
                        int result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            Share.Instance.WriteLog("实时采集数据新增成功！");
                        }
                        else
                        {
                            Share.Instance.WriteLog("实时采集数据新增失败！");
                        }

                        sql = "insert into TabHistoryData(DevMac,PhoneMac,ModuleNo,DevTime,RealTime,Channel,RSSI)";
                        sql += " (select DevMac,PhoneMac,ModuleNo,DevTime,RealTime,Channel,RSSI from TabCurData ";
                        sql += " where DevMac=@DevMac and ModuleNo=@ModuleNo and PhoneMac=@PhoneMac)";
                        result = conn.Execute(sql, info);
                        if (result > 0)
                        {
                            Share.Instance.WriteLog("历史采集数据新增成功！");
                        }
                        else
                        {
                            Share.Instance.WriteLog("历史采集数据新新增失败！");
                        }
                    }
                });
        }
    }
}
