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
        List<double> FinalCellVoltageList;
        double FinalcellVoltCnt = 0;
        List<double> finalTempList;
        double finalTempCnt = 2;

        double discharge10sTemp1 = 0.0;
        double discharge10sTemp2 = 0.0;

        public bool Delta_Temp_T1(TestItem ti)
        {
            isProcessingUI(ti);

            double deltaTemp = discharge10sTemp1 - thermistor1;
            ti.Value_ = deltaTemp;
            LogState(LogType.Info, string.Format("Discharge10s Temp 1 : {0}, OCV Temp 1 : {1}", discharge10sTemp1, thermistor1));
            return JudgementTestItem(ti);
        }
        public bool Delta_Temp_T2(TestItem ti)
        {
            isProcessingUI(ti);

            double deltaTemp = discharge10sTemp2 - thermistor2;
            ti.Value_ = deltaTemp;
            LogState(LogType.Info, string.Format("Discharge10s Temp 2 : {0}, OCV Temp 2 : {1}", discharge10sTemp2, thermistor2));
            return JudgementTestItem(ti);
        }
        
        
        public bool Final_Cell1_Voltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[0];
            }
            return JudgementTestItem(ti);
        }
        public bool Final_Cell2_Voltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[1];
            }
            return JudgementTestItem(ti);
        }
        public bool Final_Cell3_Voltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[2];
            }
            return JudgementTestItem(ti);
        }
        public bool Final_Cell4_Voltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[3];
            }
            return JudgementTestItem(ti);
        }
        public bool Final_Cell5_Voltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[4];
            }
            return JudgementTestItem(ti);
        }
        public bool Final_Cell6_Voltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[5];
            }
            return JudgementTestItem(ti);
        }
        public bool Final_Cell7_Voltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[6];
            }
            return JudgementTestItem(ti);
        }
        public bool Final_Cell8_Voltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[7];
            }
            return JudgementTestItem(ti);
        }
        public bool Final_Cell9_Voltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[8];
            }
            return JudgementTestItem(ti);
        }
        public bool Final_Cell10_Voltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                ti.Value_ = FinalCellVoltageList[9];
            }
            return JudgementTestItem(ti);
        }
        public bool Final_Cell_Delta_Voltage(TestItem ti)
        {
            isProcessingUI(ti);
            if (FinalCellVoltageList != null && FinalCellVoltageList.Count == FinalcellVoltCnt)
            {
                double deviation = FinalCellVoltageList.Max() - FinalCellVoltageList.Min();

                LogState(LogType.Info, string.Format("{0}(Max) - {1}(Min) = {2}",
                    FinalCellVoltageList.Max(), FinalCellVoltageList.Min(), deviation));
                ti.Value_ = deviation * 1000;
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
            //if (keysight == null)
            //{
            //    ti.Value_ = "NotConnected";
            //    return JudgementTestItem(ti);
            //}

            //keysight.rtstring = "";
            //finalTempCnt = 2;
            //keysight.MeasTemp("318,319");

            //int rec = keysight.sp.BytesToRead;

            //int cnt = 0;
            //while (rec < 33)//33
            //{
            //    Thread.Sleep(100);
            //    rec = keysight.sp.BytesToRead;
            //    cnt += 100;
            //    if (cnt == 2000)
            //    {
            //        keysight.MeasTemp("318,319");

            //        rec = keysight.sp.BytesToRead;

            //        cnt = 0;
            //        while (rec < 33)//33
            //        {
            //            Thread.Sleep(100);
            //            rec = keysight.sp.BytesToRead;
            //            cnt += 100;
            //            if (cnt == 2000)
            //            {
            //                ti.Value_ = _DEVICE_NOT_READY;
            //                return JudgementTestItem(ti);
            //            }
            //        }
            //        break;
            //    }
            //}
            ////받은 후에 데이터 파싱
            //byte[] bt = new byte[rec];
            //keysight.sp.Read(bt, 0, rec);

            //keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);
            //LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

            //var vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            //finalTempList = new List<double>();
            //foreach (var item in vArr)
            //{
            //    double dv = 0;
            //    if (double.TryParse(item, out dv))
            //    {
            //        finalTempList.Add(dv * 0.001);
            //    }
            //}

            //var tempTher1 = 0.0;
            //if (finalTempList != null && finalTempList.Count == finalTempCnt)
            //{
            //    double resultTemp1 = 0.0;
            //    double resultTemp2 = 0.0;
            //    Calculate(finalTempList[0], out resultTemp1);
            //    Calculate(finalTempList[1], out resultTemp2);

            //    tempTher1 = resultTemp1 - resultTemp2;
            //    LogState(LogType.Info, string.Format("{0} - {1} = {2}", resultTemp1, resultTemp2, tempTher1));
            //}            

            //ti.Value_ = tempTher1;

            // 방전 후 10s값으로 계산
            double deltaTemp = discharge10sTemp1 - discharge10sTemp2;

            ti.Value_ = deltaTemp;

            LogState(LogType.Info, string.Format("Final Delta Temperature: {0} - {1} = {2}", discharge10sTemp1, discharge10sTemp2, deltaTemp));

            return JudgementTestItem(ti);
        }

        // 2019.05.08 jeonhj's comment
        // kih 요청사항
        // 3.     Final delta temp1,2 수집항목 추가
        // 위와 같이 discharge 10s 때 측정하여 상세수집항목에 기록하던 것을 수집항목에 추가하여 합/불판정 하도록 변경 요청드립니다.
        public bool Final_Temp1(TestItem ti)
        {
            isProcessingUI(ti);

            //double resultTemp = 0.0;

            //Calculate(discharge10sTemp1, out resultTemp);

            ti.Value_ = discharge10sTemp1;
            LogState(LogType.Info, string.Format("Final Temp1: {0}, (discharge 10s value).", discharge10sTemp1));

            return JudgementTestItem(ti);
        }

        public bool Final_Temp2(TestItem ti)
        {
            isProcessingUI(ti);

            //double resultTemp = 0.0;

            //Calculate(discharge10sTemp2, out resultTemp);

            ti.Value_ = discharge10sTemp2;
            LogState(LogType.Info, string.Format("Final Temp2: {0}, (discharge 10s value).", discharge10sTemp2));

            return JudgementTestItem(ti);
        }
    }
}
