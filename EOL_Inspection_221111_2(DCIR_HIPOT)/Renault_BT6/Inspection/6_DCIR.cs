using Renault_BT6.모듈;
using Renault_BT6.클래스;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Renault_BT6
{
    public partial class MainWindow
    {
        bool isCyclerUse = false;

        // 충방전 내내 500ms마다 계측한 온도값을 저장하는 List
        List<string> tempListInDCIR = new List<string>();

        RegistryKey rk;

        #region DCIR 관련 메서드


        Thread Protection_ID1 = null;
        Thread Protection_CHARGE_ID1 = null;
        Thread Protection_DISCHARGE_ID1 = null;


        /// <summary>
        /// 분기 구분
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        private bool ModeSet(TestItem ti)
        {
            var str = "";
            switch (localTypes)
            {
                //0.4P8S +, -
                //1.4P8S(Reverse) -, +
                //2.4P7S 아래가 +, 위가 -
                //3.3P8S +, -
                //4.3P10S +, -
                //5.3P10S(Reverse) -, +
                //AA : 
                //BB : 
                //CC : 아래 +, 위 - 
                case 0:
                case 3:
                case 4:
                    str = "0AA"; //4P8S, 3P10S, 3P8S
                    break;
                case 1:
                case 5:
                    str = "0BB"; //4P8S REV, 3P10S REV
                    break;
                case 2:
                    str = "0CC"; //4P7S
                    break;
            }
            cycler.SendToDSP1(str, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            LogState(LogType.Info, "Cycler Mode Set [" + str + "]");
            Thread.Sleep(500);
            if (cycler.cycler1voltage < 10)
            {
                ti.Value_ = _DEVICE_NOT_READY;
                cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
                return JudgementTestItem(ti);
            }

            return true;
        }

        CMD localitem_ID1;
        
        DetailItems 상세저항1;// = new DetailItems();
        DetailItems 상세저항2;// = new DetailItems();

        public bool Do_Rest_Charge_New(TestItem ti)
        {
            try
            {
                //221110
                bool bIndexResult = true;

                if (bIndexResult == true)
                {
                    ////221108 nnkim
                    Protection_McResistanceStart = new Thread(() =>
                    {
                        McResistanceMeasure(McResistanceposition, out rtMcList, true);
                    });
                    Protection_McResistanceStart.Start();
                    bIndexResult = false;
                }

                // 충방전 시작 전 FF 전송 후 1초 대기
                // 박진호 선임 요청 사항 
                Device.Cycler.ChgDisControl.SetVoltageSensingOn(false);
                Thread.Sleep(500);

                foreach (var step in totalProcessList[0].ScheduleList[0].CycleList[0].StepList)
                {
                    Device.Cycler.AddToSchedule(MakeOneStepToSendCMD(step));
                }

                if (Device.Cycler.ThreadPoolCyclerStart(0.5) == false)
                {
                    Device.PLC.CyclerAlarm(true);
                    Thread.Sleep(1000);
                    Device.PLC.CyclerAlarm(false);
                    return false;
                }



                Device.Cycler.ThreadPoolEvenWait();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// schedule core
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>

   
        //private void 상시저항가져오는타이머_Tick(object obj)
        private void 상시저항가져오는타이머_Tick(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            if (!cycler.is84On)
            {
                double outTemp1 = 0.0;
                double outTemp2 = 0.0;
                //var dtiStr = getModuleTemp1N2(out tempp, out outTemp1, out outTemp2);
                상세저항1.Reportitems.Add(outTemp1);
                상세저항2.Reportitems.Add(outTemp2);
            }
        }


        // moons new MakeStepCMD
        private PhoenixonLibrary.SendCMD MakeOneStepToSendCMD(MiniScheduler.Step step)
        {

            string time = "0.000";
            try
            {
                time = (step.ClearcaseList.FirstOrDefault(x => x.TitleValue == "Time") as MiniScheduler.ClearCase).Value;

                double stepTime = 0.0;
                if (time == "0.000" || time == "00:00:00:00")
                {
                    stepTime = 0.1;
                }
                else
                {
                    var tarr = time.Split(':');//시분초
                    stepTime = (int.Parse(tarr[0]) * 86400) + (int.Parse(tarr[1]) * 3600) + (int.Parse(tarr[2]) * 60) + (int.Parse(tarr[3]));
                }

                PhoenixonLibrary.eCMD cmd = PhoenixonLibrary.eCMD.NULL;
                PhoenixonLibrary.OPMode opm = PhoenixonLibrary.OPMode.READY;

                double voltHighLimit = 0.0;
                double voltLowLimit = 0.0;
                double currHighLimit = 0.0;

                switch (step.Step_type)
                {
                    case "Rest":
                        {
                            cmd = PhoenixonLibrary.eCMD.REST;

                            if (step.SafecaseData.VoMax != "0.000" || step.SafecaseData.VoMax != "0")       
                                double.TryParse(step.SafecaseData.VoMax, out voltHighLimit);

                            if (step.SafecaseData.VoMin != "0.000" || step.SafecaseData.VoMin != "0")
                                double.TryParse(step.SafecaseData.VoMin, out voltLowLimit);
                          
                        }
                        
                        break;
                    case "Charge":
                        {
                            cmd = PhoenixonLibrary.eCMD.CHARGE;
                            if (step.SafecaseData.VoMax != "0.000" || step.SafecaseData.VoMax != "0")
                            {
                                double.TryParse(step.SafecaseData.VoMax, out voltHighLimit);
                            }

                            if (step.SafecaseData.VoMin != "0.000" || step.SafecaseData.VoMin != "0")
                            {
                                double.TryParse(step.SafecaseData.VoMin, out voltLowLimit);
                            }

                            double.TryParse(step.SafecaseData.CuMax, out currHighLimit);

                            if (step.Step_mode == "CC")
                                opm = PhoenixonLibrary.OPMode.CHARGE_CC;
                            else
                                opm = PhoenixonLibrary.OPMode.CHARGE_CV;
                        }
                        break;
                    case "Discharge":
                        {
                            cmd = PhoenixonLibrary.eCMD.DISCHARGE;
                            if (step.SafecaseData.VoMin != "0.000" || step.SafecaseData.VoMin != "0")
                            {
                                double.TryParse(step.SafecaseData.VoMin, out voltLowLimit);
                            }

                            if (step.SafecaseData.VoMax != "0.000" || step.SafecaseData.VoMax != "0")
                            {
                                double.TryParse(step.SafecaseData.VoMax, out voltHighLimit);
                            }

                            double.TryParse(step.SafecaseData.CuMax, out currHighLimit);

                            if (step.Step_mode == "CC")
                                opm = PhoenixonLibrary.OPMode.DISCHARGE_CC;
                            else
                                opm = PhoenixonLibrary.OPMode.DISCHARGE_CV;
                        }
                        break;
                }

                var voltage = 0.0;

                if (cmd == PhoenixonLibrary.eCMD.DISCHARGE)
                    double.TryParse(step.Discharge_voltage, out voltage);
                else
                    double.TryParse(step.Voltage, out voltage);

                var current = 0.0;
                double.TryParse(step.Current, out current);

                return new PhoenixonLibrary.SendCMD()
                {
                    Cmd = cmd,
                    Opmode = opm,
                    Duration = stepTime,
                    Pmode = PhoenixonLibrary.BranchMode.CHANNEL1,
                    Voltage = voltage,
                    Current = current,
                    VoltLowLimit = voltLowLimit,
                    VoltHighLimit = voltHighLimit,
                    CurrHighLimit = currHighLimit,
                    CutOffCurrent = 0,
                    CutOffVoltage = 0,

                };
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.Message);
                LogState(LogType.Fail, ex.StackTrace);

                return null;
            }
        }


        private SendCMD MakeSendCMD_OUTPUT_MC_OFF()
        {
            return new SendCMD()
            {
                Duration = 0.1,
                Index = 0,
                Cmd = CMD.OUTPUT_MC_OFF,
                Pmode = BranchMode.CHANNEL1,
                Opmode = OPMode.READY,
                Voltage = 0,
                Current = 0,
                NextOPMode = RECV_OPMode.READY_TO_INPUT
            };
        }

        #endregion

     


        #region New PhoenixonLibrary 20190628 moons
        public bool Module_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            var point1 = modulePoint_RES_DIS;
            var point2 = modulePoint_DIS;

            tempdcirmodule_DIS = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);
            LogState(LogType.TEST, "Origin DCIR Value:" +
                        string.Format("({0} - {1}) / {2}) * 1000 = {3}", 
                                            point1, 
                                            point2,
                                            endCurrent_DIS,
                                            tempdcirmodule_DIS));


            //LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + {3}", tempdcirmodule_DIS, moduleFomula1, _DCIR.ModuleTemp, moduleFomula2));

            // 기존 
            //ti.Value_ = _DCIR.GetDCIR_Cal(tempdcirmodule_DIS, moduleFomula1, moduleFomula2, _DCIR.ModuleTemp);

            string DcirLog = "";
            ti.Value_= _DCIR.GetModuleDCIR_Cal(tempdcirmodule_DIS, moduleFomula1, moduleFomula2, moduleFomula3, _DCIR.ModuleTemp, out DcirLog);

            SetContorlLimitData(29, ti.Value_.ToString());

            LogState(LogType.Info, DcirLog);
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());
         
            return JudgementTestItem(ti);
        }
        #endregion

        private void DCIRThreadAbort()
        {
            if (Protection_ID1 != null)
            {
                Protection_ID1.Abort();
            }
            if (Protection_CHARGE_ID1 != null)
            {
                Protection_CHARGE_ID1.Abort();
            }
            if (Protection_DISCHARGE_ID1 != null)
            {
                Protection_DISCHARGE_ID1.Abort();
            }             
            if (dp_disc != null)
            {
                dp_disc.Abort();
            }
            if (dp_char != null)
            {
                dp_char.Abort();
            }
            if (dp_rest_bef_disc != null)
            {
                dp_rest_bef_disc.Abort();
            }
            if (dp_rest_aft_disc != null)
            {
                dp_rest_aft_disc.Abort();
            }
            if (dp_rest_aft_char != null)
            {
                dp_rest_aft_char.Abort();
            }
            if (dp_rest_aft_char_10s != null)
            {
                dp_rest_aft_char_10s.Abort();
            }
            if (dp_rest_aft_char_20s != null)
            {
                dp_rest_aft_char_20s.Abort();
            }
            if (dp_rest_aft_char_30s != null)
            {
                dp_rest_aft_char_30s.Abort();
            }

            //221006 MC 저항 기능 nnkim
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

        /// 0: INIT VOLT
        /// 1: AFTER DISCHARGE
        /// 2: BEFORE CHARGE
        /// 3: AFTER CHARGE
        /// 4: FINISH VOLT
        /// 
        public bool Cell1_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                _DCIR.Init();

                #region 충방전 온도 계산
                //double moduleVolt = Device.KeysightDAQ.GetMeasVolt(BatteryInfo.ModuleCH);
                double moduleVolt = Device.KeysightDAQ.MeasVolt_Single(BatteryInfo.ModuleCH);
                LogState(LogType.Info, string.Format("Module Volot: {0}", moduleVolt));

                if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    LogState(LogType.Info, "Room Temp:" + Device.Tempr.GetTemp);
                }
                else
                {
                    LogState(LogType.Info, "Room Temp:" + Device.TemprCT100.GetTemprature().ToString());
                }
                //LogState(LogType.Info, "Room Temp:" + Device.Tempr.GetTemp);
                //LogState(LogType.Info, "Room Temp:" + Device.TemprCT100.GetTemprature().ToString());

                counter_Cycler++;

                double moduleRes = double.Parse(Device.KeysightDAQ.MeasRes(BatteryInfo.ModuleResCH));

                LogState(LogType.Info,
                            string.Format("Res Temp,3435 / ( 3435 / 301.2 + Math.Log(({0} * 0.001) / 10.0))  - 276.2",
                            moduleRes));

                _DCIR.ModuleTemp = _DCIR.GetModuleTempCal(moduleRes); // 충방 전 온도
                LogState(LogType.Info, "DCIR Before Module Temp:" + _DCIR.ModuleTemp);
                #endregion

                #region MES 상세 수집  충방전 온도 값
                if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.AmbientTempBefore, Device.Tempr.GetTemp.ToString()));
                    //ti.refValues_.Add(MakeDetailItem(EOL.MesID.AmbientTempBefore, Device.TemprCT100.GetTemprature().ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ModuleTempBefore, _DCIR.ModuleTemp.ToString()));
                }
                else
                {
                    //ti.refValues_.Add(MakeDetailItem(EOL.MesID.AmbientTempBefore, Device.Tempr.GetTemp.ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.AmbientTempBefore, Device.TemprCT100.GetTemprature().ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ModuleTempBefore, _DCIR.ModuleTemp.ToString()));
                }
                #endregion

                //221110 위치 이동 기존 저항 찍은 포인트
                ////221108 nnkim
                //Protection_McResistanceStart = new Thread(() =>
                //{
                //    McResistanceMeasure(McResistanceposition, out rtMcList);
                //});
                //Protection_McResistanceStart.Start();

                #region DCIR START
                if (Device.Cycler.GetDspStatus() == false)
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }

                if (counter_Cycler_limit < counter_Cycler)
                {
                    ti.Value_ = "LIMIT OF CYCLER COUNT";
                    return JudgementTestItem(ti);
                }

                if (!Do_Rest_Charge_New(ti))
                {
                    return JudgementTestItem(ti);
                }



                int listCount = totalProcessList[0].ScheduleList[0].CycleList[0].StepList.Count;
                int nCountResult = 0;
                if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    nCountResult = 1;
                }
                else
                {
                    nCountResult = 2;
                }

                if (Device.Cycler.GetDisChgPointList.Count < nCountResult) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)

                    SetMainVoltage("0.00");
                    SetMainCurrent("0.00");
                    SetMainCState("Ready");

                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }
                #endregion

                #region MES 상세 수집  충방전 이후 온도 값
                moduleRes = double.Parse(Device.KeysightDAQ.MeasRes(BatteryInfo.ModuleResCH));
                double moduleTempAfter = 0.0d;

                if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    _DCIR.ModuleTemp = _DCIR.GetModuleTempCal(moduleRes); // 20211214 : _DCIR.ModuleTem은 충방 전 온도 전용임 (해당 값으로 보정식 적용)
                    LogState(LogType.Info, string.Format("Res Temp,3435 / ( 3435 / 301.2 + Math.Log(({0} * 0.001) / 10.0))  - 276.2", moduleRes));
                    LogState(LogType.Info, "DCIR After Module Temp:" + _DCIR.ModuleTemp);
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.AmbientTempAfter, Device.Tempr.GetTemp.ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ModuleTempAfter, _DCIR.ModuleTemp.ToString()));
                }
                else
                {
                    moduleTempAfter = _DCIR.GetModuleTempCal(moduleRes);
                    LogState(LogType.Info,string.Format("Res Temp,3435 / ( 3435 / 301.2 + Math.Log(({0} * 0.001) / 10.0))  - 276.2",moduleRes));
                    LogState(LogType.Info, "DCIR After Module Temp:" + moduleTempAfter/*_DCIR.ModuleTemp*/);
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.AmbientTempAfter, Device.TemprCT100.GetTemprature().ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ModuleTempAfter, moduleTempAfter/*_DCIR.ModuleTemp*/.ToString()));
                }

                #endregion

                //221110
                try
                {
                    ti.refValues_.Add(MakeDetailItem("MC3SHORTCHECK", rtMcList[0]));
                    ti.refValues_.Add(MakeDetailItem("MC4SHORTCHECK", rtMcList[1]));
                    ti.refValues_.Add(MakeDetailItem("MC5SHORTCHECK", rtMcList[2]));
                    ti.refValues_.Add(MakeDetailItem("MC6SHORTCHECK", rtMcList[3]));
                    ti.refValues_.Add(MakeDetailItem("MC7SHORTCHECK", rtMcList[4]));
                    ti.refValues_.Add(MakeDetailItem("MC8SHORTCHECK", rtMcList[5]));

                    string strTemp = rtMcList[0].ToString() + "," + rtMcList[1].ToString()
                         + "," + rtMcList[2].ToString() + "," + rtMcList[3].ToString()
                          + "," + rtMcList[4].ToString() + "," + rtMcList[5].ToString();
                    LogState(LogType.Info, "Result : ," + strTemp + ", (MC Short Check)");

                    rtMcList.Clear();
                }
                catch (Exception)
                {
                    LogState(LogType.Info, "Please MC ResistanceCable Check");
                }

                #region Cell DCIR
                // 방전 전류  mes 전류 값으로 처리 계산
                endCurrent_DIS = Convert.ToDouble(discCur);

                if (endCurrent_DIS < 0)
                    endCurrent_DIS = -endCurrent_DIS;

                // 1. Rest 
                Point_REST_BEF_DISC(ti);
                               
                // 2. Discharge
                Point_DISC(ti);

                // 3. Rest
                Point_REST_AFT_DISC(ti);

                // 4. Charge
                Point_CHAR(ti);

                // 5. Rest
                Point_REST_AFT_CHAR(ti);

                // Cycler Count Add
                SetCounter_Cycler();


                // 방전 전 전압
                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[0];

                // 방전 후 전압
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[0];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));

                //221108 MC 저항 상세 수집 nnkim
                //rest 221006 저항 측정한거 상세 수집
                //dp_McShortCheck = new Thread(new ParameterizedThreadStart(Point_Mc_Short_check));
                //dp_McShortCheck.Start(ti);

                /*
                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));
                */

                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);

                _DCIR.CellDcirList.Add(cellDCIR);
                ti.Value_ = cellDCIR;

                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

                SetContorlLimitData(15, ti.Value_.ToString());
                #endregion

                //200624
                SetMainVoltage("0.00");
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.StackTrace);
            }
         
            return JudgementTestItem(ti);
        }
        public bool Cell2_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count < 2) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)
                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }

                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[1];
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[1];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));
                /*
                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));
                */
                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);

                _DCIR.CellDcirList.Add(cellDCIR);
                ti.Value_ = cellDCIR;

                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

                SetContorlLimitData(16, ti.Value_.ToString());
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }
        public bool Cell3_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count < 2) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)
                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }

                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[2];
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[2];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));
                /*
                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));
                                      */
                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);

                _DCIR.CellDcirList.Add(cellDCIR);
                ti.Value_ = cellDCIR;

                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

                SetContorlLimitData(17, ti.Value_.ToString());
            }
            catch
            {

            }
            return JudgementTestItem(ti);
        }
        public bool Cell4_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count < 2) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)
                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }

                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[3];
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[3];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));
                /*
                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));
                */
                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);

                _DCIR.CellDcirList.Add(cellDCIR);
                ti.Value_ = cellDCIR;

                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

                SetContorlLimitData(18, ti.Value_.ToString());

            }
            catch
            {

            }
         
            return JudgementTestItem(ti);
        }
        public bool Cell5_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count < 2) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)
                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }

                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[4];
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[4];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));
                /*
                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));
                */
                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);
                _DCIR.CellDcirList.Add(cellDCIR);
                ti.Value_ = cellDCIR;

                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

                SetContorlLimitData(19, ti.Value_.ToString());
            }
            catch
            {

            }
         
            return JudgementTestItem(ti);
        }
        public bool Cell6_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count < 2) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)
                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }

                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[5];
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[5];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));

                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));

                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);

                _DCIR.CellDcirList.Add(cellDCIR);
                ti.Value_ = cellDCIR;

                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

                SetContorlLimitData(20, ti.Value_.ToString());
            }
            catch
            {

            }
       
            return JudgementTestItem(ti);
        }
        public bool Cell7_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count < 2) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)
                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }

                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[6];
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[6];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));

                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));

                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);

                _DCIR.CellDcirList.Add(cellDCIR);
                ti.Value_ = cellDCIR;

                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

                SetContorlLimitData(21, ti.Value_.ToString());
            }
            catch
            {

            }
          
            return JudgementTestItem(ti);
        }
        public bool Cell8_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count < 2) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)
                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }

                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[7];
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[7];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));

                /*
                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));
                */

                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);

                _DCIR.CellDcirList.Add(cellDCIR);
                ti.Value_ = cellDCIR;

                SetContorlLimitData(22, ti.Value_.ToString());

                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            }
            catch
            {

            }
         
            return JudgementTestItem(ti);
        }
        public bool Cell9_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count < 2) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)
                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }

                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[8];
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[8];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));

                /*
                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));
                */

                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);

                _DCIR.CellDcirList.Add(cellDCIR);
                ti.Value_ = cellDCIR;

                SetContorlLimitData(23, ti.Value_.ToString());


                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            }
            catch
            {

            }
          
            return JudgementTestItem(ti);
        }
        public bool Cell10_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count < 2) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)
                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }

                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[9];
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[9];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));

                /*
                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));
                */

                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);

                _DCIR.CellDcirList.Add(cellDCIR);
                ti.Value_ = cellDCIR;

                SetContorlLimitData(24, ti.Value_.ToString());

                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            }
            catch
            {

            }
            
            return JudgementTestItem(ti);
        }

        public bool Cell11_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count < 2) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)
                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }

                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[10];
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[10];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));
                /*
                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));
                                      */
                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);

                _DCIR.CellDcirList.Add(cellDCIR);
                ti.Value_ = cellDCIR;


                SetContorlLimitData(25, ti.Value_.ToString());


                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            }
            catch
            {

            }
            
            return JudgementTestItem(ti);
        }

        public bool Cell12_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.Cycler.GetDisChgPointList.Count < 2) // 계산에 필요한 전압 캐치 포인트는 2개이다
                {
                    // DICR 계산에 필요한 전압 캐치가 안됬을경우 Voltage Sengsing Fail 처리로 판정 문구 수정(기존 Test NG, 유경석 사원)
                    // 전압 캐치가 안된경우는 충/방전이 비정상이니깐 안될것이고
                    // 비정상인 이유에 대해선 여러가지 이유가 있을것임 (SW Safety 조건에 걸렸다던가, 충/방전기 문제라던가..등)
                    ti.Value_ = "Voltage Sengsing Fail";
                    return JudgementTestItem(ti);
                }

                double point1 = Device.Cycler.GetDisChgPointList[0].CellVoltageList[11];
                double point2 = Device.Cycler.GetDisChgPointList[1].CellVoltageList[11];

                // origin DCIR Value:
                double oriDCIR = _DCIR.GetOriginDCIR(point1, point2, endCurrent_DIS);

                LogState(LogType.TEST, "Origin DCIR Value:" +
                                        string.Format("({0} - {1}) / {2}) * 1000 = {3}",
                                        point1,
                                        point2,
                                        endCurrent_DIS,
                                        oriDCIR));

                /*
                LogState(LogType.TEST, string.Format("{0} + ({1} * ({2} - 23)) + {3}",
                                      oriDCIR,
                                      cellFomula1,
                                      _DCIR.ModuleTemp,
                                      cellFomula2));
                */

                // Cell DCIR
                string log = "";
                double cellDCIR = _DCIR.GetDCIR_Cal(oriDCIR, cellFomula1, cellFomula2, cellFomula3, _DCIR.ModuleTemp, out log);

                _DCIR.CellDcirList.Add(cellDCIR);
                
                ti.Value_ = cellDCIR;

                SetContorlLimitData(26, ti.Value_.ToString());

                LogState(LogType.TEST, log);
                LogState(LogType.TEST,
                            ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            }
            catch
            {

            }
      
            return JudgementTestItem(ti);
        }

        public bool Cell_DCIR_Deviation(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                ti.Value_ = _DCIR.GetCellDeviation();

                LogState(LogType.Info,
                            "DCIR Deviation - Max Cell DCIR :" + _DCIR.CellDcirList.Max() +
                            "/Min Cell DCIR :" + _DCIR.CellDcirList.Min() +
                            "/Result :" + ti.Value_.ToString());

                SetContorlLimitData(27, ti.Value_.ToString());
            }
            catch
            {

            }
            return JudgementTestItem(ti);
        }
        public bool Cell_DCIR_Ratio(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                ti.Value_ = _DCIR.GetCellRatio();

                //  MES 상세 수집 
                ti.refValues_.Add(MakeDetailItem(EOL.MesID.MaxCellDCIR,
                                                            _DCIR.CellDcirList.Max().ToString()));

                ti.refValues_.Add(MakeDetailItem(EOL.MesID.MinCellDCIR,
                                                             _DCIR.CellDcirList.Min().ToString()));

                SetContorlLimitData(28, ti.Value_.ToString());

                LogState(LogType.Info,
                                "DCIR Ratio - Max Cell DCIR," + _DCIR.CellDcirList.Max().ToString() +
                                "/Min Cell DCIR," + _DCIR.CellDcirList.Min().ToString() +
                                "/Result," + ti.Value_.ToString());
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }

        #region   //220906 Sigma Level 기능 추가 NNKIM

        public double cellDCIR_Offset1 = 0.0;
        public double cellDCIR_Offset2 = 0.0;
        public double cellDCIR_Offset3 = 0.0;
        public double cellDCIR_Offset4 = 0.0;
        public double cellDCIR_Offset5 = 0.0;
        public double cellDCIR_Offset6 = 0.0;
        public double cellDCIR_Offset7 = 0.0;
        public double cellDCIR_Offset8 = 0.0;
        public double cellDCIR_Offset9 = 0.0;
        public double cellDCIR_Offset10 = 0.0;
        public double cellDCIR_Offset11 = 0.0;
        public double cellDCIR_Offset12 = 0.0;
        public double cellDCIR_Offset13 = 0.0;
        public double cellDCIR_Offset14 = 0.0;

        public double standardDeviation(List<double> doubleList)
        {
            double average = doubleList.Average();
            double sum = doubleList.Sum(d => Math.Pow(d - average, 2));
            //return Math.Sqrt((sum) / (doubleList.Count()));
            return Math.Sqrt((sum) / (doubleList.Count() - 1));
        }

        public bool dcir_SigmaLevel_Lot_Check = true;

        public bool Cell1_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] + cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] + cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] + cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] + cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] + cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] + cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] + cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] + cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] + cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] + cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2} = {3}", dd[0], celldcir_avg, cellstandard, Math.Abs((dd[0] - celldcir_avg) / cellstandard)));
            ti.Value_ = ((dd[0] - celldcir_avg) / cellstandard);

            #region 220119 Cell DCIR Offset 상세수집항목 만들기
            string cellDCIROffset = "";

            cellDCIROffset =
            cellDCIR_Offset1.ToString() + "&" +
            cellDCIR_Offset2.ToString() + "&" +
            cellDCIR_Offset3.ToString() + "&" +
            cellDCIR_Offset4.ToString() + "&" +
            cellDCIR_Offset5.ToString() + "&" +
            cellDCIR_Offset6.ToString() + "&" +
            cellDCIR_Offset7.ToString() + "&" +
            cellDCIR_Offset8.ToString() + "&" +
            cellDCIR_Offset9.ToString() + "&" +
            cellDCIR_Offset10.ToString() + "&" +
            cellDCIR_Offset11.ToString() + "&" +
            cellDCIR_Offset12.ToString();

            //220906 상세수집에 필요한 ID 확인필요.
            ti.refValues_.Add(MakeDetailItem("WP2MMMTE2048", cellDCIROffset.ToString()));
            #endregion

            // 220409 12S > Cell Order Reverse
            if (localTypes == 0)
            {
                Device.MES.m_listMesCellVoltage.Reverse();
            }

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(1);

            return JudgementTestItem(ti);
        }

        public bool Cell2_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] + cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] + cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] + cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] + cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] + cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] + cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] + cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] + cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] + cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] + cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2}", dd[1], celldcir_avg, cellstandard));
            ti.Value_ = ((dd[1] - celldcir_avg) / cellstandard);

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(2);

            return JudgementTestItem(ti);
        }

        public bool Cell3_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] +  cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] +  cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] +  cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] +  cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] +  cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] +  cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] +  cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] +  cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] +  cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] +  cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2}", dd[2], celldcir_avg, cellstandard));
            ti.Value_ = ((dd[2] - celldcir_avg) / cellstandard);

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(3);

            return JudgementTestItem(ti);
        }

        public bool Cell4_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] + cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] + cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] + cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] + cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] + cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] + cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] + cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] + cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] + cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] + cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2}", dd[3], celldcir_avg, cellstandard));
            ti.Value_ = ((dd[3] - celldcir_avg) / cellstandard);

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(4);

            return JudgementTestItem(ti);
        }

        public bool Cell5_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] + cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] + cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] + cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] + cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] + cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] + cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] + cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] + cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] + cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] + cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2}", dd[4], celldcir_avg, cellstandard));
            ti.Value_ = ((dd[4] - celldcir_avg) / cellstandard);

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(5);

            return JudgementTestItem(ti);
        }

        public bool Cell6_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] + cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] + cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] + cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] + cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] + cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] + cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] + cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] + cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] + cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] + cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2}", dd[5], celldcir_avg, cellstandard));
            ti.Value_ = ((dd[5] - celldcir_avg) / cellstandard);

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(6);

            return JudgementTestItem(ti);
        }

        public bool Cell7_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] + cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] + cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] + cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] + cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] + cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] + cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] + cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] + cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] + cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] + cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2}", dd[6], celldcir_avg, cellstandard));
            ti.Value_ = ((dd[6] - celldcir_avg) / cellstandard);

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(7);

            return JudgementTestItem(ti);
        }

        public bool Cell8_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] + cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] + cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] + cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] + cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] + cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] + cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] + cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] + cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] + cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] + cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2}", dd[7], celldcir_avg, cellstandard));
            ti.Value_ = ((dd[7] - celldcir_avg) / cellstandard);

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(8);

            return JudgementTestItem(ti);
        }

        public bool Cell9_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] + cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] + cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] + cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] + cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] + cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] + cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] + cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] + cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] + cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] + cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2}", dd[8], celldcir_avg, cellstandard));
            ti.Value_ = ((dd[8] - celldcir_avg) / cellstandard);

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(9);

            return JudgementTestItem(ti);
        }

        public bool Cell10_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] + cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] + cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] + cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] + cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] + cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] + cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] + cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] + cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] + cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] + cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2}", dd[9], celldcir_avg, cellstandard));
            ti.Value_ = ((dd[9] - celldcir_avg) / cellstandard);

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(10);

            return JudgementTestItem(ti);
        }

        public bool Cell11_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] + cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] + cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] + cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] + cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] + cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] + cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] + cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] + cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] + cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] + cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2}", dd[10], celldcir_avg, cellstandard));
            ti.Value_ = ((dd[10] - celldcir_avg) / cellstandard);

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(11);

            return JudgementTestItem(ti);
        }

        public bool Cell12_DCIR_Sigma_Level(TestItem ti)
        {
            isProcessingUI(ti);

            var dd = new List<double>();

            //2p12s 모델인 경우
            dd.Add(_DCIR.CellDcirList[0] + cellDCIR_Offset1);
            dd.Add(_DCIR.CellDcirList[1] + cellDCIR_Offset2);
            dd.Add(_DCIR.CellDcirList[2] + cellDCIR_Offset3);
            dd.Add(_DCIR.CellDcirList[3] + cellDCIR_Offset4);
            dd.Add(_DCIR.CellDcirList[4] + cellDCIR_Offset5);
            dd.Add(_DCIR.CellDcirList[5] + cellDCIR_Offset6);
            dd.Add(_DCIR.CellDcirList[6] + cellDCIR_Offset7);
            dd.Add(_DCIR.CellDcirList[7] + cellDCIR_Offset8);
            dd.Add(_DCIR.CellDcirList[8] + cellDCIR_Offset9);
            dd.Add(_DCIR.CellDcirList[9] + cellDCIR_Offset10);
            dd.Add(_DCIR.CellDcirList[10] + cellDCIR_Offset11);
            dd.Add(_DCIR.CellDcirList[11] + cellDCIR_Offset12);

            double cellstandard = standardDeviation(dd);
            LogState(LogType.Info, "Cell DCIR StandardDeviation : " + cellstandard.ToString());

            double celldcir_avg = dd.Average();
            LogState(LogType.Info, "Cell DCIR Average : " + celldcir_avg.ToString());

            LogState(LogType.Info, string.Format("Result : ({0} - {1}) / {2}", dd[11], celldcir_avg, cellstandard));
            ti.Value_ = ((dd[11] - celldcir_avg) / cellstandard);

            dcir_SigmaLevel_Lot_Check = Cell_Lot_Judgement(12);

            return JudgementTestItem(ti);
        }

        public bool Cell_Lot_Judgement(int cell_Index)
        {
            bool sameCellLot = true;
            var cell_Lot_List = new List<string>();
            string thiscell = "";
            int xindex = 1;

            foreach (var item in Device.MES.m_listMesCellVoltage)
            {
                LogState(LogType.Info, xindex.ToString() + " Cell Lot : " + item.Key.ToString());
                string temp = item.Key.ToString().Substring(0, 4);
                LogState(LogType.Info, xindex.ToString() + " Cell Lot : " + temp);
                if (cell_Index == xindex)
                {
                    thiscell = temp;
                }
                else
                {
                    cell_Lot_List.Add(temp);
                }

                xindex++;
            }

            foreach (var item in cell_Lot_List)
            {
                // 같은 Lot이 있으면 판정대로 진행
                if (thiscell == item)
                {
                    return true;
                }
                // 같은 Lot이 없으면 NG를 Pass로 변경 해야함.
                else
                {
                    sameCellLot = false;
                }
            }
            return sameCellLot;
        }
        #endregion

        //MC 저항 기능 nnkim
        public void McResistanceMeasure(string strPosition, out List<string> list, bool bDelayResult = false)
        {
            if (bDelayResult)
            {
                //Mc 저항측정 딜레이시간 추가
                string regSubkey = "Software\\EOL_Trigger";
                int nDelay = 0;
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

                var regMCResistanceDelay = rk.GetValue("MC_Resistance_Delay") as string;

                if (regMCResistanceDelay == null) { rk.SetValue("MC_Resistance_Delay", "6000"); nDelay = 6000; }
                else { nDelay = int.Parse(regMCResistanceDelay); }

                Thread.Sleep(nDelay);
            }

            var rtList = new List<double>();
            list = new List<string>();
            double dResultTemp = 0.0;


            Device.KeysightDAQ.MeasRes_Multi(out rtList, BatteryInfo.ModuleResMC, 4, true);

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


                if (regStrOffset1ch == null) { rk.SetValue("MC3_OFFSET", "25.0"); dResultTemp = 25.0; }
                else { dResultTemp = double.Parse(regStrOffset1ch); rtList[0] = rtList[0] - dResultTemp; }

                if (regStrOffset2ch == null) { rk.SetValue("MC4_OFFSET", "0.0"); dResultTemp = 0.0; }
                else { dResultTemp = double.Parse(regStrOffset2ch); rtList[1] = rtList[1] - dResultTemp; }


                if (regStrOffset3ch == null) { rk.SetValue("MC5_OFFSET", "0.0"); dResultTemp = 0.0; }
                else { dResultTemp = double.Parse(regStrOffset3ch); rtList[2] = rtList[2] - dResultTemp; }

                if (regStrOffset4ch == null) { rk.SetValue("MC6_OFFSET", "0.0"); dResultTemp = 0.0; }
                else { dResultTemp = double.Parse(regStrOffset4ch); rtList[3] = rtList[3] - dResultTemp; }

                //if (regStrOffset5ch == null) { rk.SetValue("MC7_OFFSET", "0.0"); dResultTemp = 0.0; }
                //else { dResultTemp = double.Parse(regStrOffset4ch); rtList[2] = rtList[4] - dResultTemp; }

                //if (regStrOffset6ch == null) { rk.SetValue("MC8_OFFSET", "0.0"); dResultTemp = 0.0; }
                //else { dResultTemp = double.Parse(regStrOffset4ch); rtList[3] = rtList[5] - dResultTemp; }


                for (int i = 0; i < rtList.Count; i++)
                {
                    list.Add(rtList[i].ToString());
                    if (i == 3)
                    {
                        list.Add("Not Use");
                        list.Add("Not Use");
                    }
                }
            }
            catch
            {
                LogState(LogType.Info, "Please MC ResistanceCable Check");
            }
        }

        public string mcResistanceposition;

        public string McResistanceposition
        {
            get { return mcResistanceposition; }
            set { mcResistanceposition = value; }
        }

        //MC 저항 기능 nnkim
        void Point_Mc_Short_check(object aa)
        {
            Thread.Sleep(500);
            try
            {
                var ti = aa as TestItem;
                ti.refValues_.Add(MakeDetailItem("MC3SHORTCHECK", rtMcList[0]));
                ti.refValues_.Add(MakeDetailItem("MC4SHORTCHECK", rtMcList[1]));
                ti.refValues_.Add(MakeDetailItem("MC5SHORTCHECK", rtMcList[2]));
                ti.refValues_.Add(MakeDetailItem("MC6SHORTCHECK", rtMcList[3]));
                ti.refValues_.Add(MakeDetailItem("MC7SHORTCHECK", rtMcList[4]));
                ti.refValues_.Add(MakeDetailItem("MC8SHORTCHECK", rtMcList[5]));

                string strTemp = rtMcList[0].ToString() + "," + rtMcList[1].ToString()
                     + "," + rtMcList[2].ToString() + "," + rtMcList[3].ToString()
                      + "," + rtMcList[4].ToString() + "," + rtMcList[5].ToString();
                LogState(LogType.Info, "Result : ," + strTemp + ", (MC Short Check)");

                rtMcList.Clear();
            }
            catch (Exception)
            {
                LogState(LogType.Info, "Please MC ResistanceCable Check");
            }
        }

    }
}

