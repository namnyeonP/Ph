using Renault_BT6.클래스;
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

namespace Renault_BT6.윈도우
{
    /// <summary>
    /// InspectionSpecWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InspectionSpecWindow_HiPot : Window
    {
        MainWindow mw;
        public InspectionSpecWindow_HiPot()
        {
            InitializeComponent();

        }
        public InspectionSpecWindow_HiPot(MainWindow mw)
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
            this.prodidTb.Text = mw.modelList[mw.selectedIndex].ProductCode;

            this.procidTb.Text = mw.modelList[mw.selectedIndex].ProcessID;
            this.useridTb.Text = mw.modelList[mw.selectedIndex].UserId;
            this.equipTb.Text = mw.modelList[mw.selectedIndex].EquipId;

            // Plus
            this.irVoltLevelTb.Text = EOL.IR_Recipe.BushingPlus.Volt.ToString();
            this.irVoltTimeTb.Text = EOL.IR_Recipe.BushingPlus.Time.ToString();

            this.irCoolingPlateVoltLevelTb.Text = EOL.IR_Recipe.CoolingPlatePlus.Volt.ToString();
            this.irCoolingPlateVoltTimeTb.Text = EOL.IR_Recipe.CoolingPlatePlus.Time.ToString();

            //Minus 
            this.irMinusVoltLevelTb.Text = EOL.IR_Recipe.BushingMinus.Volt.ToString();
            this.irMinusVoltTimeTb.Text = EOL.IR_Recipe.BushingMinus.Time.ToString();

            this.irMinusCoolingPlateVoltLevelTb.Text = EOL.IR_Recipe.CoolingPlateMinus.Volt.ToString();
            this.irMinusCoolingPlateVoltTimeTb.Text = EOL.IR_Recipe.CoolingPlateMinus.Time.ToString();

            if(EOL.IR_CtrlItem.Count == 6)
            {
                lblMBushVolt.IsEnabled = false;
                lblMBushTime.IsEnabled = false;

                irMinusVoltLevelTb.IsEnabled = false;
                irMinusVoltTimeTb.IsEnabled = false;

                lblMCoolVolt.IsEnabled = false;
                lblMCoolTime.IsEnabled = false;

                irMinusCoolingPlateVoltLevelTb.IsEnabled = false;
                irMinusCoolingPlateVoltTimeTb.IsEnabled = false;
            }
            else
            {
                lblMBushVolt.IsEnabled = true;
                lblMBushTime.IsEnabled = true;

                irMinusVoltLevelTb.IsEnabled = true;
                irMinusVoltTimeTb.IsEnabled = true;

                lblMCoolVolt.IsEnabled = true;
                lblMCoolTime.IsEnabled = true;

                irMinusCoolingPlateVoltLevelTb.IsEnabled = true;
                irMinusCoolingPlateVoltTimeTb.IsEnabled = true;
            }
        }

        void ModelTb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mw.RefreshToFlags();

            this.testSpecDg.ItemsSource = mw.modelList[mw.selectedIndex].TestItemList;
            this.prodidTb.Text = mw.modelList[mw.selectedIndex].ProductCode;
            this.procidTb.Text = mw.modelList[mw.selectedIndex].ProcessID;
        }


   
        // save
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mw.modelList[mw.selectedIndex].ProductCode =  CONFIG.MesProductCode = mw.prodTb.Text = this.prodidTb.Text;
                mw.modelList[mw.selectedIndex].ProcessID = CONFIG.MesProcessID = mw.procTb.Text = this.procidTb.Text;
                mw.modelList[mw.selectedIndex].UserId = mw.userTb.Text = this.useridTb.Text;
                mw.modelList[mw.selectedIndex].EquipId =  CONFIG.MesEquipID =this.equipTb.Text;

                EOL.IR_Recipe.BushingPlus.Volt = int.Parse(this.irVoltLevelTb.Text);
                EOL.IR_Recipe.BushingPlus.Time = int.Parse(this.irVoltTimeTb.Text);

                EOL.IR_Recipe.CoolingPlatePlus.Volt = int.Parse(this.irCoolingPlateVoltLevelTb.Text);
                EOL.IR_Recipe.CoolingPlatePlus.Time = int.Parse(this.irCoolingPlateVoltTimeTb.Text);

                EOL.IR_Recipe.BushingMinus.Volt = int.Parse(this.irMinusVoltLevelTb.Text);
                EOL.IR_Recipe.BushingMinus.Time = int.Parse(this.irMinusVoltTimeTb.Text);

                EOL.IR_Recipe.CoolingPlateMinus.Volt = int.Parse(this.irMinusCoolingPlateVoltLevelTb.Text);
                EOL.IR_Recipe.CoolingPlateMinus.Time = int.Parse(this.irMinusCoolingPlateVoltTimeTb.Text);

                mw.SetControlItemToCSV();
                mw.JsonSave();
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
                ct.reason.Content = "ver. 18/01/13";
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
