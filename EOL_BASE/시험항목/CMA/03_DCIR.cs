using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using Microsoft.Win32;
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
        //double tempdcircell1 = 0.0;
        //double tempdcircell2 = 0.0;
        //double tempdcircell3 = 0.0;
        //double tempdcircell4 = 0.0;
        //double tempdcircell5 = 0.0;
        //double tempdcircell6 = 0.0;
        //double tempdcircell7 = 0.0;
        //double tempdcircell8 = 0.0;
        ////double tempdcircell9 = 0.0;
        ////double tempdcircell10 = 0.0;
        ////double tempdcircell11 = 0.0;
        ////double tempdcircell12 = 0.0;
        ////double tempdcircell13 = 0.0;
        ////double tempdcircell14 = 0.0;
        ////double tempdcircell15 = 0.0;
        ////double tempdcircell16 = 0.0;

        //bool isCyclerUse = false;

        //RegistryKey rk;

        //public bool Module_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    InitDCIRVariables();

        //    atsw.LoadSpec(setting);

        //    #region DCIR
        //    if (!cycler.isAlive1)
        //    {
        //        ti.Value_ = _DEVICE_NOT_READY;
        //        return JudgementTestItem(ti);
        //    }

        //    if (counter_Cycler_limit < counter_Cycler)
        //    {
        //        ti.Value_ = "LIMIT OF CYCLER COUNT";
        //        return JudgementTestItem(ti);
        //    }

        //    calAvg = temps.tempStr;
        //    LogState(LogType.Info, "Room Temp :" + calAvg.ToString());
            
        //    cycler.is84On = false;

        //    ti.refValues_.Clear();

        //    counter_Cycler++;

        //    while (GetCyclerFlag())
        //    {
        //        LogState(LogType.Info, "Waiting to Use Cycler", null, true, false);
        //        Thread.Sleep(100);
        //        if (isStop || ispause)
        //        {
        //            DCIR_Stopped();
        //            ti.Value_ = _EMG_STOPPED;
        //            return JudgementTestItem(ti);
        //        }
        //    }

        //    SetEnableCyclerFlag();

        //    //50. DCIR 검사 충방전기 리셋(FF) 관련 로그 추가
        //    //190108 by grchoi
        //    cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        //    LogState(LogType.Info, "Cycler Mode Reset [ - FF]");
        //    Thread.Sleep(500);

        //    if (!ModeSet(ti))
        //    {
        //        SetDisableCyclerFlag();
        //        return false;
        //    }

        //    if (!Do_Rest_Charge(ti))
        //    {
        //        SetDisableCyclerFlag();
        //        return false;
        //    }

        //    if (!ModeSet_Release(ti))
        //    {
        //        SetDisableCyclerFlag();
        //        return false;
        //    }

        //    SetDisableCyclerFlag();

        //    Thread.Sleep(1000);
        //    //DCIR_Stopped();
        //    SetMainVoltage("0.00");
        //    SetMainCurrent("0.00");
        //    SetMainCState("Ready");
        //    Thread.Sleep(500);

        //    #endregion

        //    SetCounter_Cycler();

        //    var point1 = modulePoint_RES_DIS;
        //    var point2 = modulePoint_DIS;
        //    //tempdcirmodule_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);//밀리옴

        //    // Vad = voltage after discharge(10s)
        //    // Vac = voltage after charge(10s)
        //    // Id = current discharge
        //    // Ic = current charge
        //    // dcir = (((Id * Vad) + (Ic * Vac)) / (Id.Abs + Ic.Abs)) * 1000
        //    tempdcirmodule_DIS = (((-endCurrent_DIS * modulePoint_DIS) + (endCurrent_CHA * modulePoint_CHA)) / (Math.Pow(endCurrent_DIS, 2) + Math.Pow(endCurrent_CHA, 2))) * 1000;
        //    LogState(LogType.TEST, "DCIR Value:" +
        //        string.Format("((({0} * {1}) + ({2} * {3})) / ({4} + {5})) * 1000 = {6}", -endCurrent_DIS, modulePoint_DIS,
        //        endCurrent_CHA, modulePoint_CHA, Math.Pow(endCurrent_DIS, 2), Math.Pow(endCurrent_CHA, 2), tempdcirmodule_DIS));

        //    ti.Value_ = tempdcirmodule_DIS;

        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //    string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //    tempdcirmodule_DIS));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcirmodule_DIS, moduleFomula1, calAvg, moduleFomula2));
        //    //ti.Value_ = tempdcirmodule_DIS = (tempdcirmodule_DIS + (moduleFomula1 * (calAvg - 23))) + moduleFomula2;
        //    //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    ModuleTempRevision(ti);

        //    return JudgementTestItem(ti);
        //}

        ///// <summary>
        ///// module temperature revision method
        ///// 2018.10.31 jeonhj
        ///// </summary>
        ///// <param name="ti"></param>
        ///// <returns></returns>
        //private string ModuleTempRevision(TestItem ti)
        //{
        //    string regSubkey = "Software\\EOL_Trigger\\Relays";
        //    rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

        //    string temperature = rk.GetValue("NHT_RS232_TEMP").ToString();
        //    double ret_value = 0.0;

        //    double temper = double.Parse(temperature);

        //    double temp_value = double.Parse(ti.Value_.ToString());

        //    string temp_ = "";

        //    if ((temper >= double.Parse(setting.LowerTemp1)) && (temper < double.Parse(setting.HigherTemp1)))
        //    {
        //        temp_ = setting.Resistance1;
        //        if (temp_value <= double.Parse(setting.Resistance1))
        //            ti.Value_ = 0;
        //        else
        //            ti.Value_ = -1;
        //    }
        //    else if ((temper >= double.Parse(setting.LowerTemp2)) && (temper < double.Parse(setting.HigherTemp2)))
        //    {
        //        temp_ = setting.Resistance2;
        //        if (temp_value <= double.Parse(setting.Resistance2))
        //            ret_value = 0;
        //        else
        //            ret_value = -1;
        //    }
        //    else if ((temper >= double.Parse(setting.LowerTemp3)) && (temper < double.Parse(setting.HigherTemp3)))
        //    {
        //        temp_ = setting.Resistance3;
        //        if (temp_value <= double.Parse(setting.Resistance3))
        //            ret_value = 0;
        //        else
        //            ret_value = -1;
        //    }
        //    else if ((temper >= double.Parse(setting.LowerTemp4)) && (temper < double.Parse(setting.HigherTemp4)))
        //    {
        //        temp_ = setting.Resistance4;
        //        if (temp_value <= double.Parse(setting.Resistance4))
        //            ret_value = 0;
        //        else
        //            ret_value = -1;
        //    }
        //    else if ((temper >= double.Parse(setting.LowerTemp5)) && (temper < double.Parse(setting.HigherTemp5)))
        //    {
        //        temp_ = setting.Resistance5;
        //        if (temp_value <= double.Parse(setting.Resistance5))
        //            ret_value = 0;
        //        else
        //            ret_value = -1;
        //    }
        //    else
        //    {
        //        ret_value = -1; // not corret temperature range
        //    }

        //    ti.Value_ = ret_value;

        //    LogState(LogType.Info, string.Format("Temperature: {0}, Module DCIR: {1} <= {2}", temperature, temp_value, temp_));

        //    return temp_value.ToString();
        //}

        //private bool ModeSet(TestItem ti)
        //{
        //    var str = "0AA";
        //    //switch (localTypes)
        //    //{
        //    //    case 2 : str = "0AA"; break;
        //    //    case 0 : str = "0BB"; break;
        //    //}

        //    cycler.SendToDSP1(str, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        //    LogState(LogType.Info, "Cycler Mode Set ["+str+"]");
        //    Thread.Sleep(500);
        //    if (cycler.cycler1voltage < 10)
        //    {
        //        ti.Value_ = _DEVICE_NOT_READY;
        //        cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        //        LogState(LogType.Info, "Cycler Mode Reset [ - FF]");
        //        return JudgementTestItem(ti);
        //    }

        //    return true;
        //}

        //private bool ModeSet_Release(TestItem ti)
        //{
        //    var str = "0FF";

        //    cycler.SendToDSP1(str, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        //    LogState(LogType.Info, "Cycler Mode Set [" + str + "]");
        //    Thread.Sleep(500);
        //    if (cycler.cycler1voltage > 10)
        //    {
        //        ti.Value_ = _DEVICE_NOT_READY;
        //        cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        //        LogState(LogType.Info, "Cycler Mode Reset [ - FF]");
        //        return JudgementTestItem(ti);
        //    }

        //    return true;
        //}

        //private SendCMD MakeOneStepToSendCMD(MiniScheduler.Step step)
        //{
        //    string time = "0.000";
        //    try
        //    {
        //        time = (step.ClearcaseList.FirstOrDefault(x => x.TitleValue == "Time") as MiniScheduler.ClearCase).Value;
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    double total = 0.0;
        //    if (time == "0.000" || time == "00:00:00:00")
        //    {
        //        total = 0.1;
        //    }
        //    else
        //    {
        //        var tarr = time.Split(':');//시분초
        //        total = (int.Parse(tarr[0]) * 86400) + (int.Parse(tarr[1]) * 3600) + (int.Parse(tarr[2]) * 60) + (int.Parse(tarr[3]));
        //    }

        //    CMD cmd = CMD.NULL;
        //    OPMode opm = OPMode.READY;
        //    RECV_OPMode rop = RECV_OPMode.READY;
        //    double voltHighLimit = 0.0;
        //    double voltLowLimit = 0.0;

        //    double currHighLimit = 0.0;

        //    switch (step.Step_mode)
        //    {
        //        case "CC": opm = OPMode.CHARGE_CC; break;
        //    }

        //    switch (step.Step_type)
        //    {
        //        case "Rest":
        //            {
        //                cmd = CMD.REST;
        //                rop = RECV_OPMode.READY_TO_CHARGE;
        //            } break;
        //        case "Charge":
        //            {
        //                cmd = CMD.CHARGE;
        //                if (step.SafecaseData.VoMax != "0.000" || step.SafecaseData.VoMax != "0")
        //                {
        //                    double.TryParse(step.SafecaseData.VoMax, out voltHighLimit);
        //                }

        //                double.TryParse(step.SafecaseData.CuMax, out currHighLimit);

        //                if (step.Step_mode == "CC")
        //                {
        //                    opm = OPMode.CHARGE_CC;
        //                    rop = RECV_OPMode.CHARGE_CC;
        //                }
        //                else
        //                {
        //                    opm = OPMode.CHARGE_CV;
        //                    rop = RECV_OPMode.CHARGE_CV;
        //                }
        //            } break;
        //        case "Discharge":
        //            {
        //                cmd = CMD.DISCHARGE;
        //                if (step.SafecaseData.VoMin != "0.000" || step.SafecaseData.VoMin != "0")
        //                {
        //                    double.TryParse(step.SafecaseData.VoMin, out voltLowLimit);
        //                }

        //                double.TryParse(step.SafecaseData.CuMax, out currHighLimit);

        //                if (step.Step_mode == "CC")
        //                {
        //                    opm = OPMode.DISCHARGE_CC;
        //                    rop = RECV_OPMode.DISCHARGE_CC;
        //                }
        //                else
        //                {
        //                    opm = OPMode.DISCHARGE_CV;
        //                    rop = RECV_OPMode.DISCHARGE_CV;
        //                }
        //            } break;
        //    }
        //    var voltage = 0.0;
        //    if (cmd == CMD.DISCHARGE)
        //    {
        //        double.TryParse(step.Discharge_voltage, out voltage);
        //    }
        //    else
        //    {
        //        double.TryParse(step.Voltage, out voltage);
        //    }

        //    var current = 0.0;
        //    double.TryParse(step.Current, out current);

        //    return new SendCMD()
        //    {
        //        Duration = total,
        //        Index = 0,
        //        Cmd = cmd,
        //        Pmode = BranchMode.CHANNEL1,
        //        Opmode = opm,
        //        Voltage = voltage,
        //        Current = current,
        //        NextOPMode = rop,
        //        VoltLowLimit = voltLowLimit,
        //        VoltHighLimit = voltHighLimit,
        //        CurrHighLimit = currHighLimit,
        //    };
            
        //}

        //private SendCMD MakeSendCMD_INPUT_MC_ON()
        //{
        //    return new SendCMD()
        //    {
        //        Duration = 0.1,
        //        Index = 0,
        //        Cmd = CMD.INPUT_MC_ON,
        //        Pmode = BranchMode.CHANNEL1,
        //        Opmode = OPMode.READY,
        //        Voltage = 0,
        //        Current = 0,
        //        NextOPMode = RECV_OPMode.READY_TO_INPUT
        //    };
        //}

        //private SendCMD MakeSendCMD_OUTPUT_MC_ON()
        //{
        //    return new SendCMD()
        //    {
        //        Duration = 0.1,
        //        Index = 0,
        //        Cmd = CMD.OUTPUT_MC_ON,
        //        Pmode = BranchMode.CHANNEL1,
        //        Opmode = OPMode.READY,
        //        Voltage = 0,
        //        Current = 0,
        //        NextOPMode = RECV_OPMode.READY_TO_CHARGE
        //    };
        //}

        //private SendCMD MakeSendCMD_REST()
        //{
        //    return new SendCMD()
        //    {
        //        Duration = 0.1,
        //        Index = 0,
        //        Cmd = CMD.REST,
        //        Pmode = BranchMode.CHANNEL1,
        //        Opmode = OPMode.READY,
        //        Voltage = 0,
        //        Current = 0,
        //        NextOPMode = RECV_OPMode.READY_TO_CHARGE
        //    };
        //}

        //private SendCMD MakeSendCMD_OUTPUT_MC_OFF()
        //{
        //    return new SendCMD()
        //    {
        //        Duration = 0.1,
        //        Index = 0,
        //        Cmd = CMD.OUTPUT_MC_OFF,
        //        Pmode = BranchMode.CHANNEL1,
        //        Opmode = OPMode.READY,
        //        Voltage = 0,
        //        Current = 0,
        //        NextOPMode = RECV_OPMode.READY_TO_INPUT
        //    };
        //}

        ///// <summary>
        ///// schedule core
        ///// </summary>
        ///// <param name="ti"></param>
        ///// <returns></returns>
        //public bool Do_Rest_Charge(TestItem ti)
        //{
        //    sendList.Clear();

        //    CCycler.AddToSendListBytes(sendList, MakeSendCMD_INPUT_MC_ON());
        //    CCycler.AddToSendListBytes(sendList, MakeSendCMD_OUTPUT_MC_ON());

        //    //190102 임의로 스텝갯수를 변경하는 경우, 유동적으로 동작하도록 변경
        //    var stepCnt = this.totalProcessList[0].ScheduleList[0].CycleList[0].StepList.Count;

        //    for (int stepIndex = 0; stepIndex < stepCnt; stepIndex++)
        //    {
        //        var step = this.totalProcessList[0].ScheduleList[0].CycleList[0].StepList[stepIndex];
        //        CCycler.AddToSendListBytes(sendList, MakeOneStepToSendCMD(step));
        //    }

        //    CCycler.AddToSendListBytes(sendList, MakeSendCMD_REST());
        //    CCycler.AddToSendListBytes(sendList, MakeSendCMD_OUTPUT_MC_OFF());

        //    int localpos = 0;

        //    lock (send)
        //    {
        //        foreach (var sitem in sendList)
        //        {
        //            if (isStop || ispause)
        //            {
        //                DCIR_Stopped();
        //                ti.Value_ = _EMG_STOPPED;
        //                return JudgementTestItem(ti);
        //            }

        //            int milisecond = 0;

        //            for (int i = 0; i < sitem.Duration * 10; i++)
        //            {
        //                #region Sending Sequence
        //                if (isStop || ispause)
        //                {
        //                    DCIR_Stopped();
        //                    ti.Value_ = _EMG_STOPPED;
        //                    return JudgementTestItem(ti);
        //                }

        //                if (!CheckSafety(0, cycler.cycler1OP, sitem))
        //                {
        //                    DCIR_Stopped();
        //                    ti.Value_ = _CYCLER_SAFETY;
        //                    this.Dispatcher.BeginInvoke(new Action(() =>
        //                    {
        //                        MessageBox.Show("SAFETY_STOPPED", "Cycler Suspended", MessageBoxButton.OK, MessageBoxImage.Information);
        //                    }));
        //                    return JudgementTestItem(ti);
        //                }


        //                cycler.cycler1Header = CMD.NULL;
        //                cycler.cycler1OP = RECV_OPMode.NULL;

        //                if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
        //                {
        //                    LogState(LogType.Fail, "cycler #1_Send fail");

        //                    DCIR_Stopped();
        //                    ti.Value_ = _COMM_FAIL;
        //                    return JudgementTestItem(ti);
        //                }

        //                //만약 3초간 안보내주면 다시보내달라고 하고
        //                //3초기다렸다가 안보내주면 fail
        //                var oldTime = DateTime.Now;
        //                var ltime = oldTime;

        //                var after5s = oldTime.AddSeconds(30);

        //                Thread.Sleep(100);


        //                while (true)
        //                {
        //                    if ((cycler.cycler1Header == sitem.Cmd) && (cycler.cycler1OP == sitem.NextOPMode))
        //                    {
        //                        var sb = new StringBuilder();
        //                        sb.Append("cycler1(Front) :");
        //                        sb.Append(cycler.cycler1Header.ToString());
        //                        sb.Append(",Voltage,");
        //                        sb.Append(cycler.cycler1voltage.ToString());
        //                        sb.Append(",Current,");
        //                        sb.Append(cycler.cycler1current.ToString());


        //                        LogState(LogType.Info, sb.ToString(), null, false);
        //                        break;
        //                    }

        //                    if (isStop || ispause)
        //                    {
        //                        DCIR_Stopped();
        //                        ti.Value_ = _EMG_STOPPED;
        //                        return JudgementTestItem(ti);
        //                    }

        //                    ltime = DateTime.Now;

        //                    #region send fail
        //                    if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
        //                    {
        //                        LogState(LogType.Fail, "cycler #1_Send fail");

        //                        DCIR_Stopped();
        //                        ti.Value_ = _COMM_FAIL;
        //                        return JudgementTestItem(ti);
        //                    }

        //                    #endregion
        //                    Thread.Sleep(100);

        //                    if (ltime.Ticks >= after5s.Ticks)
        //                    {
        //                        Dispatcher.Invoke(new Action(() =>
        //                        {
        //                            MessageBox.Show("Disconnected DSP", "Critical");
        //                        }));

        //                        LogState(LogType.Fail, "Disconnected DSP");

        //                        DCIR_Stopped();
        //                        ti.Value_ = _COMM_FAIL;

        //                        return JudgementTestItem(ti);
        //                    }
        //                }
        //                #endregion
        //                milisecond += 100;

        //                //초기 휴지 9.5
        //                if (sitem.Cmd == CMD.REST
        //                    && ((sitem.Duration - 0.2) * 1000) == milisecond && localpos == 0)
        //                {
        //                    dp_rest_bef_disc = new Thread(new ParameterizedThreadStart(Point_REST_BEF_DISC));
        //                    dp_rest_bef_disc.Start(ti);
        //                    localpos = 1;
        //                }

        //                //방전 9.5
        //                if (sitem.Cmd == CMD.DISCHARGE
        //                    && ((sitem.Duration - 0.2) * 1000) == milisecond && localpos == 1)
        //                {
        //                    dp_disc = new Thread(new ParameterizedThreadStart(Point_DISC));
        //                    dp_disc.Start(ti);
        //                    localpos = 2;
        //                }

        //                //중간 휴지 9.5
        //                if (sitem.Cmd == CMD.REST
        //                    && ((sitem.Duration - 0.2) * 1000) == milisecond && localpos == 2)
        //                {
        //                    dp_rest_aft_disc = new Thread(new ParameterizedThreadStart(Point_REST_AFT_DISC));
        //                    dp_rest_aft_disc.Start(ti);
        //                    localpos = 3;
        //                }

        //                ////충전 9.5
        //                if (sitem.Cmd == CMD.CHARGE
        //                    && ((sitem.Duration - 0.2) * 1000) == milisecond && localpos == 3)
        //                {
        //                    dp_char = new Thread(new ParameterizedThreadStart(Point_CHAR));
        //                    dp_char.Start(ti);
        //                    localpos = 4;
        //                }


        //                //끝나기전
        //                if (sitem.Cmd == CMD.REST
        //                    && ((sitem.Duration - 0.2) * 1000) == milisecond && localpos == 4)
        //                {
        //                    dp_rest_aft_char = new Thread(new ParameterizedThreadStart(Point_REST_AFT_CHAR));
        //                    dp_rest_aft_char.Start(ti);
        //                    localpos = 5;

        //                }
        //            }
        //        }
        //    }

        //    return true;
        //}

        //List<double> cellDcir = new List<double>();
        //double tempdcirMax = 0.0;
        //double tempdcirMin = 0.0;

        //public bool Cell_DCIR_High(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    //tempdcircell1 = Math.Abs(((cellDetailList[1].CellVolt_1 - cellDetailList[0].CellVolt_1) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell2 = Math.Abs(((cellDetailList[1].CellVolt_2 - cellDetailList[0].CellVolt_2) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell3 = Math.Abs(((cellDetailList[1].CellVolt_3 - cellDetailList[0].CellVolt_3) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell4 = Math.Abs(((cellDetailList[1].CellVolt_4 - cellDetailList[0].CellVolt_4) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell5 = Math.Abs(((cellDetailList[1].CellVolt_5 - cellDetailList[0].CellVolt_5) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell6 = Math.Abs(((cellDetailList[1].CellVolt_6 - cellDetailList[0].CellVolt_6) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell7 = Math.Abs(((cellDetailList[1].CellVolt_7 - cellDetailList[0].CellVolt_7) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell8 = Math.Abs(((cellDetailList[1].CellVolt_8 - cellDetailList[0].CellVolt_8) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell9 = Math.Abs(((cellDetailList[1].CellVolt_9 - cellDetailList[0].CellVolt_9) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell10 = Math.Abs(((cellDetailList[1].CellVolt_10 - cellDetailList[0].CellVolt_10) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell11 = Math.Abs(((cellDetailList[1].CellVolt_11 - cellDetailList[0].CellVolt_11) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell12 = Math.Abs(((cellDetailList[1].CellVolt_12 - cellDetailList[0].CellVolt_12) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell13 = Math.Abs(((cellDetailList[1].CellVolt_13 - cellDetailList[0].CellVolt_13) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell14 = Math.Abs(((cellDetailList[1].CellVolt_14 - cellDetailList[0].CellVolt_14) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell15 = Math.Abs(((cellDetailList[1].CellVolt_15 - cellDetailList[0].CellVolt_15) / (endCurrent_DIS)) * 1000);
        //    //tempdcircell16 = Math.Abs(((cellDetailList[1].CellVolt_16 - cellDetailList[0].CellVolt_16) / (endCurrent_DIS)) * 1000);

        //    tempdcircell1 = CalcCellDCIR(1, cellDetailList[1].CellVolt_1, cellDetailList[3].CellVolt_1);
        //    tempdcircell2 = CalcCellDCIR(2, cellDetailList[1].CellVolt_2, cellDetailList[3].CellVolt_2);
        //    tempdcircell3 = CalcCellDCIR(3, cellDetailList[1].CellVolt_3, cellDetailList[3].CellVolt_3);
        //    tempdcircell4 = CalcCellDCIR(4, cellDetailList[1].CellVolt_4, cellDetailList[3].CellVolt_4);
        //    // cell voltage measure point 1,2,3,4,6,7,8,9
        //    tempdcircell5 = CalcCellDCIR(5, cellDetailList[1].CellVolt_6, cellDetailList[3].CellVolt_6);
        //    tempdcircell6 = CalcCellDCIR(6, cellDetailList[1].CellVolt_7, cellDetailList[3].CellVolt_7);
        //    tempdcircell7 = CalcCellDCIR(7, cellDetailList[1].CellVolt_8, cellDetailList[3].CellVolt_8);
        //    tempdcircell8 = CalcCellDCIR(8, cellDetailList[1].CellVolt_9, cellDetailList[3].CellVolt_9);

        //    cellDcir.Clear();
        //    cellDcir.Add(tempdcircell1);
        //    cellDcir.Add(tempdcircell2);
        //    cellDcir.Add(tempdcircell3);
        //    cellDcir.Add(tempdcircell4);
        //    cellDcir.Add(tempdcircell5);
        //    cellDcir.Add(tempdcircell6);
        //    cellDcir.Add(tempdcircell7);
        //    cellDcir.Add(tempdcircell8);
        //    //cellDcir.Add(tempdcircell9);
        //    //cellDcir.Add(tempdcircell10);
        //    //cellDcir.Add(tempdcircell11);
        //    //cellDcir.Add(tempdcircell12);
        //    //cellDcir.Add(tempdcircell13);
        //    //cellDcir.Add(tempdcircell14);
        //    //cellDcir.Add(tempdcircell15);
        //    //cellDcir.Add(tempdcircell16);

        //    ti.Value_ = tempdcirMax = cellDcir.Max();

        //    int index = cellDcir.IndexOf(tempdcirMax);

        //    CellTempRevision(ti, index);

        //    return JudgementTestItem(ti);
        //}

        //private double CalcCellDCIR(int cnt, double cell_point1, double cell_point2)
        //{
        //    var point1 = cell_point1;
        //    var point2 = cell_point2;

        //    double dcircell = 0.0;

        //    dcircell = (((-endCurrent_DIS * point1) + (endCurrent_CHA * point2)) / (Math.Pow(endCurrent_DIS, 2) + Math.Pow(endCurrent_CHA, 2))) * 1000;
        //    LogState(LogType.TEST, 
        //        string.Format("Cell{0} DCIR: ((({1} * {2}) + ({3} * {4})) / ({5} + {6})) * 1000 = {7}",
        //        cnt, -endCurrent_DIS, point1, endCurrent_CHA, point2,
        //        Math.Pow(endCurrent_DIS, 2), Math.Pow(endCurrent_CHA, 2), dcircell));

        //    return dcircell;
        //}

        //public bool Cell_DCIR_Low(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    ti.Value_ = tempdcirMin = cellDcir.Min();

        //    int index = cellDcir.IndexOf(tempdcirMin);

        //    CellTempRevision(ti, index);

        //    return JudgementTestItem(ti);
        //}

        ///// <summary>
        ///// cell temperature revision method
        ///// 2018.10.31 jeonhj
        ///// </summary>
        ///// <param name="ti"></param>
        ///// <returns></returns>
        //private string CellTempRevision(TestItem ti, int index)
        //{
        //    string regSubkey = "Software\\EOL_Trigger\\Relays";
        //    rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

        //    string temperature = rk.GetValue("NHT_RS232_TEMP").ToString();
        //    double ret_value = 0.0;

        //    double temper = double.Parse(temperature);

        //    double temp_value = double.Parse(ti.Value_.ToString());

        //    string temp_ = "";

        //    if ((temper >= double.Parse(setting.LowerTemp1)) && (temper < double.Parse(setting.HigherTemp1)))
        //    {
        //        temp_ = setting.CellResistance1;
        //        if (temp_value <= double.Parse(setting.CellResistance1))
        //            ti.Value_ = 0;
        //        else
        //            ti.Value_ = -1;
        //    }
        //    else if ((temper >= double.Parse(setting.LowerTemp2)) && (temper < double.Parse(setting.HigherTemp2)))
        //    {
        //        temp_ = setting.CellResistance2;
        //        if (temp_value <= double.Parse(setting.CellResistance2))
        //            ret_value = 0;
        //        else
        //            ret_value = -1;
        //    }
        //    else if ((temper >= double.Parse(setting.LowerTemp3)) && (temper < double.Parse(setting.HigherTemp3)))
        //    {
        //        temp_ = setting.CellResistance3;
        //        if (temp_value <= double.Parse(setting.CellResistance3))
        //            ret_value = 0;
        //        else
        //            ret_value = -1;
        //    }
        //    else if ((temper >= double.Parse(setting.LowerTemp4)) && (temper < double.Parse(setting.HigherTemp4)))
        //    {
        //        temp_ = setting.CellResistance4;
        //        if (temp_value <= double.Parse(setting.CellResistance4))
        //            ret_value = 0;
        //        else
        //            ret_value = -1;
        //    }
        //    else if ((temper >= double.Parse(setting.LowerTemp5)) && (temper < double.Parse(setting.HigherTemp5)))
        //    {
        //        temp_ = setting.CellResistance5;
        //        if (temp_value <= double.Parse(setting.CellResistance5))
        //            ret_value = 0;
        //        else
        //            ret_value = -1;
        //    }
        //    else
        //    {
        //        ret_value = -1; // not corret temperature range
        //    }

        //    ti.Value_ = ret_value;

        //    LogState(LogType.Info, string.Format("Temperature: {0}, Cell{1} DCIR: {2} <= {3}",temperature, index, temp_value, temp_));

        //    return ret_value.ToString();
        //}

        //public bool Cell_DCIR_Ratio(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    ti.Value_ = (tempdcirMax / tempdcirMin) * 100;

        //    return JudgementTestItem(ti);
        //}

        //// 2018.11.01
        //// kih
        //// Measure Instrument chnage P_DAQ -> 34970A
        //// Delta Voltage Measure Instrument 34970A
        ////public bool DCIR_Delta_Voltage(TestItem ti)
        ////{
        ////    isProcessingUI(ti);

        ////    List<double> deltaList = new List<double>();
        ////    deltaList.Add(cellDetailList[1].CellVolt_1);
        ////    deltaList.Add(cellDetailList[1].CellVolt_2);
        ////    deltaList.Add(cellDetailList[1].CellVolt_3);
        ////    deltaList.Add(cellDetailList[1].CellVolt_4);
        ////    //
        ////    deltaList.Add(cellDetailList[1].CellVolt_6);
        ////    deltaList.Add(cellDetailList[1].CellVolt_7);
        ////    deltaList.Add(cellDetailList[1].CellVolt_8);
        ////    deltaList.Add(cellDetailList[1].CellVolt_9);
        ////    //deltaList.Add(cellDetailList[1].CellVolt_9);
        ////    //deltaList.Add(cellDetailList[1].CellVolt_10);
        ////    //deltaList.Add(cellDetailList[1].CellVolt_11);
        ////    //deltaList.Add(cellDetailList[1].CellVolt_12);
        ////    //deltaList.Add(cellDetailList[1].CellVolt_13);
        ////    //deltaList.Add(cellDetailList[1].CellVolt_14);
        ////    //deltaList.Add(cellDetailList[1].CellVolt_15);
        ////    //deltaList.Add(cellDetailList[1].CellVolt_16);

        ////    //ti.Value_ = deltaList.Max() - deltaList.Min();
        ////    double max = deltaList.Max();
        ////    double min = deltaList.Min();

        ////    int max_index = deltaList.IndexOf(max);
        ////    int min_index = deltaList.IndexOf(min);

        ////    ti.Value_ = max - min;

        ////    LogState(LogType.Info, string.Format("Delta DCIR Voltage Max Cell[{0}] - Min Cell[{1}] : {2}, {3}", max_index, min_index, max, min));

        ////    return JudgementTestItem(ti);
        ////}

        //public bool DCIR_Delta_Voltage(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    if (keysight == null)
        //    {
        //        ti.Value_ = "NotConnected";
        //        return JudgementTestItem(ti);
        //    }

        //    //sensing regist
        //    keysight.rtstring = "";
        //    cellVoltCnt = 8;//16 cell, 1module  
        //    var measString = "101,102,103,104,106,107,108,109";

        //    keysight.MeasTemp(measString);

        //    int rec = keysight.sp.BytesToRead;

        //    int cnt = 0;
        //    while (rec < 129)
        //    {
        //        Thread.Sleep(100);
        //        rec = keysight.sp.BytesToRead;
        //        cnt += 100;
        //        //not received data
        //        if (cnt == 5000)
        //        {
        //            keysight.MeasTemp(measString);

        //            rec = keysight.sp.BytesToRead;

        //            cnt = 0;
        //            while (rec < 145)
        //            {
        //                Thread.Sleep(100);
        //                rec = keysight.sp.BytesToRead;
        //                cnt += 100;
        //                if (cnt == 5000)
        //                {
        //                    ti.Value_ = _DEVICE_NOT_READY;
        //                    return JudgementTestItem(ti);
        //                }
        //            }
        //            break;
        //        }
        //    }
        //    //받은 후에 데이터 파싱
        //    byte[] bt = new byte[rec];
        //    keysight.sp.Read(bt, 0, rec);

        //    keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);


        //    //LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

        //    var vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

        //    var resList = new List<double>();
        //    string resetr = "";
        //    foreach (var item in vArr)
        //    {
        //        double dv = 0;
        //        if (double.TryParse(item, out dv))
        //        {
        //            resList.Add(dv);
        //            resetr += dv.ToString() + ",";
        //        }
        //    }
        //    LogState(LogType.RESPONSE, "KeysightDMM RES:" + resetr);

        //    keysight.rtstring = "";
        //    keysight.MeasVolt(measString);

        //    rec = keysight.sp.BytesToRead;

        //    cnt = 0;
        //    while (rec < 129)//33
        //    {
        //        Thread.Sleep(100);
        //        rec = keysight.sp.BytesToRead;
        //        cnt += 100;
        //        //not received data
        //        if (cnt == 5000)
        //        {
        //            keysight.MeasVolt(measString);

        //            rec = keysight.sp.BytesToRead;

        //            cnt = 0;
        //            while (rec < 129)//33
        //            {
        //                Thread.Sleep(100);
        //                rec = keysight.sp.BytesToRead;
        //                cnt += 100;
        //                if (cnt == 5000)
        //                {
        //                    ti.Value_ = _DEVICE_NOT_READY;
        //                    return JudgementTestItem(ti);
        //                }
        //            }
        //            break;
        //        }
        //    }
        //    //받은 후에 데이터 파싱
        //    bt = new byte[rec];
        //    keysight.sp.Read(bt, 0, rec);

        //    keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);

        //    //LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

        //    vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

        //    CellVoltageList = new List<double>();
        //    string voltstr = "";
        //    foreach (var item in vArr)
        //    {
        //        double dv = 0;
        //        if (double.TryParse(item, out dv))
        //        {
        //            CellVoltageList.Add(dv);
        //            voltstr += dv.ToString() + ",";
        //        }
        //    }
        //    LogState(LogType.RESPONSE, "KeysightDMM Volt:" + voltstr);

        //    for (int i = 0; i < resList.Count - 1; i++)
        //    {
        //        if (resList[i] > 1000000)
        //        {
        //            CellVoltageList[i] = 0;
        //        }
        //    }

        //    //ti.Value_ = CellVoltageList.Last();

        //    //CellVoltageList.Remove(CellVoltageList.Last());

        //    List<double> deltaList = new List<double>();

        //    deltaList.Add(double.Parse(CellVoltageList[0].ToString("N4")));
        //    deltaList.Add(double.Parse(CellVoltageList[1].ToString("N4")));
        //    deltaList.Add(double.Parse(CellVoltageList[2].ToString("N4")));
        //    deltaList.Add(double.Parse(CellVoltageList[3].ToString("N4")));
        //    deltaList.Add(double.Parse(CellVoltageList[4].ToString("N4")));
        //    deltaList.Add(double.Parse(CellVoltageList[5].ToString("N4")));
        //    deltaList.Add(double.Parse(CellVoltageList[6].ToString("N4")));
        //    deltaList.Add(double.Parse(CellVoltageList[7].ToString("N4")));

        //    double max = deltaList.Max();
        //    double min = deltaList.Min();

        //    int max_index = deltaList.IndexOf(max);
        //    int min_index = deltaList.IndexOf(min);

        //    ti.Value_ = (max - min) * 100;

        //    LogState(LogType.Info, string.Format("Delta DCIR Voltage Max Cell[{0}] - Min Cell[{1}] : {2}, {3}", max_index, min_index, max, min));

        //    SetDeltaCellVoltage(deltaList);

        //    return JudgementTestItem(ti);
        //}

        //private void SetDeltaCellVoltage(List<double> deltaList)
        //{
        //    ocvDetailList[1].CellVolt_1 = deltaList[0];
        //    ocvDetailList[1].CellVolt_2 = deltaList[1];
        //    ocvDetailList[1].CellVolt_3 = deltaList[2];
        //    ocvDetailList[1].CellVolt_4 = deltaList[3];
        //    ocvDetailList[1].CellVolt_5 = deltaList[4];
        //    ocvDetailList[1].CellVolt_6 = deltaList[5];
        //    ocvDetailList[1].CellVolt_7 = deltaList[6];
        //    ocvDetailList[1].CellVolt_8 = deltaList[7];
        //}

        //public bool Cell1_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_1;
        //    var point2 = cellDetailList[1].CellVolt_1;

        //    tempdcircell1 = (((-endCurrent_DIS * point1) + (endCurrent_CHA * point2)) / (Math.Pow(endCurrent_DIS, 2) + Math.Pow(endCurrent_CHA, 2))) * 1000;
        //    LogState(LogType.TEST, "DCIR Value:" +
        //        string.Format("((({0} * {1}) + ({2} * {3})) / ({4} + {5})) * 1000 = {6}", -endCurrent_DIS, point1,
        //        endCurrent_CHA, point2, Math.Pow(endCurrent_DIS, 2), Math.Pow(endCurrent_CHA, 2), tempdcircell1));

        //    ti.Value_ = tempdcircell1;

        //    //tempdcircell1 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST,"origin DCIR Value:"+ 
        //    //    string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //    tempdcircell1));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell1, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell1 = (tempdcircell1 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    //LogState(LogType.TEST, ti.Name+" - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell2_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_2;
        //    var point2 = cellDetailList[1].CellVolt_2;

        //    tempdcircell2 = (((-endCurrent_DIS * point1) + (endCurrent_CHA * point2)) / (Math.Pow(endCurrent_DIS, 2) + Math.Pow(endCurrent_CHA, 2))) * 1000;
        //    LogState(LogType.TEST, "DCIR Value:" +
        //        string.Format("((({0} * {1}) + ({2} * {3})) / ({4} + {5})) * 1000 = {6}", -endCurrent_DIS, point1,
        //        endCurrent_CHA, point2, Math.Pow(endCurrent_DIS, 2), Math.Pow(endCurrent_CHA, 2), tempdcircell2));

        //    ti.Value_ = tempdcircell2;

        //    //tempdcircell2 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell2));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell2, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell2 = (tempdcircell2 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell3_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_3;
        //    var point2 = cellDetailList[1].CellVolt_3;

        //    tempdcircell3 = (((-endCurrent_DIS * point1) + (endCurrent_CHA * point2)) / (Math.Pow(endCurrent_DIS, 2) + Math.Pow(endCurrent_CHA, 2))) * 1000;
        //    LogState(LogType.TEST, "DCIR Value:" +
        //        string.Format("((({0} * {1}) + ({2} * {3})) / ({4} + {5})) * 1000 = {6}", -endCurrent_DIS, point1,
        //        endCurrent_CHA, point2, Math.Pow(endCurrent_DIS, 2), Math.Pow(endCurrent_CHA, 2), tempdcircell3));

        //    ti.Value_ = tempdcircell3;

        //    //tempdcircell3 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000); 
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell3));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell3, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell3 = (tempdcircell3 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell4_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_4;
        //    var point2 = cellDetailList[1].CellVolt_4;

        //    tempdcircell4 = (((-endCurrent_DIS * point1) + (endCurrent_CHA * point2)) / (Math.Pow(endCurrent_DIS, 2) + Math.Pow(endCurrent_CHA, 2))) * 1000;
        //    LogState(LogType.TEST, "DCIR Value:" +
        //        string.Format("((({0} * {1}) + ({2} * {3})) / ({4} + {5})) * 1000 = {6}", -endCurrent_DIS, point1,
        //        endCurrent_CHA, point2, Math.Pow(endCurrent_DIS, 2), Math.Pow(endCurrent_CHA, 2), tempdcircell4));

        //    ti.Value_ = tempdcircell4;

        //    //tempdcircell4 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell4));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell4, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell4 = (tempdcircell4 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell5_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    //
        //    var point1 = cellDetailList[0].CellVolt_6;
        //    var point2 = cellDetailList[1].CellVolt_6;

        //    tempdcircell5 = (((-endCurrent_DIS * point1) + (endCurrent_CHA * point2)) / (Math.Pow(endCurrent_DIS, 2) + Math.Pow(endCurrent_CHA, 2))) * 1000;
        //    LogState(LogType.TEST, "DCIR Value:" +
        //        string.Format("((({0} * {1}) + ({2} * {3})) / ({4} + {5})) * 1000 = {6}", -endCurrent_DIS, point1,
        //        endCurrent_CHA, point2, Math.Pow(endCurrent_DIS, 2), Math.Pow(endCurrent_CHA, 2), tempdcircell5));

        //    ti.Value_ = tempdcircell5;

        //    //tempdcircell5 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell5));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell5, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell5 = (tempdcircell5 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell6_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_7;
        //    var point2 = cellDetailList[1].CellVolt_7;

        //    tempdcircell6 = (((-endCurrent_DIS * point1) + (endCurrent_CHA * point2)) / (Math.Pow(endCurrent_DIS, 2) + Math.Pow(endCurrent_CHA, 2))) * 1000;
        //    LogState(LogType.TEST, "DCIR Value:" +
        //        string.Format("((({0} * {1}) + ({2} * {3})) / ({4} + {5})) * 1000 = {6}", -endCurrent_DIS, point1,
        //        endCurrent_CHA, point2, Math.Pow(endCurrent_DIS, 2), Math.Pow(endCurrent_CHA, 2), tempdcircell6));

        //    ti.Value_ = tempdcircell6;

        //    //tempdcircell6 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell6));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell6, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell6 = (tempdcircell6 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell7_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_8;
        //    var point2 = cellDetailList[1].CellVolt_8;

        //    tempdcircell7 = (((-endCurrent_DIS * point1) + (endCurrent_CHA * point2)) / (Math.Pow(endCurrent_DIS, 2) + Math.Pow(endCurrent_CHA, 2))) * 1000;
        //    LogState(LogType.TEST, "DCIR Value:" +
        //        string.Format("((({0} * {1}) + ({2} * {3})) / ({4} + {5})) * 1000 = {6}", -endCurrent_DIS, point1,
        //        endCurrent_CHA, point2, Math.Pow(endCurrent_DIS, 2), Math.Pow(endCurrent_CHA, 2), tempdcircell7));

        //    ti.Value_ = tempdcircell7;

        //    //tempdcircell7 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell7));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell7, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell7 = (tempdcircell7 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell8_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_9;
        //    var point2 = cellDetailList[1].CellVolt_9;

        //    tempdcircell8 = (((-endCurrent_DIS * point1) + (endCurrent_CHA * point2)) / (Math.Pow(endCurrent_DIS, 2) + Math.Pow(endCurrent_CHA, 2))) * 1000;
        //    LogState(LogType.TEST, "DCIR Value:" +
        //        string.Format("((({0} * {1}) + ({2} * {3})) / ({4} + {5})) * 1000 = {6}", -endCurrent_DIS, point1,
        //        endCurrent_CHA, point2, Math.Pow(endCurrent_DIS, 2), Math.Pow(endCurrent_CHA, 2), tempdcircell8));

        //    ti.Value_ = tempdcircell8;

        //    //tempdcircell8 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell8));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell8, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell8 = (tempdcircell8 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    //LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell9_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_9;
        //    var point2 = cellDetailList[1].CellVolt_9;
        //    //tempdcircell9 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell9));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell9, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell9 = (tempdcircell9 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell10_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_10;
        //    var point2 = cellDetailList[1].CellVolt_10;
        //    //tempdcircell10 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell10));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell10, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell10 = (tempdcircell10 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell11_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_11;
        //    var point2 = cellDetailList[1].CellVolt_11;
        //    //tempdcircell11 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell11));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell11, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell11 = (tempdcircell11 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell12_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_12;
        //    var point2 = cellDetailList[1].CellVolt_12;
        //    //tempdcircell12 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell12));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell12, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell12 = (tempdcircell12 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell13_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_13;
        //    var point2 = cellDetailList[1].CellVolt_13;
        //    //tempdcircell13 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell13));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell13, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell13 = (tempdcircell13 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell14_DCIR(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var point1 = cellDetailList[0].CellVolt_14;
        //    var point2 = cellDetailList[1].CellVolt_14;
        //    //tempdcircell14 = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
        //    //LogState(LogType.TEST, "origin DCIR Value:" +
        //    //     string.Format("({0} - {1}) / {2} * 1000) = {3}", point2, point1, endCurrent_DIS,
        //    //     tempdcircell14));
        //    //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcircell14, cellFomula1, calAvg, cellFomula2));
        //    //ti.Value_ = tempdcircell14 = (tempdcircell14 + (cellFomula1 * (calAvg - 23))) + cellFomula2;
        //    LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

        //    return JudgementTestItem(ti);
        //}

        //public bool Cell_DCIR_Ratio__(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var dd = new List<double>();
        //    dd.Add(tempdcircell1);
        //    dd.Add(tempdcircell2);
        //    dd.Add(tempdcircell3);
        //    dd.Add(tempdcircell4);
        //    dd.Add(tempdcircell5);
        //    dd.Add(tempdcircell6);
        //    dd.Add(tempdcircell7);
        //    dd.Add(tempdcircell8);
        //    //dd.Add(tempdcircell9);
        //    //dd.Add(tempdcircell10);
        //    //dd.Add(tempdcircell11);
        //    //dd.Add(tempdcircell12);

        //    if (localTypes == 2)
        //    {
        //        //dd.Add(tempdcircell13);
        //        //dd.Add(tempdcircell14);
        //    }
            
        //    var max = dd.Max();
        //    var min = dd.Min();

        //    ti.Value_ = (max / min) * 100;

        //    ti.refValues_.Clear();
        //    ti.refValues_.Add(MakeDetailItem("TEST0010", max.ToString()));
        //    ti.refValues_.Add(MakeDetailItem("TEST0011", min.ToString()));

        //    LogState(LogType.Info,
        //        "DCIR Ratio - Max Cell DCIR :" + max.ToString() +
        //        "/Min Cell DCIR :" + min.ToString() +
        //        "/Result :" + ti.Value_.ToString());
        //    return JudgementTestItem(ti);
        //}

        //public bool Cell_DCIR_Dev(TestItem ti)
        //{
        //    isProcessingUI(ti);

        //    var dd = new List<double>();
        //    dd.Add(tempdcircell1);
        //    dd.Add(tempdcircell2);
        //    dd.Add(tempdcircell3);
        //    dd.Add(tempdcircell4);
        //    dd.Add(tempdcircell5);
        //    dd.Add(tempdcircell6);
        //    dd.Add(tempdcircell7);
        //    dd.Add(tempdcircell8);
        //    //dd.Add(tempdcircell9);
        //    //dd.Add(tempdcircell10);
        //    //dd.Add(tempdcircell11);
        //    //dd.Add(tempdcircell12);

        //    if (localTypes == 2)
        //    {
        //        //dd.Add(tempdcircell13);
        //        //dd.Add(tempdcircell14);
        //    }
        //    var max = dd.Max();
        //    var min = dd.Min();

        //    ti.Value_ = max - min;

        //    return JudgementTestItem(ti);

        //}
    }
}