﻿
using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using Microsoft.Win32;
using MiniScheduler;
using Peak.Can.Basic;
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;

using EOL_BASE.Forms;
using System.Globalization;

using System.Windows.Threading;

namespace EOL_BASE
{

    #region Enum

    public enum CMC_i0072
    {
        CMC_SM01_a_E_OV = 0,
        CMC_SM01_a_E_UV = 1,
        CMC_SM01_a_O_OV = 2,
        CMC_SM01_a_O_UV = 3,
        CMC_SM03 = 4,
        CMC_SM04_b = 5,
        CMC_SM05_a = 8,
        CMC_SM06_a = 12,
        CMC_SM40_a = 13,
        CMC_SM34_a = 9,
        CMC_SM44_a = 10,
        CMC_SM45_a = 11,
    }

    public enum CMC_FAULT1_STATUS
    {
        CT_UV_FLT = 0,
        CT_OV_FLT = 1,
        AN_UT_FLT = 2,
        AN_OT_FLT = 3,
        IS_OC_FLT = 4,
        IS_OL_FLT = 5,
        I2C_ERR_FLT = 6,
        GPIO0_WUP_FLT = 7,
        CSB_WUP_FLT = 8,
        COM_ERR_FLT = 9,
        SM21_COM_LOSS_FLT = 10,
        SM25_VPWR_LV_FLT = 11,
        SM24_VPWR_OV_FLT = 12,
        COM_ERR_OVR_FLT = 13,
        RESET_FLT = 14,
        POR = 15,
    }

    public enum CMC_FAULT2_STATUS
    {
        SM31_FUSE_ERR_FLT = 0,
        SM13_DED_ERR_FLT = 1,
        OSC_ERR_FLT = 2,
        CB_OPEN_FLT = 3,
        CB_SHORT_FLT = 4,
        GPIO_SHORT_FLT = 4,
        AN_OPEN_FLT = 6,
        IDLE_MODE_FLT = 7,
        SM11_IC_TSD_FLT = 8,
        SM12_GND_LOSS_FLT = 9,
        SM07_ADC1_A_FLT = 10,
        SM07_ADC1_B_FLT = 11,
        SM10_VANA_UV_FLT = 12,
        SM15_VANA_OV_FLT = 13,
        SM09_VCOM_UV_FLT = 14,
        SM23_VCOM_OV_FLT = 15,
    }

    public enum CMC_FAULT3_STATUS
    {
        EOT_CB1=0,
        EOT_CB2=1,
        EOT_CB3=2,
        EOT_CB4=3,
        EOT_CB5=4,
        EOT_CB6=5,
        EOT_CB13=12,
        SM46_VCP_UV=13,
        DIAG_TO_FLT=14,
        CC_OVR_FLT=15
    }

    public enum CMC_COMMAND
    {
        SM01,
        SM02,
        SM03,
        SM03_a,
        SM03_b,
        SM03_c,
        SM03_d,
        SM04_1,
        SM04_2,
        SM05,
        SM06,
        SM06_1,
        SM34,
        SM40,
        SM41,
        SM44,
        SM45,
        SM_RESET,
        SM_ALL,
    }

    public enum StepError
    {
        TIME_ERROR,
        CUT_OFF_COND_ERROR,
        CURRENT_ERROR,
        WATT_ERROR,
        TARGET_VOLT_ERROR,
        NO_ERROR,
    }

    public enum ProgramType
    {
        VoltageInspector,
        HipotInspector,
        EOLInspector,
        Hipot_no_resin_Inspector
    }

    public enum DSPType
    {
        DSP_28335_def = 0,
        DSP_28377 = 1,
        DSP_28377_SDI = 2,
        DSP_28335_meb_1 = 3,
        DSP_28335_meb_234 = 4
    }

    public enum LogType
    {
        Info = 0,
        Fail = 1,
        Success = 2,
        Pass = 3,
        NG = 4,
        TEST = 5,
        CAN = 6,
        EMERGENCY = 7,
        RESPONSE = 8,
        REQUEST = 9,
        MANUALCONDITION = 10,
        PLC_RECV = 11,
        PLC_SEND = 12,
        CMC_RECV = 13,
        CMC_SEND = 14,
        DEVICE_CHECK = 98,
        CYCLER = 99,
        PERMISSION = 97
    }


    public enum CMD
    {
        INPUT_MC_ON,
        INPUT_MC_OFF,
        OUTPUT_MC_ON,
        CHARGE,
        DISCHARGE,
        OUTPUT_MC_OFF,
        REST,
        NULL
    }

    public enum OPMode
    {
        READY,
        CHARGE_CC,
        CHARGE_CV,
        DISCHARGE_CC,
        DISCHARGE_CV,
        CHARGE_CP,
        DISCHARGE_CP,
        NULL
    }

    public enum RECV_OPMode
    {
        READY,
        READY_TO_INPUT,
        READY_TO_CHARGE,
        CHARGE_CC,
        CHARGE_CV,
        DISCHARGE_CC,
        DISCHARGE_CV,
        CHARGE_CP,
        DISCHARGE_CP,
        COMPLETE,
        NULL
    }

    public enum BranchMode
    {
        NO_BRANCH,
        CHANNEL1,
        CHANNEL2
    }

    public enum ModelType
    {
        AUDI_NORMAL = 1,
        AUDI_MIRROR = 2,
        PORSCHE_NORMAL = 3,    // J1
        PORSCHE_MIRROR = 4,    // J1
        MASERATI_NORMAL = 5,
        //MASERATI_MIRROR = 6,
        //210312 wjs 마세라티 미러를 아래 모델로 변경
        PORSCHE_FACELIFT_NORMAL = 6,  // J1 PA
        PORSCHE_FACELIFT_MIRROR = 7,  // J1 PA
        E_UP = 8,
        MASERATI_M183_NORMAL = 9, //221101 wjs 마세라티 M183 노멀 추가
        DEFAULT,
    }

    #endregion
    
    public class singleLogItem
    {
        public LogType lt;
        public string Data;
    }

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        SafetyCyclerSetting scs = new SafetyCyclerSetting();

        #region Field

        /// <summary>
        /// 검사 항목 간 여유 시간
        /// </summary>
        int autobtwtick = 1;
        System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

        // Emergency Timer
        System.Timers.Timer emo_tmr, graph_tmr;
        bool pushedEmo = false;
        RegistryKey rkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\EOL_Trigger\\Relays");


        //string logaddr = @"C:\Users\Public\EOL_INSPECTION_LOG";
        //200214 ht changed(ljs confirm)
        static string @logaddr = @"D:\EOL_INSPECTION_LOG";

        static string @FILE_PATH_INSPECTION_RESULT = @"D:\EOL_INSPECTION_RESULT\";

        static string @FILE_PATH_SPAREPATSCOUNT = @"C:\EOL_SPAREPATSCOUNT_INFO\";

        /// <summary>
        /// 자동시작시에 흐르는 시간변수
        /// </summary>
        DateTime elapsedtime, nowTime;

        public Thread autoModeThread, loggerThread;
        public List<Process> totalProcessList = new List<Process>();

        /// <summary>
        /// 충방전을 위한 스케줄러 윈도우
        /// </summary>
        MiniScheduler.MainWindow mmw;

        public CyclerLimitSettingWindow clsw;

        /// <summary>
        /// 210121
        /// 로그인을 위한 UI 
        /// </summary>
        LoginWindow lg;

        public string Barcode = "";

        public bool isNeedDCIR = true;
        public bool isMESskip = false;

        public bool isStop = false;
        public bool isManual = false;
        //190108 by grchoi
        public string MonoFrame = "";

        private delegate void ReadDelegateHandler();

        public List<CellDetail> cellDetailList = new List<CellDetail>();
        public List<OcvDetail> ocvDetailList = new List<OcvDetail>();
        public List<string> dtcList = new List<string>();

        AmbientTemperatureSpecWindow atsw = new AmbientTemperatureSpecWindow();
        AmbientTempSetting setting = new AmbientTempSetting();

        /// <summary>
        /// BMS와 데이터 통신 시 해당 리스트에 계속 새로고침된다.
        /// </summary>
        public Dictionary<string, ReceiveType> bmsList = new Dictionary<string, ReceiveType>();

        public static string _BARCODE = "BarcodeReading";

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        Dictionary<string, List<TestItem>> groupDic = null;//;new Dictionary<string, List<TestItem>>();

        public CCycler cycler;
        public CNhtRS232_Receiver temps;
        public CRelay_Receiver relays;
        public CLambdaPower power;
        private CMES MES;
        public CKeysightDMM keysight;
        public CDAQ daq;
        public CPLC plc;
        public CBMS_CAN bms;

        public CBMS_CAN Hybrid_Instru_CAN;
        public CLambdaPower becm_power;
        public CLambdaPower pump_power;

        bool timeThreadFlag = false;
        Thread timeThread;
        //System.Windows.Forms.Timer timetick = new System.Windows.Forms.Timer();

        public int counter_Cycler = 0;
        public int counter_Cycler_limit = 50000;

		//210419 Add by KYJ 
        public string mesIP = "";
        public string mesPort = "";

        //200930 wjs add eup MES Port
        //public string mesPort_eup = "1111";

        public string program_type = "";
        public string keysight_PortName = "COM2";
        public string can_cycler1 = "PCAN_USB: 1 (51h)";


        public int INSULATION_RESISTANCE_VOLT_POS = 0;
        public int INSULATION_RESISTANCE_TIME_POS = 0;
        public int INSULATION_RESISTANCE_VOLT_NEG = 0;
        public int INSULATION_RESISTANCE_TIME_NEG = 0;

        public string PLC_RECV_ADDRESS = "";
        public string PLC_SEND_ADDRESS = "";
        public string PLC_BCRS_ADDRESS = "";
        public int PLC_LOGICAL_NUMBER = 0;

        //not used   
        public string power_PortName1 = "COM9";
        public string power_PortName2 = "COM8";
        public string nht_PortName = "COM1";
        public string can_bms = "PCAN_USB: 1 (51h)";
        public string dmmIP = "169.254.4.61";
        public string dmmPort = "5025";
        public string omega_PortName = "COM11";
        public string can_cycler2 = "PCAN_USB:FD 8 (58h)";
        public string can_bms2 = "PCAN_PCI: 2 (42h)";

        //190104 by grchoi
        string deviceStatus = "";
        string deviceRetryCount = "";
        public string lvRetryCount = "";
        //210112 wjs add (PJH)
        public bool isCMCRebootFail = false;


        BindingList<GraphItem> voltItemList = new BindingList<GraphItem>();
        BindingList<GraphItem> curItemList = new BindingList<GraphItem>();
        private DateTime start;
        private DateTime last;

        #endregion

        #region Registers
        public void Counter_Cycler()
        {
            string regSubkey = "Software\\EOL_Trigger";

            //Microsoft.Win32.Registry.CurrentUser.DeleteSubKey(regSubkey);   

            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
            //rk.SetValue("Access_DMM", "0");                        
            var regStr = rk.GetValue("COMMON_Counter_Cycler") as string;
            var regStr2 = rk.GetValue("COMMON_Counter_Cycler_Limit") as string;

            if (regStr == null)
            {
                rk.SetValue("COMMON_Counter_Cycler", "0");
                counter_Cycler = 0;
            }
            else
            {
                counter_Cycler = int.Parse(regStr);
            }

            if (regStr2 == null)
            {
                rk.SetValue("COMMON_Counter_Cycler_Limit", "50000");
                counter_Cycler_limit = 50000;
            }
            else
            {
                counter_Cycler_limit = int.Parse(regStr2);
            }

            LogState(LogType.Info, "Cycler Current Count :" + counter_Cycler.ToString());
        }

        /// <summary>
        /// 정상적으로 끝난 경우에만 카운터가 올라간다
        /// </summary>
        public void SetCounter_Cycler()
        {
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            rk.SetValue("COMMON_Counter_Cycler", counter_Cycler.ToString());
        }

        public void SetCounter_Cycle_Limit()
        {
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            rk.SetValue("COMMON_Counter_Cycler_Limit", counter_Cycler_limit.ToString());
        }


		/// <summary>
        ///모델별로 타입을 설정한다.
        ///기본값은 아래와 같으며, 없으면 만들고 있으면 사용하는데서
        ///레지스터로 바로 접근해서 가져올것이다
        ///0703 초기값 FF로 적용
        /// </summary>
        public void LoadCyclerBranch()
        {
            if (this.pro_Type == ProgramType.EOLInspector)
            {
                string regSubkey = "Software\\EOL_Trigger";
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
                //int cnt = 1;
                foreach (var md in modelDataList)
                {
                    if (rk.GetValue(position + "_" + md.ModelId + "_BRANCH") == null)
                    {
                        rk.SetValue(position + "_" + md.ModelId + "_BRANCH", "0FF");// "0A" + cnt.ToString());
                        //cnt++;
                    }
                }
            }
        }

        /// <summary>
        /// 200730 알람코드 로드 구문
        /// </summary>
        public void LoadCyclerAlarmCode()
        {
            if (this.pro_Type == ProgramType.EOLInspector)
            {
                string na = "__CYCLER_ALARM (28377 / 28335_DEF / 28335_1(MEB1) / 28335_234(MEB234)";
                string regSubkey = "Software\\EOL_Trigger";
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

                if (rk.GetValue(position + na) == null)
                {
                    //기본값은 28377
                    rk.SetValue(position + na, "28377");// "0A" + cnt.ToString());
                    dsp_Type = DSPType.DSP_28377;
                }
                else
                {
                    var data = rk.GetValue(position + na).ToString();

                    switch (data)
                    {
                        case "28377": dsp_Type = DSPType.DSP_28377; break;
                        case "28335_DEF": dsp_Type = DSPType.DSP_28335_def; break;
                        case "28335_1(MEB1)": dsp_Type = DSPType.DSP_28335_meb_1; break;
                        case "28335_234(MEB234)": dsp_Type = DSPType.DSP_28335_meb_234; break;
                        default: dsp_Type = DSPType.DSP_28377; break;
                    }
                }

                LogState(LogType.Info, "CyclerAlarmCode : " + dsp_Type.ToString());
            }
        }

        public void SetLocalData()
        {
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
            int cnt = 0;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_PROGRAM_TYPE(1:VOLT/2:HIPOT/3:EOL)", program_type == "" ? "3" : program_type); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_MES_IP", mesIP == "" ? "0.0.0.0" : mesIP); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_MES_PORT", mesPort == "" ? "5000" : mesPort); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_HIPOT_PORT", chroma_PortName == "" ? "COM2" : chroma_PortName); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_DMM_PORT", keysight_PortName == "" ? "COM3" : keysight_PortName); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_CMC_PORT", cmc_PortName == "" ? "COM8" : cmc_PortName); cnt++; //210419 Add by KYJ
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_CMC_RELAY_NUM", CMC_Relay == "" ? "IDO_4" : CMC_Relay); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_DAQ_PORT", daq_PortName == "" ? "COM5" : daq_PortName); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_CAN_CYCLER", can_cycler1 == "" ? "PCAN_USB: 1 (51h)" : can_cycler1); cnt++;

            rk.SetValue(position + "_" + cnt.ToString("D2") + "_PLC_RECV_ADDRESS", PLC_RECV_ADDRESS == "" ? "" : PLC_RECV_ADDRESS); cnt++;            
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_PLC_SEND_ADDRESS", PLC_SEND_ADDRESS== "" ? "" : PLC_SEND_ADDRESS); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_PLC_BCRS_ADDRESS", PLC_BCRS_ADDRESS == "" ? "" : PLC_BCRS_ADDRESS); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_PLC_LOGICAL_NUMBER", PLC_LOGICAL_NUMBER.ToString() == "" ? "0" : PLC_LOGICAL_NUMBER.ToString()); cnt++;

            ////200930 wjs add eup MES Port
            //rk.SetValue(position + "_" + cnt.ToString("D2") + "_MES_PORT_EUP", mesPort == null ? "5000" : mesPort); cnt++;

            //210315 wjs add FACELIFT COMPORT & RELAY NUMBER
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_CMC_PORT_FACELIFT", cmc_PortName_FL == null ? "COM9" : cmc_PortName_FL); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_CMC_RELAY_NUM_FACELIFT", CMC_Relay_FL == null ? "IDO_5" : CMC_Relay_FL); cnt++;

            rk.SetValue(position + "_" + cnt.ToString("D2") + "_CMC_PORT_PORSCHE", cmc_PortName_PORS == null ? "COM8" : cmc_PortName_PORS); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_CMC_RELAY_NUM_PORSCHE", CMC_Relay_PORS == null ? "IDO_4" : CMC_Relay_PORS); cnt++;

            //221101 wjs add mase m183
            //       FACE LIFT 모델과 동일하나 선언은 따로 추가한다.
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_CMC_PORT_MASERATI_M183", cmc_PortName_Mas_M183 == null ? "COM9" : cmc_PortName_Mas_M183); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_CMC_RELAY_NUM_MASERATI_M183", CMC_Relay_Mas_M183 == null ? "IDO_5" : CMC_Relay_Mas_M183); cnt++;

            //초기값 설정
            this.pro_Type = ProgramType.EOLInspector;
            mesIP = "0.0.0.0";
            mesPort = "5000";
            chroma_PortName = "COM2";
            keysight_PortName = "COM3";
            cmc_PortName = "COM3";
            CMC_Relay = "IDO_4";

            //210315 wjs add FACELIFT COMPORT & RELAY NUMBER
            cmc_PortName_FL = "COM4";
            CMC_Relay_FL = "IDO_5";

            daq_PortName = "COM5";
            can_cycler1 = "PCAN_USB: 1 (51h)";
            PLC_RECV_ADDRESS = "BA00";
            PLC_SEND_ADDRESS = "BA10";
            PLC_BCRS_ADDRESS = "WA0";
            PLC_LOGICAL_NUMBER = 0;

            //추가 레지스터가 필요하다면, 해당 부분에 추가
            //rk.SetValue("CYCLER_USING", "0");
            //rk.SetValue("CEI_PATH_OFFSET", "0.2");
        }
         
        public int GetLV_RETRY_COUNTER()
        {
            string regSubkey = "Software\\EOL_Trigger";
            int rtVal = 0;

            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            var regStr = rk.GetValue("LV_RETRY_COUNTER");
                
            if(regStr == null)
            {
                rk.SetValue("LV_RETRY_COUNTER", "0");
                rtVal = 0;
            }
            else
            {
                int.TryParse(regStr.ToString(),out rtVal);
            }

            return rtVal;
        }

        #region Cycler flags


        /// <summary>
        /// 사용중 : true
        /// 비사용중 : false
        /// </summary>
        /// <returns></returns>
        public bool GetCyclerFlag_OTHER()
        {
            string regSubkey = "Software\\EOL_Trigger";

            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            var tempPos = position == "#1" ? "#2" : "#1";//#1이면 #2 보게 한다.

            if (position != "#1" && position != "#2")
            {
                MessageBox.Show("Check position setting!! " + position);
                
                return false;
            }

            var regStr = rk.GetValue(string.Format("CYCLER_USING_{0}", tempPos)) as string;

            return regStr == "0" ? false : true;
        }

        /// <summary>
        /// 사용중 : true
        /// 비사용중 : false
        /// </summary>
        /// <returns></returns>
        public bool GetCyclerFlag_SELF()
        {
            string regSubkey = "Software\\EOL_Trigger";

            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            var regStr = rk.GetValue(string.Format("CYCLER_USING_{0}", position)) as string;

            return regStr == "0" ? false : true;
        }

        public void SetEnableCyclerFlag_SELF()
        {
            LogState(LogType.Info, string.Format("Enable [CYCLER_USING_{0}] Flag",position));
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
            rk.SetValue(string.Format("CYCLER_USING_{0}",position), "1"); 
        }

        public void SetDisableCyclerFlag_SELF()
        {
            LogState(LogType.Info, string.Format("Disable [CYCLER_USING_{0}] Flag", position));
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
            rk.SetValue(string.Format("CYCLER_USING_{0}", position), "0");
        }

        public void SetDisableCyclerFlag_OTHER()
        {
            var tempPos = position == "#1" ? "#2" : "#1";//#1이면 #2 보게 한다.

            LogState(LogType.Info, string.Format("Disable [CYCLER_USING_{0}] Flag", tempPos));
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
            rk.SetValue(string.Format("CYCLER_USING_{0}", tempPos), "0");
        }

        #endregion

        public void LoadLocalData()
        {
            try
            {
                string regSubkey = "Software\\EOL_Trigger";
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
                //this.Title = this.Title + " - " + MODEL_NAME + "," + position + "  (" + lastUpdated + ")";

                int cnt = 0;
                program_type = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_PROGRAM_TYPE(1:VOLT/2:HIPOT/3:EOL)") as string).ToString(); cnt++;
                switch (program_type)
                {
                    case "1": this.pro_Type = ProgramType.VoltageInspector; break;
                    case "2": this.pro_Type = ProgramType.HipotInspector; break;
                    case "3": this.pro_Type = ProgramType.EOLInspector; break;
                    case "4": this.pro_Type = ProgramType.Hipot_no_resin_Inspector; break;
                    default: this.pro_Type = ProgramType.EOLInspector; break;
                }
				
				//210419 Add by KYJ
                mesIP = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_MES_IP") as string).ToString(); cnt++;
                mesPort = int.Parse(rk.GetValue(position + "_" + cnt.ToString("D2") + "_MES_PORT") as string).ToString(); cnt++;
                chroma_PortName = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_HIPOT_PORT") as string).ToString(); cnt++;
                keysight_PortName = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_DMM_PORT") as string).ToString(); cnt++;
                cmc_PortName = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_CMC_PORT") as string).ToString(); cnt++;
                CMC_Relay = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_CMC_RELAY_NUM") as string).ToString(); cnt++;
                daq_PortName = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_DAQ_PORT") as string).ToString(); cnt++;
                can_cycler1 = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_CAN_CYCLER") as string).ToString(); cnt++;

                PLC_RECV_ADDRESS = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_PLC_RECV_ADDRESS") as string).ToString(); cnt++;
                PLC_SEND_ADDRESS = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_PLC_SEND_ADDRESS") as string).ToString(); cnt++;
                PLC_BCRS_ADDRESS = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_PLC_BCRS_ADDRESS") as string).ToString(); cnt++;
                PLC_LOGICAL_NUMBER = int.Parse(rk.GetValue(position + "_" + cnt.ToString("D2") + "_PLC_LOGICAL_NUMBER") as string); cnt++;

                //221117 MC 저항 nnkim
                McResistanceposition = position;

                //200930 wjs add eup MES Port
                //cnt++;
                //mesPort_eup = rk.GetValue(position + "_" + cnt.ToString("D2") + "_MES_PORT_EUP") as string;

                //210315 wjs add FACELIFT COMPORT & RELAY NUMBER
                //cnt++;
                cmc_PortName_FL = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_CMC_PORT_FACELIFT") as string).ToString(); cnt++;
                CMC_Relay_FL = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_CMC_RELAY_NUM_FACELIFT") as string).ToString(); cnt++;
                cmc_PortName_PORS = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_CMC_PORT_PORSCHE") as string).ToString(); cnt++;
                CMC_Relay_PORS = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_CMC_RELAY_NUM_PORSCHE") as string).ToString();

                //221101 wjs add mase m183
                cnt++;
                cmc_PortName_Mas_M183 = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_CMC_PORT_MASERATI_M183") as string).ToString(); cnt++;
                CMC_Relay_Mas_M183 = (rk.GetValue(position + "_" + cnt.ToString("D2") + "_CMC_RELAY_NUM_MASERATI_M183") as string).ToString();

                //추가 레지스터가 필요하다면, 해당 부분에 추가
                //ceiPathOffset = double.Parse(rk.GetValue("CEI_PATH_OFFSET").ToString());
            }
            catch (Exception ec)
            {
                SetLocalData();
            }
        }

        #endregion

        #region 판정
        /// <summary>
        /// 값을 비교하고 판정때리는 함수
        /// 값을 하나만 해서 동일판정일때는 min을 -1로 하고 설정값은 max에 둔다.
        /// 값이 하나인경우 1이 true, 0이 false로 친다
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        private bool JudgementTestItem(TestItem ti)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (isManual)
                {
                    resetBt.IsEnabled =
                    openButtonGrid.IsEnabled = true;
                    blinder3.Visibility = blinder2.Visibility = blinder.Visibility = System.Windows.Visibility.Hidden;
                }
            }));

            isProcessingFlag = false;
            double itemVal = 0;

            //200616 _HIPOT_EMO케이스 추가
            if (ti.Value_ != null && ti.Value_.ToString() == _HIPOT_EMO)
            {
                ti.Value_ = _HIPOT_EMO;
            }
            else
            {
                //아니라면 기존대로 EMG
                if (isStop)
                {
                    ti.Value_ = _EMG_STOPPED;
                }
            }

            //2. 검사항목 중 바코드 판정 시 All pass 적용
            if (ti.Name.Replace(" ", "") == _BARCODE)
            {

                if (this.deviceStatus == "")
                {

                    StringBuilder sb = new StringBuilder();
                    sb.Append("Test :");
                    sb.Append(ti.Name);
                    sb.Append(" End - PASS [Value:");
                    sb.Append(ti.Value_);
                    sb.Append("]");
                    ti.Result = "PASS";
                    this.LogState(LogType.Pass, sb.ToString());
                    
                    return true;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Test :");
                    sb.Append(ti.Name);
                    sb.Append(" End - NG [Value:");
                    sb.Append(ti.Value_);
                    sb.Append("]");
                    ti.Result = "NG";
                    this.LogState(LogType.NG, sb.ToString());
                    
                    if (isSkipNG_)
                    {
                        return true;
                    }

                    return false;
                }
            }

            var temp = ti.Value_;

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                    || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                    || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                    )
            {
                if (pro_Type == ProgramType.VoltageInspector || pro_Type == ProgramType.EOLInspector)
                {
                    if (ti.Name == "NormalSleep")
                    {
                        ti.Value_ = temp;
                    }
                }
            }

            //3.검사 판정 시 EMG인 경우 NG로 강제 적용(절연저항 측정 / DCIR 측정 등 인경우)
            if (ti.Value_ == null
                || ti.Value_.ToString() == "" // 200612 공백문자열 추가
                || ti.Value_.ToString() == _LV_IS_CONNECTED
                || ti.Value_.ToString() == _JIG_NOT_DEACTIVE
                || ti.Value_.ToString() == _JIG_NOT_ACTIVE
                || ti.Value_.ToString() == _CYCLER_SAFETY
                || ti.Value_.ToString() == _EMG_STOPPED
                || ti.Value_.ToString() == _DEVICE_NOT_READY
                || ti.Value_.ToString() == _VALUE_NOT_MATCHED
                || ti.Value_.ToString() == _TEMP_OVER
                || ti.Value_.ToString() == _NOT_POS_MSG
                || ti.Value_.ToString() == _NOT_NEG_MSG
                || ti.Value_.ToString() == _NOT_MODE_CHANGED
                || ti.Value_.ToString() == _CASE_113
                || ti.Value_.ToString() == _CASE_114
                || ti.Value_.ToString() == _CASE_115
                || ti.Value_.ToString() == _CASE_112
                || ti.Value_.ToString() == _CASE_33
                || ti.Value_.ToString() == _CASE_34
                || ti.Value_.ToString() == _CASE_35
                || ti.Value_.ToString() == _CASE_36
                || ti.Value_.ToString() == _CASE_37
                || ti.Value_.ToString() == _CASE_38
                || ti.Value_.ToString() == _CASE_39
                || ti.Value_.ToString() == _CASE_43
                || ti.Value_.ToString() == _CASE_49
                || ti.Value_.ToString() == _CASE_50
                || ti.Value_.ToString() == _CASE_52
                || ti.Value_.ToString() == _CASE_54
                || ti.Value_.ToString() == _CASE_55
                || ti.Value_.ToString() == _CASE_120_IR
                || ti.Value_.ToString() == _CASE_121_IR
                || ti.Value_.ToString() == _CASE_120_DC
                || ti.Value_.ToString() == _CASE_121_DC
                //|| ti.Value_.ToString() == _JIG_DOWN_TIMEOUT
                //|| ti.Value_.ToString() == _JIG_DOWN_BIT_OFF_TIMEOUT
                || ti.Value_.ToString() == _SENSING_BOARD_CHECK
                || ti.Value_.ToString() == _VOLTAGE_PIN_SENSING_FAIL
                || ti.Value_.ToString() == _DAQ_VOLT_SENSING_FAIL//9. 검사기 공통) 구간별 NG 코드 점검 필요 추가된부분
                || ti.Value_.ToString() == _CHAN_HI_FAIL//200609 내전압 값 ""일때 Pass 이슈로 인한 추가
                || ti.Value_.ToString() == _CHAN_LO_FAIL
                //|| ti.Value_.ToString() == _HIPOT_OVERFLOW//200616 overflow 추가
                || ti.Value_.ToString() == _HIPOT_EMO//200616 _HIPOT_EMO 추가
                || ti.Value_.ToString() == _COMM_FAIL
                || ti.Value_.ToString() == _CHECK_TERMINAL
                || ti.Value_.ToString() == _MES_CELL_DATA_NOT_RECIVED)
            {
                if(ti.Value_ != null && ti.Value_.ToString() == "")
                {
                    ti.Value_ = _DATA_BLANK;
                }

                if (ti.Value_ == null)
                    ti.Value_ = "NULL";

                ti.Result = "NG";
                StringBuilder sb = new StringBuilder();
                sb.Append("Test :");
                sb.Append(ti.Name);
                sb.Append(" End - NG [Min:");
                sb.Append(ti.Min.ToString());
                sb.Append("][Value:");
                sb.Append(ti.Value_);
                sb.Append("][Max:");
                sb.Append(ti.Max.ToString());
                sb.Append("]");
                this.LogState(LogType.NG, sb.ToString());
                
                    
                if (isSkipNG_)
                {
                    return true;
                }

                if (DIAG_MODE && ti.selfDiagDirectory != "" && !isStop)
                {
                    return ShowSelfDiagWindow(ti);
                }
                else
                {
                    return false;
                }
            }



            if (double.TryParse(ti.Value_.ToString(), out itemVal))
            {
                //itemVal = 9.954367;
                #region 자릿수를 만들어 주는 부분 추가
                string digit = "F" + ti.DigitLength;
                #endregion
                itemVal = double.Parse(itemVal.ToString(digit));
                if (ti.Name.Contains("1SLASH3SLASH5") || ti.Name.Contains("2SLASH4SLASH6") || ti.Name.Contains("GPIO") || ti.Name.Contains("BalanceSwitch")) { }
                else
                {
                    ti.Value_ = itemVal.ToString(digit);
                }

            }

            var d_max = 0.0;
            var d_min = 0.0;
            var rst1 = double.TryParse(ti.Max.ToString(), out d_max);
            var rst2 = double.TryParse(ti.Min.ToString(), out d_min);

            if (!rst1 && !rst2)
            {
                if (ti.Max.ToString() == ti.Value_.ToString() && ti.Min.ToString() == ti.Value_.ToString())
                {
                    ti.Result = "PASS";
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Test :");
                    sb.Append(ti.Name);
                    sb.Append(" End - Pass [Min:");
                    sb.Append(ti.Min.ToString());
                    sb.Append("][Value:");
                    sb.Append(ti.Value_);
                    sb.Append("][Max:");
                    sb.Append(ti.Max.ToString());
                    sb.Append("]");
                    this.LogState(LogType.Pass, sb.ToString());
                                        
                    return true;
                }
                else
                {
                    ti.Result = "NG";
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Test :");
                    sb.Append(ti.Name);
                    sb.Append(" End - NG [Min:");
                    sb.Append(ti.Min.ToString());
                    sb.Append("][Value:");
                    sb.Append(ti.Value_);
                    sb.Append("][Max:");
                    sb.Append(ti.Max.ToString());
                    sb.Append("]");
                    this.LogState(LogType.NG, sb.ToString());
                    

                    if (isSkipNG_)
                    {
                        return true;
                    }

                    if (DIAG_MODE && ti.selfDiagDirectory != "" && !isStop)
                    {
                        return ShowSelfDiagWindow(ti);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if (d_max >= itemVal && d_min <= itemVal)
            {
                //자릿수 만들어주는 부분

                var itemArr = ti.Value_.ToString().Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (itemArr.Length > 1)
                {
                    while (itemArr[1].Length < int.Parse(ti.DigitLength))
                    {
                        var str = ti.Value_.ToString() + "0";
                        ti.Value_ = str;
                        itemArr = ti.Value_.ToString().Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }

                ti.Result = "PASS";
                StringBuilder sb = new StringBuilder();
                sb.Append("Test :");
                sb.Append(ti.Name);
                sb.Append(" End - Pass [Min:");
                sb.Append(ti.Min.ToString());
                sb.Append("][Value:");
                sb.Append(ti.Value_);
                sb.Append("][Max:");
                sb.Append(ti.Max.ToString());
                sb.Append("]");
                this.LogState(LogType.Pass, sb.ToString());
                                    
                return true;
            }
            else
            {
                //자릿수 만들어주는 부분

                var itemArr = ti.Value_.ToString().Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (itemArr.Length > 1)
                {
                    while (itemArr[1].Length < int.Parse(ti.DigitLength))
                    {
                        var str = ti.Value_.ToString() + "0";
                        ti.Value_ = str;
                        itemArr = ti.Value_.ToString().Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }

                ti.Result = "NG";
                StringBuilder sb = new StringBuilder();
                sb.Append("Test :");
                sb.Append(ti.Name);
                sb.Append(" End - NG [Min:");
                sb.Append(ti.Min.ToString());
                sb.Append("][Value:");
                sb.Append(ti.Value_);
                sb.Append("][Max:");
                sb.Append(ti.Max.ToString());
                sb.Append("]");
                this.LogState(LogType.NG, sb.ToString());
                                    
                if (isSkipNG_)
                {
                    return true;
                }

                if (DIAG_MODE && ti.selfDiagDirectory != "" && !isStop)
                {
                    return ShowSelfDiagWindow(ti);
                }
                else
                {
                    return false;
                }
            }
        }

        bool ShowSelfDiagWindow(TestItem ti)
        {
            bool rtv = false;
            Dispatcher.Invoke(new Action(() =>
                {
                    SelfDiagnosisWindow sdw = new SelfDiagnosisWindow(this, ti);
                    sdw.ShowDialog();
                    rtv = sdw.isContinue;
                    sdw.Close();
                }));

            if (rtv)
            {
                return true;
            }
            else
            { 
                return false;
            }
        }

        #endregion

        #region Multi Models

        /// <summary>
        /// 1:AUDI_NOR
        /// 2:AUDI_MIR
        /// 3:PORSCHE_NOR
        /// 4:PORSCHE_MIR
        /// 5:MASERATI_NOR
        /// 6:MASERATI_MIR
        /// 7:E_UP
        /// </summary>
        public ModelType localTypes;

        private void sideChangedUp(object sender, MouseButtonEventArgs e)
        {
            var lb = sender as RadioButton;

            SetViewModel_To_Event(lb.Content.ToString());

            this.LoadList();

            //190124 ht MES 스킵되있을때는 스펙받아오지 아니한다
            if (!isMESskip)
            {
                GetControlItemFromMES();
                GetCollectItemFromMES();
            }
            else
            {
                GetControlItemFromCSV();
                GetCollectItemFromCSV();
            }
            SetFieldsToMESData();
            SetCyclerFieldsToMESData();

            this.testItemListDg.Items.Refresh();
        }

        public void AutoMode_change(string mode_name)
        {
            //181217 begin인 경우는 MES 스펙 받아올때랑 꼬이는 경우가 있다!!
            this.Dispatcher.Invoke(new Action(() =>
            {
                SetViewModel_To_Event(mode_name);

                this.LoadList();

                this.testItemListDg.Items.Refresh();
            }));
        }


        public void SetViewModel_To_Event(string name)
        {
            for (int i = 0; i < modelDataList.Count; i++)
            {
                if (modelDataList[i].ModelId == name)
                {
                    this.prodTb.Text = viewModel.ProdId = modelDataList[i].ProdId;
                    this.procTb.Text = viewModel.ProcId = modelDataList[i].ProcId;
                    this.equipTb.Text = viewModel.EquipId = modelDataList[i].EquipId; 
                    viewModel.PLCBit_A = modelDataList[i].PLCBit_A;
                    viewModel.PLCBit_B = modelDataList[i].PLCBit_B;
                    viewModel.PLCBit_C = modelDataList[i].PLCBit_C;
                    viewModel.PLCBit_D = modelDataList[i].PLCBit_D;
                    viewModel.PLCBit_E = modelDataList[i].PLCBit_E;
                    viewModel.PLCBit_F = modelDataList[i].PLCBit_F;

                    viewModel.ModelId = modelDataList[i].ModelId;
                    localTypes = modelDataList[i].ModelType;

                    LogState(LogType.Info, "Selected " + viewModel.ModelId + " Model");
                    break;
                }
            }
        }

        #endregion


        /// <summary>
        /// 모델의 상위 저장 폴더 이름
        /// </summary>
        string MODEL_NAME = "AUDI_PORSCHE_MASERATI_E-UP_PORSCHE_J1PA";//210323 wjs change name    before "AUDI_PORSCHE_MASERATI_E-UP_";

        /// <summary>
        /// Cycler DSP Types
        /// </summary>
        public DSPType dsp_Type = DSPType.DSP_28377;

        /// <summary>
        /// Program Types
        /// </summary>
        public ProgramType pro_Type = ProgramType.EOLInspector;

        /// <summary>
        /// last updated date (Recommanded)
        /// </summary>
        public string lastUpdated = "E_up/Audi_DCIR_MP_23.05.04.V01_Last"; //221102 wjs change date before 220822

        /// <summary>
        /// 두개이상 프로그램일 때 쓰는 윈도우 위치
        /// </summary>
        public string position = "#1";

        public string All_of_All_Header = "$1$";

        /// <summary>
        /// 각종 윈도우 및 모드 변경시 필요 비번
        /// </summary>
        public string passWord = "1212";

        /// <summary>
        /// EOL 스펙
        /// 포지션과 같이 써야함
        /// </summary>
        static string staticPath_EOL_SPEC = @"C:\EOL_SPEC";

        /// <summary>
        /// EOL 모델리스트
        /// 포지션과 같이 써야함
        /// </summary>
        static string staticPath_EOL_MOD_LIST = @"C:\EOL_MOD_LIST";

        /// <summary>
        /// 예측용 PREDICT 저장 폴더
        /// </summary>
        static string staticPath_PREDICT = @"C:\EOL_PREDICT";

        /// <summary>
        /// diag 전용 폴더
        /// </summary>
        static string staticPath_SELF_DIAG = @"C:\EOL_SELF_DIAG";

        /// <summary>
        /// 병렬 운전 모드
        /// </summary>
        public BranchMode local_Branchmode = BranchMode.NO_BRANCH;

        #region Graph Field

        /// <summary>
        /// 전압 그래프 Y축 최대값
        /// </summary>
        public int graph_Volt_Max = 13;

        /// <summary>
        /// 전압 그래프 Y축 최소값
        /// </summary>
        public int graph_Volt_Min = 9;

        /// <summary>
        /// 전류 그래프 Y축 최대값
        /// </summary>
        public int graph_Cur_Max = 132;

        /// <summary>
        /// 전류 그래프 Y축 최소값
        /// </summary>
        public int graph_Cur_Min = -132;

        #endregion

        #region Using Field
        public List<double> CMClist = new List<double>();
        public List<double> CMClist_Cell = new List<double>();
        public string CMC_Relay = "IDO_4";
        public string cmc_PortName = "COM8";

        //210419 Add by KYJ
        public string CMC_Relay_FL = "IDO_5"; //현지에서 확인 및 적용
        public string cmc_PortName_FL = "COM9"; //현지에서 확인 및 적용
        public string CMC_Relay_PORS = "IDO_4"; //현지에서 확인 및 적용
        public string cmc_PortName_PORS = "COM8"; //현지에서 확인 및 적용
        //221101 wjs add mase m183
        public string CMC_Relay_Mas_M183 = "IDO_5"; //현지에서 확인 및 적용
        public string cmc_PortName_Mas_M183 = "COM9"; //현지에서 확인 및 적용

        public string daq_PortName = "COM5";
        public double cmc1mv, cmc2mv, cmc1cv1, cmc2cv1;
        public double cbshort1Volt, cbshort2Volt, cbshort3Volt, cbshort4Volt, cbshort5Volt, cbshort6Volt;
        public double ow1Volt, ow2Volt, ow3Volt, ow4Volt, ow5Volt, ow6Volt;
        public string cmcGuid = "";
        public System.Diagnostics.Stopwatch publicWatch = new System.Diagnostics.Stopwatch();
        public CCMC cmc;
        public CChroma chroma;
        bool isDoSleep = false;

        //public int plusirlevelTb = 0;
        //public int plusirtimeTb = 0;
        //public int minusirlevelTb = 0;
        //public int minusirtimeTb = 0;

        //public int pluswithvoltageTb = 0;
        //public int pluswithtimeTb = 0;
        //public int pluswithramptimeTb = 0;
        //public int minuswithvoltageTb = 0;
        //public int minuswithtimeTb = 0;
        //public int minuswithramptimeTb = 0;
        //public int arconTb = 0;
        //public int arclimitTb = 0;


        public int withstandRetry = 0;
        public string chroma_PortName = "COM3";

        #endregion

        public List<TestModel> modelDataList = new List<TestModel>();
        public TestModel viewModel = new TestModel();

        bool isNeedOCVFlag = false;
        bool isCyclerAlarm = false;

        public MainWindow()
        {
            InitializeComponent();

            //언어 인터락 nnkim
            WindowLanguageFormatCheck();

            LoadINI(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            //staticPath += "\\" + position;
            BeforeFormLoading();

            //SetLocalData(); //초기 1회는 셋을 해줘야 함
            LoadLocalData();
            LoadModelList();
            LoadPredictFileList();
            LoadCyclerBranch();
            LoadCyclerAlarmCode();

            InitializeCommonMethods();
            InitializeSaveFileAddr();
            InitializePermissionList();
            Counter_Cycler();

            AutoLogoutFunc();

            //221019 UI Check 하이팟 DCIR 체크
            ShowInspectionUIChange((int)this.pro_Type);
            //20221017 소모품 카운트 체크
            SetPartsCountData();
        }

        #region 200213 NG_MASTER_BCR

        bool MASTER_MODE = false;
        bool DIAG_MODE = false;

        //bool isMASTER_MODE_MES_SKIP = false;

        public bool CheckBCRforMaster_Diag(string bcr)
        {
            MASTER_MODE = DIAG_MODE = false;
            isMESskip = false; // 0804 ks yoo //221107 pjh 주석했다가 풀음
            //isMASTER_MODE_MES_SKIP

            //리스트에 바코드가 있으면,
            if (mBCRs.Exists(x => x.BCR == bcr))
            {
                LogState(LogType.Info, string.Format("[CheckBCRforMaster_Diag] BCR is exist in List:{0}", bcr));

                var singleBcr = mBCRs.First(x => x.BCR == bcr) as CustomBCR;
                if (this.localTypes.GetHashCode() == singleBcr.ModelType)
                {
                    LogState(LogType.Info, string.Format("[CheckBCRforMaster_Diag] BCR is matched in this Type:{0} ", this.localTypes.ToString()));

                    switch (singleBcr.Mode)
                    {
                        case 1:
                            {
                                DIAG_MODE = true;

                                if (singleBcr.ProdID != "")
                                {
                                    this.Dispatcher.Invoke(new Action(() =>
                                    {
                                        //제품 ID
                                        this.prodTb.Text = this.viewModel.ProdId = singleBcr.ProdID;
                                        //selfDiagBlinder.Visibility = System.Windows.Visibility.Visible;
                                    }));
                                }

                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    //0609 product id가 없더라도 블라인더는 켜지도록 변경
                                    selfDiagBlinder_diag.Visibility = System.Windows.Visibility.Visible;
                                }));

                                LogState(LogType.Info, string.Format("[CheckBCRforMaster_Diag] Enabled DIAG_MODE:{0} / {1}", singleBcr.Mode.ToString(), singleBcr.ProdID));
                            }; break;
                        case 2:
                            {
                                MASTER_MODE = true;

                                if (singleBcr.ProdID != "")
                                {
                                    this.Dispatcher.Invoke(new Action(() =>
                                    {
                                        //제품 ID
                                        this.prodTb.Text = this.viewModel.ProdId = singleBcr.ProdID;
                                        //selfDiagBlinder.Visibility = System.Windows.Visibility.Visible;
                                    }));
                                }

                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    //0609 product id가 없더라도 블라인더는 켜지도록 변경
                                    selfDiagBlinder_master.Visibility = System.Windows.Visibility.Visible;
                                }));

                                LogState(LogType.Info, string.Format("[CheckBCRforMaster_Diag] Enabled MASTER_MODE:{0}", singleBcr.Mode.ToString()));
                            }; break;
                        default:
                            {
                                LogState(LogType.Info, string.Format("[CheckBCRforMaster_Diag] Failed to enter mode:{0}", singleBcr.Mode.ToString()));
                            }; break;
                    }
                }
                else
                {
                    LogState(LogType.Info, string.Format("[CheckBCRforMaster_Diag] BCR is mismatched in this Type:{0} / {1}", this.localTypes.GetHashCode().ToString(), singleBcr.ModelType.ToString()));
                }
                 

                //MES 스킵이 비활성화 상태라면(보고하도록 되어있다면)
                //no mes touch - 200223 jht 2jh
                //if(!isMESskip)
                //{
                //    isMESskip = isMASTER_MODE_MES_SKIP = true;
                //    this.MES_UI_ENABLE(false);
                //    LogState(LogType.Info, "NG MASTER MODE - MES Skip Enabled");
                //}                
            }
            else
            {
                if (bcr.Contains("kyoungsuk") == true)  //220610 we don't use master mode - ks yoo
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        DeviceCheckWindow rt = new DeviceCheckWindow(this);
                        rt.Height += 100;
                        rt.Width += 200;
                        rt.maintitle.FontSize += 20;
                        rt.maintitle.Content = "NOT RESISTERD MASTER BARCODE!!";
                        rt.reason.FontSize += 13;
                        rt.reason.Content = "Please Resister Master Barcode In Barcode List";
                        rt.okbt.FontSize += 13;
                        rt.Show();
                    }));
                    tlamp.SetTLampNG(true);

                    return false;
                }
               
                LogState(LogType.Info, string.Format("[CheckBCRforMaster_Diag] BCR is NOT exist in List:{0}", bcr));
            }

            return true;
        }

        /// <summary>
        /// 진입시 true 리턴
        /// </summary>
        /// <returns></returns>
        public bool CheckBCRforMaster_Diag_END()
        {
            if (DIAG_MODE || MASTER_MODE)
            {
                //if(isMASTER_MODE_MES_SKIP)
                //{
                //    isMESskip = false;
                //    this.MES_UI_ENABLE(true);
                //    LogState(LogType.Info, "NG MASTER MODE - MES Skip Disabled");
                //}

                if (DIAG_MODE)
                {
                    LogState(LogType.Info, string.Format("[CheckBCRforMaster_Diag_END] Disabled DIAG_MODE"));

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        selfDiagBlinder_diag.Visibility = System.Windows.Visibility.Collapsed;
                    }));
                }
                else if (MASTER_MODE)
                {
                    LogState(LogType.Info, string.Format("[CheckBCRforMaster_Diag_END] Disabled MASTER_MODE"));

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        selfDiagBlinder_master.Visibility = System.Windows.Visibility.Collapsed;
                    }));
                }
                DIAG_MODE = MASTER_MODE = false;


                return true;
            }
            return false;
        }

        public List<CustomBCR> mBCRs;

        private void LoadMasterBcr()
        {
            try
            {
                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");

                FileStream readData = new FileStream(staticPath_EOL_SPEC + "\\" + position + "\\Master_BCR.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                StreamReader streamReader = new StreamReader(readData, encode);
                mBCRs = new List<CustomBCR>();

                int cnt = 0;
                while (streamReader.Peek() > -1)
                {
                    var content = streamReader.ReadLine();

                    var ar = content.Split(',');
                    if (ar.Length < 4)
                        continue;

                    if (ar[3] == "" || ar[0].Contains("Model Type")) 
                        continue;

                    var singleBCR = new CustomBCR();

                    singleBCR.ModelType = int.Parse(ar[0]);
                    singleBCR.Mode = int.Parse(ar[1]);
                    singleBCR.ProdID = ar[2];
                    singleBCR.BCR = ar[3];

                    if (mBCRs.Exists(x => x.BCR == singleBCR.BCR))
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            DeviceCheckWindow rt = new DeviceCheckWindow(this);
                            rt.Height += 100;
                            rt.Width += 550;
                            rt.maintitle.FontSize += 20;
                            rt.maintitle.Content = "DUPLICATE BARCODE IS RECOGNIZED!!";
                            rt.reason.FontSize += 13;
                            rt.reason.Content = "Please check duplicated barcodes.\nDuplicate barcodes are not allowed in the program.";
                            rt.okbt.FontSize += 13;
                            rt.ShowDialog();


                            var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\Master_BCR.csv";
                            System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                            //200612 종료시퀀스로 확정 pjh / ht
                            System.Environment.Exit(0);
                        }));

                    }
                    else
                    {
                        mBCRs.Add(singleBCR);
                    }

                }

                LogState(LogType.Success, "Load Model List");
            }
            catch (Exception ec)
            {
                LogState(LogType.Fail, "Load Model List", ec);
            }
        }
        #endregion

        private void BeforeFormLoading()
        {
            //staticPath += "\\" + position;

            string path = "";

            #region EOL_SPEC 생성

            path = staticPath_EOL_SPEC + "\\" + position;

            if (!Directory.Exists(path.ToString()))
            {
                if (MessageBox.Show(path + " 폴더가 존재하지 않습니다. \n해당 경로에 동작에 필요한 기본 파일 및 폴더를 복사하겠습니다. \n계속 진행하겠습니까?", "경고", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    CopyFolder(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\EOL_DEF_DATA\\EOL_SPEC", path);
                }
            }
            #endregion

            #region EOL_MOD_LIST 생성

            path = staticPath_EOL_MOD_LIST + "\\" + position;

            if (!Directory.Exists(path.ToString()))
            {
                if (MessageBox.Show(path + " 폴더가 존재하지 않습니다. \n해당 경로에 동작에 필요한 기본 파일 및 폴더를 복사하겠습니다. \n계속 진행하겠습니까?", "경고", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    CopyFolder(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\EOL_DEF_DATA\\EOL_MOD_LIST", path);
                }
            }
            #endregion

            #region EOL_SELF_DIAG 생성 

            path = staticPath_SELF_DIAG;

            if (!Directory.Exists(path.ToString()))
            {
                if (MessageBox.Show(path + " 폴더가 존재하지 않습니다. \n해당 경로에 동작에 필요한 기본 파일 및 폴더를 복사하겠습니다. \n계속 진행하겠습니까?", "경고", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    CopyFolder(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\EOL_DEF_DATA\\EOL_SELF_DIAG", path);
                } 
            }
            #endregion

            #region EOL_PREDICT 생성 

            path = staticPath_PREDICT;

            if (!Directory.Exists(path.ToString()))
            {
                if (MessageBox.Show(path + " 폴더가 존재하지 않습니다. \n해당 경로에 동작에 필요한 기본 파일 및 폴더를 복사하겠습니다. \n계속 진행하겠습니까?", "경고", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    CopyFolder(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\EOL_DEF_DATA\\EOL_PREDICT", path);
                } 
            }
            #endregion

            LoadMasterBcr();

            CheckRelayBackground(); // 200102 KSM

            //200205 HT 문과장님코드 
            SetTextOnForm();

            CheckProgramRun_Overrided();
        }

        /// <summary>
        /// moons 빌드날짜가 타이틀에 표기 
        /// </summary>
        public void SetTextOnForm()
        {
            try
            {
                // 최종 수정 날짜가 컴파일 시 자동으로 업데이트 됨.
                //Version assVer = Assembly.GetExecutingAssembly().GetName().Version;
                //DateTime buildDate = new DateTime(2000, 1, 1).AddDays(assVer.Build).AddSeconds(assVer.Revision * 2);

                //210914 수동빌드 체제로 다시 전환됨..? 
                //211012 현장 빌드시 업데이트 날짜 / 빌드 날짜 차이로 인한 작업실수 방지 
                //211013 코드 수정시 lastUpdated 필드 수정 필수
                this.Title = string.Format("Phoenixon Controls {0} - {1}" + " [{2}]",
                                                            this.pro_Type.ToString(),
                                                            position,
                                                            lastUpdated);                
            }
            catch
            {

            }
        }

        public void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }

            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        } 

        private void CheckRelayBackground() // 200102 KSM
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcessesByName("RelayController_Background");

            if (processList.Length < 1)
            {
                //System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\RelayController_Background\\RelayController_Background\\bin\\Debug\\RelayController_Background.exe");
                System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "\\RelayController_Background.exe");

                LogState(LogType.Info, "RelayBackground Not Ready, Start RelayBackground...");
                Thread.Sleep(500);
            }
            else
            {
                LogState(LogType.Info, "RelayBackground Already Ready");
            }
        }

        private void CheckProgramRun_Overrided()  
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcessesByName("EOL_BASE");

            if (processList.Length > 0)
            {
                foreach (var proc in processList)
                {
                    if (this.Title == proc.MainWindowTitle)
                    {
                        LogState(LogType.Info, "EOL_BASE Already Started");
                        System.Environment.Exit(0);
                    }
                }
            }
            else
            {
            }
        }

        private int FPS = 0;
        private double LastFrame;
        // 191121 HJS - FrameRate 값 취득 관련 Event Method
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            FPS++;
            if ((DateTime.Now.TimeOfDay.TotalMilliseconds - LastFrame) > 1000)
            {
                lbFrameRate.Content = FPS;
                LastFrame = DateTime.Now.TimeOfDay.TotalMilliseconds;
                FPS = 0;
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetUI();

            SetViewModel_To_Event(this.viewModel.ModelId);

            SetModules();

            SetWindow();

            LoadList();

            //210419 Add by KYJ
            //relays.Reset();
            relays.RelayOff(CMC_Relay_FL);
            relays.RelayOff(CMC_Relay_PORS);
            //221101 wjs add mase m183
            relays.RelayOff(CMC_Relay_Mas_M183);

            mmw = new MiniScheduler.MainWindow(this);
            clsw = new CyclerLimitSettingWindow(this);
            this.totalProcessList = mmw.totalProcessList;

            GetControlItemFromCSV();
            GetCollectItemFromCSV();

            if (MES.isMESConnected)
            {
                GetControlItemFromMES();
                GetCollectItemFromMES();
            }
            else
            {
                this.monoTb.Background = this.lotTb.Background = this.prodTb.Background
                    = this.equipTb.Background
                    = this.procTb.Background = Brushes.Red;
            }

            SetFieldsToMESData();
            SetCyclerFieldsToMESData();

            emo_tmr = new System.Timers.Timer();
            emo_tmr.Interval = 500;
            emo_tmr.Elapsed += new System.Timers.ElapsedEventHandler(EmoTimer_Tick);
            emo_tmr.Start();

            graph_tmr = new System.Timers.Timer();
            graph_tmr.Interval = 100;
            graph_tmr.Elapsed += new System.Timers.ElapsedEventHandler(graph_tmr_Tick);
            graph_tmr.Start();

            loggerThread = new Thread(new ThreadStart(DequeueLogger));

            loggerThread.Start();

            tlamp.SetTLampStandBy(true);

            cellDetailList.Clear();
            cellDetailList.Add(new CellDetail() { TestName = "Init Volt" });
            cellDetailList.Add(new CellDetail() { TestName = "Discharge" });
            cellDetailList.Add(new CellDetail() { TestName = "Rest" });
            cellDetailList.Add(new CellDetail() { TestName = "Charge" });
            cellDetailList.Add(new CellDetail() { TestName = "Finish Volt" });
            ClearCellDetailList();

            CellDetailDg.ItemsSource = cellDetailList;

            // 191121 HJS - FrameRate 값 취득 관련 Event 등록
            // 200205 HT 위치 변경
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

            var th = new Thread(() =>
            {
                while (true)
                {
                    if (isCyclerAlarm == true)
                    {
                        plc.CyclerAlarm(true);
                        Thread.Sleep(1000);
                        plc.CyclerAlarm(false);

                        isCyclerAlarm = false;
                    }

                }
            });
            th.Start();
        }

        private double GetWindowsVersion()
        {
            try
            {
                string subKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion";
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey skey = key.OpenSubKey(subKey);

                string pname = skey.GetValue("ProductName").ToString();
                string cver = skey.GetValue("CurrentVersion").ToString();
                //string rid = skey.GetValue("ReleaseId").ToString();

                //windows 7 : Windows 7 Ultimate 
                //windows 10 : Windows 10 Home
                LogState(LogType.Info, "[WINDOWS] Product Name : " + pname);
                //windows 7 : 6.1
                //windows 10 : 6.3
                LogState(LogType.Info, "[WINDOWS] Current Version : " + cver);

                //windows 7 : X
                //windows 10 : 2004,2009
                //Console.WriteLine("Release Id : " + rid);

                return double.Parse(cver);
            }
            catch (Exception ec)
            {
                LogState(LogType.Info, "[WINDOWS] GetWindowsVersion Fail!!", ec);
                return 6.1;
            }
        }

        private void SetWindow()
        {
            var version = GetWindowsVersion();

            int posOffset = version > 6.1 ? 7 : 0;

            int widthOffset = version > 6.1 ? 14 : 0;

            int heightOffset = version > 6.1 ? 7 : 0;

            if (this.pro_Type == ProgramType.HipotInspector)
            {
                this.monoTb.FontSize -= 1;
                this.lotTb.FontSize -= 1;
                switch (position)
                {
                    case "#1":
                        {
                            this.Width = (SystemParameters.WorkArea.Width / 2) + widthOffset;
                            this.Height = (SystemParameters.WorkArea.Height / 2) + heightOffset;
                            this.Top = 0;
                            this.Left = 0 - posOffset;
                        }; break;
                    case "#2":
                        {
                            this.Width = (SystemParameters.WorkArea.Width / 2) + widthOffset;
                            this.Height = SystemParameters.WorkArea.Height / 2 + heightOffset;
                            this.Top = 0;
                            this.Left = (SystemParameters.WorkArea.Width / 2) - posOffset;
                        }; break;
                    case "#3":
                        {
                            this.Width = (SystemParameters.WorkArea.Width / 2) + widthOffset;
                            this.Height = (SystemParameters.WorkArea.Height / 2) + heightOffset;
                            this.Top = SystemParameters.WorkArea.Height / 2;
                            this.Left = 0 - posOffset;
                        }; break;
                    case "#4":
                        {
                            this.Width = (SystemParameters.WorkArea.Width / 2) + widthOffset;
                            this.Height = (SystemParameters.WorkArea.Height / 2) + heightOffset;
                            this.Top = SystemParameters.WorkArea.Height / 2;
                            this.Left = (SystemParameters.WorkArea.Width / 2) - posOffset;
                        }; break;
                }

            }
            else if (this.pro_Type == ProgramType.Hipot_no_resin_Inspector)
            {
                this.Width = SystemParameters.WorkArea.Width / 2;
                this.Height = SystemParameters.WorkArea.Height;
                if (position == "#2")
                {
                    this.Left = SystemParameters.WorkArea.Width / 2;
                }
                else
                {
                    this.Left = 0;
                }
                this.Top = 0;
            }
            else if(this.pro_Type == ProgramType.VoltageInspector)
            {
                this.monoTb.FontSize -= 1;
                this.lotTb.FontSize -= 1;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        double viewVoltage, viewCurrent = 0.0;

        private void graph_tmr_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (cycler != null && !cycler.isCyclerStop)
            {
                TimeSpan span = DateTime.Now - last;

                int previousTime = voltItemList.Count > 0 ? voltItemList[voltItemList.Count - 1].Time : 0;
                GraphItem newItem = new GraphItem
                {
                    Time = (int)(previousTime + span.TotalMilliseconds),
                    Value = viewVoltage
                };

                GraphItem newItem1 = new GraphItem
                {
                    Time = (int)(previousTime + span.TotalMilliseconds),
                    Value = viewCurrent

                };

                last = DateTime.Now;

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    voltItemList.Add(newItem);
                    curItemList.Add(newItem1);
                }));
            }
            else
            {
                last = start = DateTime.Now;
            }
        }

        private void EmoTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            // DI 채널 확인
            // Relay Background에서 처리된 것이 넘어와야 함.
            // EMO SW가 B접이라 평소에 Close 상태임. 그래서 스위치가 눌렸을 때 0이 되는게 맞음.
            string value = rkey.GetValue("EMO_SENSE").ToString();
            if (value == "0")
            {
                pushedEmo = true;
                StopAuto(); // EMO가 눌리면 프로그램에서 Emergency Stop이 눌렸을 때처럼 처리한다.
            }
            else
            {
                pushedEmo = false;
            }

            //LogState(LogType.Info, string.Format("EMO SW Status: {0}, {1}", pushedEmo.ToString(), value));
        }

        private void SetModules()
        {
            //181217 mes가 연결되있을 때 릴레이가 초기화가 안되어있으면 터지는거 때문에 수정
            relays = new CRelay_Receiver(this);
            // towerlamp 객체 생성
            tlamp = new TowerLamp(this);

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                MESConnect();
            }), null);

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                temps = new CNhtRS232_Receiver(this);
            }), null);


            if (this.pro_Type == ProgramType.HipotInspector || this.pro_Type == ProgramType.Hipot_no_resin_Inspector)
            {
                isRscMonitor.Height = isNeedDCIRPanel.Width = isNeedOCV.Height = new GridLength(0);
                topgridHeight.Height = new GridLength(200);
                btgridHeight.Height = new GridLength(70);
                testItemListDg.FontSize = 13;

                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                {
                    chroma = new CChroma(this);
                }), null);

                this.hipotStatus.Visibility = Visibility.Visible;
                cyclerSettingBt.Visibility = daqSettingBt.Visibility = Visibility.Collapsed;

                // Hipot 인경우 온도 표시 숨김 (211104 : 유경석 사원 요청)
                this.tempName.Visibility = Visibility.Hidden;
                this.templb.Visibility = Visibility.Hidden;
                this.tempUnit.Visibility = Visibility.Hidden;
            }
            else if (this.pro_Type == ProgramType.VoltageInspector)
            {
                cmc = new CCMC(this);
                this.cmcStatus.Visibility = Visibility.Visible;

                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                {
                    keysight = new CKeysightDMM(this, true, keysight_PortName);
                }), null);

                this.dmmStatus.Visibility = Visibility.Visible;

                isNeedDCIRPanel.Width = isNeedOCV.Height = new GridLength(0);
                topgridHeight.Height = new GridLength(200);
                btgridHeight.Height = new GridLength(100);
                cyclerSettingBt.Visibility = daqSettingBt.Visibility = Visibility.Collapsed;
            }
            else if (this.pro_Type == ProgramType.EOLInspector)
            {
                cmc = new CCMC(this);
                this.cmcStatus.Visibility = Visibility.Visible;

                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                {
                    keysight = new CKeysightDMM(this, true, keysight_PortName);
                }), null);

                this.dmmStatus.Visibility = Visibility.Visible;

                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                {
                    daq = new CDAQ(this, daq_PortName);
                }), null);

                this.daqStatus.Visibility = Visibility.Visible;

                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                {
                    cycler = new CCycler(this, this.can_cycler1);
                }), null);

                this.cyclerStatus.Visibility = Visibility.Visible;
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                plc = new CPLC(this, PLC_RECV_ADDRESS, PLC_SEND_ADDRESS, PLC_BCRS_ADDRESS, PLC_LOGICAL_NUMBER);
            }), null);
        }

        private void DisposeAllModule()
        {
            ProgressWindow pw = new ProgressWindow(this, 7);
            pw.Show();

            new Thread(() =>
            {
                this.isCLOSED = true;
                Thread.Sleep(500);

                try
                {

                    if (cycler != null)
                    {
                        cycler._Dispose();
                        pw.SetString("cycler._Dispose();"); 
                    }

                    if (keysight != null)
                    {
                        keysight._Dispose();
                        pw.SetString("keysight._Dispose();");
                    }

                    if (cmc != null)
                    {
                        cmc._Dispose();
                        pw.SetString("cmc._Dispose();");
                        //210323 wjs add port & relay num info log
                        LogState(LogType.Info, "CMC Port : " + cmc_PortName + " CMC Relay Number : " + CMC_Relay);
                    }

                    if (MES != null)
                    {
                        MES._Dispose();
                        pw.SetString("MES._Dispose();");

                    }

                    if (chroma != null)
                    {
                        chroma._Dispose();
                        pw.SetString("chroma._Dispose();");
                    }

                    if (daq != null)
                    {
                        daq._Dispose();
                        pw.SetString("daq._Dispose();");
                    }

                    loggerThread.Abort();
                    pw.SetString("loggerThread.Abort();");
                }
                catch (Exception ec)
                {
                    this.LogState(LogType.Fail, "DisposeAllModule",ec);
                }
                finally
                {
                    System.Windows.Forms.Application.Restart();
                    System.Environment.Exit(0);
                }
            }).Start();
        }


        private void SetUI()
        {
            for (int i = 0; i < modelDataList.Count; i++)
            {
                var modelRb = new RadioButton();
                modelRb.Content = modelDataList[i].ModelId.Replace("_","-");
                modelRb.Name = "type" + i.ToString() + "Rb";
                modelRb.GroupName = "ModelsGroup";
                //modelRb.MouseLeftButtonUp += sideChangedUp;
                modelRb.Checked += ModelRb_Checked;
                modelRb.Style= this.FindResource("likeToggleBt") as Style;
                modelRb.FontSize = 6;
                modelRb.FontWeight = FontWeights.SemiBold;
                modelNameGrid.Children.Add(modelRb);
            }
            (modelNameGrid.Children[0] as RadioButton).IsChecked = true;

            selectedRb = (modelNameGrid.Children[0] as RadioButton);

            //0번으로 초기화
            viewModel = new TestModel()
            {
                ProdId = modelDataList[0].ProdId,
                ProcId = modelDataList[0].ProcId,
                EquipId = modelDataList[0].EquipId,
                UserId = "EIF",
                ModelId = modelDataList[0].ModelId,
                PLCBit_A = modelDataList[0].PLCBit_A,
                PLCBit_B = modelDataList[0].PLCBit_B,
                PLCBit_C = modelDataList[0].PLCBit_C,
                PLCBit_D = modelDataList[0].PLCBit_D,
                PLCBit_E = modelDataList[0].PLCBit_E,
                PLCBit_F = modelDataList[0].PLCBit_F,
                //LotId = "NOT_LOT_ID"
                LotId = ""//200730 제어/수집에 LOT을 요청함에 따라 비워두는 것을 기본으로 함
            };

            mmw = new MiniScheduler.MainWindow(this);
            this.totalProcessList = mmw.totalProcessList;

            this.prodTb.Text = viewModel.ProdId;//제품 ID
            this.procTb.Text = viewModel.ProcId;//공정ID
            this.equipTb.Text = viewModel.EquipId;
            this.userTb.Text = viewModel.UserId;

            cellDetailList.Clear();
            cellDetailList.Add(new CellDetail() { TestName = "Init Volt" });
            cellDetailList.Add(new CellDetail() { TestName = "Discharge" });
            cellDetailList.Add(new CellDetail() { TestName = "Rest" });
            cellDetailList.Add(new CellDetail() { TestName = "Charge" });
            cellDetailList.Add(new CellDetail() { TestName = "Finish Volt" });
            ClearCellDetailList();

            ////191120 그래프 파라미터 추가 부분
            //voltGraph.MaxY = graph_Volt_Max;
            //voltGraph.MinY = graph_Volt_Min;
            //voltGraph.SeriesSource = voltItemList;

            //curGraph.MaxY = graph_Cur_Max;
            //curGraph.MinY = graph_Cur_Min;
            //curGraph.SeriesSource = curItemList;


            last = start = DateTime.Now;
        }

        Thread modelChangeTh;

        RadioButton selectedRb = null;

        private void ModelRb_Checked(object sender, RoutedEventArgs e)
        {
            //처음 로드시에 MES는 Null일수밖에 없다
            if (MES == null)
                return;

            if (modelChangeTh != null)
                return;

            //210331 설비 시작일때 권한 확인 안하기
            if (!plc.plc_isStart)
            {
                if (!CheckPermission(PERMISSION.ModelButton))
                {
                    PermissionError();

                    (sender as RadioButton).IsChecked = false;

                    selectedRb.Checked -= ModelRb_Checked;
                    selectedRb.IsChecked = true;
                    selectedRb.Checked += ModelRb_Checked;

                    return;
                }
            }

            modelNameGrid.IsEnabled = false;
            var lb = sender as RadioButton;

            selectedRb = lb;

            SetViewModel_To_Event(lb.Content.ToString().Replace("-", "_"));

            this.LoadList();

            modelChangeTh = new Thread(() =>
                {
                    GetControlItemFromCSV();
                    GetCollectItemFromCSV();

                    //plc mode change, no download spec
                    //if (!plc.plc_isStart)

                    //200401 택타임 타이머가 지금시점에 돌고있다면, 설비에서 시작한것이므로
                    //MES로부터 스펙을 받아오지 아니한다.
                    if (!_stopwatch.IsRunning)
                    {
                        //190124 ht MES 스킵되있을때는 스펙받아오지 아니한다
                        if (!isMESskip)
                        {
                            GetControlItemFromMES();
                            GetCollectItemFromMES();
                        }

                        SetFieldsToMESData();
                        SetCyclerFieldsToMESData();
                    }

                    this.Dispatcher.Invoke(new Action(() =>
                        {
                            this.testItemListDg.Items.Refresh();

                            modelNameGrid.IsEnabled = true;
                            modelChangeTh = null;
                            //var item = stwthfshdshf.ElapsedMilliseconds;
                            //stwthfshdshf.Stop();
                            //var dd = stwthfshdshf.IsRunning;
                        }));
                });
            modelChangeTh.Start();

            if (pro_Type == ProgramType.EOLInspector || pro_Type == ProgramType.VoltageInspector)
            {
                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR || localTypes == ModelType.MASERATI_NORMAL ||
                    localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                    || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                    )
                {
                    Change_CMC();
                }
            }
        }

        //public System.Diagnostics.Stopwatch stwthfshdshf = new System.Diagnostics.Stopwatch();

        public void ClearOcvDetailList()
        {
            foreach (var item in ocvDetailList)
            {
                item.CellVolt_1 = item.CellVolt_2 = item.CellVolt_3 = item.CellVolt_4 = item.CellVolt_5 = item.CellVolt_6 = item.CellVolt_7 = item.CellVolt_8 = 0.0;
            }
        }

        private void InitializeSaveFileAddr()
        {
            DirectoryInfo di = new DirectoryInfo(@logaddr);
            if (di.Exists == false)
            {
                di.Create();
            }

            string @LogDirectory = @FILE_PATH_INSPECTION_RESULT + MODEL_NAME;

            if (!Directory.Exists(@LogDirectory))
            {
                Directory.CreateDirectory(@LogDirectory);
            }

            if (!Directory.Exists(@logaddr))
            {
                Directory.CreateDirectory(@logaddr);
            }

            //221018 소모품 카운트 기능 관련 폴더 확인
            string strPathLog = @FILE_PATH_SPAREPATSCOUNT + position + "\\Log";
            if (!Directory.Exists(strPathLog))
            {
                Directory.CreateDirectory(strPathLog);
            }
        }

        private void InitializeCommonMethods()
        {
            bt_pass.Content = "-";
            bt_pass.Background = Brushes.DarkGray;
            timeThread = new Thread(() =>
            {
                while (true)
                {
                    if (timeThreadFlag)
                    {
                        try
                        {
                            if (_stopwatch.IsRunning)
                            {
                                sp = _stopwatch.Elapsed;

                                strmsg = string.Format("{0:00}:{1:00}:{2:00}", sp.Minutes, sp.Seconds, sp.Milliseconds / 10);

                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    time_elapsed_tb.Text = strmsg;
                                }));
                            }
                        }
                        catch
                        {

                        }
                    }
                    Thread.Sleep(50);
                }
            });
            timeThread.Start();
            //timetick.Interval = 100;
            //timetick.Tick += ti_Tick;
            this.Loaded += MainWindow_Loaded;
            //this.KeyDown += MainWindow_KeyDown;
            this.Closed += MainWindow_Closed;
            this.Closing += MainWindow_Closing;
            this.KeyDown += MainWindow_KeyDown;
            this.MouseMove += MainWindow_MouseMove;
        }

        /// <summary>
        /// 2개이상 프로그램을 띄울 때, 구분을 위한 config.ini 파일 생성부
        /// </summary>
        /// <param name="url"></param>
        private void LoadINI(string url)
        {
            try
            {
                if (!System.IO.File.Exists(url))
                {
                    WritePrivateProfileString("Set Position", "#1,#2,#3,#4,#5,#6", "#1", url);
                    this.LogState(LogType.Info, "Make INI");
                }
                else
                {
                    StringBuilder temp = new StringBuilder(255);

                    GetPrivateProfileString("Set Position", "#1,#2,#3,#4,#5,#6", "", temp, 255, url);
                    position = temp.ToString();

                    //4. 검사기 공통) All of All log 추가
                    //-> $1 형태를 $1$ 과같이 뒤에 $만 추가
                    All_of_All_Header = position.Replace('#', '$') + "$";
                    //labelPos.Content = position;

                    this.LogState(LogType.Info, "Load INI");
                }
                this.LogState(LogType.Info, "Program Side - " + position);
            }
            catch (Exception ec)
            {
                this.LogState(LogType.Fail, "Load INI", ec);
            }
        }

        /// <summary>
        /// 메서드 이름으로 실제 코드에서 찾아 동작시키는 메서드.
        /// 구현된 메서드 이름이 없다면 예외발생
        /// </summary>
        /// <param name="method"></param>
        /// <param name="objArr"></param>
        /// <returns></returns>
        public bool MethodInvoker(string method, object[] objArr)
        {
            try
            {
                MethodInfo mi = this.GetType().GetMethod(method);

                var ti = objArr[0] as TestItem;

                if (isDummy)
                {
                    ti.Value_ = 0;
                    ti.Result = "PASS";
                    this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                    return true;
                }

                return (bool)mi.Invoke(this, objArr);
            }
            catch (Exception ec)
            {
                if (ec is NullReferenceException)
                {
                    LogState(LogType.Fail, "MethodInvoker - Not Exist Methods (" + method + ")");
                }
                else
                {
                    LogState(LogType.Fail, "MethodInvoker", ec);
                }

            }
            return false;
        }

        #region eollist.csv를 실제 메서드로 만들고 이벤트 걸어주는 부분

        private void LoadModelList()
        {
            try
            {
                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
                //181218 여러개 모델일 때 버튼이 새로 추가 안되는 현상 FIX
                groupDic = new Dictionary<string, List<TestItem>>();
                groupDic.Clear();
                var li = new List<TestItem>();

                var fileFolder = "EOL";

                switch (pro_Type)
                {
                    case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                    case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                    case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                    case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                }

                FileStream readData = new FileStream(staticPath_EOL_MOD_LIST + "\\" + position + "\\"+ fileFolder+"\\" + "modellist.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                StreamReader streamReader = new StreamReader(readData, encode);

                int cnt = 0;
                while (streamReader.Peek() > -1)
                {
                    if (cnt == 0)
                    {
                        var columnHeaders = streamReader.ReadLine();
                        cnt++;
                    }
                    else
                    {
                        var content = streamReader.ReadLine();

                        var ar = content.Split(',');
                        if (ar[0] == "")
                            continue;

                        var model = new TestModel();
                        model.ModelId = ar[0];
                        model.ModelType = GetModelTypeToStr(ar[0]);
                        model.EquipId = ar[1];
                        model.ProdId = ar[2];
                        model.ProcId = ar[3];
                        model.PLCBit_A = int.Parse(ar[4]) == 1 ? true : false;
                        model.PLCBit_B = int.Parse(ar[5]) == 1 ? true : false;
                        model.PLCBit_C = int.Parse(ar[6]) == 1 ? true : false;
                        model.PLCBit_D = int.Parse(ar[7]) == 1 ? true : false;
                        model.PLCBit_E = int.Parse(ar[8]) == 1 ? true : false;
                        model.PLCBit_F = int.Parse(ar[9]) == 1 ? true : false;

                        modelDataList.Add(model);
                    }
                }

                LogState(LogType.Success, "Load Model List");
            }
            catch (Exception ec)
            {
                LogState(LogType.Fail, "Load Model List", ec);
            }
        }

        public ModelType GetModelTypeToStr(string mod)
        {
            if (mod == ModelType.AUDI_NORMAL.ToString())
                return ModelType.AUDI_NORMAL;
            else if (mod == ModelType.AUDI_MIRROR.ToString())
                return ModelType.AUDI_MIRROR;
            else if (mod == ModelType.PORSCHE_NORMAL.ToString())
                return ModelType.PORSCHE_NORMAL;
            else if (mod == ModelType.PORSCHE_MIRROR.ToString())
                return ModelType.PORSCHE_MIRROR;
            else if (mod == ModelType.MASERATI_NORMAL.ToString())
                return ModelType.MASERATI_NORMAL;
            else if (mod == ModelType.PORSCHE_FACELIFT_NORMAL.ToString()) //210312 wjs add pors fl
                return ModelType.PORSCHE_FACELIFT_NORMAL;
            else if (mod == ModelType.PORSCHE_FACELIFT_MIRROR.ToString()) //210312 wjs add pors fl
                return ModelType.PORSCHE_FACELIFT_MIRROR;
            else if (mod == ModelType.MASERATI_M183_NORMAL.ToString()) //221101 wjs add mase m183
                return ModelType.MASERATI_M183_NORMAL;
            else if (mod == ModelType.E_UP.ToString())
            	return ModelType.E_UP;
            else
                return ModelType.DEFAULT;
        }

        /// <summary>
        /// eolList.csv에서 실제 검사항목으로 불러오는 부분
        /// </summary>
        public void LoadList()
        {
            try
            {
                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
                //181218 여러개 모델일 때 버튼이 새로 추가 안되는 현상 FIX
                groupDic = new Dictionary<string, List<TestItem>>();
                groupDic.Clear();
                var li = new List<TestItem>();

                var fileFolder = "EOL";

                switch (pro_Type)
                {
                    case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                    case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                    case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                    case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                }

                FileStream readData = new FileStream(staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "eollist.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                StreamReader streamReader = new StreamReader(readData, encode);

                int cnt = 0;
                while (streamReader.Peek() > -1)
                {
                    if (cnt == 0)
                    {
                        var columnHeaders = streamReader.ReadLine();
                        cnt++;
                    }
                    else
                    {
                        #region item parse
                        var content = streamReader.ReadLine();

                        var ar = content.Split(',');
                        if (ar[0] == "")
                            continue;

                        var groupContent = ar[0];

                        var groupName = ar[0].Replace(" ", "").Replace("(", "_").Replace(")", "_").Replace("-", "_").Replace("/", "").Replace("%", "").Replace("~","");

                        var singleContent = ar[1];

                        var singleName = ar[1].Replace(" ", "").Replace("(", "_").Replace(")", "_").Replace("/", "SLASH").Replace("%", "PERCENT").Replace("+", "PLUS").Replace("-", "MINUS").Replace("#", "SHARP").Replace(".", "DOT").Replace("→", "_");

                        var clctItem = ar[2];
                        var min = ar[3];
                        var max = ar[4];
                        var unit = ar[5];
                        var digit = ar[6];
                        var isUse = ar[8];

                        if (isUse == "0")
                            continue;

                        var button = new Button();
                        button.Name = "bt_" + singleName;
                        button.Style = this.FindResource("manual_bt_s") as Style;
                        button.Width = 260;
                        button.FontSize = 12;
                        button.Content = singleContent;
                        button.Click += SingleButtonClick;

                        //localTypes에 따라 선별해서 불러온다
                        switch (localTypes)
                        {
                            case ModelType.AUDI_NORMAL:
                                if (ar[9] == "0") continue;
                                break;
                            case ModelType.AUDI_MIRROR:
                                if (ar[10] == "0") continue;
                                break;
                            case ModelType.PORSCHE_NORMAL:
                                if (ar[11] == "0") continue;
                                break;
                            case ModelType.PORSCHE_MIRROR:
                                if (ar[12] == "0") continue;
                                break;
                            case ModelType.MASERATI_NORMAL:
                                if (ar[13] == "0") continue;
                                break;
                            case ModelType.PORSCHE_FACELIFT_NORMAL://210312 wjs add pors fl
                                if (ar[14] == "0") continue;
                                break;
                            case ModelType.PORSCHE_FACELIFT_MIRROR://210312 wjs add pors fl
                                if (ar[16] == "0") continue;
                                break;
                            case ModelType.MASERATI_M183_NORMAL://221101 wjs add mase m183
                                if (ar[17] == "0") continue;    //221101 wjs 체크필요
                                break;
                            case ModelType.E_UP:
                                if (ar[15] == "0") continue;
                                break;
                            default:
                                break;
                        }


                        try
                        {
                            var obj = this.FindName(button.Name);
                            if (obj == null)
                            {
                                this.RegisterName(button.Name, button);
                            }
                            else
                            {
                                this.UnregisterName(button.Name);
                                this.RegisterName(button.Name, button);
                            }
                        }
                        catch (Exception esc)
                        {
                            LogState(LogType.Fail, "LoadList", esc);
                        }

                        var ti = new TestItem();
                        ti.CLCTITEM = clctItem;
                        ti.Max = max;
                        ti.Min = min;
                        ti.Unit = unit;
                        ti.No = cnt;
                        ti.Bt = button;
                        ti.GroupName = groupName;
                        ti.Name = singleName;
                        ti.SingleMethodName = ar[7];
                        ti.DigitLength = digit;

                        #region Add SelfDiag
                        var selfIndex = 17; //16;// 15;
                        if (ar.Length > selfIndex)
                        {
                            if (ar[selfIndex] != "")
                            {
                                ti.selfDiagDirectory = ar[selfIndex];
                            }
                        }

                        //if (ar.Length > 16)
                        //{
                        //    string head = "";
                        //    string addr = "";
                        //    if (ar[16] != "" && MakeSelfDiagItem(ar[16], out head, out addr))
                        //    {
                        //        ti.selfDiagList.Add(new SelfDiagItem() { Header = head, FileAddress = addr });
                        //    }
                        //}

                        #endregion


                        if (!groupDic.ContainsKey(groupContent))
                        {
                            li = new List<TestItem>();
                            li.Add(ti);

                            groupDic.Add(groupContent, li);

                        }
                        else
                        {
                            groupDic[groupContent].Add(ti);
                        }
                        //MakeSingleMethodToTextFile(ti.SingleMethodName);

                        cnt++;
                        #endregion
                    }

                }

                viewModel.TestItemList.Clear();
                //181218 여러개 모델일 때 버튼이 새로 추가 안되는 현상 FIX
                btgrid.Children.Clear();
                #region Add bt in Grid
                foreach (var dicItem in groupDic)
                {
                    var singleGrid = new Grid();
                    singleGrid.Margin = new Thickness(3);
                    var comboBox = new ComboBox();
                    comboBox.FlowDirection = System.Windows.FlowDirection.LeftToRight;
                    foreach (var initem in dicItem.Value)
                    {
                        comboBox.Items.Add(initem.Bt);
                        viewModel.TestItemList.Add(initem.Bt.Content.ToString(), initem);
                    }

                    //MakeGroupMethodToTextFile(sdic);
                    singleGrid.Children.Add(comboBox);
                    var groupButton = new Button();
                    groupButton.Content = dicItem.Key;
                    groupButton.Name = dicItem.Value[0].GroupName;
                    groupButton.Margin = new Thickness(0, 0, 30, 0);
                    groupButton.Click += GroupButtonClick;
                    groupButton.FontSize = 13;
                    groupButton.Style = this.FindResource("manual_bt_s") as Style;
                    //7.
                    //190104 grchoi
                    //20181226 wjs try~catch add(unregister->register)                    
                    try
                    {

                        var obj = this.FindName(groupButton.Name);
                        if (obj == null)
                        {
                            this.RegisterName(groupButton.Name, groupButton);
                        }
                        else
                        {
                            this.UnregisterName(groupButton.Name);
                            this.RegisterName(groupButton.Name, groupButton);
                        }
                    }
                    catch (Exception esf)
                    {

                        LogState(LogType.Fail, "LoadList", esf);
                    }
                    singleGrid.Children.Add(groupButton);

                    btgrid.Children.Add(singleGrid);
                }
                #endregion
                bt_save = new Button();
                bt_save.Margin = new Thickness(5);
                bt_save.Content = "Save Data";
                bt_save.Name = "bt_save";
                bt_save.Click += bt_save_Click;
                bt_save.Style = this.FindResource("manual_bt_s") as Style;
                bt_save.FontSize = 15;
                btgrid.Children.Add(bt_save);
                //<Button Margin="5" Grid.Column="2" Grid.Row="1" Content="Save Data" Name="bt_save" Click="bt_save_Click" Style="{StaticResource manual_bt_s}" FontSize="15"/>

                this.testItemListDg.ItemsSource = viewModel.TestItemList;

                LogState(LogType.Info, "Load Local Spec(CollectItem).");
            }
            catch (Exception ec)
            {
                LogState(LogType.Fail, "LoadList", ec);
            }
        }

        bool MakeSelfDiagItem(string array, out string key, out string addr)
        {
            try
            {
                array = array.Replace("[", "");
                array = array.Replace("]", "");
                var arrays = array.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                key = arrays[0];
                addr = arrays[1];
                return true;
            }
            catch (Exception ec)
            {
                LogState(LogType.Fail, "MakeSelfDiagItem", ec);
                key = "";
                addr = "";
                return false;
            }
        }


        ///// <summary>
        ///// 편의를 위해 특정 파일에 메서드를 텍스트로 만들어주는 부분
        ///// </summary>
        ///// <param name="dicItem"></param>
        //private void MakeGroupMethodToTextFile(Dictionary<string, List<TestItem>> dicItem)
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendLine("//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        //    foreach (var methodName in dicItem)
        //    {
        //        sb.AppendLine("public bool " + methodName.Key + "(TestItem ti)");
        //        sb.AppendLine("{");
        //        sb.AppendLine("     isProcessingUI(ti);");
        //        sb.AppendLine("     ti.Value_ = null;");
        //        sb.AppendLine("     return JudgementTestItem(ti);");
        //        sb.AppendLine("}");
        //        sb.AppendLine("");
        //    }
        //    File.AppendAllText("D:\\MethodFile.txt", sb.ToString() + "\r\n", Encoding.UTF8);
        //}

        ///// <summary>
        ///// 편의를 위해 특정 파일에 메서드를 텍스트로 만들어주는 부분
        ///// </summary>
        //private void MakeSingleMethodToTextFile(string methodName)
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendLine("public bool " + methodName + "(TestItem ti)");
        //    sb.AppendLine("{");
        //    sb.AppendLine("     isProcessingUI(ti);");
        //    sb.AppendLine("     ti.Value_ = null;");
        //    sb.AppendLine("     return JudgementTestItem(ti);");
        //    sb.AppendLine("}");
        //    sb.AppendLine("");
        //    File.AppendAllText("D:\\MethodFile.txt", sb.ToString() + "\r\n", Encoding.UTF8);
        //}

        Thread GroupBtThread, SingleBtThread;


        /// <summary>
        /// 그룹버튼을 눌렀을 때, 해당 그룹이름에 맞는 모든 검사항목을 순차적으로 돌린다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupButtonClick(object sender, RoutedEventArgs e)
        {
            if (GroupBtThread != null)
                return;

            if (!CheckPermission(PERMISSION.GroupItemButton))
            {
                PermissionError();
                return;
            }

            isStop = ispause = isEmg_ = false;
            string gbtName = (sender as Button).Name.ToString();

            if (gbtName == "BarcodeReading")
            {
                viewModel.LotId = lotTb.Text;
            }

            GroupBtThread = new Thread(() =>
            {
                //검사 전           
                bool isPass = true;

                foreach (var item in viewModel.TestItemList)
                {
                    if (item.Value.GroupName == gbtName)
                    {
                        var rst = MethodInvoker(item.Value.SingleMethodName, new object[] { item.Value });
                        if (!rst)
                        {
                            isPass = false;
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                SetCtrlToPass(false, item.Value.Bt.Name);
                            }));
                        }
                        else
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                SetCtrlToPass(true, item.Value.Bt.Name);
                            }));
                        }

                        Thread.Sleep(1);//항목간 간격
                    }
                    if (isStop || IsEmg)
                    {
                        break;
                    }
                }
                SetCtrlToPass(isPass, gbtName);
                //검사후                 

                GroupBtThread = null;
            });
            GroupBtThread.Priority = ThreadPriority.Highest;
            GroupBtThread.Start();
        }

        /// <summary>
        /// 단일검사시 동작하는 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SingleButtonClick(object sender, RoutedEventArgs e)
        {
            isStop = false;
            if (SingleBtThread != null)
                return;

            if (!CheckPermission(PERMISSION.SingleItemButton))
            {
                PermissionError();
                return;
            }

            isStop = ispause = isEmg_ = false;
            string sbtName = (sender as Button).Content.ToString();
            //8.바코드리딩 단위검사 시 Lot TextBox 정보가 lot id 저장되도록 적용
            //(1.그룹버튼, 2.싱글버튼)
            //190105 by grchoi
            if (sbtName == "BarcodeReading")
            {
                viewModel.LotId = lotTb.Text;
            }
            foreach (var item in viewModel.TestItemList)
            {
                if (item.Value.Bt.Content.ToString() == sbtName)
                {
                    SingleBtThread = new Thread(() =>
                        {
                            MethodInvoker(item.Value.SingleMethodName, new object[] { item.Value });

                            SingleBtThread = null;
                        });
                    SingleBtThread.Start();
                }
            }
        }
        Button bt_save;

        #endregion


        Random rnd = new Random();
        int i = 0;

        /// <summary>
        /// 전역 키다운 이벤트,
        /// 기본값은 F2가 충방전 스케줄링 로드
        /// F3은 오토스타트
        /// 가끔 안되는 펑션키가 있다(눌러도 이벤트를 안탄다)
        /// 취향에 맞게 설정~
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    {
                        ContinueWindow ct = new ContinueWindow(this);
                        ct.maintitle.Content = "EOL INSPECTION";
                        ct.reason.Content = "ver. " + lastUpdated;
                        ct.contibt.Content = "OK";
                        ct.shockLb.Content = "※";
                        ct.Show();
                    }; break;
                case Key.F2:
                    {
                        //CheckBCRforMaster_Diag("test1");
                        //var th = new Thread(() =>
                        //    {
                        //        clsw.SetCyclerLimit(15.1, 9.9, 120, 15);
                        //    });
                        //th.Start();
                    }; break;
                case Key.F3:
                    {
                    }; break;
                    //case Key.F4:
                    //    {
                    //        //TimeSpan span = DateTime.Now - last;
                    //        ////TimeSpan totalSpan = DateTime.Now - start;
                    //        //int previousTime = voltItemList.Count > 0 ? voltItemList[voltItemList.Count - 1].Time : 0;
                    //        //GraphItem newItem = new GraphItem {
                    //        //    Time = (int)(previousTime + span.TotalMilliseconds),
                    //        //    Value = rnd.Next(graph_Volt_Min, graph_Volt_Max)
                    //        //};
                    //        //voltItemList.Add(newItem);
                    //        //newItem = new GraphItem { Time = (int)(previousTime + span.TotalMilliseconds), Value = rnd.Next(graph_Cur_Min, graph_Cur_Max) };
                    //        //curItemList.Add(newItem);
                    //        //last = DateTime.Now;
                    //        DisposeAllModule();
                    //    };break;

            }
        }

        /// <summary>
        /// 설비에서 시작 직후 MES에서 값을 잘못 가져왔을 때, 
        /// 빠져나가기 전에 퍼즈가 들어와있으면 대기타도록 되어있다.
        /// </summary>
        /// <param name="log"></param>
        private void PauseLoop(string log)
        {
            if (ispause)
            {
                LogState(LogType.Info, log + " - PAUSE");
            }

            bool isg = false;
            while (ispause)
            {
                isg = true;
                Thread.Sleep(100);
            }
            if (isg)
            {
                LogState(LogType.Info, "RESUME");
            }
        }

        public void StartTimer()
        {
            #region time start

            LogState(LogType.Info, "START_TEST-------------------------------------------------------", null, false);

            this.Dispatcher.Invoke(new Action(() =>
            {
                this.time_start_tb.Text = this.time_finish_tb.Text = this.time_elapsed_tb.Text = "";
                this.testPb.Value = 0;
                nowTime = DateTime.Now;
                this.time_start_tb.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                elapsedtime = new DateTime();
                this.time_elapsed_tb.Text = elapsedtime.ToString("HH:mm:ss");
            }));

            _stopwatch.Reset(); 
            _stopwatch.Start();
            timeThreadFlag = true;
            this.LogState(LogType.Info, "Elapsed timer Started");


            #endregion
        }
        /// <summary>
        /// 바코드를 읽었을 때 시작
        /// </summary>
        public void AutoMode()
        {
            //이미 동작중에 다시 시작이 들어온다면 리턴처리
            if (autoModeThread != null && autoModeThread.IsAlive)
                return;

            //200401 시작부분 이동
            if (plc != null)
            {
                plc.isTesting(true);
            }

            ////설비에서 시작한게 아니라면, 여기서 타이머를 켜야한다.
            //if (!plc.isTesting_Flag)
            //{
            //    StartTimer();
            //}

            //200401 타이머가 켜져있지 않다면 여기서 타이머를 켜야한다.
            if (!_stopwatch.IsRunning)
            {
                StartTimer();
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                this.viewModel.LotId = Barcode = this.lotTb.Text;// = "FORD12V10AHCMAT06291990002";
                MonoFrame = this.monoTb.Text;
            }));

            //200213
            if (!CheckBCRforMaster_Diag(Barcode))
            {
                plc.TestResult(false);
                return;
            }
            
            //검사값 비우기
            ResetClick(this, null);
                      
            isMESSkipCb.IsEnabled = manualBt.IsEnabled = resetBt.IsEnabled = false;
            labelA.IsEnabled = labelM.IsEnabled = startAutoBt.IsEnabled = false;
            blinder3.Visibility = blinder2.Visibility = Visibility.Visible;

            bt_pass.Content = "RUN";
            bt_pass.Background = Brushes.Yellow;

            ThreadStart tstart = new ThreadStart(this.AutoModeStart);
            autoModeThread = new Thread(tstart);
            autoModeThread.IsBackground = true;
            autoModeThread.Start();

            this.LogState(LogType.Info, "Auto mode Started");
        }

        private void AutoModeStart()
        {
            //200630 LV 검사기 추가
            //if (localTypes == ModelType.MEB_12S || localTypes == ModelType.MEB_8S)

            if (pro_Type == ProgramType.VoltageInspector || pro_Type == ProgramType.EOLInspector)
                PreCheck_with_LV();

            if (plc != null)
                plc.isSuc = false;

            bool mesRetryLogic = false;

            this.Dispatcher.Invoke(new Action(() =>
            {
                if (!isMESskip && !MES.isMESConnected)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (!MES.isMESConnected)
                        {
                            LogState(LogType.Fail, string.Format("Not connected MES. {0}", (i + 1).ToString()));
                            MES.StartConnect();
                            Thread.Sleep(1000); //210419 Add by KYJ
                        }
                        else
                        {
                            LogState(LogType.Success, "Connected MES.");
                            tlamp.SetTLampMESStatus(true);
                            break;
                        }
                    }
                    tlamp.SetTLampMESStatus(false);
                    mesRetryLogic = true;
                }
            }));

            isStop = false;
            double testCount = this.viewModel.TestItemList.Count;
            double progVal = 100.0 / testCount;
            bool isPass = true;

            //prodId = this.Barcode;//제품 ID
            //userId = "EIF";
            //procId = "P8100";//공정ID
            //eqpId = "W1PBMA101-4-4";
            this.viewModel.LotId = this.Barcode;// "LGC-WAH;B0011PL230001";

            this.Dispatcher.Invoke(new Action(() =>
            {
                isMESskip = isMESSkipCb.IsChecked == false ? false : true;
                this.viewModel.UserId = userTb.Text;
            }));

            //12. AutomodeStart 시 검사모델/설비ID/제품ID/공정ID를 로그로 남김
            LogState(LogType.Info, "Started Model : " + this.viewModel.ModelId);
            LogState(LogType.Info, "Lot ID : " + this.viewModel.LotId);
            LogState(LogType.Info, "Process ID : " + this.viewModel.ProcId);
            LogState(LogType.Info, "Product ID : " + this.viewModel.ProdId);
            LogState(LogType.Info, "Equipment ID : " + this.viewModel.EquipId);

            #region 착공, 컨트롤스펙, 프로세싱 스펙
            if (!isMESskip)
            {
                if (!MES.isMESConnected)
                {
                    this.LogState(LogType.Fail, "MES_NOT_CONNECTED");

                    PauseLoop("MES_NOT_CONNECTED");

                    Finished(false,false, -1);

                    SaveData(false);

                    return;
                }


                if (!GetControlItemFromMES(this.viewModel.LotId))
                {
                    Thread.Sleep(1000);
                    //MES가 끊어져, 한번 시도시 실패하면 위의 로직을 타게되고
                    //제어항목으로 한번더 받아오도록 시도하게 수정
                    if (mesRetryLogic)
                    {
                        if (!GetControlItemFromMES(this.viewModel.LotId))
                        {
                            this.LogState(LogType.Fail, "GetControlItemFromMES");

                            PauseLoop("GetControlItemFromMES");

                            Finished(false, false, 1);

                            SaveData(false);

                            return;
                        }
                    }
                    else
                    {
                        this.LogState(LogType.Fail, "GetControlItemFromMES");

                        PauseLoop("GetControlItemFromMES");

                        Finished(false, false, 1);

                        SaveData(false);

                        return;
                    }
                }

                if (!GetCollectItemFromMES(this.viewModel.LotId))
                {
                    Thread.Sleep(1000);
                    this.LogState(LogType.Fail, "GetCollectItemFromMES");

                    PauseLoop("GetCollectItemFromMES");

                    Finished(false, false, 2);

                    SaveData(false);

                    return;
                }

                //착공보고
                if (MES.StartJobInsp(
                    this.viewModel.LotId,
                    this.viewModel.ProcId,
                    this.viewModel.EquipId,
                    this.viewModel.UserId) == "NG") //OK일때 시작
                {
                    this.LogState(LogType.Fail, "MES_StartJobInsp");

                    PauseLoop("MES_StartJobInsp");

                    Finished(false, false, 3);

                    SaveData(false);

                    return;
                }
            }
            else
            {
                GetControlItemFromCSV();
                GetCollectItemFromCSV();
            }
            SetFieldsToMESData();
            SetCyclerFieldsToMESData();

            if (pro_Type == ProgramType.VoltageInspector)
            {
                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR ||
                    localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                    || localTypes == ModelType.MASERATI_M183_NORMAL  //221101 wjs add mase m183
                    )
                {
                    SetCellVoltageOrder();
                }
            }
            #endregion

            #region Tests

            ClearCellDetailList();
            ProgressRefresh(0);

            //221018 2년 이상 되는 파일을 지워야 하는 부분.
            var FolderDelete = new Thread(() =>
            {
                //지울 경로 여기에 추가해주면 됨
                StringBuilder loadLog = new StringBuilder();
                loadLog.Append(@logaddr);
                loadLog.Append("\\");
                loadLog.Append(this.MODEL_NAME);
                loadLog.Append("\\");
                loadLog.Append(this.viewModel.ModelId);
                loadLog.Append("\\");
                deleteFolder(loadLog.ToString());

                StringBuilder loadResult = new StringBuilder();
                loadResult.Append(@FILE_PATH_INSPECTION_RESULT);
                loadResult.Append("\\");
                loadResult.Append(this.MODEL_NAME);
                loadResult.Append("\\");
                loadResult.Append(this.viewModel.ModelId);
                loadResult.Append("\\");
                deleteFolder(loadResult.ToString());

            });

            foreach (var singleGroup in groupDic)
            {

                foreach (var item in this.viewModel.TestItemList)
                {
                    if (item.Value.GroupName == singleGroup.Value[0].GroupName)
                    {
                        var rst = MethodInvoker(item.Value.SingleMethodName, new object[] { item.Value });

                        if (!rst)
                        {
                            isPass = false;
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                SetCtrlToPass(false, item.Value.Bt.Name);
                            }));
                        }
                        else
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                SetCtrlToPass(true, item.Value.Bt.Name);
                            }));
                        }

                        Thread.Sleep(autobtwtick);
                        ProgressRefresh(progVal);
                    }
                    if (isStop || IsEmg)
                    {
                        SetCtrlToPass(isPass, singleGroup.Value[0].GroupName);
                        Finished(false,true);
                        SaveData(false);
                        return;
                    }
                }

                SetCtrlToPass(isPass, singleGroup.Value[0].GroupName);

                if (!isPass || isStop || IsEmg)
                {
                    Finished(false,true);
                    SaveData(false);
                    return;
                }
            }

            ProgressRefresh(100);
            #endregion

            SaveData(Finished(true,true));
            autoModeThread = null;
        }

        
        /// <summary>
        /// 17. MES Connect 시 MES 연결후에 isMESskip=true 후 발생하는 이벤트가 수집항목 및 제어항목
        /// 파싱 전 발생되지 않아(isMESSkipCb.isChecked = true가 늦게 적용됨) 데이터 파싱 에러야기
        /// 하여 강제 체크구문 적용
        /// </summary>
        private void MESConnect()
        {
            MES = new CMES(this);
            MES.retryConntectcnt = 0;
            var rtv = MES.StartConnect();
            if (rtv == "" && MES.isMESConnected)
            {
                MES.retryConntectcnt = 3;
                this.LogState(LogType.Success, "MES Connected");
                //isMESskip = false;
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    // 2019.06.28 jeonhj
                    // 프로그램 로드 후 mes 연결 동작 시 연결이 되면 배경을 Blue로 표시한다.
                    // Messkip ischecked를 false처리해두어 스킵 이벤트를 타지 않아 해당 배경색 변경 동작을 할 수 없다.
                    this.monoTb.Background = this.lotTb.Background = this.prodTb.Background = 
                        this.equipTb.Background =
                        this.procTb.Background = Brushes.SkyBlue;
                    isMESSkipCb.IsChecked = false;
                }));

                //Server Time Request 20200413 add by kyj
                if (position == "#1")
                {
                    for (int i = 0; i < 3; i++)
                    {
                        LogState(LogType.Info, string.Format("ServerTime_Request Start - {0}", i));

                        MES.ServerTime_Request(viewModel.EquipId);

                        if (MES.bIsRequestTime == true)
                        {
                            break;
                        }
                    }

                    if (MES.bIsRequestTime == false)
                    {
                        MessageBox.Show("To Request Time is Fail From Mes!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                //
            }
            else
            {
                //isMESskip = true;
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.monoTb.Background = this.lotTb.Background = this.prodTb.Background =
                        this.equipTb.Background =
                        this.procTb.Background = Brushes.Red;
                }));
            }
        }
        
        /// <summary>
        /// 검사종류가 여러개일 때 새로고침 되는 부분
        /// </summary>
        public void RefreshToFlags()
        {
            this.testItemListDg.ItemsSource = this.viewModel.TestItemList;
            
            this.prodTb.Text = this.viewModel.ProdId;//제품 ID
            this.procTb.Text = this.viewModel.ProcId;//공정ID
            this.equipTb.Text = this.viewModel.EquipId;
            
            Barcode = this.lotTb.Text = this.viewModel.LotId;// "J9D3-10B759-AB1-170118000001";//" "LGC-WAH;B0011PL230001";
            this.userTb.Text = this.viewModel.UserId;
            
        }

        /// <summary>
        /// 파라미터로 Test Item을 만든다.
        /// </summary>
        /// <param name="clctitem">보고용 수집항목 ID</param>
        /// <param name="name">검사 항목 이름</param>
        /// <param name="bt">검사대상 버튼(UI)</param>
        /// <param name="min">검사하한</param>
        /// <param name="max">검사상한</param>
        /// <param name="unit">UI에 표시되는 단위</param>
        /// <returns></returns>
        private TestItem MakeTestItem(string clctitem, string name, Button bt, double min, double max, string unit = "")//,DHandler dhandler)
        {
            var no = 0;
            if (this.viewModel.TestItemList.ContainsKey(name))
            {
                var p = this.viewModel.TestItemList[name];
                p.Max = max;
                p.Min = min;
                p.Name = name;
                p.No = p.No;
                p.Bt = bt;
                p.CLCTITEM = clctitem;
                p.Unit = unit;
                return p;
            }
            return new TestItem()
            {
                CLCTITEM = clctitem,
                No = no,
                Name = name,
                Min = min,
                Max = max,
                Bt = bt,
                Unit = unit
                //delegate1 = dhandler
            };
        }

        #region Events

        //권한없을때 종료 못하도록 todo
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(CheckPermission(PERMISSION.EXIT))
            {
                if (MessageBoxResult.OK == MessageBox.Show("Are you sure you want to Quit?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Information)) { }
                else { e.Cancel = true; }
            }
            else
            {
                PermissionError();
                e.Cancel = true;
            }
            
        }

        public void MES_UI_ENABLE(bool isEnable)
        {
            if (isEnable)
            {
                this.monoTb.Background = this.lotTb.Background = this.prodTb.Background =
                     this.equipTb.Background = this.procTb.Background = Brushes.SkyBlue;
            }
            else
            {
                this.monoTb.Background = this.lotTb.Background = this.prodTb.Background =
                       this.equipTb.Background =
                       this.procTb.Background = Brushes.Red;

            }
        }

        /// <summary>
        /// 18. MES Skip 및 Unskip시 해당 로그 추가 및 Module BCR Text Box 추가에 따라 색상변경 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MES스킵함(object sender, RoutedEventArgs e)
        {            
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.MESSkip))
            {
                MES_UI_ENABLE(false);

                isMESskip = true;
                LogState(LogType.Info, "MES Skip");
                if (relays == null)
                    return;

                // 2019.06.28
                // mes가 skip되면 로컬데이터를 가져와야 함.
                GetControlItemFromCSV();
                GetCollectItemFromCSV();

                tlamp.SetTLampMESStatus(false);

            }
            else
            {
                //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop); 
                PermissionError();
                e.Handled = true;
                isMESSkipCb.IsChecked = false;
            }
            //rp.Close();
        }


        private void MES스킵안함(object sender, RoutedEventArgs e)
        {
            isMESskip = false;
            LogState(LogType.Info, "MES Unskip");
            GetControlItemFromCSV();
            GetCollectItemFromCSV();

            if (MES.isMESConnected)
            {
                MES_UI_ENABLE(true); 

                isMESSkipCb.IsEnabled = false;
                //isMESskip = false;
                //LogState(LogType.Info, "MES Unskip");
                tlamp.SetTLampMESStatus(true);

                //여기넣는게 맞나?? 확인필요
                var th = new Thread(() =>
                {
                    //process control param 요청
                    GetControlItemFromMES();
                    GetCollectItemFromMES();

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        isMESSkipCb.IsEnabled = true;
                    }));
                });
                th.Start();
                
            }
            else
            {
                //isMESSkipCb.IsChecked = true;
                e.Handled = true;
                tlamp.SetTLampMESStatus(false);
                LogState(LogType.Fail, "Not Connected to MES");
            }
        }

        bool isSkipNG_ = false;
        string strmsg = "";
        TimeSpan sp;
        void ti_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_stopwatch.IsRunning)
                {
                    sp = _stopwatch.Elapsed;

                    strmsg = string.Format("{0:00}:{1:00}:{2:00}", sp.Minutes, sp.Seconds, sp.Milliseconds / 10);

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        time_elapsed_tb.Text = strmsg;
                    }));
                }
            }
            catch
            {

            }
        }
        private void NGSkip(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.NGSkip))
            {
                isSkipNG.IsChecked = true;
                e.Handled = true;
                isSkipNG_ = true;
                LogState(LogType.Info, "NG Skip");
            }
            else
            {
                isSkipNG.IsChecked = false;
                e.Handled = true;
                isSkipNG_ = false;

                //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
                PermissionError();
            }
            //rp.Close();
        }

        private void NGSkipUnchecked(object sender, RoutedEventArgs e)
        {
            isSkipNG.IsChecked = false;
            e.Handled = true;
            isSkipNG_ = false;
            LogState(LogType.Info, "NG Unkip");
            
        }

        public DetailItems MakeDetailItem(string key, string data = "")
        {
            var dti = new DetailItems() { Key = key };
            dti.Reportitems.Add(data);
            return dti;
        }

        public bool isCLOSED = false;

        void MainWindow_Closed(object sender, EventArgs e)
        {
            isCLOSED = true;

            try
            {
                //relays.Reset(); //210419 Add by KYJ
                relays.RelayOff(CMC_Relay_FL);
                relays.RelayOff(CMC_Relay_PORS);
                //221101 wjs add mase m183
                relays.RelayOff(CMC_Relay_Mas_M183);

                AllThreadAbort();
                tlamp.OffTLamp();

                logoutSeq();
                this.LogState(LogType.Success, "User Closed");
            }
            catch (Exception ec)
            {
                this.LogState(LogType.Fail, "User Closed", ec);
            }
            finally
            {
                Thread.Sleep(500);
                System.Environment.Exit(0);
            }


        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (loginUser != null)
            {
                LogoutTime = DateTime.Now;
            }
        }

        // 20190312 jeonhj's comment
        // 윈도우 닫을 때 사용중인 모든 스레드를 닫는다.
        // 스레드는 더 확인해서 닫아야 함.
        // 다른 생성자들도 여기서 Close 할 수 있도록 해야 함.
        private void AllThreadAbort()
        {
            if (autoModeThread != null)
            {
                if (autoModeThread.IsAlive)
                {
                    autoModeThread.Abort();
                    autoModeThread = null;
                }
            }

            if (finishedThread != null)
            {
                if (finishedThread.IsAlive)
                {
                    finishedThread.Abort();
                    finishedThread = null;
                }
            }

            if (GroupBtThread != null)
            {
                if (GroupBtThread.IsAlive)
                {
                    GroupBtThread.Abort();
                    GroupBtThread = null;
                }
            }

            if (SingleBtThread != null)
            {
                if (SingleBtThread.IsAlive)
                {
                    SingleBtThread.Abort();
                    SingleBtThread = null;
                }
            }

            if (logoutTh != null)
            {
                b_isExit = true;

                if (logoutTh.IsAlive)
                {
                    logoutTh.Abort();
                    logoutTh = null;
                }
            }
        }


        bool isAutoScr = true;
        private void autoscrChecked(object sender, RoutedEventArgs e)
        {
            isAutoScr = true;
        }

        private void autoscrUnchecked(object sender, RoutedEventArgs e)
        {
            isAutoScr = false;
        }

        private void bt_save_Click(object sender, RoutedEventArgs e)
        {
            if (CheckPermission(PERMISSION.SAVE_DATA))
            {
                //20. Save Data 클릭 이벤트 발생 시 Module BCR text 저장
                //190104 by grchoi
                MonoFrame = monoTb.Text.ToString();
                SaveData(false);
            }
            else
            {
                PermissionError();
            }

        }

        private void ProgressRefresh(double progVal)
        {

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (progVal == 0)
                    this.testPb.Value = 0;
                else if (progVal == 100)
                    this.testPb.Value = 100;
                else
                    this.testPb.Value += progVal;
            }));

            if (ispause)
            {
                LogState(LogType.Info, "Pause");
            }

            bool isg = false;
            while (ispause)
            {
                isg = true;
              
            }
            if (isg)
            {
                LogState(LogType.Info, "Resume");
            }

        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            if (e != null)
            {
                //var rp = new RequirePasswordWindow(this);
                //rp.ShowDialog();
                if (CheckPermission(PERMISSION.RESET))
                { 
                }
                else
                {
                    //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
                    PermissionError();
                    return;
                }
                //rp.Close();
                LogState(LogType.Info, "Reset Button Clicked");
               
            }
            LogState(LogType.Info, "RESET");
            //_stopwatch.Stop(); 
            //_stopwatch.Reset();

            //200622 변수 초기화 추가
            deviceRetryCount = "";
            deviceStatus = "";

            //200630 변수 추가
            lvRetryCount = "0";

            if (pro_Type == ProgramType.HipotInspector)
            {
                withstandRetry = 0;
                connectionCheckRetryCount1 = 0;
                connectionCheckRetryCount2 = 0;
            }
            else if (pro_Type == ProgramType.Hipot_no_resin_Inspector)
            {
                connectionCheckRetryCount1 = 0;
                connectionCheckRetryCount2 = 0;
            }

            isStop = ispause = isEmg_ = false;
            bt_pass.Content = "-";
            bt_pass.Background = Brushes.DarkGray;
            ProgressRefresh(0);
            //this.time_start_tb.Text = this.time_finish_tb.Text = this.time_elapsed_tb.Text = "";
            
            //bt_save.Background = resetBt.Background;


            foreach (var item in btgrid.Children)
            {
                if (item is Grid)
                {
                    var chitem = (item as Grid).Children;
                    foreach (var item1 in chitem)
                    {
                        if (item1 is Button)
                        {
                            var it = item1 as Button;
                            it.Background = resetBt.Background;
                        }

                        if (item1 is ComboBox)
                        {
                            var ot = item1 as ComboBox;
                            foreach (var ott in ot.Items)
                            {
                                var it = ott as Button;
                                it.Background = resetBt.Background;
                            }
                        }
                    }
                }
            }


            foreach (var item in this.viewModel.TestItemList)
            {
                item.Value.Value_ = null;
                item.Value.Result = "";
                item.Value.refValues_.Clear();
            }

            ClearCellDetailList();

            //CellDetailDg.ItemsSource = cellDetailList;

            //계측기별 버퍼 초기화가 들어갈 자리
            if (keysight != null)
            {
                keysight.Discard();
            }

            if (chroma != null)
            {
                chroma.Discard();
            }

            // 2019 Noach Choi 리셋함수에 충방전기 FF부분 추가 요청(박진호 선임)
            if (cycler != null)
            {
                LogState(LogType.Info, "Send Cycler 0FF");
                cycler.SendToDSP1("0FF", new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            }

            if (pro_Type == ProgramType.EOLInspector || pro_Type == ProgramType.VoltageInspector)
            {
                relays.RelayOn(CMC_Relay);
            }

        }

        /// <summary>
        /// 안씀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InspectionSpecClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            //if (rp.isOK)
            //{
            //    var ins = new InspectionSpecWindow(this);
            //    ins.Show();
            //}
            //else
            //{
            //    MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            //}
            //rp.Close();
        }

        //기본 오토상태
        private void manualBt_Click(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.Auto_Manual))
            {
                bt_pass.Background = Brushes.DarkGray;
                bt_pass.Content = "-";
                isManual = !isManual;
                ResetClick(this, null);

                isMESskip = isMESSkipCb.IsChecked == false ? false : true;

                if (isManual)
                {
                    //isSkipNG.IsEnabled = true;
                    labelA.Background = Brushes.Gray;
                    labelM.Background = Brushes.SkyBlue;

                    openButtonGrid.IsEnabled = resetBt.IsEnabled = specBt.IsEnabled = true;
                    modelSelectGrid.IsEnabled = true;
                    blinder.Visibility = Visibility.Collapsed;

                    //isMESSkipCb.IsChecked = true;
                    LogState(LogType.Info, "---Switched to Manual mode---");
                }
                else
                {
                    isSkipNG.IsChecked = isSkipNG_ = false;
                    //isSkipNG.IsEnabled = false;
                    labelA.Background = Brushes.SkyBlue;
                    labelM.Background = Brushes.Gray;
                    openButtonGrid.IsEnabled = resetBt.IsEnabled = specBt.IsEnabled = false;
                    modelSelectGrid.IsEnabled = false;
                    blinder.Visibility = Visibility.Visible;

                    isMESSkipCb.IsChecked = false;
                    LogState(LogType.Info, "---Switched to Auto mode---");
                }

                tlamp.SetTLampTesting(false);
                tlamp.SetTLampStandBy(true);
            }
            else
            {
                PermissionError();
                //if (!rp.isESC)
                //{

                //    MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
                //}
            }

            //relays.RelayOff("IDO_2");
            //relays.RelayOn("IDO_1");
            //rp.Close();
        }

        private void SaveData(bool isSuccess)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                bt_save.Background = Brushes.Lime;

            }));

            this.Save(isSuccess);
        }

        public void SetMainCState(string volt)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.mainCStateLb1.Content = volt.Replace("_", " ");
            }));
        }

        public void SetMainCurrent(string volt)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.mainCurrentLb1.Content = volt + " A";
            }));
        }

        public void SetMainVoltage(string volt)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.mainVoltageLb1.Content = volt + " V";
            }));
        }

        /// <summary>
        /// UI의 로깅창 더블클릭시 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateLb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListBox).SelectedItem == null)
                return;

            MessageBox.Show((sender as ListBox).SelectedItem.ToString());
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            if (CheckPermission(PERMISSION.EMS))
            {
                LogState(LogType.Info, "EmergencyStop Button Clicked");
                isEmg_ = true;
                StopAuto();
            }
            else
            {
                PermissionError();
            }
            //isEmg_ = false;
        }


        private void PauseClick(object sender, RoutedEventArgs e)
        {
            if (CheckPermission(PERMISSION.Pause))
            {
                LogState(LogType.Info, "Pause Button Clicked");
                ispause_ = !ispause_;
            }
            else
            {
                (sender as System.Windows.Controls.Primitives.ToggleButton).IsChecked = false;
                PermissionError();
            }
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.StartAuto))
            {
                LogState(LogType.Info, "Start Auto Button Started");
                AutoMode();
            }
            else
            {
                PermissionError();
                //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            //rp.Close();
        }

        private void CyclerSettingsClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.CyclerSetting))
            {
                mmw.saveBt.IsEnabled = false;
                mmw.Show();
                mmw.Cbox.SelectedIndex = 0;
            }
            else
            {
                PermissionError();
                //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);

            }
            //rp.Close();
        }

        private void OpenResultClick(object sender, RoutedEventArgs e)
        {
            if (CheckPermission(PERMISSION.OpenRSTFolder))
            {
                System.Diagnostics.Process.Start("explorer.exe", @FILE_PATH_INSPECTION_RESULT);
            }
            else
            {
                PermissionError();
            }
        }

        private void OpenLogClick(object sender, RoutedEventArgs e)
        {
            if (CheckPermission(PERMISSION.OpenLogFolder))
            {
                System.Diagnostics.Process.Start("explorer.exe", @logaddr);
            }
            else
            {
                PermissionError();
            }
        }

        private void StateLb_KeyUp_0(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                StateLb.Items.Clear();
            }
        }

        private void StateLb_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                StateLb1.Items.Clear();
            }
        }

        private void StateLb_KeyUp_2(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                StateLb2.Items.Clear();
            }
        }

        private void StateLb_KeyUp_3(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                StateLb3.Items.Clear();
            }
        }

        #region list double click Event
        public Point pPoint;
        public ListWindow cp;
        public List<doubleValue> tempViewList = new List<doubleValue>();
        public List<doubleValue> cellViewList = new List<doubleValue>();
        private void testItemListDg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (testItemListDg.SelectedIndex == -1)
                return;

            var item = this.viewModel.TestItemList.ToList()[testItemListDg.SelectedIndex];

            switch (item.Key)
            {
                case "Check Delta Temp Sensors(each other) before DCIR":
                    {
                        if (tempViewList.Count == 0)
                            return;

                        if (cp != null && cp.IsLoaded)
                            cp.Close();

                        cp = new ListWindow(this, tempViewList);

                    }; break;
                case "Check Delta Temp Sensors(each other) After DCIR":
                    {
                        if (tempViewList.Count == 0)
                            return;

                        if (cp != null && cp.IsLoaded)
                            cp.Close();

                        cp = new ListWindow(this, tempViewList);

                    }; break;
                case "Cell Delta Voltage Before DCIR":
                    {
                        if (cellViewList.Count == 0)
                            return;

                        if (cp != null && cp.IsLoaded)
                            cp.Close();

                        cp = new ListWindow(this, cellViewList);

                    }; break;
                case "Cell Delta voltage After DCIR":
                    {
                        if (cellViewList.Count == 0)
                            return;

                        if (cp != null && cp.IsLoaded)
                            cp.Close();

                        cp = new ListWindow(this, cellViewList);

                    }; break;
                default: return;
            }

            pPoint = e.GetPosition(this);
            pPoint = this.PointToScreen(pPoint);

            double diff = System.Windows.SystemParameters.WorkArea.Height - pPoint.Y;
            if (pPoint.Y > cp.Height && diff < cp.Height)
                pPoint.Y -= cp.Height;

            diff = System.Windows.SystemParameters.WorkArea.Width - pPoint.X;
            if (diff < cp.Width)
                pPoint.X -= cp.Width;

            cp.Top = pPoint.Y;
            cp.Left = pPoint.X;
            cp.Focus();
            cp.Show();
            cp.Closing -= cp_Closing;
            cp.Closing += cp_Closing;
        }
        void cp_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cp = null;
            MemoryRefresh();
        }
        #endregion

        private void OpenRelayControllerClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.RelayControl))
            {
                RelayControllerWindow rc = new RelayControllerWindow(this);
                rc.ShowDialog();
            }
            else
            {
                PermissionError();
                //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            //rp.Close();
        }

        private void OpenPredClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.PREDICT))
            {
                var prewindow = new PredictWindow(this);
                prewindow.Show();
            }
            else
            {
                PermissionError();
                //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            //rp.Close();
        }

        private void OpenEOLListClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.OpenEOLLIST))
            {
                var fileFolder = "EOL";

                switch (pro_Type)
                {
                    case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                    case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                    case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                    case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                }

                var fileDir = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "eollist.csv";

                FileInfo fi = new FileInfo(fileDir);
                var lastWriteTime = fi.LastWriteTime;

                CSVEditWindow cv = new CSVEditWindow(this, fileDir);
                cv.ShowDialog();

                //var process = new System.Diagnostics.Process() { StartInfo = new System.Diagnostics.ProcessStartInfo(fileDir) };
                //process.Start();
                //process.WaitForExit();
                fi = new FileInfo(fileDir);
                
                if (lastWriteTime != fi.LastWriteTime)                    
                {
                    //파일 수정이 이뤄졌을때 재시작할 것인지 물어본다
                    if (MessageBox.Show("Recognized the file changed.\nDo you want reboot the program?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        DisposeAllModule();
                    }
                } 
            }
            else
            {
                PermissionError();
                //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            //rp.Close();
        }

        private void OpenControlListClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.OpenCtrllist))
            {
                var fileFolder = "EOL";

                switch (pro_Type)
                {
                    case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                    case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                    case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                    case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                }

                var fileDir = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";

                FileInfo fi = new FileInfo(fileDir);
                var lastWriteTime = fi.LastWriteTime;

                CSVEditWindow cv = new CSVEditWindow(this, fileDir);
                cv.ShowDialog();

                //var process = new System.Diagnostics.Process() { StartInfo = new System.Diagnostics.ProcessStartInfo(fileDir) };
                //process.Start();
                //process.WaitForExit();
                fi = new FileInfo(fileDir);

                if (lastWriteTime != fi.LastWriteTime)
                {
                    //파일 수정이 이뤄졌을때 재시작할 것인지 물어본다
                    if (MessageBox.Show("Recognized the file changed.\nDo you want reboot the program?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        DisposeAllModule(); 
                    }
                }
            }
            else
            {
                PermissionError();
                //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            //rp.Close();
        }

        /// <summary>
        /// not used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenMasterBcrClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.MASTER))
            {
                MasterBcrWindow bw = new MasterBcrWindow(this);
                bw.ShowDialog();

                if (!bw.ch)
                {
                    //파일 수정이 이뤄졌을때 재시작할 것인지 물어본다
                    if (MessageBox.Show("Recognized the file changed.\nDo you want reboot the program?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        DisposeAllModule();
                    }
                }
            }
            else
            {
                PermissionError();
                //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            //rp.Close();
        }

        private void OpenBCRListClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.OpenBCRList))
            {                
                var fileDir = staticPath_EOL_SPEC + "\\" + position + "\\Master_BCR.csv";

                FileInfo fi = new FileInfo(fileDir);
                var lastWriteTime = fi.LastWriteTime;

                CSVEditWindow cv = new CSVEditWindow(this, fileDir,
                    "Model Type (1:Audi_N / 2:Audi_M / 3:Porsche_N / 4:Porsche_M / 5:Maserati_N / 6:Maserati_M / 7:E_UP)",
                    "Mode (1:Diagnosis / 2:Master )"
                    );
                cv.ShowDialog();

                //var process = new System.Diagnostics.Process() { StartInfo = new System.Diagnostics.ProcessStartInfo(fileDir) };
                //process.Start();
                //process.WaitForExit();
                fi = new FileInfo(fileDir);

                if (lastWriteTime != fi.LastWriteTime)
                {
                    //파일 수정이 이뤄졌을때 재시작할 것인지 물어본다
                    if (MessageBox.Show("Recognized the file changed.\nDo you want reboot the program?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        DisposeAllModule();
                    }
                }
            }
            else
            {
                PermissionError();
                //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            //rp.Close();
        }


        private void OpenModelListClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            if (CheckPermission(PERMISSION.OpenModelList))
            {
                var fileFolder = "EOL";

                switch (pro_Type)
                {
                    case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                    case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                    case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                    case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                }

                var fileDir = staticPath_EOL_MOD_LIST + "\\" + position + "\\" + fileFolder + "\\" + "modellist.csv";

                FileInfo fi = new FileInfo(fileDir);
                var lastWriteTime = fi.LastWriteTime;

                CSVEditWindow cv = new CSVEditWindow(this, fileDir);
                cv.ShowDialog();

                //var process = new System.Diagnostics.Process() { StartInfo = new System.Diagnostics.ProcessStartInfo(fileDir) };
                //process.Start();
                //process.WaitForExit();
                fi = new FileInfo(fileDir);

                if (lastWriteTime != fi.LastWriteTime)
                {
                    //파일 수정이 이뤄졌을때 재시작할 것인지 물어본다
                    if (MessageBox.Show("Recognized the file changed.\nDo you want reboot the program?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        DisposeAllModule(); 
                    }
                }
            }
            else
            {
                PermissionError();
                //MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            //rp.Close();
        }
        
        #endregion

        #region Logging / Save data
        Object singleObj = new Object();
        Object allDayObj = new Object();
        Object cyclerObj = new Object();
        Object exceptionObj = new Object();
        Object allofallDayObj = new Object();
        System.Collections.Concurrent.ConcurrentQueue<singleLogItem> que = new System.Collections.Concurrent.ConcurrentQueue<singleLogItem>();

        public void LogState(LogType lt, string str, Exception ec = null,
            bool isView = true, bool isSave = true, bool isVol = false, int index = 0, bool isTest = false, string testName = "")
        {
            //System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            //st.Start(); 
            var header = "";

            #region Log Header
            switch (lt)
            {
                case LogType.Info: header = "[I N F O],"; break;
                case LogType.Fail: header = "[F A I L],"; break;
                case LogType.Success: header = "[SUCCESS],"; break;
                case LogType.Pass: header = "[P A S S],"; break;
                case LogType.NG: header = "[N     G],"; break;
                case LogType.TEST: header = "[T E S T],"; break;
                case LogType.EMERGENCY: header = "[EMERGEN],"; break;
                case LogType.RESPONSE: header = "[RESPNSE],"; break;
                case LogType.REQUEST: header = "[REQUEST],"; break;
                case LogType.MANUALCONDITION: header = "[MANCOND],"; break;
                case LogType.PLC_RECV: header = "[PLCRECV],"; break;
                case LogType.PLC_SEND: header = "[PLCSEND],"; break;
                case LogType.CAN: header = ""; isView = false; break;
                case LogType.CMC_RECV: header = "[CMCRECV],"; break;
                case LogType.CMC_SEND: header = "[CMCSEND],"; break;
                case LogType.DEVICE_CHECK: header = "[DEVICE_CHECK],"; break;
                case LogType.CYCLER : header = "";break;
                case LogType.PERMISSION: header = "[PERMISSION],"; break;
            }
            #endregion

            str = str.Replace("\n", "");
            StringBuilder sb = new StringBuilder();
            sb.Append(System.DateTime.Now.ToString("HH:mm:ss:fff,"));
            sb.Append(header);
            sb.Append(str);
            sb.Append(ec != null ? (" - " + ec.Message + " : " + ec.StackTrace + ",") : ",");
            #region ui View
            if (isView)
            {
                //메인 UI에 비동기로 접근시켜서 로그를 새로고침 시킨다.
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //UI에서 5천개이상 데이터가 쌓이면 강제로 비우도록 한다.
                    if (StateLb.Items.Count > 5000)
                        StateLb.Items.Clear();

                    try
                    {
                        StateLb.Items.Insert(0, sb.ToString());
                    }
                    catch (Exception)
                    {
                    }

                    switch (lt)
                    {
                        case LogType.Success:
                            {
                                if (StateLb2.Items.Count > 5000)
                                    StateLb2.Items.Clear();

                                try
                                {
                                    StateLb2.Items.Insert(0, str.ToString());
                                }
                                catch (Exception)
                                {
                                }
                            }; break;
                        case LogType.Fail:
                            {
                                if (StateLb3.Items.Count > 5000)
                                    StateLb3.Items.Clear();

                                try
                                {
                                    StringBuilder sbb = new StringBuilder();
                                    sbb.Append(str);
                                    sbb.Append(ec != null ? (" - " + ec.Message + " : " + ec.StackTrace + ",") : ",");
                                    StateLb3.Items.Insert(0, sbb.ToString());
                                }
                                catch (Exception)
                                {
                                }
                            }; break;
                        default:
                            {
                                if (StateLb1.Items.Count > 5000)
                                    StateLb1.Items.Clear();

                                try
                                {
                                    StateLb1.Items.Insert(0, str.ToString());
                                }
                                catch (Exception)
                                {
                                }
                            }; break;
                    }


                }));
            }
            #endregion
            if (isSave)
            {
                var sl = new singleLogItem() { lt = lt, Data = sb.ToString() + "\r\n" };
                que.Enqueue(sl);
            }
            //Console.WriteLine(st.ElapsedMilliseconds.ToString());
            //st.Stop(); 
        }


        private void DequeueLogger()
        {
            while (true)
            {
                singleLogItem logData;
                if (que.TryDequeue(out logData))
                {

                    StringBuilder load = new StringBuilder();
                    load.Append(@logaddr);
                    load.Append("\\");
                    load.Append(this.MODEL_NAME);
                    load.Append("\\");
                    load.Append(this.viewModel.ModelId);
                    load.Append("\\");
                    load.Append(System.DateTime.Now.ToString("yyyyMMdd"));

                    if (!Directory.Exists(load.ToString()))
                    {
                        Directory.CreateDirectory(load.ToString());
                    }

                    var tBarcode = "NO_BCR";

                    if (Barcode != "")
                    {
                        tBarcode = Barcode.Replace(":", "_").ToString().Replace(";", "_").ToString().Replace("\r\n", "").ToString();
                    }

                    try
                    {
                        if (logData.lt == LogType.CYCLER)
                        {
                            if (Directory.Exists(load.ToString()))
                            {
                                lock (cyclerObj)
                                {
                                    load.Append("\\");
                                    load.Append(position);
                                    load.Append("\\");
                                    load.Append(tBarcode);
                                    load.Append("_");
                                    load.Append(System.DateTime.Now.ToString("HH"));
                                    load.Append("_Cycler.txt");

                                    File.AppendAllText(load.ToString(), logData.Data, Encoding.UTF8);
                                }
                            }
                        }
                        else
                        {
                            if (Directory.Exists(load.ToString()))
                            {
                                lock (allofallDayObj)
                                {
                                    File.AppendAllText(load.ToString() + "\\All_STATION_LOG.txt", All_of_All_Header + "," + logData.Data, Encoding.UTF8);
                                }
                            }

                            load.Append("\\");
                            load.Append(position);

                            if (!Directory.Exists(load.ToString()))
                            {
                                Directory.CreateDirectory(load.ToString());
                            }

                            if (Directory.Exists(load.ToString()))
                            {
                                lock (allDayObj)
                                {
                                    File.AppendAllText(load.ToString() + "\\AllDay_" + System.DateTime.Now.ToString("yyyyMMdd") + ".txt", logData.Data, Encoding.UTF8);
                                }

                                lock (singleObj)
                                {
                                    load.Append("\\");
                                    load.Append(tBarcode);
                                    load.Append("_");
                                    load.Append(System.DateTime.Now.ToString("HH"));
                                    load.Append(".txt");
                                    File.AppendAllText(load.ToString(), logData.Data, Encoding.UTF8);
                                }
                            }
                            else
                            {
                                Directory.CreateDirectory(load.ToString());
                            }
                        }
                    }
                    catch (Exception ecs)
                    {
                        lock (exceptionObj)
                        {
                            try
                            {
                                tBarcode = "NOT_POSSIBLE_FILE_NAME";
                                File.AppendAllText(load + "\\" + tBarcode + "_" + System.DateTime.Now.ToString("HH") + ".txt", logData.Data, Encoding.UTF8);
                            }
                            catch (Exception eccc)
                            {
                            }
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 23.  Save() 함수 요청반영사항
        ///      -> 상세수집항목 저장 정렬구문 추가(헤더/내용 모두)
        ///      -> 저장항목 define(LOT id, Module BCR, Device STAT 등 코드 헤더 및 저장내용 참조)
        ///      -> 바코드 항목 예외구문 추가
        ///      -> 착공 NG 시 NG 처리구문 추가
        /// </summary>
        /// <param name="saveCase">-1: MES NOT CONNECTED / 0: normal / 1: 제어항목NG / 2: 수집항목NG / 3: 착공NG</param>        
        private void Save(bool isSuccess)
        {
            string @LogDirectory = @FILE_PATH_INSPECTION_RESULT + MODEL_NAME + "\\" + this.viewModel.ModelId;           

            if (!Directory.Exists(@LogDirectory))
            {
                Directory.CreateDirectory(@LogDirectory);
            }

            //190104 by grchoi
            List<DetailItems> dlist = new List<DetailItems>();  //20181224 wjs detail order
            foreach (var item in this.viewModel.TestItemList)
            {
                //상세수집 데이터 모으기
                if (item.Value.refValues_.Count != 0)
                {
                    foreach (var ritem in item.Value.refValues_) dlist.Add(ritem as DetailItems);
                }
            }
            var orderedList = dlist.OrderBy(x => x.order);

            var dir = @LogDirectory + "\\" + System.DateTime.Now.ToString("[yyyy-MM-dd]EOL_RESULT") + ".txt";
            var subdir = @LogDirectory + "\\" + System.DateTime.Now.ToString("[yyyy-MM-dd]EOL_RESULT_FILE_OPENED") + ".txt";

            // 파일이 없다면 최초에 컬럼을 박아넣어야함.
            if (!File.Exists(dir))
            {
                //190106 by grchoi
                // 2020.01.22 KSM : 결과파일에 재시도 헤더 추가
                // 2020.02.26 Noah Choi 헤더 명칭 수정 RETRY_COUNT -> Machine_CHECK
                var header = "PASS FAIL,STATION,LOT_ID,MODULE_BCR,IS_MES_SKIP,MACHINE_CHECK,DEVICE_STAT,LV_RETRY_CNT,";
                if (pro_Type == ProgramType.HipotInspector)
                {
                    //5. Hipot) Contact Check Retry Logic
                    //-> Contact Check Retry 했을경우 Result File에 남길수 있도록
                    header += "WITH_RETRY,CONT_RETRY_1,CONT_RETRY_2,";
                }
                else if(pro_Type == ProgramType.Hipot_no_resin_Inspector)
                {
                    header += "CONT_RETRY_1,CONT_RETRY_2,";
                }

                foreach (var item in this.viewModel.TestItemList)
                {
                    header += item.Key + ",";
                }

                //190104 by grchoi
                foreach (var item in orderedList) header += (item as DetailItems).Key + ",";

                header += "Started Time,Elapsed Time,Finished Time";
                File.AppendAllText(dir, header + "\r\n", Encoding.UTF8);
            }

            var first = "PASS";

            if (!isSuccess)
                first = "NG";

            //foreach (var item in this.viewModel.TestItemList)
            //{
            //    //190104 by grchoi
            //    if (item.Key.Replace(" ", "") == _BARCODE) continue;
            //    if (item.Value.Result != null && item.Value.Result != "PASS") first = "NG";
            //}

            var tail = first + ",";

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0},", position));
            sb.Append(Barcode);
            sb.Append(",");

            //190104 by grchoi
            sb.Append(string.Format("{0},", MonoFrame));

            //191128 initialize field
            MonoFrame = "";
            this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.monoTb.Text = "";
                }));

            sb.Append(string.Format("{0},", isMESskip == true ? "YES" : "NO"));

            // 2020.01.22 KSM : 재시도 횟수 결과 파일에 추가
            if (deviceRetryCount == "") sb.Append(string.Format("{0},", "NONE"));
            else sb.Append(string.Format("{0},", deviceRetryCount));

            if (deviceStatus == "") sb.Append(string.Format("{0},", "OK"));
            else sb.Append(string.Format("{0},", deviceStatus));
            
            sb.Append(string.Format("{0},", lvRetryCount));

            if (pro_Type == ProgramType.HipotInspector)
            {
                sb.Append(string.Format("{0},", withstandRetry.ToString()));
                sb.Append(string.Format("{0},", connectionCheckRetryCount1.ToString()));
                sb.Append(string.Format("{0},", connectionCheckRetryCount2.ToString()));
            }
            else if (pro_Type == ProgramType.Hipot_no_resin_Inspector)
            {
                sb.Append(string.Format("{0},", connectionCheckRetryCount1.ToString()));
                sb.Append(string.Format("{0},", connectionCheckRetryCount2.ToString()));
            }

            //sb.Append(position);
            //sb.Append(",");

            foreach (var item in this.viewModel.TestItemList)
            {
                sb.Append(string.Format("{0}", item.Value.Value_));
                sb.Append(",");
            }

            //190104 by grchoi
            foreach (var ritem in orderedList)
            {
                var dti = ritem as DetailItems;

                for (int i = 0; i < dti.Reportitems.Count; i++)
                {
                    sb.Append(string.Format("{0}", dti.Reportitems[i].ToString()));
                    if (dti.Reportitems.Count != 1) sb.Append("&");
                }
                if (dti.Reportitems.Count != 1) sb.Remove(sb.Length - 1, 1);
                sb.Append(",");
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                sb.Append(this.time_start_tb.Text);
                sb.Append(",");
                sb.Append(this.time_elapsed_tb.Text);
                sb.Append(",");
                sb.Append(this.time_finish_tb.Text);
            }));

            //190104 by grchoi
            try { File.AppendAllText(dir, tail + sb.ToString() + "\r\n", Encoding.UTF8); }
            catch (Exception)
            {
                this.LogState(LogType.Fail, "Save Data - File is open");
                File.AppendAllText(subdir, tail + sb.ToString() + "\r\n", Encoding.UTF8);
            }

            this.LogState(LogType.Info, "Saved Data");

            MemoryRefresh();
            //200730 기본동작은 이제 lotid가 비어야 하는데, 비우는 로직을 넣어야 한다면
            //여기를 사용
            //this.Dispatcher.Invoke(new Action(() =>
            //{
            //    this.viewModel.LotId = Barcode = this.lotTb.Text = "";
            //    MonoFrame = this.monoTb.Text = "";
            //}));
        }

        #endregion

        /// <summary>
        /// 항목이 진행중임을 표시하는 메서드.
        /// 일시정지일때, 해당부분에 일시정지 무한루프로 걸린다.
        /// </summary>
        /// <param name="ti"></param>
        public void isProcessingUI(TestItem ti)
        {
            ti.refValues_.Clear();

            if (pushedEmo)
            {
                LogState(LogType.Info, "Pushed emergency switch.");
            }

            //24. 수동검사로그저장을위해 해당함수에서 lotTb의 스트링 저장구문 추가
            //190104 by grchoi
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.viewModel.LotId = Barcode = this.lotTb.Text;
            }));

            if (ispause)
            {
                LogState(LogType.Info, "Pause");
            }

            bool isg = false;
            while (ispause)
            {
                isg = true;
            }
            if (isg)
            {
                LogState(LogType.Info, "Resume");
            }

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (isManual)
                {
                    resetBt.IsEnabled = 
                    openButtonGrid.IsEnabled = false;
                    blinder3.Visibility = blinder2.Visibility = blinder.Visibility = System.Windows.Visibility.Visible;
                }
            }));

            isProcessingFlag = true;
            AutoScrolling(ti);
            //relays.Reset();

            LogState(LogType.Info, "-----------------------------------------------------------------", null, false);
            LogState(LogType.Info, "Test :" + ti.Name + " Start");

            //ti.Result = "RUN";
            SetYellow(ti.Bt);

            //relays.RelayOff("IDO_1");
            //relays.RelayOn("IDO_2");
            tlamp.SetTLampStandBy(false);
            tlamp.SetTLampTesting(true);
        }

        private void AutoScrolling(TestItem testItem)
        {
            if (!isAutoScr)
                return;

            var index = GetIndex(this.viewModel.TestItemList, testItem.Name);
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                testItemListDg.SelectedIndex = index;
                testItemListDg.ScrollIntoView(testItemListDg.SelectedItem);
            }));
        }

        public int GetIndex(Dictionary<string, TestItem> dictionary, string key)
        {
            for (int index = 0; index < dictionary.Count; index++)
            {
                if (dictionary.Skip(index).First().Value.Name == key)
                    return index; // We found the item
            }

            return -1;
        }

        #region UI 변경 메서드

        private void SetYellow(Button bt)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                bt.Background = Brushes.Yellow;
            }));
        }

        bool isYellow(Button bt)
        {
            bool rtv = false;
            this.Dispatcher.Invoke(new Action(() =>
            {

                if (bt.Background == Brushes.Yellow)
                    rtv = true;
                else
                    rtv = false;
            }));
            return rtv;
        }

        bool isProcessingFlag = false;

        private bool SetCtrlToPass(bool isPass, TestItem item, Button bt = null)
        {
            //relays.RelayOff("IDO_2");
            //relays.RelayOn("IDO_1");

            LogState(LogType.Info, "-----------------------------------------------------------------", null, false);
            bool rtv = false;
            if (isPass)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (bt != null)
                        bt.Background = Brushes.Lime;
                    else
                        item.Bt.Background = Brushes.Lime;
                }));
                rtv = true;
            }
            else
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (bt != null)
                        bt.Background = Brushes.Red;
                    else
                        item.Bt.Background = Brushes.Red;
                }));
            }

            if (isStop)
                return false;

            return rtv;
        }

        /// <summary>
        /// isStop flag 내장
        /// </summary>
        /// <param name="isPass"></param>
        /// <param name="btName"></param>
        /// <returns></returns>
        private bool SetCtrlToPass(bool isPass, string btName)
        {
            //relays.RelayOff("IDO_2");
            //relays.RelayOn("IDO_1");

            LogState(LogType.Info, "-----------------------------------------------------------------", null, false);
            bool rtv = false;
            if (isPass)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var bt = (Button)this.FindName(btName);
                    bt.Background = Brushes.Lime;
                }));
                rtv = true;
            }
            else
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var bt = (Button)this.FindName(btName);
                    bt.Background = Brushes.Red;
                }));
            }

            if (isStop)
                return false;

            return rtv;
        }

        private void Pass(bool isPass)
        {
            if (isPass)
            {
                bt_pass.Content = "PASS";
                bt_pass.Background = Brushes.Lime;
            }
            else
            {
                bt_pass.Content = "NG";
                bt_pass.Background = Brushes.Red;
            }
        }

        #endregion

        /// <summary>
        /// 강제 정지 메서드
        /// 일시정지 상태를 풀며, NG로 빠지게 한다.
        /// </summary>
        public void StopAuto()
        {
            LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");
            ispause = false;
            isStop = true;
            //isEmg_ = false;

            timeThreadFlag = false; 
        }

        public bool isEmg_ = false;
        public bool IsEmg
        {
            get { return isEmg_; }
        }

        private bool ispause_ = false;
        ContinueWindow pcw;
        public bool ispause
        {
            get { return ispause_; }
            set
            {

                ispause_ = value;
                if (ispause_)
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        pausebt.IsChecked = true;
                        pcw = new ContinueWindow(this);
                        pcw.maintitle.Content = "PAUSE";
                        pcw.reason.Content = "Equipment is paused.\nPress the 'Resume' button to resume.";
                         
                        pcw.Show();
                    }));
                }
                else
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        pausebt.IsChecked = false;
                        if (pcw != null)
                        {
                            if (plc.plc_Pause_Continue)
                            {
                                LogState(LogType.Info, "Close pause window. plc pause continue bit on.");
                                pcw.Close();
                            }
                            if (pcw.isContinue)
                            {
                                LogState(LogType.Info, "Close pause window. pause button click.");
                                pcw.Close();
                            }
                        }
                    }));
                }

            }
        }
        

        /// <summary>
        /// 사용안함(유지)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void isdummy(object sender, RoutedEventArgs e)
        {
            var rp = new RequirePasswordWindow(this);
            rp.ShowDialog();
            if (rp.isOK)
            {
                isDummy = true;
            }
            else
            {
                isDummyCb.IsChecked = false;
                e.Handled = true;
                isDummy = false;

                MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void isNotdummy(object sender, RoutedEventArgs e)
        {
            isDummy = false;
        }

        bool isDummy = false;

        public bool isJigChange_ = false;
        public bool isJigChange
        {
            get { return isJigChange_; }
            set { isJigChange_ = value; }
        }

        private void CellDetailDg_LayoutUpdated_1(object sender, EventArgs e)
        {
            //if (localTypes == 0 || localTypes == 1)
            //{
            //    CellDetailDg.Columns[9].Visibility = Visibility.Visible;
            //    CellDetailDg.Columns[10].Visibility =
            //        CellDetailDg.Columns[11].Visibility = Visibility.Collapsed;
            //}
            //else if (localTypes == 2)
            //{
            //    CellDetailDg.Columns[9].Visibility =
            //    CellDetailDg.Columns[10].Visibility =
            //        CellDetailDg.Columns[11].Visibility = Visibility.Collapsed;
            //}
            //else if (localTypes == 3)
            //{
            //    CellDetailDg.Columns[9].Visibility = Visibility.Visible;
            //    CellDetailDg.Columns[10].Visibility =
            //        CellDetailDg.Columns[11].Visibility = Visibility.Collapsed;
            //}
            //else if (localTypes == 4 || localTypes == 5)
            //{
            //    CellDetailDg.Columns[9].Visibility =
            //    CellDetailDg.Columns[10].Visibility =
            //        CellDetailDg.Columns[11].Visibility = Visibility.Visible;
            //}
        }

        private void DaqSpecClick(object sender, RoutedEventArgs e)
        {
            if (CheckPermission(PERMISSION.ShowDAQ))
            {
                var ins = new DAQWindow(this);
                ins.Show();
            }
            else
            {
                PermissionError(); 
            }
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            //221117
            //if (lg == null)
            //    lg = new LoginWindow(this);

            //lg.ShowDialog();
            if (MessageBox.Show("Do you have permission? \nczy masz pozwolenie? \nУ вас є дозвіл? \n ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
            {
                PermissionResult = true;
            }
            else
            {
                PermissionResult = false;
            }
        }

        //210419 Add by KYJ
        static AutoResetEvent autoEvent = new AutoResetEvent(false);

        public void Sleep_Event(int MilSec)
        {
            var th = new Thread(() =>
            {
                Thread.Sleep(MilSec);
                autoEvent.Set();
            });
            th.Start();

            autoEvent.WaitOne(); ;
        }

        //20221116
        //소모품 카운트 기능 nnkim
        public void ShowInspectionUIChange(int nIndex)
        {



            switch (nIndex)
            {
                case (int)ProgramType.HipotInspector:

                    this.mainCStateLb1.Visibility = System.Windows.Visibility.Hidden;
                    this.mainVoltageLb1.Visibility = System.Windows.Visibility.Hidden;
                    this.mainCurrentLb1.Visibility = System.Windows.Visibility.Hidden;

                    this.lbLvconnector.Visibility = System.Windows.Visibility.Hidden;
                    this.lbCurrentProbe.Visibility = System.Windows.Visibility.Hidden;
                    this.lbMc1.Visibility = System.Windows.Visibility.Hidden;
                    this.lbMc2.Visibility = System.Windows.Visibility.Hidden;
                    this.lbMc3.Visibility = System.Windows.Visibility.Hidden;
                    this.btnDcirOpenCntSetting.Visibility = System.Windows.Visibility.Hidden;

                    break;
                case (int)ProgramType.EOLInspector:
                    this.btnOpenHIPOTCntSetting.Visibility = System.Windows.Visibility.Hidden;
                    this.btnPortUI.Visibility = System.Windows.Visibility.Hidden;

                    break;
                default:
                    break;
            }
        }
        //소모품 카운트 기능 nnkim
        private void btnOpenCntSetting(object sender, RoutedEventArgs e)
        {
            int nModeCheck = (int)blinder.Visibility;

            //메뉴얼 모드 일 경우에만 
            if (nModeCheck == 2)
            {
                if ((int)this.pro_Type == (int)ProgramType.EOLInspector)
                {
                    //221012 스페어파츠카운트 설정하는 부분
                    FormPartsCountSetting newFormPartsCountSetting = new FormPartsCountSetting(this);
                    newFormPartsCountSetting.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    if (position == "#1") { newFormPartsCountSetting.Location = new System.Drawing.Point(700, 200); }
                    else { newFormPartsCountSetting.Location = new System.Drawing.Point(50, 200); }

                    newFormPartsCountSetting.ShowDialog();
                }
                else if ((int)this.pro_Type == (int)ProgramType.HipotInspector)
                {
                    //221020 스페어파츠카운트 설정하는 부분
                    FormPartsCountHipotSetting newFormPartsCountHipotSetting = new FormPartsCountHipotSetting(this);
                    newFormPartsCountHipotSetting.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    if (position == "#1") { newFormPartsCountHipotSetting.Location = new System.Drawing.Point(50, 100); }
                    else if (position == "#2") { newFormPartsCountHipotSetting.Location = new System.Drawing.Point(700, 100); }
                    else if (position == "#3") { newFormPartsCountHipotSetting.Location = new System.Drawing.Point(50, 600); }
                    else if (position == "#4") { newFormPartsCountHipotSetting.Location = new System.Drawing.Point(700, 600); }
                    else { }

                    newFormPartsCountHipotSetting.ShowDialog();
                }
                else
                {
                }
            }
            else
            {

            }
        }

        //소모품 카운트 기능 nnkim
        public void SetPartsCountData(TestItem ti = null, string strRun = null)
        {
            if ((int)this.pro_Type == (int)ProgramType.EOLInspector)
            {
                FormPartsCountSetting PCS = new FormPartsCountSetting(this, "START");
                try
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        //1. LV Connector
                        lbLvconnector.Content = string.Format("LV Connector {0} / {1}", PCS.nRealData_ResultLc, PCS.nLimitData_ResultLc);
                        if (PCS.nRealData_ResultLc > PCS.nLimitData_ResultLc) lbLvconnector.Background = Brushes.Red;
                        else lbLvconnector.Background = Brushes.Green;

                        //2.Current Probe
                        lbCurrentProbe.Content = string.Format("Current Probe {0} / {1}", PCS.nRealData_ResultCp, PCS.nLimitData_ResultCp);
                        if (PCS.nRealData_ResultCp > PCS.nLimitData_ResultCp) lbCurrentProbe.Background = Brushes.Red;
                        else lbCurrentProbe.Background = Brushes.Green;

                        //4. MC 1
                        lbMc1.Content = string.Format("MC 1 {0} / {1}", PCS.nRealData_ResultMc1, PCS.nLimitData_ResultMc1);
                        if (PCS.nRealData_ResultMc1 > PCS.nLimitData_ResultMc1) lbMc1.Background = Brushes.Red;
                        else lbMc1.Background = Brushes.Green;

                        //5. MC 2
                        lbMc2.Content = string.Format("MC 2 {0} / {1}", PCS.nRealData_ResultMc2, PCS.nLimitData_ResultMc2);
                        if (PCS.nRealData_ResultMc2 > PCS.nLimitData_ResultMc2) lbMc2.Background = Brushes.Red;
                        else lbMc2.Background = Brushes.Green;

                        //6. MC 3
                        lbMc3.Content = string.Format("MC 3 {0} / {1}", PCS.nRealData_ResultMc3, PCS.nLimitData_ResultMc3);
                        if (PCS.nRealData_ResultMc3 > PCS.nLimitData_ResultMc3) lbMc3.Background = Brushes.Red;
                        else lbMc3.Background = Brushes.Green;
                    }));

                    if (strRun == "StartRun")
                    {
                        ti.refValues_.Add(MakeDetailItem("LVCONNECTORCOUNT", PCS.nRealData_ResultLc.ToString()));
                        ti.refValues_.Add(MakeDetailItem("CURRENTPROBECOUNT", PCS.nRealData_ResultCp.ToString()));
                    }
                }
                catch
                {
                }
            }
            else if ((int)this.pro_Type == (int)ProgramType.HipotInspector)
            {
                FormPartsCountHipotSetting PCHS = new FormPartsCountHipotSetting(this, "START");

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    //1.Current Probe
                    btnOpenHIPOTCntSetting.Content = string.Format("HI-POT PIN {0} / {1}", PCHS.nRealData_ResultHp, PCHS.nLimitData_ResultHp);
                    if (PCHS.nRealData_ResultHp > PCHS.nLimitData_ResultHp) btnOpenHIPOTCntSetting.Foreground = Brushes.Red;
                    else btnOpenHIPOTCntSetting.Foreground = Brushes.Green;
                }));

                if (strRun == "StartRun")
                {
                    ti.refValues_.Add(MakeDetailItem("HIPOTPINCOUNT", PCHS.nRealData_ResultHp.ToString()));
                }
            }
            else
            {

            }

        }

        //파일 삭제 기능 nnkim 
        private static void deleteFolder(string folderDir)
        {
            try
            {
                int deleteDay = 730;

                DirectoryInfo di = new DirectoryInfo(folderDir);

                if (di.Exists)
                {

                    DirectoryInfo[] dirInfo = di.GetDirectories();
                    string lDate = DateTime.Today.AddDays(-deleteDay).ToString("yyyyMMdd");

                    foreach (DirectoryInfo dir in dirInfo)
                    {
                        if (lDate.CompareTo(dir.LastWriteTime.ToString("yyyyMMdd")) > 0)
                        {
                            // 폴더 속성이 읽기, 쓰기 설정에 따라 삭제가 안될 수 있음
                            // 때문에 미리 속성 Normal로 설정
                            dir.Attributes = FileAttributes.Normal;
                            dir.Delete(true);
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        //20221030 언어 인터락
        public void WindowLanguageFormatCheck()
        {
            CultureInfo currentData = CultureInfo.CurrentCulture;
            string strCurrentData = currentData.Name;

            if (strCurrentData != "en-US")
            {
                //System.Windows.MessageBox.Show("Please follow along and check. \nCMD -> Intl.cpl -> Please change the format to US.");
                //System.Environment.Exit(0);
            }
        }

        private void OpenPortUI(object sender, RoutedEventArgs e)
        {
            //221030 
            FormPortNumber newFormPortNumber = new FormPortNumber(this);

            if (position == "#1") { newFormPortNumber.Location = new System.Drawing.Point(50, 100); }
            else if (position == "#2") { newFormPortNumber.Location = new System.Drawing.Point(700, 100); }
            else if (position == "#3") { newFormPortNumber.Location = new System.Drawing.Point(50, 1600); }
            else if (position == "#4") { newFormPortNumber.Location = new System.Drawing.Point(700, 600); }
            else { }

            newFormPortNumber.Show();
        }
    }
}
