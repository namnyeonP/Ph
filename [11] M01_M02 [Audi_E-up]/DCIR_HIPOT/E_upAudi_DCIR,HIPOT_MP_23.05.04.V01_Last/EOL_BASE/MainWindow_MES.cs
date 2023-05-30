using EOL_BASE.모듈;
using EOL_BASE.클래스;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        #region CONTROL ITEM

        #region PORSCHE_CONTROL_ITEM_VOLTAGE
        static string PORS_VOLT_ControlKey01 = "WP5MTC113";
        #endregion

        #region PORSCHE_CONTROL_ITEM_HIPOT
        static string PORS_HIPOT_ControlKey01 = "WMPMTC4201";
        static string PORS_HIPOT_ControlKey02 = "WMPMTC4202";
        static string PORS_HIPOT_ControlKey03 = "WMPMTC4203";
        static string PORS_HIPOT_ControlKey04 = "WMPMTC4204";
        static string PORS_HIPOT_ControlKey05 = "WMPMTC4205";
        static string PORS_HIPOT_ControlKey06 = "WMPMTC4206";
        static string PORS_HIPOT_ControlKey07 = "WMPMTC4207";
        static string PORS_HIPOT_ControlKey08 = "WMPMTC4208";
        #endregion

        #region PORSCHE_CONTROL_ITEM_HIPOT_NO_RESIN
        static string PORS_HIPNR_ControlKey01 = "WMPMTC4601";
        static string PORS_HIPNR_ControlKey02 = "WMPMTC4602";
        static string PORS_HIPNR_ControlKey03 = "WMPMTC4603";
        #endregion

        #region PORSCHE_CONTROL_ITEM_EOL
        static string PORS_EOL_ControlKey01 = "WMPMTC4401";
        static string PORS_EOL_ControlKey02 = "WMPMTC4402";
        static string PORS_EOL_ControlKey03 = "WMPMTC4403";
        static string PORS_EOL_ControlKey04 = "WMPMTC4404";
        static string PORS_EOL_ControlKey05 = "WMPMTC4405";
        static string PORS_EOL_ControlKey06 = "WMPMTC4406";
        static string PORS_EOL_ControlKey07 = "WMPMTC4407";
        static string PORS_EOL_ControlKey08 = "WMPMTC4408";
        static string PORS_EOL_ControlKey09 = "WMPMTC4409";
        static string PORS_EOL_ControlKey10 = "WMPMTC4410";
        static string PORS_EOL_ControlKey11 = "WMPMTC4411";
        static string PORS_EOL_ControlKey12 = "WMPMTC4412";
        static string PORS_EOL_ControlKey13 = "WMPMTC4413";
        static string PORS_EOL_ControlKey14 = "WMPMTC4414";
        static string PORS_EOL_ControlKey15 = "WMPMTC4415";
        #endregion


        #region AUDI_CONTROL_ITEM_VOLTAGE
        static string AUDI_VOLT_ControlKey1 = "WMAMTC4001";
        static string AUDI_VOLT_ControlKey2 = "WMAMTC4002";
        #endregion

        #region AUDI_CONTROL_ITEM_HIPOT
        static string AUDI_HIPOT_ControlKey01 = "WMAMTC4201";
        static string AUDI_HIPOT_ControlKey02 = "WMAMTC4202";
        static string AUDI_HIPOT_ControlKey03 = "WMAMTC4203";
        static string AUDI_HIPOT_ControlKey04 = "WMAMTC4204";
        static string AUDI_HIPOT_ControlKey05 = "WMAMTC4205";
        static string AUDI_HIPOT_ControlKey06 = "WMAMTC4206";
        static string AUDI_HIPOT_ControlKey07 = "WMAMTC4207";

        static string AUDI_HIPOT_ControlKey08 = "WMAMTC4208"; ////ir RANGE
        //static string AUDI_HIPOT_ControlKey09 = "WMAMTC4209"; ////dc RANGE
        #endregion

        #region AUDI_CONTROL_ITEM_EOL
        static string AUDI_EOL_ControlKey01 = "WMAMTC4401";
        static string AUDI_EOL_ControlKey02 = "WMAMTC4402";
        static string AUDI_EOL_ControlKey03 = "WMAMTC4403";
        static string AUDI_EOL_ControlKey04 = "WMAMTC4404";
        static string AUDI_EOL_ControlKey05 = "WMAMTC4405";
        static string AUDI_EOL_ControlKey06 = "WMAMTC4406";
        static string AUDI_EOL_ControlKey07 = "WMAMTC4407";
        static string AUDI_EOL_ControlKey08 = "WMAMTC4408";
        static string AUDI_EOL_ControlKey09 = "WMAMTC4409";
        static string AUDI_EOL_ControlKey10 = "WMAMTC4410";
        static string AUDI_EOL_ControlKey11 = "WMAMTC4411";
        static string AUDI_EOL_ControlKey12 = "WMAMTC4412";
        static string AUDI_EOL_ControlKey13 = "WMAMTC4413";
        static string AUDI_EOL_ControlKey14 = "WMAMTC4414";
        static string AUDI_EOL_ControlKey15 = "WMAMTC4415";
        static string AUDI_EOL_ControlKey16 = "WMAMTC4416";
        static string AUDI_EOL_ControlKey17 = "WMAMTC4417";
        static string AUDI_EOL_ControlKey18 = "WMAMTC4418";
        static string AUDI_EOL_ControlKey19 = "WMAMTC4419";
        #endregion


        #region E_UP_CONTROL_ITEM_VOLTAGE
        static string E_UP_VOLT_ControlKey1 = "W2M1MTC4001";
        static string E_UP_VOLT_ControlKey2 = "W2M1MTC4002";
        #endregion

        #region E_UP_CONTROL_ITEM_HIPOT
        static string E_UP_HIPOT_ControlKey01 = "W2M1MTC4201";//ir volt
        static string E_UP_HIPOT_ControlKey02 = "W2M1MTC4202";//ir time
        static string E_UP_HIPOT_ControlKey03 = "W2M1MTC4203";//wi volt
        static string E_UP_HIPOT_ControlKey04 = "W2M1MTC4204";//wi time
        static string E_UP_HIPOT_ControlKey05 = "W2M1MTC4205";//wi rampup
        static string E_UP_HIPOT_ControlKey06 = "W2M1MTC4206";//arc
        static string E_UP_HIPOT_ControlKey07 = "W2M1MTC4207";//arc limit
        static string E_UP_HIPOT_ControlKey08 = "W2M1MTC4208";//ir RANGE
        #endregion

        #region E_UP_CONTROL_ITEM_EOL
        static string E_UP_EOL_ControlKey01 = "W2M1MTC4401"; //Rest Time before discharge 
        static string E_UP_EOL_ControlKey02 = "W2M1MTC4402"; //Discharge Current 
        static string E_UP_EOL_ControlKey03 = "W2M1MTC4403"; //Discharge Time 
        static string E_UP_EOL_ControlKey04 = "W2M1MTC4404"; //Discharge Current Upper Limit 
        static string E_UP_EOL_ControlKey05 = "W2M1MTC4405"; //Rest Time after discharge 
        static string E_UP_EOL_ControlKey06 = "W2M1MTC4406"; //Charge Current 
        static string E_UP_EOL_ControlKey07 = "W2M1MTC4407"; //Charge Time 
        static string E_UP_EOL_ControlKey08 = "W2M1MTC4408"; //Charge Current Upper Limit 
        static string E_UP_EOL_ControlKey09 = "W2M1MTC4409"; //Rest Time after charge 
        static string E_UP_EOL_ControlKey10 = "W2M1MTC4410"; //Safety Voltage Upper Limit 
        static string E_UP_EOL_ControlKey11 = "W2M1MTC4411"; //Safety Voltage Lower Limit 
        static string E_UP_EOL_ControlKey12 = "W2M1MTC4412"; //Cell DCIR 온도보정식 계수1 
        static string E_UP_EOL_ControlKey13 = "W2M1MTC4413"; //Cell DCIR 온도보정식 계수2 
        static string E_UP_EOL_ControlKey14 = "W2M1MTC4414"; //Module DCIR 온도보정식 계수1 
        static string E_UP_EOL_ControlKey15 = "W2M1MTC4415"; //Module DCIR 온도보정식 계수2 
        static string E_UP_EOL_ControlKey16 = "W2M1MTC4416"; //Cell DCIR 온도보정식 계수3 
        static string E_UP_EOL_ControlKey17 = "W2M1MTC4417"; //Module DCIR 온도보정식 계수3 
        static string E_UP_EOL_ControlKey18 = "W2M1MTC4418";
        static string E_UP_EOL_ControlKey19 = "W2M1MTC4419";
        #endregion


        //201217 wjs add maserati control item key
        #region MASERATI_CONTROL_ITEM_VOLTAGE
        static string MAS_VOLT_ControlKey01 = "WMPMTC4101";
        #endregion

        #region MASERATI_CONTROL_ITEM_HIPOT
        static string MAS_HIPOT_ControlKey01 = "WMPMTC4301";
        static string MAS_HIPOT_ControlKey02 = "WMPMTC4302";
        static string MAS_HIPOT_ControlKey03 = "WMPMTC4303";
        static string MAS_HIPOT_ControlKey04 = "WMPMTC4304";
        static string MAS_HIPOT_ControlKey05 = "WMPMTC4305";
        static string MAS_HIPOT_ControlKey06 = "WMPMTC4306";
        static string MAS_HIPOT_ControlKey07 = "WMPMTC4307";
        static string MAS_HIPOT_ControlKey08 = "WMPMTC4308";
        #endregion

        #region MASERATI_CONTROL_ITEM_HIPOT_NO_RESIN
        static string MAS_HIPNR_ControlKey01 = "WMPMTC4701";
        static string MAS_HIPNR_ControlKey02 = "WMPMTC4702";
        static string MAS_HIPNR_ControlKey03 = "WMPMTC4703";
        #endregion

        #region MASERATI_CONTROL_ITEM_EOL
        static string MAS_EOL_ControlKey01 = "WMPMTC4501";
        static string MAS_EOL_ControlKey02 = "WMPMTC4502";
        static string MAS_EOL_ControlKey03 = "WMPMTC4503";
        static string MAS_EOL_ControlKey04 = "WMPMTC4504";
        static string MAS_EOL_ControlKey05 = "WMPMTC4505";
        static string MAS_EOL_ControlKey06 = "WMPMTC4506";
        static string MAS_EOL_ControlKey07 = "WMPMTC4507";
        static string MAS_EOL_ControlKey08 = "WMPMTC4508";
        static string MAS_EOL_ControlKey09 = "WMPMTC4509";
        static string MAS_EOL_ControlKey10 = "WMPMTC4510";
        static string MAS_EOL_ControlKey11 = "WMPMTC4511";
        static string MAS_EOL_ControlKey12 = "WMPMTC4512";
        static string MAS_EOL_ControlKey13 = "WMPMTC4513";
        static string MAS_EOL_ControlKey14 = "WMPMTC4514";
        static string MAS_EOL_ControlKey15 = "WMPMTC4515";
        #endregion


        #region PORSCHE_FACELIFT_CONTROL_ITEM_VOLTAGE
        static string PORS_FL_VOLT_ControlKey01 = "WPFMMTC4001";
        #endregion

        #region PORSCHE_FACELIFT_CONTROL_ITEM_HIPOT
        static string PORS_FL_HIPOT_ControlKey01 = "WPFMMTC4201";
        static string PORS_FL_HIPOT_ControlKey02 = "WPFMMTC4202";
        static string PORS_FL_HIPOT_ControlKey03 = "WPFMMTC4203";
        static string PORS_FL_HIPOT_ControlKey04 = "WPFMMTC4204";
        static string PORS_FL_HIPOT_ControlKey05 = "WPFMMTC4205";
        static string PORS_FL_HIPOT_ControlKey06 = "WPFMMTC4206";
        static string PORS_FL_HIPOT_ControlKey07 = "WPFMMTC4207";
        static string PORS_FL_HIPOT_ControlKey08 = "WPFMMTC4208";

        #endregion

        #region PORSCHE_FACELIFT_CONTROL_ITEM_HIPOT_NO_RESIN
        static string PORS_FL_HIPNR_ControlKey01 = "WPFMMTC4501";
        static string PORS_FL_HIPNR_ControlKey02 = "WPFMMTC4502";
        static string PORS_FL_HIPNR_ControlKey03 = "WPFMMTC4503";
        #endregion

        #region PORSCHE_FACELIFT_CONTROL_ITEM_EOL
        static string PORS_FL_EOL_ControlKey01 = "WPFMMTC4401";
        static string PORS_FL_EOL_ControlKey02 = "WPFMMTC4402";
        static string PORS_FL_EOL_ControlKey03 = "WPFMMTC4403";
        static string PORS_FL_EOL_ControlKey04 = "WPFMMTC4404";
        static string PORS_FL_EOL_ControlKey05 = "WPFMMTC4405";
        static string PORS_FL_EOL_ControlKey06 = "WPFMMTC4406";
        static string PORS_FL_EOL_ControlKey07 = "WPFMMTC4407";
        static string PORS_FL_EOL_ControlKey08 = "WPFMMTC4408";
        static string PORS_FL_EOL_ControlKey09 = "WPFMMTC4409";
        static string PORS_FL_EOL_ControlKey10 = "WPFMMTC4410";
        static string PORS_FL_EOL_ControlKey11 = "WPFMMTC4411";
        static string PORS_FL_EOL_ControlKey12 = "WPFMMTC4412";
        static string PORS_FL_EOL_ControlKey13 = "WPFMMTC4413";
        static string PORS_FL_EOL_ControlKey14 = "WPFMMTC4414";
        static string PORS_FL_EOL_ControlKey15 = "WPFMMTC4415";
        #endregion

        //221101 wjs add mase m183
        #region MASERATI_M183_CONTROL_ITEM_VOLTAGE
        static string MAS_M183_VOLT_ControlKey01 = "WM183MMTC4001";
        #endregion

        #region MASERATI_M183_CONTROL_ITEM_HIPOT
        static string MAS_M183_HIPOT_ControlKey01 = "WM183MMTC4201";
        static string MAS_M183_HIPOT_ControlKey02 = "WM183MMTC4202";
        static string MAS_M183_HIPOT_ControlKey03 = "WM183MMTC4203";
        static string MAS_M183_HIPOT_ControlKey04 = "WM183MMTC4204";
        static string MAS_M183_HIPOT_ControlKey05 = "WM183MMTC4205";
        static string MAS_M183_HIPOT_ControlKey06 = "WM183MMTC4206";
        static string MAS_M183_HIPOT_ControlKey07 = "WM183MMTC4207";
        static string MAS_M183_HIPOT_ControlKey08 = "WM183MMTC4208";

        #endregion

        #region MASERATI_M183_CONTROL_ITEM_HIPOT_NO_RESIN
        static string MAS_M183_HIPNR_ControlKey01 = "WM183MMTC4501";
        static string MAS_M183_HIPNR_ControlKey02 = "WM183MMTC4502";
        static string MAS_M183_HIPNR_ControlKey03 = "WM183MMTC4503";
        #endregion

        #region MASERATI_M183_CONTROL_ITEM_EOL
        static string MAS_M183_EOL_ControlKey01 = "WM183MMTC4401";
        static string MAS_M183_EOL_ControlKey02 = "WM183MMTC4402";
        static string MAS_M183_EOL_ControlKey03 = "WM183MMTC4403";
        static string MAS_M183_EOL_ControlKey04 = "WM183MMTC4404";
        static string MAS_M183_EOL_ControlKey05 = "WM183MMTC4405";
        static string MAS_M183_EOL_ControlKey06 = "WM183MMTC4406";
        static string MAS_M183_EOL_ControlKey07 = "WM183MMTC4407";
        static string MAS_M183_EOL_ControlKey08 = "WM183MMTC4408";
        static string MAS_M183_EOL_ControlKey09 = "WM183MMTC4409";
        static string MAS_M183_EOL_ControlKey10 = "WM183MMTC4410";
        static string MAS_M183_EOL_ControlKey11 = "WM183MMTC4411";
        static string MAS_M183_EOL_ControlKey12 = "WM183MMTC4412";
        static string MAS_M183_EOL_ControlKey13 = "WM183MMTC4413";
        static string MAS_M183_EOL_ControlKey14 = "WM183MMTC4414";
        static string MAS_M183_EOL_ControlKey15 = "WM183MMTC4415";
        #endregion

        #endregion

        #region DETAIL ITEM

        #region PORSCHE_DETAIL_ITEM_EOL

        #region Data
        string KEY_EOL_POR_NOR_DATA__CMC_GUID = "WMPMTE4401";
        string KEY_EOL_POR_NOR_AMBIENT___TEMP = "WMPMTE4402";
        string KEY_EOL_POR_NOR_TEMP1______ADC = "WMPMTE4403";
        string KEY_EOL_POR_NOR_TEMP2______ADC = "WMPMTE4404";
        string KEY_EOL_POR_NOR_PCB___TEMP_ADC = "WMPMTE4405";
        string KEY_EOL_POR_NOR_CELL_V_DEV_MAX = "WMPMTE4406";
        string KEY_EOL_POR_NOR_CELL_V_DEV_MIN = "WMPMTE4407";
        string KEY_EOL_POR_NOR_MODULE____VOLT = "WMPMTE4408";
        string KEY_EOL_POR_NOR_BAL__OP_SWITCH = "WMPMTE4422";
        string KEY_EOL_POR_NOR_OVER___V_FAULT = "WMPMTE4423";
        string KEY_EOL_POR_NOR_UNDER__V_FAULT = "WMPMTE4424";
        string KEY_EOL_POR_NOR_CMC____VERSION = "WMPMTE4440";
        string KEY_EOL_POR_NOR_HI__CONT_S_BEF = "WMPMTE4441";
        string KEY_EOL_POR_NOR_HI__CONT_S_AFT = "WMPMTE4442";
        string KEY_EOL_POR_NOR_TEMP_NEW_CHECK = "WMPMTE4443";
        string KEY_EOL_POR_NOR_BAL__ST_SWITCH = "WMPMTE4444";
        string KEY_EOL_POR_NOR_CELL_VOL_VERFY = "WMPMTE4445";
        string KEY_EOL_POR_NOR_CELL_TEMN_LK_1 = "WMPMTE4446";
        string KEY_EOL_POR_NOR_CELL_TEMN_LK_2 = "WMPMTE4447";
        string KEY_EOL_POR_NOR_CELL_STAK_COPR = "WMPMTE4448";
        string KEY_EOL_POR_NOR_IC_TEMP_TSHORT = "WMPMTE4449";
        #endregion

        #region in DCIR
        string KEY_EOL_POR_NOR_DCIR_BEF_AMB_T = "WMPMTE4409";
        string KEY_EOL_POR_NOR_DCIR_TEMP1_ADC = "WMPMTE4410";
        string KEY_EOL_POR_NOR_DCIR_TEMP2_ADC = "WMPMTE4411";
        string KEY_EOL_POR_NOR_DCIR_CVBEF_DIS = "WMPMTE4412";
        string KEY_EOL_POR_NOR_DCIR_CV_DISCHA = "WMPMTE4413";
        string KEY_EOL_POR_NOR_DCIR_CVBEF_CHA = "WMPMTE4414";
        string KEY_EOL_POR_NOR_DCIR_CV_CHARGE = "WMPMTE4415";
        string KEY_EOL_POR_NOR_DCIR_CVAFT_CHA = "WMPMTE4416";
        string KEY_EOL_POR_NOR_DCIR_MVBEF_DIS = "WMPMTE4417";
        string KEY_EOL_POR_NOR_DCIR_MV_DISCHA = "WMPMTE4418";
        string KEY_EOL_POR_NOR_DCIR_MVBEF_CHA = "WMPMTE4419";
        string KEY_EOL_POR_NOR_DCIR_MV_CHARGE = "WMPMTE4420";
        string KEY_EOL_POR_NOR_DCIR_MVAFT_CHA = "WMPMTE4421";
        string KEY_EOL_POR_NOR_DCIR_500MS_DIS = "WMPMTE4450";
        #endregion

        //221004 수집 항목
        #region MC_OPEN_CHECK_DETAIL_ITEM_EOL
        string KEY_EOL_MC_3_OPEN_CHECK = "MC3OPENCHECK";
        string KEY_EOL_MC_4_OPEN_CHECK = "MC4OPENCHECK";
        string KEY_EOL_MC_5_OPEN_CHECK = "MC5OPENCHECK";
        string KEY_EOL_MC_6_OPEN_CHECK = "MC6OPENCHECK";
        string KEY_EOL_MC_7_OPEN_CHECK = "MC7OPENCHECK";
        string KEY_EOL_MC_8_OPEN_CHECK = "MC8OPENCHECK";
        string KEY_EOL_MC_9_OPEN_CHECK = "MC9OPENCHECK";
        string KEY_EOL_MC_10_OPEN_CHECK = "MC10OPENCHECK";
        string KEY_EOL_MC_11_OPEN_CHECK = "MC11OPENCHECK";
        string KEY_EOL_MC_12_OPEN_CHECK = "MC12OPENCHECK";
        #endregion

        #region MC_HORT_CHECK_DETAIL_ITEM_EOL
        string KEY_EOL_MC_3_SHORT_CHECK = "MC3SHORTCHECK";
        string KEY_EOL_MC_4_SHORT_CHECK = "MC4SHORTCHECK";
        string KEY_EOL_MC_5_SHORT_CHECK = "MC5SHORTCHECK";
        string KEY_EOL_MC_6_SHORT_CHECK = "MC6SHORTCHECK";
        string KEY_EOL_MC_7_SHORT_CHECK = "MC7SHORTCHECK";
        string KEY_EOL_MC_8_SHORT_CHECK = "MC8SHORTCHECK";
        string KEY_EOL_MC_9_SHORT_CHECK = "MC9SHORTCHECK";
        string KEY_EOL_MC_10_SHORT_CHECK = "MC10SHORTCHECK";
        string KEY_EOL_MC_11_SHORT_CHECK = "MC11SHORTCHECK";
        string KEY_EOL_MC_12_SHORT_CHECK = "MC12SHORTCHECK";
        #endregion

        #region Report Spec
        string KEY_EOL_POR_NOR_SPEC_BEF_DIS_REST_T = "WMPMTE4425";
        string KEY_EOL_POR_NOR_SPEC_DISCHARGE_CURR = "WMPMTE4426";
        string KEY_EOL_POR_NOR_SPEC_DISCHARGE_TIME = "WMPMTE4427";
        string KEY_EOL_POR_NOR_SPEC_DIS_CURR_LIMIT = "WMPMTE4428";
        string KEY_EOL_POR_NOR_SPEC_AFT_DIS_REST_T = "WMPMTE4429";
        string KEY_EOL_POR_NOR_SPEC_CHARGE____CURR = "WMPMTE4430";
        string KEY_EOL_POR_NOR_SPEC_CHARGE____TIME = "WMPMTE4431";
        string KEY_EOL_POR_NOR_SPEC_CHA_CURR_LIMIT = "WMPMTE4432";
        string KEY_EOL_POR_NOR_SPEC_AFT_CHA_RES__T = "WMPMTE4433";
        string KEY_EOL_POR_NOR_SPEC_SAFE_VOL_H_LIM = "WMPMTE4434";
        string KEY_EOL_POR_NOR_SPEC_SAFE_VOL_L_LIM = "WMPMTE4435";
        string KEY_EOL_POR_NOR_SPEC_CELL_FOMULA__1 = "WMPMTE4436";
        string KEY_EOL_POR_NOR_SPEC_CELL_FOMULA__2 = "WMPMTE4437";
        string KEY_EOL_POR_NOR_SPEC_MODULE_FOMULA1 = "WMPMTE4438";
        string KEY_EOL_POR_NOR_SPEC_MODULE_FOMULA2 = "WMPMTE4439";
        #endregion

        #endregion

        #region PORSCHE_DETAIL_ITEM_HIPOT
        string KEY_HIP_POR_NOR_PLUS__IR_LEVEL = "WMPMTE4201";
        string KEY_HIP_POR_NOR_PLUS___IR_TIME = "WMPMTE4202";
        string KEY_HIP_POR_NOR_PLUS_WI__LEVEL = "WMPMTE4203";
        string KEY_HIP_POR_NOR_PLUS_WI___TIME = "WMPMTE4204";
        string KEY_HIP_POR_NOR_PLUS_WI_RAMP_T = "WMPMTE4205";
        string KEY_HIP_POR_NOR_PLUS_WI_ARC_ON = "WMPMTE4206";
        string KEY_HIP_POR_NOR_PLUS_WI_ARC_LM = "WMPMTE4207";
        string KEY_HIP_POR_NOR_PLUS_IR__RANGE = "WMPMTE4208"; //ir RANGE
        //string KEY_HIP_POR_NOR_PLUS_WI__RANGE = "WMPMTE4209"; //dc RANGE
        #endregion

        #region PORSCHE_DETAIL_ITEM_HIPOT_NO_RESIN
        string KEY_HNR_POR_NOR_PLUS__IR_LEVEL = "WMPMTE4601";
        string KEY_HNR_POR_NOR_PLUS___IR_TIME = "WMPMTE4602";
        string KEY_HNR_POR_NOR_PLUS_IR__RANGE = "WMPMTE4603";
        string KEY_HIP_POR_NOR_FETCH_IR_OUTVOLT = "WMPMTE4604";
        string KEY_HIP_POR_NOR_FETCH_IR_TIME = "WMPMTE4605";
        string KEY_HIP_POR_NOR_FETCH_IR_MEASURE = "WMPMTE4606";
        #endregion

        #region PORSCHE_DETAIL_ITEM_VOLTAGE
        string KEY_VOL_POR_NOR_DATA__CMC_GUID = "WMPMTE4001";
        string KEY_VOL_POR_NOR_AMBIENT___TEMP = "WMPMTE4002";
        string KEY_VOL_POR_NOR_TEMP1______ADC = "WMPMTE4003";
        string KEY_VOL_POR_NOR_TEMP2______ADC = "WMPMTE4004";
        string KEY_VOL_POR_NOR_PCB___TEMP_ADC = "WMPMTE4005";
        string KEY_VOL_POR_NOR_CELL_V_DEV_MAX = "WMPMTE4006";
        string KEY_VOL_POR_NOR_CELL_V_DEV_MIN = "WMPMTE4007";
        string KEY_VOL_POR_NOR_MODULE____VOLT = "WMPMTE4008";
        string KEY_VOL_POR_NOR_BAL__OP_SWITCH = "WMPMTE4009";
        string KEY_VOL_POR_NOR_OVER___V_FAULT = "WMPMTE4010";
        string KEY_VOL_POR_NOR_UNDER__V_FAULT = "WMPMTE4011";
        string KEY_VOL_POR_NOR_CMC____VERSION = "WMPMTE4012";
        string KEY_VOL_POR_NOR_HI__CONT_S_BEF = "WMPMTE4013";
        string KEY_VOL_POR_NOR_HI__CONT_S_AFT = "WMPMTE4014";
        string KEY_VOL_POR_NOR_DEV_VOLT__REAL = "WMPMTE4015";
        string KEY_VOL_POR_NOR_DEV_VOLT___BCR = "WMPMTE4016";
        #endregion


        #region AUDI_DETAIL_ITEM_EOL

        #region Data
        string KEY_EOL_AUD_NOR_AMBIENT___TEMP = "WMAMTE4401";
        string KEY_EOL_AUD_NOR_TEMP1_____TEMP = "WMAMTE4402";
        string KEY_EOL_AUD_NOR_TEMP2_____TEMP = "WMAMTE4403";
        string KEY_EOL_AUD_NOR_LINE__OP_UPPER = "WMAMTE4434";
        string KEY_EOL_AUD_NOR_LINE__OP_LOWER = "WMAMTE4435";
        string KEY_EOL_AUD_NOR_LINE_OP_RESIST = "WMAMTE4436";
        #endregion

        #region in DCIR
        string KEY_EOL_AUD_NOR_DCIR_BEF_AMB_T = "WMAMTE4404";
        string KEY_EOL_AUD_NOR_DCIR_TEMP1_ADC = "WMAMTE4405";
        string KEY_EOL_AUD_NOR_DCIR_TEMP2_ADC = "WMAMTE4406";

        string KEY_EOL_AUD_NOR_DCIR_CVBEF_DIS = "WMAMTE4407";
        string KEY_EOL_AUD_NOR_DCIR_CV_DISCHA = "WMAMTE4408";
        string KEY_EOL_AUD_NOR_DCIR_CVBEF_CHA = "WMAMTE4409";
        string KEY_EOL_AUD_NOR_DCIR_CV_CHARGE = "WMAMTE4410";
        string KEY_EOL_AUD_NOR_DCIR_CVAFT_CHA = "WMAMTE4411";
        string KEY_EOL_AUD_NOR_DCIR_MVBEF_DIS = "WMAMTE4412";
        string KEY_EOL_AUD_NOR_DCIR_MV_DISCHA = "WMAMTE4413";
        string KEY_EOL_AUD_NOR_DCIR_MVBEF_CHA = "WMAMTE4414";
        string KEY_EOL_AUD_NOR_DCIR_MV_CHARGE = "WMAMTE4415";
        string KEY_EOL_AUD_NOR_DCIR_MVAFT_CHA = "WMAMTE4416"; 
        string KEY_EOL_AUD_NOR_DCIR_500MS_DIS = "WMAMTE4440";
        #endregion

        #region Report Control Spec
        string KEY_EOL_AUD_NOR_SPEC_BEF_DIS_REST_T = "WMAMTE4417";
        string KEY_EOL_AUD_NOR_SPEC_DISCHARGE_CURR = "WMAMTE4418";
        string KEY_EOL_AUD_NOR_SPEC_DISCHARGE_TIME = "WMAMTE4419";
        string KEY_EOL_AUD_NOR_SPEC_DIS_CURR_LIMIT = "WMAMTE4420";
        string KEY_EOL_AUD_NOR_SPEC_AFT_DIS_REST_T = "WMAMTE4421";
        string KEY_EOL_AUD_NOR_SPEC_CHARGE____CURR = "WMAMTE4422";
        string KEY_EOL_AUD_NOR_SPEC_CHARGE____TIME = "WMAMTE4423";
        string KEY_EOL_AUD_NOR_SPEC_CHA_CURR_LIMIT = "WMAMTE4424";
        string KEY_EOL_AUD_NOR_SPEC_AFT_CHA_RES__T = "WMAMTE4425";
        string KEY_EOL_AUD_NOR_SPEC_SAFE_VOL_H_LIM = "WMAMTE4426";
        string KEY_EOL_AUD_NOR_SPEC_SAFE_VOL_L_LIM = "WMAMTE4427";
        string KEY_EOL_AUD_NOR_SPEC_CELL_FOMULA__1 = "WMAMTE4428";
        string KEY_EOL_AUD_NOR_SPEC_CELL_FOMULA__2 = "WMAMTE4429";
        string KEY_EOL_AUD_NOR_SPEC_MODULE_FOMULA1 = "WMAMTE4430";
        string KEY_EOL_AUD_NOR_SPEC_MODULE_FOMULA2 = "WMAMTE4431";
        string KEY_EOL_AUD_NOR_SPEC_CELL_FOMULA__3 = "WMAMTE4432";
        string KEY_EOL_AUD_NOR_SPEC_MODULE_FOMULA3 = "WMAMTE4433";


        #endregion

        #region TEMP CHECK
        string KEY_EOL_AUD_TEMP_NEW_CHECK2 = "WMAMTE4437";
        #endregion

        #endregion

        #region AUDI_DETAIL_ITEM_HIPOT
        string KEY_HIP_AUD_NOR_PLUS__IR_LEVEL = "WMAMTE4201";
        string KEY_HIP_AUD_NOR_PLUS___IR_TIME = "WMAMTE4202";
        string KEY_HIP_AUD_NOR_PLUS_WI__LEVEL = "WMAMTE4203";
        string KEY_HIP_AUD_NOR_PLUS_WI___TIME = "WMAMTE4204";
        string KEY_HIP_AUD_NOR_PLUS_WI_RAMP_T = "WMAMTE4205";
        string KEY_HIP_AUD_NOR_PLUS_WI_ARC_ON = "WMAMTE4206";
        string KEY_HIP_AUD_NOR_PLUS_WI_ARC_LM = "WMAMTE4207";

        string KEY_HIP_AUD_NOR_PLUS_IR__RANGE = "WMAMTE4208"; //ir RANGE
        //string KEY_HIP_AUD_NOR_PLUS_WI__RANGE = "WMAMTE4209"; //dc RANGE
        #endregion

        #region AUDI_DETAIL_ITEM_VOLTAGE
        string KEY_VOL_AUD_NOR_TEMP1_____TEMP = "WMAMTE4001";
        string KEY_VOL_AUD_NOR_TEMP2_____TEMP = "WMAMTE4002";
        string KEY_VOL_AUD_NOR_AMBIENT___TEMP = "WMAMTE4003";
        string KEY_VOL_AUD_NOR_LINE__OP_UPPER = "WMAMTE4004";
        string KEY_VOL_AUD_NOR_LINE__OP_LOWER = "WMAMTE4005";
        string KEY_VOL_AUD_NOR_LINE_OP_RESIST = "WMAMTE4006";
        #endregion

        #region AUDI_DETAIL_CELL DEV
        string KEY_EOL_AUDI_CELL_VOLT = "WMAMTE4438";
        string KEY_EOL_AUDI_CALC_CELL_VOLT = "WMAMTE4439";
        #endregion


        #region E_UP_DETAIL_ITEM_EOL

        #region Data
        string KEY_EOL_E_UP_AMBIENT___TEMP = "W2M1MTE4401";
        string KEY_EOL_E_UP_TEMP1_____TEMP = "W2M1MTE4402";
        string KEY_EOL_E_UP_TEMP2_____TEMP = "W2M1MTE4403";
        string KEY_EOL_E_UP_LINE__OP_UPPER = "W2M1MTE4434";
        string KEY_EOL_E_UP_LINE__OP_LOWER = "W2M1MTE4435";
        string KEY_EOL_E_UP_LINE_OP_RESIST = "W2M1MTE4436";

        #endregion

        #region in DCIR
        string KEY_EOL_E_UP_DCIR_BEF_AMB_T = "W2M1MTE4404";
        string KEY_EOL_E_UP_DCIR_TEMP1_ADC = "W2M1MTE4405";
        string KEY_EOL_E_UP_DCIR_TEMP2_ADC = "W2M1MTE4406";

        string KEY_EOL_E_UP_DCIR_CVBEF_DIS = "W2M1MTE4407";
        string KEY_EOL_E_UP_DCIR_CV_DISCHA = "W2M1MTE4408";
        string KEY_EOL_E_UP_DCIR_CVBEF_CHA = "W2M1MTE4409";
        string KEY_EOL_E_UP_DCIR_CV_CHARGE = "W2M1MTE4410";
        string KEY_EOL_E_UP_DCIR_CVAFT_CHA = "W2M1MTE4411";

        string KEY_EOL_E_UP_DCIR_MVBEF_DIS = "W2M1MTE4412";
        string KEY_EOL_E_UP_DCIR_MV_DISCHA = "W2M1MTE4413";
        string KEY_EOL_E_UP_DCIR_MVBEF_CHA = "W2M1MTE4414";
        string KEY_EOL_E_UP_DCIR_MV_CHARGE = "W2M1MTE4415";
        string KEY_EOL_E_UP_DCIR_MVAFT_CHA = "W2M1MTE4416"; 
        string KEY_EOL_E_UP_DCIR_500MS_DIS = "W2M1MTE4440";
        string KEY_EOL_E_UP_LINE_THERMSITORSHORTCHECK = "W2M1MTE4441";

        #endregion

        #region Report Control Spec
        string KEY_EOL_E_UP_SPEC_BEF_DIS_REST_T = "W2M1MTE4417";
        string KEY_EOL_E_UP_SPEC_DISCHARGE_CURR = "W2M1MTE4418";
        string KEY_EOL_E_UP_SPEC_DISCHARGE_TIME = "W2M1MTE4419";
        string KEY_EOL_E_UP_SPEC_DIS_CURR_LIMIT = "W2M1MTE4420";
        string KEY_EOL_E_UP_SPEC_AFT_DIS_REST_T = "W2M1MTE4421";
        string KEY_EOL_E_UP_SPEC_CHARGE____CURR = "W2M1MTE4422";
        string KEY_EOL_E_UP_SPEC_CHARGE____TIME = "W2M1MTE4423";
        string KEY_EOL_E_UP_SPEC_CHA_CURR_LIMIT = "W2M1MTE4424";
        string KEY_EOL_E_UP_SPEC_AFT_CHA_RES__T = "W2M1MTE4425";
        string KEY_EOL_E_UP_SPEC_SAFE_VOL_H_LIM = "W2M1MTE4426";
        string KEY_EOL_E_UP_SPEC_SAFE_VOL_L_LIM = "W2M1MTE4427";
        string KEY_EOL_E_UP_SPEC_CELL_FOMULA__1 = "W2M1MTE4428";
        string KEY_EOL_E_UP_SPEC_CELL_FOMULA__2 = "W2M1MTE4429";
        string KEY_EOL_E_UP_SPEC_MODULE_FOMULA1 = "W2M1MTE4430";
        string KEY_EOL_E_UP_SPEC_MODULE_FOMULA2 = "W2M1MTE4431";
        string KEY_EOL_E_UP_SPEC_CELL_FOMULA__3 = "W2M1MTE4432";
        string KEY_EOL_E_UP_SPEC_MODULE_FOMULA3 = "W2M1MTE4433";
        #endregion

        #region TEMP CHECK
        string KEY_EOL_E_UP_TEMP_NEW_CHECK2 = "W2M1MTE4437";
        #endregion

        #region CELL DEV
        string KEY_EOL_E_UP_CELL_VOLT = "W2M1MTE4438";
        string KEY_EOL_E_UP_CALC_CELL_VOLT = "W2M1MTE4439";
        #endregion

        #endregion

        #region E_UP_DETAIL_ITEM_HIPOT
        string KEY_HIP_E_UP_PLUS__IR_LEVEL = "W2M1MTE4201";//ir volt
        string KEY_HIP_E_UP_PLUS___IR_TIME = "W2M1MTE4202";//ir time
        string KEY_HIP_E_UP_PLUS_WI__LEVEL = "W2M1MTE4203";//wi volt
        string KEY_HIP_E_UP_PLUS_WI___TIME = "W2M1MTE4204";//wi time
        string KEY_HIP_E_UP_PLUS_WI_RAMP_T = "W2M1MTE4205";//wi rampup
        string KEY_HIP_E_UP_PLUS_WI_ARC_ON = "W2M1MTE4206";//arc
        string KEY_HIP_E_UP_PLUS_WI_ARC_LM = "W2M1MTE4207";//arc limit
        string KEY_HIP_E_UP_PLUS_IR__RANGE = "W2M1MTE4208";//ir RANGE
        #endregion

        #region E_UP_DETAIL_ITEM_VOLTAGE
        string KEY_VOL_E_UP_TEMP1_____TEMP = "W2M1MTE4001";
        string KEY_VOL_E_UP_TEMP2_____TEMP = "W2M1MTE4002";
        string KEY_VOL_E_UP_AMBIENT___TEMP = "W2M1MTE4003";
        string KEY_VOL_E_UP_LINE__OP_UPPER = "W2M1MTE4004";
        string KEY_VOL_E_UP_LINE__OP_LOWER = "W2M1MTE4005";
        string KEY_VOL_E_UP_LINE_OP_RESIST = "W2M1MTE4006";
        #endregion


        //201217 wjs add maserati detail item key
        #region MASERATI_DETAIL_ITEM_EOL

        #region Data
        string KEY_EOL_MAS_NOR_DATA__CMC_GUID = "WMPMTE4501";
        string KEY_EOL_MAS_NOR_AMBIENT___TEMP = "WMPMTE4502";
        string KEY_EOL_MAS_NOR_TEMP1______ADC = "WMPMTE4503";
        string KEY_EOL_MAS_NOR_TEMP2______ADC = "WMPMTE4504";
        string KEY_EOL_MAS_NOR_PCB___TEMP_ADC = "WMPMTE4505";
        string KEY_EOL_MAS_NOR_CELL_V_DEV_MAX = "WMPMTE4506";
        string KEY_EOL_MAS_NOR_CELL_V_DEV_MIN = "WMPMTE4507";
        string KEY_EOL_MAS_NOR_MODULE____VOLT = "WMPMTE4508";
        string KEY_EOL_MAS_NOR_BAL__OP_SWITCH = "WMPMTE4522"; // soo jung pil yo
        string KEY_EOL_MAS_NOR_OVER___V_FAULT = "WMPMTE4523";
        string KEY_EOL_MAS_NOR_UNDER__V_FAULT = "WMPMTE4524";
        string KEY_EOL_MAS_NOR_CMC____VERSION = "WMPMTE4540";
        string KEY_EOL_MAS_NOR_HI__CONT_S_BEF = "WMPMTE4541";
        string KEY_EOL_MAS_NOR_HI__CONT_S_AFT = "WMPMTE4542";
        string KEY_EOL_MAS_NOR_TEMP_NEW_CHECK = "WMPMTE4543";
        string KEY_EOL_MAS_NOR_CELL_VOL_VERFY = "WMPMTE4544";
        string KEY_EOL_MAS_NOR_BAL__ST_SWITCH = "WMPMTE4545";
        string KEY_EOL_MAS_NOR_CELL_TEMN_LK_1 = "WMPMTE4546";
        string KEY_EOL_MAS_NOR_CELL_TEMN_LK_2 = "WMPMTE4547";
        string KEY_EOL_MAS_NOR_CELL_STAK_COPR = "WMPMTE4548";
        string KEY_EOL_MAS_NOR_IC_TEMP_TSHORT = "WMPMTE4549";
        #endregion

        #region in DCIR
        string KEY_EOL_MAS_NOR_DCIR_BEF_AMB_T = "WMPMTE4509";
        string KEY_EOL_MAS_NOR_DCIR_TEMP1_ADC = "WMPMTE4510";
        string KEY_EOL_MAS_NOR_DCIR_TEMP2_ADC = "WMPMTE4511";
        string KEY_EOL_MAS_NOR_DCIR_CVBEF_DIS = "WMPMTE4512";
        string KEY_EOL_MAS_NOR_DCIR_CV_DISCHA = "WMPMTE4513";
        string KEY_EOL_MAS_NOR_DCIR_CVBEF_CHA = "WMPMTE4514";
        string KEY_EOL_MAS_NOR_DCIR_CV_CHARGE = "WMPMTE4515";
        string KEY_EOL_MAS_NOR_DCIR_CVAFT_CHA = "WMPMTE4516";
        string KEY_EOL_MAS_NOR_DCIR_MVBEF_DIS = "WMPMTE4517";
        string KEY_EOL_MAS_NOR_DCIR_MV_DISCHA = "WMPMTE4518";
        string KEY_EOL_MAS_NOR_DCIR_MVBEF_CHA = "WMPMTE4519";
        string KEY_EOL_MAS_NOR_DCIR_MV_CHARGE = "WMPMTE4520";
        string KEY_EOL_MAS_NOR_DCIR_MVAFT_CHA = "WMPMTE4521";
        string KEY_EOL_MAS_NOR_DCIR_500MS_DIS = "WMPMTE4550";
        #endregion

        #region Report Spec
        string KEY_EOL_MAS_NOR_SPEC_BEF_DIS_REST_T = "WMPMTE4525";
        string KEY_EOL_MAS_NOR_SPEC_DISCHARGE_CURR = "WMPMTE4526";
        string KEY_EOL_MAS_NOR_SPEC_DISCHARGE_TIME = "WMPMTE4527";
        string KEY_EOL_MAS_NOR_SPEC_DIS_CURR_LIMIT = "WMPMTE4528";
        string KEY_EOL_MAS_NOR_SPEC_AFT_DIS_REST_T = "WMPMTE4529";
        string KEY_EOL_MAS_NOR_SPEC_CHARGE____CURR = "WMPMTE4530";
        string KEY_EOL_MAS_NOR_SPEC_CHARGE____TIME = "WMPMTE4531";
        string KEY_EOL_MAS_NOR_SPEC_CHA_CURR_LIMIT = "WMPMTE4532";
        string KEY_EOL_MAS_NOR_SPEC_AFT_CHA_RES__T = "WMPMTE4533";
        string KEY_EOL_MAS_NOR_SPEC_SAFE_VOL_H_LIM = "WMPMTE4534";
        string KEY_EOL_MAS_NOR_SPEC_SAFE_VOL_L_LIM = "WMPMTE4535";
        string KEY_EOL_MAS_NOR_SPEC_CELL_FOMULA__1 = "WMPMTE4536";
        string KEY_EOL_MAS_NOR_SPEC_CELL_FOMULA__2 = "WMPMTE4537";
        string KEY_EOL_MAS_NOR_SPEC_MODULE_FOMULA1 = "WMPMTE4538";
        string KEY_EOL_MAS_NOR_SPEC_MODULE_FOMULA2 = "WMPMTE4539";
        #endregion

        #endregion

        #region MASERATI_DETAIL_ITEM_HIPOT
        string KEY_HIP_MAS_NOR_PLUS__IR_LEVEL = "WMPMTE4301";
        string KEY_HIP_MAS_NOR_PLUS___IR_TIME = "WMPMTE4302";
        string KEY_HIP_MAS_NOR_PLUS_WI__LEVEL = "WMPMTE4303";
        string KEY_HIP_MAS_NOR_PLUS_WI___TIME = "WMPMTE4304";
        string KEY_HIP_MAS_NOR_PLUS_WI_RAMP_T = "WMPMTE4305";
        string KEY_HIP_MAS_NOR_PLUS_WI_ARC_ON = "WMPMTE4306";
        string KEY_HIP_MAS_NOR_PLUS_WI_ARC_LM = "WMPMTE4307";
        string KEY_HIP_MAS_NOR_PLUS_IR__RANGE = "WMPMTE4308"; //ir RANGE
        //string KEY_HIP_POR_NOR_PLUS_WI__RANGE = "WMPMTE4209"; //dc RANGE

        #endregion

        #region MASERATI_DETAIL_ITEM_HIPOT_NO_RESIN
        string KEY_HNR_MAS_NOR_PLUS__IR_LEVEL = "WMPMTE4701";
        string KEY_HNR_MAS_NOR_PLUS___IR_TIME = "WMPMTE4702";
        string KEY_HNR_MAS_NOR_PLUS_IR__RANGE = "WMPMTE4703"; 
        string KEY_HIP_MAS_NOR_FETCH_IR_OUTVOLT = "WMPMTE4704";
        string KEY_HIP_MAS_NOR_FETCH_IR_TIME = "WMPMTE4705";
        string KEY_HIP_MAS_NOR_FETCH_IR_MEASURE = "WMPMTE4706";
        #endregion

        #region MASERATI_DETAIL_ITEM_VOLTAGE
        string KEY_VOL_MAS_NOR_DATA__CMC_GUID = "WMPMTE4101";
        string KEY_VOL_MAS_NOR_AMBIENT___TEMP = "WMPMTE4102";
        string KEY_VOL_MAS_NOR_TEMP1______ADC = "WMPMTE4103";
        string KEY_VOL_MAS_NOR_TEMP2______ADC = "WMPMTE4104";
        string KEY_VOL_MAS_NOR_PCB___TEMP_ADC = "WMPMTE4105";
        string KEY_VOL_MAS_NOR_CELL_V_DEV_MAX = "WMPMTE4106";
        string KEY_VOL_MAS_NOR_CELL_V_DEV_MIN = "WMPMTE4107";
        string KEY_VOL_MAS_NOR_MODULE____VOLT = "WMPMTE4108";
        string KEY_VOL_MAS_NOR_BAL__OP_SWITCH = "WMPMTE4109";
        string KEY_VOL_MAS_NOR_OVER___V_FAULT = "WMPMTE4110";
        string KEY_VOL_MAS_NOR_UNDER__V_FAULT = "WMPMTE4111";
        string KEY_VOL_MAS_NOR_CMC____VERSION = "WMPMTE4112";
        string KEY_VOL_MAS_NOR_HI__CONT_S_BEF = "WMPMTE4113";
        string KEY_VOL_MAS_NOR_HI__CONT_S_AFT = "WMPMTE4114";
        string KEY_VOL_MAS_NOR_DEV_VOLT__REAL = "WMPMTE4115";
        string KEY_VOL_MAS_NOR_DEV_VOLT___BCR = "WMPMTE4116";
        #endregion


        #region PORSCHE_FACELIFT_DETAIL_ITEM_EOL

        #region Data
        string KEY_EOL_POR_F_L_CMC____VERSION = "WPFMMTE4416";
        string KEY_EOL_POR_F_L_DATA__CMC_GUID = "WPFMMTE4417";
        string KEY_EOL_POR_F_L_AMBIENT___TEMP = "WPFMMTE4418";
        string KEY_EOL_POR_F_L_TEMP1______ADC = "WPFMMTE4419";
        string KEY_EOL_POR_F_L_TEMP2______ADC = "WPFMMTE4420";
        string KEY_EOL_POR_F_L_TEMP3______ADC = "WPFMMTE4421";
        string KEY_EOL_POR_F_L_PCB___TEMP_ADC = "WPFMMTE4422";
        string KEY_EOL_POR_F_L_HI__CONT_S_BEF = "WPFMMTE4423";
        string KEY_EOL_POR_F_L_HI__CONT_S_AFT = "WPFMMTE4424";
        string KEY_EOL_POR_F_L_CELL_V_DEV_MAX = "WPFMMTE4425";
        string KEY_EOL_POR_F_L_CELL_V_DEV_MIN = "WPFMMTE4426";
        string KEY_EOL_POR_F_L_MODULE____VOLT = "WPFMMTE4427";
        string KEY_EOL_POR_F_L_BAL__OP_SWITCH = "WPFMMTE4442";
        string KEY_EOL_POR_F_L_OVER___V_FAULT = "WPFMMTE4443";
        string KEY_EOL_POR_F_L_UNDER__V_FAULT = "WPFMMTE4444";
        string KEY_EOL_POR_F_L_TEMP_NEW_CHECK = "WPFMMTE4445";
        string KEY_EOL_POR_F_L_CELL_VOL_VERFY = "WPFMMTE4446";
        string KEY_EOL_POR_F_L_BAL__ST_SWITCH = "WPFMMTE4447";
        string KEY_EOL_POR_F_L_CELL_TEMN_LK_1 = "WPFMMTE4448";
        string KEY_EOL_POR_F_L_CELL_TEMN_LK_2 = "WPFMMTE4449";
        string KEY_EOL_POR_F_L_CELL_STAK_COPR = "WPFMMTE4450";
        string KEY_EOL_POR_F_L_IC_TEMP_TSHORT = "WPFMMTE4451";
        #endregion

        #region in DCIR
        string KEY_EOL_POR_F_L_DCIR_BEF_AMB_T = "WPFMMTE4428";
        string KEY_EOL_POR_F_L_DCIR_TEMP1_ADC = "WPFMMTE4429";
        string KEY_EOL_POR_F_L_DCIR_TEMP2_ADC = "WPFMMTE4430";
        string KEY_EOL_POR_F_L_DCIR_TEMP3_ADC = "WPFMMTE4431";
        string KEY_EOL_POR_F_L_DCIR_CVBEF_DIS = "WPFMMTE4432";
        string KEY_EOL_POR_F_L_DCIR_CV_DISCHA = "WPFMMTE4433";
        string KEY_EOL_POR_F_L_DCIR_CVBEF_CHA = "WPFMMTE4434";
        string KEY_EOL_POR_F_L_DCIR_CV_CHARGE = "WPFMMTE4435";
        string KEY_EOL_POR_F_L_DCIR_CVAFT_CHA = "WPFMMTE4436";
        string KEY_EOL_POR_F_L_DCIR_MVBEF_DIS = "WPFMMTE4437";
        string KEY_EOL_POR_F_L_DCIR_MV_DISCHA = "WPFMMTE4438";
        string KEY_EOL_POR_F_L_DCIR_MVBEF_CHA = "WPFMMTE4439";
        string KEY_EOL_POR_F_L_DCIR_MV_CHARGE = "WPFMMTE4440";
        string KEY_EOL_POR_F_L_DCIR_MVAFT_CHA = "WPFMMTE4441";
        string KEY_EOL_POR_F_L_DCIR_500MS_DIS = "WPFMMTE4452";
        #endregion

        #region Report Spec
        string KEY_EOL_POR_F_L_SPEC_BEF_DIS_REST_T = "WPFMMTE4401";
        string KEY_EOL_POR_F_L_SPEC_DISCHARGE_CURR = "WPFMMTE4402";
        string KEY_EOL_POR_F_L_SPEC_DISCHARGE_TIME = "WPFMMTE4403";
        string KEY_EOL_POR_F_L_SPEC_DIS_CURR_LIMIT = "WPFMMTE4404";
        string KEY_EOL_POR_F_L_SPEC_AFT_DIS_REST_T = "WPFMMTE4405";
        string KEY_EOL_POR_F_L_SPEC_CHARGE____CURR = "WPFMMTE4406";
        string KEY_EOL_POR_F_L_SPEC_CHARGE____TIME = "WPFMMTE4407";
        string KEY_EOL_POR_F_L_SPEC_CHA_CURR_LIMIT = "WPFMMTE4408";
        string KEY_EOL_POR_F_L_SPEC_AFT_CHA_RES__T = "WPFMMTE4409";
        string KEY_EOL_POR_F_L_SPEC_SAFE_VOL_H_LIM = "WPFMMTE4410";
        string KEY_EOL_POR_F_L_SPEC_SAFE_VOL_L_LIM = "WPFMMTE4411";
        string KEY_EOL_POR_F_L_SPEC_CELL_FOMULA__1 = "WPFMMTE4412";
        string KEY_EOL_POR_F_L_SPEC_CELL_FOMULA__2 = "WPFMMTE4413";
        string KEY_EOL_POR_F_L_SPEC_MODULE_FOMULA1 = "WPFMMTE4414";
        string KEY_EOL_POR_F_L_SPEC_MODULE_FOMULA2 = "WPFMMTE4415";
        #endregion

        #endregion

        #region PORSCHE_FACELIFT_DETAIL_ITEM_HIPOT
        string KEY_HIP_POR_F_L_PLUS__IR_LEVEL = "WPFMMTE4201";
        string KEY_HIP_POR_F_L_PLUS___IR_TIME = "WPFMMTE4202";
        string KEY_HIP_POR_F_L_PLUS_IR__RANGE = "WPFMMTE4203"; //ir RANGE
        string KEY_HIP_POR_F_L_PLUS_WI__LEVEL = "WPFMMTE4204";
        string KEY_HIP_POR_F_L_PLUS_WI___TIME = "WPFMMTE4205";
        string KEY_HIP_POR_F_L_PLUS_WI_RAMP_T = "WPFMMTE4206";
        string KEY_HIP_POR_F_L_PLUS_WI_ARC_ON = "WPFMMTE4207";
        string KEY_HIP_POR_F_L_PLUS_WI_ARC_LM = "WPFMMTE4208";
        //string KEY_HIP_POR_NOR_PLUS_WI__RANGE = "WMPMTE4209"; //dc RANGE
        #endregion

        #region PORSCHE_FACELIFT_DETAIL_ITEM_HIPOT_NO_RESIN
        string KEY_HNR_POR_F_L_PLUS__IR_LEVEL = "WPFMMTE4501";
        string KEY_HNR_POR_F_L_PLUS___IR_TIME = "WPFMMTE4502";
        string KEY_HNR_POR_F_L_PLUS_IR__RANGE = "WPFMMTE4503";
        string KEY_HIP_POR_F_L_FETCH_IR_OUTVOLT = "WPFMMTE4504";
        string KEY_HIP_POR_F_L_FETCH_IR_TIME = "WPFMMTE4505";
        string KEY_HIP_POR_F_L_FETCH_IR_MEASURE = "WPFMMTE4506";
        #endregion

        #region PORSCHE_FACELIFT_DETAIL_ITEM_VOLTAGE
        //210312 wjs add FACELIFT

        string KEY_VOL_POR_F_L_CMC____VERSION = "WPFMMTE4001";
        string KEY_VOL_POR_F_L_DATA__CMC_GUID = "WPFMMTE4002";
        string KEY_VOL_POR_F_L_AMBIENT___TEMP = "WPFMMTE4003";
        string KEY_VOL_POR_F_L_TEMP1______ADC = "WPFMMTE4004";
        string KEY_VOL_POR_F_L_TEMP2______ADC = "WPFMMTE4005";
        string KEY_VOL_POR_F_L_TEMP3______ADC = "WPFMMTE4006";
        string KEY_VOL_POR_F_L_PCB___TEMP_ADC = "WPFMMTE4007";
        string KEY_VOL_POR_F_L_HI__CONT_S_BEF = "WPFMMTE4008";
        string KEY_VOL_POR_F_L_HI__CONT_S_AFT = "WPFMMTE4009";
        string KEY_VOL_POR_F_L_CELL_V_DEV_MAX = "WPFMMTE4010";
        string KEY_VOL_POR_F_L_CELL_V_DEV_MIN = "WPFMMTE4011";
        string KEY_VOL_POR_F_L_MODULE____VOLT = "WPFMMTE4012";
        string KEY_VOL_POR_F_L_DEV_VOLT__REAL = "WPFMMTE4013";
        string KEY_VOL_POR_F_L_DEV_VOLT___BCR = "WPFMMTE4014";
        string KEY_VOL_POR_F_L_BAL__OP_SWITCH = "WPFMMTE4015";
        string KEY_VOL_POR_F_L_OVER___V_FAULT = "WPFMMTE4016";
        string KEY_VOL_POR_F_L_UNDER__V_FAULT = "WPFMMTE4017";
        #endregion

        //221101 wjs 마세라티 M183 상세수집 추가
        #region MASERATI_M183_DETAIL_ITEM_EOL

        #region Data
        string KEY_EOL_MAS_M183_CMC____VERSION = "WM183MMTE4416";
        string KEY_EOL_MAS_M183_DATA__CMC_GUID = "WM183MMTE4417";
        string KEY_EOL_MAS_M183_AMBIENT___TEMP = "WM183MMTE4418";
        string KEY_EOL_MAS_M183_TEMP1______ADC = "WM183MMTE4419";
        string KEY_EOL_MAS_M183_TEMP2______ADC = "WM183MMTE4420";
        string KEY_EOL_MAS_M183_TEMP3______ADC = "WM183MMTE4421"; //221101 미 사용
        string KEY_EOL_MAS_M183_PCB___TEMP_ADC = "WM183MMTE4422";
        string KEY_EOL_MAS_M183_HI__CONT_S_BEF = "WM183MMTE4423";
        string KEY_EOL_MAS_M183_HI__CONT_S_AFT = "WM183MMTE4424";
        string KEY_EOL_MAS_M183_CELL_V_DEV_MAX = "WM183MMTE4425";
        string KEY_EOL_MAS_M183_CELL_V_DEV_MIN = "WM183MMTE4426";
        string KEY_EOL_MAS_M183_MODULE____VOLT = "WM183MMTE4427";
        string KEY_EOL_MAS_M183_BAL__OP_SWITCH = "WM183MMTE4442";
        string KEY_EOL_MAS_M183_OVER___V_FAULT = "WM183MMTE4443";
        string KEY_EOL_MAS_M183_UNDER__V_FAULT = "WM183MMTE4444";
        string KEY_EOL_MAS_M183_TEMP_NEW_CHECK = "WM183MMTE4445";
        string KEY_EOL_MAS_M183_CELL_VOL_VERFY = "WM183MMTE4446";
        string KEY_EOL_MAS_M183_BAL__ST_SWITCH = "WM183MMTE4447";
        string KEY_EOL_MAS_M183_CELL_TEMN_LK_1 = "WM183MMTE4448";
        string KEY_EOL_MAS_M183_CELL_TEMN_LK_2 = "WM183MMTE4449";
        string KEY_EOL_MAS_M183_CELL_STAK_COPR = "WM183MMTE4450";
        string KEY_EOL_MAS_M183_IC_TEMP_TSHORT = "WM183MMTE4451";
        #endregion

        #region in DCIR
        string KEY_EOL_MAS_M183_DCIR_BEF_AMB_T = "WM183MMTE4428";
        string KEY_EOL_MAS_M183_DCIR_TEMP1_ADC = "WM183MMTE4429";
        string KEY_EOL_MAS_M183_DCIR_TEMP2_ADC = "WM183MMTE4430";
        string KEY_EOL_MAS_M183_DCIR_TEMP3_ADC = "WM183MMTE4431"; //221101 wjs 미 사용
        string KEY_EOL_MAS_M183_DCIR_CVBEF_DIS = "WM183MMTE4432";
        string KEY_EOL_MAS_M183_DCIR_CV_DISCHA = "WM183MMTE4433";
        string KEY_EOL_MAS_M183_DCIR_CVBEF_CHA = "WM183MMTE4434";
        string KEY_EOL_MAS_M183_DCIR_CV_CHARGE = "WM183MMTE4435";
        string KEY_EOL_MAS_M183_DCIR_CVAFT_CHA = "WM183MMTE4436";
        string KEY_EOL_MAS_M183_DCIR_MVBEF_DIS = "WM183MMTE4437";
        string KEY_EOL_MAS_M183_DCIR_MV_DISCHA = "WM183MMTE4438";
        string KEY_EOL_MAS_M183_DCIR_MVBEF_CHA = "WM183MMTE4439";
        string KEY_EOL_MAS_M183_DCIR_MV_CHARGE = "WM183MMTE4440";
        string KEY_EOL_MAS_M183_DCIR_MVAFT_CHA = "WM183MMTE4441";
        string KEY_EOL_MAS_M183_DCIR_500MS_DIS = "WM183MMTE4452";
        #endregion

        #region Report Spec
        string KEY_EOL_MAS_M183_SPEC_BEF_DIS_REST_T = "WM183MMTE4401";
        string KEY_EOL_MAS_M183_SPEC_DISCHARGE_CURR = "WM183MMTE4402";
        string KEY_EOL_MAS_M183_SPEC_DISCHARGE_TIME = "WM183MMTE4403";
        string KEY_EOL_MAS_M183_SPEC_DIS_CURR_LIMIT = "WM183MMTE4404";
        string KEY_EOL_MAS_M183_SPEC_AFT_DIS_REST_T = "WM183MMTE4405";
        string KEY_EOL_MAS_M183_SPEC_CHARGE____CURR = "WM183MMTE4406";
        string KEY_EOL_MAS_M183_SPEC_CHARGE____TIME = "WM183MMTE4407";
        string KEY_EOL_MAS_M183_SPEC_CHA_CURR_LIMIT = "WM183MMTE4408";
        string KEY_EOL_MAS_M183_SPEC_AFT_CHA_RES__T = "WM183MMTE4409";
        string KEY_EOL_MAS_M183_SPEC_SAFE_VOL_H_LIM = "WM183MMTE4410";
        string KEY_EOL_MAS_M183_SPEC_SAFE_VOL_L_LIM = "WM183MMTE4411";
        string KEY_EOL_MAS_M183_SPEC_CELL_FOMULA__1 = "WM183MMTE4412";
        string KEY_EOL_MAS_M183_SPEC_CELL_FOMULA__2 = "WM183MMTE4413";
        string KEY_EOL_MAS_M183_SPEC_MODULE_FOMULA1 = "WM183MMTE4414";
        string KEY_EOL_MAS_M183_SPEC_MODULE_FOMULA2 = "WM183MMTE4415";
        #endregion

        #endregion

        #region MASERATI_M183_DETAIL_ITEM_HIPOT
        string KEY_HIP_MAS_M183_PLUS__IR_LEVEL = "WM183MMTE4201";
        string KEY_HIP_MAS_M183_PLUS___IR_TIME = "WM183MMTE4202";
        string KEY_HIP_MAS_M183_PLUS_IR__RANGE = "WM183MMTE4203"; //ir RANGE
        string KEY_HIP_MAS_M183_PLUS_WI__LEVEL = "WM183MMTE4204";
        string KEY_HIP_MAS_M183_PLUS_WI___TIME = "WM183MMTE4205";
        string KEY_HIP_MAS_M183_PLUS_WI_RAMP_T = "WM183MMTE4206";
        string KEY_HIP_MAS_M183_PLUS_WI_ARC_ON = "WM183MMTE4207";
        string KEY_HIP_MAS_M183_PLUS_WI_ARC_LM = "WM183MMTE4208";
        //string KEY_HIP_POR_NOR_PLUS_WI__RANGE = "WMPMTE4209"; //dc RANGE
        #endregion

        #region MASERATI_M183_DETAIL_ITEM_HIPOT_NO_RESIN
        string KEY_HNR_MAS_M183_PLUS____IR_LEVEL = "WM183MMTE4501";
        string KEY_HNR_MAS_M183_PLUS_____IR_TIME = "WM183MMTE4502";
        string KEY_HNR_MAS_M183_PLUS_IR____RANGE = "WM183MMTE4503";
        string KEY_HIP_MAS_M183_FETCH_IR_OUTVOLT = "WM183MMTE4504";
        string KEY_HIP_MAS_M183_FETCH_IR____TIME = "WM183MMTE4505";
        string KEY_HIP_MAS_M183_FETCH_IR_MEASURE = "WM183MMTE4506";
        #endregion

        #region MASERATI_M183_DETAIL_ITEM_VOLTAGE

        string KEY_VOL_MAS_M183_CMC____VERSION = "WM183MMTE4001";
        string KEY_VOL_MAS_M183_DATA__CMC_GUID = "WM183MMTE4002";
        string KEY_VOL_MAS_M183_AMBIENT___TEMP = "WM183MMTE4003";
        string KEY_VOL_MAS_M183_TEMP1______ADC = "WM183MMTE4004";
        string KEY_VOL_MAS_M183_TEMP2______ADC = "WM183MMTE4005";
        string KEY_VOL_MAS_M183_TEMP3______ADC = "WM183MMTE4006";
        string KEY_VOL_MAS_M183_PCB___TEMP_ADC = "WM183MMTE4007";
        string KEY_VOL_MAS_M183_HI__CONT_S_BEF = "WM183MMTE4008";
        string KEY_VOL_MAS_M183_HI__CONT_S_AFT = "WM183MMTE4009";
        string KEY_VOL_MAS_M183_CELL_V_DEV_MAX = "WM183MMTE4010";
        string KEY_VOL_MAS_M183_CELL_V_DEV_MIN = "WM183MMTE4011";
        string KEY_VOL_MAS_M183_MODULE____VOLT = "WM183MMTE4012";
        string KEY_VOL_MAS_M183_DEV_VOLT__REAL = "WM183MMTE4013";
        string KEY_VOL_MAS_M183_DEV_VOLT___BCR = "WM183MMTE4014";
        string KEY_VOL_MAS_M183_BAL__OP_SWITCH = "WM183MMTE4015";
        string KEY_VOL_MAS_M183_OVER___V_FAULT = "WM183MMTE4016";
        string KEY_VOL_MAS_M183_UNDER__V_FAULT = "WM183MMTE4017";
        #endregion

        #endregion

        bool mes_cv_order = true;
        private void SetCellVoltageOrder()
        {
            LogState(LogType.Info, "MES Cell voltage Order : " + (mes_cv_order == true ? "NORMAL" : "REVERSE"));

            if (!mes_cv_order)
            {
                Dictionary<string, double> tmepdic = new Dictionary<string, double>();
                var arr = MES.mes_CellList.Reverse();
                foreach (var item in arr)
                {
                    tmepdic.Add(item.Key, item.Value);
                }
                MES.mes_CellList = tmepdic;
            }
        }
         
        private bool GetControlItemFromMES(string LotID = "")
        {
            //MES에 제어항목을 요청한다
            string controlItem = MES.GetProcessControlParameterRequest(
                this.viewModel.EquipId,
                this.viewModel.ProdId,
                this.viewModel.ProcId, 
                LotID);

            if (controlItem == string.Empty || controlItem == "NG")
            {
                LogState(LogType.Fail, "GetControlItemFromMES");
                return false;
            }

            try
            {
                //받은 데이터를 파싱한다
                string[] parsed = controlItem.Split('@');
                var str = parsed[0].Split(new string[] { "CTRLITEM" }, StringSplitOptions.RemoveEmptyEntries);
                parsed[0] = "CTRLITEM" + str[1];
                parsed[parsed.Length - 1] = parsed[parsed.Length - 1].Remove(parsed[parsed.Length - 1].Length - 4, 4);

                //3. 검사기 공통) 제어/스펙 요청 무결성 점검
                //-> 문자열은 수신가능한 상태라고 박선임님께 이야기 완료
                //-> 항목 ID외에 항목수로도 구분할 수 있도록
                //제어항목 부분
                if (this.viewModel.ControlItemList.Count != parsed.Length)
                {
                    LogState(LogType.Fail, "GetControlItemFromMES_IntergrityCheck");
                    return false;
                }

                foreach (string item in parsed)
                {
                    string[] splitter = new string[] { "^", "=" };
                    var arr = item.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                    string ctrlItem = arr[1];
                     
                    var inValue = arr[3];
                     
                    if (!this.viewModel.ControlItemList.ContainsKey(ctrlItem))
                    {
                        LogState(LogType.Fail, "GetControlItemFromMES_IntergrityCheck");
                        return false;
                    }
                    else
                    {
                        this.viewModel.ControlItemList[ctrlItem] = inValue;
                    } 
                } 
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, "GetControlItemFromMES", ex);
                return false;
            }

            //MES로부터 받아온 제어항목을 CSV에 저장한다.
            SetControlItemToCSV();
            SetFieldsToMESData();
            SetCyclerFieldsToMESData();
            return true;
        }

        private bool GetCollectItemFromMES(string LotID = "")
        {
            //MES에 수집항목을 요청한다
            string collectItem = MES.GetProcessingSpecRequest(
                this.viewModel.EquipId,
                this.viewModel.ProdId,
                this.viewModel.ProcId,
                LotID);

            if (collectItem == string.Empty || collectItem == "NG")
            {
                LogState(LogType.Fail, "GetCollectItemFromMES");
                return false;
            }

            collectItem = collectItem.Remove(collectItem.Length - 1, 1);

            try
            {
                //받은 데이터를 파싱한다
                string[] parsed = collectItem.Split('@');
                var str = parsed[0].Split(new string[] { "CLCITEM" }, StringSplitOptions.RemoveEmptyEntries);
                parsed[0] = "CLCITEM" + str[1];
                parsed[parsed.Length - 1] = parsed[parsed.Length - 1].Remove(parsed[parsed.Length - 1].Length - 3, 3);

                //3. 검사기 공통) 제어/스펙 요청 무결성 점검
                //-> 문자열은 수신가능한 상태라고 박선임님께 이야기 완료
                //-> 항목 ID외에 항목수로도 구분할 수 있도록
                //수집항목 부분
                if (this.viewModel.CollectItemList.Count != parsed.Length)
                {
                    LogState(LogType.Fail, "GetCollectItemFromMES_IntergrityCheck");
                    return false;
                }

                foreach (string item in parsed)
                {
                    string[] splitter = new string[] { "^", "=" };
                    var arr = item.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                    if (arr.Length < 6) continue;

                    string clctItem = arr[1];
                    
                    if (!this.viewModel.CollectItemList.ContainsKey(clctItem))
                    {
                        LogState(LogType.Fail, "GetCollectItemFromMES_IntergrityCheck");
                        return false;
                    }
                    else
                    {
                        this.viewModel.CollectItemList[clctItem] = arr[5];
                    }

                    foreach (var testItem in this.viewModel.TestItemList)
                    {
                        if (testItem.Value.CLCTITEM == clctItem)
                        { 
                            testItem.Value.Min = arr[3]; 
                            testItem.Value.Max = arr[5]; 
                            break;
                        }
                    }
                     
                }
                 


            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, "GetCollectItemFromMES", ex);
                return false;
            }

            //MES로부터 받아온 제어항목을 CSV에 저장한다.
            SetCollectItemToCSV();
            return true;
        }

        private void GetControlItemFromCSV()
        {
            StringBuilder logsb = new StringBuilder();
            logsb.Append("ACK=OK,ERRMSG=,CTRLITEM=");

            try
            {
                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");

                LogState(LogType.MANUALCONDITION, "Use To Local Data(Control)");

                //ControlList.csv에서 항목별 ID를 가져온다
                //장비별로 수정필요

                var fileFolder = "EOL";

                switch (pro_Type)
                {
                    case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                    case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                    case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                    case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                }

                FileStream readData = new FileStream(staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader streamReader = new StreamReader(readData, encode);

                //헤더 따로 저장
                string[] columnHeaders = streamReader.ReadLine().Split(',');

                this.viewModel.ControlItemList.Clear();

                //파일에서 ID와 값을 가져온다
                List<string> ctrlItems = new List<string>();
                while (streamReader.Peek() > -1)
                {
                    string[] arr = streamReader.ReadLine().Split(',');
                    string ctrlItem = arr[0];
                    var inValue = arr[1];
                    string temp = "";

                    this.viewModel.ControlItemList.Add(ctrlItem, inValue);

                    logsb.Append(ctrlItem);
                    logsb.Append("^CTRLVAL=");
                    logsb.Append(arr[1]);
                    logsb.Append("@CTRLITEM=");
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, "GetControlItemFromCSV", ex);
                return;
            }
            logsb.Remove(logsb.Length - 10, 10);

            this.LogState(LogType.MANUALCONDITION, "GetProcessControlParameterRequest - " + logsb.ToString());
        }

        private void GetCollectItemFromCSV()
        {
            try
            {
                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");

                LogState(LogType.MANUALCONDITION, "Use To Local Data(Collect)");

                var fileFolder = "EOL";

                switch (pro_Type)
                {
                    case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                    case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                    case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                    case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                }

                //eollist.csv에서 항목별 ID를 가져온다
                FileStream readData = new FileStream(staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "eollist.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader streamReader = new StreamReader(readData, encode);

                //헤더 따로 저장
                string[] columnHeaders = streamReader.ReadLine().Split(',');

                this.viewModel.CollectItemList.Clear();

                //파일에서 ID와 값을 가져온다
                List<TestItem> tis = new List<TestItem>();
                while (streamReader.Peek() > -1)
                {
                    string[] arr = streamReader.ReadLine().Split(',');
                    if (arr.Length < 6) continue;
                    if ((localTypes == ModelType.AUDI_NORMAL && arr[9] != "1") || (localTypes == ModelType.AUDI_MIRROR && arr[10] != "1")
                        || (localTypes == ModelType.PORSCHE_NORMAL && arr[11] != "1") || (localTypes == ModelType.PORSCHE_MIRROR && arr[12] != "1")
                        || (localTypes == ModelType.MASERATI_NORMAL && arr[13] != "1") || (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL && arr[14] != "1")//210312 wjs add pors fl
                        || (localTypes == ModelType.PORSCHE_FACELIFT_MIRROR && arr[16] != "1") || (localTypes == ModelType.E_UP && arr[15] != "1")
                        || (localTypes == ModelType.MASERATI_M183_NORMAL && arr[17] != "1") //221101 wjs 체크필요
                        )
                    {
                        continue;
                    }

                    string clctItem = arr[2];

                    foreach (var testItem in this.viewModel.TestItemList)
                    {
                        if (testItem.Value.CLCTITEM == clctItem)
                        {
                            if(clctItem == "")
                            {
                                continue;
                            }
                            testItem.Value.Min = arr[3];
                            testItem.Value.Max = arr[4];
                            this.viewModel.CollectItemList.Add(clctItem, arr[4].ToString());
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { LogState(LogType.Fail, "GetCollectItemFromCSV", ex); }

            StringBuilder sb = new StringBuilder();
            sb.Append("ACK=OK,ERRMSG=,");
            foreach (var item in this.viewModel.TestItemList)
            {
                if (item.Key.Replace(" ", "") == _BARCODE)
                    continue;

                //CLCITEM=CTQW2201,CLCTLSL=9.532,CLCTUSL=12.25@
                sb.Append("CLCITEM=");
                sb.Append(item.Value.CLCTITEM.ToString());
                sb.Append("^CLCTLSL=");
                sb.Append(item.Value.Min.ToString());
                sb.Append("^CLCTUSL=");
                sb.Append(item.Value.Max.ToString());
                sb.Append("@");
            }
            this.LogState(LogType.MANUALCONDITION, "GetProcessingSpecRequest - " + sb.ToString());
        }

        public void SetControlItemToCSV()
        {
            string columnHeaders = "";
            List<string> ctrlItems = new List<string>();
            StringBuilder sb = new StringBuilder();


            try
            {

                var fileFolder = "EOL";

                switch (pro_Type)
                {
                    case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                    case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                    case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                    case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                }
                
                using (FileStream readData = new FileStream(staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv",
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(readData, Encoding.UTF8))
                    {
                        columnHeaders = sr.ReadLine();

                        while (sr.Peek() > -1)
                        {
                            string[] arr = sr.ReadLine().Split(',');

                            string value = "";

                            string ctrlItem = arr[0];

                            value = this.viewModel.ControlItemList[ctrlItem];

                            if (value != "")
                            {
                                sb.Append(arr[0] + "," + value + "," + arr[2] + "," + arr[3] + "\n");
                            }
                            else
                            {
                                sb.Append(arr[0] + "," + "0" + "," + arr[2] + "," + arr[3] + "\n");
                            }
                        }

                        //저장된 제어항목을 파일에 저장
                        using (StreamWriter sw = new StreamWriter(staticPath_EOL_SPEC + "\\" + position +
                            "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv", false))
                        {
                            sw.WriteLine(columnHeaders);
                            sw.Write(sb.ToString());
                        }
                    }
                }

                Thread.Sleep(1000);
            }
            catch (Exception ex) { LogState(LogType.Fail, "SetControlItemToCSV", ex); }
        }

        public void SetCollectItemToCSV()
        {
            string columnHeaders = "";
            List<string[]> collections = new List<string[]>();
            try
            {

                var fileFolder = "EOL";

                switch (pro_Type)
                {
                    case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                    case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                    case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                    case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                }

                //eollist.csv를 불러온다
                using (FileStream readData = new FileStream(staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "eollist.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(readData, Encoding.UTF8))
                    {
                        //헤더 따로 저장
                        columnHeaders = sr.ReadLine();

                        //제어항목을 불러온다
                        while (sr.Peek() > -1)
                            collections.Add(sr.ReadLine().Split(','));

                    }
                }

                //저장된 제어항목을 파일에 저장
                using (StreamWriter sw = new StreamWriter(
                        staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "eollist.csv",
                        false))
                {
                    StringBuilder sb = new StringBuilder();

                    //헤더를 파일에 쓴다
                    sw.WriteLine(columnHeaders);

                    //localTypes를 참조하여 현재 수집항목 조건을 파일에 쓴다
                    for (int i = 0; i < collections.Count; i++)
                    {
                        foreach (var testItem in this.viewModel.TestItemList)
                        {
                            if ((localTypes == ModelType.AUDI_NORMAL && collections[i][9] == "1" && collections[i][2] == testItem.Value.CLCTITEM)
                                || (localTypes == ModelType.AUDI_MIRROR && collections[i][10] == "1" && collections[i][2] == testItem.Value.CLCTITEM)
                                || (localTypes == ModelType.PORSCHE_NORMAL && collections[i][11] == "1" && collections[i][2] == testItem.Value.CLCTITEM)
                                || (localTypes == ModelType.PORSCHE_MIRROR && collections[i][12] == "1" && collections[i][2] == testItem.Value.CLCTITEM)
                                || (localTypes == ModelType.MASERATI_NORMAL && collections[i][13] == "1" && collections[i][2] == testItem.Value.CLCTITEM)
                                || (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL && collections[i][14] == "1" && collections[i][2] == testItem.Value.CLCTITEM)//210312 wjs add pors fl
                                || (localTypes == ModelType.PORSCHE_FACELIFT_MIRROR && collections[i][16] == "1" && collections[i][2] == testItem.Value.CLCTITEM)//210312 wjs add pors fl
                                || (localTypes == ModelType.E_UP && collections[i][15] == "1" && collections[i][2] == testItem.Value.CLCTITEM)
                                || (localTypes == ModelType.MASERATI_M183_NORMAL && collections[i][17] == "1" && collections[i][2] == testItem.Value.CLCTITEM)  //221101 wjs add mase m183
                                )
                            {
                                collections[i][3] = testItem.Value.Min.ToString();
                                collections[i][4] = testItem.Value.Max.ToString();
                                break;
                            }
                        }
                        foreach (string s in collections[i]) sb.Append(s + ',');
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("\n");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sw.Write(sb.ToString());

                    sb.Clear();
                }

                Thread.Sleep(1000);
            }
            catch (Exception ex) { LogState(LogType.Fail, "SetCollectItemToCSV", ex); }
        }

        Thread finishedThread;

        /// <summary>
        /// MES에 상세수집 / 수집항목 완공처리
        /// 중간에 NG나도 여기타고 다끝나고 PASS해도 여기탐
        /// </summary>
        /// <param name="isSuccess"></param>
        private bool Finished(bool isSuccess, bool isFinished, int saveCase = 0)
        {
            isEmg_ = false;
            //스킵NG일땐 무조건 NG처리
            //190109 NG필터 추가
            if (isSkipNG_)
            {
                LogState(LogType.NG, "Result Filter :SkipNG");
                isSuccess = false;
            }

            //200106 add case with pjh
            if (!isMESskip && MES.isMES_SYS_Disconnected)
            {
                LogState(LogType.NG, "Result Filter :Disconnected MES");
                isSuccess = false;
            }


            //if failed, go to sleep
            if (!isSuccess)
            {
                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                    || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                    || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                    )
                {
                    if (pro_Type == ProgramType.VoltageInspector || pro_Type == ProgramType.EOLInspector)
                    {
                        var ti = this.viewModel.TestItemList["Normal Sleep"];
                        var rst = MethodInvoker(ti.SingleMethodName, new object[] { ti });

                        if (!rst)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                SetCtrlToPass(false, ti.Bt.Name);
                                SetCtrlToPass(false, ti.GroupName);
                            }));
                        }
                        else
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                SetCtrlToPass(true, ti.Bt.Name);
                                SetCtrlToPass(true, ti.GroupName);
                            }));
                        }

                    }
                }
            }

            //검사항목을 한개라도 검사했는지 확인하는 변수
            var isEmptyItems = true;

            foreach (var item in this.viewModel.TestItemList)
            {
                if (item.Value.Name.Replace(" ", "") == _BARCODE)
                {
                    continue;
                }

                if (item.Value.Value_ != null)
                {
                    isEmptyItems = false;
                    break;
                }
            }

            #region MES 스킵상태가 아니고, 자동모드로 진입했고(autoModeThread로 판단), 검사항목들(바코드제외)이 모두 비어있을 때

            //200102 changed 
            //if (autoModeThread != null && autoModeThread.IsAlive && !isMESskip && isEmptyItems)
            if (autoModeThread != null && autoModeThread.IsAlive && isEmptyItems)
            {
                //Result 폴더에서 확인을 위해 강제로 바코드를 기입
                this.viewModel.TestItemList.ToList()[0].Value.Value_ = this.viewModel.LotId;


                if (isStop)
                {
                    // 2019.07.03 jeonhj
                    // 착공NG와 EMS 상황을 분리
                    this.viewModel.TestItemList.ToList()[1].Value.Value_ = _EMG_STOPPED;
                }
                else
                {
                    switch (saveCase)
                    {
                        case 0:/*normal case, 여길 탈수 없음*/; break;
                        case -1: this.viewModel.TestItemList.ToList()[1].Value.Value_ = "MES_Disconnected NG"; break;
                        case 1: this.viewModel.TestItemList.ToList()[1].Value.Value_ = "MES_GetControlItem NG"; break;
                        case 2: this.viewModel.TestItemList.ToList()[1].Value.Value_ = "MES_GetCollectItem NG"; break;
                        case 3: this.viewModel.TestItemList.ToList()[1].Value.Value_ = "MES_StartJob NG"; break;
                        default: break;
                    }
                }
                //두번째 검사항목을 NG처리
                this.viewModel.TestItemList.ToList()[1].Value.Result = "NG";
            }

            #endregion
            
            StringBuilder sb = new StringBuilder();
            StringBuilder sbc = new StringBuilder();

            List<DetailItems> dlist = new List<DetailItems>();

            //완공을 위한 아이템별 데이터 모으기
            foreach (var item in this.viewModel.TestItemList)
            {
                //26. finished 함수내 수집/상세수집 데이터 모을때 바코드 항목의 상세수집은 모으고 수집은
                //Continue로 미수집 처리(상세수집 중 제어항목 관련 item은 바코드 리딩시 처리되도록 박진호 선임 요청함)
                //not finish item filter
                //190104 by grchoi
                //if (item.Key == _BARCODE) continue;
                if (item.Key.Replace(" ", "") == _BARCODE)
                {
                    if (item.Value.refValues_.Count != 0)
                    {
                        foreach (var ritem in item.Value.refValues_) dlist.Add(ritem as DetailItems);
                    }
                    continue;
                }

                sb.Append(string.Format("{0}={1}={2}^", item.Value.CLCTITEM, item.Value.Value_, (item.Value.Result == "PASS" ? "Y" : "N")));

                //상세수집 데이터 모으기
                if (item.Value.refValues_.Count != 0)
                {
                    foreach (var ritem in item.Value.refValues_)
                    {
                        var dti = ritem as DetailItems;

                        dlist.Add(dti);

                    }
                }
            }

            // 20190813 Noah Choi 인터페이스 명세서 도착 후 수정 결정예정
            var orderedList = dlist.OrderBy(x => x.Key.ToString());


            //공정 결과 상세 보고 정렬
            foreach (var item in orderedList)
            {
                var dti = item as DetailItems;

                var clct = dti.Key;

                sbc.Append(string.Format("{0}=", clct));
                for (int i = 0; i < dti.Reportitems.Count; i++)
                {
                    sbc.Append(string.Format("{0}&", dti.Reportitems[i].ToString()));
                }

                sbc.Remove(sbc.Length - 1, 1);
                sbc.Append("^");

            }

            //순서정렬

            if (sbc.Length > 0)
            {
                sbc.Remove(sbc.Length - 1, 1);
                LogState(LogType.Info, "GetProcessDataReport - " + sbc.ToString());
            }

            sb.Remove(sb.Length - 1, 1);
            LogState(LogType.Info, "EndJobInsp - " + sb.ToString());

            if (isSuccess)
            {
                LogState(LogType.Info, "EOL - total Result - PASS");
            }
            else
            {
                LogState(LogType.Info, "EOL - total Result - NG");
            }

            //27. finished로 검사 종료 시 검사모델/설비ID/제품ID/공정ID를 로그로 남김
            LogState(LogType.Info, "Started Model : " + this.viewModel.ModelId);
            LogState(LogType.Info, "Lot ID : " + this.viewModel.LotId);
            LogState(LogType.Info, "Process ID : " + this.viewModel.ProcId);
            LogState(LogType.Info, "Product ID : " + this.viewModel.ProdId);
            LogState(LogType.Info, "Equipment ID : " + this.viewModel.EquipId);

            if (!isMESskip)
            {
                if (isFinished)
                {
                    string retDItem = MES.GetProcessDataReport(
                        this.viewModel.LotId,
                        this.viewModel.ProcId,
                        this.viewModel.EquipId,
                        this.viewModel.UserId,
                        sbc.ToString());
                    if ((retDItem != string.Empty) || (retDItem == "NG")) //OK일때 시작
                    {
                        this.LogState(LogType.Fail, "MES_GetProcessDataReport");
                        isSuccess = false;
                    }
                    else
                    {
                        this.LogState(LogType.Success, "MES_GetProcessDataReport - " + sbc.ToString());
                    }
                    PauseLoop("MES_GetProcessDataReport");

                    //공정 결과 Data 보고                
                    if (MES.EndJobInsp(
                        this.viewModel.LotId,
                        this.viewModel.ProcId,
                        this.viewModel.EquipId,
                        this.viewModel.UserId,
                        (isSuccess == true ? "OK" : "NG"), sb.ToString()) == "NG") //OK일때 시작
                    {
                        this.LogState(LogType.Fail, "MES_EndJobInsp");

                        isSuccess = false;
                    }
                    else
                    {
                        this.LogState(LogType.Success, "MES_EndJobInsp - " + sb.ToString());
                    }
                    PauseLoop("MES_EndJobInsp");
                }
                else
                {
                    isSuccess = false;
                }
            }

            //if (plc != null && plc.isTesting_Flag)
            //    plc.TestResult(isSuccess);


            //200702 진단모드시 무조건 설비에는 NG 주게 수정 pjh
            if (CheckBCRforMaster_Diag_END())
            {
                if (plc != null)
                    plc.TestResult(false);
            }
            else
            {
                //200401 진행중 비트와 상관없이 한다.
                if (plc != null)
                    plc.TestResult(isSuccess);
            }


            //200106 change position with pjh
            #region END Timer
            timeThreadFlag = false;
            _stopwatch.Stop();
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.time_finish_tb.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }));

            this.LogState(LogType.Info, "Elapsed timer Stopped");
            #endregion


            //상세수집, 완공 결과에 따라 UI 최종결과가 결정된다.
            this.Dispatcher.Invoke(new Action(() =>
            {
                Pass(isSuccess);

                isMESSkipCb.IsEnabled = manualBt.IsEnabled = true;
                if (isManual)
                {
                    openButtonGrid.IsEnabled = true;
                    resetBt.IsEnabled = true;
                }
                labelA.IsEnabled = labelM.IsEnabled = startAutoBt.IsEnabled = true;

                blinder3.Visibility = blinder2.Visibility = System.Windows.Visibility.Collapsed;


            }));

            if (finishedThread == null)
            {
                finishedThread = new Thread(() =>
                {
                    //191229 CHANGEed
                    tlamp.SetTLampTesting(false);


                    //빨간불이 이미 계측기때문에 들어와있다면,
                    //191204 ht add case
                    var isComDied = tlamp.GetTLampNG();// relays.RelayStatus("IDO_0");
                    if (!isSuccess)
                    {
                        tlamp.SetTLampNG(true);
                        Thread.Sleep(3000);
                        if (!isComDied)
                        {
                            tlamp.SetTLampNG(false);
                        }
                    }

                    tlamp.SetTLampStandBy(true);

                    //if (!isMESskip)
                    //{
                    //    tlamp.SetTLampMESStatus(true);
                    //}

                    LogState(LogType.Info, "FINISHED_TEST----------------------------------------------------", null, false);


                    finishedThread = null;
                });
                finishedThread.Start();
            }
            return isSuccess;
        }
    }
}
