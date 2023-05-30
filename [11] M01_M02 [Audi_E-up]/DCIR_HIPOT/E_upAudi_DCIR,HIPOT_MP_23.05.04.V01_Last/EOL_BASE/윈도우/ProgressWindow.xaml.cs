using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// ProgressWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgressWindow : Window
    {
        double itemCnt = 0.0;
        MainWindow mw;
        public ProgressWindow(MainWindow mw, int cnt)
        {
            InitializeComponent();
            lb.Items.Insert(0,"Initialized");
            this.mw = mw;
            //itemCnt = 100.0 / cnt;
            //pb.Value = 0;
        }

        public void SetString(string str)
        {
            mw.LogState(LogType.Info, str);
            //mw.Dispatcher.Invoke(new Action(() =>
            //{
            //    //pb.Value += itemCnt;
            //    lb.Items.Insert(0, str);
            //}));
            Thread.Sleep(500);
        }
    }
}
