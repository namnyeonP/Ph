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
        public bool HVIL1_Check(TestItem ti)
        {
            isProcessingUI(ti);

            relays.RelayOff("IDO_34");
            relays.RelayOff("IDO_35");
            relays.RelayOff("IDO_36");
            Thread.Sleep(500);

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

                hvil1_rtn_open = (Convert.ToInt32(rList[28] + rList[29], 16) / 134.162);//V

                hvil2_rtn_open = (Convert.ToInt32(rList[32] + rList[33], 16) / 134.162);

                hvil3_rtn_open = (Convert.ToInt32(rList[36] + rList[37], 16) / 134.162);

                ti.Value_ = hvil1_rtn_open;
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