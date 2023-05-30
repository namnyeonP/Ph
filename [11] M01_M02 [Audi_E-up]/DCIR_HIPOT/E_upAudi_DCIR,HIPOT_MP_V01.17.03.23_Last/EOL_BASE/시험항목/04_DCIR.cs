using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
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
        bool judgementDAQ_CellVoltage(string header, double volt,TestItem ti)
        {
            if (cellVoltMaxSpec < volt || cellVoltMinSpec > volt)
            {
                LogState(LogType.Info, localTypes.ToString() + " - "+header+" :" + volt.ToString());
                ti.Value_ = _DAQ_VOLT_SENSING_FAIL;
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool Cell1_DCIR(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            #region Field Initialize
            //값 초기화
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


            modulePoint_RES_CHA = modulePoint_RES_DIS = 0.0;
            modulePoint_CHA = modulePoint_DIS = 0.0;
            endCurrent_CHA = endCurrent_DIS = 0.0;
            calAvg = 0;
            mTemp_DCIR = 0;

            currentPosition = 0;
            #endregion

            if (!cycler.isAlive1)
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }

            if (counter_Cycler_limit < counter_Cycler)
            {
                ti.Value_ = "LIMIT OF CYCLER COUNT";
                return JudgementTestItem(ti);
            }

            #region Cycler Safety case send
            if (autoModeThread != null && autoModeThread.IsAlive)
            {
                //phoenixon Rules : used current + 30A
                // -> used current + 5%(by jht, 2jh)

                #region Audi
                if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
                {
                    double voltOver = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey10]);
                    double voltUnder = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey11]);
                    double currOver = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey02]); //jgh 실제사용 전류값으로 변경

                    //200804 current 자릿수 맞춤
                    currOver = double.Parse(
                        (currOver + (currOver * 0.05)).ToString("N1"));
                    //currOver = double.Parse(
                    //    (currOver + (currOver * 0.05)).ToString("N3"));

                    //200730 전압상/하한도 자릿수 맞춤
                    //N3처리 - pjh / ht
                    voltOver = double.Parse(voltOver.ToString("N3"));
                    voltUnder = double.Parse(voltUnder.ToString("N3"));
                    LogState(LogType.Info, "Cycler safety - voltage decimal point 3");
                    LogState(LogType.Info, "Cycler safety - current decimal point 1");

                    //fix 13sec
                    int timeoutSec = 13;

                    if (!clsw.SetCyclerLimit(voltOver, voltUnder, currOver, timeoutSec))
                    {
                        ti.Value_ = _CYCLER_SAFETY;
                        return JudgementTestItem(ti);
                    }
                }
                #endregion

                #region Porsche

                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    double voltOver = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey10]);
                    double voltUnder = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey11]);
                    double currOver = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey02]); //jgh 실제사용 전류값으로 변경

                    //200804 current 자릿수 맞춤
                    currOver = double.Parse(
                        (currOver + (currOver * 0.05)).ToString("N1"));
                    //currOver = double.Parse(
                    //    (currOver + (currOver * 0.05)).ToString("N3"));

                    //200730 전압상/하한도 자릿수 맞춤
                    //N3처리 - pjh / ht
                    voltOver = double.Parse(voltOver.ToString("N3"));
                    voltUnder = double.Parse(voltUnder.ToString("N3"));
                    LogState(LogType.Info, "Cycler safety - voltage decimal point 3");
                    LogState(LogType.Info, "Cycler safety - current decimal point 1");

                    //fix 13sec
                    int timeoutSec = 13;

                    if (!clsw.SetCyclerLimit(voltOver, voltUnder, currOver, timeoutSec))
                    {
                        ti.Value_ = _CYCLER_SAFETY;
                        return JudgementTestItem(ti);
                    }
                }

                #endregion

                #region Maserati

                if (localTypes == ModelType.MASERATI_NORMAL)
                {
                    double voltOver = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey10]);
                    double voltUnder = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey11]);
                    double currOver = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey02]); //jgh 실제사용 전류값으로 변경

                    //200804 current 자릿수 맞춤
                    currOver = double.Parse(
                        (currOver + (currOver * 0.05)).ToString("N1"));
                    //currOver = double.Parse(
                    //    (currOver + (currOver * 0.05)).ToString("N3"));

                    //200730 전압상/하한도 자릿수 맞춤
                    //N3처리 - pjh / ht
                    voltOver = double.Parse(voltOver.ToString("N3"));
                    voltUnder = double.Parse(voltUnder.ToString("N3"));
                    LogState(LogType.Info, "Cycler safety - voltage decimal point 3");
                    LogState(LogType.Info, "Cycler safety - current decimal point 1");

                    //fix 13sec
                    int timeoutSec = 13;

                    if (!clsw.SetCyclerLimit(voltOver, voltUnder, currOver, timeoutSec))
                    {
                        ti.Value_ = _CYCLER_SAFETY;
                        return JudgementTestItem(ti);
                    }
                }

                #endregion

                //210312 wjs add pors fl
                #region Porsche Facelift

                if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    double voltOver = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey10]);
                    double voltUnder = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey11]);
                    double currOver = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey02]); //jgh 실제사용 전류값으로 변경

                    //200804 current 자릿수 맞춤
                    currOver = double.Parse(
                        (currOver + (currOver * 0.05)).ToString("N1"));
                    //currOver = double.Parse(
                    //    (currOver + (currOver * 0.05)).ToString("N3"));

                    //200730 전압상/하한도 자릿수 맞춤
                    //N3처리 - pjh / ht
                    voltOver = double.Parse(voltOver.ToString("N3"));
                    voltUnder = double.Parse(voltUnder.ToString("N3"));
                    LogState(LogType.Info, "Cycler safety - voltage decimal point 3");
                    LogState(LogType.Info, "Cycler safety - current decimal point 1");

                    //fix 13sec
                    int timeoutSec = 13;

                    if (!clsw.SetCyclerLimit(voltOver, voltUnder, currOver, timeoutSec))
                    {
                        ti.Value_ = _CYCLER_SAFETY;
                        return JudgementTestItem(ti);
                    }
                }

                #endregion

                //221101 wjs add mase m183
                #region Maserati M183

                if (localTypes == ModelType.MASERATI_M183_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    double voltOver = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey10]);
                    double voltUnder = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey11]);
                    double currOver = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey02]); //jgh 실제사용 전류값으로 변경

                    //200804 current 자릿수 맞춤
                    currOver = double.Parse(
                        (currOver + (currOver * 0.05)).ToString("N1"));
                    //currOver = double.Parse(
                    //    (currOver + (currOver * 0.05)).ToString("N3"));

                    //200730 전압상/하한도 자릿수 맞춤
                    //N3처리 - pjh / ht
                    voltOver = double.Parse(voltOver.ToString("N3"));
                    voltUnder = double.Parse(voltUnder.ToString("N3"));
                    LogState(LogType.Info, "Cycler safety - voltage decimal point 3");
                    LogState(LogType.Info, "Cycler safety - current decimal point 1");

                    //fix 13sec
                    int timeoutSec = 13;

                    if (!clsw.SetCyclerLimit(voltOver, voltUnder, currOver, timeoutSec))
                    {
                        ti.Value_ = _CYCLER_SAFETY;
                        return JudgementTestItem(ti);
                    }
                }

                #endregion

                #region E_UP
                if (localTypes == ModelType.E_UP)
                {
                    double voltOver = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey10]);
                    double voltUnder = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey11]);
                    double currOver = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey02]); //jgh 실제사용 전류값으로 변경

                    currOver = double.Parse(
                        (currOver + (currOver * 0.05)).ToString("N1"));

                    //200730 전압상/하한도 자릿수 맞춤
                    //N3처리 - pjh / ht
                    voltOver = double.Parse(voltOver.ToString("N3"));
                    voltUnder = double.Parse(voltUnder.ToString("N3"));
                    LogState(LogType.Info, "Cycler safety - voltage decimal point 3");
                    LogState(LogType.Info, "Cycler safety - current decimal point 1");

                    //fix 13sec
                    int timeoutSec = 13;

                    if (!clsw.SetCyclerLimit(voltOver, voltUnder, currOver, timeoutSec))
                    {
                        ti.Value_ = _CYCLER_SAFETY;
                        return JudgementTestItem(ti);
                    }
                }
                #endregion
            }
            #endregion

            #region Initialize Detail Items
            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_BEF_AMB_T));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_TEMP1_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_TEMP2_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_CVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_CV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_CVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_CV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_CVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_MVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_MV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_MVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_MV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_MVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_500MS_DIS));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_BEF_AMB_T));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_TEMP1_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_TEMP2_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_CVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_CV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_CVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_CV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_CVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_MVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_MV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_MVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_MV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_MVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_500MS_DIS));
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_BEF_AMB_T));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_TEMP1_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_TEMP2_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_CVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_CV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_CVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_CV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_CVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_MVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_MV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_MVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_MV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_MVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_500MS_DIS));
            }
            //210312 wjs add pors fl
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_BEF_AMB_T));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_TEMP1_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_TEMP2_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_TEMP3_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_CVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_CV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_CVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_CV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_CVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_MVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_MV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_MVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_MV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_MVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_500MS_DIS));                
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_BEF_AMB_T));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_TEMP1_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_TEMP2_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_TEMP3_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_CVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_CV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_CVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_CV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_CVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_MVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_MV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_MVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_MV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_MVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_500MS_DIS));
            }
            else if (localTypes == ModelType.E_UP)
            {
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_BEF_AMB_T));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_TEMP1_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_TEMP2_ADC));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_CVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_CV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_CVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_CV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_CVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_MVBEF_DIS));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_MV_DISCHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_MVBEF_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_MV_CHARGE));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_MVAFT_CHA));
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_500MS_DIS));
            }

            #endregion

            //200416 3. DCIR DAQ Voltage Sensing Fail 알람 추가
            //DCIR 진입 시 Barcode 검사항목에 있는 CMC/DAQ 연결성 진단 로직을 그대로 추가하고 그 다음 Cell Voltage 측정 진행하여 Cell 전압 정상 측정
            #region Check DAQ/CMC status before voltage check
            bool result = false;
            int retryCount = 0;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR
                || localTypes == ModelType.E_UP)
            {
                #region DAQ Check
                result = ConnectionCheck_DAQ(out retryCount);

                if (!result)
                {
                    ti.Value_ = _DAQ_VOLT_SENSING_FAIL;
                    return JudgementTestItem(ti);
                }
                #endregion
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )//210312 wjs add pors fl
            {
                #region CMC Check
                result = ConnectionCheck_CMC(out retryCount);

                if (!result)
                {
                    ti.Value_ = _DAQ_VOLT_SENSING_FAIL;
                    return JudgementTestItem(ti);
                }
                #endregion
            }
            #endregion

            //200225 jht, 2jh
            #region Check DAQ/CMC Voltages
            //9. 검사기 공통) 구간별 NG 코드 점검 필요 추가된부분
            if (localTypes == ModelType.AUDI_NORMAL)
            {
                if (!judgementDAQ_CellVoltage("Cell1", daq.DAQList[0], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell2", daq.DAQList[1], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell3", daq.DAQList[2], ti)) { return JudgementTestItem(ti); }
            }
            else if (localTypes == ModelType.AUDI_MIRROR)
            {
                if (!judgementDAQ_CellVoltage("Cell1", daq.DAQList[3], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell2", daq.DAQList[4], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell3", daq.DAQList[5], ti)) { return JudgementTestItem(ti); }
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )
            {
                if (!judgementDAQ_CellVoltage("Cell1", CMClist_Cell[0], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell2", CMClist_Cell[1], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell3", CMClist_Cell[2], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell4", CMClist_Cell[3], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell5", CMClist_Cell[4], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell6", CMClist_Cell[5], ti)) { return JudgementTestItem(ti); }
            }
            else if (localTypes == ModelType.E_UP)
            {
                if (!judgementDAQ_CellVoltage("Cell1", daq.DAQList[6], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell2", daq.DAQList[7], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell3", daq.DAQList[8], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell4", daq.DAQList[9], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell5", daq.DAQList[10], ti)) { return JudgementTestItem(ti); }
                if (!judgementDAQ_CellVoltage("Cell6", daq.DAQList[11], ti)) { return JudgementTestItem(ti); }
            }

            #endregion

            if (!ModeSet_Release(ti))
            {
                return false;
            }

            if (!ModeSet(ti))
            {
                return false;
            }


            #region Predict test

            #endregion


            ti.refValues_.Clear();

            double calTemp1 = 0;
            double calTemp2 = 0;
            double calTemp3 = 0;

            endCurrent_DIS = 0.0;
            var cellFomula1 = 0.0;
            var cellFomula2 = 0.0;
            var cellFomula3 = 0.0;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                endCurrent_DIS = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey02]);

                cellFomula1 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey17]);

                var realTemp = temps.tempStr.ToString();
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_BEF_AMB_T, realTemp));
                LogState(LogType.Info, "Ambient Temp - " + realTemp);

                var dmmChInfo = new DMMChannelInfo(localTypes);

                var resList = new List<double>();
                if (!keysight.MeasRes_Multi(out resList, dmmChInfo.ModuleRes, 2))
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }

                if (resList[0] > 9.9E+30)
                {
                    resList[0] = 99999;
                }
                if (resList[1] > 9.9E+30)
                {
                    resList[1] = 99999;
                }

                var tempRes1 = resList[0] * 0.001;
                var tempRes2 = resList[1] * 0.001;

                calTemp1 = GetTempToResist_Audi(tempRes1);
                LogState(LogType.Info, "Thermistor Check 1 - Temp:" + tempRes1.ToString());
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_TEMP1_ADC, calTemp1.ToString()));

                calTemp2 = GetTempToResist_Audi(tempRes2);
                LogState(LogType.Info, "Thermistor Check 2 - Temp :" + tempRes2.ToString());
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_DCIR_TEMP2_ADC, calTemp2.ToString()));

            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                endCurrent_DIS = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey02]);

                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey13]);

                var realTemp = temps.tempStr.ToString();
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_BEF_AMB_T, realTemp));
                LogState(LogType.Info, "Ambient Temp - " + realTemp);

                var rs12 = 0.0;
                var adc = CMClist[12];
                cmc.CalculateToMC33772ModuleTemp(adc, out rs12);
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_TEMP1_ADC, rs12.ToString()));
                LogState(LogType.Info, "Thermistor Check 1 - ADC :" + adc.ToString());
                LogState(LogType.Info, "Thermistor Check 1 - Temp:" + rs12.ToString());
                calTemp1 = rs12;

                //Temperature2_DCIR Item

                var rs13 = 0.0;
                var adc1 = CMClist[13];
                cmc.CalculateToMC33772ModuleTemp(adc1, out rs13);
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DCIR_TEMP2_ADC, rs13.ToString()));
                LogState(LogType.Info, "Thermistor Check 2 - ADC :" + adc1.ToString());
                LogState(LogType.Info, "Thermistor Check 2 - Temp :" + rs13.ToString());
                calTemp2 = rs13;

            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                endCurrent_DIS = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey02]);

                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey13]);

                var realTemp = temps.tempStr.ToString();
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_BEF_AMB_T, realTemp));
                LogState(LogType.Info, "Ambient Temp - " + realTemp);

                var rs12 = 0.0;
                var adc = CMClist[12];
                cmc.CalculateToMC33772ModuleTemp(adc, out rs12);
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_TEMP1_ADC, rs12.ToString()));
                LogState(LogType.Info, "Thermistor Check 1 - ADC :" + adc.ToString());
                LogState(LogType.Info, "Thermistor Check 1 - Temp:" + rs12.ToString());
                calTemp1 = rs12;

                //Temperature2_DCIR Item

                var rs13 = 0.0;
                var adc1 = CMClist[13];
                cmc.CalculateToMC33772ModuleTemp(adc1, out rs13);
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DCIR_TEMP2_ADC, rs13.ToString()));
                LogState(LogType.Info, "Thermistor Check 2 - ADC :" + adc1.ToString());
                LogState(LogType.Info, "Thermistor Check 2 - Temp :" + rs13.ToString());
                calTemp2 = rs13;
            }
            //210312 wjs add pors fl
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                endCurrent_DIS = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey02]);

                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey13]);

                var realTemp = temps.tempStr.ToString();
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_BEF_AMB_T, realTemp));
                LogState(LogType.Info, "Ambient Temp - " + realTemp);

                var rs12 = 0.0;
                var adc = CMClist[12];
                cmc.CalculateToMC33772ModuleTemp(adc, out rs12);
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_TEMP1_ADC, rs12.ToString()));
                LogState(LogType.Info, "Thermistor Check 1 - ADC :" + adc.ToString());
                LogState(LogType.Info, "Thermistor Check 1 - Temp:" + rs12.ToString());
                calTemp1 = rs12;

                //Temperature2_DCIR Item

                var rs13 = 0.0;
                var adc1 = CMClist[13];
                cmc.CalculateToMC33772ModuleTemp(adc1, out rs13);
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_TEMP2_ADC, rs13.ToString()));
                LogState(LogType.Info, "Thermistor Check 2 - ADC :" + adc1.ToString());
                LogState(LogType.Info, "Thermistor Check 2 - Temp :" + rs13.ToString());
                calTemp2 = rs13;

                //Temperature3_DCIR Item
                //220314 Porsche FL 사용안함 
                //var rs14 = 0.0;
                //var adc2 = CMClist[11];
                //cmc.CalculateToMC33772ModuleTemp(adc2, out rs14);
                //ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_TEMP3_ADC, rs14.ToString()));
                //LogState(LogType.Info, "Thermistor Check 3 - ADC :" + adc2.ToString());
                //LogState(LogType.Info, "Thermistor Check 3 - Temp :" + rs14.ToString());
                //calTemp3 = rs14;
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                endCurrent_DIS = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey02]);

                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey13]);

                var realTemp = temps.tempStr.ToString();
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_BEF_AMB_T, realTemp));
                LogState(LogType.Info, "Ambient Temp - " + realTemp);

                var rs12 = 0.0;
                var adc = CMClist[12];
                cmc.CalculateToMC33772ModuleTemp(adc, out rs12);
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_TEMP1_ADC, rs12.ToString()));
                LogState(LogType.Info, "Thermistor Check 1 - ADC :" + adc.ToString());
                LogState(LogType.Info, "Thermistor Check 1 - Temp:" + rs12.ToString());
                calTemp1 = rs12;

                //Temperature2_DCIR Item

                var rs13 = 0.0;
                var adc1 = CMClist[13];
                cmc.CalculateToMC33772ModuleTemp(adc1, out rs13);
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DCIR_TEMP2_ADC, rs13.ToString()));
                LogState(LogType.Info, "Thermistor Check 2 - ADC :" + adc1.ToString());
                LogState(LogType.Info, "Thermistor Check 2 - Temp :" + rs13.ToString());
                calTemp2 = rs13;

                //221101 wjs M183도 Temp3은 아디오스 사요나라
                //Temperature3_DCIR Item
                //220314 Porsche FL 사용안함 
                //var rs14 = 0.0;
                //var adc2 = CMClist[11];
                //cmc.CalculateToMC33772ModuleTemp(adc2, out rs14);
                //ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DCIR_TEMP3_ADC, rs14.ToString()));
                //LogState(LogType.Info, "Thermistor Check 3 - ADC :" + adc2.ToString());
                //LogState(LogType.Info, "Thermistor Check 3 - Temp :" + rs14.ToString());
                //calTemp3 = rs14;
            }
            else if (localTypes == ModelType.E_UP)
            {
                endCurrent_DIS = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey02]);

                cellFomula1 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey17]);

                var realTemp = temps.tempStr.ToString();
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_BEF_AMB_T, realTemp));
                LogState(LogType.Info, "Ambient Temp - " + realTemp);

                var dmmChInfo = new DMMChannelInfo(localTypes);

                var resList = new List<double>();
                if (!keysight.MeasRes_Multi(out resList, dmmChInfo.ModuleRes, 2))
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }

                if (resList[0] > 9.9E+30)
                {
                    resList[0] = 99999;
                }
                if (resList[1] > 9.9E+30)
                {
                    resList[1] = 99999;
                }

                var tempRes1 = resList[0] * 0.001;
                var tempRes2 = resList[1] * 0.001;

                calTemp1 = GetTempToResist_Audi(tempRes1);
                LogState(LogType.Info, "Thermistor Check 1 - Temp:" + tempRes1.ToString());
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_TEMP1_ADC, calTemp1.ToString()));

                calTemp2 = GetTempToResist_Audi(tempRes2);
                LogState(LogType.Info, "Thermistor Check 2 - Temp :" + tempRes2.ToString());
                ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_DCIR_TEMP2_ADC, calTemp2.ToString()));

            }

            //220314 Porsche FL 사용안함 
            ////210312 WJS ADD PORS FL 얘는 서미스터가 3개여...
            //if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            //{
            //    mTemp_DCIR = (calTemp1 + calTemp2 + calTemp3) / 3;
            //}
            //else
            {
                mTemp_DCIR = (calTemp1 + calTemp2) / 2;
            }
            LogState(LogType.Info, "Thermistor Average : " + mTemp_DCIR.ToString());


            #region DCIR

            cycler.isCyclerStop = false;
            counter_Cycler++;

            if (!Do_Rest_Charge(ti))
            {
                return false;
            }

            SetMainVoltage("0.00");
            SetMainCurrent("0.00");
            SetMainCState("Ready");

            SetCounter_Cycler();

            #endregion

            if (!ModeSet_Release(ti))
            {
                return false;
            }


            var point1 = cellDetailList[0].CellVolt_1;
            var point2 = cellDetailList[1].CellVolt_1;
            tempdcircell1_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, "origin DCIR Value:" + string.Format("( ({0} - {1}) / {2} ) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell1_DIS));

            var originDCIRVal = tempdcircell1_DIS;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR
                || localTypes == ModelType.E_UP)
            {
                LogState(LogType.TEST, string.Format("{0} + ({1}) * ({2} - 23) + ({3}) * ({4} - 23)^2 + ({5}) * ({6} - 23)^3",
                            originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR, cellFomula3, mTemp_DCIR));

                ti.Value_ = tempdcircell1_DIS = (originDCIRVal + cellFomula1 * (mTemp_DCIR - 23) + cellFomula2 * Math.Pow((mTemp_DCIR - 23), 2) + cellFomula3 * Math.Pow((mTemp_DCIR - 23), 3));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
              || localTypes == ModelType.MASERATI_NORMAL)
            {
                //200911 code refactoring
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + (({3}) * ({4} - 23)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell1_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 23))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 23), 2)));
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell1_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            //221101 wjs add mase m183
            // 같이 엮어도 될거 같긴한데...그래도 따로 가즈아
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell1_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }


            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        public bool Cell2_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            var cellFomula1 = 0.0;
            var cellFomula2 = 0.0;
            var cellFomula3 = 0.0;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey17]);
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey13]);
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey13]);
            }
            //210312 wjs add pors fl
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey13]);
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey13]);
            }
            else if (localTypes == ModelType.E_UP)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey17]);
            }

            var point1 = cellDetailList[0].CellVolt_2;
            var point2 = cellDetailList[1].CellVolt_2;
            tempdcircell2_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, "origin DCIR Value:" + string.Format("( ({0} - {1}) / {2} ) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell2_DIS));
            var originDCIRVal = tempdcircell2_DIS;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR
                || localTypes == ModelType.E_UP)
            {
                LogState(LogType.TEST, string.Format("{0} + ({1}) * ({2} - 23) + ({3}) * ({4} - 23)^2 + ({5}) * ({6} - 23)^3",
                    originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR, cellFomula3, mTemp_DCIR));
                ti.Value_ = tempdcircell2_DIS = (originDCIRVal + cellFomula1 * (mTemp_DCIR - 23) + cellFomula2 * Math.Pow((mTemp_DCIR - 23), 2) + cellFomula3 * Math.Pow((mTemp_DCIR - 23), 3));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
              || localTypes == ModelType.MASERATI_NORMAL)
            {
                //200911 code refactoring
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + (({3}) * ({4} - 23)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell2_DIS = (originDCIRVal + ((cellFomula1) * (mTemp_DCIR - 23))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 23), 2)));
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell2_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell2_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }

            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        public bool Cell3_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            var cellFomula1 = 0.0;
            var cellFomula2 = 0.0;
            var cellFomula3 = 0.0;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey17]);
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey13]);
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey13]);
            }
            //210312 wjs add pors fl
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey13]);
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey13]);
            }
            else if (localTypes == ModelType.E_UP)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey17]);
            }

            var point1 = cellDetailList[0].CellVolt_3;
            var point2 = cellDetailList[1].CellVolt_3;
            tempdcircell3_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, "origin DCIR Value:" + string.Format("( ({0} - {1}) / {2} ) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell3_DIS));
            var originDCIRVal = tempdcircell3_DIS;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR
                || localTypes == ModelType.E_UP)
            {
                LogState(LogType.TEST, string.Format("{0} + ({1}) * ({2} - 23) + ({3}) * ({4} - 23)^2 + ({5}) * ({6} - 23)^3",
                    originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR, cellFomula3, mTemp_DCIR));
                ti.Value_ = tempdcircell3_DIS = (originDCIRVal + cellFomula1 * (mTemp_DCIR - 23) + cellFomula2 * Math.Pow((mTemp_DCIR - 23), 2) + cellFomula3 * Math.Pow((mTemp_DCIR - 23), 3));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                   || localTypes == ModelType.MASERATI_NORMAL)
            {
                //200911 code refactoring
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + (({3}) * ({4} - 23)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell3_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 23))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 23), 2)));
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell3_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell3_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }

            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        public bool Cell4_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            var cellFomula1 = 0.0;
            var cellFomula2 = 0.0;
            var cellFomula3 = 0.0;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey17]);
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey13]);
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey13]);
            }
            //210312 wjs add pors fl
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey13]);
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey13]);
            }
            else if (localTypes == ModelType.E_UP)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey17]);
            }

            var point1 = cellDetailList[0].CellVolt_4;
            var point2 = cellDetailList[1].CellVolt_4;
            tempdcircell4_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, "origin DCIR Value:" + string.Format("( ({0} - {1}) / {2} ) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell4_DIS));
            var originDCIRVal = tempdcircell4_DIS;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR
                || localTypes == ModelType.E_UP)
            {
                LogState(LogType.TEST, string.Format("{0} + ({1}) * ({2} - 23) + ({3}) * ({4} - 23)^2 + ({5}) * ({6} - 23)^3",
                    originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR, cellFomula3, mTemp_DCIR));
                ti.Value_ = tempdcircell4_DIS = (originDCIRVal + cellFomula1 * (mTemp_DCIR - 23) + cellFomula2 * Math.Pow((mTemp_DCIR - 23), 2) + cellFomula3 * Math.Pow((mTemp_DCIR - 23), 3));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                   || localTypes == ModelType.MASERATI_NORMAL)
            {
                //200911 code refactoring
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + (({3}) * ({4} - 23)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell4_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 23))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 23), 2)));
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell4_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell4_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        public bool Cell5_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            var cellFomula1 = 0.0;
            var cellFomula2 = 0.0;
            var cellFomula3 = 0.0;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey17]);
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey13]);
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey13]);
            }
            //210312 wjs add pors fl
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey13]);
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey13]);
            }
            else if (localTypes == ModelType.E_UP)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey17]);
            }

            var point1 = cellDetailList[0].CellVolt_5;
            var point2 = cellDetailList[1].CellVolt_5;
            tempdcircell5_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, "origin DCIR Value:" + string.Format("( ({0} - {1}) / {2} ) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell5_DIS));
            var originDCIRVal = tempdcircell5_DIS;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR
                || localTypes == ModelType.E_UP)
            {
                LogState(LogType.TEST, string.Format("{0} + ({1}) * ({2} - 23) + ({3}) * ({4} - 23)^2 + ({5}) * ({6} - 23)^3",
                    originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR, cellFomula3, mTemp_DCIR));
                ti.Value_ = tempdcircell5_DIS = (originDCIRVal + cellFomula1 * (mTemp_DCIR - 23) + cellFomula2 * Math.Pow((mTemp_DCIR - 23), 2) + cellFomula3 * Math.Pow((mTemp_DCIR - 23), 3));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                   || localTypes == ModelType.MASERATI_NORMAL)
            {
                //200911 code refactoring
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + (({3}) * ({4} - 23)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell5_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 23))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 23), 2)));
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell5_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell5_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        public bool Cell6_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            var cellFomula1 = 0.0;
            var cellFomula2 = 0.0;
            var cellFomula3 = 0.0;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey17]);
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey13]);
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey13]);
            }
            //210312 wjs add pors fl
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey13]);
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey13]);
            }
            else if (localTypes == ModelType.E_UP)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey17]);
            }

            var point1 = cellDetailList[0].CellVolt_6;
            var point2 = cellDetailList[1].CellVolt_6;
            tempdcircell6_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, "origin DCIR Value:" + string.Format("( ({0} - {1}) / {2} ) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell6_DIS));
            var originDCIRVal = tempdcircell6_DIS;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR
                || localTypes == ModelType.E_UP)
            {
                LogState(LogType.TEST, string.Format("{0} + ({1}) * ({2} - 23) + ({3}) * ({4} - 23)^2 + ({5}) * ({6} - 23)^3",
                    originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR, cellFomula3, mTemp_DCIR));
                ti.Value_ = tempdcircell6_DIS = (originDCIRVal + cellFomula1 * (mTemp_DCIR - 23) + cellFomula2 * Math.Pow((mTemp_DCIR - 23), 2) + cellFomula3 * Math.Pow((mTemp_DCIR - 23), 3));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                   || localTypes == ModelType.MASERATI_NORMAL)
            {
                //200911 code refactoring
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + (({3}) * ({4} - 23)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell6_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 23))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 23), 2)));
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell6_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                ti.Value_ = tempdcircell6_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        public bool Module_DCIR(TestItem ti)
        {
            isProcessingUI(ti);

            var moduleFomula1 = 0.0;
            var moduleFomula2 = 0.0;
            var moduleFomula3 = 0.0;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                moduleFomula1 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey14]);
                moduleFomula2 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey15]);
                moduleFomula3 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey16]);
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                moduleFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey14]);
                moduleFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey15]);
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                moduleFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey14]);
                moduleFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey15]);
            }
            //210312 wjs add PORSCHE FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                moduleFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey14]);
                moduleFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey15]);
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                moduleFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey14]);
                moduleFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey15]);
            }
            else if (localTypes == ModelType.E_UP)
            {
                moduleFomula1 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey14]);
                moduleFomula2 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey15]);
                moduleFomula3 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey16]);
            }

            var point1 = modulePoint_RES_DIS;
            var point2 = modulePoint_DIS;
            tempdcirmodule_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, "origin DCIR Value:" + string.Format("( ({0} - {1}) / {2} ) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcirmodule_DIS));
            var originDCIRVal = tempdcirmodule_DIS;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR
                || localTypes == ModelType.E_UP)
            {
                LogState(LogType.TEST, string.Format("{0} + ({1}) * ({2} - 23) + ({3}) * ({4} - 23)^2 + ({5}) * ({6} - 23)^3",
                    originDCIRVal, moduleFomula1, mTemp_DCIR, moduleFomula2, mTemp_DCIR, moduleFomula3, mTemp_DCIR));
                ti.Value_ = tempdcirmodule_DIS = (originDCIRVal + moduleFomula1 * (mTemp_DCIR - 23) + moduleFomula2 * Math.Pow((mTemp_DCIR - 23), 2) + moduleFomula3 * Math.Pow((mTemp_DCIR - 23), 3));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
              || localTypes == ModelType.MASERATI_NORMAL)//210312 wjs add pors fl
            {
                //200911 code refactoring
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + (({3}) * ({4} - 23)^2)", originDCIRVal, moduleFomula1, mTemp_DCIR, moduleFomula2, mTemp_DCIR));
                ti.Value_ = tempdcirmodule_DIS = (originDCIRVal + (moduleFomula1 * (mTemp_DCIR - 23))) + ((moduleFomula2) * (Math.Pow((mTemp_DCIR - 23), 2)));
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, moduleFomula1, mTemp_DCIR, moduleFomula2, mTemp_DCIR));
                ti.Value_ = tempdcirmodule_DIS = (originDCIRVal + (moduleFomula1 * (mTemp_DCIR - 25))) + ((moduleFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, moduleFomula1, mTemp_DCIR, moduleFomula2, mTemp_DCIR));
                ti.Value_ = tempdcirmodule_DIS = (originDCIRVal + (moduleFomula1 * (mTemp_DCIR - 25))) + ((moduleFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            return JudgementTestItem(ti);
        }

        public bool Cell_DCIR_Ratio(TestItem ti)
        {
            isProcessingUI(ti);
            var list = new List<double>();

            #region Cell DCIR 1번 다시 구하기

            var cellFomula1 = 0.0;
            var cellFomula2 = 0.0;
            var cellFomula3 = 0.0;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey17]);
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)//201217 wjs delete maserati
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_EOL_ControlKey13]);
            }
            //201217 wjs add maserati
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_EOL_ControlKey13]);
            }
            //210312 wjs add pors fl
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[PORS_FL_EOL_ControlKey13]);
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[MAS_M183_EOL_ControlKey13]);
            }
            else if (localTypes == ModelType.E_UP)
            {
                cellFomula1 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey12]);
                cellFomula2 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey13]);
                cellFomula3 = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey17]);
            }

            var point1 = cellDetailList[0].CellVolt_1;
            var point2 = cellDetailList[1].CellVolt_1;
            tempdcircell1_DIS = Math.Abs(((point2 - point1) / (endCurrent_DIS)) * 1000);
            LogState(LogType.TEST, "origin DCIR Value:" + string.Format("( ({0} - {1}) / {2} ) * 1000 = {3}", point1, point2, endCurrent_DIS,
                tempdcircell1_DIS));
            var originDCIRVal = tempdcircell1_DIS;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR
                || localTypes == ModelType.E_UP)
            {
                LogState(LogType.TEST, string.Format("{0} + ({1}) * ({2} - 23) + ({3}) * ({4} - 23)^2 + ({5}) * ({6} - 23)^3",
                    originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR, cellFomula3, mTemp_DCIR));
                 tempdcircell1_DIS = (originDCIRVal + cellFomula1 * (mTemp_DCIR - 23) + cellFomula2 * Math.Pow((mTemp_DCIR - 23), 2) + cellFomula3 * Math.Pow((mTemp_DCIR - 23), 3));
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
              || localTypes == ModelType.MASERATI_NORMAL)
            {
                //200911 code refactoring
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 23))) + (({3}) * ({4} - 23)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                 tempdcircell1_DIS = (originDCIRVal + ((cellFomula1) * (mTemp_DCIR - 23))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 23), 2)));
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                 tempdcircell1_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                //210331  - 내용 : DCIR 온도 보정식 내 기준 온도 변경(23->25)
                LogState(LogType.TEST, string.Format("({0} + ({1} * ({2} - 25))) + (({3}) * ({4} - 25)^2)", originDCIRVal, cellFomula1, mTemp_DCIR, cellFomula2, mTemp_DCIR));
                tempdcircell1_DIS = (originDCIRVal + (cellFomula1 * (mTemp_DCIR - 25))) + ((cellFomula2) * (Math.Pow((mTemp_DCIR - 25), 2)));
            }
            LogState(LogType.TEST, ti.Name + " - Point2:" + point2.ToString() + ", Point1:" + point1.ToString() + ", EndCurrent:" + endCurrent_DIS.ToString());

            #endregion

            list.Add(tempdcircell1_DIS);
            list.Add(tempdcircell2_DIS);
            list.Add(tempdcircell3_DIS);

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
              || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
              || localTypes == ModelType.MASERATI_M183_NORMAL   //221101 wjs add mase m183
              || localTypes == ModelType.E_UP)
            {
                list.Add(tempdcircell4_DIS);
                list.Add(tempdcircell5_DIS);
                list.Add(tempdcircell6_DIS);
            }

            var max = list.Max();
            var min = list.Min();

            ti.Value_ = (max / min * 100);

            LogState(LogType.Info,
                string.Format("( {0} / {1} ) * 100 = {2}", max, min, ti.Value_.ToString()));
            return JudgementTestItem(ti);
        }

        public bool Cell_DCIR_Deviation(TestItem ti)
        {
            isProcessingUI(ti);

            var list = new List<double>();
            list.Add(tempdcircell1_DIS);
            list.Add(tempdcircell2_DIS);
            list.Add(tempdcircell3_DIS);

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
              || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
              || localTypes == ModelType.MASERATI_M183_NORMAL   //221101 wjs add mase m183
              || localTypes == ModelType.E_UP)
            {
                list.Add(tempdcircell4_DIS);
                list.Add(tempdcircell5_DIS);
                list.Add(tempdcircell6_DIS);
            }

            var max = list.Max();
            var min = list.Min();

            ti.Value_ = max - min;

            //ti.refValues_.Add(MakeDetailItem("WM5MTE212", max.ToString("N3")));
            //ti.refValues_.Add(MakeDetailItem("WM5MTE213", min.ToString("N3")));

            LogState(LogType.Info,
                string.Format("{0} - {1} = {2}", max, min, ti.Value_.ToString()));

            return JudgementTestItem(ti);
        }

        // 211006
        public bool DCIR_Inner_Verification(TestItem ti)
        {
            isProcessingUI(ti);

            #region Cell DCIR Value Check

            var allCellDCIR = new List<double>();

            allCellDCIR.Add(tempdcircell1_DIS);
            allCellDCIR.Add(tempdcircell2_DIS);
            allCellDCIR.Add(tempdcircell3_DIS);
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
              || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
              || localTypes == ModelType.MASERATI_M183_NORMAL   //221101 wjs add mase m183
              || localTypes == ModelType.E_UP)
            {
                allCellDCIR.Add(tempdcircell4_DIS);
                allCellDCIR.Add(tempdcircell5_DIS);
                allCellDCIR.Add(tempdcircell6_DIS);
            }

            int i = 0;
            foreach (var item in allCellDCIR)
            {
                LogState(LogType.Info, (i + 1).ToString() + " Cell DCIR = " + item.ToString());
                if (item == 0)
                {
                    LogState(LogType.Fail, "Cell DCIR Value is null");
                    return JudgementTestItem(ti);
                }
                i++;
            }

            #endregion

            var outCellDCIR = new List<double>();
            var inCellDCIR = new List<double>();

            double max_outCellDCIR = 0.0;
            double max_inCellDCIR = 0.0;

            // Audi 일때 (4P3S)
            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                // Max(CellDCIR Bank1,3) - Max(CellDCIR Bank2)

                outCellDCIR.Add(tempdcircell1_DIS);
                outCellDCIR.Add(tempdcircell3_DIS);

                max_outCellDCIR = outCellDCIR.Max();
                max_inCellDCIR = tempdcircell2_DIS;

                LogState(LogType.Info, "outCellDCIR = " + max_outCellDCIR.ToString());
                LogState(LogType.Info, "inCellDCIR = " + max_inCellDCIR.ToString());
                LogState(LogType.Info, max_outCellDCIR.ToString() + " - " + max_inCellDCIR.ToString() + " = " + (max_outCellDCIR - max_inCellDCIR).ToString());
            }
            // Pors N,M / Maserati / Pors FaceLift N,M (2P12S)
            else if (
                localTypes == ModelType.PORSCHE_NORMAL || 
                localTypes == ModelType.PORSCHE_MIRROR || 
                localTypes == ModelType.MASERATI_NORMAL || 
                localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || 
                localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                || localTypes == ModelType.MASERATI_M183_NORMAL  //221101 wjs add mase m183
              )
            {
                // Max(CellDCIR Bank1,2,5,6) - Max(CellDCIR Bank3,4)

                outCellDCIR.Add(tempdcircell1_DIS);
                outCellDCIR.Add(tempdcircell2_DIS);
                outCellDCIR.Add(tempdcircell5_DIS);
                outCellDCIR.Add(tempdcircell6_DIS);

                inCellDCIR.Add(tempdcircell3_DIS);
                inCellDCIR.Add(tempdcircell4_DIS);

                max_outCellDCIR = outCellDCIR.Max();
                max_inCellDCIR = inCellDCIR.Max();

                LogState(LogType.Info, "outCellDCIR = " + max_outCellDCIR.ToString());
                LogState(LogType.Info, "inCellDCIR = " + max_inCellDCIR.ToString());
                LogState(LogType.Info, max_outCellDCIR.ToString() + " - " + max_inCellDCIR.ToString() + 
                                       " = " + (max_outCellDCIR - max_inCellDCIR).ToString());
            }
            // e-UP (2P12S)
            else if (localTypes == ModelType.E_UP)
            {
                // Max(CellDCIR Bank1,2,5,6) - Max(CellDCIR Bank3,4)

                outCellDCIR.Add(tempdcircell1_DIS);
                outCellDCIR.Add(tempdcircell2_DIS);
                outCellDCIR.Add(tempdcircell5_DIS);
                outCellDCIR.Add(tempdcircell6_DIS);

                inCellDCIR.Add(tempdcircell3_DIS);
                inCellDCIR.Add(tempdcircell4_DIS);

                max_outCellDCIR = outCellDCIR.Max();
                max_inCellDCIR = inCellDCIR.Max();

                LogState(LogType.Info, "outCellDCIR = " + max_outCellDCIR.ToString());
                LogState(LogType.Info, "inCellDCIR = " + max_inCellDCIR.ToString());
                LogState(LogType.Info, max_outCellDCIR.ToString() + " - " + max_inCellDCIR.ToString() + 
                                       " = " + (max_outCellDCIR - max_inCellDCIR).ToString());
            }

            ti.Value_ = max_outCellDCIR - max_inCellDCIR;

            return JudgementTestItem(ti);
        }

        // 211006
        public bool DCIR_Outer_Verification(TestItem ti)
        {
            isProcessingUI(ti);

            #region Cell DCIR Value Check

            var allCellDCIR = new List<double>();

            allCellDCIR.Add(tempdcircell1_DIS);
            allCellDCIR.Add(tempdcircell2_DIS);
            allCellDCIR.Add(tempdcircell3_DIS);
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
              || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
              || localTypes == ModelType.MASERATI_M183_NORMAL   //221101 wjs add mase m183
              || localTypes == ModelType.E_UP)
            {
                allCellDCIR.Add(tempdcircell4_DIS);
                allCellDCIR.Add(tempdcircell5_DIS);
                allCellDCIR.Add(tempdcircell6_DIS);
            }

            int i = 0;
            foreach (var item in allCellDCIR)
            {
                LogState(LogType.Info, (i + 1).ToString() + " Cell DCIR = " + item.ToString());
                if (item == 0)
                {
                    LogState(LogType.Fail, "Cell DCIR Value is null");
                    return JudgementTestItem(ti);
                }
                i++;
            }
            #endregion

            var outCellDCIR = new List<double>();
            var inCellDCIR = new List<double>();

            double max_outCellDCIR = 0.0;
            double min_outCellDCIR = 0.0;

            // Audi 일때
            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                // Max(CellDCIR Bank1,3) - Min(CellDCIR Bank1,3)

                outCellDCIR.Add(tempdcircell1_DIS);
                outCellDCIR.Add(tempdcircell3_DIS);

                max_outCellDCIR = outCellDCIR.Max();
                min_outCellDCIR = outCellDCIR.Min();

                LogState(LogType.Info, "outCellDCIR(Max) = " + max_outCellDCIR.ToString());
                LogState(LogType.Info, "outCellDCIR(Min) = " + min_outCellDCIR.ToString());
                LogState(LogType.Info, max_outCellDCIR.ToString() + " - " + min_outCellDCIR.ToString() + 
                                       " = " + (max_outCellDCIR - min_outCellDCIR).ToString());
            }
            // Pors N,M / Maserati / Pors FaceLift N,M
            else if (
                localTypes == ModelType.PORSCHE_NORMAL ||
                localTypes == ModelType.PORSCHE_MIRROR ||
                localTypes == ModelType.MASERATI_NORMAL ||
                localTypes == ModelType.PORSCHE_FACELIFT_NORMAL ||
                localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
              )
            {
                // Max(CellDCIR Bank1,2,5,6) - Min(CellDCIR Bank1,2,5,6)

                outCellDCIR.Add(tempdcircell1_DIS);
                outCellDCIR.Add(tempdcircell2_DIS);
                outCellDCIR.Add(tempdcircell5_DIS);
                outCellDCIR.Add(tempdcircell6_DIS);

                inCellDCIR.Add(tempdcircell3_DIS);
                inCellDCIR.Add(tempdcircell4_DIS);

                max_outCellDCIR = outCellDCIR.Max();
                min_outCellDCIR = outCellDCIR.Min();

                LogState(LogType.Info, "outCellDCIR(Max) = " + max_outCellDCIR.ToString());
                LogState(LogType.Info, "outCellDCIR(Min) = " + min_outCellDCIR.ToString());
                LogState(LogType.Info, max_outCellDCIR.ToString() + " - " + min_outCellDCIR.ToString() + 
                                       " = " + (max_outCellDCIR - min_outCellDCIR).ToString());
            }
            // e-UP
            else if (localTypes == ModelType.E_UP)
            {
                // Max(CellDCIR Bank1,2,5,6) - Min(CellDCIR Bank1,2,5,6)

                outCellDCIR.Add(tempdcircell1_DIS);
                outCellDCIR.Add(tempdcircell2_DIS);
                outCellDCIR.Add(tempdcircell5_DIS);
                outCellDCIR.Add(tempdcircell6_DIS);

                inCellDCIR.Add(tempdcircell3_DIS);
                inCellDCIR.Add(tempdcircell4_DIS);

                max_outCellDCIR = outCellDCIR.Max();
                min_outCellDCIR = outCellDCIR.Min();

                LogState(LogType.Info, "outCellDCIR = " + max_outCellDCIR.ToString());
                LogState(LogType.Info, "inCellDCIR = " + min_outCellDCIR.ToString());
                LogState(LogType.Info, max_outCellDCIR.ToString() + " - " + min_outCellDCIR.ToString() + 
                                       " = " + (max_outCellDCIR - min_outCellDCIR).ToString());
            }

            ti.Value_ = max_outCellDCIR - min_outCellDCIR;

            return JudgementTestItem(ti);
        }

        #region Not used
        public bool Cell7_DCIR(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_7;
            var point2 = cellDetailList[1].CellVolt_7;
            return JudgementTestItem(ti);
        }
        public bool Cell8_DCIR(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_8;
            var point2 = cellDetailList[1].CellVolt_8;
            return JudgementTestItem(ti);
        }
        public bool Cell9_DCIR(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_9;
            var point2 = cellDetailList[1].CellVolt_9;
            return JudgementTestItem(ti);
        }
        public bool Cell10_DCIR(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_10;
            var point2 = cellDetailList[1].CellVolt_10;
            return JudgementTestItem(ti);
        }
        public bool Cell11_DCIR(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_11;
            var point2 = cellDetailList[1].CellVolt_11;
            return JudgementTestItem(ti);
        }
        public bool Cell12_DCIR(TestItem ti)
        {
            isProcessingUI(ti);
            var point1 = cellDetailList[0].CellVolt_12;
            var point2 = cellDetailList[1].CellVolt_12;
            return JudgementTestItem(ti);
        }
        #endregion

    }
}