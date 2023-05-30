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

        public bool Coolant_Full(TestItem ti)
        {
            isProcessingUI(ti);

            if (SetExtendedMode())
            {
                relays.RelayOn("IDO_30");
                Thread.Sleep(300);
                relays.RelayOn("IDO_24");
                relays.RelayOn("IDO_25");
                Thread.Sleep(1000);

                //rpm 15
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 15) { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00}");
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 15) { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00}");
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 15) { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00}");
                
                string expectVal = "01";
                int errorCnt = 0;
                for (int i = 0; i < 30; i++)
                {
                    var rList = new List<string>();
                    GetToDID_singleData(0xFD, 0xD5, "05 62", false, out rList);
                    if (rList == null)
                    {
                        errorCnt++;
                    }

                    if (rList!=null && rList[4] != expectVal)
                    {
                        errorCnt++;
                    }
                    Thread.Sleep(450);
                }
                relays.RelayOff("IDO_30");
                Thread.Sleep(300);
                relays.RelayOff("IDO_24");
                relays.RelayOff("IDO_25");
                Thread.Sleep(1000);

                if (!SetDefaultMode())
                {
                    ti.Value_ = _NOT_MODE_CHANGED;                    
                }

                if (errorCnt > 10)
                {
                    ti.Value_ = 0;
                }
                else
                {
                    ti.Value_ = 1;
                }

                //rpm 0
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 0) { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00}");
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 0) { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00}");
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 0) { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00}");
            }
            else
            {
                ti.Value_ = _NOT_MODE_CHANGED;
            }
            return JudgementTestItem(ti);
        }


        public bool Coolant_Empty(TestItem ti)
        {
            isProcessingUI(ti);

            if (SetExtendedMode())
            {
                relays.RelayOn("IDO_30");
                Thread.Sleep(300);
                relays.RelayOn("IDO_26");
                relays.RelayOn("IDO_25");
                Thread.Sleep(1000);

                //rpm 15
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 15) { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00}");
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 15) { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00}");
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 15) { 0x02, 0x00, 0x09, 0xC4, 0x05, 0xDC, 0x88, 0x00}");
                
                string expectVal = "00";
                int errorCnt = 0;
                for (int i = 0; i < 30; i++)
                {
                    var rList = new List<string>();
                    GetToDID_singleData(0xFD, 0xD5, "05 62", false, out rList);
                    
                    if (rList!= null && rList[4] != expectVal)
                    {
                        errorCnt++;
                    }
                    Thread.Sleep(450);
                }
                relays.RelayOff("IDO_30");
                Thread.Sleep(300);
                relays.RelayOff("IDO_26");
                relays.RelayOff("IDO_25");
                Thread.Sleep(1000);


                if (errorCnt > 10)
                {
                    ti.Value_ = -1;
                }
                else
                {
                    ti.Value_ = 0;
                }

                if (!SetDefaultMode())
                {
                    ti.Value_ = _NOT_MODE_CHANGED;
                }

                //rpm 0
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 0) { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00}");
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 0) { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00}");
                Hybrid_Instru_CAN.SendToCAN("181", new byte[] { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00 });
                LogState(LogType.Info, "Send 181(RPM 0) { 0x02, 0x00, 0x09, 0xC4, 0x00, 0x00, 0x00, 0x00}");
                Thread.Sleep(1000);                    
            }
            else
            {
                ti.Value_ = _NOT_MODE_CHANGED;
            }
            return JudgementTestItem(ti);
        }


        public bool PumpCheck(TestItem ti)
        {
            isProcessingUI(ti);
            _3way_valve = 0.0;
            if (SetExtendedMode())
            {
                pump_powerOn();
                //pump on
                var rList = new List<string>();
                if (GetToDID_singleData(0x01, 0x40, "05 71", false, out rList, 0x06, 0x31, 0x19, 0x01, 0x32))
                {
                    LogState(LogType.Info, "Wait 5000ms");
                    Thread.Sleep(5000);
                    //pump check
                    rList = new List<string>();
                    GetToDID_singleData(0x48, 0x67, "04 62", false, out rList);
                    ti.Value_ = Convert.ToInt32(rList[3], 16);



                    //3way valve check
                    rList = new List<string>();
                    if (GetToDID_singleData(0x01, 0x40, "05 71", false, out rList, 0x05, 0x31, 0x1B, 0x01))
                    {

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

                            rList = new List<string>();
                            foreach (var fe009c in Hybrid_Instru_CAN._635List)
                            {
                                LogState(LogType.Success, "Recved data - " + fe009c);
                                var arr = fe009c.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 1; i < arr.Length; i++)
                                {
                                    rList.Add(arr[i]);
                                }
                            }
                            _3way_valve = (Convert.ToInt32(rList[52] + rList[53], 16) / 823.116);//A
                        }
                        else
                        {
                            foreach (var fe009c in Hybrid_Instru_CAN._635List)
                            {
                                LogState(LogType.Fail, "Recved data - " + fe009c);
                            }
                            _3way_valve = 0.0;
                        }

                        //Thread.Sleep(500);
                        //rList = new List<string>();
                        //if (GetToDID_singleData(0x01, 0x40, "05 71", false, out rList, 0x05, 0x31, 0x1B, 0x01))
                        //{
                        //    rList = new List<string>();
                        //    if (GetToDID_singleData(0xD9, 0x19, " ", false, out rList))
                        //    {
                        //        var tt = (Convert.ToInt32(rList[3] + rList[4], 16) / 823.116);//A
                        //    }
                        //}
                    }





                    //pump off
                    rList = new List<string>();
                    GetToDID_singleData(0x02, 0x40, "03 7F", false, out rList, 0x04, 0x31, 0x19);

                    int retryCnt = 5;
                    while (rList == null || "7F" != rList[0])//retry pump off
                    {
                        Thread.Sleep(500);
                        rList = new List<string>();
                        GetToDID_singleData(0x02, 0x40, "03 7F", false, out rList, 0x04, 0x31, 0x19);
                        retryCnt--;

                        if (retryCnt == 0)
                        {
                            ti.Value_ = _NOT_NEG_MSG;
                            break;
                        }
                    }
                    Thread.Sleep(500);
                    if (!SetDefaultMode())
                    {
                        ti.Value_ = _NOT_MODE_CHANGED;
                    }
                    Thread.Sleep(500);
                }
                else
                {
                    ti.Value_ = _VALUE_NOT_MATCHED;
                }
                pump_powerOff();
            }
            else
            {
                ti.Value_ = _NOT_MODE_CHANGED;
            }

            return JudgementTestItem(ti);
        }

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