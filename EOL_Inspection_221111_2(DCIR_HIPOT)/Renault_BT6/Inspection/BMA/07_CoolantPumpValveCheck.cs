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

        
    

    
        double _3way_valve = 0.0;

        public bool ValveCheck(TestItem ti)
        {
            isProcessingUI(ti);

            if (_3way_valve == 0.0)
            {
                ti.Value_ = _VALUE_NOT_MATCHED;
            }
            else
            {
                ti.Value_ = _3way_valve;
            }

            //if (SetExtendedMode())
            //{
            //    var rList = new List<string>();
            //    if (GetToDID_singleData(0x01, 0x40, "05 71", false, out rList, 0x05, 0x31, 0x1B, 0x01))
            //    {
            //        Hybrid_Instru_CAN._635List.Clear();
            //        Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x03, 0x22, 0xD9, 0x00, 0x00, 0x00, 0x00, 0x00 });
            //        LogState(LogType.Info, "Send 735 { 0x03, 0x22, 0xD9, 0x00, 0x00, 0x00, 0x00, 0x00}");

            //        Thread.Sleep(50);
            //        if (Hybrid_Instru_CAN._635List.Count == 1 && Hybrid_Instru_CAN._635List[0].Contains("10 53"))
            //        {
            //            foreach (var fe009c in Hybrid_Instru_CAN._635List)
            //            {
            //                LogState(LogType.Success, "Recved data - " + fe009c);
            //            }
            //            Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            //            LogState(LogType.Info, "Send 735 { 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}");

            //            Thread.Sleep(100);

            //            rList = new List<string>();
            //            foreach (var fe009c in Hybrid_Instru_CAN._635List)
            //            {
            //                LogState(LogType.Success, "Recved data - " + fe009c);
            //                var arr = fe009c.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            //                for (int i = 1; i < arr.Length; i++)
            //                {
            //                    rList.Add(arr[i]);
            //                }
            //            }

            //            ti.Value_ = (Convert.ToInt32(rList[52] + rList[53], 16) / 823.116);//A
            //        }
            //        else
            //        {
            //            foreach (var fe009c in Hybrid_Instru_CAN._635List)
            //            {
            //                LogState(LogType.Fail, "Recved data - " + fe009c);
            //            }
            //            ti.Value_ = _VALUE_NOT_MATCHED;
            //        }

            //    }
            //    else
            //    {
            //    }

            //    rList = new List<string>();
            //    if (!GetToDID_singleData(0x01, 0x40, "05 71", false, out rList, 0x05, 0x31, 0x1B))
            //    {
            //        ti.Value_ = _VALUE_NOT_MATCHED;
            //    }

            //    if (!SetDefaultMode())
            //    {
            //        ti.Value_ = _NOT_MODE_CHANGED;
            //    }
            //}
            //else
            //{
            //    ti.Value_ = _NOT_MODE_CHANGED;
            //}
            return JudgementTestItem(ti);
        }

    }
}