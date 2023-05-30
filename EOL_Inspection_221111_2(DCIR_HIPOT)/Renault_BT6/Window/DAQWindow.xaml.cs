using System;
using System.Collections.Generic;
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

namespace Renault_BT6.윈도우
{
    /// <summary>
    /// DAQWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DAQWindow : Window
    {
        MainWindow mw;
        List<double> _CellVoltageList = new List<double>();
        Dictionary<int, double> tempdir = new Dictionary<int, double>();

        public DAQWindow(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;

            ShowVoltList();
        }

        private void ShowVoltList()
        {
            try
            {
                Dictionary<int, double> tempdic = new Dictionary<int, double>();

                for (int i = 0; i < BatteryInfo.CellCount; i++)
                {
                    double voltage = Device.Cycler.CellVoltageDict[i];

                    tempdic.Add(i + 1, voltage);
                }

                try
                {
                    this.testSpecDg.ItemsSource = tempdic;
                }
                catch (Exception)
                {
                }
            }
            catch
            {

            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowVoltList();
            }
            catch
            {

            }
           
        }
    }
}
