using Renault_BT6.클래스;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Media;
using Renault_BT6.모듈;
using System.Diagnostics;
using System.Windows;

namespace Renault_BT6
{
    public partial class MainWindow
    {
        bool mes_cv_order = true;
        private void SetCellVoltageOrder()
        {
            LogState(LogType.Info, "MES Cell voltage Order : " + (mes_cv_order == true ? "NORMAL" : "REVERSE"));
            LogState(LogType.Info, "NZ : " + (mes_cv_order == true ? "NORMAL" : "REVERSE"));

            if (!mes_cv_order)
            {
                Dictionary<string, double> tmepdic = new Dictionary<string, double>();
                var arr = Device.MES.mes_CellList.Reverse();
                foreach (var item in arr)
                {
                    tmepdic.Add(item.Key, item.Value);
                }
                Device.MES.mes_CellList = tmepdic;
            }

        }
        Thread finishedThread;

        private bool GetControlItemFromMES()
        {
            LogState(LogType.Info, "GetControlItemFromMES");

            //MES에 제어항목을 요청한다
            string controlItem = Device.MES.GetProcessControlParameterRequest(
                                                            this.modelList[selectedIndex].EquipId,
                                                            this.modelList[selectedIndex].ProductCode,
                                                            this.modelList[selectedIndex].ProcessID);

            if (controlItem == string.Empty || controlItem == "NG")
            {
                LogState(LogType.Fail, "GetControlItemFromMES controlItem NG");
                return false;
            }

            try
            {
                LogState(LogType.Info, "GetControlItemFromMES");

                //받은 데이터를 파싱한다
                string[] parsed = controlItem.Split('@');
                var str = parsed[0].Split(new string[] { "CTRLITEM" }, StringSplitOptions.RemoveEmptyEntries);
                parsed[0] = "CTRLITEM" + str[1];
                parsed[parsed.Length - 1] = parsed[parsed.Length - 1].Remove(parsed[parsed.Length - 1].Length - 4, 4);

                foreach (string item in parsed)
                {
                    string[] splitter = new string[] { "^", "=" };
                    var arr = item.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                    // 2019.07.08 jeonhj
                    // arr[3]의 데이터가 존재할때만 데이터를 갱신한다.
                    // 없는 데이터를 갱신하다보니 ControlList에 저장하는 부분에서 문제가 생기는 것으로 보임.
                    if (arr[3] != null)
                    {
                        switch (arr[1])
                        {
                            // HIPOT List
                            case MesControlID.BushingPlusVolt:
                                EOL.IR_Recipe.BushingPlus.Volt = int.Parse(arr[3]);
                                break;
                            case MesControlID.BushingPlusTime:
                                EOL.IR_Recipe.BushingPlus.Time = int.Parse(arr[3]);
                                break;
                            case MesControlID.CoolingPlatePlusVolt:
                                EOL.IR_Recipe.CoolingPlatePlus.Volt = int.Parse(arr[3]);
                                break;
                            case MesControlID.CoolingPlatePlusTime:
                                EOL.IR_Recipe.CoolingPlatePlus.Time = int.Parse(arr[3]);
                                break;
                            case MesControlID.BushingMinusVolt:
                                EOL.IR_Recipe.BushingMinus.Volt = int.Parse(arr[3]);
                                break;
                            case MesControlID.BushingMinusTime:
                                EOL.IR_Recipe.BushingMinus.Time = int.Parse(arr[3]);
                                break;
                            case MesControlID.CoolingPlateMinusVolt:
                                EOL.IR_Recipe.CoolingPlateMinus.Volt = int.Parse(arr[3]);
                                break;
                            case MesControlID.CoolingPlateMinusTime:
                                EOL.IR_Recipe.CoolingPlateMinus.Time = int.Parse(arr[3]);
                                break;
                            case MesControlID.IR_RANGE:
                                EOL.IR_Recipe.CoolingPlatePlus.Range = arr[3];
                                break;

                            // EOL List
                            case MesControlID.CellOpenUpper:
                                EOL.CellLineUpperLimit = int.Parse(arr[3]);
                                break;
                            case MesControlID.CellOpenLower:
                                EOL.CellLineLowLimit = int.Parse(arr[3]);
                                break;

                            case MesControlID.RestTimebeforeDischarge:
                                befDiscRestTime = arr[3];
                                break;
                            case MesControlID.DischargeCurrent:
                                discCur = arr[3];
                                break;
                            case MesControlID.DischargeTime:
                                discTime = arr[3];
                                break;
                            case MesControlID.DischargeCurrentUpperLimit:
                                discCurLimit = arr[3];
                                break;
                            case MesControlID.RestTimeafterDischarge:
                                aftDiscRestTime = arr[3];
                                break;
                            case MesControlID.ChargeCurrent:
                                charCur = arr[3];
                                break;
                            case MesControlID.ChargeTime:
                                charTime = arr[3];
                                break;
                            case MesControlID.ChargeCurrentUpperLimit:
                                charCurLimit = arr[3];
                                break;
                            case MesControlID.RestTimeafterCharge:
                                aftCharRestTime = arr[3];
                                break;
                            case MesControlID.SafetyVoltageUpperLimit:
                                safeVoltHighLimit = arr[3];
                                break;
                            case MesControlID.SafetyVoltageLowerLimit:
                                safeVoltLowLimit = arr[3];
                                break;
                            case MesControlID.CellDCIR1:
                                cellFomula1 = double.Parse(arr[3]);
                                break;
                            case MesControlID.CellDCIR2:
                                cellFomula2 = double.Parse(arr[3]);
                                break;
                            case MesControlID.CellDCIR3:
                                cellFomula3 = double.Parse(arr[3]);
                                break;
                            case MesControlID.ModuleDCIR1:
                                moduleFomula1 = double.Parse(arr[3]);
                                break;
                            case MesControlID.ModuleDCIR2:
                                moduleFomula2 = double.Parse(arr[3]);
                                break;
                            case MesControlID.ModuleDCIR3:
                                moduleFomula3 = double.Parse(arr[3]);
                                break;
                            case MesControlID.MES_CV_ORDER:
                                mes_cv_order = int.Parse(arr[3]) == 0 ? true : false;
                                break;

                            //220906 DCIR Offset Add
                            case "WP2MMMTC2021": cellDCIR_Offset1 = double.Parse(arr[3]); break;
                            case "WP2MMMTC2022": cellDCIR_Offset2 = double.Parse(arr[3]); break;
                            case "WP2MMMTC2023": cellDCIR_Offset3 = double.Parse(arr[3]); break;
                            case "WP2MMMTC2024": cellDCIR_Offset4 = double.Parse(arr[3]); break;
                            case "WP2MMMTC2025": cellDCIR_Offset5 = double.Parse(arr[3]); break;
                            case "WP2MMMTC2026": cellDCIR_Offset6 = double.Parse(arr[3]); break;
                            case "WP2MMMTC2027": cellDCIR_Offset7 = double.Parse(arr[3]); break;
                            case "WP2MMMTC2028": cellDCIR_Offset8 = double.Parse(arr[3]); break;
                            case "WP2MMMTC2029": cellDCIR_Offset9 = double.Parse(arr[3]); break;
                            case "WP2MMMTC2030": cellDCIR_Offset10 = double.Parse(arr[3]); break;
                            case "WP2MMMTC2031": cellDCIR_Offset11 = double.Parse(arr[3]); break;
                            case "WP2MMMTC2032": cellDCIR_Offset12 = double.Parse(arr[3]); break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, "GetControlItemFromMES", ex);
                return false;
            }

            //MES로부터 받아온 제어항목을 CSV에 저장한다.
            SetControlItemToCSV();
            SetCyclerStepToMESData();

            return true;
        }


        private void GetControlItemFromCSV()
        {
            StringBuilder logsb = new StringBuilder();
            logsb.Append("ACK=OK,ERRMSG=,CTRLITEM=");

            try
            {
                LogState(LogType.MANUALCONDITION, "Use To Local Data(Control)");

                //ControlList.csv에서 항목별 ID를 가져온다
                //장비별로 수정필요
                string typeName = "";
                switch (localTypes)
                {
                    case 0: typeName = BatteryInfo.ModelName; break;
                }

                FileStream readData = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "ControlList_" + typeName + ".csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader streamReader = new StreamReader(readData, Encoding.UTF8);

                //헤더 따로 저장
                string[] columnHeaders = streamReader.ReadLine().Split(',');

                //파일에서 ID와 값을 가져온다
                List<string> ctrlItems = new List<string>();
                while (streamReader.Peek() > -1)
                {
                    string[] arr = streamReader.ReadLine().Split(',');
                    string ctrlItem = arr[0];

                    switch (ctrlItem)
                    {
                        case MesControlID.BushingPlusVolt:
                            EOL.IR_Recipe.BushingPlus.Volt = int.Parse(arr[1]);
                            break;
                        case MesControlID.BushingPlusTime:
                            EOL.IR_Recipe.BushingPlus.Time = int.Parse(arr[1]);
                            break;
                        case MesControlID.CoolingPlatePlusVolt:
                            EOL.IR_Recipe.CoolingPlatePlus.Volt = int.Parse(arr[1]);
                            break;
                        case MesControlID.CoolingPlatePlusTime:
                            EOL.IR_Recipe.CoolingPlatePlus.Time = int.Parse(arr[1]);
                            break;
                        case MesControlID.BushingMinusVolt:
                            EOL.IR_Recipe.BushingMinus.Volt = int.Parse(arr[1]);
                            break;
                        case MesControlID.BushingMinusTime:
                            EOL.IR_Recipe.BushingMinus.Time = int.Parse(arr[1]);
                            break;
                        case MesControlID.CoolingPlateMinusVolt:
                            EOL.IR_Recipe.CoolingPlateMinus.Volt = int.Parse(arr[1]);
                            break;
                        case MesControlID.CoolingPlateMinusTime:
                            EOL.IR_Recipe.CoolingPlateMinus.Time = int.Parse(arr[1]);
                            break;
                        case MesControlID.IR_RANGE:
                            EOL.IR_Recipe.CoolingPlatePlus.Range = arr[1];
                            break;
                        case MesControlID.CellOpenUpper:
                            EOL.CellLineUpperLimit = int.Parse(arr[1]);
                            break;
                        case MesControlID.CellOpenLower:
                            EOL.CellLineLowLimit = int.Parse(arr[1]);
                            break;

                        // REST
                        case MesControlID.RestTimebeforeDischarge:
                            befDiscRestTime = arr[1];
                            break;

                        case MesControlID.DischargeCurrent:
                            discCur = arr[1];
                            break;
                        case MesControlID.DischargeTime:
                            discTime = arr[1];
                            break;
                        case MesControlID.DischargeCurrentUpperLimit:
                            discCurLimit = arr[1];
                            break;

                        case MesControlID.RestTimeafterDischarge:
                            aftDiscRestTime = arr[1];
                            break;

                        case MesControlID.ChargeCurrent:
                            charCur = arr[1];
                            break;
                        case MesControlID.ChargeTime:
                            charTime = arr[1];
                            break;
                        case MesControlID.ChargeCurrentUpperLimit:
                            charCurLimit = arr[1];
                            break;

                        case MesControlID.RestTimeafterCharge:
                            aftCharRestTime = arr[1];
                            break;

                        case MesControlID.SafetyVoltageUpperLimit:
                            safeVoltHighLimit = arr[1];
                            break;
                        case MesControlID.SafetyVoltageLowerLimit:
                            safeVoltLowLimit = arr[1];
                            break;

                        case MesControlID.CellDCIR1:
                            cellFomula1 = double.Parse(arr[1]);
                            break;
                        case MesControlID.CellDCIR2:
                            cellFomula2 = double.Parse(arr[1]);
                            break;
                        case MesControlID.CellDCIR3:
                            cellFomula3 = double.Parse(arr[1]);
                            break;
                        case MesControlID.ModuleDCIR1:
                            moduleFomula1 = double.Parse(arr[1]);
                            break;
                        case MesControlID.ModuleDCIR2:
                            moduleFomula2 = double.Parse(arr[1]);
                            break;
                        case MesControlID.ModuleDCIR3:
                            moduleFomula3 = double.Parse(arr[1]);
                            break;

                        //220906 DCIR Offset Add
                        case "WP2MMMTC2021": cellDCIR_Offset1 = double.Parse(arr[3]); break;
                        case "WP2MMMTC2022": cellDCIR_Offset2 = double.Parse(arr[3]); break;
                        case "WP2MMMTC2023": cellDCIR_Offset3 = double.Parse(arr[3]); break;
                        case "WP2MMMTC2024": cellDCIR_Offset4 = double.Parse(arr[3]); break;
                        case "WP2MMMTC2025": cellDCIR_Offset5 = double.Parse(arr[3]); break;
                        case "WP2MMMTC2026": cellDCIR_Offset6 = double.Parse(arr[3]); break;
                        case "WP2MMMTC2027": cellDCIR_Offset7 = double.Parse(arr[3]); break;
                        case "WP2MMMTC2028": cellDCIR_Offset8 = double.Parse(arr[3]); break;
                        case "WP2MMMTC2029": cellDCIR_Offset9 = double.Parse(arr[3]); break;
                        case "WP2MMMTC2030": cellDCIR_Offset10 = double.Parse(arr[3]); break;
                        case "WP2MMMTC2031": cellDCIR_Offset11 = double.Parse(arr[3]); break;
                        case "WP2MMMTC2032": cellDCIR_Offset12 = double.Parse(arr[3]); break;
                    }

                    logsb.Append(ctrlItem);
                    logsb.Append("^CTRLVAL=");
                    logsb.Append(arr[1]);
                    logsb.Append("@CTRLITEM=");
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, "GetControlItemFromCSV", ex);
                return;
            }

            logsb.Remove(logsb.Length - 10, 10);
            this.LogState(LogType.MANUALCONDITION, "GetProcessControlParameterRequest - " + logsb.ToString());
        }

        public void SetControlItemToCSV()
        {
            string columnHeaders = "";
            List<string> ctrlItems = new List<string>();
            StringBuilder sb = new StringBuilder();

            string typeName = "";

            //장비별로 수정필요
            switch (localTypes)
            {
                case 0: typeName = BatteryInfo.ModelName; break;
            }

            try
            {
                using (FileStream readData = new FileStream(
                    AppDomain.CurrentDomain.BaseDirectory +
                    "ControlList_" + typeName + ".csv",
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(readData, Encoding.UTF8))
                    {
                        columnHeaders = sr.ReadLine();

                        while (sr.Peek() > -1)
                        {
                            string[] arr = sr.ReadLine().Split(',');

                            string value = "";

                            switch (arr[0])
                            {
                                case MesControlID.BushingPlusVolt:
                                    value = EOL.IR_Recipe.BushingPlus.Volt.ToString();
                                    break;
                                case MesControlID.BushingPlusTime:
                                    value = EOL.IR_Recipe.BushingPlus.Time.ToString();
                                    break;
                                case MesControlID.CoolingPlatePlusVolt:
                                    value = EOL.IR_Recipe.CoolingPlatePlus.Volt.ToString();
                                    break;
                                case MesControlID.CoolingPlatePlusTime:
                                    value = EOL.IR_Recipe.CoolingPlatePlus.Time.ToString();
                                    break;
                                case MesControlID.BushingMinusVolt:
                                    value = EOL.IR_Recipe.BushingMinus.Volt.ToString();
                                    break;
                                case MesControlID.BushingMinusTime:
                                    value = EOL.IR_Recipe.BushingMinus.Time.ToString();
                                    break;
                                case MesControlID.CoolingPlateMinusVolt:
                                    value = EOL.IR_Recipe.CoolingPlateMinus.Volt.ToString();
                                    break;
                                case MesControlID.CoolingPlateMinusTime:
                                    value = EOL.IR_Recipe.CoolingPlateMinus.Time.ToString();
                                    break;
                                case MesControlID.IR_RANGE:
                                    value = EOL.IR_Recipe.CoolingPlatePlus.Range.ToString();
                                    break;
                                case MesControlID.CellOpenUpper:
                                    value = EOL.CellLineUpperLimit.ToString();
                                    break;
                                case MesControlID.CellOpenLower:
                                    value = EOL.CellLineLowLimit.ToString();
                                    break;

                                case MesControlID.RestTimebeforeDischarge:
                                    value = befDiscRestTime; break;

                                case MesControlID.DischargeCurrent:
                                    value = discCur; break;
                                case MesControlID.DischargeTime:
                                    value = discTime; break;
                                case MesControlID.DischargeCurrentUpperLimit:
                                    value = discCurLimit; break;

                                case MesControlID.RestTimeafterDischarge:
                                    value = aftDiscRestTime; break;

                                case MesControlID.ChargeCurrent:
                                    value = charCur; break;
                                case MesControlID.ChargeTime:
                                    value = charTime; break;
                                case MesControlID.ChargeCurrentUpperLimit:
                                    value = charCurLimit; break;

                                case MesControlID.RestTimeafterCharge:
                                    value = aftCharRestTime; break;

                                case MesControlID.SafetyVoltageUpperLimit:
                                    value = safeVoltHighLimit; break;
                                case MesControlID.SafetyVoltageLowerLimit:
                                    value = safeVoltLowLimit; break;

                                case MesControlID.CellDCIR1:
                                    value = cellFomula1.ToString(); break;
                                case MesControlID.CellDCIR2:
                                    value = cellFomula2.ToString(); break;
                                case MesControlID.CellDCIR3:
                                    value = cellFomula3.ToString(); break;

                                case MesControlID.ModuleDCIR1:
                                    value = moduleFomula1.ToString(); break;
                                case MesControlID.ModuleDCIR2:
                                    value = moduleFomula2.ToString(); break;
                                case MesControlID.ModuleDCIR3:
                                    value = moduleFomula3.ToString(); break;

                                //220906 DCIR Offset Add
                                case "WP2MMMTC2021": cellDCIR_Offset1 = double.Parse(arr[3]); break;
                                case "WP2MMMTC2022": cellDCIR_Offset2 = double.Parse(arr[3]); break;
                                case "WP2MMMTC2023": cellDCIR_Offset3 = double.Parse(arr[3]); break;
                                case "WP2MMMTC2024": cellDCIR_Offset4 = double.Parse(arr[3]); break;
                                case "WP2MMMTC2025": cellDCIR_Offset5 = double.Parse(arr[3]); break;
                                case "WP2MMMTC2026": cellDCIR_Offset6 = double.Parse(arr[3]); break;
                                case "WP2MMMTC2027": cellDCIR_Offset7 = double.Parse(arr[3]); break;
                                case "WP2MMMTC2028": cellDCIR_Offset8 = double.Parse(arr[3]); break;
                                case "WP2MMMTC2029": cellDCIR_Offset9 = double.Parse(arr[3]); break;
                                case "WP2MMMTC2030": cellDCIR_Offset10 = double.Parse(arr[3]); break;
                                case "WP2MMMTC2031": cellDCIR_Offset11 = double.Parse(arr[3]); break;
                                case "WP2MMMTC2032": cellDCIR_Offset12 = double.Parse(arr[3]); break;
                            }

                            if (value != "")
                                sb.Append(arr[0] + "," + value + "," + arr[2] + "," + arr[3] + "\n");
                            else
                                sb.Append(arr[0] + "," + "0" + "," + arr[2] + "," + arr[3] + "\n");
                        }

                        //저장된 제어항목을 파일에 저장
                        using (StreamWriter sw = 
                                    new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "ControlList_" + typeName + ".csv", false))
                        {
                            sw.WriteLine(columnHeaders);
                            sw.Write(sb.ToString());
                        }
                    }
                }

                LogState(LogType.Info, "SetControlItemToCSV");
                LogState(LogType.Info, sb.ToString());
            }
            catch (Exception ex)
            { LogState(LogType.Fail, "SetControlItemToCSV", ex); }
        }

        private void GetCollectItemFromCSV()
        {
            try
            {
                string fileName = "";

                if (CONFIG.EolInspType == InspectionType.EOL)
                    fileName = PATH.FileNameEOL;
                else if (CONFIG.EolInspType == InspectionType.HIPOT)
                    fileName = PATH.FileNameHIPOT;

                LogState(LogType.MANUALCONDITION, "Use To Local Data(Collect)");

                //eollist.csv에서 항목별 ID를 가져온다
                FileStream readData = new FileStream(AppDomain.CurrentDomain.BaseDirectory + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader streamReader = new StreamReader(readData, Encoding.UTF8);

                //헤더 따로 저장
                string[] columnHeaders = streamReader.ReadLine().Split(',');

                //파일에서 ID와 값을 가져온다
                List<TestItem> tis = new List<TestItem>();
                while (streamReader.Peek() > -1)
                {
                    string[] arr = streamReader.ReadLine().Split(',');
                    if (arr.Length < 6) continue;
                    if ((localTypes == 0 && arr[9] != "1") || (localTypes == 1 && arr[10] != "1")
                        || (localTypes == 2 && arr[11] != "1") || (localTypes == 3 && arr[12] != "1")
                        || (localTypes == 4 && arr[13] != "1") || (localTypes == 2 && arr[14] != "1"))
                    {
                        continue;
                    }

                    string clctItem = arr[1];
                    double min = 0.0, max = 0.0;
                    bool dTrigger1 = double.TryParse(arr[3], out min);
                    bool dTrigger2 = double.TryParse(arr[4], out max);

                    foreach (var testItem in modelList[selectedIndex].TestItemList)
                    {
                        if (testItem.Value.CLCTITEM == clctItem)
                        {
                            if (dTrigger1) testItem.Value.Min = min;
                            else testItem.Value.Min = arr[3];

                            if (dTrigger2) testItem.Value.Max = max;
                            else testItem.Value.Max = arr[4];

                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { LogState(LogType.Fail, "GetCollectItemFromCSV", ex); }

            StringBuilder sb = new StringBuilder();
            sb.Append("ACK=OK,ERRMSG=,");
            foreach (var item in modelList[selectedIndex].TestItemList)
            {
                if (item.Key == _BarCodeName)
                    continue;

                //CLCITEM=CTQW2201,CLCTLSL=9.532,CLCTUSL=12.25@
                sb.Append("CLCITEM=");
                sb.Append(item.Value.CLCTITEM.ToString());
                sb.Append("^CLCTLSL=");
                sb.Append(item.Value.Min.ToString());
                sb.Append("^CLCTUSL=");
                sb.Append(item.Value.Max.ToString());
                sb.Append("@");
            }

            this.LogState(LogType.MANUALCONDITION, "GetProcessingSpecRequest - " +
                sb.ToString().Remove(sb.ToString().Length - 1, 1));
        }

        private bool GetCollectItemFromMES()
        {
            //MES에 수집항목을 요청한다
            string collectItem = Device.MES.GetProcessingSpecRequest(this.modelList[selectedIndex].EquipId,
                                                                                this.modelList[selectedIndex].ProductCode,
                                                                                this.modelList[selectedIndex].ProcessID);
            if (collectItem == "" || collectItem == "NG")
            {
                LogState(LogType.Fail, "GetCollectItemFromMES");
                // 20190314
                //return false;
                return true;
            }

            EOL.IR_CtrlItem.Clear();
            DelKeyList.Clear();

            collectItem = collectItem.Remove(collectItem.Length - 1, 1);

            try
            {
                //받은 데이터를 파싱한다
                string[] parsed = collectItem.Split('@');
                var str = parsed[0].Split(new string[] { "CLCITEM" }, StringSplitOptions.RemoveEmptyEntries);
                parsed[0] = "CLCITEM" + str[1];
                parsed[parsed.Length - 1] = parsed[parsed.Length - 1].Remove(parsed[parsed.Length - 1].Length - 3, 3);

                foreach (string item in parsed)
                {
                    string[] splitter = new string[] { "^", "=" };
                    var arr = item.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                    if (arr.Length < 6) continue;

                    string clctItem = arr[1];
                    double min = 0.0, max = 0.0;
                    bool dTrigger1 = double.TryParse(arr[3], out min);
                    bool dTrigger2 = double.TryParse(arr[5], out max);

                    EOL.IR_CtrlItem.Add(clctItem);

                    foreach (var testItem in modelList[selectedIndex].TestItemList)
                    {
                        if (testItem.Value.CLCTITEM == clctItem)
                        {
                            if (dTrigger1)
                                testItem.Value.Min = min;
                            else
                                testItem.Value.Min = arr[3];

                            if (dTrigger2)
                                testItem.Value.Max = max;
                            else
                                testItem.Value.Max = arr[5];

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, "GetCollectItemFromMES", ex);
                return false;
            }

            //MES로부터 받아온 제어항목을 CSV에 저장한다.
            SetCollectItemToCSV();

            // moons
            if (CONFIG.EolInspType == InspectionType.HIPOT)
            {
                if (EOL.CtrlItemOldCount == 0)
                    EOL.CtrlItemOldCount = EOL.IR_CtrlItem.Count;
          
                if(EOL.CtrlItemOldCount != EOL.IR_CtrlItem.Count)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        LoadList();
                    }));
                }
         
                MesContorlItemCheckAndDelete();
            }


            return true;
        }

        public void SetCollectItemToCSV()
        {
            string columnHeaders = "";
            List<string[]> collections = new List<string[]>();
            try
            {
                string fileName = "";

                if (CONFIG.EolInspType == InspectionType.EOL)
                    fileName = PATH.FileNameEOL;
                else if (CONFIG.EolInspType == InspectionType.HIPOT)
                    fileName = PATH.FileNameHIPOT;

                //eollist.csv를 불러온다
                using (FileStream readData = new FileStream(
                        AppDomain.CurrentDomain.BaseDirectory + fileName
                        , FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(readData, Encoding.UTF8))
                    {
                        //헤더 따로 저장
                        columnHeaders = sr.ReadLine();

                        //제어항목을 불러온다
                        while (sr.Peek() > -1)
                            collections.Add(sr.ReadLine().Split(','));

                    }
                }

                //저장된 제어항목을 파일에 저장
                using (StreamWriter sw = new StreamWriter(
                        AppDomain.CurrentDomain.BaseDirectory + fileName,
                        false))
                {
                    StringBuilder sb = new StringBuilder();

                    //헤더를 파일에 쓴다
                    sw.WriteLine(columnHeaders);

                    //localTypes를 참조하여 현재 수집항목 조건을 파일에 쓴다
                    for (int i = 0; i < collections.Count; i++)
                    {
                        foreach (var testItem in modelList[selectedIndex].TestItemList)
                        {
                            if ((localTypes == 0 && collections[i][9] == "1" && collections[i][2] == testItem.Value.CLCTITEM))
                            {
                                collections[i][3] = testItem.Value.Min.ToString();
                                collections[i][4] = testItem.Value.Max.ToString();

                                break;
                            }
                        }

                        foreach (string s in collections[i]) sb.Append(s + ',');
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("\n");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sw.Write(sb.ToString());

                    sb.Clear();
                }

                Thread.Sleep(100);
            }
            catch (Exception ex) { LogState(LogType.Fail, "SetCollectItemToCSV", ex); }
        }


        /// <summary>
        /// 17. MES Connect 시 MES 연결후에 isMESskip=true 후 발생하는 이벤트가 수집항목 및 제어항목
        /// 파싱 전 발생되지 않아(isMESSkipCb.isChecked = true가 늦게 적용됨) 데이터 파싱 에러야기
        /// 하여 강제 체크구문 적용
        /// </summary>
        private void MESConnect()
        {
            Device.MES = new CMES(this);
            Device.MES.retryConntectcnt = 0;
            var rtv = Device.MES.Open();

            if (rtv == "" && Device.MES.IsMESConnected)
            {
                Device.MES.retryConntectcnt = 3;
                this.LogState(LogType.Success, "MES Connected");
                IsMESskip = false;

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    // 2019.06.28 jeonhj
                    // 프로그램 로드 후 mes 연결 동작 시 연결이 되면 배경을 Blue로 표시한다.
                    // Messkip ischecked를 false처리해두어 스킵 이벤트를 타지 않아 해당 배경색 변경 동작을 할 수 없다.
                    this.EquipIDTb.Background = this.lotTb.Background = this.prodTb.Background = this.procTb.Background = Brushes.SkyBlue;
                    isMESSkipCb.IsChecked = false;
                }));

                //221111 nnkim
                LogState(LogType.Info, string.Format("ServerTime_Request Start -"));

                Device.MES.ServerTime_Request(CONFIG.MesEquipID);                    

                    if (Device.MES.bIsRequestTime == false)
                    {
                        MessageBox.Show("To Request Time is Fail From Mes!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                

            }
            else
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.EquipIDTb.Background = this.lotTb.Background = this.prodTb.Background = this.procTb.Background = Brushes.Red;
                }));
            }
        }



        /// <summary>
        /// MES에 상세수집 / 수집항목 완공처리
        /// 중간에 NG나도 여기타고 다끝나고 PASS해도 여기탐
        /// </summary>
        /// <param name="isSuccess"></param>
        private void Finished(bool isSuccess, bool isFinished = true)
        {
            //스킵NG일땐 무조건 NG처리
            //190109 NG필터 추가
            if (isSkipNG_)
            {
                LogState(LogType.NG, "Result Filter :SkipNG");
                isSuccess = false;
            }

            //190109 NG필터 추가
            if (Device.MES.isMES_SYS_Disconnected)
            {
                LogState(LogType.NG, "Result Filter :Disconnected MES");
                isSuccess = false;
            }

            StringBuilder sb = new StringBuilder();
            StringBuilder sbc = new StringBuilder();

            List<DetailItems> dlist = new List<DetailItems>();

            //완공을 위한 아이템별 데이터 모으기
            foreach (var item in this.modelList[selectedIndex].TestItemList)
            {
                //26. finished 함수내 수집/상세수집 데이터 모을때 바코드 항목의 상세수집은 모으고 수집은
                //Continue로 미수집 처리(상세수집 중 제어항목 관련 item은 바코드 리딩시 처리되도록 박진호 선임 요청함)
                //not finish item filter
                //190104 by grchoi
                //if (item.Key == _BARCODE) continue;
                if (item.Key.Replace(" ", "") == _BarCodeName)
                {
                    if (item.Value.refValues_.Count != 0)
                    {
                        foreach (var ritem in item.Value.refValues_) dlist.Add(ritem as DetailItems);
                    }
                    continue;
                }

                sb.Append(string.Format("{0}={1}={2}^", item.Value.CLCTITEM, item.Value.Value_, (item.Value.Result == "PASS" ? "Y" : "N")));

                //상세수집 데이터 모으기
                if (item.Value.refValues_.Count != 0)
                {
                    foreach (var ritem in item.Value.refValues_)
                    {
                        var dti = ritem as DetailItems;

                        dlist.Add(dti);
                    }
                }
            }

			//  var orderedList = dlist.OrderBy(x => x.Key.ToString());
            var orderedList = dlist.OrderBy(x => x.order);

            //공정 결과 상세 보고 정렬
            foreach (var item in orderedList)
            {
                var dti = item as DetailItems;

                var clct = dti.Key;

                sbc.Append(string.Format("{0}=", clct));
                for (int i = 0; i < dti.Reportitems.Count; i++)
                {
                    sbc.Append(string.Format("{0}&", dti.Reportitems[i].ToString()));
                }

                sbc.Remove(sbc.Length - 1, 1);
                sbc.Append("^");
            }

            //순서정렬
            if (sbc.Length > 0)
            {
                sbc.Remove(sbc.Length - 1, 1);
                LogState(LogType.Info, "GetProcessDataReport - " + sbc.ToString());
            }

            sb.Remove(sb.Length - 1, 1);
            LogState(LogType.Info, "EndJobInsp - " + sb.ToString());

            if (isSuccess)
            {
                LogState(LogType.Info, "EOL - total Result - PASS");
            }
            else
            {
                LogState(LogType.Info, "EOL - total Result - NG");
            }

            //27. finished로 검사 종료 시 검사모델/설비ID/제품ID/공정ID를 로그로 남김
            LogState(LogType.Info, "Started Model : " + this.modelList[selectedIndex].ModelId);
            LogState(LogType.Info, "Lot ID : " + this.modelList[selectedIndex].LotId);
            LogState(LogType.Info, "Proc ID : " + this.modelList[selectedIndex].ProcessID);
            LogState(LogType.Info, "Prod ID : " + this.modelList[selectedIndex].ProductCode);
            LogState(LogType.Info, "Equip ID : " + this.modelList[selectedIndex].EquipId);
                        
            if (!IsMESskip)
            {
                //startJob NG Flag
                if (isFinished)
                {
                    string retDItem = Device.MES.GetProcessDataReport(
                        this.modelList[selectedIndex].LotId,
                        this.modelList[selectedIndex].ProcessID,
                        this.modelList[selectedIndex].EquipId,
                        this.modelList[selectedIndex].UserId,
                        sbc.ToString());

                    if ((retDItem != string.Empty) || (retDItem == "NG")) //OK일때 시작
                    {
                        this.LogState(LogType.Fail, "MES_GetProcessDataReport");
                        isSuccess = false;
                    }
                    else
                    {
                        this.LogState(LogType.Success, "MES_GetProcessDataReport - " + sbc.ToString());
                    }

                    PauseLoop("MES_GetProcessDataReport");

                    //공정 결과 Data 보고                
                    if (Device.MES.EndJobInsp(
                            this.modelList[selectedIndex].LotId,
                            this.modelList[selectedIndex].ProcessID,
                            this.modelList[selectedIndex].EquipId,
                            this.modelList[selectedIndex].UserId,
                            (isSuccess == true ? "OK" : "NG"), sb.ToString()) == "NG") //OK일때 시작
                    {
                        this.LogState(LogType.Fail, "MES_EndJobInsp");

                        isSuccess = false;
                    }
                    else
                    {
                        this.LogState(LogType.Success, "MES_EndJobInsp - " + sb.ToString());
                    }
                    PauseLoop("MES_EndJobInsp");
                }
                else
                {
                    isSuccess = false;
                }

            }

            if (Device.PLC != null)
                Device.PLC.TestResult(isSuccess);
            
            //상세수집, 완공 결과에 따라 UI 최종결과가 결정된다.
            this.Dispatcher.Invoke(new Action(() =>
            {
                Pass(isSuccess);

                isMESSkipCb.IsEnabled = manualBt.IsEnabled = true;
             
                //type1lb.IsEnabled = true;
                //resetBt.IsEnabled = true;
                //specBt.IsEnabled = true;

                if (IsManual)
                {
                    resetBt.IsEnabled = specBt.IsEnabled = cyclerBt.IsEnabled = relayConBt.IsEnabled = true;
                    blinder.IsEnabled = true;
                }
                blinder3.Visibility = System.Windows.Visibility.Collapsed;

                _stopwatch.Stop();
                this.time_finish_tb.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }));

            if (finishedThread == null)
            {
                finishedThread = new Thread(() =>
                {
                    //빨간불이 이미 계측기때문에 들어와있다면,
                    var isComDied = Device.Relay.GetRelayStatus(RELAY.LIGHT_RED);
                    
                    if (!isSuccess)
                    {
                        Device.Relay.On(RELAY.LIGHT_RED);
                        Thread.Sleep(3000);

                        if (!isComDied)
                        {
                            Device.Relay.Off(RELAY.LIGHT_RED);
                        }
                    }
  					 if (!IsMESskip)
                    {
                        SetTLampMESStatus(true);
                    }

                    Device.Relay.Off(RELAY.LIGHT_GREEN);
                    Device.Relay.On(RELAY.LIGHT_YELLOW);

                    LogState(LogType.Info, "FINISHED_TEST----------------------------------------------------", null, false);
                    finishedThread = null;
                    _Barcode = "";
                });
                finishedThread.Start();
            }
        }
    }
}
