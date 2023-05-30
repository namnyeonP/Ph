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

        public bool CellDeltaVoltageBeforeDCIR(TestItem ti)
        {
            isProcessingUI(ti);
            var rList = new List<string>();
            GetToDID_singleData(0x48, 0x06, "10 C3", true, out rList);

            if (rList != null)
            {
                cellViewList.Clear();
                var cellList = new List<double>();

                int cnt = 1;
                for (int i = 4; i < 196; i = i + 2)
                {
                    var cv = Convert.ToInt32(rList[i] + rList[i + 1], 16) * 0.001;
                    LogState(LogType.Info, "Cell " + cnt.ToString() + " : " + cv.ToString("N3"));
                    cellList.Add(cv);// V


                    cellViewList.Add(new doubleValue() { Name = "Cell" + (cnt).ToString(), Value_ = cv });
                    cnt++;
                }

                LogState(LogType.Info, "Max Voltage:" + cellList.Max() + "/Min Voltage:" + cellList.Min());
                ti.Value_ = (cellList.Max() - cellList.Min())*1000;//mV
            }

            //SendToCAN("56", new byte[] { 0x0, 0x0B, 0x0, 0x0, 0x41, 0x0, 0x0, 0x0 });
            return JudgementTestItem(ti);
        }

        public bool InletTemp(TestItem ti)
        {
            isProcessingUI(ti);
            var rList = new List<string>();
            GetToDID_singleData(0x48, 0x04, "04 62", false, out rList);

            if (rList != null)
            {
                var inlet = Convert.ToInt32(rList[3],16) - 50;

                //var localTemp = Device.Tempr.GetTemp;
                //var localTemp = Device.TemprCT100.GetTemprature();

                var localTemp = new float();

                if (CONFIG.EolInspType == InspectionType.EOL)
                {
                    localTemp = Device.TemprCT100.GetTemprature();
                }
                else if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    localTemp = Device.Tempr.GetTemp;
                }
                else { }

                LogState(LogType.Info, "Ambient Temp:" + localTemp);
                ti.Value_ = localTemp - inlet;
            }
            return JudgementTestItem(ti);
        }

        
        double packVolt_cycler, be_a, be_b, be_c, be_d, be_e, be_f, be_g, be_h;

        public bool Pack_BE_A(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = be_a - packVolt_cycler;
            return JudgementTestItem(ti);
        }

        public bool Pack_BE_B(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = be_b - packVolt_cycler;
            return JudgementTestItem(ti);
        }


        public bool Pack_BE_C(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = be_c - packVolt_cycler;
            return JudgementTestItem(ti);
        }


        public bool Pack_BE_D(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = be_d - packVolt_cycler;
            return JudgementTestItem(ti);
        }


        public bool Pack_BE_E(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = be_e - packVolt_cycler;
            return JudgementTestItem(ti);
        }


        public bool Pack_BE_F(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = be_f - packVolt_cycler;
            return JudgementTestItem(ti);
        }


        public bool Pack_BE_G(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = be_g;
            return JudgementTestItem(ti);
        }


        public bool Pack_BE_H(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = be_h;
            return JudgementTestItem(ti);
        }

        public bool Dev_Cell_Volt_1(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = "TODO";
            return JudgementTestItem(ti);
        }

        public bool Dev_Cell_Volt_2(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = "TODO";
            return JudgementTestItem(ti);
        }

        public bool Dev_Cell_Volt_3(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = "TODO";
            return JudgementTestItem(ti);
        }
        public bool Dev_Cell_Volt_4(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = "TODO";
            return JudgementTestItem(ti);
        }

        public bool Dev_Cell_Volt_5(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = "TODO";
            return JudgementTestItem(ti);
        }

        public bool Dev_Cell_Volt_6(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = "TODO";
            return JudgementTestItem(ti);
        }
        
    }
}