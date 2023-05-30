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
        void becm_powerOn()
        {
            //var instanceThread = new Thread(() =>
            //    {
            //        while (true)
            //        {
            //            Hybrid_Instru_CAN.SendToCAN("522", new byte[] { 0x22, 0x40, 0x0, 0xFF, 0x0, 0x0, 0x0, 0x0 });
            //            Thread.Sleep(20);
            //        }
            //    });
            //instanceThread.Start();

            Hybrid_Instru_CAN.IsSendMsg = true;
            Thread.Sleep(500);
            relays.RelayOn("IDO_19");            
            relays.RelayOn("IDO_20");
            Thread.Sleep(300);
            becm_power.Set_OUTP_On();
            Thread.Sleep(300);
            //instanceThread.Abort();
        }

        void becm_powerOff(double wait)
        {
            Thread.Sleep(500);
            relays.RelayOff("IDO_19");
            Hybrid_Instru_CAN.IsSendMsg = false;

            LogState(LogType.Info, "Wait to Sleep " + wait / 1000 + "sec");//, Remain : " + wait * 0.001, null, true, false);
            while (wait > 1)
            {
                LogState(LogType.Info, "Remain : "+wait*0.001,null,true,false);
                wait -= 100;
                Thread.Sleep(100);
            }
            //relays.RelayOff("IDO_20");
            //Thread.Sleep(300);
            //becm_power.Set_OUTP_Off();
        }

        /// <summary>
        /// lin pump : IDO_41
        /// pwm pump : IDO_42 - todo
        /// </summary>
        void pump_powerOn()
        {
            relays.RelayOn("IDO_41");
            Thread.Sleep(300);
            relays.RelayOn("IDO_27");
            Thread.Sleep(300);
            pump_power.Set_OUTP_On();
            Thread.Sleep(500);
        }


        /// <summary>
        /// lin pump : IDO_41
        /// pwm pump : IDO_42 - todo
        /// </summary>
        void pump_powerOff()
        {
            pump_power.Set_OUTP_Off();
            Thread.Sleep(500);
            relays.RelayOff("IDO_27");
            Thread.Sleep(300);
            relays.RelayOff("IDO_41");
            Thread.Sleep(300);
        }

        public bool TurnBMSCheck1(TestItem ti)
        {
            isProcessingUI(ti);

            //"CAN request ID 735 : 검사기 => BECM, CAN response ID 635 : BECM => 검사기
            //0x56    00 0B 00 00 41 00 00 00  :  BECM에 전원이 들어가기 전부터 10ms 단위로 메시지 전송, BECM on시 항상 전송 되야 함
            //0x522   22 40 00 FF 00 00 00 00 :  BECM에 전원이 들어가기 전부터 20ms 단위로 메시지 전송, BECM on시 항상 전송 되야 함
            //0x7FF   02 3E 80 00 00 00 00 00 : Extended EOL 진입 후 Extended EOL 유지위해 2s 단위로 메세지 전송
            //Normal sleep : CPSR 제거 후 모든 메세지 전송 중지 => 40s 후 Bvatt 제거"		
				

            hvil1_rtn = 0.0;
            hvil2_rtn = 0.0;
            hvil3_rtn = 0.0;


            //becm_powerOn();

            var instanceThread = new Thread(() =>
            {
                while (true)
                {
                    Hybrid_Instru_CAN.SendToCAN("522", new byte[] { 0x22, 0x40, 0x0, 0xFF, 0x0, 0x0, 0x0, 0x0 });
                    Thread.Sleep(20);
                }
            });
            instanceThread.Start();
            

            Thread.Sleep(500);
            relays.RelayOn("IDO_19");
            relays.RelayOn("IDO_20");
            Thread.Sleep(300);
            becm_power.Set_OUTP_On();
            Thread.Sleep(500);

            var rList = new List<string>();
            if (SetExtendedMode())
            {
                #region Memory Init / HW Reset

                Thread.Sleep(100);

                //memory init
                // 04 31 01 40 34
                rList = new List<string>();
                LogState(LogType.Info, "Request Memory INIT");
                if (GetToDID_singleData(0x01, 0x40, " ", false, out rList, 0x04, 0x31, 0x34))
                {
                    Thread.Sleep(100);

                    //HW Reset
                    // 02 11 01
                    rList = new List<string>();
                    LogState(LogType.Info, "Request HW RESET");
                    GetToDID_singleData(0x01, 0x0, " ", false, out rList, 0x02, 0x11);
                }
                #endregion

                LogState(LogType.Info, "Wait 30000 msec");        
                Thread.Sleep(30000);

                //if (SetExtendedMode())
                //{
                    rList = new List<string>();
                    if (GetToDID_singleData(0x48, 0x95, "03 ", false, out rList, 0x04, 0x2E, 0x55))
                    {
                        Thread.Sleep(100);
                        rList = new List<string>();
                        if (GetToDID_singleData(0x48, 0x95, "04 ", false, out rList))
                        {
                            LogState(LogType.Success, "DID 4855 - Result:" + rList[3]);

                        }
                        else
                        {
                            ti.Value_ = _VALUE_NOT_MATCHED;
                            instanceThread.Abort();
                            return JudgementTestItem(ti);
                        }
                    }
                    else
                    {
                        ti.Value_ = _VALUE_NOT_MATCHED;
                        instanceThread.Abort();
                        return JudgementTestItem(ti);
                    }
                //}
                //else
                //{
                //    ti.Value_ = _NOT_MODE_CHANGED;
                //    return JudgementTestItem(ti);
                //}
            }
            else
            {
                ti.Value_ = _NOT_MODE_CHANGED;
                instanceThread.Abort();
                return JudgementTestItem(ti);
            }

            Thread.Sleep(500);


            //HvSysSts Can Signal check

            //contactor close
            Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
            LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");

            Thread.Sleep(1000);
            if (!Hybrid_Instru_CAN.bmsList.ContainsKey("141h"))
            {
                ti.Value_ = _DEVICE_NOT_READY;
                instanceThread.Abort();
                return JudgementTestItem(ti);
            }

            var self_integ = Hybrid_Instru_CAN.GetBMSData("141", 2, 2,false);
            LogState(LogType.Info, "Read 141(Contactor Status) { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");
            if (self_integ == "0")
            {
                //contactor open
                Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });
                LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");

                instanceThread.Abort();
                ClearDTC();
                becm_powerOff(40000);
                becm_powerOn();
            }
            else if (self_integ == "1")
            {
                int cnt = 90;
                while (self_integ != "2")
                {
                    self_integ = Hybrid_Instru_CAN.GetBMSData("141", 2, 2);
                    Thread.Sleep(1000);

                    if (cnt == 0)
                    {
                        ti.Value_ = "INTERGRIY_FAIL";
                        return JudgementTestItem(ti);

                    }
                    cnt--;
                }
                //contactor open
                Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });
                LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");

                instanceThread.Abort();
                ClearDTC();
                becm_powerOff(40000);
                becm_powerOn();
            }

            //ClearDTC();
            //Thread.Sleep(100);

            //Hybrid_Instru_CAN.IsSendMsg = true;

            Thread.Sleep(1000);


            relays.RelayOn("IDO_34");
            relays.RelayOn("IDO_35");
            relays.RelayOn("IDO_36");
            Thread.Sleep(1500);

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

                rList = new List<string>();
                foreach (var fe009c in Hybrid_Instru_CAN._635List)
                {
                    LogState(LogType.Success, "Recved data - " + fe009c);
                    var arr = fe009c.Split(new string[]{" "}, StringSplitOptions.RemoveEmptyEntries);
                    for(int i=1;i<arr.Length;i++)
                    {
                        rList.Add(arr[i]);
                    }
                }

                hvil1_rtn = (Convert.ToInt32(rList[28] + rList[29], 16) / 134.162) * 1000;//mV

                hvil2_rtn = (Convert.ToInt32(rList[32] + rList[33], 16) / 134.162) * 1000;

                hvil3_rtn = (Convert.ToInt32(rList[36] + rList[37], 16) / 134.162) * 1000;

                ti.Value_ = hvil1_rtn;
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

        double hvil1_rtn = 0.0;
        double hvil2_rtn = 0.0;
        double hvil3_rtn = 0.0;

        double hvil1_rtn_open = 0.0;
        double hvil2_rtn_open = 0.0;
        double hvil3_rtn_open = 0.0;
        public bool TurnBMSCheck2(TestItem ti)
        {
            isProcessingUI(ti);
            
            ti.Value_ = hvil2_rtn;
            
            return JudgementTestItem(ti);
        }


        public bool TurnBMSCheck3(TestItem ti)
        {
            isProcessingUI(ti);
            
            ti.Value_ = hvil3_rtn;            

            return JudgementTestItem(ti);
        }

    }
}