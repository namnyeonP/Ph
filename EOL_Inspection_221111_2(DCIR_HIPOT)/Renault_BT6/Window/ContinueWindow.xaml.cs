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
    /// ContinueWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContinueWindow : Window
    {
        public bool isContinue = false;
        MainWindow mw;
        public ContinueWindow(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;
        }

        private void contibt_Click(object sender, RoutedEventArgs e)
        {
            isContinue = true;
            mw.Ispause = false;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            isContinue = false;
            this.Hide();
        }
    }
}
