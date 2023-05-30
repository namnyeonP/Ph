using PhoenixonLibrary.ETC;
using PhoenixonLibrary;
using PhoenixonLibrary.Device;
using Renault_BT6.모듈;
using System.Collections.Generic;
using Renault_BT6.Module;

namespace Renault_BT6
{
    public enum InspectionType
    {
        NONE,
        EOL,
        HIPOT,
        EOL_HIPOT
    }

    #region BT6 제어 항목 ID
    public class MesControlID
    {
        #region HIPOT MES Contorl ID
        public const string BushingPlusVolt = "WP2MMMTC1001";
        public const string BushingPlusTime = "WP2MMMTC1002";

        public const string CoolingPlatePlusVolt = "WP2MMMTC1003"; 
        public const string CoolingPlatePlusTime = "WP2MMMTC1004";

        public const string BushingMinusVolt = "WP2MMMTC1005";
        public const string BushingMinusTime = "WP2MMMTC1006"; 

        public const string CoolingPlateMinusVolt = "WP2MMMTC1007"; 
        public const string CoolingPlateMinusTime = "WP2MMMTC1008";
        public const string IR_RANGE = "WP2MMMTC1009";
        #endregion

        #region EOL MES Contorl ID
        public const string CellOpenUpper = "WP2MMMTC2001";
        public const string CellOpenLower = "WP2MMMTC2002";

        // rest
        public const string RestTimebeforeDischarge = "WP2MMMTC2003";

        // 방전
        public const string DischargeCurrent = "WP2MMMTC2004";
        public const string DischargeTime = "WP2MMMTC2005";
        public const string DischargeCurrentUpperLimit = "WP2MMMTC2006";

        // rest
        public const string RestTimeafterDischarge = "WP2MMMTC2007";

        // 충전
        public const string ChargeCurrent = "WP2MMMTC2008";
        public const string ChargeTime = "WP2MMMTC2009";
        public const string ChargeCurrentUpperLimit = "WP2MMMTC2010";

        // rest
        public const string RestTimeafterCharge = "WP2MMMTC2011";

        // 안전 조건 
        public const string SafetyVoltageUpperLimit = "WP2MMMTC2012";
        public const string SafetyVoltageLowerLimit = "WP2MMMTC2013";

        // Cell, Module 온도 보정식 계수
        public const string CellDCIR1 = "WP2MMMTC2014";
        public const string CellDCIR2 = "WP2MMMTC2015";
        public const string CellDCIR3 = "WP2MMMTC2018";

        public const string ModuleDCIR1 = "WP2MMMTC2016";
        public const string ModuleDCIR2 = "WP2MMMTC2017";
        public const string ModuleDCIR3 = "WP2MMMTC2019";

        public const string MES_CV_ORDER = "WP2MMMTC2020";
        #endregion
    }
    #endregion

    #region MES 수집 ID
    public class MesCollectionID
    {
        #region HIPOT 상세 수집 항목 ID
        public const string Contact_Bushing = "WP2MMMTQ1001";
        public const string Contact_CoolingPlate = "WP2MMMTQ1002";

        public const string IR_Plus_Bushing = "WP2MMMTQ1003";
        public const string IR_Plus_Cooling_Plate = "WP2MMMTQ1004";

        public const string IR_Minus_Bushing = "WP2MMMTQ1005";
        public const string IR_Minus_Cooling_Plate = "WP2MMMTQ1006";

        public const string Contact_Bushing2 = "WP2MMMTQ1007";
        public const string Contact_CoolingPlate2 = "WP2MMMTQ1008";
        #endregion
    }

    #endregion

    #region MES 상세 수집 ID
    public class MesDetailCollectionID
    {
        #region HIPOT 상세 수집 항목 ID
        public readonly string BushingPlusVolt = "WP2MMMTE1001";
        public readonly string BushingPlusTime = "WP2MMMTE1002";

        public readonly string CoolingPlusVolt = "WP2MMMTE1003";
        public readonly string CoolingPlusTime = "WP2MMMTE1004";

        public readonly string BushingMinusVolt = "WP2MMMTE1005";
        public readonly string BushingMinusTime = "WP2MMMTE1006";

        public readonly string CoolingMinusVolt = "WP2MMMTE1007";
        public readonly string CoolingMinusTime = "WP2MMMTE1008";

        public readonly string AmbientTempHiPot = "WP2MMMTE1033";


        public readonly string MES_CELL_DEV_LIST = "WP2MMMTE2038";
        public readonly string MES_CELL_ORDER = "WP2MMMTE2039";
        public readonly string MES_CELL_ID = "WP2MMMTE2040";
        public readonly string MES_CELL_FROM_LGQ = "WP2MMMTE2041";

        public List<string> BushingPlusResult = new List<string>();
        public List<string> CoolingPlusResult = new List<string>();

        public List<string> BushingMinusResult = new List<string>();
        public List<string> CoolingMinusResult = new List<string>();

        public void BushingPlusIdAdd()
        {
            // WP2MMMTE1009
            string id = "WP2MMMTE"; // 1009
            int number = 1009;
            string idTemp = "";

            for (int idx = 0; idx < 12; idx++)
            {
                idTemp = string.Format("{0}{1}", id, number);
                BushingPlusResult.Add(idTemp);
                number++;
            }
        }

        public void CoolingPlusIdAdd()
        {
            // WP2MMMTE1021
            string id = "WP2MMMTE"; // 1009
            int number = 1021;
            string idTemp = "";

            for (int idx = 0; idx < 12; idx++)
            {
                idTemp = string.Format("{0}{1}", id, number);
                CoolingPlusResult.Add(idTemp);
                number++;
            }
        }

        public void BushingMinusIdAdd()
        {
            // WP2MMMTE1034
            string id = "WP2MMMTE"; // 1009
            int number = 1034;
            string idTemp = "";

            for (int idx = 0; idx < 12; idx++)
            {
                idTemp = string.Format("{0}{1}", id, number);
                BushingMinusResult.Add(idTemp);
                number++;
            }
        }

        public void CoolingMinusIdAdd()
        {
            // WP2MMMTE1046
            string id = "WP2MMMTE"; // 1009
            int number = 1046;
            string idTemp = "";

            for (int idx = 0; idx < 12; idx++)
            {
                idTemp = string.Format("{0}{1}", id, number);
                CoolingMinusResult.Add(idTemp);
                number++;
            }
        }
        #endregion

        #region  DCIR 상세 수집 ID
        public readonly string RestTimeBeforeDischarge = "WP2MMMTE2001";

        public readonly string DischargeCurrent = "WP2MMMTE2002";
        public readonly string DischargeTime = "WP2MMMTE2003";
        public readonly string DischargeCurrentUpperLimit = "WP2MMMTE2004";

        public readonly string RestTimeAfterDischarge = "WP2MMMTE2005";

        public readonly string ChargeCurrent = "WP2MMMTE2006";
        public readonly string ChargeTime = "WP2MMMTE2007";
        public readonly string ChargeCurrentUpperLimit = "WP2MMMTE2008";

        public readonly string RestTimeafterCharge  = "WP2MMMTE2009";

        public readonly string SafetyVoltageUpperLimit = "WP2MMMTE2010";
        public readonly string SafetyVoltageLowLimit = "WP2MMMTE2011";

        public readonly string CellDCIR1 = "WP2MMMTE2012";
        public readonly string CellDCIR2 = "WP2MMMTE2013";

        public readonly string ModuleDCIR1 = "WP2MMMTE2014";
        public readonly string ModuleDCIR2 = "WP2MMMTE2015";
        public readonly string CellDCIR3 = "WP2MMMTE2036";

        public readonly string AmbientTemp = "WP2MMMTE2016";
        public readonly string ModuleTemp = "WP2MMMTE2017";
        public readonly string TempCheck2 = "WP2MMMTE2042";
        public readonly string ModuleDCIR3 = "WP2MMMTE2037";

        public readonly string CellVoltMax = "WP2MMMTE2018";
        public readonly string CellVoltMin = "WP2MMMTE2019";

        public readonly string AmbientTempBefore = "WP2MMMTE2020";
        public readonly string ModuleTempBefore = "WP2MMMTE2021";

        public readonly string AmbientTempAfter= "WP2MMMTE2022";
        public readonly string ModuleTempAfter = "WP2MMMTE2023";

        public readonly string BeforeDischargeCell = "WP2MMMTE2024";
        public readonly string DischargeCell = "WP2MMMTE2025";
        public readonly string BeforeChargeCell = "WP2MMMTE2026";
        public readonly string ChargeCell = "WP2MMMTE2027";
        public readonly string ChargeAfterCell = "WP2MMMTE2028";

        public readonly string MaxCellDCIR = "WP2MMMTE2029";
        public readonly string MinCellDCIR = "WP2MMMTE2030";


        public readonly string BeforeDischargeVolt = "WP2MMMTE2031";
        public readonly string DischargeVolt = "WP2MMMTE2032";
        public readonly string BeforeChargeVolt = "WP2MMMTE2033";
        public readonly string ChargeVolt = "WP2MMMTE2034";
        public readonly string ChargeAfterVolt = "WP2MMMTE2035";

        public readonly string CoumtCylcerMC = "WP2MMMTE2044";
        public readonly string PreCheckCyclerVolt = "WP2MMMTE2045";
        public readonly string PreCheckDMMSensingVolt = "WP2MMMTE2046";
        public readonly string PreCheckDMMProbeVolt = "WP2MMMTE2047";
        #endregion

        //221004 MC 저항 기능 nnkim
        #region MC_OPEN_CHECK_DETAIL_ITEM_EOL
        public readonly string KEY_EOL_MC_3_OPEN_CHECK = "MC3OPENCHECK";
        public readonly string KEY_EOL_MC_4_OPEN_CHECK = "MC4OPENCHECK";
        public readonly string KEY_EOL_MC_5_OPEN_CHECK = "MC5OPENCHECK";
        public readonly string KEY_EOL_MC_6_OPEN_CHECK = "MC6OPENCHECK";
        public readonly string KEY_EOL_MC_7_OPEN_CHECK = "MC7OPENCHECK";
        public readonly string KEY_EOL_MC_8_OPEN_CHECK = "MC8OPENCHECK";
        #endregion

        #region MC_HORT_CHECK_DETAIL_ITEM_EOL
        public readonly string KEY_EOL_MC_3_SHORT_CHECK = "MC3SHORTCHECK";
        public readonly string KEY_EOL_MC_4_SHORT_CHECK = "MC4SHORTCHECK";
        public readonly string KEY_EOL_MC_5_SHORT_CHECK = "MC5SHORTCHECK";
        public readonly string KEY_EOL_MC_6_SHORT_CHECK = "MC6SHORTCHECK";
        public readonly string KEY_EOL_MC_7_SHORT_CHECK = "MC7SHORTCHECK";
        public readonly string KEY_EOL_MC_8_SHORT_CHECK = "MC8SHORTCHECK";
        #endregion
    }
    #endregion

    /// <summary>
    ///  BT6 12S 
    /// </summary>
    class BatteryInfo
    {
        public const string ModelName = "BT6";

        public static int CellCount
        {
            get { return 12; }
        }

        // BT6 Module DAQ 채널 정보
        public const string CellCH = "101,102,103,104,105,106,107,108,109,110,111,112";
        public const string ModuleCH = "114";
        public const string ModuleResCH = "113";
        public const string ProbeModuleVoltCH = "120";
        public const string ModuleResMC = "115,116,117,118";
    }

    public class EOL
    {
        public static ChromaRecipe IR_Recipe = new ChromaRecipe();

        // MES control item 없으면 검사 항목 제거
        public static List<string> IR_CtrlItem = new List<string>();
        public static int CtrlItemOldCount = 0;

        public static int CellLineUpperLimit { get; set; }
        public static int CellLineLowLimit { get; set; }


        private static  MesDetailCollectionID _mes = new MesDetailCollectionID();
        public static MesDetailCollectionID MesID { get { return _mes; } }

        public static JsonParser JsonParser = new JsonParser();
        public static int RleayCount = 4;
    }

    public struct IR_Recipe
    {
        public int Volt { get; set; }  // 전압 설정

        public int Time { get; set; }  // 시간 설정

        public string Range { get; set; }
    }

    public class ChromaRecipe
    {
        #region HiPOT Channl List
        // 1 Plus
        // 2 Minus
        // 3 Bushing 상
        // 4 Bushing 하
        // 5 CoolingPlate 좌
        // 6 CoolingPlate 우
        #endregion

        public int PlusCH { get { return 1; } }
        public int MinusCH { get { return 2; } }
        public int BushingUpperCH { get { return 3; } }
        public int BushingLowCH { get { return 4; } }
        public int CoolingPlateRightCH { get { return 5; } }
        public int CoolingPlateLeftCH { get { return 6; } }

        public IR_Recipe BushingPlus = new IR_Recipe();
        public IR_Recipe CoolingPlatePlus = new IR_Recipe();

        public IR_Recipe BushingMinus = new IR_Recipe();
        public IR_Recipe CoolingPlateMinus = new IR_Recipe();


        public ChromaRecipe()
        {
            
        }
    }


    public class Device  //PhoenixonLibrary Device
    {
        public static RelayControl Relay { get; set; }

        // Cycler CAN, 사내 DAQ RS232 제어 
        public static PhoenixonCycler Cycler { get; set; } 

        public static Chroma_19053 Chroma { get; set; }

        public static Nht_RS232 Tempr { get; set; }

        public static CTempCT100 TemprCT100 { get; set; }

        //public static Keysight_34970A KeysightDAQ { get; set; }
        public static CKeysightDMM KeysightDAQ { get; set; }

        public static CPLC PLC { get; set; }

        public static CMES MES { get; set; }
    }


    struct CONFIG
    {
        // 검사기 종류EOL, HIPOT
        public static InspectionType EolInspType { get; set; }

        // Device 
        public static string Cycler { get; set; }
        public static string PhDAQ { get; set; }
        public static string KeysightDAQ { get; set; }
        public static string TEMP { get; set; }
        public static string CHROMA { get; set; }

        // PLC 
        public static string PlcNumber { get; set; }
        public static string SendAddr { get; set; }
        public static string RecvAddr { get; set; }
        public static string BcrAddr { get; set; }

        // MES
        public static string MesEquipID { get; set; }  // 설비 ID 
        public static string MesProcessID { get; set; } // 공정 ID
        public static string MesProductCode { get; set; } //제품 코드
        public static string MesIP { get; set; } // MES IP
        public static string MesPort { get; set; } // MES Port
        public static string MesUserID { get; set; } // MES User ID
    }

    struct RELAY
    {
        public static string LIGHT_RED { get; set; }
        public static string LIGHT_GREEN { get; set; }
        public static string LIGHT_YELLOW { get; set; }
        public static string LIGHT_BLUE { get; set; }
    }
    
    struct PATH
    {
        public const string JSON_SET = "Resources\\Setting.json";

        //public const string LOG = @"C:\Users\Public\EOL_INSPECTION_LOG";
        //public const string INSPECTION_RESULT = "c:\\Logs\\Inspection_result\\";

        public const string LOG = @"D:\EOL_INSPECTION_LOG";
        public const string INSPECTION_RESULT = "D:\\Inspection_result\\";

        public const string FileNameEOL = "eollist_eol.csv";
        public const string FileNameHIPOT = "eollist_hipot.csv";
    }


}//end
