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
                
                var localTemp = temps.tempStr;
                LogState(LogType.Info, "Ambient Temp:" + localTemp);
                ti.Value_ = localTemp - inlet;
            }
            return JudgementTestItem(ti);
        }


        public bool PackVoltage(TestItem ti)
        {
            isProcessingUI(ti);

            packVolt_cycler = be_a = be_b = be_c = be_d = be_e = be_f = be_g = be_h = 0;
            //id 141 cjt byte

            ClearDTC();
            Thread.Sleep(500);
            Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
            LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");

            Thread.Sleep(1000);

            if (!Hybrid_Instru_CAN.bmsList.ContainsKey("141h"))
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }

            var arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");

            int retryCnt = 10;
            while (arr[0].Substring(0,1) != "4")//0x80 == 128
            {
                ClearDTC();
                Thread.Sleep(500);
                Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
                LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");
                arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                LogState(LogType.Info, "Read 141 { "+Hybrid_Instru_CAN.bmsList["141h"].Data+"}");
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

            Thread.Sleep(1000);
            var rList = new List<string>();
            GetToDID_singleData(0xD9, 0x30, "10 ", true, out rList);

            if (rList != null)
            {
                relays.RelayOn("IDO_32");
                Thread.Sleep(300);
                if (keysight != null && keysight.socket.Connected)
                {
                    packVolt_cycler = keysight.TrySend("MEAS:VOLT:DC?\n");
                }
                relays.RelayOff("IDO_32");
                Thread.Sleep(300);
                //packVolt_cycler = 316.23;

                be_a = Convert.ToInt32(rList[4] + rList[5], 16) / 8.10891;
                be_b = Convert.ToInt32(rList[6] + rList[7], 16) / 8.10891;
                be_c = Convert.ToInt32(rList[8] + rList[9], 16) / 8.10891;
                be_d = Convert.ToInt32(rList[10] + rList[11], 16) / 8.10891;
                be_e = Convert.ToInt32(rList[12] + rList[13], 16) / 8.10891;
                be_f = Convert.ToInt32(rList[14] + rList[15], 16) / 8.10891;
                be_g = (Convert.ToInt32(rList[16] + rList[17], 16) / 4.07463) - 503;
                be_h = Convert.ToInt32(rList[18] + rList[19], 16) / 409.5;

                ti.Value_ = packVolt_cycler;
            }
            else
            {
                ti.Value_ = null;
            }

            Hybrid_Instru_CAN._635List.Clear();
            Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });
            LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");

            Thread.Sleep(50);
            if (Hybrid_Instru_CAN._635List.Count == 1 && Hybrid_Instru_CAN._635List[0].Contains("02 11"))
            {
                LogState(LogType.Success, "CONTACTOR_OPEN");
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Success, "Recved data - " + fe009c);
                }
            }
            else
            {
                LogState(LogType.Fail, "CONTACTOR_OPEN");
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Fail, "Recved data - " + fe009c);
                }
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