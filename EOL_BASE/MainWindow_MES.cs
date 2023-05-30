using EOL_BASE.모듈;
using EOL_BASE.클래스;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        static string MEB_SPEC_CV_MIN_RESULT = "3.520000";
        
        //221004 수집 항목 포드 12,13라인
        #region MC_OPEN_CHECK_DETAIL_ITEM_EOL
        string KEY_EOL_MC_3_OPEN_CHECK_NEW = "MC3OPENCHECK";
        string KEY_EOL_MC_4_OPEN_CHECK_NEW = "MC4OPENCHECK";
        string KEY_EOL_MC_5_OPEN_CHECK_NEW = "MC5OPENCHECK";
        string KEY_EOL_MC_6_OPEN_CHECK_NEW = "MC6OPENCHECK";
        string KEY_EOL_MC_7_OPEN_CHECK_NEW = "MC7OPENCHECK";
        string KEY_EOL_MC_8_OPEN_CHECK_NEW = "MC8OPENCHECK";
        string KEY_EOL_MC_9_OPEN_CHECK_NEW = "MC9OPENCHECK";
        string KEY_EOL_MC_10_OPEN_CHECK_NEW = "MC10OPENCHECK";
        string KEY_EOL_MC_11_OPEN_CHECK_NEW = "MC11OPENCHECK";
        string KEY_EOL_MC_12_OPEN_CHECK_NEW = "MC12OPENCHECK";
        string KEY_EOL_MC_13_OPEN_CHECK_NEW = "MC13OPENCHECK";
        string KEY_EOL_MC_14_OPEN_CHECK_NEW = "MC14OPENCHECK";
        string KEY_EOL_MC_15_OPEN_CHECK_NEW = "MC15OPENCHECK";
        string KEY_EOL_MC_16_OPEN_CHECK_NEW = "MC16OPENCHECK";
        string KEY_EOL_MC_17_OPEN_CHECK_NEW = "MC17OPENCHECK";
        string KEY_EOL_MC_18_OPEN_CHECK_NEW = "MC18OPENCHECK";
        string KEY_EOL_MC_19_OPEN_CHECK_NEW = "MC19OPENCHECK";
        string KEY_EOL_MC_20_OPEN_CHECK_NEW = "MC20OPENCHECK";
        #endregion

        #region MC_HORT_CHECK_DETAIL_ITEM_EOL
        string KEY_EOL_MC_3_SHORT_CHECK_NEW = "MC3SHORTCHECK";
        string KEY_EOL_MC_4_SHORT_CHECK_NEW = "MC4SHORTCHECK";
        string KEY_EOL_MC_5_SHORT_CHECK_NEW = "MC5SHORTCHECK";
        string KEY_EOL_MC_6_SHORT_CHECK_NEW = "MC6SHORTCHECK";
        string KEY_EOL_MC_7_SHORT_CHECK_NEW = "MC7SHORTCHECK";
        string KEY_EOL_MC_8_SHORT_CHECK_NEW = "MC8SHORTCHECK";
        string KEY_EOL_MC_9_SHORT_CHECK_NEW = "MC9SHORTCHECK";
        string KEY_EOL_MC_10_SHORT_CHECK_NEW = "MC10SHORTCHECK";
        string KEY_EOL_MC_11_SHORT_CHECK_NEW = "MC11SHORTCHECK";
        string KEY_EOL_MC_12_SHORT_CHECK_NEW = "MC12SHORTCHECK";
        string KEY_EOL_MC_13_SHORT_CHECK_NEW = "MC13SHORTCHECK";
        string KEY_EOL_MC_14_SHORT_CHECK_NEW = "MC14SHORTCHECK";
        string KEY_EOL_MC_15_SHORT_CHECK_NEW = "MC15SHORTCHECK";
        string KEY_EOL_MC_16_SHORT_CHECK_NEW = "MC16SHORTCHECK";
        string KEY_EOL_MC_17_SHORT_CHECK_NEW = "MC17SHORTCHECK";
        string KEY_EOL_MC_18_SHORT_CHECK_NEW = "MC18SHORTCHECK";
        string KEY_EOL_MC_19_SHORT_CHECK_NEW = "MC19SHORTCHECK";
        string KEY_EOL_MC_20_SHORT_CHECK_NEW = "MC20SHORTCHECK";
        #endregion

        //221004 수집 항목 포드 22,23,24라인
        #region MC_OPEN_CHECK_DETAIL_ITEM_EOL
        string KEY_EOL_MC_3_OPEN_CHECK_OLD = "MC3OPENCHECK";
        string KEY_EOL_MC_4_OPEN_CHECK_OLD = "MC4OPENCHECK";
        string KEY_EOL_MC_5_OPEN_CHECK_OLD = "MC5OPENCHECK";
        string KEY_EOL_MC_6_OPEN_CHECK_OLD = "MC6OPENCHECK";
        string KEY_EOL_MC_7_OPEN_CHECK_OLD = "MC7OPENCHECK";
        string KEY_EOL_MC_8_OPEN_CHECK_OLD = "MC8OPENCHECK";
        string KEY_EOL_MC_9_OPEN_CHECK_OLD = "MC9OPENCHECK";
        string KEY_EOL_MC_10_OPEN_CHECK_OLD = "MC10OPENCHECK";
        string KEY_EOL_MC_11_OPEN_CHECK_OLD = "MC11OPENCHECK";
        string KEY_EOL_MC_12_OPEN_CHECK_OLD = "MC12OPENCHECK";

        #endregion

        #region MC_HORT_CHECK_DETAIL_ITEM_EOL
        string KEY_EOL_MC_3_SHORT_CHECK_OLD = "MC3SHORTCHECK";
        string KEY_EOL_MC_4_SHORT_CHECK_OLD = "MC4SHORTCHECK";
        string KEY_EOL_MC_5_SHORT_CHECK_OLD = "MC5SHORTCHECK";
        string KEY_EOL_MC_6_SHORT_CHECK_OLD = "MC6SHORTCHECK";
        string KEY_EOL_MC_7_SHORT_CHECK_OLD = "MC7SHORTCHECK";
        string KEY_EOL_MC_8_SHORT_CHECK_OLD = "MC8SHORTCHECK";
        string KEY_EOL_MC_9_SHORT_CHECK_OLD = "MC9SHORTCHECK";
        string KEY_EOL_MC_10_SHORT_CHECK_OLD = "MC10SHORTCHECK";
        string KEY_EOL_MC_11_SHORT_CHECK_OLD = "MC11SHORTCHECK";
        string KEY_EOL_MC_12_SHORT_CHECK_OLD = "MC12SHORTCHECK";

        #endregion

        private bool GetControlItemFromMES(string LotID = "")
        {
            //데이터 중복 확인 및 저장 중복 확인
            bool bResult = false;

            //MES에 제어항목을 요청한다
            string controlItem = MES.GetProcessControlParameterRequest(this.modelList[selectedIndex].EquipId, this.modelList[selectedIndex].ProdId, this.modelList[selectedIndex].ProcId, LotID);
            if (controlItem == "" || controlItem == "NG")
            {
                LogState(LogType.Fail, "GetControlItemFromMES");
                return false;
            }

            try
            {
                //받은 데이터를 파싱한다
                string[] parsed = controlItem.Split('@');
                var str = parsed[0].Split(new string[] { "CTRLITEM" }, StringSplitOptions.RemoveEmptyEntries);
                parsed[0] = "CTRLITEM" + str[1];
                parsed[parsed.Length - 1] = parsed[parsed.Length - 1].Remove(parsed[parsed.Length - 1].Length - 4, 4);

                foreach (string item in parsed)
                {
                    string[] splitter = new string[] { "^", "=" };
                    var arr = item.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                    switch (arr[1])
                    {
                        case "W2MLMTC4001": GetResultCompareint(INSULATION_RESISTANCE_VOLT, arr[3],out INSULATION_RESISTANCE_VOLT,ref bResult);                            break;
                        case "W2MLMTC4002": GetResultCompareint(INSULATION_RESISTANCE_TIME, arr[3],out INSULATION_RESISTANCE_TIME, ref bResult);                           break;
                        case "W2MLMTC4003": GetResultCompareString(befDiscRestTime, arr[3],out befDiscRestTime, ref bResult);                                              break;
                        case "W2MLMTC4004": GetResultCompareString(discCur, arr[3],out discCur, ref bResult);                                                              break;                            
                        case "W2MLMTC4005": GetResultCompareString(discTime, arr[3],out discTime, ref bResult);                                                            break;  
                        case "W2MLMTC4006": GetResultCompareString(discCurLimit, arr[3],out discCurLimit, ref bResult);                                                    break; 
                        case "W2MLMTC4007": GetResultCompareString(aftDiscRestTime, arr[3],out aftDiscRestTime, ref bResult);                                              break; 
                        case "W2MLMTC4008": GetResultCompareString(charCur, arr[3],out charCur, ref bResult);                                                              break; 
                        case "W2MLMTC4009": GetResultCompareString(charTime, arr[3],out charTime, ref bResult);                                                            break; 
                        case "W2MLMTC4010": GetResultCompareString(aftCharRestTime, arr[3],out aftCharRestTime, ref bResult);                                              break; 
                        case "W2MLMTC4011": GetResultCompareString(charCurLimit, arr[3],out charCurLimit, ref bResult);                                                    break; 
                        case "W2MLMTC4012": GetResultCompareString(safeVoltHighLimit, arr[3],out safeVoltHighLimit, ref bResult);                                          break; 
                        case "W2MLMTC4013": GetResultCompareString(safeVoltLowLimit, arr[3],out safeVoltLowLimit, ref bResult);                                            break; 
                        case "W2MLMTC4014": GetResultCompareint(LINE_OPEN_CHECK_UPPER_LIMIT, arr[3],out LINE_OPEN_CHECK_UPPER_LIMIT, ref bResult);                         break;
                        case "W2MLMTC4015": GetResultCompareint(LINE_OPEN_CHECK_LOWER_LIMIT, arr[3],out LINE_OPEN_CHECK_LOWER_LIMIT, ref bResult);                         break;
                        case "W2MLMTC4016": GetResultCompareDouble(cellFomula1, arr[3],out cellFomula1, ref bResult);                                                      break;
                        case "W2MLMTC4017": GetResultCompareDouble(cellFomula2, arr[3],out cellFomula2, ref bResult);                                                      break;
                        case "W2MLMTC4018": GetResultCompareDouble(moduleFomula1, arr[3],out moduleFomula1, ref bResult);                                                  break;
                        case "W2MLMTC4019": GetResultCompareDouble(moduleFomula2, arr[3],out moduleFomula2, ref bResult);                                                  break;
                        case "W2MLMTC4020": GetResultCompareDouble(coefficientT, arr[3],out coefficientT, ref bResult);                                                    break;
                        case "W2MLMTC4021": GetResultCompareDouble(coefficientV, arr[3],out coefficientV, ref bResult);                                                    break;
                        case "W2MLMTC4022": GetResultCompareint(INSULATION_RESISTANCE_RAMP_TIME, arr[3],out INSULATION_RESISTANCE_RAMP_TIME, ref bResult);                 break;
                        case "W2MLMTC4023": GetResultCompareString(INSULATION_RESISTANCE_RANGE, arr[3],out INSULATION_RESISTANCE_RANGE, ref bResult);                      break;
                        case "W2MLMTC4024": GetResultCompareDouble(chroma.CON_WITHSTANDING_VOLTAGE_V, arr[3],out chroma.CON_WITHSTANDING_VOLTAGE_V, ref bResult);          break;
                        case "W2MLMTC4025": GetResultCompareDouble(chroma.CON_WITHSTANDING_RAMP_UP_TIME, arr[3],out chroma.CON_WITHSTANDING_RAMP_UP_TIME, ref bResult);    break;
                        case "W2MLMTC4026": GetResultCompareDouble(chroma.CON_WITHSTANDING_VOLTAGE_T, arr[3],out chroma.CON_WITHSTANDING_VOLTAGE_T, ref bResult);          break;
                        case "W2MLMTC4027": GetResultCompareDouble(chroma.CON_ARC_LIMIT, arr[3],out chroma.CON_ARC_LIMIT, ref bResult);                                    break;
                        case "W2MLMTC4028": GetResultCompareDouble(chroma.CON_ARC_ENABLE, arr[3],out chroma.CON_ARC_ENABLE, ref bResult);                                  break;
                        case "W2MLMTC4029": GetResultCompareDouble(chroma.CON_WITHSTANDING_FALLDOWN_TIME, arr[3],out chroma.CON_WITHSTANDING_FALLDOWN_TIME, ref bResult);  break;
                        case "W2MLMTC4030": GetResultCompareint(CONTACT_CHECK_UPPER_LIMIT, arr[3],out CONTACT_CHECK_UPPER_LIMIT, ref bResult);                             break;

                    }
                }

                //유효성 검사 (필요시 항목 추가, 함수로 따자)

                if (int.Parse(aftCharRestTime) <= 0 || int.Parse(aftCharRestTime) > 40)
                {
                    throw new Exception("abnormal ctrl spec W2MLMTC4010, 5step rest time : " + aftCharRestTime);
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, "GetControlItemFromMES", ex);
                return false;
            }

            SetCyclerStepToMESData(0);

            //Data가 바뀌었을 경우 저장하도록 조건문 추가
            if(bResult == true)
            {
                //MES로부터 받아온 제어항목을 CSV에 저장한다.
                SetControlItemToCSV();
            }

            return true;
        }

        private void GetControlItemFromCSV()
        {
            StringBuilder logsb = new StringBuilder();
            logsb.Append("ACK=OK,ERRMSG=,CTRLITEM=");

            //데이터 중복 확인 및 저장 중복 확인
            bool bResult = false;

            try
            {
                LogState(LogType.MANUALCONDITION, "Use To Local Data(Control)");

                while(chroma==null)
                {

                }

                //ControlList.csv에서 항목별 ID를 가져온다
                //장비별로 수정필요
                string typeName = "";
                switch (localTypes)
                {
                    case 0: typeName = "4P8S"; break;
                    case 1: typeName = "4P8S_Rev"; break;
                    case 2: typeName = "4P7S"; break;
                    case 3: typeName = "3P8S"; break;
                    case 4: typeName = "3P10S"; break;
                    case 5: typeName = "3P10S_Rev"; break;
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
                        case "W2MLMTC4001": GetResultCompareint(INSULATION_RESISTANCE_VOLT, arr[1], out INSULATION_RESISTANCE_VOLT, ref bResult);                           break;
                        case "W2MLMTC4002": GetResultCompareint(INSULATION_RESISTANCE_TIME, arr[1], out INSULATION_RESISTANCE_TIME, ref bResult);                           break;
                        case "W2MLMTC4003": GetResultCompareString(befDiscRestTime, arr[1], out befDiscRestTime, ref bResult);                                              break;
                        case "W2MLMTC4004": GetResultCompareString(discCur, arr[1], out discCur, ref bResult);                                                              break;
                        case "W2MLMTC4005": GetResultCompareString(discTime, arr[1], out discTime, ref bResult);                                                            break;
                        case "W2MLMTC4006": GetResultCompareString(discCurLimit, arr[1], out discCurLimit, ref bResult);                                                    break;
                        case "W2MLMTC4007": GetResultCompareString(aftDiscRestTime, arr[1], out aftDiscRestTime, ref bResult);                                              break;
                        case "W2MLMTC4008": GetResultCompareString(charCur, arr[1], out charCur, ref bResult);                                                              break;
                        case "W2MLMTC4009": GetResultCompareString(charTime, arr[1], out charTime, ref bResult);                                                            break;
                        case "W2MLMTC4010": GetResultCompareString(aftCharRestTime, arr[1], out aftCharRestTime, ref bResult);                                              break;
                        case "W2MLMTC4011": GetResultCompareString(charCurLimit, arr[1], out charCurLimit, ref bResult);                                                    break;
                        case "W2MLMTC4012": GetResultCompareString(safeVoltHighLimit, arr[1], out safeVoltHighLimit, ref bResult);                                          break;
                        case "W2MLMTC4013": GetResultCompareString(safeVoltLowLimit, arr[1], out safeVoltLowLimit, ref bResult);                                            break;
                        case "W2MLMTC4014": GetResultCompareint(LINE_OPEN_CHECK_UPPER_LIMIT, arr[1], out LINE_OPEN_CHECK_UPPER_LIMIT, ref bResult);                         break;
                        case "W2MLMTC4015": GetResultCompareint(LINE_OPEN_CHECK_LOWER_LIMIT, arr[1], out LINE_OPEN_CHECK_LOWER_LIMIT, ref bResult);                         break;
                        case "W2MLMTC4016": GetResultCompareDouble(cellFomula1, arr[1], out cellFomula1, ref bResult);                                                      break;
                        case "W2MLMTC4017": GetResultCompareDouble(cellFomula2, arr[1], out cellFomula2, ref bResult);                                                      break;
                        case "W2MLMTC4018": GetResultCompareDouble(moduleFomula1, arr[1], out moduleFomula1, ref bResult);                                                  break;
                        case "W2MLMTC4019": GetResultCompareDouble(moduleFomula2, arr[1], out moduleFomula2, ref bResult);                                                  break;
                        case "W2MLMTC4020": GetResultCompareDouble(coefficientT, arr[1], out coefficientT, ref bResult);                                                    break;
                        case "W2MLMTC4021": GetResultCompareDouble(coefficientV, arr[1], out coefficientV, ref bResult);                                                    break;
                        case "W2MLMTC4022": GetResultCompareint(INSULATION_RESISTANCE_RAMP_TIME, arr[1], out INSULATION_RESISTANCE_RAMP_TIME, ref bResult);                 break;
                        case "W2MLMTC4023": GetResultCompareString(INSULATION_RESISTANCE_RANGE, arr[1], out INSULATION_RESISTANCE_RANGE, ref bResult);                      break;
                        case "W2MLMTC4024": GetResultCompareDouble(chroma.CON_WITHSTANDING_VOLTAGE_V, arr[1], out chroma.CON_WITHSTANDING_VOLTAGE_V, ref bResult);          break;
                        case "W2MLMTC4025": GetResultCompareDouble(chroma.CON_WITHSTANDING_RAMP_UP_TIME, arr[1], out chroma.CON_WITHSTANDING_RAMP_UP_TIME, ref bResult);    break;
                        case "W2MLMTC4026": GetResultCompareDouble(chroma.CON_WITHSTANDING_VOLTAGE_T, arr[1], out chroma.CON_WITHSTANDING_VOLTAGE_T, ref bResult);          break;
                        case "W2MLMTC4027": GetResultCompareDouble(chroma.CON_ARC_LIMIT, arr[1], out chroma.CON_ARC_LIMIT, ref bResult);                                    break;
                        case "W2MLMTC4028": GetResultCompareDouble(chroma.CON_ARC_ENABLE, arr[1], out chroma.CON_ARC_ENABLE, ref bResult);                                  break;
                        case "W2MLMTC4029": GetResultCompareDouble(chroma.CON_WITHSTANDING_FALLDOWN_TIME, arr[1], out chroma.CON_WITHSTANDING_FALLDOWN_TIME, ref bResult);  break;
                        case "W2MLMTC4030": GetResultCompareint(CONTACT_CHECK_UPPER_LIMIT, arr[1], out CONTACT_CHECK_UPPER_LIMIT, ref bResult);                             break;

                    }


                    logsb.Append(ctrlItem);
                    logsb.Append("^CTRLVAL=");
                    logsb.Append(arr[1]);
                    logsb.Append("@CTRLITEM=");
                    //}
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, "GetControlItemFromCSV", ex);
                return;
            }
            logsb.Remove(logsb.Length - 10, 10);

            SetCyclerStepToMESData(0);
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
                case 0: typeName = "4P8S"; break;
                case 1: typeName = "4P8S_Rev"; break;
                case 2: typeName = "4P7S"; break;
                case 3: typeName = "3P8S"; break;
                case 4: typeName = "3P10S"; break;
                case 5: typeName = "3P10S_Rev"; break;
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
                                case "W2MLMTC4001": value = INSULATION_RESISTANCE_VOLT.ToString(); break;
                                case "W2MLMTC4002": value = INSULATION_RESISTANCE_TIME.ToString(); break;
                                case "W2MLMTC4003": value = befDiscRestTime; break;
                                case "W2MLMTC4004": value = discCur; break;
                                case "W2MLMTC4005": value = discTime; break;
                                case "W2MLMTC4006": value = discCurLimit; break;
                                case "W2MLMTC4007": value = aftDiscRestTime; break;
                                case "W2MLMTC4008": value = charCur; break;
                                case "W2MLMTC4009": value = charTime; break;
                                case "W2MLMTC4010": value = aftCharRestTime; break;
                                case "W2MLMTC4011": value = charCurLimit; break;
                                case "W2MLMTC4012": value = safeVoltHighLimit; break;
                                case "W2MLMTC4013": value = safeVoltLowLimit; break;
                                case "W2MLMTC4014": value = LINE_OPEN_CHECK_UPPER_LIMIT.ToString(); break;
                                case "W2MLMTC4015": value = LINE_OPEN_CHECK_LOWER_LIMIT.ToString(); break;
                                case "W2MLMTC4016": value = cellFomula1.ToString(); break;
                                case "W2MLMTC4017": value = cellFomula2.ToString(); break;
                                case "W2MLMTC4018": value = moduleFomula1.ToString(); break;
                                case "W2MLMTC4019": value = moduleFomula2.ToString(); break;
                                case "W2MLMTC4020": value = coefficientT.ToString(); break;
                                case "W2MLMTC4021": value = coefficientV.ToString(); break;
                                case "W2MLMTC4022": value = INSULATION_RESISTANCE_RAMP_TIME.ToString(); break; // jgh Ramp Up Time 추가
                                case "W2MLMTC4023": value = INSULATION_RESISTANCE_RANGE; break; // add by kyj
                                case "W2MLMTC4024": value = chroma.CON_WITHSTANDING_VOLTAGE_V.ToString(); break;        //200723 ht 추가
                                case "W2MLMTC4025": value = chroma.CON_WITHSTANDING_RAMP_UP_TIME.ToString(); break;     //200723 ht 추가
                                case "W2MLMTC4026": value = chroma.CON_WITHSTANDING_VOLTAGE_T.ToString(); break;        //200723 ht 추가
                                case "W2MLMTC4027": value = chroma.CON_ARC_LIMIT.ToString(); break;                     //200723 ht 추가
                                case "W2MLMTC4028": value = chroma.CON_ARC_ENABLE.ToString(); break;                    //200723 ht 추가
                                case "W2MLMTC4029": value = chroma.CON_WITHSTANDING_FALLDOWN_TIME.ToString(); break;    //200723 ht 추가
                                case "W2MLMTC4030": value = CONTACT_CHECK_UPPER_LIMIT.ToString(); break;    //210802 JKD 추가


                            }

                            if (value != "") sb.Append(arr[0] + "," + value + "," + arr[2] + "," + arr[3] + "\n");
                        }

                        //저장된 제어항목을 파일에 저장
                        using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "ControlList_" + typeName + ".csv", false))
                        {
                            sw.WriteLine(columnHeaders);
                            sw.Write(sb.ToString());
                        }
                    }
                }

                Thread.Sleep(1000);
            }
            catch (Exception ex) { LogState(LogType.Fail, "SetControlItemToCSV", ex); }
        }

        private void GetCollectItemFromCSV()
        {
            try
            {
                LogState(LogType.MANUALCONDITION, "Use To Local Data(Collect)");

                //eollist.csv에서 항목별 ID를 가져온다
                FileStream readData = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "eollist.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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

                    //if (arr[7 + localTypes] != "1") continue;

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
                if (item.Key == _BARCODE)
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

        private bool GetCollectItemFromMES(string LotID = "")
        {
            //MES에 수집항목을 요청한다
            string collectItem = MES.GetProcessingSpecRequest(this.modelList[selectedIndex].EquipId, this.modelList[selectedIndex].ProdId, this.modelList[selectedIndex].ProcId, LotID);
            if (collectItem == "" || collectItem == "NG")
            {
                LogState(LogType.Fail, "GetCollectItemFromMES");
                //return false;
            }

            collectItem = collectItem.Remove(collectItem.Length - 1, 1);
            bool bResult = false;

            try
            {
                //받은 데이터를 파싱한다
                string[] parsed = collectItem.Split('@');
                var str = parsed[0].Split(new string[] { "CLCITEM" }, StringSplitOptions.RemoveEmptyEntries);
                parsed[0] = "CLCITEM" + str[1];
                parsed[parsed.Length - 1] = parsed[parsed.Length - 1].Remove(parsed[parsed.Length - 1].Length - 3, 3);

                foreach (string item in parsed)
                {
                    //220913 nn 
                    string[] splitter = new string[] { "^", "=" };
                    var arr = item.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                    if (arr.Length < 6) continue;

                    string clctItem = arr[1];
                    string strArrMin = "NULL", strArrMax = "NULL";

                    double min = 0.0, max = 0.0;


                    bool dTrigger1 = double.TryParse(arr[3], out min);
                    bool dTrigger2 = double.TryParse(arr[5], out max);

                    //220913 소수점 2자리로 통일
                    strArrMin = string.Format("{0}", min);
                    strArrMax = string.Format("{0}", max);


                    foreach (var testItem in modelList[selectedIndex].TestItemList)
                    {
                        if (testItem.Value.CLCTITEM == clctItem)
                        {
                            if (!testItem.Value.Min.Equals(strArrMin))
                            {
                                if (dTrigger1) testItem.Value.Min = min;
                                else testItem.Value.Min = strArrMin;
                                bResult = true;
                            }

                            if (!testItem.Value.Max.Equals(strArrMax))
                            {
                                if (dTrigger2) testItem.Value.Max = max;
                                else testItem.Value.Max = strArrMax;
                                bResult = true;
                            }

                            break;
                        }
                    }

                    //기존에 사용한 로직
                    //string[] splitter = new string[] { "^", "=" };
                    //var arr = item.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                    //if (arr.Length < 6) continue;

                    //string clctItem = arr[1];
                    //double min = 0.0, max = 0.0;
                    //bool dTrigger1 = double.TryParse(arr[3], out min);
                    //bool dTrigger2 = double.TryParse(arr[5], out max);

                    //foreach (var testItem in modelList[selectedIndex].TestItemList)
                    //{
                    //    if (testItem.Value.CLCTITEM == clctItem)
                    //    {
                    //        if (dTrigger1) testItem.Value.Min = min;
                    //        else testItem.Value.Min = arr[3];

                    //        if (dTrigger2) testItem.Value.Max = max;
                    //        else testItem.Value.Max = arr[5];

                    //        break;
                    //    }
                    //}
                }

            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, "GetCollectItemFromMES", ex);
                return false;
            }

            if(bResult)
            {
                //MES로부터 받아온 제어항목을 CSV에 저장한다.
                SetCollectItemToCSV();
            }

            return true;
        }

        public void SetCollectItemToCSV()
        {
            string columnHeaders = "";
            List<string[]> collections = new List<string[]>();
            try
            {
                //eollist.csv를 불러온다
                using (FileStream readData = new FileStream(
                        AppDomain.CurrentDomain.BaseDirectory + "eollist.csv"
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
                        AppDomain.CurrentDomain.BaseDirectory + "eollist.csv",
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
                            if ((localTypes == 0 && collections[i][9] == "1" && collections[i][2] == testItem.Value.CLCTITEM)
                                || (localTypes == 1 && collections[i][10] == "1" && collections[i][2] == testItem.Value.CLCTITEM)
                                || (localTypes == 2 && collections[i][11] == "1" && collections[i][2] == testItem.Value.CLCTITEM)
                                || (localTypes == 3 && collections[i][12] == "1" && collections[i][2] == testItem.Value.CLCTITEM)
                                || (localTypes == 4 && collections[i][13] == "1" && collections[i][2] == testItem.Value.CLCTITEM)
                                || (localTypes == 5 && collections[i][14] == "1" && collections[i][2] == testItem.Value.CLCTITEM))
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

                Thread.Sleep(1000);
            }
            catch (Exception ex) { LogState(LogType.Fail, "SetCollectItemToCSV", ex); }
        }
         
        Thread finishedThread;

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

            //200821
            if (m_bIsWithstandNG)
            {
                LogState(LogType.NG, "Result Filter : Withstand NG");
                isSuccess = false;
            }
           
            if (MES.isMES_SYS_Disconnected)
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
               
                if (item.Key.Replace(" ", "") == _BARCODE)
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

            //var orderedList = dlist.OrderBy(x => x.Key.ToString());

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

            // 20191122 Noah Choi 검사 종료시 로그에 찍히는 수집항목 값 정렬

            char[] charSb = "^".ToCharArray();

            string[] tempStringArray = sb.ToString().Split(charSb);

            Array.Sort(tempStringArray);

            StringBuilder sortSb = new StringBuilder();

            foreach (var item in tempStringArray)
            {
                sortSb.Append(item);
                sortSb.Append("^");
            }
            sortSb.Remove(sortSb.Length - 1, 1);


            LogState(LogType.Info, "EndJobInsp - " + sortSb.ToString());

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
            LogState(LogType.Info, "Proc ID : " + this.modelList[selectedIndex].ProcId);
            LogState(LogType.Info, "Prod ID : " + this.modelList[selectedIndex].ProdId);
            LogState(LogType.Info, "Equip ID : " + this.modelList[selectedIndex].EquipId);

            if (plc.judgementFlag == true) // PLC로 시작일때만 Bit ON
            {
                plc.PreUnloadingFlag(true);
            }
                        
            if (!isMESskip)
            {
                //startJob NG Flag
                if (isFinished)
                {
                    if (MES.GetProcessDataReport(
                        this.modelList[selectedIndex].LotId,
                        this.modelList[selectedIndex].ProcId,
                        this.modelList[selectedIndex].EquipId,
                        this.modelList[selectedIndex].UserId,
                        sbc.ToString()) != string.Empty) //OK일때 시작
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
                    if (MES.EndJobInsp(
                        this.modelList[selectedIndex].LotId,
                        this.modelList[selectedIndex].ProcId,
                        this.modelList[selectedIndex].EquipId,
                        this.modelList[selectedIndex].UserId,
                        (isSuccess == true ? "OK" : "NG"), sb.ToString()) == "NG") //OK일때 시작
                    {
                        this.LogState(LogType.Fail, "MES_EndJobInsp");

                        isSuccess = false;
                    }
                    else
                    {
                        this.LogState(LogType.Success, "MES_EndJobInsp - " + sortSb.ToString());
                    }
                    PauseLoop("MES_EndJobInsp");
                }
                else
                {
                    isSuccess = false;
                }

            }

            if (plc.judgementFlag == true) // PLC로 시작일때만 Bit ON
            {
                plc.PreUnloadingFlag(false);
            }

            if (plc != null)
                plc.TestResult(isSuccess);
            
            //상세수집, 완공 결과에 따라 UI 최종결과가 결정된다.
            this.Dispatcher.Invoke(new Action(() =>
            {
                Pass(isSuccess);

                isMESSkipCb.IsEnabled = manualBt.IsEnabled = true;
                blinder3.Visibility = blinder2.Visibility = System.Windows.Visibility.Collapsed;
                _stopwatch.Stop();
                this.time_finish_tb.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }));

            if (finishedThread == null)
            {
                finishedThread = new Thread(() =>
                {
                    //빨간불이 이미 계측기때문에 들어와있다면,
                    var isComDied = relays.RelayStatus("IDO_0");
                    //relays.Reset();
                    if (!isSuccess)
                    {
                        relays.RelayOn("IDO_0");
                        Thread.Sleep(3000);
                        if (!isComDied)
                        {
                            relays.RelayOff("IDO_0");
                        }
                    }

                    relays.RelayOff("IDO_2");
                    relays.RelayOn("IDO_1");

                    if (!isMESskip)
                    {
                        relays.RelayOn("IDO_3");
                    }

                    LogState(LogType.Info, "FINISHED_TEST----------------------------------------------------", null, false);
                    finishedThread = null;

                    timetick.Stop();
                });
                finishedThread.Start();
            }


            // 20191208 Noah Choi NG판정 후 NGSkip 체크해제하도록 수정 요청(강인혁 사원)
            if (isMasterBcr == true && masterTempLot == "MASTER" || plc.masterTempBcr == "MASTER")
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    isSkipNG.IsChecked = false;
                    isSkipNG_ = false;
                    LogState(LogType.Info, "NG Skip UnCheck");
                }));
            }
        }

        //Data 값 비교 및 초기화 하는 부분 NNKIM
        #region         
        private bool GetResultCompareint(int nCompareDataSub, string strParseData,out int nCompareData, ref bool bValue)
        {
            nCompareData = 0;
            if (nCompareDataSub != int.Parse(strParseData))
            {
                //바뀐 값이 있을 경우 csv 저장을 위한 value
                if(bValue == false)
                {
                    bValue = true;
                }
            }
            nCompareData = int.Parse(strParseData);

            return bValue;
        }

        private bool GetResultCompareString(string strCompareDataSub, string strParseData,out string strCompareData, ref bool bValue)
        {
            strCompareData = "NULL";
            if (strCompareDataSub.CompareTo(strParseData) != 0)
            {
                //바뀐 값이 있을 경우 csv 저장을 위한 value
                if (bValue == false)
                {
                    bValue = true;
                }
            }
            strCompareData = strParseData;

            return bValue;
        }

        private bool GetResultCompareDouble(double dCompareDataSub, string strParseData,out double dCompareData, ref bool bValue)
        {
            dCompareData = 0.0;
            if (dCompareDataSub != double.Parse(strParseData))
            {
                //바뀐 값이 있을 경우 csv 저장을 위한 value
                if (bValue == false)
                {
                    bValue = true;
                }
            }
            dCompareData = double.Parse(strParseData);

            return bValue;
        }
        #endregion
    }
}
