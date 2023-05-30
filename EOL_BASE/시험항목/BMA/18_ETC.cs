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
        public bool Welded_ERAD_Plus_To_Chassis(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_37");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                keysight.TrySend("MEAS:CAP?\n");
                ti.Value_ = keysight.TrySend("MEAS:VOLT:DC?\n");
            }
            relays.RelayOff("IDO_37");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }


        public bool Welded_ERAD_Minus_To_Chassis(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_38");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                keysight.TrySend("MEAS:CAP?\n");
                ti.Value_ = keysight.TrySend("MEAS:VOLT:DC?\n");
            }
            relays.RelayOff("IDO_38");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }


        public bool Welded_ERAD_Plus_To_Minus(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_32");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                keysight.TrySend("MEAS:CAP?\n");
                ti.Value_ = keysight.TrySend("MEAS:VOLT:DC?\n");
            }
            relays.RelayOff("IDO_32");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }


        public bool Cap_ERAD_Plus_To_Chassis(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_37");
            Thread.Sleep(300);

            //range 1u setting
            //request to shin

            if (keysight != null && keysight.socket.Connected)
            {
                var rst = keysight.TrySend("MEAS:CAP? 1 uF\n") * 1000000000;
                ti.Value_ = Math.Truncate(rst);
            }
            relays.RelayOff("IDO_37");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }


        public bool Cap_ERAD_Minus_To_Chassis(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_38");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                var rst = keysight.TrySend("MEAS:CAP? 1 uF\n") * 1000000000;
                ti.Value_ = Math.Truncate(rst);
            }
            relays.RelayOff("IDO_38");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }


        public bool Volt_Dev_1(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = null;
            return JudgementTestItem(ti);
        }


        public bool Volt_Dev_2(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = null;
            return JudgementTestItem(ti);
        }


        public bool Volt_Dev_3(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = null;
            return JudgementTestItem(ti);
        }


        public bool Volt_Dev_4(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = null;
            return JudgementTestItem(ti);
        }


        public bool Volt_Dev_5(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = null;
            return JudgementTestItem(ti);
        }


        public bool Volt_Dev_6(TestItem ti)
        {
            isProcessingUI(ti);
            ti.Value_ = null;
            return JudgementTestItem(ti);
        }


        

    }
}