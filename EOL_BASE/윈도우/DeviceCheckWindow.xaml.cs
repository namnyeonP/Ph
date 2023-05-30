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
    /// DeviceCheckWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeviceCheckWindow : Window
    {
        MainWindow mw;
        public DeviceCheckWindow(MainWindow mw)
        {
            InitializeComponent();
            if (mw.time_elapsed_tb.Text == "")
                autortrybt.Visibility = Visibility.Collapsed;

            this.Closing += DeviceCheckWindow_Closing;
            this.mw = mw;

            mw.isDeviceFail = true;
        }

        private void DeviceCheckWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mw.isDeviceFail = false;
        }

        private void Button_Retry_Click(object sender, RoutedEventArgs e)
        {
            //mw.AutoMode();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            mw.isDeviceFail = false;
        }
    }
}
