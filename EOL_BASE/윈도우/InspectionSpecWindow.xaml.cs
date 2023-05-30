using EOL_BASE.클래스;
using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace EOL_BASE.윈도우
{
    /// <summary>
    /// InspectionSpecWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InspectionSpecWindow : Window
    {
        MainWindow mw;
        public InspectionSpecWindow()
        {
            InitializeComponent();

        }
        public InspectionSpecWindow(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;
            this.Loaded += InspectionSpecWindow_Loaded;
        }

        void InspectionSpecWindow_Loaded(object sender, RoutedEventArgs e)
        {

            ModelTb.SelectionChanged += ModelTb_SelectionChanged;

            this.testSpecDg.ItemsSource = mw.modelList[mw.selectedIndex].TestItemList;

            //55.Inspection Window 로딩 시 설비 ID 를 불러오는 구문이 없어 추가
            this.prodidTb.Text = mw.modelList[mw.selectedIndex].ProdId;



            this.procidTb.Text = mw.modelList[mw.selectedIndex].ProcId;
            this.useridTb.Text = mw.modelList[mw.selectedIndex].UserId;
            this.equipTb.Text = mw.modelList[mw.selectedIndex].EquipId;

            this.irVoltLevelTb.Text = mw.INSULATION_RESISTANCE_VOLT.ToString();
            this.irVoltTimeTb.Text = mw.INSULATION_RESISTANCE_TIME.ToString();
            this.irVoltRampTimeTb.Text = mw.INSULATION_RESISTANCE_RAMP_TIME.ToString();
            this.irFetchCycleTimeTb.Text = mw.FETCH_CHECK_CYCLE_TIME.ToString();
            this.irrangeTb.Text = mw.INSULATION_RESISTANCE_RANGE;
            
            //200724 wjs add for withstand manual test
            this.wsVoltLeveTb.Text = mw.chroma.CON_WITHSTANDING_VOLTAGE_V.ToString();
            this.wsVoltTimeTb.Text = mw.chroma.CON_WITHSTANDING_VOLTAGE_T.ToString();
            this.wsArcLimitTb.Text = mw.chroma.CON_ARC_LIMIT.ToString();
            this.wsArcDetecTb.Text = mw.chroma.CON_ARC_ENABLE.ToString();
            this.wsRampTimeTb.Text = mw.chroma.CON_WITHSTANDING_RAMP_UP_TIME.ToString();
            this.wsFallTimeTb.Text = mw.chroma.CON_WITHSTANDING_FALLDOWN_TIME.ToString();


            this.cellfomu1Tb.Text = mw.cellFomula1.ToString();
            this.cellfomu2Tb.Text = mw.cellFomula2.ToString();
            this.modulefomu1Tb.Text = mw.moduleFomula1.ToString();
            this.modulefomu2Tb.Text = mw.moduleFomula2.ToString();

            this.coefficientTTb.Text = mw.coefficientT.ToString();
            this.coefficientVTb.Text = mw.coefficientV.ToString();

            this.cyclerCountTb.Text = mw.counter_Cycler.ToString();
            this.cyclerCountLimitTb.Text = mw.counter_Cycler_limit.ToString();

            this.lineOpenUpper.Text = mw.LINE_OPEN_CHECK_UPPER_LIMIT.ToString();
            this.lineOpenLower.Text = mw.LINE_OPEN_CHECK_LOWER_LIMIT.ToString();

            this.lineCheck.Text = mw.connTargetOhm.ToString();
                 

            //this.lowVolt.Text = mw.power1LowVolt.ToString();
            //this.highVolt.Text = mw.power1HighVolt.ToString();
            //this.NormVolt.Text = mw.power1NormalVolt.ToString();


            //this.Timeout.Text = mw.chroma19053.CONNECTION__COMMUNICATION_TIMEOUT_SEC.ToString();
            //this.HipotTime.Text = mw.chroma19053.CON_WITHSTANDING_VOLTAGE_T.ToString();
            //this.HipotVolt.Text = mw.chroma19053.CON_WITHSTANDING_VOLTAGE_V.ToString();

            //this.NormHz.Text = mw.normalHz.ToString();
            //this.CrashHz.Text = mw.crashHz.ToString();


            //this.intTerm.Text = mw.IRInternalWait.ToString();
            //this.extTerm.Text = mw.IRExternalWait.ToString();
            //this.modelspecDg.Items.Add(new Object[]{mw.modelList[mw.selectedIndex].ModelId, mw.modelList[mw.selectedIndex].ModelDesc});
        }

        void ModelTb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mw.RefreshToFlags();


            this.testSpecDg.ItemsSource = mw.modelList[mw.selectedIndex].TestItemList;

            this.prodidTb.Text = mw.modelList[mw.selectedIndex].ProdId;
            this.procidTb.Text = mw.modelList[mw.selectedIndex].ProcId;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            var str = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(str);

            try
            {
                mw.modelList[mw.selectedIndex].ProdId = mw.prodTb.Text = this.prodidTb.Text;
                mw.modelList[mw.selectedIndex].ProcId = mw.procID = mw.procTb.Text = this.procidTb.Text;
                mw.modelList[mw.selectedIndex].UserId = mw.userTb.Text = this.useridTb.Text;
                mw.modelList[mw.selectedIndex].EquipId = mw.equipID = mw.equipTb.Text= this.equipTb.Text;

                mw.INSULATION_RESISTANCE_VOLT = int.Parse(this.irVoltLevelTb.Text);
                mw.INSULATION_RESISTANCE_TIME = int.Parse(this.irVoltTimeTb.Text);
                mw.INSULATION_RESISTANCE_RAMP_TIME = int.Parse(this.irVoltRampTimeTb.Text);

                //200724 wjs add for withstand manual test
                mw.chroma.CON_WITHSTANDING_VOLTAGE_V = double.Parse(this.wsVoltLeveTb.Text);
                mw.chroma.CON_WITHSTANDING_VOLTAGE_T = double.Parse(this.wsVoltTimeTb.Text);
                mw.chroma.CON_ARC_LIMIT = double.Parse(this.wsArcLimitTb.Text);
                mw.chroma.CON_ARC_ENABLE = double.Parse(this.wsArcDetecTb.Text);
                mw.chroma.CON_WITHSTANDING_RAMP_UP_TIME = double.Parse(this.wsRampTimeTb.Text);
                mw.chroma.CON_WITHSTANDING_FALLDOWN_TIME = double.Parse(this.wsFallTimeTb.Text);
                
                // 20191023 Noah Choi Inspection Spec에서 변경된 값이 남아 있을 수 있도록 코드 수정
                mw.FETCH_CHECK_CYCLE_TIME = int.Parse(this.irFetchCycleTimeTb.Text);
                mw.INSULATION_RESISTANCE_RANGE = this.irrangeTb.Text;
                mw.SetCounter_Fetch_Cycle_Time(); 

                mw.cellFomula1 = double.Parse(this.cellfomu1Tb.Text);
                mw.cellFomula2 = double.Parse(this.cellfomu2Tb.Text);

                mw.moduleFomula1 = double.Parse(this.modulefomu1Tb.Text);
                mw.moduleFomula2 = double.Parse(this.modulefomu2Tb.Text);

                mw.coefficientT = double.Parse(this.coefficientTTb.Text);
                mw.coefficientV = double.Parse(this.coefficientVTb.Text);

                mw.counter_Cycler_limit = int.Parse(this.cyclerCountLimitTb.Text);
                mw.SetCounter_Cycle_Limit();

                mw.LINE_OPEN_CHECK_UPPER_LIMIT = int.Parse(this.lineOpenUpper.Text);
                mw.LINE_OPEN_CHECK_LOWER_LIMIT = int.Parse(this.lineOpenLower.Text);
                

                if (mw.localTypes == 0)
                {
                    mw.prodID_4P8S = this.prodidTb.Text;
                }
                else if(mw.localTypes == 1)
                {
                    mw.prodID_4P8S_Rev = this.prodidTb.Text; 
                }
                else if (mw.localTypes == 2)
                {
                    mw.prodID_4P7S = this.prodidTb.Text;  
                }
                else if(mw.localTypes == 3)
                {
                    mw.prodID_3P8S = this.prodidTb.Text;   
                }
                else if (mw.localTypes == 4)
                {
                    mw.prodID_3P10S = this.prodidTb.Text;   
                }
                else if (mw.localTypes == 5)
                {
                    mw.prodID_3P10S_Rev = this.prodidTb.Text;   
                }

                mw.SetLocalData();

                mw.SetControlItemToCSV();

                //mw.modelList[mw.selectedIndex].ProdId = mw.prodTb.Text = this.prodidTb.Text;
                //mw.modelList[mw.selectedIndex].ProcId = mw.procTb.Text = this.procidTb.Text;
                //mw.prodID = this.prodidTb.Text;
                //mw.equipID = this.equipTb.Text;

                ////181217 set to regedit
                //mw.SetLocalData();

                //mw.counter_Cycler_limit = int.Parse(this.cyclerCountLimitTb.Text);
                //mw.SetCounter_Cycle_Limit();

                mw.connTargetOhm = this.lineCheck.Text;
            }
            catch (Exception ec)
            {
                MessageBox.Show("TextBox is Not Matched Type!!");
                return;
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void testSpecDg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var p = (KeyValuePair<string, TestItem>)this.testSpecDg.SelectedItems[0];
            stb.Text = p.Key;
            maxtb.Text = p.Value.Max.ToString();
            mintb.Text = p.Value.Min.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (stb.Text != "")
            {
                try
                {
                    mw.modelList[mw.selectedIndex].TestItemList[stb.Text].Max = double.Parse(maxtb.Text);
                    mw.modelList[mw.selectedIndex].TestItemList[stb.Text].Min = double.Parse(mintb.Text);

                    mw.SetCollectItemToCSV();

                    //// 2018.12.18 HJS - UI를 통한 스펙 수정 기능 (eolList.csv Write)
                    //// eollist.csv 쓰기 실패 시 MessageBox.Show 후 return
                    //if (Write_eolList_Spec(stb.Text, maxtb.Text, mintb.Text))
                    //{
                    //    mw.modelList[mw.selectedIndex].TestItemList[stb.Text].Max = double.Parse(maxtb.Text);
                    //    mw.modelList[mw.selectedIndex].TestItemList[stb.Text].Min = double.Parse(mintb.Text);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Apply Chenged Fail", "Info", MessageBoxButton.OK);
                    //    return;
                    //}
                }
                catch (Exception ec)
                {
                    mw.LogState(LogType.Fail, "Inspection Spec Change", ec);
                }
            }
        }

        public bool Write_eolList_Spec(string singleContent, string sMax, string sMin)
        {
            try
            {
                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");

                FileStream readWriteData = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "eollist.csv", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                StreamReader streamReader = new StreamReader(readWriteData, encode);

                string columnHeaders = string.Empty;
                var datas = new List<string[]>();

                int nCol_singleContent = 0;
                int nCol_minValue = 0;
                int nCol_maxValue = 0;

                // eollist.csv에서 columnHeader 구하는 부분
                columnHeaders = streamReader.ReadLine();
                var columnHeadersArray = columnHeaders.Split(',');

                for (int i = 0; i < columnHeadersArray.Length; i++)
                {
                    if (columnHeadersArray[i] == "singleContents")
                    {
                        nCol_singleContent = i;
                    }
                    else if (columnHeadersArray[i] == "MIN Value")
                    {
                        nCol_minValue = i;
                    }
                    else if (columnHeadersArray[i] == "MAX Value")
                    {
                        nCol_maxValue = i;
                    }
                }

                // eollist.csv에서 columnHeader를 제외한 내용을 Read 및 Parsing하는 부분
                int cnt = 0;
                while (streamReader.Peek() > -1)
                {
                    if (!(cnt == 0))
                    {
                        var content = streamReader.ReadLine();

                        datas.Add(content.Split(','));
                    }
                    cnt++;
                }

                // 선택된 singleContent에 해당하는 Row의 MIN, MAX 값을 Update 해주는 부분
                for (int index = 0; index < datas.Count; index++)
                {
                    if (datas[index][nCol_singleContent] == singleContent)
                    {
                        datas[index][nCol_minValue] = sMin;
                        datas[index][nCol_maxValue] = sMax;
                        break;
                    }
                }

                // eolList.csv에 모든 Data(Update된 Data 포함) OverWrite하는 부분
                readWriteData.Dispose();

                StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "eollist.csv", false);
                StringBuilder sb = new StringBuilder();

                sw.WriteLine(columnHeaders);
                for (int i = 0; i < datas.Count; i++)
                {
                    for (int j = 0; j < datas[i].Length; j++)
                    {
                        sb.Append(datas[i][j]);
                        sb.Append(",");
                    }

                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("\n");
                }
                sw.Write(sb.ToString());

                sb.Clear();
                streamReader.Dispose();
                sw.Dispose();

                Thread.Sleep(1000);

                return true;
            }

            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "Write Spec to eolist.csv", ec);

                return false;
            }
        }

        #region Versions
        private void cellfomu1Tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                ContinueWindow ct = new ContinueWindow(mw);
                ct.maintitle.Content = "EOL INSPECTION";
                ct.reason.Content = "ver. " + mw.lastUpdated;
                ct.contibt.Content = "OK";
                ct.shockLb.Content = "※";
                ct.Show();
            }
        }
        #endregion

        // 2018.11.01 jeonhj's comment
        // 외기 온도에 따른 기준 저항값 변경을 위해 추가 윈도우 생성
        private void btnAmbTempSet_Click(object sender, RoutedEventArgs e)
        {
            AmbientTemperatureSpecWindow atsw = new AmbientTemperatureSpecWindow();

            atsw.ShowDialog();
        }
    }
}
