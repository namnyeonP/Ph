using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EOL_BASE
{
    /// <summary>
    /// CyclerLimitSettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CyclerLimitSettingWindow : Window
    {
        MainWindow mw;

        byte cmd_01 = 0x01;//Write Safety Conditions(Voltage)
        byte cmd_02 = 0x02;//Write Safety Conditions(Current, Time)
        byte cmd_11 = 0x11;//Read Safety Conditions(요청할 경우 2개의 데이터를 DSP에서 보냄)

        public CyclerLimitSettingWindow(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;

            this.Closing += CyclerLimitSettingWindow_Closing;
        }

        void CyclerLimitSettingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();

            e.Cancel = true;
        }

        private void SetVoltagesLimit(double voltOver, double voltUnder,bool isShow = false)
        {
            int voltOver_1000 = (int)(voltOver * 1000);
            int voltUnder_1000 = (int)(voltUnder * 1000);

            var vOverBt1 = (byte)(voltOver_1000 >> 16 & 0xFF);
            var vOverBt2 = (byte)(voltOver_1000 >> 8 & 0xFF);
            var vOverBt3 = (byte)(voltOver_1000 >> 0 & 0xFF);

            var vUnderBt1 = (byte)(voltUnder_1000 >> 16 & 0xFF);
            var vUnderBt2 = (byte)(voltUnder_1000 >> 8 & 0xFF);
            var vUnderBt3 = (byte)(voltUnder_1000 >> 0 & 0xFF);
             
            mw.cycler.SendToDSP1("0x10", new byte[]{
                cmd_01, 
                vOverBt1, vOverBt2, vOverBt3, 
                vUnderBt1, vUnderBt2, vUnderBt3, 
                0x0});

            if (isShow)
            {
                this.tbSetVoltageMax.Text = voltOver.ToString();
                this.tbSetVoltageMin.Text = voltUnder.ToString();
            }


            mw.LogState(LogType.Success, "[SEND] CYCLER_SAFETY_LIMIT_SETTING - VOLT_OVER:" +
                voltOver.ToString() + " / VOLT_UNDER:" + voltUnder.ToString());
        }

        private void SetCurrentNTimeout(double currOver, int timeoutSec, bool isShow = false)
        {  
            int currOver_1000 = (int)(currOver * 1000);

            var cOverBt1 = (byte)(currOver_1000 >> 16 & 0xFF);
            var cOverBt2 = (byte)(currOver_1000 >> 8 & 0xFF);
            var cOverBt3 = (byte)(currOver_1000 >> 0 & 0xFF);


            var tOverBt1 = (byte)(timeoutSec >> 16 & 0xFF);
            var tOverBt2 = (byte)(timeoutSec >> 8 & 0xFF);
            var tOverBt3 = (byte)(timeoutSec >> 0 & 0xFF);
             
            mw.cycler.SendToDSP1("0x10", new byte[]{
                cmd_02, 
                cOverBt1, cOverBt2, cOverBt3, 
                tOverBt1, tOverBt2, tOverBt3, 
                0x0});

            if (isShow)
            {
                this.tbSetCurrent.Text = currOver.ToString();
                this.tbSetTimeout.Text = timeoutSec.ToString();
            }
            mw.LogState(LogType.Success, "[SEND] CYCLER_SAFETY_LIMIT_SETTING - CURR_OVER:" +
                currOver.ToString() + " / TIME_OUT(SEC):" + timeoutSec.ToString());
        }

        private bool ReadDataToCycler(out double recvVoltOver, out double recvVoltUnder,
            out double recvCurrOver, out int recvTimeout)
        {
            mw.cycler.SendToDSP1("0x10", new byte[]{
                cmd_11, 
                0x0,0x0,0x0,0x0,0x0,0x0,0x0});

            Thread.Sleep(500);

            if (mw.cycler.id11_10 != "" && mw.cycler.id11_20 != "")
            {
                var starr = mw.cycler.id11_10.Replace(" ", ",").Split(',');

                StringBuilder sb = new StringBuilder();
                sb.Append(starr[1].ToString());
                sb.Append(starr[2].ToString());
                sb.Append(starr[3].ToString());
                //200731 wjs N1 -> N3
                string recvVoltOverBefore = (double.Parse(mw.cycler.binaryToInt(sb.ToString())) * 0.001).ToString("N3");
                recvVoltOver = double.Parse(recvVoltOverBefore);
                //recvVoltOver = (double.Parse(mw.cycler.binaryToInt(sb.ToString())) * 0.001);

                sb = new StringBuilder();
                sb.Append(starr[4].ToString());
                sb.Append(starr[5].ToString());
                sb.Append(starr[6].ToString());
                //200731 wjs N1 -> N3
                string recvVoltUnderBefore = (double.Parse(mw.cycler.binaryToInt(sb.ToString())) * 0.001).ToString("N3");
                recvVoltUnder = double.Parse(recvVoltUnderBefore);
                //recvVoltUnder = (double.Parse(mw.cycler.binaryToInt(sb.ToString())) * 0.001);

                starr = mw.cycler.id11_20.Replace(" ", ",").Split(',');

                sb = new StringBuilder();
                sb.Append(starr[1].ToString());
                sb.Append(starr[2].ToString());
                sb.Append(starr[3].ToString());
                ////200731 wjs N1 -> N3
                //string recvCurrOverBefore = (double.Parse(mw.cycler.binaryToInt(sb.ToString())) * 0.001).ToString("N3");
                //recvCurrOver = double.Parse(recvCurrOverBefore);
                //200804 jhp N3 -> N1
                string recvCurrOverBefore = (double.Parse(mw.cycler.binaryToInt(sb.ToString())) * 0.001).ToString("N1");
                recvCurrOver = double.Parse(recvCurrOverBefore);

                sb = new StringBuilder();
                sb.Append(starr[4].ToString());
                sb.Append(starr[5].ToString());
                sb.Append(starr[6].ToString());

                recvTimeout = (int.Parse(mw.cycler.binaryToInt(sb.ToString())));

                mw.LogState(LogType.Success, "[RECV] CYCLER_SAFETY_LIMIT_SETTING - VOLT_OVER:" +
                    recvVoltOver.ToString() + " / VOLT_UNDER:" + recvVoltUnder.ToString());
                mw.LogState(LogType.Success, "[RECV] CYCLER_SAFETY_LIMIT_SETTING - CURR_OVER:" +
                    recvCurrOver.ToString() + " / TIME_OUT(SEC):" + recvTimeout.ToString());
                return true;
            }
            else
            {
                mw.LogState(LogType.Fail, "[RECV] CYCLER_SAFETY_LIMIT_SETTING");
                recvVoltOver = recvVoltUnder = recvCurrOver = 0.0;
                recvTimeout = 0;
                return false;
            }
        }
          
        public bool SetCyclerLimit(double voltOver, double voltUnder, double currOver, int timeoutSec)
        {
            #region Buffer Clear
            mw.cycler.id11_10 = "";
            mw.cycler.id11_20 = "";
            #endregion

            double recvVoltOver = 0.0;
            double recvVoltUnder = 0.0;
            double recvCurrOver = 0.0;
            int recvTimeout = 0;

            SetVoltagesLimit(voltOver, voltUnder);

            Thread.Sleep(200); 

            SetCurrentNTimeout(currOver, timeoutSec);
              
            Thread.Sleep(200);
             
            if (!ReadDataToCycler(out recvVoltOver, out recvVoltUnder, out recvCurrOver, out recvTimeout))
            {
                return false;
            }

            #region judgement
            if (voltOver == recvVoltOver &&
                voltUnder == recvVoltUnder &&
                currOver == recvCurrOver &&
                timeoutSec == recvTimeout)
            {
                return true;
            }
            else
            {
                return false;
            }
            #endregion
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            switch (cbWriteCmd.SelectedIndex)
            {
                case 0:
                    {
                        try
                        {
                            var param1 = double.Parse(tbSetVoltageMax.Text.ToString());
                            var param2 = double.Parse(tbSetVoltageMin.Text.ToString());

                            var param3 = double.Parse(tbSetCurrent.Text.ToString());
                            var param4 = int.Parse(tbSetTimeout.Text.ToString());

                            SetVoltagesLimit(param1, param2, true);
                            SetCurrentNTimeout(param3, param4, true);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Data is NOT Allowed", "Warning", MessageBoxButton.OK);
                        }
                    }; break; 
                case 1:
                    {
                        double recvVoltOver = 0.0;
                        double recvVoltUnder = 0.0;
                        double recvCurrOver = 0.0;
                        int recvTimeout = 0;

                        var th = new Thread(() =>
                            {
                                if (ReadDataToCycler(out recvVoltOver, out recvVoltUnder, out recvCurrOver, out recvTimeout))
                                {
                                    mw.Dispatcher.Invoke(new Action(() =>
                                        {
                                            tbGetVoltageMax.Text = recvVoltOver.ToString();
                                            tbGetVoltageMin.Text = recvVoltUnder.ToString();
                                            tbGetCurrent.Text = recvCurrOver.ToString();
                                            tbGetTimeout.Text = recvTimeout.ToString();
                                        }));
                                }
                            });
                        th.Start();

                    }; break;
                case 3:
                    {
                    }; break;
            }
        }

    }
}
