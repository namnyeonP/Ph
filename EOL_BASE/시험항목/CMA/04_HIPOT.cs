using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        public bool ContactPositive_Check(TestItem ti)
        {
            isProcessingUI(ti);

            var cap = 0.0;
            chroma.isPlusFlag = true;
            chroma.GetCapacitance(1, 3, out cap);
            
            ti.Value_ = (cap * 1000000000);//nF
            
            return JudgementTestItem(ti);
        }
        public bool ChassisToPositive_IR(TestItem ti)
        {
            isProcessingUI(ti);

            var res = 0.0;
            chroma.isPlusFlag = true;
            chroma.GetResistance(1, 3, out res);

            if (res > 9.9E+30)
            {
                res = 99999;
                ti.Value_ = res;
            }
            else if (res == -1)
            {
                ti.Value_ = _DEVICE_NOT_READY;
            }
            else
                ti.Value_ = res / 1000000;


            return JudgementTestItem(ti);
        }

        public bool ChassisToPositive_WITH(TestItem ti)
        {
            isProcessingUI(ti);

            var current = 0.0;
            chroma.isPlusFlag = true;
            chroma.GetVoltage(1, 3, out current);

            ti.Value_ = current*1000;

            return JudgementTestItem(ti);
        }


        public bool ContactNegative_Check(TestItem ti)
        {
            isProcessingUI(ti);

            var cap = 0.0;
            chroma.isPlusFlag = false;
            chroma.GetCapacitance(2, 3, out cap);

            ti.Value_ = (cap * 1000000000);//nF

            return JudgementTestItem(ti);
        }
        public bool ChassisToNegative_IR(TestItem ti)
        {
            isProcessingUI(ti);

            var res = 0.0;
            chroma.isPlusFlag = false;
            chroma.GetResistance(2, 3, out res);

            if (res > 9.9E+30)
            {
                res = 99999;
                ti.Value_ = res;
            }
            else
                ti.Value_ = res / 1000000;


            return JudgementTestItem(ti);
        }

        public bool ChassisToNegative_WITH(TestItem ti)
        {
            isProcessingUI(ti);

            var current = 0.0;
            chroma.isPlusFlag = false;
            chroma.GetVoltage(3, 2, out current);

            ti.Value_ = current * 1000;

            return JudgementTestItem(ti);
        }
    }
}