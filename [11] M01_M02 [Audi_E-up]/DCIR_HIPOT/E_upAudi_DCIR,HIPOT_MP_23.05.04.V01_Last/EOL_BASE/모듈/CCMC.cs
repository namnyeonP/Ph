using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE.모듈
{
    public class CCMC
    {
        #region Fields
        MainWindow mw;

        public bool isAlive = false;

        public DateTime localDt = new DateTime();

        public SerialPort sp;
        Action kfR = null;
        IAsyncResult recv_result;
        Stream spStream;
        bool endloop = false;
        public Thread setValuesThread;
        public Thread readThread;
        ManualResetEvent _readDone = new ManualResetEvent(false);
        byte[] buffer = new byte[4096];
        public static Mutex mtx = new Mutex(false, "MutexName5"); //210419 Add by KYJ
        //Thread CellRefresher;
        Stopwatch stopwatch = new Stopwatch();
        public Dictionary<string, string> raw_cmcList = new Dictionary<string, string>();

        int recvingTimeInterval = 100;
        object obj = new object();
        List<string> sendinglist = new List<string>();

        bool isDCIRMode = true;

        bool isReceiving = false;
        public bool cbFlag = false;

        public string rawbyte_bal = "";
        public string rawbyte_bal_reverse = "";


        public Dictionary<string, byte> Commands = new Dictionary<string, byte>();

        #region Recv flags

        public bool b_isRecieveOk = false;

        public bool CB1_OPEN_FLT = false;
        public bool CB2_OPEN_FLT = false;
        public bool CB3_OPEN_FLT = false;
        public bool CB4_OPEN_FLT = false;
        public bool CB5_OPEN_FLT = false;
        public bool CB6_OPEN_FLT = false;
        public bool CB1_SHORT_FLT = false;
        public bool CB2_SHORT_FLT = false;
        public bool CB3_SHORT_FLT = false;
        public bool CB4_SHORT_FLT = false;
        public bool CB5_SHORT_FLT = false;
        public bool CB6_SHORT_FLT = false;
        //public bool AN0_OT = false;
        //public bool AN1_OT = false;
        //public bool AN2_OT = false;
        //public bool AN3_OT = false;
        public bool AN4_OT = false;
        public bool AN5_OT = false;
        public bool AN6_OT = false;
        //public bool AN0_UT = false;
        //public bool AN1_UT = false;
        //public bool AN2_UT = false;
        //public bool AN3_UT = false;
        public bool AN4_UT = false;
        public bool AN5_UT = false;
        public bool AN6_UT = false;
        //public bool CT0_OV = false;
        public bool CT1_OV = false;
        public bool CT2_OV = false;
        public bool CT3_OV = false;
        public bool CT4_OV = false;
        public bool CT5_OV = false;
        public bool CT6_OV = false;
        //public bool CT0_UV = false;
        public bool CT1_UV = false;
        public bool CT2_UV = false;
        public bool CT3_UV = false;
        public bool CT4_UV = false;
        public bool CT5_UV = false;
        public bool CT6_UV = false;
        public bool CT0_OPEN_WIRE = false;
        public bool CT1_OPEN_WIRE = false;
        public bool CT2_OPEN_WIRE = false;
        public bool CT3_OPEN_WIRE = false;
        public bool CT4_OPEN_WIRE = false;
        public bool CT5_OPEN_WIRE = false;
        public bool CT6_OPEN_WIRE = false;
        public bool CT1_BAL = false;
        public bool CT2_BAL = false;
        public bool CT3_BAL = false;
        public bool CT4_BAL = false;
        public bool CT5_BAL = false;
        public bool CT6_BAL = false;
        public double CV1_CB = 0.0;
        public double CV2_CB = 0.0;
        public double CV3_CB = 0.0;
        public double CV4_CB = 0.0;
        public double CV5_CB = 0.0;
        public double CV6_CB = 0.0;


        public double CV1_LEAK_POL_0 = 0.0;
        public double CV2_LEAK_POL_0 = 0.0;
        public double CV3_LEAK_POL_0 = 0.0;
        public double CV4_LEAK_POL_0 = 0.0;
        public double CV5_LEAK_POL_0 = 0.0;
        public double CV6_LEAK_POL_0 = 0.0;
        public double STK_LEAK_POL_0 = 0.0;
        public double CV1_LEAK_POL_1 = 0.0;
        public double CV2_LEAK_POL_1 = 0.0;
        public double CV3_LEAK_POL_1 = 0.0;
        public double CV4_LEAK_POL_1 = 0.0;
        public double CV5_LEAK_POL_1 = 0.0;
        public double CV6_LEAK_POL_1 = 0.0;
        public double STK_LEAK_POL_1 = 0.0;

        public string FAULT1_STATUS = "0000";
        public string FAULT2_STATUS = "0000";
        public string FAULT3_STATUS = "0000";

        public double CV1_VRFY = 0.0;
        public double CV2_VRFY = 0.0;
        public double CV3_VRFY = 0.0;
        public double CV4_VRFY = 0.0;
        public double CV5_VRFY = 0.0;
        public double CV6_VRFY = 0.0;

        public long IC_TEMP = 0;

        /// <summary>
        /// FAULT1 STATUS
        /// </summary>
        public List<bool> i006E_data = new List<bool>();

        /// <summary>
        /// FAULT2 STATUS
        /// </summary>
        public List<bool> i006F_data = new List<bool>();

        /// <summary>
        /// FAULT3 STATUS
        /// </summary>
        public List<bool> i0070_data = new List<bool>();

        /// <summary>
        /// DEBUGGING
        /// </summary>
        public List<bool> i0072_data = new List<bool>();

        /// <summary>
        /// EVEN_CELL_OV
        /// </summary>
        public List<bool> i0073_data = new List<bool>();

        /// <summary>
        /// EVEN_CELL_UV
        /// </summary>
        public List<bool> i0074_data = new List<bool>();

        /// <summary>
        /// ODD_CELL_OV
        /// </summary>
        public List<bool> i0075_data = new List<bool>();

        /// <summary>
        /// ODD_CELL_UV
        /// </summary>
        public List<bool> i0076_data = new List<bool>();

        /// <summary>
        /// AN_OT_UT_FLT
        /// </summary>
        public List<bool> i0077_data = new List<bool>();

        /// <summary>
        /// GPIO_SHORT_Anx_OPEN_STS
        /// </summary>
        public List<bool> i0078_data = new List<bool>();

        #endregion


        #endregion

        #region Methods

        #region Ctor

        /// <summary>
        /// port=COM3 / baud=9600 / parity=none / data=8 / stop=1
        /// </summary>
        /// <param name="mw"></param>
        public CCMC(MainWindow mw)
        {
            this.mw = mw;

            InitializeComandList();

            mtx.WaitOne();

            if (mw.cmc_PortName == "")
            {
                mtx.ReleaseMutex();
                return;
            }

            #region initialize list

            if (mw.CMClist.Count == 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    mw.CMClist.Add(0);
                }
                for (int i = 0; i < 6; i++)
                {
                    mw.CMClist_Cell.Add(0);
                }
            }
            else
            {
                for (int i = 0; i < 30; i++)
                {
                    mw.CMClist[i] = 0;
                }
                for (int i = 0; i < 6; i++)
                {
                    mw.CMClist_Cell[i] = 0;
                }
            }

            #endregion

            #region 안씀
            //if (CellRefresher != null)
            //{
            //    if (CellRefresher.ThreadState == System.Threading.ThreadState.Running)
            //    {
            //        CellRefresher.Abort();
            //        Thread.Sleep(100);
            //    }
            //    CellRefresher = null;
            //}

            //CellRefresher = new Thread(() =>
            //    {
            //        while (true)
            //        {
            //            try
            //            {

            //                if (mw.CMClist.Count > 0)
            //                {
            //                    mw.CMClist_Cell[0] = mw.CMClist[1] * 0.0001525925;
            //                    mw.CMClist_Cell[1] = mw.CMClist[2] * 0.0001525925;
            //                    mw.CMClist_Cell[2] = mw.CMClist[3] * 0.0001525925;
            //                    mw.CMClist_Cell[3] = mw.CMClist[4] * 0.0001525925;
            //                    mw.CMClist_Cell[4] = mw.CMClist[5] * 0.0001525925;
            //                    mw.CMClist_Cell[5] = mw.CMClist[6] * 0.0001525925;
            //                    Console.WriteLine("Time elapsed: {0},{1}", stopwatch.Elapsed, mw.CMClist[1]);
            //                }
            //            }
            //            catch(Exception)
            //            {
            //            }
            //            Thread.Sleep(10);
            //        }
            //    });
            //CellRefresher.Start();
            #endregion

            //실행 PC에 원하는 comport가 없다면 바로 나가게
            if (!ExistPortName(mw.cmc_PortName))
            {
                mtx.ReleaseMutex();
                return;
            }

            mw.relays.RelayOff(mw.CMC_Relay);
            Thread.Sleep(1000);
            mw.relays.RelayOn(mw.CMC_Relay);
            mw.Sleep_Event(5000); //210419 Add by KYJ


            sp = new System.IO.Ports.SerialPort();
            sp.DataBits = 8;
            sp.BaudRate = 115200;
            sp.PortName = mw.cmc_PortName;
            sp.Parity = System.IO.Ports.Parity.None;
            sp.StopBits = System.IO.Ports.StopBits.One;
            sp.ReadBufferSize = 4096;

            //210323 wjs add port & relay num info log
            mw.LogState(LogType.Info, "CMC Port : " + mw.cmc_PortName + " CMC Relay Number : " + mw.CMC_Relay);

            try
            {
                sp.Open();
                if (sp.IsOpen)
                {

                    spStream = sp.BaseStream;
                    endloop = false;
                    readThread = new Thread(() => ReadThreadProc());
                    readThread.IsBackground = true;
                    readThread.Start();
                    isAlive = true;

                    if (setValuesThread != null)
                    {
                        setValuesThread.Abort();
                    }
                    setValuesThread = new Thread(new ThreadStart(setValues));
                    setValuesThread.Start();
                    stopwatch.Start();
                    mw.LogState(LogType.DEVICE_CHECK, "CMC - " + sp.PortName + " Open Success");

                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_cmc.Background = System.Windows.Media.Brushes.Green;
                    }));
                }
                else
                {
                    mw.LogState(LogType.DEVICE_CHECK, "CMC - " + sp.PortName + " Open Fail");

                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                    }));
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.DEVICE_CHECK, "CMC Exception", ec);

                mw.Dispatcher.BeginInvoke(new Action(() =>
                {
                    mw.contBt_cmc.Background = System.Windows.Media.Brushes.Red;
                }));
            }

            mtx.ReleaseMutex();
        }

        #endregion

        #region Data Read

        private void ReadThreadProc()
        {
            while (endloop == false)
            {
                _readDone.Reset();

                recv_result = spStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallBack), spStream);
                Thread.Sleep(10);
                _readDone.WaitOne();
            }
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            int bytesRead = 0;

            if (sp.IsOpen)
            {
                if (spStream.CanRead)
                {
                    bytesRead = spStream.EndRead(ar);
                }
            }

            if (bytesRead > 0)
            {
                byte[] received = new byte[bytesRead];
                Buffer.BlockCopy(buffer, 0, received, 0, bytesRead);

                try
                {
                    localDt = DateTime.Now;
                    var tet = Encoding.ASCII.GetString(received);

                    string[] stringSeparators = new string[] { "\n" };

                    string[] lines = tet.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    Recv_SetDic(lines);
                }
                catch (Exception tet)
                {
                    b_isRecieveOk = false;

                    mw.LogState(LogType.Fail, "CMC_PARSING_FAIL:", tet);
                }
                _readDone.Set();
            }
        }

        private void Recv_SetDic(string[] lines)
        {
            foreach (var item in lines)
            {
                if (!item.Contains("i"))
                {
                    //Console.WriteLine(item);
                    continue;
                }

                if (item.Length > 6)
                {
                    var subStr = item.Substring(1, 4);
                    if (!raw_cmcList.ContainsKey(subStr))
                    {
                        raw_cmcList.Add(subStr, item.Replace(subStr, "").Replace("i", ""));
                    }
                    else
                    {
                        var val = item.Replace(subStr, "").Replace("i", "");
                        if (val.Length == 4)
                            raw_cmcList[subStr] = val; 

                        if (subStr == "0072"||
                            subStr == "0073"||
                            subStr == "0074" ||
                            subStr == "0075" ||
                            subStr == "0076" ||
                            subStr == "0077" ||
                            subStr == "0078" ||
                            subStr == "0079" ||
                            subStr == "007A" ||
                            subStr == "007B" ||
                            subStr == "007C" ||
                            subStr == "007D" ||
                            subStr == "007E" ||
                            subStr == "007F" ||
                            subStr == "0080" ||
                            subStr == "0081" ||
                            subStr == "0082" ||
                            subStr == "0083" ||
                            subStr == "0084" ||
                            subStr == "0085" ||
                            subStr == "0086" ||
                            subStr == "00C7" ||
                            subStr == "007C")
                        { 
                            Console.WriteLine("{0},{1}", subStr, item);
                        }

                        if (subStr == "012D")
                        {
                            mw.CMClist[1] = GetHexToLong(CheckKeyNReturnVal("012D"));// * 0.0001525925;
                            mw.CMClist_Cell[0] = mw.CMClist[1] * 0.0001525925;
                            //Console.WriteLine("Time elapsed: {0},{1}", stopwatch.Elapsed, mw.CMClist_Cell[0]);
                        }

                        if (subStr == "012E")
                        {
                            mw.CMClist[2] = GetHexToLong(CheckKeyNReturnVal("012E"));// * 0.0001525925;
                            mw.CMClist_Cell[1] = mw.CMClist[2] * 0.0001525925;
                            //Console.WriteLine("Time elapsed: {0},{1}", stopwatch.Elapsed, mw.CMClist_Cell[1]);
                        }

                        if (subStr == "012F")
                        {
                            mw.CMClist[3] = GetHexToLong(CheckKeyNReturnVal("012F"));// * 0.0001525925;
                            mw.CMClist_Cell[2] = mw.CMClist[3] * 0.0001525925;
                            //Console.WriteLine("Time elapsed: {0},{1}", stopwatch.Elapsed, mw.CMClist_Cell[2]);
                        }

                        if (subStr == "0130")
                        {
                            mw.CMClist[4] = GetHexToLong(CheckKeyNReturnVal("0130"));// * 0.0001525925;
                            mw.CMClist_Cell[3] = mw.CMClist[4] * 0.0001525925;
                            //Console.WriteLine("Time elapsed: {0},{1}", stopwatch.Elapsed, mw.CMClist_Cell[3]);
                        }

                        if (subStr == "0131")
                        {
                            mw.CMClist[5] = GetHexToLong(CheckKeyNReturnVal("0131"));// * 0.0001525925;
                            mw.CMClist_Cell[4] = mw.CMClist[5] * 0.0001525925;
                            //Console.WriteLine("Time elapsed: {0},{1}", stopwatch.Elapsed, mw.CMClist_Cell[4]);
                        }

                        if (subStr == "0132")
                        {
                            mw.CMClist[6] = GetHexToLong(CheckKeyNReturnVal("0132"));// * 0.0001525925;
                            mw.CMClist_Cell[5] = mw.CMClist[6] * 0.0001525925;
                            //Console.WriteLine("Time elapsed: {0},{1}", stopwatch.Elapsed, mw.CMClist_Cell[5]);
                        }

                        //    //mw.LogState(LogType.Info, "Cell1: " + mw.CMClist[1] * 0.0001525925);
                        //    mw.CMClist[2] = GetHexToLong(CheckKeyNReturnVal("012E"));// * 0.0001525925;
                        //    mw.CMClist[3] = GetHexToLong(CheckKeyNReturnVal("012F"));// * 0.0001525925;
                        //    mw.CMClist[4] = GetHexToLong(CheckKeyNReturnVal("0130"));// * 0.0001525925;
                        //    mw.CMClist[5] = GetHexToLong(CheckKeyNReturnVal("0131"));// * 0.0001525925;
                        //    mw.CMClist[6] = GetHexToLong(CheckKeyNReturnVal("0132"));// *0.0001525925;

                        //    mw.CMClist_Cell[1] = mw.CMClist[2] * 0.0001525925;
                        //    mw.CMClist_Cell[2] = mw.CMClist[3] * 0.0001525925;
                        //    mw.CMClist_Cell[3] = mw.CMClist[4] * 0.0001525925;
                        //    mw.CMClist_Cell[4] = mw.CMClist[5] * 0.0001525925;
                        //    mw.CMClist_Cell[5] = mw.CMClist[6] * 0.0001525925;


                        //if (subStr == "012D")
                        //{
                        ////    //flag1 = true;
                        ////    //var val = item.Replace(subStr, "").Replace("i", "");
                        ////    mw.CMClist[1] = GetHexToLong(item.Substring(5, 4));
                        //}
                    }
                    //Console.WriteLine(item);
                }
                else
                {
                }
            }
        }

        private void setValues()
        {
            try
            {
                while (isAlive)
                {
                    Thread.Sleep(1);

                    //Stopwatch stopwatch = new Stopwatch();
                    //stopwatch.Start();

                    mw.cmc1mv = GetHexToLong(CheckKeyNReturnVal("0514"));//* 0.0024415;
                    mw.cmc2mv = GetHexToLong(CheckKeyNReturnVal("08FC"));//* 0.0024415;
                    mw.cmc1cv1 = GetHexToLong(CheckKeyNReturnVal("0515"));// * 0.0001525925;
                    mw.cmc2cv1 = GetHexToLong(CheckKeyNReturnVal("08FD"));// *0.0001525925;
                    mw.CMClist[0] = GetHexToLong(CheckKeyNReturnVal("012C"));// * 0.0024415;
                    //mw.LogState(LogType.Info, "Cell1: " + mw.CMClist[1] * 0.0001525925);
                    //mw.CMClist[1] = GetHexToLong(CheckKeyNReturnVal("012D"));// * 0.0001525925;
                    //mw.CMClist[2] = GetHexToLong(CheckKeyNReturnVal("012E"));// * 0.0001525925;
                    //mw.CMClist[3] = GetHexToLong(CheckKeyNReturnVal("012F"));// * 0.0001525925;
                    //mw.CMClist[4] = GetHexToLong(CheckKeyNReturnVal("0130"));// * 0.0001525925;
                    //mw.CMClist[5] = GetHexToLong(CheckKeyNReturnVal("0131"));// * 0.0001525925;
                    //mw.CMClist[6] = GetHexToLong(CheckKeyNReturnVal("0132"));// * 0.0001525925;

                    //210312 wjs add mw.CMClist[8]
                    mw.CMClist[8] = GetHexToLong(CheckKeyNReturnVal("013C")); // //210312 WJS 얘는 페이스리프트에서는 하드웨어버전1번이고 나머지 애들은 안씀

                    mw.CMClist[9] = GetHexToLong(CheckKeyNReturnVal("013D"));
                    mw.CMClist[10] = GetHexToLong(CheckKeyNReturnVal("013E"));
                    mw.CMClist[11] = GetHexToLong(CheckKeyNReturnVal("013F")); //210312 WJS 얘는 페이스리프트에서는 서미스터 3번이고 나머지 애들은 하드웨어버전 3번



                    //var rs12 = 0.0;
                    //CalculateToMC33772ModuleTemp(GetHexToLong(CheckKeyNReturnVal("0140")), out rs12);
                    mw.CMClist[12] = GetHexToLong(CheckKeyNReturnVal("0140"));

                    //var rs13 = 0.0;
                    //CalculateToMC33772ModuleTemp(GetHexToLong(CheckKeyNReturnVal("0141")), out rs13);
                    mw.CMClist[13] = GetHexToLong(CheckKeyNReturnVal("0141"));

                    //var rs14 = 0.0;
                    //CalculateToMC33772PCB(GetHexToLong(CheckKeyNReturnVal("0142")), out rs14);
                    mw.CMClist[14] = GetHexToLong(CheckKeyNReturnVal("0142"));

                    //CheckNWrite(CheckKeyNReturnVal("00D2").ToCharArray(), "1FFF", "e127"); 
                    //CheckNWrite(CheckKeyNReturnVal("00D3").ToCharArray(), "FE7F", "e128"); 
                    //CheckNWrite(CheckKeyNReturnVal("00D4").ToCharArray(), "E03F", "e129"); 
                    //CheckNWrite(CheckKeyNReturnVal("00D5").ToCharArray(), "199F", "e12A"); 
                    //CheckNWrite(CheckKeyNReturnVal("00D6").ToCharArray(), "FF36", "e12B"); 
                    //CheckNWrite(CheckKeyNReturnVal("00D7").ToCharArray(), "A03F", "e12C"); 

                    CheckBalanceSwitch();
                    CheckOverVolt();
                    CheckUnderVolt();
                    CheckFaultTemp();
                    CheckWireDetect();
                    CheckBalance();
                    CheckSM("0072", i0072_data);
                    CheckSM("0073", i0073_data);
                    CheckSM("0074", i0074_data);
                    CheckSM("0075", i0075_data);
                    CheckSM("0076", i0076_data);
                    CheckSM("0077", i0077_data);
                    CheckSM("0078", i0078_data);


                    CheckSM("006E", i006E_data);
                    CheckSM("006F", i006F_data);
                    CheckSM("0070", i0070_data);

                    FAULT1_STATUS = CheckKeyNReturnVal("006E");
                    FAULT2_STATUS = CheckKeyNReturnVal("006F");
                    FAULT3_STATUS = CheckKeyNReturnVal("0070");

                    IC_TEMP = GetHexToLong(CheckKeyNReturnVal("0143"));

                    var tb1 = GetHexToLong(CheckKeyNReturnVal("0173"));
                    if (tb1 != 0) { raw_cmcList.Remove("0173"); if (mw.cbshort1Volt == -1) { mw.cbshort1Volt = tb1; mw.LogState(LogType.CMC_RECV, "cbshort1Volt:" + tb1 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); } }

                    var tb2 = GetHexToLong(CheckKeyNReturnVal("0174"));
                    if (tb2 != 0) { raw_cmcList.Remove("0174"); if (mw.cbshort2Volt == -1) { mw.cbshort2Volt = tb2; mw.LogState(LogType.CMC_RECV, "cbshort2Volt:" + tb2 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); } }

                    var tb3 = GetHexToLong(CheckKeyNReturnVal("0175"));
                    if (tb3 != 0) { raw_cmcList.Remove("0175"); if (mw.cbshort3Volt == -1) { mw.cbshort3Volt = tb3; mw.LogState(LogType.CMC_RECV, "cbshort3Volt:" + tb3 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); } }

                    var tb4 = GetHexToLong(CheckKeyNReturnVal("0176"));
                    if (tb4 != 0) { raw_cmcList.Remove("0176"); if (mw.cbshort4Volt == -1) { mw.cbshort4Volt = tb4; mw.LogState(LogType.CMC_RECV, "cbshort4Volt:" + tb4 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); } }

                    var tb5 = GetHexToLong(CheckKeyNReturnVal("0177"));
                    if (tb5 != 0) { raw_cmcList.Remove("0177"); if (mw.cbshort5Volt == -1) { mw.cbshort5Volt = tb5; mw.LogState(LogType.CMC_RECV, "cbshort5Volt:" + tb5 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); } }

                    var tb6 = GetHexToLong(CheckKeyNReturnVal("0178"));
                    if (tb6 != 0) { raw_cmcList.Remove("0178"); if (mw.cbshort6Volt == -1) { mw.cbshort6Volt = tb6; mw.LogState(LogType.CMC_RECV, "cbshort6Volt:" + tb6 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); } }


                    var tb7 = GetHexToLong(CheckKeyNReturnVal("017D"));
                    if (tb7 != 0) { if (CheckKeyNReturnVal("017D").Length == 4 && mw.ow1Volt == 0) { mw.ow1Volt = tb7; mw.LogState(LogType.CMC_RECV, GetHexToLong(CheckKeyNReturnVal("017D")) + "ow1Volt:" + tb7 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); raw_cmcList.Remove("017D"); } }

                    var tb8 = GetHexToLong(CheckKeyNReturnVal("017E"));
                    if (tb8 != 0) { if (CheckKeyNReturnVal("017E").Length == 4 && mw.ow2Volt == 0) { mw.ow2Volt = tb8; mw.LogState(LogType.CMC_RECV, GetHexToLong(CheckKeyNReturnVal("017E")) + "ow2Volt:" + tb8 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); raw_cmcList.Remove("017E"); } }

                    var tb9 = GetHexToLong(CheckKeyNReturnVal("017F"));
                    if (tb9 != 0) { if (CheckKeyNReturnVal("017F").Length == 4 && mw.ow3Volt == 0) { mw.ow3Volt = tb9; mw.LogState(LogType.CMC_RECV, GetHexToLong(CheckKeyNReturnVal("017F")) + "ow3Volt:" + tb9 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); raw_cmcList.Remove("017F"); } }

                    var tb10 = GetHexToLong(CheckKeyNReturnVal("0180"));
                    if (tb10 != 0) { if (CheckKeyNReturnVal("0180").Length == 4 && mw.ow4Volt == 0) { mw.ow4Volt = tb10; mw.LogState(LogType.CMC_RECV, GetHexToLong(CheckKeyNReturnVal("0180")) + "ow4Volt:" + tb10 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); raw_cmcList.Remove("0180"); } }

                    var tb11 = GetHexToLong(CheckKeyNReturnVal("0181"));
                    if (tb11 != 0) { if (CheckKeyNReturnVal("0181").Length == 4 && mw.ow5Volt == 0) { mw.ow5Volt = tb11; mw.LogState(LogType.CMC_RECV, GetHexToLong(CheckKeyNReturnVal("0181")) + "ow5Volt:" + tb11 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); raw_cmcList.Remove("0181"); } }

                    var tb12 = GetHexToLong(CheckKeyNReturnVal("0182"));
                    if (tb12 != 0) { if (CheckKeyNReturnVal("0182").Length == 4 && mw.ow6Volt == 0) { mw.ow6Volt = tb12; mw.LogState(LogType.CMC_RECV, GetHexToLong(CheckKeyNReturnVal("0182")) + "ow6Volt:" + tb12 + "/Time elapsed: " + mw.publicWatch.Elapsed, null, true); raw_cmcList.Remove("0182"); } }

                    var str = CheckKeyNReturnVal("0096");

                    if (str != "0")
                    {
                        //mw.LogState(LogType.CMC_RECV, str + "/Length:" + str.Length + "/Time elapsed: " + mw.publicWatch.Elapsed);
                        mw.publicWatch.Stop();

                        mw.cmcGuid = str;
                        raw_cmcList.Remove("0096");
                    }

                    str = CheckKeyNReturnVal("0960");

                    if (str != "0")
                    {
                        //mw.LogState(LogType.CMC_RECV, str + "/Length:" + str.Length + "/Time elapsed: " + mw.publicWatch.Elapsed);
                        mw.publicWatch.Stop();

                        mw.cmcGuid = str;
                        raw_cmcList.Remove("0960");
                    }


                    CV1_LEAK_POL_0 = GetHexToLong(CheckKeyNReturnVal("0079"));
                    CV2_LEAK_POL_0 = GetHexToLong(CheckKeyNReturnVal("007A"));
                    CV3_LEAK_POL_0 = GetHexToLong(CheckKeyNReturnVal("007B"));
                    CV4_LEAK_POL_0 = GetHexToLong(CheckKeyNReturnVal("007C"));
                    CV5_LEAK_POL_0 = GetHexToLong(CheckKeyNReturnVal("007D"));
                    CV6_LEAK_POL_0 = GetHexToLong(CheckKeyNReturnVal("007E"));
                    STK_LEAK_POL_0 = GetHexToLong(CheckKeyNReturnVal("007F"));
                    CV1_LEAK_POL_1 = GetHexToLong(CheckKeyNReturnVal("0080"));
                    CV2_LEAK_POL_1 = GetHexToLong(CheckKeyNReturnVal("0081"));
                    CV3_LEAK_POL_1 = GetHexToLong(CheckKeyNReturnVal("0082"));
                    CV4_LEAK_POL_1 = GetHexToLong(CheckKeyNReturnVal("0083"));
                    CV5_LEAK_POL_1 = GetHexToLong(CheckKeyNReturnVal("0084"));
                    CV6_LEAK_POL_1 = GetHexToLong(CheckKeyNReturnVal("0085"));
                    STK_LEAK_POL_1 = GetHexToLong(CheckKeyNReturnVal("0086"));

                    var tempVal = 0.0;
                    if (SetDataOrNot("0087", out tempVal)) CV1_VRFY = tempVal;
                    if (SetDataOrNot("0088", out tempVal)) CV2_VRFY = tempVal;
                    if (SetDataOrNot("0089", out tempVal)) CV3_VRFY = tempVal;
                    if (SetDataOrNot("008A", out tempVal)) CV4_VRFY = tempVal;
                    if (SetDataOrNot("008B", out tempVal)) CV5_VRFY = tempVal;
                    if (SetDataOrNot("008C", out tempVal)) CV6_VRFY = tempVal;

                    //mw.cbshort2Volt = GetHexToLong(CheckKeyNReturnVal("0174")) * 0.0001525925;
                    //mw.cbshort3Volt = GetHexToLong(CheckKeyNReturnVal("0175")) * 0.0001525925;
                    //mw.cbshort4Volt = GetHexToLong(CheckKeyNReturnVal("0176")) * 0.0001525925;
                    //mw.cbshort5Volt = GetHexToLong(CheckKeyNReturnVal("0177")) * 0.0001525925;
                    //mw.cbshort6Volt = GetHexToLong(CheckKeyNReturnVal("0178")) * 0.0001525925;


                    //mw.ow1Volt = GetHexToLong(CheckKeyNReturnVal("017D")) * 0.0001525925;
                    //mw.ow2Volt = GetHexToLong(CheckKeyNReturnVal("017E")) * 0.0001525925;
                    //mw.ow3Volt = GetHexToLong(CheckKeyNReturnVal("017F")) * 0.0001525925;
                    //mw.ow4Volt = GetHexToLong(CheckKeyNReturnVal("0180")) * 0.0001525925;
                    //mw.ow5Volt = GetHexToLong(CheckKeyNReturnVal("0181")) * 0.0001525925;
                    //mw.ow6Volt = GetHexToLong(CheckKeyNReturnVal("0182")) * 0.0001525925;

                    //stopwatch.Stop();

                    //// Write result.
                    //Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
                }
            }
            catch (Exception ex)
            {
            }

            #region Fault masking
            //    } break;
            //case "006E"://fault1, e124 폴트플래그는 없으면 뜨지않는다
            //    {
            //        if (s.Substring(5, 4) == "404A" || s.Substring(5, 4) == "004A")//open_Flt
            //        {
            //            //nsleepTrigger1 = true;
            //        }
            //        else
            //        {
            //            CheckNWrite(s.Substring(5, 4).ToCharArray(), "004A", "e124");
            //        }
            //    } break;
            //case "006F"://fault2, e125
            //    {
            //        if (s.Substring(5, 4) == "0000")//open_Flt
            //        {
            //            //nsleepTrigger2 = true;
            //        }
            //        else
            //        {
            //            CheckNWrite(s.Substring(5, 4).ToCharArray(), "0000", "e125");
            //        }
            //    } break;
            //case "0070"://fault2, e126
            //    {
            //        if (s.Substring(5, 4) == "8000")//open_Flt
            //        {
            //            //nsleepTrigger3 = true;
            //        }
            //        else
            //        {
            //            CheckNWrite(s.Substring(5, 4).ToCharArray(), "8000", "e126");
            //        }
            //    } break;
            //default: break;
            #endregion
        }

        private void CheckOverVolt()
        {
            var bso = CheckKeyNReturnVal("0064");
            if (bso != "0")
            {
                if (bso == "0000")//open_Flt f00660000
                {
                    //CT0_OV = false;
                    CT1_OV = false;
                    CT2_OV = false;
                    CT3_OV = false;
                    CT4_OV = false;
                    CT5_OV = false;
                    CT6_OV = false;
                }
                else
                {
                    var temp1 = Convert.ToString(Convert.ToInt32(bso, 16), 2);

                    while (temp1.Length < 16)
                    {
                        temp1 = "0" + temp1;
                    }

                    var arr = temp1.ToArray();

                    //CT0_OV = arr[0] == '1' ? true : false;
                    CT1_OV = arr[15] == '1' ? true : false;
                    CT2_OV = arr[14] == '1' ? true : false;
                    CT3_OV = arr[13] == '1' ? true : false;
                    CT4_OV = arr[12] == '1' ? true : false;
                    CT5_OV = arr[11] == '1' ? true : false;
                    CT6_OV = arr[10] == '1' ? true : false;

                }
            }
        }

        private void CheckUnderVolt()
        {
            var bso = CheckKeyNReturnVal("0065");
            if (bso != "0")
            {
                if (bso == "0000")//open_Flt f00660000
                {
                    //CT0_UV = false;
                    CT1_UV = false;
                    CT2_UV = false;
                    CT3_UV = false;
                    CT4_UV = false;
                    CT5_UV = false;
                    CT6_UV = false;
                }
                else
                {
                    var temp1 = Convert.ToString(Convert.ToInt32(bso, 16), 2);

                    while (temp1.Length < 16)
                    {
                        temp1 = "0" + temp1;
                    }

                    var arr = temp1.ToArray();

                    //CT0_UV = arr[0] == '1' ? true : false;
                    CT1_UV = arr[15] == '1' ? true : false;
                    CT2_UV = arr[14] == '1' ? true : false;
                    CT3_UV = arr[13] == '1' ? true : false;
                    CT4_UV = arr[12] == '1' ? true : false;
                    CT5_UV = arr[11] == '1' ? true : false;
                    CT6_UV = arr[10] == '1' ? true : false;

                }
            }
        }

        private void CheckFaultTemp()
        {
            var bso = CheckKeyNReturnVal("006A");
            if (bso != "0")
            {
                if (bso == "0000")//open_Flt f00660000
                {
                    //AN0_OT = false;
                    //AN1_OT = false;
                    //AN2_OT = false;
                    //AN3_OT = false;
                    AN4_OT = false;
                    AN5_OT = false;
                    AN6_OT = false;

                    //AN0_UT = false;
                    //AN1_UT = false;
                    //AN2_UT = false;
                    //AN3_UT = false;
                    AN4_UT = false;
                    AN5_UT = false;
                    AN6_UT = false;
                }
                else
                {
                    var temp1 = Convert.ToString(Convert.ToInt32(bso, 16), 2);

                    while (temp1.Length < 16)
                    {
                        temp1 = "0" + temp1;
                    }

                    var arr = temp1.ToArray();

                    //AN0_OT = arr[7] == '1' ? true : false;
                    //AN1_OT = arr[6] == '1' ? true : false;
                    //AN2_OT = arr[5] == '1' ? true : false;
                    //AN3_OT = arr[4] == '1' ? true : false;
                    AN4_OT = arr[3] == '1' ? true : false;
                    AN5_OT = arr[2] == '1' ? true : false;
                    AN6_OT = arr[1] == '1' ? true : false;

                    //AN0_UT = arr[15] == '1' ? true : false;
                    //AN1_UT = arr[14] == '1' ? true : false;
                    //AN2_UT = arr[13] == '1' ? true : false;
                    //AN3_UT = arr[12] == '1' ? true : false;
                    AN4_UT = arr[11] == '1' ? true : false;
                    AN5_UT = arr[10] == '1' ? true : false;
                    AN6_UT = arr[9] == '1' ? true : false;

                }
            }

        }

        private void CheckWireDetect()
        {
            var bso = CheckKeyNReturnVal("017C");
            if (bso != "0")
            {
                if (bso == "0000")
                {
                    CT0_OPEN_WIRE = false;
                    CT1_OPEN_WIRE = false;
                    CT2_OPEN_WIRE = false;
                    CT3_OPEN_WIRE = false;
                    CT4_OPEN_WIRE = false;
                    CT5_OPEN_WIRE = false;
                    CT6_OPEN_WIRE = false;
                }
                else
                {
                    var temp1 = Convert.ToString(Convert.ToInt32(bso, 16), 2);

                    while (temp1.Length < 16)
                    {
                        temp1 = "0" + temp1;
                    }

                    var arr = temp1.ToArray();

                    CT0_OPEN_WIRE = arr[15] == '1' ? true : false;
                    CT1_OPEN_WIRE = arr[14] == '1' ? true : false;
                    CT2_OPEN_WIRE = arr[13] == '1' ? true : false;
                    CT3_OPEN_WIRE = arr[12] == '1' ? true : false;
                    CT4_OPEN_WIRE = arr[11] == '1' ? true : false;
                    CT5_OPEN_WIRE = arr[10] == '1' ? true : false;
                    CT6_OPEN_WIRE = arr[9] == '1' ? true : false;
                }
            }
        }

        private void CheckBalance()
        {
            //var bso = CheckKeyNReturnVal("0068");
            var bso = CheckKeyNReturnVal("00BF");
            if (bso != "0")
            {
                if (bso == "0000")
                {
                    CT1_BAL = false;
                    CT2_BAL = false;
                    CT3_BAL = false;
                    CT4_BAL = false;
                    CT5_BAL = false;
                    CT6_BAL = false;

                    rawbyte_bal = "0000000000000000";
                    rawbyte_bal_reverse = "0000000000000000";
                    cbFlag = false;
                }
                else
                {
                    var temp1 = Convert.ToString(Convert.ToInt32(bso, 16), 2);

                    while (temp1.Length < 16)
                    {
                        temp1 = "0" + temp1;
                    }

                    var arr = temp1.ToArray();
                    var arr1 = arr.Reverse();
                    var arr2 = arr1.ToArray();

                    var sb = new StringBuilder();
                    foreach (var c in arr2)
                    {
                        sb.Append(c);
                    }


                    rawbyte_bal = temp1;
                    rawbyte_bal_reverse = sb.ToString();

                    CT1_BAL = arr2[0] == '1' ? true : false;
                    CT2_BAL = arr2[1] == '1' ? true : false;
                    CT3_BAL = arr2[2] == '1' ? true : false;
                    CT4_BAL = arr2[3] == '1' ? true : false;
                    CT5_BAL = arr2[4] == '1' ? true : false;
                    CT6_BAL = arr2[5] == '1' ? true : false;

                    this.CV1_CB = GetHexToLong(CheckKeyNReturnVal("0169"));
                    this.CV2_CB = GetHexToLong(CheckKeyNReturnVal("016A"));
                    this.CV3_CB = GetHexToLong(CheckKeyNReturnVal("016B"));
                    this.CV4_CB = GetHexToLong(CheckKeyNReturnVal("016C"));
                    this.CV5_CB = GetHexToLong(CheckKeyNReturnVal("016D"));
                    this.CV6_CB = GetHexToLong(CheckKeyNReturnVal("016E"));

                    cbFlag = true;
                    //mw.LogState(LogType.CMC_RECV, temp1+"/Time elapsed: " + mw.publicWatch.Elapsed);
                }
            }
        }

        private void CheckBalanceSwitch()
        {
            #region balanceSwitch_open

            var bso = CheckKeyNReturnVal("0066");
            if (bso != "0")
            {
                if (bso == "0000")//open_Flt f00660000
                {
                    CB1_OPEN_FLT = false;
                    CB2_OPEN_FLT = false;
                    CB3_OPEN_FLT = false;
                    CB4_OPEN_FLT = false;
                    CB5_OPEN_FLT = false;
                    CB6_OPEN_FLT = false;
                }
                else
                {
                    var temp1 = Convert.ToString(Convert.ToInt32(bso, 16), 2);

                    while (temp1.Length < 16)
                    {
                        temp1 = "0" + temp1;
                    }

                    var arr = temp1.ToArray();

                    CB1_OPEN_FLT = arr[15] == '1' ? true : false;
                    CB2_OPEN_FLT = arr[14] == '1' ? true : false;
                    CB3_OPEN_FLT = arr[13] == '1' ? true : false;
                    CB4_OPEN_FLT = arr[12] == '1' ? true : false;
                    CB5_OPEN_FLT = arr[11] == '1' ? true : false;
                    CB6_OPEN_FLT = arr[10] == '1' ? true : false;

                }
            }

            #endregion

            #region balanceSwitch_short

            var bs = CheckKeyNReturnVal("0067");
            if (bs != "0")
            {
                if (bs == "0000")//short_Flt
                {
                    CB1_SHORT_FLT = false;
                    CB2_SHORT_FLT = false;
                    CB3_SHORT_FLT = false;
                    CB4_SHORT_FLT = false;
                    CB5_SHORT_FLT = false;
                    CB6_SHORT_FLT = false;
                }
                else
                {
                    //hex를 binary로 바꿔서 
                    //해당비트에 1이 있는지 확인해야함

                    var temp1 = Convert.ToString(Convert.ToInt32(bs, 16), 2);

                    while (temp1.Length < 16)
                    {
                        temp1 = "0" + temp1;
                    }
                    var arr = temp1.ToArray();

                    CB1_SHORT_FLT = arr[15] == '1' ? true : false;
                    CB2_SHORT_FLT = arr[14] == '1' ? true : false;
                    CB3_SHORT_FLT = arr[13] == '1' ? true : false;
                    CB4_SHORT_FLT = arr[12] == '1' ? true : false;
                    CB5_SHORT_FLT = arr[11] == '1' ? true : false;
                    CB6_SHORT_FLT = arr[10] == '1' ? true : false;
                }
            }
            #endregion
        }

        public void CheckSM(String id,List<bool> list)
        {
            //var bso = CheckKeyNReturnVal("0068");
            var bso = CheckKeyNReturnVal(id);
            if (bso != "0")
            {
                if (bso == "0000")
                {
                }
                else
                {
                    var temp1 = Convert.ToString(Convert.ToInt32(bso, 16), 2);

                    while (temp1.Length < 16)
                    {
                        temp1 = "0" + temp1;
                    }

                    while(list.Count <16)
                    {
                        list.Add(false);
                    }

                    var arr = temp1.ToArray().Reverse().ToArray();

                    for (int i = 0; i < arr.Length; i++)
                    {
                        list[i] = arr[i] == '1' ? true : false;
                    }
                    
                }
                //???
            }
        }

        #endregion

        #region Resistor to TEMP Calc

        public void CalculateToMC33772PCB(double Resistance, out double Temp)
        {
            Temp = 255;

            if (Resistance < 1590) { Temp = -255; return; }
            if (1590 <= Resistance && Resistance < 1695) { Temp = -40; return; }
            if (1695 <= Resistance && Resistance < 1783) { Temp = -39; return; }
            if (1783 <= Resistance && Resistance < 1875) { Temp = -38; return; }
            if (1875 <= Resistance && Resistance < 1972) { Temp = -37; return; }
            if (1972 <= Resistance && Resistance < 2073) { Temp = -36; return; }
            if (2073 <= Resistance && Resistance < 2177) { Temp = -35; return; }
            if (2177 <= Resistance && Resistance < 2286) { Temp = -34; return; }
            if (2286 <= Resistance && Resistance < 2399) { Temp = -33; return; }
            if (2399 <= Resistance && Resistance < 2517) { Temp = -32; return; }
            if (2517 <= Resistance && Resistance < 2638) { Temp = -31; return; }
            if (2638 <= Resistance && Resistance < 2765) { Temp = -30; return; }
            if (2765 <= Resistance && Resistance < 2896) { Temp = -29; return; }
            if (2896 <= Resistance && Resistance < 3033) { Temp = -28; return; }
            if (3033 <= Resistance && Resistance < 3174) { Temp = -27; return; }
            if (3174 <= Resistance && Resistance < 3320) { Temp = -26; return; }
            if (3320 <= Resistance && Resistance < 3471) { Temp = -25; return; }
            if (3471 <= Resistance && Resistance < 3627) { Temp = -24; return; }
            if (3627 <= Resistance && Resistance < 3788) { Temp = -23; return; }
            if (3788 <= Resistance && Resistance < 3954) { Temp = -22; return; }
            if (3954 <= Resistance && Resistance < 4126) { Temp = -21; return; }
            if (4126 <= Resistance && Resistance < 4302) { Temp = -20; return; }
            if (4302 <= Resistance && Resistance < 4485) { Temp = -19; return; }
            if (4485 <= Resistance && Resistance < 4672) { Temp = -18; return; }
            if (4672 <= Resistance && Resistance < 4865) { Temp = -17; return; }
            if (4865 <= Resistance && Resistance < 5063) { Temp = -16; return; }
            if (5063 <= Resistance && Resistance < 5267) { Temp = -15; return; }
            if (5267 <= Resistance && Resistance < 5476) { Temp = -14; return; }
            if (5476 <= Resistance && Resistance < 5690) { Temp = -13; return; }
            if (5690 <= Resistance && Resistance < 5909) { Temp = -12; return; }
            if (5909 <= Resistance && Resistance < 6134) { Temp = -11; return; }
            if (6134 <= Resistance && Resistance < 6365) { Temp = -10; return; }
            if (6365 <= Resistance && Resistance < 6599) { Temp = -9; return; }
            if (6599 <= Resistance && Resistance < 6839) { Temp = -8; return; }
            if (6839 <= Resistance && Resistance < 7084) { Temp = -7; return; }
            if (7084 <= Resistance && Resistance < 7334) { Temp = -6; return; }
            if (7334 <= Resistance && Resistance < 7589) { Temp = -5; return; }
            if (7589 <= Resistance && Resistance < 7849) { Temp = -4; return; }
            if (7849 <= Resistance && Resistance < 8113) { Temp = -3; return; }
            if (8113 <= Resistance && Resistance < 8381) { Temp = -2; return; }
            if (8381 <= Resistance && Resistance < 8653) { Temp = -1; return; }
            if (8653 <= Resistance && Resistance < 8929) { Temp = 0; return; }
            if (8929 <= Resistance && Resistance < 9210) { Temp = 1; return; }
            if (9210 <= Resistance && Resistance < 9494) { Temp = 2; return; }
            if (9494 <= Resistance && Resistance < 9782) { Temp = 3; return; }
            if (9782 <= Resistance && Resistance < 10072) { Temp = 4; return; }
            if (10072 <= Resistance && Resistance < 10366) { Temp = 5; return; }
            if (10366 <= Resistance && Resistance < 10663) { Temp = 6; return; }
            if (10663 <= Resistance && Resistance < 10961) { Temp = 7; return; }
            if (10961 <= Resistance && Resistance < 11264) { Temp = 8; return; }
            if (11264 <= Resistance && Resistance < 11567) { Temp = 9; return; }
            if (11567 <= Resistance && Resistance < 11873) { Temp = 10; return; }
            if (11873 <= Resistance && Resistance < 12182) { Temp = 11; return; }
            if (12182 <= Resistance && Resistance < 12490) { Temp = 12; return; }
            if (12490 <= Resistance && Resistance < 12803) { Temp = 13; return; }
            if (12803 <= Resistance && Resistance < 13113) { Temp = 14; return; }
            if (13113 <= Resistance && Resistance < 13427) { Temp = 15; return; }
            if (13427 <= Resistance && Resistance < 13739) { Temp = 16; return; }
            if (13739 <= Resistance && Resistance < 14052) { Temp = 17; return; }
            if (14052 <= Resistance && Resistance < 14366) { Temp = 18; return; }
            if (14366 <= Resistance && Resistance < 14678) { Temp = 19; return; }
            if (14678 <= Resistance && Resistance < 14990) { Temp = 20; return; }
            if (14990 <= Resistance && Resistance < 15302) { Temp = 21; return; }
            if (15302 <= Resistance && Resistance < 15612) { Temp = 22; return; }
            if (15612 <= Resistance && Resistance < 15923) { Temp = 23; return; }
            if (15923 <= Resistance && Resistance < 16230) { Temp = 24; return; }
            if (16230 <= Resistance && Resistance < 16536) { Temp = 25; return; }
            if (16536 <= Resistance && Resistance < 16841) { Temp = 26; return; }
            if (16841 <= Resistance && Resistance < 17145) { Temp = 27; return; }
            if (17145 <= Resistance && Resistance < 17446) { Temp = 28; return; }
            if (17446 <= Resistance && Resistance < 17745) { Temp = 29; return; }
            if (17745 <= Resistance && Resistance < 18041) { Temp = 30; return; }
            if (18041 <= Resistance && Resistance < 18335) { Temp = 31; return; }
            if (18335 <= Resistance && Resistance < 18626) { Temp = 32; return; }
            if (18626 <= Resistance && Resistance < 18914) { Temp = 33; return; }
            if (18914 <= Resistance && Resistance < 19199) { Temp = 34; return; }
            if (19199 <= Resistance && Resistance < 19480) { Temp = 35; return; }
            if (19480 <= Resistance && Resistance < 19758) { Temp = 36; return; }
            if (19758 <= Resistance && Resistance < 20033) { Temp = 37; return; }
            if (20033 <= Resistance && Resistance < 20304) { Temp = 38; return; }
            if (20304 <= Resistance && Resistance < 20572) { Temp = 39; return; }
            if (20572 <= Resistance && Resistance < 20836) { Temp = 40; return; }
            if (20836 <= Resistance && Resistance < 21096) { Temp = 41; return; }
            if (21096 <= Resistance && Resistance < 21352) { Temp = 42; return; }
            if (21352 <= Resistance && Resistance < 21603) { Temp = 43; return; }
            if (21603 <= Resistance && Resistance < 21852) { Temp = 44; return; }
            if (21852 <= Resistance && Resistance < 22096) { Temp = 45; return; }
            if (22096 <= Resistance && Resistance < 22336) { Temp = 46; return; }
            if (22336 <= Resistance && Resistance < 22572) { Temp = 47; return; }
            if (22572 <= Resistance && Resistance < 22804) { Temp = 48; return; }
            if (22804 <= Resistance && Resistance < 23031) { Temp = 49; return; }
            if (23031 <= Resistance && Resistance < 23254) { Temp = 50; return; }
            if (23254 <= Resistance && Resistance < 23474) { Temp = 51; return; }
            if (23474 <= Resistance && Resistance < 23688) { Temp = 52; return; }
            if (23688 <= Resistance && Resistance < 23899) { Temp = 53; return; }
            if (23899 <= Resistance && Resistance < 24106) { Temp = 54; return; }
            if (24106 <= Resistance && Resistance < 24309) { Temp = 55; return; }
            if (24309 <= Resistance && Resistance < 24507) { Temp = 56; return; }
            if (24507 <= Resistance && Resistance < 24700) { Temp = 57; return; }
            if (24700 <= Resistance && Resistance < 24891) { Temp = 58; return; }
            if (24891 <= Resistance && Resistance < 25077) { Temp = 59; return; }
            if (25077 <= Resistance && Resistance < 25260) { Temp = 60; return; }
            if (25260 <= Resistance && Resistance < 25437) { Temp = 61; return; }
            if (25437 <= Resistance && Resistance < 25611) { Temp = 62; return; }
            if (25611 <= Resistance && Resistance < 25781) { Temp = 63; return; }
            if (25781 <= Resistance && Resistance < 25948) { Temp = 64; return; }
            if (25948 <= Resistance && Resistance < 26111) { Temp = 65; return; }
            if (26111 <= Resistance && Resistance < 26269) { Temp = 66; return; }
            if (26269 <= Resistance && Resistance < 26425) { Temp = 67; return; }
            if (26425 <= Resistance && Resistance < 26577) { Temp = 68; return; }
            if (26577 <= Resistance && Resistance < 26725) { Temp = 69; return; }
            if (26725 <= Resistance && Resistance < 26870) { Temp = 70; return; }
            if (26870 <= Resistance && Resistance < 27012) { Temp = 71; return; }
            if (27012 <= Resistance && Resistance < 27148) { Temp = 72; return; }
            if (27148 <= Resistance && Resistance < 27284) { Temp = 73; return; }
            if (27284 <= Resistance && Resistance < 27415) { Temp = 74; return; }
            if (27415 <= Resistance && Resistance < 27543) { Temp = 75; return; }
            if (27543 <= Resistance && Resistance < 27669) { Temp = 76; return; }
            if (27669 <= Resistance && Resistance < 27791) { Temp = 77; return; }
            if (27791 <= Resistance && Resistance < 27909) { Temp = 78; return; }
            if (27909 <= Resistance && Resistance < 28026) { Temp = 79; return; }
            if (28026 <= Resistance && Resistance < 28139) { Temp = 80; return; }
            if (28139 <= Resistance && Resistance < 28248) { Temp = 81; return; }
            if (28248 <= Resistance && Resistance < 28356) { Temp = 82; return; }
            if (28356 <= Resistance && Resistance < 28462) { Temp = 83; return; }
            if (28462 <= Resistance && Resistance < 28565) { Temp = 84; return; }
            if (28565 <= Resistance && Resistance < 28665) { Temp = 85; return; }
            if (28665 <= Resistance && Resistance < 28760) { Temp = 86; return; }
            if (28760 <= Resistance && Resistance < 28855) { Temp = 87; return; }
            if (28855 <= Resistance && Resistance < 28947) { Temp = 88; return; }
            if (28947 <= Resistance && Resistance < 29038) { Temp = 89; return; }
            if (29038 <= Resistance && Resistance < 29126) { Temp = 90; return; }
            if (29126 <= Resistance && Resistance < 29212) { Temp = 91; return; }
            if (29212 <= Resistance && Resistance < 29295) { Temp = 92; return; }
            if (29295 <= Resistance && Resistance < 29377) { Temp = 93; return; }
            if (29377 <= Resistance && Resistance < 29454) { Temp = 94; return; }
            if (29454 <= Resistance && Resistance < 29531) { Temp = 95; return; }
            if (29531 <= Resistance && Resistance < 29608) { Temp = 96; return; }
            if (29608 <= Resistance && Resistance < 29680) { Temp = 97; return; }
            if (29680 <= Resistance && Resistance < 29753) { Temp = 98; return; }
            if (29753 <= Resistance && Resistance < 29822) { Temp = 99; return; }
            if (29822 <= Resistance && Resistance < 29890) { Temp = 100; return; }
            if (29890 <= Resistance && Resistance < 29955) { Temp = 101; return; }
            if (29955 <= Resistance && Resistance < 30020) { Temp = 102; return; }
            if (30020 <= Resistance && Resistance < 30083) { Temp = 103; return; }
            if (30083 <= Resistance && Resistance < 30143) { Temp = 104; return; }
            if (30143 <= Resistance && Resistance < 30204) { Temp = 105; return; }
            if (30204 <= Resistance && Resistance < 30261) { Temp = 106; return; }
            if (30261 <= Resistance && Resistance < 30317) { Temp = 107; return; }
            if (30317 <= Resistance && Resistance < 30373) { Temp = 108; return; }
            if (30373 <= Resistance && Resistance < 30425) { Temp = 109; return; }
            if (30425 <= Resistance && Resistance < 30479) { Temp = 110; return; }
            if (30479 <= Resistance && Resistance < 30530) { Temp = 111; return; }
            if (30530 <= Resistance && Resistance < 30580) { Temp = 112; return; }
            if (30580 <= Resistance && Resistance < 30629) { Temp = 113; return; }
            if (30629 <= Resistance && Resistance < 30676) { Temp = 114; return; }
            if (30676 <= Resistance && Resistance < 30722) { Temp = 115; return; }
            if (30722 <= Resistance && Resistance < 30767) { Temp = 116; return; }
            if (30767 <= Resistance && Resistance < 30810) { Temp = 117; return; }
            if (30810 <= Resistance && Resistance < 30852) { Temp = 118; return; }
            if (30852 <= Resistance && Resistance < 30896) { Temp = 119; return; }
            if (30896 <= Resistance && Resistance < 30935) { Temp = 120; return; }
            if (30935 <= Resistance && Resistance < 30975) { Temp = 121; return; }
            if (30975 <= Resistance && Resistance < 31014) { Temp = 122; return; }
            if (31014 <= Resistance && Resistance < 31051) { Temp = 123; return; }
            if (31051 <= Resistance && Resistance < 31086) { Temp = 124; return; }
            if (31086 <= Resistance && Resistance < 31166) { Temp = 125; return; }
            if (31166 <= Resistance) { Temp = 255; return; }
        }

        public void CalculateToMC33772ModuleTemp(double Resistance, out double Temp)
        {
            Temp = 255;
            if (Resistance > 31772) { Temp = -255; return; }
            if (31772 > Resistance && Resistance >= 31675) { Temp = -40; return; }
            if (31675 > Resistance && Resistance >= 31607) { Temp = -39; return; }
            if (31607 > Resistance && Resistance >= 31535) { Temp = -38; return; }
            if (31535 > Resistance && Resistance >= 31459) { Temp = -37; return; }
            if (31459 > Resistance && Resistance >= 31379) { Temp = -36; return; }
            if (31379 > Resistance && Resistance >= 31296) { Temp = -35; return; }
            if (31296 > Resistance && Resistance >= 31208) { Temp = -34; return; }
            if (31208 > Resistance && Resistance >= 31115) { Temp = -33; return; }
            if (31115 > Resistance && Resistance >= 31018) { Temp = -32; return; }
            if (31018 > Resistance && Resistance >= 30916) { Temp = -31; return; }
            if (30916 > Resistance && Resistance >= 30810) { Temp = -30; return; }
            if (30810 > Resistance && Resistance >= 30698) { Temp = -29; return; }
            if (30698 > Resistance && Resistance >= 30582) { Temp = -28; return; }
            if (30582 > Resistance && Resistance >= 30460) { Temp = -27; return; }
            if (30460 > Resistance && Resistance >= 30332) { Temp = -26; return; }
            if (30332 > Resistance && Resistance >= 30199) { Temp = -25; return; }
            if (30199 > Resistance && Resistance >= 30060) { Temp = -24; return; }
            if (30060 > Resistance && Resistance >= 29915) { Temp = -23; return; }
            if (29915 > Resistance && Resistance >= 29765) { Temp = -22; return; }
            if (29765 > Resistance && Resistance >= 29608) { Temp = -21; return; }
            if (29608 > Resistance && Resistance >= 29444) { Temp = -20; return; }
            if (29444 > Resistance && Resistance >= 29275) { Temp = -19; return; }
            if (29275 > Resistance && Resistance >= 29098) { Temp = -18; return; }
            if (29098 > Resistance && Resistance >= 28916) { Temp = -17; return; }
            if (28916 > Resistance && Resistance >= 28726) { Temp = -16; return; }
            if (28726 > Resistance && Resistance >= 28530) { Temp = -15; return; }
            if (28530 > Resistance && Resistance >= 28327) { Temp = -14; return; }
            if (28327 > Resistance && Resistance >= 28117) { Temp = -13; return; }
            if (28117 > Resistance && Resistance >= 27901) { Temp = -12; return; }
            if (27901 > Resistance && Resistance >= 27677) { Temp = -11; return; }
            if (27677 > Resistance && Resistance >= 27447) { Temp = -10; return; }
            if (27447 > Resistance && Resistance >= 27209) { Temp = -9; return; }
            if (27209 > Resistance && Resistance >= 26965) { Temp = -8; return; }
            if (26965 > Resistance && Resistance >= 26714) { Temp = -7; return; }
            if (26714 > Resistance && Resistance >= 26456) { Temp = -6; return; }
            if (26456 > Resistance && Resistance >= 26191) { Temp = -5; return; }
            if (26191 > Resistance && Resistance >= 25920) { Temp = -4; return; }
            if (25920 > Resistance && Resistance >= 25642) { Temp = -3; return; }
            if (25642 > Resistance && Resistance >= 25358) { Temp = -2; return; }
            if (25358 > Resistance && Resistance >= 25068) { Temp = -1; return; }
            if (25068 > Resistance && Resistance >= 24772) { Temp = 0; return; }
            if (24772 > Resistance && Resistance >= 24470) { Temp = 1; return; }
            if (24470 > Resistance && Resistance >= 24162) { Temp = 2; return; }
            if (24162 > Resistance && Resistance >= 23849) { Temp = 3; return; }
            if (23849 > Resistance && Resistance >= 23530) { Temp = 4; return; }
            if (23530 > Resistance && Resistance >= 23207) { Temp = 5; return; }
            if (23207 > Resistance && Resistance >= 22879) { Temp = 6; return; }
            if (22879 > Resistance && Resistance >= 22547) { Temp = 7; return; }
            if (22547 > Resistance && Resistance >= 22211) { Temp = 8; return; }
            if (22211 > Resistance && Resistance >= 21871) { Temp = 9; return; }
            if (21871 > Resistance && Resistance >= 21527) { Temp = 10; return; }
            if (21527 > Resistance && Resistance >= 21181) { Temp = 11; return; }
            if (21181 > Resistance && Resistance >= 20832) { Temp = 12; return; }
            if (20832 > Resistance && Resistance >= 20480) { Temp = 13; return; }
            if (20480 > Resistance && Resistance >= 20126) { Temp = 14; return; }
            if (20126 > Resistance && Resistance >= 19771) { Temp = 15; return; }
            if (19771 > Resistance && Resistance >= 19414) { Temp = 16; return; }
            if (19414 > Resistance && Resistance >= 19056) { Temp = 17; return; }
            if (19056 > Resistance && Resistance >= 18698) { Temp = 18; return; }
            if (18698 > Resistance && Resistance >= 18340) { Temp = 19; return; }
            if (18340 > Resistance && Resistance >= 17981) { Temp = 20; return; }
            if (17981 > Resistance && Resistance >= 17623) { Temp = 21; return; }
            if (17623 > Resistance && Resistance >= 17266) { Temp = 22; return; }
            if (17266 > Resistance && Resistance >= 16910) { Temp = 23; return; }
            if (16910 > Resistance && Resistance >= 16555) { Temp = 24; return; }
            if (16555 > Resistance && Resistance >= 16206) { Temp = 25; return; }
            if (16206 > Resistance && Resistance >= 15856) { Temp = 26; return; }
            if (15856 > Resistance && Resistance >= 15507) { Temp = 27; return; }
            if (15507 > Resistance && Resistance >= 15162) { Temp = 28; return; }
            if (15162 > Resistance && Resistance >= 14819) { Temp = 29; return; }
            if (14819 > Resistance && Resistance >= 14480) { Temp = 30; return; }
            if (14480 > Resistance && Resistance >= 14144) { Temp = 31; return; }
            if (14144 > Resistance && Resistance >= 13812) { Temp = 32; return; }
            if (13812 > Resistance && Resistance >= 13484) { Temp = 33; return; }
            if (13484 > Resistance && Resistance >= 13160) { Temp = 34; return; }
            if (13160 > Resistance && Resistance >= 12841) { Temp = 35; return; }
            if (12841 > Resistance && Resistance >= 12526) { Temp = 36; return; }
            if (12526 > Resistance && Resistance >= 12215) { Temp = 37; return; }
            if (12215 > Resistance && Resistance >= 11910) { Temp = 38; return; }
            if (11910 > Resistance && Resistance >= 11609) { Temp = 39; return; }
            if (11609 > Resistance && Resistance >= 11313) { Temp = 40; return; }
            if (11313 > Resistance && Resistance >= 11023) { Temp = 41; return; }
            if (11023 > Resistance && Resistance >= 10738) { Temp = 42; return; }
            if (10738 > Resistance && Resistance >= 10458) { Temp = 43; return; }
            if (10458 > Resistance && Resistance >= 10183) { Temp = 44; return; }
            if (10183 > Resistance && Resistance >= 9914) { Temp = 45; return; }
            if (9914 > Resistance && Resistance >= 9651) { Temp = 46; return; }
            if (9651 > Resistance && Resistance >= 9393) { Temp = 47; return; }
            if (9393 > Resistance && Resistance >= 9140) { Temp = 48; return; }
            if (9140 > Resistance && Resistance >= 8893) { Temp = 49; return; }
            if (8893 > Resistance && Resistance >= 8652) { Temp = 50; return; }
            if (8652 > Resistance && Resistance >= 8416) { Temp = 51; return; }
            if (8416 > Resistance && Resistance >= 8185) { Temp = 52; return; }
            if (8185 > Resistance && Resistance >= 7960) { Temp = 53; return; }
            if (7960 > Resistance && Resistance >= 7740) { Temp = 54; return; }
            if (7740 > Resistance && Resistance >= 7526) { Temp = 55; return; }
            if (7526 > Resistance && Resistance >= 7316) { Temp = 56; return; }
            if (7316 > Resistance && Resistance >= 7113) { Temp = 57; return; }
            if (7113 > Resistance && Resistance >= 6914) { Temp = 58; return; }
            if (6914 > Resistance && Resistance >= 6720) { Temp = 59; return; }
            if (6720 > Resistance && Resistance >= 6531) { Temp = 60; return; }
            if (6531 > Resistance && Resistance >= 6348) { Temp = 61; return; }
            if (6348 > Resistance && Resistance >= 6169) { Temp = 62; return; }
            if (6169 > Resistance && Resistance >= 5995) { Temp = 63; return; }
            if (5995 > Resistance && Resistance >= 5826) { Temp = 64; return; }
            if (5826 > Resistance && Resistance >= 5661) { Temp = 65; return; }
            if (5661 > Resistance && Resistance >= 5501) { Temp = 66; return; }
            if (5501 > Resistance && Resistance >= 5345) { Temp = 67; return; }
            if (5345 > Resistance && Resistance >= 5193) { Temp = 68; return; }
            if (5193 > Resistance && Resistance >= 5046) { Temp = 69; return; }
            if (5046 > Resistance && Resistance >= 4903) { Temp = 70; return; }
            if (4903 > Resistance && Resistance >= 4764) { Temp = 71; return; }
            if (4764 > Resistance && Resistance >= 4629) { Temp = 72; return; }
            if (4629 > Resistance && Resistance >= 4498) { Temp = 73; return; }
            if (4498 > Resistance && Resistance >= 4370) { Temp = 74; return; }
            if (4370 > Resistance && Resistance >= 4247) { Temp = 75; return; }
            if (4247 > Resistance && Resistance >= 4127) { Temp = 76; return; }
            if (4127 > Resistance && Resistance >= 4010) { Temp = 77; return; }
            if (4010 > Resistance && Resistance >= 3897) { Temp = 78; return; }
            if (3897 > Resistance && Resistance >= 3787) { Temp = 79; return; }
            if (3787 > Resistance && Resistance >= 3567) { Temp = 80; return; }
            if (3567 > Resistance) { Temp = 255; return; }
        }

        #endregion

        #region Commands

        #region Safety Mechanism

        private byte GetByteToStr(string str)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            if (bytes.Length != 0)
                return bytes[0];
            else
                return 0x0;
        }

        private void InitializeComandList()
        {            
            Commands.Clear();
            Commands.Add(CMC_COMMAND.SM01.ToString(), GetByteToStr("v"));
            Commands.Add(CMC_COMMAND.SM02.ToString(), GetByteToStr("O"));
            Commands.Add(CMC_COMMAND.SM03_a.ToString(), GetByteToStr("a"));
            Commands.Add(CMC_COMMAND.SM03_b.ToString(), GetByteToStr("b"));
            Commands.Add(CMC_COMMAND.SM03_c.ToString(), GetByteToStr("c"));
            Commands.Add(CMC_COMMAND.SM03_d.ToString(), GetByteToStr("d"));
            Commands.Add(CMC_COMMAND.SM04_1.ToString(), GetByteToStr("T"));
            Commands.Add(CMC_COMMAND.SM04_2.ToString(), GetByteToStr("t"));
            Commands.Add(CMC_COMMAND.SM05.ToString(), GetByteToStr("p"));
            Commands.Add(CMC_COMMAND.SM06.ToString(), GetByteToStr("g"));
            Commands.Add(CMC_COMMAND.SM06_1.ToString(), GetByteToStr("f"));
            Commands.Add(CMC_COMMAND.SM34.ToString(), GetByteToStr("k"));
            Commands.Add(CMC_COMMAND.SM40.ToString(), GetByteToStr("o"));
            Commands.Add(CMC_COMMAND.SM41.ToString(), GetByteToStr("B"));
            Commands.Add(CMC_COMMAND.SM44.ToString(), GetByteToStr("l"));
            Commands.Add(CMC_COMMAND.SM45.ToString(), GetByteToStr("j"));
            Commands.Add(CMC_COMMAND.SM_RESET.ToString(), GetByteToStr("W"));
            Commands.Add(CMC_COMMAND.SM_ALL.ToString(), GetByteToStr("A"));
        }

        public bool CMC_SEND_CMD(CMC_COMMAND cmd)
        {
            var target = Commands[cmd.ToString()];

            var bt = new Byte[] { target, 0x0a };

            if (sp == null || !sp.IsOpen) return false;

            mw.LogState(LogType.CMC_SEND, 
                string.Format("CMC_[{0}]({1}) : {2}, {3}", 
                Encoding.ASCII.GetString(bt,0,1),
                cmd.ToString(), 
                target.ToString("X"),
                0x0a.ToString("X")));

            this.sp.Write(bt, 0, bt.Length);

            return true;
        }


        public int CMC_RECV_i0072(CMC_i0072 index)
        {
            if (i0072_data.Count < 16)
                return 0;

            return i0072_data[index.GetHashCode()] == true ? 1 : 0;
        }

        public int CMC_RECV_FAULT1(CMC_FAULT1_STATUS index)
        {
            if (i006E_data.Count < 16)
                return 0;

            return i006E_data[index.GetHashCode()] == true ? 1 : 0;
        }

        public int CMC_RECV_FAULT2(CMC_FAULT2_STATUS index)
        {
            if (i006F_data.Count < 16)
                return 0;

            return i006F_data[index.GetHashCode()] == true ? 1 : 0;
        }

        public int CMC_RECV_FAULT3(CMC_FAULT3_STATUS index)
        {
            if (i0070_data.Count < 16)
                return 0;

            return i0070_data[index.GetHashCode()] == true ? 1 : 0;
        }


        #endregion

        //20180913 wjs add watchdog reset send cmd
        public void CMC_WATCHDOG_RESET()
        {
            var bt = new Byte[] { 0x58, 0x0a };

            if (!sp.IsOpen) return;

            mw.LogState(LogType.CMC_SEND, "CMC_WATCHDOG_RESET : 0x58, 0x0a");
            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_Q_RESET()
        {
            var bt = new Byte[] { 0x51, 0x0a };

            if (!sp.IsOpen) return;

            mw.LogState(LogType.CMC_SEND, "CMC_Q_RESET : 0x51, 0x0a");
            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_DCIR_MODE_ENABLE()
        {
            var bt = new Byte[] { 0x49, 0x31, 0x0a };

            if (!sp.IsOpen) return;

            mw.LogState(LogType.CMC_SEND, "CMC_DCIR_MODE_ENABLE : 0x49, 0x31, 0x0a");
            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_DCIR_MODE_DISABLE()
        {
            var bt = new Byte[] { 0x49, 0x30, 0x0a };

            if (!sp.IsOpen) return;

            mw.LogState(LogType.CMC_SEND, "CMC_DCIR_MODE_DISABLE : 0x49, 0x30, 0x0a");
            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_GUID()
        {
            var bt = new Byte[] { 0x55, 0x0a };

            if (!sp.IsOpen) return;

            mw.LogState(LogType.CMC_SEND, "CMC_GUID : 0x55, 0x0a");
            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_OPEN()
        {
            var bt = new Byte[] { 0x50, 0x0a };

            if (!sp.IsOpen) return;

            mw.LogState(LogType.CMC_SEND, "CMC_OPEN : 0x50, 0x0a");
            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_Short(bool isLog = false)
        {
            var bt = new Byte[] { 0x54, 0x0a };

            if (!sp.IsOpen) return;

            if (isLog)
            {
                mw.LogState(LogType.CMC_SEND, "CMC_SHORT : 0x54, 0x0a");
            }
            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_Wire(bool isLog = false)
        {
            var bt = new Byte[] { 0x4F, 0x0a };

            if (!sp.IsOpen) return;
            if (isLog)
            {
                mw.LogState(LogType.CMC_SEND, "CMC_WIRE : 0x4F, 0x0a");
            }
            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_Recover()
        {
            var bt = new Byte[] { 0x52, 0x0a };

            if (!sp.IsOpen) return;

            mw.LogState(LogType.CMC_SEND, "CMC_RECOVER : 0x52, 0x0a");
            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_GlobalReset()
        {
            var bt = new Byte[] { 0x47, 0x0a };

            if (!sp.IsOpen) return;

            mw.LogState(LogType.CMC_SEND, "CMC_GLOBAL_RESET : 0x47, 0x0a");
            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_Sleep()
        {
            var bt = new Byte[] { 0x53, 0x0a };

            if (!sp.IsOpen) return;

            mw.LogState(LogType.CMC_SEND, "CMC_SLEEP : 0x53, 0x0a");
            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_V4328()
        {

            var bt = new Byte[] { 0x56, 0x34, 0x33, 0x32, 0x38, 0x0a };

            if (!sp.IsOpen) return;

            mw.LogState(LogType.CMC_SEND, "CMC_V4328 : 0x56, 0x34, 0x33, 0x32, 0x38, 0x0a");

            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_WakeUp()
        {
            var bt = new Byte[] { 0x57, 0x0a };

            if (!sp.IsOpen) return;

            mw.LogState(LogType.CMC_SEND, "CMC_WAKE_UP : 0x57, 0x0a");

            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_ModeChange()
        {
            var bt = new Byte[] { 0x4D, 0x32, 0x0a };

            if (!sp.IsOpen) return;


            mw.LogState(LogType.CMC_SEND, "CMC_MODE_CHANGE : 0x4D, 0x32, 0x0a");

            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_CON()
        {
            var bt = new Byte[] { 0x43, 0x4F, 0x4E, 0x0a };

            if (!sp.IsOpen) return;


            mw.LogState(LogType.CMC_SEND, "CMC_CON : 0x43, 0x4F, 0x4E, 0x0a");

            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_COF()
        {
            var bt = new Byte[] { 0x43, 0x4F, 0x46, 0x0a };

            if (!sp.IsOpen) return;


            mw.LogState(LogType.CMC_SEND, "CMC_COF : 0x43, 0x4F, 0x46, 0x0a");

            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_CEN()
        {
            var bt = new Byte[] { 0x43, 0x45, 0x4E, 0x0a };

            if (!sp.IsOpen) return;


            mw.LogState(LogType.CMC_SEND, "CMC_CEN : 0x43, 0x45, 0x4E, 0x0a");

            this.sp.Write(bt, 0, bt.Length);
        }

        public void CMC_CEF()
        {
            var bt = new Byte[] { 0x43, 0x45, 0x46, 0x0a };

            if (!sp.IsOpen) return;


            mw.LogState(LogType.CMC_SEND, "CMC_CEF : 0x43, 0x45, 0x46, 0x0a");

            this.sp.Write(bt, 0, bt.Length);
        }

        #endregion

        #region ETC

        public void _Dispose()
        {
            if (mw == null)
                return;

            mw.CMClist.Clear();
            endloop = true;
            _readDone.Set();

            if (readThread != null)
            {
                if (readThread != null)
                {
                    readThread.Join();
                }
            }

            //Port Denied에서 효과를 볼수있을까?
            try
            {
                if(sp != null && sp.IsOpen)
                {
                    sp.Close();
                }

                if (spStream!= null && (spStream.CanRead || spStream.CanWrite))
                {
                    sp.Close();
                    sp.Dispose();
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "CCMC.cs - Dispose_()", ec);
            }

            if (setValuesThread != null)
            {
                setValuesThread.Abort();
            }
        }

        public bool ExistPortName(string pname)
        {
            if (!SerialPort.GetPortNames().Any(x => x == pname))
            {
                mw.LogState(LogType.Fail, "Not Exist Port :" + pname);
                return false;
            }
            else
            {
                return true;
            }
        }

        void CCMC_PinChanged(object sender, System.IO.Ports.SerialPinChangedEventArgs e)
        {

        }

        public long GetHexToLong(string str)
        {
            long parseResult = 0;
            long.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out parseResult);
            return parseResult;
        }

        private void CheckNWrite(char[] four, string p1, string p2)
        {
            if (four[0].ToString() + four[1].ToString() + four[2].ToString() + four[3].ToString() == p1)
                return;
            var target = p2 + SetToTarget(four);

            if (!target.Contains("0000"))
            {
                if (!sendinglist.Contains(target))
                    sendinglist.Add(target);
            }

        }

        private string SetToTarget(char[] four)
        {
            var target = "";
            switch (four[0].ToString())
            {
                case "0": target += "F"; break;
                case "1": target += "E"; break;
                case "2": target += "D"; break;
                case "3": target += "C"; break;
                case "4": target += "B"; break;
                case "5": target += "A"; break;
                case "6": target += "9"; break;
                case "7": target += "8"; break;
                case "8": target += "7"; break;
                case "9": target += "6"; break;
                case "A": target += "5"; break;
                case "B": target += "4"; break;
                case "C": target += "3"; break;
                case "D": target += "2"; break;
                case "E": target += "1"; break;
                case "F": target += "0"; break;
            }
            switch (four[1].ToString())
            {
                case "0": target += "F"; break;
                case "1": target += "E"; break;
                case "2": target += "D"; break;
                case "3": target += "C"; break;
                case "4": target += "B"; break;
                case "5": target += "A"; break;
                case "6": target += "9"; break;
                case "7": target += "8"; break;
                case "8": target += "7"; break;
                case "9": target += "6"; break;
                case "A": target += "5"; break;
                case "B": target += "4"; break;
                case "C": target += "3"; break;
                case "D": target += "2"; break;
                case "E": target += "1"; break;
                case "F": target += "0"; break;
            }

            switch (four[2].ToString())
            {
                case "0": target += "F"; break;
                case "1": target += "E"; break;
                case "2": target += "D"; break;
                case "3": target += "C"; break;
                case "4": target += "B"; break;
                case "5": target += "A"; break;
                case "6": target += "9"; break;
                case "7": target += "8"; break;
                case "8": target += "7"; break;
                case "9": target += "6"; break;
                case "A": target += "5"; break;
                case "B": target += "4"; break;
                case "C": target += "3"; break;
                case "D": target += "2"; break;
                case "E": target += "1"; break;
                case "F": target += "0"; break;
            }

            switch (four[3].ToString())
            {
                case "0": target += "F"; break;
                case "1": target += "E"; break;
                case "2": target += "D"; break;
                case "3": target += "C"; break;
                case "4": target += "B"; break;
                case "5": target += "A"; break;
                case "6": target += "9"; break;
                case "7": target += "8"; break;
                case "8": target += "7"; break;
                case "9": target += "6"; break;
                case "A": target += "5"; break;
                case "B": target += "4"; break;
                case "C": target += "3"; break;
                case "D": target += "2"; break;
                case "E": target += "1"; break;
                case "F": target += "0"; break;
            }
            return target;
        }

        /// <summary>
        /// true 리턴시 값이 0이다
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="arr"></param>
        /// <param name="ch"></param>
        /// <param name="triigger"></param>
        private void GetDaisyChain(int temp, byte[] arr, int ch, out bool triigger)
        {
            triigger = true;

            for (int i = 0; i < temp - 4; i++)
            {
                if (arr[i] == 170 && arr[i + 1] == 170)
                {
                    if (arr[i + 2] == ch) //1채널일때
                    {
                        if (arr[i + 3] == 45)//셀1
                        {
                            if (arr[i + 4] != 0)
                                triigger = false;
                            if (arr[i + 5] != 0)
                                triigger = false;
                            if (arr[i + 6] != 0)
                                triigger = false;
                            if (arr[i + 7] != 0)
                                triigger = false;
                            if (arr[i + 8] != 0)
                                triigger = false;
                            if (arr[i + 9] != 0)
                                triigger = false;
                        }
                    }
                }
            }

        }

        bool SetDataOrNot(string id, out double val)
        {
            var tb7 = GetHexToLong(CheckKeyNReturnVal(id));
            if (tb7 != 0)
            {
                if (CheckKeyNReturnVal(id).Length == 4)
                {
                    val = tb7;
                    //mw.LogState(LogType.CMC_RECV, GetHexToLong(CheckKeyNReturnVal(id)) + ":" + tb7);
                    return true;
                    //raw_cmcList.Remove(id);
                }
                else
                {
                    val = 0.0;
                    return false;
                }
            }
            else
            {
                val = 0.0;
                return false;
            }
        }

        string CheckKeyNReturnVal(string key)
        {
            try
            {
                if (raw_cmcList.ContainsKey(key))
                    return raw_cmcList[key];
                else
                    return "0";
            }
            catch (Exception)
            {
            }
            return "0";

        }

        #endregion

        #endregion



    }
}
