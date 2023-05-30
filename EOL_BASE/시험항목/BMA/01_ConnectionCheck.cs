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
        public bool EoLVersionCheck(TestItem ti)
        {
            isProcessingUI(ti);

            ti.Value_ = "1";
            ti.Result = "PASS";

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (isManual)
                    blinder.Visibility = System.Windows.Visibility.Hidden;
            }));

            isProcessingFlag = false;
            return true;
        }

        /// <summary>
        /// OBD- TO ERAD-
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck1(TestItem ti)
        {
            isProcessingUI(ti);

            this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    modelList[selectedIndex].LotId = lotTb.Text;
                }));

            relays.RelayOn("IDO_4");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ =  keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_4");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// OBD- TO CIDD-
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck14(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_5");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_5");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// OBD- TO HVCH-
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck2(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_6");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_6");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// CIDD+ TO ERAD+
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck3(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_7");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_7");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// CIDD+ TO OBC+
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck4(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_8");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_8");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// CIDD+ TO HVCH+
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck5(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_9");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_9");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }


        public bool PackVoltageInMSD(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_10");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:VOLT:DC?\n");
            }
            relays.RelayOff("IDO_10");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// LINK+ TO ERAD+ (MSD)
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck6(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_11");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_11");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// LINK- TO ERAD- (MSD)
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck7(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_12");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_12");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// HVCH SHIELD TO CHASSIS
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck8(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_13");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_13");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// CIDD+ SHIELD TO CHASSIS
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck9(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_15");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_15");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// CIDD- SHIELD TO CHASSIS
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck10(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_16");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_16");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// ERAD+ SHIELD TO CHASSIS
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck11(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_17");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_17");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// ERAD- SHIELD TO CHASSIS
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck12(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_18");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_18");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

        /// <summary>
        /// OBD SHIELD TO CHASSIS
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool ConnectionCheck13(TestItem ti)
        {
            isProcessingUI(ti);
            relays.RelayOn("IDO_14");
            Thread.Sleep(300);
            if (keysight != null && keysight.socket.Connected)
            {
                ti.Value_ = keysight.TrySend("MEAS:RES?\n");
            }
            relays.RelayOff("IDO_14");
            Thread.Sleep(300);
            return JudgementTestItem(ti);
        }

    }
}