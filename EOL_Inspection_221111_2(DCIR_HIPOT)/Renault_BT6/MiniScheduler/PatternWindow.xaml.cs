using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace MiniScheduler
{
    /// <summary>
    /// PatternWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PatternWindow : Window
    {
        #region Fields

        List<Object> targetSteps = new List<Object>();
        public MainWindow mw;
        public bool isModify = false;
        string modeString = string.Empty;
        string typeString = string.Empty;

        #endregion

        #region Ctor

        public PatternWindow(MainWindow mw, Step step, Cycle cycle)
        {
            this.mw = mw;
            InitializeComponent();

            this.cycle_cycle.Text = step.Cycle_no == "" ? "1" : step.Cycle_no;
            this.step_cycle.Text = step.Step_no == "" ? "1" : step.Step_no;
            InitializeClearCase();
            InitializeToStep(step);
            InitializeToCycle(cycle);

            this.Loaded += PatternWindow_Loaded;
        }

        void PatternWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(isCharge.IsChecked == false && isDischarge.IsChecked == false && isRest.IsChecked == false)
                isCharge.IsChecked = true;
        }

        /// <summary>
        /// 스텝추가로 들어왔을때
        /// </summary>
        /// <param name="mw"></param>
        /// <param name="cname"></param>
        /// <param name="sname"></param>
        /// <param name="cycle"></param>
        public PatternWindow(MainWindow mw, string cname, int sname, Cycle cycle)
        {
            this.mw = mw;
            InitializeComponent();
            InitializeClearCase();
            InitializeToStep(new Step());
            InitializeToCycle(cycle);
            this.cycle_cycle.Text = cname;
            if (sname != -1)
            {
                this.step_cycle.Text = (sname + 1).ToString();
            }

            this.Loaded += PatternWindow_Loaded;
        }


        #endregion

        #region Events


        void MakeNFinish()
        {
            List<ClearcaseUC> selectedClearcaseList = new List<ClearcaseUC>();

            //비우는 로직 사라짐
            //유저에 의해 변경될 여지 X
            foreach (var item in this.clearCaseSP.Children)
            {
                var item1 = (ClearcaseUC)item;
                selectedClearcaseList.Add(item1);
            }

            List<ClearCase> ccList = new List<ClearCase>();
            foreach (var item in selectedClearcaseList)
            {
                var value = item.valueTb.Text;

                if (item.cb.IsChecked == true)
                {
                    if (item.titleLb.Content.ToString() == "Time" || item.titleLb.Content.ToString() == "Real Time")
                    {
                        value = item.timePicker.DisplayDays + ":" + item.timePicker.DisplayTimeHours + ":" + item.timePicker.DisplayTimeMinutes + ":" + item.timePicker.DisplayTimeSeconds;
                        if (value == "00:00:00:00")
                            continue;
                    }

                }

                ccList.Add(new ClearCase() { TitleValue = item.titleLb.Content.ToString(), TypeValue = item.typeLb.Content.ToString(), Value = value, NextStep = item.Combobox.SelectedItem.ToString() });
            }

            foreach (var item in modeSP.Children)
            {
                var radio = item as RadioButton;
                if (radio.IsChecked == true)
                {
                    modeString = radio.Content.ToString();
                    break;
                }
            }

            foreach (var item in typeSP.Children)
            {
                var radio = item as RadioButton;
                if (radio.IsChecked == true)
                {
                    typeString = radio.Content.ToString();
                    break;
                }
            }

            SafeCase sc = new SafeCase()
            {
                VoMax = this.VoMax_step.Text,
                VoMin = this.VoMin_step.Text,
                CuMax = this.CuMax_step.Text,
                CuMin = this.CuMin_step.Text,
                CaMax = this.CaMax_step.Text,
                PoMax = this.PoMax_step.Text,
                WHMax = this.WHMax_step.Text,
                Exceed = this.Exceed_step.Text,
                CeVoMaxH = this.CeVoMaxH_step.Text,
                CeVoMaxL = this.CeVoMaxL_step.Text,
                CeVoMinH = this.CeVoMinH_step.Text,
                CeVoMinL = this.CeVoMinL_step.Text,
                CeDeVoMax = this.CeDeVoMax_step.Text,
                CeVoSuMax = this.CeVoSuMax_step.Text,
                CeVoSuMin = this.CeVoSuMin_step.Text,
                CeVoAvMax = this.CeVoAvMax_step.Text,
                CeVoAvMin = this.CeVoAvMin_step.Text,
                CeTeMaxH = this.CeTeMaxH_step.Text,
                CeTeMaxL = this.CeTeMaxL_step.Text,
                CeTeMinH = this.CeTeMinH_step.Text,
                CeTeMinL = this.CeTeMinL_step.Text,
                CeDeTeMax = this.CeDeTeMax_step.Text,
                CeTeSuMax = this.CeTeSuMax_step.Text,
                CeTeSuMin = this.CeTeSuMin_step.Text,
                CeTeAvMax = this.CeTeAvMax_step.Text,
                CeTeAvMin = this.CeTeAvMin_step.Text,
                HVIL = this.HVIL_step.Text,
                COV = this.COV_step.Text
            };

            var data = "0/0:0/0.0/0:65533";
            if (this.ratio_cond1_step.SelectedIndex > 0)
            {

                var data1 = this.ratio_cond1_step.SelectedIndex.ToString();
                var data2 = "1:1";
                var data3 = "50";
                var data4 = "0:65533";

                var dp = this.ratio_cond3_step.SelectedValue.ToString().Split('S');
                var cy = dp[0].Remove(0, 1);
                var st = dp[1];
                data2 = cy + ":" + st;

                data3 = this.ratio_cond2_step.Text;//비율


                if (this.ratio_cond4_step.SelectedIndex == 0) { data4 = "0:65533"; }
                else if (this.ratio_cond4_step.SelectedIndex == 1) { data4 = "0:65534"; }
                else if (this.ratio_cond4_step.SelectedIndex == 2) { data4 = "0:65535"; }
                else
                {


                    dp = this.ratio_cond4_step.SelectedItem.ToString().Split('S');
                    cy = dp[0].Remove(0, 1);
                    st = dp[1];
                    data4 = cy + ":" + st;
                }
                data = data1 + "/" + data2 + "/" + data3 + "/" + data4;
            }
            Step step = new Step()
            {
                RATIO_COND = data,
                Cycle_no = this.cycle_cycle.Text == "" ? "1" : this.cycle_cycle.Text,
                Step_no = this.step_cycle.Text == "" ? "1" : this.step_cycle.Text,
                Step_type = typeString,
                Step_mode = modeString,
                Voltage = this.voltage_step.Text,
                Discharge_voltage = this.disvoltage_step.Text,
                Current = this.current_step.Text,
                Watt = this.power_step.Text,
                SafecaseData = sc,
                ClearcaseList = ccList,
                Log_write = "Ti=" + this.log_write1_step.Text + "/Vo=" + this.log_write2_step.Text + "/Cu=" + this.log_write3_step.Text + "/Te=" + this.log_write4_step.Text,
                Pattern_id = this.pattern_id_step.Text,
                Pattern_name = this.pattern_name_step.Text,
                Pattern_Addr = this.pattern_fullAddr_step.Text,
                Can_cond = this.can_cond_step,
                Can_send = this.can_send_step,
                Time_method = this.time_method_step.Text,
                Output_mode = this.output_mode_step.Text,
                Time = this.time_step.Text,
                Output = this.output_step.Text,
                CURR_LIMIT_CHARGE = this.curr_limit_charge_step.Text,
                CURR_LIMIT_DISCHARGE = this.curr_limit_discharge_step.Text,
                Chamber_cond = this.chamber_cond_step,
                CHILLER_COND = this.chiller_temp_step.Text ==
                "-100" ? "-100/0/0" : this.chiller_temp_step.Text + "/" + this.chiller_oil_step.Text + "/" + this.chiller_press_step.Text,// chiller 온도가 -100이면 사용안함
                Relay = useRelay.IsSelected == true ? this.relay_step.Content.ToString() : "0/0/0/0/0/0/0/0/0/0/0/0",
                Loader = useLoader.IsSelected == true ? (loader_mode_step.SelectedItem as ComboBoxItem).Content.ToString() + "/" + loader_value_step.Text + "/1" : "NO/0/0" // this.loader_step.Text,//사용하면 가져와서 데이터만들기
            };


            switch (typeString)
            {
                case "Pattern":
                    step.Step_mode = step.Voltage = step.Discharge_voltage = step.Current = step.Watt = null;
                    break;
                case "Rest":
                    step.Step_mode = step.Voltage = step.Discharge_voltage = step.Current = step.Watt = null;
                    step.Pattern_Addr = step.Pattern_id = step.Pattern_name = step.Time_method = step.Time = step.Output = step.Output_mode = step.CURR_LIMIT_CHARGE = step.CURR_LIMIT_DISCHARGE = "";
                    break;
                case "Discharge":
                    step.Voltage = null;
                    step.Pattern_Addr = step.Pattern_id = step.Pattern_name = step.Time_method = step.Time = step.Output = step.Output_mode = step.CURR_LIMIT_CHARGE = step.CURR_LIMIT_DISCHARGE = "";
                    break;
                case "Charge":
                    step.Discharge_voltage = null;
                    step.Pattern_Addr = step.Pattern_id = step.Pattern_name = step.Time_method = step.Time = step.Output = step.Output_mode = step.CURR_LIMIT_CHARGE = step.CURR_LIMIT_DISCHARGE = "";
                    break;
                default: break;
            }

            targetSteps.Clear();
            targetSteps.Add(step);

            switch (modeString)
            {
                case "CC":
                    //step.Voltage = 
                    step.Watt = null;
                    break;
                case "CC/CV":
                    step.Watt = null;
                    break;
                case "CP":
                    //step.Voltage = 
                    step.Current = null;
                    break;
                default: break;
            }

            var cycle = new Cycle();

            cycle.SafeCase.VoMax = (this.VoMax_cycle.Text);
            cycle.SafeCase.VoMin = (this.VoMin_cycle.Text);
            cycle.SafeCase.CuMax = (this.CuMax_cycle.Text);
            cycle.SafeCase.CuMin = (this.CuMin_cycle.Text);
            cycle.SafeCase.CaMax = (this.CaMax_cycle.Text);
            cycle.SafeCase.PoMax = (this.PoMax_cycle.Text);
            cycle.SafeCase.WHMax = (this.WHMax_cycle.Text);
            cycle.SafeCase.Exceed = (this.Exceed_cycle.Text);
            cycle.SafeCase.CeVoMaxH = (this.CeVoMaxH_cycle.Text);
            cycle.SafeCase.CeVoMaxL = (this.CeVoMaxL_cycle.Text);
            cycle.SafeCase.CeVoMinH = (this.CeVoMinH_cycle.Text);
            cycle.SafeCase.CeVoMinL = (this.CeVoMinL_cycle.Text);
            cycle.SafeCase.CeDeVoMax = (this.CeDeVoMax_cycle.Text);
            cycle.SafeCase.CeVoSuMax = (this.CeVoSuMax_cycle.Text);
            cycle.SafeCase.CeVoSuMin = (this.CeVoSuMin_cycle.Text);
            cycle.SafeCase.CeVoAvMax = (this.CeVoAvMax_cycle.Text);
            cycle.SafeCase.CeVoAvMin = (this.CeVoAvMin_cycle.Text);
            cycle.SafeCase.CeTeMaxH = (this.CeTeMaxH_cycle.Text);
            cycle.SafeCase.CeTeMaxL = (this.CeTeMaxL_cycle.Text);
            cycle.SafeCase.CeTeMinH = (this.CeTeMinH_cycle.Text);
            cycle.SafeCase.CeTeMinL = (this.CeTeMinL_cycle.Text);
            cycle.SafeCase.CeDeTeMax = (this.CeDeTeMax_cycle.Text);
            cycle.SafeCase.CeTeSuMax = (this.CeTeSuMax_cycle.Text);
            cycle.SafeCase.CeTeSuMin = (this.CeTeSuMin_cycle.Text);
            cycle.SafeCase.CeTeAvMax = (this.CeTeAvMax_cycle.Text);
            cycle.SafeCase.CeTeAvMin = (this.CeTeAvMin_cycle.Text);
            cycle.SafeCase.HVIL = (this.HVIL_cycle.Text);
            cycle.SafeCase.COV = (this.COV_cycle.Text);
            cycle.Log_write_cond = "Ti=" + this.log_write1_cycle.Text + "/Vo=" + this.log_write2_cycle.Text + "/Cu=" + this.log_write3_step.Text + "/Te=" + this.log_write4_step.Text; //
            cycle.Loop_count = int.Parse(this.loop_count_cycle.Text) < 1 ? "1" : this.loop_count_cycle.Text;
            cycle.Goto_cycle = this.goto_cycle.Text;
            cycle.Goto_loop_count = this.goto_loop_count_cycle.Text;
            cycle.Goto_next_cycle = this.goto_next_cycle.Text;
            cycle.Cycle_end_time = this.cycle_end_time_cycle.Text;
            cycle.Cycle_ah = this.cycle_ah_cycle.Text;
            cycle.Can_cond = this.can_cond_cycle;

            targetSteps.Add(cycle);
        }

        void NotCheckedValueFilter()
        {
            foreach (var item in this.clearCaseSP.Children)
            {
                var item1 = (ClearcaseUC)item;
                //폴스면 값을 비워줌
                //선택안된거에 대한 값을 날리는것
                ////선택됬는데 0.000이면 체크를 풀자
                //if (item1.cb.IsChecked == true)
                //{
                //    if (item1.valueTb.Text == "0.000" || item1.valueTb.Text == "00:00:00:00")
                //        item1.cb.IsChecked = false;
                //}

                if (item1.cb.IsChecked == false)
                {
                    switch (item1.titleLb.Content.ToString())
                    {
                        case "Voltage HIGH": item1.valueTb.Text = "0.000"; break;
                        case "Voltage LOW": item1.valueTb.Text = "0.000"; break;
                        case "Current": item1.valueTb.Text = "0.000"; break;
                        case "Time": item1.valueTb.Text = "00:00:00:00"; break;
                        default: break;
                    }
                }
            }
        }

        /// <summary>
        /// 완료기능이 같이 들어가있다, 분할필요성있음
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextClick(object sender, RoutedEventArgs e)
        {
            this.CreateTab.SelectedIndex = this.CreateTab.SelectedIndex + 1;
        }


        private void PrevClick(object sender, RoutedEventArgs e)
        {
            if (this.CreateTab.SelectedIndex > 0)
                this.CreateTab.SelectedIndex = this.CreateTab.SelectedIndex - 1;

        }

        private void ApplyClick(object sender, RoutedEventArgs e)
        {
            NotCheckedValueFilter();
            MakeNFinish();

            if ((targetSteps[0] as Step).Step_mode == "CC/CV")
            {
                if ((targetSteps[0] as Step).ClearcaseList[2].Value == "0.000")
                {
                    MessageBox.Show("CC/CV Modes Require Current Limit", "Warning");
                    return;
                }
            }

            //저쪽 리스트에 cycle을 넘긴다.
            if (targetSteps.Count > 0)
            {
                if (isModify)
                {
                    mw.ModifyList(targetSteps);
                }
                else
                {
                    mw.SetList(targetSteps);
                }
                mw.isNeedSave = true;//적용을 누르니 저장해야되는상태
            }
            targetSteps = null;
            this.Close();
        }

        #endregion

        #region Methods

        private void InitializeToCycle(Cycle cycle)
        {
            this.VoMax_cycle.Text = cycle.SafeCase.VoMax.ToString();
            this.VoMin_cycle.Text = cycle.SafeCase.VoMin.ToString();
            this.CuMax_cycle.Text = cycle.SafeCase.CuMax.ToString();
            this.CuMin_cycle.Text = cycle.SafeCase.CuMin.ToString();
            this.CaMax_cycle.Text = cycle.SafeCase.CaMax.ToString();
            this.PoMax_cycle.Text = cycle.SafeCase.PoMax.ToString();
            this.WHMax_cycle.Text = cycle.SafeCase.WHMax.ToString();
            this.Exceed_cycle.Text = cycle.SafeCase.Exceed.ToString();
            this.CeVoMaxH_cycle.Text = cycle.SafeCase.CeVoMaxH.ToString();
            this.CeVoMaxL_cycle.Text = cycle.SafeCase.CeVoMaxL.ToString();
            this.CeVoMinH_cycle.Text = cycle.SafeCase.CeVoMinH.ToString();
            this.CeVoMinL_cycle.Text = cycle.SafeCase.CeVoMinL.ToString();
            this.CeDeVoMax_cycle.Text = cycle.SafeCase.CeDeVoMax.ToString();
            this.CeVoSuMax_cycle.Text = cycle.SafeCase.CeVoSuMax.ToString();
            this.CeVoSuMin_cycle.Text = cycle.SafeCase.CeVoSuMin.ToString();
            this.CeVoAvMax_cycle.Text = cycle.SafeCase.CeVoAvMax.ToString();
            this.CeVoAvMin_cycle.Text = cycle.SafeCase.CeVoAvMin.ToString();
            this.CeTeMaxH_cycle.Text = cycle.SafeCase.CeTeMaxH.ToString();
            this.CeTeMaxL_cycle.Text = cycle.SafeCase.CeTeMaxL.ToString();
            this.CeTeMinH_cycle.Text = cycle.SafeCase.CeTeMinH.ToString();
            this.CeTeMinL_cycle.Text = cycle.SafeCase.CeTeMinL.ToString();
            this.CeDeTeMax_cycle.Text = cycle.SafeCase.CeDeTeMax.ToString();
            this.CeTeSuMax_cycle.Text = cycle.SafeCase.CeTeSuMax.ToString();
            this.CeTeSuMin_cycle.Text = cycle.SafeCase.CeTeSuMin.ToString();
            this.CeTeAvMax_cycle.Text = cycle.SafeCase.CeTeAvMax.ToString();
            this.CeTeAvMin_cycle.Text = cycle.SafeCase.CeTeAvMin.ToString();
            this.HVIL_cycle.Text = cycle.SafeCase.HVIL.ToString();
            this.COV_cycle.Text = cycle.SafeCase.COV.ToString();

            char[] ch = { '/', '=' };
            var tmp = cycle.Log_write_cond.Split(ch);

            if (tmp.Length != 1)
            {
                this.log_write1_cycle.Text = tmp[1];
                this.log_write2_cycle.Text = tmp[3];
                this.log_write3_cycle.Text = tmp[5];
                this.log_write4_cycle.Text = tmp[7];
            }

            //this.log_write_cond_cycle.Text = cycle.Log_write_cond;
            this.loop_count_cycle.Text = int.Parse(cycle.Loop_count) < 1 ? "1" : cycle.Loop_count;
            this.goto_cycle.Text = cycle.Goto_cycle;
            this.goto_loop_count_cycle.Text = cycle.Goto_loop_count;
            this.goto_next_cycle.Text = cycle.Goto_next_cycle;
            this.cycle_end_time_cycle.Text = cycle.Cycle_end_time;
            this.cycle_ah_cycle.Text = cycle.Cycle_ah;
            this.can_cond_cycle = cycle.Can_cond;

            localCanCondCycle_Lb.Content = can_cond_cycle;

        }

        //모두 추가부분
        int totalCycleCount = 0;
        int localstepCount = 0;

        private void InitializeToStep(Step step)
        {
            //type설정
            foreach (var item in typeSP.Children)
            {
                var radio = item as RadioButton;
                if (radio.Content.ToString() == step.Step_type)
                {
                    typeString = radio.Content.ToString();
                    radio.IsChecked = true;
                    break;
                }
            }

            //Mode설정
            foreach (var item in modeSP.Children)
            {
                var radio = item as RadioButton;
                if (radio.Content.ToString() == step.Step_mode)
                {
                    modeString = radio.Content.ToString();
                    radio.IsChecked = true;
                    break;
                }
            }
            this.voltage_step.Text = step.Voltage == "" ? ".000" : step.Voltage;
            this.disvoltage_step.Text = step.Discharge_voltage == "" ? ".000" : step.Discharge_voltage;
            this.current_step.Text = step.Current == "" ? ".000" : step.Current;
            this.power_step.Text = step.Watt == "" ? ".000" : step.Watt;

            if (step.ClearcaseList == null) return;

            foreach (var cc in step.ClearcaseList)
            {
                foreach (var item in this.clearCaseSP.Children)
                {
                    var item1 = (ClearcaseUC)item;
                    if (item1.titleLb.Content.ToString() == cc.TitleValue && cc.Value.ToString() != "0.000" && cc.Value.ToString() != "0"
                        && cc.Value.ToString() != "100.0" && cc.Value.ToString() != "-100.0" && cc.Value.ToString() != "00:00:00" && cc.Value.ToString() != "00:00:00:00"
                        )
                    {
                        if (cc.TitleValue == "Time" || cc.TitleValue == "Real Time")
                        {
                            var ar = cc.Value.Split(':');
                            if (ar.Length == 3)
                            {//재설정
                                ar = "00:00:00:00".Split(':');
                            }

                            if (ar.Length > 1)
                            {
                                item1.timePicker.DisplayDays = ar[0];
                                item1.timePicker.DisplayTimeHours = ar[1];
                                item1.timePicker.DisplayTimeMinutes = ar[2];
                                item1.timePicker.DisplayTimeSeconds = ar[3];
                            }
                        }

                        item1.cb.IsChecked = true;
                        item1.valueTb.Text = cc.Value;
                        var ct = 0;
                        foreach (var p in item1.Combobox.Items)
                        {
                            if (p.ToString() == cc.NextStep)
                            {
                                item1.Combobox.SelectedIndex = ct;
                                break;
                            }
                            ct += 1;
                        }
                    }
                }
            }

            this.VoMax_step.Text = step.SafecaseData.VoMax.ToString();
            this.VoMin_step.Text = step.SafecaseData.VoMin.ToString();
            this.CuMax_step.Text = step.SafecaseData.CuMax.ToString();
            this.CuMin_step.Text = step.SafecaseData.CuMin.ToString();
            this.CaMax_step.Text = step.SafecaseData.CaMax.ToString();
            this.PoMax_step.Text = step.SafecaseData.PoMax.ToString();
            this.WHMax_step.Text = step.SafecaseData.WHMax.ToString();
            this.Exceed_step.Text = step.SafecaseData.Exceed.ToString();
            this.CeVoMaxH_step.Text = step.SafecaseData.CeVoMaxH.ToString();
            this.CeVoMaxL_step.Text = step.SafecaseData.CeVoMaxL.ToString();
            this.CeVoMinH_step.Text = step.SafecaseData.CeVoMinH.ToString();
            this.CeVoMinL_step.Text = step.SafecaseData.CeVoMinL.ToString();
            this.CeDeVoMax_step.Text = step.SafecaseData.CeDeVoMax.ToString();
            this.CeVoSuMax_step.Text = step.SafecaseData.CeVoSuMax.ToString();
            this.CeVoSuMin_step.Text = step.SafecaseData.CeVoSuMin.ToString();
            this.CeVoAvMax_step.Text = step.SafecaseData.CeVoAvMax.ToString();
            this.CeVoAvMin_step.Text = step.SafecaseData.CeVoAvMin.ToString();
            this.CeTeMaxH_step.Text = step.SafecaseData.CeTeMaxH.ToString();
            this.CeTeMaxL_step.Text = step.SafecaseData.CeTeMaxL.ToString();
            this.CeTeMinH_step.Text = step.SafecaseData.CeTeMinH.ToString();
            this.CeTeMinL_step.Text = step.SafecaseData.CeTeMinL.ToString();
            this.CeDeTeMax_step.Text = step.SafecaseData.CeDeTeMax.ToString();
            this.CeTeSuMax_step.Text = step.SafecaseData.CeTeSuMax.ToString();
            this.CeTeSuMin_step.Text = step.SafecaseData.CeTeSuMin.ToString();
            this.CeTeAvMax_step.Text = step.SafecaseData.CeTeAvMax.ToString();
            this.CeTeAvMin_step.Text = step.SafecaseData.CeTeAvMin.ToString();
            this.HVIL_step.Text = step.SafecaseData.HVIL.ToString();
            this.COV_step.Text = step.SafecaseData.COV.ToString();

            //"Ti="+this.log_write1_step.Text+"/Vo="+this.log_write2_step.Text+"/Cu="+this.log_write3_step.Text+"/Te"+this.log_write4_step.Text

            char[] ch = { '/', '=' };
            var tmp = step.Log_write.Split(ch);


            this.log_write1_step.Text = tmp[1];
            this.log_write2_step.Text = tmp[3];
            this.log_write3_step.Text = tmp[5];

            if (tmp.Length == 8)
                this.log_write4_step.Text = tmp[7];

            this.pattern_id_step.Text = step.Pattern_id;
            this.pattern_name_step.Text = step.Pattern_name;
            this.can_cond_step = step.Can_cond;
            this.can_send_step = step.Can_send;
            this.pattern_fullAddr_step.Text = this.routeTb.Text = step.Pattern_Addr == "" ? step.Pattern_name : step.Pattern_Addr;//패턴 주소가 없으면(기존경우) 이름을 주소로 써라

            localCanCond_Lb.Content = can_cond_step;

            this.time_method_step.Text = step.Time_method;
            this.output_mode_step.Text = step.Output_mode;
            this.time_step.Text = step.Time;
            this.output_step.Text = step.Output;
            this.curr_limit_charge_step.Text = step.CURR_LIMIT_CHARGE;
            this.curr_limit_discharge_step.Text = step.CURR_LIMIT_DISCHARGE;
            this.chamber_cond_step = step.Chamber_cond;

            var arr = step.CHILLER_COND.Split('/');
            if (arr.Length > 1)
            {
                this.chiller_temp_step.Text = arr[0];
                this.chiller_oil_step.Text = arr[1];
                this.chiller_press_step.Text = arr[2];
            }
            localChamberCond_Lb.Content = chamber_cond_step;
            if (step.Relay.Contains('1'))
            {
                this.relay_use_step.SelectedIndex = 1;
                this.relay_step.Content = step.Relay;
            }
            //Loader =  useLoader.IsSelected == true ? loader_mode_step.SelectedItem.ToString()+"/"+loader_value_step.Text+"/1":"NO/0/0" // this.loader_step.Text,//사용하면 가져와서 데이터만들기
            //NO/0/0
            arr = step.Loader.Split('/');
            if (arr.Length != 1)
            {
                var str = arr[0];
                int cnt = 0;
                foreach (var item in this.loader_mode_step.Items)
                {
                    if ((item as ComboBoxItem).Content.ToString() == str)
                    {
                        this.loader_mode_step.SelectedIndex = cnt;
                        break;
                    }
                    cnt++;
                }
                loader_value_step.Text = arr[1];
                loader_use_step.SelectedIndex = arr[2] == "0" ? 0 : 1;
            }

            //항목/기준사이클스텝/설정비율/스텝이동
            //this.ratio_cond_step.Text = step.RATIO_COND;

            //cycle,step 모든항목 추가 이후 선택된걸 찾아야함
            ratio_cond4_step.Items.Add("Next");
            ratio_cond4_step.Items.Add("Pause");
            ratio_cond4_step.Items.Add("Completion");
            ratio_cond4_step.SelectedIndex = 0;


            if (mw.totalProcessList.Count != 0)
            {
                totalCycleCount = mw.totalProcessList[mw.index_process].ScheduleList[mw.index_schedule].CycleList.Count;

                for (int i = 0; i < totalCycleCount; i++)//사이클 총 갯수
                {
                    //해당사이클의 스텝을 전부 추가 
                    var cycle = mw.totalProcessList[mw.index_process].ScheduleList[mw.index_schedule].CycleList[i];
                    for (int k = 0; k < cycle.StepList.Count; k++)
                    {
                        var item = "C" + (i + 1).ToString() + "S" + (k + 1).ToString();
                        if ("C" + cycle_cycle.Text + "S" + step_cycle.Text == item)
                            continue;

                        //0824 요청 
                        //용량기준 스텝종료조건
                        //-. 이전 사이클이 Rest 등의 용량계산이 안되는 스텝일경우, 미선택되도록 기능구현 필요.
                        if (cycle.StepList[k].Step_type != "Rest")
                        {
                            ratio_cond3_step.Items.Add(item);
                        }
                        ratio_cond4_step.Items.Add(item);
                    }

                }
            }

            tmp = step.RATIO_COND.Split('/');
            if (tmp.Length != 1)
            {
                this.ratio_cond1_step.SelectedIndex = int.Parse(tmp[0]); // 사용여부

                //사용안함이 아니라면
                if (this.ratio_cond1_step.SelectedIndex > 0)
                {
                    var ra = tmp[1].Split(':');// 스탭이동
                    var str = "C" + ra[0] + "S" + ra[1];

                    foreach (var item in this.ratio_cond3_step.Items)
                    {
                        if (item.ToString() == str)
                        {
                            this.ratio_cond3_step.SelectedItem = str;
                            break;
                        }
                    }

                    this.ratio_cond2_step.Text = tmp[2];//비율

                    switch (tmp[3])
                    {
                        case "0:65533": this.ratio_cond4_step.SelectedIndex = 0; break;
                        case "0:65534": this.ratio_cond4_step.SelectedIndex = 1; break;
                        case "0:65535": this.ratio_cond4_step.SelectedIndex = 2; break;
                        default:
                            {
                                ra = tmp[3].Split(':');// 스탭이동
                                str = "C" + ra[0] + "S" + ra[1];

                                foreach (var item in this.ratio_cond4_step.Items)
                                {
                                    if (item.ToString() == str)
                                    {
                                        this.ratio_cond4_step.SelectedItem = str;
                                        break;
                                    }
                                }
                            };
                            break;
                    }

                }
            }

            if (step.Step_type == "Pattern")
            {
                string temp1 = string.Empty, temp2 = string.Empty, temp3 = string.Empty;
                if (step.CURR_LIMIT_CHARGE == "0" || step.CURR_LIMIT_CHARGE == "")
                {
                    var tarr = step.Time.Split('/');
                    var oarr = step.Output.Split('/');
                    for (int i = 0; i < tarr.Length; i++)
                    {
                        this.timelist.Add(tarr[i]);
                        this.valuelist1.Add(oarr[i]);
                    }
                }
                else
                {
                    this.isVoltage = true;
                    var tarr = step.Time.Split('/');
                    var oarr = step.Output.Split('/');
                    var varr = step.CURR_LIMIT_CHARGE.Split('/');
                    var varr1 = step.CURR_LIMIT_DISCHARGE.Split('/');
                    for (int i = 0; i < tarr.Length; i++)
                    {
                        this.timelist.Add(tarr[i]);
                        this.valuelist1.Add(oarr[i]);
                        this.valuelist2_1.Add(varr[i]);
                        this.valuelist2_2.Add(varr1[i]);
                    }
                }

                innerTb.Text = "Parsed Data\nTime : " + step.Time + "\nOutput : " + step.Output;

                if (isVoltage)
                    innerTb.Text += "\nLimit_Charge : " + step.CURR_LIMIT_CHARGE + "\nLimit_Discharge : " + step.CURR_LIMIT_DISCHARGE;

            }

        }

        private void InitializeClearCase()
        {
            List<ClearCase> initlist = new List<ClearCase>();
            initlist.Add(new ClearCase() { TitleValue = "Voltage HIGH", TypeValue = "V" });
            initlist.Add(new ClearCase() { TitleValue = "Voltage LOW", TypeValue = "V" });
            initlist.Add(new ClearCase() { TitleValue = "Current", TypeValue = "A" });
            initlist.Add(new ClearCase() { TitleValue = "Time", TypeValue = "s", Value = "00:00:00:00" });
            
            this.clearCaseSP.Children.Clear();
            this.afterLoopCb.Items.Clear();
            this.afterLoopCb.Items.Add("Next Step");

            //뭘 선택할지 몰라 스탭많은걸 추가
            var maxCount = -1;

            for (int i = 0; i < mw.CycleListBox.Items.Count; i++)
            {
                if (maxCount < (mw.CycleListBox.Items[i] as Cycle).StepList.Count)
                {
                    maxCount = (mw.CycleListBox.Items[i] as Cycle).StepList.Count;
                }
            }

            for (int i = 1; i <= maxCount; i++)
            {
                this.afterLoopCb.Items.Add("Step " + i);
            }

            this.afterLoopCb.SelectedIndex = 0;
            foreach (var item in initlist)
            {
                ClearcaseUC uc = new ClearcaseUC();
                if (item.TitleValue == "Real Time")
                {
                    uc.valueTb.Visibility = Visibility.Collapsed;
                    uc.timePicker.Visibility = Visibility.Visible;
                    uc.typeLb.Content = "";
                    uc.timePicker.useAfterDay.Visibility = Visibility.Visible;

                    var arr = item.Value.Split(':');
                    if (arr.Length > 1)
                    {
                        uc.timePicker.DisplayTimeHours = arr[0];
                        uc.timePicker.DisplayTimeMinutes = arr[1];
                        uc.timePicker.DisplayTimeSeconds = arr[2];
                    }
                }
                else if (item.TitleValue == "Time")
                {
                    uc.valueTb.Visibility = Visibility.Collapsed;
                    uc.timePicker.Visibility = Visibility.Visible;
                    uc.typeLb.Content = "";
                    uc.timePicker.useThisDay.Visibility = Visibility.Visible;

                    var arr = item.Value.Split(':');
                    if (arr.Length > 1)
                    {
                        uc.timePicker.DisplayDays = arr[0];
                        uc.timePicker.DisplayTimeHours = arr[1];
                        uc.timePicker.DisplayTimeMinutes = arr[2];
                        uc.timePicker.DisplayTimeSeconds = arr[3];
                    }
                }
                else
                {
                    uc.typeLb.Content = item.TypeValue;
                    uc.valueTb.Text = item.Value;
                }

                uc.titleLb.Content = item.TitleValue;

                uc.Combobox.Items.Add("Next Step"); //스텝추가 말고
                if (mw.totalProcessList.Count == 0)
                    return;

                totalCycleCount = mw.totalProcessList[mw.index_process].ScheduleList[mw.index_schedule].CycleList.Count;

                for (int i = 0; i < totalCycleCount; i++)//사이클 총 갯수
                {
                    //해당사이클의 스텝을 전부 추가 
                    var cycle = mw.totalProcessList[mw.index_process].ScheduleList[mw.index_schedule].CycleList[i];
                    for (int k = 0; k < cycle.StepList.Count; k++)
                    {
                        var cs = "C" + (i + 1).ToString() + "S" + (k + 1).ToString();
                        if ("C" + cycle_cycle.Text + "S" + step_cycle.Text == cs)
                            continue;

                        uc.Combobox.Items.Add(cs);
                    }

                }

                //uc.Combobox.Items.Add("Next Step"); //스텝추가 말고

                //for (int i = 1; i <= maxCount; i++)
                //{
                //    uc.Combobox.Items.Add("Step " + i);
                //}
                uc.Combobox.SelectedIndex = 0;
                this.clearCaseSP.Children.Add(uc);
            }
        }

        /// <summary>
        /// 복붙메서드, 반복구조에 개선이 필요하다
        /// </summary>
        public void CopyPaste()//넥스트클릭의 모든과정이 하나에 합쳐짐
        {
            MakeNFinish();
            mw.SetList(targetSteps);
            targetSteps = null;
        }

        #endregion
        List<string> timelist = new List<string>();
        List<string> valuelist1 = new List<string>();
        List<string> valuelist2_1 = new List<string>();
        List<string> valuelist2_2 = new List<string>();
        bool isVoltage = false;
        private void patternLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "csv files (*.csv)|*.csv|All Files(*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                routeTb.Text = openFileDialog.FileName;
                pattern_fullAddr_step.Text = routeTb.Text;
                isVoltage = false;
                try
                {
                    StringBuilder sb = new StringBuilder();
                    string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension(openFileDialog.FileName, ".csv"), Encoding.Default);
                    foreach (string line in lines)
                    {
                        sb.AppendLine(line);
                    }

                    Pattern localPattern = new Pattern();

                    localPattern.List.Clear();

                    this.pattern_name_step.Text = lines[8].Split(',')[0].ToString();

                    this.time_method_step.Text = lines[12].Split(',')[0].ToString();
                    this.output_mode_step.Text = lines[12].Split(',')[1].ToString();


                    string temp1 = string.Empty;
                    string temp2 = string.Empty;
                    string temp3 = string.Empty;
                    string temp4 = string.Empty;

                    if (this.output_mode_step.Text == "2")
                        isVoltage = true;

                    for (int i = 14; i < lines.Length; i++)
                    {
                        var arr = lines[i].Split(',');
                        temp1 += arr[0] + "/";
                        temp2 += arr[1] + "/";

                        timelist.Add(arr[0]);
                        valuelist1.Add(arr[1]);

                        if (isVoltage)
                        {
                            temp3 += arr[2] + "/";
                            temp4 += arr[3] + "/";
                            valuelist2_1.Add(arr[2]);
                            valuelist2_2.Add(arr[3]);
                        }
                    }
                    this.time_step.Text = temp1.Remove(temp1.Length - 1, 1);
                    this.output_step.Text = temp2.Remove(temp2.Length - 1, 1);

                    if (isVoltage)
                    {
                        this.curr_limit_charge_step.Text = temp3.Remove(temp3.Length - 1, 1);
                        this.curr_limit_discharge_step.Text = temp4.Remove(temp4.Length - 1, 1);
                    }
                    else
                    {
                        this.curr_limit_charge_step.Text = "";
                        this.curr_limit_discharge_step.Text = "";
                    }


                    innerTb.Text = "Parsed Data\nTime : " + temp1 + "\nOutput : " + temp2;

                    if (isVoltage)
                        innerTb.Text += "\nLimit_Charge : " + temp3 + "\nLimit_Discharge : " + temp4;

                }
                catch (IOException ex)
                {
                    //mw.stateLb.Content = ex.Message.ToString();
                }
            }
        }
        
        private void CloseToolWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void VisibleBorder(object sender, RoutedEventArgs e)
        {
            //hiddenBorder.Visibility = Visibility.Collapsed;
        }

        

        public string can_send_step = "";
        public string can_cond_step = "";

        

        public string chamber_cond_step = "";

        

        public string can_cond_cycle = "";
                
        private void discharged(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.clearCaseSP.Children)
            {
                var cc = item as ClearcaseUC;
                if (cc.titleLb.Content.ToString() == "Voltage HIGH" ||
                    cc.titleLb.Content.ToString().Contains("상한"))
                {
                    cc.IsEnabled = false;
                }
                else
                {
                    cc.IsEnabled = true;
                }
            }
        }

        private void charged(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.clearCaseSP.Children)
            {
                var cc = item as ClearcaseUC;
                if (cc.titleLb.Content.ToString() == "Voltage LOW" ||
                    cc.titleLb.Content.ToString().Contains("하한"))
                {
                    cc.IsEnabled = false;
                }
                else
                {
                    cc.IsEnabled = true;
                }
            }
        }

        private void EnableType(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.clearCaseSP.Children)
            {
                var cc = item as ClearcaseUC;
                cc.IsEnabled = true;

            }
        }

    }

    [Serializable]
    public class Pattern
    {
        Dictionary<double, string> list = new Dictionary<double, string>();
        string mode = "CC";

        public string Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        public Dictionary<double, string> List
        {
            get { return list; }
            set { list = value; }
        }
    }
}
