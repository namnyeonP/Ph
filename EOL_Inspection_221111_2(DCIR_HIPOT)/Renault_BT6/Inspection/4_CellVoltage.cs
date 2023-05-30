using Renault_BT6.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Renault_BT6
{
    public partial class MainWindow
    {
        public bool Module_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ.isAlive == false)
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }

                // Cycler SetVoltageSensingOn
                Device.Cycler.ChgDisControl.SetVoltageSensingOn(true);
                double volt = 0;
                int timeChk = 0;

                Thread.Sleep(1000);

                double CyclerVolt = Device.Cycler._Voltage;

                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    //volt = Device.KeysightDAQ.GetMeasVolt(BatteryInfo.ModuleCH);
                    volt = Device.KeysightDAQ.MeasVolt_Single(BatteryInfo.ModuleCH);
                }

                LogState(LogType.Info, string.Format("Cycler Voltage {0}", CyclerVolt));
                LogState(LogType.Info, string.Format("DMM Voltage {0}", volt));

                ti.Value_ = volt;

                // control limit

                SetContorlLimitData(14, ti.Value_.ToString());
                Device.Cycler.ChgDisControl.SetVoltageSensingOn(false);
                /*
                while (true)
                {
                    if (Device.KeysightDAQ != null && Device.KeysightDAQ.Alive)
                    {
                        volt = Device.KeysightDAQ.GetMeasVolt(BatteryInfo.ModuleCH);
                    }

                    if (volt > 10)
                    {
                        ti.Value_ = volt;
                        break;
                    }
                    else if (timeChk > 3000)
                        break;

                    timeChk += 100;
                    Thread.Sleep(200);
                }
                */
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }

        List<double> cvArr = new List<double>();
        List<double> cvResArr = new List<double>();

        public bool Cell1_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            ti.refValues_.Clear();
            try
            {
                LogState(LogType.Info, "Cell Resistance ");

                if (Device.KeysightDAQ.isAlive == false)
                {
                    ti.Value_ = _DEVICE_NOT_READY;

                    return JudgementTestItem(ti);
                }

                cvArr.Clear();
                cvResArr.Clear();

                if (IsManual == true)
                {
                    cvResArr = Device.KeysightDAQ.MeasVolt_ResManual(BatteryInfo.CellCH);
                }
                else
                {
                    cvResArr = mSecondPreCheckListCellRes;
                }

                string str = "";
                for (int i = 0; i < cvResArr.Count; i++)
                {
                    if (i == cvResArr.Count - 1)
                    {
                        str += cvResArr[i];
                    }
                    else
                    {
                        str += cvResArr[i] + "&";
                    }
                }

                if (IsManual == true)
                {
                    bool trigger = false;
                    foreach (var tempRes in cvResArr)
                    {
                        if (EOL.CellLineLowLimit * 1000 > tempRes || EOL.CellLineUpperLimit * 1000 < tempRes)
                        {
                            // 저항측정시 Upper, Lower안에 저항이 안들어 오면 비정상
                            trigger = true;
                        }
                    }

                    if (trigger)
                    {
                        LogState(LogType.Info, "Cell Resistance retry");
                        Thread.Sleep(500);
                        cvResArr.Clear();
                        cvResArr = Device.KeysightDAQ.MeasVolt_Res(BatteryInfo.CellCH);
                        str = "";

                        for (int i = 0; i < cvResArr.Count; i++)
                        {
                            if (i == cvResArr.Count - 1)
                            {
                                str += cvResArr[i];
                            }
                            else
                            {
                                str += cvResArr[i] + "&";
                            }
                        }
                    }
                }

                LogState(LogType.RESPONSE, "KeysightDMM RES:" + str.ToString());

                if (IsManual == true)
                {
                    Thread.Sleep(300);

                    cvArr = Device.KeysightDAQ.MeasVolt_MultiCh(BatteryInfo.CellCH);
                }
                else
                {
                    cvArr = mSecondPreCheckListCellVolt;
                }
                
                if (EOL.CellLineLowLimit * 1000 > cvResArr[0] || cvResArr[0] > EOL.CellLineUpperLimit * 1000)
                {
                    // 저항값이 비정상이면 0
                    ti.Value_ = cvArr[0] = 0;
                }
                else
                {
                    // 저항값이 정상이면 측정된 전압값을 입력
                    ti.Value_ = cvArr[0].ToString();

                }

                // control limit
                SetContorlLimitData(1, ti.Value_.ToString());

                //200630 mes 수신값 비교구문 추가
                //제어값이 0이면 역방향 1이면 정방향 비교 필요
                ti.refValues_.Add(MakeDetailItem(EOL.MesID.MES_CELL_ORDER, mes_cv_order ? "0" : "1"));

                var dti = new DetailItems() { Key = EOL.MesID.MES_CELL_DEV_LIST };
                var dti1 = new DetailItems() { Key = EOL.MesID.MES_CELL_ID };
                var dti2 = new DetailItems() { Key = EOL.MesID.MES_CELL_FROM_LGQ };

                //200706 wjs EOLlist의 Cell voltage 1함수의 digit length로 처리하는 구문추가(YKS)
                int digit = 0;
                bool rst = int.TryParse(ti.DigitLength, out digit);

                if (!rst)
                    digit = 3;

                for (int i = 0; i < cvArr.Count; i++)
                    cvArr[i] = Math.Round(cvArr[i], digit, MidpointRounding.AwayFromZero);

                int cnt = 0;
                if (IsMESskip != true)  //200706 wjs add
                {
                    if (Device.MES.mes_CellList.Count == 12)
                    {
                        foreach (var dic in Device.MES.mes_CellList)
                        {
                            var dev = cvArr[cnt] - dic.Value;
                            dti.Reportitems.Add(dev);

                            dti1.Reportitems.Add(dic.Key);
                            dti2.Reportitems.Add(dic.Value);

                            LogState(LogType.Info, string.Format("dev:{0} / MES ID:{1} / MES CV:{2}", dev, dic.Key, dic.Value));
                            cnt++;
                        }
                    }
                    else
                    {
                        LogState(LogType.Info, "no matched data. cnt:" + Device.MES.mes_CellList.Count.ToString());
                    }
                }
                else
                {
                    LogState(LogType.Info, "MES Skip mode. Write DMM Measure val.");
                    foreach (var cell_volt in cvArr)
                    {
                        var dev = cell_volt;
                        dti.Reportitems.Add(dev);

                        //dti1.Reportitems.Add(dic.Key);
                        //dti2.Reportitems.Add(dic.Value);

                        LogState(LogType.Info, string.Format("DMM Volt:{0}", dev));
                        //cnt++;
                    }
                }

                ti.refValues_.Add(dti);
                ti.refValues_.Add(dti1);
                ti.refValues_.Add(dti2);


            }
            catch (Exception ex)
            {

            }

            return JudgementTestItem(ti);
        }
        public bool Cell2_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    int index = 1;

                    if (EOL.CellLineLowLimit * 1000 > cvResArr[index] || cvResArr[index] > EOL.CellLineUpperLimit * 1000)
                    {
                        // 저항값이 비정상이면 0
                        ti.Value_ = cvArr[index] = 0;
                    }
                    else
                    {
                        // 저항값이 정상이면 측정된 전압값을 입력
                        ti.Value_ = cvArr[index].ToString();
                    }
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    LogState(LogType.Info, "No Data");
                }

                // control limit
                SetContorlLimitData(2, ti.Value_.ToString());
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }
        public bool Cell3_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    int index = 2;

                    if (EOL.CellLineLowLimit * 1000 > cvResArr[index] || cvResArr[index] > EOL.CellLineUpperLimit * 1000)
                    {
                        // 저항값이 비정상이면 0
                        ti.Value_ = cvArr[index] = 0;
                    }
                    else
                    {
                        // 저항값이 정상이면 측정된 전압값을 입력
                        ti.Value_ = cvArr[index].ToString();
                    }
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    LogState(LogType.Info, "No Data");
                }

                // control limit
                SetContorlLimitData(3, ti.Value_.ToString());
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }
        public bool Cell4_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    int index = 3;

                    if (EOL.CellLineLowLimit * 1000 > cvResArr[index] || cvResArr[index] > EOL.CellLineUpperLimit * 1000)
                    {
                        // 저항값이 비정상이면 0
                        ti.Value_ = cvArr[index] = 0;
                    }
                    else
                    {
                        // 저항값이 정상이면 측정된 전압값을 입력
                        ti.Value_ = cvArr[index].ToString();
                    }
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    LogState(LogType.Info, "No Data");
                }
                // control limit
                SetContorlLimitData(4, ti.Value_.ToString());
            }
            catch
            {

            }


            return JudgementTestItem(ti);
        }
        public bool Cell5_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    int index = 4;

                    if (EOL.CellLineLowLimit * 1000 > cvResArr[index] || cvResArr[index] > EOL.CellLineUpperLimit * 1000)
                    {
                        // 저항값이 비정상이면 0
                        ti.Value_ = cvArr[index] = 0;
                    }
                    else
                    {
                        // 저항값이 정상이면 측정된 전압값을 입력
                        ti.Value_ = cvArr[index].ToString();
                    }
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    LogState(LogType.Info, "No Data");
                }
            }
            catch
            {

            }
            // control limit
            SetContorlLimitData(5, ti.Value_.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell6_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    int index = 5;

                    if (EOL.CellLineLowLimit * 1000 > cvResArr[index] || cvResArr[index] > EOL.CellLineUpperLimit * 1000)
                    {
                        // 저항값이 비정상이면 0
                        ti.Value_ = cvArr[index] = 0;
                    }
                    else
                    {
                        // 저항값이 정상이면 측정된 전압값을 입력
                        ti.Value_ = cvArr[index].ToString();
                    }
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    LogState(LogType.Info, "No Data");
                }
            }
            catch
            {

            }

            // control limit
            SetContorlLimitData(6, ti.Value_.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell7_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    int index = 6;

                    if (EOL.CellLineLowLimit * 1000 > cvResArr[index] || cvResArr[index] > EOL.CellLineUpperLimit * 1000)
                    {
                        // 저항값이 비정상이면 0
                        ti.Value_ = cvArr[index] = 0;
                    }
                    else
                    {
                        // 저항값이 정상이면 측정된 전압값을 입력
                        ti.Value_ = cvArr[index].ToString();
                    }
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    LogState(LogType.Info, "No Data");
                }

                // control limit
                SetContorlLimitData(7, ti.Value_.ToString());
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }
        public bool Cell8_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    int index = 7;

                    if (EOL.CellLineLowLimit * 1000 > cvResArr[index] || cvResArr[index] > EOL.CellLineUpperLimit * 1000)
                    {
                        // 저항값이 비정상이면 0
                        ti.Value_ = cvArr[index] = 0;
                    }
                    else
                    {
                        // 저항값이 정상이면 측정된 전압값을 입력
                        ti.Value_ = cvArr[index].ToString();
                    }
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    LogState(LogType.Info, "No Data");
                }
            }
            catch
            {

            }

            // control limit
            SetContorlLimitData(8, ti.Value_.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell9_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    int index = 8;

                    if (EOL.CellLineLowLimit * 1000 > cvResArr[index] || cvResArr[index] > EOL.CellLineUpperLimit * 1000)
                    {
                        // 저항값이 비정상이면 0
                        ti.Value_ = cvArr[index] = 0;
                    }
                    else
                    {
                        // 저항값이 정상이면 측정된 전압값을 입력
                        ti.Value_ = cvArr[index].ToString();
                    }
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    LogState(LogType.Info, "No Data");
                }
            }
            catch
            {

            }

            // control limit
            SetContorlLimitData(9, ti.Value_.ToString());

            return JudgementTestItem(ti);
        }
        public bool Cell10_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    int index = 9;

                    if (EOL.CellLineLowLimit * 1000 > cvResArr[index] || cvResArr[index] > EOL.CellLineUpperLimit * 1000)
                    {
                        // 저항값이 비정상이면 0
                        ti.Value_ = cvArr[index] = 0;
                    }
                    else
                    {
                        // 저항값이 정상이면 측정된 전압값을 입력
                        ti.Value_ = cvArr[index].ToString();
                    }
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    LogState(LogType.Info, "No Data");
                }
            }
            catch
            {

            }


            // control limit
            SetContorlLimitData(10, ti.Value_.ToString());

            return JudgementTestItem(ti);
        }

        public bool Cell11_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    int index = 10;

                    if (EOL.CellLineLowLimit * 1000 > cvResArr[index] || cvResArr[index] > EOL.CellLineUpperLimit * 1000)
                    {
                        // 저항값이 비정상이면 0
                        ti.Value_ = cvArr[index] = 0;
                    }
                    else
                    {
                        // 저항값이 정상이면 측정된 전압값을 입력
                        ti.Value_ = cvArr[index].ToString();
                    }
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    LogState(LogType.Info, "No Data");
                }

                // control limit
                SetContorlLimitData(11, ti.Value_.ToString());
            }
            catch
            {

            }
            return JudgementTestItem(ti);
        }

        public bool Cell12_Voltage(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                {
                    int index = 11;

                    if (EOL.CellLineLowLimit * 1000 > cvResArr[index] || cvResArr[index] > EOL.CellLineUpperLimit * 1000)
                    {
                        // 저항값이 비정상이면 0
                        ti.Value_ = cvArr[index] = 0;
                    }
                    else
                    {
                        // 저항값이 정상이면 측정된 전압값을 입력
                        ti.Value_ = cvArr[index].ToString();
                    }

                    // control limit
                    SetContorlLimitData(12, ti.Value_.ToString());
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    LogState(LogType.Info, "No Data");
                }
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }

        public bool Cell_Voltage_Deviation(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                if (Device.KeysightDAQ.isAlive == false)
                {
                    ti.Value_ = _DEVICE_NOT_READY;

                    return JudgementTestItem(ti);
                }

                if (cvArr != null && cvArr.Count == BatteryInfo.CellCount)
                {
                    double deviation = cvArr.Max() - cvArr.Min();

                    LogState(LogType.Info, string.Format("{0}(Max) - {1}(Min) = {2}",
                            cvArr.Max(), cvArr.Min(), deviation));

                    ti.Value_ = deviation * 1000;
                }

                // control limit
                SetContorlLimitData(13, ti.Value_.ToString());

                #region MES 상세 수집 
                ti.refValues_.Add(MakeDetailItem(EOL.MesID.CellVoltMax, cvArr.Max().ToString()));
                ti.refValues_.Add(MakeDetailItem(EOL.MesID.CellVoltMin, cvArr.Min().ToString()));
                #endregion
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }

        public bool Cell_Voltage_Deviation_FromCellInspection(TestItem ti)
        {
            isProcessingUI(ti);

            double mesCellVoltDEV = 0.0;
            double moduleCellVoltDEV = 0.0;

            double moduleDEV_mesDEV_Result = 0.0;

            List<double> dListMESCellVoltages1P12S = new List<double>();

            try
            {
                if (Device.KeysightDAQ.isAlive == false)
                {
                    ti.Value_ = _DEVICE_NOT_READY;

                    return JudgementTestItem(ti);
                }

                if (cvArr != null && cvArr.Count == BatteryInfo.CellCount)
                {
                    double deviation = cvArr.Max() - cvArr.Min();

                    moduleCellVoltDEV = deviation * 1000;

                    LogState(LogType.Info, string.Format("( {0}(Max) - {1}(Min) ) *1000 = {2}",
                            cvArr.Max(), cvArr.Min(), moduleCellVoltDEV));
                }
                else
                {
                    ti.Value_ = _DEVICE_NOT_READY;

                    return JudgementTestItem(ti);
                }

                if (Device.MES.mes_CellList.Count == 12)
                {
                    foreach (var dic in Device.MES.mes_CellList)
                    {
                        dListMESCellVoltages1P12S.Add(dic.Value);
                    }

                    mesCellVoltDEV = (dListMESCellVoltages1P12S.Max() - dListMESCellVoltages1P12S.Min()) * 1000;

                    LogState(LogType.Info, string.Format("( {0}(Max) - {1}(Min) ) *1000 = {2}",
                            dListMESCellVoltages1P12S.Max(), dListMESCellVoltages1P12S.Min(), mesCellVoltDEV));
                }
                else
                {
                    LogState(LogType.Info, "no matched data. cnt:" + Device.MES.mes_CellList.Count.ToString());
                    ti.Value_ = "Not Received MES Cell DATA";
                    return JudgementTestItem(ti);
                }

                moduleDEV_mesDEV_Result = moduleCellVoltDEV - mesCellVoltDEV;
                LogState(LogType.Info, string.Format("{0}(ModuleCellVoltageDEV) - {1}(MESCellVoltageDEV) = {2}",
                            moduleCellVoltDEV, mesCellVoltDEV, moduleDEV_mesDEV_Result));

                ti.Value_ = moduleDEV_mesDEV_Result;
            }
            catch
            {
            }

            return JudgementTestItem(ti);
        }
    }
}
