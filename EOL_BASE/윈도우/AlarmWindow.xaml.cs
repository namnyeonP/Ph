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
    /// AlarmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="wlist">Warning List</param>
        /// <param name="glist">Grid List</param>
        /// <param name="dlist">DC List</param>
        /// <param name="flist">Fault List</param>
        public AlarmWindow(MainWindow mw,List<string> wlist, List<string> glist, List<string> dlist, List<string> flist)
        {
            InitializeComponent();
            wLb.ItemsSource = wlist;
            gLb.ItemsSource = glist;
            dLb.ItemsSource = dlist;
            fLb.ItemsSource = flist;

            maintitle1.Content = "Check the terminal and currnet probe surface";

            mw.LogState(LogType.Info, "DSP Type:" + mw.dsp_Type.ToString());
            foreach(var w in wlist)
            {
                mw.LogState(LogType.Info, w);
            }
            foreach (var w in glist)
            {
                mw.LogState(LogType.Info, w);
            }
            foreach (var w in dlist)
            {
                mw.LogState(LogType.Info, w);
            }
            foreach (var w in flist)
            {
                mw.LogState(LogType.Info, w);
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
