using PassengerFlowDal.Model;
using PassengerFlowDal.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PassengerFlowServer.Utility
{
    public class DBHandler
    {
        private static DBHandler _DBHandler;
        public static DBHandler Instance
        {
            get
            {
                if(_DBHandler ==null)
                {
                    _DBHandler = new DBHandler();
                }
                return _DBHandler;
            }
        }

        private static IDalSvr DalProxy;

        public static IDalSvr GetProxy
        {
            get
            {
                if(DalProxy==null)
                {
                    CreateChannel();
                }
                return DalProxy;
            }
        }

        private static void CreateChannel()
        {
            string dbEndPointName = ConfigurationManager.AppSettings["DBEndPoint"];
            ChannelFactory<IDalSvr> factory = new ChannelFactory<IDalSvr>(dbEndPointName);
            DalProxy = factory.CreateChannel();
        }

        //private void MonitorChannel()
        //{
        //    AspectF.Define.Retry(Share.Instance.CatchExpection,()=> { CreateChannel(); })
        //        .Do(() =>
        //        {
        //            if(((IClientChannel)DalProxy).State==CommunicationState.Closed)
        //            {
        //                DalProxy = null;
        //            }
        //        });
        //}
        /// <summary>
        /// 新增同步时间
        /// </summary>
        /// <param name="info"></param>
        public void AddCheckTime(TabTimeSync info)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    DalParameter para = new DalParameter();
                    para.BusinessObject = info;
                    para.BusinessType = BusinessType.AddCheckTime;
                    GetProxy.AddInfo(para);
                });
        }

        /// <summary>
        /// 新增实时采集数据
        /// </summary>
        /// <param name="info"></param>
        public void AddCurData(TabCurData info)
        {
            AspectF.Define.Retry(Share.Instance.CatchExpection)
                .Do(() =>
                {
                    DalParameter para = new DalParameter();
                    para.BusinessObject = info;
                    para.BusinessType = BusinessType.AddData;
                    GetProxy.AddInfo(para);
                });
        }
    }
}
