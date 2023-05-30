using Renault_BT6.모듈;
using Renault_BT6.윈도우;
using Renault_BT6.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Renault_BT6
{

    public partial class MainWindow
    {

        #region Fields
        double tempVerifyCurrSensorERAD_dis, tempVerifyCurrSensorERAD_cha,
              tempVerifyCurrSensorCIDD_dis, tempVerifyCurrSensorCIDD_cha;
        double currentCheck1 = 0.0;
        double currentCheck2 = 0.0;
        double tempdcirmodule_DIS = 0.0;
        double tempdcirmodule_CHA = 0.0;
        double tempdcircell1_DIS = 0.0;
        double tempdcircell2_DIS = 0.0;
        double tempdcircell3_DIS = 0.0;
        double tempdcircell4_DIS = 0.0;
        double tempdcircell5_DIS = 0.0;
        double tempdcircell6_DIS = 0.0;
        double tempdcircell7_DIS = 0.0;
        double tempdcircell8_DIS = 0.0;
        double tempdcircell9_DIS = 0.0;
        double tempdcircell10_DIS = 0.0;
        double tempdcircell11_DIS = 0.0;
        double tempdcircell12_DIS = 0.0;
        double tempdcircell13_DIS = 0.0;
        double tempdcircell14_DIS = 0.0;
        double tempdcircell15_DIS = 0.0;
        double tempdcircell16_DIS = 0.0;
        double tempdcircell17_DIS = 0.0;
        double tempdcircell18_DIS = 0.0;
        double tempdcircell19_DIS = 0.0;
        double tempdcircell20_DIS = 0.0;

        double tempdcircell1_CHA { get; set; }
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
        double tempdcircell17_CHA = 0.0;
        double tempdcircell18_CHA = 0.0;
        double tempdcircell19_CHA = 0.0;
        double tempdcircell20_CHA = 0.0;

        double modulePoint_RES_DIS = 0.0;
        double modulePoint_DIS = 0.0;
        double modulePoint_RES_CHA = 0.0;
        double modulePoint_CHA = 0.0;

        double endCurrent_DIS { get; set; }
        double endCurrent_CHA = 0.0;
        
        #endregion

        private DetailItems SetCellVoltagesNloggingToIndex(int p1, string dtiKey, string loggingTitle, string moduleVolt)
        {

            var dti = new DetailItems() { Key = dtiKey };

            try
            {
                SetCellDetailList(p1);

                //var logstr = loggingTitle + moduleVolt;
                var logstr = string.Format("{0}{1}{2}", dtiKey, loggingTitle, moduleVolt);

                int cnt = 1;

                for (int n =0; n < BatteryInfo.CellCount; n++)
                {
                    dti.Reportitems.Add(Device.Cycler.GetDisChgPointList[p1].CellVoltageList[n]);

                    logstr += string.Format(",CELL{0},{1}",cnt, Device.Cycler.GetDisChgPointList[p1].CellVoltageList[n]);
                    cnt++;
                }
                LogState(LogType.Info, logstr);

            }
            catch
            {

            }

            return dti;
        }

        void InitDCIRVariables()
        {
            tempVerifyCurrSensorERAD_dis = tempVerifyCurrSensorERAD_cha =
            tempVerifyCurrSensorCIDD_dis = tempVerifyCurrSensorCIDD_cha = 
            currentCheck1 = currentCheck2 = 0.0;
            tempdcirmodule_CHA = tempdcirmodule_DIS = 0.0;
            tempdcircell1_CHA = tempdcircell1_DIS = 0.0;
            tempdcircell2_CHA = tempdcircell2_DIS = 0.0;
            tempdcircell3_CHA = tempdcircell3_DIS = 0.0;
            tempdcircell4_CHA = tempdcircell4_DIS = 0.0;
            tempdcircell5_CHA = tempdcircell5_DIS = 0.0;
            tempdcircell6_CHA = tempdcircell6_DIS = 0.0;
            tempdcircell7_CHA = tempdcircell7_DIS = 0.0;
            tempdcircell8_CHA = tempdcircell8_DIS = 0.0;
            tempdcircell9_CHA = tempdcircell9_DIS = 0.0;
            tempdcircell10_CHA = tempdcircell10_DIS = 0.0;
            tempdcircell11_CHA = tempdcircell11_DIS = 0.0;
            tempdcircell12_CHA = tempdcircell12_DIS = 0.0;
            tempdcircell13_CHA = tempdcircell13_DIS = 0.0;
            tempdcircell14_CHA = tempdcircell14_DIS = 0.0;
            tempdcircell15_CHA = tempdcircell15_DIS = 0.0;
            tempdcircell16_CHA = tempdcircell16_DIS = 0.0;
            tempdcircell17_CHA = tempdcircell17_DIS = 0.0;
            tempdcircell18_CHA = tempdcircell18_DIS = 0.0;
            tempdcircell19_CHA = tempdcircell19_DIS = 0.0;
            tempdcircell20_CHA = tempdcircell20_DIS = 0.0;

            modulePoint_RES_CHA = modulePoint_RES_DIS = 0.0;
            modulePoint_CHA = modulePoint_DIS = 0.0;
            endCurrent_CHA = endCurrent_DIS = 0.0;
        }

        double GetVerifyCurrentSensor(bool isCharge)
        {
            var rList = new List<string>();
            if (GetToDID_singleData(0x48, 0x02, "05 62", false, out rList))
            {
                LogState(LogType.Info, "Current Sensor:" + rList[3] + rList[4]);

                var data = rList[3] + rList[4];// +"000000000000";// "056248020FA20000";    

               var arr =  Enumerable.Range(0, data.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(data.Substring(x, 2), 16))
                             .ToArray();

               if (isCharge || arr[0] > 0x7f)
               {

                   var toint = BitConverter.ToInt16(new byte[] { arr[1], arr[0] }, 0);

                   var doub = double.Parse(toint.ToString());

                   LogState(LogType.Info, "Current Sensor(parsed):" + doub + " / 50 = " + (doub / 50.0).ToString());

                   return doub / 50.0;
               }
               else
               {

                   var toint = Convert.ToInt32(rList[3] + rList[4], 16); // BitConverter.ToInt16(new byte[] { arr[0], arr[1] }, 0);

                   var doub = double.Parse(toint.ToString());

                   LogState(LogType.Info, "Current Sensor(parsed):" + doub + " / 50 = " + (doub / 50.0).ToString());

                   return doub / 50.0;
               }
            }
            else
            {
                return 0.0;
            }
        }

        List<double> tempVerifyCurrSensorCIDD_cha_List = new List<double>();
        List<double> tempVerifyCurrSensorERAD_dis_List = new List<double>();

        Thread dp_disc, dp_char, dp_rest_bef_disc, dp_rest_aft_disc, dp_rest_aft_char, dp_rest_aft_char_10s, dp_rest_aft_char_20s, dp_rest_aft_char_30s;
        Thread dp_rest_aft_char_5s;
        //nnkim
        Thread dp_McShortCheck, Protection_McResistanceStart, Protection_PartsCountStart;

        public bool DCIR_ERAD(TestItem ti)
        {
            isProcessingUI(ti);

            #region Check Relay Status
            ClearDTC();
            Thread.Sleep(500);
            Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
            LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");

            Thread.Sleep(1000);

            var arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");

            int retryCnt = 10;
            while (arr[0].Substring(0, 1) != "4")//0x80 == 128
            {
                ClearDTC();
                Thread.Sleep(500);
                Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
                LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");
                arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");
                retryCnt--;

                if (retryCnt == 0)
                {
                    ti.Value_ = _VALUE_NOT_MATCHED;

                    Hybrid_Instru_CAN._635List.Clear();
                    Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });
                    LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");
                    Thread.Sleep(50);
                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Success, "Recved data - " + fe009c);
                    }
                    return JudgementTestItem(ti);
                }
            }


            #endregion

            cycler.SendToDSP1("0AA", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            LogState(LogType.Info, "Cycler Mode Set [AA]");
            Thread.Sleep(3000);
            if (cycler.cycler1voltage < 50)
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }
            InitDCIRVariables();

            #region DCIR
            if (!cycler.isAlive1)
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }
            SetToSteplist(this.totalProcessList[0].ScheduleList[0].CycleList[0].StepList);
            cycler.is84On = false;

            ti.refValues_.Clear();

            if (counter_Cycler_limit < counter_Cycler)
            {
                ti.Value_ = "LIMIT OF CYCLER COUNT";
                return JudgementTestItem(ti);
            }

            counter_Cycler++;

            int localpos = 0;

            bool isLastRest = false;
            lock (send)
            {
                foreach (var sitem in sendList)
                {
                    if (IsEmgStop || Ispause)
                    {
                        DCIR_Stopped();
                        ti.Value_ = _EMG_STOPPED;
                        return JudgementTestItem(ti);
                    }

                    int milisecond = 0;

                    for (int i = 0; i < sitem.Duration * 10; i++)
                    {
                        #region Sending Sequence
                        if (IsEmgStop || Ispause)
                        {
                            DCIR_Stopped();
                            ti.Value_ = _EMG_STOPPED;
                            return JudgementTestItem(ti);
                        }

                        if (!CheckSafety(0, cycler.cycler1OP, sitem))
                        {
                            DCIR_Stopped();
                            ti.Value_ = _CYCLER_SAFETY;
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                MessageBox.Show("SAFETY_STOPPED", "Cycler Suspended", MessageBoxButton.OK, MessageBoxImage.Information);
                            }));
                            return JudgementTestItem(ti);
                        }


                        cycler.cycler1Header = CMD.NULL;
                        cycler.cycler1OP = RECV_OPMode.NULL;

                        if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
                        {
                            LogState(LogType.Fail, "cycler #1_Send fail");

                            DCIR_Stopped();
                            ti.Value_ = _COMM_FAIL;
                            return JudgementTestItem(ti);
                        }

                        //만약 3초간 안보내주면 다시보내달라고 하고
                        //3초기다렸다가 안보내주면 fail
                        var oldTime = DateTime.Now;
                        var ltime = oldTime;

                        var after5s = oldTime.AddSeconds(30);

                        Thread.Sleep(100);


                        while (true)
                        {
                            if ((cycler.cycler1Header == sitem.Cmd) && (cycler.cycler1OP == sitem.NextOPMode))
                            {
                                var sb = new StringBuilder();
                                sb.Append("cycler1(Front) :");
                                sb.Append(cycler.cycler1Header.ToString());
                                sb.Append(",Voltage,");
                                sb.Append(cycler.cycler1voltage.ToString());
                                sb.Append(",Current,");
                                sb.Append(cycler.cycler1current.ToString());


                                LogState(LogType.Info, sb.ToString(), null, false);
                                break;
                            }

                            if (IsEmgStop || Ispause)
                            {
                                DCIR_Stopped();
                                ti.Value_ = _EMG_STOPPED;
                                return JudgementTestItem(ti);
                            }

                            ltime = DateTime.Now;

                            #region send fail
                            if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
                            {
                                LogState(LogType.Fail, "cycler #1_Send fail");

                                DCIR_Stopped();
                                ti.Value_ = _COMM_FAIL;
                                return JudgementTestItem(ti);
                            }

                            #endregion
                            Thread.Sleep(100);

                            if (ltime.Ticks >= after5s.Ticks)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    MessageBox.Show("Disconnected DSP", "Critical");
                                }));

                                LogState(LogType.Fail, "Disconnected DSP");

                                DCIR_Stopped();
                                ti.Value_ = _COMM_FAIL;

                                return JudgementTestItem(ti);
                            }
                        }
                        #endregion
                        milisecond += 100;

                        //초기 휴지 9.5
                        if (sitem.Cmd == CMD.REST && ((sitem.Duration - 0.2) * 1000) == milisecond && modulePoint_RES_DIS == 0.0)//휴식 10초중 9초일때 측정
                        {
                            dp_rest_bef_disc = new Thread(new ParameterizedThreadStart(Point_REST_BEF_DISC));
                            dp_rest_bef_disc.Start(ti);
                        }

                        //방전 9.5
                        if (sitem.Cmd == CMD.DISCHARGE && ((sitem.Duration - 1) * 1000) == milisecond && localpos == 0)//9.0 sec
                        {
                            dp_disc = new Thread(new ParameterizedThreadStart(Point_DISC));
                            dp_disc.Start(ti);
                            localpos = 1;
                        }


                        //중간 휴지 9.5
                        if (sitem.Cmd == CMD.REST && ((sitem.Duration - 1) * 1000) == milisecond && modulePoint_RES_DIS != 0.0 && modulePoint_DIS != 0.0 && !isLastRest)//휴식 10초중 9초일때 측정
                        {
                            isLastRest = true;
                            dp_rest_aft_disc = new Thread(new ParameterizedThreadStart(Point_REST_AFT_DISC));
                            dp_rest_aft_disc.Start(ti); 
                        }

                        ////충전 9.5
                        //if (sitem.Cmd == CMD.CHARGE && ((sitem.Duration - 1) * 1000) == milisecond)
                        //{
                        //    isLastRest = true;
                        //    dp_char = new Thread(new ParameterizedThreadStart(Point_CHAR));
                        //    dp_char.Start(ti);
                        //}

                        ////끝나기전
                        //if (sitem.Cmd == CMD.REST && ((sitem.Duration - 1) * 1000) == milisecond && isLastRest)//휴식 10초중 9초일때 측정
                        //{
                        //    dp_rest_aft_char = new Thread(new ParameterizedThreadStart(Point_REST_AFT_CHAR));
                        //    dp_rest_aft_char.Start(ti);

                        //}
                    }
                }
            }
            Thread.Sleep(1000);
            DCIR_Stopped();
            SetMainVoltage("0.00");
            SetMainCurrent("0.00");
            SetMainCState("Ready");
            Thread.Sleep(500);

            #endregion


            SetCounter_Cycler();

            ti.Value_ = tempdcirmodule_DIS = Math.Abs(((modulePoint_DIS - modulePoint_RES_DIS) / (endCurrent_DIS)) * 1000);//밀리옴
            LogState(LogType.TEST, string.Format("({0} - {1}) / {2} * 1000)", modulePoint_DIS, modulePoint_RES_DIS, endCurrent_DIS));

            //LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23) + {3})", tempdcirmodule_DIS, moduleFomula1, calAvg, moduleFomula2));
            //ti.Value_ = tempdcirmodule_DIS = tempdcirmodule_DIS + (moduleFomula1 * (calAvg - 23) + moduleFomula2);
            //LogState(LogType.TEST, "DCIR - Point2:" + modulePoint_DIS.ToString() + ", Point1:" + modulePoint_RES_DIS.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());



            //SetCounter_Cycler();
            return JudgementTestItem(ti);
        }

        public bool DCIR_CIDD(TestItem ti)
        {
            isProcessingUI(ti);

            //continue test, just check voltage
            #region Check Relay Status
            ClearDTC();
            Thread.Sleep(500);
            Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
            LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");

            Thread.Sleep(1000);

            var arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");

            int retryCnt = 10;
            while (arr[0].Substring(0, 1) != "4")//0x80 == 128
            {
                ClearDTC();
                Thread.Sleep(500);
                Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
                LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");
                arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");
                retryCnt--;

                if (retryCnt == 0)
                {
                    ti.Value_ = _VALUE_NOT_MATCHED;

                    Hybrid_Instru_CAN._635List.Clear();
                    Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });
                    LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");
                    Thread.Sleep(50);
                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Success, "Recved data - " + fe009c);
                    }
                    return JudgementTestItem(ti);
                }
            }


            #endregion

            cycler.SendToDSP1("0AA", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            LogState(LogType.Info, "Cycler Mode Set [AA]");
            Thread.Sleep(3000);
            if (cycler.cycler1voltage < 50)
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }
            //InitDCIRVariables();

            #region DCIR
            if (!cycler.isAlive1)
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }
            SetToSteplist(this.totalProcessList[0].ScheduleList[1].CycleList[0].StepList);
            cycler.is84On = false;

            ti.refValues_.Clear();

            if (counter_Cycler_limit < counter_Cycler)
            {
                ti.Value_ = "LIMIT OF CYCLER COUNT";
                return JudgementTestItem(ti);
            }

            counter_Cycler++;


            int localpos = 0;

            bool isLastRest = false;
            lock (send)
            {
                foreach (var sitem in sendList)
                {
                    if (IsEmgStop || Ispause)
                    {
                        DCIR_Stopped();
                        ti.Value_ = _EMG_STOPPED;
                        return JudgementTestItem(ti);
                    }

                    int milisecond = 0;

                    for (int i = 0; i < sitem.Duration * 10; i++)
                    {
                        #region Sending Sequence
                        if (IsEmgStop || Ispause)
                        {
                            DCIR_Stopped();
                            ti.Value_ = _EMG_STOPPED;
                            return JudgementTestItem(ti);
                        }

                        if (!CheckSafety(0, cycler.cycler1OP, sitem))
                        {
                            DCIR_Stopped();
                            ti.Value_ = _CYCLER_SAFETY;
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                MessageBox.Show("SAFETY_STOPPED", "Cycler Suspended", MessageBoxButton.OK, MessageBoxImage.Information);
                            }));
                            return JudgementTestItem(ti);
                        }


                        cycler.cycler1Header = CMD.NULL;
                        cycler.cycler1OP = RECV_OPMode.NULL;

                        if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
                        {
                            LogState(LogType.Fail, "cycler #1_Send fail");

                            DCIR_Stopped();
                            ti.Value_ = _COMM_FAIL;
                            return JudgementTestItem(ti);
                        }

                        //만약 3초간 안보내주면 다시보내달라고 하고
                        //3초기다렸다가 안보내주면 fail
                        var oldTime = DateTime.Now;
                        var ltime = oldTime;

                        var after5s = oldTime.AddSeconds(30);

                        Thread.Sleep(100);


                        while (true)
                        {
                            if ((cycler.cycler1Header == sitem.Cmd) && (cycler.cycler1OP == sitem.NextOPMode))
                            {
                                var sb = new StringBuilder();
                                sb.Append("cycler1(Front) :");
                                sb.Append(cycler.cycler1Header.ToString());
                                sb.Append(",Voltage,");
                                sb.Append(cycler.cycler1voltage.ToString());
                                sb.Append(",Current,");
                                sb.Append(cycler.cycler1current.ToString());


                                LogState(LogType.Info, sb.ToString(), null, false);
                                break;
                            }

                            if (IsEmgStop || Ispause)
                            {
                                DCIR_Stopped();
                                ti.Value_ = _EMG_STOPPED;
                                return JudgementTestItem(ti);
                            }

                            ltime = DateTime.Now;

                            #region send fail
                            if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
                            {
                                LogState(LogType.Fail, "cycler #1_Send fail");

                                DCIR_Stopped();
                                ti.Value_ = _COMM_FAIL;
                                return JudgementTestItem(ti);
                            }

                            #endregion
                            Thread.Sleep(100);

                            if (ltime.Ticks >= after5s.Ticks)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    MessageBox.Show("Disconnected DSP", "Critical");
                                }));

                                LogState(LogType.Fail, "Disconnected DSP");

                                DCIR_Stopped();
                                ti.Value_ = _COMM_FAIL;

                                return JudgementTestItem(ti);
                            }
                        }
                        #endregion
                        milisecond += 100;

                        ////초기 휴지 9.5
                        //if (sitem.Cmd == CMD.REST && ((sitem.Duration - 0.5) * 1000) == milisecond && modulePoint_RES_DIS == 0.0)//휴식 10초중 9초일때 측정
                        //{
                        //    dp_rest_bef_disc = new Thread(new ParameterizedThreadStart(Point_REST_BEF_DISC));
                        //    dp_rest_bef_disc.Start(ti);
                        //}

                        ////방전 9.5
                        //if (sitem.Cmd == CMD.DISCHARGE && ((sitem.Duration - 0.5) * 1000) == milisecond && localpos == 0)//9.0 sec
                        //{
                        //    dp_disc = new Thread(new ParameterizedThreadStart(Point_DISC));
                        //    dp_disc.Start(ti);
                        //    localpos = 1;
                        //}


                        //중간 휴지 9.5
                        if (sitem.Cmd == CMD.REST && ((sitem.Duration - 1) * 1000) == milisecond && !isLastRest)//휴식 10초중 9초일때 측정
                        {
                            dp_rest_aft_disc = new Thread(new ParameterizedThreadStart(Point_REST_AFT_DISC));
                            dp_rest_aft_disc.Start(ti);
                        }

                        //충전 9.5
                        if (sitem.Cmd == CMD.CHARGE && ((sitem.Duration - 1) * 1000) == milisecond)
                        {
                            isLastRest = true;
                            dp_char = new Thread(new ParameterizedThreadStart(Point_CHAR));
                            dp_char.Start(ti);
                        }

                        //끝나기전
                        if (sitem.Cmd == CMD.REST && ((sitem.Duration - 1) * 1000) == milisecond && isLastRest)//휴식 10초중 9초일때 측정
                        {
                            //dp_rest_aft_char = new Thread(new ParameterizedThreadStart(Point_REST_AFT_CHAR));
                            //dp_rest_aft_char.Start(ti);

                        }
                    }
                }
            }
            Thread.Sleep(1000);
            DCIR_Stopped();
            SetMainVoltage("0.00");
            SetMainCurrent("0.00");
            SetMainCState("Ready");
            Thread.Sleep(500);

            #endregion


            SetCounter_Cycler();

            ti.Value_ = tempdcirmodule_CHA = Math.Abs(((modulePoint_CHA - modulePoint_RES_CHA) / (endCurrent_CHA)) * 1000);//밀리옴
            LogState(LogType.TEST, string.Format("({0} - {1}) / {2} * 1000)", modulePoint_CHA, modulePoint_RES_CHA, endCurrent_CHA));


            return JudgementTestItem(ti);
        }
    }
}