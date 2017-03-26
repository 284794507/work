using CTMDAL.Interface;
using CTMDAL.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CTMDAL.WcfServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“CTMDal”。
    public class CTMDal : ICTMDal
    {
        private ICRUD crud = null;
        public CTMDalParameter GetInfo(CTMDalParameter para)
        {
            CTMDalParameter result = new CTMDalParameter();

            crud = ContainerHandler.Instance.GetService();
            switch(para.BusinessType)
            {
                case BusinessType.GetPlatFormInfo:
                    result = crud.GetPlatFormInfo();
                    break;
                case BusinessType.GetCollectorInfo:
                    result = crud.GetCollectorInfo();
                    break;
                case BusinessType.GetNextUpgradeInfoByID:
                    result = crud.GetNextUpgradeInfoByID(para);
                    break;
                case BusinessType.GetNextUpgradeInfoByIDAndNo:
                    result = crud.GetNextUpgradeInfoByIDAndNo(para);
                    break;
                case BusinessType.GetUpgradeStatus:
                    result = crud.GetUpgradeStatus(para);
                    break;
                case BusinessType.GetRouteByAddr:
                    result = crud.GetRouteByAddr(para);
                    break;
                case BusinessType.GetRouteList:
                    result = crud.GetRouteList(para);
                    break;
                case BusinessType.GetGprsCommStatus:
                    result =crud.GetGprsCommStatus(para);
                    break;
                case BusinessType.GetSupplyInfo:
                    result = crud.GetSupplyInfo(para);
                    break;
            }

            return result;
        }

        public void DelInfo(CTMDalParameter para)
        {
            crud = ContainerHandler.Instance.GetService();
            //switch (para.BusinessType)
            //{

            //}
        }

        public void AddInfo(CTMDalParameter para)
        {
            crud = ContainerHandler.Instance.GetService();
            switch (para.BusinessType)
            {
                case BusinessType.InsertCollectorInfo:
                    crud.InsertCollectorInfo(para);
                    break;
                case BusinessType.InsertIntoPlatForm:
                    crud.InsertIntoPlatForm(para);
                    break;
                case BusinessType.InsertUpdateFileToDB:
                    crud.InsertUpdateFileToDB(para);
                    break;
                case BusinessType.AddUpgradeFile:
                    crud.AddUpgradeFile(para);
                    break;
                //case BusinessType.AddNewPlatForm:
                    //crud.AddNewPlatForm(para);
                    //break;
                case BusinessType.AddRoute:
                    crud.AddRoute(para);
                    break;
                case BusinessType.BindSupplyCode:
                    crud.BindSupplyCode(para);
                    break;
                case BusinessType.InsertIntoMeter:
                    crud.InsertIntoMeter(para);
                    break;
            }
        }

        public void UpdateInfo(CTMDalParameter para)
        {
            crud = ContainerHandler.Instance.GetService();
            switch (para.BusinessType)
            {
                case BusinessType.UpdateCollectorInfo:
                    crud.UpdateCollectorInfo(para);
                    break;
                case BusinessType.UpdateDataRecRTM:
                    crud.UpdateDataRecRTM(para);
                    break;
                case BusinessType.UpdateCollectorWireLessStatus:
                    crud.UpdateCollectorWireLessStatus(para);
                    break;
                case BusinessType.UpdateCollectorPLCCommStatus:
                    crud.UpdateCollectorPLCCommStatus(para);
                    break;
                case BusinessType.UpdateCollectorMasterCommStatus:
                    crud.UpdateCollectorMasterCommStatus(para);
                    break;
                case BusinessType.UpdateCollectorMasterComm:
                    crud.UpdateCollectorMasterComm(para);
                    break;
                case BusinessType.UpdateCollectorPhase:
                    crud.UpdateCollectorPhase(para);
                    break;
                case BusinessType.UpdateCollectorUp:
                    crud.UpdateCollectorUp(para);
                    break;
                case BusinessType.UpdateCollectorGPRS:
                    crud.UpdateCollectorGPRS(para);
                    break;
                case BusinessType.UpdateCollectorWireLess:
                    crud.UpdateCollectorWireLess(para);
                    break;

            }
        }
    }
}
