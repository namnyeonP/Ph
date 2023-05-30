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
        public void ClearDTC()
        {
            LogState(LogType.Info, "Clear DTC-------------------------------------------------------");
            Hybrid_Instru_CAN._635List.Clear();
            Hybrid_Instru_CAN.SendToCAN("7FF", new byte[] { 0x04, 0x14, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00 });
            LogState(LogType.Info, "Send 7FF : 04 14 FF FF FF 00 00 00");

            Thread.Sleep(100);
            //if (Hybrid_Instru_CAN._635List.Count == 1 && Hybrid_Instru_CAN._635List[0].Contains("10 53"))
            //{
            //    foreach (var fe009c in Hybrid_Instru_CAN._635List)
            //    {
            //        LogState(LogType.Success, "Recved data - " + fe009c);
            //    }
            //}
            //else
            {
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Info, "Recv 635 :" + fe009c);
                }
            }
        }

    

      
    }
}