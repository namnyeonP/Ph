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
   
        double hvil1_rtn = 0.0;
        double hvil2_rtn = 0.0;
        double hvil3_rtn = 0.0;

        double hvil1_rtn_open = 0.0;
        double hvil2_rtn_open = 0.0;
        double hvil3_rtn_open = 0.0;
        public bool TurnBMSCheck2(TestItem ti)
        {
            isProcessingUI(ti);
            
            ti.Value_ = hvil2_rtn;
            
            return JudgementTestItem(ti);
        }


        public bool TurnBMSCheck3(TestItem ti)
        {
            isProcessingUI(ti);
            
            ti.Value_ = hvil3_rtn;            

            return JudgementTestItem(ti);
        }

    }
}