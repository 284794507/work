using CTMDAL.Model;
using CTMDAL.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMDAL.Interface
{
    public interface ICRUD
    {
        CTMDalParameter GetPlatFormInfo();
        
        CTMDalParameter GetCollectorInfo();
        
        CTMDalParameter GetNextUpgradeInfoByID(CTMDalParameter data);

        CTMDalParameter GetNextUpgradeInfoByIDAndNo(CTMDalParameter data);

        CTMDalParameter GetUpgradeStatus(CTMDalParameter data);

        CTMDalParameter GetRouteByAddr(CTMDalParameter data);

        CTMDalParameter GetRouteList(CTMDalParameter data);

        CTMDalParameter GetGprsCommStatus(CTMDalParameter data);

        CTMDalParameter GetSupplyInfo(CTMDalParameter data);

        void InsertCollectorInfo(CTMDalParameter data);

        void InsertIntoPlatForm(CTMDalParameter data);
        
        void InsertUpdateFileToDB(CTMDalParameter data);

        void AddUpgradeFile(CTMDalParameter data);

        //void AddNewPlatForm(CTMDalParameter data);

        void AddRoute(CTMDalParameter data);

        void BindSupplyCode(CTMDalParameter data);

        void InsertIntoMeter(CTMDalParameter data);


        void UpdateCollectorInfo(CTMDalParameter data);

        void UpdateCollectorStatus(CTMDalParameter data);

        void UpdateCollectorPhase(CTMDalParameter data);

        void UpdateDataRecRTM(CTMDalParameter data);

        void UpdateCollectorWireLessStatus(CTMDalParameter data);

        void UpdateCollectorPLCCommStatus(CTMDalParameter data);

        void UpdateCollectorMasterCommStatus(CTMDalParameter data);

        void UpdateCollectorMasterComm(CTMDalParameter data);

        void UpdateCollectorUp(CTMDalParameter data);

        void UpdateCollectorGPRS(CTMDalParameter data);

        void UpdateCollectorWireLess(CTMDalParameter data);
    }
}
