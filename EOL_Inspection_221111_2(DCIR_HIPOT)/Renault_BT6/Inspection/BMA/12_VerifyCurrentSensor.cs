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
        public bool VerifyCurrent_CIDD(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = tempVerifyCurrSensorCIDD_cha;
            return JudgementTestItem(ti);
        }


        public bool VerifyCurrent_ERAD(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = tempVerifyCurrSensorERAD_dis;
            return JudgementTestItem(ti);
        }


        public bool SOC(TestItem ti)
        {
            isProcessingUI(ti);

            var rList = new List<string>();
            GetToDID_singleData(0x48, 0x01, "05 62", true, out rList);

            ti.Value_ = Convert.ToInt32(rList[3] + rList[4], 16) / 500;

            return JudgementTestItem(ti);
        }


        public bool DeltaVoltage_After_DCIR(TestItem ti)
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
                ti.Value_ = (cellList.Max() - cellList.Min()) * 1000;//mV
            }
            return JudgementTestItem(ti);
        }


        public bool DeltaTemp_After_DCIR(TestItem ti)
        {
            isProcessingUI(ti);
            temp1delta = 0.0;
            temp2delta = 0.0;
            temp3delta = 0.0;
            temp4delta = 0.0;
            temp5delta = 0.0;
            temp6delta = 0.0;
            temp7delta = 0.0;
            temp8delta = 0.0;
            temp9delta = 0.0;
            temp10delta = 0.0;
            temp11delta = 0.0;
            temp12delta = 0.0;
            var rList = new List<string>();
            GetToDID_singleData(0x49, 0x09, "05 62", false, out rList);
            temp1delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0A, "05 62", false, out rList);
            temp2delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0B, "05 62", false, out rList);
            temp3delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0C, "05 62", false, out rList);
            temp4delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0D, "05 62", false, out rList);
            temp5delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0E, "05 62", false, out rList);
            temp6delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x0F, "05 62", false, out rList);
            temp7delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x10, "05 62", false, out rList);
            temp8delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x11, "05 62", false, out rList);
            temp9delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x12, "05 62", false, out rList);
            temp10delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x13, "05 62", false, out rList);
            temp11delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;

            rList = new List<string>();
            GetToDID_singleData(0x49, 0x14, "05 62", false, out rList);
            temp12delta = (Convert.ToInt32(rList[3] + rList[4], 16) * 0.01) - 50;


            List<double> tempList = new List<double>();
            tempList.Add(temp1delta);
            tempList.Add(temp2delta);
            tempList.Add(temp3delta);
            tempList.Add(temp4delta);
            tempList.Add(temp5delta);
            tempList.Add(temp6delta);
            tempList.Add(temp7delta);
            tempList.Add(temp8delta);
            tempList.Add(temp9delta);
            tempList.Add(temp10delta);
            tempList.Add(temp11delta);
            tempList.Add(temp12delta);

            tempViewList.Clear();
            for (int i = 0; i < tempList.Count; i++)
            {
                tempViewList.Add(new doubleValue() { Name = "Temp" + (i + 1).ToString(), Value_ = tempList[i] });
            }

            LogState(LogType.Info, "Max Temp:" + tempList.Max() + "/Min Temp:" + tempList.Min());
            ti.Value_ = tempList.Max() - tempList.Min();

            return JudgementTestItem(ti);
        }

    }
}