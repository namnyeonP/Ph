using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TPCANHandle = System.UInt16;
using TPCANBitrateFD = System.String;
using TPCANTimestampFD = System.UInt64;
using Peak.Can.Basic;

namespace EOL_BASE.모듈
{
    public class SendCMD
    {
        double duration;

        byte[] sendingByte;

        double voltHighLimit = 0.0;

        public double VoltHighLimit
        {
            get { return voltHighLimit; }
            set { voltHighLimit = value; }
        }
        double voltLowLimit = 0.0;

        public double VoltLowLimit
        {
            get { return voltLowLimit; }
            set { voltLowLimit = value; }
        }

        double currHighLimit = 0.0;

        public double CurrHighLimit
        {
            get { return currHighLimit; }
            set { currHighLimit = value; }
        }

        public byte[] SendingByte
        {
            get { return sendingByte; }
            set { sendingByte = value; }
        }

        public double Duration
        {
            get { return duration; }
            set { duration = value; }
        }


        private double endcase_Current;
        public double Endcase_Current 
        { 
            get { return endcase_Current; }
            set { endcase_Current = value; }
        }

        private double endcase_voltHigh;
        public double Endcase_VoltHigh
        {
            get { return endcase_voltHigh; }
            set { endcase_voltHigh = value; }
        }

        private double endcase_voltLow;
        public double Endcase_VoltLow
        {
            get { return endcase_voltLow; }
            set { endcase_voltLow = value; }
        }


        RECV_OPMode nextOPMode;

        public RECV_OPMode NextOPMode
        {
            get { return nextOPMode; }
            set { nextOPMode = value; }
        }

        OPMode opmode;

        public OPMode Opmode
        {
            get { return opmode; }
            set { opmode = value; }
        }
        CMD cmd;

        public CMD Cmd
        {
            get { return cmd; }
            set { cmd = value; }
        }
        BranchMode pmode;

        public BranchMode Pmode
        {
            get { return pmode; }
            set { pmode = value; }
        }


        double voltage;

        public double Voltage
        {
            get { return voltage; }
            set { voltage = value; }
        }
        double current;

        public double Current
        {
            get { return current; }
            set { current = value; }
        }

        double watt;

        public double Watt
        {
            get { return watt; }
            set { watt = value; }
        }

        int index;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }
    }


    public class CCycler// : CSerialModule
    {

        public Languages Lag;
        MainWindow mw;

        public bool isAlive1 = false;
        public bool isAlive2 = false;

        private bool isClear = false;
        public bool IsClear
        {
            set { isClear = value; }
            get { return isClear; }
        }

        /// <summary>
        /// port=COM1/COM2 / baud=9600 / parity=none / data=8 / stop=1
        /// </summary>
        /// <param name="mw"></param>
        public CCycler(MainWindow mw, string canID)
        {
            if (canID == "")
            {
                return;
            }

            Lag = new Languages();
            this.mw = mw;
            InitializeBasicComponents();
            SetPCAN_BMS1(0, canID);

            //alarm test
            //bmsList.Add("120h", new ReceiveType() { Data = "00 00 FF FF FF FF FF FF" });
            //bmsList.Add("120h", new ReceiveType() { Data = "00 00 10 04 00 04 00 00" });//28335_def_test
            t1 = new Thread(() =>
                {
                    while (true)
                    {
                        tmr_Tick1(this, null);
                        Thread.Sleep(50);
                    }
                });
            t1.Start();
            t2 = new Thread(() =>
            {
                while (true)
                {
                    errorTmr_Tick(this, null);
                    Thread.Sleep(500);
                }
            });
            t2.Start();

            tt = new Thread(() =>
            {
                Thread.Sleep(3000);

                var nowCycler = localDt;
                nowCycler = nowCycler.AddSeconds(5);

                if (DateTime.Now > nowCycler)
                {
                    mw.LogState(LogType.DEVICE_CHECK, "CYCLER - " + canID + " Open Fail");
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_cycler.Background = System.Windows.Media.Brushes.Red;
                    }));
                }
                else
                {
                    mw.LogState(LogType.DEVICE_CHECK, "CYCLER - " + canID + " Open Success");
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_cycler.Background = System.Windows.Media.Brushes.Green;
                    }));
                }
            });
            tt.Start();

            ttt = new Thread(() =>
            {
                Thread.Sleep(8000);

                var nowCycler = localDt;
                nowCycler = nowCycler.AddSeconds(5);

                if (DateTime.Now > nowCycler)
                {
                    mw.LogState(LogType.DEVICE_CHECK, "CYCLER - " + canID + " Open Fail");
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_cycler.Background = System.Windows.Media.Brushes.Red;
                    }));
                }
                else
                {
                    mw.LogState(LogType.DEVICE_CHECK, "CYCLER - " + canID + " Open Success");
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_cycler.Background = System.Windows.Media.Brushes.Green;
                    }));
                }
            });
            ttt.Start();
        }

        public void _Dispose()
        {
            try
            {
                m_ReceiveEvent1Flag = false;
                Thread.Sleep(500);
                m_ReceiveEvent1.Close();
                Thread.Sleep(100);

                if (m_ReadThread1 != null)
                {
                    m_ReadThread1.Abort();
                    Thread.Sleep(100);
                }
                
                t1.Abort();
                Thread.Sleep(100);
                t2.Abort();
                Thread.Sleep(100);

                if (t3 != null)
                {
                    t3.Abort();
                    Thread.Sleep(100);
                }
                
                tt.Abort();
                Thread.Sleep(100);
                ttt.Abort();
                Thread.Sleep(100);
                PCANBasic.Uninitialize(m_PcanHandle1);
                Thread.Sleep(100);
            }
            catch(Exception ec)
            {
                mw.LogState(LogType.Fail, "DisposeAllModule - Cycler", ec);
            }
        }
        Thread t1, t2, t3, tt, ttt;
        List<string> errorGrid = new List<string>();
        List<string> errorDC = new List<string>();
        List<string> errorFault = new List<string>();
        List<string> errorWarning = new List<string>();
        
        string[] localbuffer;
        public bool isCyclerStop = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">절대,상대경로</param>
        private void playSound(string path)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = path;
            player.Load();
            player.Play();
        }

        public DateTime localDt = new DateTime();
        private void tmr_Tick1(object sender, EventArgs e)
        {
            //playSound(@"C:\Users\Administrator\Downloads\Door Bell-SoundBible.com-1986366504.wav");

            var starr = new string[] { "00", "00", "00", "00", "00", "00", "00", "00" };

            if (bmsList.ContainsKey("110h"))
            {
                isAlive1 = true;
                starr = bmsList["110h"].Data.Replace(" ", ",").Split(',');

                localbuffer = starr;
                switch (starr[0].ToString())
                {
                    case "96": cycler1Header = CMD.REST; break;
                    case "94": cycler1Header = CMD.OUTPUT_MC_OFF; break;
                    case "93": cycler1Header = CMD.DISCHARGE; break;
                    case "92": cycler1Header = CMD.CHARGE; break;
                    case "91": cycler1Header = CMD.OUTPUT_MC_ON; break;
                    case "90": cycler1Header = CMD.INPUT_MC_ON; break;
                }
                switch (starr[1].ToString())
                {
                    case "00": cycler1OP = RECV_OPMode.READY; break;
                    case "01": cycler1OP = RECV_OPMode.READY; break;
                    case "02": cycler1OP = RECV_OPMode.READY; break;
                    case "10": cycler1OP = RECV_OPMode.READY_TO_INPUT; break;
                    case "11": cycler1OP = RECV_OPMode.READY_TO_INPUT; break;
                    case "12": cycler1OP = RECV_OPMode.READY_TO_INPUT; break;
                    case "20": cycler1OP = RECV_OPMode.READY_TO_CHARGE; break;
                    case "21": cycler1OP = RECV_OPMode.READY_TO_CHARGE; break;
                    case "22": cycler1OP = RECV_OPMode.READY_TO_CHARGE; break;
                    case "30": cycler1OP = RECV_OPMode.CHARGE_CC; break;
                    case "31": cycler1OP = RECV_OPMode.CHARGE_CC; break;
                    case "32": cycler1OP = RECV_OPMode.CHARGE_CC; break;
                    case "40": cycler1OP = RECV_OPMode.CHARGE_CV; break;//cv mode change cc mode //20170103
                    case "41": cycler1OP = RECV_OPMode.CHARGE_CV; break;
                    case "42": cycler1OP = RECV_OPMode.CHARGE_CV; break;
                    case "50": cycler1OP = RECV_OPMode.DISCHARGE_CC; break;
                    case "51": cycler1OP = RECV_OPMode.DISCHARGE_CC; break;
                    case "52": cycler1OP = RECV_OPMode.DISCHARGE_CC; break;
                    case "60": cycler1OP = RECV_OPMode.DISCHARGE_CV; break;//cv mode change cc mode //20170103 
                    case "61": cycler1OP = RECV_OPMode.DISCHARGE_CV; break;//cv mode 복원 //180719
                    case "62": cycler1OP = RECV_OPMode.DISCHARGE_CV; break;
                    case "70": cycler1OP = RECV_OPMode.CHARGE_CP; break;
                    case "71": cycler1OP = RECV_OPMode.CHARGE_CP; break;
                    case "72": cycler1OP = RECV_OPMode.CHARGE_CP; break;
                    case "80": cycler1OP = RECV_OPMode.DISCHARGE_CP; break;
                    case "81": cycler1OP = RECV_OPMode.DISCHARGE_CP; break;
                    case "82": cycler1OP = RECV_OPMode.DISCHARGE_CP; break;
                    case "90": cycler1OP = RECV_OPMode.COMPLETE; break;
                    case "91": cycler1OP = RECV_OPMode.COMPLETE; break;
                    case "92": cycler1OP = RECV_OPMode.COMPLETE; break;
                }
                string t = cycler1OP.ToString();

                if (t != "NULL")
                {
                    mw.SetMainCState(t);
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(starr[2].ToString());
                sb.Append(starr[3].ToString());
                sb.Append(starr[4].ToString());
                string voltage = binaryToInt(sb.ToString());

                sb = new StringBuilder();
                sb.Append(starr[5].ToString());
                sb.Append(starr[6].ToString());
                sb.Append(starr[7].ToString());

                string current = binaryToInt(sb.ToString());

                this.cycler1voltage = (double.Parse(voltage) * 0.001);
                this.cycler1current = (double.Parse(current) * 0.001);

                if (!isCyclerStop)
                {
                    mw.SetMainVoltage(cycler1voltage.ToString("F3"));

                    if (cycler1Header == CMD.DISCHARGE)
                    {
                        sb = new StringBuilder();
                        sb.Append("-");
                        sb.Append(cycler1current.ToString("F3"));
                        mw.SetMainCurrent(sb.ToString());
                    }
                    else
                        mw.SetMainCurrent(cycler1current.ToString("F3"));

                }
                else
                {
                    mw.SetMainVoltage(0.ToString("F3"));
                }
            }

            string dataWarn = "000000000000";
            string dataGrid = "000000000000";
            string data__Dc = "000000000000";
            string dataFalt = "000000000000";

            if (bmsList.ContainsKey("120h"))
            {
                var splitedData = bmsList["120h"].Data.Replace(" ", ",").Split(',');

                #region Mode Parser
                string binary = ConvertHexToBinary(splitedData[1].ToString());

                #endregion
                #region Alarm Parser
                #region AlarmField

                var data2 = ConvertHexToBinary(splitedData[2].ToString());
                var data3 = ConvertHexToBinary(splitedData[3].ToString());
                var data4 = ConvertHexToBinary(splitedData[4].ToString());
                var data5 = ConvertHexToBinary(splitedData[5].ToString());
                var data6 = ConvertHexToBinary(splitedData[6].ToString());
                var data7 = ConvertHexToBinary(splitedData[7].ToString());

                // 00000000 00000000 11111111 11111111 11111111 11111111 11111111 11111111
                // none     none     w2  w1   w0  g2   g1  g0   d2  d1   d0  f2   f1  f0

                dataWarn = data2.Substring(0, 4) + data2.Substring(4, 4) + data3.Substring(0, 4); // warning 2/1/0
                dataGrid = data3.Substring(4, 4) + data4.Substring(0, 4) + data4.Substring(4, 4); // grid    2/1/0
                data__Dc = data5.Substring(0, 4) + data5.Substring(4, 4) + data6.Substring(0, 4); // dc      2/1/0
                dataFalt = data6.Substring(4, 4) + data7.Substring(0, 4) + data7.Substring(4, 4); // fault   2/1/0
                #endregion


                //에러 처리부, 없으면 리턴
                if (dataWarn == "000000000000" && dataGrid == "000000000000" && data__Dc == "000000000000" && dataFalt == "000000000000")
                {
                    //dataWarn = "";
                    //dataGrid = "";
                    //data__Dc = "";
                    //dataFalt = "";
                }
                else
                {
                    if (bmsList.ContainsKey("121h") && bmsList.ContainsKey("122h"))
                    {
                        var arr = bmsList["121h"].Data.Replace(" ", ",").Split(',');

                        StringBuilder sb = new StringBuilder();
                        sb.Append(arr[0].ToString());
                        sb.Append(arr[1].ToString());
                        AC_R_PHASE_VOLT = Convert.ToInt32(ConvertHexToBinary(sb.ToString()), 2) * 0.1;

                        sb = new StringBuilder();
                        sb.Append(arr[2].ToString());
                        sb.Append(arr[3].ToString());
                        AC_S_PHASE_VOLT = Convert.ToInt32(ConvertHexToBinary(sb.ToString()), 2) * 0.1;

                        sb = new StringBuilder();
                        sb.Append(arr[4].ToString());
                        sb.Append(arr[5].ToString());
                        sb.Append(arr[6].ToString());
                        AC_R_PHASE_CUR = Convert.ToInt32(ConvertHexToBinary(sb.ToString()), 2) * 0.001;

                        arr = bmsList["122h"].Data.Replace(" ", ",").Split(',');

                        sb = new StringBuilder();
                        sb.Append(arr[0].ToString());
                        sb.Append(arr[1].ToString());
                        sb.Append(arr[2].ToString());
                        AC_S_PHASE_CUR = Convert.ToInt32(ConvertHexToBinary(sb.ToString()), 2) * 0.001;

                        sb = new StringBuilder();
                        sb.Append(arr[3].ToString());
                        sb.Append(arr[4].ToString());
                        sb.Append(arr[5].ToString());
                        AC_T_PHASE_CUR = Convert.ToInt32(ConvertHexToBinary(sb.ToString()), 2) * 0.001;
                    }
                    
                    #region DSP_28335_def
                    if (mw.dsp_Type == DSPType.DSP_28335_def)
                    {
                        if (errorWarning.Count == 0)
                        {
                            if (dataWarn.Substring(11, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_def_W001); }
                            if (dataWarn.Substring(10, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_def_W002); }
                            if (dataWarn.Substring(9, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_def_W004); }
                            // 190306 JHT - dataWarn.Substring(8, 1) → dataWarn.Substring(4, 1) 수정 (잘못 매칭되어 있던 내용)
                            if (dataWarn.Substring(4, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_def_W080); }
                            if (dataWarn.Substring(3, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_def_W100); }
                        }

                        if (errorGrid.Count == 0)
                        {
                            if (dataGrid.Substring(11, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_def_G001); }
                            if (dataGrid.Substring(10, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_def_G002); }
                            if (dataGrid.Substring(9, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_def_G004); }
                            if (dataGrid.Substring(7, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_def_G010); }
                            if (dataGrid.Substring(6, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_def_G020); }
                            if (dataGrid.Substring(5, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_def_G040); }
                            if (dataGrid.Substring(3, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_def_G100); }
                            if (dataGrid.Substring(2, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_def_G200); }
                            if (dataGrid.Substring(1, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_def_G400); }
                        }

                        if (errorDC.Count == 0)
                        {
                            if (data__Dc.Substring(11, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_def_D001); }
                            if (data__Dc.Substring(10, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_def_D002); }
                            if (data__Dc.Substring(9, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_def_D004); }
                            if (data__Dc.Substring(7, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_def_D010); }
                            if (data__Dc.Substring(6, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_def_D020); }
                            if (data__Dc.Substring(5, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_def_D040); }
                        }

                        if (errorFault.Count == 0)
                        {
                            if (dataFalt.Substring(11, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F001); }
                            if (dataFalt.Substring(10, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F002); }
                            if (dataFalt.Substring(9, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F004); }
                            if (dataFalt.Substring(8, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F008); }
                            if (dataFalt.Substring(7, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F010); }
                            if (dataFalt.Substring(6, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F020); }
                            if (dataFalt.Substring(5, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F040); }
                            if (dataFalt.Substring(4, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F080); }
                            if (dataFalt.Substring(3, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F100); }
                            if (dataFalt.Substring(2, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F200); }
                            if (dataFalt.Substring(1, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F400); }
                            if (dataFalt.Substring(0, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_def_F800); }
                        }
                    }
                    #endregion

                    #region DSP_28335_meb1
                    if (mw.dsp_Type == DSPType.DSP_28335_meb_1)
                    {
                        if (errorWarning.Count == 0)
                        {
                            if (dataWarn.Substring(11, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_1_W001); }
                            if (dataWarn.Substring(10, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_1_W002); }
                            if (dataWarn.Substring(9, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_1_W004); }
                            if (dataWarn.Substring(8, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_1_W008); }
                            if (dataWarn.Substring(7, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_1_W010); }
                            if (dataWarn.Substring(6, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_1_W020); }
                            if (dataWarn.Substring(5, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_1_W040); }
                            if (dataWarn.Substring(4, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_1_W080); }
                            if (dataWarn.Substring(3, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_1_W100); }
                        }

                        if (errorGrid.Count == 0)
                        {
                            if (dataGrid.Substring(11, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_1_G001); }
                            if (dataGrid.Substring(10, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_1_G002); }
                            if (dataGrid.Substring(9, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_1_G004); }
                            if (dataGrid.Substring(8, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_1_G008); }
                            if (dataGrid.Substring(7, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_1_G010); }
                            if (dataGrid.Substring(6, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_1_G020); }
                            if (dataGrid.Substring(5, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_1_G040); }
                            if (dataGrid.Substring(4, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_1_G080); }
                            if (dataGrid.Substring(3, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_1_G100); }
                            if (dataGrid.Substring(2, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_1_G200); }
                            if (dataGrid.Substring(1, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_1_G400); }
                        }

                        if (errorDC.Count == 0)
                        {
                            if (data__Dc.Substring(11, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_1_D001); }
                            if (data__Dc.Substring(10, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_1_D002); }
                            if (data__Dc.Substring(9, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_1_D004); }
                            if (data__Dc.Substring(8, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_1_D008); }
                            if (data__Dc.Substring(7, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_1_D010); }
                            if (data__Dc.Substring(6, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_1_D020); }
                            if (data__Dc.Substring(5, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_1_D040); }
                            if (data__Dc.Substring(4, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_1_D080); }
                        }

                        if (errorFault.Count == 0)
                        {
                            if (dataFalt.Substring(11, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_1_F001); }
                            if (dataFalt.Substring(10, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_1_F002); }
                            if (dataFalt.Substring(9, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_1_F004); }
                            if (dataFalt.Substring(8, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_1_F008); }
                            if (dataFalt.Substring(7, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_1_F010); }
                            if (dataFalt.Substring(6, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_1_F020); }
                            if (dataFalt.Substring(5, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_1_F040); }
                            if (dataFalt.Substring(4, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_1_F080); }
                            if (dataFalt.Substring(3, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_1_F100); }
                            if (dataFalt.Substring(2, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_1_F200); }
                            if (dataFalt.Substring(1, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_1_F400); }
                        }
                    }
                    #endregion

                    #region DSP_28335_meb234
                    if (mw.dsp_Type == DSPType.DSP_28335_meb_234)
                    {
                        if (errorWarning.Count == 0)
                        {
                            if (dataWarn.Substring(11, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W001); }
                            if (dataWarn.Substring(10, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W002); }
                            if (dataWarn.Substring(9, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W004); }
                            if (dataWarn.Substring(8, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W008); }
                            if (dataWarn.Substring(7, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W010); }
                            if (dataWarn.Substring(6, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W020); }
                            if (dataWarn.Substring(5, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W040); }
                            if (dataWarn.Substring(4, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W080); }
                            if (dataWarn.Substring(3, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W100); }
                            if (dataWarn.Substring(2, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W200); }
                            if (dataWarn.Substring(1, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W400); }
                            if (dataWarn.Substring(0, 1) == "1") { errorWarning.Add(Lag.lSystem120_28335_meb_2_W800); }
                        }

                        if (errorGrid.Count == 0)
                        {
                            if (dataGrid.Substring(11, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_2_G001); }
                            if (dataGrid.Substring(10, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_2_G002); }
                            if (dataGrid.Substring(9, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_2_G004); }
                            if (dataGrid.Substring(8, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_2_G008); }
                            if (dataGrid.Substring(7, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_2_G010); }
                            if (dataGrid.Substring(6, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_2_G020); }
                            if (dataGrid.Substring(5, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_2_G040); }
                            if (dataGrid.Substring(4, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_2_G080); }
                            if (dataGrid.Substring(3, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_2_G100); }
                            if (dataGrid.Substring(2, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_2_G200); }
                            if (dataGrid.Substring(1, 1) == "1") { errorGrid.Add(Lag.lSystem120_28335_meb_2_G400); }
                        }

                        if (errorDC.Count == 0)
                        {
                            if (data__Dc.Substring(11, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_2_D001); }
                            if (data__Dc.Substring(10, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_2_D002); }
                            if (data__Dc.Substring(9, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_2_D004); }
                            if (data__Dc.Substring(8, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_2_D008); }
                            if (data__Dc.Substring(7, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_2_D010); }
                            if (data__Dc.Substring(6, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_2_D020); }
                            if (data__Dc.Substring(5, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_2_D040); }
                            if (data__Dc.Substring(4, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_2_D080); }
                            if (data__Dc.Substring(3, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_2_D100); }
                            if (data__Dc.Substring(2, 1) == "1") { errorDC.Add(Lag.lSystem120_28335_meb_2_D200); }
                        }

                        if (errorFault.Count == 0)
                        {
                            if (dataFalt.Substring(11, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_2_F001); }
                            if (dataFalt.Substring(10, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_2_F002); }
                            if (dataFalt.Substring(9, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_2_F004); }
                            if (dataFalt.Substring(8, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_2_F008); }
                            if (dataFalt.Substring(7, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_2_F010); }
                            if (dataFalt.Substring(6, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_2_F020); }
                            if (dataFalt.Substring(5, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_2_F040); }
                            if (dataFalt.Substring(4, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_2_F080); }
                            if (dataFalt.Substring(3, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_2_F100); }
                            if (dataFalt.Substring(2, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_2_F200); }
                            if (dataFalt.Substring(1, 1) == "1") { errorFault.Add(Lag.lSystem120_28335_meb_2_F400); }
                        }
                    }
                    #endregion

                    #region DSP_28377
                    if (mw.dsp_Type == DSPType.DSP_28377)
                    {
                        if (errorWarning.Count == 0)
                        {
                            if (dataWarn.Substring(11, 1) == "1") { errorWarning.Add(Lag.lSystem120_28377_W001); }
                            if (dataWarn.Substring(10, 1) == "1") { errorWarning.Add(Lag.lSystem120_28377_W002); }
                            if (dataWarn.Substring(9, 1) == "1") { errorWarning.Add(Lag.lSystem120_28377_W004); }
                            if (dataWarn.Substring(8, 1) == "1") { errorWarning.Add(Lag.lSystem120_28377_W008); }
                            if (dataWarn.Substring(7, 1) == "1") { errorWarning.Add(Lag.lSystem120_28377_W010); }
                            if (dataWarn.Substring(6, 1) == "1") { errorWarning.Add(Lag.lSystem120_28377_W020); }
                            if (dataWarn.Substring(5, 1) == "1") { errorWarning.Add(Lag.lSystem120_28377_W040); }
                            if (dataWarn.Substring(4, 1) == "1") { errorWarning.Add(Lag.lSystem120_28377_W080); }
                        }

                        if (errorGrid.Count == 0)
                        {
                            if (dataGrid.Substring(11, 1) == "1") { errorGrid.Add(Lag.lSystem120_28377_G001); }
                            if (dataGrid.Substring(10, 1) == "1") { errorGrid.Add(Lag.lSystem120_28377_G002); }
                            if (dataGrid.Substring(9, 1) == "1") { errorGrid.Add(Lag.lSystem120_28377_G004); }
                            if (dataGrid.Substring(8, 1) == "1") { errorGrid.Add(Lag.lSystem120_28377_G008); }
                            if (dataGrid.Substring(7, 1) == "1") { errorGrid.Add(Lag.lSystem120_28377_G010); }
                            if (dataGrid.Substring(6, 1) == "1") { errorGrid.Add(Lag.lSystem120_28377_G020); }
                            if (dataGrid.Substring(5, 1) == "1") { errorGrid.Add(Lag.lSystem120_28377_G040); }
                            if (dataGrid.Substring(4, 1) == "1") { errorGrid.Add(Lag.lSystem120_28377_G080); }
                            if (dataGrid.Substring(3, 1) == "1") { errorGrid.Add(Lag.lSystem120_28377_G100); }
                            if (dataGrid.Substring(2, 1) == "1") { errorGrid.Add(Lag.lSystem120_28377_G200); }
                            if (dataGrid.Substring(1, 1) == "1") { errorGrid.Add(Lag.lSystem120_28377_G400); }
                        }

                        if (errorDC.Count == 0)
                        {
                            if (data__Dc.Substring(11, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D001); }
                            if (data__Dc.Substring(10, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D002); }
                            if (data__Dc.Substring(9, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D004); }
                            if (data__Dc.Substring(8, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D008); }
                            if (data__Dc.Substring(7, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D010); }
                            if (data__Dc.Substring(6, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D020); }
                            if (data__Dc.Substring(5, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D040); }
                            if (data__Dc.Substring(4, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D080); }
                            if (data__Dc.Substring(3, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D100); }
                            if (data__Dc.Substring(2, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D200); }
                            if (data__Dc.Substring(1, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D400); }
                            //if (data__Dc.Substring(0, 1) == "1") { errorDC.Add(Lag.lSystem120_28377_D800); }
                        }

                        if (errorFault.Count == 0)
                        {
                            if (dataFalt.Substring(11, 1) == "1") { errorFault.Add(Lag.lSystem120_28377_F001); }
                            if (dataFalt.Substring(10, 1) == "1") { errorFault.Add(Lag.lSystem120_28377_F002); }
                            if (dataFalt.Substring(9, 1) == "1") { errorFault.Add(Lag.lSystem120_28377_F004); }
                            if (dataFalt.Substring(8, 1) == "1") { errorFault.Add(Lag.lSystem120_28377_F008); }
                            if (dataFalt.Substring(7, 1) == "1") { errorFault.Add(Lag.lSystem120_28377_F010); }
                            if (dataFalt.Substring(6, 1) == "1") { errorFault.Add(Lag.lSystem120_28377_F020); }
                            if (dataFalt.Substring(5, 1) == "1") { errorFault.Add(Lag.lSystem120_28377_F040); }
                            if (dataFalt.Substring(4, 1) == "1") { errorFault.Add(Lag.lSystem120_28377_F080); }
                            if (dataFalt.Substring(3, 1) == "1") { errorFault.Add(Lag.lSystem120_28377_F100); }
                            if (dataFalt.Substring(2, 1) == "1") { errorFault.Add(Lag.lSystem120_28377_F200); }
                            if (dataFalt.Substring(1, 1) == "1") { errorFault.Add(Lag.lSystem120_28377_F400); }
                        }
                    }
                    #endregion
                }
                #endregion
            }

            if (!isCyclerStop)
            {
                var logSb = new StringBuilder();
                logSb.Append("CMD(0x110),");
                logSb.Append(cycler1Header.ToString());
                logSb.Append(",");
                logSb.Append(cycler1OP.ToString());
                logSb.Append(",");
                logSb.Append(cycler1voltage.ToString("N2"));
                logSb.Append("V,");
                logSb.Append(cycler1current.ToString("N2"));
                logSb.Append("A,TIMESTAMP:");
                logSb.Append(cycler_272_timestamp.ToString("D3"));
                logSb.Append(",ALARM(0x120),WARNING:");
                logSb.Append(Convert.ToInt32(dataWarn, 2).ToString("X"));
                logSb.Append(",GRID:");
                logSb.Append(Convert.ToInt32(dataGrid, 2).ToString("X"));
                logSb.Append(",DC:");
                logSb.Append(Convert.ToInt32(data__Dc, 2).ToString("X"));
                logSb.Append(",FAULT:");
                logSb.Append(Convert.ToInt32(dataFalt, 2).ToString("X"));
                logSb.Append(",TIMESTAMP:");
                logSb.Append(cycler_288_timestamp.ToString());

                mw.LogState(LogType.CYCLER, logSb.ToString(), null, false, true, true, 1);
            }
        }

        private void errorTmr_Tick(object sender, EventArgs e)
        {

            if (errorWarning.Count != 0 ||
                errorGrid.Count != 0 ||
                errorDC.Count != 0 ||
                errorFault.Count != 0)
            {
                if (!isAlarmOpen)
                {
                    ShowAlarm();
                }
            }
        }

        /// <summary>
        /// 알람팝업이 켜져있는지 확인되는 변수
        /// </summary>
        bool isAlarmOpen = false;

        private void ShowAlarm()
        {
            mw.Dispatcher.Invoke(new Action(() =>
            {
                mw.tlamp.SetTLampNG(true); 
            }));
            isAlarmOpen = true;
            mw.isStop = true;

            //알람 상황일때, 강제 출력 OFF
            var toDSP1OFF = new Thread(new ThreadStart(ToDSP1STOP));
            toDSP1OFF.Start();

            //두개 채널일때
            var toDSP2OFF = new Thread(new ThreadStart(ToDSP2STOP));
            toDSP2OFF.Start();

            //PLC Alarm Bit On
            if (mw.autoModeThread != null && mw.autoModeThread.IsAlive)
            {
                if (mw.plc.judgementFlag)
                {
                    mw.plc.CyclerAlarm(true);
                    Thread.Sleep(1000);
                    mw.plc.CyclerAlarm(false);
                }
            }

            mw.Dispatcher.Invoke(new Action(() =>
            {
                AlarmWindow aw = new AlarmWindow(mw,errorWarning,errorGrid,errorDC,errorFault,
                    AC_R_PHASE_VOLT,AC_S_PHASE_VOLT,AC_R_PHASE_CUR,AC_S_PHASE_CUR,AC_T_PHASE_CUR);
                aw.ShowDialog();

                //알람창을 확인눌러 끄면 에러클리어를 10회 날리고 중단.
                var th = new Thread(() =>
                {
                    mw.LogState(LogType.Info, "Send error clear message to Cycler");
                    for (int i = 0; i < 10; i++)
                    {
                        SendToDSP1("100", new byte[] { 0x89, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
                        SendToDSP2("200", new byte[] { 0x89, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(5000);
                    isAlarmOpen = false;
                    errorGrid.Clear();
                    errorDC.Clear();
                    errorFault.Clear();
                    errorWarning.Clear();

                    mw.Dispatcher.Invoke(new Action(() =>
                    {
                        mw.tlamp.SetTLampNG(false); 
                    }));
                });
                th.Start();

            }));            
        }

        public void ToDSP1STOP()
        {
            SendToDSP1("100", new byte[] { 0x86, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            SendToDSP1("100", new byte[] { 0x86, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            SendToDSP1("100", new byte[] { 0x84, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            SendToDSP1("100", new byte[] { 0x84, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
        }

        public void ToDSP2STOP()
        {
            SendToDSP2("200", new byte[] { 0x86, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            SendToDSP2("200", new byte[] { 0x86, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            SendToDSP2("200", new byte[] { 0x84, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
            SendToDSP2("200", new byte[] { 0x84, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            Thread.Sleep(100);
        }


        private string ConvertHexToBinary(string p)
        {
            var 십육진수 = "0x" + p;
            var 십진수 = Convert.ToInt32(십육진수, 16);
            var str = Convert.ToString(int.Parse(십진수.ToString()), 2);
            if (str.Length != 8)
            {
                for (int i = str.Length; i < 8; i++)
                {
                    str = "0" + str;
                }
            }

            return str;

        }
        public string binaryToInt(string d)
        {
            var str = Convert.ToInt32(ConvertHexToBinary(d), 2).ToString();
            return str == "0" ? "00" : str;
        }

        #region Alarm
        private string GridAlarm(byte[] bt)
        {
            string hex = (bt[0] - 48).ToString() + (bt[1] - 48).ToString() + (bt[2] - 48).ToString();
            var rtv = hex;
            switch (hex)
            {
                case "001": rtv = "입력 케이블 상순 불량"; break;
                case "002": rtv = "입력R상 과전류 발생"; break;
                case "004": rtv = "입력S상 과전류 발생"; break;
                case "010": rtv = "입력T상 과전류 발생"; break;
                case "020": rtv = "입력 과전압 발생"; break;
                case "040": rtv = "입력 저전압 발생"; break;
                case "100": rtv = "초기 충전 실패"; break;
                case "200": rtv = "입력 전압 불평형"; break;
            }

            return rtv;
        }

        private string FaultAlarm(byte[] bt)
        {
            string hex = (bt[0] - 48).ToString() + (bt[1] - 48).ToString() + (bt[2] - 48).ToString();
            var rtv = hex;
            switch (hex)
            {
                case "001": rtv = "방열판 과온도 발생"; break;
                case "002": rtv = "입력 퓨즈 소손"; break;
                case "004": rtv = "출력 퓨즈 소손"; break;
                case "008": rtv = "방열FAN 불량"; break;
                case "010": rtv = "비상스위치 동작"; break;
                case "020": rtv = "입력 R상 IGBT 문제 발생"; break;
                case "040": rtv = "입력 S상 IGBT 문제 발생"; break;
                case "080": rtv = "입력 T상 IGBT 문제 발생"; break;
                case "100": rtv = "채널1 IGBT 문제 발생"; break;
                case "800": rtv = "PC-DSP 통신 문제 발생"; break;
            }

            return rtv;
        }

        private string WarningAlarm(byte[] bt)
        {
            string hex = (bt[0] - 48).ToString() + (bt[1] - 48).ToString() + (bt[2] - 48).ToString();
            var rtv = hex;
            switch (hex)
            {
                case "001": rtv = "배터리 역결선 "; break;
                case "002": rtv = "채널1 시험 전류 초과"; break;
                case "004": rtv = "채널1 시험 전압 초과"; break;
                case "008": rtv = "배터리#1 시료 없음"; break;
            }

            return rtv;
        }

        private string DCAlarm(byte[] bt)
        {
            string hex = (bt[0] - 48).ToString() + (bt[1] - 48).ToString() + (bt[2] - 48).ToString();
            var rtv = hex;
            switch (hex)
            {
                case "001": rtv = "정류단 과충전 발생"; break;
                case "002": rtv = "정류단 전압 불평형"; break;
                case "004": rtv = "콘덴서#1 과충전 발생"; break;
                case "008": rtv = "콘덴서#1 전압 불평형"; break;
                case "010": rtv = "배터리#1 과충전 발생"; break;
                case "020": rtv = "배터리#1 과전류 발생"; break;
            }

            return rtv;
        }

        #endregion
        public CMD cycler1Header;
        public CMD cycler2Header;
        public RECV_OPMode cycler1OP;
        public RECV_OPMode cycler2OP;
        public bool cycler1Chksum = false;
        public bool cycler2Chksum = false;
        public double cycler1voltage = 0.0;
        public double cycler1current = 0.0;
        public double cycler2voltage = 0.0;
        public double cycler2current = 0.0;

        #region Debugging Data

        public double AC_R_PHASE_VOLT = 0.0;
        public double AC_S_PHASE_VOLT = 0.0;
        public double AC_R_PHASE_CUR = 0.0;
        public double AC_S_PHASE_CUR = 0.0;
        public double AC_T_PHASE_CUR = 0.0;

        #endregion

        public bool BTUtoDSP(int index, byte[] bt, CMD cmd)
        {
            try
            {
                Send(index, bt);
                mw.LogState(LogType.Success, "Cycler Send Msg " + cmd.ToString(), null, true, false);
                return true;
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "Cycler Send Msg", ec);
                return false;
            }
        }

        /// <summary>
        /// 실제 커맨드를 바이트로 만들어서 추가하는 부분
        /// </summary>
        /// <param name="slist">만들어진 byte[]를 포함할 리스트</param>
        /// <param name="scmd">byte[]로 만들어질 SendCMD</param>
        /// <returns></returns>
        public static bool AddToSendListBytes(List<SendCMD> slist, SendCMD scmd)
        {
            try
            {
                var bt = new byte[8];
                switch (scmd.Cmd)
                {
                    case CMD.INPUT_MC_ON:
                        {
                            bt[0] = 0x80;
                        }; break;
                    case CMD.OUTPUT_MC_ON:
                        {
                            bt[0] = 0x81;
                        } break;
                    case CMD.CHARGE:
                        {
                            bt[0] = 0x82;
                        } break;
                    case CMD.DISCHARGE:
                        {
                            bt[0] = 0x83;
                        } break;
                    case CMD.OUTPUT_MC_OFF:
                        {
                            bt[0] = 0x84;
                        } break;
                    case CMD.INPUT_MC_OFF:
                        {
                            bt[0] = 0x85;
                        }
                        break;
                    case CMD.REST:
                        {
                            bt[0] = 0x86;
                        } break;
                    default: break;
                }

                int curint = (int)(scmd.Current * 1000);

                if (curint == 0)
                {
                    bt[5] = bt[6] = bt[7] = 00;
                }
                else
                {

                    bt[5] = (byte)(curint >> 16 & 0xFF);
                    bt[6] = (byte)(curint >> 8 & 0xFF);
                    bt[7] = (byte)(curint >> 0 & 0xFF);
                }

                var s = 0;
                switch (scmd.Opmode)
                {
                    case OPMode.CHARGE_CC: s = 3 << 4; break;
                    case OPMode.CHARGE_CV: s = 4 << 4; break;
                    case OPMode.DISCHARGE_CC: s = 5 << 4; break;
                    case OPMode.DISCHARGE_CV: s = 6 << 4; break;
                    case OPMode.CHARGE_CP:
                        {
                            s = 7 << 4;
                            int curwat = (int)(scmd.Watt * 1000);

                            if (curwat == 0)
                            {
                                bt[5] = bt[6] = bt[7] = 00;
                            }
                            else
                            {

                                bt[5] = (byte)(curwat >> 16 & 0xFF);
                                bt[6] = (byte)(curwat >> 8 & 0xFF);
                                bt[7] = (byte)(curwat >> 0 & 0xFF);
                            }
                        }
                        break;
                    case OPMode.DISCHARGE_CP:
                        {
                            s = 8 << 4;
                            int curwat = (int)(scmd.Watt * 1000);

                            if (curwat == 0)
                            {
                                bt[5] = bt[6] = bt[7] = 00;
                            }
                            else
                            {

                                bt[5] = (byte)(curwat >> 16 & 0xFF);
                                bt[6] = (byte)(curwat >> 8 & 0xFF);
                                bt[7] = (byte)(curwat >> 0 & 0xFF);
                            }
                        }
                        break;
                }
                var end = 0;
                switch (scmd.Pmode)
                {
                    case BranchMode.NO_BRANCH: end = 0; break;
                    case BranchMode.CHANNEL1: end = 1; break;
                    case BranchMode.CHANNEL2: end = 2; break;
                }

                var str = StringToByteArray((s + end).ToString());
                bt[1] = str;

                int curvol = (int)(scmd.Voltage * 1000);

                if (curvol == 0)
                {
                    bt[2] = bt[3] = bt[4] = 00;
                }
                else
                {
                    bt[2] = (byte)(curvol >> 16 & 0xFF);
                    bt[3] = (byte)(curvol >> 8 & 0xFF);
                    bt[4] = (byte)(curvol >> 0 & 0xFF);
                }               

                scmd.SendingByte = bt;
                slist.Add(scmd);
                return true;
            }
            catch (Exception ec)
            {
                return false;
            }
        }

        public static byte ComputeAdditionChecksum(byte[] data)
        {
            int sumdata = 0;
            int multifle = 1;
            for (int i = 1; i < data.Length - 1; i++)
            {
                var da = (data[i] - 48);
                if (da > 0)
                    sumdata += (da * multifle);

                multifle++;

                if (multifle == 10)
                    multifle = 1;
            }

            var rtval = sumdata % 10;
            rtval += 48;
            var sum = Convert.ToByte(rtval);
            return sum;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte StringToByteArray(string hex)
        {
            return Convert.ToByte(hex);

            //return Enumerable.Range(0, hex.Length)
            //                 .Where(x => x % 2 == 0)
            //                 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            //                 .ToArray();
        }

        private void Send(int index, byte[] bt)
        {
            if (index == 0)
            {
                SendToDSP1("100", bt);
            }
            else if (index == 1)
            {
                SendToDSP2("200", bt);
            }
        }

        #region PCAN
        System.Windows.Forms.Timer tmrDisplay1 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer tmrDisplay2 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer tmrDisplay3 = new System.Windows.Forms.Timer();

        System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer tmr1 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer errorTmr = new System.Windows.Forms.Timer();

        System.Windows.Forms.Timer tmr84 = new System.Windows.Forms.Timer();

        private TPCANHandle[] m_HandlesArray;
        private bool m_IsFD;
        private TPCANHandle m_PcanHandle1;
        private TPCANHandle m_PcanHandle2;
        private TPCANHandle m_PcanHandle3;
        List<string> PCANHandleNameList = new List<string>();
        private TPCANBaudrate m_Baudrate;
        private TPCANType m_HwType;
        /// <summary>
        /// Stores the status of received messages for its display
        /// </summary>
        private delegate void ReadDelegateHandler();
        public System.Collections.ArrayList m_LastMsgsList1;
        private System.Collections.ArrayList m_LastMsgsList2;
        private System.Collections.ArrayList m_LastMsgsList3;
        private ReadDelegateHandler m_ReadDelegate1;
        private ReadDelegateHandler m_ReadDelegate2;
        private ReadDelegateHandler m_ReadDelegate3;
        private System.Threading.AutoResetEvent m_ReceiveEvent1;
        private bool m_ReceiveEvent1Flag;
        private System.Threading.AutoResetEvent m_ReceiveEvent2;
        private System.Threading.AutoResetEvent m_ReceiveEvent3;
        private System.Threading.Thread m_ReadThread1;
        private System.Threading.Thread m_ReadThread2;
        private System.Threading.Thread m_ReadThread3;

        private void ReadMessages1()
        {
            if (mw.isCLOSED)
                return;

            TPCANStatus stsResult;

            do
            {
                stsResult = m_IsFD ? ReadMessageFD1() : ReadMessage1();
                if (stsResult == TPCANStatus.PCAN_ERROR_ILLOPERATION)
                    break;
            } while (!Convert.ToBoolean(stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY));
        }

        private void ReadMessages2()
        {
            if (mw.isCLOSED)
                return;

            TPCANStatus stsResult;

            do
            {
                stsResult = m_IsFD ? ReadMessageFD2() : ReadMessage2();
                if (stsResult == TPCANStatus.PCAN_ERROR_ILLOPERATION)
                    break;
            } while (!Convert.ToBoolean(stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY));
        }

        private void ReadMessages3()
        {
            if (mw.isCLOSED)
                return;

            TPCANStatus stsResult;

            do
            {
                stsResult = m_IsFD ? ReadMessageFD3() : ReadMessage3();
                if (stsResult == TPCANStatus.PCAN_ERROR_ILLOPERATION)
                    break;
            } while (!Convert.ToBoolean(stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY));
        }

        private TPCANStatus ReadMessageFD1()
        {
            TPCANMsgFD CANMsg;
            TPCANTimestampFD CANTimeStamp;
            TPCANStatus stsResult;

            stsResult = PCANBasic.ReadFD(m_PcanHandle1, out CANMsg, out CANTimeStamp);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                ProcessMessage1(CANMsg, CANTimeStamp);

            currmsg = CANMsg;
            return stsResult;
        }

        private TPCANStatus ReadMessageFD2()
        {
            TPCANMsgFD CANMsg;
            TPCANTimestampFD CANTimeStamp;
            TPCANStatus stsResult;

            stsResult = PCANBasic.ReadFD(m_PcanHandle2, out CANMsg, out CANTimeStamp);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                ProcessMessage2(CANMsg, CANTimeStamp);

            currmsg = CANMsg;
            return stsResult;
        }

        private TPCANStatus ReadMessageFD3()
        {
            TPCANMsgFD CANMsg;
            TPCANTimestampFD CANTimeStamp;
            TPCANStatus stsResult;

            stsResult = PCANBasic.ReadFD(m_PcanHandle3, out CANMsg, out CANTimeStamp);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                ProcessMessage3(CANMsg, CANTimeStamp);

            currmsg = CANMsg;
            return stsResult;
        }

        ushort cycler_272_timestamp;
        ushort cycler_288_timestamp;

        private TPCANStatus ReadMessage1()
        {
            TPCANMsg CANMsg;
            TPCANTimestamp CANTimeStamp;
            TPCANStatus stsResult;

            // We execute the "Read" function of the PCANBasic                
            //
            stsResult = PCANBasic.Read(m_PcanHandle1, out CANMsg, out CANTimeStamp);


            if (CANMsg.ID == 272)//110
            {
                cycler_272_timestamp = CANTimeStamp.micros;
            }

            if (CANMsg.ID == 288)//120
            {
                cycler_288_timestamp = CANTimeStamp.micros;
            }


            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                ProcessMessage1(CANMsg, CANTimeStamp);

            return stsResult;
        }

        private TPCANStatus ReadMessage2()
        {
            TPCANMsg CANMsg;
            TPCANTimestamp CANTimeStamp;
            TPCANStatus stsResult;

            // We execute the "Read" function of the PCANBasic                
            //
            stsResult = PCANBasic.Read(m_PcanHandle2, out CANMsg, out CANTimeStamp);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                ProcessMessage2(CANMsg, CANTimeStamp);

            return stsResult;
        }

        private TPCANStatus ReadMessage3()
        {
            TPCANMsg CANMsg;
            TPCANTimestamp CANTimeStamp;
            TPCANStatus stsResult;

            // We execute the "Read" function of the PCANBasic                
            //
            stsResult = PCANBasic.Read(m_PcanHandle3, out CANMsg, out CANTimeStamp);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                ProcessMessage3(CANMsg, CANTimeStamp);

            return stsResult;
        }

        private void ProcessMessage1(TPCANMsg theMsg, TPCANTimestamp itsTimeStamp)
        {
            TPCANMsgFD newMsg;
            TPCANTimestampFD newTimestamp;

            newMsg = new TPCANMsgFD();
            newMsg.DATA = new byte[64];
            newMsg.ID = theMsg.ID;
            newMsg.DLC = theMsg.LEN;
            for (int i = 0; i < ((theMsg.LEN > 8) ? 8 : theMsg.LEN); i++)
                newMsg.DATA[i] = theMsg.DATA[i];
            newMsg.MSGTYPE = theMsg.MSGTYPE;

            newTimestamp = Convert.ToUInt64(itsTimeStamp.micros + 1000 * itsTimeStamp.millis + 0x100000000 * 1000 * itsTimeStamp.millis_overflow);
            ProcessMessage1(newMsg, newTimestamp);
        }

        private void ProcessMessage2(TPCANMsg theMsg, TPCANTimestamp itsTimeStamp)
        {
            TPCANMsgFD newMsg;
            TPCANTimestampFD newTimestamp;

            newMsg = new TPCANMsgFD();
            newMsg.DATA = new byte[64];
            newMsg.ID = theMsg.ID;
            newMsg.DLC = theMsg.LEN;
            for (int i = 0; i < ((theMsg.LEN > 8) ? 8 : theMsg.LEN); i++)
                newMsg.DATA[i] = theMsg.DATA[i];
            newMsg.MSGTYPE = theMsg.MSGTYPE;

            newTimestamp = Convert.ToUInt64(itsTimeStamp.micros + 1000 * itsTimeStamp.millis + 0x100000000 * 1000 * itsTimeStamp.millis_overflow);
            ProcessMessage2(newMsg, newTimestamp);
        }

        private void ProcessMessage3(TPCANMsg theMsg, TPCANTimestamp itsTimeStamp)
        {
            TPCANMsgFD newMsg;
            TPCANTimestampFD newTimestamp;

            newMsg = new TPCANMsgFD();
            newMsg.DATA = new byte[64];
            newMsg.ID = theMsg.ID;
            newMsg.DLC = theMsg.LEN;
            for (int i = 0; i < ((theMsg.LEN > 8) ? 8 : theMsg.LEN); i++)
                newMsg.DATA[i] = theMsg.DATA[i];
            newMsg.MSGTYPE = theMsg.MSGTYPE;

            newTimestamp = Convert.ToUInt64(itsTimeStamp.micros + 1000 * itsTimeStamp.millis + 0x100000000 * 1000 * itsTimeStamp.millis_overflow);
            ProcessMessage3(newMsg, newTimestamp);
        }

        public string id11_10 = "";
        public string id11_20 = ""; 

        private void ProcessMessage1(TPCANMsgFD theMsg, TPCANTimestampFD itsTimeStamp)
        {
            // We search if a message (Same ID and Type) is 
            // already received or if this is a new message
            //
            lock (m_LastMsgsList1.SyncRoot)
            {
                var msgStatus = new MessageStatus(theMsg, itsTimeStamp, this.bmsList.Count);

                //190612
                if (msgStatus.IdString.Equals("110h"))
                {
                    localDt = DateTime.Now;
                }

                if (msgStatus.IdString.Equals("011h"))
                {
                    if (msgStatus.DataString.Substring(0, 2) == "10")
                    {
                        id11_10 = msgStatus.DataString;
                    }
                    else if (msgStatus.DataString.Substring(0, 2) == "20")
                    {
                        id11_20 = msgStatus.DataString;
                    }
                }

                ////마지막 메시지와 같으면 업데이트시키는데, 일단 빼자
                foreach (MessageStatus msg in m_LastMsgsList1)
                {
                    if ((msg.CANMsg.ID == theMsg.ID) && (msg.CANMsg.MSGTYPE == theMsg.MSGTYPE))
                    {
                        // Modify the message and exit
                        //
                        msg.Update(theMsg, itsTimeStamp);
                        return;
                    }
                }
                // Message not found. It will created
                //
                InsertMsgEntry1(theMsg, itsTimeStamp);
            }
        }

        private void ProcessMessage2(TPCANMsgFD theMsg, TPCANTimestampFD itsTimeStamp)
        {
            // We search if a message (Same ID and Type) is 
            // already received or if this is a new message
            //
            lock (m_LastMsgsList2.SyncRoot)
            {
                ////마지막 메시지와 같으면 업데이트시키는데, 일단 빼자
                foreach (MessageStatus msg in m_LastMsgsList2)
                {
                    if ((msg.CANMsg.ID == theMsg.ID) && (msg.CANMsg.MSGTYPE == theMsg.MSGTYPE))
                    {
                        // Modify the message and exit
                        //
                        msg.Update(theMsg, itsTimeStamp);
                        return;
                    }
                }
                // Message not found. It will created
                //
                InsertMsgEntry2(theMsg, itsTimeStamp);
            }
        }

        private void ProcessMessage3(TPCANMsgFD theMsg, TPCANTimestampFD itsTimeStamp)
        {
            // We search if a message (Same ID and Type) is 
            // already received or if this is a new message
            //
            lock (m_LastMsgsList3.SyncRoot)
            {
                ////마지막 메시지와 같으면 업데이트시키는데, 일단 빼자
                foreach (MessageStatus msg in m_LastMsgsList3)
                {
                    if ((msg.CANMsg.ID == theMsg.ID) && (msg.CANMsg.MSGTYPE == theMsg.MSGTYPE))
                    {
                        // Modify the message and exit
                        //
                        msg.Update(theMsg, itsTimeStamp);
                        return;
                    }
                }
                // Message not found. It will created
                //
                InsertMsgEntry3(theMsg, itsTimeStamp);
            }
        }

        private void InsertMsgEntry1(TPCANMsgFD newMsg, TPCANTimestampFD timeStamp)
        {
            MessageStatus msgStsCurrentMsg;
            //ListViewItem lviCurrentItem;

            lock (m_LastMsgsList1.SyncRoot)
            {
                // We add this status in the last message list
                //
                msgStsCurrentMsg = new MessageStatus(newMsg, timeStamp, this.bmsList.Count);
                msgStsCurrentMsg.ShowingPeriod = true;
                m_LastMsgsList1.Add(msgStsCurrentMsg);

                //mw.LogState(LogType.Info, "CAN1 msg - " + msgStsCurrentMsg.IdString + " : " + msgStsCurrentMsg.DataString);
            }
        }

        private void InsertMsgEntry2(TPCANMsgFD newMsg, TPCANTimestampFD timeStamp)
        {
            MessageStatus msgStsCurrentMsg;
            //ListViewItem lviCurrentItem;

            lock (m_LastMsgsList2.SyncRoot)
            {
                // We add this status in the last message list
                //
                msgStsCurrentMsg = new MessageStatus(newMsg, timeStamp, this.bmsList.Count);
                msgStsCurrentMsg.ShowingPeriod = true;
                m_LastMsgsList2.Add(msgStsCurrentMsg);

                //mw.LogState(LogType.Info, "CAN2 msg - " + msgStsCurrentMsg.IdString + " : " + msgStsCurrentMsg.DataString);
            }
        }

        private void InsertMsgEntry3(TPCANMsgFD newMsg, TPCANTimestampFD timeStamp)
        {
            MessageStatus msgStsCurrentMsg;
            //ListViewItem lviCurrentItem;

            lock (m_LastMsgsList3.SyncRoot)
            {
                // We add this status in the last message list
                //
                msgStsCurrentMsg = new MessageStatus(newMsg, timeStamp, this.bmsList.Count);
                msgStsCurrentMsg.ShowingPeriod = true;
                m_LastMsgsList3.Add(msgStsCurrentMsg);

                //mw.LogState(LogType.Info, "CAN3 msg - " + msgStsCurrentMsg.IdString + " : " + msgStsCurrentMsg.DataString);
            }
        }
        public Dictionary<string, ReceiveType> bmsList = new Dictionary<string, ReceiveType>();
        public string GetBMSData(object id, int stbit, int length)
        {
            var sid = id.ToString() + "h";
            try
            {
                if (bmsList.ContainsKey(sid))
                {
                    var data = bmsList[sid].Data.Replace(" ", "");

                    var lup = new Dictionary<char, string>{
            { '0', "0000"},
            { '1', "0001"},
            { '2', "0010"}, 
            { '3', "0011"},

            { '4', "0100"}, 
            { '5', "0101"}, 
            { '6', "0110"}, 
            { '7', "0111"},

            { '8', "1000"}, 
            { '9', "1001"}, 
            { 'A', "1010"}, 
            { 'B', "1011"},

            { 'C', "1100"}, 
            { 'D', "1101"}, 
            { 'E', "1110"}, 
            { 'F', "1111"}};

                    var ret = string.Join("", from character in data
                                              select lup[character]);


                    //long longValue = Convert.ToInt64(data, 16);
                    //string binRepresentation = Convert.ToString(longValue, 2);
                    //var arr = binRepresentation.ToArray();
                    var arr = ret.ToArray();
                    var rtv = "";
                    for (int i = stbit; i < stbit + length; i++)
                    {
                        rtv += arr[i];
                    }
                    return Convert.ToInt64(rtv, 2).ToString();

                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Fail, "DetailDataSave", ex);
            }
            return "0";
        }

        private void InitializeBasicComponents()
        {
            // Creates the list for received messages
            //
            m_LastMsgsList1 = new System.Collections.ArrayList();
            m_ReadDelegate1 = new ReadDelegateHandler(ReadMessages1);
            m_ReceiveEvent1 = new System.Threading.AutoResetEvent(false);

            m_LastMsgsList2 = new System.Collections.ArrayList();
            m_ReadDelegate2 = new ReadDelegateHandler(ReadMessages2);
            m_ReceiveEvent2 = new System.Threading.AutoResetEvent(false);

            m_LastMsgsList3 = new System.Collections.ArrayList();
            m_ReadDelegate3 = new ReadDelegateHandler(ReadMessages3);
            m_ReceiveEvent3 = new System.Threading.AutoResetEvent(false);
            // Creates an array with all possible PCAN-Channels
            //
            m_HandlesArray = new TPCANHandle[] 
            { 
                PCANBasic.PCAN_ISABUS1,
                PCANBasic.PCAN_ISABUS2,
                PCANBasic.PCAN_ISABUS3,
                PCANBasic.PCAN_ISABUS4,
                PCANBasic.PCAN_ISABUS5,
                PCANBasic.PCAN_ISABUS6,
                PCANBasic.PCAN_ISABUS7,
                PCANBasic.PCAN_ISABUS8,
                PCANBasic.PCAN_DNGBUS1,
                PCANBasic.PCAN_PCIBUS1,
                PCANBasic.PCAN_PCIBUS2,
                PCANBasic.PCAN_PCIBUS3,
                PCANBasic.PCAN_PCIBUS4,
                PCANBasic.PCAN_PCIBUS5,
                PCANBasic.PCAN_PCIBUS6,
                PCANBasic.PCAN_PCIBUS7,
                PCANBasic.PCAN_PCIBUS8,
                PCANBasic.PCAN_PCIBUS9,
                PCANBasic.PCAN_PCIBUS10,
                PCANBasic.PCAN_PCIBUS11,
                PCANBasic.PCAN_PCIBUS12,
                PCANBasic.PCAN_PCIBUS13,
                PCANBasic.PCAN_PCIBUS14,
                PCANBasic.PCAN_PCIBUS15,
                PCANBasic.PCAN_PCIBUS16,
                PCANBasic.PCAN_USBBUS1,
                PCANBasic.PCAN_USBBUS2,
                PCANBasic.PCAN_USBBUS3,
                PCANBasic.PCAN_USBBUS4,
                PCANBasic.PCAN_USBBUS5,
                PCANBasic.PCAN_USBBUS6,
                PCANBasic.PCAN_USBBUS7,
                PCANBasic.PCAN_USBBUS8,
                PCANBasic.PCAN_USBBUS9,
                PCANBasic.PCAN_USBBUS10,
                PCANBasic.PCAN_USBBUS11,
                PCANBasic.PCAN_USBBUS12,
                PCANBasic.PCAN_USBBUS13,
                PCANBasic.PCAN_USBBUS14,
                PCANBasic.PCAN_USBBUS15,
                PCANBasic.PCAN_USBBUS16,
                PCANBasic.PCAN_PCCBUS1,
                PCANBasic.PCAN_PCCBUS2,
                PCANBasic.PCAN_LANBUS1,
                PCANBasic.PCAN_LANBUS2,
                PCANBasic.PCAN_LANBUS3,
                PCANBasic.PCAN_LANBUS4,
                PCANBasic.PCAN_LANBUS5,
                PCANBasic.PCAN_LANBUS6,
                PCANBasic.PCAN_LANBUS7,
                PCANBasic.PCAN_LANBUS8,
                PCANBasic.PCAN_LANBUS9,
                PCANBasic.PCAN_LANBUS10,
                PCANBasic.PCAN_LANBUS11,
                PCANBasic.PCAN_LANBUS12,
                PCANBasic.PCAN_LANBUS13,
                PCANBasic.PCAN_LANBUS14,
                PCANBasic.PCAN_LANBUS15,
                PCANBasic.PCAN_LANBUS16,
            };

            UInt32 iBuffer;
            iBuffer = PCANBasic.LOG_FUNCTION_ALL;
            PCANBasic.SetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_LOG_CONFIGURE, ref iBuffer, sizeof(UInt32));
        }

        /// <summary>
        /// PCAN 핸들을 고정으로 박아버린다.
        /// </summary>
        private void SetPCANHandle1(int index)
        {
            bool bNonPnP;
            var strTemp = PCANHandleNameList[index];
            if (strTemp == null)
                return;

            mw.LogState(LogType.Info, "PCAN Port Set : " + strTemp);

            strTemp = strTemp.Substring(strTemp.IndexOf('(') + 1, 3);

            strTemp = strTemp.Replace('h', ' ').Trim(' ');

            m_PcanHandle1 = Convert.ToUInt16(strTemp, 16);
            bNonPnP = m_PcanHandle1 <= PCANBasic.PCAN_DNGBUS1;
        }

        /// <summary>
        /// PCAN 핸들을 고정으로 박아버린다.
        /// </summary>
        private void SetPCANHandle2(int index)
        {
            bool bNonPnP;
            var strTemp = PCANHandleNameList[index];
            if (strTemp == null)
                return;

            mw.LogState(LogType.Info, "PCAN Port Set : " + strTemp);

            strTemp = strTemp.Substring(strTemp.IndexOf('(') + 1, 3);

            strTemp = strTemp.Replace('h', ' ').Trim(' ');

            m_PcanHandle2 = Convert.ToUInt16(strTemp, 16);
            bNonPnP = m_PcanHandle2 <= PCANBasic.PCAN_DNGBUS1;
        }

        /// <summary>
        /// PCAN 핸들을 고정으로 박아버린다.
        /// </summary>
        private void SetPCANHandle3(int index)
        {
            bool bNonPnP;
            var strTemp = PCANHandleNameList[index];
            if (strTemp == null)
                return;

            mw.LogState(LogType.Info, "PCAN Port Set : " + strTemp);

            strTemp = strTemp.Substring(strTemp.IndexOf('(') + 1, 3);

            strTemp = strTemp.Replace('h', ' ').Trim(' ');

            m_PcanHandle3 = Convert.ToUInt16(strTemp, 16);
            bNonPnP = m_PcanHandle3 <= PCANBasic.PCAN_DNGBUS1;
        }

        private string GetFormatedError(TPCANStatus error)
        {
            StringBuilder strTemp;
            strTemp = new StringBuilder(256);
            if (PCANBasic.GetErrorText(error, 0, strTemp) != TPCANStatus.PCAN_ERROR_OK)
                return string.Format("An error occurred. Error-code's text ({0:X}) couldn't be retrieved", error);
            else
                return strTemp.ToString();
        }

        private void ConfigureTraceFile1()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            iBuffer = 5;
            stsResult = PCANBasic.SetValue(m_PcanHandle1, TPCANParameter.PCAN_TRACE_SIZE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                mw.LogState(LogType.Fail, GetFormatedError(stsResult));

            iBuffer = PCANBasic.TRACE_FILE_SINGLE | PCANBasic.TRACE_FILE_OVERWRITE;
            stsResult = PCANBasic.SetValue(m_PcanHandle1, TPCANParameter.PCAN_TRACE_CONFIGURE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                mw.LogState(LogType.Fail, GetFormatedError(stsResult));
        }
        private void ConfigureTraceFile2()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;
            iBuffer = 5;
            stsResult = PCANBasic.SetValue(m_PcanHandle2, TPCANParameter.PCAN_TRACE_SIZE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                mw.LogState(LogType.Fail, GetFormatedError(stsResult));

            iBuffer = PCANBasic.TRACE_FILE_SINGLE | PCANBasic.TRACE_FILE_OVERWRITE;
            stsResult = PCANBasic.SetValue(m_PcanHandle2, TPCANParameter.PCAN_TRACE_CONFIGURE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                mw.LogState(LogType.Fail, GetFormatedError(stsResult));
        }
        private void ConfigureTraceFile3()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;
            iBuffer = 5;
            stsResult = PCANBasic.SetValue(m_PcanHandle3, TPCANParameter.PCAN_TRACE_SIZE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                mw.LogState(LogType.Fail, GetFormatedError(stsResult));

            iBuffer = PCANBasic.TRACE_FILE_SINGLE | PCANBasic.TRACE_FILE_OVERWRITE;
            stsResult = PCANBasic.SetValue(m_PcanHandle3, TPCANParameter.PCAN_TRACE_CONFIGURE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                mw.LogState(LogType.Fail, GetFormatedError(stsResult));
        }

        private void SetConnectionStatus1(int index, bool bConnected)
        {
            if (!bConnected)
            {
                bool bNonPnP;
                string strTemp;

                strTemp = PCANHandleNameList[index];
                strTemp = strTemp.Substring(strTemp.IndexOf('(') + 1, 3);
                strTemp = strTemp.Replace('h', ' ').Trim(' ');
                m_PcanHandle1 = Convert.ToUInt16(strTemp, 16);
                bNonPnP = m_PcanHandle1 <= PCANBasic.PCAN_DNGBUS1;
            }
        }

        private void SetConnectionStatus2(int index, bool bConnected)
        {
            if (!bConnected)
            {
                bool bNonPnP;
                string strTemp;

                strTemp = PCANHandleNameList[index];
                strTemp = strTemp.Substring(strTemp.IndexOf('(') + 1, 3);
                strTemp = strTemp.Replace('h', ' ').Trim(' ');
                m_PcanHandle2 = Convert.ToUInt16(strTemp, 16);
                bNonPnP = m_PcanHandle2 <= PCANBasic.PCAN_DNGBUS1;
            }
        }

        private void SetConnectionStatus3(int index, bool bConnected)
        {
            if (!bConnected)
            {
                bool bNonPnP;
                string strTemp;

                strTemp = PCANHandleNameList[index];
                strTemp = strTemp.Substring(strTemp.IndexOf('(') + 1, 3);
                strTemp = strTemp.Replace('h', ' ').Trim(' ');
                m_PcanHandle3 = Convert.ToUInt16(strTemp, 16);
                bNonPnP = m_PcanHandle3 <= PCANBasic.PCAN_DNGBUS1;
            }
        }

        /// <summary>
        /// BMS(CAN) 초기화부분
        /// </summary>
        private void SetPCAN_BMS1(int index, string fname)
        {
            PCANHandleNameList.Add(fname);
            TPCANStatus stsResult;
            SetPCANHandle1(index);
            m_Baudrate = TPCANBaudrate.PCAN_BAUD_1M;
            m_HwType = TPCANType.PCAN_TYPE_ISA;

            stsResult = PCANBasic.Initialize(
                m_PcanHandle1,
                m_Baudrate,
                m_HwType,
                Convert.ToUInt32("0100", 16),//IO port
                Convert.ToUInt16("3"));//Interrupt

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                if (stsResult != TPCANStatus.PCAN_ERROR_CAUTION)
                {
                    this.mw.LogState(LogType.Fail, "PCAN_CYCLER_" + GetFormatedError(stsResult));
                }
                else
                {
                    //StateLb.Items.Insert(0, "******************************************************");
                    //StateLb.Items.Insert(0, "The bitrate being used is different than the given one");
                    //StateLb.Items.Insert(0, "******************************************************");
                    stsResult = TPCANStatus.PCAN_ERROR_OK;
                }
            }
            else
            {
                ConfigureTraceFile1(); //Prepares the PCAN-Basic's PCAN-Trace file

                System.Threading.ThreadStart threadDelegate = new System.Threading.ThreadStart(this.CANReadThreadFunc1);
                m_ReadThread1 = new System.Threading.Thread(threadDelegate);
                m_ReadThread1.IsBackground = true;
                m_ReadThread1.Start();

                t3 = new Thread(() =>
                {
                    while (true)
                    {
                        tmrDisplay_Tick1(this, null);
                        Thread.Sleep(100);
                    }
                });
                t3.Start();
                 
                //mw.LogState(LogType.Info, "CAN1 Timer Started");

                //mw.LogState(LogType.Info, "PCAN1 Connected");

                SendToDSP1("100", new byte[] { 0x80, 0x00, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
                Thread.Sleep(500);
                SendToDSP1("100", new byte[] { 0x84, 0x00, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            }
            SetConnectionStatus1(index, stsResult == TPCANStatus.PCAN_ERROR_OK);

        }

        private void SetPCAN_BMS2(int index, string fname)
        {
            PCANHandleNameList.Add(fname);
            TPCANStatus stsResult;
            SetPCANHandle2(index);
            m_Baudrate = TPCANBaudrate.PCAN_BAUD_1M;
            m_HwType = TPCANType.PCAN_TYPE_ISA;
            stsResult = PCANBasic.Initialize(
                m_PcanHandle2,
                m_Baudrate,
                m_HwType,
                Convert.ToUInt32("0100", 16),//IO port
                Convert.ToUInt16("3"));//Interrupt

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                if (stsResult != TPCANStatus.PCAN_ERROR_CAUTION)
                {
                    mw.LogState(LogType.Fail, "PCAN_BMS_" + GetFormatedError(stsResult));
                }
                else
                {
                    //StateLb.Items.Insert(0, "******************************************************");
                    //StateLb.Items.Insert(0, "The bitrate being used is different than the given one");
                    //StateLb.Items.Insert(0, "******************************************************");
                    stsResult = TPCANStatus.PCAN_ERROR_OK;
                }
            }
            else
            {
                ConfigureTraceFile2(); //Prepares the PCAN-Basic's PCAN-Trace file

                System.Threading.ThreadStart threadDelegate = new System.Threading.ThreadStart(this.CANReadThreadFunc2);
                m_ReadThread2 = new System.Threading.Thread(threadDelegate);
                m_ReadThread2.IsBackground = true;
                m_ReadThread2.Start();

                tmrDisplay2.Interval = 100;
                tmrDisplay2.Tick += tmrDisplay_Tick2;
                tmrDisplay2.Start();

                mw.LogState(LogType.Info, "CAN2 Timer Started");

                mw.LogState(LogType.Info, "PCAN2 Connected");

                SendToDSP2("200", new byte[] { 0x80, 0x01, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
                Thread.Sleep(500);
                SendToDSP2("200", new byte[] { 0x84, 0x00, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
            }
            SetConnectionStatus2(index, stsResult == TPCANStatus.PCAN_ERROR_OK);

        }

        private void SetPCAN_BMS3(int index, string fname)
        {
            PCANHandleNameList.Add(fname);
            TPCANStatus stsResult;
            SetPCANHandle3(index);
            m_Baudrate = TPCANBaudrate.PCAN_BAUD_500K;
            m_HwType = TPCANType.PCAN_TYPE_ISA;
            stsResult = PCANBasic.Initialize(
                m_PcanHandle3,
                m_Baudrate,
                m_HwType,
                Convert.ToUInt32("0100", 16),//IO port
                Convert.ToUInt16("3"));//Interrupt

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                if (stsResult != TPCANStatus.PCAN_ERROR_CAUTION)
                {
                    this.mw.LogState(LogType.Fail, "PCAN_BMS_" + GetFormatedError(stsResult));
                }
                else
                {
                    //StateLb.Items.Insert(0, "******************************************************");
                    //StateLb.Items.Insert(0, "The bitrate being used is different than the given one");
                    //StateLb.Items.Insert(0, "******************************************************");
                    stsResult = TPCANStatus.PCAN_ERROR_OK;
                }
            }
            else
            {
                ConfigureTraceFile3(); //Prepares the PCAN-Basic's PCAN-Trace file

                System.Threading.ThreadStart threadDelegate = new System.Threading.ThreadStart(this.CANReadThreadFunc3);
                m_ReadThread3 = new System.Threading.Thread(threadDelegate);
                m_ReadThread3.IsBackground = true;
                m_ReadThread3.Start();

                tmrDisplay3.Interval = 100;
                tmrDisplay3.Tick += tmrDisplay_Tick3;
                tmrDisplay3.Start();

                mw.LogState(LogType.Info, "CAN3 Timer Started");

                mw.LogState(LogType.Info, "PCAN3 Connected");

            }
            SetConnectionStatus3(index, stsResult == TPCANStatus.PCAN_ERROR_OK);


        }

        int localcount = 0;
        private void tmrDisplay_Tick1(object sender, EventArgs e)
        {
            DisplayMessages1();
        }

        private void tmrDisplay_Tick2(object sender, EventArgs e)
        {
            DisplayMessages2();
        }

        private void tmrDisplay_Tick3(object sender, EventArgs e)
        {
            DisplayMessages3();
        }

        TPCANMsgFD currmsg;

        public static int GetLengthFromDLC(int dlc, bool isSTD)
        {
            if (dlc <= 8)
                return dlc;

            if (isSTD)
                return 8;

            switch (dlc)
            {
                case 9: return 12;
                case 10: return 16;
                case 11: return 20;
                case 12: return 24;
                case 13: return 32;
                case 14: return 48;
                case 15: return 64;
                default: return dlc;
            }
        }
        public void SendToDSP1(string id, byte[] bts)
        {
            if (m_PcanHandle1 == null)
                return;


            TPCANMsg CANMsg = new TPCANMsg();
            CANMsg.DATA = new byte[8];


            CANMsg.ID = Convert.ToUInt32(id, 16);//ID입력
            CANMsg.LEN = Convert.ToByte(8);//DLC
            CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;//스탠다드 고정

            for (int i = 0; i < bts.Length; i++)
            {
                CANMsg.DATA[i] = bts[i];
            }

            PCANBasic.Write(m_PcanHandle1, ref CANMsg);

        }

        public void SendToDSP2(string id, byte[] bts)
        {
            if (m_PcanHandle2 == null)
                return;


            TPCANMsg CANMsg = new TPCANMsg();
            CANMsg.DATA = new byte[8];


            CANMsg.ID = Convert.ToUInt32(id, 16);//ID입력
            CANMsg.LEN = Convert.ToByte(8);//DLC
            CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;//스탠다드 고정

            for (int i = 0; i < bts.Length; i++)
            {
                CANMsg.DATA[i] = bts[i];
            }

            PCANBasic.Write(m_PcanHandle2, ref CANMsg);

        }

        public void SendToDSP3(string id, byte[] bts)
        {
            if (m_PcanHandle3 == null)
                return;


            TPCANMsg CANMsg = new TPCANMsg();
            CANMsg.DATA = new byte[8];


            CANMsg.ID = Convert.ToUInt32(id, 16);//ID입력
            CANMsg.LEN = Convert.ToByte(8);//DLC
            CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;//스탠다드 고정

            for (int i = 0; i < bts.Length; i++)
            {
                CANMsg.DATA[i] = bts[i];
            }

            PCANBasic.Write(m_PcanHandle3, ref CANMsg);

        }
        /// <summary>
        /// 바뀐값이 계속 bmsList에 새로고침되는부분
        /// </summary>
        private void DisplayMessages1()
        {
            lock (m_LastMsgsList1.SyncRoot)
            {
                for (int i = 0; i < m_LastMsgsList1.Count; i++)
                {
                    var msgStatus = m_LastMsgsList1[i] as MessageStatus;
                    // 20190814 Noah Choi 플래그 변수를 사용하여 초기화 진행(전해준 대리님 수정)
                    if (!isClear)
                    {
                        if (!bmsList.ContainsKey(msgStatus.IdString))
                        {
                            bmsList.Add(msgStatus.IdString, new ReceiveType()
                            {
                                Type = msgStatus.TypeString,
                                Id = msgStatus.IdString,
                                Length = GetLengthFromDLC(msgStatus.CANMsg.DLC, (msgStatus.CANMsg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0),
                                Count = 0,    //msgStatus.Count,
                                RcvTime = msgStatus.TimeString,
                                Data = msgStatus.DataString
                            });
                        }
                        else
                        {
                            bmsList[msgStatus.IdString] = new ReceiveType()
                            {
                                Type = msgStatus.TypeString,
                                Id = msgStatus.IdString,
                                Length = GetLengthFromDLC(msgStatus.CANMsg.DLC, (msgStatus.CANMsg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0),
                                Count = 0,    //msgStatus.Count,
                                RcvTime = msgStatus.TimeString,
                                Data = msgStatus.DataString
                            };
                        }
                    }
                }
            }
        }

        private void DisplayMessages2()
        {
            lock (m_LastMsgsList2.SyncRoot)
            {
                for (int i = 0; i < m_LastMsgsList2.Count; i++)
                {
                    var msgStatus = m_LastMsgsList2[i] as MessageStatus;

                    if (!bmsList.ContainsKey(msgStatus.IdString))
                    {
                        bmsList.Add(msgStatus.IdString, new ReceiveType()
                        {
                            Type = msgStatus.TypeString,
                            Id = msgStatus.IdString,
                            Length = GetLengthFromDLC(msgStatus.CANMsg.DLC, (msgStatus.CANMsg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0),
                            Count = 0,    //msgStatus.Count,
                            RcvTime = msgStatus.TimeString,
                            Data = msgStatus.DataString
                        });
                    }
                    else
                    {
                        bmsList[msgStatus.IdString] = new ReceiveType()
                        {
                            Type = msgStatus.TypeString,
                            Id = msgStatus.IdString,
                            Length = GetLengthFromDLC(msgStatus.CANMsg.DLC, (msgStatus.CANMsg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0),
                            Count = 0,    //msgStatus.Count,
                            RcvTime = msgStatus.TimeString,
                            Data = msgStatus.DataString
                        };
                    }

                }
            }
        }

        private void DisplayMessages3()
        {
            lock (m_LastMsgsList3.SyncRoot)
            {
                for (int i = 0; i < m_LastMsgsList3.Count; i++)
                {
                    var msgStatus = m_LastMsgsList3[i] as MessageStatus;

                    if (!bmsList.ContainsKey(msgStatus.IdString))
                    {
                        bmsList.Add(msgStatus.IdString, new ReceiveType()
                        {
                            Type = msgStatus.TypeString,
                            Id = msgStatus.IdString,
                            Length = GetLengthFromDLC(msgStatus.CANMsg.DLC, (msgStatus.CANMsg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0),
                            Count = 0,    //msgStatus.Count,
                            RcvTime = msgStatus.TimeString,
                            Data = msgStatus.DataString
                        });
                    }
                    else
                    {
                        bmsList[msgStatus.IdString] = new ReceiveType()
                        {
                            Type = msgStatus.TypeString,
                            Id = msgStatus.IdString,
                            Length = GetLengthFromDLC(msgStatus.CANMsg.DLC, (msgStatus.CANMsg.MSGTYPE & TPCANMessageType.PCAN_MESSAGE_FD) == 0),
                            Count = 0,    //msgStatus.Count,
                            RcvTime = msgStatus.TimeString,
                            Data = msgStatus.DataString
                        };
                    }

                }
            }
        }

        private void CANReadThreadFunc1()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            iBuffer = Convert.ToUInt32(m_ReceiveEvent1.SafeWaitHandle.DangerousGetHandle().ToInt32());
            // Sets the handle of the Receive-Event.
            //
            stsResult = PCANBasic.SetValue(m_PcanHandle1, TPCANParameter.PCAN_RECEIVE_EVENT, ref iBuffer, sizeof(UInt32));

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                mw.LogState(LogType.Info, "[PCAN] - retry Initialize logic");
                Thread.Sleep(1000);
                PCANBasic.Uninitialize(m_PcanHandle1);
                mw.LogState(LogType.Info, "[PCAN] - Uninitialize");
                Thread.Sleep(1000);

                stsResult = PCANBasic.Initialize(
                m_PcanHandle1,
                m_Baudrate,
                m_HwType,
                Convert.ToUInt32("0100", 16),//IO port
                Convert.ToUInt16("3"));//Interrupt
                mw.LogState(LogType.Info, "[PCAN] - Initialize");

                if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                {
                    mw.LogState(LogType.Fail, "[PCAN] - Initialize");
                    mw.LogState(LogType.Info, GetFormatedError(stsResult));
                    return;
                }
                else
                {
                    mw.LogState(LogType.Success, "[PCAN] - Initialize");
                }
                
                stsResult = PCANBasic.SetValue(m_PcanHandle1, TPCANParameter.PCAN_RECEIVE_EVENT, ref iBuffer, sizeof(UInt32));
                if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                {
                    mw.LogState(LogType.Fail, "[PCAN] - SetValue");
                    mw.LogState(LogType.Info, GetFormatedError(stsResult));
                    return;
                }
                else
                {
                    mw.LogState(LogType.Success, "[PCAN] - SetValue");
                }
            }

            m_ReceiveEvent1Flag = true;

            while (!mw.isCLOSED)
            {
                if (m_ReceiveEvent1Flag)
                {
                    if (m_ReceiveEvent1.WaitOne(50))
                        mw.Dispatcher.Invoke(m_ReadDelegate1);
                }
                else
                {
                    break;
                }
            }
        }

        private void CANReadThreadFunc2()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            iBuffer = Convert.ToUInt32(m_ReceiveEvent2.SafeWaitHandle.DangerousGetHandle().ToInt32());
            // Sets the handle of the Receive-Event.
            //
            stsResult = PCANBasic.SetValue(m_PcanHandle2, TPCANParameter.PCAN_RECEIVE_EVENT, ref iBuffer, sizeof(UInt32));

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                mw.LogState(LogType.Info, GetFormatedError(stsResult));
                return;
            }

            while (!mw.isCLOSED)
            {
                if (m_ReceiveEvent2.WaitOne(50))
                    mw.Dispatcher.Invoke(m_ReadDelegate2);
            }
        }

        private void CANReadThreadFunc3()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            iBuffer = Convert.ToUInt32(m_ReceiveEvent3.SafeWaitHandle.DangerousGetHandle().ToInt32());
            // Sets the handle of the Receive-Event.
            //
            stsResult = PCANBasic.SetValue(m_PcanHandle3, TPCANParameter.PCAN_RECEIVE_EVENT, ref iBuffer, sizeof(UInt32));

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                mw.LogState(LogType.Info, GetFormatedError(stsResult));
                return;
            }

            while (!mw.isCLOSED)
            {
                if (m_ReceiveEvent3.WaitOne(50))
                    mw.Dispatcher.Invoke(m_ReadDelegate3);
            }
        }

		// 20190814 Noah Choi Cycler 연결성체크시 CanDataList Clear 진행(전해준대리님 수정)  
        public void ClearCANDataList()
        {
            isClear = true;
            Thread.Sleep(300);
            bmsList.Clear();
            m_LastMsgsList1.Clear();
            bmsList.Clear();
            Thread.Sleep(300);
            isClear = false;
        }
        #endregion

    }
}
