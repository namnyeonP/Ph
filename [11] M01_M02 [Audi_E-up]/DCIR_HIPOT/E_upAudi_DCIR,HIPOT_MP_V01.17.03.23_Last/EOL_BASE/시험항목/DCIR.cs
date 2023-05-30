using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using EOL_Inspection.윈도우;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EOL_BASE
{

    public partial class MainWindow
    {
        #region MES Spec To Schedule
        public List<SendCMD> sendList = new List<SendCMD>();
        object send = new object();

        /// <summary>
        /// 제어스펙으로 내려오는 값으로 충방전 스케줄을 변경한다.
        /// </summary>
        /// <param name="index"></param>
        public void SetStepToMES(int index)
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
            list[3].SafecaseData.VoMax = list[3].Voltage = safeVoltHighLimit;
            list[3].SafecaseData.VoMin = safeVoltLowLimit;

            //REST
            list[4].Time = aftCharRestTime;
            list[4].ClearCaseString = "Time = 00:00:00:" + int.Parse(aftCharRestTime).ToString("D2") + " → Next Step ";
            cc = list[4].ClearcaseList[3];
            cc.Value = "00:00:00:" + int.Parse(aftCharRestTime).ToString("D2");
            list[4].ClearcaseList[3] = cc;

            this.Dispatcher.Invoke(new Action(() =>
            {
                this.totalProcessList[0].ScheduleList[index].CycleList[0].StepList = list;
                mmw.totalProcessList = this.totalProcessList;
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
                Cmd = CMD.MC_ON_INPUT,
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
                Cmd = CMD.MC_ON_OUTPUT,
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
                Cmd = CMD.MC_OFF_OUT,
                Pmode = pmo,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_INPUT
            });
        }

        public void DCIR_Stopped()
        {
            cycler.SendToDSP1("100", new byte[] { 0x86, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            cycler.SendToDSP1("100", new byte[] { 0x86, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            cycler.SendToDSP1("100", new byte[] { 0x86, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);

            cycler.SendToDSP1("100", new byte[] { 0x84, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            cycler.SendToDSP1("100", new byte[] { 0x84, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            cycler.SendToDSP1("100", new byte[] { 0x84, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            cycler.is84On = true;
            
            //만약, 통신간 정지할때 신호가 필요하다면 여기서 처리
            LogState(LogType.Info, "Send Cycler 0FF");
            cycler.SendToDSP1("0FF", new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
        }

        /// <summary>
        /// 안전조건에 의해 멈추는 부분
        /// 충전할때는 전압상한, 방전할때는 전압하한을 따른다
        /// 전류는 충방전 동일
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

            if (sitem.Cmd == CMD.MC_OFF_OUT)
                return true;

            if (sitem.Cmd == CMD.MC_ON_INPUT)
                return true;
            if (sitem.Cmd == CMD.MC_ON_OUTPUT)
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
                //if (((sitem.Current * 0.7) * -1) > cycler.cycler1current)
                //{
                //    LogState(LogType.Fail, "[SAFETY] Cycler 1 - Current is Low than Setting Current(70%) : " + cycler.cycler1current.ToString());
                //    return false;
                //}

                return true;
            }

            return true;

        }
        #endregion

        double tempdcirmodule_DIS = 0.0;
        double tempdcirmodule_CHA = 0.0;
        double tempdcircell1_DIS  = 0.0;
        double tempdcircell2_DIS  = 0.0;
        double tempdcircell3_DIS  = 0.0;
        double tempdcircell4_DIS  = 0.0;
        double tempdcircell5_DIS  = 0.0;
        double tempdcircell6_DIS  = 0.0;
        double tempdcircell7_DIS  = 0.0;
        double tempdcircell8_DIS  = 0.0;
        double tempdcircell9_DIS  = 0.0;
        double tempdcircell10_DIS = 0.0;
        double tempdcircell11_DIS = 0.0;
        double tempdcircell12_DIS = 0.0;
        double tempdcircell13_DIS = 0.0;
        double tempdcircell14_DIS = 0.0;
        double tempdcircell15_DIS = 0.0;
        double tempdcircell16_DIS = 0.0;

        double tempdcircell1_CHA = 0.0;
        double tempdcircell2_CHA = 0.0;
        double tempdcircell3_CHA = 0.0;
        double tempdcircell4_CHA = 0.0;
        double tempdcircell5_CHA = 0.0;
        double tempdcircell6_CHA = 0.0;
        double tempdcircell7_CHA = 0.0;
        double tempdcircell8_CHA = 0.0;
        double tempdcircell9_CHA = 0.0;
        double tempdcircell10_CHA = 0.0;
        double tempdcircell11_CHA = 0.0;
        double tempdcircell12_CHA = 0.0;
        double tempdcircell13_CHA = 0.0;
        double tempdcircell14_CHA = 0.0;
        double tempdcircell15_CHA = 0.0;
        double tempdcircell16_CHA = 0.0;

        double modulePoint_RES_DIS = 0.0;
        double modulePoint_DIS = 0.0;
        double modulePoint_RES_CHA = 0.0;
        double modulePoint_CHA = 0.0;
        double endCurrent_DIS = 0.0;
        double endCurrent_CHA = 0.0;
        double calAvg = 0;
        double mTemp_DCIR = 0.0;

        bool is2ndDischarge = false;
        bool is2ndCharge = false;

        double currentMeas_Discharge = 0.0;
        double currentMeas_Charge = 0.0;


        int currentPosition = 0;

        // 190314
        Thread Protection_ID1 = null;
        Thread Protection_CHARGE_ID1 = null;
        Thread Protection_DISCHARGE_ID1 = null;

        Thread Protection_ID2 = null;
        Thread Protection_CHARGE_ID2 = null;
        Thread Protection_DISCHARGE_ID2 = null;
        CMD localitem_ID1;
        CMD localitem_ID2;

        Thread writeCurrentThread;

        /// <summary>
        /// 실제 충방전을 진행하는 부분
        /// DCIR 다른 항목의 데이터도 여기서 전부 만든다.
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        private bool PackDCIR_Dis(TestItem ti)
        {
            isProcessingUI(ti);

            //값 초기화
            tempdcirmodule_CHA = tempdcirmodule_DIS = 0.0;
            tempdcircell1_CHA = tempdcircell1_DIS = 0.0;
            tempdcircell2_CHA = tempdcircell2_DIS = 0.0;
            tempdcircell3_CHA = tempdcircell3_DIS = 0.0;
            tempdcircell4_CHA = tempdcircell4_DIS = 0.0;
            tempdcircell5_CHA = tempdcircell5_DIS = 0.0;
            tempdcircell6_CHA = tempdcircell6_DIS = 0.0;

            modulePoint_RES_CHA = modulePoint_RES_DIS = 0.0;
            modulePoint_CHA = modulePoint_DIS = 0.0;
            endCurrent_CHA = endCurrent_DIS = 0.0;
            calAvg = 0;

            currentPosition = 0;

            //예외로 나갔을때를 위한 상세수집 데이터는 미리 만들어둔다.
            ti.refValues_.Clear();

            ti.refValues_.Add(MakeDetailItem("WP2PTE106"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE107"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE108"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE109"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE110"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE111"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE112"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE113"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE114"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE115"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE116"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE117"));
            ti.refValues_.Add(MakeDetailItem("WP2PTE118"));

            #region DCIR
            if (!cycler.isAlive1)
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }
            SetToSteplist(this.totalProcessList[0].ScheduleList[0].CycleList[0].StepList);
            cycler.is84On = false;

            ti.refValues_.Clear();

            //내부 카운터 5만회를 넘기면 해당 조건에 걸린다.
            //변경 및 초기화 해서 사용하면 됨
            if (counter_Cycler_limit < counter_Cycler)
            {
                ti.Value_ = "LIMIT OF CYCLER COUNT";
                return JudgementTestItem(ti);
            }

            //실제 아래에 충방전 로직을 타기 직전에 카운트를 올린다.
            counter_Cycler++;
            calAvg = temps.tempStr;


            #region Set Detail Items

            //시작전 모듈온도를 잴 때 다음 로직을 사용한다.

            ////Temperature1_DCIR Item
            //relays.RelayOn("IDO_5");
            //Thread.Sleep(500);
            //var resista = keysight.TrySend("MEAS:RES?\n") * 0.001;
            //var caltemp1 = GetTempToResist(resista);
            //ti.refValues_.Add(MakeDetailItem("CTEW2201", caltemp1.ToString()));
            //LogState(LogType.Info, "Thermistor Check 1 - Temp :" + resista.ToString() + "/Temp :" + caltemp1.ToString());
            //relays.RelayOff("IDO_5");
            //Thread.Sleep(500);

            ////Temperature2_DCIR Item
            //relays.RelayOn("IDO_6");
            //Thread.Sleep(500);
            //resista = keysight.TrySend("MEAS:RES?\n") * 0.001;
            //var caltemp2 = GetTempToResist(resista);
            //ti.refValues_.Add(MakeDetailItem("CTEW2202", caltemp2.ToString()));
            //LogState(LogType.Info, "Thermistor Check 2 - Temp :" + resista.ToString() + "/Temp :" + caltemp2.ToString());
            //relays.RelayOff("IDO_6");
            //Thread.Sleep(500);

            ////Ambient Temp_Temp DCIR Item
            //var realTemp = temps.tempStr.ToString();
            //ti.refValues_.Add(MakeDetailItem("CTEW2213", realTemp));
            //LogState(LogType.Info, "Ambient Temp - " + realTemp);

            // (caltemp1 + caltemp2) * 0.5;
            #endregion

            bool isLastRest = false;
            lock (send)
            {
                foreach (var sitem in sendList)
                {
                    if (isStop || ispause)
                    {
                        DCIR_Stopped();
                        ti.Value_ = "EMERGENCY_STOPPED";
                        return JudgementTestItem(ti);
                    }

                    int milisecond = 0;

                    #region 2차 프로텍션 - 충/방전일 때 실제 동작시간 +5초까지만 대기, 이후 팝업, STOP

                    localitem_ID1 = sitem.Cmd;

                    if (sitem.Cmd == CMD.CHARGE && Protection_CHARGE_ID1 == null)
                    {
                        Protection_CHARGE_ID1 = new Thread(() =>
                        {
                            double countLimit = (sitem.Duration * 1000) + 5000; // (10sec * 1000) + 5000 == 15000msec
                            double count = 0;


                            while (localitem_ID1 == CMD.CHARGE)//wjs
                            {
                                count += 1000;
                                Thread.Sleep(1000);
                                if (count > countLimit)
                                {
                                    isStop = true;
                                    this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        RetryWindow rt = new RetryWindow(this);
                                        rt.maintitle.Content = "SAFETY_STOPPED";
                                        rt.reason.Content = "Please check communication to Cycler(Charge after 5sec)";
                                        rt.Show();
                                    }));
                                    break;
                                }
                            }
                            Protection_CHARGE_ID1 = null;
                        }
                        );
                        Protection_CHARGE_ID1.Start();
                    }

                    //if (sitem.Cmd == CMD.DISCHARGE && Protection_DISCHARGE_ID1 == null)
                    //{
                    //    Protection_DISCHARGE_ID1 = new Thread(() =>
                    //    {
                    //        double countLimit = (sitem.Duration * 1000) + 5000; // (10sec * 1000) + 5000 == 15000msec
                    //        double count = 0;


                    //        while (localitem_ID1 == CMD.DISCHARGE)
                    //        {
                    //            count += 1000;
                    //            Thread.Sleep(1000);
                    //            if (count > countLimit)
                    //            {
                    //                isStop = true;
                    //                this.Dispatcher.BeginInvoke(new Action(() =>
                    //                {
                    //                    RetryWindow rt = new RetryWindow(this);
                    //                    rt.maintitle.Content = "SAFETY_STOPPED";
                    //                    rt.reason.Content = "Please check communication to Cycler";
                    //                    rt.Show();
                    //                }));
                    //                break;
                    //            }
                    //        }
                    //        Protection_DISCHARGE_ID1 = null;
                    //    }
                    //    );
                    //    Protection_DISCHARGE_ID1.Start();
                    //}

                    #endregion
                    for (int i = 0; i < sitem.Duration * 10; i++)
                    {

                        #region Sending Sequence
                        if (isStop || ispause)
                        {
                            DCIR_Stopped();
                            ti.Value_ = "EMERGENCY_STOPPED";
                            return JudgementTestItem(ti);
                        }

                        if (!CheckSafety(0, cycler.cycler1OP, sitem))
                        {
                            DCIR_Stopped();
                            ti.Value_ = "SAFETY_STOPPED";
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                var al = new AlarmWindow("SAFETY_STOPPED");
                                al.Show();
                            }));
                            return JudgementTestItem(ti);
                        }


                        cycler.cycler1Header = CMD.NULL;
                        cycler.cycler1OP = RECV_OPMode.NULL;

                        if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
                        {
                            LogState(LogType.Fail, "cycler #1_Send fail");

                            DCIR_Stopped();
                            ti.Value_ = "DSP1_SEND_FAIL";
                            return JudgementTestItem(ti);
                        }

                        //타임아웃을 위한 설정부분
                        var oldTime = DateTime.Now;
                        var ltime = oldTime;

                        var after5s = oldTime.AddSeconds(5);

                        if (sitem.Cmd == CMD.MC_ON_OUTPUT || sitem.Cmd == CMD.MC_ON_INPUT || sitem.Cmd == CMD.MC_OFF_OUT)
                        {   //MC 동작은 최대 10초까지 걸리니, 해당 동작은 15초로 한다.
                            after5s = oldTime.AddSeconds(15);
                        }

                        //해당 슬립이 전체 로직을 100ms로 동작하도록 유지한다.
                        Thread.Sleep(100);


                        while (true)
                        {
                            //명령을 날리고 DSP에서 들어온 값이 기대치일때, 로그를 찍는다.
                            if ((cycler.cycler1Header == sitem.Cmd) && (cycler.cycler1OP == sitem.NextOPMode))
                            {
                                //만약 100ms주기가 아닌 500ms라면 아래 구문을 사용한다.
                                //if (i % 5 == 0)
                                //{
                                var sb = new StringBuilder();
                                sb.Append("cycler1(Front) :");
                                sb.Append(cycler.cycler1Header.ToString());
                                sb.Append(",Voltage,");
                                sb.Append(cycler.cycler1voltage.ToString());
                                sb.Append(",Current,");
                                sb.Append(cycler.cycler1current.ToString());

                                //충방전 도중 셀 전압을 기록해야 한다면, 다음 부분을 사용한다.
                                //sb.Append(",Cell1,");
                                //sb.Append(lin.CV1.ToString());
                                //sb.Append(",Cell2,");
                                //sb.Append(lin.CV2.ToString());
                                //sb.Append(",Cell3,");
                                //sb.Append(lin.CV3.ToString());
                                //sb.Append(",Cell4,");
                                //sb.Append(lin.CV4.ToString());
                                //sb.Append(",Cell5,");
                                //sb.Append(lin.CV5.ToString());
                                //sb.Append(",Cell6,");
                                //sb.Append(lin.CV6.ToString());

                                LogState(LogType.Info, sb.ToString(), null, false);
                                
                                //예상대로 들어왔다면 break지만, 아니라면 아래에 다시 보내는 루트로 빠진다.
                                break;
                            }
                            
                            //일시정지나 정지가 들어온다면, NG로 빠진다.
                            if (isStop || ispause)
                            {
                                DCIR_Stopped();
                                ti.Value_ = "EMERGENCY_STOPPED";
                                return JudgementTestItem(ti);
                            }

                            ltime = DateTime.Now;

                            #region send fail
                            //CAN통신 자체가 실패했을 때 빠져나가는 구문
                            if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
                            {
                                LogState(LogType.Fail, "cycler #1_Send fail");

                                DCIR_Stopped();
                                ti.Value_ = "DSP1_SEND_FAIL";
                                return JudgementTestItem(ti);
                            }

                            #endregion
                            Thread.Sleep(100);

                            //통신간 커맨드가 30초동안 바뀌지 않으면 강제 스탑 및 메시지
                            if (ltime.Ticks >= after5s.Ticks)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    AlarmWindow ar = new AlarmWindow("Disconnected DSP. Check Cycler");
                                    ar.closebt.Content = "Close";
                                    ar.Show();
                                }));

                                LogState(LogType.Fail, "Disconnected DSP");

                                DCIR_Stopped();
                                ti.Value_ = "DISCONNECTED_DSP";
                                return JudgementTestItem(ti);
                            }
                        }
                        #endregion
                        milisecond += 100;

                        #region 초기 휴지 -0.2sec before
                        if (sitem.Cmd == CMD.REST && ((sitem.Duration - 0.2) * 1000) == milisecond && currentPosition == 0)//modulePoint_RES_DIS == 0.0)//휴식 10초중 9초일때 측정
                        {
                            currentPosition = 1;
                            modulePoint_RES_DIS = cycler.cycler1voltage;
                            SetCellDetailList(0);

                            //Cell Voltage 1~3 before discharge
                            var dti = new DetailItems() { Key = "WP2PTE111" };//방전 전 휴지일때 셀전압 상세수집 ID
                            var cell1 = cellDetailList[0].CellVolt_1;
                            var cell2 = cellDetailList[0].CellVolt_2;
                            var cell3 = cellDetailList[0].CellVolt_3;
                            var cell4 = cellDetailList[0].CellVolt_4;
                            var cell5 = cellDetailList[0].CellVolt_5;
                            var cell6 = cellDetailList[0].CellVolt_6;

                            dti.Reportitems.Add(cell1);
                            dti.Reportitems.Add(cell2);
                            dti.Reportitems.Add(cell3);
                            dti.Reportitems.Add(cell4);
                            dti.Reportitems.Add(cell5);
                            dti.Reportitems.Add(cell6);
                            ti.refValues_.Add(dti);
                            LogState(LogType.Info, "[INIT VOLT      ] - MODULE:" + modulePoint_RES_DIS.ToString() +
                                "/CELL1:" + cell1.ToString() + "/CELL2:" + cell2.ToString() + "/CELL3:" + cell3.ToString() +
                                "/CELL4:" + cell4.ToString() + "/CELL5:" + cell5.ToString() + "/CELL6:" + cell6.ToString() +
                                "/TEMP:" + cellDetailList[0].Temp1.ToString());

                            //Module Voltage_Cycler before discharge
                            ti.refValues_.Add(MakeDetailItem("WP2PTE115", modulePoint_RES_DIS.ToString()));//방전 전 휴지일때 모듈전압 상세수집 ID

                            LogState(LogType.TEST, "DCIR - Voltage Point 1:" + sitem.Cmd.ToString() + ":" + milisecond.ToString() + ":" + modulePoint_RES_DIS.ToString());

                        }
                        #endregion                        
                        #region 방전 -0.2 
                        if (sitem.Cmd == CMD.DISCHARGE && ((sitem.Duration - 0.2) * 1000) == milisecond && currentPosition == 2)
                        {
                            endCurrent_DIS = sitem.Current;
                            modulePoint_DIS = cycler.cycler1voltage;
                            SetCellDetailList(1);

                            //Cell Voltage 1~3 at discharge 10s
                            var dti = new DetailItems() { Key = "WP2PTE112" };
                            var cell1 = cellDetailList[1].CellVolt_1;
                            var cell2 = cellDetailList[1].CellVolt_2;
                            var cell3 = cellDetailList[1].CellVolt_3;
                            var cell4 = cellDetailList[1].CellVolt_4;
                            var cell5 = cellDetailList[1].CellVolt_5;
                            var cell6 = cellDetailList[1].CellVolt_6;

                            dti.Reportitems.Add(cell1);
                            dti.Reportitems.Add(cell2);
                            dti.Reportitems.Add(cell3);
                            dti.Reportitems.Add(cell4);
                            dti.Reportitems.Add(cell5);
                            dti.Reportitems.Add(cell6);
                            ti.refValues_.Add(dti);
                            LogState(LogType.Info, "[AFTER DISCHARGE] - MODULE:" + modulePoint_DIS.ToString() +
                                "/CELL1:" + cell1.ToString() + "/CELL2:" + cell2.ToString() + "/CELL3:" + cell3.ToString() +
                                "/CELL4:" + cell4.ToString() + "/CELL5:" + cell5.ToString() + "/CELL6:" + cell6.ToString() +
                                "/TEMP:" + cellDetailList[1].Temp1.ToString());

                            //Module Voltage_Cycler at discharge 10s
                            ti.refValues_.Add(MakeDetailItem("WP2PTE116", modulePoint_DIS.ToString()));

                            LogState(LogType.TEST, "DCIR - Voltage Point 2:" + sitem.Cmd.ToString() + ":" + milisecond.ToString() + ":" + modulePoint_DIS.ToString());

                        }
                        #endregion
                        #region 중간 휴지 -0.2
                        if (sitem.Cmd == CMD.REST && ((sitem.Duration - 0.2) * 1000) == milisecond && currentPosition == 2)// modulePoint_RES_DIS != 0.0 && modulePoint_DIS != 0.0 && !isLastRest)//휴식 10초중 9초일때 측정
                        {
                            currentPosition = 3;
                            SetCellDetailList(2);
                            modulePoint_RES_CHA = cycler.cycler1voltage;
                            //Cell Voltage 1~3 before charge
                            var dti = new DetailItems() { Key = "WP2PTE113" };
                            var cell1 = cellDetailList[2].CellVolt_1;
                            var cell2 = cellDetailList[2].CellVolt_2;
                            var cell3 = cellDetailList[2].CellVolt_3;
                            var cell4 = cellDetailList[2].CellVolt_4;
                            var cell5 = cellDetailList[2].CellVolt_5;
                            var cell6 = cellDetailList[2].CellVolt_6;

                            dti.Reportitems.Add(cell1);
                            dti.Reportitems.Add(cell2);
                            dti.Reportitems.Add(cell3);
                            dti.Reportitems.Add(cell4);
                            dti.Reportitems.Add(cell5);
                            dti.Reportitems.Add(cell6);
                            ti.refValues_.Add(dti);
                            LogState(LogType.Info, "[BEFORE CHARGE  ] - MODULE:" + modulePoint_RES_CHA.ToString() +
                                "/CELL1:" + cell1.ToString() + "/CELL2:" + cell2.ToString() + "/CELL3:" + cell3.ToString() +
                                "/CELL4:" + cell4.ToString() + "/CELL5:" + cell5.ToString() + "/CELL6:" + cell6.ToString() +
                                "/TEMP:" + cellDetailList[2].Temp1.ToString());

                            //Module Voltage_Cycler before charge
                            ti.refValues_.Add(MakeDetailItem("WP2PTE117", modulePoint_RES_CHA.ToString()));
                        }
                        #endregion
                        #region 충전 -0.2
                        if (sitem.Cmd == CMD.CHARGE && ((sitem.Duration - 0.5) * 1000) == milisecond && currentPosition == 3)//!isLastRest)
                        {
                            currentPosition = 4;
                            isLastRest = true;
                            endCurrent_CHA = sitem.Current;
                            modulePoint_CHA = cycler.cycler1voltage;
                            SetCellDetailList(3);

                            //Cell Voltage 1~3 at charge 10s
                            var dti = new DetailItems() { Key = "WP2PTE114" };
                            var cell1 = cellDetailList[3].CellVolt_1;
                            var cell2 = cellDetailList[3].CellVolt_2;
                            var cell3 = cellDetailList[3].CellVolt_3;
                            var cell4 = cellDetailList[3].CellVolt_4;
                            var cell5 = cellDetailList[3].CellVolt_5;
                            var cell6 = cellDetailList[3].CellVolt_6;

                            dti.Reportitems.Add(cell1);
                            dti.Reportitems.Add(cell2);
                            dti.Reportitems.Add(cell3);
                            dti.Reportitems.Add(cell4);
                            dti.Reportitems.Add(cell5);
                            dti.Reportitems.Add(cell6);
                            ti.refValues_.Add(dti);
                            LogState(LogType.Info, "[AFTER CHARGE   ] - MODULE:" + modulePoint_CHA.ToString() +
                                "/CELL1:" + cell1.ToString() + "/CELL2:" + cell2.ToString() + "/CELL3:" + cell3.ToString() +
                                "/CELL4:" + cell4.ToString() + "/CELL5:" + cell5.ToString() + "/CELL6:" + cell6.ToString() +
                                "/TEMP:" + cellDetailList[3].Temp1.ToString());

                            //Module Voltage_Cycler at charge 10s
                            ti.refValues_.Add(MakeDetailItem("WP2PTE118", modulePoint_CHA.ToString()));
                        }
                        #endregion
                        #region 마지막 휴지 -0.2
                        if (sitem.Cmd == CMD.REST && ((sitem.Duration - 0.2) * 1000) == milisecond && currentPosition == 4)// isLastRest)//휴식 10초중 9초일때 측정
                        {
                            currentPosition = 5;
                            SetCellDetailList(4);
                            var moduleVoltage = cycler.cycler1voltage;

                            //Cell Voltage 1~3 at rest time 30s after charge
                            //var dti = new DetailItems() { Key = "CTEW2207" };
                            var cell1 = cellDetailList[4].CellVolt_1;
                            var cell2 = cellDetailList[4].CellVolt_2;
                            var cell3 = cellDetailList[4].CellVolt_3;
                            var cell4 = cellDetailList[4].CellVolt_4;
                            var cell5 = cellDetailList[4].CellVolt_5;
                            var cell6 = cellDetailList[4].CellVolt_6;

                            //dti.Reportitems.Add(cell1);
                            //dti.Reportitems.Add(cell2);
                            //dti.Reportitems.Add(cell3);
                            //dti.Reportitems.Add(cell4);
                            //dti.Reportitems.Add(cell5);
                            //dti.Reportitems.Add(cell6);
                            //ti.refValues_.Add(dti);
                            LogState(LogType.Info, "[FINISH VOLT    ] - MODULE:" + moduleVoltage.ToString() +
                                "/CELL1:" + cell1.ToString() + "/CELL2:" + cell2.ToString() + "/CELL3:" + cell3.ToString() +
                                "/CELL4:" + cell4.ToString() + "/CELL5:" + cell5.ToString() + "/CELL6:" + cell6.ToString() +
                                "/TEMP:" + cellDetailList[4].Temp1.ToString());

                            //Module Voltage_Cycler at rest time 30s after charge
                            //ti.refValues_.Add(MakeDetailItem("CTEW2212", moduleVoltage.ToString()));

                        }
                        #endregion
                    }
                }
            }


            Thread.Sleep(1000);
            cycler.is84On = true;


            DCIR_Stopped();
            Thread.Sleep(1500);
            SetMainVoltage("0.00");
            SetMainCurrent("0.00");
            SetMainCState("Ready");

            SetCounter_Cycler();

            #endregion

            tempdcirmodule_DIS = Math.Abs(((modulePoint_DIS - modulePoint_RES_DIS) / (endCurrent_DIS)) * 1000);//밀리옴
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcirmodule_DIS, moduleFomula1, calAvg, moduleFomula2));
            ti.Value_ = tempdcirmodule_DIS = tempdcirmodule_DIS + (moduleFomula1 * (calAvg - 23) + moduleFomula2);
            LogState(LogType.TEST, "DCIR - Point2:" + modulePoint_DIS.ToString() + ", Point1:" + modulePoint_RES_DIS.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        private bool PackDCIR_Cha(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            tempdcirmodule_CHA = Math.Abs(((modulePoint_CHA - modulePoint_RES_CHA) / (endCurrent_CHA)) * 1000);//밀리옴
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcirmodule_CHA, moduleFomula1, calAvg, moduleFomula2));
            ti.Value_ = tempdcirmodule_CHA = tempdcirmodule_CHA + (moduleFomula1 * (calAvg - 23) + moduleFomula2);
            LogState(LogType.TEST, "DCIR - Point2:" + modulePoint_CHA.ToString() + ", Point1:" + modulePoint_RES_CHA.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());
            return JudgementTestItem(ti);
        }

        private bool PackDCIRCellDCIRSum_Dis(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[0].CellVolt_1;
            var point2 = cellDetailList[1].CellVolt_1;
            tempdcircell1_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            tempdcircell1_DIS = tempdcircell1_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);

            point1 = cellDetailList[0].CellVolt_2;
            point2 = cellDetailList[1].CellVolt_2;
            tempdcircell2_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            tempdcircell2_DIS = tempdcircell2_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);

            point1 = cellDetailList[0].CellVolt_3;
            point2 = cellDetailList[1].CellVolt_3;
            tempdcircell3_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            tempdcircell3_DIS = tempdcircell3_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);

            point1 = cellDetailList[0].CellVolt_4;
            point2 = cellDetailList[1].CellVolt_4;
            tempdcircell4_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            tempdcircell4_DIS = tempdcircell4_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);

            point1 = cellDetailList[0].CellVolt_5;
            point2 = cellDetailList[1].CellVolt_5;
            tempdcircell5_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            tempdcircell5_DIS = tempdcircell5_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);

            point1 = cellDetailList[0].CellVolt_6;
            point2 = cellDetailList[1].CellVolt_6;
            tempdcircell6_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            tempdcircell6_DIS = tempdcircell6_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);

            ti.Value_ = tempdcirmodule_DIS - (tempdcircell1_DIS + tempdcircell2_DIS + tempdcircell3_DIS +
                tempdcircell4_DIS + tempdcircell5_DIS + tempdcircell6_DIS);

            return JudgementTestItem(ti);
        }

        private bool PackDCIRCellDCIRSum_Cha(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[2].CellVolt_1;
            var point2 = cellDetailList[3].CellVolt_1;
            tempdcircell1_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            tempdcircell1_CHA = tempdcircell1_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);

            point1 = cellDetailList[2].CellVolt_2;
            point2 = cellDetailList[3].CellVolt_2;
            tempdcircell2_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            tempdcircell2_CHA = tempdcircell2_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);

            point1 = cellDetailList[2].CellVolt_3;
            point2 = cellDetailList[3].CellVolt_3;
            tempdcircell3_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            tempdcircell3_CHA = tempdcircell3_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);

            point1 = cellDetailList[2].CellVolt_4;
            point2 = cellDetailList[3].CellVolt_4;
            tempdcircell4_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            tempdcircell4_CHA = tempdcircell4_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);

            point1 = cellDetailList[2].CellVolt_5;
            point2 = cellDetailList[3].CellVolt_5;
            tempdcircell5_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            tempdcircell5_CHA = tempdcircell5_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);

            point1 = cellDetailList[2].CellVolt_6;
            point2 = cellDetailList[3].CellVolt_6;
            tempdcircell6_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            tempdcircell6_CHA = tempdcircell6_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);

            ti.Value_ = tempdcirmodule_CHA - (tempdcircell1_CHA + tempdcircell2_CHA + tempdcircell3_CHA +
                tempdcircell4_CHA + tempdcircell5_CHA + tempdcircell6_CHA);

            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_DIS1(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[0].CellVolt_1;
            var point2 = cellDetailList[1].CellVolt_1;
            tempdcircell1_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell1_DIS, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell1_DIS = tempdcircell1_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());
            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_DIS2(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[0].CellVolt_2;
            var point2 = cellDetailList[1].CellVolt_2;
            tempdcircell2_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell2_DIS, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell2_DIS = tempdcircell2_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());
            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_DIS3(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[0].CellVolt_3;
            var point2 = cellDetailList[1].CellVolt_3;
            tempdcircell3_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell3_DIS, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell3_DIS = tempdcircell3_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_DIS4(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[0].CellVolt_4;
            var point2 = cellDetailList[1].CellVolt_4;
            tempdcircell4_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell4_DIS, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell4_DIS = tempdcircell4_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_DIS5(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[0].CellVolt_5;
            var point2 = cellDetailList[1].CellVolt_5;
            tempdcircell5_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell5_DIS, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell5_DIS = tempdcircell5_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_DIS6(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[0].CellVolt_6;
            var point2 = cellDetailList[1].CellVolt_6;
            tempdcircell6_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell6_DIS, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell6_DIS = tempdcircell6_DIS + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_CHA1(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[2].CellVolt_1;
            var point2 = cellDetailList[3].CellVolt_1;
            tempdcircell1_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell1_CHA, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell1_CHA = tempdcircell1_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_CHA.ToString());
            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_CHA2(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[2].CellVolt_2;
            var point2 = cellDetailList[3].CellVolt_2;
            tempdcircell2_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell2_CHA, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell2_CHA = tempdcircell2_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_CHA.ToString());
            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_CHA3(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[2].CellVolt_3;
            var point2 = cellDetailList[3].CellVolt_3;
            tempdcircell3_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell3_CHA, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell3_CHA = tempdcircell3_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_CHA.ToString());
            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_CHA4(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[2].CellVolt_4;
            var point2 = cellDetailList[3].CellVolt_4;
            tempdcircell4_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell4_CHA, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell4_CHA = tempdcircell4_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_CHA.ToString());
            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_CHA5(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[2].CellVolt_5;
            var point2 = cellDetailList[3].CellVolt_5;
            tempdcircell5_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell5_CHA, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell5_CHA = tempdcircell5_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_CHA.ToString());
            return JudgementTestItem(ti);
        }

        private bool Cell_DCIR_CHA6(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var point1 = cellDetailList[2].CellVolt_6;
            var point2 = cellDetailList[3].CellVolt_6;
            tempdcircell6_CHA = Math.Abs(((point2 - point1) / (endCurrent_CHA)) * 1000);
            LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcircell6_CHA, cellFomula1, calAvg, cellFomula2));
            ti.Value_ = tempdcircell6_CHA = tempdcircell6_CHA + (cellFomula1 * (calAvg - 23) + cellFomula2);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_CHA.ToString());
            return JudgementTestItem(ti);
        }

        private bool DCIR_Cell_Deviation(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(100);
            var dd = new List<double>();
            dd.Add(tempdcircell1_DIS);
            dd.Add(tempdcircell2_DIS);
            dd.Add(tempdcircell3_DIS);

            var max = dd.Max();
            var min = dd.Min();

            ti.Value_ = max - min;

            LogState(LogType.Info, "DCIR Cell Deviation - Max :" + max.ToString() + "/Min :" + min.ToString());
            return JudgementTestItem(ti);
        }

        private bool DCIR_Ratio(TestItem ti)
        {
            isProcessingUI(ti);

            var list = new List<double>();
            list.Add(tempdcircell1_DIS);
            list.Add(tempdcircell2_DIS);
            list.Add(tempdcircell3_DIS);

            var max = list.Max();
            var min = list.Min();

            ti.Value_ = max / min * 100;

            LogState(LogType.Info,
                "DCIR Ratio - Max Cell DCIR :" + max.ToString() +
                "/Min Cell DCIR :" + min.ToString() +
                "/Result :" + ti.Value_.ToString());

            return JudgementTestItem(ti);
        }

    }
}
