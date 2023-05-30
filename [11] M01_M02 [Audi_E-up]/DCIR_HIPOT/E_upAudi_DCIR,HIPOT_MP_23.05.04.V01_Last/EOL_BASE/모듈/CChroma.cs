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
    public enum GetDiffType
    {
        defaultDiff,
        ContainsinBMS,
        ContainsinBMSData
    }
    class CommonMethods
    {
        public static bool IsTimeOut(int iStartTick, int iTimeoutMilleSec)
        {
            uint num1 = (uint)((ulong)iStartTick - 18446744071562067968UL);
            uint num2 = (uint)((ulong)Environment.TickCount - 18446744071562067968UL);
            return (num1 > num2 ? -1 - ((int)num1 - (int)num2) : (int)num2 - (int)num1) >= iTimeoutMilleSec;
        }

    }

    public class CFecthData
    {
        public string m_strMeasure;     // 측정 값
        public string m_strProcTime;    // 측정 실제 동작시간
        public string m_strOutVolt;     // 측정 출력 전압
    }

    public enum EN_FETCH_KEY
    {
        SEC_10 = 10,                    // 01 ~ 10초 데이터
        SEC_20 = 20,                    // 11 ~ 20초 데이터
        SEC_30 = 30,                    // 21 ~ 30초 데이터
        SEC_40 = 40,                    // 31 ~ 40초 데이터
        SEC_50 = 50,                    // 41 ~ 50초 데이터
        SEC_60 = 60,                    // 51 ~ 60초 데이터

        // 계측 시간이 늘어난다면 이어서 확장
    }


    public class CChroma
    {
        //int CONNECTION__COMMUNICATION_TIMEOUT_SEC = 20;
        //int CON_WITHSTANDING_VOLTAGE_T = 5;//시간
        //int CON_WITHSTANDING_VOLTAGE_V = 1000;//전압 

        public SerialPort serialPort;
        MainWindow mw;

        /// <summary>
        /// 사용 전후로 반드시 비워라!
        /// </summary>
        //public List<string> fetch_list = new List<string>();
        public Dictionary<EN_FETCH_KEY, List<CFecthData>> m_mapFetchData = new Dictionary<EN_FETCH_KEY, List<CFecthData>>();

        /// <summary>
        /// 사용 전후로 반드시 꺼둬라!!
        /// </summary>
        public bool isUseFetch = false;

        public bool isAlive = false;

        /// <summary>
        /// SerialSpec
        /// port=COM4 / baud=9600 / parity=none / data=8 / stop=1
        /// </summary>
        public CChroma(MainWindow mw)
        {
            if (mw.chroma_PortName == "")
            {
                return;
            }

            this.mw = mw;
            serialPort = new System.IO.Ports.SerialPort();
            serialPort.DataBits = 8;
            serialPort.BaudRate = 9600;
            serialPort.PortName = mw.chroma_PortName;
            serialPort.Parity = System.IO.Ports.Parity.None;
            serialPort.StopBits = System.IO.Ports.StopBits.One;

            CONNECTION__COMMUNICATION_TIMEOUT_SEC = 2;

            try
            {
                serialPort.Open();
                if (serialPort.IsOpen && IDN() != "")
                {
                    mw.LogState(LogType.DEVICE_CHECK, "Chroma Open Success - " + serialPort.PortName);
                    isAlive = true;
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_hipot.Background = System.Windows.Media.Brushes.Green;
                    }));
                }
                else
                {
                    mw.LogState(LogType.DEVICE_CHECK, "Chroma Open Fail - " + serialPort.PortName);
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_hipot.Background = System.Windows.Media.Brushes.Red;

                    }));
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.DEVICE_CHECK, "Chroma Exception - " + serialPort.PortName,ec);
            }
        } 

        public void Discard()
        {
            if(serialPort != null)
            {
                if(serialPort.IsOpen)
                {
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    mw.LogState(LogType.Info, "Chroma : Discard IN/OUT Buffer");
                }
            }
        }

        public void _Dispose()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
            }
        }

        public string IDN()
        {
            string idn = "";
            int cnt = 0;
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                serialPort.WriteLine("*IDN?");
                while (serialPort.BytesToRead < 10)
                {
                    cnt++;
                    Thread.Sleep(100);
                    if (cnt > 10)
                    {
                        return idn;
                    }
                }
                int rec = serialPort.BytesToRead;
                //받은 후에 데이터 파싱
                byte[] bt = new byte[rec];
                serialPort.Read(bt, 0, rec);
                var rtstring = Encoding.Default.GetString(bt, 0, rec);
                idn = rtstring;

                //LogState(LogType.Success, "Chroma - " + rtstring);
            }

            return idn;
        }

        #region
        /// <summary>
        /// 
        ///  (Access Type : In/Out, Data Type : int)
        /// </summary>
        public int CONNECTION__COMMUNICATION_TIMEOUT_SEC
        {
            get;
            set;
        }

        /// <summary>
        /// Capactiance
        ///  (Access Type : In/Out, Data Type : double)
        /// </summary>
        public double I_CAPACTANCE
        {
            get;
            set;
        }

        /// <summary>
        /// Resistance
        ///  (Access Type : In/Out, Data Type : double)
        /// </summary>
        public double I_RESISTANCE
        {
            get;
            set;
        }

        /// <summary>
        /// Current
        ///  (Access Type : In/Out, Data Type : double)
        /// </summary>
        public double I_CURRENT
        {
            get;
            set;
        }

        /// <summary>
        /// Result Code
        ///  (Access Type : In/Out, Data Type : double)
        /// </summary>
        public double I_RESULTCODE
        {
            get;
            set;
        }

        /// <summary>
        /// Result Text
        ///  (Access Type : In/Out, Data Type : string)
        /// </summary>
        public string I_RESULTTEXT
        {
            get;
            set;
        }

        /// <summary>
        /// Emg Stop
        ///  (Access Type : In/Out, Data Type : bool)
        /// </summary>
        public bool I_EMGSTOP
        {
            get;
            set;
        }
        #endregion



        public int Read(System.IO.Ports.SerialPort sp, int index, int iLength, int iTimeout, out string data, byte byEndByte, bool bCloseByReadFailure)
        {
            byte[] data1 = (byte[])null;
            data = string.Empty;
            int num;
            if ((num = this.Read(sp, index, iLength, iTimeout, out data1, byEndByte, bCloseByReadFailure)) <= 0)
                return -1;
            data = Encoding.ASCII.GetString(data1, 0, data1.Length);
            return num;
        }

        public int Read(System.IO.Ports.SerialPort sp, int index, int iLength, int iTimeout, out byte[] data, byte byEndByte, bool bCloseByReadFailure)
        {
            data = new byte[iLength];
            //if (this.Connected)
            {
                try
                {
                    Stream stream = sp.BaseStream;
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
                                sp.Close();
                                sp.Open();
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
                        CSerialPacket cserialPacket = new CSerialPacket(enumSerialPacketType.RECV, byData, Encoding.ASCII);
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns>0:Stoppped, 1:Testing, 2:Discharging, -1:Timeout</returns>
        /// not use
        /*
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
                if (isUseFetch)
                {
                    serialPort.DiscardInBuffer();
                    sp.Write("SAFE:FETC? MMET\n");

                    cnt = 0;
                    while (serialPort.BytesToRead < 15)//33
                    {
                        Thread.Sleep(10);
                        cnt += 10;
                        //not received data
                        if (cnt == 5000)
                        {
                            mw.LogState(LogType.Info, "NOT_RECEIVED_DATA");
                            return 0;
                        }
                    }
                    rec = serialPort.BytesToRead;
                    //받은 후에 데이터 파싱
                    bt = new byte[rec];
                    serialPort.Read(bt, 0, rec);
                    strbuff = Encoding.Default.GetString(bt, 0, rec);


                    double val = 0.0;
                    double.TryParse(strbuff, out val);
                    val = val / 1000000;
                    mw.LogState(LogType.Info, string.Format("FETCH Result: {0} /Time: {1}", val.ToString("F0"), DateTime.Now.Millisecond));
                    fetch_list.Add(val.ToString("F0"));
                }
                return 1;
            }
        }
        */

        public int Read(System.IO.Ports.SerialPort sp, int index, int iLength, int iTimeout, out string data)
        {
            return this.Read(sp, index, iLength, iTimeout, out data, false);
        }

        public int Read(System.IO.Ports.SerialPort sp, int index, int iLength, int iTimeout, out string data, bool bCloseByReadFailure)
        {
            byte[] data1 = (byte[])null;
            data = string.Empty;
            int num;
            if ((num = this.Read(sp, index, iLength, iTimeout, out data1, bCloseByReadFailure)) != iLength)
                return -1;
            data = Encoding.ASCII.GetString(data1, 0, data1.Length);
            return num;
        }


        public int Read(System.IO.Ports.SerialPort sp, int index, int iLength, int iTimeout, out byte[] data, bool bCloseByReadFailure)
        {
            data = new byte[iLength];
            //if (this.Connected)
            {
                try
                {
                    Stream stream = sp.BaseStream;
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
                                sp.Close();
                                sp.Open();
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


        //public int Read(System.IO.Ports.SerialPort sp,int index, int iLength, int iTimeout, out string data)
        //{
        //    return this.Read(sp,index, iLength, iTimeout, out data, false);
        //}

        public int ChangeRemote(System.IO.Ports.SerialPort sp)
        {
            sp.Write("SYSTem:REMote\n");
            System.Threading.Thread.Sleep(300);

            return 0;
        }

        public int EmgStop()
        {
            I_EMGSTOP = true;

            return 0;
        }

        public int EmgStopClear()
        {
            I_EMGSTOP = false;

            return 0;
        }

        StringBuilder logSb;
        /// <summary>
        /// 보내고 슬립하는 메서드
        /// </summary>
        /// <param name="sendStr">보낼 문자열</param>
        /// <param name="sleepInterval">대기시길 시간</param>
        private void Send(string sendStr,int delayTimeMSEC)
        {
            serialPort.WriteLine(sendStr);
            logSb = new StringBuilder();
            logSb.Append(sendStr);
            logSb.Append(", Interval:");
            logSb.Append(delayTimeMSEC.ToString());
            mw.LogState(LogType.Info, logSb.ToString());
            if (delayTimeMSEC > 0)
            {
                Thread.Sleep(delayTimeMSEC);
            }
        }
        //200616 추가
        public string CON_INSULATION_RESISTANCE_RANGE = "Auto";

        //200622 추가
        //public string CON_WITHSTANDING_RANGE = "Auto";

        public double CON_INSULATION_RESISTANCE_V = 0;
        public double CON_INSULATION_RESISTANCE_T = 0;
        public double CON_WITHSTANDING_VOLTAGE_V = 0;
        public double CON_WITHSTANDING_VOLTAGE_T = 0;
        public double CON_WITHSTANDING_RAMP_UP_TIME = 0;
        public double CON_WITHSTANDING_FALLDOWN_TIME = 0;
        public double CON_ARC_ENABLE = 0;
        public double CON_ARC_LIMIT = 0;

        public string judgementStr = "";

        private int GetSettingChannel(string cmd)
        {
            int rcvVal = -1;

            try
            {
                serialPort.Write(cmd);

                byte[] btBuff = RecvChroma19053();

                if (btBuff == null)
                {
                    mw.LogState(LogType.Info, "NOT_RECEIVED_DATA");

                    return -1;
                }

                var strbuff = Encoding.Default.GetString(btBuff, 0, btBuff.Length);

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

        /// <summary>
        /// IR (200612)
        /// </summary>
        /// <param name="plusch"></param>
        /// <param name="minusch"></param>
        /// <param name="Resistance"></param>
        /// <returns></returns>
        public int GetResistance(int plusch, int minusch, out double Resistance)
        {
            judgementStr = "";

            try
            {
                if(isUseFetch == true)
                {
                    InitFetchData();
                }

                if (serialPort.IsOpen)
                {
                    mw.LogState(LogType.Info, "Chroma19053 Set Voltage :" +
                        CON_INSULATION_RESISTANCE_V.ToString() + "/ Time : " +
                        CON_INSULATION_RESISTANCE_T.ToString() + "/ Range(A) : " +
                        CON_INSULATION_RESISTANCE_RANGE.ToString());

                    mw.LogState(LogType.Info, "Chroma19053 Plus Channel :" +
                        plusch.ToString() + "/ Minus Channel :" + minusch.ToString());

                    int bRet = -1;
                    Resistance = -1;

                    serialPort.DiscardInBuffer();
                    Send("SOUR:SAFE:STOP", 250);

                    #region Channel Initialize
                    Send("SAFE:STEP1:IR:CHAN(@(0))", 250);
                    Send("SAFE:STEP1:IR:CHAN:LOW(@(0))", 250);
                    #endregion

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

                    Send(String.Format("SOUR:SAFE:STEP1:IR:LEV {0}", CON_INSULATION_RESISTANCE_V.ToString()), 250);
                    Send(String.Format("SOUR:SAFE:STEP1:IR:TIME {0}", CON_INSULATION_RESISTANCE_T.ToString()), 250);

                    #region Manual range setting

                    //10mA  0.01
                    //3mA   0.003
                    //300uA 0.0003
                    //30uA  0.00003
                    //3uA   0.000003
                    //300nA 0.0000003
                    //Auto  Auto
                    //Send(String.Format("SAFE:STEP1:IR:RANG:{0}", CON_INSULATION_RESISTANCE_RANGE), 250);
                    //Send(String.Format("SAFE:STEP1:IR:RANG:LOW {0}", CON_INSULATION_RESISTANCE_RANGE), 250);

                    switch (CON_INSULATION_RESISTANCE_RANGE)
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

                    //Send(String.Format("SAFE:STEP1:IR:RANG:LOW {0}", 0.01), 250);//10mA
                    //Send(String.Format("SAFE:STEP1:IR:RANG:LOW {0}", 0.003), 250);//3mA
                    //Send(String.Format("SAFE:STEP1:IR:RANG:LOW {0}", 0.0003), 250);//300uA
                    //Send(String.Format("SAFE:STEP1:IR:RANG:LOW {0}", 0.00003), 250);//30uA
                    //Send(String.Format("SAFE:STEP1:IR:RANG:LOW {0}", 0.000003), 250);//3uA
                    //Send(String.Format("SAFE:STEP1:IR:RANG:LOW {0}", 0.0000003), 250);//300nA
                    #endregion

                    Send("SOUR:SAFE:STAR", 250);

                    #region wait for running

                    if(isUseFetch == false)
                    {
                        if (CheckState() == -1)
                        {
                            return -1;
                        }
                    }

                    #endregion

                    #region Fetch

                    if (isUseFetch == true)
                    {
                         if (RunFetch() == -1)
                        {
                             return -1;
                         }
                    }
                    
                    #endregion

                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();

                    string judgementRcvMessage = "";
                    string judgement = "";

                    Send("SAFE:RES:STEP1:JUDG?", 500);

                    int recvCheck = -1;

                    byte[] btBuff = RecvChroma19053();

                    if(btBuff != null)
                    {
                        judgementRcvMessage = Encoding.ASCII.GetString(btBuff, 0, btBuff.Length);

                        recvCheck = btBuff.Length;
                    }
                    
                    //recvCheck = Read(serialPort, 0, 100, CONNECTION__COMMUNICATION_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n', false);
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

                                recvCheck = -1;

                                btBuff = RecvChroma19053();

                                if (btBuff != null)
                                {
                                    judgementRcvMessage = Encoding.ASCII.GetString(btBuff, 0, btBuff.Length);

                                    recvCheck = btBuff.Length;
                                }

                                //recvCheck = Read(serialPort, 0, 100, CONNECTION__COMMUNICATION_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n', false);
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

                                recvCheck = -1;

                                btBuff = RecvChroma19053();

                                if (btBuff != null)
                                {
                                    judgementRcvMessage = Encoding.ASCII.GetString(btBuff, 0, btBuff.Length);

                                    recvCheck = btBuff.Length;
                                }

                                //recvCheck = Read(serialPort, 0, 100, CONNECTION__COMMUNICATION_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n', false);
                                mw.LogState(LogType.Info, string.Format("Recv Message:{0}", judgementRcvMessage));

                                if (judgementRcvMessage == "\n" || judgementRcvMessage == "\r\n" || judgementRcvMessage == "")
                                {
                                    Thread.Sleep(500);

                                    recvCheck = -1;

                                    btBuff = RecvChroma19053();

                                    if (btBuff != null)
                                    {
                                        judgementRcvMessage = Encoding.ASCII.GetString(btBuff, 0, btBuff.Length);

                                        recvCheck = btBuff.Length;
                                    }

                                    //recvCheck = Read(serialPort, 0, 100, CONNECTION__COMMUNICATION_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n', false);
                                    mw.LogState(LogType.Info, string.Format("Recv Message:{0}", judgementRcvMessage));
                                }

                                if (recvCheck > 0)
                                {
                                    try
                                    {
                                        //성공
                                        Resistance = double.Parse(judgementRcvMessage);
                                        bRet = 0;
                                        mw.LogState(LogType.Info, string.Format("parsed resistance. resistance: {0}, recv msg: {1}", Resistance, judgementRcvMessage));
                                    }
                                    catch
                                    {
                                        Resistance = 0;
                                        bRet = -1;
                                        mw.LogState(LogType.Info, string.Format("not parsed resistance. recv msg: {0}", judgementRcvMessage));
                                    }
                                }//200227 added case
                                else
                                {
                                    Resistance = 0;
                                    bRet = -1;
                                    mw.LogState(LogType.Info, string.Format("not received resistance. recv msg: {0}", judgementRcvMessage));
                                }
                            }
                            else
                            {
                                mw.LogState(LogType.Fail, "Chroma Result : " + judgement.ToString());

                                switch (judgement)
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
                    else
                    {
                        Resistance = 0;
                        bRet = -1;
                        mw.LogState(LogType.Info, string.Format("not Received judgement Str. recv msg: {0}", judgementRcvMessage));
                    }

                    Send("SAFE:STEP1:IR:CHAN(@(0))", 250);
                    Send("SAFE:STEP1:IR:CHAN:LOW(@(0))", 250);

                    Send("SOUR:SAFE:STOP", 0);
                    Send("SOUR:SAFE:STOP", 0);

                    return bRet;
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "Chroma_GetCapacitance", ec);
                if (serialPort.IsOpen)
                {
                    Send("SOUR:SAFE:STOP", 100);
                    Send("SOUR:SAFE:STOP", 0);
                }
                Resistance = -1;
                return -1;
            }
            Resistance = -1;
            return -1;
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
                        (CON_ARC_LIMIT).ToString() + "/ TimeOut : " +
                        CONNECTION__COMMUNICATION_TIMEOUT_SEC.ToString());// + "/ Range(A) : " +
                        //CON_WITHSTANDING_RANGE.ToString());

                    mw.LogState(LogType.Info, "Chroma19053 Plus Channel :" +
                        plusch.ToString() + "/ Minus Channel :" + minusch.ToString());

                    int bRet = -1;
                    Current = -1;

                    serialPort.DiscardInBuffer();
                    Send("SOUR:SAFE:STOP", 250);

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

                    Send(String.Format("SOUR:SAFE:STEP1:DC:LEV {0}", CON_WITHSTANDING_VOLTAGE_V.ToString()), 250);
                    Send("SOUR:SAFE:STEP1:DC:LIMIT 0.002999",250);
                    Send(String.Format("SOUR:SAFE:STEP1:DC:TIME:RAMP {0}", CON_WITHSTANDING_RAMP_UP_TIME.ToString()), 250);
                    // 2020.02.27 Noah Choi Falldown 명령어 추가 진행
                    Send(String.Format("SOUR:SAFE:STEP1:DC:TIME:FALL {0}", CON_WITHSTANDING_FALLDOWN_TIME.ToString()), 250);
                    Send(String.Format("SOUR:SAFE:STEP1:DC:TIME {0}", CON_WITHSTANDING_VOLTAGE_T.ToString()), 250);

                    if (int.Parse(CON_ARC_ENABLE.ToString()) == 1)
                    {
                        Send(String.Format("SOUR:SAFE:STEP1:DC:LIM:ARC {0}", (CON_ARC_LIMIT * 0.001).ToString()), 250);
                    }

                    Send("SOUR:SAFE:STAR", 250);

                    #region wait for running

                    if (isUseFetch == false)
                    {
                        if (CheckState() == -1)
                        {
                            return -1;
                        }
                    }

                    #endregion

                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();

                    string judgementRcvMessage = "";
                    string judgement = "";

                    Send("SAFE:RES:STEP1:JUDG?",500);
                    int recvCheck = -1;

                    recvCheck = Read(serialPort, 0, 100, CONNECTION__COMMUNICATION_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n', false);
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
                                recvCheck = Read(serialPort, 0, 100, CONNECTION__COMMUNICATION_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n', false);
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

                                recvCheck = Read(serialPort, 0, 100, CONNECTION__COMMUNICATION_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n', false);
                                mw.LogState(LogType.Info, string.Format("Recv Message:{0}", judgementRcvMessage));

                                if (judgementRcvMessage == "\n" || judgementRcvMessage == "\r\n" || judgementRcvMessage == "")
                                {
                                    Thread.Sleep(500);
                                    recvCheck = Read(serialPort, 0, 100, CONNECTION__COMMUNICATION_TIMEOUT_SEC, out judgementRcvMessage, (byte)'\n', false);
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
                                mw.LogState(LogType.Fail, "Chroma Result : " + judgement.ToString());

                                switch (judgement)
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

                    Send("SAFE:STEP1:DC:CHAN(@(0))", 250);
                    Send("SAFE:STEP1:DC:CHAN:LOW(@(0))", 250);

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

        /// <summary>
        /// OSC (200612)
        /// </summary>
        /// <param name="plusch"></param>
        /// <param name="minusch"></param>
        /// <param name="Capacitance"></param>
        /// <returns></returns>
        public int GetCapacitance(int plusch, int minusch, out double Capacitance)
        {
            judgementStr = "";
            Capacitance = new double();

            try
            {
                if (serialPort.IsOpen)
                {
                    mw.LogState(LogType.Info, "Chroma19053 Plus Channel :" +
                        plusch.ToString() + "/ Minus Channel :" + minusch.ToString());

                    int bRet = -1;

                    serialPort.DiscardInBuffer();
                    Send("SOUR:SAFE:STOP", 250);

                    #region Channel Initialize
                    Send("SAFE:STEP1:OSC:CHAN(@(0))", 250);
                    Send("SAFE:STEP1:OSC:CHAN:LOW(@(0))", 250);
                    #endregion

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

                    Send("SAFE:STEP1:OSC:TIME 3.0", 250);
                    Send("SAFE:STEP1:OSC:LIM:SHOR 1", 250);
                    Send("SAFE:STAR:CST GET", 250);
                    Send("SAFE:STEP1:OSC:CST 3, 0.0000000200", 250);

                    Send("SOUR:SAFE:STAR", 250);

                    #region wait for running

                    if (isUseFetch == false)
                    {
                        if (CheckState() == -1)
                        {
                            return -1;
                        }
                    }

                    #endregion

                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();

                    Send("SAFE:RES:ALL:MMET?", 500);
                    string RcvMessage = string.Empty;

                    int recvCheck = -1;

                    recvCheck = Read(serialPort, 0, 100, CONNECTION__COMMUNICATION_TIMEOUT_SEC, out RcvMessage, (byte)'\n', false);
                    mw.LogState(LogType.Info, string.Format("Recv Message:{0}", RcvMessage));

                    if (recvCheck > 0)
                    {
                        Capacitance = double.Parse(RcvMessage);
                        mw.LogState(LogType.Info, string.Format("parsed capacitance. capacitance: {0}, recv msg: {1}", Capacitance, RcvMessage));

                        bRet = 0;
                    }
                    else
                    {
                        mw.LogState(LogType.Fail, string.Format("Check recv data: {0}", RcvMessage));
                        Capacitance = -1;
                        bRet = -1;
                    }


                    Send("SAFE:STEP1:OSC:CHAN(@(0))", 250);
                    Send("SAFE:STEP1:OSC:CHAN:LOW(@(0))", 250);

                    Send("SOUR:SAFE:STOP", 0);
                    Send("SOUR:SAFE:STOP", 0);

                    return bRet;
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "Chroma_GetCapacitance", ec);
                if (serialPort.IsOpen)
                {
                    Send("SOUR:SAFE:STOP", 100);
                    Send("SOUR:SAFE:STOP", 0);
                }
                Capacitance = -1;
                return -1;
            }

            Capacitance = -1;
            return -1;
        }

        private byte[] RecvChroma19053(bool bIsLogWrite = true, int nTimeOut = 2000)
        {
            /* Modify By SJH 211028 : Chroma19053 모델 Receive 처리
             * 
             * nTimeOut        @in param         : 타임아웃 설정 시간, 단위 ms (sleep 사용으로 비교 연산시 /2, 미세 오차 감안), 기본값 2초
             * btBuffer        @local scope      : 가공될 버퍼
             * btResult        @local scope      : 최종 결과 데이터
             * bIsReadComplete @local scope      : 데이터 버퍼 가공 성공 판단, 정상일시 true
             * nWaitCount      @local scope      : 실제 Recv 대기시간
             * nPos            @local scope      : 리시브 버퍼의 내부 인덱스 위치
             * return                            : 정상 btResult, 비정상 null
             * 
             * 주의 : 동기 로직에만 사용 할 것
             * 참고 : 패치 모니터링 이후 코드 검증이 되었다면 하이팟 통신 관련 해당 함수로 적용 할 것
            */

            if (serialPort.IsOpen == false)
            {
                return null;
            }

            try
            {
                int nPos = 0;

                int nWaitCount = 0;

                byte[] btBuffer = new byte[1024];

                while (true)
                {
                    Thread.Sleep(1);

                    if (++nWaitCount > (nTimeOut/2))
                    {
                        mw.LogState(LogType.Fail, "Recv Time Out !");

                        return null;
                    }

                    int nReadSize = serialPort.BytesToRead;

                    byte[] btRecv = new byte[nReadSize];

                    serialPort.Read(btRecv, 0, nReadSize);

                    Buffer.BlockCopy(btRecv, 0, btBuffer, nPos, nReadSize);
                    
                    nPos += nReadSize;

                    if (nPos > 2) // 읽기 최소 단위는 2byte
                    {
                        if (btBuffer[(nPos-1) -1] == 0x0D /*CR*/ && btBuffer[(nPos-1)] == 0x0A/*LF*/)
                        {
                            // 실제 크기 만큼 가공해서 리턴

                            byte[] btResult = new byte[nPos];

                            Buffer.BlockCopy(btBuffer, 0, btResult, 0, nPos);

                            if(bIsLogWrite == true)
                            {
                                mw.LogState(LogType.Info, "Recv Total Size : " + nPos.ToString());
                            }
                            
                            return btResult;
                        }
                        else
                        {
                            //mw.LogState(LogType.Info, "need to read more");
                        }
                    }
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "Chroma19053 ec:" + ec.ToString());

                return null;
            }
        }

        private int CheckState()
        {
            /* Modify by SJH 211028 : 계측기 상태 체크
             * 
             * 기존 GetResistance, GetVoltage, GetCapacitance 에서 사용되었으며
             * Fetch 기능과 혼용되어 사용되면 안된다 (isUseFetch 플래그로 구분)
             * Recv 처리 보안, Fetch 사용의 따른 기능 분리를 위해 신규 작성
             * 
             * nNullCount   @local scope    : 데이터 유효성 검사
             * return                       : 정상 0, 비정상 -1
             * 
             * 참고 : Send()를 통한 인터벌 설정
             * 참고 : 5회 이상의 비정상 수신버퍼 감지시 비정상 종료
            */

            int nNullCount = 0;

            while (true)
            {
                if (I_EMGSTOP || mw.isStop || mw.ispause)
                {
                    Send("SOUR:SAFE:STOP", 100);

                    Send("SOUR:SAFE:STOP", 0);

                    return -1;
                }

                serialPort.DiscardInBuffer();

                Send("SOUR:SAFE:STATUS?", 300);

                byte[] btBuff = RecvChroma19053();

                if (btBuff == null)
                {
                    ++nNullCount;

                    mw.LogState(LogType.Fail, "Status Data Blank, Count : " + nNullCount.ToString());

                    if (nNullCount > 5)
                    {
                        mw.LogState(LogType.Fail, "Status Data Blank, Count : " + nNullCount.ToString() + "(return -1)");

                        return -1;
                    }

                    continue;
                }

                var strbuff = Encoding.Default.GetString(btBuff, 0, btBuff.Length);

                if (strbuff.Contains("STOPPED"))
                {
                    mw.LogState(LogType.Info, "State Stopped !");

                    return 0;
                }

                if (strbuff.Contains("RUNNING"))
                {
                    // Running, continue..
                }
            }
        }

        private int RunFetch()
        {
            /* Modify By SJH 211028 : 패치 기능 추가
             * 
             * bIsDummy         @local scope    : 더미 데이터 체크, 
             * nNullCount       @local scope    : 수신버퍼 비정상시 카운트 증가
             * dCatchTime       @local scope    : 수집시간 단위(sec), ms 수집 권장 안함
             * dResMaxTime      @local scope    : 응답 최대 시간 단위(sec)
             *                                  : 물리적 환경의 따라 차이 날 수 있다
             *                                  : 0.4sec(400ms) 설정으로 일단 테스트 진행
             * return                           : 정상 0, 비정상 -1 
             * 
             * 참고 : 응답 시간의 따라 인터벌이 계산 되기 때문에 Send()를 통한 패치 명령시 인터벌 0으로 고정할것
             * 참고 : 5회 이상의 비정상 수신버퍼 감지시 비정상 종료
            */

            bool bIsDummy = true;

            int nNullCount = 0;

            double dCatchTime = 0;

            const double dResMaxTime = 0.3; /* 211104 : 모니터링 결과 0.3sec이 최적의 시간, 수집 오차 범위 +-100ms
                                             * 레지스트리 등록 하려 하였지만 고정 사용이 안전할것같다
                                            */

            while (true)
            {
                // 1. 정지 이밴트 발생시 처리

                if (I_EMGSTOP || mw.isStop || mw.ispause)
                {
                    Send("SOUR:SAFE:STOP", 100);

                    Send("SOUR:SAFE:STOP", 0);

                    return -1; // 밑에서 패치 응답을 통해 확인 가능한데 기존 시나리오를 따라 간다
                }

                // 2. Fetch 명령 및 응답 버퍼 확인

                serialPort.DiscardInBuffer();

                Send("SAFE:FETC?MMET, TELA, OMET", 0); // MMELT : 저항값, TELA : 실제 동작 시간, OMET : 출력 전압

                byte[] btBuff = RecvChroma19053(false);

                if (btBuff == null)
                {
                    ++nNullCount;

                    mw.LogState(LogType.Fail, "Fetch Data Blank, Count : " + nNullCount.ToString());

                    if (nNullCount > 5)
                    {
                        mw.LogState(LogType.Fail, "Fetch Data Blank, Count : " + nNullCount.ToString() + "(return -1)");

                        return -1;
                    }

                    Thread.Sleep(300);

                    continue;
                }

                // 3. 응답 버퍼 가공

                string strBuff = Encoding.Default.GetString(btBuff, 0, btBuff.Length - 2); // CR, LF 2Byte 제외

                string[] strArrData = strBuff.Split(';');

                if(strArrData.Count() != 3) // 3개의 파라메터 요청을 했으니 3개의 데이터 응답이 와야한다
                {
                    mw.LogState(LogType.Fail, "Fetch Data Abnormal Parsing, Count : " + strArrData.Count());

                    Thread.Sleep(300);

                    continue;
                }

                // 4. 측정 데이터 취득

                double dMeasure = 0.0;

                double.TryParse(strArrData[0], out dMeasure);

                dMeasure /= 1000000;

                // 5. 실제 동작 시간 취득

                string strProcTime = Convert.ToDouble(strArrData[1]).ToString("f1");

                double dProcTime = 0.0;

                double.TryParse(strProcTime, out dProcTime);

                // 6. 출력 전압 취득

                double dOutVolt = 0.0;

                double.TryParse(strArrData[2], out dOutVolt);

                mw.LogState(LogType.Info, dProcTime.ToString()      + "(realtime), "    +
                                          dMeasure.ToString("f0")   + "(measure), "     + 
                                          dOutVolt.ToString()       + "(volt)");

                // 7. 동작 상태 확인 처리 (계측기가 동작을 하지 않으면 ProcTime은 쓰레기값으로 응답이 온다)

                if (dProcTime > 99999)
                {
                    mw.LogState(LogType.Info, "Not Running !, End Fetch");

                    return 0;
                }

                // 9. 패치 데이터 최종 가공

                if (bIsDummy == false)
                {
                    CFecthData fetchData = new CFecthData();

                    fetchData.m_strMeasure = dMeasure.ToString("f0");

                    fetchData.m_strOutVolt = dOutVolt.ToString();

                    fetchData.m_strProcTime = dProcTime.ToString();

                    // 반올림 처리해야 정확한 key를 얻을수 있다

                    EN_FETCH_KEY key = GetFetchKey((int)Math.Round(dProcTime, MidpointRounding.AwayFromZero));

                    PushFetchData(key, fetchData);
                }

                if (bIsDummy == true)
                {
                    bIsDummy = false;

                    mw.LogState(LogType.Info, "Dummy Data !");
                }

                // 10. 측정 동기를 위한 인터벌 설정

                dCatchTime += 1.0; // 1초단위로 수집

                double dWaitTime = dCatchTime - dProcTime - dResMaxTime;

                if(dWaitTime < 0) // 계산된 값이 음수가 나오면 안된다
                {
                    dWaitTime = 0.3;
                }

                Thread.Sleep(Convert.ToInt32(dWaitTime * 1000));
            }
        }

        private EN_FETCH_KEY GetFetchKey(int nProcTime)
        {
            // Modify By SJH 211028 : 동작 시간의 해당하는 Key 리턴
            
            EN_FETCH_KEY enKey = EN_FETCH_KEY.SEC_60;
            
            if (0 < nProcTime && nProcTime <= 10)
                enKey = EN_FETCH_KEY.SEC_10;

            if (10 < nProcTime && nProcTime <= 20)
                enKey = EN_FETCH_KEY.SEC_20;

            if (20 < nProcTime && nProcTime <= 30)
                enKey = EN_FETCH_KEY.SEC_30;

            if (30 < nProcTime && nProcTime <= 40)
                enKey = EN_FETCH_KEY.SEC_40;

            if (40 < nProcTime && nProcTime <= 50)
                enKey = EN_FETCH_KEY.SEC_50;

            if (50 < nProcTime && nProcTime <= 60)
                enKey = EN_FETCH_KEY.SEC_60;

            return enKey;
        }

        private void InitFetchData()
        {
            // Modify By SJH 211028 : Fectch 데이터 초기화 및 할당
            
            foreach (var list in m_mapFetchData)
            {
                list.Value.Clear();
            }
            
            m_mapFetchData.Clear();

            foreach (var key in Enum.GetValues(typeof(EN_FETCH_KEY)))
            {
                List<CFecthData> listFetch = new List<CFecthData>();

                m_mapFetchData.Add((EN_FETCH_KEY)key, listFetch);
            }
        }

        private void PushFetchData(EN_FETCH_KEY enKey, CFecthData fetchData)
        {
            // Modify By SJH 211028 : Fectch 데이터 삽입
            
            if (m_mapFetchData.ContainsKey(enKey) == true)
            {
                List<CFecthData> listFetch;

                m_mapFetchData.TryGetValue(enKey, out listFetch);

                listFetch.Add(fetchData);
            }
            else
            {
                mw.LogState(LogType.Fail, "Check Fetch Key Value");
            }
        }

        private CFecthData GetLastFecthData(EN_FETCH_KEY enKey)
        {
            // Modify By SJH 211028 : 리스트의 마지막 패치 데이터 반환
    
            List<CFecthData> listFecth = null;

            m_mapFetchData.TryGetValue(enKey, out listFecth);

            if(listFecth != null)
            {
                if (listFecth.Count != 0)
                {
                    return listFecth.Last();
                }
            }

            return null;
        }

        private List<CFecthData> GetListFecthData(EN_FETCH_KEY enKey)
        {
            // Modify By SJH 211028 : 패치 리스트 반환

            List<CFecthData> listFecth = null;

            m_mapFetchData.TryGetValue(enKey, out listFecth);

            if(listFecth != null)
            {
                return listFecth;
            }

            return null;
        }

        private int GetFecthDataCollectCount()
        {
            // Modify By SJH 211028 : 수집된 개수(단위 : 초) 반환

            int nCount = 0;

            foreach (var list in m_mapFetchData)
            {
                nCount += list.Value.Count();
            }

            return nCount;
        }

        public void LastFetchDataProc(int nIsNormal, /*double*/object objJudgeValue)
        {
            // Modify By SJH 211104 : 측정된 Judge 결과 값 패치 마지막 데이터로 적용

            if(isUseFetch == false)
            {
                mw.LogState(LogType.Info, "LastFetchDataProc() Fetch Flag (flase), Skip");

                return;
            }

            try
            {
                if(nIsNormal == 0) // JudgeValue가 정상인 경우
                {
                    double dJudgeValue = 0.0;

                    // 데이터 타입이 double인지 한번더 확인하기 위함 (Error발생시 string이 적용 되기 때문)

                    if (double.TryParse(objJudgeValue.ToString(), out dJudgeValue) == true)
                    {
                        int nCollectTime = GetFecthDataCollectCount();

                        int nRunTime = Convert.ToInt32(CON_INSULATION_RESISTANCE_T);

                        EN_FETCH_KEY key = GetFetchKey(nRunTime);

                        if (nCollectTime == nRunTime) 
                        {
                            // 기대 수집과 실제 수집 개수가 동일하면 데이터만 변환 하여 적용

                            CFecthData lastData = GetLastFecthData(key);

                            string strLog = string.Empty;

                            strLog = "LastFetchDataProc() Before Data" +
                                     " ,realtime:"                     + lastData.m_strProcTime   +
                                     " ,measure:"                      + lastData.m_strMeasure    +
                                     " ,outvolt:"                      + lastData.m_strOutVolt;

                            mw.LogState(LogType.Info, strLog);

                            if (lastData != null)
                            {
                                lastData.m_strMeasure = dJudgeValue.ToString(); // Judge 값

                                lastData.m_strProcTime = CON_INSULATION_RESISTANCE_T.ToString(); // 제어 스팩 시간

                                lastData.m_strOutVolt = CON_INSULATION_RESISTANCE_V.ToString(); // 제어 스팩 전압

                                strLog = string.Empty;

                                strLog = "LastFetchDataProc() After Data" +
                                         ", realtime:"                    + lastData.m_strProcTime   +
                                         ", measure:"                     + lastData.m_strMeasure    +
                                         ", outvolt:"                     + lastData.m_strOutVolt;

                                mw.LogState(LogType.Info, strLog);
                            }

                        }
                        else if (nCollectTime == nRunTime -1)
                        {
                            // 기대 수집과 실제 수집 개수가 -1 만큼 차이가 나면 신규 데이터 추가 적용

                            CFecthData newLastData = new CFecthData();

                            newLastData.m_strMeasure = dJudgeValue.ToString(); // Judge 값

                            newLastData.m_strProcTime = CON_INSULATION_RESISTANCE_T.ToString(); // 제어 스팩 시간

                            newLastData.m_strOutVolt = CON_INSULATION_RESISTANCE_V.ToString(); // 제어 스팩 전압

                            PushFetchData(key, newLastData);

                            string strLog = string.Empty;

                            strLog = "LastFetchDataProc() New Last Data" +
                                     ", realtime:"                       + newLastData.m_strProcTime +
                                     ", measure:"                        + newLastData.m_strMeasure +
                                     ", outvolt:"                        + newLastData.m_strOutVolt;

                            mw.LogState(LogType.Info, strLog);
                        }
                        else
                        {
                            // 해당 케이스 모니터링 필요, Blank 가 1개 이상인 경우임

                            mw.LogState(LogType.Info, "LastFetchDataProc() Need Debuging, Blank Count Is 1 or More");
                        }
                    }
                    else
                    {
                        mw.LogState(LogType.Fail, "LastFetchDataProc() Abnormal Judge Value, Skip");
                    }
                }
                else
                {
                    mw.LogState(LogType.Info, "LastFetchDataProc() Abnormal Judge Value Type, Skip");

                    return;
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "LastFetchDataProc() Exception : ", ec);
            }
        }
    }
}

