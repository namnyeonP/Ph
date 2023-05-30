using EOL_BASE.모듈;
using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        #region Multimedia Timer Fields

        [DllImport("winmm.dll")] private static extern int timeSetEvent(int delay, int resolution, 공용_이벤트핸들러 handler, IntPtr user, int eventType);
        [DllImport("winmm.dll")] private static extern int timeKillEvent(int id);
        [DllImport("winmm.dll")] private static extern int timeBeginPeriod(int msec);
        [DllImport("winmm.dll")] private static extern int timeEndPeriod(int msec);

        //private delegate void 공용_이벤트핸들러(Object obj);
        private delegate void 공용_이벤트핸들러(int id, int msg, IntPtr user, int dw1, int dw2);
        private 공용_이벤트핸들러 상시저항가져오는타이머_핸들러;
        private int 상시저항가져오는타이머_아이디;

        #endregion

        #region 충방전 스케줄을 위한 제어값

        /// <summary>
        /// 방전 전 휴지 시간
        /// </summary>
        public string befDiscRestTime = "";

        /// <summary>
        /// 방전 전류
        /// </summary>
        public string discCur = "";

        /// <summary>
        /// 방전 시간
        /// </summary>
        public string discTime = "";

        /// <summary>
        /// 방전 전류 안전상한
        /// </summary>
        public string discCurLimit = "";

        /// <summary>
        /// 방전이후 휴지시간
        /// </summary>
        public string aftDiscRestTime = "";

        /// <summary>
        /// 충전시의 전압상한
        /// </summary>
        public string safeVoltHighLimit = "";

        /// <summary>
        /// 방전시의 전압하한
        /// </summary>
        public string safeVoltLowLimit = "";

        /// <summary>
        /// 충전직후 휴지시간
        /// </summary>
        public string aftCharRestTime = "";

        /// <summary>
        /// 충전 전류
        /// </summary>
        public string charCur = "";

        /// <summary>
        /// 충전 시간
        /// </summary>
        public string charTime = "";

        /// <summary>
        /// 충전 전류 안전상한
        /// </summary>
        public string charCurLimit = "";

        /// <summary>
        /// 모듈 온도보정식 1
        /// </summary>
        public double moduleFomula1 = 0;

        /// <summary>
        /// 모듈 온도보정식 2
        /// </summary>
        public double moduleFomula2 = 0;

        /// <summary>
        /// 셀 온도보정식 1
        /// </summary>
        public double cellFomula1 = 0;

        /// <summary>
        /// 셀 온도보정식 2
        /// </summary>
        public double cellFomula2 = 0;

        /// <summary>
        /// 계수 T
        /// </summary>
        public double coefficientT = 0;

        /// <summary>
        /// 계수 V
        /// </summary>
        public double coefficientV = 0;
         

        #endregion

        /// <summary>
        /// 충방전시 셀 데이터 로깅을 하는데, 변수 모두 수동으로 비우는 부분
        /// </summary>
        public void ClearCellDetailList()
        {
            foreach (var item in cellDetailList)
            {
                item.ModuleVolt = item.Temp1 = item.Temp2 =
                    item.CellVolt_1 = item.CellVolt_2 = item.CellVolt_3 = item.CellVolt_4 = item.CellVolt_5 = item.CellVolt_6 = item.CellVolt_7 = item.CellVolt_8 = item.CellVolt_9 = item.CellVolt_10 =
                    item.CellVolt_11 = item.CellVolt_12 = item.CellVolt_13 = item.CellVolt_14 = item.CellVolt_15 = item.CellVolt_16 = item.CellVolt_17 = item.CellVolt_18 = item.CellVolt_19 = item.CellVolt_20 =
                    item.CellVolt_20 = item.CellVolt_21 = item.CellVolt_22 = item.CellVolt_23 = item.CellVolt_24 = item.CellVolt_25 = item.CellVolt_26 = item.CellVolt_27 = item.CellVolt_28 = item.CellVolt_29 = item.CellVolt_30 =
                    item.CellVolt_31 = item.CellVolt_32 = item.CellVolt_33 = item.CellVolt_34 = item.CellVolt_35 = item.CellVolt_36 = item.CellVolt_37 = item.CellVolt_38 = item.CellVolt_39 = item.CellVolt_40 =
                    item.CellVolt_41 = item.CellVolt_42 = item.CellVolt_43 = item.CellVolt_44 = item.CellVolt_45 = item.CellVolt_46 = item.CellVolt_47 = item.CellVolt_48 = item.CellVolt_49 = item.CellVolt_50 =
                    item.CellVolt_51 = item.CellVolt_52 = item.CellVolt_53 = item.CellVolt_54 = item.CellVolt_55 = item.CellVolt_56 = item.CellVolt_57 = item.CellVolt_58 = item.CellVolt_59 = item.CellVolt_60 =
                    item.CellVolt_61 = item.CellVolt_62 = item.CellVolt_63 = item.CellVolt_64 = item.CellVolt_65 = item.CellVolt_66 = item.CellVolt_67 = item.CellVolt_68 = item.CellVolt_69 = item.CellVolt_70 =
                    item.CellVolt_71 = item.CellVolt_72 = item.CellVolt_73 = item.CellVolt_74 = item.CellVolt_75 = item.CellVolt_76 = item.CellVolt_77 = item.CellVolt_78 = item.CellVolt_79 = item.CellVolt_80 =
                    item.CellVolt_81 = item.CellVolt_82 = item.CellVolt_83 = item.CellVolt_84 = item.CellVolt_85 = item.CellVolt_86 = item.CellVolt_87 = item.CellVolt_88 = item.CellVolt_89 = item.CellVolt_90 =
                    item.CellVolt_91 = item.CellVolt_92 = item.CellVolt_93 = item.CellVolt_94 = item.CellVolt_95 = item.CellVolt_96 =
                    0.0;
            }
        }

        /// <summary>
        /// 셀 데이터를 새로고침 해야 할 때 해당 리스트를 쓴다.
        /// 0: INIT VOLT
        /// 1: AFTER DISCHARGE
        /// 2: BEFORE CHARGE
        /// 3: AFTER CHARGE
        /// 4: FINISH VOLT
        /// </summary>
        /// <param name="index"></param>
        public void SetCellDetailList(int index)
        {
            //25. DAQ 데이터 모델별 구분처리 추가
            //8s
            if (localTypes == 0)
            {
                //190107 by grchoi
                //DAQ 포트 이상 및 코드 통일을 위한 소켓 변경
                //4P8S
                cellDetailList[index].CellVolt_1 = daq.DAQList[10];
                cellDetailList[index].CellVolt_2 = daq.DAQList[11];
                cellDetailList[index].CellVolt_3 = daq.DAQList[12];
                cellDetailList[index].CellVolt_4 = daq.DAQList[13];
                cellDetailList[index].CellVolt_5 = daq.DAQList[14];
                cellDetailList[index].CellVolt_6 = daq.DAQList[15];
                cellDetailList[index].CellVolt_7 = daq.DAQList[16];
                cellDetailList[index].CellVolt_8 = daq.DAQList[17];
            }
            else if (localTypes == 1)
            {
                //4P8S Reverse
                cellDetailList[index].CellVolt_1 = daq.DAQList[28];
                cellDetailList[index].CellVolt_2 = daq.DAQList[29];
                cellDetailList[index].CellVolt_3 = daq.DAQList[30];
                cellDetailList[index].CellVolt_4 = daq.DAQList[31];
                cellDetailList[index].CellVolt_5 = daq.DAQList[32];
                cellDetailList[index].CellVolt_6 = daq.DAQList[33];
                cellDetailList[index].CellVolt_7 = daq.DAQList[34];
                cellDetailList[index].CellVolt_8 = daq.DAQList[35];
            }
            else if (localTypes == 2)
            {
                //4P7S
                cellDetailList[index].CellVolt_1 = daq.DAQList[36];
                cellDetailList[index].CellVolt_2 = daq.DAQList[37];
                cellDetailList[index].CellVolt_3 = daq.DAQList[38];
                cellDetailList[index].CellVolt_4 = daq.DAQList[39];
                cellDetailList[index].CellVolt_5 = daq.DAQList[40];
                cellDetailList[index].CellVolt_6 = daq.DAQList[41];
                cellDetailList[index].CellVolt_7 = daq.DAQList[42];
            }
            else if (localTypes == 3)
            {
                //3P8S
                cellDetailList[index].CellVolt_1 = daq.DAQList[10];
                cellDetailList[index].CellVolt_2 = daq.DAQList[11];
                cellDetailList[index].CellVolt_3 = daq.DAQList[12];
                cellDetailList[index].CellVolt_4 = daq.DAQList[13];
                cellDetailList[index].CellVolt_5 = daq.DAQList[14];
                cellDetailList[index].CellVolt_6 = daq.DAQList[15];
                cellDetailList[index].CellVolt_7 = daq.DAQList[16];
                cellDetailList[index].CellVolt_8 = daq.DAQList[17];
            }
            else if (localTypes == 4)
            {
                //3P10S
                cellDetailList[index].CellVolt_1 = daq.DAQList[0];
                cellDetailList[index].CellVolt_2 = daq.DAQList[1];
                cellDetailList[index].CellVolt_3 = daq.DAQList[2];
                cellDetailList[index].CellVolt_4 = daq.DAQList[3];
                cellDetailList[index].CellVolt_5 = daq.DAQList[4];
                cellDetailList[index].CellVolt_6 = daq.DAQList[5];
                cellDetailList[index].CellVolt_7 = daq.DAQList[6];
                cellDetailList[index].CellVolt_8 = daq.DAQList[7];
                cellDetailList[index].CellVolt_9 = daq.DAQList[8];
                cellDetailList[index].CellVolt_10 = daq.DAQList[9];
            }
            else if (localTypes == 5)
            {
                //3P10S Rev
                cellDetailList[index].CellVolt_1 = daq.DAQList[18];
                cellDetailList[index].CellVolt_2 = daq.DAQList[19];
                cellDetailList[index].CellVolt_3 = daq.DAQList[20];
                cellDetailList[index].CellVolt_4 = daq.DAQList[21];
                cellDetailList[index].CellVolt_5 = daq.DAQList[22];
                cellDetailList[index].CellVolt_6 = daq.DAQList[23];
                cellDetailList[index].CellVolt_7 = daq.DAQList[24];
                cellDetailList[index].CellVolt_8 = daq.DAQList[25];
                cellDetailList[index].CellVolt_9 = daq.DAQList[26];
                cellDetailList[index].CellVolt_10 = daq.DAQList[27];
            }


            //BMS에서 Cell Voltage 받아서 처리            
            //var rList = new List<string>();
            //GetToDID_singleData(0x48, 0x06, "10 C3", true, out rList);

            //if (rList != null && rList.Count > 194)
            //{
            //    cellDetailList[index].CellVolt_1 = Convert.ToInt32(rList[4] + rList[5], 16) * 0.001;
            //    cellDetailList[index].CellVolt_2 = Convert.ToInt32(rList[6] + rList[7], 16) * 0.001;
            //    cellDetailList[index].CellVolt_3 = Convert.ToInt32(rList[8] + rList[9], 16) * 0.001;
            //    cellDetailList[index].CellVolt_4 = Convert.ToInt32(rList[10] + rList[11], 16) * 0.001;
            //    cellDetailList[index].CellVolt_5 = Convert.ToInt32(rList[12] + rList[13], 16) * 0.001;
            //    cellDetailList[index].CellVolt_6 = Convert.ToInt32(rList[14] + rList[15], 16) * 0.001;
            //    cellDetailList[index].CellVolt_7 = Convert.ToInt32(rList[16] + rList[17], 16) * 0.001;
            //    cellDetailList[index].CellVolt_8 = Convert.ToInt32(rList[18] + rList[19], 16) * 0.001;
            //    cellDetailList[index].CellVolt_9 = Convert.ToInt32(rList[20] + rList[21], 16) * 0.001;
            //    cellDetailList[index].CellVolt_10 = Convert.ToInt32(rList[22] + rList[23], 16) * 0.001;
            //    cellDetailList[index].CellVolt_11 = Convert.ToInt32(rList[24] + rList[25], 16) * 0.001;
            //    cellDetailList[index].CellVolt_12 = Convert.ToInt32(rList[26] + rList[27], 16) * 0.001;
            //    cellDetailList[index].CellVolt_13 = Convert.ToInt32(rList[28] + rList[29], 16) * 0.001;
            //    cellDetailList[index].CellVolt_14 = Convert.ToInt32(rList[30] + rList[31], 16) * 0.001;
            //    cellDetailList[index].CellVolt_15 = Convert.ToInt32(rList[32] + rList[33], 16) * 0.001;
            //    cellDetailList[index].CellVolt_16 = Convert.ToInt32(rList[34] + rList[35], 16) * 0.001;
            //    cellDetailList[index].CellVolt_17 = Convert.ToInt32(rList[36] + rList[37], 16) * 0.001;
            //    cellDetailList[index].CellVolt_18 = Convert.ToInt32(rList[38] + rList[39], 16) * 0.001;
            //    cellDetailList[index].CellVolt_19 = Convert.ToInt32(rList[40] + rList[41], 16) * 0.001;
            //    cellDetailList[index].CellVolt_20 = Convert.ToInt32(rList[42] + rList[43], 16) * 0.001;
            //    cellDetailList[index].CellVolt_21 = Convert.ToInt32(rList[44] + rList[45], 16) * 0.001;
            //    cellDetailList[index].CellVolt_22 = Convert.ToInt32(rList[46] + rList[47], 16) * 0.001;
            //    cellDetailList[index].CellVolt_23 = Convert.ToInt32(rList[48] + rList[49], 16) * 0.001;
            //    cellDetailList[index].CellVolt_24 = Convert.ToInt32(rList[50] + rList[51], 16) * 0.001;
            //    cellDetailList[index].CellVolt_25 = Convert.ToInt32(rList[52] + rList[53], 16) * 0.001;
            //    cellDetailList[index].CellVolt_26 = Convert.ToInt32(rList[54] + rList[55], 16) * 0.001;
            //    cellDetailList[index].CellVolt_27 = Convert.ToInt32(rList[56] + rList[57], 16) * 0.001;
            //    cellDetailList[index].CellVolt_28 = Convert.ToInt32(rList[58] + rList[59], 16) * 0.001;
            //    cellDetailList[index].CellVolt_29 = Convert.ToInt32(rList[60] + rList[61], 16) * 0.001;
            //    cellDetailList[index].CellVolt_30 = Convert.ToInt32(rList[62] + rList[63], 16) * 0.001;
            //    cellDetailList[index].CellVolt_31 = Convert.ToInt32(rList[64] + rList[65], 16) * 0.001;
            //    cellDetailList[index].CellVolt_32 = Convert.ToInt32(rList[66] + rList[67], 16) * 0.001;
            //    cellDetailList[index].CellVolt_33 = Convert.ToInt32(rList[68] + rList[69], 16) * 0.001;
            //    cellDetailList[index].CellVolt_34 = Convert.ToInt32(rList[70] + rList[71], 16) * 0.001;
            //    cellDetailList[index].CellVolt_35 = Convert.ToInt32(rList[72] + rList[73], 16) * 0.001;
            //    cellDetailList[index].CellVolt_36 = Convert.ToInt32(rList[74] + rList[75], 16) * 0.001;
            //    cellDetailList[index].CellVolt_37 = Convert.ToInt32(rList[76] + rList[77], 16) * 0.001;
            //    cellDetailList[index].CellVolt_38 = Convert.ToInt32(rList[78] + rList[79], 16) * 0.001;
            //    cellDetailList[index].CellVolt_39 = Convert.ToInt32(rList[80] + rList[81], 16) * 0.001;
            //    cellDetailList[index].CellVolt_40 = Convert.ToInt32(rList[82] + rList[83], 16) * 0.001;
            //    cellDetailList[index].CellVolt_41 = Convert.ToInt32(rList[84] + rList[85], 16) * 0.001;
            //    cellDetailList[index].CellVolt_42 = Convert.ToInt32(rList[86] + rList[87], 16) * 0.001;
            //    cellDetailList[index].CellVolt_43 = Convert.ToInt32(rList[88] + rList[89], 16) * 0.001;
            //    cellDetailList[index].CellVolt_44 = Convert.ToInt32(rList[90] + rList[91], 16) * 0.001;
            //    cellDetailList[index].CellVolt_45 = Convert.ToInt32(rList[92] + rList[93], 16) * 0.001;
            //    cellDetailList[index].CellVolt_46 = Convert.ToInt32(rList[94] + rList[95], 16) * 0.001;
            //    cellDetailList[index].CellVolt_47 = Convert.ToInt32(rList[96] + rList[97], 16) * 0.001;
            //    cellDetailList[index].CellVolt_48 = Convert.ToInt32(rList[98] + rList[99], 16) * 0.001;
            //    cellDetailList[index].CellVolt_49 = Convert.ToInt32(rList[100] + rList[101], 16) * 0.001;
            //    cellDetailList[index].CellVolt_50 = Convert.ToInt32(rList[102] + rList[103], 16) * 0.001;
            //    cellDetailList[index].CellVolt_51 = Convert.ToInt32(rList[104] + rList[105], 16) * 0.001;
            //    cellDetailList[index].CellVolt_52 = Convert.ToInt32(rList[106] + rList[107], 16) * 0.001;
            //    cellDetailList[index].CellVolt_53 = Convert.ToInt32(rList[108] + rList[109], 16) * 0.001;
            //    cellDetailList[index].CellVolt_54 = Convert.ToInt32(rList[110] + rList[111], 16) * 0.001;
            //    cellDetailList[index].CellVolt_55 = Convert.ToInt32(rList[112] + rList[113], 16) * 0.001;
            //    cellDetailList[index].CellVolt_56 = Convert.ToInt32(rList[114] + rList[115], 16) * 0.001;
            //    cellDetailList[index].CellVolt_57 = Convert.ToInt32(rList[116] + rList[117], 16) * 0.001;
            //    cellDetailList[index].CellVolt_58 = Convert.ToInt32(rList[118] + rList[119], 16) * 0.001;
            //    cellDetailList[index].CellVolt_59 = Convert.ToInt32(rList[120] + rList[121], 16) * 0.001;
            //    cellDetailList[index].CellVolt_60 = Convert.ToInt32(rList[122] + rList[123], 16) * 0.001;
            //    cellDetailList[index].CellVolt_61 = Convert.ToInt32(rList[124] + rList[125], 16) * 0.001;
            //    cellDetailList[index].CellVolt_62 = Convert.ToInt32(rList[126] + rList[127], 16) * 0.001;
            //    cellDetailList[index].CellVolt_63 = Convert.ToInt32(rList[128] + rList[129], 16) * 0.001;
            //    cellDetailList[index].CellVolt_64 = Convert.ToInt32(rList[130] + rList[131], 16) * 0.001;
            //    cellDetailList[index].CellVolt_65 = Convert.ToInt32(rList[132] + rList[133], 16) * 0.001;
            //    cellDetailList[index].CellVolt_66 = Convert.ToInt32(rList[134] + rList[135], 16) * 0.001;
            //    cellDetailList[index].CellVolt_67 = Convert.ToInt32(rList[136] + rList[137], 16) * 0.001;
            //    cellDetailList[index].CellVolt_68 = Convert.ToInt32(rList[138] + rList[139], 16) * 0.001;
            //    cellDetailList[index].CellVolt_69 = Convert.ToInt32(rList[140] + rList[141], 16) * 0.001;
            //    cellDetailList[index].CellVolt_70 = Convert.ToInt32(rList[142] + rList[143], 16) * 0.001;
            //    cellDetailList[index].CellVolt_71 = Convert.ToInt32(rList[144] + rList[145], 16) * 0.001;
            //    cellDetailList[index].CellVolt_72 = Convert.ToInt32(rList[146] + rList[147], 16) * 0.001;
            //    cellDetailList[index].CellVolt_73 = Convert.ToInt32(rList[148] + rList[149], 16) * 0.001;
            //    cellDetailList[index].CellVolt_74 = Convert.ToInt32(rList[150] + rList[151], 16) * 0.001;
            //    cellDetailList[index].CellVolt_75 = Convert.ToInt32(rList[152] + rList[153], 16) * 0.001;
            //    cellDetailList[index].CellVolt_76 = Convert.ToInt32(rList[154] + rList[155], 16) * 0.001;
            //    cellDetailList[index].CellVolt_77 = Convert.ToInt32(rList[156] + rList[157], 16) * 0.001;
            //    cellDetailList[index].CellVolt_78 = Convert.ToInt32(rList[158] + rList[159], 16) * 0.001;
            //    cellDetailList[index].CellVolt_79 = Convert.ToInt32(rList[160] + rList[161], 16) * 0.001;
            //    cellDetailList[index].CellVolt_80 = Convert.ToInt32(rList[162] + rList[163], 16) * 0.001;
            //    cellDetailList[index].CellVolt_81 = Convert.ToInt32(rList[164] + rList[165], 16) * 0.001;
            //    cellDetailList[index].CellVolt_82 = Convert.ToInt32(rList[166] + rList[167], 16) * 0.001;
            //    cellDetailList[index].CellVolt_83 = Convert.ToInt32(rList[168] + rList[169], 16) * 0.001;
            //    cellDetailList[index].CellVolt_84 = Convert.ToInt32(rList[170] + rList[171], 16) * 0.001;
            //    cellDetailList[index].CellVolt_85 = Convert.ToInt32(rList[172] + rList[173], 16) * 0.001;
            //    cellDetailList[index].CellVolt_86 = Convert.ToInt32(rList[174] + rList[175], 16) * 0.001;
            //    cellDetailList[index].CellVolt_87 = Convert.ToInt32(rList[176] + rList[177], 16) * 0.001;
            //    cellDetailList[index].CellVolt_88 = Convert.ToInt32(rList[178] + rList[179], 16) * 0.001;
            //    cellDetailList[index].CellVolt_89 = Convert.ToInt32(rList[180] + rList[181], 16) * 0.001;
            //    cellDetailList[index].CellVolt_90 = Convert.ToInt32(rList[182] + rList[183], 16) * 0.001;
            //    cellDetailList[index].CellVolt_91 = Convert.ToInt32(rList[184] + rList[185], 16) * 0.001;
            //    cellDetailList[index].CellVolt_92 = Convert.ToInt32(rList[186] + rList[187], 16) * 0.001;
            //    cellDetailList[index].CellVolt_93 = Convert.ToInt32(rList[188] + rList[189], 16) * 0.001;
            //    cellDetailList[index].CellVolt_94 = Convert.ToInt32(rList[190] + rList[191], 16) * 0.001;
            //    cellDetailList[index].CellVolt_95 = Convert.ToInt32(rList[192] + rList[193], 16) * 0.001;
            //    cellDetailList[index].CellVolt_96 = Convert.ToInt32(rList[194] + rList[195], 16) * 0.001;
            //}
            cellDetailList[index].ModuleVolt = cycler.cycler1voltage;

        }
        
        /// <summary>
        /// 실제로 100ms마다 동작될 리스트.
        /// </summary>
        public List<SendCMD> sendList = new List<SendCMD>();

        object send = new object();

        public void SetCyclerStepToMESData(int index)
        {

            //들어온값으로 steplist 새로만듬
            var list = this.totalProcessList[0].ScheduleList[index].CycleList[0].StepList;

            //초기 휴지
            list[0].Time = befDiscRestTime;
            list[0].ClearCaseString = "Time = 00:00:00:" + int.Parse(befDiscRestTime).ToString("D2") + " → Next Step ";
            var cc = list[0].ClearcaseList[3];
            cc.Value = "00:00:00:" + int.Parse(befDiscRestTime).ToString("D2");
            list[0].ClearcaseList[3] = cc;


            //방전
            list[1].Current = discCur;
            list[1].Time = discTime;
            list[1].ClearCaseString = "Time = 00:00:00:" + int.Parse(discTime).ToString("D2") + " → Next Step ";
            cc = list[1].ClearcaseList[3];
            cc.Value = "00:00:00:" + int.Parse(discTime).ToString("D2");
            list[1].ClearcaseList[3] = cc;
            list[1].SafecaseData.CuMax = discCurLimit;
            list[1].SafecaseData.VoMax = safeVoltHighLimit;
            list[1].SafecaseData.VoMin = list[1].Discharge_voltage = safeVoltLowLimit;

            //this.totalProcessList[0].ScheduleList[index].CycleList[0].StepList = list;
            //list = this.totalProcessList[0].ScheduleList[1].CycleList[0].StepList;

            //중간 휴지
            list[2].Time = aftDiscRestTime;
            list[2].ClearCaseString = "Time = 00:00:00:" + int.Parse(aftDiscRestTime).ToString("D2") + " → Next Step ";
            cc = list[2].ClearcaseList[3];
            cc.Value = "00:00:00:" + int.Parse(aftDiscRestTime).ToString("D2");
            list[2].ClearcaseList[3] = cc;

            //충전
            list[3].Current = charCur;
            list[3].Time = charTime;
            list[3].ClearCaseString = "Time = 00:00:00:" + int.Parse(charTime).ToString("D2") + " → Next Step ";
            cc = list[3].ClearcaseList[3];
            cc.Value = "00:00:00:" + int.Parse(charTime).ToString("D2");
            list[3].ClearcaseList[3] = cc;
            list[3].SafecaseData.CuMax = charCurLimit;
            list[3].SafecaseData.VoMax = list[3].Voltage = safeVoltHighLimit;//181217 오타 수정
            list[3].SafecaseData.VoMin = safeVoltLowLimit;

            //종료 휴지
            list[4].Time = aftCharRestTime;
            list[4].ClearCaseString = "Time = 00:00:00:" + int.Parse(aftCharRestTime).ToString("D2") + " → Next Step ";
            cc = list[4].ClearcaseList[3];
            cc.Value = "00:00:00:" + int.Parse(aftCharRestTime).ToString("D2");
            list[4].ClearcaseList[3] = cc;

            this.totalProcessList[0].ScheduleList[index].CycleList[0].StepList = list;


            mmw.totalProcessList = this.totalProcessList;
            this.Dispatcher.Invoke(new Action(() =>
            {
                mmw.CycleListBox.Items.Refresh();
            }));
        }

        /// <summary>
        /// twin - curr / 2
        /// 해당 번호의 스텝리스트를 기준으로 실제 돌릴 sendList를 만드는 부분
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pmo"></param>
        /// <param name="isTwin"></param>
        private void SetToSteplist(List<MiniScheduler.Step> list, BranchMode pmo = BranchMode.CHANNEL1, bool isTwin = true)
        {
            sendList.Clear();

            CCycler.AddToSendListBytes(sendList, new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.INPUT_MC_ON,
                Pmode = pmo,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_INPUT
            });


            //분기선택을 하도록 
            //패러럴 모드를 분기로 사용하라 (1로)
            CCycler.AddToSendListBytes(sendList, new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.OUTPUT_MC_ON,
                Pmode = pmo,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_CHARGE
            });

            foreach (var step in list)
            {
                string time = "0.000";
                try
                {
                    time = (step.ClearcaseList.FirstOrDefault(x => x.TitleValue == "Time") as MiniScheduler.ClearCase).Value;
                }
                catch (Exception)
                {
                }

                double total = 0.0;
                if (time == "0.000" || time == "00:00:00:00")
                {
                    total = 0.1;
                }
                else
                {
                    var tarr = time.Split(':');//시분초
                    total = (int.Parse(tarr[0]) * 86400) + (int.Parse(tarr[1]) * 3600) + (int.Parse(tarr[2]) * 60) + (int.Parse(tarr[3]));
                }

                CMD cmd = CMD.NULL;
                OPMode opm = OPMode.READY;
                RECV_OPMode rop = RECV_OPMode.READY;
                double voltHighLimit = 0.0;
                double voltLowLimit = 0.0;

                double currHighLimit = 0.0;

                switch (step.Step_mode)
                {
                    case "CC": opm = OPMode.CHARGE_CC; break;
                }

                switch (step.Step_type)
                {
                    case "Rest":
                        {
                            cmd = CMD.REST;
                            rop = RECV_OPMode.READY_TO_CHARGE;
                        } break;
                    case "Charge":
                        {
                            cmd = CMD.CHARGE;
                            if (step.SafecaseData.VoMax != "0.000" || step.SafecaseData.VoMax != "0")
                            {
                                double.TryParse(step.SafecaseData.VoMax, out voltHighLimit);
                            }

                            double.TryParse(step.SafecaseData.CuMax, out currHighLimit);

                            if (step.Step_mode == "CC")
                            {
                                opm = OPMode.CHARGE_CC;
                                rop = RECV_OPMode.CHARGE_CC;
                            }
                            else
                            {
                                opm = OPMode.CHARGE_CV;
                                rop = RECV_OPMode.CHARGE_CV;
                            }
                        } break;
                    case "Discharge":
                        {
                            cmd = CMD.DISCHARGE;
                            if (step.SafecaseData.VoMin != "0.000" || step.SafecaseData.VoMin != "0")
                            {
                                double.TryParse(step.SafecaseData.VoMin, out voltLowLimit);
                            }

                            double.TryParse(step.SafecaseData.CuMax, out currHighLimit);

                            if (step.Step_mode == "CC")
                            {
                                opm = OPMode.DISCHARGE_CC;
                                rop = RECV_OPMode.DISCHARGE_CC;
                            }
                            else
                            {
                                opm = OPMode.DISCHARGE_CV;
                                rop = RECV_OPMode.DISCHARGE_CV;
                            }
                        } break;
                }
                var voltage = 0.0;
                if (cmd == CMD.DISCHARGE)
                {
                    double.TryParse(step.Discharge_voltage, out voltage);
                }
                else
                {
                    double.TryParse(step.Voltage, out voltage);
                }

                var current = 0.0;
                double.TryParse(step.Current, out current);

                //if (isTwin)
                //{
                //    current = current * 0.5;
                //}

                CCycler.AddToSendListBytes(sendList, new SendCMD()
                {
                    Duration = total,
                    Index = 0,
                    Cmd = cmd,
                    Pmode = pmo,
                    Opmode = opm,
                    Voltage = voltage,
                    Current = current,
                    NextOPMode = rop,
                    VoltLowLimit = voltLowLimit,
                    VoltHighLimit = voltHighLimit,
                    CurrHighLimit = currHighLimit,
                });
            }

            CCycler.AddToSendListBytes(sendList, new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.REST,
                Pmode = pmo,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_CHARGE
            });

            CCycler.AddToSendListBytes(sendList, new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.OUTPUT_MC_OFF,
                Pmode = pmo,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_INPUT
            });
        }

        public void DCIR_Stopped()
        {
            timeKillEvent(상시저항가져오는타이머_아이디);

            //REST Mode
            cycler.SendToDSP1("100", new byte[] { 0x86, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            cycler.SendToDSP1("100", new byte[] { 0x86, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            cycler.SendToDSP1("100", new byte[] { 0x86, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);

            //OUT MC OFF
            cycler.SendToDSP1("100", new byte[] { 0x84, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            cycler.SendToDSP1("100", new byte[] { 0x84, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            cycler.SendToDSP1("100", new byte[] { 0x84, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            cycler.is84On = true;

            cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            LogState(LogType.Info, "Cycler Mode Reset [FF]");

            // 20191021 Noah Choi 분기박스 mc제어용 릴레이 추가로 인한 코드 추가
            relays.RelayOff("IDO_4");

            if (Protection_ID1 != null)
            {
                Protection_ID1.Abort();
                Protection_ID1 = null;
            }

            if (Protection_DISCHARGE_ID1 != null)
            {
                Protection_DISCHARGE_ID1.Abort();
                Protection_DISCHARGE_ID1 = null;
            }

            if (Protection_CHARGE_ID1 != null)
            {
                Protection_CHARGE_ID1.Abort();
                Protection_CHARGE_ID1 = null;
            }

            DCIRThreadAbort();

            //timeKillEvent(상시저항가져오는타이머_아이디);

        }

        /// <summary>
        /// 충방전시 안전조건 부분
        /// </summary>
        /// <param name="index"></param>
        /// <param name="rECV_OPMode"></param>
        /// <param name="sitem"></param>
        /// <returns></returns>
        public bool CheckSafety(int index, RECV_OPMode rECV_OPMode, SendCMD sitem)
        {
            if (rECV_OPMode == RECV_OPMode.READY)
                return true;

            if (rECV_OPMode == RECV_OPMode.READY_TO_CHARGE)
                return true;

            if (rECV_OPMode == RECV_OPMode.READY_TO_INPUT)
                return true;

            if (rECV_OPMode == RECV_OPMode.NULL)
                return true;

            if (rECV_OPMode == RECV_OPMode.COMPLETE)
                return true;

            if (sitem.Cmd == CMD.REST)
                return true;

            if (sitem.Cmd == CMD.OUTPUT_MC_OFF)
                return true;

            if (sitem.Cmd == CMD.INPUT_MC_ON)
                return true;
            if (sitem.Cmd == CMD.OUTPUT_MC_ON)
                return true;
            if (sitem.Cmd == CMD.NULL)
                return true;

            if (index == 0)
            {
                if (cycler.cycler1OP == RECV_OPMode.CHARGE_CC || cycler.cycler1OP == RECV_OPMode.CHARGE_CV)
                {
                    if (sitem.VoltHighLimit != 0 && cycler.cycler1voltage > sitem.VoltHighLimit)
                    {
                        LogState(LogType.Fail, "[SAFETY] Cycler 1 - Voltage High : " + cycler.cycler1voltage.ToString());
                        return false;
                    }

                    if (sitem.CurrHighLimit != 0 && cycler.cycler1current > sitem.CurrHighLimit)
                    {
                        LogState(LogType.Fail, "[SAFETY] Cycler 1 - Current High : " + cycler.cycler1current.ToString());
                        return false;
                    }
                }

                if (cycler.cycler1OP == RECV_OPMode.DISCHARGE_CC || cycler.cycler1OP == RECV_OPMode.DISCHARGE_CV)
                {
                    if (sitem.VoltLowLimit != 0 && cycler.cycler1voltage < sitem.VoltLowLimit)
                    {
                        LogState(LogType.Fail, "[SAFETY] Cycler 1 - Voltage Low : " + cycler.cycler1voltage.ToString());
                        return false;
                    }
                }

                // 실 전류가 세팅전류의 70%이상 나오지 않을 경우 Fail
                //if ((sitem.Current * 0.7) > cycler.cycler1current)
                //{
                //    LogState(LogType.Fail, "[SAFETY] Cycler 1 - Current is Low than Setting Current(70%) : " + cycler.cycler1current.ToString());
                //    return false;
                //}

                return true;
            }
            else if (index == 1)
            {
                if (cycler.cycler2OP == RECV_OPMode.CHARGE_CC || cycler.cycler2OP == RECV_OPMode.CHARGE_CV)
                {
                    if (sitem.VoltHighLimit != 0 && cycler.cycler2voltage > sitem.VoltHighLimit)
                    {
                        LogState(LogType.Fail, "[SAFETY] Cycler 2 - Voltage High : " + cycler.cycler2voltage.ToString());
                        return false;
                    }

                    if (sitem.CurrHighLimit != 0 && cycler.cycler2current > sitem.CurrHighLimit)
                    {
                        LogState(LogType.Fail, "[SAFETY] Cycler 2 - Current High : " + cycler.cycler2current.ToString());
                        return false;
                    }
                }

                if (cycler.cycler2OP == RECV_OPMode.DISCHARGE_CC || cycler.cycler2OP == RECV_OPMode.DISCHARGE_CV)
                {
                    if (sitem.VoltLowLimit != 0 && cycler.cycler2voltage < sitem.VoltLowLimit)
                    {
                        LogState(LogType.Fail, "[SAFETY] Cycler 2 - Voltage Low : " + cycler.cycler2voltage.ToString());
                        return false;
                    }
                }

                // 실 전류가 세팅전류의 70%이상 나오지 않을 경우 Fail 
                //if ((sitem.Current * 0.7) > cycler.cycler1current)
                //{
                //    LogState(LogType.Fail, "[SAFETY] Cycler 1 - Current is Low than Setting Current(70%) : " + cycler.cycler1current.ToString());
                //    return false;
                //}

                return true;
            }

            return true;

        }


        /// <summary>
        /// set Rest CellVoltages / Current Sensing
        /// </summary>
        void Point_REST_BEF_DISC(object aa)
        {
            var ti = aa as TestItem;
            modulePoint_RES_DIS = cycler.cycler1voltage;

            ti.refValues_.Add(SetCellVoltagesNloggingToIndex(0, "W2MLMTE4003", "[INIT VOLT      ] - MODULE:", modulePoint_RES_DIS.ToString()));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4014", modulePoint_RES_DIS.ToString()));
             
            //ti.refValues_.Add(MakeDetailItem("W1MDMTE4022", getModuleTemp1N2(out dcirTemp)));
            //ti.refValues_.Add(MakeDetailItem("W1MDMTE4022", getModuleTemp1N2(out dcirTemp, out beforeDisc10sTemp1, out beforeDisc10sTemp2)));
        }

        /// <summary>
        /// set Discharge CellVoltages / Current Sensing
        /// </summary>
        void Point_DISC(object aa)
        {
            var ti = aa as TestItem;
            endCurrent_DIS = cycler.cycler1current;
            modulePoint_DIS = cycler.cycler1voltage;

            ti.refValues_.Add(SetCellVoltagesNloggingToIndex(1, "W2MLMTE4004", "[DISCHARGE      ] - MODULE:", modulePoint_DIS.ToString()));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4015", modulePoint_DIS.ToString()));

            //var tempp = 0.0;
            ////ti.refValues_.Add(MakeDetailItem("W1MDMTE4023", getModuleTemp1N2(out tempp)));
            //ti.refValues_.Add(MakeDetailItem("W1MDMTE4023", getModuleTemp1N2(out tempp, out discharge10sTemp1, out discharge10sTemp2)));
        }


        /// <summary>
        /// set Rest CellVoltages / Current Sensing
        /// </summary>
        void Point_REST_AFT_DISC(object aa)
        {
            var ti = aa as TestItem;
            modulePoint_RES_CHA = cycler.cycler1voltage;

            ti.refValues_.Add(SetCellVoltagesNloggingToIndex(2, "W2MLMTE4005", "[BEFORE CHARGE  ] - MODULE:", modulePoint_RES_CHA.ToString()));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4016", modulePoint_RES_CHA.ToString()));

            //var tempp = 0.0;

            //ti.refValues_.Add(MakeDetailItem("W1MDMTE4046", getModuleTemp1N2(out tempp, out beforeChargeTemp1, out beforeChargeTemp2)));
        }

        /// <summary>
        /// set Charge CellVoltages / Current Sensing
        /// </summary>
        void Point_CHAR(object aa)
        {
            var ti = aa as TestItem;
            endCurrent_CHA = cycler.cycler1current;
            modulePoint_CHA = cycler.cycler1voltage;

            ti.refValues_.Add(SetCellVoltagesNloggingToIndex(3, "W2MLMTE4006", "[AFTER CHARGE   ] - MODULE:", modulePoint_CHA.ToString()));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4017", modulePoint_CHA.ToString()));

            //var tempp = 0.0;
            ////ti.refValues_.Add(MakeDetailItem("W1MDMTE4024", getModuleTemp1N2(out tempp)));
            //ti.refValues_.Add(MakeDetailItem("W1MDMTE4024", getModuleTemp1N2(out tempp, out charge10sTemp1, out charge10sTemp2)));
        }

        /// <summary>
        /// set Rest CellVoltages / Current Sensing
        /// </summary>
        //private void Point_REST_AFT_CHAR_5s(object aa)
        //{
        //    var ti = aa as TestItem;

        //    var tempp = 0.0;

        //    ti.refValues_.Add(MakeDetailItem("W1MDMTE4047", getModuleTemp1N2(out tempp, out afterCharge5sTemp1, out afterCharge5sTemp2)));
        //}
         
        /// <summary>
        /// set Rest CellVoltages / Current Sensing
        /// </summary>
        private void Point_REST_AFT_CHAR_10s(object aa)
        {
            var ti = aa as TestItem;

            var moduleVoltage = cycler.cycler1voltage;

            ti.refValues_.Add(SetCellVoltagesNloggingToIndex(4, "W2MLMTE4007", "[FINISH VOLT-10s] - MODULE:", moduleVoltage.ToString()));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4018", moduleVoltage.ToString()));

            //var tempp = 0.0;

            //ti.refValues_.Add(MakeDetailItem("W1MDMTE4048", getModuleTemp1N2(out tempp, out afterCharge10sTemp1, out afterCharge10sTemp2)));
        }
        /// <summary>
        /// set Rest CellVoltages / Current Sensing
        /// </summary>
        private void Point_REST_AFT_CHAR_20s(object aa)
        {
            var ti = aa as TestItem;

            var moduleVoltage = cycler.cycler1voltage;

            ti.refValues_.Add(SetCellVoltagesNloggingToIndex(5, "W2MLMTE4008", "[FINISH VOLT-20s] - MODULE:", moduleVoltage.ToString()));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4019", moduleVoltage.ToString()));
        }
        /// <summary>
        /// set Rest CellVoltages / Current Sensing
        /// </summary>
        private void Point_REST_AFT_CHAR_30s(object aa)
        {
            var ti = aa as TestItem;

            var moduleVoltage = cycler.cycler1voltage;

            ti.refValues_.Add(SetCellVoltagesNloggingToIndex(6, "W2MLMTE4009", "[FINISH VOLT-30s] - MODULE:", moduleVoltage.ToString()));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4020", moduleVoltage.ToString()));
        }
        /// <summary>
        /// set Rest CellVoltages / Current Sensing
        /// </summary>
        private void Point_REST_AFT_CHAR_40s(object aa)
        {
            var ti = aa as TestItem;

            var moduleVoltage = cycler.cycler1voltage;

            ti.refValues_.Add(SetCellVoltagesNloggingToIndex(7, "W2MLMTE4010", "[FINISH VOLT-40s] - MODULE:", moduleVoltage.ToString()));

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4021", moduleVoltage.ToString()));
        }
    }
}