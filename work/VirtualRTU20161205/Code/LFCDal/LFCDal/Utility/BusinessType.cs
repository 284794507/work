using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFCDal.Utility
{
    public enum BusinessType:int
    {
        GetCtuInfo = 1,
        GetLampList = 2,
        GetSysRunStatus = 3,
        GetLampStatusByLampNo = 4,
        GetLampDataRecByLampNo = 5,
        GetLampTempCtrlCfg = 6,
        DelLampTmpCtrlCfgByNo = 7,
        DelAllLampTmpCtrlCfg = 8,
        AddLampTmpCtrlCfg = 9,
        DelLampWeekCtrlCfgByWeekDay = 10,
        DelAllLampWeekCtrlCfg = 11,
        AddLampWeekCtrlCfg = 12,
        GetChronologyOfToday = 13,
        GetPlanFromLampTmpCtrlCfg = 14,
        GetPlanFromLampWeekCtrlCfg = 15,
        GetMaxNoFromElecData = 16,
        SaveElecData = 17,
        GetMaxLampTmpCtrlCfgNo = 18,
        DelLampTmpCtrlCfgByCfgNo = 19,
        QueryLampTempCtrlCfg  =  20,
        UpdateLampTmpCtrlStatus = 21,
        QueryPlanFromLampWeekCtrlCfg = 22,
        SaveLampGroupCfg = 23,
        UpdateLampGrpCfgStatus = 24
    }
}
