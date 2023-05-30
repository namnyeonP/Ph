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
     
        double cvtn1_ref_volt = 0.0;
        double cvtn2_ref_volt = 0.0;
        double cvtn3_ref_volt = 0.0;
        double cvtn4_ref_volt = 0.0;
        double cvtn5_ref_volt = 0.0;
        double cvtn6_ref_volt = 0.0;

        public bool CVTN1_Ref(TestItem ti)
        {
            isProcessingUI(ti);

            cvtn1_ref_volt = 0.0;
            cvtn2_ref_volt = 0.0;
            cvtn3_ref_volt = 0.0;
            cvtn4_ref_volt = 0.0;
            cvtn5_ref_volt = 0.0;
            cvtn6_ref_volt = 0.0;

            Hybrid_Instru_CAN._635List.Clear();
            Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x03, 0x22, 0xDA, 0xD7, 0x00, 0x00, 0x00, 0x00 });
            LogState(LogType.Info, "Send 735 { 0x03, 0x22, 0xDA, 0xD7, 0x00, 0x00, 0x00, 0x00}");

            Thread.Sleep(50);
            if (Hybrid_Instru_CAN._635List.Count == 1 && Hybrid_Instru_CAN._635List[0].Contains("10 "))
            {
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Success, "Recved data - " + fe009c);
                }
                Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                LogState(LogType.Info, "Send 735 { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}");

                Thread.Sleep(100);

                var rList = new List<string>();
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Success, "Recved data - " + fe009c);
                    var arr = fe009c.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < arr.Length; i++)
                    {
                        rList.Add(arr[i]);
                    }
                }

                cvtn1_ref_volt = (Convert.ToInt32(rList[4] + rList[5], 16) *0.001);
                cvtn2_ref_volt = (Convert.ToInt32(rList[6] + rList[7], 16) * 0.001);
                cvtn3_ref_volt = (Convert.ToInt32(rList[8] + rList[9], 16) * 0.001);
                cvtn4_ref_volt = (Convert.ToInt32(rList[10] + rList[11], 16) * 0.001);
                cvtn5_ref_volt = (Convert.ToInt32(rList[12] + rList[13], 16) * 0.001);
                cvtn6_ref_volt = (Convert.ToInt32(rList[14] + rList[15], 16) * 0.001);

                ti.Value_ = cvtn1_ref_volt;
            }
            else
            {
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Fail, "Recved data - " + fe009c);
                }
            }
            return JudgementTestItem(ti);
        }


        public bool CVTN2_Ref(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = cvtn2_ref_volt;
            return JudgementTestItem(ti);
        }


        public bool CVTN3_Ref(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = cvtn3_ref_volt;
            return JudgementTestItem(ti);
        }


        public bool CVTN4_Ref(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = cvtn4_ref_volt;
            return JudgementTestItem(ti);
        }


        public bool CVTN5_Ref(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = cvtn5_ref_volt;
            return JudgementTestItem(ti);
        }


        public bool CVTN6_Ref(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = cvtn6_ref_volt;
            return JudgementTestItem(ti);
        }
    }
}