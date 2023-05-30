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
    /// NextWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NextWindow : Window
    {
        public NextWindow(string mainstr, string substr, MainWindow mw)
        {
            InitializeComponent();
            maintitle.Content = mainstr;
            reason.Content = substr;
        }


        public void contibt_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void Alarm_Close()
        {
            this.Close();
        }
    }
}
