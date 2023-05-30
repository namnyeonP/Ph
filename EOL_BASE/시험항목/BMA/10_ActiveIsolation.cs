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

        public bool ActiveIsolation1(TestItem ti)
        {
            isProcessingUI(ti);

            relays.RelayOn("IDO_22");//becm(-) , Chassis short 
            Thread.Sleep(300);
            if (SetDefaultMode())
            {
                Thread.Sleep(1000);
                if (SetExtendedMode())
                {
                    ClearDTC();
                    Thread.Sleep(500);
                    Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
                    LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");

                    Thread.Sleep(1000);

                    var arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");

                    int retryCnt = 10;
                    while (arr[0].Substring(0, 1) != "4")//0x80 == 128
                    {
                        ClearDTC();
                        Thread.Sleep(500);
                        Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });//close contactor
                        LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");
                        arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");

                        retryCnt--;

                        if (retryCnt == 0)
                        {
                            ti.Value_ = _VALUE_NOT_MATCHED;

                            Hybrid_Instru_CAN._635List.Clear();
                            Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });//open contactor
                            LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");
                            Thread.Sleep(50);
                            foreach (var fe009c in Hybrid_Instru_CAN._635List)
                            {
                                LogState(LogType.Success, "Recved data - " + fe009c);
                            }
                            relays.RelayOff("IDO_22");//becm(-) , Chassis short 
                            Thread.Sleep(300);
                            return JudgementTestItem(ti);
                        }
                    }

                    relays.RelayOn("IDO_29");
                    Thread.Sleep(300);

                    //send 20sec
                    var rList = new List<string>();
                    for (int i = 0; i < 19; i++)
                    {
                        rList = new List<string>();
                        GetToDID_singleData(0x48, 0xC3, "07 62", false, out rList);//sleep 50ms
                        Thread.Sleep(950);
                    }

                    rList = new List<string>();
                    GetToDID_singleData(0x48, 0xC3, "07 62", false, out rList);//sleep 50ms
                    ti.Value_ = Convert.ToInt32(rList[3] + rList[4] + rList[5] + rList[6], 16) * 0.001;//kohm

                    relays.RelayOff("IDO_29");
                    Thread.Sleep(300);

                    Hybrid_Instru_CAN._635List.Clear();
                    Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });//open contactor
                    LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");
                    Thread.Sleep(500);

                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Success, "Recved data - " + fe009c);
                    }

                    if (!SetDefaultMode())
                    {
                        ti.Value_ = _NOT_MODE_CHANGED;
                    }
                }
                else
                {
                    ti.Value_ = _NOT_MODE_CHANGED;
                }
            }
            else
            {
                ti.Value_ = _NOT_MODE_CHANGED;
            }

            relays.RelayOff("IDO_22");//becm(-) , Chassis short 
            Thread.Sleep(300);

            return JudgementTestItem(ti);
        }


        public bool ActiveIsolation2(TestItem ti)
        {
            isProcessingUI(ti);

            relays.RelayOn("IDO_22");//becm(-) , Chassis short 
            Thread.Sleep(300);

            if (SetDefaultMode())
            {
                Thread.Sleep(1000);
                if (SetExtendedMode())
                {
                    ClearDTC();
                    Thread.Sleep(500);
                    Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
                    LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");

                    Thread.Sleep(1000);

                    var arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");

                    int retryCnt = 10;
                    while (arr[0].Substring(0, 1) != "4")//0x80 == 128
                    {
                        ClearDTC();
                        Thread.Sleep(500);
                        Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });//close contactor
                        LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");
                        arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");

                        retryCnt--;

                        if (retryCnt == 0)
                        {
                            ti.Value_ = _VALUE_NOT_MATCHED;

                            Hybrid_Instru_CAN._635List.Clear();
                            Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });//open contactor
                            LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");
                            Thread.Sleep(50);
                            foreach (var fe009c in Hybrid_Instru_CAN._635List)
                            {
                                LogState(LogType.Success, "Recved data - " + fe009c);
                            } 
                            relays.RelayOff("IDO_22");//becm(-) , Chassis short 
                            Thread.Sleep(300);
                            return JudgementTestItem(ti);
                        }
                    }
                    
                    //send 20sec
                    var rList = new List<string>();
                    for (int i = 0; i < 19; i++)
                    {
                        rList = new List<string>();
                        GetToDID_singleData(0x48, 0xC3, "07 62", false, out rList);//sleep 50ms
                        Thread.Sleep(950);
                    }

                    rList = new List<string>();
                    GetToDID_singleData(0x48, 0xC3, "07 62", false, out rList);//sleep 50ms
                    ti.Value_ = Convert.ToInt32(rList[3] + rList[4] + rList[5] + rList[6], 16) * 0.000001;//Mohm
                    
                    Hybrid_Instru_CAN._635List.Clear();
                    Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });//open contactor
                    LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");
                    Thread.Sleep(500);

                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Success, "Recved data - " + fe009c);
                    }

                    if (!SetDefaultMode())
                    {
                        ti.Value_ = _NOT_MODE_CHANGED;
                    }
                }
                else
                {
                    ti.Value_ = _NOT_MODE_CHANGED;
                }
            }
            else
            {
                ti.Value_ = _NOT_MODE_CHANGED;
            } 
            relays.RelayOff("IDO_22");//becm(-) , Chassis short 
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }


        public bool ActiveIsolation3(TestItem ti)
        {
            isProcessingUI(ti); 
            relays.RelayOn("IDO_22");//becm(-) , Chassis short 
            Thread.Sleep(300);
            if (SetDefaultMode())
            {
                Thread.Sleep(1000);
                if (SetExtendedMode())
                {
                    ClearDTC();
                    Thread.Sleep(500);
                    Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });
                    LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");

                    Thread.Sleep(1000);

                    var arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");

                    int retryCnt = 10;
                    while (arr[0].Substring(0, 1) != "4")//0x80 == 128
                    {
                        ClearDTC();
                        Thread.Sleep(500);
                        Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 });//close contactor
                        LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x18, 0x00 }");
                        arr = Hybrid_Instru_CAN.bmsList["141h"].Data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        LogState(LogType.Info, "Read 141 { " + Hybrid_Instru_CAN.bmsList["141h"].Data + "}");

                        retryCnt--;

                        if (retryCnt == 0)
                        {
                            ti.Value_ = _VALUE_NOT_MATCHED;

                            Hybrid_Instru_CAN._635List.Clear();
                            Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });//open contactor
                            LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");
                            Thread.Sleep(50);
                            foreach (var fe009c in Hybrid_Instru_CAN._635List)
                            {
                                LogState(LogType.Success, "Recved data - " + fe009c);
                            } 
                            relays.RelayOff("IDO_22");//becm(-) , Chassis short 
                            Thread.Sleep(300);
                            return JudgementTestItem(ti);
                        }
                    }

                    relays.RelayOn("IDO_28");
                    Thread.Sleep(300);

                    //send 20sec
                    var rList = new List<string>();
                    for (int i = 0; i < 19; i++)
                    {
                        rList = new List<string>();
                        GetToDID_singleData(0x48, 0xC3, "07 62", false, out rList);//sleep 50ms
                        Thread.Sleep(950);
                    }

                    rList = new List<string>();
                    GetToDID_singleData(0x48, 0xC3, "07 62", false, out rList);//sleep 50ms
                    ti.Value_ = Convert.ToInt32(rList[3] + rList[4] + rList[5] + rList[6], 16) * 0.001;//kohm

                    relays.RelayOff("IDO_28");
                    Thread.Sleep(300);

                    Hybrid_Instru_CAN._635List.Clear();
                    Hybrid_Instru_CAN.SendToCAN("191", new byte[] { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 });//open contactor
                    LogState(LogType.Info, "Send 191 { 0x3F, 0xFE, 0x00, 0x80, 0x00, 0x00, 0x08, 0x00 }");
                    Thread.Sleep(500);

                    foreach (var fe009c in Hybrid_Instru_CAN._635List)
                    {
                        LogState(LogType.Success, "Recved data - " + fe009c);
                    }

                    if (!SetDefaultMode())
                    {
                        ti.Value_ = _NOT_MODE_CHANGED;
                    }
                }
                else
                {
                    ti.Value_ = _NOT_MODE_CHANGED;
                }
            }
            else
            {
                ti.Value_ = _NOT_MODE_CHANGED;
            } 
            relays.RelayOff("IDO_22");//becm(-) , Chassis short 
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }
    }
}