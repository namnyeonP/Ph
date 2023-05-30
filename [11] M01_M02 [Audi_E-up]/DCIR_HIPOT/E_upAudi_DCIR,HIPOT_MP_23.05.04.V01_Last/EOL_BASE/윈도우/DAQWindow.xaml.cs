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

namespace EOL_BASE.윈도우
{
    /// <summary>
    /// DAQWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DAQWindow : Window
    {
        MainWindow mw;
        public DAQWindow(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;

            Dictionary<int, double> tempdir = new Dictionary<int, double>();
            foreach (var item in mw.daq.DAQList)
            {
                tempdir.Add(item.Key+1, item.Value);
            }


            try
            {
                this.testSpecDg.ItemsSource = tempdir;
            }
            catch (Exception)
            {
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Dictionary<int, double> tempdir = new Dictionary<int, double>();
            foreach (var item in mw.daq.DAQList)
            {
                tempdir.Add(item.Key + 1, item.Value);
            }


            try
            {
                this.testSpecDg.ItemsSource = tempdir;
            }
            catch (Exception)
            {
            }
        }
    }
}
