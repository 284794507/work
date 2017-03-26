using LFCDal.Model;
using LFCDal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFCDal.Interface
{
    public interface IDBBLL
    {
        LFCDalParameter GetALLCtuInfo();

        LFCDalParameter GetSysRunStatus();

        LFCDalParameter GetLampList();

        LFCDalParameter GetLampStatusByLampNo(LFCDalParameter para);

        LFCDalParameter GetLampDataRecByLampNo(LFCDalParameter para);

        LFCDalParameter GetLampTempCtrlCfg();

        void DelLampTmpCtrlCfgByNo(LFCDalParameter para);

        void DelAllLampTmpCtrlCfg(LFCDalParameter para);

        void AddLampTmpCtrlCfg(LFCDalParameter para);

        void DelLampWeekCtrlCfgByWeekDay(LFCDalParameter para);

        void DelAllLampWeekCtrlCfg(LFCDalParameter para);

        void AddLampWeekCtrlCfg(LFCDalParameter para);

        LFCDalParameter GetChronologyOfToday(LFCDalParameter para);

        LFCDalParameter GetPlanFromLampTmpCtrlCfg(LFCDalParameter para);

        LFCDalParameter GetPlanFromLampWeekCtrlCfg(LFCDalParameter para);

        //LFCDalParameter GetMaxNoFromElecData(LFCDalParameter para);

        void SaveElecData(LFCDalParameter para);

        LFCDalParameter GetMaxLampTmpCtrlCfgNo(LFCDalParameter para);

        void DelLampTmpCtrlCfgByCfgNo(LFCDalParameter para);

        LFCDalParameter QueryLampTempCtrlCfg(LFCDalParameter para);

        void UpdateLampTmpCtrlStatus(LFCDalParameter para);

        LFCDalParameter QueryPlanFromLampWeekCtrlCfg(LFCDalParameter para);

        void SaveLampGroupCfg(LFCDalParameter para);

        void UpdateLampGrpCfgStatus(LFCDalParameter para);
    }
}
