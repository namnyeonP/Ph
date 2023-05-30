using EOL_BASE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace MiniScheduler
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        EOL_BASE.MainWindow mw;

        public List<Process> totalProcessList = new List<Process>();

        public MainWindow(EOL_BASE.MainWindow mw)
        {
            InitializeComponent();


            var url = AppDomain.CurrentDomain.BaseDirectory + "\\processList.xml";
            try
            {
                using (StreamReader sr = File.OpenText(url))
                {
                    XmlDocument xml = new XmlDocument();
                    FileInfo fi = new FileInfo(url);
                    if (fi.Length > 230000000)//230MB
                    {
                        xml.Load(url);//많을때는 느린방식
                    }
                    else
                    {
                        xml.LoadXml(sr.ReadToEnd());//적을때는 빠른방식
                    }

                    XmlNodeList xnList = xml.SelectNodes("ROOT");

                    foreach (XmlNode xmlnode in xnList)
                    {
                        #region get Process
                        foreach (XmlNode process in xmlnode.SelectNodes("PROCESS"))
                        {
                            var proc = new Process()
                            {
                                ProcessName = process["PROCESS_NAME"].InnerText,
                                ProcessWriter = process["PROCESS_WRITER"].InnerText,
                                ProcessDesc = process["PROCESS_DESC"].InnerText
                            };

                            #region get Schedule
                            foreach (XmlNode singleSchedule in (process.SelectNodes("SCHEDULE_LIST/SCHEDULE") as XmlNodeList))
                            {
                                var schedule = new Schedule();
                                schedule.Sname = singleSchedule["SCH_ID"].InnerText;
                                schedule.Sdescription = singleSchedule["SCH_DESC"].InnerText;

                                #region get cycles
                                XmlNodeList inner = singleSchedule.SelectNodes("CYCLE_LIST/CYCLE");
                                foreach (XmlNode xn in inner)
                                {
                                    var cycle = new Cycle();
                                    cycle.CycleName = xn["CYCLE_NO"].InnerText;
                                    cycle.Log_write_cond = xn["LOG_WRITE_COND"].InnerText;
                                    cycle.Loop_count = xn["LOOP_COUNT"].InnerText;
                                    cycle.Goto_cycle = xn["GOTO_CYCLE"].InnerText;
                                    cycle.Goto_loop_count = xn["GOTO_LOOP_COUNT"].InnerText;
                                    cycle.Goto_next_cycle = xn["GOTO_NEXT_CYCLE"].InnerText;
                                    cycle.Cycle_end_time = xn["CYCLE_END_TIME"].InnerText;
                                    cycle.Cycle_ah = xn["CYCLE_AH"].InnerText;
                                    cycle.Can_cond = xn["CAN_COND"].InnerText;

                                    XmlNodeList steps = xn.SelectNodes("STEP_LIST/STEP");
                                    #region get steps
                                    foreach (XmlNode step in steps)
                                    {
                                        var s = new Step();
                                        s.Cycle_no = step["CYCLE_NO"].InnerText;
                                        s.Step_no = step["STEP_NO"].InnerText;
                                        s.Step_type = step["STEP_TYPE"].InnerText;
                                        s.Step_mode = step["STEP_MODE"].InnerText;
                                        s.Voltage = step["VOLTAGE"].InnerText;
                                        if (step["DISCHARGE_VOLTAGE"] != null)
                                            s.Discharge_voltage = step["DISCHARGE_VOLTAGE"].InnerText;
                                        s.Current = step["CURRENT_A"].InnerText;
                                        s.Watt = step["WATT"].InnerText;

                                        //s.ClearCaseString = step["FINISH_COND"].InnerText;
                                        char[] carr = { '/', '=' };
                                        var arr = step["FINISH_COND"].InnerText.Split(carr);

                                        var clearCaseTemp = new List<ClearCase> { };
                                        for (int i = 0; i < arr.Length - 2; i++)
                                        {
                                            ClearCase clearcaseone = new ClearCase();

                                            switch (arr[i])
                                            {
                                                case "HV": clearcaseone.TitleValue = "Voltage HIGH"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "LV": clearcaseone.TitleValue = "Voltage LOW"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "Cu": clearcaseone.TitleValue = "Current"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "Ti": clearcaseone.TitleValue = "Time"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "RT": clearcaseone.TitleValue = "Real Time"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "Ca": clearcaseone.TitleValue = "용량"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "Po": clearcaseone.TitleValue = "전력"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "WH": clearcaseone.TitleValue = "전력량"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "CHVH": clearcaseone.TitleValue = "셀전압 Max 상한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "CHVL": clearcaseone.TitleValue = "셀전압 Max 하한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "CLVH": clearcaseone.TitleValue = "셀전압 Min 상한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "CLVL": clearcaseone.TitleValue = "셀전압 Min 하한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "CDHV": clearcaseone.TitleValue = "셀전압 Delta 상한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "CSHV": clearcaseone.TitleValue = "셀전압 Sum 상한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "CSLV": clearcaseone.TitleValue = "셀전압 Sum 하한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "CAHV": clearcaseone.TitleValue = "셀전압 평균 상한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "CALV": clearcaseone.TitleValue = "셀전압 평균 하한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "THVH": clearcaseone.TitleValue = "셀온도 Max 상한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "THVL": clearcaseone.TitleValue = "셀온도 Max 하한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "TLVH": clearcaseone.TitleValue = "셀온도 Min 상한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "TLVL": clearcaseone.TitleValue = "셀온도 Min 하한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "TDHV": clearcaseone.TitleValue = "셀온도 Delta 상한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "TSHV": clearcaseone.TitleValue = "셀온도 Sum 상한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "TSLV": clearcaseone.TitleValue = "셀온도 Sum 하한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "TAHV": clearcaseone.TitleValue = "셀온도 평균 상한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "TALV": clearcaseone.TitleValue = "셀온도 평균 하한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "CCCV": clearcaseone.TitleValue = "셀 CC/CV 전압 설정"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "ACP": clearcaseone.TitleValue = "충전 파워 제한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "ADP": clearcaseone.TitleValue = "방전 파워 제한"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                case "CANCOM": clearcaseone.TitleValue = "CAN통신 정지 해제"; clearcaseone.Value = arr[i + 1]; clearcaseone.NextStep = (arr[i + 2] == "0:65533" ? "Next Step" : arr[i + 2]); clearCaseTemp.Add(clearcaseone); break;
                                                default: i++; break;
                                            }
                                        }
                                        s.RATIO_COND = step["RATIO_COND"].InnerText;
                                        s.ClearcaseList = clearCaseTemp;
                                        arr = step["SAFETY_LIMIT_COND"].InnerText.Split(carr);

                                        SafeCase sc = new SafeCase();
                                        sc.VoMax = (arr[1]);
                                        sc.VoMin = (arr[3]);
                                        sc.CuMax = (arr[5]);
                                        sc.CuMin = (arr[7]);
                                        sc.CaMax = (arr[9]);
                                        sc.PoMax = (arr[11]);
                                        sc.WHMax = (arr[13]);
                                        sc.Exceed = (arr[15]);
                                        sc.CeVoMaxH = (arr[17]);
                                        sc.CeVoMaxL = (arr[19]);
                                        sc.CeVoMinH = (arr[21]);
                                        sc.CeVoMinL = (arr[23]);
                                        sc.CeDeVoMax = (arr[25]);
                                        sc.CeVoSuMax = (arr[27]);
                                        sc.CeVoSuMin = (arr[29]);
                                        sc.CeVoAvMax = (arr[31]);
                                        sc.CeVoAvMin = (arr[33]);
                                        sc.CeTeMaxH = (arr[35]);
                                        sc.CeTeMaxL = (arr[37]);
                                        sc.CeTeMinH = (arr[39]);
                                        sc.CeTeMinL = (arr[41]);
                                        sc.CeDeTeMax = (arr[43]);
                                        sc.CeTeSuMax = (arr[45]);
                                        sc.CeTeSuMin = (arr[47]);
                                        sc.CeTeAvMax = (arr[49]);
                                        sc.CeTeAvMin = (arr[51]);
                                        sc.HVIL = (arr[53]);
                                        sc.COV = (arr[55]);

                                        s.SafecaseData = sc;

                                        s.Log_write = step["LOG_WRITE_COND"].InnerText;
                                        s.Pattern_Addr = step["PATTERN_ADDR"].InnerText;
                                        s.Pattern_id = step["PATTERN_ID"].InnerText;
                                        s.Pattern_name = step["PATTERN_NAME"].InnerText;
                                        s.Can_cond = step["CAN_COND"].InnerText;
                                        s.Can_send = step["CAN_SEND"].InnerText;
                                        s.Time_method = step["TIME_METHOD"].InnerText;
                                        s.Output_mode = step["OUTPUT_MODE"].InnerText;
                                        s.Time = step["TIME"].InnerText;
                                        s.Output = step["OUTPUT"].InnerText;
                                        s.CURR_LIMIT_CHARGE = step["CURR_LIMIT_CHARGE"].InnerText;
                                        s.CURR_LIMIT_DISCHARGE = step["CURR_LIMIT_DISCHARGE"].InnerText;
                                        s.Chamber_cond = step["CHAMBER_COND"].InnerText;
                                        if (step["CHILLER_COND"] != null)
                                            s.CHILLER_COND = step["CHILLER_COND"].InnerText;
                                        s.Relay = step["RELAY"].InnerText;
                                        s.Loader = step["LOADER"].InnerText;

                                        cycle.StepList.Add(s);
                                    }
                                    #endregion

                                    schedule.CycleList.Add(cycle);
                                }

                                #endregion
                                #region get Common Cases
                                //var commonSafeCase = new CommonSafeCase()
                                //{
                                //    VoltageHigh = singleSchedule["SAFETY_COMMON"]["VOLT_HL"].InnerText,
                                //    VoltageLow = singleSchedule["SAFETY_COMMON"]["VOLT_LL"].InnerText,
                                //    CurrentHigh = singleSchedule["SAFETY_COMMON"]["CURR_HL"].InnerText,
                                //    PowerHigh = singleSchedule["SAFETY_COMMON"]["POW_HL"].InnerText,
                                //    PowerHourHigh = singleSchedule["SAFETY_COMMON"]["POWH_HL"].InnerText,
                                //    CapacityHigh = singleSchedule["SAFETY_COMMON"]["CAP_HL"].InnerText
                                //};

                                //var logCase = new LogCase()
                                //{
                                //    TimeChange = singleSchedule["LOG_COMMON"]["TIME_CHANGE"].InnerText,
                                //    CurrChange = singleSchedule["LOG_COMMON"]["CURR_CHANGE"].InnerText,
                                //    VoltChange = singleSchedule["LOG_COMMON"]["VOLT_CHANGE"].InnerText,
                                //    TempChange = singleSchedule["LOG_COMMON"]["TEMP_CHANGE"].InnerText
                                //};

                                //var chamberCase = new ChamberCase()
                                //{
                                //    TempChamber = singleSchedule["CHAMBER_COMMON"]["TEMPERATURE"].InnerText,
                                //    TempChangeChamber = singleSchedule["CHAMBER_COMMON"]["TEMP_CHANGE"].InnerText,
                                //    HumidChamber = singleSchedule["CHAMBER_COMMON"]["HUMIDITY"].InnerText,
                                //    HumidChangeChamber = singleSchedule["CHAMBER_COMMON"]["HUMID_CHANGE"].InnerText
                                //};
                                #endregion

                                proc.ScheduleList.Add(schedule);
                            }
                            #endregion

                            totalProcessList.Add(proc);
                        }
                        #endregion
                    }
                }

                this.Closed += MainWindow_Closed;
                this.Closing += MainWindow_Closing;

                this.mw = mw;
            }
            catch(Exception ec)
            {
               mw.LogState(LogType.Fail, "Load processList", ec);
            }

        }
         

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //SaveProcessToXml();
            //mw.totalProcessList = this.totalProcessList;
             
            this.Hide();

            e.Cancel = true;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            
        }

        private void SaveProcessToXml()
        {
            try
            {
                XElement root = new XElement("ROOT");

                var singleProcess = this.totalProcessList[0];

                {
                    var process = new XElement("PROCESS");
                    var pname = new XElement("PROCESS_NAME", singleProcess.ProcessName);
                    var pwriter = new XElement("PROCESS_WRITER", singleProcess.ProcessWriter);
                    var pdesc = new XElement("PROCESS_DESC", singleProcess.ProcessDesc);
                    var pscheduleList = new XElement("SCHEDULE_LIST");

                    foreach (var sch in singleProcess.ScheduleList)
                    {
                        var schedule = new XElement("SCHEDULE");
                        var sname = new XElement("SCH_ID", sch.Sname == null ? "" : sch.Sname);
                        var sdescription = new XElement("SCH_DESC", sch.Sdescription == null ? "" : sch.Sdescription);
                        var cyclelist = new XElement("CYCLE_LIST");

                        #region each cycle Save
                        foreach (var c in sch.CycleList)
                        {
                            var cycle = new XElement("CYCLE");
                            cycle.Add(new XElement("CYCLE_NO", c.CycleName));
                            cycle.Add(new XElement("SAFETY_LIMIT_COND",
                                   "VoMax=" + c.SafeCase.VoMax + "/" +
                                   "VoMin=" + c.SafeCase.VoMin + "/" +
                                   "CuMax=" + c.SafeCase.CuMax + "/" +
                                   "CuMin=" + c.SafeCase.CuMin + "/" +
                                   "CaMax=" + c.SafeCase.CaMax + "/" +
                                   "PoMax=" + c.SafeCase.PoMax + "/" +
                                   "WHMax=" + c.SafeCase.WHMax + "/" +
                                   "Exceed=" + c.SafeCase.Exceed + "/" +
                                   "CeVoMaxH=" + c.SafeCase.CeVoMaxH + "/" +
                                   "CeVoMaxL=" + c.SafeCase.CeVoMaxL + "/" +
                                   "CeVoMinH=" + c.SafeCase.CeVoMinH + "/" +
                                   "CeVoMinL=" + c.SafeCase.CeVoMinL + "/" +
                                   "CeDeVoMax=" + c.SafeCase.CeDeVoMax + "/" +
                                   "CeVoSuMax=" + c.SafeCase.CeVoSuMax + "/" +
                                   "CeVoSuMin=" + c.SafeCase.CeVoSuMin + "/" +
                                   "CeVoAvMax=" + c.SafeCase.CeVoAvMax + "/" +
                                   "CeVoAvMin=" + c.SafeCase.CeVoAvMin + "/" +
                                   "CeTeMaxH=" + c.SafeCase.CeTeMaxH + "/" +
                                   "CeTeMaxL=" + c.SafeCase.CeTeMaxL + "/" +
                                   "CeTeMinH=" + c.SafeCase.CeTeMinH + "/" +
                                   "CeTeMinL=" + c.SafeCase.CeTeMinL + "/" +
                                   "CeDeTeMax=" + c.SafeCase.CeDeTeMax + "/" +
                                   "CeTeSuMax=" + c.SafeCase.CeTeSuMax + "/" +
                                   "CeTeSuMin=" + c.SafeCase.CeTeSuMin + "/" +
                                   "CeTeAvMax=" + c.SafeCase.CeTeAvMax + "/" +
                                   "CeTeAvMin=" + c.SafeCase.CeTeAvMin + "/" +
                                   "HVIL=" + c.SafeCase.HVIL + "/" +
                                   "COV=" + c.SafeCase.COV));

                            cycle.Add(new XElement("LOG_WRITE_COND", c.Log_write_cond));
                            cycle.Add(new XElement("LOOP_COUNT", c.Loop_count));
                            cycle.Add(new XElement("GOTO_CYCLE", c.Goto_cycle));
                            cycle.Add(new XElement("GOTO_LOOP_COUNT", c.Goto_loop_count));
                            cycle.Add(new XElement("GOTO_NEXT_CYCLE", c.Goto_next_cycle));
                            cycle.Add(new XElement("CYCLE_END_TIME", c.Cycle_end_time));
                            cycle.Add(new XElement("CYCLE_AH", c.Cycle_ah));
                            cycle.Add(new XElement("CAN_COND", c.Can_cond));
                            var steplist = new XElement("STEP_LIST");

                            #region Steplist
                            foreach (var cc in c.StepList)
                            {
                                var step = new XElement("STEP");
                                step.Add(new XElement("CYCLE_NO", cc.Cycle_no));
                                step.Add(new XElement("STEP_NO", cc.Step_no));
                                step.Add(new XElement("STEP_TYPE", cc.Step_type));
                                step.Add(new XElement("STEP_MODE", cc.Step_mode));
                                step.Add(new XElement("VOLTAGE", cc.Voltage));
                                step.Add(new XElement("DISCHARGE_VOLTAGE", cc.Discharge_voltage));
                                step.Add(new XElement("CURRENT_A", cc.Current));
                                step.Add(new XElement("WATT", cc.Watt));

                                string ccl = string.Empty;
                                foreach (var clearcaseone in cc.ClearcaseList)
                                {
                                    switch (clearcaseone.TitleValue)
                                    {
                                        case "Voltage HIGH": ccl += "HV=" + clearcaseone.Value + "=" + (clearcaseone.NextStep == "Next Step" ? "0:65533" : clearcaseone.NextStep) + "/"; break;
                                        case "Voltage LOW": ccl += "LV=" + clearcaseone.Value + "=" + (clearcaseone.NextStep == "Next Step" ? "0:65533" : clearcaseone.NextStep) + "/"; break;
                                        case "Current": ccl += "Cu=" + clearcaseone.Value + "=" + (clearcaseone.NextStep == "Next Step" ? "0:65533" : clearcaseone.NextStep) + "/"; break;
                                        case "Time": ccl += "Ti=" + clearcaseone.Value + "=" + (clearcaseone.NextStep == "Next Step" ? "0:65533" : clearcaseone.NextStep) + "/"; break;
                                        default: break;
                                    }
                                }

                                //정상데이터가 아님
                                if (ccl.Length == 0)
                                {
                                    MessageBox.Show("비정상적인 데이터가 있어 저장하지 않습니다.\n스텝확인이 필요합니다.", "경고");

                                    return;
                                }

                                step.Add(new XElement("FINISH_COND", ccl.Remove(ccl.Length - 1, 1)));
                                step.Add(new XElement("SAFETY_LIMIT_COND",
                                    "VoMax=" + cc.SafecaseData.VoMax + "/" +
                                    "VoMin=" + cc.SafecaseData.VoMin + "/" +
                                    "CuMax=" + cc.SafecaseData.CuMax + "/" +
                                    "CuMin=" + cc.SafecaseData.CuMin + "/" +
                                    "CaMax=" + cc.SafecaseData.CaMax + "/" +
                                    "PoMax=" + cc.SafecaseData.PoMax + "/" +
                                    "WHMax=" + cc.SafecaseData.WHMax + "/" +
                                    "Exceed=" + cc.SafecaseData.Exceed + "/" +
                                    "CeVoMaxH=" + cc.SafecaseData.CeVoMaxH + "/" +
                                    "CeVoMaxL=" + cc.SafecaseData.CeVoMaxL + "/" +
                                    "CeVoMinH=" + cc.SafecaseData.CeVoMinH + "/" +
                                    "CeVoMinL=" + cc.SafecaseData.CeVoMinL + "/" +
                                    "CeDeVoMax=" + cc.SafecaseData.CeDeVoMax + "/" +
                                    "CeVoSuMax=" + cc.SafecaseData.CeVoSuMax + "/" +
                                    "CeVoSuMin=" + cc.SafecaseData.CeVoSuMin + "/" +
                                    "CeVoAvMax=" + cc.SafecaseData.CeVoAvMax + "/" +
                                    "CeVoAvMin=" + cc.SafecaseData.CeVoAvMin + "/" +
                                    "CeTeMaxH=" + cc.SafecaseData.CeTeMaxH + "/" +
                                    "CeTeMaxL=" + cc.SafecaseData.CeTeMaxL + "/" +
                                    "CeTeMinH=" + cc.SafecaseData.CeTeMinH + "/" +
                                    "CeTeMinL=" + cc.SafecaseData.CeTeMinL + "/" +
                                    "CeDeTeMax=" + cc.SafecaseData.CeDeTeMax + "/" +
                                    "CeTeSuMax=" + cc.SafecaseData.CeTeSuMax + "/" +
                                    "CeTeSuMin=" + cc.SafecaseData.CeTeSuMin + "/" +
                                    "CeTeAvMax=" + cc.SafecaseData.CeTeAvMax + "/" +
                                    "CeTeAvMin=" + cc.SafecaseData.CeTeAvMin + "/" +
                                    "HVIL=" + cc.SafecaseData.HVIL + "/" +
                                    "COV=" + cc.SafecaseData.COV));

                                step.Add(new XElement("LOG_WRITE_COND", cc.Log_write));
                                step.Add(new XElement("PATTERN_ADDR", cc.Pattern_Addr));
                                step.Add(new XElement("PATTERN_ID", cc.Pattern_id));
                                step.Add(new XElement("PATTERN_NAME", cc.Pattern_name));
                                step.Add(new XElement("CAN_COND", cc.Can_cond));
                                step.Add(new XElement("CAN_SEND", cc.Can_send));
                                step.Add(new XElement("TIME_METHOD", cc.Time_method));
                                step.Add(new XElement("OUTPUT_MODE", cc.Output_mode));
                                step.Add(new XElement("TIME", cc.Time));
                                step.Add(new XElement("OUTPUT", cc.Output));
                                step.Add(new XElement("CURR_LIMIT_CHARGE", cc.CURR_LIMIT_CHARGE));
                                step.Add(new XElement("CURR_LIMIT_DISCHARGE", cc.CURR_LIMIT_DISCHARGE));
                                step.Add(new XElement("CHAMBER_COND", cc.Chamber_cond));
                                step.Add(new XElement("CHILLER_COND", cc.CHILLER_COND == "" ? "-100/0/0" : cc.CHILLER_COND));
                                step.Add(new XElement("RELAY", cc.Relay));
                                step.Add(new XElement("LOADER", cc.Loader));
                                step.Add(new XElement("RATIO_COND", cc.RATIO_COND));


                                if (cc.PatternXElement != null)
                                {
                                    step.Add(cc.PatternXElement);
                                }

                                steplist.Add(step);
                            }
                            #endregion

                            cycle.Add(steplist);
                            cyclelist.Add(cycle);
                        }
                        #endregion
                        
                        schedule.Add(sname, sdescription, cyclelist);//, commonSafecase, logCase, chamberCase);
                        pscheduleList.Add(schedule);
                    }

                    process.Add(pname, pwriter, pdesc, pscheduleList);
                    root.Add(process);
                }
                root.Save(AppDomain.CurrentDomain.BaseDirectory + "\\processList.xml");


            }
            catch (XmlException xe)
            {
                mw.LogState(LogType.Fail, "Scheduler", xe);
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            
            //this.CycleListBox.ItemsSource = this.totalProcessList[0].ScheduleList[0].CycleList;
        }

        public int index_process;
        public int index_schedule;
        public int index_cycle;
        public int index_step;
        Step selectedStep;
        List<Step> selectedStepList = new List<Step>();
        public bool isNeedSave = false;

        object lastSelectedOb;
        private void cyclebt_Click(object sender, RoutedEventArgs e)
        {
            lastSelectedOb = (sender as Button).Parent;
            var lb = ((sender as Button).Parent as Grid).Children[1] as ListBox;
            lb.Focus();
            SelectAllCommand(lb, null);
            //this.stateLb.Content = "Cycle 선택됨";
        }

        private void CycleListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListBox).SelectedItems.Count == 0)
                return;

            var item = (sender as ListBox).SelectedItems[0] as Step;

            if (item == null) return;


            //찾아서 있으면 그 i가 index_cycle이죠
            //찾은건 index_step이고
            for (int i = 0; i < CycleListBox.Items.Count; i++)
            {
                var cy = CycleListBox.Items[i] as Cycle;
                index_step = cy.StepList.FindIndex(x => x == item);

                if (index_step != -1) { index_cycle = i; break; }
            }

            if (index_cycle == -1) return;

            //this.stateLb.Content = "스텝 수정.";
            PatternWindow cp = new PatternWindow(this, item, this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle]);
            cp.isModify = true;
            cp.ShowDialog();//동기화할때 Show로 하면 바뀐값이 적용안됨
            this.CycleListBox.Items.Refresh();

            saveBt.IsEnabled = true;
        }

        private void CycleListBox_Selected(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListBox).SelectedItems;

            if (item == null) return;
            if (item.Count == 0) return;

            for (int i = 0; i < CycleListBox.Items.Count; i++)
            {
                var cy = CycleListBox.Items[i] as Cycle;
                index_step = cy.StepList.FindIndex(x => x == (sender as ListBox).SelectedItems[0] as Step);

                if (index_step != -1) { index_cycle = i; break; }
            }


            selectedStepList.Clear();
            if (item.Count > 1)
            {
                foreach (var cycle in item)
                {
                    selectedStepList.Add(cycle as Step);
                }
                //this.stateLb.Content = "여러 스텝이 선택됨";
            }
            else
            {
                selectedStep = item[0] as Step;
                //this.stateLb.Content = "스텝이 선택됨";
            }

        }

        private void CreateCycleClick(object sender, RoutedEventArgs e)
        {
            var newCycle = new Cycle()
            {
                StepList = new List<Step> { new Step() { Cycle_no = (this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList.Count + 1).ToString() } },
                CycleName = (this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList.Count + 1).ToString()
            };
            this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList.Add(newCycle);

            //this.totalProcessList[index_process].ScheduleList[index_schedule].ScycleList.Add(new Step() { Cycle_no = (this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList.Count + 1).ToString() });

            this.CycleListBox.Items.Refresh();
            //this.stateLb.Content = "새 스텝 추가";
            isNeedSave = true;
        }

        private int GetStepCount()
        {
            //cycle이름이 같은거에서 step이 가장 높은거(그 리스트의 카운트)
            for (int i = 0; i < CycleListBox.Items.Count; i++)
            {
                var cy = CycleListBox.Items[i] as Cycle;
                if (cy.CycleName == selectedStep.Cycle_no)
                {
                    return cy.StepList.Count;
                }
            }
            return -1;
        }

        private void CopyCommand(object sender, RoutedEventArgs e)
        {
            cutcopyStepList = new List<Step>();
            foreach (var item in selectedStepList)
            {
                cutcopyStepList.Add(new Step()
                {
                    Cycle_no = item.Cycle_no,
                    Step_no = item.Step_no,
                    Step_type = item.Step_type,
                    Step_mode = item.Step_mode,
                    Voltage = item.Voltage,
                    Discharge_voltage = item.Discharge_voltage,
                    Current = item.Current,
                    Watt = item.Watt,
                    SafecaseData = item.SafecaseData,
                    ClearcaseList = item.ClearcaseList,
                    Log_write = item.Log_write,
                    Pattern_id = item.Pattern_id,
                    Pattern_name = item.Pattern_name,
                    Can_cond = item.Can_cond,
                    Can_send = item.Can_send,
                    Time_method = item.Time_method,
                    Output_mode = item.Output_mode,
                    Time = item.Time,
                    Output = item.Output,
                    CURR_LIMIT_CHARGE = item.CURR_LIMIT_CHARGE,
                    CURR_LIMIT_DISCHARGE = item.CURR_LIMIT_DISCHARGE,
                    Chamber_cond = item.Chamber_cond,
                    Relay = item.Relay,
                    Loader = item.Loader,
                    RATIO_COND = item.RATIO_COND,
                });
            }
            this.cutcopyStep = selectedStep;

            //this.stateLb.Content = "스텝들이 복사됨";
        }

        private void PasteCommand(object sender, RoutedEventArgs e)
        {
            //붙여넣을 사이클을 확인후에, 사이클명을 바꾼후 들어가야한다.
            var item = selectedStep;
            //들어갈 대상의 공통 cycle조건으로 바뀐다.

            if (item == null) return;

            try
            {
                if (cutcopyStepList.Count > 1)
                {
                    foreach (var cycle in cutcopyStepList)
                    {
                        this.SetCycleNStepIndex(item);
                        if (index_cycle == -1) return;

                        var cc = new Step();
                        cc = cycle;
                        cc.Cycle_no = item.Cycle_no.ToString();
                        PatternWindow cp = new PatternWindow(this, cc, this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle]);
                        cp.CopyPaste();
                    }
                    //this.stateLb.Content = "여러 아이템이 복사됨.";
                }
                else
                {
                    this.SetCycleNStepIndex(item);
                    if (index_cycle == -1) return;

                    cutcopyStep.Cycle_no = selectedStep.Cycle_no;
                    PatternWindow cp = new PatternWindow(this, cutcopyStep, this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle]);
                    cp.CopyPaste();
                    //this.stateLb.Content = "붙여넣기.";

                    //if (is덮어쓰기.IsChecked == true)
                    {
                       // this.stateLb.Content = "덮어쓰기.";
                    }
                }

                StepSetClick(this, null);
                isNeedSave = true;
            }
            catch (Exception)
            {
                //MessageBox.Show("붙여넣을 Cycle이 없습니다. 일단 Cycle을 만들어주세요.");
            }
        }

        private void CreatePatternEditorClick(object sender, RoutedEventArgs e)
        {
            var item = selectedStep;
            if (item == null)
            {
                //this.stateLb.Content = "선택된 사이클이 없습니다.";
                return;
            }

            for (int i = 0; i < CycleListBox.Items.Count; i++)
            {
                var cy = CycleListBox.Items[i] as Cycle;
                index_step = cy.StepList.FindIndex(x => x == item);

                if (index_step != -1) { index_cycle = i; break; }
            }

            if (index_cycle == -1) return;

            PatternWindow cp = new PatternWindow(this, selectedStep.Cycle_no, GetStepCount(), this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle]);
            cp.ShowDialog();


            this.CycleListBox.Items.Refresh();
            isNeedSave = true;
        }

        private void ListBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                (((VisualTreeHelper.GetChild(this.CycleListBox, 0) as Grid).Children[1] as Border).Child as ScrollViewer).LineUp();
            }
            else if (e.Delta < 0)
            {
                (((VisualTreeHelper.GetChild(this.CycleListBox, 0) as Grid).Children[1] as Border).Child as ScrollViewer).LineDown();
            }
        }


        public void SetList(List<object> list)
        {
            var step = list[0] as Step;
            var c = list[1] as Cycle;
            //선택대상의 데이터가 이름만 덜렁있을때 지우고 추가해야함
            bool isAdded = false;
            foreach (var item in this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList)
            {
                if (item.CycleName == step.Cycle_no)
                {

                    {
                        if (item.StepList.Count == 1 && item.StepList[0].Step_no == "")//스탭이 비어있는거.. 빈거일때!
                            item.StepList.Clear();

                        //이름이 같다면 스탭을 추가
                        item.StepList.Add(step);

                        isAdded = true;
                        //this.totalProcessList[index_process].ScheduleList[index_schedule].ScycleList.Add(cycle);
                        break;
                    }
                }
            }

            if (!isAdded)
            {
                //cycle이름이 같지 않다면 다른사이클에 추가해야함

                var cycle = new Cycle()
                {
                    CycleName = step.Cycle_no,
                    Can_cond = c.Can_cond,
                    Cycle_ah = c.Cycle_ah,
                    Cycle_end_time = c.Cycle_end_time,
                    Goto_cycle = c.Goto_cycle,
                    Goto_loop_count = c.Goto_loop_count,
                    Goto_next_cycle = c.Goto_next_cycle,
                    Log_write_cond = c.Log_write_cond,
                    Loop_count = c.Loop_count,
                    SafeCase = c.SafeCase,
                };
                cycle.StepList.Add(step);


                this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList.Add(cycle);

                //this.totalProcessList[index_process].ScheduleList[index_schedule].ScycleList.Add(cycle);
            }

            //어쨌든 cycle정보는 갱신



            this.CycleListBox.Items.Refresh();

        }

        public void ModifyList(List<object> list)
        {
            if (index_cycle != -1 && index_step != -1)
            {
                //스텝이 완료되서 저장된다.
                //사이클이름은 미리 정해져야 한다.
                try
                {
                    var c = list[1] as Cycle;

                    this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle].Can_cond = c.Can_cond;
                    this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle].Cycle_ah = c.Cycle_ah;
                    this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle].Cycle_end_time = c.Cycle_end_time;
                    this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle].StepList[index_step] = list[0] as Step;
                    this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle].Goto_cycle = c.Goto_cycle;
                    this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle].Goto_loop_count = c.Goto_loop_count;
                    this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle].Goto_next_cycle = c.Goto_next_cycle;
                    this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle].Log_write_cond = c.Log_write_cond;
                    this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle].Loop_count = c.Loop_count;
                    this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle].SafeCase = c.SafeCase;

                }
                catch (Exception)
                {
                    //this.stateLb.Content = "스케줄이 잘못 선택되었습니다. 수정에 실패했습니다" + index_schedule;
                }
                //this.totalProcessList[index_process].ScheduleList[index_schedule].ScycleList[scyclelist_singlecycle_index] = cycle;
            }

            //scyclelist_singlecycle_index = -1;
            index_cycle = index_step = -1;
           // this.stateLb.Content = "스텝 수정됨.";
        }

        #region Commands

        Step cutcopyStep;
        List<Step> cutcopyStepList = new List<Step>();

        private void CopyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            cutcopyStepList = new List<Step>();
            foreach (var item in selectedStepList)
            {
                cutcopyStepList.Add(new Step()
                {
                    Cycle_no = item.Cycle_no,
                    Step_no = item.Step_no,
                    Step_type = item.Step_type,
                    Step_mode = item.Step_mode,
                    Voltage = item.Voltage,
                    Discharge_voltage = item.Discharge_voltage,
                    Current = item.Current,
                    Watt = item.Watt,
                    SafecaseData = item.SafecaseData,
                    ClearcaseList = item.ClearcaseList,
                    Log_write = item.Log_write,
                    Pattern_id = item.Pattern_id,
                    Pattern_name = item.Pattern_name,
                    Can_cond = item.Can_cond,
                    Can_send = item.Can_send,
                    Time_method = item.Time_method,
                    Output_mode = item.Output_mode,
                    Time = item.Time,
                    Output = item.Output,
                    CURR_LIMIT_CHARGE = item.CURR_LIMIT_CHARGE,
                    CURR_LIMIT_DISCHARGE = item.CURR_LIMIT_DISCHARGE,
                    Chamber_cond = item.Chamber_cond,
                    Relay = item.Relay,
                    Loader = item.Loader,
                    RATIO_COND = item.RATIO_COND,
                });
            }
            this.cutcopyStep = selectedStep;

            //this.stateLb.Content = "스텝들이 복사됨";
        }

        /// <summary>
        /// step으로 현재 타겟인 cycle과 step index를 찾는다.
        /// </summary>
        /// <param name="item"></param>
        private void SetCycleNStepIndex(Step item)
        {
            for (int i = 0; i < CycleListBox.Items.Count; i++)
            {
                var cy = CycleListBox.Items[i] as Cycle;
                index_step = cy.StepList.FindIndex(x => x == item);

                if (index_step != -1) { index_cycle = i; break; }
            }
        }

        private void PasteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //200914 pjh, 실제 사용하는 스케줄(index:0)은 스텝 추가 / 삭제 / 편집이 되면 안됨
            if (index_schedule == 0)
            {
                return;
            }

            //붙여넣을 사이클을 확인후에, 사이클명을 바꾼후 들어가야한다.
            var item = selectedStep;
            //들어갈 대상의 공통 cycle조건으로 바뀐다.

            if (item == null) return;

            try
            {
                if (cutcopyStepList.Count > 1)
                {
                    foreach (var cycle in cutcopyStepList)
                    {
                        this.SetCycleNStepIndex(item);
                        if (index_cycle == -1) return;

                        var cc = new Step();
                        cc = cycle;
                        cc.Cycle_no = item.Cycle_no.ToString();
                        PatternWindow cp = new PatternWindow(this, cc, this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle]);
                        cp.CopyPaste();
                    }
                    //this.stateLb.Content = "여러 아이템이 복사됨.";
                }
                else
                {
                    this.SetCycleNStepIndex(item);
                    if (index_cycle == -1) return;


                    cutcopyStep.Cycle_no = selectedStep.Cycle_no;
                    PatternWindow cp = new PatternWindow(this, cutcopyStep, this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[index_cycle]);
                    cp.CopyPaste();
                    //this.stateLb.Content = "붙여넣기.";

                   // if (is덮어쓰기.IsChecked == true)
                    {
                     //   this.stateLb.Content = "덮어쓰기.";
                    }
                }
                StepSetClick(this, null);
                isNeedSave = true;
            }
            catch (Exception)
            {
                //MessageBox.Show("붙여넣을 Cycle이 없습니다. 일단 Cycle을 만들어주세요.");
            }
        }

        private void StepSetClick(object sender, RoutedEventArgs e)
        {

            if (index_process == -1 && index_schedule == -1)
            {
                MessageBox.Show("Not Loaded Data");
            }
            foreach (var cycle in this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList)
            {
                for (int i = 0; i < cycle.StepList.Count; i++)
                {
                    cycle.StepList[i].Step_no = (i + 1).ToString();
                }
            }

            for (int i = 0; i < this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList.Count; i++)
            {
                this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList[i].CycleName = (i + 1).ToString();

                foreach (var cy in this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList[i].StepList)
                {
                    cy.Cycle_no = (i + 1).ToString();
                }
            }

            this.CycleListBox.ItemsSource = this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList;
            this.CycleListBox.Items.Refresh();

            //this.stateLb.Content = "스탭 정렬됨.";
        }

        private void DeleteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //200914 pjh, 실제 사용하는 스케줄(index:0)은 스텝 추가 / 삭제 / 편집이 되면 안됨
            if (index_schedule == 0)
            {
                return;
            }

            this.DeleteItemClick(this, null);
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            var item = selectedStep;

            if (item == null) return;

            if (selectedStepList.Count == 0)
            {
                var yes = MessageBox.Show("Selected step is deleted.", "Warning", MessageBoxButton.YesNo);
                if (yes == MessageBoxResult.Yes)
                {
                    for (int i = 0; i < CycleListBox.Items.Count; i++)
                    {
                        var cy = CycleListBox.Items[i] as Cycle;
                        int index1 = cy.StepList.FindIndex(x => x == item); // cyclelist에서 singlecycle의 index
                        if (index1 != -1)
                        {
                            //(CycleListBox.Items[i] as Cycle).CycleList.RemoveAt(index1);

                            this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList[i].StepList.RemoveAt(index1);
                            //this.totalProcessList[index_process].ScheduleList[this.index_schedule].ScycleList.Remove(item);
                            //clist[i].CycleList.RemoveAt(index1); 바인딩되서 같이 지워지는거같다!!

                            if (this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList[i].StepList.Count == 0)
                            {
                                this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList.RemoveAt(i);
                            }
                            this.CycleListBox.Items.Refresh();
                            selectedStep = null;
                            break;
                        }
                    }
                    //this.stateLb.Content = "단일 사이클이 삭제됨.";
                    isNeedSave = true;
                }
            }
            else
            {
                var yes = MessageBox.Show("Selected steps are deleted.", "Warning", MessageBoxButton.YesNo);
                if (yes == MessageBoxResult.Yes)
                {
                    //같은 사이클내에 있다
                    for (int i = 0; i < CycleListBox.Items.Count; i++)
                    {
                        var cy = CycleListBox.Items[i] as Cycle;
                        int index1 = cy.StepList.FindIndex(x => x == item); // cyclelist에서 singlecycle의 index
                        if (index1 != -1)
                        {
                            foreach (var occ in selectedStepList)
                            {
                                this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList[i].StepList.Remove(occ);
                                //this.totalProcessList[index_process].ScheduleList[this.index_schedule].ScycleList.Remove(occ);
                            }
                            if (this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList[i].StepList.Count == 0)
                            {
                                this.totalProcessList[index_process].ScheduleList[this.index_schedule].CycleList.RemoveAt(i);
                            }

                            this.CycleListBox.Items.Refresh();
                            selectedStep = null;
                            selectedStepList.Clear();
                            break;
                        }
                    }
                    //this.stateLb.Content = "여러 사이클이 삭제됨.";
                    isNeedSave = true;
                }
            }
        }

        private void CutCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //200914 pjh, 실제 사용하는 스케줄(index:0)은 스텝 추가 / 삭제 / 편집이 되면 안됨
            if (index_schedule == 0)
            {
                return;
            }

            this.Cut(this, null);
        }

        private void SelectAllCommand(object sender, ExecutedRoutedEventArgs e)
        {
            (sender as ListBox).SelectAll();

            var item = (sender as ListBox).SelectedItems;

            selectedStepList.Clear();
            if (item.Count > 1)
            {
                foreach (var cycle in item)
                {
                    selectedStepList.Add(cycle as Step);
                }
            }
            //this.stateLb.Content = "모든 스텝이 선택됨.";
        }

        private void Cut(object sender, RoutedEventArgs e)
        {
            var item = selectedStep;

            if (item == null) return;

            cutcopyStepList.Clear();
            foreach (var cycle in selectedStepList)
            {
                cutcopyStepList.Add(cycle);
            }

            if (selectedStepList.Count == 0)
            {
                for (int i = 0; i < CycleListBox.Items.Count; i++)
                {
                    var cy = CycleListBox.Items[i] as Cycle;
                    int index1 = cy.StepList.FindIndex(x => x == item); // cyclelist에서 singlecycle의 index
                    if (index1 != -1)
                    {
                        //(CycleListBox.Items[i] as Cycle).CycleList.RemoveAt(index1);
                        this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[i].StepList.RemoveAt(index1);
                        //this.totalProcessList[index_process].ScheduleList[this.index_schedule].ScycleList.Remove(item);

                        if (this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[i].StepList.Count == 0)
                        {
                            this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList.RemoveAt(i);
                        }

                        cutcopyStep = selectedStep;

                        selectedStep = null;
                        //this.stateLb.Content = "단일 사이클 잘라내기.";
                        isNeedSave = true;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < CycleListBox.Items.Count; i++)
                {
                    var cy = CycleListBox.Items[i] as Cycle;
                    int index1 = cy.StepList.FindIndex(x => x == item); // cyclelist에서 singlecycle의 index
                    if (index1 != -1)
                    {
                        foreach (var occ in selectedStepList)
                        {
                            this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[i].StepList.Remove(occ);
                            //this.totalProcessList[index_process].ScheduleList[this.index_schedule].ScycleList.Remove(occ);
                        }

                        if (this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList[i].StepList.Count == 0)
                        {
                            this.totalProcessList[index_process].ScheduleList[index_schedule].CycleList.RemoveAt(i);
                        }

                        selectedStep = null;
                        selectedStepList.Clear();
                        //this.stateLb.Content = "여러 사이클 잘라내기";
                        isNeedSave = true;
                        break;
                    }
                }

            }
            this.CycleListBox.Items.Refresh();
        }

        #endregion

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            index_schedule = (sender as ComboBox).SelectedIndex;
            this.CycleListBox.ItemsSource = this.totalProcessList[0].ScheduleList[index_schedule].CycleList;

            saveBt.IsEnabled = true;

            //200914 pjh, 실제 사용하는 스케줄(index:0)은 스텝 추가 / 삭제 / 편집이 되면 안됨
            if(index_schedule == 0)
            {
                insertStepBt.IsEnabled = deleteStepBt.IsEnabled = false;
            }
            else
            {
                insertStepBt.IsEnabled = deleteStepBt.IsEnabled = true;
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            SaveProcessToXml();

            //var list = this.totalProcessList[0].ScheduleList[0].CycleList[0].StepList;

            //string tmptime0 = list[0].ClearcaseList[3].Value.Replace("00:00:00:", "");
            //string tmptime1 = list[1].ClearcaseList[3].Value.Replace("00:00:00:", "");
            //string tmptime2 = list[2].ClearcaseList[3].Value.Replace("00:00:00:", "");
            //string tmptime3 = list[3].ClearcaseList[3].Value.Replace("00:00:00:", "");
            //string tmptime4 = list[4].ClearcaseList[3].Value.Replace("00:00:00:", "");

            //tmptime0 = int.Parse(tmptime0).ToString("D2");
            //tmptime1 = int.Parse(tmptime1).ToString("D2");
            //tmptime2 = int.Parse(tmptime2).ToString("D2");
            //tmptime3 = int.Parse(tmptime3).ToString("D2");
            //tmptime4 = int.Parse(tmptime4).ToString("D2");

            //mw.befDiscRestTime = tmptime0;
            //mw.discCur =   list[1].Current;
            //mw.discTime =  tmptime1;
            //mw.discCurLimit =  list[1].SafecaseData.CuMax;
            //mw.aftDiscRestTime =   tmptime2;
            //mw.charCur =    list[3].Current; ;
            //mw.charTime = tmptime3;
            //mw.charCurLimit =  list[3].SafecaseData.CuMax;
            //mw.aftCharRestTime =  tmptime4;
            //mw.safeVoltHighLimit =  list[3].SafecaseData.VoMax;
            //mw.safeVoltLowLimit =  list[1].SafecaseData.VoMin;


            //mw.SetControlItemToCSV();

            saveBt.IsEnabled = false;
        }

        private void CyclerSafetyClick(object sender, RoutedEventArgs e)
        {
            mw.clsw.Show();
        }

        Thread userCycler;
        private void StartSchedule(object sender, RoutedEventArgs e)
        {
            if (userCycler != null)
                return;

            var ti = new EOL_BASE.클래스.TestItem() { Name = "TEST", Max = "NULL", Min = "NULL" };
                       
            userCycler = new Thread(() =>
            {
                mw.isStop = false;
                if (!mw.ModeSet(ti))
                {
                    userCycler = null;
                    return;
                }

                mw.cycler.isCyclerStop = false;

                mw.Do_UserSchedule(index_schedule);

                mw.ModeSet_Release(ti);
                
                mw.cycler.isCyclerStop = true;

                userCycler = null;
            });

            userCycler.Start();

        }

        private void StopSchedule(object sender, RoutedEventArgs e)
        {
            mw.LogState(LogType.Info, "EmergencyStop Button Clicked");
            mw.isEmg_ = true;
            mw.isStop = true;
        }


    }

    public class Step
    {
        string clearCaseString = "";

        public string ClearCaseString
        {
            get { return clearCaseString; }
            set { clearCaseString = value; }
        }
        string cycle_no = "";

        public string Cycle_no
        {
            get { return cycle_no; }
            set { cycle_no = value; }
        }
        string step_no = "";

        public string Step_no
        {
            get { return step_no; }
            set { step_no = value; }
        }
        string step_type = "";

        public string Step_type
        {
            get { return step_type; }
            set { step_type = value; }
        }
        string step_mode = "";

        public string Step_mode
        {
            get { return step_mode; }
            set { step_mode = value; }
        }
        string voltage = "";

        public string Voltage
        {
            get { return voltage; }
            set { voltage = value; }
        }
        string current = "";

        public string Current
        {
            get { return current; }
            set { current = value; }
        }
        string watt = "";

        public string Watt
        {
            get { return watt; }
            set { watt = value; }
        }
        List<ClearCase> clearcaseList = new List<ClearCase>();

        public List<ClearCase> ClearcaseList
        {
            get { return clearcaseList; }
            set
            {

                clearcaseList = value;
                string ccl = string.Empty;

                foreach (var cc in clearcaseList)
                {
                    if (cc.Value.ToString() != "0.000" && cc.Value.ToString() != "0"
                        && cc.Value.ToString() != "100.0" && cc.Value.ToString() != "-100.0" && cc.Value.ToString() != "00:00:00" && cc.Value.ToString() != "00:00:00:00"
                        )
                    {
                        ccl += cc.TitleValue + " = " + cc.Value + " → " + cc.NextStep + " or ";
                    }
                }
                if (ccl.Length == 0)
                {
                    ClearCaseString = "";

                    if (this.rATIO_COND != "0/0:0/0.0/0:65533" && this.rATIO_COND != "")
                    {
                        var rtmp = rATIO_COND.Split('/');
                        var use = "WH";
                        if (rtmp[0] == "2")
                            use = "AH";
                        var center = "C" + rtmp[1].Split(':')[0] + "S" + rtmp[1].Split(':')[1];
                        var val = rtmp[2];
                        var gotoval = rtmp[3];
                        switch (rtmp[3])
                        {
                            case "0:65533": gotoval = "Next Step"; break;
                            case "0:65534": gotoval = "Pause"; break;
                            case "0:65535": gotoval = "Completion"; break;
                            default: gotoval = "C" + rtmp[3].Split(':')[0] + "S" + rtmp[3].Split(':')[1]; break;
                        }

                        ClearCaseString += center + "(" + use + ") = " + val + "% → " + gotoval;
                    }
                }
                else
                {
                    ClearCaseString = ccl.Remove(ccl.Length - 3, 3);


                    if (this.rATIO_COND != "0/0:0/0.0/0:65533" && this.rATIO_COND != "")
                    {
                        var rtmp = rATIO_COND.Split('/');
                        var use = "WH";
                        if (rtmp[0] == "2")
                            use = "AH";
                        var center = "C" + rtmp[1].Split(':')[0] + "S" + rtmp[1].Split(':')[1];
                        var val = rtmp[2];
                        var gotoval = rtmp[3];
                        switch (rtmp[3])
                        {
                            case "0:65533": gotoval = "Next Step"; break;
                            case "0:65534": gotoval = "Pause"; break;
                            case "0:65535": gotoval = "Completion"; break;
                            default: gotoval = "C" + rtmp[3].Split(':')[0] + "S" + rtmp[3].Split(':')[1]; break;
                        }

                        ClearCaseString += " or " + center + "(" + use + ") = " + val + "% → " + gotoval;
                    }
                }

            }
        }
        SafeCase safecaseData = new SafeCase();

        public SafeCase SafecaseData
        {
            get { return safecaseData; }
            set { safecaseData = value; }
        }
        string log_write = "Ti=1.0/Vo=0.000/Cu=0.000/Te=";

        public string Log_write
        {
            get { return log_write; }
            set { log_write = value; }
        }
        string pattern_id = "";

        public string Pattern_id
        {
            get { return pattern_id; }
            set { pattern_id = value; }
        }
        string pattern_name = "";

        public string Pattern_name
        {
            get { return pattern_name; }
            set { pattern_name = value; }
        }
        string can_cond = "";

        public string Can_cond
        {
            get { return can_cond; }
            set { can_cond = value; }
        }
        string can_send = "";

        public string Can_send
        {
            get { return can_send; }
            set { can_send = value; }
        }
        string time_method = "";

        public string Time_method
        {
            get { return time_method; }
            set { time_method = value; }
        }
        string output_mode = "";

        public string Output_mode
        {
            get { return output_mode; }
            set { output_mode = value; }
        }
        string time = "";

        public string Time
        {
            get { return time; }
            set { time = value; }
        }
        string output = "";

        public string Output
        {
            get { return output; }
            set { output = value; }
        }

        string cURR_LIMIT_CHARGE = "";
        string cURR_LIMIT_DISCHARGE = "";

        public string CURR_LIMIT_DISCHARGE
        {
            get { return cURR_LIMIT_DISCHARGE; }
            set { cURR_LIMIT_DISCHARGE = value; }
        }

        public string CURR_LIMIT_CHARGE
        {
            get { return cURR_LIMIT_CHARGE; }
            set { cURR_LIMIT_CHARGE = value; }
        }

        string chamber_cond = "";

        public string Chamber_cond
        {
            get { return chamber_cond; }
            set { chamber_cond = value; }
        }
        string relay = "";

        public string Relay
        {
            get { return relay; }
            set { relay = value; }
        }
        string loader = "";

        public string Loader
        {
            get { return loader; }
            set { loader = value; }
        }

        string rATIO_COND = "";

        public string RATIO_COND
        {
            get { return rATIO_COND; }
            set { rATIO_COND = value; }
        }

        string discharge_voltage = "";

        public string Discharge_voltage
        {
            get { return discharge_voltage; }
            set { discharge_voltage = value; }
        }

        string cHILLER_COND = "";

        public string CHILLER_COND
        {
            get { return cHILLER_COND; }
            set { cHILLER_COND = value; }
        }


        XElement patternXElement;

        public XElement PatternXElement
        {
            get { return patternXElement; }
            set { patternXElement = value; }
        }

        string pattern_Addr = "";

        public string Pattern_Addr
        {
            get { return pattern_Addr; }
            set { pattern_Addr = value; }
        }

        public Step()
        {
        }
    }

    public class ClearCase
    {
        string titleValue = "";

        string typeValue = "";

        string value = "0.000";

        string nextStep = "";

        public string NextStep
        {
            get { return nextStep; }
            set { nextStep = value; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string TypeValue
        {
            get { return typeValue; }
            set { typeValue = value; }
        }

        public string TitleValue
        {
            get { return titleValue; }
            set { titleValue = value; }
        }

        public ClearCase()
        {
        }

    }

    public class Cycle
    {
        string cycleName;
        List<Step> stepList = new List<Step>();
        SafeCase safeCase = new SafeCase();

        string log_write_cond = "Ti=1.0/Vo=0.000/Cu=0.000/Te=";

        public string Log_write_cond
        {
            get { return log_write_cond; }
            set { log_write_cond = value; }
        }
        string loop_count = "1";

        public string Loop_count
        {
            get { return loop_count; }
            set { loop_count = value; }
        }
        string goto_cycle = "0";

        public string Goto_cycle
        {
            get { return goto_cycle; }
            set { goto_cycle = value; }
        }
        string goto_loop_count = "0";

        public string Goto_loop_count
        {
            get { return goto_loop_count; }
            set { goto_loop_count = value; }
        }
        string goto_next_cycle = "0";

        public string Goto_next_cycle
        {
            get { return goto_next_cycle; }
            set { goto_next_cycle = value; }
        }
        string cycle_end_time = "0";

        public string Cycle_end_time
        {
            get { return cycle_end_time; }
            set { cycle_end_time = value; }
        }
        string cycle_ah = "0";

        public string Cycle_ah
        {
            get { return cycle_ah; }
            set { cycle_ah = value; }
        }
        string can_cond = "";

        public string Can_cond
        {
            get { return can_cond; }
            set { can_cond = value; }
        }

        public SafeCase SafeCase
        {
            get { return safeCase; }
            set { safeCase = value; }
        }

        public List<Step> StepList
        {
            get { return stepList; }
            set { stepList = value; }
        }

        public string CycleName
        {
            get { return cycleName; }
            set { cycleName = value; }
        }

        public Cycle()
        {
        }
    }

    public class SafeCase
    {
        string voMax = "0.000";

        public string VoMax
        {
            get { return voMax; }
            set { voMax = value; }
        }
        string voMin = "0.000";

        public string VoMin
        {
            get { return voMin; }
            set { voMin = value; }
        }

        string cuMax = "0.000";

        public string CuMax
        {
            get { return cuMax; }
            set { cuMax = value; }
        }
        string cuMin = "0.000";

        public string CuMin
        {
            get { return cuMin; }
            set { cuMin = value; }
        }

        string caMax = "0.000";

        public string CaMax
        {
            get { return caMax; }
            set { caMax = value; }
        }

        string poMax = "0.000";

        public string PoMax
        {
            get { return poMax; }
            set { poMax = value; }
        }

        string wHMax = "0.000";

        public string WHMax
        {
            get { return wHMax; }
            set { wHMax = value; }
        }

        string exceed = "0";

        public string Exceed
        {
            get { return exceed; }
            set { exceed = value; }
        }
        string ceVoMaxH = "0.000";

        public string CeVoMaxH
        {
            get { return ceVoMaxH; }
            set { ceVoMaxH = value; }
        }
        string ceVoMaxL = "0.000";

        public string CeVoMaxL
        {
            get { return ceVoMaxL; }
            set { ceVoMaxL = value; }
        }
        string ceVoMinH = "0.000";

        public string CeVoMinH
        {
            get { return ceVoMinH; }
            set { ceVoMinH = value; }
        }
        string ceVoMinL = "0.000";

        public string CeVoMinL
        {
            get { return ceVoMinL; }
            set { ceVoMinL = value; }
        }
        string ceDeVoMax = "0.000";

        public string CeDeVoMax
        {
            get { return ceDeVoMax; }
            set { ceDeVoMax = value; }
        }
        string ceVoSuMax = "0.000";

        public string CeVoSuMax
        {
            get { return ceVoSuMax; }
            set { ceVoSuMax = value; }
        }
        string ceVoSuMin = "0.000";

        public string CeVoSuMin
        {
            get { return ceVoSuMin; }
            set { ceVoSuMin = value; }
        }
        string ceVoAvMax = "0.000";

        public string CeVoAvMax
        {
            get { return ceVoAvMax; }
            set { ceVoAvMax = value; }
        }
        string ceVoAvMin = "0.000";

        public string CeVoAvMin
        {
            get { return ceVoAvMin; }
            set { ceVoAvMin = value; }
        }
        string ceTeMaxH = "100.0";

        public string CeTeMaxH
        {
            get { return ceTeMaxH; }
            set { ceTeMaxH = value; }
        }
        string ceTeMaxL = "-100.0";

        public string CeTeMaxL
        {
            get { return ceTeMaxL; }
            set { ceTeMaxL = value; }
        }
        string ceTeMinH = "100.0";

        public string CeTeMinH
        {
            get { return ceTeMinH; }
            set { ceTeMinH = value; }
        }
        string ceTeMinL = "-100.0";

        public string CeTeMinL
        {
            get { return ceTeMinL; }
            set { ceTeMinL = value; }
        }
        string ceDeTeMax = "100.0";

        public string CeDeTeMax
        {
            get { return ceDeTeMax; }
            set { ceDeTeMax = value; }
        }
        string ceTeSuMax = "100.0";

        public string CeTeSuMax
        {
            get { return ceTeSuMax; }
            set { ceTeSuMax = value; }
        }
        string ceTeSuMin = "-100.0";

        public string CeTeSuMin
        {
            get { return ceTeSuMin; }
            set { ceTeSuMin = value; }
        }
        string ceTeAvMax = "100.0";

        public string CeTeAvMax
        {
            get { return ceTeAvMax; }
            set { ceTeAvMax = value; }
        }
        string ceTeAvMin = "-100.0";

        public string CeTeAvMin
        {
            get { return ceTeAvMin; }
            set { ceTeAvMin = value; }
        }
        string hVIL = "0";

        public string HVIL
        {
            get { return hVIL; }
            set { hVIL = value; }
        }
        string cOV = "0";

        public string COV
        {
            get { return cOV; }
            set { cOV = value; }
        }

        //string voMax     = "0.000";
        //string voMin     = "0.000";
        //string cuMax     = "0.000";
        //string cuMin     = "0.000";
        //string caMax     = "0.000";
        //string poMax     = "0.000";
        //string wHMax     = "0.000";
        //string exceed    = 0;
        //string ceVoMaxH       = "0.000";
        //string ceVoMaxL       = "0.000";
        //string ceVoMinH       = "0.000";
        //string ceVoMinL       = "0.000";
        //string ceDeVoMax       = "0.000";
        //string ceVoSuMax       = "0.000";
        //string ceVoSuMin       = "0.000";
        //string ceVoAvMax       = "0.000";
        //string ceVoAvMin       = "0.000";
        //string ceTeMaxH          = "100.0";
        //string ceTeMaxL          = "-100.0";
        //string ceTeMinH          = "100.0";
        //string ceTeMinL          = "-100.0";
        //string ceDeTeMax       = "100.0";
        //string ceTeSuMax       = "100.0";
        //string ceTeSuMin       = "-100.0";
        //string ceTeAvMax       = "100.0";
        //string ceTeAvMin       = "-100.0";
        //string hVIL              = 0;
        //string cOV              = 0;
        //VoMax
        //VoMin
        //CuMax
        //CuMin
        //CaMax
        //PoMax
        //WHMax
        //Exceed   
        //CeVoMaxH 
        //CeVoMaxL 
        //CeVoMinH 
        //CeVoMinL 
        //CeDeVoMax
        //CeVoSuMax
        //CeVoSuMin
        //CeVoAvMax
        //CeVoAvMin
        //CeTeMaxH 
        //CeTeMaxL 
        //CeTeMinH 
        //CeTeMinL 
        //CeDeTeMax
        //CeTeSuMax
        //CeTeSuMin
        //CeTeAvMax
        //CeTeAvMin
        //HVIL     
        //COV
        public SafeCase()
        {
        }
    }

    public class Process
    {

        string processName;
        string processWriter;
        string processDesc;
        List<Schedule> scheduleList = new List<Schedule>();

        public string ProcessDesc
        {
            get { return processDesc; }
            set { processDesc = value; }
        }


        public string ProcessWriter
        {
            get { return processWriter; }
            set { processWriter = value; }
        }

        public string ProcessName
        {
            get { return processName; }
            set { processName = value; }
        }

        /// <summary>
        /// 0 : DCIR
        /// 1 : Current Check
        /// </summary>
        public List<Schedule> ScheduleList
        {
            get { return scheduleList; }
            set { scheduleList = value; }
        }
        public Process()
        {
        }
    }

    public interface ICloneable<T>
    {
        T Clone();
    }

    public class Schedule : ICloneable<Schedule>
    {
        string sname;
        string sdescription;
        List<Cycle> cycleList = new List<Cycle>();

        public List<Cycle> CycleList
        {
            get { return cycleList; }
            set { cycleList = value; }
        }
        
        public string Sdescription
        {
            get { return sdescription; }
            set { sdescription = value; }
        }

        public string Sname
        {
            get { return sname; }
            set { sname = value; }
        }

        public Schedule()
        {
        }

        public Schedule Clone()
        {

            return new Schedule()
            {
                cycleList = cycleList,
                sdescription = sdescription,
                sname = sname
            };
        }
    }
}
