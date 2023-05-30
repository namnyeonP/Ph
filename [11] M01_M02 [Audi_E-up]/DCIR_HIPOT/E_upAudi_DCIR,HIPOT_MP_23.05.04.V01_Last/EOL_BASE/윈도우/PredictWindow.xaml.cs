using Microsoft.WindowsAPICodePack.DirectX.Direct3D10;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Diagnostics;
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
    /// AlarmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PredictWindow : Window
    {
        MainWindow mw;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="wlist">Warning List</param>
        /// <param name="glist">Grid List</param>
        /// <param name="dlist">DC List</param>
        /// <param name="flist">Fault List</param>
        public PredictWindow(MainWindow mw)
        {
            InitializeComponent();
            this.Closing += AlarmWindow_Closing;
            this.mw = mw;

            this.Loaded += PredictWindow_Loaded;
        }

        private void PredictWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var md in mw.modelDataList)
            {
                modelCb.Items.Add(md.ModelId);
            }

            stationCb.Items.Add("#1");
            stationCb.Items.Add("#2");
            stationCb.Items.Add("#3");
            stationCb.Items.Add("#4");
        }

        private void AlarmWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        void EnableUI()
        {
            searchBt.IsEnabled = wLb.IsEnabled = true;
            searchBt.Content = "Search data";
        }

        void DisableUI()
        {
            searchBt.IsEnabled = wLb.IsEnabled = false;
            searchBt.Content = "Searching..";
        }
        Thread searchTh = null;
        private void searchBt_Click(object sender, RoutedEventArgs e)
        {
            if (searchTh != null)
                return;
            
            wLb.ItemsSource = null;


            #region Make Query N Execute
            DateTime vFDate = DateTime.Now;
            DateTime vTDate = DateTime.Now;
            //int year, int month, int day, int hour, int minute, int second, int millisecond

            if (dateRb.IsChecked == true)
            {
                if (startDp.SelectedDate == null)
                {
                    sercLb.Content = "Search fail!!";
                    return;
                }

                if (endDp.SelectedDate == null)
                {
                    sercLb.Content = "Search fail!!";
                    return;
                }

                vFDate = (DateTime)startDp.SelectedDate;
                vTDate = (DateTime)endDp.SelectedDate;

                //오늘일때, 시간을 23:59:59로 만든다.
                if (vTDate.Year == DateTime.Now.Year &&
                    vTDate.Month == DateTime.Now.Month &&
                    vTDate.Day == DateTime.Now.Day)
                {
                    vTDate = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                23,
                59,
                59,
                0);
                }
            }
            else if (datepRb.IsChecked == true)
            {
                int day = 1;
                if(!int.TryParse(dateperiodTb.Text,out day))
                {
                    sercLb.Content = "Search fail!!";
                    return;
                }   

                vTDate = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                23,
                59,
                59,
                0);

                vFDate = vTDate.AddDays(day * -1);

                vFDate = new DateTime(
                    vFDate.Year,
                    vFDate.Month,
                    vFDate.Day,
                    0, 0, 0, 0);
            }
            
            var vStr = "SELECT * FROM " + mw.tableName +
                " WHERE ModelName = " + @"'" + modelCb.SelectedItem.ToString() + @"'" +
                " AND " +
                "StationNum = " + @"'" + stationCb.SelectedItem.ToString() + @"'";

            if (passCb.IsChecked == true && ngCb.IsChecked == true)
            {
                //vStr += @" AND IsPass = 'YES' OR 'NO'";
            }
            else if (passCb.IsChecked == true)
            {
                vStr += @" AND IsPass = 'YES'";
            }
            else if (ngCb.IsChecked == true)
            {
                vStr += @" AND IsPass = 'NO'";
            }

            vStr += " AND FinishedTime BETWEEN @FD AND @TD";

            #endregion

            OleDbCommand cmd = new OleDbCommand(vStr);
            cmd.Parameters.AddWithValue("@FD", vFDate);
            cmd.Parameters.AddWithValue("@TD", vTDate);

            DisableUI();
            searchTh = new Thread(() =>
            {
                Stopwatch st = new Stopwatch();
                st.Start();
                
                var reader = mw.GetDataToSQL(cmd);

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    wLb.ItemsSource = reader;
                    sercLb.Content = st.ElapsedMilliseconds.ToString();

                    EnableUI();
                }));
                searchTh = null;
            });
            searchTh.Start();

            //var reader = mw.GetDataToSQL("SELECT * FROM " + mw.tableName +
            //    " WHERE ModelName = " + @"'" + modelCb.SelectedItem.ToString() + @"'" +
            //    " AND " +
            //    "StationNum = " + @"'" + stationCb.SelectedItem.ToString() + @"'");
            //" AND " +
            //    "FinishedTime BEETWEEN > " + @"'" + startDp.SelectedDate. stationCb.SelectedItem.ToString() + @"'");

        }
    }
}
