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
        public bool SetDefaultMode()
        {
            this.Hybrid_Instru_CAN.ExtendedMsg = false;
            LogState(LogType.Info, "[MODE CHANGE] Default Mode");
            var rList = new List<string>();
            var rst = GetToDID_singleData(0x01, 0x00, "06 50", false, out rList,0x02,0x10);

            if (rst)
            {
                LogState(LogType.Success, "[MODE CHANGE] Default Mode");
                return true;
            }
            else
            {
                LogState(LogType.Fail, "[MODE CHANGE] Default Mode");
                return rst;
            }
        }

        public bool SetDiagSession()
        {
            //0x735, 8, new byte[] { 0x02, 0x10, 0x02, 0, 0, 0, 0, 0 });
            LogState(LogType.Info, "[MODE CHANGE] Diag Session Mode");
            var rList = new List<string>();
            var rst = GetToDID_singleData(0x02, 0x00, " ", false, out rList, 0x02, 0x10);

            if (rst)
            {
                LogState(LogType.Success, "[MODE CHANGE] Diag Session Mode");
                this.Hybrid_Instru_CAN.ExtendedMsg = true;
                return true;
            }
            else
            {
                LogState(LogType.Fail, "[MODE CHANGE] Diag Session Mode");
                return rst;
            }

        }

        public bool SetExtendedMode()
        {
            LogState(LogType.Info, "[MODE CHANGE] Extended Mode");
            var rList = new List<string>();
            var rst = GetToDID_singleData(0x03, 0x00, "06 50", false, out rList, 0x02, 0x10);

            if (rst)
            {
                LogState(LogType.Success, "[MODE CHANGE] Extended Mode");
                this.Hybrid_Instru_CAN.ExtendedMsg = true;
                return true;
            }
            else
            {
                LogState(LogType.Fail, "[MODE CHANGE] Extended Mode");
                return rst;
            }
        }

        public bool SetSWReset()
        {
            LogState(LogType.Info, "[SW RESET]");
            Hybrid_Instru_CAN.SendToCAN("735", new byte[] { 0x02, 0x11, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 });
            LogState(LogType.Info, "Send 735 { 0x02, 0x11, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 }");
            return true;
        }

        public bool SetDisable_Isolation_Detect()
        {
            LogState(LogType.Info, "[MODE CHANGE] Disable_Isolation_Detect");
            var rList = new List<string>();
            var rst = GetToDID_singleData(0x01, 0x40, "05 71", false, out rList, 0x05, 0x31,0x2F,0x01);

            if (rst)
            {
                LogState(LogType.Success, "[MODE CHANGE] Disable_Isolation_Detect");
                return true;
            }
            else
            {
                LogState(LogType.Fail, "[MODE CHANGE] Disable_Isolation_Detect");
                return rst;
            }
        }

        public bool SetTurn_Isolation_Meas_Circuit_Off()
        {
            LogState(LogType.Info, "[MODE CHANGE] Turn_Isolation_Meas_Circuit_Off");
            var rList = new List<string>();
            var rst = GetToDID_singleData(0x01, 0xDC, "05 71", false, out rList, 0x04, 0x31);

            if (rst)
            {
                LogState(LogType.Success, "[MODE CHANGE] Turn_Isolation_Meas_Circuit_Off");
                return true;
            }
            else
            {
                LogState(LogType.Fail, "[MODE CHANGE] Turn_Isolation_Meas_Circuit_Off");
                return rst;
            }
        }

        public bool SetPositive_Contactor_Close()
        {
            LogState(LogType.Info, "[MODE CHANGE] Positive_Contactor_Close");
            var rList = new List<string>();
            var rst = GetToDID_singleData(0x01, 0x40, "05 71", false, out rList, 0x06, 0x31,0x11,0x01,0x01);

            if (rst)
            {
                LogState(LogType.Success, "[MODE CHANGE] Positive_Contactor_Close");
                return true;
            }
            else
            {
                LogState(LogType.Fail, "[MODE CHANGE] Positive_Contactor_Close");
                return rst;
            }
        }

        public bool SetPositive_Contactor_Open()
        {
            LogState(LogType.Info, "[MODE CHANGE] Positive_Contactor_Open");
            var rList = new List<string>();
            var rst = GetToDID_singleData(0x01, 0x40, "05 71", false, out rList, 0x06, 0x31, 0x11, 0x02);

            if (rst)
            {
                LogState(LogType.Success, "[MODE CHANGE] Positive_Contactor_Open");
                return true;
            }
            else
            {
                LogState(LogType.Fail, "[MODE CHANGE] Positive_Contactor_Open");
                return rst;
            }
        }

        public bool SetNegative_Contactor_Close()
        {
            LogState(LogType.Info, "[MODE CHANGE] Negative_Contactor_Close");
            var rList = new List<string>();
            var rst = GetToDID_singleData(0x01, 0x40, "05 71", false, out rList, 0x06, 0x31, 0x11, 0x02, 0x01);

            if (rst)
            {
                LogState(LogType.Success, "[MODE CHANGE] Negative_Contactor_Close");
                return true;
            }
            else
            {
                LogState(LogType.Fail, "[MODE CHANGE] Negative_Contactor_Close");
                return rst;
            }
        }

        public bool SetNegative_Contactor_Open()
        {
            LogState(LogType.Info, "[MODE CHANGE] Negative_Contactor_Open");
            var rList = new List<string>();
            var rst = GetToDID_singleData(0x01, 0x40, "05 71", false, out rList, 0x06, 0x31, 0x11, 0x02);

            if (rst)
            {
                LogState(LogType.Success, "[MODE CHANGE] Negative_Contactor_Open");
                return true;
            }
            else
            {
                LogState(LogType.Fail, "[MODE CHANGE] Negative_Contactor_Open");
                return rst;
            }
        }

        public bool IR_ERAD_Plus(TestItem ti)
        {
            isProcessingUI(ti);
            Thread.Sleep(3000);
            if (SetDefaultMode())
            {
                Thread.Sleep(1000);
                SetSWReset();
                Thread.Sleep(1000);
                if (SetDefaultMode())
                {
                    Thread.Sleep(1000);
                    if (SetExtendedMode())
                    {
                        this.Hybrid_Instru_CAN.SendToCAN("7FF", new byte[] { 0x02, 0x3E, 0x0, 0xFF, 0x0, 0x0, 0x0, 0x0 });
                        Thread.Sleep(1000);
                        if (SetDisable_Isolation_Detect())
                        {
                            Thread.Sleep(1000);
                            if (SetTurn_Isolation_Meas_Circuit_Off())
                            {
                                Thread.Sleep(1000);

                                SetPositive_Contactor_Close();

                                Thread.Sleep(1000);

                                var _613 = Hybrid_Instru_CAN.GetBMSData("613", 0, 8);

                                int retryCnt = 5;
                                while (_613 != "128")//0x80 == 128
                                {
                                    ClearDTC();
                                    Thread.Sleep(500);
                                    SetPositive_Contactor_Close();
                                    Thread.Sleep(500);
                                    _613 = Hybrid_Instru_CAN.GetBMSData("613", 0, 8);

                                    retryCnt--;

                                    if (retryCnt == 0)
                                    {
                                        ti.Value_ = _VALUE_NOT_MATCHED;
                                        return JudgementTestItem(ti);
                                    }
                                }


                                relays.RelayOn("IDO_40");
                                Thread.Sleep(5000);//5sec delay
                                var cap = 0.0;
                                if (chroma.GetCapacitance(3, 1, out cap) == 0 && (cap * 1000000000) > 1)
                                {
                                    LogState(LogType.Info, "CAP :" + (cap * 1000000000).ToString() + "nF");
                                    var res = 0.0;
                                    chroma.GetResistance(3, 1, out res);

                                    if (res > 9.9E+30)
                                    {
                                        res = 99999;
                                        ti.Value_ = res;
                                    }
                                    else
                                        ti.Value_ = res / 1000000;
                                }
                                else
                                {
                                    ti.Value_ = "CONNECT_FAIL_" + (cap * 1000000000).ToString() + "nF";
                                }
                                relays.RelayOff("IDO_40");
                                Thread.Sleep(500);

                                if (!SetPositive_Contactor_Open())
                                {
                                    ti.Value_ = _NOT_POS_MSG;
                                }

                                Thread.Sleep(1000);

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
            }

            return JudgementTestItem(ti);
        }


        public bool IR_ERAD_Minus(TestItem ti)
        {
            isProcessingUI(ti);

            if (SetDefaultMode())
            {
                Thread.Sleep(1000);
                SetSWReset();
                Thread.Sleep(1000);
                if (SetDefaultMode())
                {
                    Thread.Sleep(1000);
                    if (SetExtendedMode())
                    {
                        Thread.Sleep(1000);
                        if (SetDisable_Isolation_Detect())
                        {
                            Thread.Sleep(1000);
                            if (SetTurn_Isolation_Meas_Circuit_Off())
                            {
                                Thread.Sleep(1000);

                                SetNegative_Contactor_Close();

                                Thread.Sleep(1000);

                                var _613 = Hybrid_Instru_CAN.GetBMSData("613", 0, 8);

                                int retryCnt = 5;
                                while (_613 != "32")//0x20 == 32
                                {
                                    ClearDTC();
                                    Thread.Sleep(1000);
                                    SetPositive_Contactor_Close();
                                    Thread.Sleep(1000);
                                    _613 = Hybrid_Instru_CAN.GetBMSData("613", 0, 8);

                                    retryCnt--;

                                    if (retryCnt == 0)
                                    {
                                        ti.Value_ = _VALUE_NOT_MATCHED;
                                        return JudgementTestItem(ti);
                                    }
                                }

                                relays.RelayOn("IDO_39");
                                Thread.Sleep(5000);//5sec delay
                                var cap = 0.0;
                                if (chroma.GetCapacitance(3, 2, out cap) == 0 && (cap * 1000000000) > 1)
                                {
                                    LogState(LogType.Info, "CAP :" + (cap * 1000000000).ToString() + "nF");
                                    var res = 0.0;
                                    chroma.GetResistance(3, 2, out res);

                                    if (res > 9.9E+30)
                                    {
                                        res = 99999;
                                        ti.Value_ = res;
                                    }
                                    else
                                        ti.Value_ = res / 1000000;
                                }
                                else
                                {
                                    ti.Value_ = "CONNECT_FAIL_" + (cap * 1000000000).ToString() + "nF";
                                }
                                relays.RelayOff("IDO_39");
                                Thread.Sleep(500);

                                if (!SetNegative_Contactor_Open())
                                {
                                    ti.Value_ = _NOT_POS_MSG;
                                }
                                Thread.Sleep(1000);
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
            }

            return JudgementTestItem(ti);
        }

    }
}