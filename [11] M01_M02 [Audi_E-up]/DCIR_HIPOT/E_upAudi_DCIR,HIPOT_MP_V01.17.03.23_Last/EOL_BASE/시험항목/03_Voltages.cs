using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE
{

    public partial class MainWindow
    {
        double tempCv1 = 0.0;
        double tempCv2 = 0.0;
        double tempCv3 = 0.0;
        double tempCv4 = 0.0;
        double tempCv5 = 0.0;
        double tempCv6 = 0.0;

        double cellVoltMaxSpec = 0.0;
        double cellVoltMinSpec = 0.0;

        //Cell Dev 결과 값.
        double m_dCellDev = 0.0;

        public bool CellVoltage1(TestItem ti)
        {
            isProcessingUI(ti);

            cellVoltMaxSpec = double.Parse(ti.Max.ToString());
            cellVoltMinSpec = double.Parse(ti.Min.ToString());

			cellVoltMaxSpec += 0.005;//5mv
			cellVoltMinSpec -= 0.005;//5mv


            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            tempCv1 = 0.0;
            tempCv2 = 0.0;
            tempCv3 = 0.0;
            tempCv4 = 0.0;
            tempCv5 = 0.0;
            tempCv6 = 0.0;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {

                try
                {
                    LogState(LogType.Info, "Cell Resistance ");

                    string resData = "";
                    int rangeMax = 1000000;
                    bool specResult = true;
                    List<double> resList = new List<double>();
                    List<double> cvList = new List<double>();

                    #region 처음 측정시에는 수동 레인지(1,000,000 / 1Mohm)

                    var dmmChInfo = new DMMChannelInfo(localTypes);

                    if (!keysight.MeasRes_Multi(out resList, dmmChInfo.CellCH, 3))
                    {
                        ti.Value_ = _DEVICE_NOT_READY;
                        return JudgementTestItem(ti);
                    }

                    for (int i = 0; i < resList.Count; i++)
                    {
                        if (resList[i] > 9.9E+30)
                        {
                            resList[i] = 99999;
                        }

                        if (i == resList.Count - 1)
                            resData += resList[i];
                        else
                            resData += resList[i] + "&";
                    }
                    #endregion

                    #region 오픈스펙 상/하한 조건에 걸리는 지 확인


                    var lineOpenCheckUpperLimit = 0.0;
                    var lineOpenCheckLowerLimit = 0.0;
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        lineOpenCheckUpperLimit = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey18]);
                        lineOpenCheckLowerLimit = double.Parse(this.viewModel.ControlItemList[AUDI_EOL_ControlKey19]);
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        lineOpenCheckUpperLimit = double.Parse(this.viewModel.ControlItemList[AUDI_VOLT_ControlKey1]);
                        lineOpenCheckLowerLimit = double.Parse(this.viewModel.ControlItemList[AUDI_VOLT_ControlKey2]);
                    }

                    bool autoModeRetry = false;
                    foreach (var ress in resList)
                    {
                        if (lineOpenCheckLowerLimit * 1000 > ress || lineOpenCheckUpperLimit * 1000 < ress)
                            autoModeRetry = true;
                    }

                    #endregion

                    #region 오픈스펙 상/하한 조건에 걸린다면 오토 레인지로 재측정

                    if (autoModeRetry)
                    {
                        LogState(LogType.Info, "Cell Resistance retry");
                        Thread.Sleep(200);

                        resList.Clear();
                        resData = "";
                        if (!keysight.MeasRes_Multi(out resList, dmmChInfo.CellCH, 3, true))
                        {
                            ti.Value_ = _DEVICE_NOT_READY;
                            return JudgementTestItem(ti);
                        }
                        for (int i = 0; i < resList.Count; i++)
                        {
                            if (resList[i] > 9.9E+30)
                            {
                                resList[i] = 99999;
                            }

                            if (i == resList.Count - 1)
                                resData += resList[i];
                            else
                                resData += resList[i] + "&";
                        }
                    }

                    LogState(LogType.Info, "Cell Resistance," + resData);

                    #endregion

                    #region 가져온 저항값(재시도했다면 오토레인지값) 상세수집으로 추가

                    ti.refValues_.Clear();

                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_LINE__OP_UPPER, lineOpenCheckUpperLimit.ToString() ));
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_LINE__OP_LOWER, lineOpenCheckLowerLimit.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_LINE_OP_RESIST, resData));
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_AUD_NOR_LINE__OP_UPPER, lineOpenCheckUpperLimit.ToString() ));
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_AUD_NOR_LINE__OP_LOWER, lineOpenCheckLowerLimit.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_AUD_NOR_LINE_OP_RESIST, resData));
                    }
                    #endregion

                    #region 가져온 저항값 기준으로 전압을 읽은 뒤, 저항스펙 안되는넘을 0으로 치환하는 부분

                    if (!keysight.MeasVolt_Multi(out cvList, dmmChInfo.CellCH, 3))
                    {
                        ti.Value_ = _DEVICE_NOT_READY;
                        return JudgementTestItem(ti);
                    }
                    if (resList.Count == cvList.Count)
                    {
                        for (int i = 0; i < resList.Count; i++)
                        {
                            if (lineOpenCheckLowerLimit * 1000 > resList[i] || resList[i] > lineOpenCheckUpperLimit * 1000)
                            {
                                cvList[i] = 0;
                            }
                        }
                    }
                    else
                    {
                        ti.Value_ = _DEVICE_NOT_READY;
                        return JudgementTestItem(ti);
                    }

                    #endregion

                    ti.Value_ = tempCv1 = cvList[0];
                    tempCv2 = cvList[1];
                    tempCv3 = cvList[2];
                }
                catch
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )
            {
                var raw = CMClist[1];
                ti.Value_ = tempCv1 = (raw * 0.0001525925);
                LogState(LogType.Info, ti.Name + " - " + raw + " * 0.0001525925 = " + tempCv1);
            }
            else if (localTypes == ModelType.E_UP)
            {

                try
                {
                    LogState(LogType.Info, "Cell Resistance ");

                    string resData = "";
                    int rangeMax = 1000000;
                    bool specResult = true;
                    List<double> resList = new List<double>();
                    List<double> cvList = new List<double>();

                    #region 처음 측정시에는 수동 레인지(1,000,000 / 1Mohm)

                    var dmmChInfo = new DMMChannelInfo(localTypes);

                    if (!keysight.MeasRes_Multi(out resList, dmmChInfo.CellCH, 6))
                    {
                        ti.Value_ = _DEVICE_NOT_READY;
                        return JudgementTestItem(ti);
                    }

                    for (int i = 0; i < resList.Count; i++)
                    {
                        if (resList[i] > 9.9E+30)
                        {
                            resList[i] = 99999;
                        }

                        if (i == resList.Count - 1)
                            resData += resList[i];
                        else
                            resData += resList[i] + "&";
                    }
                    #endregion

                    #region 오픈스펙 상/하한 조건에 걸리는 지 확인


                    var lineOpenCheckUpperLimit = 0.0;
                    var lineOpenCheckLowerLimit = 0.0;
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        lineOpenCheckUpperLimit = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey18]);
                        lineOpenCheckLowerLimit = double.Parse(this.viewModel.ControlItemList[E_UP_EOL_ControlKey19]);
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        lineOpenCheckUpperLimit = double.Parse(this.viewModel.ControlItemList[E_UP_VOLT_ControlKey1]);
                        lineOpenCheckLowerLimit = double.Parse(this.viewModel.ControlItemList[E_UP_VOLT_ControlKey2]);
                    }

                    bool autoModeRetry = false;
                    foreach (var ress in resList)
                    {
                        if (lineOpenCheckLowerLimit * 1000 > ress || lineOpenCheckUpperLimit * 1000 < ress)
                            autoModeRetry = true;
                    }

                    #endregion

                    #region 오픈스펙 상/하한 조건에 걸린다면 오토 레인지로 재측정

                    if (autoModeRetry)
                    {
                        LogState(LogType.Info, "Cell Resistance retry");
                        Thread.Sleep(200);

                        resList.Clear();
                        resData = "";
                        if (!keysight.MeasRes_Multi(out resList, dmmChInfo.CellCH, 6, true))
                        {
                            ti.Value_ = _DEVICE_NOT_READY;
                            return JudgementTestItem(ti);
                        }
                        for (int i = 0; i < resList.Count; i++)
                        {
                            if (resList[i] > 9.9E+30)
                            {
                                resList[i] = 99999;
                            }

                            if (i == resList.Count - 1)
                                resData += resList[i];
                            else
                                resData += resList[i] + "&";
                        }
                    }

                    LogState(LogType.Info, "Cell Resistance," + resData);

                    #endregion

                    #region 가져온 저항값(재시도했다면 오토레인지값) 상세수집으로 추가

                    ti.refValues_.Clear();

                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_LINE__OP_UPPER, lineOpenCheckUpperLimit.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_LINE__OP_LOWER, lineOpenCheckLowerLimit.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_LINE_OP_RESIST, resData));
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_E_UP_LINE__OP_UPPER, lineOpenCheckUpperLimit.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_E_UP_LINE__OP_LOWER, lineOpenCheckLowerLimit.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_E_UP_LINE_OP_RESIST, resData));
                    }
                    #endregion

                    #region 가져온 저항값 기준으로 전압을 읽은 뒤, 저항스펙 안되는넘을 0으로 치환하는 부분

                    if (!keysight.MeasVolt_Multi(out cvList, dmmChInfo.CellCH, 6))
                    {
                        ti.Value_ = _DEVICE_NOT_READY;
                        return JudgementTestItem(ti);
                    }
                    if (resList.Count == cvList.Count)
                    {
                        for (int i = 0; i < resList.Count; i++)
                        {
                            if (lineOpenCheckLowerLimit * 1000 > resList[i] || resList[i] > lineOpenCheckUpperLimit * 1000)
                            {
                                cvList[i] = 0;
                            }
                        }
                    }
                    else
                    {
                        ti.Value_ = _DEVICE_NOT_READY;
                        return JudgementTestItem(ti);
                    }

                    #endregion

                    ti.Value_ = tempCv1 = cvList[0];
                    tempCv2 = cvList[1];
                    tempCv3 = cvList[2];
                    tempCv4 = cvList[3];
                    tempCv5 = cvList[4];
                    tempCv6 = cvList[5];
                }
                catch
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage2(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                ti.Value_ = tempCv2;
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )
            {
                var raw = CMClist[2];
                ti.Value_ = tempCv2 = (raw * 0.0001525925);
                LogState(LogType.Info, ti.Name + " - " + raw + " * 0.0001525925 = " + tempCv2);
            }
            else if(localTypes == ModelType.E_UP)
            {
                ti.Value_ = tempCv2;
            }
            return JudgementTestItem(ti);
        }

        public bool CellVoltage3(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                ti.Value_ = tempCv3;
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )
            {
                var raw = CMClist[3];
                ti.Value_ = tempCv3 = (raw * 0.0001525925);
                LogState(LogType.Info, ti.Name + " - " + raw + " * 0.0001525925 = " + tempCv3);
            }
            else if (localTypes == ModelType.E_UP)
            {
                ti.Value_ = tempCv3;
            }
            return JudgementTestItem(ti);
        }

        public bool CellVoltage4(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )
            {
                var raw = CMClist[4];
                ti.Value_ = tempCv4 = (raw * 0.0001525925);
                LogState(LogType.Info, ti.Name + " - " + raw + " * 0.0001525925 = " + tempCv4);
            }
            else if (localTypes == ModelType.E_UP)
            {
                ti.Value_ = tempCv4;
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage5(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )
            {
                var raw = CMClist[5];
                ti.Value_ = tempCv5 = (raw * 0.0001525925);
                LogState(LogType.Info, ti.Name + " - " + raw + " * 0.0001525925 = " + tempCv5);
            }
            else if (localTypes == ModelType.E_UP)
            {
                ti.Value_ = tempCv5;
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage6(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR//210312 wjs add pors fl
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )
            {
                var raw = CMClist[6];
                ti.Value_ = tempCv6 = (raw * 0.0001525925);
                LogState(LogType.Info, ti.Name + " - " + raw + " * 0.0001525925 = " + tempCv6);
            }
            else if (localTypes == ModelType.E_UP)
            {
                ti.Value_ = tempCv6;
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltageDeviation(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.refValues_.Clear();
            List<double> dl = new List<double>();
            dl.Add(tempCv1);
            dl.Add(tempCv2);
            dl.Add(tempCv3);

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                || localTypes == ModelType.E_UP
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )
            {
                dl.Add(tempCv4);
                dl.Add(tempCv5);
                dl.Add(tempCv6);
            }

            var max = dl.Max();
            var min = dl.Min();
            m_dCellDev = (max - min) * 1000;
            ti.Value_ = (max - min) * 1000;

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )
            {
                ti.refValues_.Clear();
                //201217 wjs add maserati
                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CELL_V_DEV_MAX, max.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CELL_V_DEV_MIN, min.ToString()));
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_CELL_V_DEV_MAX, max.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_CELL_V_DEV_MIN, min.ToString()));
                    }
                }
                else if (localTypes == ModelType.MASERATI_NORMAL)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_CELL_V_DEV_MAX, max.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_CELL_V_DEV_MIN, min.ToString()));
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_CELL_V_DEV_MAX, max.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_CELL_V_DEV_MIN, min.ToString()));
                    }
                }
                else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_CELL_V_DEV_MAX, max.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_CELL_V_DEV_MIN, min.ToString()));
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_CELL_V_DEV_MAX, max.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_CELL_V_DEV_MIN, min.ToString()));
                    }
                }
                //221101 wjs add mase m183
                else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_CELL_V_DEV_MAX, max.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_CELL_V_DEV_MIN, min.ToString()));
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_CELL_V_DEV_MAX, max.ToString()));
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_CELL_V_DEV_MIN, min.ToString()));
                    }
                }
                //if (pro_Type == ProgramType.EOLInspector)
                //{
                //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CELL_V_DEV_MAX, max.ToString()));
                //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CELL_V_DEV_MIN, min.ToString()));
                //}
                //else if (pro_Type == ProgramType.VoltageInspector)
                //{
                //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_CELL_V_DEV_MAX, max.ToString()));
                //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_CELL_V_DEV_MIN, min.ToString()));
                //}

            }
            LogState(LogType.Info, "Cell Voltage Deviation - Max :" + max.ToString() + "/Min :" + min.ToString());
            return JudgementTestItem(ti);
        }


        public bool CellVoltageDeviation_FromCellInspection(TestItem ti)
        {
            isProcessingUI(ti);

            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            if (localTypes == ModelType.E_UP || 
                localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
            {
                if (MES.mes_CellList.Count < 12)
                {
                    ti.Value_ = _MES_CELL_DATA_NOT_RECIVED;
                    return JudgementTestItem(ti);
                }
            }

            ti.refValues_.Clear();

            List<double> dl = new List<double>();

            if (localTypes == ModelType.E_UP) //2p6s
            {
                dl.Add((MES.mes_CellList.ToList()[0].Value +
                        MES.mes_CellList.ToList()[1].Value) / 2);

                dl.Add((MES.mes_CellList.ToList()[2].Value +
                        MES.mes_CellList.ToList()[3].Value) / 2);

                dl.Add((MES.mes_CellList.ToList()[4].Value +
                        MES.mes_CellList.ToList()[5].Value) / 2);

                dl.Add((MES.mes_CellList.ToList()[6].Value +
                        MES.mes_CellList.ToList()[7].Value) / 2);

                dl.Add((MES.mes_CellList.ToList()[8].Value +
                        MES.mes_CellList.ToList()[9].Value) / 2);

                dl.Add((MES.mes_CellList.ToList()[10].Value +
                        MES.mes_CellList.ToList()[11].Value) / 2);
            }
            else if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR) //4p3s
            {
                dl.Add((MES.mes_CellList.ToList()[0].Value +
                        MES.mes_CellList.ToList()[1].Value +
                        MES.mes_CellList.ToList()[2].Value +
                        MES.mes_CellList.ToList()[3].Value) / 4);

                dl.Add((MES.mes_CellList.ToList()[4].Value +
                        MES.mes_CellList.ToList()[5].Value +
                        MES.mes_CellList.ToList()[6].Value +
                        MES.mes_CellList.ToList()[7].Value) / 4);

                dl.Add((MES.mes_CellList.ToList()[8].Value +
                        MES.mes_CellList.ToList()[9].Value +
                        MES.mes_CellList.ToList()[10].Value +
                        MES.mes_CellList.ToList()[11].Value) / 4);
            }
            else {}

            var max = dl.Max();

            var min = dl.Min();

            LogState(LogType.Info, "Cell Voltage Deviation From Mes - Max :" + max.ToString() + "/Min :" + min.ToString());

            double dDev_FromMesCv = (max - min) * 1000;

            ti.Value_ = m_dCellDev - dDev_FromMesCv;

            LogState(LogType.Info, "Cell Voltage Deviation :" + m_dCellDev.ToString() + "Cell Voltage Deviation From Mes :" + dDev_FromMesCv.ToString());

            string strCV = "";

            for (int i = 0; i < MES.mes_CellList.Count; i++)
            {
                strCV += (MES.mes_CellList.ToList()[i].Value.ToString() + "&");
            }

            strCV = strCV.Remove(strCV.Length - 1, 1);

            string strCV_CalCulated = "";

            for (int i = 0; i < dl.Count; i++)
            {
                strCV_CalCulated += (dl[i].ToString() + "&");
            }

            strCV_CalCulated = strCV_CalCulated.Remove(strCV_CalCulated.Length - 1, 1);

            if (pro_Type == ProgramType.EOLInspector)
            {
                if (localTypes == ModelType.E_UP) 
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_CELL_VOLT, strCV));

                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_CALC_CELL_VOLT, strCV_CalCulated));
                }
                else if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUDI_CELL_VOLT, strCV));

                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUDI_CALC_CELL_VOLT, strCV_CalCulated));
                }
            }

            return JudgementTestItem(ti);
        }

        double tempMv1 = 0.0;
        public bool ModuleVoltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var dmmChInfo = new DMMChannelInfo(localTypes);

            tempMv1 = 0.0;

            if (keysight.MeasVolt(out tempMv1, dmmChInfo.ModuleCH))
            {
                //아우디는 값으로 쓴다
                if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
                {
                    ti.Value_ = tempMv1;
                }
                else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR
                || localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                )
                {
                    //포르셰 마세라티는 CMC를 쓰고, 상세수집으로 모듈전압을 쓴다
                    var raw = CMClist[0];
                    ti.Value_ = (raw * 0.0024415);
                    LogState(LogType.Info, ti.Name + " - " + raw + " * 0.0024415 = " + ti.Value_);

                    ti.refValues_.Clear();
                    //201217 wjs add maserati
                    if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                    {
                        if (pro_Type == ProgramType.EOLInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_MODULE____VOLT, tempMv1.ToString()));
                        else if (pro_Type == ProgramType.VoltageInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_MODULE____VOLT, tempMv1.ToString()));
                    }
                    else if (localTypes == ModelType.MASERATI_NORMAL)
                    {

                        if (pro_Type == ProgramType.EOLInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_MODULE____VOLT, tempMv1.ToString()));
                        else if (pro_Type == ProgramType.VoltageInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_MODULE____VOLT, tempMv1.ToString()));
                    }
                    //210312 WJS ADD PORS FL
                    else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                    {
                        if (pro_Type == ProgramType.EOLInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_MODULE____VOLT, tempMv1.ToString()));
                        else if (pro_Type == ProgramType.VoltageInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_MODULE____VOLT, tempMv1.ToString()));
                    }
                    //221101 wjs add mase m183
                    else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                    {
                        if (pro_Type == ProgramType.EOLInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_MODULE____VOLT, tempMv1.ToString()));
                        else if (pro_Type == ProgramType.VoltageInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_MODULE____VOLT, tempMv1.ToString()));
                    }
                    //if (pro_Type == ProgramType.EOLInspector)
                    //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_MODULE____VOLT, tempMv1.ToString())); 
                    //else if (pro_Type == ProgramType.VoltageInspector)
                    //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_MODULE____VOLT, tempMv1.ToString()));

                }
                else if (localTypes == ModelType.E_UP)
                {
                    ti.Value_ = tempMv1;
                }
            }
            else
            {
                tempMv1 = 0;
                ti.Value_ = _DEVICE_NOT_READY;
            }             

            return JudgementTestItem(ti);
        }

        #region Porsche - MES Deviation
        public bool CellVoltage1_dev(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var mesCv1 = MES.mes_CellList.ToList()[0].Value;
            var mesCv2 = MES.mes_CellList.ToList()[1].Value;
            var avg = (mesCv1 + mesCv2) / 2.0;

            ti.Value_ = (tempCv1 - avg) * 1000;
            LogState(LogType.Info, ti.Name + " : " + string.Format("( {0} - (( {1} + {2} ) / 2.0 )) * 1000 = {3}", tempCv1,
                mesCv1, mesCv2, ti.Value_.ToString()));

            ti.refValues_.Clear();

            //201217 wjs add maserati
            string detail_KeynameCV = "";
            string detail_KeynameCB = "";
            if (localTypes == ModelType.MASERATI_NORMAL)
            {
                detail_KeynameCV = KEY_VOL_MAS_NOR_DEV_VOLT__REAL;
                detail_KeynameCB = KEY_VOL_MAS_NOR_DEV_VOLT___BCR;
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                detail_KeynameCV = KEY_VOL_POR_NOR_DEV_VOLT__REAL;
                detail_KeynameCB = KEY_VOL_POR_NOR_DEV_VOLT___BCR;
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR) //210312 WJS ADD PORS FL !!!
            {
                detail_KeynameCV = KEY_VOL_POR_F_L_DEV_VOLT__REAL;
                detail_KeynameCB = KEY_VOL_POR_F_L_DEV_VOLT___BCR;
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                detail_KeynameCV = KEY_VOL_MAS_M183_DEV_VOLT__REAL;
                detail_KeynameCB = KEY_VOL_MAS_M183_DEV_VOLT___BCR;
            }
            var dti1 = new DetailItems() { Key = detail_KeynameCV };
            var dti2 = new DetailItems() { Key = detail_KeynameCB };

            //var dti1 = new DetailItems() { Key = "WMPMTE4015" };
            //var dti2 = new DetailItems() { Key = "WMPMTE4016" };

            foreach (var dic in MES.mes_CellList)
            {
                dti1.Reportitems.Add(dic.Value);
                dti2.Reportitems.Add(dic.Key);
            }

            ti.refValues_.Add(dti1);
            ti.refValues_.Add(dti2);

            return JudgementTestItem(ti);
        }

        public bool CellVoltage2_dev(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var mesCv1 = MES.mes_CellList.ToList()[2].Value;
            var mesCv2 = MES.mes_CellList.ToList()[3].Value;
            var avg = (mesCv1 + mesCv2) / 2.0;

            ti.Value_ = (tempCv2 - avg) * 1000;
            LogState(LogType.Info, ti.Name + " : " + string.Format("( {0} - (( {1} + {2} ) / 2.0 )) * 1000 = {3}", tempCv2,
                mesCv1, mesCv2, ti.Value_.ToString()));

            return JudgementTestItem(ti);
        }

        public bool CellVoltage3_dev(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var mesCv1 = MES.mes_CellList.ToList()[4].Value;
            var mesCv2 = MES.mes_CellList.ToList()[5].Value;
            var avg = (mesCv1 + mesCv2) / 2.0;

            ti.Value_ = (tempCv3 - avg) * 1000;
            LogState(LogType.Info, ti.Name + " : " + string.Format("( {0} - (( {1} + {2} ) / 2.0 )) * 1000 = {3}", tempCv3,
                mesCv1, mesCv2, ti.Value_.ToString()));

            return JudgementTestItem(ti);
        }

        public bool CellVoltage4_dev(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var mesCv1 = MES.mes_CellList.ToList()[6].Value;
            var mesCv2 = MES.mes_CellList.ToList()[7].Value;
            var avg = (mesCv1 + mesCv2) / 2.0;

            ti.Value_ = (tempCv4 - avg) * 1000;
            LogState(LogType.Info, ti.Name + " : " + string.Format("( {0} - (( {1} + {2} ) / 2.0 )) * 1000 = {3}", tempCv4,
                mesCv1, mesCv2, ti.Value_.ToString()));

            return JudgementTestItem(ti);
        }

        public bool CellVoltage5_dev(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var mesCv1 = MES.mes_CellList.ToList()[8].Value;
            var mesCv2 = MES.mes_CellList.ToList()[9].Value;
            var avg = (mesCv1 + mesCv2) / 2.0;

            ti.Value_ = (tempCv5 - avg) * 1000;
            LogState(LogType.Info, ti.Name + " : " + string.Format("( {0} - (( {1} + {2} ) / 2.0 )) * 1000 = {3}", tempCv5,
                mesCv1, mesCv2, ti.Value_.ToString()));
             
            return JudgementTestItem(ti);
        }

        public bool CellVoltage6_dev(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var mesCv1 = MES.mes_CellList.ToList()[10].Value;
            var mesCv2 = MES.mes_CellList.ToList()[11].Value;
            var avg = (mesCv1 + mesCv2) / 2.0;

            ti.Value_ = (tempCv6 - avg) * 1000;
            LogState(LogType.Info, ti.Name + " : " + string.Format("( {0} - (( {1} + {2} ) / 2.0 )) * 1000 = {3}", tempCv6,
                mesCv1, mesCv2, ti.Value_.ToString()));

            #region mes_CellList Initialize
            Dictionary<string, double> tmepdic = new Dictionary<string, double>();
            foreach (var item in MES.mes_CellList)
            {
                tmepdic.Add(item.Key, 0.0);
            }
            MES.mes_CellList = tmepdic;
            #endregion

            return JudgementTestItem(ti);
        }
        #endregion
    }
}
