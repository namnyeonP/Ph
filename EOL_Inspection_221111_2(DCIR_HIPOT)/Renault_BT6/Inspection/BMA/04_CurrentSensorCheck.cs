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
        double fineCurr, coarseCurr = 0.0;
        public bool FineCurrentSensor(TestItem ti)
        {
            isProcessingUI(ti);

            fineCurr=coarseCurr = 0.0;

            Hybrid_Instru_CAN._635List.Clear();

            Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x03, 0x22, 0xD9, 0x00, 0x00, 0x00, 0x00, 0x00 });
            LogState(LogType.Info, "Send 735 { 0x03, 0x22, 0xD9, 0x00, 0x00, 0x00, 0x00, 0x00}");

            Thread.Sleep(50);
            if (Hybrid_Instru_CAN._635List.Count == 1 && Hybrid_Instru_CAN._635List[0].Contains("10 53"))
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

                fineCurr = (Convert.ToInt32(rList[42] + rList[43], 16) / 54.6) - 37.5;

                coarseCurr = (Convert.ToInt32(rList[44] + rList[45], 16) / 54.6) - 37.5;

                ti.Value_ = fineCurr;
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


        public bool CoarseCurrentSensor(TestItem ti)
        {
            isProcessingUI(ti);
            if (fineCurr == 0.0)
            {
                ti.Value_ = null;
            }
            else
            {
                ti.Value_ = coarseCurr;
            }
            return JudgementTestItem(ti);
        }

    }
}