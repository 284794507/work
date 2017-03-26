using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMDAL.Utility
{
    public enum BusinessType
    {
        GetPlatFormInfo = 1,
        GetCollectorInfo = 2,
        GetNextUpgradeInfoByID = 3,
        GetNextUpgradeInfoByIDAndNo = 4,
        GetUpgradeStatus = 5,
        InsertIntoPlatForm = 6,
        InsertUpdateFileToDB = 7,
        UpdateDataRecRTM = 8,
        UpdateCollectorInfo = 9,
        UpdateCollectorWireLessStatus = 10,
        UpdateCollectorMasterCommStatus = 11,
        UpdateCollectorPLCCommStatus = 12,
        UpdateCollectorMasterComm = 13,
        UpdateCollectorStatus = 14,
        UpdateCollectorPhase = 15,
        AddUpgradeFile = 16,
        UpdateCollectorUp = 17,
        GetRouteByAddr = 18,
        GetRouteList = 19,
        UpdateCollectorGPRS =  20,
        GetGprsCommStatus = 21,
        AddNewPlatForm = 22,
        UpdateCollectorWireLess = 23,
        AddRoute = 24,
        InsertIntoMeter=25,
        InsertCollectorInfo=26,
        GetSupplyInfo=27,
        BindSupplyCode=28

    }
}
