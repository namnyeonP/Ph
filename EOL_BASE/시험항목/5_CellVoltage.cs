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
        public double ocv_voltage = 0.0;
        public int nTestcnt = 0;
        public bool bRetryLVFirst = true;       


        private bool ModeSet_LV_CURR(TestItem ti)
        {
            if (plc.isTesting_Flag)
            {
                if (!plc.jig_Down_Flag)
                {
                    plc.Jig_Down(true);
                    int cnt = 0;
                    while (true)
                    {
                        Thread.Sleep(100);
                        cnt += 100;//5s

                        //while (ispause)
                        //{
                        //    Thread.Sleep(1);
                        //}

                        if (cnt > 15000 || isStop)
                        {
                            ti.Value_ = _JIG_NOT_ACTIVE;
                            LogState(LogType.Fail, "Jig Not Activated");
                            cycler.SendToDSP1("0FF", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
                            LogState(LogType.Info, "Cycler Mode Reset [ - FF]");
                            plc.Jig_Down(false);
                            return JudgementTestItem(ti);
                        }

                        if (plc.plc_isJigDown)
                        {
                            LogState(LogType.Success, "Jig Activated at " + cnt.ToString() + "msc");
                            plc.Jig_Down(false);
                            break;
                        }
                    }
                }
            }
            return true;
        }
        void SetChannels(out string measString, out int cellVoltCnt, out int reccnt)
        {
            if (localTypes == 0 || localTypes == 1 || localTypes == 3)
            {
                reccnt = 145;
            }
            else if (localTypes == 2)
            {
                reccnt = 129;
            }
            else if (localTypes == 4 || localTypes == 5)
            {
                reccnt = 177;
            }
            else
            {
                reccnt = 0;
            }

            if (localTypes == 0)
            {
                //4P8S
                cellVoltCnt = 8;//8 cell, 1module  
                measString = "111,112,113,114,115,116,117,118,120";
            }
            else if (localTypes == 1)
            {
                //4P8S_rev
                cellVoltCnt = 8;//8 cell, 1module  
                measString = "211,212,213,214,215,216,217,218,220";
            }
            else if (localTypes == 2)
            {
                //4P7S
                cellVoltCnt = 7;//7 cell, 1module  
                measString = "301,302,303,304,305,306,307,320";
            }
            else if (localTypes == 3)
            {
                //3P8S
                cellVoltCnt = 8;//8 cell, 1module  
                measString = "111,112,113,114,115,116,117,118,120";
            }
            else if (localTypes == 4)
            {
                //3P10S
                cellVoltCnt = 10;//10 cell, 1module  
                measString = "101,102,103,104,105,106,107,108,109,110,120";
            }
            else if (localTypes == 5)
            {
                //3P10S_rev
                cellVoltCnt = 10;//10 cell, 1module  
                measString = "201,202,203,204,205,206,207,208,209,210,220";
            }
            else
            {
                cellVoltCnt = 0;
                measString = "";
            }
        }
        public bool Module_Voltage(TestItem ti)
        {
            cellVoltCnt = 0;
            string measString = "";
            int reccnt = 0;
            List<double> voltList = new List<double>();
            List<double> resList = new List<double>();

            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            //221202         public int bRetryLVFirst = 0;
            if (bRetryLVFirst)
            {
                if (!ModeSet_LV_CURR(ti))
                {
                    return false;
                }
                bRetryLVFirst = false;
            }


            if (isLineFlag)
            {
                #region DAQ970A
                if (daq970_1 == null)
                {
                    ti.Value_ = "NotConnected";
                    return JudgementTestItem(ti);
                }

                SetChannels(out measString, out cellVoltCnt, out reccnt);

                #region measure Each Resistance
                if (!daq970_1.MeasRes_Multi(out resList, measString, cellVoltCnt + 1))
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }

                string resStr = "";
                var resdti = new DetailItems() { Key = "W2MLMTE4044" };

                foreach (var item in resList)
                {
                    resdti.Reportitems.Add(item);
                    resStr += item.ToString() + "&";
                }

                LogState(LogType.RESPONSE, "KeysightDAQ970 RES:" + resStr);

                ti.refValues_.Add(resdti);
                #endregion

                #region measure Each Voltage
                if (!daq970_1.MeasVolt_Multi(out voltList, measString, cellVoltCnt + 1))
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }

                CellVoltageList = new List<double>();
                CellVoltageList = voltList;

                string voltStr = "";
                foreach (var item in CellVoltageList)
                {
                    voltStr += item.ToString() + ",";
                }

                LogState(LogType.RESPONSE, "KeysightDAQ970 Volt:" + voltStr);
                #endregion
                #endregion
            }
            else
            {
                #region DMM34970
                keysight.rtstring = "";

                if (keysight == null)
                {
                    ti.Value_ = "NotConnected";
                    return JudgementTestItem(ti);
                }

                SetChannels(out measString, out cellVoltCnt, out reccnt);

                #region measure Each Resistance
                keysight.MeasTemp(measString);

                int rec = keysight.sp.BytesToRead;

                int cnt = 0;
                while (rec < reccnt)
                {
                    Thread.Sleep(100);
                    rec = keysight.sp.BytesToRead;
                    cnt += 100;
                    //not received data
                    if (cnt == 5000)
                    {
                        keysight.MeasTemp(measString);

                        rec = keysight.sp.BytesToRead;

                        cnt = 0;
                        while (rec < 145)
                        {
                            Thread.Sleep(100);
                            rec = keysight.sp.BytesToRead;
                            cnt += 100;
                            if (cnt == 5000)
                            {
                                ti.Value_ = _DEVICE_NOT_READY;
                                return JudgementTestItem(ti);
                            }
                        }
                        break;
                    }
                }
                //받은 후에 데이터 파싱
                byte[] bt = new byte[rec];
                keysight.sp.Read(bt, 0, rec);

                keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);


                //LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

                var vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                string resetr = "";

                var resdti = new DetailItems() { Key = "W2MLMTE4044" };

                foreach (var item in vArr)
                {
                    double dv = 0;
                    if (double.TryParse(item, out dv))
                    {
                        resdti.Reportitems.Add(dv);
                        resList.Add(dv);
                        resetr += dv.ToString() + "&";
                    }
                }
                LogState(LogType.RESPONSE, "KeysightDMM RES:" + resetr);

                ti.refValues_.Add(resdti);
                #endregion

                #region measure Each Voltage
                keysight.rtstring = "";
                keysight.MeasVolt(measString);

                rec = keysight.sp.BytesToRead;

                cnt = 0;
                while (rec < reccnt)//33
                {
                    Thread.Sleep(100);
                    rec = keysight.sp.BytesToRead;
                    cnt += 100;
                    //not received data
                    if (cnt == 5000)
                    {
                        keysight.MeasVolt(measString);

                        rec = keysight.sp.BytesToRead;

                        cnt = 0;
                        while (rec < 145)//33
                        {
                            Thread.Sleep(100);
                            rec = keysight.sp.BytesToRead;
                            cnt += 100;
                            if (cnt == 5000)
                            {
                                ti.Value_ = _DEVICE_NOT_READY;
                                return JudgementTestItem(ti);
                            }
                        }
                        break;
                    }
                }
                //받은 후에 데이터 파싱
                bt = new byte[rec];
                keysight.sp.Read(bt, 0, rec);

                keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);

                //LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

                vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                CellVoltageList = new List<double>();
                string voltstr = "";
                foreach (var item in vArr)
                {
                    double dv = 0;
                    if (double.TryParse(item, out dv))
                    {
                        CellVoltageList.Add(dv);
                        voltstr += dv.ToString() + ",";
                    }
                }
                LogState(LogType.RESPONSE, "KeysightDMM Volt:" + voltstr);
                #endregion
                #endregion
            }

            #region Filtering by res Val
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4038", LINE_OPEN_CHECK_UPPER_LIMIT.ToString()));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4039", LINE_OPEN_CHECK_LOWER_LIMIT.ToString()));

            var resHighLimit = LINE_OPEN_CHECK_UPPER_LIMIT * 1000;
            var resLowLimit = LINE_OPEN_CHECK_LOWER_LIMIT * 1000;

           for (int i = 0; i < resList.Count - 1; i++)
            {
                if (resList[i] > resHighLimit || resList[i] < resLowLimit)
                {
                    CellVoltageList[i] = 0;
               }
            }
            #endregion

            ti.Value_ = ocv_voltage = CellVoltageList.Last();

            CellVoltageList.Remove(CellVoltageList.Last());

            SetOcvCellVoltage(CellVoltageList);

            return JudgementTestItem(ti);
        }
        public bool Cell1_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[0];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Cell2_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[1];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Cell3_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[2];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Cell4_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[3];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Cell5_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[4];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Cell6_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[5];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Cell7_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[6];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Cell8_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[7];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Cell9_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[8];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Cell10_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                ti.Value_ = CellVoltageList[9];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Cell_Voltage_Deviation(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                double deviation = CellVoltageList.Max() - CellVoltageList.Min();
                LogState(LogType.Info, string.Format("{0}(Max) - {1}(Min) = {2}",
                    CellVoltageList.Max(), CellVoltageList.Min(), deviation));
                ti.Value_ = deviation * 1000;
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltageDeviation_FromCellInspection(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            ti.refValues_.Clear();

            #region MES Cell Voltage 갯수 확인
            if (localTypes == 0) //4P8S 32 Cell
            {
                if (!(MES.m_listMesCellVoltage.Count == 32))
                {
                    ti.Value_ = _MES_CELL_DATA_COUNT_NG;
                    return JudgementTestItem(ti);
                }
            }
            else if (localTypes == 1) //4P8S_rev 32 Cell
            {
                if (!(MES.m_listMesCellVoltage.Count == 32))
                {
                    ti.Value_ = _MES_CELL_DATA_COUNT_NG;
                    return JudgementTestItem(ti);
                }
            }
            else if (localTypes == 2) //4P7S 28 Cell
            {
                if (!(MES.m_listMesCellVoltage.Count == 28))
                {
                    ti.Value_ = _MES_CELL_DATA_COUNT_NG;
                    return JudgementTestItem(ti);
                }
            }
            else if (localTypes == 3) //3P8S 24 Cell
            {
                if (!(MES.m_listMesCellVoltage.Count == 24))
                {
                    ti.Value_ = _MES_CELL_DATA_COUNT_NG;
                    return JudgementTestItem(ti);
                }
            }
            else if (localTypes == 4) //3P10S 30 Cell
            {
                if (!(MES.m_listMesCellVoltage.Count == 30))
                {
                    ti.Value_ = _MES_CELL_DATA_COUNT_NG;
                    return JudgementTestItem(ti);
                }
            }
            else if (localTypes == 5) //3P10S_rev 30 Cell
            {
                if (!(MES.m_listMesCellVoltage.Count == 30))
                {
                    ti.Value_ = _MES_CELL_DATA_COUNT_NG;
                    return JudgementTestItem(ti);
                }
            }
            else // 이상한거....
            {
                ti.Value_ = _MES_CELL_DATA_COUNT_NG;
                return JudgementTestItem(ti);
            }
            #endregion

            List<double> calculated_MES_Cell = new List<double>();

            #region MES Cell Voltage 예상 전압 계산
            if (localTypes == 0) //4P8S 32 Cell
            {
                for (int i = 0; i < 32; i += 4)
                {
                    calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[i].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 1].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 2].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 3].Value) / 4);
                }
                if (!(calculated_MES_Cell.Count == cellVoltCnt))
                {
                    ti.Value_ = _MES_CELL_DATA_CALCULATE_NG;
                    return JudgementTestItem(ti);
                }

                #region Manual Calculate
                /*
                // 1번째 Cell 전압
                calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[0].Value +
                                         MES.m_listMesCellVoltage.ToList()[1].Value +
                                         MES.m_listMesCellVoltage.ToList()[2].Value +
                                         MES.m_listMesCellVoltage.ToList()[3].Value) / 4);
                // 2번째 Cell 전압
                calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[4].Value +
                                         MES.m_listMesCellVoltage.ToList()[5].Value +
                                         MES.m_listMesCellVoltage.ToList()[6].Value +
                                         MES.m_listMesCellVoltage.ToList()[7].Value) / 4);
                // 3번째 Cell 전압
                calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[8].Value +
                                         MES.m_listMesCellVoltage.ToList()[9].Value +
                                         MES.m_listMesCellVoltage.ToList()[10].Value +
                                         MES.m_listMesCellVoltage.ToList()[11].Value) / 4);
                // 4번째 Cell 전압
                calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[12].Value +
                                         MES.m_listMesCellVoltage.ToList()[13].Value +
                                         MES.m_listMesCellVoltage.ToList()[14].Value +
                                         MES.m_listMesCellVoltage.ToList()[15].Value) / 4);
                // 5번째 Cell 전압
                calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[16].Value +
                                         MES.m_listMesCellVoltage.ToList()[13].Value +
                                         MES.m_listMesCellVoltage.ToList()[14].Value +
                                         MES.m_listMesCellVoltage.ToList()[15].Value) / 4);
                // 6번째 Cell 전압
                calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[20].Value +
                                         MES.m_listMesCellVoltage.ToList()[13].Value +
                                         MES.m_listMesCellVoltage.ToList()[14].Value +
                                         MES.m_listMesCellVoltage.ToList()[15].Value) / 4);
                // 7번째 Cell 전압
                calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[24].Value +
                                         MES.m_listMesCellVoltage.ToList()[13].Value +
                                         MES.m_listMesCellVoltage.ToList()[14].Value +
                                         MES.m_listMesCellVoltage.ToList()[15].Value) / 4);
                // 8번째 Cell 전압
                calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[28].Value +
                                         MES.m_listMesCellVoltage.ToList()[13].Value +
                                         MES.m_listMesCellVoltage.ToList()[14].Value +
                                         MES.m_listMesCellVoltage.ToList()[15].Value) / 4);
                 */
                #endregion
            }
            else if (localTypes == 1) //4P8S_rev 32 Cell
            {
                for (int i = 0; i < 32; i += 4)
                {
                    calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[i].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 1].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 2].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 3].Value) / 4);
                }
                if (!(calculated_MES_Cell.Count == cellVoltCnt))
                {
                    ti.Value_ = _MES_CELL_DATA_CALCULATE_NG;
                    return JudgementTestItem(ti);
                }
            }
            else if (localTypes == 2) //4P7S 28 Cell
            {
                for (int i = 0; i < 28; i += 4)
                {
                    calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[i].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 1].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 2].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 3].Value) / 4);
                }
                if (!(calculated_MES_Cell.Count == cellVoltCnt))
                {
                    ti.Value_ = _MES_CELL_DATA_CALCULATE_NG;
                    return JudgementTestItem(ti);
                }
            }
            else if (localTypes == 3) //3P8S 24 Cell
            {
                for (int i = 0; i < 24; i += 3)
                {
                    calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[i].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 1].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 2].Value) / 3);
                }
                if (!(calculated_MES_Cell.Count == cellVoltCnt))
                {
                    ti.Value_ = _MES_CELL_DATA_CALCULATE_NG;
                    return JudgementTestItem(ti);
                }
            }
            else if (localTypes == 4) //3P10S 30 Cell
            {
                for (int i = 0; i < 30; i += 3)
                {
                    calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[i].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 1].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 2].Value) / 3);
                }
                if (!(calculated_MES_Cell.Count == cellVoltCnt))
                {
                    ti.Value_ = _MES_CELL_DATA_CALCULATE_NG;
                    return JudgementTestItem(ti);
                }
            }
            else if (localTypes == 5) //3P10S_rev 30 Cell
            {
                for (int i = 0; i < 30; i += 3)
                {
                    calculated_MES_Cell.Add((MES.m_listMesCellVoltage.ToList()[i].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 1].Value +
                                             MES.m_listMesCellVoltage.ToList()[i + 2].Value) / 3);
                }
                if (!(calculated_MES_Cell.Count == cellVoltCnt))
                {
                    ti.Value_ = _MES_CELL_DATA_CALCULATE_NG;
                    return JudgementTestItem(ti);
                }
            }
            #endregion

            #region Cell 전압 Log, 상세수집항목 만들기
            string mes_Cell_Voltage_Ref = "";

            // MES Cell 전압
            foreach (var item in MES.m_listMesCellVoltage)
            {
                LogState(LogType.Info, "MES Cell Voltage : " + item.Value.ToString());
                mes_Cell_Voltage_Ref = mes_Cell_Voltage_Ref + item.Value.ToString() + "&";
            }

            // MES Cell 전압 / 계산된 Cell 전압
            mes_Cell_Voltage_Ref = mes_Cell_Voltage_Ref + "/";

            // Calaulated Cell 전압
            foreach (var item in calculated_MES_Cell)
            {
                LogState(LogType.Info, "Calculated Cell Voltage : " + item.ToString());
                mes_Cell_Voltage_Ref = mes_Cell_Voltage_Ref + item.ToString() + "&";
            }

            mes_Cell_Voltage_Ref = mes_Cell_Voltage_Ref.Remove(mes_Cell_Voltage_Ref.Length - 1, 1);
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4071", mes_Cell_Voltage_Ref.ToString()));
            #endregion

            double deviation_Module = 0.0;
            double deviation_MES = 0.0;

            if (CellVoltageList != null && CellVoltageList.Count == cellVoltCnt)
            {
                deviation_Module = (CellVoltageList.Max() - CellVoltageList.Min()) * 1000;

                LogState(LogType.Info, string.Format("{0}(Max) - {1}(Min) = {2}",
                    CellVoltageList.Max(), CellVoltageList.Min(), deviation_Module));
            }

            deviation_MES = (calculated_MES_Cell.Max() - calculated_MES_Cell.Min()) * 1000;

            LogState(LogType.Info, string.Format("{0}(Max) - {1}(Min) = {2}",
                    calculated_MES_Cell.Max(), calculated_MES_Cell.Min(), deviation_MES));

            LogState(LogType.Info, string.Format("{0}(Module DEV) - {1}(MES DEV) = {2}",
                    deviation_Module, deviation_MES, deviation_Module - deviation_MES));

            ti.Value_ = deviation_Module - deviation_MES;

            return JudgementTestItem(ti);
        }
    }
}
