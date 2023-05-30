using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        /// <summary>
        /// 실제로 100ms마다 동작될 리스트.
        /// </summary>
        public List<SendCMD> sendList = new List<SendCMD>();
        public List<string> rtMcList = new List<string>();

        object send = new object();

        #region Common Cycler methods

        public bool CheckEndStep(SendCMD sitem)
        {
            //todo
            //추가 종료조건은 여기서 설정해야 한다.                            
            //Endcase_Current = end_current,
            //Endcase_VoltHigh = end_voltHigh,
            //Endcase_VoltLow = end_voltLow,

            if (cycler.cycler1OP == RECV_OPMode.READY)
                return true;

            //CV/CP일때 전류컷오프 하는 부분
            if (cycler.cycler1OP == RECV_OPMode.DISCHARGE_CV 
                || cycler.cycler1OP == RECV_OPMode.CHARGE_CV 
                || cycler.cycler1OP == RECV_OPMode.DISCHARGE_CP
                || cycler.cycler1OP == RECV_OPMode.CHARGE_CP)
            {
                if (sitem.Endcase_Current != 0.0 && cycler.cycler1current != 0.0 &&
                    sitem.Endcase_Current >= cycler.cycler1current)
                {
                    return false;
                }
            }

            if (sitem.Endcase_VoltHigh != 0.0 && sitem.Endcase_VoltHigh <= cycler.cycler1voltage)
            {
                return false;
            }

            if (sitem.Endcase_VoltLow != 0.0 && sitem.Endcase_VoltLow >= cycler.cycler1voltage)
            {
                return false;
            }

            return true;
        }

        #region 필수조건 확인 부분
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

                    //200212 ht
                    if (sitem.CurrHighLimit != 0 && Math.Abs(cycler.cycler1current) > sitem.CurrHighLimit)
                    {
                        LogState(LogType.Fail, "[SAFETY] Cycler 1 - Current High : " + cycler.cycler1current.ToString());
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

        private bool Check_Exist_Time(MiniScheduler.Step step)
        {
            string time = "0.000";
            try
            {
                time = (step.ClearcaseList.FirstOrDefault(x => x.TitleValue == "Time") as MiniScheduler.ClearCase).Value;
            }
            catch (Exception)
            {
            }

            if (time == "0.000" || time == "00:00:00:00")
            {
                return false;
            }

            return true;
        }

        private bool Check_Exist_CutOff_Current(MiniScheduler.Step step)
        {
            double end_current = 0.0;

            var rst = double.TryParse((step.ClearcaseList.FirstOrDefault(x => x.TitleValue == "Current") as MiniScheduler.ClearCase).Value, out end_current);
            if(!rst || end_current == 0.0)
            {
                return false;
            }
            return true;
        }

        private bool Check_Exist_Current(MiniScheduler.Step step)
        {
            var current = 0.0;
            var rst = double.TryParse(step.Current, out current);
            if (!rst || current == 0.0)
            {
                return false;
            }
            return true;
        }

        private bool Check_Exist_Watt(MiniScheduler.Step step)
        {
            var watt = 0.0;
            var rst = double.TryParse(step.Watt, out watt);
            if (!rst || watt == 0.0)
            {
                return false;
            }
            return true;
        }

        private bool Check_Exist_ChaVoltage(MiniScheduler.Step step)
        {
            var voltage = 0.0;
            var rst = double.TryParse(step.Voltage, out voltage);
            if (!rst || voltage == 0.0)
            {
                return false;
            }
            return true;
        }

        private bool Check_Exist_DisVoltage(MiniScheduler.Step step)
        {
            var voltage = 0.0;
            var rst = double.TryParse(step.Discharge_voltage, out voltage);
            if (!rst || voltage == 0.0)
            {
                return false;
            }
            return true;
        }

        private bool CheckStepEndCondition(MiniScheduler.Step step,out StepError se)
        {
            se = StepError.NO_ERROR;
            switch (step.Step_type)
            {
                case "Charge":
                   {
                        if(!Check_Exist_ChaVoltage(step))
                        {
                            se = StepError.TARGET_VOLT_ERROR;
                            return false;
                        }

                        if (step.Step_mode == "CC")
                        {
                            //시간조건
                            //전압조건(설정 안해도 장비스펙으로 해야함, recomanded)
                            //전류조건
                            if (!Check_Exist_Time(step))
                            {
                                se = StepError.TIME_ERROR;
                                return false;
                            }

                            if (!Check_Exist_Current(step))
                            {
                                se = StepError.CURRENT_ERROR;
                                return false;
                            }
                        }
                        else if (step.Step_mode == "CP")
                        {
                            //와뜨조건
                            //todo
                            if(!Check_Exist_Watt(step))
                            {
                                se = StepError.WATT_ERROR;
                                return false;
                            }
                        }
                        else
                        {
                            //아마 CC/CV
                            //컷오프만 필수
                            if(!Check_Exist_Current(step))
                            {
                                se = StepError.CURRENT_ERROR;
                                return false;
                            }

                            if (!Check_Exist_CutOff_Current(step))
                            {
                                se = StepError.CUT_OFF_COND_ERROR;
                                return false;
                            }
                        }
                        return true;
                    }
                    break;
                case "Discharge":
                    {
                        if (!Check_Exist_DisVoltage(step))
                        {
                            se = StepError.TARGET_VOLT_ERROR;
                            return false;
                        }

                        if (step.Step_mode == "CC")
                        {
                            //시간조건
                            //전압조건(설정 안해도 장비스펙으로 해야함, recomanded)
                            //전류조건
                            if (!Check_Exist_Time(step))
                            {
                                se = StepError.TIME_ERROR;
                                return false;
                            }

                            if (!Check_Exist_Current(step))
                            {
                                se = StepError.CURRENT_ERROR;
                                return false;
                            }
                        }
                        else if (step.Step_mode == "CP")
                        {
                            //와뜨조건
                            //todo
                            if (!Check_Exist_Watt(step))
                            {
                                se = StepError.WATT_ERROR;
                                return false;
                            }
                        }
                        else
                        {
                            //아마 CC/CV
                            //컷오프만 필수
                            if (!Check_Exist_Current(step))
                            {
                                se = StepError.CURRENT_ERROR;
                                return false;
                            }

                            if (!Check_Exist_CutOff_Current(step))
                            {
                                se = StepError.CUT_OFF_COND_ERROR;
                                return false;
                            }
                        }
                        return true;
                    }
                    break;
            }
            return true;
        }
        #endregion

        private void LoggingCyclerData(bool isNormal)
        {
            var sb = new StringBuilder();

            if(isNormal)
                sb.Append("Cycler(Normal),");
            else
                sb.Append("Cycler(Loop),");


            sb.Append(cycler.cycler1Header.ToString());
            sb.Append(",Voltage,");
            sb.Append(cycler.cycler1voltage.ToString());
            sb.Append(",Current,");
            sb.Append(cycler.cycler1current.ToString());

            LogState(LogType.Info, sb.ToString(), null, false);
        }

        private SendCMD MakeOneStepToSendCMD(MiniScheduler.Step step)
        {
            double end_totalTime = 0.0;
            double end_voltHigh = 0.0;
            double end_voltLow = 0.0;
            double end_current = 0.0;

            #region END_CASE - TIME
            string time = "0.000";
            try
            {
                time = (step.ClearcaseList.FirstOrDefault(x => x.TitleValue == "Time") as MiniScheduler.ClearCase).Value;
            }
            catch (Exception)
            {
            }

            if (time == "0.000" || time == "00:00:00:00")
            {
                end_totalTime = 0.1;
            }
            else
            {
                var tarr = time.Split(':');//시분초
                end_totalTime = (int.Parse(tarr[0]) * 86400) + (int.Parse(tarr[1]) * 3600) + (int.Parse(tarr[2]) * 60) + (int.Parse(tarr[3]));
            }
            #endregion

            #region END_CASE - VOLTAGE HIGH
            double.TryParse((step.ClearcaseList.FirstOrDefault(x => x.TitleValue == "Voltage HIGH") as MiniScheduler.ClearCase).Value, out end_voltHigh);
            #endregion

            #region END_CASE - VOLTAGE LOW
            double.TryParse((step.ClearcaseList.FirstOrDefault(x => x.TitleValue == "Voltage LOW") as MiniScheduler.ClearCase).Value, out end_voltLow);
            #endregion

            #region END_CASE - CURRENT
            double.TryParse((step.ClearcaseList.FirstOrDefault(x => x.TitleValue == "Current") as MiniScheduler.ClearCase).Value, out end_current);
            #endregion

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
                    }
                    break;
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
                        else if(step.Step_mode == "CP")
                        {
                            opm = OPMode.CHARGE_CP;
                            rop = RECV_OPMode.CHARGE_CP;
                        }
                        else
                        {
                            opm = OPMode.CHARGE_CC;
                            rop = RECV_OPMode.CHARGE_CC;
                        }
                    }
                    break;
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
                        else if (step.Step_mode == "CP")
                        {
                            opm = OPMode.DISCHARGE_CP;
                            rop = RECV_OPMode.DISCHARGE_CP;
                        }
                        else
                        {
                            opm = OPMode.DISCHARGE_CC;
                            rop = RECV_OPMode.DISCHARGE_CC;
                        }
                    }
                    break;
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

            var watt = 0.0;
            double.TryParse(step.Watt, out watt);

            return new SendCMD()
            {
                Endcase_Current = end_current,
                Endcase_VoltHigh = end_voltHigh,
                Endcase_VoltLow = end_voltLow,
                Duration = end_totalTime,
                Index = 0,
                Cmd = cmd,
                Pmode = local_Branchmode,
                Opmode = opm,
                Voltage = voltage,
                Current = current,
                Watt = watt,
                NextOPMode = rop,
                VoltLowLimit = voltLowLimit,
                VoltHighLimit = voltHighLimit,
                CurrHighLimit = currHighLimit,
            };

        }

        private SendCMD MakeSendCMD_INPUT_MC_ON()
        {
            return new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.INPUT_MC_ON,
                Pmode = local_Branchmode,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_INPUT
            };
        }

        private SendCMD MakeSendCMD_OUTPUT_MC_ON()
        {
            return new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.OUTPUT_MC_ON,
                Pmode = local_Branchmode,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_CHARGE
            };
        }

        private SendCMD MakeSendCMD_REST()
        {
            return new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.REST,
                Pmode = local_Branchmode,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_CHARGE
            };
        }

        private SendCMD MakeSendCMD_REST_2time()
        {
            return new SendCMD()
            {
                Duration = 0.2,
                Index = 0,
                Cmd = CMD.REST,
                Pmode = local_Branchmode,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_CHARGE
            };
        }

        private SendCMD MakeSendCMD_REST_10time()
        {
            return new SendCMD()
            {
                Duration = 1,
                Index = 0,
                Cmd = CMD.REST,
                Pmode = local_Branchmode,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_CHARGE
            };
        }

        private SendCMD MakeSendCMD_OUTPUT_MC_OFF()
        {
            return new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.OUTPUT_MC_OFF,
                Pmode = local_Branchmode,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_INPUT
            };
        }

        #endregion

        #region For DCIR 

        #region Branch control

        /// <summary>
        /// 분기 구분
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ModeSet(TestItem ti)
        {
            var str = "0AA";

            #region get branch to register

            string regSubkey = "Software\\EOL_Trigger";
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
            str = rk.GetValue(position + "_" + viewModel.ModelId + "_BRANCH").ToString();
            #endregion

            //switch (localTypes)
            //{
            //    case ModelType.AUDI_NORMAL: str = "0A1"; break;
            //    case ModelType.AUDI_MIRROR: str = "0A2"; break;
            //    case ModelType.PORSCHE_NORMAL: str = "0A3"; break;
            //    case ModelType.PORSCHE_MIRROR: str = "0A4"; break;
            //    case ModelType.MASERATI_NORMAL: str = "0A5"; break;
            //    case ModelType.MASERATI_MIRROR: str = "0A6"; break;
            //}
            cycler.SendToDSP1(str, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            LogState(LogType.Info, "Cycler Mode Set [" + str + "]");

            Thread.Sleep(500);
            if (cycler.cycler1voltage < 10)
            {
                ti.Value_ = _VOLTAGE_PIN_SENSING_FAIL;
                cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
                return JudgementTestItem(ti);
            }

            return true;
        }

        public bool ModeSet_Release(TestItem ti)
        {
            var str = "0FF";

            cycler.SendToDSP1(str, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            LogState(LogType.Info, "Cycler Mode Set [" + str + "]");
            Thread.Sleep(500);
            if (cycler.cycler1voltage > 10)
            {
                ti.Value_ = _SENSING_BOARD_CHECK;
                cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
                return JudgementTestItem(ti);
            }

            return true;
        }

        #endregion

        #region UI
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
            if (localTypes == ModelType.AUDI_NORMAL)
            {
                cellDetailList[index].CellVolt_1 = daq.DAQList[0];
                cellDetailList[index].CellVolt_2 = daq.DAQList[1];
                cellDetailList[index].CellVolt_3 = daq.DAQList[2];
            }
            else if (localTypes == ModelType.AUDI_MIRROR)
            {
                cellDetailList[index].CellVolt_1 = daq.DAQList[3];
                cellDetailList[index].CellVolt_2 = daq.DAQList[4];
                cellDetailList[index].CellVolt_3 = daq.DAQList[5];
            }
            else if (localTypes == ModelType.E_UP)
            {
                //200804 두번째 DAQ 쪽보드의 0번째부터 6개
                cellDetailList[index].CellVolt_1 = daq.DAQList[6];
                cellDetailList[index].CellVolt_2 = daq.DAQList[7];
                cellDetailList[index].CellVolt_3 = daq.DAQList[8];
                cellDetailList[index].CellVolt_4 = daq.DAQList[9];
                cellDetailList[index].CellVolt_5 = daq.DAQList[10];
                cellDetailList[index].CellVolt_6 = daq.DAQList[11];
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )
            {
                cellDetailList[index].CellVolt_1 = CMClist_Cell[0];
                cellDetailList[index].CellVolt_2 = CMClist_Cell[1];
                cellDetailList[index].CellVolt_3 = CMClist_Cell[2];
                cellDetailList[index].CellVolt_4 = CMClist_Cell[3];
                cellDetailList[index].CellVolt_5 = CMClist_Cell[4];
                cellDetailList[index].CellVolt_6 = CMClist_Cell[5];
            }

            cellDetailList[index].ModuleVolt = cycler.cycler1voltage;
        }

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
        #endregion

        #region for MES
        public void SetFieldsToMESData()
        {
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                if (pro_Type == ProgramType.VoltageInspector)
                {
                    try
                    {
                        var contVal = this.viewModel.ControlItemList[PORS_VOLT_ControlKey01];

                        if (contVal == "0")
                            mes_cv_order = true;
                        else
                            mes_cv_order = false;
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
                else if (pro_Type == ProgramType.HipotInspector)
                {
                    //211013 MEB와 동일하게 수정
                    continueBreaker(1000);

                    try
                    {
                        chroma.CON_INSULATION_RESISTANCE_V = int.Parse(viewModel.ControlItemList[PORS_HIPOT_ControlKey01]);
                        chroma.CON_INSULATION_RESISTANCE_T = int.Parse(viewModel.ControlItemList[PORS_HIPOT_ControlKey02]);

                        chroma.CON_WITHSTANDING_VOLTAGE_V = int.Parse(viewModel.ControlItemList[PORS_HIPOT_ControlKey03]);
                        chroma.CON_WITHSTANDING_VOLTAGE_T = int.Parse(viewModel.ControlItemList[PORS_HIPOT_ControlKey04]);
                        chroma.CON_WITHSTANDING_RAMP_UP_TIME = int.Parse(viewModel.ControlItemList[PORS_HIPOT_ControlKey05]);
                        chroma.CON_ARC_ENABLE = int.Parse(viewModel.ControlItemList[PORS_HIPOT_ControlKey06]);
                        chroma.CON_ARC_LIMIT = int.Parse(viewModel.ControlItemList[PORS_HIPOT_ControlKey07]);

                        chroma.CON_INSULATION_RESISTANCE_RANGE = viewModel.ControlItemList[PORS_HIPOT_ControlKey08];
                        //chroma.CON_WITHSTANDING_RANGE = viewModel.ControlItemList[PORS_HIPOT_ControlKey09];
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
                else if (pro_Type == ProgramType.Hipot_no_resin_Inspector)
                {
                    //211013 MEB와 동일하게 수정
                    continueBreaker(1000);

                    try
                    {
                        chroma.CON_INSULATION_RESISTANCE_V = int.Parse(viewModel.ControlItemList[PORS_HIPNR_ControlKey01]);
                        chroma.CON_INSULATION_RESISTANCE_T = int.Parse(viewModel.ControlItemList[PORS_HIPNR_ControlKey02]);
                        chroma.CON_INSULATION_RESISTANCE_RANGE = viewModel.ControlItemList[PORS_HIPNR_ControlKey03];
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
            }
            else if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                if (pro_Type == ProgramType.HipotInspector)
                {
                    //211013 MEB와 동일하게 수정
                    continueBreaker(1000);

                    try
                    {
                        chroma.CON_INSULATION_RESISTANCE_V = int.Parse(viewModel.ControlItemList[AUDI_HIPOT_ControlKey01]);
                        chroma.CON_INSULATION_RESISTANCE_T = int.Parse(viewModel.ControlItemList[AUDI_HIPOT_ControlKey02]);

                        chroma.CON_WITHSTANDING_VOLTAGE_V = int.Parse(viewModel.ControlItemList[AUDI_HIPOT_ControlKey03]);
                        chroma.CON_WITHSTANDING_VOLTAGE_T = int.Parse(viewModel.ControlItemList[AUDI_HIPOT_ControlKey04]);
                        chroma.CON_WITHSTANDING_RAMP_UP_TIME = int.Parse(viewModel.ControlItemList[AUDI_HIPOT_ControlKey05]);
                        chroma.CON_ARC_ENABLE = int.Parse(viewModel.ControlItemList[AUDI_HIPOT_ControlKey06]);
                        chroma.CON_ARC_LIMIT = int.Parse(viewModel.ControlItemList[AUDI_HIPOT_ControlKey07]);

                        chroma.CON_INSULATION_RESISTANCE_RANGE = viewModel.ControlItemList[AUDI_HIPOT_ControlKey08];
                        //chroma.CON_WITHSTANDING_RANGE = viewModel.ControlItemList[AUDI_HIPOT_ControlKey09];
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
            }
            else if (localTypes == ModelType.E_UP)
            {
                if (pro_Type == ProgramType.HipotInspector)
                {
                    //211013 MEB와 동일하게 수정
                    continueBreaker(1000);

                    try
                    {
                        chroma.CON_INSULATION_RESISTANCE_V = int.Parse(viewModel.ControlItemList[E_UP_HIPOT_ControlKey01]);
                        chroma.CON_INSULATION_RESISTANCE_T = int.Parse(viewModel.ControlItemList[E_UP_HIPOT_ControlKey02]);

                        chroma.CON_WITHSTANDING_VOLTAGE_V = int.Parse(viewModel.ControlItemList[E_UP_HIPOT_ControlKey03]);
                        chroma.CON_WITHSTANDING_VOLTAGE_T = int.Parse(viewModel.ControlItemList[E_UP_HIPOT_ControlKey04]);
                        chroma.CON_WITHSTANDING_RAMP_UP_TIME = int.Parse(viewModel.ControlItemList[E_UP_HIPOT_ControlKey05]);
                        chroma.CON_ARC_ENABLE = int.Parse(viewModel.ControlItemList[E_UP_HIPOT_ControlKey06]);
                        chroma.CON_ARC_LIMIT = int.Parse(viewModel.ControlItemList[E_UP_HIPOT_ControlKey07]);

                        chroma.CON_INSULATION_RESISTANCE_RANGE = viewModel.ControlItemList[E_UP_HIPOT_ControlKey08];
                        //chroma.CON_WITHSTANDING_RANGE = viewModel.ControlItemList[AUDI_HIPOT_ControlKey09];
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
            }
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                if (pro_Type == ProgramType.VoltageInspector)
                {
                    try
                    {
                        var contVal = this.viewModel.ControlItemList[MAS_VOLT_ControlKey01];

                        if (contVal == "0")
                            mes_cv_order = true;
                        else
                            mes_cv_order = false;
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
                else if (pro_Type == ProgramType.HipotInspector)
                {
                    //211013 MEB와 동일하게 수정
                    continueBreaker(1000);

                    try
                    { 
                        chroma.CON_INSULATION_RESISTANCE_V = int.Parse(viewModel.ControlItemList[MAS_HIPOT_ControlKey01]);
                        chroma.CON_INSULATION_RESISTANCE_T = int.Parse(viewModel.ControlItemList[MAS_HIPOT_ControlKey02]);

                        chroma.CON_WITHSTANDING_VOLTAGE_V = int.Parse(viewModel.ControlItemList[MAS_HIPOT_ControlKey03]);
                        chroma.CON_WITHSTANDING_VOLTAGE_T = int.Parse(viewModel.ControlItemList[MAS_HIPOT_ControlKey04]);
                        chroma.CON_WITHSTANDING_RAMP_UP_TIME = int.Parse(viewModel.ControlItemList[MAS_HIPOT_ControlKey05]);
                        chroma.CON_ARC_ENABLE = int.Parse(viewModel.ControlItemList[MAS_HIPOT_ControlKey06]);
                        chroma.CON_ARC_LIMIT = int.Parse(viewModel.ControlItemList[MAS_HIPOT_ControlKey07]);

                        chroma.CON_INSULATION_RESISTANCE_RANGE = viewModel.ControlItemList[MAS_HIPOT_ControlKey08];
                        //chroma.CON_WITHSTANDING_RANGE = viewModel.ControlItemList[PORS_HIPOT_ControlKey09];
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
                else if (pro_Type == ProgramType.Hipot_no_resin_Inspector)
                {
                    //211013 MEB와 동일하게 수정
                    continueBreaker(1000);

                    try
                    {
                        chroma.CON_INSULATION_RESISTANCE_V = int.Parse(viewModel.ControlItemList[MAS_HIPNR_ControlKey01]);
                        chroma.CON_INSULATION_RESISTANCE_T = int.Parse(viewModel.ControlItemList[MAS_HIPNR_ControlKey02]);
                        chroma.CON_INSULATION_RESISTANCE_RANGE = viewModel.ControlItemList[MAS_HIPNR_ControlKey03];
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                if (pro_Type == ProgramType.VoltageInspector)
                {
                    try
                    {
                        var contVal = this.viewModel.ControlItemList[PORS_FL_VOLT_ControlKey01];

                        if (contVal == "0")
                            mes_cv_order = true;
                        else
                            mes_cv_order = false;
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
                else if (pro_Type == ProgramType.HipotInspector)
                {
                    //211013 MEB와 동일하게 수정
                    continueBreaker(1000);

                    try
                    {
                        chroma.CON_INSULATION_RESISTANCE_V = int.Parse(viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey01]);
                        chroma.CON_INSULATION_RESISTANCE_T = int.Parse(viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey02]);
                        chroma.CON_INSULATION_RESISTANCE_RANGE = viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey03];
                        chroma.CON_WITHSTANDING_VOLTAGE_V = int.Parse(viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey04]);
                        chroma.CON_WITHSTANDING_VOLTAGE_T = int.Parse(viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey05]);
                        chroma.CON_WITHSTANDING_RAMP_UP_TIME = int.Parse(viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey06]);
                        chroma.CON_ARC_ENABLE = int.Parse(viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey07]);
                        chroma.CON_ARC_LIMIT = int.Parse(viewModel.ControlItemList[PORS_FL_HIPOT_ControlKey08]);
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
                else if (pro_Type == ProgramType.Hipot_no_resin_Inspector)
                {
                    //211013 MEB와 동일하게 수정
                    continueBreaker(1000);

                    try
                    {
                        chroma.CON_INSULATION_RESISTANCE_V = int.Parse(viewModel.ControlItemList[PORS_FL_HIPNR_ControlKey01]);
                        chroma.CON_INSULATION_RESISTANCE_T = int.Parse(viewModel.ControlItemList[PORS_FL_HIPNR_ControlKey02]);
                        chroma.CON_INSULATION_RESISTANCE_RANGE = viewModel.ControlItemList[PORS_FL_HIPNR_ControlKey03];
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                if (pro_Type == ProgramType.VoltageInspector)
                {
                    try
                    {
                        var contVal = this.viewModel.ControlItemList[MAS_M183_VOLT_ControlKey01];

                        if (contVal == "0")
                            mes_cv_order = true;
                        else
                            mes_cv_order = false;
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
                else if (pro_Type == ProgramType.HipotInspector)
                {
                    //211013 MEB와 동일하게 수정
                    continueBreaker(1000);

                    try
                    {
                        chroma.CON_INSULATION_RESISTANCE_V = int.Parse(viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey01]);
                        chroma.CON_INSULATION_RESISTANCE_T = int.Parse(viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey02]);
                        chroma.CON_INSULATION_RESISTANCE_RANGE = viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey03];
                        chroma.CON_WITHSTANDING_VOLTAGE_V = int.Parse(viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey04]);
                        chroma.CON_WITHSTANDING_VOLTAGE_T = int.Parse(viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey05]);
                        chroma.CON_WITHSTANDING_RAMP_UP_TIME = int.Parse(viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey06]);
                        chroma.CON_ARC_ENABLE = int.Parse(viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey07]);
                        chroma.CON_ARC_LIMIT = int.Parse(viewModel.ControlItemList[MAS_M183_HIPOT_ControlKey08]);
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
                else if (pro_Type == ProgramType.Hipot_no_resin_Inspector)
                {
                    //211013 MEB와 동일하게 수정
                    continueBreaker(1000);

                    try
                    {
                        chroma.CON_INSULATION_RESISTANCE_V = int.Parse(viewModel.ControlItemList[MAS_M183_HIPNR_ControlKey01]);
                        chroma.CON_INSULATION_RESISTANCE_T = int.Parse(viewModel.ControlItemList[MAS_M183_HIPNR_ControlKey02]);
                        chroma.CON_INSULATION_RESISTANCE_RANGE = viewModel.ControlItemList[MAS_M183_HIPNR_ControlKey03];
                    }
                    catch (Exception ec)
                    {
                        // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                        // 200416 예외처리. 박선임이 이렇게 하라고 했씀

                        var fileFolder = "EOL";

                        switch (pro_Type)
                        {
                            case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                            case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                            case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                            case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                        }

                        System.Windows.MessageBox.Show(ec.Message + "\n"
                            + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                        var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                        System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                        //200612 종료시퀀스로 확정 pjh / ht
                        System.Environment.Exit(0);

                    }
                }
            }
        }
        public void SetCyclerFieldsToMESData()
        {

            if (pro_Type != ProgramType.EOLInspector)
                return;

            #region DCIR Schedule
            #region EOL_제어항목 변수
            int index = 0;
            /// <summary>
            /// 방전 전 휴지 시간
            /// </summary>
            var befDiscRestTime = "0";

            /// <summary>
            /// 방전 전류
            /// </summary>
            var discCur = "0";

            /// <summary>
            /// 방전 시간
            /// </summary>
            var discTime = "0";

            /// <summary>
            /// 방전 전류 안전상한
            /// </summary>
            var discCurLimit = "0";

            /// <summary>
            /// 방전이후 휴지시간
            /// </summary>
            var aftDiscRestTime = "0";

            /// <summary>
            /// 충전 전류
            /// </summary>
            var charCur = "0";

            /// <summary>
            /// 충전 시간
            /// </summary>
            var charTime = "0";

            /// <summary>
            /// 충전 전류 안전상한
            /// </summary>
            var charCurLimit = "0";

            /// <summary>
            /// 충전직후 휴지시간
            /// </summary>
            var aftCharRestTime = "0";

            /// <summary>
            /// 충전시의 전압상한
            /// </summary>
            var safeVoltHighLimit = "0";

            /// <summary>
            /// 방전시의 전압하한
            /// </summary>
            var safeVoltLowLimit = "0";
            #endregion
            try
            {
                if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
                {
                    befDiscRestTime = this.viewModel.ControlItemList[AUDI_EOL_ControlKey01];
                    discCur = this.viewModel.ControlItemList[AUDI_EOL_ControlKey02];
                    discTime = this.viewModel.ControlItemList[AUDI_EOL_ControlKey03];
                    discCurLimit = this.viewModel.ControlItemList[AUDI_EOL_ControlKey04];
                    aftDiscRestTime = this.viewModel.ControlItemList[AUDI_EOL_ControlKey05];
                    charCur = this.viewModel.ControlItemList[AUDI_EOL_ControlKey06];
                    charTime = this.viewModel.ControlItemList[AUDI_EOL_ControlKey07];
                    charCurLimit = this.viewModel.ControlItemList[AUDI_EOL_ControlKey08];
                    aftCharRestTime = this.viewModel.ControlItemList[AUDI_EOL_ControlKey09];
                    safeVoltHighLimit = this.viewModel.ControlItemList[AUDI_EOL_ControlKey10];
                    safeVoltLowLimit = this.viewModel.ControlItemList[AUDI_EOL_ControlKey11];
                }

                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    befDiscRestTime = this.viewModel.ControlItemList[PORS_EOL_ControlKey01];
                    discCur = this.viewModel.ControlItemList[PORS_EOL_ControlKey02];
                    discTime = this.viewModel.ControlItemList[PORS_EOL_ControlKey03];
                    discCurLimit = this.viewModel.ControlItemList[PORS_EOL_ControlKey04];
                    aftDiscRestTime = this.viewModel.ControlItemList[PORS_EOL_ControlKey05];
                    charCur = this.viewModel.ControlItemList[PORS_EOL_ControlKey06];
                    charTime = this.viewModel.ControlItemList[PORS_EOL_ControlKey07];
                    charCurLimit = this.viewModel.ControlItemList[PORS_EOL_ControlKey08];
                    aftCharRestTime = this.viewModel.ControlItemList[PORS_EOL_ControlKey09];
                    safeVoltHighLimit = this.viewModel.ControlItemList[PORS_EOL_ControlKey10];
                    safeVoltLowLimit = this.viewModel.ControlItemList[PORS_EOL_ControlKey11];
                }
                //201217 wjs add maserati
                if (localTypes == ModelType.MASERATI_NORMAL)
                {
                    befDiscRestTime = this.viewModel.ControlItemList[MAS_EOL_ControlKey01];
                    discCur = this.viewModel.ControlItemList[MAS_EOL_ControlKey02];
                    discTime = this.viewModel.ControlItemList[MAS_EOL_ControlKey03];
                    discCurLimit = this.viewModel.ControlItemList[MAS_EOL_ControlKey04];
                    aftDiscRestTime = this.viewModel.ControlItemList[MAS_EOL_ControlKey05];
                    charCur = this.viewModel.ControlItemList[MAS_EOL_ControlKey06];
                    charTime = this.viewModel.ControlItemList[MAS_EOL_ControlKey07];
                    charCurLimit = this.viewModel.ControlItemList[MAS_EOL_ControlKey08];
                    aftCharRestTime = this.viewModel.ControlItemList[MAS_EOL_ControlKey09];
                    safeVoltHighLimit = this.viewModel.ControlItemList[MAS_EOL_ControlKey10];
                    safeVoltLowLimit = this.viewModel.ControlItemList[MAS_EOL_ControlKey11];
                }
                //210312 wjs add por fl
                if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    befDiscRestTime = this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey01];
                    discCur = this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey02];
                    discTime = this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey03];
                    discCurLimit = this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey04];
                    aftDiscRestTime = this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey05];
                    charCur = this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey06];
                    charTime = this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey07];
                    charCurLimit = this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey08];
                    aftCharRestTime = this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey09];
                    safeVoltHighLimit = this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey10];
                    safeVoltLowLimit = this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey11];
                }
                //221101 wjs add mase m183
                if (localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    befDiscRestTime = this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey01];
                    discCur = this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey02];
                    discTime = this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey03];
                    discCurLimit = this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey04];
                    aftDiscRestTime = this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey05];
                    charCur = this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey06];
                    charTime = this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey07];
                    charCurLimit = this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey08];
                    aftCharRestTime = this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey09];
                    safeVoltHighLimit = this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey10];
                    safeVoltLowLimit = this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey11];
                }
                if (localTypes == ModelType.E_UP)
                {
                    befDiscRestTime = this.viewModel.ControlItemList[E_UP_EOL_ControlKey01];
                    discCur = this.viewModel.ControlItemList[E_UP_EOL_ControlKey02];
                    discTime = this.viewModel.ControlItemList[E_UP_EOL_ControlKey03];
                    discCurLimit = this.viewModel.ControlItemList[E_UP_EOL_ControlKey04];
                    aftDiscRestTime = this.viewModel.ControlItemList[E_UP_EOL_ControlKey05];
                    charCur = this.viewModel.ControlItemList[E_UP_EOL_ControlKey06];
                    charTime = this.viewModel.ControlItemList[E_UP_EOL_ControlKey07];
                    charCurLimit = this.viewModel.ControlItemList[E_UP_EOL_ControlKey08];
                    aftCharRestTime = this.viewModel.ControlItemList[E_UP_EOL_ControlKey09];
                    safeVoltHighLimit = this.viewModel.ControlItemList[E_UP_EOL_ControlKey10];
                    safeVoltLowLimit = this.viewModel.ControlItemList[E_UP_EOL_ControlKey11];
                }
            }
            catch (Exception ec)
            {
                // 4. 제어부분 ID 미일치 시 문제되는 부분은 추가 확인 부탁드립니다
                // 200416 예외처리. 박진호 선임 확인됨

                var fileFolder = "EOL";

                switch (pro_Type)
                {
                    case ProgramType.EOLInspector: fileFolder = "EOL"; break;
                    case ProgramType.HipotInspector: fileFolder = "HIPOT"; break;
                    case ProgramType.VoltageInspector: fileFolder = "VOLT"; break;
                    case ProgramType.Hipot_no_resin_Inspector: fileFolder = "HIPOT_NO_RESIN"; break;
                }

                System.Windows.MessageBox.Show(ec.Message + "\n"
                    + staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId +
                    ".csv \n파일을 확인하세요!", "Critical!!!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

                var folderAddr = staticPath_EOL_SPEC + "\\" + position + "\\" + fileFolder + "\\" + "ControlList_" + this.viewModel.ModelId + ".csv";
                System.Diagnostics.Process.Start("EXCEL.EXE", folderAddr);

                //200612 종료시퀀스로 확정 pjh / ht
                System.Environment.Exit(0);

            }

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
            #endregion

            this.Dispatcher.Invoke(new Action(() =>
            {
                //voltGraph.MaxY = double.Parse(safeVoltHighLimit);
                //voltGraph.MinY = double.Parse(safeVoltLowLimit);
                //curGraph.MaxY = double.Parse(charCur) + 10;
                //curGraph.MinY = (double.Parse(discCur) * -1.0) - 10;

                this.totalProcessList[0].ScheduleList[index].CycleList[0].StepList = list;
                mmw.totalProcessList = this.totalProcessList;
                mmw.CycleListBox.Items.Refresh();
            }));


        }
        #endregion

        void continueBreaker(int waitTimeMsec)
        {
            int cnt = 0;
            while (chroma == null)
            {
                Thread.Sleep(10);
                cnt += 10;
                LogState(LogType.Info, "wait for Chroma Initilize:" + cnt.ToString(), null, true, false);

                if (cnt > waitTimeMsec)
                    break;
            }
        }


        public void DCIR_Stopped(bool isEMG = true)
        {
            LogState(LogType.Info, "DCIR_Stopped Start:" + (isEMG ? "EMG" : "Normal") +
                "---------------------------------------------");
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

            if (isEMG)
            {
                //IN MC OFF
                cycler.SendToDSP1("100", new byte[] { 0x85, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
                Thread.Sleep(100);
                cycler.SendToDSP1("100", new byte[] { 0x85, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
                Thread.Sleep(100);
                cycler.SendToDSP1("100", new byte[] { 0x85, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
                Thread.Sleep(100);
            }

            cycler.isCyclerStop = true;
            viewVoltage = viewCurrent = 0.0;
            cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            LogState(LogType.Info, "Cycler Mode Reset [FF] in DCIR_Stopped");
            DCIRThreadAbort();
        }

        #region DCIR 간 사용되는 스레드 해지

        Thread dp_disc, dp_disc500ms, dp_char, dp_rest_bef_disc, dp_rest_aft_disc, dp_rest_aft_char, dp_rest_aft_char_10s, dp_rest_aft_char_20s, dp_rest_aft_char_30s;


        private void DCIRThreadAbort()
        {
            if (Protection_ID1 != null)
            {
                Protection_ID1.Abort();
                Protection_ID1 = null;
            }
            if (Protection_CHARGE_ID1 != null)
            {
                Protection_CHARGE_ID1.Abort();
                Protection_CHARGE_ID1 = null;
            }
            if (Protection_DISCHARGE_ID1 != null)
            {
                Protection_DISCHARGE_ID1.Abort();
                Protection_DISCHARGE_ID1 = null;
            }
            if (dp_disc != null)
            {
                dp_disc.Abort();
                dp_disc = null;
            }
             if(dp_disc500ms != null)
            {
                dp_disc500ms.Abort();
                dp_disc500ms = null;
            }
            if (dp_char != null)
            {
                dp_char.Abort();
                dp_char = null;
            }
            if (dp_rest_bef_disc != null)
            {
                dp_rest_bef_disc.Abort();
                dp_rest_bef_disc = null;
            }
            if (dp_rest_aft_disc != null)
            {
                dp_rest_aft_disc.Abort();
                dp_rest_aft_disc = null;
            }
            if (dp_rest_aft_char != null)
            {
                dp_rest_aft_char.Abort();
                dp_rest_aft_char = null;
            }
            if (dp_rest_aft_char_10s != null)
            {
                dp_rest_aft_char_10s.Abort();
                dp_rest_aft_char_10s = null;
            }
            if (dp_rest_aft_char_20s != null)
            {
                dp_rest_aft_char_20s.Abort();
                dp_rest_aft_char_20s = null;
            }
            if (dp_rest_aft_char_30s != null)
            {
                dp_rest_aft_char_30s.Abort();
                dp_rest_aft_char_30s = null;
            }

            //221006 MC 저항 측정 및 수집항목 스레드 추가 nnkim
            if (dp_McShortCheck != null)
            {
                dp_McShortCheck.Abort();
                dp_McShortCheck = null;
            }

            if (Protection_McResistanceStart != null)
            {
                Protection_McResistanceStart.Abort();
                Protection_McResistanceStart = null;
            }

            if (Protection_PartsCountStart != null)
            {
                Protection_PartsCountStart.Abort();
                Protection_PartsCountStart = null;
            }
        }

        #endregion

        #region DCIR_field

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

        public int RestChargeLossTimeMSEC = 0;
        public int RestDischargeLossTimeMSEC = 0;
        public int MC_ON_OUTPUTMSEC = 0;
        public int MC_ON_INPUTMSEC = 0;
        public int JIG_CNTL_MSEC = 0;
        Stopwatch cyclerStwc = new Stopwatch();

        int currentPosition = 0;

        // 190314
        Thread Protection_ID1 = null;
        Thread Protection_CHARGE_ID1 = null;
        Thread Protection_DISCHARGE_ID1 = null;

        Thread Protection_ID2 = null;
        Thread Protection_CHARGE_ID2 = null;
        Thread Protection_DISCHARGE_ID2 = null;

        //221005 저항 측정용 nnkim
        Thread Protection_McResistanceStart = null;
        Thread dp_McShortCheck = null;
        //221017 소모품 카운트용
        Thread Protection_PartsCountStart = null;

        

        CMD localitem_ID1;
        CMD localitem_ID2;
        #endregion

        public bool Do_Rest_Charge(TestItem ti)
        {
            sendList.Clear();

            CCycler.AddToSendListBytes(sendList, MakeSendCMD_INPUT_MC_ON());
            CCycler.AddToSendListBytes(sendList, MakeSendCMD_OUTPUT_MC_ON());

            foreach (var step in this.totalProcessList[0].ScheduleList[0].CycleList[0].StepList)
            {
                CCycler.AddToSendListBytes(sendList, MakeOneStepToSendCMD(step));
            }

            CCycler.AddToSendListBytes(sendList, MakeSendCMD_OUTPUT_MC_OFF());

            int localpos = 0;

            localitem_ID1 = CMD.NULL;
            Protection_ID1 = null;
            Protection_CHARGE_ID1 = null;
            Protection_DISCHARGE_ID1 = null;
            //221006 저항 측정 한번만 할 수 있게 변수 추가 nnkim
            bool bMcStartCheck = true;

            this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    voltItemList.Clear();
                    curItemList.Clear();
                }));
            lock (send)
            {
                #region 3차 프로텍션 - 모든과정 +10초까지만 대기, 이후 팝업, STOP

                Protection_ID1 = new Thread(() =>
                {
                    var totalTime = 0.0;
                    foreach (var sitem in sendList)
                    {
                        totalTime += sitem.Duration;
                    }

                    double countLimit = (totalTime * 1000) + 10000; // (??sec * 1000) + 10000 == @ + 20000msec
                    double count = 0;

                    while (!cycler.isCyclerStop)//충방전이 진행중에만 루프
                    {
                        count += 1000;
                        Thread.Sleep(1000);
                        if (count > countLimit)
                        {
                            isStop = true;
                            LogState(LogType.EMERGENCY, "3rd Protection Stopped(Total time over 10s)");
                            Dispatcher.Invoke(new Action(() =>
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                                    dcw.maintitle.Content = "3rd Protection Stopped(Total time over 10s)";
                                    dcw.reason.Content = "Please check communication to Cycler";
                                    dcw.Show();
                                }));
                            }));
                            break;
                        }
                    }
                    //Protection_ID1 = null;
                });
                Protection_ID1.Start();
                #endregion

                //221006 MC 저항 측정하는 부분 
                if (bMcStartCheck && (int)this.pro_Type == (int)ProgramType.EOLInspector)
                {
                    Protection_McResistanceStart = new Thread(() =>
                    {
                        McResistanceMeasure(McResistanceposition, out rtMcList, true);
                    }
                    );
                    Protection_McResistanceStart.Start();
                    bMcStartCheck = false;
                }

                foreach (var sitem in sendList)
                {
                    if (isStop || ispause)
                    {
                        DCIR_Stopped();

                        ti.Value_ = _EMG_STOPPED;

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


                            while (localitem_ID1 == CMD.CHARGE)
                            {
                                count += 1000;
                                Thread.Sleep(1000);
                                if (count > countLimit)
                                {
                                    isStop = true;
                                    LogState(LogType.EMERGENCY, "2nd Protection Stopped(CHARGE)");
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        this.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                                            dcw.maintitle.Content = "2nd Protection Stopped(CHARGE)";
                                            dcw.reason.Content = "Please check communication to Cycler";
                                            dcw.Show();
                                        }));
                                    }));
                                    break;
                                }
                            }
                            //Protection_CHARGE_ID1 = null;
                        }
                        );
                        Protection_CHARGE_ID1.Start();
                    }

                    if (sitem.Cmd == CMD.DISCHARGE && Protection_DISCHARGE_ID1 == null)
                    {
                        Protection_DISCHARGE_ID1 = new Thread(() =>
                        {
                            double countLimit = (sitem.Duration * 1000) + 5000; // (10sec * 1000) + 5000 == 15000msec
                            double count = 0;

                            while (localitem_ID1 == CMD.DISCHARGE)
                            {
                                count += 1000;
                                Thread.Sleep(1000);
                                if (count > countLimit)
                                {
                                    isStop = true;
                                    LogState(LogType.EMERGENCY, "2nd Protection Stopped(DISCHARGE)");
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        this.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                                            dcw.maintitle.Content = "2nd Protection Stopped(DISCHARGE)";
                                            dcw.reason.Content = "Please check communication to Cycler";
                                            dcw.Show();
                                        }));
                                    }));
                                    break;
                                }

                                // 방전 시 전류가 셋팅 된 전류보다 낮게 흐르면 전촉 문제로 판단해서 알람 발생. 
                                // 접촉이 잘 않된 상태에서 충전으로 넘어갔을 경우 번트 발생 방지 인터락.
                                if (count == 9000)
                                {
                                    if (Math.Abs(cycler.cycler1current) < Math.Abs((sitem.Current - (sitem.Current * 0.01))))
                                    {
                                        if (autoModeThread != null && autoModeThread.IsAlive)
                                        {
                                            if (plc.judgementFlag)
                                            {
                                                isCyclerAlarm = true;
                                            }
                                        }

                                        isStop = true;
                                        this.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                                            dcw.maintitle.Content = "CHECK_TERMINAL";
                                            dcw.reason.Content = "Check the Module Terminal and Current Probe Surface";
                                            dcw.Show();
                                        }));
                                        break;
                                    }
                                }
                            }
                            //Protection_DISCHARGE_ID1 = null;
                        }
                        );
                        Protection_DISCHARGE_ID1.Start();
                    }

                    #endregion
                    for (int i = 0; i < sitem.Duration * 10; i++)
                    {
                        #region Sending Sequence (190312)
                        if (isStop || ispause)
                        {
                            DCIR_Stopped();

                            ti.Value_ = _EMG_STOPPED;

                            return JudgementTestItem(ti);
                        }

                        #region 기본 프로텍션 - 전압상하한
                        if (!CheckSafety(0, cycler.cycler1OP, sitem))
                        {
                            DCIR_Stopped();
                            ti.Value_ = _CYCLER_SAFETY;

                            LogState(LogType.EMERGENCY, "Default Protection Stopped");

                            Dispatcher.Invoke(new Action(() =>
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                                    dcw.maintitle.Content = "Voltage setting limit over";
                                    dcw.reason.Content = "Please check communication to Cycler";
                                    dcw.Show();
                                }));
                            }));

                            return JudgementTestItem(ti);
                        }
                        #endregion

                        cycler.cycler1Header = CMD.NULL;
                        cycler.cycler1OP = RECV_OPMode.NULL;

                        if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
                        {
                            LogState(LogType.Fail, "cycler #1_Send fail");

                            DCIR_Stopped();
                            ti.Value_ = _COMM_FAIL;
                            return JudgementTestItem(ti);
                        }
                        #region 1차 프로텍션 - 타임아웃 - 설정
                        var oldTime = DateTime.Now;
                        var ltime = oldTime;
                        DateTime common_Timeout = oldTime.AddSeconds(5);

                        //if ((sitem.Cmd == CMD.OUTPUT_MC_ON) || (sitem.Cmd == CMD.INPUT_MC_ON) || (sitem.Cmd == CMD.OUTPUT_MC_OFF))
                        //{   // MC 동작은 최대10초까지 걸리니, 해당 동작은 15초로 한다.
                        //    common_Timeout = oldTime.AddSeconds(15);
                        //}

                        #endregion

                        // 해당 슬립이 전체 로직을 100ms로 동작하도록 유지한다.
                        Thread.Sleep(100);

                        while (true)
                        {
                            if ((cycler.cycler1Header == sitem.Cmd) && (cycler.cycler1OP == sitem.NextOPMode))
                            {
                                var sb = new StringBuilder();
                                sb.Append("Cycler(Normal),");
                                sb.Append(cycler.cycler1Header.ToString());
                                sb.Append(",Voltage,");
                                sb.Append(cycler.cycler1voltage.ToString());
                                sb.Append(",Current,");
                                sb.Append(cycler.cycler1current.ToString());

                                if (localTypes == ModelType.AUDI_NORMAL)
                                {
                                    sb.Append(",Cell1,"); sb.Append(daq.DAQList[0].ToString());
                                    sb.Append(",Cell2,"); sb.Append(daq.DAQList[1].ToString());
                                    sb.Append(",Cell3,"); sb.Append(daq.DAQList[2].ToString());
                                }
                                else if (localTypes == ModelType.AUDI_MIRROR)
                                {
                                    sb.Append(",Cell1,"); sb.Append(daq.DAQList[3].ToString());
                                    sb.Append(",Cell2,"); sb.Append(daq.DAQList[4].ToString());
                                    sb.Append(",Cell3,"); sb.Append(daq.DAQList[5].ToString());
                                }
                                else if (localTypes == ModelType.E_UP)
                                {
                                    //200804 두번째 DAQ 쪽보드의 0번째부터 6개
                                    sb.Append(",Cell1,"); sb.Append(daq.DAQList[6].ToString());
                                    sb.Append(",Cell2,"); sb.Append(daq.DAQList[7].ToString());
                                    sb.Append(",Cell3,"); sb.Append(daq.DAQList[8].ToString());
                                    sb.Append(",Cell4,"); sb.Append(daq.DAQList[9].ToString());
                                    sb.Append(",Cell5,"); sb.Append(daq.DAQList[10].ToString());
                                    sb.Append(",Cell6,"); sb.Append(daq.DAQList[11].ToString());
                                }
                                else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                                    || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                                    || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                                    )
                                {
                                    sb.Append(",Cell1,"); sb.Append(CMClist_Cell[0]);
                                    sb.Append(",Cell2,"); sb.Append(CMClist_Cell[1]);
                                    sb.Append(",Cell3,"); sb.Append(CMClist_Cell[2]);
                                    sb.Append(",Cell4,"); sb.Append(CMClist_Cell[3]);
                                    sb.Append(",Cell5,"); sb.Append(CMClist_Cell[4]);
                                    sb.Append(",Cell6,"); sb.Append(CMClist_Cell[5]);
                                }

                                LogState(LogType.Info, sb.ToString(), null, false);
                                break;
                            }
                            else
                            {
                                var sb = new StringBuilder();
                                sb.Append("Cycler(Loop),");
                                sb.Append(cycler.cycler1Header.ToString());
                                sb.Append(",Voltage,");
                                sb.Append(cycler.cycler1voltage.ToString());
                                sb.Append(",Current,");
                                sb.Append(cycler.cycler1current.ToString());

                                if (localTypes == ModelType.AUDI_NORMAL)
                                {
                                    sb.Append(",Cell1,"); sb.Append(daq.DAQList[0].ToString());
                                    sb.Append(",Cell2,"); sb.Append(daq.DAQList[1].ToString());
                                    sb.Append(",Cell3,"); sb.Append(daq.DAQList[2].ToString());
                                }
                                else if (localTypes == ModelType.AUDI_MIRROR)
                                {
                                    sb.Append(",Cell1,"); sb.Append(daq.DAQList[3].ToString());
                                    sb.Append(",Cell2,"); sb.Append(daq.DAQList[4].ToString());
                                    sb.Append(",Cell3,"); sb.Append(daq.DAQList[5].ToString());
                                }
                                else if (localTypes == ModelType.E_UP)
                                {
                                    //200804 두번째 DAQ 쪽보드의 0번째부터 6개
                                    sb.Append(",Cell1,"); sb.Append(daq.DAQList[6].ToString());
                                    sb.Append(",Cell2,"); sb.Append(daq.DAQList[7].ToString());
                                    sb.Append(",Cell3,"); sb.Append(daq.DAQList[8].ToString());
                                    sb.Append(",Cell4,"); sb.Append(daq.DAQList[9].ToString());
                                    sb.Append(",Cell5,"); sb.Append(daq.DAQList[10].ToString());
                                    sb.Append(",Cell6,"); sb.Append(daq.DAQList[11].ToString());
                                }
                                else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                                    || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                                    || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                                    )
                                {
                                    sb.Append(",Cell1,"); sb.Append(CMClist_Cell[0]);
                                    sb.Append(",Cell2,"); sb.Append(CMClist_Cell[1]);
                                    sb.Append(",Cell3,"); sb.Append(CMClist_Cell[2]);
                                    sb.Append(",Cell4,"); sb.Append(CMClist_Cell[3]);
                                    sb.Append(",Cell5,"); sb.Append(CMClist_Cell[4]);
                                    sb.Append(",Cell6,"); sb.Append(CMClist_Cell[5]);
                                }

                                LogState(LogType.Info, sb.ToString(), null, false);
                            }

                            if (isStop || ispause)
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
                            #region 1차 프로텍션 - 타임아웃 - 트리거

                            if (ltime.Ticks >= common_Timeout.Ticks)
                            {
                                LogState(LogType.EMERGENCY, "1st Protection Stopped(Timeout)");

                                DCIR_Stopped();

                                ti.Value_ = _COMM_FAIL;

                                Dispatcher.Invoke(new Action(() =>
                                {
                                    this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                                        dcw.maintitle.Content = "1st Protection Stopped(Timeout)";
                                        dcw.reason.Content = "Please check communication to Cycler";
                                        dcw.Show();
                                    }));
                                }));

                                return JudgementTestItem(ti);
                            }

                            #endregion 
                        }
                        #endregion
                        milisecond += 100;

                        //초기 휴지 9.5
                        if (sitem.Cmd == CMD.REST
                            && ((sitem.Duration - 0.5) * 1000) == milisecond && localpos == 0)
                        {
                            dp_rest_bef_disc = new Thread(new ParameterizedThreadStart(Point_REST_BEF_DISC));
                            dp_rest_bef_disc.Start(ti);
                            localpos = 1;
                        }

                        //방전 0.5
                        if (sitem.Cmd == CMD.DISCHARGE
                            && 500 == milisecond && localpos == 1)
                        {
                            /// <summary>
                            /// 방전 0.5초때의 셀전압을 상세수집으로 보고 요청건 적용
                            /// 적용 모델은 포르쉐 노멀 미러 만 해당
                            /// sitem.Duration 의 데이터 형식은 초단위 * 1000(ms), 예: 9초 -> Value 9 * 1000(ms)
                            /// 의미 없어서 500ms로 픽스, localpos는 단순히 1회성 캐치를 위해 존재
                            /// 210914 전체모델 상세수집 추가
                            /// </summary>
                            
                            dp_disc500ms = new Thread(new ParameterizedThreadStart(Point_DISC_500ms));
                            dp_disc500ms.Start(ti);
                            localpos = 2;
                        }


                        //방전 9.5
                        if (sitem.Cmd == CMD.DISCHARGE
                            && ((sitem.Duration - 0.5) * 1000) == milisecond && localpos == 2)
                        {
                            dp_disc = new Thread(new ParameterizedThreadStart(Point_DISC));
                            dp_disc.Start(ti);
                            localpos = 3;
                        }

                        //중간 휴지 9.5
                        if (sitem.Cmd == CMD.REST
                            && ((sitem.Duration - 0.5) * 1000) == milisecond && localpos == 3)
                        {
                            dp_rest_aft_disc = new Thread(new ParameterizedThreadStart(Point_REST_AFT_DISC));
                            dp_rest_aft_disc.Start(ti);
                            localpos = 4;
                        }
                        
                        //충전 9.5
                        if (sitem.Cmd == CMD.CHARGE
                            && ((sitem.Duration - 0.5) * 1000) == milisecond && localpos == 4)
                        {
                            dp_char = new Thread(new ParameterizedThreadStart(Point_CHAR));
                            dp_char.Start(ti);
                            localpos = 5;
                        }

                        //마지막 Rest 9.5초
                        if (sitem.Cmd == CMD.REST
                            && ((sitem.Duration - 0.5) * 1000) == milisecond && localpos == 5)
                        {
                            dp_rest_aft_char_10s = new Thread(new ParameterizedThreadStart(Point_REST_AFT_CHAR));
                            dp_rest_aft_char_10s.Start(ti);
                            localpos = 6;
                        }
                        viewVoltage = cycler.cycler1voltage;
                        viewCurrent = sitem.Cmd == 
                            CMD.DISCHARGE ? (cycler.cycler1current * -1) : cycler.cycler1current;

                        if (!bMcStartCheck && 1000 == milisecond && (int)this.pro_Type == (int)ProgramType.EOLInspector)
                        {
                            //rest 221006 저항 측정한거 상세 수집
                            dp_McShortCheck = new Thread(new ParameterizedThreadStart(Point_Mc_Short_check));
                            dp_McShortCheck.Start(ti);
                        }
                    }
                }
            }

            DCIR_Stopped(false);
            return true;
        }

        /// <summary>
        /// set Rest CellVoltages / Current Sensing
        /// </summary>
        void Point_REST_BEF_DISC(object aa)
        {
            var ti = aa as TestItem;
            modulePoint_RES_DIS = cycler.cycler1voltage;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(0, KEY_EOL_AUD_NOR_DCIR_CVBEF_DIS, "[INIT VOLT      ] - MODULE,", modulePoint_RES_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_MVBEF_DIS, modulePoint_RES_DIS.ToString()));
            }
            else if(localTypes == ModelType.E_UP)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(0, KEY_EOL_E_UP_DCIR_CVBEF_DIS, "[INIT VOLT      ] - MODULE,", modulePoint_RES_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_MVBEF_DIS, modulePoint_RES_DIS.ToString()));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(0, KEY_EOL_POR_NOR_DCIR_CVBEF_DIS, "[INIT VOLT      ] - MODULE,", modulePoint_RES_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_MVBEF_DIS, modulePoint_RES_DIS.ToString()));
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(0, KEY_EOL_MAS_NOR_DCIR_CVBEF_DIS, "[INIT VOLT      ] - MODULE,", modulePoint_RES_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_MVBEF_DIS, modulePoint_RES_DIS.ToString()));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(0, KEY_EOL_POR_F_L_DCIR_CVBEF_DIS, "[INIT VOLT      ] - MODULE,", modulePoint_RES_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_MVBEF_DIS, modulePoint_RES_DIS.ToString()));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(0, KEY_EOL_MAS_M183_DCIR_CVBEF_DIS, "[INIT VOLT      ] - MODULE,", modulePoint_RES_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_MVBEF_DIS, modulePoint_RES_DIS.ToString()));
            }

        }

        /// <summary>
        /// set Discharge 500ms CellVoltages
        /// </summary>
        void Point_DISC_500ms(object obj)
        {
            TestItem tItem = obj as TestItem;

            double dModuleVoltage = cycler.cycler1voltage;

            //현재는 셀 상세수집만 추가로 보고중!
            //DCIR은 하기 인덱스 {~~gToIndex(1, KEY_EOL_~~}에 따라 cellDetailList에 저장되는데,
            //지금은 500ms 방전 인덱스(1)에 저장은 하나, 기존 9.5sec 방전 인덱스(1)가 덮어쓰는 방식임!
            //나중에 별도 Ratio를 해야한다면, 모든 Cell DCIR 및 아래 인덱스를 전부 하나씩 밀어야함

            LogState(LogType.Info, "Get " + localTypes.ToString() + " 500ms Discharge Voltage (CELL)");

            switch (localTypes)
            {
                case ModelType.AUDI_NORMAL:
                case ModelType.AUDI_MIRROR: tItem.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_AUD_NOR_DCIR_500MS_DIS, "[DISCHARGE 500ms] - MODULE,", dModuleVoltage.ToString())); break;
                case ModelType.E_UP: tItem.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_E_UP_DCIR_500MS_DIS, "[DISCHARGE 500ms] - MODULE,", dModuleVoltage.ToString())); break;
                case ModelType.MASERATI_NORMAL: tItem.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_MAS_NOR_DCIR_500MS_DIS, "[DISCHARGE 500ms] - MODULE,", dModuleVoltage.ToString())); break;
                case ModelType.PORSCHE_NORMAL:
                case ModelType.PORSCHE_MIRROR: tItem.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_POR_NOR_DCIR_500MS_DIS, "[DISCHARGE 500ms] - MODULE,", dModuleVoltage.ToString())); break;
                case ModelType.PORSCHE_FACELIFT_NORMAL:
                case ModelType.PORSCHE_FACELIFT_MIRROR: tItem.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_POR_F_L_DCIR_500MS_DIS, "[DISCHARGE 500ms] - MODULE,", dModuleVoltage.ToString())); break;
                //221101 wjs add mase m183
                case ModelType.MASERATI_M183_NORMAL: tItem.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_MAS_M183_DCIR_500MS_DIS, "[DISCHARGE 500ms] - MODULE,", dModuleVoltage.ToString())); break;
            }
        }

        /// <summary>
        /// set Discharge CellVoltages / Current Sensing
        /// </summary>
        void Point_DISC(object aa)
        {
            var ti = aa as TestItem;

            //7. DCIR 계산 시 실측값으로 나눠지고 있음
            //-> 제어항목에서 받아온 값으로 계산하도록
            //여기서 실측값을 넣고 있으므로 주석처리, 이 이전에 찾아보면 제어값으로 넣는부분이 이미 존재함
            //endCurrent_DIS = cycler.cycler1current;
            modulePoint_DIS = cycler.cycler1voltage;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_AUD_NOR_DCIR_CV_DISCHA, "[DISCHARGE      ] - MODULE,", modulePoint_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_MV_DISCHA, modulePoint_DIS.ToString()));
            }
            else if (localTypes == ModelType.E_UP)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_E_UP_DCIR_CV_DISCHA, "[DISCHARGE      ] - MODULE,", modulePoint_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_MV_DISCHA, modulePoint_DIS.ToString()));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_POR_NOR_DCIR_CV_DISCHA, "[DISCHARGE      ] - MODULE,", modulePoint_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_MV_DISCHA, modulePoint_DIS.ToString()));
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_MAS_NOR_DCIR_CV_DISCHA, "[DISCHARGE      ] - MODULE,", modulePoint_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_MV_DISCHA, modulePoint_DIS.ToString()));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_POR_F_L_DCIR_CV_DISCHA, "[DISCHARGE      ] - MODULE,", modulePoint_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_MV_DISCHA, modulePoint_DIS.ToString()));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(1, KEY_EOL_MAS_M183_DCIR_CV_DISCHA, "[DISCHARGE      ] - MODULE,", modulePoint_DIS.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_MV_DISCHA, modulePoint_DIS.ToString()));
            }
        }


        /// <summary>
        /// set Rest CellVoltages / Current Sensing
        /// </summary>
        void Point_REST_AFT_DISC(object aa)
        {
            var ti = aa as TestItem;
            modulePoint_RES_CHA = cycler.cycler1voltage;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(2, KEY_EOL_AUD_NOR_DCIR_CVBEF_CHA, "[BEFORE CHARGE  ] - MODULE,", modulePoint_RES_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_MVBEF_CHA, modulePoint_RES_CHA.ToString()));
            }
            else if (localTypes == ModelType.E_UP)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(2, KEY_EOL_E_UP_DCIR_CVBEF_CHA, "[BEFORE CHARGE  ] - MODULE,", modulePoint_RES_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_MVBEF_CHA, modulePoint_RES_CHA.ToString()));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(2, KEY_EOL_POR_NOR_DCIR_CVBEF_CHA, "[BEFORE CHARGE  ] - MODULE,", modulePoint_RES_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_MVBEF_CHA, modulePoint_RES_CHA.ToString()));
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(2, KEY_EOL_MAS_NOR_DCIR_CVBEF_CHA, "[BEFORE CHARGE  ] - MODULE,", modulePoint_RES_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_MVBEF_CHA, modulePoint_RES_CHA.ToString()));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(2, KEY_EOL_POR_F_L_DCIR_CVBEF_CHA, "[BEFORE CHARGE  ] - MODULE,", modulePoint_RES_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_MVBEF_CHA, modulePoint_RES_CHA.ToString()));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(2, KEY_EOL_MAS_M183_DCIR_CVBEF_CHA, "[BEFORE CHARGE  ] - MODULE,", modulePoint_RES_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_MVBEF_CHA, modulePoint_RES_CHA.ToString()));
            }
        }

        /// <summary>
        /// set Charge CellVoltages / Current Sensing
        /// </summary>
        void Point_CHAR(object aa)
        {
            var ti = aa as TestItem;
            endCurrent_CHA = cycler.cycler1current;
            modulePoint_CHA = cycler.cycler1voltage;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(3, KEY_EOL_AUD_NOR_DCIR_CV_CHARGE, "[AFTER CHARGE   ] - MODULE,", modulePoint_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_MV_CHARGE, modulePoint_CHA.ToString()));
            }
            else if (localTypes == ModelType.E_UP)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(3, KEY_EOL_E_UP_DCIR_CV_CHARGE, "[AFTER CHARGE   ] - MODULE,", modulePoint_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_MV_CHARGE, modulePoint_CHA.ToString()));

            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(3, KEY_EOL_POR_NOR_DCIR_CV_CHARGE, "[AFTER CHARGE   ] - MODULE,", modulePoint_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_MV_CHARGE, modulePoint_CHA.ToString()));
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(3, KEY_EOL_MAS_NOR_DCIR_CV_CHARGE, "[AFTER CHARGE   ] - MODULE,", modulePoint_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_MV_CHARGE, modulePoint_CHA.ToString()));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(3, KEY_EOL_POR_F_L_DCIR_CV_CHARGE, "[AFTER CHARGE   ] - MODULE,", modulePoint_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_MV_CHARGE, modulePoint_CHA.ToString()));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(3, KEY_EOL_MAS_M183_DCIR_CV_CHARGE, "[AFTER CHARGE   ] - MODULE,", modulePoint_CHA.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_MV_CHARGE, modulePoint_CHA.ToString()));
            }
        }

        /// <summary>
        /// set Rest CellVoltages / Current Sensing
        /// </summary>
        private void Point_REST_AFT_CHAR(object aa)
        {
            var ti = aa as TestItem;
            var moduleVoltage = cycler.cycler1voltage;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(4, KEY_EOL_AUD_NOR_DCIR_CVAFT_CHA, "[FINISH VOLT] - MODULE,", moduleVoltage.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_MVAFT_CHA, moduleVoltage.ToString()));
            }
            else if (localTypes == ModelType.E_UP)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(4, KEY_EOL_E_UP_DCIR_CVAFT_CHA, "[FINISH VOLT] - MODULE,", moduleVoltage.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_MVAFT_CHA, moduleVoltage.ToString()));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(4, KEY_EOL_POR_NOR_DCIR_CVAFT_CHA, "[FINISH VOLT] - MODULE,", moduleVoltage.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_MVAFT_CHA, moduleVoltage.ToString()));
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(4, KEY_EOL_MAS_NOR_DCIR_CVAFT_CHA, "[FINISH VOLT] - MODULE,", moduleVoltage.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_MVAFT_CHA, moduleVoltage.ToString()));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(4, KEY_EOL_POR_F_L_DCIR_CVAFT_CHA, "[FINISH VOLT] - MODULE,", moduleVoltage.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_MVAFT_CHA, moduleVoltage.ToString()));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                ti.refValues_.Add(SetCellVoltagesNloggingToIndex(4, KEY_EOL_MAS_M183_DCIR_CVAFT_CHA, "[FINISH VOLT] - MODULE,", moduleVoltage.ToString()));

                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_MVAFT_CHA, moduleVoltage.ToString()));
            }
        }

        private DetailItems SetCellVoltagesNloggingToIndex(int p1, string dtiKey, string loggingTitle, string moduleVolt)
        {
            SetCellDetailList(p1);

            //Cell Voltage 1~3 before discharge

            var dti = new DetailItems() { Key = dtiKey };
            dti.Reportitems.Add(cellDetailList[p1].CellVolt_1);
            dti.Reportitems.Add(cellDetailList[p1].CellVolt_2);
            dti.Reportitems.Add(cellDetailList[p1].CellVolt_3);

            //포르셰 모델만 더 추가하는 방향으로
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                || localTypes == ModelType.MASERATI_M183_NORMAL  //221101 wjs add mase m183
                || localTypes == ModelType.E_UP)
            {
                dti.Reportitems.Add(cellDetailList[p1].CellVolt_4);
                dti.Reportitems.Add(cellDetailList[p1].CellVolt_5);
                dti.Reportitems.Add(cellDetailList[p1].CellVolt_6);
            }

            //속도개선 190320
            StringBuilder sb = new StringBuilder();
            sb.Append(loggingTitle);
            sb.Append(moduleVolt);
            int cnt = 1;
            foreach (var item in dti.Reportitems)
            {
                sb.Append(",CELL");
                sb.Append(cnt.ToString());
                sb.Append(",");
                sb.Append(item.ToString());
                cnt++;
            }
            LogState(LogType.Info, sb.ToString());
             
            return dti;
        }

        #endregion

        public string mcResistanceposition;

        public string McResistanceposition
        {
            get { return mcResistanceposition; }
            set { mcResistanceposition = value; }
        }

        //221004 저항 측정하는 함수 nnkim
        public void McResistanceMeasure(string strPosition, out List<string> list, bool bDelayResult = false)
        {
            if (bDelayResult)
            {
                //Mc 저항측정 딜레이시간 추가
                string regSubkey = "Software\\EOL_Trigger";
                int nDelay = 0;
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

                var regMCResistanceDelay = rk.GetValue("MC_Resistance_Delay") as string;

                if (regMCResistanceDelay == null) { rk.SetValue("MC_Resistance_Delay", "1000"); nDelay = 1000; }
                else { nDelay = int.Parse(regMCResistanceDelay); }

                Thread.Sleep(nDelay);
            }

            var rtList = new List<double>();
            var dmmChInfo = new DMMChannelInfo(localTypes);
            list = new List<string>();
            double dResultTemp = 0.0;

            keysight.MeasRes_Multi(out rtList, dmmChInfo.ModuleResMCCH, 4, true);

            try
            {
                string regSubkey = "Software\\EOL_Trigger";
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
                var regStrOffset1ch = rk.GetValue("MC3_OFFSET") as string;
                var regStrOffset2ch = rk.GetValue("MC4_OFFSET") as string;
                var regStrOffset3ch = rk.GetValue("MC5_OFFSET") as string;
                var regStrOffset4ch = rk.GetValue("MC6_OFFSET") as string;
                var regStrOffset5ch = rk.GetValue("MC7_OFFSET") as string;
                var regStrOffset6ch = rk.GetValue("MC8_OFFSET") as string;
                var regStrOffset7ch = rk.GetValue("MC9_OFFSET") as string;
                var regStrOffset8ch = rk.GetValue("MC10_OFFSET") as string;
                var regStrOffset9ch = rk.GetValue("MC11_OFFSET") as string;
                var regStrOffset10ch = rk.GetValue("MC12_OFFSET") as string;


                if(localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.E_UP)
                {
                    if (regStrOffset1ch == null) { rk.SetValue("MC3_OFFSET", "25.0"); dResultTemp = 25.0; }
                    else { dResultTemp = double.Parse(regStrOffset1ch); rtList[0] = rtList[0] - dResultTemp; }

                    if (regStrOffset2ch == null) { rk.SetValue("MC4_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset2ch); rtList[1] = rtList[1] - dResultTemp; }

                    if (regStrOffset3ch == null) { rk.SetValue("MC5_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset3ch); rtList[2] = rtList[2] - dResultTemp; }

                    if (regStrOffset4ch == null) { rk.SetValue("MC6_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset4ch); rtList[3] = rtList[3] - dResultTemp; }
                }
                else if(localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    if (regStrOffset1ch == null) { rk.SetValue("MC3_OFFSET", "25.0"); dResultTemp = 25.0; }
                    else { dResultTemp = double.Parse(regStrOffset1ch); rtList[0] = rtList[0] - dResultTemp; }

                    if (regStrOffset2ch == null) { rk.SetValue("MC4_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset2ch); rtList[1] = rtList[1] - dResultTemp; }

                    if (regStrOffset3ch == null) { rk.SetValue("MC9_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset3ch); rtList[2] = rtList[2] - dResultTemp; }

                    if (regStrOffset4ch == null) { rk.SetValue("MC10_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset4ch); rtList[3] = rtList[3] - dResultTemp; }
                }
                else if(localTypes == ModelType.PORSCHE_MIRROR)
                {
                    if (regStrOffset1ch == null) { rk.SetValue("MC3_OFFSET", "25.0"); dResultTemp = 25.0; }
                    else { dResultTemp = double.Parse(regStrOffset1ch); rtList[0] = rtList[0] - dResultTemp; }

                    if (regStrOffset2ch == null) { rk.SetValue("MC4_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset2ch); rtList[1] = rtList[1] - dResultTemp; }

                    if (regStrOffset3ch == null) { rk.SetValue("MC7_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset3ch); rtList[2] = rtList[2] - dResultTemp; }

                    if (regStrOffset4ch == null) { rk.SetValue("MC8_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset4ch); rtList[3] = rtList[3] - dResultTemp; }
                }
                else if(localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    if (regStrOffset1ch == null) { rk.SetValue("MC3_OFFSET", "25.0"); dResultTemp = 25.0; }
                    else { dResultTemp = double.Parse(regStrOffset1ch); rtList[0] = rtList[0] - dResultTemp; }

                    if (regStrOffset2ch == null) { rk.SetValue("MC4_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset2ch); rtList[1] = rtList[1] - dResultTemp; }

                    if (regStrOffset3ch == null) { rk.SetValue("MC11_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset3ch); rtList[2] = rtList[2] - dResultTemp; }

                    if (regStrOffset4ch == null) { rk.SetValue("MC12_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset4ch); rtList[3] = rtList[3] - dResultTemp; }
                }
                else
                {
                    if (regStrOffset1ch == null) { rk.SetValue("MC3_OFFSET", "25.0"); dResultTemp = 25.0; }
                    else { dResultTemp = double.Parse(regStrOffset1ch); rtList[0] = rtList[0] - dResultTemp; }

                    if (regStrOffset2ch == null) { rk.SetValue("MC4_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset2ch); rtList[1] = rtList[1] - dResultTemp; }

                    if (regStrOffset3ch == null) { rk.SetValue("MC5_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset3ch); rtList[2] = rtList[2] - dResultTemp; }

                    if (regStrOffset4ch == null) { rk.SetValue("MC6_OFFSET", "0.0"); dResultTemp = 0.0; }
                    else { dResultTemp = double.Parse(regStrOffset4ch); rtList[3] = rtList[3] - dResultTemp; }
                }
                
            }
            catch { }

            for (int i = 0; i < 10; i++)
            {
                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.E_UP)
                {

                    if (i == 0 || i == 1 || i == 2 || i == 3 ) 
                    {
                        if (rtList[i] > 9.9E+30)
                        {
                            rtList[i] = 99999;
                        }
                        list.Add(rtList[i].ToString());        
                    }
                    else                                       
                    {  
                        list.Add("not use");                   
                    }
                }
                else if(localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    if (i == 0 || i == 1) { list.Add(rtList[i].ToString()); }
                    else if (i == 6) { list.Add(rtList[2].ToString()); }
                    else if (i == 7) { list.Add(rtList[3].ToString()); }
                    else { list.Add("not use"); }
                }
                else if(localTypes == ModelType.PORSCHE_MIRROR)
                {
                    if (i == 0 || i == 1) { list.Add(rtList[i].ToString()); }
                    else if (i == 4) { list.Add(rtList[2].ToString()); }
                    else if (i == 5) { list.Add(rtList[3].ToString()); }
                    else { list.Add("not use"); }
                }
                else if(localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    if (i == 0 || i == 1) { list.Add(rtList[i].ToString()); }
                    else if (i == 8) { list.Add(rtList[2].ToString()); }
                    else if (i == 9) { list.Add(rtList[3].ToString()); }
                    else { list.Add("not use"); }
                }
                else
                {
                    list.Add("not use");
                }
            }
        }

        //221006 저항 측정 상세수집
        void Point_Mc_Short_check(object aa)
        {
            Thread.Sleep(1000);
            try
            {
                var ti = aa as TestItem;
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_3_SHORT_CHECK, rtMcList[0]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_4_SHORT_CHECK, rtMcList[1]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_5_SHORT_CHECK, rtMcList[2]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_6_SHORT_CHECK, rtMcList[3]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_7_SHORT_CHECK, rtMcList[4]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_8_SHORT_CHECK, rtMcList[5]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_9_SHORT_CHECK, rtMcList[6]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_10_SHORT_CHECK, rtMcList[7]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_11_SHORT_CHECK, rtMcList[8]));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_12_SHORT_CHECK, rtMcList[9]));

                string strTemp = rtMcList[0].ToString() + "," + rtMcList[1].ToString()
                     + "," + rtMcList[2].ToString() + "," + rtMcList[3].ToString()
                      + "," + rtMcList[4].ToString() + "," + rtMcList[5].ToString()
                      + "," + rtMcList[6].ToString() + "," + rtMcList[7].ToString()
                      + "," + rtMcList[8].ToString() + "," + rtMcList[9].ToString();

                LogState(LogType.Info, "Result : ," + strTemp + ", (MC Short Check)");

                rtMcList.Clear();
            }
            catch (Exception)
            {

            }

        }
        #region For User Schedule

        public bool Do_UserSchedule(int index)
        {
            sendList.Clear();

            CCycler.AddToSendListBytes(sendList, MakeSendCMD_INPUT_MC_ON());
            CCycler.AddToSendListBytes(sendList, MakeSendCMD_OUTPUT_MC_ON());

            //user schedule
            foreach (var step in this.totalProcessList[0].ScheduleList[index].CycleList[0].StepList)
            {
                CCycler.AddToSendListBytes(sendList, MakeOneStepToSendCMD(step));

                StepError se = StepError.NO_ERROR;
                if(!CheckStepEndCondition(step,out se))
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                            dcw.maintitle.Content = "Schedule FAIL : "+se.ToString();
                            dcw.reason.Content = "Please check Schedules.";
                            dcw.Show();
                        }));
                    }));
                    return false;
                }
            }

            CCycler.AddToSendListBytes(sendList, MakeSendCMD_OUTPUT_MC_OFF());

            int localpos = 0;

            localitem_ID1 = CMD.NULL;
            Protection_ID1 = null;
            Protection_CHARGE_ID1 = null;
            Protection_DISCHARGE_ID1 = null;

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                voltItemList.Clear();
                curItemList.Clear();
            }));
            lock (send)
            {
                #region 3차 프로텍션 - 모든과정 +10초까지만 대기, 이후 팝업, STOP

                Protection_ID1 = new Thread(() =>
                {
                    var totalTime = 0.0;
                    foreach (var sitem in sendList)
                    {
                        totalTime += sitem.Duration;
                    }

                    double countLimit = (totalTime * 1000) + 10000; // (??sec * 1000) + 10000 == @ + 20000msec
                    double count = 0;

                    while (!cycler.isCyclerStop)//충방전이 진행중에만 루프
                    {
                        count += 1000;
                        Thread.Sleep(1000);
                        if (count > countLimit)
                        {
                            isStop = true;
                            LogState(LogType.EMERGENCY, "3rd Protection Stopped(Total time over 10s)");
                            Dispatcher.Invoke(new Action(() =>
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                                    dcw.maintitle.Content = "3rd Protection Stopped(Total time over 10s)";
                                    dcw.reason.Content = "Please check communication to Cycler";
                                    dcw.Show();
                                }));
                            }));
                            break;
                        }
                    }
                    //Protection_ID1 = null;
                });
                Protection_ID1.Start();

                #endregion

                foreach (var sitem in sendList)
                {
                    if (isStop || ispause)
                    {
                        DCIR_Stopped();
                        return false;
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


                            while (localitem_ID1 == CMD.CHARGE)
                            {
                                count += 1000;
                                Thread.Sleep(1000);
                                if (count > countLimit)
                                {
                                    isStop = true;
                                    LogState(LogType.EMERGENCY, "2nd Protection Stopped(CHARGE)");
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        this.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                                            dcw.maintitle.Content = "2nd Protection Stopped(CHARGE)";
                                            dcw.reason.Content = "Please check communication to Cycler";
                                            dcw.Show();
                                        }));
                                    }));
                                    break;
                                }
                            }
                            //Protection_CHARGE_ID1 = null;
                        }
                        );
                        Protection_CHARGE_ID1.Start();
                    }

                    if (sitem.Cmd == CMD.DISCHARGE && Protection_DISCHARGE_ID1 == null)
                    {
                        Protection_DISCHARGE_ID1 = new Thread(() =>
                        {
                            double countLimit = (sitem.Duration * 1000) + 5000; // (10sec * 1000) + 5000 == 15000msec
                            double count = 0;


                            while (localitem_ID1 == CMD.DISCHARGE)
                            {
                                count += 1000;
                                Thread.Sleep(1000);
                                if (count > countLimit)
                                {
                                    isStop = true;
                                    LogState(LogType.EMERGENCY, "2nd Protection Stopped(DISCHARGE)");
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        this.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                                            dcw.maintitle.Content = "2nd Protection Stopped(DISCHARGE)";
                                            dcw.reason.Content = "Please check communication to Cycler";
                                            dcw.Show();
                                        }));
                                    }));
                                    break;
                                }
                            }
                            //Protection_DISCHARGE_ID1 = null;
                        }
                        );
                        Protection_DISCHARGE_ID1.Start();
                    }

                    #endregion
                    for (int i = 0; i < sitem.Duration * 10; i++)
                    {
                        #region Sending Sequence (190312)
                        if (isStop || ispause)
                        {
                            DCIR_Stopped();
                            return false;
                        }

                        #region 기본 프로텍션 - 전압상하한
                        if (!CheckSafety(0, cycler.cycler1OP, sitem))
                        {
                            DCIR_Stopped();

                            LogState(LogType.EMERGENCY, "Default Protection Stopped");

                            Dispatcher.Invoke(new Action(() =>
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                                    dcw.maintitle.Content = "Voltage setting limit over";
                                    dcw.reason.Content = "Please check communication to Cycler";
                                    dcw.Show();
                                }));
                            }));

                            return false;
                        }
                        #endregion

                        //다음 스텝으로 전환
                        if (!CheckEndStep(sitem))
                        {
                            LogState(LogType.Success, "Reached to setting value", null, true, false);
                            break;
                        }

                        cycler.cycler1Header = CMD.NULL;
                        cycler.cycler1OP = RECV_OPMode.NULL;

                        if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
                        {
                            LogState(LogType.Fail, "cycler #1_Send fail");

                            DCIR_Stopped();
                            return false;
                        }
                        #region 1차 프로텍션 - 타임아웃 - 설정

                        var oldTime = DateTime.Now;
                        var ltime = oldTime;
                        DateTime common_Timeout = oldTime.AddSeconds(5);

                        #endregion

                        // 해당 슬립이 전체 로직을 100ms로 동작하도록 유지한다.
                        Thread.Sleep(100);

                        while (true)
                        {
                            //todo
                            //추가 종료조건은 여기서 설정해야 한다.                            
                            //Endcase_Current = end_current,
                            //Endcase_VoltHigh = end_voltHigh,
                            //Endcase_VoltLow = end_voltLow,

                            //설정된게 CC인데, CV가 들어와도 정상동작 해야함
                            if (sitem.NextOPMode == RECV_OPMode.DISCHARGE_CC
                                || sitem.NextOPMode == RECV_OPMode.DISCHARGE_CP)
                            {
                                //while만 종료
                                if (!CheckEndStep(sitem))
                                {
                                    LogState(LogType.Success, "Reached to setting value", null, true, false);
                                    break;
                                }

                                //자기자신이거나, CV면 동작
                                if (cycler.cycler1OP == sitem.NextOPMode ||
                                    cycler.cycler1OP == RECV_OPMode.DISCHARGE_CV)
                                {
                                    LoggingCyclerData(true);
                                    break;
                                }
                                else
                                {
                                    LoggingCyclerData(false);
                                }
                            }
                            else if (sitem.NextOPMode == RECV_OPMode.CHARGE_CC
                                  || sitem.NextOPMode == RECV_OPMode.CHARGE_CP)
                            {
                                //자기자신이거나, CV면 동작
                                if (cycler.cycler1OP == sitem.NextOPMode ||
                                    cycler.cycler1OP == RECV_OPMode.CHARGE_CV)
                                {
                                    LoggingCyclerData(true);
                                    break;
                                }
                                else
                                {
                                    LoggingCyclerData(false);
                                }
                            }
                            else
                            {
                                if ((cycler.cycler1Header == sitem.Cmd) && (cycler.cycler1OP == sitem.NextOPMode))
                                {
                                    LoggingCyclerData(true);
                                    break;
                                }
                                else
                                {
                                    LoggingCyclerData(false);
                                }
                            }
                            


                            if (isStop || ispause)
                            {
                                DCIR_Stopped();
                                return false;
                            }

                            ltime = DateTime.Now;

                            #region send fail
                            if (!cycler.BTUtoDSP(sitem.Index, sitem.SendingByte, sitem.Cmd))
                            {
                                LogState(LogType.Fail, "cycler #1_Send fail");

                                DCIR_Stopped();
                                return false;
                            }

                            #endregion
                            Thread.Sleep(100);
                            #region 1차 프로텍션 - 타임아웃 - 트리거

                            if (ltime.Ticks >= common_Timeout.Ticks)
                            {
                                LogState(LogType.EMERGENCY, "1st Protection Stopped(Timeout)");

                                DCIR_Stopped();
                                 
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        DeviceCheckWindow dcw = new DeviceCheckWindow(this);
                                        dcw.maintitle.Content = "1st Protection Stopped(Timeout)";
                                        dcw.reason.Content = "Please check communication to Cycler";
                                        dcw.Show();
                                    }));
                                }));

                                return false;
                            }

                            #endregion 
                        }
                        #endregion
                        milisecond += 100;

                        viewVoltage = cycler.cycler1voltage;
                        viewCurrent = sitem.Cmd ==
                            CMD.DISCHARGE ? (cycler.cycler1current * -1) : cycler.cycler1current;
                    }
                }
            }

            DCIR_Stopped(false);
            return true;
        }
        #endregion
    }
}