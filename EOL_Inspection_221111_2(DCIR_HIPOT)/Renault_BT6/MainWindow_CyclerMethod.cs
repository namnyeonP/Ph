using Renault_BT6.모듈;
using Renault_BT6.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Renault_BT6
{
    public partial class MainWindow
    {
        //MC 저항 기능 nnkim
        public List<string> rtMcList = new List<string>();

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
        public string befDiscRestTime { get; set; }

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
        /// 모듈 온도보정식 3
        /// </summary>
        public double moduleFomula3 = 0;

        /// <summary>
        /// 셀 온도보정식 1
        /// </summary>
        public double cellFomula1 = 0;

        /// <summary>
        /// 셀 온도보정식 2
        /// </summary>
        public double cellFomula2 = 0;

        /// <summary>
        /// 셀 온도보정식 3
        /// </summary>
        public double cellFomula3 = 0;


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
            try
            {
                if (Device.Cycler.GetDisChgPointList.Count == 0 ||
                        Device.Cycler.GetDisChgPointList[index].CellVoltageList.Count == 0)
                {
                    return;
                }

                cellDetailList[index].CellVolt_1 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[0];
                cellDetailList[index].CellVolt_2 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[1];
                cellDetailList[index].CellVolt_3 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[2];
                cellDetailList[index].CellVolt_4 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[3];
                cellDetailList[index].CellVolt_5 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[4];
                cellDetailList[index].CellVolt_6 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[5];
                cellDetailList[index].CellVolt_7 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[6];
                cellDetailList[index].CellVolt_8 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[7];
                cellDetailList[index].CellVolt_9 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[8];
                cellDetailList[index].CellVolt_10 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[9];
                cellDetailList[index].CellVolt_11 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[10];
                cellDetailList[index].CellVolt_12 = Device.Cycler.GetDisChgPointList[index].CellVoltageList[11];

                cellDetailList[index].ModuleVolt = Device.Cycler.GetDisChgPointList[index].Vlot;

            }
            catch(Exception ex)
            {
                LogState(LogType.Fail, ex.StackTrace);
            }
        }
        
        /// <summary>
        /// 실제로 100ms마다 동작될 리스트.
        /// </summary>
        public List<SendCMD> sendList = new List<SendCMD>();

        object send = new object();

        public void SetCyclerStepToMESData(int index = 0)
        {
            try
            {
                if (CONFIG.EolInspType != InspectionType.EOL)
                    return;

                //들어온값으로 steplist 새로만듬
                var list = this.totalProcessList[0].ScheduleList[index].CycleList[0].StepList;

                if (befDiscRestTime == null)
                {
                    LogState(LogType.Fail, "SetCyclerStepToMESData Data Null");
                    return;
                }

                //초기 휴지
                list[0].Time = befDiscRestTime;
                list[0].ClearCaseString = "Time = 00:00:00:" + int.Parse(befDiscRestTime).ToString("D2") + " → Next Step ";
                var cc = list[0].ClearcaseList[3];
                cc.Value = "00:00:00:" + int.Parse(befDiscRestTime).ToString("D2");
                list[0].ClearcaseList[3] = cc;

                // Rest 안전 조건 없을 경우 0 으로 설정
                list[0].SafecaseData.VoMax = safeVoltHighLimit;
                list[0].SafecaseData.VoMin = safeVoltLowLimit;
                list[0].SafecaseData.CuMax = "0";

                //방전
                list[1].Current = discCur;
                list[1].Time = discTime;
                list[1].ClearCaseString = "Time = 00:00:00:" + int.Parse(discTime).ToString("D2") + " → Next Step ";
                cc = list[1].ClearcaseList[3];
                cc.Value = "00:00:00:" + int.Parse(discTime).ToString("D2");
                list[1].ClearcaseList[3] = cc;

                // 방전 안전 조건
                list[1].SafecaseData.CuMax = discCurLimit;  // 방전 전류 Limit
                list[1].SafecaseData.VoMax = safeVoltHighLimit; // 안전조건 Volt Hight Limit
                list[1].SafecaseData.VoMin = list[1].Discharge_voltage = safeVoltLowLimit;  // 안전조건 Volt Low Limit

                //중간 휴지
                list[2].Time = aftDiscRestTime;
                list[2].ClearCaseString = "Time = 00:00:00:" + int.Parse(aftDiscRestTime).ToString("D2") + " → Next Step ";
                cc = list[2].ClearcaseList[3];
                cc.Value = "00:00:00:" + int.Parse(aftDiscRestTime).ToString("D2");
                list[2].ClearcaseList[3] = cc;

                // Rest 안전 조건 없을 경우 0 으로 설정
                list[2].SafecaseData.VoMax = safeVoltHighLimit;
                list[2].SafecaseData.VoMin = safeVoltLowLimit;
                list[2].SafecaseData.CuMax = "0";

                //충전
                list[3].Current = charCur;
                list[3].Time = charTime;
                list[3].ClearCaseString = "Time = 00:00:00:" + int.Parse(charTime).ToString("D2") + " → Next Step ";
                cc = list[3].ClearcaseList[3];
                cc.Value = "00:00:00:" + int.Parse(charTime).ToString("D2");
                list[3].ClearcaseList[3] = cc;

                // 충전 안전조건
                list[3].SafecaseData.CuMax = charCurLimit; // 충전 전류 Limit
                list[3].SafecaseData.VoMax = list[3].Voltage = safeVoltHighLimit;  // 안전조건 Volt Hight Limit
                list[3].SafecaseData.VoMin = safeVoltLowLimit; // 안전조건 Volt Low Limit

                //종료 휴지
                list[4].Time = aftCharRestTime;
                list[4].ClearCaseString = "Time = 00:00:00:" + int.Parse(aftCharRestTime).ToString("D2") + " → Next Step ";
                cc = list[4].ClearcaseList[3];
                cc.Value = "00:00:00:" + int.Parse(aftCharRestTime).ToString("D2");
                list[4].ClearcaseList[3] = cc;

                // Rest 안전 조건 없을 경우 0 으로 설정
                list[4].SafecaseData.VoMax = safeVoltHighLimit;
                list[4].SafecaseData.VoMin = safeVoltLowLimit;
                list[4].SafecaseData.CuMax = "0";

                totalProcessList[0].ScheduleList[index].CycleList[0].StepList = list;

                mmw.totalProcessList = this.totalProcessList;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    mmw.CycleListBox.Items.Refresh();
                }));
            }
            catch(Exception ex)
            {
                LogState(LogType.Fail, ex.StackTrace);
            }
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

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count > 0)
                {
                    double moduleVoltage = Device.Cycler.GetDisChgPointList[0].Vlot;
                    modulePoint_RES_DIS = moduleVoltage;

                    ti.refValues_.Add(SetCellVoltagesNloggingToIndex(0, 
                                                                                    EOL.MesID.BeforeDischargeCell, 
                                                                                    "[INIT VOLT      ] - MODULE,",
                                                                                    moduleVoltage.ToString()));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.BeforeDischargeVolt,
                                                                  moduleVoltage.ToString()));
                }
            }
            catch

            {

            }
        }

        /// <summary>
        /// set Discharge CellVoltages / Current Sensing
        /// </summary>
        void Point_DISC(object aa)
        {
            var ti = aa as TestItem;

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count > 1)
                {
                    double moduleVoltage = Device.Cycler.GetDisChgPointList[1].Vlot;
                    modulePoint_DIS = moduleVoltage;

                    ti.refValues_.Add(SetCellVoltagesNloggingToIndex(1,
                                                                                    EOL.MesID.DischargeCell, 
                                                                                    "[DISCHARGE      ] - MODULE,",
                                                                                    moduleVoltage.ToString()));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.DischargeVolt, moduleVoltage.ToString()));
                }

            }catch
            {

            }
        }


        /// <summary>
        /// set Rest CellVoltages / Current Sensing
        /// </summary>
        void Point_REST_AFT_DISC(object aa)
        {
            try
            {
                if (Device.Cycler.GetDisChgPointList.Count > 2)
                {
                    var ti = aa as TestItem;

                    double moduleVoltage = Device.Cycler.GetDisChgPointList[2].Vlot;

                    ti.refValues_.Add(SetCellVoltagesNloggingToIndex(2, 
                                                                                    EOL.MesID.BeforeChargeCell, 
                                                                                    "[BEFORE CHARGE  ] - MODULE,",
                                                                                    moduleVoltage.ToString()));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.BeforeChargeVolt, moduleVoltage.ToString()));
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// set Charge CellVoltages / Current Sensing
        /// </summary>
        void Point_CHAR(object aa)
        {
            try
            {
                if (Device.Cycler.GetDisChgPointList.Count > 3)
                {
                    var ti = aa as TestItem;

                    double moduleVoltage = Device.Cycler.GetDisChgPointList[3].Vlot;

                    ti.refValues_.Add(SetCellVoltagesNloggingToIndex(3, 
                                                                                    EOL.MesID.ChargeCell,
                                                                                    "[AFTER CHARGE   ] - MODULE,",
                                                                                    moduleVoltage.ToString()));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ChargeVolt, moduleVoltage.ToString()));
                }
            }
            catch
            {

            }
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
        private void Point_REST_AFT_CHAR(object aa)
        {
            try
            {
                if (Device.Cycler.GetDisChgPointList.Count > 3)
                {
                    var ti = aa as TestItem;

                    double moduleVoltage = Device.Cycler.GetDisChgPointList[4].Vlot;

                    ti.refValues_.Add(SetCellVoltagesNloggingToIndex(4, 
                                                                                    EOL.MesID.ChargeAfterCell,
                                                                                    "[FINISH VOLT] - MODULE,",
                                                                                    moduleVoltage.ToString()));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ChargeAfterVolt, 
                                                                moduleVoltage.ToString()));
                }
            }
            catch
            {

            }
        }
    }
}