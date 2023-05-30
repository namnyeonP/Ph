using EOL_BASE.모듈;
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
    /// ListWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ListWindow : Window
    {
        MainWindow mw;
        public ListWindow(MainWindow mw, List<doubleValue> list)
        {
            InitializeComponent();

            this.mw = mw;
            this.MouseLeave -= ListWindow_MouseLeave;
            this.MouseLeave += ListWindow_MouseLeave;

            detail_dg.ItemsSource = list;
            detail_dg.Items.Refresh();
        }

        void ListWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Close();
        }
    }
}
