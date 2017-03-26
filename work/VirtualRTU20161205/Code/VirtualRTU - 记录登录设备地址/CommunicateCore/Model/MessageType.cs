using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicateCore.Model
{    
    public enum MessageType
    {
        login = 1,
        loginBack = 2,

        heartBeat = 3,
        heartBeatBack = 4,

        alarm = 5,

        queryTerminalInfo = 6,
        queryTerminalInfoBack = 7,

        reSet = 8,
        reSetBack = 9,

        realTimeCtrl = 10,
        realTimeCtrlBack = 11,

        queryElecData = 12,
        queryElecDataBack = 13,

        queryOtherParamter = 14,
        queryOtherParamterBack = 15,

        setTime = 16,
        setTimeBack = 17,

        setBasicParamter = 18,
        setBasicParamterBack = 19,

        setCommParamter = 20,
        setCommParamterTimeBack = 21,

        setNetWorkParamter = 22,
        setNetWorkParamterBack = 23,

        setLampParamter = 24,
        setLampParamterBack = 25,

        setAlarmLimit = 26,
        setAlarmLimitBack = 27,

        setYearTable = 28,
        setYearTableBack = 29,

        setTempCtrl = 30,
        setTempCtrlBack = 31,

        setSelfRun = 32,
        setSelfRunBack = 33,

        setLight = 34,
        setLightBack = 35,

        setLeakLimit = 36,
        setLeakLimitBack = 37,

        queryTime = 38,
        queryTimeBack = 39,

        queryBasicParamter = 40,
        queryBasicParamterBack = 41,

        queryCommParamter = 42,
        queryCommParamterTimeBack = 43,

        queryNetWorkParamter = 44,
        queryNetWorkParamterBack = 45,

        queryLampParamter = 46,
        queryLampParamterBack = 47,

        queryAlarmLimit = 48,
        queryAlarmLimitBack = 49,

        queryYearTable = 50,
        queryYearTableBack = 51,

        queryTempCtrl = 52,
        queryTempCtrlBack = 53,

        querySelfRun = 54,
        querySelfRunBack = 55,

        queryLight = 56,
        queryLightBack = 57,

        queryLeakLimit = 58,
        queryLeakLimitBack = 59,
        
        forward= 60,
        forwardBack = 61,

        upgrade = 62,
        upgradeBack = 63
    }
}
