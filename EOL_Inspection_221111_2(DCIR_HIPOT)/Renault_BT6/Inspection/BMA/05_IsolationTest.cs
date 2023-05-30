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

     
     
    }
}