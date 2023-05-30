using EOL_BASE.외부모듈;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE.모듈
{
    public class CChroma
    {
        MainWindow mw; 
        
        private Encoding Encoder = Encoding.ASCII;

        public bool isAlive = false;

        SerialPort serialPort;

        //200723 ht 20sec > 2sec
        int CONNECT_TIMEOUT_SEC = 2;

        int WITHSTANDING_VOLT = 1000;
        int WITHSTANDING_TIME = 5;

        public bool isPlusFlag = true;


        public double CON_INSULATION_RESISTANCE_V = 0;
        public double CON_INSULATION_RESISTANCE_T = 0;
        public double CON_WITHSTANDING_VOLTAGE_V = 0;
        public double CON_WITHSTANDING_VOLTAGE_T = 0;
        public double CON_WITHSTANDING_RAMP_UP_TIME = 0;
        public double CON_WITHSTANDING_FALLDOWN_TIME = 0;
        public double CON_ARC_ENABLE = 0;
        public double CON_ARC_LIMIT = 0;
        public double ARC_DC_HI_TIME = 0;

        public string judgementStr = "";


        public List<string> fetchList500 = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="port">COM port 설정</param>
        public CChroma(MainWindow mw,string port)
        {
            Connect(mw, port);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="port">COM port 설정</param>
        private void Connect(MainWindow mw, string port)
        {
            this.mw = mw;
            serialPort = new SerialPort();
            serialPort.DataBits = 8;
            serialPort.BaudRate = 9600;//공장초기화 값 9600
            serialPort.PortName = port;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;

            try
            {
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    mw.LogState(LogType.Success, serialPort.PortName + ", Chroma");
                    isAlive = true;

                    string RcvMessage = string.Empty;

                    Send(0, "*idn?\n");
                    Thread.Sleep(500); 
                    if (Read(0, 100, CONNECT_TIMEOUT_SEC, out RcvMessage, (byte)'\n') > 0)
                    {
                        mw.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mw.contBt_hipot.Background = System.Windows.Media.Brushes.Green;
                        }));
                        mw.LogState(LogType.Success, "Chroma 19053 - Recv :" + RcvMessage.Replace("\r", "").Replace("\n", ""));
                    }
                }
                else
                    mw.LogState(LogType.Fail, "Chroma" + serialPort.PortName + " Open");
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "Chroma", ec);
            }
        }

        public void Discard()
        {
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                Thread.Sleep(10);
            }
        }

        public void Dispose()
        {
            serialPort.Dispose();
        }

        //20180713 wjs add chroma reconnected func
        public int ChromaAliveCheck()
        {
            int res = 0;
            //살아있으면 버전 읽고
            if (serialPort != null && isAlive)
            {
                Send(0, String.Format("*IDN?\n"));
                string RcvMessage = string.Empty;
                //Thread.Sleep(1000); //jgh 택타임 단축 요청 이지현사원
                Thread.Sleep(500);

                //버전 읽으면 로그찍고 바로 나가고
                if (Read(0, 50, 10, out RcvMessage, (byte)'\n') > 0)
                {
                    if (RcvMessage.Contains("Chroma"))
                    {
                        mw.LogState(LogType.Info, "chroma : " + RcvMessage);
                        res = 1;
                    }
                    else
                    {
                        mw.LogState(LogType.Fail, "chroma Version Read Fail");
                        res = 0;
                    }
                }
                else
                {
                    mw.LogState(LogType.Fail, "chroma Version Read Fail");
                    res = 0;
                }
            }
            //else
            if (res == 0)    //살아있지 않거나 버전 읽기 실패하면 죽이고 다시 살림
            {
                try
                {
                    serialPort.Dispose();
                    serialPort = null;

                    //chroma = new CChroma(this, this.chroma_PortName[mChromaNum]);
                    Connect(mw, mw.chroma_PortName);
                    mw.LogState(LogType.Info, "Port : " + mw.chroma_PortName + " chroma Reconnected Success");
                    Thread.Sleep(500);

                    Send(0, String.Format("*IDN?\n"));
                    string RcvMessage = string.Empty;
                    Thread.Sleep(500);

                    if (Read(0, 50, 10, out RcvMessage, (byte)'\n') > 0)
                    {
                        mw.LogState(LogType.Info, "chroma : " + RcvMessage);
                        res = 1;
                    }
                    else
                    {//다시 살렸는데도 실패
                        mw.LogState(LogType.Fail, "chroma Version Read Fail");
                        res = 0;
                    }
                }
                catch (Exception e)
                {
                    mw.LogState(LogType.Fail, "chroma Reconnected Fail and exception : " + e.ToString());
                    res = 0;
                }
            }
            return res; //데이터 제대로 읽었으면 1 아니면 0리턴
        }

        /// <summary>
        /// 접촉성검사용 Cap
        /// </summary>
        /// <param name="plusch"></param>
        /// <param name="minusch"></param>
        /// <param name="Capacitance"></param>
        /// <returns></returns>
        public int GetCapacitance(int plusch, int minusch, out double Capacitance)
        {
            Capacitance = new double();

            mw.LogState(LogType.Info, "Chroma Plus Channel :" + plusch.ToString() + "/ Minus Channel :" + minusch.ToString());

            int bRet = -1;
            serialPort.DiscardInBuffer();
            //채널선택
            Send(0, "SOUR:SAFE:STOP\n");
            System.Threading.Thread.Sleep(250);
            //채널 닫기
            //for (int i = 1; i >= 99; i++)
            //{
            //    Send(0, String.Format($"SAFE:STEP {i}:DEL\n"));
            //    System.Threading.Thread.Sleep(250);
            //}


            //221121 Chroma nnkim
            if (this.mw.strChromaModelType == "NEW")
            {
                Send(0, String.Format("SAFE:STEP1:OSC:CHAN(@1(0))\n"));
                System.Threading.Thread.Sleep(250);
                Send(0, String.Format("SAFE:STEP1:OSC:CHAN:LOW(@1(0))\n"));
                System.Threading.Thread.Sleep(250);
                Send(String.Format("SAFE:STEP1:OSC:CHAN(@1({0}))", (plusch).ToString()), 250);
                Send(String.Format("SAFE:STEP1:OSC:CHAN:LOW (@1({0}))", (minusch).ToString()), 250);
            }
            else
            {
                Send(0, String.Format("SAFE:STEP1:OSC:CHAN(@(0))\n"));
                System.Threading.Thread.Sleep(250);
                Send(0, String.Format("SAFE:STEP1:OSC:CHAN:LOW(@(0))\n"));
                System.Threading.Thread.Sleep(250);
                #region Hi Channel set
                for (int i = 0; i < 3; i++)
                {
                    mw.LogState(LogType.Info, string.Format("Setting Hi({0}) Channel Try ({1})", plusch.ToString(), (i + 1).ToString()));

                    Send(String.Format("SAFE:STEP1:OSC:CHAN(@({0}))", (plusch).ToString()), 250);
                    var rtCh = GetSettingChannel("SAFE:STEP1:OSC:CHAN?\n");
                    if (rtCh == -1 || rtCh != plusch)
                    {
                        //retry, 3회시도에도 안되면 빠져나감
                        if (i == 2)
                        {
                            judgementStr = mw._CHAN_HI_FAIL;
                            bRet = -1;
                            return -1;
                        }
                    }
                    else
                    {
                        //ok
                        break;
                    }
                }
                #endregion


                #region Lo Channel set
                for (int i = 0; i < 3; i++)
                {
                    mw.LogState(LogType.Info, string.Format("Setting Lo({0}) Channel Try ({1})", minusch.ToString(), (i + 1).ToString()));

                    Send(String.Format("SAFE:STEP1:OSC:CHAN:LOW (@({0}))", (minusch).ToString()), 250);
                    var rtCh = GetSettingChannel("SAFE:STEP1:OSC:CHAN:LOW?\n");
                    if (rtCh == -1 || rtCh != minusch)
                    {
                        //retry, 3회시도에도 안되면 빠져나감
                        if (i == 2)
                        {
                            judgementStr = mw._CHAN_LO_FAIL;
                            bRet = -1;
                            return -1;
                        }
                    }
                    else
                    {
                        //ok
                        break;
                    }
                }
                #endregion
            }

            Send(0, String.Format("SAFE:STEP1:OSC:TIME 3.0\n"));
            System.Threading.Thread.Sleep(250);

            //커패시턴스 읽기
            Send(0, String.Format("SAFE:STEP1:OSC:LIM:SHOR 1\n")); //3->5->2->1
            //            Send(0,String.Format("SAFE:STEP1:OSC:LIM:OPEN 0.3\n"));
            System.Threading.Thread.Sleep(250);
            Send(0, String.Format("SAFE:STAR:CST GET\n"));
            System.Threading.Thread.Sleep(250);
            //Send(0,String.Format("SAFE:STEP1:OSC:CSTandard?\n"));
            //            Send(0,String.Format("SAFE:STEP1:OSC:CST 3, 0.0000000050\n"));
            Send(0, String.Format("SAFE:STEP1:OSC:CST 3, 0.0000000200\n")); //20160418 kys 베터리연결안되어있을경우 pass
            System.Threading.Thread.Sleep(250);

            Send(0, "SAFE:STAR\n");

            int iTick = Environment.TickCount;
            while (CheckState() != 1)   //체크스테이트가 1이면 검사시작중
            {
                if (CommonMethods.IsTimeOut(iTick, 5000))
                {
                    break;
                }
            }

            //검사시작후 시간대기            
            while (CheckState() != 0)   //체크스테이트가 0이면 검사완료
            {
                if (CommonMethods.IsTimeOut(iTick, 5000))
                {
                    break;
                }
            }

            serialPort.DiscardInBuffer();
            System.Threading.Thread.Sleep(100); //jgh 택타임 축소 요청 이지현사원
            string tRcvMessage = string.Empty;
            

            serialPort.DiscardInBuffer();
            System.Threading.Thread.Sleep(100); //jgh 택타임 축소 요청 이지현사원
            Send(0, "SAFE:RES:ALL:MMET?\n");
            System.Threading.Thread.Sleep(500);
            string RcvMessage = string.Empty;

            if (Read(0, 100, CONNECT_TIMEOUT_SEC, out RcvMessage, (byte)'\n') > 0)
            {
                Capacitance = double.Parse(RcvMessage);

                bRet = 0;
            }


            Send(0, "SOUR:SAFE:STOP\n");
            Send(0, "SOUR:SAFE:STOP\n");
            if (this.mw.strChromaModelType == "NEW")
            {
                Send(0, String.Format("SAFE:STEP1:OSC:CHAN(@1(0))\n"));
                System.Threading.Thread.Sleep(150);
                Send(0, String.Format("SAFE:STEP1:OSC:CHAN:LOW(@1(0))\n"));
                System.Threading.Thread.Sleep(150);
            }
            else
            {
                Send(0, String.Format("SAFE:STEP1:OSC:CHAN(@(0))\n"));
                System.Threading.Thread.Sleep(150);
                Send(0, String.Format("SAFE:STEP1:OSC:CHAN:LOW(@(0))\n"));
                System.Threading.Thread.Sleep(150);
            }


            return bRet;
        }

        private int GetSettingChannel(string cmd)
        {
            int rcvVal = -1;

            try
            {
                serialPort.Write(cmd);
                int cnt = 0;
                while (serialPort.BytesToRead < 8)
                {
                    Thread.Sleep(1);
                    cnt += 1;
                    if (cnt == 5000)
                    {
                        mw.LogState(LogType.Info, "NOT_RECEIVED_DATA");
                        return -1;
                    }
                }

                var rec = serialPort.BytesToRead;
                var bt = new byte[rec];
                serialPort.Read(bt, 0, rec);
                var strbuff = Encoding.Default.GetString(bt, 0, rec);

                if (strbuff.Contains("STOPPED"))
                {
                    mw.LogState(LogType.Info, "Contains stop signal.");
                    return -1;
                }
                else
                {
                    // 숫자만 추출
                    var val = Regex.Replace(strbuff, @"\D", "");
                    rcvVal = Convert.ToInt32(val);

                    mw.LogState(LogType.RESPONSE, cmd.Remove(cmd.Length - 1, 1) + " : " + rcvVal.ToString());
                    Thread.Sleep(100);
                    return rcvVal;
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "GetSettingChannel", ex);
            }

            return rcvVal;
        }

        public int CheckState(System.IO.Ports.SerialPort sp)
        {
            sp.DiscardInBuffer();
            sp.Write("SOUR:SAFE:STATUS?\n");
            int cnt = 0;
            while (sp.BytesToRead < 9)
            {
                Thread.Sleep(1);
                cnt += 1;
                if (cnt == 5000)
                {
                    mw.LogState(LogType.Info, "NOT_RECEIVED_DATA");
                    return -1;
                }
            }

            var rec = sp.BytesToRead;
            var bt = new byte[rec];
            sp.Read(bt, 0, rec);
            var strbuff = Encoding.Default.GetString(bt, 0, rec);

            if (strbuff.Contains("STOPPED"))
            {
                mw.LogState(LogType.Info, "Contains stop signal.");
                return 0;
            }
            else
            {
                return 1;
            }
        }
        /// <summary>
        /// Withstand (200612)
        /// </summary>
        /// <param name="plusch"></param>
        /// <param name="minusch"></param>
        /// <param name="Current"></param>
        /// <returns></returns>
        public int GetVoltage(int plusch, int minusch, out double Current)
        {
            judgementStr = "";
            try
            {
                if (serialPort.IsOpen)
                {
                    // 2020.02.27 Noah Choi Falldown Time 로그추가
                    mw.LogState(LogType.Info, "Chroma19053 Set Voltage :" +
                        CON_WITHSTANDING_VOLTAGE_V.ToString() + "/ Time : " +
                        CON_WITHSTANDING_VOLTAGE_T.ToString() + "/ Ramp up Time : " +
                        CON_WITHSTANDING_RAMP_UP_TIME.ToString() + "/ Falldown Time :" +
                        CON_WITHSTANDING_FALLDOWN_TIME.ToString() + "/ ARC ENABLED : " +
                        CON_ARC_ENABLE.ToString() + "/ ARC LIMIT(A) : " +
                        (CON_ARC_LIMIT * 0.001).ToString() + "/ TimeOut : " +
                        CONNECT_TIMEOUT_SEC.ToString());// + "/ Range(A) : " +
                                                        //CON_WITHSTANDING_RANGE.ToString());

                    mw.LogState(LogType.Info, "Chroma19053 Plus Channel :" +
                        plusch.ToString() + "/ Minus Channel :" + minusch.ToString());

                    int bRet = -1;
                    Current = -1;

                    serialPort.DiscardInBuffer();
                    Send("SOUR:SAFE:STOP", 250);

                    if (this.mw.strChromaModelType == "NEW")
                    {
                        Send("SAFE:STEP1:DC:CHAN (@1(0))", 250);
                        Send("SAFE:STEP1:DC:CHAN:LOW (@1(0))", 250);
                        Send(String.Format("SAFE:STEP1:DC:CHAN (@1({0}))", (plusch).ToString()), 250);
                        Send(String.Format("SAFE:STEP1:DC:CHAN:LOW (@1({0}))", (minusch).ToString()), 250);
                    }
                    else
                    {
                        #region Channel Initialize
                        Send("SAFE:STEP1:DC:CHAN(@(0))", 250);
                        Send("SAFE:STEP1:DC:CHAN:LOW(@(0))", 250);
                        #endregion

                        #region Hi Channel set
                        for (int i = 0; i < 3; i++)
                        {
                            mw.LogState(LogType.Info, string.Format("Setting Hi({0}) Channel Try ({1})", plusch.ToString(), (i + 1).ToString()));

                            Send(String.Format("SAFE:STEP1:DC:CHAN(@({0}))", (plusch).ToString()), 250);

                            var rtCh = GetSettingChannel("SAFE:STEP1:DC:CHAN?\n");
                            if (rtCh == -1 || rtCh != plusch)
                            {
                                //retry, 3회시도에도 안되면 빠져나감
                                if (i == 2)
                                {
                                    //200609 추가
                                    judgementStr = mw._CHAN_HI_FAIL;
                                    bRet = -1;
                                    return -1;
                                }
                            }
                            else
                            {
                                //ok
                                break;
                            }
                        }
                        #endregion

                        #region Lo Channel set
                        for (int i = 0; i < 3; i++)
                        {
                            mw.LogState(LogType.Info, string.Format("Setting Lo({0}) Channel Try ({1})", minusch.ToString(), (i + 1).ToString()));

                            Send(String.Format("SAFE:STEP1:DC:CHAN:LOW (@({0}))", (minusch).ToString()), 250);

                            var rtCh = GetSettingChannel("SAFE:STEP1:DC:CHAN:LOW?\n");
                            if (rtCh == -1 || rtCh != minusch)
                            {
                                //retry, 3회시도에도 안되면 빠져나감
                                if (i == 2)
                                {
                                    //200609 추가
                                    judgementStr = mw._CHAN_LO_FAIL;
                                    bRet = -1;
                                    return -1;
                                }
                            }
                            else
                            {
                                //ok
                                break;
                            }
                        }
                        #endregion
                    }

                    Send(String.Format("SOUR:SAFE:STEP1:DC:LEV {0}", CON_WITHSTANDING_VOLTAGE_V.ToString()), 250);
                    Send("SOUR:SAFE:STEP1:DC:LIMIT 0.002999", 250);
                    Send(String.Format("SOUR:SAFE:STEP1:DC:TIME:RAMP {0}", CON_WITHSTANDING_RAMP_UP_TIME.ToString()), 250);
                    // 2020.02.27 Noah Choi Falldown 명령어 추가 진행
                    Send(String.Format("SOUR:SAFE:STEP1:DC:TIME:FALL {0}", CON_WITHSTANDING_FALLDOWN_TIME.ToString()), 250);
                    Send(String.Format("SOUR:SAFE:STEP1:DC:TIME {0}", CON_WITHSTANDING_VOLTAGE_T.ToString()), 250);

                    if (int.Parse(CON_ARC_ENABLE.ToString()) == 1)
                    {
                        Send(String.Format("SOUR:SAFE:STEP1:DC:LIM:ARC {0}", (CON_ARC_LIMIT * 0.001).ToString()), 250);
                    }

                    Send("SOUR:SAFE:STAR", 250);
                    int iTick = Environment.TickCount;

                    Stopwatch st = new Stopwatch();
                    st.Start();

                    #region wait for running
                    while (CheckState(serialPort) == 1)
                    {
                        if (mw.isStop || mw.ispause)
                        {
                            Send("SOUR:SAFE:STOP", 100);
                            Send("SOUR:SAFE:STOP", 0);
                            bRet = -1;
                            return -1;
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                    #endregion

                    st.Stop();
                    var elapsetime = st.ElapsedMilliseconds;
                    ARC_DC_HI_TIME = elapsetime / 1000;

                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();

                    string judgementRcvMessage = "";
                    string judgement = "";

                    Send("SAFE:RES:STEP1:JUDG?", 500);
                    int recvCheck = -1;

                    recvCheck = Read(0, 100, CONNECT_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n');
                    mw.LogState(LogType.Info, string.Format("Recv Message:{0}", judgementRcvMessage));

                    if (recvCheck > 0)
                    {
                        try
                        {
                            judgement = judgementRcvMessage.Replace("\r\n", "");
                            mw.LogState(LogType.Info, string.Format("Judgement first check: {0}", judgement));

                            #region 비었을 때 리트라이 코드
                            if (judgementRcvMessage == "\n" || judgementRcvMessage == "\r\n" || judgementRcvMessage == "")
                            {
                                Thread.Sleep(500);
                                recvCheck = Read(0, 100, CONNECT_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n');
                                mw.LogState(LogType.Info, string.Format("Recv Message:{0}", judgementRcvMessage));
                                if (recvCheck > 0)
                                {
                                    judgement = judgementRcvMessage.Replace("\r\n", "");
                                    mw.LogState(LogType.Info, string.Format("Judgement retry check: {0}", judgement));
                                }
                            }
                            #endregion

                            if (judgement.Equals("116"))
                            {
                                serialPort.DiscardInBuffer();
                                serialPort.DiscardOutBuffer();

                                Send("SAFE:RES:ALL:MMET?", 500);
                                judgementRcvMessage = string.Empty;

                                recvCheck = Read(0, 100, CONNECT_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n', false);
                                mw.LogState(LogType.Info, string.Format("Recv Message:{0}", judgementRcvMessage));

                                if (judgementRcvMessage == "\n" || judgementRcvMessage == "\r\n" || judgementRcvMessage == "")
                                {
                                    Thread.Sleep(500);
                                    recvCheck = Read(0, 100, CONNECT_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n', false);
                                    mw.LogState(LogType.Info, string.Format("Recv Message:{0}", judgementRcvMessage));
                                }

                                if (recvCheck > 0)
                                {
                                    try
                                    {
                                        //성공
                                        Current = double.Parse(judgementRcvMessage);
                                        bRet = 0;
                                        mw.LogState(LogType.Info, string.Format("parsed Current. Current: {0}, recv msg: {1}", Current, judgementRcvMessage));
                                    }
                                    catch
                                    {
                                        Current = 0;
                                        bRet = -1;
                                        mw.LogState(LogType.Info, string.Format("not parsed Current. recv msg: {0}", judgementRcvMessage));
                                    }
                                }
                                else
                                {
                                    Current = 0;
                                    bRet = -1;
                                    mw.LogState(LogType.Info, string.Format("not Received judgement Str. recv msg: {0}", judgementRcvMessage));
                                }
                            }
                            else
                            {
                                mw.LogState(LogType.Fail, "Chroma Result : Channel setting error" );// 0519 kyoungsuk yoo

                                switch (judgement)
                                {
                                    case "113": judgementStr = mw._CASE_113; mw.LogState(LogType.Info, "USER STOP"); break;
                                    case "114": judgementStr = mw._CASE_114; mw.LogState(LogType.Info, "CAN NOT TEST"); break;
                                    case "115": judgementStr = mw._CASE_115; mw.LogState(LogType.Info, "TESTING"); break;
                                    case "112": judgementStr = mw._CASE_112; mw.LogState(LogType.Info, "STOP"); break;
                                    case "33": judgementStr = mw._CASE_33; mw.LogState(LogType.Info, "DC - HI"); break;
                                    case "34": judgementStr = mw._CASE_34; mw.LogState(LogType.Info, "DC - LO"); break;
                                    case "35": judgementStr = mw._CASE_35; mw.LogState(LogType.Info, "Chroma Result : Channel setting error"); break; // 0519 kyoungsuk yoo
                                    case "36": judgementStr = mw._CASE_36; mw.LogState(LogType.Info, "DC - IO"); break;
                                    case "37": judgementStr = mw._CASE_37; mw.LogState(LogType.Info, "DC - CHECK LOW"); break;
                                    case "38": judgementStr = mw._CASE_38; mw.LogState(LogType.Info, "DC - ADV OVER"); break;
                                    case "39": judgementStr = mw._CASE_39; mw.LogState(LogType.Info, "DC - ADI OVER"); break;
                                    case "43": judgementStr = mw._CASE_43; mw.LogState(LogType.Info, "DC - IO-F"); break;
                                    case "49": judgementStr = mw._CASE_49; mw.LogState(LogType.Info, "IR - HI"); break;
                                    case "50": judgementStr = mw._CASE_50; mw.LogState(LogType.Info, "IR - LO"); break;
                                    case "52": judgementStr = mw._CASE_52; mw.LogState(LogType.Info, "IR - IO"); break;
                                    case "54": judgementStr = mw._CASE_54; mw.LogState(LogType.Info, "IR - ADV OVER"); break;
                                    case "55": judgementStr = mw._CASE_55; mw.LogState(LogType.Info, "IR - ADI OVER"); break;
                                    case "120": judgementStr = mw._CASE_120_DC; mw.LogState(LogType.Info, "DC - GR CONT."); break;
                                    case "121": judgementStr = mw._CASE_121_DC; mw.LogState(LogType.Info, "DC - TRIPPED"); break;
                                }

                                Current = -1;
                                bRet = -9;
                            }
                        }
                        catch
                        {
                            Current = -1;
                            bRet = -1;
                        }
                    }
                    else
                    {
                        Current = 0;
                        bRet = -1;
                        mw.LogState(LogType.Info, string.Format("not Received judgement Str. recv msg: {0}", judgementRcvMessage));
                    }

                    if (this.mw.strChromaModelType == "NEW")
                    {
                        Send("SAFE:STEP1:DC:CHAN (@1(0))", 250);
                        Send("SAFE:STEP1:DC:CHAN:LOW (@1(0))", 250);
                    }
                    else
                    {
                        Send("SAFE:STEP1:DC:CHAN(@(0))", 250);
                        Send("SAFE:STEP1:DC:CHAN:LOW(@(0))", 250);
                    }


                    Send("SOUR:SAFE:STOP", 0);
                    Send("SOUR:SAFE:STOP", 0);

                    return bRet;
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "Chroma_GetVoltage", ec);
                if (serialPort.IsOpen)
                {
                    serialPort.Write("SOUR:SAFE:STOP\n");
                    Thread.Sleep(100);
                    serialPort.Write("SOUR:SAFE:STOP\n");
                }
                Current = -1;
                return -1;
            }
            Current = -1;
            return -1;
        }




        #region Multimedia Timer Fields

        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, 공용_이벤트핸들러 handler, IntPtr user, int eventType);
        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);
        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(int msec);
        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(int msec);

        //private delegate void 공용_이벤트핸들러(Object obj);
        private delegate void 공용_이벤트핸들러(int id, int msg, IntPtr user, int dw1, int dw2);
        private 공용_이벤트핸들러 타이머_핸들러;
        private int 타이머_아이디;
        public List<string> fetchList = new List<string>();
        bool isTimerRunning = false;
        #endregion

        /// <summary>
        /// Fetch 10초가 지난 후 120MOhm 이 넘어갈 시 넣을 시간 변수
        /// 20191022 Yoon S.C Add
        /// </summary>
        public int fetchCNTTime = 0;
        /// <summary>
        /// 10초때의 FETCH 값이 120 이하일 경우 Flag
        /// 20191022 Yoon S.C Add
        /// </summary>
        private bool isFetchFlag = false;
        /// <summary>
        /// FETCH PASS FAIL Flag
        /// 20191022 Yoon S.C Add
        /// </summary>
        private bool isFetchOk = true;

        //jgh Chroma Stop Msg 수신할경우 Flag on
        private bool isChromaStop = false;
        public bool bIsFetChNg = false;
        //201012 wjs add recv 0 flag
        public bool recvZero_Before10S = false;
        public bool bIs10sValueNg = false;

        //private void 상시저항가져오는타이머_Tick(object obj)
        private void 상시저항가져오는타이머_Tick(int id, int msg, IntPtr user, int dw1, int dw2)
        {   
            Stopwatch st = new Stopwatch();
            st.Start();
            
            serialPort.DiscardInBuffer();
            Send(0, "SAFE:FETC? MMET\n");
            var cnt = 0;

            //200721 이게 10이면 10회값으로 비교
            //       20으로 변경
            int nMiddleCheckTime = 20;//10;


            while (serialPort.BytesToRead < 15)
            {
                Thread.Sleep(1);
                cnt += 1;
                if (cnt == 5000)
                {
                    mw.LogState(LogType.Info, "NOT_RECEIVED_DATA");                        
                    st.Stop();
                    st = null;
                    return;
                }
            }
            var rec = serialPort.BytesToRead;
            var bt = new byte[rec];
            serialPort.Read(bt, 0, rec);
            var strbuff = Encoding.Default.GetString(bt, 0, rec);

            //200721 로그 추가
            mw.LogState(LogType.Info, "RECEIVED_DATA(FETC):" + strbuff);

            double val = 0.0;
            double.TryParse(strbuff, out val);            
            val = val / 1000000;
            var elapsedTime = st.ElapsedMilliseconds;
            st.Stop();
            st = null;

            serialPort.DiscardInBuffer();
            Send(0, "SOUR:SAFE:STATUS?\n");
            cnt = 0;

            while (serialPort.BytesToRead < 9)
            {
                Thread.Sleep(1);
                cnt += 1;
                if (cnt == 5000)
                {
                    mw.LogState(LogType.Info, "NOT_RECEIVED_DATA");
                    return;
                }
            }
            rec = serialPort.BytesToRead;
            bt = new byte[rec];
            serialPort.Read(bt, 0, rec);
            strbuff = Encoding.Default.GetString(bt, 0, rec);

            //200721 로그 추가
            mw.LogState(LogType.Info, "RECEIVED_DATA(STAT):" + strbuff);

            //201013 TEST!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //if (fetchList.Count == 25)
           // {
           //     val = 0.0;
          //  }
            //else
            //{
           //     val = 0.0;
           // }


            if (strbuff.Contains("STOPPED"))
            {
                mw.LogState(LogType.Fail, "Chroma Send Stopped");
                isChromaStop = true;
                timeKillEvent(타이머_아이디);
                isTimerRunning = false;
                return;
            }
            else
            {
                //처음 10초는 1초 마다 측정을 진행한다.                
                if(isFetchFlag == false)
                {
                    mw.LogState(LogType.Info, string.Format("FETCH RST: {0} MOhm / Elapsed(msec): {1}", val.ToString("F0"), elapsedTime));
                }
                fetchList.Add(val.ToString("F1"));

                if (fetchList.Count < (nMiddleCheckTime + (mw.INSULATION_RESISTANCE_RAMP_TIME * 2)))
                {
                    if (val == 0.0)
                    {
                        //201012 wjs add recv 0 flag
                        mw.LogState(LogType.Info, "Fetch Meas 0.0 before 10sec");
                        recvZero_Before10S = true;
                    }
                }

                //200721 램프업시간 * 2 수정
                //2.판정 시작 기준(10s)가 FETCH 13번째 Data로 되어 있습니다.
                //    -해당 기준이 26번째 Data가 되어야 합니다.
                //etchList.Count(26) == nMiddleCheckTime(20) + (RAMP_TIME(3) * 2)
                if (fetchList.Count == (nMiddleCheckTime + (mw.INSULATION_RESISTANCE_RAMP_TIME * 2)))
                {
                    //10초가 되었을 때
                    //200721 26초가 되었을 때 임
                    fetchCNTTime = fetchList.Count;
                    mw.LogState(LogType.Info, string.Format("IR 10S : {0} MOhm", val.ToString("F0")));
                    if (recvZero_Before10S)
                    {
                        //10초  한번이라도 0이 나오면 강제로 NG 판정시킨다.(Lee Jae sung)
                        //recvZero_Before10S = false;
                        mw.LogState(LogType.Info, "Fetch Meas 0.0 before 10sec");
                        isFetchFlag = false;
                        fetchCNTTime = fetchList.Count;
                        mw.LogState(LogType.Info, string.Format("IR {0}S : {1} MOhm, IR FETCH Cycle Time {2}S", fetchCNTTime, val.ToString("F0"), mw.FETCH_CHECK_CYCLE_TIME));
                        timeKillEvent(타이머_아이디);
                        isTimerRunning = false;
                        return;
                    }
                    else
                    {
                        if (val > mw.FETCH_STANDARD_MIN_VALUE && val < 9.9E+30)
                        {
                            //10s Value - 9.5s Value 500메가옴 이상 차이면 9.5s 값 판정.
                            double ninepoiintfivevalue = double.Parse(fetchList[fetchCNTTime - 2]);
                            double dTemp = val - double.Parse(fetchList[fetchCNTTime - 2]);

                            if (ninepoiintfivevalue > 160)
                            {
                                if (dTemp > 500)
                                {
                                    mw.LogState(LogType.Info, string.Format("Fetch Meas 10s Value - 9.5s Value : {0} MOhm", dTemp.ToString("F0")));
                                    bIs10sValueNg = true;
                                }
                            }
                            //FETCH 기능 이용하여 초기 10s 때의 값으로 Pass 판정 후 stop
                            timeKillEvent(타이머_아이디);
                            isTimerRunning = false;
                            return;
                        }
                        else
                        {
                            //10초때의 FETCH 값이 120 이하일 경우 Flag를 올린다.
                            isFetchFlag = true;
                        }
                    }
                }

                if (isFetchFlag)
                {
                    //Abnormal Case Park Jin Ho Request add by kyj200703
                    if (fetchList.Count <= mw.INSULATION_RESISTANCE_RAMP_TIME*2)
                    {
                        bIsFetChNg = true;
                        isFetchFlag = false;
                        fetchCNTTime = fetchList.Count;
                        mw.LogState(LogType.Info, string.Format("IR {0}S : {1} MOhm, IR FETCH Cycle Time {2}S", fetchCNTTime, val.ToString("F0"), mw.FETCH_CHECK_CYCLE_TIME));
                        timeKillEvent(타이머_아이디);
                        isTimerRunning = false;
                        return;
                    }

                    //만약 120Mohm 이하 일 경우 5s주기(해당 주기는 option 처리, ex) 1초, 3초, 10초 설정에 따라 주기 변경할 수 있도록)로 check 하여 120Mohm 이상 될 시 결과값 올리고 Pass 처리 후 stop
                    if (fetchList.Count % mw.FETCH_CHECK_CYCLE_TIME == 0)
                    {
                        mw.LogState(LogType.Info, string.Format("FETCH RST: {0} MOhm / Elapsed(msec): {1}", val.ToString("F0"), elapsedTime));
                        if (val > mw.FETCH_STANDARD_MIN_VALUE && val < 9.9E+30)
                        {
                            //10s 때의 값이 120MOhm 이상이 안되는 경우에는 120MOhm 이 넘어가는 시간을 기록
                            isFetchFlag = false;
                            fetchCNTTime = fetchList.Count;                            
                            mw.LogState(LogType.Info, string.Format("IR {0}S : {1} MOhm, IR FETCH Cycle Time {2}S", fetchCNTTime, val.ToString("F0"), mw.FETCH_CHECK_CYCLE_TIME));                            
                            timeKillEvent(타이머_아이디);
                            isTimerRunning = false;
                            return;
                        }
                    }
                    //if (recvZero_Before10S)
                    //{
                    //    //10초  한번이라도 0이 나오면 강제로 NG 판정시킨다.(Lee Jae sung)
                    //    //recvZero_Before10S = false;
                    //    mw.LogState(LogType.Info, "Fetch Meas 0.0 before 10sec");
                    //    isFetchFlag = false;
                    //    fetchCNTTime = fetchList.Count;
                    //    mw.LogState(LogType.Info, string.Format("IR {0}S : {1} MOhm, IR FETCH Cycle Time {2}S", fetchCNTTime, val.ToString("F0"), mw.FETCH_CHECK_CYCLE_TIME));
                    //    timeKillEvent(타이머_아이디);
                    //    isTimerRunning = false;
                    //    return;
                    //}
                    //201012 wjs add for over 10sec if value is 0 case NG
                    if (val == 0.0)
                    {
                        //10초 이후에 한번이라도 0이 나오면 강제로 NG 판정시킨다.(Lee Jae sung)
                        mw.LogState(LogType.Info, "Fetch Meas 0.0");
                        isFetchFlag = false;
                        fetchCNTTime = fetchList.Count;
                        mw.LogState(LogType.Info, string.Format("IR {0}S : {1} MOhm, IR FETCH Cycle Time {2}S", fetchCNTTime, val.ToString("F0"), mw.FETCH_CHECK_CYCLE_TIME));
                        timeKillEvent(타이머_아이디);
                        isTimerRunning = false;
                        return;
                    }
                }

                #region 비정상으로 타니까 일단 주석처리
                //int totalTime = mw.INSULATION_RESISTANCE_RAMP_TIME + mw.INSULATION_RESISTANCE_TIME;
                //if (fetchList.Count == (totalTime - 1)) // jgh Rampup Time 추가로 인해 조건 변경 
                //
                //
                //if (fetchList.Count == ((mw.INSULATION_RESISTANCE_TIME - 1) + mw.INSULATION_RESISTANCE_RAMP_TIME)) 
                //{
                //    // 20191031 Noah Choi 크로마에서 측정시 60초가 넘어가서 값이 59개 밖에 안나옴 
                //    // 59초에서 다시 한번 측정하여 60개 값 얻을 수 있도록 수정 요청(이지현 사원)                   
                //    serialPort.DiscardInBuffer();
                //    Send(0, "SAFE:FETC? MMET\n");
                //    cnt = 0;
                //    while (serialPort.BytesToRead < 15)
                //    {
                //        Thread.Sleep(1);
                //        cnt += 1;
                //        if (cnt == 5000)
                //        {
                //            mw.LogState(LogType.Info, "NOT_RECEIVED_DATA");
                //            return;
                //        }
                //    }
                //    rec = serialPort.BytesToRead;
                //    bt = new byte[rec];
                //    serialPort.Read(bt, 0, rec);
                //    strbuff = Encoding.Default.GetString(bt, 0, rec);
                //
                //    val = 0.0;
                //    double.TryParse(strbuff, out val);
                //    val = val / 1000000;
                //
                //    fetchList.Add(val.ToString("F1"));
                //
                //    // 60s 까지 120Mohm을 넘기지 못할 경우 Fail
                //    mw.LogState(LogType.Info, string.Format("FETCH RST: {0} MOhm / Elapsed(msec): {1}", val.ToString("F0"), elapsedTime));
                //
                //    mw.LogState(LogType.Info, string.Format("IR {0}S : {1} MOhm", mw.INSULATION_RESISTANCE_TIME, val.ToString("F0")));
                //    if (val < mw.FETCH_STANDARD_MIN_VALUE)
                //    {
                //        isFetchOk = false;
                //    }
                //    fetchCNTTime = fetchList.Count;  
                //    isFetchFlag = false;
                //    timeKillEvent(타이머_아이디);
                //    isTimerRunning = false;
                //    return;
                //}

                #endregion
            }
        }
        
        /// <summary>
        /// 절연저항검사용 Resistance
        /// </summary>
        /// <param name="plusch"></param>
        /// <param name="minusch"></param>
        /// <param name="Resistance"></param>
        /// <returns></returns>
        public int GetResistance(int plusch, int minusch, out double Resistance)
        {
            mw.LogState(LogType.Info, "Chroma Set Voltage :" +
               mw.INSULATION_RESISTANCE_VOLT.ToString() + "/ Ramp Up Time : " +
               mw.INSULATION_RESISTANCE_RAMP_TIME.ToString() + "/ Time : " +
               mw.INSULATION_RESISTANCE_TIME.ToString() + "/ FETCH Cycle Time : " +
               mw.FETCH_CHECK_CYCLE_TIME.ToString());

            mw.LogState(LogType.Info, "Chroma19053 Plus Channel :" +
                plusch.ToString() + "/ Minus Channel :" + minusch.ToString());

            int bRet = -1;
            Resistance = -1;

            fetchCNTTime = 0;
            bIs10sValueNg = false;

            serialPort.DiscardInBuffer();
            Send(0, "SOUR:SAFE:STOP\n");
            System.Threading.Thread.Sleep(500);
            if (this.mw.strChromaModelType == "NEW")
            {
                Send(0, String.Format("SAFE:STEP1:IR:CHAN (@1(0))\n"));
                System.Threading.Thread.Sleep(250);
                Send(String.Format("SAFE:STEP1:IR:CHAN (@1({0}))", (plusch).ToString()), 250);
                Send(String.Format("SAFE:STEP1:IR:CHAN:LOW (@1({0}))", (minusch).ToString()), 250);
            }
            else
            {
                Send(0, String.Format("SAFE:STEP1:IR:CHAN(@(0))\n"));
                System.Threading.Thread.Sleep(250);

                #region Hi Channel set
                for (int i = 0; i < 3; i++)
                {
                    mw.LogState(LogType.Info, string.Format("Setting Hi({0}) Channel Try ({1})", plusch.ToString(), (i + 1).ToString()));

                    Send(String.Format("SAFE:STEP1:IR:CHAN(@({0}))", (plusch).ToString()), 250);

                    var rtCh = GetSettingChannel("SAFE:STEP1:IR:CHAN?\n");
                    if (rtCh == -1 || rtCh != plusch)
                    {
                        //retry, 3회시도에도 안되면 빠져나감
                        if (i == 2)
                        {
                            //200609 추가
                            judgementStr = mw._CHAN_HI_FAIL;
                            bRet = -1;
                            return bRet;
                        }
                    }
                    else
                    {
                        //ok
                        break;
                    }
                }
                #endregion

                #region Lo Channel set
                for (int i = 0; i < 3; i++)
                {
                    mw.LogState(LogType.Info, string.Format("Setting Lo({0}) Channel Try ({1})", minusch.ToString(), (i + 1).ToString()));

                    Send(String.Format("SAFE:STEP1:IR:CHAN:LOW (@({0}))", (minusch).ToString()), 250);

                    var rtCh = GetSettingChannel("SAFE:STEP1:IR:CHAN:LOW?\n");
                    if (rtCh == -1 || rtCh != minusch)
                    {
                        //retry, 3회시도에도 안되면 빠져나감
                        if (i == 2)
                        {
                            //200609 추가
                            judgementStr = mw._CHAN_LO_FAIL;
                            bRet = -1;
                            return bRet;
                        }
                    }
                    else
                    {
                        //ok
                        break;
                    }
                }
                #endregion
            }

            Send(0, String.Format("SOUR:SAFE:STEP1:IR:LEV {0}\n", mw.INSULATION_RESISTANCE_VOLT.ToString()));
            System.Threading.Thread.Sleep(250);
            Send(0, String.Format("SOUR:SAFE:STEP1:IR:LIMIT 1000000\n"));
            System.Threading.Thread.Sleep(250);
            Send(0, String.Format("SOUR:SAFE:STEP1:IR:TIME:RAMP {0}\n", mw.INSULATION_RESISTANCE_RAMP_TIME.ToString()));
            System.Threading.Thread.Sleep(250);
            Send(0, String.Format("SOUR:SAFE:STEP1:IR:TIME {0}\n", mw.INSULATION_RESISTANCE_TIME.ToString()));
            System.Threading.Thread.Sleep(250);
            Resistance = new double();

            #region Manual Range Setting
            switch (mw.INSULATION_RESISTANCE_RANGE)
            {
                case "0.01"://check
                    Send(String.Format("SAFE:STEP1:IR:RANG:LOW 0.01"), 250); break;
                case "0.003":
                    Send(String.Format("SAFE:STEP1:IR:RANG:LOW 0.003"), 250); break;
                case "0.0003":
                    Send(String.Format("SAFE:STEP1:IR:RANG:LOW 0.0003"), 250); break;
                case "0.00003":
                    Send(String.Format("SAFE:STEP1:IR:RANG:LOW 0.00003"), 250); break;
                case "0.000003":
                    Send(String.Format("SAFE:STEP1:IR:RANG:LOW 0.000003"), 250); break;
                case "0.0000003":
                    Send(String.Format("SAFE:STEP1:IR:RANG:LOW 0.0000003"), 250); break;
                default://included "A" / "a" / anything str
                    Send(String.Format("SAFE:STEP1:IR:RANG:Auto on"), 250); break;
            }

            //Send(0, String.Format("SAFE:STEP1:IR:RANG {0}\n", mw.INSULATION_RESISTANCE_RANGE.ToString()));
            //System.Threading.Thread.Sleep(120);

            //Send(0, String.Format("SAFE:STEP1:IR:RANG:LOW {0}\n", mw.INSULATION_RESISTANCE_RANGE.ToString()));
            //System.Threading.Thread.Sleep(120);
            #endregion

            Send(0, "SOUR:SAFE:STAR\n");
            mw.LogState(LogType.Info, "SOUR:SAFE:STAR");

            int iTick = Environment.TickCount;
            int irtotaltime = mw.INSULATION_RESISTANCE_RAMP_TIME + mw.INSULATION_RESISTANCE_TIME;

            isFetchFlag = false;
            bIsFetChNg = false;
            //201012 wjs add recv 0 flag
            recvZero_Before10S = false;


            fetchList = new List<string>();
            int timerInterval = 500;
            타이머_핸들러 = new 공용_이벤트핸들러(상시저항가져오는타이머_Tick);
            타이머_아이디 = timeSetEvent(timerInterval, 0, 타이머_핸들러, IntPtr.Zero, 1);
            isTimerRunning = true;
            timeBeginPeriod(1);
            mw.LogState(LogType.Info, string.Format("FETCH MEAS START(Interval:{0})", timerInterval.ToString()));
            while (!CommonMethods.IsTimeOut(iTick, (int)(irtotaltime * 1000) + 500)
                && isTimerRunning)
            {
                if (mw.isStop || mw.ispause)
                {
                    Send(0, "SOUR:SAFE:STOP\n");
                    mw.LogState(LogType.Info, "SOUR:SAFE:STOP");
                    Send(0, "SOUR:SAFE:STOP\n");
                    mw.LogState(LogType.Info, "SOUR:SAFE:STOP");
                    bRet = -1;
                    return -1;
                }
                
                System.Threading.Thread.Sleep(100);
            }
            
            isTimerRunning = false;
            timeKillEvent(타이머_아이디);
            mw.LogState(LogType.Info, "FETCH MEAS STOPPED");

            if (bIsFetChNg == true)
            {
                Send(0, "SOUR:SAFE:STOP\n");
                mw.LogState(LogType.Info, "SOUR:SAFE:STOP");
                Send(0, "SOUR:SAFE:STOP\n");
                mw.LogState(LogType.Info, "SOUR:SAFE:STOP");
            }
            
            //191213 JGH 강인혁 사원 요청으로 Fetch 방식에 Judgement추가
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
            Thread.Sleep(400); //jgh 택타임 축소 요청 이지현사원
            Send(0, "SAFE:RES:STEP1:JUDG?\n");
            Thread.Sleep(500);
            string RcvJudg = string.Empty;
            string fetchMsg = string.Empty;

            var recvCheck = Read(0, 100, CONNECT_TIMEOUT_SEC, out RcvJudg, (byte)'\n');
            mw.LogState(LogType.Info, "Chroma Judgement" + RcvJudg.ToString());

            if (recvCheck > 0)
            {
                try
                {
                    var judge = RcvJudg.Replace("\r\n", "");
                    bRet = 0;

                    if (judge.Equals("116") || judge.Equals("115"))
                    {
                        if (bIs10sValueNg == true)
                        {
                            fetchMsg = fetchList[fetchList.Count - 2]; // 9.5s Value 
                        }
                        else
                        {
                            fetchMsg = fetchList.Last();
                        }

                        mw.LogState(LogType.Info, "Chroma Judgement Msg" + judge.ToString()); //judge확인

                        if (isFetchOk)
                        {
                            //pass
                            bRet = 0;
                            Resistance = double.Parse(fetchMsg);
                            mw.LogState(LogType.Success, string.Format("FETCH MEAS Last Resistance : {0}", Resistance));
                        }
                        else //60s 이후에도 120MOhm 못넘는경우
                        {
                            //200321 JGH Fetch Last값 무한일경우 fetchlist에서 아닌값 가져오도록 with KIH
                            if (fetchMsg == "99000000000000000000000000000000.0")
                            {
                                for (int i = fetchList.Count; i > 0; i--)
                                {
                                    if (fetchList[i] != "99000000000000000000000000000000.0")
                                    {
                                        isFetchOk = true;
                                        Resistance = double.Parse(fetchList[i]);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                //fail   
                                isFetchOk = true;
                                Resistance = double.Parse(fetchMsg); 
                            }
                            mw.LogState(LogType.Fail, string.Format("FETCH MEAS Last Resistance : {0}", Resistance));
                        }
                    }
                    else
                    {
                        mw.LogState(LogType.Fail, "Chroma Result : " + judge.ToString());

                        switch (judge)
                        {
                            case "113": judgementStr = mw._CASE_113; mw.LogState(LogType.Info, "USER STOP"); break;
                            case "114": judgementStr = mw._CASE_114; mw.LogState(LogType.Info, "CAN NOT TEST"); break;
                            case "115": judgementStr = mw._CASE_115; mw.LogState(LogType.Info, "TESTING"); break;
                            case "112": judgementStr = mw._CASE_112; mw.LogState(LogType.Info, "STOP"); break;
                            case "33": judgementStr = mw._CASE_33; mw.LogState(LogType.Info, "DC - HI"); break;
                            case "34": judgementStr = mw._CASE_34; mw.LogState(LogType.Info, "DC - LO"); break;
                            case "35": judgementStr = mw._CASE_35; mw.LogState(LogType.Info, "DC - ARC"); break;
                            case "36": judgementStr = mw._CASE_36; mw.LogState(LogType.Info, "DC - IO"); break;
                            case "37": judgementStr = mw._CASE_37; mw.LogState(LogType.Info, "DC - CHECK LOW"); break;
                            case "38": judgementStr = mw._CASE_38; mw.LogState(LogType.Info, "DC - ADV OVER"); break;
                            case "39": judgementStr = mw._CASE_39; mw.LogState(LogType.Info, "DC - ADI OVER"); break;
                            case "43": judgementStr = mw._CASE_43; mw.LogState(LogType.Info, "DC - IO-F"); break;
                            case "49": judgementStr = mw._CASE_49; mw.LogState(LogType.Info, "IR - HI"); break;
                            case "50": judgementStr = mw._CASE_50; mw.LogState(LogType.Info, "IR - LO"); break;
                            case "52": judgementStr = mw._CASE_52; mw.LogState(LogType.Info, "IR - IO"); break;
                            case "54": judgementStr = mw._CASE_54; mw.LogState(LogType.Info, "IR - ADV OVER"); break;
                            case "55": judgementStr = mw._CASE_55; mw.LogState(LogType.Info, "IR - ADI OVER"); break;
                            case "120": judgementStr = mw._CASE_120_IR; mw.LogState(LogType.Info, "IR - GR CONT."); break;
                            case "121": judgementStr = mw._CASE_121_IR; mw.LogState(LogType.Info, "IR - TRIPPED"); break;
                        }

                        Resistance = -1;
                        bRet = -9;
                    }
                }
                catch
                {
                    Resistance = -1;
                    bRet = -1;
                }
            }
            //jgh 이지현 사원 요청으로 750V검사후 Judgement 못받아올 경우에도 FetchList마지막 값으로 결과값 출력
            if (RcvJudg == "\n" || RcvJudg == "\r\n" || RcvJudg == "")
            {                
                fetchMsg = fetchList.Last();
                mw.LogState(LogType.Info, "Chroma Judgement Msg" + RcvJudg.ToString());

                if (isFetchOk)
                {
                    bRet = 0;
                    Resistance = double.Parse(fetchMsg);
                    mw.LogState(LogType.Success, string.Format("FETCH MEAS Last Resistance : {0}", Resistance));
                }
                else //60s 이후에도 120MOhm 못넘는경우
                {
                    bRet = 0;
                    isFetchOk = true;
                    Resistance = double.Parse(fetchMsg);                    
                    mw.LogState(LogType.Fail, string.Format("FETCH MEAS Last Resistance : {0}", Resistance));
                }
            }

            //201012 wjs 10sec before recv zero
            if (recvZero_Before10S)
            {
                recvZero_Before10S = false;
                Resistance = 0.0;
            }

            Send(0, "SOUR:SAFE:STOP\n");
            Send(0, "SOUR:SAFE:STOP\n");
            if (this.mw.strChromaModelType == "NEW")
            {
                Send(0, String.Format("SAFE:STEP1:IR:CHAN (@1(0))\n"));
                Send(0, String.Format("SAFE:STEP1:IR:CHAN:LOW (@1(0))\n"));
            }
            else
            {
                Send(0, String.Format("SAFE:STEP1:IR:CHAN(@(0))\n"));
                Send(0, String.Format("SAFE:STEP1:IR:CHAN:LOW(@(0))\n"));
            }

            return bRet;
        }
      

        /// <summary>
        /// 절연저항검사용 Resistance
        /// </summary>
        /// <param name="plusch"></param>
        /// <param name="minusch"></param>
        /// <param name="Resistance"></param>
        /// <returns></returns>
        public int GetResistance500(int plusch, int minusch, int volt, int rampT, int time, out double Resistance)
        {
            mw.LogState(LogType.Info, "Chroma Set Voltage :" +
               volt.ToString() + "/ Ramp Up Time : " +
               rampT.ToString() + "/ Time : " +
               time.ToString());

            mw.LogState(LogType.Info, "Chroma19053 Plus Channel :" +
                plusch.ToString() + "/ Minus Channel :" + minusch.ToString());

            int bRet = -1;
            Resistance = -1;

            serialPort.DiscardInBuffer();
            Send(0, "SOUR:SAFE:STOP\n");
            System.Threading.Thread.Sleep(500);
            if (this.mw.strChromaModelType == "NEW")
            {
                Send(0, String.Format("SAFE:STEP1:IR:CHAN (@(0))\n"));
            }
            else
            {
                Send(0, String.Format("SAFE:STEP1:IR:CHAN(@(0))\n"));
            }
                
            System.Threading.Thread.Sleep(120);
            //Send(0, String.Format("SAFE:STEP1:IR:CHAN(@({0}))\n", (plusch).ToString()));
            //System.Threading.Thread.Sleep(120);
            //Send(0, String.Format("SAFE:STEP1:IR:CHAN:LOW (@({0}))\n", (minusch).ToString()));
            //System.Threading.Thread.Sleep(120);
            #region Hi Channel set
            for (int i = 0; i < 3; i++)
            {
                mw.LogState(LogType.Info, string.Format("Setting Hi({0}) Channel Try ({1})", plusch.ToString(), (i + 1).ToString()));

                Send(String.Format("SAFE:STEP1:IR:CHAN(@({0}))", (plusch).ToString()), 250);

                var rtCh = GetSettingChannel("SAFE:STEP1:IR:CHAN?\n");
                if (rtCh == -1 || rtCh != plusch)
                {
                    //retry, 3회시도에도 안되면 빠져나감
                    if (i == 2)
                    {
                        //200609 추가
                        judgementStr = mw._CHAN_HI_FAIL;
                        bRet = -1;
                        return bRet;
                    }
                }
                else
                {
                    //ok
                    break;
                }
            }
            #endregion

            #region Lo Channel set
            for (int i = 0; i < 3; i++)
            {
                mw.LogState(LogType.Info, string.Format("Setting Lo({0}) Channel Try ({1})", minusch.ToString(), (i + 1).ToString()));

                Send(String.Format("SAFE:STEP1:IR:CHAN:LOW (@({0}))", (minusch).ToString()), 250);

                var rtCh = GetSettingChannel("SAFE:STEP1:IR:CHAN:LOW?\n");
                if (rtCh == -1 || rtCh != minusch)
                {
                    //retry, 3회시도에도 안되면 빠져나감
                    if (i == 2)
                    {
                        //200609 추가
                        judgementStr = mw._CHAN_LO_FAIL;
                        bRet = -1;
                        return bRet;
                    }
                }
                else
                {
                    //ok
                    break;
                }
            }
            #endregion
            Send(0, String.Format("SOUR:SAFE:STEP1:IR:LEV {0}\n", volt.ToString()));
            System.Threading.Thread.Sleep(120);
            Send(0, String.Format("SOUR:SAFE:STEP1:IR:LIMIT 30000\n"));
            System.Threading.Thread.Sleep(120);
            Send(0, String.Format("SOUR:SAFE:STEP1:IR:TIME:RAMP {0}\n", rampT.ToString()));
            System.Threading.Thread.Sleep(120);
            Send(0, String.Format("SOUR:SAFE:STEP1:IR:TIME {0}\n", time.ToString()));
            System.Threading.Thread.Sleep(120);
            Resistance = new double();

            Send(0, "SOUR:SAFE:STAR\n");
            mw.LogState(LogType.Info, "SOUR:SAFE:STAR");

            fetchList500 = new List<string>();
            int timerInterval = 1000; //1초마다
            타이머_핸들러 = new 공용_이벤트핸들러(FecthTimer);
            타이머_아이디 = timeSetEvent(timerInterval, 0, 타이머_핸들러, IntPtr.Zero, 1);
            isTimerRunning = true;
            timeBeginPeriod(1);
            mw.LogState(LogType.Info, string.Format("FETCH MEAS START(Delay:{0}ms, Interval:{1}ms)", 700, timerInterval.ToString()));
            int iTick = Environment.TickCount;
            while (!CommonMethods.IsTimeOut(iTick, (int)(time * 1000) + 500)
                && isTimerRunning)
            {
                if (mw.isStop || mw.ispause)
                {
                    Send(0, "SOUR:SAFE:STOP\n");
                    Send(0, "SOUR:SAFE:STOP\n");
                    bRet = -1;
                    return -1;
                }

                System.Threading.Thread.Sleep(100);
            }
            //int iTick = Environment.TickCount;
            //while (CheckState() != 0)   //체크스테이트가 0이면 검사완료
            //{
            //    if (CommonMethods.IsTimeOut(iTick, (int)(time * 1000) + 500))
            //    {
            //        break;
            //    }
            //    if (mw.isStop || mw.ispause)
            //    {
            //        Send(0, "SOUR:SAFE:STOP\n");
            //        bRet = -1;
            //        return -1;
            //    }
            //    System.Threading.Thread.Sleep(600);
            //}

            isTimerRunning = false;
            timeKillEvent(타이머_아이디);
            mw.LogState(LogType.Info, "FETCH MEAS STOPPED");

            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
            Thread.Sleep(1000); //jgh 택타임 축소 요청 이지현사원
            serialPort.Write("SAFE:RES:STEP1:JUDG?\n");
            //Send(0, "SAFE:RES:STEP1:JUDG?\n");
            Thread.Sleep(500);
            string RcvJudg = string.Empty;
            string fetchMsg500 = string.Empty;

            var recvCheck = Read(0, 100, CONNECT_TIMEOUT_SEC, out RcvJudg, (byte)'\n');
            mw.LogState(LogType.Info, "Chroma Judgement" + RcvJudg.ToString());

            if (RcvJudg == "\n" || RcvJudg == "\r\n" || RcvJudg == "")
            {
                Thread.Sleep(500);
                recvCheck = Read(0, 100, CONNECT_TIMEOUT_SEC, out RcvJudg, (byte)'\n');
                mw.LogState(LogType.Info, "Chroma Judgement" + RcvJudg.ToString());
                if (RcvJudg == "\n" || RcvJudg == "\r\n" || RcvJudg == "")
                {
                    Thread.Sleep(500);
                    recvCheck = Read(0, 100, CONNECT_TIMEOUT_SEC, out RcvJudg, (byte)'\n');
                    mw.LogState(LogType.Info, "Chroma Judgement" + RcvJudg.ToString());
                }
            }

            if (recvCheck > 0)
            {
                try
                {
                    var judge = RcvJudg.Replace("\r\n", "");
                    bRet = 0;

                    if (judge.Equals("49"))
                    {
                        mw.LogState(LogType.Fail, "Chroma result:" + judge.ToString());
                        Resistance = 0;
                        bRet = 49;
                    }
                    else if (judge.Equals("50"))
                    {
                        mw.LogState(LogType.Fail, "Chroma result:" + judge.ToString());
                        Resistance = 0;
                        bRet = 50;
                    }
                    else if (judge.Equals("52"))
                    {
                        mw.LogState(LogType.Fail, "Chroma result:" + judge.ToString());
                        Resistance = 0;
                        bRet = 52;
                    }
                    else if (judge.Equals("54"))
                    {
                        mw.LogState(LogType.Fail, "Chroma result:" + judge.ToString());
                        Resistance = 0;
                        bRet = 54;
                    }
                    else if (judge.Equals("55"))
                    {
                        mw.LogState(LogType.Fail, "Chroma result:" + judge.ToString());
                        Resistance = 0;
                        bRet = 55;
                    }
                    else if (judge.Equals("120"))
                    {
                        mw.LogState(LogType.Fail, "Chroma result:" + judge.ToString());
                        Resistance = 0;
                        bRet = 120;
                    }
                    else if (judge.Equals("121"))
                    {
                        mw.LogState(LogType.Fail, "Chroma result:" + judge.ToString());
                        Resistance = 0;
                        bRet = 121;
                    }
                    else if (judge.Equals("112"))
                    {
                        mw.LogState(LogType.Fail, "Chroma result:" + judge.ToString());
                        Resistance = 0;
                        bRet = 112;
                    }
                    else if (judge.Equals("113"))
                    {
                        mw.LogState(LogType.Fail, "Chroma result:" + judge.ToString());
                        Resistance = 0;
                        bRet = 113;
                    }
                    else if (judge.Equals("114"))
                    {
                        mw.LogState(LogType.Fail, "Chroma result:" + judge.ToString());
                        Resistance = 0;
                        bRet = 114;
                    }
                    //200309 JGH judge 115들어올경우 Pass LJH요청
                    //else if (judge.Equals("115")) 
                    //{
                    //    mw.LogState(LogType.Fail, "Chroma result:" + judge.ToString());
                    //    Resistance = 0;
                    //    bRet = 115;
                    //}
                    else
                    {
                        fetchMsg500 = fetchList500.Last();
                        double dFetch = double.Parse(fetchList500.Last());
                        mw.LogState(LogType.Info, "Chroma Judgement Msg" + judge.ToString()); //judge확인

                        //200321 JGH Fetch Last값 무한일경우 fetchlist에서 아닌값 가져오도록 with KIH
                        if (fetchMsg500 == "99000000000000000000000000000000.0")
                        {
                            for (int i = fetchList500.Count - 1; i > 0; i--)
                            {
                                fetchList500[15] = "111";
                                if (fetchList500[i] != "99000000000000000000000000000000.0")
                                {
                                    Resistance = double.Parse(fetchList500[i]);
                                    bRet = 0;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Resistance = double.Parse(fetchMsg500);
                            bRet = 0;
                        }
                        mw.LogState(LogType.Fail, string.Format("FETCH MEAS Last Resistance : {0}", Resistance));
                        
                    }
                }
                catch
                {
                    Resistance = -1;
                    bRet = -1;
                }
            }
            Send(0, "SOUR:SAFE:STOP\n");
            Send(0, "SOUR:SAFE:STOP\n");
            Send(0, String.Format("SAFE:STEP1:IR:CHAN(@(0))\n"));
            Send(0, String.Format("SAFE:STEP1:IR:CHAN:LOW(@(0))\n"));
            return bRet;
        }


        public int Send(int index, string strMessage)
        {
            serialPort.Write(strMessage);
            return -1;
        }

        /// <summary>
        /// 보내고 슬립하는 메서드
        /// </summary>
        /// <param name="sendStr">보낼 문자열</param>
        /// <param name="sleepInterval">대기시길 시간</param>
        private void Send(string sendStr, int delayTimeMSEC)
        {
            serialPort.WriteLine(sendStr);
            if (delayTimeMSEC > 0)
            {
                Thread.Sleep(delayTimeMSEC);
            }
        }

        public int CheckState()
        {
            serialPort.DiscardInBuffer();
            serialPort.Write("SOUR:SAFE:STATUS?\n");
            int cnt = 0;
            while (serialPort.BytesToRead < 9)
            {
                Thread.Sleep(1);
                cnt += 1;
                if (cnt == 5000)
                {
                    mw.LogState(LogType.Info, "NOT_RECEIVED_DATA");
                    return -1;
                }
            }

            var rec = serialPort.BytesToRead;
            var bt = new byte[rec];
            serialPort.Read(bt, 0, rec);
            var strbuff = Encoding.Default.GetString(bt, 0, rec);

            if (strbuff.Contains("STOPPED"))
            {
                mw.LogState(LogType.Info, "Contains stop signal.");
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private void FecthTimer(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            Stopwatch st = new Stopwatch();
            st.Start();

            serialPort.DiscardInBuffer();
            Send(0, "SAFE:FETC? MMET\n");
            var cnt = 0;
            while (serialPort.BytesToRead < 15)
            {
                Thread.Sleep(1);
                cnt += 1;
                if (cnt == 2000)
                {
                    mw.LogState(LogType.Info, "NOT_RECEIVED_DATA");
                    st.Stop();
                    st = null;
                    return;
                }
            }
            var rec = serialPort.BytesToRead;
            var bt = new byte[rec];
            serialPort.Read(bt, 0, rec);
            var strbuff = Encoding.Default.GetString(bt, 0, rec);

            double val = 0.0;
            double.TryParse(strbuff, out val);
            val = val / 1000000;
            var elapsedTime = st.ElapsedMilliseconds;
            st.Stop();
            st = null;

            serialPort.DiscardInBuffer();
            Send(0, "SOUR:SAFE:STATUS?\n");
            cnt = 0;

            while (serialPort.BytesToRead < 9)
            {
                Thread.Sleep(1);
                cnt += 1;
                if (cnt == 2000)
                {
                    mw.LogState(LogType.Info, "NOT_RECEIVED_DATA");
                    return;
                }
            }
            rec = serialPort.BytesToRead;
            bt = new byte[rec];
            serialPort.Read(bt, 0, rec);
            strbuff = Encoding.Default.GetString(bt, 0, rec);

            if (strbuff.Contains("STOPPED"))
            {
                timeKillEvent(타이머_아이디);
                isTimerRunning = false;
                return;
            }
            else
            {
                mw.LogState(LogType.Info, string.Format("FETCH RST: {0} MOhm / Elapsed(msec): {1}", val.ToString("F0"), elapsedTime));
                fetchList500.Add(val.ToString("F1"));
            }
            if (fetchList500.Count == 19) //fetch 9개 들어올경우
            {
                serialPort.DiscardInBuffer();
                Send(0, "SAFE:FETC? MMET\n");
                cnt = 0;
                while (serialPort.BytesToRead < 15)
                {
                    Thread.Sleep(1);
                    cnt += 1;
                    if (cnt == 2000)
                    {
                        mw.LogState(LogType.Info, "NOT_RECEIVED_DATA");
                        return;
                    }
                }
                rec = serialPort.BytesToRead;
                bt = new byte[rec];
                serialPort.Read(bt, 0, rec);
                strbuff = Encoding.Default.GetString(bt, 0, rec);

                val = 0.0;
                double.TryParse(strbuff, out val);
                val = val / 1000000;

                mw.LogState(LogType.Info, string.Format("FETCH RST: {0} MOhm / Elapsed(msec): {1}", val.ToString("F0"), elapsedTime));
                fetchList500.Add(val.ToString("F1"));
            }
        }

        #region Override Read methods
        public int Read(int index, int iLength, int iTimeout, out byte[] data)
        {
            return this.Read(index, iLength, iTimeout, out data, false);
        }

        public int Read(int index, int iLength, int iTimeout, out byte[] data, bool bCloseByReadFailure)
        {
            data = new byte[iLength];
            //if (this.Connected)
            {
                try
                {
                    Stream stream = serialPort.BaseStream;
                    stream.ReadTimeout = iTimeout;
                    int count = iLength;
                    int length = 0;
                    while (count > 0)
                    {
                        int num;
                        try
                        {
                            num = stream.Read(data, data.Length - count, count);
                            if (num != 0)
                                Thread.Sleep(1);
                            else
                                break;
                        }
                        catch (Exception ex)
                        {

                            if (bCloseByReadFailure)
                            {
                                serialPort.Close();
                                serialPort.Open();
                                break;
                            }
                            break;
                        }
                        count -= num;
                        length += num;
                    }
                    if (length > 0)
                    {
                        byte[] byData = new byte[length];
                        Array.Copy((Array)data, 0, (Array)byData, 0, length);
                        data = byData;
                    }
                    return length;
                }
                catch (Exception ex)
                {
                    //SystemLogger.Log(ex);
                }
            }
            return -1;
        }

        public int Read(int index, int iLength, int iTimeout, out byte[] data, byte byEndByte)
        {
            return this.Read(index, iLength, iTimeout, out data, byEndByte, false);
        }

        public int Read(int index, int iLength, int iTimeout, out byte[] data, byte byEndByte, bool bCloseByReadFailure)
        {
            data = new byte[iLength];
            //if (this.Connected)
            {
                try
                {
                    Stream stream = serialPort.BaseStream;
                    stream.ReadTimeout = iTimeout;
                    int length = 0;
                    while (iLength > length)
                    {
                        try
                        {
                            if (stream.Read(data, length, 1) != 0)
                            {
                                ++length;
                                if ((int)data[length - 1] == (int)byEndByte)
                                    break;
                            }
                            else
                                break;
                        }
                        catch (Exception ex)
                        {
                            if (bCloseByReadFailure)
                            {
                                serialPort.Close();
                                serialPort.Open();
                                break;
                            }
                            break;
                        }
                    }
                    if (length > 0)
                    {
                        byte[] byData = new byte[length];
                        Array.Copy((Array)data, 0, (Array)byData, 0, length);
                        data = byData;
                        CSerialPacket cserialPacket = new CSerialPacket(enumSerialPacketType.RECV, byData, this.Encoder);
                    }
                    return length;
                }
                catch (Exception ex)
                {
                    //SystemLogger.Log(ex);
                }
            }
            return -1;
        }

        public int Read(int index, int iLength, int iTimeout, out string data, byte byEndByte)
        {
            return this.Read(index, iLength, iTimeout, out data, byEndByte, false);
        }

        public int Read(int index, int iLength, int iTimeout, out string data, byte byEndByte, bool bCloseByReadFailure)
        {
            byte[] data1 = (byte[])null;
            data = string.Empty;
            int num;
            if ((num = this.Read(index, iLength, iTimeout, out data1, byEndByte, bCloseByReadFailure)) <= 0)
                return -1;
            data = this.Encoder.GetString(data1, 0, data1.Length);
            return num;
        }

        public int Read(int index, int iLength, int iTimeout, out string data)
        {
            return this.Read(index, iLength, iTimeout, out data, false);
        }

        public int Read(int index, int iLength, int iTimeout, out string data, bool bCloseByReadFailure)
        {
            byte[] data1 = (byte[])null;
            data = string.Empty;
            int num;
            if ((num = this.Read(index, iLength, iTimeout, out data1, bCloseByReadFailure)) != iLength)
                return -1;
            data = this.Encoder.GetString(data1, 0, data1.Length);
            return num;
        }
        #endregion
    }


    class CommonMethods
    {
        public static bool IsTimeOut(int iStartTick, int iTimeoutMilleSec)
        {
            uint num1 = (uint)((ulong)iStartTick - 18446744071562067968UL);
            uint num2 = (uint)((ulong)Environment.TickCount - 18446744071562067968UL);
            return (num1 > num2 ? -1 - ((int)num1 - (int)num2) : (int)num2 - (int)num1) >= iTimeoutMilleSec;
        }

        public static bool TimeoutTimer(object expectvalue, object realValue, int time, GetDiffType gt, string id = "")
        {
            if (gt == GetDiffType.defaultDiff)
            {

                for (int i = 0; i < time * 1000; i++)
                {
                    if (expectvalue.ToString() == realValue.ToString())
                    {
                        return true;
                    }
                    System.Threading.Thread.Sleep(1);
                }

                return false;
            }
            else if (gt == GetDiffType.ContainsinBMS)
            {
                for (int i = 0; i < time * 1000; i++)
                {
                    if ((realValue as Dictionary<string, Peak.Can.Basic.ReceiveType>).ContainsKey(expectvalue.ToString()))
                    {
                        return true;
                    }
                    System.Threading.Thread.Sleep(1);
                }

                return false;
            }
            else if (gt == GetDiffType.ContainsinBMSData)
            {
                for (int i = 0; i < time * 1000; i++)
                {
                    if ((realValue as Dictionary<string, Peak.Can.Basic.ReceiveType>)[id].Data.Contains(expectvalue.ToString()))
                    {
                        return true;
                    }
                    System.Threading.Thread.Sleep(1);
                }

                return false;
            }
            return false;


        }
    }
    
    public enum GetDiffType
    {
        defaultDiff,
        ContainsinBMS,
        ContainsinBMSData
    }
}
