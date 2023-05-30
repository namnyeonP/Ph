using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EOL_BASE.클래스;

/*
 * variables에서는 변수 설정만 한다.
 */
namespace EOL_BASE
{
    public partial class MainWindow
    {
        // 다음과 같이 작성하여 명시한 후에 RelayOn이나 RelayOff 메소드에서 파라미터에 아래 선언한 상수를 넣는다.
        // 모듈과 다르게 팩 프로젝트의 경우 사용하는 릴레이 수가 많은 프로젝트들도 있으며 해당 프로젝트의 코드에 모두 IDO_'no' 형태로 사용할 시
        // 단순 혼동 뿐만 아니라 검사기 또는 팩을 망가뜨릴 수 있다.

        // 릴레이 제어 상수 선언은 다음과 같이 한다.
        // Relay_'사용할 이름' = "IDO_'no'";
        // 앞에 Relay라는 이름을 붙이는 이유는 코드 작성 시 Relay로만 검색 가능하도록 하기 위함이다.
        #region Declare relay const variables

        // ex)
        //const string Relay_BMSPower = "IDO_15";
        //const string Relay_SignalPower = "IDO_16";
        //const string Relay_PackPlus2Chassis = "IDO_17";
        //const string Relay_PackMinus2Chassis = "IDO_18";

        #endregion

        public TowerLamp tlamp = null;

        #region 값 판정 부분

        #region cycler

        private const string _CYCLER_SAFETY = "CYCLER_SAFETY_STOPPED";
        private const string _VOLTAGE_PIN_SENSING_FAIL = "VOLTAGE_PIN_SENSING_FAIL";

        #endregion

        #region common

        private const string _VALUE_NOT_MATCHED = "VALUE_NOT_MATCHED";
        private const string _DEVICE_NOT_READY = "DEVICE_NOT_READY";
        public string _EMG_STOPPED = "EMERGENCY_STOPPED";
        private const string _COMM_FAIL = "COMM_FAIL";
        public string _CHECK_TERMINAL = "EMERGENCY_STOPPED_CHECK_THE_TERMINAL";

        //210112 wjs add (PJH)
        private const string _CMC_OPEN_FAIL = "CMC_OPEN_FAIL";

        //200611 추가
        public const string _DATA_BLANK = "DATA_BLANK";

        public string _MES_CELL_DATA_NOT_RECIVED = "MES_CELL_DATA_NOT_RECIEVED";

        #endregion

        #region chroma

        //private const string _GFI_FAULT = "GFI_FAULT";

        #region 기존 EOL 문자열

        //public string _CASE_113 = "USER STOP";          // 사용자가 계측기 전면부 정지 버튼 누를 시 발생
        //public string _CASE_114 = "CAN NOT TEST";       // 원인미상(계측기에서 수신됨)
        //public string _CASE_115 = "TESTING";            // 원인미상(계측기에서 수신됨)
        //public string _CASE_112 = "STOP";               // 원인미상(계측기에서 수신됨)
        //public string _CASE_33 = "DC - HI";             // 내전압 검사 시 High 채널 문제 발생
        //public string _CASE_34 = "DC - LO";             // 내전압 검사 시 Low 채널 문제 발생
        //public string _CASE_35 = "DC - ARC";            // 내전압 검사 시 ARC 감지
        //public string _CASE_36 = "DC - IO";             // 원인미상(계측기에서 수신됨)
        //public string _CASE_37 = "DC - CHECK LOW";      // 원인미상(계측기에서 수신됨)
        //public string _CASE_38 = "DC - ADV OVER";       // 원인미상(계측기에서 수신됨)
        //public string _CASE_39 = "DC - ADI OVER";       // 원인미상(계측기에서 수신됨)
        //public string _CASE_43 = "DC - IO-F";           // 원인미상(계측기에서 수신됨)
        //public string _CASE_49 = "IR - HI";             // 절연저항 검사 시 High 채널 문제 발생
        //public string _CASE_50 = "IR - LO";             // 절연저항 검사 시 Low 채널 문제 발생
        //public string _CASE_52 = "IR - IO";             // 원인미상(계측기에서 수신됨)
        //public string _CASE_54 = "IR - ADV OVER";       // 원인미상(계측기에서 수신됨)
        //public string _CASE_55 = "IR - ADI OVER";       // 원인미상(계측기에서 수신됨)
        //public string _CASE_120 = "DC / IR - GR CONT."; // 원인미상(계측기에서 수신됨)
        //public string _CASE_121 = "DC / IR - TRIPPED";  // 스테이션 상에서 모듈과 설비 간 절연이 깨진 경우 발생(누설전류)

        #endregion

        #region 변경 EOL 문자열_ver1

        public string _CASE_113 = "REINPUT_USER STOP";          // 사용자가 계측기 전면부 정지 버튼 누를 시 발생
        public string _CASE_114 = "REINPUT_CAN NOT TEST";       // 원인미상(계측기에서 수신됨)
        public string _CASE_115 = "REINPUT_TESTING";            // 원인미상(계측기에서 수신됨)
        public string _CASE_112 = "REINPUT_STOP";               // 원인미상(계측기에서 수신됨)
        public string _CASE_33 = "-33";                         // 내전압 검사 시 High 채널 문제 발생 
        public string _CASE_49 = "-49";                         // 절연저항 검사 시 High 채널 문제 발생
        public string _CASE_34 = "-34";                         // 내전압 검사 시 Low 채널 문제 발생
        public string _CASE_50 = "-50";                         // 절연저항 검사 시 Low 채널 문제 발생
        public string _CASE_35 = "-35";                         // 내전압 검사 시 ARC 감지
        public string _CASE_36 = "-36";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_52 = "-52";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_37 = "-37";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_38 = "-38";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_54 = "-54";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_39 = "-39";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_55 = "-55";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_120_DC = "REINPUT_DC_GR_CONT";         // 원인미상(계측기에서 수신됨)
        public string _CASE_120_IR = "REINPUT_IR_GR_CONT";       //문자열 공용
        public string _CASE_121_DC = "REINPUT_DC_TRIPPED";         // 스테이션 상에서 모듈과 설비 간 절연이 깨진 경우 발생(누설전류)
        public string _CASE_121_IR = "REINPUT_IR_TRIPPED";       //문자열 공용
        public string _CASE_43 = "REINPUT_DC_IO-F";             // 원인미상(계측기에서 수신됨)
        public string _HIPOT_EMO = "REINPUT_EMO";               // 200616 추가
        public string _HIPOT_OVERFLOW = "REINPUT_OVERFLOW";     // 200616 추가

        #endregion

        //200609 추가
        public string _CHAN_HI_FAIL = "SET_CH_FAIL(HI)";
        public string _CHAN_LO_FAIL = "SET_CH_FAIL(LO)";

        #endregion

        #region device

        private const string _TEMP_OVER = "AMBIENT_TEMP_OVER";
        private const string _SENSING_BOARD_CHECK = "SENSING_BOARD_CHECK";
        private const string _DAQ_VOLT_SENSING_FAIL = "DAQ_VOLT_SENSING_FAIL";

        #endregion

        #region bms

        private const string _NOT_POS_MSG = "NOT_POSITIVE_MSG";
        private const string _NOT_NEG_MSG = "NOT_NEGATIVE_MSG";
        private const string _NOT_MODE_CHANGED = "NOT_MODE_CHANGED";

        #endregion

        #region with PLC

        public string _JIG_NOT_ACTIVE = "JIG_NOT_ACTIVE";
        public string _LV_IS_CONNECTED = "LV_IS_CONNECTED";
        public string _JIG_NOT_DEACTIVE = "JIG_NOT_DEACTIVE";

        #endregion

        #region Return Error Message(Collect Item Test)
        //public const string _JIG_DOWN_TIMEOUT = "JIG_DOWN_TIMEOUT";
        //public const string _JIG_DOWN_BIT_OFF_TIMEOUT = "JIG_DOWN_BIT_OFF_TIMEOUT";
        #endregion

        #endregion
    }
}
