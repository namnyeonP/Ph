
using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using Microsoft.Win32;
using MiniScheduler;
using Peak.Can.Basic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using EOL_BASE.Forms;
using System.Globalization;
using System.Windows.Threading;

namespace EOL_BASE
{

    #region Enum

    public enum DSPType
    {
        DSP_28335=0,
        DSP_28377=1,
        DSP_28377_SDI=2
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
    }


    public enum CMD
    {
        INPUT_MC_ON,
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
        CHANNEL1,
        CHANNEL2
    }

    public enum DataType
    {
        strStringType,
        nIntType,
        dDoubleType,
        NULL
    }


    #endregion

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

        // Emergency Timer
        System.Timers.Timer emo_tmr;
        bool pushedEmo = false;
        RegistryKey rkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\EOL_Trigger\\Relays");

        /// <summary>
        /// 각종 윈도우 및 모드 변경시 필요 비번
        /// </summary>
        public string passWord = "1212";

        string logaddr = @"C:\Users\Public\EOL_INSPECTION_LOG";
        static string FILE_PATH_INSPECTION_RESULT = "c:\\Logs\\Inspection_result\\";

        /// <summary>
        /// 자동시작시에 흐르는 시간변수
        /// </summary>
        DateTime elapsedtime, nowTime;

        System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

        public Thread autoModeThread;
        public List<Process> totalProcessList = new List<Process>();

        /// <summary>
        /// 충방전을 위한 스케줄러 윈도우
        /// </summary>
        MiniScheduler.MainWindow mmw;

        public List<Models> modelList = new List<Models>();
        public string Barcode = "";
        public int selectedIndex = -1;

        public bool isNeedDCIR = true;
        public bool isMESskip = false;

        public bool isStop = false;
        public bool isManual = false;

        public bool isLineFlag = false;
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

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        Dictionary<string, List<TestItem>> groupDic = null;//;new Dictionary<string, List<TestItem>>();

        public CChroma chroma;
        public CCycler cycler;
        public CNhtRS232_Receiver temps;
        public CRelay_Receiver relays;
        public CLambdaPower becm_power;
        public CLambdaPower pump_power;
        private CMES MES;
        public CKeysightDMM keysight;
        public CDAQ daq;
        public CPLC plc;
        public CBMS_CAN Hybrid_Instru_CAN;
        public CDAQ970 daq970_1; // Voltage DMM
        public CDAQ970 daq970_2; // Contact Check DMM
        public CDAQ970 daq970_3; // Insulation Voltage Check DMM

        System.Windows.Forms.Timer timetick = new System.Windows.Forms.Timer();

        public int counter_Cycler = 0;
        public int counter_Cycler_limit = 50000;

        //public string mesIP = "10.32.169.81";
        //public string mesPort = "619";
        public string mesIP = "10.47.66.212";
        public string mesPort = "627";
        public string dmmIP = "169.254.4.61";
        public string dmmPort = "5025";

        public string keysight_PortName = "COM2";
        public string daq_PortName = "COM3";
        public string chroma_PortName = "COM2";
        public string lineFlag = "0";
        public string daq970_IP_First = "169.254.9.70";
        public string daq970_Port_First = "5024";
        public string daq970_IP_Second = "169.254.9.71";
        public string daq970_Port_Second = "5024";
        public string daq970_IP_Third = "169.254.9.72";
        public string daq970_Port_Third = "5024";

        public string power_PortName1 = "COM9";
        public string power_PortName2 = "COM8";
        public string nht_PortName = "COM3";

        public string can_bms = "PCAN_PCI: 1 (41h)";
        public string can_cycler1 = "PCAN_USB: 1 (51h)";

        public int INSULATION_RESISTANCE_VOLT = 0;
        public int INSULATION_RESISTANCE_TIME = 0;
        public string INSULATION_RESISTANCE_RANGE = "";
        public int INSULATION_RESISTANCE_RAMP_TIME = 0;

        public int CONTACT_CHECK_UPPER_LIMIT = 0;
        public int FETCH_CHECK_CYCLE_TIME = 3;

        public double FETCH_STANDARD_MIN_VALUE = 0;
        public double FETCH_STANDARD_MAX_VALUE = 0;

        public int LINE_OPEN_CHECK_UPPER_LIMIT = 0;
        public int LINE_OPEN_CHECK_LOWER_LIMIT = 0;

        public string PLC_RECV_ADDRESS = "BA00";
        public string PLC_SEND_ADDRESS = "BA10";
        public string PLC_BCRS_ADDRESS = "WA0";
        public int PLC_LOGICAL_NUMBER = 0;

        //not used
        public string omega_PortName = "COM11";
        public string can_cycler2 = "PCAN_USB:FD 8 (58h)";
        public string can_bms2 = "PCAN_PCI: 2 (42h)";

        //190104 by grchoi
        string deviceStatus = "";
        string deviceRetryCount = "";
        public string strlvRetryCount = "";


        public string masterTempLot = "";

        //200820 WITHSTAND NG FLAG, IR NG FLAG
        bool m_bIsWithstandNG = false;
        bool m_bIsIRNG = false;
        //////////////////////////////////////

        //착공 OK, NG FLAG AND FINISH FLAG
        public bool m_bStartJobResult = false;
        public bool m_bIsStartJobFinish = false;
        //////////////////////////////////////

        //221201
        public bool PermissionResult = false;

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

        public void SetCounter_Fetch_Cycle_Time()
        {
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            rk.SetValue("Fetch_Check_Cycle_Time", FETCH_CHECK_CYCLE_TIME.ToString());
        }



        public void SetLocalData()
        {
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
            int cnt = 1;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_EQUIP_ID", equipID); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_4P8S_PRODUCT_ID", prodID_4P8S); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_4P8S_Rev_PRODUCT_ID", prodID_4P8S_Rev); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_4P7S_PRODUCT_ID", prodID_4P7S); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_3P8S_PRODUCT_ID", prodID_3P8S); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_3P10S_PRODUCT_ID", prodID_3P10S); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_3P10S_Rev_PRODUCT_ID", prodID_3P10S_Rev); cnt++;

            rk.SetValue(position + "_" + cnt.ToString("D2") + "_PROCESS_ID", procID); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_MES_IP", mesIP); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_MES_PORT", mesPort); cnt++;
            //rk.SetValue(position + "_" + cnt.ToString("D2") + "_DMM_IP", dmmIP); cnt++;
            //rk.SetValue(position + "_" + cnt.ToString("D2") + "_DMM_PORT", dmmPort); cnt++;
            //rk.SetValue(position + "_" + cnt.ToString("D2") + "_POWER1_PORT", power_PortName1); cnt++;
            //rk.SetValue(position + "_" + cnt.ToString("D2") + "_POWER2_PORT", power_PortName2); cnt++;
            //rk.SetValue(position + "_" + cnt.ToString("D2") + "_TEMP_PORT", nht_PortName); cnt++;

            rk.SetValue(position + "_" + cnt.ToString("D2") + "_DAQ_PORT", daq_PortName); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_HIPOT_PORT", chroma_PortName); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_DMM_PORT", keysight_PortName); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_CAN_CYCLER", can_cycler1); cnt++;

            rk.SetValue(position + "_" + cnt.ToString("D2") + "_PLC_RECV_ADDRESS", PLC_RECV_ADDRESS); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_PLC_SEND_ADDRESS", PLC_SEND_ADDRESS); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_PLC_BCRS_ADDRESS", PLC_BCRS_ADDRESS); cnt++;
            rk.SetValue(position + "_" + cnt.ToString("D2") + "_PLC_LOGICAL_NUMBER", PLC_LOGICAL_NUMBER.ToString()); cnt++;

            //추가 레지스터가 필요하다면, 해당 부분에 추가
            rk.SetValue("CYCLER_USING", "0"); cnt++;
            rk.SetValue("Fetch_Check_Cycle_Time", FETCH_CHECK_CYCLE_TIME.ToString());
            //rk.SetValue("CEI_PATH_OFFSET", "0.2");
        }

        public void LoadLocalData()
        {
            try
            {
                string regSubkey = "Software\\EOL_Trigger";
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
                this.Title = this.Title + " - " + MODEL_NAME + " (" + lastUpdated + ")";

                int cnt = 1;
                equipID = rk.GetValue(position + "_" + cnt.ToString("D2") + "_EQUIP_ID") as string; cnt++;
                prodID_4P8S = rk.GetValue(position + "_" + cnt.ToString("D2") + "_4P8S_PRODUCT_ID") as string; cnt++;
                prodID_4P8S_Rev = rk.GetValue(position + "_" + cnt.ToString("D2") + "_4P8S_Rev_PRODUCT_ID") as string; cnt++;
                prodID_4P7S = rk.GetValue(position + "_" + cnt.ToString("D2") + "_4P7S_PRODUCT_ID") as string; cnt++;
                prodID_3P8S = rk.GetValue(position + "_" + cnt.ToString("D2") + "_3P8S_PRODUCT_ID") as string; cnt++;
                prodID_3P10S = rk.GetValue(position + "_" + cnt.ToString("D2") + "_3P10S_PRODUCT_ID") as string; cnt++;
                prodID_3P10S_Rev = rk.GetValue(position + "_" + cnt.ToString("D2") + "_3P10S_Rev_PRODUCT_ID") as string; cnt++;
                procID = rk.GetValue(position + "_" + cnt.ToString("D2") + "_PROCESS_ID") as string; cnt++;
                mesIP = rk.GetValue(position + "_" + cnt.ToString("D2") + "_MES_IP") as string; cnt++;
                mesPort = rk.GetValue(position + "_" + cnt.ToString("D2") + "_MES_PORT") as string; cnt++;
                daq_PortName = rk.GetValue(position + "_" + cnt.ToString("D2") + "_DAQ_PORT") as string; cnt++;
                chroma_PortName = rk.GetValue(position + "_" + cnt.ToString("D2") + "_HIPOT_PORT") as string; cnt++;
                keysight_PortName = rk.GetValue(position + "_" + cnt.ToString("D2") + "_DMM_PORT") as string; cnt++;

                //dmmIP = rk.GetValue(position + "_" + cnt.ToString("D2") + "_DMM_IP") as string; cnt++;
                //dmmPort = rk.GetValue(position + "_" + cnt.ToString("D2") + "_DMM_PORT") as string; cnt++;
                //power_PortName1 = rk.GetValue(position + "_" + cnt.ToString("D2") + "_POWER1_PORT") as string; cnt++;
                //power_PortName2 = rk.GetValue(position + "_" + cnt.ToString("D2") + "_POWER2_PORT") as string; cnt++;
                //nht_PortName = rk.GetValue(position + "_" + cnt.ToString("D2") + "_TEMP_PORT") as string; cnt++;

                can_cycler1 = rk.GetValue(position + "_" + cnt.ToString("D2") + "_CAN_CYCLER") as string; cnt++;

                PLC_RECV_ADDRESS = rk.GetValue(position + "_" + cnt.ToString("D2") + "_PLC_RECV_ADDRESS") as string; cnt++;
                PLC_SEND_ADDRESS = rk.GetValue(position + "_" + cnt.ToString("D2") + "_PLC_SEND_ADDRESS") as string; cnt++;
                PLC_BCRS_ADDRESS = rk.GetValue(position + "_" + cnt.ToString("D2") + "_PLC_BCRS_ADDRESS") as string; cnt++;
                PLC_LOGICAL_NUMBER = int.Parse(rk.GetValue(position + "_" + cnt.ToString("D2") + "_PLC_LOGICAL_NUMBER") as string); cnt++;

                string lineCheck = rk.GetValue("LINE_FLAG(Old:0, New:1)") as string;
                if (lineCheck == null || lineCheck == "")
                {
                    rk.SetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_IP_FIRST", daq970_IP_First); cnt++;
                    rk.SetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_PORT_FIRST", daq970_Port_First); cnt++;
                    rk.SetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_IP_SECOND", daq970_IP_Second); cnt++;
                    rk.SetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_PORT_SECOND", daq970_Port_Second); cnt++;
                    rk.SetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_IP_THIRD", daq970_IP_Third); cnt++;
                    rk.SetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_PORT_THIRD", daq970_Port_Third); cnt++;
                    cnt = 19;

                    //210806 JKD - DAQ970 New Line(12호,13호) 구분
                    rk.SetValue("LINE_FLAG(Old:0, New:1)", lineFlag.ToString());
                }

                daq970_IP_First = rk.GetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_IP_FIRST") as string; cnt++;
                daq970_Port_First = rk.GetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_PORT_FIRST") as string; cnt++;
                daq970_IP_Second = rk.GetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_IP_SECOND") as string; cnt++;
                daq970_Port_Second = rk.GetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_PORT_SECOND") as string; cnt++;
                daq970_IP_Third = rk.GetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_IP_THIRD") as string; cnt++;
                daq970_Port_Third = rk.GetValue(position + "_" + cnt.ToString("D2") + "_DAQ970_PORT_THIRD") as string; cnt++;
                //210806 JKD - DAQ970 New Line(12호,13호) 구분
                lineFlag = rk.GetValue("LINE_FLAG(Old:0, New:1)") as string;

                if (lineFlag == "1") //기존라인("0"), 신규라인("1")
                {
                    isLineFlag = true;
                    logaddr = @"C:\Users\Public\EOL_INSPECTION_LOG";
                    FILE_PATH_INSPECTION_RESULT = "c:\\Logs\\Inspection_result\\";
                }
                else
                {
                    isLineFlag = false;
                    logaddr = @"D:\EOL_INSPECTION_LOG";
                    FILE_PATH_INSPECTION_RESULT = "d:\\Logs\\Inspection_result\\";
                }

                FETCH_CHECK_CYCLE_TIME = int.Parse(rk.GetValue("Fetch_Check_Cycle_Time") as string);
                //추가 레지스터가 필요하다면, 해당 부분에 추가
                //ceiPathOffset = double.Parse(rk.GetValue("CEI_PATH_OFFSET").ToString());
            }
            catch (Exception ec)
            {
                SetLocalData();
            }
        }

        public string connTargetOhm = "0";
        public string connTargetCh = "310";

        public void GetConnTarget()
        {
            try
            {
                string regSubkey = "Software\\EOL_Trigger";
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
                if (rk.GetValue("connTargetOhm") == null)
                {
                    rk.SetValue("connTargetOhm", connTargetOhm);
                    rk.SetValue("connTargetCh", connTargetCh);
                    connTargetOhm = "0";
                    connTargetCh = "310";

                }
                else
                {
                    connTargetOhm = rk.GetValue("connTargetOhm") as string;
                    connTargetCh = rk.GetValue("connTargetCh") as string;
                }
                LogState(LogType.Info, "Cooling pin connection check limit Spec :" + connTargetOhm.ToString());
                LogState(LogType.Info, "Cooling pin connection check limit Channel :" + connTargetCh.ToString());
            }
            catch (Exception ec)
            {
            }
        }
        #endregion

        #region 값 판정 부분

        public static string _COMM_FAIL = "COMM_FAIL";
        public static string _CYCLER_SAFETY = "CYCLER_SAFETY_STOPPED";
        public static string _EMG_STOPPED = "EMERGENCY_STOPPED";
        public static string _DEVICE_NOT_READY = "DEVICE_NOT_READY";
        public static string _VALUE_NOT_MATCHED = "VALUE_NOT_MATCHED";
        public static string _TEMP_OVER = "AMBIENT_TEMP_OVER";
        public static string _NOT_POS_MSG = "NOT_POSITIVE_MSG";
        public static string _NOT_NEG_MSG = "NOT_NEGATIVE_MSG";
        public static string _NOT_MODE_CHANGED = "NOT_MODE_CHANGED";
        public static string _JIG_NOT_DEACTIVE = "JIG_NOT_DEACTIVE";
        public static string _JIG_NOT_ACTIVE = "JIG_NOT_ACTIVE";
        public static string _LV_IS_CONNECTED = "LV_IS_CONNECTED";
        public static string _CONT_CLOSE_STUCK = "CONTACTOR_CLOSE_STUCK";
        public static string _HI_LIMIT = "HI_LIMIT";
        public static string _LOW_LIMIT = "LOW_LIMIT";
        public static string _IO = "IO";
        public static string _ADI_OVER = "ADI_OVER";
        public static string _ADV_OVER = "ADV_OVER";
        public static string _GR_CONT = "GR_CONT";
        public static string _GFI_FAULT = "GFI_FAULT";
        public static string _STOP = "STOP";
        public static string _USER_STOP = "USER_STOP";
        public static string _CAN_NOT_TEST = "CAN_NOT_TEST";
        public static string _TESTING = "TESTING";
        public static string _VOLTAGE_PIN_SENSING_FAIL = "VOLTAGE_PIN_SENSING_FAIL";
        public static string _SENSING_BOARD_CHECK = "SENSING_BOARD_CHECK";
        public static string _CHECK_TERMINAL = "EMERGENCY_STOPPED_CHECK_THE_TERMINAL";
        public static string _DAQ_COM_NG = "DAQ_COMMUNICATION_NG";
        public static string _DAQ_VOLTAGE_NG = "DAQ_VOLTAGE_NG";
        public static string _START_JOB_NG = "START_JOB_NG";

        //200611 추가
        public const string _DATA_BLANK = "DATA_BLANK";

        // 211027
        public const string _MES_CELL_DATA_COUNT_NG = "MES_Cell_Data_Count_NG";
        public const string _MES_CELL_DATA_CALCULATE_NG = "MES_Cell_Data_Calculate_NG";

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
                    blinder3.Visibility = blinder2.Visibility = blinder.Visibility = System.Windows.Visibility.Hidden;
                }
            }));

            isProcessingFlag = false;
            double itemVal = 0;

            //2. 검사항목 중 바코드 판정 시 All pass 적용
            if (ti.Name == _BARCODE || ti.Name == "BarcodeReading2")
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

            if (ti.Value_.ToString() != _CHECK_TERMINAL)
            {
                if (isStop)
                {
                    ti.Value_ = _EMG_STOPPED;
                }
            }

            if (ti.Value_.ToString() == "PASS")
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

            //3.검사 판정 시 EMG인 경우 NG로 강제 적용(절연저항 측정 / DCIR 측정 등 인경우)
            if (ti.Value_ == null
                || ti.Value_.ToString().Contains(":")
                || ti.Value_.ToString() == ""
                || ti.Value_.ToString() == _HI_LIMIT
                || ti.Value_.ToString() == _LOW_LIMIT
                || ti.Value_.ToString() == _IO
                || ti.Value_.ToString() == _ADI_OVER
                || ti.Value_.ToString() == _ADV_OVER
                || ti.Value_.ToString() == _GR_CONT
                || ti.Value_.ToString() == _CONT_CLOSE_STUCK
                || ti.Value_.ToString() == _GFI_FAULT
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
                || ti.Value_.ToString() == _VOLTAGE_PIN_SENSING_FAIL
                || ti.Value_.ToString() == _SENSING_BOARD_CHECK
                || ti.Value_.ToString() == _CHAN_HI_FAIL
                || ti.Value_.ToString() == _CHAN_LO_FAIL
                || ti.Value_.ToString() == _CASE_113
                || ti.Value_.ToString() == _CASE_114
                || ti.Value_.ToString() == _CASE_115
                || ti.Value_.ToString() == _CASE_112
                || ti.Value_.ToString() == _CASE_33
                || ti.Value_.ToString() == _CASE_49
                || ti.Value_.ToString() == _CASE_34
                || ti.Value_.ToString() == _CASE_50
                || ti.Value_.ToString() == _CASE_35
                || ti.Value_.ToString() == _CASE_36
                || ti.Value_.ToString() == _CASE_52
                || ti.Value_.ToString() == _CASE_37
                || ti.Value_.ToString() == _CASE_38
                || ti.Value_.ToString() == _CASE_54
                || ti.Value_.ToString() == _CASE_39
                || ti.Value_.ToString() == _CASE_55
                || ti.Value_.ToString() == _CASE_120_DC
                || ti.Value_.ToString() == _CASE_120_IR
                || ti.Value_.ToString() == _CASE_121_DC
                || ti.Value_.ToString() == _CASE_121_IR
                || ti.Value_.ToString() == _CASE_43
                || ti.Value_.ToString() == _HIPOT_EMO
                || ti.Value_.ToString() == _HIPOT_OVERFLOW
                || ti.Value_.ToString() == _CHECK_TERMINAL
                || ti.Value_.ToString() == _DAQ_COM_NG
                || ti.Value_.ToString() == _DAQ_VOLTAGE_NG
                || ti.Value_.ToString() == _START_JOB_NG
            //211204 JKD - MES 셀 전압 편차 항목 판정
            || ti.Value_.ToString() == _MES_CELL_DATA_COUNT_NG
            || ti.Value_.ToString() == _MES_CELL_DATA_CALCULATE_NG)

            {
                //200727 wjs add
                if (ti.Value_ != null && ti.Value_.ToString() == "")
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

                //내전압에서 Arc 및 DC-HIGH 일 경우 DCIR까지 진행해야 됨 //200820 add by kyj
                if (ti.btname == "(+) Withstand voltage check" && (ti.Value_.ToString() == _CASE_33 ||
                    ti.Value_.ToString() == _CASE_35))
                {
                    m_bIsWithstandNG = true;
                }

                //내전압에서 Arc 및 DC-HIGH 일 경우 DCIR까지 진행해야 됨 //200820 add by kyj
                if (ti.btname == "(+) Isolation check")
                {
                    m_bIsIRNG = true;
                }

                return false;
            }

            if (double.TryParse(ti.Value_.ToString(), out itemVal))
            {
                //itemVal = 9.954367;
                #region 자릿수를 만들어 주는 부분 추가
                string digit = "F" + ti.DigitLength;
                #endregion

                ti.Value_ = itemVal = double.Parse(itemVal.ToString(digit));
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

                    return false;
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

                //내전압에서 Arc 및 DC-HIGH 일 경우 DCIR까지 진행해야 됨 //200820 add by kyj
                if (ti.btname == "(+) Isolation check")
                {
                    m_bIsIRNG = true;
                }

                return false;
            }
        }

        #endregion

        #region Multi Models

        /// <summary>
        /// -1:DUMMY
        /// 0:12S
        /// 2:14S 
        /// </summary>
        public int localTypes = 0;

        private void sideChangedUp(object sender, MouseButtonEventArgs e)
        {
            var lb = sender as Label;


            this.ChangeUIModes(lb.Name.ToString());

            this.LoadList();

            modelGrid.IsEnabled = false;
            //190124 ht MES 스킵되있을때는 스펙받아오지 아니한다
            var th = new Thread(() =>
            {
                if (MES.isMESConnected && !isMESskip)
                {
                    GetControlItemFromMES();
                    GetCollectItemFromMES();
                }
                else
                {
                    GetControlItemFromCSV();
                    GetCollectItemFromCSV();

                }

                this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.testItemListDg.Items.Refresh();
                        modelGrid.IsEnabled = true;
                    }));
            });
            th.Start();
        }

        public void AutoMode_change(string mode_name)
        {
            //181217 begin인 경우는 MES 스펙 받아올때랑 꼬이는 경우가 있다!!
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.ChangeUIModes(mode_name);

                this.LoadList();

                this.testItemListDg.Items.Refresh();
            }));
        }

        private void entranceDummyMode(object sender, MouseButtonEventArgs e)
        {
            this.AutoMode_change("dummy");
        }

        //190104 by grchoi
        public void ChangeUIModes_Init()
        {
            string initLbName = "";

            if (type1lb.Background == Brushes.SkyBlue)
            {
                initLbName = type1lb.Name.ToString();
            }
            else if (type3lb.Background == Brushes.SkyBlue)
            {
                initLbName = type3lb.Name.ToString();
            }
            else
            {
                initLbName = "dummy";
            }
            switch (initLbName)
            {
                case "type1lb":
                    type1lb.Background = Brushes.SkyBlue; type2lb.Background = Brushes.Gray; type3lb.Background = Brushes.Gray; type4lb.Background = Brushes.Gray; type5lb.Background = Brushes.Gray; type6lb.Background = Brushes.Gray; localTypes = 0;
                    LogState(LogType.Info, "4P8S Mode");
                    this.prodTb.Text = prodID_4P8S;
                    modelList[selectedIndex].ProdId = prodID_4P8S;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_4P8S";
                    break;
                case "type2lb":
                    type2lb.Background = Brushes.SkyBlue; type1lb.Background = Brushes.Gray; type3lb.Background = Brushes.Gray; type4lb.Background = Brushes.Gray; type5lb.Background = Brushes.Gray; type6lb.Background = Brushes.Gray; localTypes = 1;
                    LogState(LogType.Info, "4P8S(Reverse) Mode");
                    this.prodTb.Text = prodID_4P8S_Rev;
                    modelList[selectedIndex].ProdId = prodID_4P8S_Rev;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_4P8S_REV";
                    break;
                case "type3lb":
                    type3lb.Background = Brushes.SkyBlue; type1lb.Background = Brushes.Gray; type2lb.Background = Brushes.Gray; type4lb.Background = Brushes.Gray; type5lb.Background = Brushes.Gray; type6lb.Background = Brushes.Gray; localTypes = 2;
                    LogState(LogType.Info, "4P7S Mode");
                    this.prodTb.Text = prodID_4P7S;
                    modelList[selectedIndex].ProdId = prodID_4P7S;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_4P7S";

                    break;
                case "type4lb":
                    type4lb.Background = Brushes.SkyBlue; type1lb.Background = Brushes.Gray; type2lb.Background = Brushes.Gray; type3lb.Background = Brushes.Gray; type5lb.Background = Brushes.Gray; type6lb.Background = Brushes.Gray; localTypes = 3;
                    LogState(LogType.Info, "3P8S Mode");
                    this.prodTb.Text = prodID_3P8S;
                    modelList[selectedIndex].ProdId = prodID_3P8S;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_3P8S";

                    break;
                case "type5lb":
                    type5lb.Background = Brushes.SkyBlue; type1lb.Background = Brushes.Gray; type2lb.Background = Brushes.Gray; type3lb.Background = Brushes.Gray; type4lb.Background = Brushes.Gray; type6lb.Background = Brushes.Gray; localTypes = 4;
                    LogState(LogType.Info, "3P10S Mode");
                    this.prodTb.Text = prodID_3P10S;
                    modelList[selectedIndex].ProdId = prodID_3P10S;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_3P10S";

                    break;
                case "type6lb":
                    type6lb.Background = Brushes.SkyBlue; type1lb.Background = Brushes.Gray; type2lb.Background = Brushes.Gray; type3lb.Background = Brushes.Gray; type4lb.Background = Brushes.Gray; type5lb.Background = Brushes.Gray; localTypes = 5;
                    LogState(LogType.Info, "3P10S(Reverse) Mode");
                    this.prodTb.Text = prodID_3P10S_Rev;
                    modelList[selectedIndex].ProdId = prodID_3P10S_Rev;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_3P10S_REV";
                    break;
                case "dummy":
                    //type1lb.Background = type2lb.Background = type3lb.Background = Brushes.SkyBlue; localTypes = -1;
                    LogState(LogType.Info, "NG MASTER Mode ");
                    //this.prodTb.Text = prodID_12S;
                    //modelList[selectedIndex].ProdId = prodID_12S;
                    break;

            }
        }

        public void ChangeUIModes(string mode_name)
        {
            switch (mode_name)
            {
                case "type1lb":
                    type1lb.Background = Brushes.SkyBlue; type2lb.Background = Brushes.Gray; type3lb.Background = Brushes.Gray; type4lb.Background = Brushes.Gray; type5lb.Background = Brushes.Gray; type6lb.Background = Brushes.Gray; localTypes = 0;
                    LogState(LogType.Info, "4P8S Mode");
                    this.prodTb.Text = prodID_4P8S;
                    modelList[selectedIndex].ProdId = prodID_4P8S;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_4P8S";
                    break;
                case "type2lb":
                    type2lb.Background = Brushes.SkyBlue; type1lb.Background = Brushes.Gray; type3lb.Background = Brushes.Gray; type4lb.Background = Brushes.Gray; type5lb.Background = Brushes.Gray; type6lb.Background = Brushes.Gray; localTypes = 1;
                    LogState(LogType.Info, "4P8S(Reverse) Mode");
                    this.prodTb.Text = prodID_4P8S_Rev;
                    modelList[selectedIndex].ProdId = prodID_4P8S_Rev;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_4P8S_REV";
                    break;
                case "type3lb":
                    type3lb.Background = Brushes.SkyBlue; type1lb.Background = Brushes.Gray; type2lb.Background = Brushes.Gray; type4lb.Background = Brushes.Gray; type5lb.Background = Brushes.Gray; type6lb.Background = Brushes.Gray; localTypes = 2;
                    LogState(LogType.Info, "4P7S Mode");
                    this.prodTb.Text = prodID_4P7S;
                    modelList[selectedIndex].ProdId = prodID_4P7S;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_4P7S";

                    break;
                case "type4lb":
                    type4lb.Background = Brushes.SkyBlue; type1lb.Background = Brushes.Gray; type2lb.Background = Brushes.Gray; type3lb.Background = Brushes.Gray; type5lb.Background = Brushes.Gray; type6lb.Background = Brushes.Gray; localTypes = 3;
                    LogState(LogType.Info, "3P8S Mode");
                    this.prodTb.Text = prodID_3P8S;
                    modelList[selectedIndex].ProdId = prodID_3P8S;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_3P8S";

                    break;
                case "type5lb":
                    type5lb.Background = Brushes.SkyBlue; type1lb.Background = Brushes.Gray; type2lb.Background = Brushes.Gray; type3lb.Background = Brushes.Gray; type4lb.Background = Brushes.Gray; type6lb.Background = Brushes.Gray; localTypes = 4;
                    LogState(LogType.Info, "3P10S Mode");
                    this.prodTb.Text = prodID_3P10S;
                    modelList[selectedIndex].ProdId = prodID_3P10S;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_3P10S";

                    break;
                case "type6lb":
                    type6lb.Background = Brushes.SkyBlue; type1lb.Background = Brushes.Gray; type2lb.Background = Brushes.Gray; type3lb.Background = Brushes.Gray; type4lb.Background = Brushes.Gray; type5lb.Background = Brushes.Gray; localTypes = 5;
                    LogState(LogType.Info, "3P10S(Reverse) Mode");
                    this.prodTb.Text = prodID_3P10S_Rev;
                    modelList[selectedIndex].ProdId = prodID_3P10S_Rev;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_3P10S_REV";
                    break;
                case "dummy":
                    //type1lb.Background = type2lb.Background = type3lb.Background = Brushes.SkyBlue; localTypes = -1;
                    LogState(LogType.Info, "NG MASTER Mode ");
                    //this.prodTb.Text = prodID_12S;
                    //modelList[selectedIndex].ProdId = prodID_12S;
                    break;

            }
        }

        #endregion

        /// <summary>
        /// 로그로 저장될 폴더명
        /// </summary>
        string MODEL_NAME = "C727";

        /// <summary>
        /// Cycler DSP Types
        /// </summary>
        public DSPType dsp_Type = DSPType.DSP_28377;

        /// <summary>
        /// last updated date (Recommanded)
        /// </summary>
        public string lastUpdated = "Ford_DCIR.HIPOT_MP_V01.23.02.23_Last";

        /// <summary>
        /// 기본 Equip ID
        /// </summary>
        public string equipID = "";

        public string prodID_4P8S = "";
        public string prodID_4P8S_Rev = "";
        public string prodID_4P7S = "";
        public string prodID_3P8S = "";
        public string prodID_3P10S = "";
        public string prodID_3P10S_Rev = "";

        //크로마 버젼 사용 221121
        public string strChromaModelType = "";

        /// <summary>
        /// 기본 Equip ID
        /// </summary>
        public string procID = "";

        bool isNeedOCVFlag = false;


        public bool isMasterBcr = false;

        public MainWindow()
        {
            InitializeComponent();

            //언어 인터락 nnkim 221122
            WindowLanguageFormatCheck();

            LoadINI(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            //SetLocalData(); //초기 1회는 셋을 해줘야 함
            LoadLocalData();
            GetConnTarget();
            //221121 크로마 모델 확인
            LoadChromaModelType();

            InitializeCommonMethods();
            InitializeSaveFileAddr();
            Counter_Cycler();

            //20221017 소모품 카운트 체크
            SetPartsCountData();


        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetUI();

            ChangeUIModes_Init();

            SetModules();

            SetWindow();

            LoadList();

            // 2019.08.27 jeonhj's comment
            // MES 연결에 상관없이 Local List를 로드한다.
            // 만약 MES 제품정보 등 id가 맞지 않아 MES에서 값을 받지 못하면 ControlList가 날아가는 문제가 있어 Local을 먼저 로드한다.

            var th = new Thread(() =>
            {
                GetControlItemFromCSV();
                GetCollectItemFromCSV();

                if (MES.isMESConnected)
                {
                    GetControlItemFromMES();
                    GetCollectItemFromMES();
                }
            });
            th.Start();

            //scs.LoadSafetyCyclerSpec();

            // 211206 JKD - 검사기 EMO Switch 동작 추가
            emo_tmr = new System.Timers.Timer();
            emo_tmr.Interval = 500;
            emo_tmr.Elapsed += new System.Timers.ElapsedEventHandler(EmoTimer_Tick);
            emo_tmr.Start();

            //221111 DMM UI Hide
            this.contBt_daq970_2.Visibility = System.Windows.Visibility.Hidden;
            this.lbdaq970_2.Visibility = System.Windows.Visibility.Hidden;
            this.contBt_daq970_3.Visibility = System.Windows.Visibility.Hidden;
            this.lbdaq970_3.Visibility = System.Windows.Visibility.Hidden;

        }

        private void EmoTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            // DI 채널 확인
            // Relay Background에서 처리된 것이 넘어와야 함.
            // EMO SW가 B접이라 평소에 Close 상태임. 그래서 스위치가 눌렸을 때 0이 되는게 맞음.
            string regSubkey = "Software\\EOL_Trigger";
            string strValue = "";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            var regValue = rk.GetValue("EMO_SENSE") as string;

            if (regValue == null) { rk.SetValue("EMO_SENSE", "1"); strValue = "1"; }
            else { strValue = regValue; }

            if (strValue == "0")
            {
                pushedEmo = true;
                StopAuto(); // EMO가 눌리면 프로그램에서 Emergency Stop이 눌렸을 때처럼 처리한다.
            }
            else
            {
                pushedEmo = false;
            }
        }

        private void SetWindow()
        {
            this.Width = SystemParameters.WorkArea.Width;
            this.Height = SystemParameters.WorkArea.Height;
            this.Top = 0;
            this.Left = 0;
        }

        private void SetModules()
        {
            //181217 mes가 연결되있을 때 릴레이가 초기화가 안되어있으면 터지는거 때문에 수정
            relays = new CRelay_Receiver(this);

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                chroma = new CChroma(this, this.chroma_PortName);
            }), null);

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                MESConnect();
            }), null);

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                plc = new CPLC(this, PLC_RECV_ADDRESS, PLC_SEND_ADDRESS, PLC_BCRS_ADDRESS, PLC_LOGICAL_NUMBER);
            }), null);

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                cycler = new CCycler(this, this.can_cycler1);//pcan
            }), null);

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                temps = new CNhtRS232_Receiver(this);
            }), null);

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                daq = new CDAQ(this, daq_PortName);
            }), null);

            if (isLineFlag)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                {
                    daq970_1 = new CDAQ970(this, daq970_IP_First, daq970_Port_First);
                }), null);

                //ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                //{
                //    daq970_2 = new CDAQ970(this, daq970_IP_Second, daq970_Port_Second);
                //}), null);

                //ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                //{
                //    daq970_3 = new CDAQ970(this, daq970_IP_Third, daq970_Port_Third);
                //}), null);

                DAQ1_Status.Visibility = Visibility.Visible;
                //DAQ2_Status.Visibility = Visibility.Visible;
                //DAQ3_Status.Visibility = Visibility.Visible;
                DMM_Status.Visibility = Visibility.Collapsed;
            }
            else
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                {
                    //keysight = new CKeysightDMM(this, "169.254.4.61", 5025);
                    keysight = new CKeysightDMM(this, true, keysight_PortName);
                }), null);

                DAQ1_Status.Visibility = Visibility.Collapsed;
                //DAQ2_Status.Visibility = Visibility.Collapsed;
                //DAQ3_Status.Visibility = Visibility.Collapsed;
                DMM_Status.Visibility = Visibility.Visible;
            }
        }

        private void SetUI()
        {
            selectedIndex = 0;

            modelList.Add(new Models()
            {
                Number = "0",
                ProdId = prodID_4P8S,
                ProcId = procID,
                EquipId = equipID,
                UserId = "EIF",
                ModelId = MODEL_NAME,
                LotId = "NOT_LOT_ID"
            });

            mmw = new MiniScheduler.MainWindow(this);
            this.totalProcessList = mmw.totalProcessList;

            this.prodTb.Text = this.modelList[selectedIndex].ProdId;//제품 ID
            this.procTb.Text = this.modelList[selectedIndex].ProcId;//공정ID
            this.userTb.Text = this.modelList[selectedIndex].UserId;
            this.equipTb.Text = this.modelList[selectedIndex].EquipId;

            cellDetailList.Clear();
            cellDetailList.Add(new CellDetail() { TestName = "Init Volt" });
            cellDetailList.Add(new CellDetail() { TestName = "Discharge" });
            cellDetailList.Add(new CellDetail() { TestName = "Rest" });
            cellDetailList.Add(new CellDetail() { TestName = "Charge" });
            cellDetailList.Add(new CellDetail() { TestName = "Finish Volt (10s)" });
            cellDetailList.Add(new CellDetail() { TestName = "Finish Volt (20s)" });
            cellDetailList.Add(new CellDetail() { TestName = "Finish Volt (30s)" });
            cellDetailList.Add(new CellDetail() { TestName = "Finish Volt (40s)" });
            ClearCellDetailList();

            CellDetailDg.ItemsSource = cellDetailList;

            if (isNeedOCVFlag)
            {
                isNeedOCV.Height = new GridLength(80);
                ocvDetailList.Clear();
                ocvDetailList.Add(new OcvDetail() { TestName = "OCV Voltage" });
                ocvDetailList.Add(new OcvDetail() { TestName = "After DCIR Delta Voltage" });
                ClearOcvDetailList();

                OCVDetailDg.ItemsSource = ocvDetailList;
            }
        }

        public void ClearOcvDetailList()
        {
            foreach (var item in ocvDetailList)
            {
                item.CellVolt_1 = item.CellVolt_2 = item.CellVolt_3 = item.CellVolt_4 = item.CellVolt_5 = item.CellVolt_6 = item.CellVolt_7 = item.CellVolt_8 = 0.0;
            }
        }

        private void InitializeSaveFileAddr()
        {
            DirectoryInfo di = new DirectoryInfo(logaddr);
            if (di.Exists == false)
            {
                di.Create();
            }

            string @LogDirectory = "";

            if (isLineFlag)
            {
                @LogDirectory = @"C:\Logs\Inspection_result\" + MODEL_NAME;
            }
            else
            {
                @LogDirectory = @"D:\Logs\Inspection_result\" + MODEL_NAME;
            }

            if (!Directory.Exists(@LogDirectory))
            {
                Directory.CreateDirectory(@LogDirectory);
            }
                        
            if (isLineFlag)
            {
                @LogDirectory = @"C:\Users\Public\EOL_INSPECTION_LOG";
            }
            else
            {
                @LogDirectory = @"D:\EOL_INSPECTION_LOG";
            }

            if (!Directory.Exists(@LogDirectory))
            {
                Directory.CreateDirectory(@LogDirectory);
            }
        }

        private void InitializeCommonMethods()
        {
            bt_pass.Content = "-";
            bt_pass.Background = Brushes.DarkGray;
            timetick.Interval = 100;
            timetick.Tick += ti_Tick;
            this.Loaded += MainWindow_Loaded;
            //this.KeyDown += MainWindow_KeyDown;
            this.Closed += MainWindow_Closed;
            this.Closing += MainWindow_Closing;
            this.KeyDown += MainWindow_KeyDown;
        }

        public string position = "#1";

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

        /// <summary>
        /// eolList.csv에서 실제 검사항목으로 불러오는 부분
        /// </summary>
        private void LoadList()
        {
            try
            {
                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
                //var arr = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "eollist.csv", Encoding.Default);

                //181218 여러개 모델일 때 버튼이 새로 추가 안되는 현상 FIX
                groupDic = new Dictionary<string, List<TestItem>>();
                groupDic.Clear();
                var li = new List<TestItem>();

                FileStream readData = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "eollist.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

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

                        var groupName = ar[0].Replace(" ", "").Replace("(", "_").Replace(")", "_").Replace("-", "_").Replace("/", "").Replace("%", "");

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

                        switch (localTypes)
                        {
                            case 0:
                                if (ar[9] == "0") continue;
                                break;
                            case 1:
                                if (ar[10] == "0") continue;
                                break;
                            case 2:
                                if (ar[11] == "0") continue;
                                break;
                            case 3:
                                if (ar[12] == "0") continue;
                                break;
                            case 4:
                                if (ar[13] == "0") continue;
                                break;
                            case 5:
                                if (ar[14] == "0") continue;
                                break;
                            default:
                                break;
                        }

                        try
                        {
                            var obj = this.FindName(button.Name);
                            if (obj == null)
                                this.RegisterName(button.Name, button);
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

                modelList[selectedIndex].TestItemList.Clear();
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
                        modelList[selectedIndex].TestItemList.Add(initem.Bt.Content.ToString(), initem);
                    }

                    //MakeGroupMethodToTextFile(sdic);
                    singleGrid.Children.Add(comboBox);
                    var groupButton = new Button();
                    groupButton.Content = dicItem.Key;
                    groupButton.Name = dicItem.Value[0].GroupName;
                    groupButton.Margin = new Thickness(0, 0, 30, 0);
                    groupButton.Click += GroupButtonClick;
                    groupButton.Style = this.FindResource("manual_bt_s") as Style;
                    //7.
                    //190104 grchoi
                    //20181226 wjs try~catch add(unregister->register)                    
                    try
                    {
                        this.RegisterName(groupButton.Name, groupButton);
                    }
                    catch (Exception)
                    {
                        this.UnregisterName(groupButton.Name);
                        //LogState(LogType.Fail, "UN Reregister Bt");
                        try
                        {
                            //LogState(LogType.Fail, "Reregister Bt");
                            this.RegisterName(groupButton.Name, groupButton);
                        }
                        catch (Exception)
                        {
                            //LogState(LogType.Fail, "Reregister Bt");
                        }
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

                this.testItemListDg.ItemsSource = modelList[selectedIndex].TestItemList;
            }
            catch (Exception ec)
            {
                LogState(LogType.Fail, "LoadList", ec);
            }
        }


        Thread GroupBtThread, SingleBtThread;


        const string RETRY_MSG = "연결상태가 정상이 아닙니다. 다시 ";
        const string HIPOT_ENABLE_MAIN_MSG = "HIPOT 프로브를 연결해주세요.";
        const string HIPOT_ENABLE_SUB_MSG = "1. HIPOT 프로브 연결 확인.\n2. LV커넥터 제거 확인\n3. 전류 프로브 제거 확인\n";

        const string LV_ENABLE_MAIN_MSG = "LV커넥터를 연결해주세요.";
        const string LV_ENABLE_SUB_MSG = "1. 전류 프로브 연결 확인.\n2. LV커넥터 연결 확인";
        const string CURRENT_ENABLE_MAIN_MSG = "전류 프로브를 연결해주세요.";
        const string CURRENT_ENABLE_SUB_MSG = "1. 전류 프로브 연결 확인.\n2. LV커넥터 연결 확인\n3. HIPOT 프로브 제거 확인";
        const string DISABLE_MAIN_MSG = "모든 프로브를 제거해주세요.";
        const string DISABLE_SUB_MSG = "1. HIPOT 프로브 제거 확인.\n2. LV커넥터 제거 확인\n3. 전류 프로브 제거 확인\n";

        const string CONTACT_ENABLE_MAIN_MSG = "Contact Check 프로브를 제거해주세요";
        const string CONTACT_ENABLE_SUB_MSG = "1. Contact Check 프로브 제거 확인.\n2. HIPOT 프로브 연결 확인.\n3. LV커넥터 제거 확인\n4. 전류 프로브 제거 확인\n";


        const string IV_ENABLE_MAIN_MSG = "IV Check 프로브를 연결해주세요";
        const string IV_ENABLE_SUB_MSG = "1. IV Check 프로브 연결 확인.\n2. HIPOT 프로브 제거 확인\n3. LV커넥터 제거 확인\n4. 전류 프로브 제거 확인";

        bool SensingVoltToMux()
        {
            LogState(LogType.Info, "-----------------------------------------------------------------");
            LogState(LogType.Info, "SensingVoltToMux Start");

            cellVoltCnt = 0;
            string measString = "";
            int reccnt = 0;
            bool isOVER3V = false;
            SetChannels(out measString, out cellVoltCnt, out reccnt);

            if (isLineFlag)
            {
                #region measure Each Voltage
                List<double> dvList = new List<double>();
                string voltstr = "";

                if (!daq970_1.MeasVolt_Multi(out dvList, measString, reccnt))
                {
                    LogState(LogType.Fail, "SensingVoltToMux");
                    return true;
                }
                else
                {
                    isOVER3V = false;
                }

                for (int i = 0; i < dvList.Count; i++)
                {
                    double dv = 0;
                    dv = Math.Abs(dvList[i]);

                    if (dv > 3)
                        isOVER3V = true;

                    voltstr += dv.ToString() + ",";
                }
                LogState(LogType.RESPONSE, "KeysightDAQ970 Volt:" + voltstr);
                #endregion
            }
            else
            {
                #region measure Each Voltage
                keysight.rtstring = "";
                keysight.MeasVolt(measString);

                var rec = keysight.sp.BytesToRead;

                var cnt = 0;
                while (rec < reccnt)//33
                {
                    Thread.Sleep(100);
                    rec = keysight.sp.BytesToRead;
                    cnt += 100;
                    //not received data
                    if (cnt == 5000)
                    {
                        keysight.MeasVolt(measString);

                        rec = keysight.sp.BytesToRead;

                        cnt = 0;
                        while (rec < 145)//33
                        {
                            Thread.Sleep(100);
                            rec = keysight.sp.BytesToRead;
                            cnt += 100;
                            if (cnt == 5000)
                            {
                                LogState(LogType.Fail, "SensingVoltToMux");
                                return true;
                            }
                        }
                        break;
                    }
                }
                //받은 후에 데이터 파싱
                var bt = new byte[rec];
                keysight.sp.Read(bt, 0, rec);

                keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);

                //LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

                var vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                CellVoltageList = new List<double>();
                string voltstr = "";

                for (int i = 0; i < vArr.Length - 1; i++)
                {
                    double dv = 0;
                    if (double.TryParse(vArr[i], out dv))
                    {
                        if (dv > 3)
                            isOVER3V = true;

                        CellVoltageList.Add(dv);
                        voltstr += dv.ToString() + ",";
                    }
                }

                LogState(LogType.RESPONSE, "KeysightDMM Volt:" + voltstr);
                #endregion
            }

            LogState(LogType.Info, "SensingVoltToMux end");
            LogState(LogType.Info, "-----------------------------------------------------------------");
            return isOVER3V ? true : false;
        }


        bool SensingResToMux()
        {
            // 20191021 Noah Choi 분기박스 mc제어용 릴레이 추가로 인한 코드 추가
            relays.RelayOff("IDO_4");


            LogState(LogType.Info, "-----------------------------------------------------------------");
            LogState(LogType.Info, "SensingResToMux Start");

            cellVoltCnt = 0;
            string measString = "312,313,314,315,316,317";
            int reccnt = 6;
            bool isUNDER1K = false;

            #region measure Each Resistance
            if (isLineFlag)
            {
                #region DAQ970A
                List<double> resList = new List<double>();
                var daqChInfo = new DAQChannelInfo(localTypes);
                string resStr = "";

                if (!daq970_1.MeasRes_Multi(out resList, measString, reccnt))
                {
                    LogState(LogType.Fail, "SensingResToMux");
                    // 20191021 Noah Choi 분기박스 mc제어용 릴레이 추가로 인한 코드 추가
                    relays.RelayOff("IDO_4");
                    return true;
                }
                else
                {
                    for (int i = 0; i < resList.Count; i++)
                    {
                        double dv = 0;
                        dv = Math.Abs(resList[i]);

                        if (dv < 1000)
                            isUNDER1K = true;

                        resStr += dv.ToString() + ",";
                    }

                    isUNDER1K = false;
                }

                // 20191021 Noah Choi 분기박스 mc제어용 릴레이 추가로 인한 코드 추가
                relays.RelayOn("IDO_4");

                LogState(LogType.RESPONSE, "KeysightDAQ970 Res:" + resStr);
                #endregion
            }
            else
            {
                #region DMM34970
                keysight.rtstring = "";
                keysight.MeasRes(measString);

                reccnt = 97;
                var rec = keysight.sp.BytesToRead;

                var cnt = 0;
                while (rec < reccnt)//33
                {
                    Thread.Sleep(100);
                    rec = keysight.sp.BytesToRead;
                    cnt += 100;
                    //not received data
                    if (cnt == 5000)
                    {
                        keysight.MeasRes(measString);

                        rec = keysight.sp.BytesToRead;

                        cnt = 0;
                        while (rec < 145)//33
                        {
                            Thread.Sleep(100);
                            rec = keysight.sp.BytesToRead;
                            cnt += 100;
                            if (cnt == 5000)
                            {
                                LogState(LogType.Fail, "SensingVoltToMux");
                                // 20191021 Noah Choi 분기박스 mc제어용 릴레이 추가로 인한 코드 추가
                                relays.RelayOff("IDO_4");
                                return true;
                            }
                        }
                        break;
                    }
                }

                //받은 후에 데이터 파싱
                var bt = new byte[rec];
                keysight.sp.Read(bt, 0, rec);

                keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);

                //LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

                var vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                string resstr = "";

                for (int i = 0; i < vArr.Length; i++)
                {
                    double dv = 0;
                    if (double.TryParse(vArr[i], out dv))
                    {
                        if (dv < 1000)
                            isUNDER1K = true;

                        resstr += dv.ToString() + ",";
                    }
                }

                // 20191021 Noah Choi 분기박스 mc제어용 릴레이 추가로 인한 코드 추가
                relays.RelayOn("IDO_4");

                LogState(LogType.RESPONSE, "KeysightDMM Res:" + resstr);
                #endregion
            }
            #endregion

            LogState(LogType.Info, "SensingResToMux end");
            LogState(LogType.Info, "-----------------------------------------------------------------");
            return isUNDER1K ? true : false;
        }

        void ShowPopup(string gbtName)
        {
            if (plc.isTesting_Flag)
                return;

            //200624
            if (gbtName == "IRCheck" || gbtName == "ProbeContactCheck" || gbtName == "IRCheck750V" || gbtName == "IRCheck500V")
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    var nextWindow = new NextWindow(HIPOT_ENABLE_MAIN_MSG, HIPOT_ENABLE_SUB_MSG, this);
                    nextWindow.ShowDialog();

                    //200724 wjs SensingVoltToMux delete in IR
                    //if (gbtName == "IRCheck" || gbtName == "IRCheck750V" || gbtName == "IRCheck500V")
                    //200724 wjs SensingVoltToMux add withdstand
                    if (gbtName == "Withstandvoltagecheck")
                    {
                        while (SensingVoltToMux())
                        {
                            nextWindow = new NextWindow(RETRY_MSG + HIPOT_ENABLE_MAIN_MSG, HIPOT_ENABLE_SUB_MSG, this);
                            nextWindow.gradStop.Color = Colors.Red;
                            nextWindow.contibt.Content = "RETRY";
                            nextWindow.infoLb.Content = "!";
                            nextWindow.ShowDialog();
                        }
                    }
                }));
            }
            else if (gbtName == "WithstandIRCheck")
            {
                if (isLineFlag)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        var nextWindow = new NextWindow(CONTACT_ENABLE_MAIN_MSG, CONTACT_ENABLE_SUB_MSG, this);
                        nextWindow.ShowDialog();
                    }));
                }
            }
            else if (gbtName == "Voltages" || gbtName == "ThermistorCheck" || gbtName == "FinalVoltages" || gbtName == "FinalThermistorCheck")
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    var nextWindow = new NextWindow(LV_ENABLE_MAIN_MSG, LV_ENABLE_SUB_MSG, this);
                    nextWindow.ShowDialog();
                }));
            }
            else if (gbtName == "DCIR")
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    var nextWindow = new NextWindow(CURRENT_ENABLE_MAIN_MSG, CURRENT_ENABLE_SUB_MSG, this);
                    nextWindow.ShowDialog();
                }));
            }
            //210804 JKD - Popupp 추가 
            else if (gbtName == "IVCheck")
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    var nextWindow = new NextWindow(IV_ENABLE_MAIN_MSG, IV_ENABLE_SUB_MSG, this);
                    nextWindow.ShowDialog();
                }));
            }
        }

        void HidePopup(string gbtName)
        {
            if (plc.isTesting_Flag)
                return;

            if (gbtName == _BARCODE)
                return;

            if (gbtName == "ProbeContactCheck")
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    var nextWindow = new NextWindow(HIPOT_ENABLE_MAIN_MSG, HIPOT_ENABLE_SUB_MSG, this);
                    nextWindow.gradStop.Color = Colors.Green;
                    nextWindow.contibt.Content = "OK";
                    nextWindow.ShowDialog();
                }));
            }
            else
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    var nextWindow = new NextWindow(DISABLE_MAIN_MSG, DISABLE_SUB_MSG, this);
                    nextWindow.gradStop.Color = Colors.Green;
                    nextWindow.contibt.Content = "OK";
                    nextWindow.ShowDialog();
                }));
            }

        }



        /// <summary>
        /// 그룹버튼을 눌렀을 때, 해당 그룹이름에 맞는 모든 검사항목을 순차적으로 돌린다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupButtonClick(object sender, RoutedEventArgs e)
        {
            if (GroupBtThread != null)
                return;

            isStop = ispause = false;
            string gbtName = (sender as Button).Name.ToString();

            if (gbtName == "BarcodeReading")
            {
                modelList[selectedIndex].LotId = lotTb.Text;
            }

            GroupBtThread = new Thread(() =>
            {
                //검사 전
                //ShowPopup(gbtName);

                bool isPass = true;
                foreach (var item in modelList[selectedIndex].TestItemList)
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
                }

                SetCtrlToPass(isPass, gbtName);

                //검사후
                HidePopup(gbtName);

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

            isStop = ispause = false;
            string sbtName = (sender as Button).Content.ToString();
            //8.바코드리딩 단위검사 시 Lot TextBox 정보가 lot id 저장되도록 적용
            //(1.그룹버튼, 2.싱글버튼)
            //190105 by grchoi
            if (sbtName == "BarcodeReading")
            {
                modelList[selectedIndex].LotId = lotTb.Text;
            }
            foreach (var item in modelList[selectedIndex].TestItemList)
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
                        mmw.savedCb.IsChecked = false;
                        mmw.Show();
                    }; break;
                case Key.F3: { AutoMode(); }; break;

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

        /// <summary>
        /// 바코드를 읽었을 때 시작
        /// </summary>
        public void AutoMode()
        {
            //이미 동작중에 다시 시작이 들어온다면 리턴처리
            if (autoModeThread != null && autoModeThread.IsAlive)
                return;

            this.Dispatcher.Invoke(new Action(() =>
            {
                modelList[0].LotId = Barcode = this.lotTb.Text;
                MonoFrame = this.monoTb.Text;
            }));

            masterTempLot = modelList[0].LotId;

            if (masterTempLot.Length > 6)
            {
                masterTempLot = masterTempLot.Substring(0, 6);

                if (masterTempLot == "MASTER")
                {
                    LogState(LogType.Info, "Recevice MASTER_BCR - " + modelList[0].LotId);
                    isMasterBcr = true;
                    //isSkipNG.IsChecked = true;
                    Thread.Sleep(1000);

                }
                else
                {
                    isMasterBcr = false;
                }
            }

            LogState(LogType.Info, "START_TEST-------------------------------------------------------", null, false);

            //검사값 비우기
            ResetClick(this, null);

            //Flag 초기화 200820
            m_bIsIRNG = false;
            m_bIsWithstandNG = false;
            /////////////////////////

            this.time_start_tb.Text = this.time_finish_tb.Text = this.time_elapsed_tb.Text = "";
            this.testPb.Value = 0;
            nowTime = DateTime.Now;
            this.time_start_tb.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            _stopwatch.Reset();
            _stopwatch.Start();
            this.time_start_tb.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            elapsedtime = new DateTime();
            isMESSkipCb.IsEnabled = manualBt.IsEnabled = false;
            blinder3.Visibility = blinder2.Visibility = Visibility.Visible;

            ThreadStart tstart = new ThreadStart(this.AutoModeStart);
            autoModeThread = new Thread(tstart);
            autoModeThread.IsBackground = true;
            autoModeThread.Start();
            timetick.Start();
            this.LogState(LogType.Info, "Auto mode Started");
        }
        private void AutoModeStart()
        {
            if (plc != null)
                plc.isSuc = false;

            // 2019.08.27 jeonhj's comment
            // MES 재연결
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
                        }
                        else
                        {
                            LogState(LogType.Success, "Connected MES.");
                            relays.RelayOn("IDO_3");
                            break;
                        }
                    }
                    relays.RelayOff("IDO_3");
                }
            }));

            isStop = false;
            double testCount = this.modelList[selectedIndex].TestItemList.Count;
            double progVal = 100.0 / testCount;
            bool isPass = true;

            //prodId = this.Barcode;//제품 ID
            //userId = "EIF";
            //procId = "P8100";//공정ID
            //eqpId = "W1PBMA101-4-4";
            this.modelList[selectedIndex].LotId = this.Barcode;// "LGC-WAH;B0011PL230001";

            this.Dispatcher.Invoke(new Action(() =>
            {
                // 2019.08.27 jeonhj's comment
                // mes skip flag에 의해 변경되지 않아야 함.
                //isMESskip = isMESSkipCb.IsChecked == false ? false : true;
                //isC727MESOk = isC727MESCb.IsChecked == false ? false : true;
                this.modelList[selectedIndex].UserId = userTb.Text;
            }));

            //12. AutomodeStart 시 검사모델/설비ID/제품ID/공정ID를 로그로 남김
            LogState(LogType.Info, "Started Model : " + this.modelList[selectedIndex].ModelId);
            LogState(LogType.Info, "Lot ID : " + this.modelList[selectedIndex].LotId);
            LogState(LogType.Info, "Proc ID : " + this.modelList[selectedIndex].ProcId);
            LogState(LogType.Info, "Prod ID : " + this.modelList[selectedIndex].ProdId);
            LogState(LogType.Info, "Equip ID : " + this.modelList[selectedIndex].EquipId);

            #region 착공, 컨트롤스펙, 프로세싱 스펙
            if (!isMESskip)
            {
                if (!MES.isMESConnected)
                {
                    this.LogState(LogType.Fail, "MES_NOT_CONNECTED");

                    PauseLoop("MES_NOT_CONNECTED");

                    SaveData();

                    Finished(false, false);

                    return;
                }

                //190612
                if (!GetControlItemFromMES(modelList[selectedIndex].LotId))
                {
                    this.LogState(LogType.Fail, "GetControlItemFromMES");

                    PauseLoop("GetControlItemFromMES");

                    SaveData();

                    Finished(false, false);

                    return;
                }

                //190612
                if (!GetCollectItemFromMES(modelList[selectedIndex].LotId))
                {
                    this.LogState(LogType.Fail, "GetCollectItemFromMES");

                    PauseLoop("GetCollectItemFromMES");

                    SaveData();

                    Finished(false, false);

                    return;
                }

                //착공보고
                m_bStartJobResult = true;
                m_bIsStartJobFinish = false;
                var th = new Thread(() =>
                {
                    if (MES.StartJobInsp(this.modelList[selectedIndex].LotId, this.modelList[selectedIndex].ProcId, this.modelList[selectedIndex].EquipId, this.modelList[selectedIndex].UserId) == "NG") //OK일때 시작
                    {
                        this.LogState(LogType.Fail, "MES_StartJobInsp");

                        PauseLoop("MES_StartJobInsp");

                        m_bStartJobResult = false;

                        //SaveData();

                        //Finished(false, false);
                    }
                    else
                    {
                        this.LogState(LogType.Success, "MES_StartJobInsp");
                        m_bStartJobResult = true;
                    }

                    m_bIsStartJobFinish = true;
                });
                th.Start();
            }
            else
            {
                GetControlItemFromCSV();
                GetCollectItemFromCSV();
            }
            SetCyclerStepToMESData(0);
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
                loadLog.Append(this.modelList[selectedIndex].ModelId);
                loadLog.Append("\\");
                deleteFolder(loadLog.ToString());

                StringBuilder loadResult = new StringBuilder();
                loadResult.Append(@FILE_PATH_INSPECTION_RESULT);
                loadResult.Append("\\");
                loadResult.Append(this.MODEL_NAME);
                loadResult.Append("\\");
                loadResult.Append(this.modelList[selectedIndex].ModelId);
                loadResult.Append("\\");
                deleteFolder(loadResult.ToString());

            });

            FolderDelete.Start();

            //retry 확인
            int nLvRetryCntCheck = FordLineRetryRegCheck();
            int nLvRetryCount = 0;
            bool bLvRetryCheck = false;
            bRetryLVFirst = true;

            foreach (var singleGroup in groupDic)
            {
                //221123 nnkim
                //검사 전
                ShowPopup(singleGroup.Value[0].GroupName);

                //200820
                isPass = true;

                foreach (var item in modelList[selectedIndex].TestItemList)
                {
                    var rst = false;

                    if (item.Value.GroupName == singleGroup.Value[0].GroupName)
                    {
                        //Retry 할 부분
                        if(nLvRetryCntCheck != 0)
                        {
                            if (item.Value.GroupName == "Voltages_ThermistorCheck" && bLvRetryCheck == false)
                            {
                            ReVoltage_ReThermistor:
                                //Retry
                                if (nLvRetryCount < nLvRetryCntCheck)
                                {
                                    // 재 foreach 진행 
                                    foreach (var itemRy in modelList[selectedIndex].TestItemList)
                                    {
                                        if (itemRy.Value.GroupName == "Voltages_ThermistorCheck")
                                        {
                                            rst = MethodInvoker(itemRy.Value.SingleMethodName, new object[] { itemRy.Value });

                                            if (!rst)
                                            {
                                                this.Dispatcher.BeginInvoke(new Action(() =>
                                                {
                                                    SetCtrlToPass(false, itemRy.Value.Bt.Name);
                                                }));
                                            }
                                            else
                                            {
                                                this.Dispatcher.BeginInvoke(new Action(() =>
                                                {
                                                    SetCtrlToPass(true, itemRy.Value.Bt.Name);
                                                }));
                                            }

                                            //NG 났을 경우 Retry
                                            if (rst == false)
                                            {
                                                nLvRetryCount++;
                                                PreCheck_With_LV();
                                                bLvRetryCheck = false;
                                                goto ReVoltage_ReThermistor;
                                            }
                                            else
                                            {
                                                bLvRetryCheck = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Retry 하고 나서 NG가 또 나올 경우 NG 처리
                                    isPass = false;
                                    this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        SetCtrlToPass(false, item.Value.Bt.Name);
                                    }));

                                    Thread.Sleep(autobtwtick);
                                    ProgressRefresh(progVal);

                                    //221201 nnkim
                                    break;
                                }
                            }
                            else
                            {
                                if (item.Value.GroupName == "Voltages_ThermistorCheck" && bLvRetryCheck == true) continue;

                                rst = MethodInvoker(item.Value.SingleMethodName, new object[] { item.Value });
                            }
                        }
                        else
                        {
                            rst = MethodInvoker(item.Value.SingleMethodName, new object[] { item.Value });
                        }

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
                }

                SetCtrlToPass(isPass, singleGroup.Value[0].GroupName);

                //검사후
                HidePopup(singleGroup.Value[0].GroupName);

                //내전압에서 Arc 및 DC-HIGH 일 경우 DCIR까지 진행해야 됨 //200820 add by kyj
                if (singleGroup.Value[0].GroupName == "WithstandIRCheck" && !isStop)
                {
                    if (m_bIsIRNG == false && m_bIsWithstandNG == true)
                    {
                        continue;
                    }
                }
                //190612
                if (!isPass || isStop)
                {
                    //190911 pause flag in testing
                    PauseLoop("PAUSE_FLAG_IN_TESTING");

                    SaveData();
                    Finished(false);
                    return;
                }
            }

            ProgressRefresh(100);
            #endregion

            SaveData();
            Finished(true);
            autoModeThread = null;
        }

        public int FordLineRetryRegCheck()
        {
            string regSubkey = "Software\\EOL_Trigger";
            int nRtVal = 0;

            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            var regStr = rk.GetValue("LV_RETRY_COUNTER");

            if (regStr == null)
            {
                rk.SetValue("LV_RETRY_COUNTER", "0");
                nRtVal = 0;
            }
            else
            {
                int.TryParse(regStr.ToString(), out nRtVal);
            }

            return nRtVal;
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
                isMESskip = false;
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    isMESSkipCb.IsChecked = false;
                }));

                //Server Time Request 20221122  nnkim
                if (position == "#1")
                {
                    for (int i = 0; i < 3; i++)
                    {
                        LogState(LogType.Info, string.Format("ServerTime_Request Start - {0}", i));

                        MES.ServerTime_Request(this.modelList[selectedIndex].EquipId);

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
            }
            else
            {
                //isMESskip = true;
            }
        }

        /// <summary>
        /// 검사종류가 여러개일 때 새로고침 되는 부분
        /// </summary>
        public void RefreshToFlags()
        {
            this.testItemListDg.ItemsSource = modelList[selectedIndex].TestItemList;


            this.prodTb.Text = this.modelList[selectedIndex].ProdId;//제품 ID
            this.procTb.Text = this.modelList[selectedIndex].ProcId;//공정ID
            this.equipTb.Text = this.modelList[selectedIndex].EquipId;
            Barcode = this.lotTb.Text = this.modelList[selectedIndex].LotId;// "J9D3-10B759-AB1-170118000001";//" "LGC-WAH;B0011PL230001";
            this.userTb.Text = this.modelList[selectedIndex].UserId;

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
            if (modelList[selectedIndex].TestItemList.ContainsKey(name))
            {
                var p = modelList[selectedIndex].TestItemList[name];
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

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBoxResult.OK == MessageBox.Show("Are you sure you want to Quit?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Information)) { }
            else { e.Cancel = true; }
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
            LoginClickCheckBox();
            if (PermissionResult)
            {
                this.monoTb.Background = this.lotTb.Background = this.prodTb.Background = this.procTb.Background = this.equipTb.Background = Brushes.Red;
                isMESskip = true;
                LogState(LogType.Info, "MES Skip");
                if (relays == null)
                    return;

                relays.RelayOff("IDO_3");

                var th = new Thread(() =>
                {
                    GetControlItemFromCSV();
                    GetCollectItemFromCSV();
                });
                th.Start();
            }
            else
            {
                MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
                e.Handled = true;
                isMESSkipCb.IsChecked = false;
            }
            //rp.Close();
        }

        private void MES스킵안함(object sender, RoutedEventArgs e)
        {
            if (MES.isMESConnected)
            {
                this.monoTb.Background = this.lotTb.Background = this.prodTb.Background = this.procTb.Background = this.equipTb.Background = Brushes.SkyBlue;
                isMESskip = false;
                LogState(LogType.Info, "MES Unskip");
                relays.RelayOn("IDO_3");
                isMESSkipCb.IsEnabled = false;

                var th = new Thread(() =>
                {
                    GetControlItemFromMES();
                    GetCollectItemFromMES();
                    this.Dispatcher.BeginInvoke(new Action(() =>
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
                relays.RelayOff("IDO_3");
                LogState(LogType.Fail, "Not Connected to MES");
            }
        }

        bool isSkipNG_ = false;

        //void ti_Tick(object sender, EventArgs e)
        //{
        //    elapsedtime = elapsedtime.AddMilliseconds(100);
        //    time_elapsed_tb.Text = elapsedtime.ToString("HH:mm:ss:f");
        //}
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
            // 20191208 Noah Choi PLC한테 마스터바코드를 받거나, 수동으로 마스터바코드를 입력할시 NGSkip 처리 진행
            if (isMasterBcr == true && masterTempLot == "MASTER" || plc.masterTempBcr == "MASTER")
            {
                isSkipNG.IsChecked = true;
                e.Handled = true;
                isSkipNG_ = true;
                LogState(LogType.Info, "NG Skip");
                return;
            }
            //else 
            //{
            //    isSkipNG.IsChecked = false;
            //    e.Handled = true;
            //    isSkipNG_ = false;
            //    return;
            //} //jgh 191213 조건수정

            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            LoginClickCheckBox();
            if (PermissionResult)
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

                MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
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
                AllThreadAbort();
                relays.Reset();
                this.LogState(LogType.Success, "User Closed");
            }
            catch (Exception ec)
            {
                this.LogState(LogType.Fail, "User Closed", ec);
            }

            Thread.Sleep(500);
            System.Environment.Exit(0);
        }

        // 20190312 jeonhj's comment
        // 윈도우 닫을 때 사용중인 모든 스레드를 닫는다.
        // 스레드는 더 확인해서 닫아야 함.
        // 다른 생성자들도 여기서 Close 할 수 있도록 해야 함.
        private void AllThreadAbort()
        {
            if (autoModeThread.IsAlive)
            {
                autoModeThread.Abort();
                autoModeThread = null;
            }

            if (finishedThread.IsAlive)
            {
                finishedThread.Abort();
                finishedThread = null;
            }

            if (GroupBtThread.IsAlive)
            {
                GroupBtThread.Abort();
                GroupBtThread = null;
            }

            if (SingleBtThread.IsAlive)
            {
                SingleBtThread.Abort();
                SingleBtThread = null;
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
            //20. Save Data 클릭 이벤트 발생 시 Module BCR text 저장
            //190104 by grchoi
            MonoFrame = monoTb.Text.ToString();
            SaveData();
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
            _stopwatch.Stop();
            _stopwatch.Reset();

            isStop = ispause = false;
            bt_pass.Content = "-";
            bt_pass.Background = Brushes.DarkGray;
            ProgressRefresh(0);
            this.time_start_tb.Text = this.time_finish_tb.Text = this.time_elapsed_tb.Text = "";
            this.bmsList.Clear();//ksw
            bt_save.Background = resetBt.Background;

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


            foreach (var item in modelList[selectedIndex].TestItemList)
            {
                item.Value.Value_ = null;
                item.Value.Result = "";
                item.Value.refValues_.Clear();
            }

            ClearCellDetailList();

            //CellDetailDg.ItemsSource = cellDetailList;

            chroma.Discard();

            // 2019.03.20 jeonhj's comment
            // 박진호 선임 요청사항
            // Reset 시 충방전기 FF 동작
            //var str = "0FF";

            //cycler.SendToDSP1(str, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            //LogState(LogType.Info, "Cycler Mode Set [" + str + "]");
            //Thread.Sleep(500);
            //if (cycler.cycler1voltage > 10)
            //{
            //    cycler.SendToDSP1(str, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            //    LogState(LogType.Info, "Cycler Mode Set [" + str + "]");
            //}
        }

        private void InspectionSpecClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            LoginClickCheckBox();
            if (PermissionResult)
            {
                var ins = new InspectionSpecWindow(this);
                ins.Show();
            }
            else
            {
                MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            //rp.Close();

        }

        //기본 오토상태
        private void manualBt_Click(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            LoginClickCheckBox();
            if (PermissionResult)
            {
                bt_pass.Background = Brushes.DarkGray;
                bt_pass.Content = "-";
                isManual = !isManual;
                ResetClick(this, null);
                // 211201 JKD - Manual Group Button 클릭시 착공 Flag On(ContackCheck 항목에서 착공 확인함)
                m_bStartJobResult = true;
                m_bIsStartJobFinish = true;

                if (isManual)
                {
                    //isSkipNG.IsEnabled = true;
                    labelA.Background = Brushes.Gray;
                    labelM.Background = Brushes.SkyBlue;

                    relayConBt.IsEnabled = dtcClearBt.IsEnabled = resetBt.IsEnabled = cyclerBt.IsEnabled = specBt.IsEnabled = true;
                    blinder.Visibility = Visibility.Collapsed;

                    //isMESSkipCb.IsChecked = true;
                    LogState(LogType.Info, "---Switched to Manual mode---");
                }
                else
                {
                    //isSkipNG.IsChecked = isSkipNG_ = false;
                    //isSkipNG.IsEnabled = false;
                    labelA.Background = Brushes.SkyBlue;
                    labelM.Background = Brushes.Gray;
                    relayConBt.IsEnabled = dtcClearBt.IsEnabled = resetBt.IsEnabled = cyclerBt.IsEnabled = specBt.IsEnabled = false;
                    blinder.Visibility = Visibility.Visible;

                    //isMESSkipCb.IsChecked = false;
                    LogState(LogType.Info, "---Switched to Auto mode---");
                }
            }
            else
            {
                //if (!rp.isESC)
                //{

                //    MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
                //}
            }

            relays.RelayOff("IDO_2");
            relays.RelayOn("IDO_1");
            //rp.Close();
        }

        private void SaveData()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                bt_save.Background = Brushes.Lime;

            }));

            this.Save();
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
            LogState(LogType.Info, "Stop Button Clicked");
            StopAuto();
        }


        private void PauseClick(object sender, RoutedEventArgs e)
        {
            LogState(LogType.Info, "Pause Button Clicked");
            ispause_ = !ispause_;
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            LoginClickCheckBox();
            if (PermissionResult)
            {
                LogState(LogType.Info, "Start Button Clicked");
                AutoMode();
            }
            else
            {
                MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);

            }
            //rp.Close();

        }

        private void CyclerSettingsClick(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            LoginClickCheckBox();
            if (PermissionResult)
            {
                mmw.savedCb.IsChecked = false;
                mmw.Show();
            }
            else
            {
                MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);

            }
            //rp.Close();
        }

        private void OpenResultClick(object sender, RoutedEventArgs e)
        {
            if (isLineFlag)
            {
                System.Diagnostics.Process.Start("explorer.exe", @"C:\Logs\Inspection_result");
            }
            else
            {
                System.Diagnostics.Process.Start("explorer.exe", @"D:\Logs\Inspection_result");
            }
        }

        private void OpenLogClick(object sender, RoutedEventArgs e)
        {
            if (isLineFlag)
            {
                System.Diagnostics.Process.Start("explorer.exe", @"C:\Users\Public\EOL_INSPECTION_LOG");
            }
            else
            {
                System.Diagnostics.Process.Start("explorer.exe", @"D:\EOL_INSPECTION_LOG");
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

            var item = this.modelList[selectedIndex].TestItemList.ToList()[testItemListDg.SelectedIndex];

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
            LoginClickCheckBox();
            if (PermissionResult)
            {
                RelayControllerWindow rc = new RelayControllerWindow(this);
                rc.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);

            }
            //rp.Close();
        }

        private void DTCClearClick(object sender, RoutedEventArgs e)
        {
            ClearDTC();
        }



        #endregion

        #region Logging / Save data
        Object obj = new Object();

        public void LogState(LogType lt, string str, Exception ec = null,
            bool isView = true, bool isSave = true, bool isVol = false, int index = 0, bool isTest = false, string testName = "")
        {
            var header = "";
            switch (lt)
            {
                case LogType.Info: header = "[I N F O],"; break;
                case LogType.Fail: header = "[F A I L],"; break;
                case LogType.Success: header = "[SUCCESS],"; break;
                case LogType.Pass: header = "[P A S S],"; break;
                case LogType.NG: header = "[N     G],"; break;
                case LogType.TEST: header = "[T E S T],"; break;
                case LogType.EMERGENCY: header = "[EMERGEN],"; break;
                case LogType.RESPONSE: header =  "[RESPNSE],"; break;
                case LogType.REQUEST: header =          "[REQUEST],"; break;
                case LogType.MANUALCONDITION: header = "[MANCOND],"; break;
                case LogType.PLC_RECV: header = "[PLCRECV],"; break;
                case LogType.PLC_SEND: header = "[PLCSEND],"; break;
                case LogType.CAN: header = ""; isView = false; break;
            }

            str = str.Replace("\n", "");
            StringBuilder sb = new StringBuilder();
            sb.Append(System.DateTime.Now.ToString("HH:mm:ss:fff,"));
            sb.Append(header);
            sb.Append(str);
            sb.Append(ec != null ? (" - " + ec.Message + " : " + ec.StackTrace + ",") : ",");
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
                            };break;
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
                            };break;
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
                            };break;
                    }
                    

                }));
            }

            var load = logaddr + "\\" + System.DateTime.Now.ToString("yyyyMMdd");

            if (!Directory.Exists(load))
            {
                Directory.CreateDirectory(load);
            }

            if (isSave)
            {
                var tBarcode = Barcode.Replace(":", "_").ToString().Replace(";", "_").ToString().Replace("\r\n", "").ToString();
                try
                {


                    if (lt == LogType.CAN)
                    {
                        //CAN 로그(충방전시에 로그)는 별도폴더에 저장된다.
                        if (!Directory.Exists(load + "\\CAN"))
                        {
                            Directory.CreateDirectory(load + "\\CAN");
                        }

                        lock (obj)
                        {
                            File.AppendAllText(load + "\\CAN\\" + tBarcode + "_" + System.DateTime.Now.ToString("HH") + ".txt", sb.ToString() + "\r\n", Encoding.UTF8);
                        }

                        return;
                    }
                    if (isVol)
                    {
                        lock (obj)
                        {
                            File.AppendAllText(load + "\\" + Barcode + "_" + System.DateTime.Now.ToString("HH") + "_Cycler" + index.ToString() + ".txt", sb.ToString() + "\r\n", Encoding.UTF8);

                        }
                    }
                    else
                    {
                        if (isTest)
                        {
                            lock (obj)
                            {
                                File.AppendAllText(load + "\\" + Barcode + "_" + testName + "_" + System.DateTime.Now.ToString("HH") + ".txt", sb.ToString() + "\r\n", Encoding.UTF8);

                            }
                        }
                        else
                        {
                            lock (obj)
                            {
                                File.AppendAllText(load + "\\" + Barcode + "_" + System.DateTime.Now.ToString("HH") + ".txt", sb.ToString() + "\r\n", Encoding.UTF8);
                                File.AppendAllText(load + "\\AllDay_" + System.DateTime.Now.ToString("yyyyMMdd") + ".txt", sb.ToString() + "\r\n", Encoding.UTF8);
                            }
                        }
                    }

                    //lock (obj)
                    //{

                    //    File.AppendAllText(load + "\\" + tBarcode + "_" + System.DateTime.Now.ToString("HH") + ".txt", sb.ToString() + "\r\n", Encoding.UTF8);
                    //}
                }
                catch (Exception ecs)
                {
                    tBarcode = "NOT_POSSIBLE_FILE_NAME";
                    File.AppendAllText(load + "\\" + tBarcode + "_" + System.DateTime.Now.ToString("HH") + ".txt", sb.ToString() + "\r\n", Encoding.UTF8);                    
                }

            }
        }
        /// <summary>
        /// 23.  Save() 함수 요청반영사항
        ///      -> 상세수집항목 저장 정렬구문 추가(헤더/내용 모두)
        ///      -> 저장항목 define(LOT id, Module BCR, Device STAT 등 코드 헤더 및 저장내용 참조)
        ///      -> 바코드 항목 예외구문 추가
        ///      -> 착공 NG 시 NG 처리구문 추가
        /// </summary>
        private void Save()
        {
            string @LogDirectory = FILE_PATH_INSPECTION_RESULT + "\\" + this.modelList[selectedIndex].ModelId;            

            if (!Directory.Exists(@LogDirectory))
            {
                Directory.CreateDirectory(@LogDirectory);
            }

            //MES가 연결되어있고, 자동모드스레드가 살아있고, 검사항목이 모두 비어있을 때
            //정상 검사 상태일 때 착공NG가 난다면, 해당 루틴에 따라 첫 항목을 MES_StartJob NG로 기입한다.
            #region MES_StartJob NG 입력

            var isEmptyItems = true;
            foreach (var item in this.modelList[selectedIndex].TestItemList)
            {
                if (item.Key == _BARCODE)
                {
                    continue;
                }
                
                if (item.Value != null && item.Value.Value_ != null)
                {
                    isEmptyItems = false;
                }
            }

            if (autoModeThread != null && autoModeThread.IsAlive && !isMESskip && isEmptyItems)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                     this.modelList[selectedIndex].TestItemList.ToList()[0].Value.Value_ = lotTb.Text;
                }));

                if (isStop)
                {
                    this.modelList[selectedIndex].TestItemList.ToList()[1].Value.Value_ = _EMG_STOPPED;
                }
                else
                {
                    this.modelList[selectedIndex].TestItemList.ToList()[1].Value.Value_ = "MES_StartJob NG";
                }
                this.modelList[selectedIndex].TestItemList.ToList()[1].Value.Result = "NG";
            }

            #endregion

            //190104 by grchoi
            List<DetailItems> dlist = new List<DetailItems>();  //20181224 wjs detail order
            foreach (var item in this.modelList[selectedIndex].TestItemList)
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
                var header = "PASS FAIL,STATION,LOT_ID,MODULE_BCR,IS_MES_SKIP,DEVICE_STAT,";
                foreach (var item in this.modelList[selectedIndex].TestItemList)
                {
                    header += item.Key + ",";
                }

                header += "StartTime,ElapsedTime,";

                //190104 by grchoi
                foreach (var item in orderedList) header += (item as DetailItems).Key + ",";

                header.Remove(header.Length - 1, 1);
                File.AppendAllText(dir, header + "\r\n", Encoding.UTF8);
            }

            var first = "PASS";

            foreach (var item in this.modelList[selectedIndex].TestItemList)
            {
                //190104 by grchoi
                if (item.Key.Replace(" ", "") == _BARCODE) continue;
                if (item.Value.Result != null && item.Value.Result != "PASS") first = "NG";
            }

            var tail = first + ",";

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0},", position));
            sb.Append(Barcode);
            sb.Append(",");

            //190104 by grchoi
            sb.Append(string.Format("{0},", MonoFrame));
            sb.Append(string.Format("{0},", isMESskip == true ? "YES" : "NO"));
            if (deviceStatus == "") sb.Append(string.Format("{0},", "OK"));
            else sb.Append(string.Format("{0},", deviceStatus));     
            //sb.Append(position);
            //sb.Append(",");

            foreach (var item in this.modelList[selectedIndex].TestItemList)
            {
                sb.Append(string.Format("{0}", item.Value.Value_));
                sb.Append(",");
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                sb.Append(this.time_start_tb.Text);
                sb.Append(",");
                sb.Append(this.time_elapsed_tb.Text);
                sb.Append(",");
            }));

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
            sb.Remove(sb.Length - 1, 1);
            //sb.Append(preRes900.ToString());
            //sb.Append(",");
            
            //190104 by grchoi
            try { File.AppendAllText(dir, tail + sb.ToString() + "\r\n", Encoding.UTF8); }
            catch (Exception)
            {
                this.LogState(LogType.Fail, "Save Data - File is open");
                File.AppendAllText(subdir, tail + sb.ToString() + "\r\n", Encoding.UTF8);
            }

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.LogState(LogType.Info, "Save Data");
            }));
        }

        #endregion

        /// <summary>
        /// 항목이 진행중임을 표시하는 메서드.
        /// 일시정지일때, 해당부분에 일시정지 무한루프로 걸린다.
        /// </summary>
        /// <param name="ti"></param>
        public void isProcessingUI(TestItem ti)
        {
            // 211206 JKD - 검사기 EMO Switch pushedEmo Flag 추가 
            if (pushedEmo)
            {
                LogState(LogType.Info, "Pushed emergency switch.");
            }

            //24. 수동검사로그저장을위해 해당함수에서 lotTb의 스트링 저장구문 추가
            //190104 by grchoi
            this.Dispatcher.Invoke(new Action(() =>
            {
                modelList[selectedIndex].LotId = Barcode = this.lotTb.Text;
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
                    blinder3.Visibility = blinder2.Visibility = blinder.Visibility = System.Windows.Visibility.Visible;
            }));

            isProcessingFlag = true;
            AutoScrolling(ti);
            //relays.Reset();

            LogState(LogType.Info, "-----------------------------------------------------------------", null, false);
            LogState(LogType.Info, "Test :" + ti.Name + " Start");

            SetYellow(ti.Bt);

            relays.RelayOff("IDO_1");
            relays.RelayOn("IDO_2");


        }

        private void AutoScrolling(TestItem testItem)
        {
            if (!isAutoScr)
                return;

            var index = GetIndex(this.modelList[selectedIndex].TestItemList, testItem.Name);
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

            timetick.Stop();
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
                            pcw.Close();
                        }
                    }));
                }

            }
        }
        

        private void isdummy(object sender, RoutedEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            LoginClickCheckBox();
            if (PermissionResult)
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

        //private void isC727MES(object sender, RoutedEventArgs e)
        //{
        //    isC727MESOk = true;
        //    LogState(LogType.Info, "isC727MES OK Check!! C727 Use Local");
        //    GetControlItemFromCSV();
        //    SetCyclerStepToMESData(0);
            
        //}

        //private void isNotC727MES(object sender, RoutedEventArgs e)
        //{
        //    isC727MESOk = false;
        //    LogState(LogType.Info, "isC727MES OK Uncheck!! C727 Use Local");
        //}

        bool isDummy = false;

        private void CellDetailDg_LayoutUpdated_1(object sender, EventArgs e)
        {
            if (localTypes == 0 || localTypes == 1)
            {
                CellDetailDg.Columns[9].Visibility = Visibility.Visible;
                CellDetailDg.Columns[10].Visibility =
                    CellDetailDg.Columns[11].Visibility = Visibility.Collapsed;
            }
            else if (localTypes == 2)
            {
                CellDetailDg.Columns[9].Visibility =
                CellDetailDg.Columns[10].Visibility =
                    CellDetailDg.Columns[11].Visibility = Visibility.Collapsed;
            }
            else if (localTypes == 3)
            {
                CellDetailDg.Columns[9].Visibility = Visibility.Visible;
                CellDetailDg.Columns[10].Visibility =
                    CellDetailDg.Columns[11].Visibility = Visibility.Collapsed;
            }
            else if (localTypes == 4 || localTypes == 5)
            {
                CellDetailDg.Columns[9].Visibility =
                CellDetailDg.Columns[10].Visibility =
                    CellDetailDg.Columns[11].Visibility = Visibility.Visible;
            }
        }

        private void DaqSpecClick(object sender, RoutedEventArgs e)
        {
            var ins = new DAQWindow(this);
            ins.Show();
        }

        //nnkim 크로마 모델 타입설정
        private void LoadChromaModelType()
        {
            //크로마 모델 타입 
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            var regMCResistanceDelay = rk.GetValue("Chroma_Model_Type") as string;

            if (regMCResistanceDelay == null) { rk.SetValue("Chroma_Model_Type", "OLD"); strChromaModelType = "OLD"; }
            else { strChromaModelType = regMCResistanceDelay; }
        }

        //소모품 카운트 기능 nnkim
        private void btnOpenCntSetting(object sender, RoutedEventArgs e)
        {
            int nModeCheck = (int)blinder.Visibility;

            //메뉴얼 모드 일 경우에만 
            if (nModeCheck == 2)
            {

                //221012 스페어파츠카운트 설정하는 부분
                FormPartsCountSetting newFormPartsCountSetting = new FormPartsCountSetting(this);
                newFormPartsCountSetting.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                if (position == "#1") { newFormPartsCountSetting.Location = new System.Drawing.Point(700, 200); }
                else { newFormPartsCountSetting.Location = new System.Drawing.Point(50, 200); }

                newFormPartsCountSetting.ShowDialog();
                
            }
            else
            {

            }
        }

        //소모품 카운트 기능 nnkim
        public void SetPartsCountData(TestItem ti = null, string strRun = null)
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
            
                    lbHipotPin.Content = string.Format("HI-POT PIN {0} / {1}", PCS.nRealData_ResultHp, PCS.nLimitData_ResultHp);
                    if (PCS.nRealData_ResultHp > PCS.nLimitData_ResultHp) lbHipotPin.Foreground = Brushes.Red;
                    else lbHipotPin.Background = Brushes.Green;
            
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
                    ti.refValues_.Add(MakeDetailItem("HIPOTPINCOUNT", PCS.nRealData_ResultHp.ToString()));
                }
            }
            catch
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
                //221121
                System.Windows.MessageBox.Show("Please follow along and check. \nCMD -> Intl.cpl -> Please change the format to US.");
                System.Environment.Exit(0);
            }
        }

        private void OpenPortUI(object sender, RoutedEventArgs e)
        {
            //221202 nnkim
            //////221030 
            FormPortNumber newFormPortNumber = new FormPortNumber(this);

            if (position == "#1") { newFormPortNumber.Location = new System.Drawing.Point(50, 100); }
            else if (position == "#2") { newFormPortNumber.Location = new System.Drawing.Point(700, 100); }
            else if (position == "#3") { newFormPortNumber.Location = new System.Drawing.Point(50, 1600); }
            else if (position == "#4") { newFormPortNumber.Location = new System.Drawing.Point(700, 600); }
            else { }
        }

        //221123
        public void PreCheck_With_LV()
        {
            //NG 일 경우 다시 검사 부분
            //PLC 리트라이 ON 해주고 재 커넥터 후진 후 전진
            LV_RetryTest();
            //검사기 쪽에서 처음부터 다시 검사.
           
            return;
        }

        public bool LV_RetryTest()
        {
            LogState(LogType.Info, "RetryTest Start---------------------------------------------");

            if (!plc.isAlive)
            {
                LogState(LogType.Info, "RetryTest END---------------------------------------------");
                return false;
            }

            try
            {
                if (!plc.LV_Retry_Flag)
                {
                    //retry singal On
                    plc.SendLV_Retry(true);
                    //inspection off
                    plc.SendInspectionStartOnOff(false);

                    int cnt = 0;
                    while (true)
                    {
                        Thread.Sleep(100);
                        cnt += 100;//2s

                        if (cnt > 20000) // ks yoo 220720
                        {
                            strlvRetryCount = "RETRY_NOT_ACTIVE";
                            LogState(LogType.Fail, "Not Activated in 10sec");
                            plc.SendLV_Retry(false);
                            LogState(LogType.Info, "RetryTest END---------------------------------------------");
                            return false;
                        }

                        if (isStop || ispause)
                        {
                            plc.SendLV_Retry(false);
                            strlvRetryCount = _EMG_STOPPED;
                            LogState(LogType.Info, "RetryTest END---------------------------------------------");
                            return false;
                        }

                        if (plc.plc_lv_retry_recv_ok)
                        {
                            LogState(LogType.Info, "Success To RetryTest Flag in Time out 10sec");
                            plc.SendLV_Retry(false);
                            plc.SendInspectionStartOnOff(true);
                            LogState(LogType.Info, "RetryTest END---------------------------------------------");
                            return true;
                        }
                    }
                }
                else
                {
                    LogState(LogType.Info, "RetryTest END---------------------------------------------");
                    return false;
                }

            }
            catch (Exception ec)
            {
                LogState(LogType.Info, "RetryTest", ec);
                LogState(LogType.Info, "RetryTest END---------------------------------------------");
                return false;
            }
        }

        //221201
        private void LoginClickCheckBox()
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
    }
}
