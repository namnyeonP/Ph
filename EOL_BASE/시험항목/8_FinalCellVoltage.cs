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
        List<double> FinalCellVoltageList;
        double FinalcellVoltCnt = 0;
        List<double> finalTempList;
        double finalTempCnt = 2;

        double discharge10sTemp1 = 0.0;
        double discharge10sTemp2 = 0.0;

        double mLastResistanceTemp1 = 0.0;
        double mLastResistanceTemp2 = 0.0;

        double reststemp1 = 0.0;
        double reststemp2 = 0.0;

        double mTemp1RiseDCIR = 0.0;
        double mTemp2RiseDCIR = 0.0;

        public bool Delta_Temp_T1(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }
            //double deltaTemp = discharge10sTemp1 - thermistor1;
            //ti.Value_ = deltaTemp;
            //LogState(LogType.Info, string.Format("Discharge10s Temp 1 : {0}, OCV Temp 1 : {1}", discharge10sTemp1, thermistor1));
            mTemp1RiseDCIR = mLastResistanceTemp1 - reststemp1;

            ti.Value_ = mTemp1RiseDCIR;
            LogState(LogType.Info, string.Format("Module Temperature1 Rise DCIR : {0}, NormalResistance1 : {1}, NormalTemp1 : {2}", mTemp1RiseDCIR, mLastResistanceTemp1, reststemp1));
            LogState(LogType.Info, string.Format("Rest 39.5s Temp 1 : {0}, OCV Temp 1 : {1}", mLastResistanceTemp1, reststemp1));
            return JudgementTestItem(ti);
        }
        public bool Delta_Temp_T2(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }
            //double deltaTemp = discharge10sTemp2 - thermistor2;
            //ti.Value_ = deltaTemp;
            //LogState(LogType.Info, string.Format("Discharge10s Temp 2 : {0}, OCV Temp 2 : {1}", discharge10sTemp2, thermistor2));
            mTemp2RiseDCIR = mLastResistanceTemp2 - reststemp2;
            ti.Value_ = mTemp2RiseDCIR;

            LogState(LogType.Info, string.Format("Module Temperature1 Rise DCIR : {0}, NormalResistance1 : {1}, NormalTemp1 : {2}", mTemp2RiseDCIR, mLastResistanceTemp2, reststemp2));
            LogState(LogType.Info, string.Format("Rest 39.5s Temp 2 : {0}, OCV Temp 2 : {1}", mLastResistanceTemp2, reststemp2));
            return JudgementTestItem(ti);
        }
        
        public bool Final_Module_Voltage(TestItem ti)
        {
            int cellVoltCnt = 0;
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

            if (isLineFlag)
            {
                #region DAQ970A
                if (daq970_1 == null)
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }

                SetChannels(out measString, out cellVoltCnt, out reccnt);
                FinalcellVoltCnt = cellVoltCnt;

                #region measure Each Resistance
                if (!daq970_1.MeasRes_Multi(out resList, measString, cellVoltCnt + 1))
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }

                var resdti = new DetailItems() { Key = "W2MLMTE4045" };

                string resStr = "";
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

                FinalCellVoltageList = new List<double>();
                FinalCellVoltageList = voltList;

                string voltStr = "";
                foreach (var item in voltList)
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
                if (keysight == null)
                {
                    ti.Value_ = "NotConnected";
                    return JudgementTestItem(ti);
                }
                keysight.rtstring = "";

                SetChannels(out measString, out cellVoltCnt, out reccnt);
                FinalcellVoltCnt = cellVoltCnt;

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

                var resdti = new DetailItems() { Key = "W2MLMTE4045" };

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

                FinalCellVoltageList = new List<double>();
                string voltstr = "";
                foreach (var item in vArr)
                {
                    double dv = 0;
                    if (double.TryParse(item, out dv))
                    {
                        FinalCellVoltageList.Add(dv);
                        voltstr += dv.ToString() + ",";
                    }
                }
                LogState(LogType.RESPONSE, "KeysightDMM Volt:" + voltstr);
                #endregion
               
                #endregion
            }

            #region Filtering by res Val
            var resHighLimit = LINE_OPEN_CHECK_UPPER_LIMIT * 1000;
            var resLowLimit = LINE_OPEN_CHECK_LOWER_LIMIT * 1000;

            for (int i = 0; i < resList.Count - 1; i++)
            {
                if (resList[i] > resHighLimit || resList[i] < resLowLimit)
                {
                    FinalCellVoltageList[i] = 0;
                }
            }
            #endregion

            ti.Value_ = FinalCellVoltageList.Last();

            FinalCellVoltageList.Remove(FinalCellVoltageList.Last());

            SetOcvCellVoltage(FinalCellVoltageList);

            return JudgementTestItem(ti);
        }
        public bool Final_Cell1_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[0];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Final_Cell2_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[1];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Final_Cell3_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[2];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Final_Cell4_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[3];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Final_Cell5_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[4];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Final_Cell6_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[5];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Final_Cell7_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[6];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Final_Cell8_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[7];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Final_Cell9_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[8];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Final_Cell10_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[9];
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }
        public bool Final_Cell_Delta_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                var max = double.Parse(FinalCellVoltageList.Max().ToString("F3"));//190830 ljs request
                var min = double.Parse(FinalCellVoltageList.Min().ToString("F3"));

                double deviation = max - min;

                LogState(LogType.Info, string.Format("{0}(Max) - {1}(Min) = {2}",
                    max, min, deviation));
                ti.Value_ = deviation * 1000;
            }
            else
            {
                ti.Value_ = 0;
            }

            return JudgementTestItem(ti);
        }

        // 2019.05.08 jeonhj's comment
        // kih 요청사항
        // 2.     Final delta temperature 시점 변경
        // 현재 Final delta temperature(charge 가 끝난 후 T1-T2의 값)의 측정 시점은 charge가 끝난 후 입니다.
        //해당 측정 시점을 위와 같이 discharge 10s로 통일하도록 요청드립니다. (기존에 상세수집항목을 위해 측정하기 때문에 어렵지 않을 것으로 보입니다.)
        public bool Final_Cell_Delta_Temperature(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            // 방전 후 10s값으로 계산
            double deltaTemp = mLastResistanceTemp1 - mLastResistanceTemp2;
            ti.Value_ = deltaTemp;

            LogState(LogType.Info, string.Format("Final Delta Temperature: {0} - {1} = {2}", mLastResistanceTemp1, mLastResistanceTemp2, deltaTemp));

            return JudgementTestItem(ti);
        }

        // 2019.05.08 jeonhj's comment
        // kih 요청사항
        // 3.     Final delta temp1,2 수집항목 추가
        // 위와 같이 discharge 10s 때 측정하여 상세수집항목에 기록하던 것을 수집항목에 추가하여 합/불판정 하도록 변경 요청드립니다.
        public bool Final_Temp1(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            //double resultTemp = 0.0;

            //Calculate(discharge10sTemp1, out resultTemp);

            //ti.Value_ = discharge10sTemp1;
            //LogState(LogType.Info, string.Format("Final Temp1: {0}, (discharge 10s value).", discharge10sTemp1));

            int cellVoltCnt = 0;

            switch (localTypes)
            {
                case 0: cellVoltCnt = 8; break; //4P8S
                case 1: cellVoltCnt = 8; break; //4P8S_rev
                case 2: cellVoltCnt = 7; break; //4P7S
                case 3: cellVoltCnt = 8; break; //3P8S
                case 4: cellVoltCnt = 10; break; //3P10S
                case 5: cellVoltCnt = 10; break; //3P10S_rev
                default: cellVoltCnt = 0; break;
            }

            double temp = mTemp1RiseDCIR + coefficientT * (thermistor1 - 25) + coefficientV * (ocv_voltage - (3.585) * cellVoltCnt);

            ti.Value_ = temp;
            LogState(LogType.Info, string.Format("{0} + {1} * ({2} - 25) + {3} * ({4} - (3.585 * {5}))",
                mTemp1RiseDCIR, coefficientT, thermistor1, coefficientV, ocv_voltage, cellVoltCnt));

            return JudgementTestItem(ti);
        }

        public bool Final_Temp2(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            //double resultTemp = 0.0;

            //Calculate(discharge10sTemp2, out resultTemp);

            //ti.Value_ = discharge10sTemp2;
            //LogState(LogType.Info, string.Format("Final Temp2: {0}, (discharge 10s value).", discharge10sTemp2));

            int cellVoltCnt = 0;

            switch (localTypes)
            {
                case 0: cellVoltCnt = 8; break; //4P8S
                case 1: cellVoltCnt = 8; break; //4P8S_rev
                case 2: cellVoltCnt = 7; break; //4P7S
                case 3: cellVoltCnt = 8; break; //3P8S
                case 4: cellVoltCnt = 10; break; //3P10S
                case 5: cellVoltCnt = 10; break; //3P10S_rev
                default: cellVoltCnt = 0; break;
            }

            double temp = mTemp2RiseDCIR + coefficientT * (thermistor2 - 25) + coefficientV * (ocv_voltage - (3.585) * cellVoltCnt);

            ti.Value_ = temp;
            LogState(LogType.Info, string.Format("{0} + {1} * ({2} - 25) + {3} * ({4} - (3.585 * {5}))",
                mTemp2RiseDCIR, coefficientT, thermistor2, coefficientV, ocv_voltage, cellVoltCnt));

            return JudgementTestItem(ti);
        }

        public bool CorrectedTemperature1RiseDCIR(TestItem testItem)
        {
            // [Th1 온도@ n초] - [Th1 온도 @ 1초], n초는 레지스트리로 운영 할 것

            isProcessingUI(testItem);

            if (isStop || ispause)
            {
                testItem.Value_ = _EMG_STOPPED;

                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(testItem);
            }

            testItem.Value_ = null;

            try
            {
                Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\EOL_Trigger");

                int whichPoint = int.Parse(regKey.GetValue("USER_CORRECTED_TEMP1_RISE_DCIR").ToString()); // n초, 1base

                if(whichPoint <= 0 || whichPoint > 상세저항1.Reportitems.Count)
                {
                    throw new Exception("Please Check REG : USER_CORRECTED_TEMP1_RISE_DCIR" +
                                        ", whichPoint : " +
                                        whichPoint.ToString() +
                                        ", 1.Reportitems.Count : " +
                                        상세저항1.Reportitems.Count.ToString());
                }

                double fisrtData = double.Parse(상세저항1.Reportitems[0].ToString());

                double whichData = double.Parse(상세저항1.Reportitems[whichPoint - 1].ToString());

                testItem.Value_ = whichData - fisrtData;

                LogState(LogType.Info, "whichPoint : " +
                                       whichPoint.ToString() +
                                       ", fisrtPoint : 1");

                LogState(LogType.Info, whichData.ToString() +
                                       " - " +
                                       fisrtData.ToString() +
                                       " = " +
                                       testItem.Value_.ToString());
            }
            catch(Exception ec)
            {
                LogState(LogType.Fail, "Exception : " + ec.Message);
            }

            if(testItem.Value_ == null)
            {
                LogState(LogType.Fail, "Result Value Is Null");

                testItem.Value_ = "Not Calculate";
            }

            return JudgementTestItem(testItem);
        }

        public bool CorrectedTemperature2RiseDCIR(TestItem testItem)
        {
            // [Th2 온도@ n초] - [Th2 온도 @ 1초], n초는 레지스트리로 운영 할 것

            isProcessingUI(testItem);

            if (isStop || ispause)
            {
                testItem.Value_ = _EMG_STOPPED;

                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(testItem);
            }

            testItem.Value_ = null;

            try
            {
                Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\EOL_Trigger");

                int whichPoint = int.Parse(regKey.GetValue("USER_CORRECTED_TEMP2_RISE_DCIR").ToString()); // n초, 1base

                if (whichPoint <= 0 || whichPoint > 상세저항2.Reportitems.Count)
                {
                    throw new Exception("Please Check REG : USER_CORRECTED_TEMP2_RISE_DCIR" +
                                        ", whichPoint : " +
                                        whichPoint.ToString() +
                                        ", 2.Reportitems.Count : " +
                                        상세저항2.Reportitems.Count.ToString());
                }

                double fisrtData = double.Parse(상세저항2.Reportitems[0].ToString());

                double whichData = double.Parse(상세저항2.Reportitems[whichPoint - 1].ToString());

                testItem.Value_ = whichData - fisrtData;

                LogState(LogType.Info, "whichPoint : " +
                                       whichPoint.ToString() +
                                       ", fisrtPoint : 1");

                LogState(LogType.Info, whichData.ToString() +
                                       " - " +
                                       fisrtData.ToString() +
                                       " = " +
                                       testItem.Value_.ToString());
            }
            catch (Exception ec)
            {
                LogState(LogType.Fail, "Exception : " + ec.Message);
            }

            if (testItem.Value_ == null)
            {
                LogState(LogType.Fail, "Result Value Is Null");

                testItem.Value_ = "Not Calculate";
            }

            return JudgementTestItem(testItem);
        }
    }
}
