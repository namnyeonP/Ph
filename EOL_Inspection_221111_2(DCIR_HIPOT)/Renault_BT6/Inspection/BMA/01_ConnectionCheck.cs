using Renault_BT6.모듈;
using Renault_BT6.윈도우;
using Renault_BT6.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Renault_BT6
{
    public partial class MainWindow
    {
        public bool EoLVersionCheck(TestItem ti)
        {
            isProcessingUI(ti);

            ti.Value_ = "1";
            ti.Result = "PASS";

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (IsManual)
                    blinder.Visibility = System.Windows.Visibility.Hidden;
            }));

            isProcessingFlag = false;
            return true;
        }



    }
}