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

        public bool ClearDTC_N_ShutDn(TestItem ti)
        {
            isProcessingUI(ti);
            ClearDTC();

            #region EF Write

            Thread.Sleep(500);

            if (!SetDefaultMode())
            {
                ti.Value_ = _NOT_MODE_CHANGED;
                return JudgementTestItem(ti);
            }

            Thread.Sleep(500);

            if (!SetExtendedMode())
            {
                ti.Value_ = _NOT_MODE_CHANGED;
                return JudgementTestItem(ti);
            }

            Thread.Sleep(500);

            var rList = new List<string>();
            if (!GetToDID_singleData(0x48, 0x95, "03 ", false, out rList, 0x04, 0x2E, 0xEF))
            {
                ti.Value_ = _VALUE_NOT_MATCHED;
                return JudgementTestItem(ti);
            }

            Thread.Sleep(500);

            if (!SetDefaultMode())
            {
                ti.Value_ = _NOT_MODE_CHANGED;
                return JudgementTestItem(ti);
            }

            #endregion

            becm_powerOff(40000);

            //open hvil interlock
            relays.RelayOff("IDO_34");
            relays.RelayOff("IDO_35");
            relays.RelayOff("IDO_36");

            ti.Value_ = 1;
            return JudgementTestItem(ti);
        }


        public bool SleepCurrent(TestItem ti)
        {
            isProcessingUI(ti);

            //relays.RelayOn("IDO_20");
            //Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:CURR?\n") * 1000000;
            }
            //relays.RelayOff("IDO_20");
            //Thread.Sleep(300);
            becm_power.Set_OUTP_Off();

            relays.RelayOff("IDO_20");

            return JudgementTestItem(ti);
        }
    }
}