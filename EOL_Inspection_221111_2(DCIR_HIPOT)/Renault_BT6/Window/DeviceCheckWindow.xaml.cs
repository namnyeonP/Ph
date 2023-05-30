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
    /// DeviceCheckWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeviceCheckWindow : Window
    {
        MainWindow mw;

        public int m_nWndType = 0;

        public DeviceCheckWindow(MainWindow mw, bool bIsShowButton = true, int nWindType = 0)
        {
            InitializeComponent();
            if (mw.time_elapsed_tb.Text == "")
                autortrybt.Visibility = Visibility.Collapsed;

            this.mw = mw;

            if(bIsShowButton == false)
            {
                okbt.Visibility = System.Windows.Visibility.Hidden;
            }

            m_nWndType = nWindType;

        }

        private void Button_Retry_Click(object sender, RoutedEventArgs e)
        {
            //mw.AutoMode();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_nWndType == 1)
            {
                this.Hide();

                return;
            }

            this.Close();
        }
    }
}
