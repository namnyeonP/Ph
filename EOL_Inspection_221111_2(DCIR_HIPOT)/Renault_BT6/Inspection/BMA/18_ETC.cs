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
     




 
        public bool Volt_Dev_2(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = null;
            return JudgementTestItem(ti);
        }


        public bool Volt_Dev_3(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = null;
            return JudgementTestItem(ti);
        }


        public bool Volt_Dev_4(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = null;
            return JudgementTestItem(ti);
        }


        public bool Volt_Dev_5(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = null;
            return JudgementTestItem(ti);
        }


        public bool Volt_Dev_6(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = null;
            return JudgementTestItem(ti);
        }


        

    }
}