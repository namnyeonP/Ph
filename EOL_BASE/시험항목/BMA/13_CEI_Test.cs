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

        public bool CEI_Test(TestItem ti)
        {
            isProcessingUI(ti);

            var cei_path = 0.0;

            ClearDTC();
            Thread.Sleep(500);
            Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
            LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");

            Thread.Sleep(1000);

            var arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");

            int retryCnt = 5;
            while (arr[0].Substring(0, 1) != "4")//0x80 == 128
            {
                ClearDTC();
                Thread.Sleep(500);
                Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
                LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");
                arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                //Thread.Sleep(1000);
                LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");
                retryCnt--;

                if (retryCnt == 0)
                {
                    ti.Value_ = _VALUE_NOT_MATCHED;

                    Hybrid_Instru_CAN._635List.Clear();
                    Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });
                    LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");
                    Thread.Sleep(50);
                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Success, "Recved data - " + fe009c);
                    }
                    return JudgementTestItem(ti);
                }
            }
            relays.RelayOn("IDO_32");
            Thread.Sleep(300);
            var befVolt = 0.0;
            if (keysight != null && keysight.socket.Connected)
            {
                befVolt = keysight.TrySend("MEAS:VOLT:DC?\n");
            }
            relays.RelayOff("IDO_32");
            Thread.Sleep(300);

            LogState(LogType.Info, "Before ERAD+ to ERAD- : " + befVolt);
            Thread.Sleep(500);
            //cei power on!
            //maybe contactor is opened..check
            
            LogState(LogType.Info, "[SIMU] Crash ON");
            relays.RelayOn("IDO_31");
            Thread.Sleep(300);

            LogState(LogType.Info, "[SIMU] Power ON");
            relays.RelayOn("IDO_30");

            LogState(LogType.Info, "Wait 2000ms");
            //Thread.Sleep(2000);
            relays.RelayOn("IDO_32");    
            List<double> klist = new List<double>();
            if (keysight != null && keysight.socket.Connected)
            {
                for (int i = 0; i < 10; i++)
                {
                    var kvolt = keysight.TrySend("MEAS:VOLT:DC?\n");
                    if (kvolt < 0)
                    {
                        break;
                    }
                    else
                    {
                        klist.Add(kvolt);
                    }
                }
            }

            if (klist.Count > 0)
            {
                ti.Value_ = klist[klist.Count-1];
            }
            relays.RelayOff("IDO_32");
            
            Thread.Sleep(2000);
            
            LogState(LogType.Info, "[SIMU] Power OFF");
            relays.RelayOff("IDO_30");
            Thread.Sleep(300);
            LogState(LogType.Info, "[SIMU] Crash OFF");
            relays.RelayOff("IDO_31");
            Thread.Sleep(100);

            ClearDTC();
            becm_powerOff(40000);
            becm_powerOn();


            //relays.RelayOff("IDO_19");
            //relays.RelayOff("IDO_20");
            //Thread.Sleep(500);
            //relays.RelayOn("IDO_19");
            //relays.RelayOn("IDO_20");
            //Thread.Sleep(500);
            
            #region NOT use, openContactor

            //Thread.Sleep(1000);
            //Hybrid_Instru_CAN._635List.Clear();
            //Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });
            //LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");

            //Thread.Sleep(50);
            //if (Hybrid_Instru_CAN._635List.Count == 1 && Hybrid_Instru_CAN._635List[0].Contains("02 11"))
            //{
            //    LogState(LogType.Success, "CONTACTOR_OPEN");
            //    foreach (var fe009c in Hybrid_Instru_CAN._635List)
            //    {
            //        LogState(LogType.Success, "Recved data - " + fe009c);
            //    }
            //}
            //else
            //{
            //    LogState(LogType.Fail, "CONTACTOR_OPEN");
            //    foreach (var fe009c in Hybrid_Instru_CAN._635List)
            //    {
            //        LogState(LogType.Fail, "Recved data - " + fe009c);
            //    }
            //}
            #endregion

            return JudgementTestItem(ti);
        }
         
    }
}