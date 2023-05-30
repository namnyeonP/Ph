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
    /// PopupWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopupWindow : Window
    {
        MainWindow mw;

        public PopupWindow(MainWindow mw)
        {
            InitializeComponent();

            this.mw = mw;
            
            this.Height += 100;
            this.Width += 200;
            this.maintitle.FontSize += 18;
            this.reason.FontSize += 13;
            this.okbt.FontSize += 13;

            this.maintitle.Content = "Remove Connector";
            this.reason.Content = "Remove hipot probe pin";
            //this.Show();
        }

        private void okbt_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
