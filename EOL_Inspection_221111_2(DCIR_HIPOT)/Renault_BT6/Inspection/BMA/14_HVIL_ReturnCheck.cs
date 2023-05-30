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
    
        public bool HVIL2_Check(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = hvil2_rtn_open;
            return JudgementTestItem(ti);
        }


        public bool HVIL3_Check(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = hvil3_rtn_open;
            return JudgementTestItem(ti);
        }
    }
}