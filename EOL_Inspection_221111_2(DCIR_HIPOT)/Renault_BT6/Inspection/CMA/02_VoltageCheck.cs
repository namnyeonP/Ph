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
        List<double> _CellVoltageList { get; set; }

    private void SetOcvCellVoltage(List<double> CellVoltageList)
        {
            //ocvDetailList[0].CellVolt_1 = double.Parse(CellVoltageList[0].ToString("N4"));
            //ocvDetailList[0].CellVolt_2 = double.Parse(CellVoltageList[1].ToString("N4"));
            //ocvDetailList[0].CellVolt_3 = double.Parse(CellVoltageList[2].ToString("N4"));
            //ocvDetailList[0].CellVolt_4 = double.Parse(CellVoltageList[3].ToString("N4"));
            //ocvDetailList[0].CellVolt_5 = double.Parse(CellVoltageList[4].ToString("N4"));
            //ocvDetailList[0].CellVolt_6 = double.Parse(CellVoltageList[5].ToString("N4"));
            //ocvDetailList[0].CellVolt_7 = double.Parse(CellVoltageList[6].ToString("N4"));
            //ocvDetailList[0].CellVolt_8 = double.Parse(CellVoltageList[7].ToString("N4"));
        }

        public bool CellVoltage_High(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList.Max();
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage_Low(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList.Min();
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage_Dev(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                // 2018.11.02 jeonhj's comment
                // Deviation unit is mV(1V == 100mV)
                ti.Value_ = (_CellVoltageList.Max()- _CellVoltageList.Min()) * 100;
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage1(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[0];
            }
               
            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage2(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[1];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage3(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[2];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage4(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[3];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage5(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[4];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage6(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[5];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage7(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[6];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage8(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[7];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage9(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[8];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage10(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[9];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage11(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[10];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltage12(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[11];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage13(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[12];
            }

            return JudgementTestItem(ti);
        }

        public bool CellVoltage14(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count == BatteryInfo.CellCount)
            {
                ti.Value_ = _CellVoltageList[13];
            }

            return JudgementTestItem(ti);
        }
        
        public bool CellVoltageDev(TestItem ti)
        {
            isProcessingUI(ti);

            if (_CellVoltageList != null && _CellVoltageList.Count > 2)
            {
                _CellVoltageList.Remove(_CellVoltageList.Max());
                ti.Value_ = (_CellVoltageList.Max() - _CellVoltageList.Min()) * 100;
            }

            return JudgementTestItem(ti);
        }
    }
}