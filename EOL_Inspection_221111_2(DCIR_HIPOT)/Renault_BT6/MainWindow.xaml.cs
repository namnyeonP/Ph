
using Renault_BT6.모듈;
using Renault_BT6.윈도우;
using Renault_BT6.클래스;
using Microsoft.Win32;
using Peak.Can.Basic;
using System;
using System.Collections.Generic;
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

using PhoenixonLibrary.Device;
using PhoenixonLibrary.ETC;
using PhoenixonLibrary;
using PhoenixonLibrary.Cycler;
using Renault_BT6.Module;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Media.Imaging;

using Renault_BT6.Forms;
using System.Globalization;

namespace Renault_BT6
{

    #region Enum

    public enum DSPType
    {
        DSP_28335 = 0,
        DSP_28377 = 1,
        DSP_28377_SDI = 2
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

        /// <summary>
        /// 각종 윈도우 및 모드 변경시 필요 비번
        /// </summary>
        //public string passWord = "";
        public string passWord = "1212";    //200706 wjs add password 1212


        //static string FILE_PATH_INSPECTION_RESULT = "c:\\Logs\\Inspection_result\\";

        /// <summary>
        /// 자동시작시에 흐르는 시간변수
        /// </summary>
        DateTime nowTime;
        Stopwatch _stopwatch = new Stopwatch();

        public Thread autoModeThread;
        public List<MiniScheduler.Process> totalProcessList = new List<MiniScheduler.Process>();

        /// <summary>
        /// 충방전을 위한 스케줄러 윈도우
        /// </summary>
        MiniScheduler.MainWindow mmw { get; set; }

        public List<Models> modelList = new List<Models>();

        public string _Barcode { get; set; }

        public int selectedIndex = -1;

        public bool isNeedDCIR = true;

        public bool IsMESskip  { get; set; }

        public bool IsEmgStop  { get; set; }
        public bool IsManual  { get; set; }

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

        public const string _BarCodeName = "BarcodeReading";

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        Dictionary<string, List<TestItem>> groupDic = null;//;new Dictionary<string, List<TestItem>>();

        public JsonParser _JsonSetting = new JsonParser();
        public DCIR_Process _DCIR = new DCIR_Process();


        public CCycler cycler;
        public CLambdaPower becm_power;
        public CLambdaPower pump_power;
        public CBMS_CAN Hybrid_Instru_CAN;

        System.Windows.Forms.Timer timetick = new System.Windows.Forms.Timer();
        DispatcherTimer _timerUI = new DispatcherTimer();    //객체생성
                                                             //타이머 시작. 종료는 timer.Stop(); 으로 한다

        public int counter_Cycler = 0;
        public int counter_Cycler_limit = 50000;

        //190104 by grchoi
        string deviceStatus  { get; set; }

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
        public static string _GFI_FAULT = "GFI_FAULT";
        public static string _CONT_CLOSE_STUCK = "CONTACTOR_CLOSE_STUCK";
        public static string _LOW_LIMIT = "LOW_LIMIT";
        public static string _HI_LIMIT = "HI_LIMIT";

        public static string _DC_IR_TRIPPED = "DC_IR_TRIPPED";
        public static string _DC_ARC = "DC_ARC";
        public static string _DC_IO = "DC_IO";
        public static string _DC_ADV_OVER = "DC_ADV_OVER";
        public static string _DC_ADI_OVER = "_DC_ADI_OVER";
        public static string _DC_IO_F = "DC_IO_F";
        public static string _DC_IR_GR_CONT = "DC_IR_GR_CONT";

        public static string _NULL = "NULL";
        public static string _VOLTAGE_PIN_SENSING_FAIL = "VOLTAGE_PIN_SENSING_FAIL";
        public static string _SENSING_BOARD_CHECK = "SENSING_BOARD_CHECK";


        public static string groupName = "";
        public static int groupNo = 0;

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
                if (IsManual)
                {
                   blinder3.Visibility = blinder.Visibility = Visibility.Hidden;
                }
            }));

            isProcessingFlag = false;
            double itemVal = 0;
            
            //2. 검사항목 중 바코드 판정 시 All pass 적용
            if (ti.Name == _BarCodeName)
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

            if (IsEmgStop)
            {
                ti.Value_ = _EMG_STOPPED;
            }

            //3.검사 판정 시 EMG인 경우 NG로 강제 적용(절연저항 측정 / DCIR 측정 등 인경우)
            if (ti.Value_ == null
                || ti.Value_.ToString() == ""
                || ti.Value_.ToString() == _NULL

                || ti.Value_.ToString() == Judge.CAN_NOT_TEST.ToString()
                || ti.Value_.ToString() == Judge.CHANNEL_NG.ToString()
                || ti.Value_.ToString() == Judge.DC_ADI_OVER.ToString()
                || ti.Value_.ToString() == Judge.DC_ADV_OVER.ToString()
                || ti.Value_.ToString() == Judge.DC_ARC.ToString()
                || ti.Value_.ToString() == Judge.DC_CHECK_LOW.ToString()
                || ti.Value_.ToString() == Judge.DC_HI.ToString()
                || ti.Value_.ToString() == Judge.DC_IO.ToString()
                || ti.Value_.ToString() == Judge.DC_IO_F.ToString()
                || ti.Value_.ToString() == Judge.DC_IR_GR_CONT.ToString()
                || ti.Value_.ToString() == Judge.DC_IR_TRIPPED.ToString()
                || ti.Value_.ToString() == Judge.DC_LO.ToString()
                || ti.Value_.ToString() == Judge.DISCONNECT.ToString()
                || ti.Value_.ToString() == Judge.EMERGENCY.ToString()
                || ti.Value_.ToString() == Judge.IR_ADI_OVER.ToString()
                || ti.Value_.ToString() == Judge.IR_ADV_OVER.ToString()
                || ti.Value_.ToString() == Judge.IR_HI.ToString()
                || ti.Value_.ToString() == Judge.IR_IO.ToString()
                || ti.Value_.ToString() == Judge.IR_LO.ToString()
                || ti.Value_.ToString() == Judge.NULL.ToString()
                || ti.Value_.ToString() == Judge.STOP.ToString()
                || ti.Value_.ToString() == Judge.USER_STOP.ToString()

                || ti.Value_.ToString() == _CONT_CLOSE_STUCK
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
                || ti.Value_.ToString() == _NOT_MODE_CHANGED)
            {
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
        public int localTypes { get; set; }


        private void sideChangedUp(object sender, MouseButtonEventArgs e)
        {
            var lb = sender as Label;

            this.ChangeUIModes(lb.Name.ToString());

            this.LoadList();

            //190124 ht MES 스킵되있을때는 스펙받아오지 아니한다
            if (!IsMESskip)
            {
                GetControlItemFromMES();
                GetCollectItemFromMES();
            }
            else
            {
                GetControlItemFromCSV();
                GetCollectItemFromCSV();
            }

            SetCyclerStepToMESData();  

            this.testItemListDg.Items.Refresh();
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

            switch (initLbName)
            {
                case "type1lb":
                    type1lb.Background = Brushes.SkyBlue;
                    localTypes = 0;
                    LogState(LogType.Info, "BT6 Mode");

                    this.prodTb.Text = CONFIG.MesProductCode;
                    this.EquipIDTb.Text = CONFIG.MesEquipID;

                    modelList[selectedIndex].ProductCode = CONFIG.MesProductCode;
                    modelList[selectedIndex].EquipId = CONFIG.MesEquipID;
                    modelList[selectedIndex].ProcessID = CONFIG.MesProcessID;
                    modelList[selectedIndex].ModelId = MODEL_NAME;
                    break;
            }
        }

        public void ChangeUIModes(string mode_name)
        {
            switch (mode_name)
            {
                case "type1lb":
                    type1lb.Background = Brushes.SkyBlue;
                    localTypes = 0;
                    LogState(LogType.Info, "BT6  Mode");
                    this.prodTb.Text = CONFIG.MesProductCode;
                    this.EquipIDTb.Text = CONFIG.MesEquipID;
                    modelList[selectedIndex].ProductCode = CONFIG.MesProductCode;
                    modelList[selectedIndex].ModelId = MODEL_NAME + "_2P12S";
                    break;

            }
        }

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

        /// <summary>
        /// 로그로 저장될 폴더명
        /// </summary>
        string MODEL_NAME = "BT6";

        /// <summary>
        /// Cycler DSP Types
        /// </summary>
        public DSPType dsp_Type = DSPType.DSP_28377;

        bool isNeedOCVFlag = false;

        public MainWindow()
        {
            InitializeComponent();

            //언어 인터락 nnkim
            WindowLanguageFormatCheck();

            // moons json file load 
            JsonLoadSet();

            LoadINI(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            InitializeCommonMethods();
            InitializeSaveFileAddr();
            Counter_Cycler();
            
            //소모품 카운트 기능 nnkim
            ShowInspectionUIChange((int)CONFIG.EolInspType);
            SetPartsCountData();

        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //빌드 시간 타이틀 적용
            SetTextOnForm();
			
            DeviceConnect();

            SetUI();

            SetUI_EOL_HIPOT();

            //190104 by grchoi
            //4. 초기화 시 ChangeUIModes 함수 보완을 위해 Model ID 등의 입력 정보를 위해 추가
            //   함수 적용 : ChangeUIModes_Init
            ChangeUIModes_Init();

            SetWindow();

            LoadList();
            
            GetControlItemFromCSV();
            GetCollectItemFromCSV();

            // add 20200107
            SetCyclerStepToMESData();

            MesDetailListIDAdd();

            _timerUI.Start();
        }

        public void GetMesData()
        {
            try
            {
                Thread.Sleep(100);

                if (Device.MES.IsMESConnected)
                {
                    GetControlItemFromMES();
                    GetCollectItemFromMES();
                }
            }
            catch
            {

            }
         
        }

        private void MesDetailListIDAdd()
        {
            try
            {
                EOL.MesID.BushingMinusIdAdd();
                EOL.MesID.CoolingMinusIdAdd();

                EOL.MesID.BushingPlusIdAdd();
                EOL.MesID.CoolingPlusIdAdd();
            }
            catch(Exception ex)
            {
                LogState(LogType.Fail, ex.StackTrace);
            }
        }


        public void JsonSave()
        {
            try
            {
                _JsonSetting.JsonWrite(JsonType.MES, "PRODUCT", modelList[selectedIndex].ProductCode);
                _JsonSetting.JsonWrite(JsonType.MES, "EQUIP", modelList[selectedIndex].EquipId);
                _JsonSetting.JsonWrite(JsonType.MES, "PROCESS", modelList[selectedIndex].ProcessID);
                _JsonSetting.JsonWrite(JsonType.MES, "USERID", modelList[selectedIndex].UserId);
            }
            catch (Exception ex)
            {

            }
        }

        // moons config.json file load Set
        public void JsonLoadSet()
        {
            try
            {
                // setting json file load
              bool ret =   _JsonSetting.JsonFileLoad(PATH.JSON_SET);

                // Relay
                RELAY.LIGHT_RED = _JsonSetting.JsonRead(JsonType.RELAY, "RED");
                RELAY.LIGHT_GREEN = _JsonSetting.JsonRead(JsonType.RELAY, "GREEN");
                RELAY.LIGHT_YELLOW = _JsonSetting.JsonRead(JsonType.RELAY, "YELLOW");
                RELAY.LIGHT_BLUE = _JsonSetting.JsonRead(JsonType.RELAY, "BLUE");

                // Inspection Tpye
                string type = _JsonSetting.JsonRead(JsonType.CONFIG, "EOL_TYPE");
                switch (type)
                {
                    case "EOL":
                        CONFIG.EolInspType = InspectionType.EOL;
                        break;
                    case "HIPOT":
                        CONFIG.EolInspType = InspectionType.HIPOT;
                        break;
                    default:
                        CONFIG.EolInspType = InspectionType.EOL;
                        break;
                }

                // Device
                CONFIG.Cycler = _JsonSetting.JsonRead(JsonType.CONFIG, "DSP");
                CONFIG.CHROMA = _JsonSetting.JsonRead(JsonType.CONFIG, "Chroma");
                CONFIG.PhDAQ = _JsonSetting.JsonRead(JsonType.CONFIG, "DAQ");
                CONFIG.KeysightDAQ = _JsonSetting.JsonRead(JsonType.CONFIG, "DMM");
                CONFIG.TEMP = _JsonSetting.JsonRead(JsonType.CONFIG, "TEMP");

                // PLC
                CONFIG.PlcNumber = _JsonSetting.JsonRead(JsonType.PLC, "Number");
                CONFIG.SendAddr = _JsonSetting.JsonRead(JsonType.PLC, "SEND");
                CONFIG.RecvAddr = _JsonSetting.JsonRead(JsonType.PLC, "RECV");
                CONFIG.BcrAddr = _JsonSetting.JsonRead(JsonType.PLC, "BCR");

                // MES
                CONFIG.MesIP = _JsonSetting.JsonRead(JsonType.MES, "IP");
                CONFIG.MesPort = _JsonSetting.JsonRead(JsonType.MES, "PORT");
                CONFIG.MesEquipID = _JsonSetting.JsonRead(JsonType.MES, "EQUIP");
                CONFIG.MesProcessID = _JsonSetting.JsonRead(JsonType.MES, "PROCESS");
                CONFIG.MesProductCode = _JsonSetting.JsonRead(JsonType.MES, "PRODUCT");
                CONFIG.MesUserID = _JsonSetting.JsonRead(JsonType.MES, "USERID");

                LogState(LogType.Info, "JsonLoadSet Success !!");
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.StackTrace);
                LogState(LogType.Fail, ex.Message);
            }
        }




        /// <summary>
        /// moons 빌드날짜가 타이틀에 표기 
        /// </summary>
        public void SetTextOnForm()
        {
            try
            {
                Version assVer = Assembly.GetExecutingAssembly().GetName().Version;

                DateTime buildDate = new DateTime(2000, 1, 1).AddDays(assVer.Build).AddSeconds(assVer.Revision * 2);

                // 최종 수정 날짜가 컴파일 시 자동으로 업데이트 됨.
                mainTitle.Title = string.Format("Phoenixon {0} Inspection {1}" + " [221111_2]",
                                                            CONFIG.EolInspType.ToString(),
                                                            position/*, buildDate.ToString()*/);
            }
            catch
            {

            }
        }

        private void SetWindow()
        {
            this.Width = SystemParameters.WorkArea.Width;
            this.Height = SystemParameters.WorkArea.Height;
            this.Top = 0;
            this.Left = 0;

            //221111 nnkim
            //CONFIG.EolInspType = InspectionType.HIPOT;

            //if (this.pro_Type == ProgramType.HipotInspector)
            //{
            //    this.monoTb.FontSize -= 1;
            //    this.lotTb.FontSize -= 1;
            //    switch (position)
            //    {
            //        case "#1":
            //            {
            //                this.Width = SystemParameters.WorkArea.Width / 2;
            //                this.Height = SystemParameters.WorkArea.Height / 2;
            //                this.Top = 0;
            //                this.Left = 0;
            //            }; break;
            //        case "#2":
            //            {

            //                this.Width = SystemParameters.WorkArea.Width / 2;
            //                this.Height = SystemParameters.WorkArea.Height / 2;
            //                this.Top = 0;
            //                this.Left = SystemParameters.WorkArea.Width / 2;
            //            }; break;
            //        case "#3":
            //            {
            //                this.Width = SystemParameters.WorkArea.Width / 2;
            //                this.Height = SystemParameters.WorkArea.Height / 2;
            //                this.Top = SystemParameters.WorkArea.Height / 2;
            //                this.Left = 0;
            //            }; break;
            //        case "#4":
            //            {
            //                this.Width = SystemParameters.WorkArea.Width / 2;
            //                this.Height = SystemParameters.WorkArea.Height / 2;
            //                this.Top = SystemParameters.WorkArea.Height / 2;
            //                this.Left = SystemParameters.WorkArea.Width / 2;
            //            }; break;
            //    }

            //}
            //else if (this.pro_Type == ProgramType.VoltageInspector)
            //{
            //    this.monoTb.FontSize -= 1;
            //    this.lotTb.FontSize -= 1;
            //    this.WindowState = WindowState.Maximized;
            //}
            //else
            //{
            //    this.WindowState = WindowState.Maximized;
            //}
        }
        public void MesInit()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                {
                    MESConnect();
                }), null);

            }
            catch
            {

            }
        }

        public void PLCInit()
        {
            try
            {
                string sendAddr = CONFIG.SendAddr;
                string recvAddr = CONFIG.RecvAddr;
                string bcrAddr = CONFIG.BcrAddr;

                new Thread(() =>
                {
                    Device.PLC = new CPLC(this, recvAddr,
                                                        sendAddr,
                                                        bcrAddr,
                                                        Convert.ToInt16(CONFIG.PlcNumber));
                }).Start();
            }
            catch
            {

            }
        }

        public void RelayInit()
        {
            try
            {
                Device.Relay = new RelayControl();

                Device.Relay.Open();

                Device.Relay.On(RELAY.LIGHT_YELLOW);
            }
            catch
            {

            }
        }

        



        /// <summary>
        ///  상용 DAQ 34970A
        /// </summary>
        public void KeysightDMMInit()
        {
            try
            {
                //ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state)
                //{
                Device.KeysightDAQ = new CKeysightDMM(this, true, CONFIG.KeysightDAQ);
                       //keysight = new CKeysightDMM(this, true, keysight_PortName);
                //Device.KeysightDAQ.EventWriteLog += KeysightDAQ_EventWriteLog;
                //Device.KeysightDAQ.Open();
                //}), null);
            }
            catch
            {

            }
        }

        private void KeysightDAQ_EventWriteLog(object sender, EventArgs e)
        {
            try
            {
                string msg = sender as string;

                LogState(LogType.Info, msg);
            }
            catch
            {

            }
        }

        public void ChromaInit()
        {
            try
            {
                // ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                // {
                    Device.Chroma = new Chroma_19053(CONFIG.CHROMA);
                    Device.Chroma.EventWriteLog += Chroma_EventWriteLog;

                    Device.Chroma.Open();
                    Device.Chroma.SendDelay = 120;    // myunghwan choi, 210815, a) 220 -> 120
               //}), null);
            }
            catch
            {

            }
        }

        private void Chroma_EventWriteLog(object sender, EventArgs e)
        {
            try
            {
                string msg = sender as string;
                
                LogState(LogType.Info, msg);
            }
            catch
            {

            }
        }


        // moons 
        public void CyclerInit(bool logOnlyFlag)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                {

                    if (logOnlyFlag == true)  // 로그만 사용
                    {
                        Device.Cycler = new PhoenixonCycler(true);
                        // Log Event
                        Device.Cycler._EventWriteLog += Cycler__EventWriteLog;
                     }
                    else
                    {
                        Device.Cycler = new PhoenixonCycler();

                        // Cycler Alarm
                        Device.Cycler.ChgDisControl.EventChgDisError += _chgDisControl__EventChgDisError1;
                        Device.Cycler.ChgDisControl.EventChgDisSend += _chgDisControl_EventChgDisSend;
                        Device.Cycler.ChgDisControl._EventWriteLog += ChgDisControl__EventWriteLog;

                        // Step Alarm
                        Device.Cycler._EventStepError += Cycler__EventStepError;

                        // state data Event
                        Device.Cycler._EventState += Cycler__EventState;

                        // Get Point ChgDis Data
                        Device.Cycler._EventGetDataPoint += Cycler__EventGetDataPoint;

                        // Log Event
                        Device.Cycler._EventWriteLog += Cycler__EventWriteLog;

                        // PCAN Open
                        if (CONFIG.Cycler == "1")
                            Device.Cycler.PcanOpen(PCAN_TYPE.USB_BUS1);
                        else if (CONFIG.Cycler == "2")
                            Device.Cycler.PcanOpen(PCAN_TYPE.USB_BUS2);

                        // 피닉슨 DAQ 
                        int cellCount = 98;  // "(12 * 4) + 2 = 50"
                        Device.Cycler.PhDaqNew(CONFIG.PhDAQ, cellCount);
                        Device.Cycler.DAQOpen();
                    }
                 }), null);
            }
            catch
            {

            }
        }

        private void Cycler__EventWriteLog(object sender, EventArgs e)
        {
            try
            {
                string msg  = sender as string;

                LogState(LogType.Info, msg);
            }
            catch
            {

            }
        }

        private void ChgDisControl__EventWriteLog(object sender, EventArgs e)
        {
            try
            {
                string msg = sender as string;

                LogState(LogType.Info, msg);
            }
            catch
            {

            }
        }

        
        private void Cycler__EventGetDataPoint(object sender, EventArgs e)
        {
            try
            {
                List<double> PointCount = sender as List<double>;

                int listcnt =  Device.Cycler.GetDisChgPointList.Count -1;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    cellDetailList[listcnt].CellVolt_1 = PointCount[0];
                    cellDetailList[listcnt].CellVolt_2 = PointCount[1];
                    cellDetailList[listcnt].CellVolt_3 = PointCount[2];
                    cellDetailList[listcnt].CellVolt_4 = PointCount[3];

                    cellDetailList[listcnt].CellVolt_5 = PointCount[4];
                    cellDetailList[listcnt].CellVolt_6 = PointCount[5];
                    cellDetailList[listcnt].CellVolt_7 = PointCount[6];
                    cellDetailList[listcnt].CellVolt_8 = PointCount[7];
                    cellDetailList[listcnt].CellVolt_9 = PointCount[8];
                    cellDetailList[listcnt].CellVolt_10 = PointCount[9];
                    cellDetailList[listcnt].CellVolt_11 = PointCount[10];
                    cellDetailList[listcnt].CellVolt_12 = PointCount[11];

                    //cellDetailList[listcnt].ModuleVolt = Device.Cycler._Voltage;
                    cellDetailList[listcnt].ModuleVolt = Device.Cycler.GetDisChgPointList[listcnt].Vlot;
                }));
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.Message);
                LogState(LogType.Fail, ex.StackTrace);
            }
        }

        private void _chgDisControl_EventChgDisSend(object sender, EventArgs e)
        {
            try
            {
                eCMD? cmd = sender as eCMD?;
                
                //LogState(LogType.Info, string.Format("Cycler Send,{0}", cmd.ToString()));
            }
            catch
            {

            }
        }

        private void Cycler__EventState(object sender, EventArgs e)
        {
            try
            {
                DisChgPoint cds = sender as DisChgPoint;

                SetMainCState(cds.OpMode.ToString());
                SetMainVoltage(cds.Vlot.ToString());
                SetMainCurrent(cds.Curr.ToString());

                if (cds.Time != 0 && cds.Time < 100)
                {
                    if (cds.OpMode != PhoenixonLibrary.RECV_OPMode.READY_TO_INPUT)
                    {
                       /*
                        LogState(LogType.Info, (string.Format("Cycler: {0}, StepTime: {1}, Volt: {2}, Curr: {3}",
                                                    cds.OpMode.ToString(),
                                                    cds.Time,
                                                    cds.Vlot,
                                                    cds.Curr)));
                                                    */
                    }
                }
            }
            catch
            {

            }
        }

        private void Cycler__EventStepError(object sender, EventArgs e)
        {
            try
            {
                StringBuilder alarmMsg = sender as StringBuilder;

                Dispatcher.Invoke(new Action(() =>
                {
                    AlarmWindow am = new AlarmWindow(alarmMsg.ToString());

                    am.Show();
                }));
            }
            catch
            {

            }
        }

        private void _chgDisControl__EventChgDisError1(object sender, EventArgs e)
        {
            try
            {
                var alarm = sender as ChgDisAlarms;

                Dispatcher.Invoke(new Action(() =>
                {
                    CyclerAlarmWindow aw = new CyclerAlarmWindow(this,
                                                    alarm.ErrWarnning,
                                                    alarm.ErrGrid,
                                                    alarm.ErrDC,
                                                    alarm.ErrFault);
                    aw.ShowDialog();

                    var th = new Thread(() =>
                    {
                        LogState(LogType.Info, "Send error clear message to Cycler");
                        for (int i = 0; i < 10; i++)
                        {
                            // cycler alarm clear
                            Device.Cycler.ChgDisControl.SetErrorClear();
                            Thread.Sleep(100);
                        }

                        // cycler alarm List Reset 
                        Device.Cycler.ChgDisAlarmReset();
                        Thread.Sleep(1000);
                        Device.Relay.Off(RELAY.LIGHT_RED);
                    });
                    th.Start();
                }));
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.Message);
                LogState(LogType.Fail, ex.StackTrace);
            }
        }

        private void _chgDisControl__EventChgDisError(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void TemperInit()
        {
            if (CONFIG.EolInspType == InspectionType.HIPOT)
            {
                try
                {
                    Device.Tempr = new Nht_RS232(CONFIG.TEMP);
                    Device.Tempr.Open();
                    Device.Tempr.EventTemp += Tempr_EventTemp;

                }
                catch               {                }
            }
            else
            {
                try
                {
                    Device.TemprCT100 = new CTempCT100(CONFIG.TEMP, EventUITemp);
                }
                catch               {               }
            }

        }

        private void EventUITemp(object sender, EventArgs e)
        {
            try
            {
                float? temp = sender as float?;

                Dispatcher.Invoke(new Action(() =>
                {
                    templb.Content = temp.ToString();
                }));
            }
            catch
            {

            }
        }

        private void Tempr_EventTemp(object sender, EventArgs e)
        {
            try
            {
                float? temp = sender as float?;

                Dispatcher.Invoke(new Action(() =>
                {
                    templb.Content = temp.ToString();
                }));
            }
            catch
            {

            }
        }

        private void DeviceConnect()
        {
            try
            {
              
                if (CONFIG.EolInspType == InspectionType.EOL)
                {
                    CyclerInit(false);

                    KeysightDMMInit();
                }
                else if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    CyclerInit(true);
                    ChromaInit();
                }

                // 공통 
                TemperInit();
                RelayInit();
                PLCInit();
                MesInit();
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.Message);
                LogState(LogType.Fail, ex.StackTrace);
            }
        }

        private void PLC_PLC_MonitorEvent(object sender, EventArgs e)
        {
            try
            {
                PLCMonitor data = (PLCMonitor)sender;
                int nAddr = Convert.ToInt32(data.address, 16);

                /*
                if (nAddr == PLC_ReadCMD.AutoManual)
                {
                    if (data.value)
                    {
                        Console.WriteLine("ON");
                        Device.PLC.SetAutoMode(true);
                    }
                    else
                    {
                        Console.WriteLine("OFF");
                        Device.PLC.SetAutoMode(false);
                    }
                }
                else if (nAddr == PLC_ReadCMD.test)
                {
                    Console.WriteLine("BB");

                }
                */
            }
            catch
            {

            }
        }

        private void SetUI_EOL_HIPOT()
        {
            try
            {
                switch (CONFIG.EolInspType)
                {
                    case InspectionType.EOL:

                        break;
                    case InspectionType.HIPOT:

                       
                        lblCycler.Visibility = Visibility.Collapsed;
                        contBt_cycler.Visibility = Visibility.Collapsed;

                        lblDMM.Content = "HIPOT";
                        //lblDAQ.Visibility = Visibility.Collapsed;
                        lblDAQ.Content = "Temp";

                        contBt_Temp.Visibility = Visibility.Collapsed;
                        //contBt_daq1.Visibility = Visibility.Collapsed;
                        gridCellDetailDg.Visibility = Visibility.Collapsed;

                        btnShowDAQ.Visibility = Visibility.Collapsed;
                        cyclerShow.Visibility = Visibility.Collapsed;
                        cyclerBt.Visibility = Visibility.Collapsed;
                        test11.Visibility = Visibility.Collapsed;

                        lable_temp.Visibility = Visibility.Collapsed;
                       // lblTemp.Visibility = Visibility.Collapsed;
                       // templb.Visibility = Visibility.Collapsed;
                      //  lblC.Visibility = Visibility.Collapsed;

                        Grid.SetRowSpan(testItemListDg, 3);

                        Grid.SetRow(relayConBt, 14);
                        Grid.SetRow(btnResult, 15);
                        Grid.SetRow(btnLog, 16);

                        break;
                    case InspectionType.EOL_HIPOT:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.Message);
                LogState(LogType.Fail, ex.StackTrace);
            }
        }

        private void SetUI()
        {
            selectedIndex = 0;

            modelList.Add(new Models()
            {
                Number = "0",
                ProductCode = CONFIG.MesProductCode,
                ProcessID = CONFIG.MesProcessID,
                EquipId = CONFIG.MesEquipID,
                UserId = CONFIG.MesUserID,
                ModelId = BatteryInfo.ModelName,
                LotId = "NOT_LOT_ID"
            });

            mmw = new MiniScheduler.MainWindow(this);
            this.totalProcessList = mmw.totalProcessList;

            this.prodTb.Text = this.modelList[selectedIndex].ProductCode;//제품 ID
            this.procTb.Text = this.modelList[selectedIndex].ProcessID;//공정ID
            this.userTb.Text = this.modelList[selectedIndex].UserId;
            this.EquipIDTb.Text = this.modelList[selectedIndex].EquipId;

            cellDetailList.Clear();
            cellDetailList.Add(new CellDetail() { TestName = "Init Volt" });
            cellDetailList.Add(new CellDetail() { TestName = "Discharge" });
            cellDetailList.Add(new CellDetail() { TestName = "Rest" });
            cellDetailList.Add(new CellDetail() { TestName = "Charge" });
            cellDetailList.Add(new CellDetail() { TestName = "Rest" });

            //cellDetailList.Add(new CellDetail() { TestName = "Finish Volt (20s)" });
            //cellDetailList.Add(new CellDetail() { TestName = "Finish Volt (30s)" });
            //cellDetailList.Add(new CellDetail() { TestName = "Finish Volt (40s)" });

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

            this.EquipIDTb.Background = this.lotTb.Background = this.prodTb.Background = this.procTb.Background = Brushes.Red;

            mWindowPreCheck = new PreCheckRetry(this);//DeviceCheckWindow(this, false, 1);
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
            DirectoryInfo di = new DirectoryInfo(PATH.LOG);
            if (di.Exists == false)
            {
                di.Create();
            }

            //string @LogDirectory = @"C:\Logs\Inspection_result\" + MODEL_NAME;
            string @LogDirectory = @"D:\Inspection_result\" + MODEL_NAME;

            if (!Directory.Exists(@LogDirectory))
            {
                Directory.CreateDirectory(@LogDirectory);
            }

            //@LogDirectory = @"C:\Users\Public\EOL_INSPECTION_LOG";
            @LogDirectory = @"D:\EOL_INSPECTION_LOG";

            if (!Directory.Exists(@LogDirectory))
            {
                Directory.CreateDirectory(@LogDirectory);
            }

            //소모품 카운트 기능 nnkim
            string strPathLog = @"C:\EOL_SPAREPATSCOUNT_INFO\" + position + "\\Log";
            if (!Directory.Exists(strPathLog))
            {
                Directory.CreateDirectory(strPathLog);
            }
        }

        private void InitializeCommonMethods()
        {
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
            this.Closing += MainWindow_Closing;
            this.KeyDown += MainWindow_KeyDown;

            bt_pass.Content = "-";
            bt_pass.Background = Brushes.DarkGray;
            timetick.Interval = 100;
            timetick.Tick += ti_Tick;

            _timerUI.Interval = TimeSpan.FromSeconds(2);         //시간간격 설정
            _timerUI.Tick += new EventHandler(timer_Tick);         //이벤트 추가

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (CONFIG.EolInspType == InspectionType.EOL)
                {
                    // keysight DAQ 
                    if (Device.KeysightDAQ.isAlive)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                        }));
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                        }));
                    }

                    // Cycler 
                    if (Device.Cycler.GetDspStatus())
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            contBt_cycler.Background = System.Windows.Media.Brushes.Green;
                        }));
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            contBt_cycler.Background = System.Windows.Media.Brushes.Red;
                        }));
                    }

                    // Ph DAQ
                    if (Device.Cycler.DaqAlive)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            contBt_daq1.Background = Brushes.Green;
                        }));
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            contBt_daq1.Background = Brushes.Red;
                        }));
                    }


                    // Temp
                    if (CONFIG.EolInspType == InspectionType.HIPOT)
                    {
                        if (Device.Tempr.IsAlive)
                        {
                            if (Device.Tempr.GetTemp == -255)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    contBt_Temp.Background = Brushes.Red;
                                }));
                            }
                            else
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    contBt_Temp.Background = Brushes.Green;
                                }));
                            }
                        }
                        else
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                contBt_Temp.Background = Brushes.Red;
                            }));
                        }
                    }
                    else
                    {
                        //if (Device.Tempr.IsAlive)
                        if (Device.TemprCT100.GetOpenState() == true)
                        {
                            //if (Device.Tempr.GetTemp == -255)
                            if (Device.TemprCT100.GetTemprature() == -255)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    contBt_Temp.Background = Brushes.Red;
                                }));
                            }
                            else
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    contBt_Temp.Background = Brushes.Green;
                                }));
                            }
                        }
                        else
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                contBt_Temp.Background = Brushes.Red;
                            }));
                        }
                    }
                }
                else if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    if (Device.Chroma.Alive)
                    {
                        
                            contBt_dmm.Background = Brushes.Green;
           
                    }
                    else
                    {
                            contBt_dmm.Background = Brushes.Red;
               
                    }

                    if (CONFIG.EolInspType == InspectionType.HIPOT)
                    {
                        if (Device.Tempr.IsAlive)
                        //if (Device.TemprCT100.GetOpenState() == true)
                        {
                            if (Device.Tempr.GetTemp == -255)
                            //if (Device.TemprCT100.GetTemprature() == -255)
                            {
                                contBt_daq1.Background = Brushes.Red;
                            }
                            else
                            {
                                contBt_daq1.Background = Brushes.Green;
                            }
                        }
                        else
                        {
                            contBt_daq1.Background = System.Windows.Media.Brushes.Red;
                        }
                    }
                    else
                    {
                        //if (Device.Tempr.IsAlive)
                        if (Device.TemprCT100.GetOpenState() == true)
                        {
                            //if (Device.Tempr.GetTemp == -255)
                            if (Device.TemprCT100.GetTemprature() == -255)
                            {
                                contBt_daq1.Background = Brushes.Red;
                            }
                            else
                            {
                                contBt_daq1.Background = Brushes.Green;
                            }
                        }
                        else
                        {
                            contBt_daq1.Background = System.Windows.Media.Brushes.Red;
                        }
                    }
                }

                #region PLC, MES
                if (Device.PLC != null)
                {
                    /*
                    if (Device.PLC.isAlive)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            contBt_plc.Background = Brushes.Green;
                        }));
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            contBt_plc.Background = Brushes.Red;
                        }));
                    }
                    */
                    if (Device.PLC.isAlive)
                    {
                      
                            contBt_plc.Background = Brushes.Green;
        
                    }
                    else
                    {
                       
                            contBt_plc.Background = Brushes.Red;
                    
                    }
                }
  

                if(Device.MES != null)
                {
                    if (Device.MES.IsMESConnected)
                    {
                            contBt_mes.Background = Brushes.Green;
                    }
                    else
                    {
                        contBt_mes.Background = Brushes.Red;
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.Message);
                LogState(LogType.Fail, ex.StackTrace);
            }
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
                string fileName = "";

                if (CONFIG.EolInspType == InspectionType.EOL)
                    fileName = PATH.FileNameEOL;
                else if (CONFIG.EolInspType == InspectionType.HIPOT)
                    fileName = PATH.FileNameHIPOT;

                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");

                //181218 여러개 모델일 때 버튼이 새로 추가 안되는 현상 FIX
                groupDic = new Dictionary<string, List<TestItem>>();
                groupDic.Clear();
                var li = new List<TestItem>();

                FileStream readData = new FileStream(AppDomain.CurrentDomain.BaseDirectory + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

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

                        var ti = new TestItem
                        {
                            CLCTITEM = clctItem,
                            Max = max,
                            Min = min,
                            Unit = unit,
                            No = cnt,
                            Bt = button,
                            GroupName = groupName,
                            Name = singleName,
                            SingleMethodName = ar[7],
                            DigitLength = digit
                        };

                        if (!groupDic.ContainsKey(groupContent))
                        {
                            li = new List<TestItem>
                            {
                                ti
                            };
                            groupDic.Add(groupContent, li);
                        }
                        else
                        {
                            groupDic[groupContent].Add(ti);
                        }
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

                this.testItemListDg.ItemsSource = modelList[selectedIndex].TestItemList;
                this.testItemListDg.Items.Refresh();

                LogState(LogType.Info, "Load Local Spec(CollectItem).");
            }
            catch (Exception ec)
            {
                LogState(LogType.Fail, "LoadList", ec);
            }
        }

        /// <summary>
        /// 편의를 위해 특정 파일에 메서드를 텍스트로 만들어주는 부분
        /// </summary>
        /// <param name="dicItem"></param>
        private void MakeGroupMethodToTextFile(Dictionary<string, List<TestItem>> dicItem)
        {
            var sb = new StringBuilder();
            sb.AppendLine("//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            foreach (var methodName in dicItem)
            {
                sb.AppendLine("public bool " + methodName.Key + "(TestItem ti)");
                sb.AppendLine("{");
                sb.AppendLine("     isProcessingUI(ti);");
                sb.AppendLine("     ti.Value_ = null;");
                sb.AppendLine("     return JudgementTestItem(ti);");
                sb.AppendLine("}");
                sb.AppendLine("");
            }
            File.AppendAllText("D:\\MethodFile.txt", sb.ToString() + "\r\n", Encoding.UTF8);
        }

        /// <summary>
        /// 편의를 위해 특정 파일에 메서드를 텍스트로 만들어주는 부분
        /// </summary>
        private void MakeSingleMethodToTextFile(string methodName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("public bool " + methodName + "(TestItem ti)");
            sb.AppendLine("{");
            sb.AppendLine("     isProcessingUI(ti);");
            sb.AppendLine("     ti.Value_ = null;");
            sb.AppendLine("     return JudgementTestItem(ti);");
            sb.AppendLine("}");
            sb.AppendLine("");
            File.AppendAllText("D:\\MethodFile.txt", sb.ToString() + "\r\n", Encoding.UTF8);
        }

        Thread GroupBtThread, SingleBtThread;



        const string RETRY_MSG = "연결상태가 정상이 아닙니다. 다시 ";
        const string HIPOT_ENABLE_MAIN_MSG = "HIPOT 프로브를 연결해주세요.";
        const string HIPOT_ENABLE_SUB_MSG = "1. HIPOT 프로브 연결 확인.\n2. LV커넥터 제거 확인\n3. 전류 프로브 제거 확인";
        const string LV_ENABLE_MAIN_MSG = "LV커넥터를 연결해주세요.";
        const string LV_ENABLE_SUB_MSG = "1. 전류 프로브 연결 확인.\n2. LV커넥터 연결 확인";
        const string CURRENT_ENABLE_MAIN_MSG = "전류 프로브를 연결해주세요.";
        const string CURRENT_ENABLE_SUB_MSG = "1. 전류 프로브 연결 확인.\n2. LV커넥터 연결 확인\n3. HIPOT 프로브 제거 확인";
        const string DISABLE_MAIN_MSG = "모든 프로브를 제거해주세요.";
        const string DISABLE_SUB_MSG = "1. HIPOT 프로브 제거 확인.\n2. LV커넥터 제거 확인\n3. 전류 프로브 제거 확인";

        bool SensingVoltToMux()
        {
            return true;

            /*
            LogState(LogType.Info, "-----------------------------------------------------------------");
            LogState(LogType.Info, "SensingVoltToMux Start");
            _//device.DAQ.rtstring = "";

            cellVoltCnt = 0;
            string measString = "";
            int reccnt = 0;
            //SetChannels(out measString, out cellVoltCnt, out reccnt);

            #region measure Each Voltage
            //_device.DAQ.rtstring = "";
           // _device.DAQ.MeasVolt(measString);

            var rec = _device.DAQ.sp.BytesToRead;
            
            var cnt = 0;
            while (rec < reccnt)//33
            {
                Thread.Sleep(100);
                rec = _device.DAQ.sp.BytesToRead;
                cnt += 100;
                //not received data
                if (cnt == 5000)
                {
                    _device.DAQ.MeasVolt(measString);

                    rec = _device.DAQ.sp.BytesToRead;

                    cnt = 0;
                    while (rec < 145)//33
                    {
                        Thread.Sleep(100);
                        rec = _device.DAQ.sp.BytesToRead;
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
            _device.DAQ.sp.Read(bt, 0, rec);

            _device.DAQ.rtstring = Encoding.Default.GetString(bt, 0, rec);

            //LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

            var vArr = _device.DAQ.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            CellVoltageList = new List<double>();
            string voltstr = "";

            bool isOVER3V = false;
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

            LogState(LogType.Info, "SensingVoltToMux end");
            LogState(LogType.Info, "-----------------------------------------------------------------");
            return isOVER3V ? true : false;
            */
        }


        void ShowPopup(string gbtName)
        {
            return;

            if (Device.PLC.isTesting_Flag)
                return;

            if (gbtName == "IRCheck" || gbtName == "ProbeContactCheck")
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    var nextWindow = new NextWindow(HIPOT_ENABLE_MAIN_MSG, HIPOT_ENABLE_SUB_MSG, this);
                    nextWindow.ShowDialog();

                    if (gbtName == "IRCheck")
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
                /*
                this.Dispatcher.Invoke(new Action(() =>
                {
                    var nextWindow = new NextWindow(CURRENT_ENABLE_MAIN_MSG, CURRENT_ENABLE_SUB_MSG, this);
                    nextWindow.ShowDialog();                     
                }));
                */
            }
        }

        void HidePopup(string gbtName)
        {
            if (Device.PLC.isTesting_Flag)
                return;

            if (gbtName == _BarCodeName)
                return;

            this.Dispatcher.Invoke(new Action(() =>
            {
                var nextWindow = new NextWindow(DISABLE_MAIN_MSG, DISABLE_SUB_MSG, this);
                nextWindow.gradStop.Color = Colors.Green;
                nextWindow.contibt.Content = "OK";
                nextWindow.ShowDialog();
            }));

        }

        /// <summary>
        /// 테스트 진행시 버튼 비활성화, 활성화 함수
        /// </summary>
        /// <param name="flag"></param>
        private void BtnEnableChange(bool status)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    resetBt.IsEnabled = status;
                    specBt.IsEnabled = status;
                    cyclerBt.IsEnabled = status;
                    btnShowDAQ.IsEnabled = status;

                    isMESSkipCb.IsEnabled = status;
                    isSkipNG.IsEnabled = status;

                    type1lb.IsEnabled = status;

                    resetBt.IsEnabled = status;
                    specBt.IsEnabled = status;
                    relayConBt.IsEnabled = status;
                }));
            }
            catch
            {

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

            IsEmgStop = Ispause = false;
            CyclerEmergency(false);
            ChromaEmergency(false);

            string gbtName = (sender as Button).Name.ToString();

            if (gbtName == "BarcodeReading")
            {
                modelList[selectedIndex].LotId = lotTb.Text;
            }

            GroupBtThread = new Thread(() =>
            {
                BtnEnableChange(false);

                //검사 전
                ShowPopup(gbtName);

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

                    #region Chroma.Emergency
                    if (CONFIG.EolInspType == InspectionType.HIPOT)
                    {
                        if (Device.Chroma.Emergency == true)
                            break;
                    }
                    #endregion
                }

                if (IsEmgStop == true)
                {
                    LogState(LogType.Info, "EMERGENCY_STOPPED");
                    
                }
            
                SetCtrlToPass(isPass, gbtName);
                BtnEnableChange(true);
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
            IsEmgStop = false;
            if (SingleBtThread != null)
                return;

            IsEmgStop = Ispause = false;
            ChromaEmergency(false);


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
                        BtnEnableChange(false);
                        MethodInvoker(item.Value.SingleMethodName, new object[] { item.Value });

                        SingleBtThread = null;
                        BtnEnableChange(true);
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
                case Key.F4:
                    {
                        var th = new Thread(() =>
                        {
                            SetCellDetailList(0);
                            SetCellDetailList(1);
                            SetCellDetailList(2);
                            SetCellDetailList(3);
                            SetCellDetailList(4);
                        });
                        th.Start();
                    }; break;
                case Key.F5:
                    {
                    }; break;
            }
        }

        /// <summary>
        /// MES 연동 시 Blue 점등
        /// </summary>
        /// <param name="isOn"></param>
        public void SetTLampMESStatus(bool isOn)
        {
            try
            {
                if (isOn)
                {
                    Device.Relay.On(RELAY.LIGHT_BLUE);
                }
                else
                {
                    Device.Relay.Off(RELAY.LIGHT_BLUE);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 설비에서 시작 직후 MES에서 값을 잘못 가져왔을 때, 
        /// 빠져나가기 전에 퍼즈가 들어와있으면 대기타도록 되어있다.
        /// </summary>
        /// <param name="log"></param>
        private void PauseLoop(string log)
        {
            if (Ispause)
            {
                LogState(LogType.Info, log + " - PAUSE");
            }

            bool isg = false;
            while (Ispause)
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
            try
            {
                //이미 동작중에 다시 시작이 들어온다면 리턴처리
                if (autoModeThread != null && autoModeThread.IsAlive)
                    return;

                LogState(LogType.Info, "START_TEST-------------------------------------------------------", null, false);

                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (mWindowPreCheck != null && mWindowPreCheck.IsVisible == true)
                    {
                        mWindowPreCheck.Hide(); // 프리체크 팝업 활성화 되있다면 숨기고 시작
                    }

                    modelList[0].LotId = _Barcode = this.lotTb.Text;

                    //검사값 비우기
                    ResetClick(this, null);

                    this.time_start_tb.Text = this.time_finish_tb.Text = this.time_elapsed_tb.Text = "";
                    this.testPb.Value = 0;
                    nowTime = DateTime.Now;

                    _stopwatch.Reset();
                    _stopwatch.Start();
                    this.time_start_tb.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    //elapsedtime = new DateTime();
                    //this.time_elapsed_tb.Text = elapsedtime.ToString("HH:mm:ss");

                    isMESSkipCb.IsEnabled = manualBt.IsEnabled = false;
                    blinder.IsEnabled = false;
                    blinder3.Visibility = blinder.Visibility = Visibility.Visible;
                }));

                ThreadStart tstart = new ThreadStart(this.AutoModeStart);
                autoModeThread = new Thread(tstart)
                {
                    IsBackground = true
                };

                timetick.Start();
                autoModeThread.Start();
          

                this.LogState(LogType.Info, "Auto mode Started");
            }
            catch
            {

            }
            
        }
        private void AutoModeStart()
        {
            
            try
            {
                if (Device.PLC != null)
                    Device.PLC.isSuccess = false;

                #region MES Conncet Check
                MesConnectCheck();
                #endregion
          
                IsEmgStop = false;

                double testCount = this.modelList[selectedIndex].TestItemList.Count;
                double progVal = 100.0 / testCount;
                bool isPass = true;

                //prodId = this.Barcode;//제품 ID
                //userId = "EIF";
                //procId = "P8100";//공정ID
                //eqpId = "W1PBMA101-4-4";
                this.modelList[selectedIndex].LotId = this._Barcode;// "LGC-WAH;B0011PL230001";

                this.Dispatcher.Invoke(new Action(() =>
                {
                    // 2019.08.27 jeonhj's comment
                    // mes skip flag에 의해 변경되지 않아야 함.
                    IsMESskip = isMESSkipCb.IsChecked == false ? false : true;
                    this.modelList[selectedIndex].UserId = userTb.Text;
                }));

                #region AutomodeStart 시 검사모델/설비ID/제품ID/공정ID를 로그로 남김
                LogState(LogType.Info, "Started Model : " + this.modelList[selectedIndex].ModelId);
                LogState(LogType.Info, "Lot ID : " + this.modelList[selectedIndex].LotId);
                LogState(LogType.Info, "Process ID : " + this.modelList[selectedIndex].ProcessID);
                LogState(LogType.Info, "ProductCode ID : " + this.modelList[selectedIndex].ProductCode);
                LogState(LogType.Info, "Equip ID : " + this.modelList[selectedIndex].EquipId);
                #endregion

                #region 검사 시작전 사전 검사

                if (RunPreCheck(this.modelList[selectedIndex].LotId) == false)
                {
                    SaveData(4);

                    Finished(false, false);

                    return;
                }

                #endregion

                #region 착공, 컨트롤스펙, 프로세싱 스펙
                if (!IsMESskip)
                {
                    if (!Device.MES.IsMESConnected)
                    {
                        this.LogState(LogType.Fail, "MES_NOT_CONNECTED");

                        PauseLoop("MES_NOT_CONNECTED");

                        SaveData(0);

                        Finished(false, false);

                        return;
                    }

                    // MES 제어항목 요청
                    if (!GetControlItemFromMES())
                    {
                        this.LogState(LogType.Fail, "GetControlItemFromMES");

                        PauseLoop("GetControlItemFromMES");

                        SaveData(1);

                        Finished(false, false);

                        return;
                    }

                    //  MES 수집 항목 요청
                    if (!GetCollectItemFromMES())
                    {
                        this.LogState(LogType.Fail, "GetCollectItemFromMES");

                        PauseLoop("GetCollectItemFromMES");

                        SaveData(2);

                        Finished(false, false);

                        return;
                    }


                    //착공보고
                    if (Device.MES.StartJobInsp(this.modelList[selectedIndex].LotId,
                                                this.modelList[selectedIndex].ProcessID,
                                                this.modelList[selectedIndex].EquipId,
                                                this.modelList[selectedIndex].UserId) == "NG") //OK일때 시작
                    {
                        this.LogState(LogType.Fail, "MES_StartJobInsp");

                        PauseLoop("MES_StartJobInsp");

                        SaveData(3);

                        Finished(false, false);

                        return;
                    }

                }
                else
                {
                    GetControlItemFromCSV();
                    GetCollectItemFromCSV();
                }

                SetCyclerStepToMESData();
                #endregion

                if (CONFIG.EolInspType == InspectionType.EOL)
                    SetCellVoltageOrder();

                //파일 삭제 기능 nnkim
                var FolderDelete = new Thread(() =>
                {
                    string @LogDirectory = @"D:\Inspection_result\" + MODEL_NAME;

                    //지울 경로 여기에 추가해주면 됨
                    StringBuilder loadLog = new StringBuilder();
                    loadLog.Append(@LogDirectory);
                    loadLog.Append("\\");
                    loadLog.Append(this.MODEL_NAME);
                    loadLog.Append("\\");
                    loadLog.Append(this.modelList[selectedIndex].ModelId);
                    loadLog.Append("\\");
                    deleteFolder(loadLog.ToString());
                    this.LogState(LogType.Info, loadLog.ToString());

                    string @FILE_PATH_INSPECTION_RESULT = @"D:\EOL_INSPECTION_LOG" ;
                    StringBuilder loadResult = new StringBuilder();
                    loadResult.Append(@FILE_PATH_INSPECTION_RESULT);
                    loadResult.Append("\\");
                    deleteFolder(loadResult.ToString());
                    this.LogState(LogType.Info, loadResult.ToString());
                });

                FolderDelete.Start();

                #region Tests

                ClearCellDetailList();

                ProgressRefresh(0);

                foreach (var singleGroup in groupDic)
                {
                    foreach (var item in modelList[selectedIndex].TestItemList)
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
                        //moon 20191119 유경석 사원 요청
                        if(CONFIG.EolInspType == InspectionType.HIPOT)
                        {
                            if (!isPass)  // hipot 인 경우 ng 이면 stop
                                break;
                        }
                    }

                    SetCtrlToPass(isPass, singleGroup.Value[0].GroupName);

                    if (!isPass || IsEmgStop )
                    {
                        LogState(LogType.Info, "AutoModeStart Stop");

                        //190911 pause flag in testing
                        PauseLoop("PAUSE_FLAG_IN_TESTING");

                        SaveData(0);
                        Finished(false);
                        return;
                    }
                }

                ProgressRefresh(100);
                #endregion

                #region Save and Finished 
                SaveData(0);
                Finished(true);
                autoModeThread = null;
                #endregion
            }
            catch(Exception ex)
            {
                LogState(LogType.Fail, ex.StackTrace);
                LogState(LogType.Fail, ex.Message);
            }
        }


        private void MesConnectCheck()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (!IsMESskip && !Device.MES.IsMESConnected)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (!Device.MES.IsMESConnected)
                        {
                            LogState(LogType.Fail, string.Format("Not connected MES. {0}", (i + 1).ToString()));
                            Device.MES.Open();
                        }
                        else
                        {
                            LogState(LogType.Success, "Connected MES.");
                            Device.Relay.On(RELAY.LIGHT_BLUE);
                            break;
                        }
                    }
                }
            }));
        }



        bool test1 = false;

        List<string> DelKeyList = new List<string>();
        /// <summary>
        ///  MES 제어 항목 중 제어 항목 값이 없으면 검사 항목에서 제외 처리 moons 
        /// </summary>
        public void MesContorlItemCheckAndDelete()
        {
            if (EOL.IR_CtrlItem.Count == 6)
            {
                DelKeyList.Add("(-) Terminal Insulation resistance(HV Bushing)");
                DelKeyList.Add("(-) Terminal Insulation resistance(HV Cooling plate)");
            }

            foreach (var id in DelKeyList)
            {
                modelList[selectedIndex].TestItemList.Remove(id);
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                this.testItemListDg.ItemsSource = null;

                
                foreach (var child in btgrid.Children.OfType<Grid>())
                {
                    var button = child.Children.OfType<Button>().FirstOrDefault(); //groupbutton

                    /*
                    var combobox = child.Children.OfType<ComboBox>().FirstOrDefault(); //combobox
                    var comboboxButton = combobox.Items.OfType<Button>().FirstOrDefault(); //Buttons in combobox

                    if (comboboxButton.Content.ToString().Contains(""))
                    {
                        for (int i = 0; i < combobox.Items.Count; i++)
                        {
                            var type = (Button)combobox.Items[i];
                            if (type.Content.ToString().Contains(""))
                            {
                                combobox.Items.Remove(type);                                
                            }
                        }
                    }
                    */

                    var aa = child.Children.OfType<ComboBox>().FirstOrDefault();
                    
                    if (button.Content.ToString().Contains("IR Check Minus"))
                    {
                        btgrid.Children.Remove(child);
                        break;
                    }
                }
                

                int no = 1;
                foreach (var item in modelList[selectedIndex].TestItemList)
                {
                    item.Value.No = no++;
                }

                this.testItemListDg.ItemsSource = modelList[selectedIndex].TestItemList;
                this.testItemListDg.Items.Refresh();
            }));
        }


        /// <summary>
        /// 검사종류가 여러개일 때 새로고침 되는 부분
        /// </summary>
        public void RefreshToFlags()
        {
            this.testItemListDg.ItemsSource = modelList[selectedIndex].TestItemList;


            this.prodTb.Text = this.modelList[selectedIndex].ProductCode;//제품 ID
            this.procTb.Text = this.modelList[selectedIndex].ProcessID;//공정ID
            this.EquipIDTb.Text = this.modelList[selectedIndex].EquipId;// 설비ID

            _Barcode = this.lotTb.Text = this.modelList[selectedIndex].LotId;// "J9D3-10B759-AB1-170118000001";//" "LGC-WAH;B0011PL230001";
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
            try
            {
                LogState(LogType.Info, "MES Skip");

                // MES Skip 체크 박스 비활성화 
                isMESSkipCb.IsEnabled = false;
                Thread.Sleep(200);

                this.EquipIDTb.Background = this.lotTb.Background = Brushes.Red;
                this.prodTb.Background = this.procTb.Background = Brushes.Red;

                IsMESskip = true;

                if (Device.Relay != null)
                    Device.Relay.Off(RELAY.LIGHT_BLUE);

                // 2019.06.28
                // mes가 skip되면 로컬데이터를 가져와야 함.
                if (CONFIG.EolInspType != InspectionType.NONE)
                {
                    //if(Device.MES.IsMESConnected != true)
                    if(IsMESskip == true)
                    {
                        GetControlItemFromCSV();
                        GetCollectItemFromCSV();
                    }
                }

                // MES Skip 체크 박스 활성화 
                Thread.Sleep(500);
                isMESSkipCb.IsEnabled = true;
            }
            catch
            {

            }
        }

        private void MES스킵안함(object sender, RoutedEventArgs e)
        {
            try
            {
                // MES Skip 체크 박스 비활성화 
                isMESSkipCb.IsEnabled = false;
                Thread.Sleep(200);

                IsMESskip = false;

                LogState(LogType.Info, "MES Unskip");
                if (Device.MES.IsMESConnected)
                {
                     this.EquipIDTb.Background = this.lotTb.Background = Brushes.SkyBlue;
                    this.prodTb.Background = this.procTb.Background = Brushes.SkyBlue;

                    Device.Relay.On(RELAY.LIGHT_BLUE);

                    //여기넣는게 맞나?? 확인필요
                    var th = new Thread(() =>
                    {
                        //process control param 요청
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
                    e.Handled = true;
                    Device.Relay.Off(RELAY.LIGHT_BLUE);

                    LogState(LogType.Info, "Not Connected to MES");

                    isMESSkipCb.IsEnabled = true;
                }

                // MES Skip 체크 박스 
              //  Thread.Sleep(500);
              //  isMESSkipCb.IsEnabled = true;
            }
            catch
            {

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
            var rp = new RequirePasswordWindow(this);
            rp.ShowDialog();
            if (rp.isOK)
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
            rp.Close();
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

            try
            {
                dti.Reportitems.Add(data);

                string log = "";
                log = string.Format("MES DetailItem: {0}, Value: {1}", key, data);
                LogState(LogType.Info, log);
            }
            catch
            {

            }

            return dti;
        }

        public bool isCLOSED = false;

        void MainWindow_Closed(object sender, EventArgs e)
        {
            isCLOSED = true;
            _timerUI.Stop();
            timetick.Stop();

            try
            {
                AllThreadAbort();
                this.LogState(LogType.Success, "User Closed");

                DeviceClose();
            }
            catch (Exception ec)
            {
                this.LogState(LogType.Fail, "User Closed", ec);
            }

            Thread.Sleep(200);
            System.Environment.Exit(0);
        }

        enum TESt
        {
            EOL = 0,
            HIPOT,
            CELL,
            EOL_HIPOT

        }



        private void DeviceRelayClose()
        {
            try
            {
                if(CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    Device.Relay.RelayOff();
                }
                else
                {
                    Device.Relay.AllOff();
                }

                Device.Relay.Close();
            }
            catch
            {

            }
        }

        public void DeviceClose()
        {
            try
            {
                // 공통 
                DeviceRelayClose();

                Device.PLC.DisConnect();
                Device.MES.Close();

                if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    if (Device.Chroma != null)
                    {
                        Device.Chroma.ExitClose();
                    }
                }
                else if (CONFIG.EolInspType == InspectionType.EOL)
                {
                    Device.Cycler.AllClose();
                    
                    Device.KeysightDAQ.Close();

                    //Device.Tempr.Close();

                    Device.TemprCT100.ReleaseDevice();
                }


            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.Message);
                LogState(LogType.Fail, ex.StackTrace);
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
            SaveData(0);
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

            if (Ispause)
            {
                LogState(LogType.Info, "Pause");
            }

            bool isg = false;
            while (Ispause)
            {
                isg = true;
            }
            if (isg)
            {
                LogState(LogType.Info, "Resume");
            }

        }

        private void  ResetClick(object sender, RoutedEventArgs e)
        {
            try
            {
                LogState(LogType.Info, "RESET");

                CyclerEmergency(false);
                ChromaEmergency(false);

                _stopwatch.Stop();
                _stopwatch.Reset();

                IsEmgStop = Ispause = false;
                bt_pass.Content = "-";
                bt_pass.Background = Brushes.DarkGray;
                ProgressRefresh(0);

                this.time_start_tb.Text = this.time_finish_tb.Text = this.time_elapsed_tb.Text = "";
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

                if(CONFIG.EolInspType == InspectionType.EOL)
                {
                    if (Device.Cycler != null)
                    {
                        if(Device.Cycler.CyclerAlive)
                            Device.Cycler.ChgDisControl.SetVoltageSensingOn(false);
                    }

                    if(Device.KeysightDAQ !=null )
                    {
                        Device.KeysightDAQ.Discard();
                    }
                }
                else if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    if(Device.Chroma != null)
                    {
                        Device.Chroma.ClearStatus();
                    }
                }
            }
            catch
            {

            }
        }

        private void InspectionSpecClick(object sender, RoutedEventArgs e)
        {
            var rp = new RequirePasswordWindow(this);
            rp.ShowDialog();
            if (rp.isOK)
            {
                if (CONFIG.EolInspType == InspectionType.EOL)
                {
                    var ins = new InspectionSpecWindow(this);
                    ins.ShowDialog();
                }
                else if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    var ins = new InspectionSpecWindow_HiPot(this);
                    ins.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            rp.Close();
        }

        //기본 오토상태
        private void manualBt_Click(object sender, RoutedEventArgs e)
        {
            var rp = new RequirePasswordWindow(this);
            rp.ShowDialog();

            if (rp.isOK)
            {
                bt_pass.Background = Brushes.DarkGray;
                bt_pass.Content = "-";
                IsManual = !IsManual;

                ResetClick(this, null);

                //IsMESskip = isMESSkipCb.IsChecked == false ? false : true;

                if (IsManual)
                {
                    // MES Skip 체크 박스 활성화 
                    isMESSkipCb.IsEnabled = true;
                    isSkipNG.IsEnabled = true;

                    labelA.Background = Brushes.Gray;
                    labelM.Background = Brushes.SkyBlue;

                    relayConBt.IsEnabled = dtcClearBt.IsEnabled = resetBt.IsEnabled = cyclerBt.IsEnabled = specBt.IsEnabled = true;
                    blinder.Visibility = Visibility.Collapsed;

                    LogState(LogType.Info, "---Switched to Manual mode---");
                }
                else  // Auto mode 
                {
                    isMESSkipCb.IsChecked = false;

                    if(Device.MES.IsMESConnected == false)
                    {
                        isMESSkipCb.IsEnabled = false;
                    }

                    isSkipNG.IsChecked = isSkipNG_ = false;
                    labelA.Background = Brushes.SkyBlue;
                    labelM.Background = Brushes.Gray;
                    relayConBt.IsEnabled = dtcClearBt.IsEnabled = resetBt.IsEnabled = cyclerBt.IsEnabled = specBt.IsEnabled = false;
                    blinder.Visibility = Visibility.Visible;


                    LogState(LogType.Info, "---Switched to Auto mode---");
                    
                    mmw.LoadProcessShow();
                    SetCyclerStepToMESData();
                }
            }
            else
            {
                if (!rp.isESC)
                {

                    MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }

            Device.Relay.Off(RELAY.LIGHT_GREEN);
            Device.Relay.On(RELAY.LIGHT_YELLOW);

            rp.Close();
        }

        private void SaveData(int saveCase)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                bt_save.Background = Brushes.Lime;

            }));

            this.Save(saveCase);
        }

        public void SetMainCState(string volt)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.mainCStateLb1.Content = volt.Replace("_", " ");
            }));
        }

        public void SetMainCurrent(string volt)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.mainCurrentLb1.Content = volt + " A";
            }));
        }

        public void SetMainVoltage(string volt)
        {
            this.Dispatcher.Invoke(new Action(() =>
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
            try
            {
                LogState(LogType.Info, "EMERGENCY Stop Button Clicked");
                StopAuto();
            }
            catch
            {

            }
        }


        private void PauseClick(object sender, RoutedEventArgs e)
        {
            LogState(LogType.Info, "Pause Button Clicked");
            Ispause = !Ispause_;

          
        }

        private void CyclerEmergency(bool flag)
        {
            try
            {
                if (CONFIG.EolInspType == InspectionType.EOL)
                {
                    // Cycler stop  
                    // 유경석 사원 요청 사항 20190731 중간검수
                    if (Device.Cycler != null)
                        Device.Cycler.Emergency = flag;
                }
            }
            catch
            {

            }
            
        }

        private void ChromaEmergency(bool flag)
        {
            try
            {
                if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    if (Device.Chroma != null)
                    {
                        Device.Chroma.Emergency = flag;
                    }
                }
            }
            catch
            {

            }
        }

        private void ChromaPause(bool flag)
        {
            try
            {
                if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    if (Device.Chroma != null)
                    {
                        Device.Chroma.Pause = flag;
                    }
                }
            }
            catch
            {

            }
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var rp = new RequirePasswordWindow(this);
                rp.ShowDialog();
                if (rp.isOK)
                {
                    LogState(LogType.Info, "Start Button Clicked");
                    AutoMode();
                }
                else
                {
                    MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                rp.Close();
            }
            catch
            {

            }
        }

        private void CyclerSettingsClick(object sender, RoutedEventArgs e)
        {
            var rp = new RequirePasswordWindow(this);
            rp.ShowDialog();
            if (rp.isOK)
            {
                mmw.savedCb.IsChecked = false;
                mmw.Show();
            }
            else
            {
                MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);

            }
            rp.Close();
        }

        private void OpenResultClick(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start("explorer.exe", @"C:\Logs\Inspection_result");
            System.Diagnostics.Process.Start("explorer.exe", PATH.INSPECTION_RESULT);
        }

        private void OpenLogClick(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start("explorer.exe", @"C:\Users\Public\EOL_INSPECTION_LOG");
            System.Diagnostics.Process.Start("explorer.exe", PATH.LOG);
        }

        private void StateLb_KeyUp_0(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                StateLb.Items.Clear();
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
            var rp = new RequirePasswordWindow(this);
            rp.ShowDialog();
            if (rp.isOK)
            {
                RelayControllerWindow rc = new RelayControllerWindow();
                rc.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);

            }
            rp.Close();
        }

        private void DTCClearClick(object sender, RoutedEventArgs e)
        {
            ClearDTC();
        }
        #endregion

        #region Logging / Save data

        public void LogState(LogType lt, string str, Exception ec = null, bool isView = true)
        {
            string header = "";
            StringBuilder sb = new StringBuilder(1024);

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
            }

            str = str.Replace("\n", "");
            sb.Append(header);
            sb.Append(str);
            sb.Append(ec != null ? (" - " + ec.Message + " : " + ec.StackTrace + ",") : ",");

            if (isView)
            {
                //메인 UI에 동기로 접근시켜서 로그를 새로고침 시킨다.
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        //UI에서 5천개이상 데이터가 쌓이면 강제로 비우도록 한다.
                        if (StateLb.Items.Count > 5000)
                            StateLb.Items.Clear();
                     
                        StateLb.Items.Insert(0, sb.ToString());
                    }
                    catch (Exception ex)
                    {
                         if(Device.Cycler != null)
                            Device.Cycler.LogWrite(ex.Message);
                    }
               }));
            }

            string tBarcode = "";

            try
            {
                if (_Barcode != "" && _Barcode != null)
                {
                    tBarcode = _Barcode.Replace(":", "_").ToString().Replace(";", "_").ToString().Replace("\r\n", "").ToString();

                    Device.Cycler.LogBarcodeWrite(tBarcode, sb.ToString());
                }

                if (Device.Cycler != null)
                {
                    Device.Cycler.LogAllDayWrite(sb.ToString());
                    Device.Cycler.LogWriteAndUI(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                tBarcode = "NOT_POSSIBLE_FILE_NAME";
                //Device.Cycler.LogBarcodeWrite(tBarcode, sb.ToString());
            }
        }




        /// <summary>
        /// 23.  Save() 함수 요청반영사항
        ///      -> 상세수집항목 저장 정렬구문 추가(헤더/내용 모두)
        ///      -> 저장항목 define(LOT id, Module BCR, Device STAT 등 코드 헤더 및 저장내용 참조)
        ///      -> 바코드 항목 예외구문 추가
        ///      -> 착공 NG 시 NG 처리구문 추가
        /// </summary>
        /// <param name="saveCase">0: normal / 1: 제어항목NG / 2: 수집항목NG / 3: 착공NG</param>
        private void Save(int saveCase = 0)
        {
            string @LogDirectory = PATH.INSPECTION_RESULT + "\\" + this.modelList[selectedIndex].ModelId;

            if (!Directory.Exists(@LogDirectory))
            {
                Directory.CreateDirectory(@LogDirectory);
            }

            //MES가 연결되어있고, 자동모드스레드가 살아있고, 검사항목이 모두 비어있을 때
            //정상 검사 상태일 때 착공NG가 난다면, 해당 루틴에 따라 첫 항목을 MES_StartJob NG로 기입한다.
            #region MES_StartJob NG 입력

            var isEmptyItems = true;
            // 2019.06.28 jeonhj
            // 착공 NG 때 첫번째 수집항목에 StartJob NG 기록하는 것 수정
            // item.Value의 Value_를 확인하지 않아 아래 착공NG 기록에 걸리지 않았음.
            // M10 보고 수정함.
            foreach (var item in this.modelList[selectedIndex].TestItemList)
            {
                if (item.Value.Name == _BarCodeName)
                {
                    continue;
                }

                if (item.Value != null && item.Value.Value_ != null)
                {
                    isEmptyItems = false;
                }
            }

            if (autoModeThread != null && autoModeThread.IsAlive && !IsMESskip && isEmptyItems)
            {
                this.modelList[selectedIndex].TestItemList.ToList()[1].Value.Value_ = "MES_StartJob NG";
                this.modelList[selectedIndex].TestItemList.ToList()[1].Value.Result = "NG";

                // 2019.07.03 jeonhj
                // 착공NG와 EMS 상황을 분리
                if (IsEmgStop)
                {
                    this.modelList[selectedIndex].TestItemList.ToList()[1].Value.Value_ = _EMG_STOPPED;
                }
                else
                {
                    switch (saveCase)
                    {
                        case 1: this.modelList[selectedIndex].TestItemList.ToList()[1].Value.Value_ = "MES_GetControlItem NG"; break;
                        case 2: this.modelList[selectedIndex].TestItemList.ToList()[1].Value.Value_ = "MES_GetCollectItem NG"; break;
                        case 3: this.modelList[selectedIndex].TestItemList.ToList()[1].Value.Value_ = "MES_StartJob NG"; break;
                        case 4: this.modelList[selectedIndex].TestItemList.ToList()[1].Value.Value_ = "EQ_PreCheck NG"; break;
                        default: break;
                    }
                }
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
                //var header = "PASS FAIL,STATION,LOT_ID,MODULE_BCR,IS_MES_SKIP,DEVICE_STAT,";
                var header = "PASS FAIL,STATION,LOT_ID,IS_MES_SKIP,DEVICE_STAT,";
                foreach (var item in this.modelList[selectedIndex].TestItemList)
                {
                    header += item.Key + ",";
                }

                //190104 by grchoi
                foreach (var item in orderedList)
                    header += (item as DetailItems).Key + ",";

                header += "StartTime,ElapsedTime,";

                header.Remove(header.Length - 1, 1);
                File.AppendAllText(dir, header + "\r\n", Encoding.UTF8);
            }

            var first = "PASS";


            // test result data PASS and NG Check
            foreach (var item in this.modelList[selectedIndex].TestItemList)
            {
                //190104 by grchoi
                if (item.Key.Replace(" ", "") == _BarCodeName)
                    continue;

                if (item.Value.Result != null && item.Value.Result != "PASS")
                    first = "NG";
            }

            var tail = first + ",";

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0},", position));
            sb.Append(_Barcode);
            sb.Append(",");

            //190104 by grchoi
            sb.Append(string.Format("{0},", IsMESskip == true ? "YES" : "NO"));

            if (deviceStatus == "")
                sb.Append(string.Format("{0},", "OK"));
            else
                sb.Append(string.Format("{0},", deviceStatus));
      

            // test result
            foreach (var item in this.modelList[selectedIndex].TestItemList)
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
                if (dti.Reportitems.Count != 1)
                    sb.Remove(sb.Length - 1, 1);

                sb.Append(",");
            }

            // Test Time
            this.Dispatcher.Invoke(new Action(() =>
            {
                sb.Append(this.time_start_tb.Text);
                sb.Append(",");
                sb.Append(this.time_elapsed_tb.Text);
                sb.Append(",");
            }));

            sb.Remove(sb.Length - 1, 1);

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
            //ti.refValues_.Clear();

            //24. 수동검사로그저장을위해 해당함수에서 lotTb의 스트링 저장구문 추가
            //190104 by grchoi
            this.Dispatcher.Invoke(new Action(() =>
            {
                modelList[selectedIndex].LotId = _Barcode = this.lotTb.Text;
            }));

            if (Ispause)
            {
                LogState(LogType.Info, "Pause");
            }

            bool isg = false;
            while (Ispause)
            {
                isg = true;
                Thread.Sleep(500);
            }

            if (isg)
            {
                LogState(LogType.Info, "Resume");
            }

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (IsManual)
                    blinder3.Visibility = blinder.Visibility = Visibility.Visible;
            }));

            isProcessingFlag = true;
            AutoScrolling(ti);

            LogState(LogType.Info, "-----------------------------------------------------------------", null, false);
            //LogState(LogType.Info, "Test :" + ti.Name + " Start");
            LogState(LogType.Info, string.Format("Test: {0} Start", ti.Name));

            SetYellow(ti.Bt);

            Device.Relay.On(RELAY.LIGHT_GREEN);
            Device.Relay.Off(RELAY.LIGHT_YELLOW);
        }

        public void SetMESSkipCheck()
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    isMESSkipCb.IsEnabled = false;
                    lotTb.Background = EquipIDTb.Background = prodTb.Background = procTb.Background = Brushes.Red;
                }));

                Device.Relay.On(RELAY.LIGHT_RED);
                Device.Relay.Off(RELAY.LIGHT_BLUE);
            }
            catch
            {

            }
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

            LogState(LogType.Info, "-----------------------------------------------------------------", null);
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

            if (IsEmgStop)
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

            if (IsEmgStop)
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
		  
            try
            {
                Ispause = false;
                IsEmgStop = true;

                CyclerEmergency(true);
                ChromaEmergency(true);

                timetick.Stop();
            }
            catch
            {

            }
        }


        private bool Ispause_ { get; set; }
        ContinueWindow pcw;
        public bool Ispause
        {
            get { return Ispause_; }
            set
            {
                Ispause_ = value;

                #region  Library Emg True
                if (value == true)
                {
                    CyclerEmergency(value);
                    ChromaEmergency(value);
                }

                #endregion

                if (Ispause_)
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
                            if (Device.PLC.plc_Pause_Continue)
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
            try
            {
                var ins = new DAQWindow(this);
                ins.ShowDialog();
            }
            catch
            {

            }
            
        }

        // PreCheck 관련, 테스트 이후 Class화 진행 할 것, Fisrt 및 Second 는 독립적으로 운영 (구분 되야함)

        public /*private*/ PreCheckRetry/*DeviceCheckWindow*/ mWindowPreCheck = null;

        public double mSecondPreCheckThermistorRes = 0.0d;

        public List<double> mSecondPreCheckListCellVolt = new List<double>();

        public List<double> mSecondPreCheckListCellRes = new List<double>();

        public double mFisrtPreCheckReportCyclerVolt = 0.0d;

        public double mFisrtPreCheckReportDMMProbeVolt = 0.0d;

        public double mFisrtPreCheckReportDMMSensingVolt = 0.0d;

        private int mSecondPreCheckTryCount = 0;

        private string mSecondPreCheckTraceLotID = string.Empty;

        private bool RunPreCheck(string lotID)
        {
            LogState(LogType.Info, "Excute, RunPreCheck !");

            try
            {
                if (IsReliability() == true) 
                {
                    // 내부 함수 사용 직전에 유효성 검사 할 것(동작부 부터는 관련 값들이 유효하다고 가정 하고 진행)

                    if (IsPassFisrtPreCheck(lotID) == false)
                    {
                        if (mWindowPreCheck.IsVisible == false)
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                mWindowPreCheck.SetImg(0);

                                mWindowPreCheck.Show();
                            }));
                        }

                        return false;
                    }

                    if (IsPassSecondPreCheck(lotID) == false)
                    {
                        if (mWindowPreCheck.IsVisible == false)
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                mWindowPreCheck.SetImg(1);

                                mWindowPreCheck.Show();
                            }));
                        }

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.Message);

                return false;
            }

            return true; // 착공 진행
        }

        private bool IsReliability()
        {
            if (CONFIG.EolInspType != InspectionType.EOL)
            {
                LogState(LogType.Info, "INSP Type is not EOL, Skip PreCheck()");

                return false;
            }

            if (IsManual == true)
            {
                LogState(LogType.Info, "INSP State is manual, Skip PreCheck()");

                return false;
            }

            if (Device.Cycler == null || Device.PLC == null || Device.KeysightDAQ == null)
            {
                LogState(LogType.Fail, "Need SW Debuging! obj is null, Skip PreCheck()");

                return false;
            }

            //사용 안함
            /*
            if (Device.Cycler.CyclerAlive == false || Device.PLC.isAlive == false || Device.KeysightDAQ.isAlive == false || Device.Tempr.GetTemp == -255)
            {
                LogState(LogType.Fail, "Device Not Ready"       +
                                       ", CyclerAlive:"         + Device.Cycler.CyclerAlive.ToString() +
                                       ", PLCAlive:"            + Device.PLC.isAlive.ToString() +
                                       ", KeysightDAQAlive:"    + Device.KeysightDAQ.isAlive.ToString() +
                                       ", Tempr:"               + Device.Tempr.GetTemp.ToString() +
                                       ", Skip PreCheck()");

                return false;
            }
            */

            return true;
        }

        private bool IsPassFisrtPreCheck(string lotID)
        {
            // 테스트 이후 함수로 정리 할 것

            LogState(LogType.Info, "Excute, FisrtPreCheck");

            #region Clear PreCheck Member

            mFisrtPreCheckReportCyclerVolt = 0.0d;

            mFisrtPreCheckReportDMMProbeVolt = 0.0d;

            mFisrtPreCheckReportDMMSensingVolt = 0.0d;

            #endregion----------------------------------------------------------------------------------------------

            #region Check Try Count

            //디폴트 항시 1회 무조건 진행으로 변경

            #endregion----------------------------------------------------------------------------------------------

            #region Check Cycler Module Voltage

            LogState(LogType.Info, "Start Check Cycler Module Voltage");

            /*
             *  need check !
            
            if (Device.Cycler._OpMode != PhoenixonLibrary.RECV_OPMode.READY_TO_INPUT) 
            {
                LogState(LogType.Info, "Now OPMode :" + Device.Cycler._OpMode.ToString());

                Device.Cycler.ChgDisControl.SetInputMcOn();

                DateTime startTime = DateTime.Now;

                while (true)
                {
                    Thread.Sleep(100);

                    TimeSpan timeSpan = DateTime.Now - startTime;

                    if (timeSpan.TotalMilliseconds > 1000 * 100) //10sec
                    {
                        LogState(LogType.Fail, "Time Out Input MC ON");

                        //return false;

                        break; // 비정상 케이스 발생시 밑에 AA 명령 시점 에서 후처리
                    }

                    if (Device.Cycler._OpMode == PhoenixonLibrary.RECV_OPMode.READY_TO_INPUT)
                    {
                        LogState(LogType.Info, "Now OPMode :" + Device.Cycler._OpMode.ToString());

                        break;
                    }
                }
            }
            */

            double cyclerVolt = 0.0d;

            cyclerVolt = Device.Cycler._Voltage;

            Device.Cycler.ChgDisControl.SetVoltageSensingOn(true);

            Thread.Sleep(1000);

            cyclerVolt = Device.Cycler._Voltage;

            LogState(LogType.Info, "Cycler Module Voltage:" + cyclerVolt.ToString());

            Device.Cycler.ChgDisControl.SetVoltageSensingOn(false);

            #endregion----------------------------------------------------------------------------------------------

            #region Judge Dev(Cycler Volt Min)

            double specCyclerVoltMin = 0.0d;

            Double.TryParse(Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\EOL_Trigger").GetValue("COMMON_FIRST_PRECHECK_CYCLER_VOLT_MIN").ToString()
                                                                                                              , out specCyclerVoltMin);

            LogState(LogType.Info, "Spec Volt Min:" + specCyclerVoltMin.ToString());

            if (cyclerVolt < specCyclerVoltMin)
            {
                LogState(LogType.Fail, "Cycler Voltage NG!");

                return false;
            }

            #endregion----------------------------------------------------------------------------------------------

            #region Check DMM Module Voltage

            LogState(LogType.Info, "Start Check DMM Module Voltage");

            double dmmSensingVolt = 0.0d;

            dmmSensingVolt = Device.KeysightDAQ.MeasVolt_Single(BatteryInfo.ModuleCH);

            Thread.Sleep(1000);
            
            #endregion----------------------------------------------------------------------------------------------

            #region Check DMM Module Voltage(Probe Voltage)

            double dmmProbeVolt = 0.0d;

            dmmProbeVolt = Device.KeysightDAQ.MeasVolt_Single(BatteryInfo.ProbeModuleVoltCH);

            #endregion----------------------------------------------------------------------------------------------

            #region Judge Dev(DMM)

            double dmmDevVolt = dmmSensingVolt - dmmProbeVolt;

            double specDevVoltMin = 0.0d;
            
            double specDevVoltMax = 0.0d;

            Double.TryParse(Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\EOL_Trigger").GetValue("COMMON_FIRST_PRECHECK_DEV_VOLT_MIN").ToString()
                                                                                                              , out specDevVoltMin);
            Double.TryParse(Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\EOL_Trigger").GetValue("COMMON_FIRST_PRECHECK_DEV_VOLT_MAX").ToString()
                                                                                                              , out specDevVoltMax);

            /*
            LogState(LogType.Info, "DMM Sensing Module Voltage:"  + dmmSensingVolt.ToString() +
                                               ", DMM Probe Module Voltage:"   + dmmProbeVolt.ToString() +
                                               ", DMM Dev Module Voltage:"     + dmmDevVolt.ToString());
            */

            LogState(LogType.Info, "Spec Dev Min:"   + specDevVoltMin.ToString() +
                                   ", Spec Dev Max:" + specDevVoltMax.ToString());

            LogState(LogType.Info, "DMM Sensing Module Voltage:" + dmmSensingVolt.ToString());
            LogState(LogType.Info, "DMM Probe Module Voltage:" + dmmProbeVolt.ToString());
            LogState(LogType.Info, "Dev Voltage:" + dmmDevVolt.ToString());
            

            if (dmmDevVolt < specDevVoltMin || dmmDevVolt > specDevVoltMax)
            {
                LogState(LogType.Fail, "DMM Dev Voltage NG!");

                return false;
            }

            LogState(LogType.Info, "Pass!");

            #endregion----------------------------------------------------------------------------------------------

            #region Get Report(MES) Detail Data (case only pass)

            mFisrtPreCheckReportCyclerVolt = cyclerVolt;

            mFisrtPreCheckReportDMMProbeVolt = dmmProbeVolt;

            mFisrtPreCheckReportDMMSensingVolt = dmmSensingVolt;

            #endregion----------------------------------------------------------------------------------------------

            return true;
        }


        private bool IsPassSecondPreCheck(string lotID)
        {
            LogState(LogType.Info, "Excute, SecondPreCheck");

            #region Check Try Count

            int preCheckMaxCount = 0;

            Int32.TryParse(Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\EOL_Trigger").GetValue("COMMON_SECOND_PRECHECK_TRY_CNT").ToString()
                                                                                                               , out preCheckMaxCount); // unit(count)
            if (mSecondPreCheckTraceLotID != lotID)
            {
                mSecondPreCheckTryCount = 0;

                mSecondPreCheckTraceLotID = lotID;
            }

            if (mSecondPreCheckTraceLotID == lotID)
            {
                mSecondPreCheckTryCount += 1;
            }

            LogState(LogType.Info, "Try Count:"     + mSecondPreCheckTryCount.ToString() +
                                   ", Max Count:"   + preCheckMaxCount.ToString());

            if (mSecondPreCheckTryCount > preCheckMaxCount)
            {
                LogState(LogType.Info, "Retry Exceeded !");

                return true;
            }

            #endregion----------------------------------------------------------------------------------------------

            #region Clear PreCheck Member

            mSecondPreCheckThermistorRes = 0.0d;

            mSecondPreCheckListCellVolt.Clear();

            mSecondPreCheckListCellRes.Clear();

            #endregion----------------------------------------------------------------------------------------------

            #region Measure

            mSecondPreCheckThermistorRes = double.Parse(Device.KeysightDAQ.MeasRes(BatteryInfo.ModuleResCH));

            LogState(LogType.Info, "ThermistorRes:" + mSecondPreCheckThermistorRes.ToString());

            Thread.Sleep(300);
            
            mSecondPreCheckListCellRes = Device.KeysightDAQ.MeasVolt_ResManual(BatteryInfo.CellCH);

            for (int cellNumber = 0; cellNumber < mSecondPreCheckListCellRes.Count; ++cellNumber)
            {

                LogState(LogType.Info, "CellNum:"   + (cellNumber + 1).ToString() +
                                       ", Res:"     + mSecondPreCheckListCellRes[cellNumber].ToString());
            }
            
            Thread.Sleep(300);
            
            mSecondPreCheckListCellVolt = Device.KeysightDAQ.MeasVolt_MultiCh(BatteryInfo.CellCH);

            for (int cellNumber = 0; cellNumber < mSecondPreCheckListCellVolt.Count; ++cellNumber)
            {
                LogState(LogType.Info, "CellNum:"   + (cellNumber + 1).ToString() +
                                       ", Volt:"    + mSecondPreCheckListCellVolt[cellNumber].ToString());
            }

            #endregion----------------------------------------------------------------------------------------------

            #region Judge

            const double specThermistorRes = 1000000.0d;

            const double specCellResMin = 650000.0d;

            const double specCellResMax = 750000.0d;

            if (mSecondPreCheckThermistorRes > specThermistorRes)
            {
                LogState(LogType.Info, "ThermistorRes NG!"      +
                                       ", SpecThermistorRes:"   + specThermistorRes.ToString());

                return false;
            }

            for (int cellNumber = 0; cellNumber < mSecondPreCheckListCellRes.Count; ++cellNumber)
            {
                if (mSecondPreCheckListCellRes[cellNumber] < specCellResMin || mSecondPreCheckListCellRes[cellNumber] > specCellResMax)
                {
                    LogState(LogType.Info, "Cell Res NG!"   +
                                           ", SpecResMin:"  + specCellResMin.ToString() +
                                           ", SpecResMax:"  + specCellResMax.ToString());

                    return false;
                }
            }

            // 전압 검사 액션은 따로 하지 않고 로깅만 하는 것으로 변경 (해당 구간에서 측정된 셀전압값을 검사항목에 엎어치는건 유효)

            LogState(LogType.Info, "Good!");

            return true;

            #endregion----------------------------------------------------------------------------------------------
        }
        //소모품 카운트 기능 nnkim
        public void ShowInspectionUIChange(int nIndex)
        {



            switch (nIndex)
            {
                case (int)InspectionType.HIPOT:

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
                case (int)InspectionType.EOL:
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
                if ((int)CONFIG.EolInspType == (int)InspectionType.EOL)
                {
                    //221012 스페어파츠카운트 설정하는 부분
                    FormPartsCountSetting newFormPartsCountSetting = new FormPartsCountSetting(this);
                    newFormPartsCountSetting.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    if (position == "#1") { newFormPartsCountSetting.Location = new System.Drawing.Point(700, 200); }
                    else { newFormPartsCountSetting.Location = new System.Drawing.Point(50, 200); }

                    newFormPartsCountSetting.ShowDialog();
                }
                else if ((int)CONFIG.EolInspType == (int)InspectionType.HIPOT)
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
            if ((int)CONFIG.EolInspType == (int)InspectionType.EOL)
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
            else if ((int)CONFIG.EolInspType == (int)InspectionType.HIPOT)
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
                System.Windows.MessageBox.Show("Please follow along and check. \nCMD -> Intl.cpl -> Please change the format to US.");
                System.Environment.Exit(0);
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
