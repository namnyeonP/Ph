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
        MainWindow mw;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="wlist">Warning List</param>
        /// <param name="glist">Grid List</param>
        /// <param name="dlist">DC List</param>
        /// <param name="flist">Fault List</param>
        public AlarmWindow(MainWindow mw, List<string> wlist, List<string> glist, List<string> dlist, List<string> flist,
            double rVolt, double sVolt, double rCur, double sCur, double tCur)
        {
            InitializeComponent();
            this.Closing += AlarmWindow_Closing;
            this.mw = mw;

            maintitle1.Content = "Check the terminal and currnet probe surface";

            wLb.ItemsSource = wlist;
            gLb.ItemsSource = glist;
            dLb.ItemsSource = dlist;
            fLb.ItemsSource = flist;

            mw.LogState(LogType.Info, "DSP Type:" + mw.dsp_Type.ToString());
            foreach (var w in wlist)
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

            mw.isCyclerFail = true;

            this.acRVoltLb.Content = "[" + rVolt.ToString() + "]";
            this.acSVoltLb.Content = "[" + sVolt.ToString() + "]";

            this.acRCurLb.Content = "[" + rCur.ToString() + "]";
            this.acSCurLb.Content = "[" + sCur.ToString() + "]";
            this.acTCurLb.Content = "[" + tCur.ToString() + "]";

            mw.LogState(LogType.Info, "AC_R_PHASE_VOLT : " + rVolt.ToString());
            mw.LogState(LogType.Info, "AC_S_PHASE_VOLT : " + sVolt.ToString());
            mw.LogState(LogType.Info, "AC_R_PHASE_CUR  : " + rCur.ToString());
            mw.LogState(LogType.Info, "AC_S_PHASE_CUR  : " + sCur.ToString());
            mw.LogState(LogType.Info, "AC_T_PHASE_CUR  : " + tCur.ToString());
        }

        private void AlarmWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mw.isCyclerFail = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mw.isCyclerFail = false;
            this.Hide();
        }
    }
}
