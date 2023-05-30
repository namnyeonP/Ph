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
    /// ContinueWindowC727.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContinueWindowC727 : Window
    {
        public bool isContinue = false;
        public ContinueWindowC727()
        {
            InitializeComponent();
        }

        private void contibt_Click(object sender, RoutedEventArgs e)
        {
            isContinue = true;
            this.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            isContinue = false;
            this.Hide();
        }
    }
}
