using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;


namespace EOL_BASE.모듈
{
    public class CDAQ970
    {
        MainWindow mw;
        public bool isAlive = false;
        System.Windows.Forms.Timer reConnectTimer;

        NetworkStream netStream;
        TcpClient tcpClient;

        Ping pingSender = new Ping();
        PingOptions options = new PingOptions();

        string rtString = "";
        string unrecvStrVal = "";
        string header = "\u0001";
        string header2 = "\u0003";
        string connectMsg = "Welcome";

        Thread th;

        /// <summary>
        /// TCP
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="ipa">기본값 164.254.9.70</param>
        /// <param name="port">기본값 5024</param>
        public CDAQ970(MainWindow mw, string daqIP, string port)
        {
            this.mw = mw;
            try
            {
                // 211213 JKD - DMM970A 통신 Wakeup 
                int cnt = 0;
                while (true)
                {
                    PingReply reply = pingSender.Send(daqIP);

                    if (cnt > 500) // 500ms
                    {
                        break;
                    }

                    if (reply.Status == IPStatus.Success)
                    {
                        break;
                    }

                    cnt += 100;
                    Thread.Sleep(100);
                }

                tcpClient = new TcpClient(daqIP, int.Parse(port));
                tcpClient.ReceiveBufferSize = 1024;
                netStream = tcpClient.GetStream();
                netStream.ReadTimeout = System.Threading.Timeout.Infinite;

                if (tcpClient.Connected)
                {
                    reConnectTimer = new System.Windows.Forms.Timer();
                    reConnectTimer.Interval = 8000;
                    reConnectTimer.Tick += reConnectTimer_Tick;
                    reConnectTimer.Start();
                    isAlive = true;

                    th = new Thread(() =>
                    {
                        while (true)
                        {
                            try
                            {

                                byte[] recvbytes = new byte[(int)tcpClient.ReceiveBufferSize];

                                //처음읽을때는 버퍼 크게해서 읽어보고, 
                                int rev = netStream.Read(recvbytes, 0, (int)tcpClient.ReceiveBufferSize);
                                if (rev > 0)
                                {
                                    var strVal = Encoding.UTF8.GetString(recvbytes);
                                    strVal = unrecvStrVal + strVal; 
                                    string[] strRcv = strVal.Split(new char[] { '\r' });

                                    if (strVal.Contains(header) || 
                                       strVal.Contains(header2) || 
                                       strVal.Contains(connectMsg)) //Start of Heading. DMM970A Connect msg
                                    {
                                        continue;
                                    }

                                    //ETX msg
                                    if (strVal.Contains("DAQ970"))
                                    {
                                        if (strRcv.Length >= 2)
                                        {
                                            int Index;

                                            for (int i = 1; i < strRcv.Length; i++)
                                            {
                                                if (strRcv[i].Contains("DAQ970"))
                                                {
                                                    unrecvStrVal = "";
                                                    Index = i;
                                                    rtString = strRcv[i - 1].Replace("\n", "");
                                                    //return true;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //211210 JKD - 데이터 잘린 경우
                                        if (!strVal.Contains("?")) // 211210 JKD - 송신한 CMD 제외한 Data
                                        {
                                            unrecvStrVal = strVal;
                                            mw.LogState(LogType.Info, string.Format("DAQ970 : {0}", strVal));
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    });
                    th.Start();


                    Type_Check(daqIP, port);
                }
                else
                {
                    Type_Check(daqIP, port);
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "KeysightDAQ970 - ", ec);
                try
                {
                    tcpClient = new TcpClient(daqIP, int.Parse(port));
                    tcpClient.ReceiveBufferSize = 1024;
                    netStream = tcpClient.GetStream();
                    netStream.ReadTimeout = 15000;

                    if (tcpClient.Connected)
                    {
                        reConnectTimer = new System.Windows.Forms.Timer();
                        reConnectTimer.Interval = 8000;
                        reConnectTimer.Tick += reConnectTimer_Tick;
                        reConnectTimer.Start();
                        isAlive = true;

                        Type_Check(daqIP, port);
                    }
                    else
                    {
                        Type_Check(daqIP, port);
                    }
                }
                catch (Exception ecc)
                {
                    Type_Check(daqIP, port);
                    mw.LogState(LogType.Fail, "KeysightDAQ970 - ", ecc);
                }
            }
        }

        public void Type_Check(string daqIP, string port)
        {
            switch (daqIP)
            {
                case "169.254.9.70":
                    {
                        string tempstr = IDN();
                        if (tempstr == "IDN_Fail")
                        {
                            mw.LogState(LogType.Info, string.Format("KeysightDAQ970 - {0}, {1} Fail",daqIP, port));
                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_daq970_1.Background = System.Windows.Media.Brushes.Red;
                            }));
                        }
                        else
                        {
                            mw.LogState(LogType.Info, string.Format("KeysightDAQ970 - {0}, {1} Open Success",daqIP, port));
                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_daq970_1.Background = System.Windows.Media.Brushes.Green;
                            }));
                        }
                    }
                    break;
                case "169.254.9.71":
                    {
                        string tempstr = IDN();
                        if (tempstr == "IDN_Fail")
                        {
                            mw.LogState(LogType.Info, string.Format("KeysightDAQ970 - {0}, {1} Fail",daqIP, port));
                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_daq970_2.Background = System.Windows.Media.Brushes.Red;
                            }));
                        }
                        else
                        {
                            mw.LogState(LogType.Info, string.Format("KeysightDAQ970 - {0}, {1} Open Success",daqIP, port));
                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_daq970_2.Background = System.Windows.Media.Brushes.Green;
                            }));
                        }
                    }
                    break;
                case "169.254.9.72":
                    {
                        string tempstr = IDN();
                        if (tempstr == "IDN_Fail")
                        {
                            mw.LogState(LogType.Info, string.Format("KeysightDAQ970 - {0}, {1} Fail",daqIP, port));
                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_daq970_3.Background = System.Windows.Media.Brushes.Red;
                            }));
                        }
                        else
                        {
                            mw.LogState(LogType.Info, string.Format("KeysightDAQ970 - {0}, {1} Open Success",daqIP, port));
                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_daq970_3.Background = System.Windows.Media.Brushes.Green;
                            }));
                        }
                    }
                    break;
            }
        }

        public void _Dispose()
        {
            if (tcpClient != null && tcpClient.Connected)
                tcpClient.Close();
        }

        void reConnectTimer_Tick(object sender, EventArgs e)
        {
            if (!tcpClient.Connected)
            {
                tcpClient.Close();
                netStream = tcpClient.GetStream();
            }
        }
        public bool TrySend(string sendmsg, out string val)
        {
            string strVal = "";
            try
            {
                if (!tcpClient.Connected)

                {
                    mw.LogState(LogType.Info, "Check KeysightDAQ970 Connection");
                    val = strVal;
                    return false;
                }
                else
                {
                    rtString = "";
                    var body = System.Text.Encoding.ASCII.GetBytes(sendmsg);
                    byte[] bt = new byte[body.Length + 2];

                    Array.Copy(body, 0, bt, 0, body.Length);

                    bt[bt.Length - 2] = 0x0D; // CR \r
                    bt[bt.Length - 1] = 0x0A; // LF \n 

                    // Write
                    netStream.Write(bt, 0, bt.Length);
                    mw.LogState(LogType.REQUEST, "KeysightDAQ970 Send - " + sendmsg.Replace("\n", ""));

                    int cnt = 0;
                    while (rtString == "")
                    {
                        if (mw.isStop || mw.ispause)
                        {
                            mw.LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");
                            val = "EMS";
                            return false;
                        }
                        Thread.Sleep(1);
                        cnt++;
                        if (cnt > 20000) // 20sec
                        {
                            mw.LogState(LogType.Fail, "DATA RECEIVE FAIL!!");
                            val = "EMS";
                            return false;
                        }
                    }

                    val = rtString;
                    return true; 
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "KeysightDAQ970 ", ec);
                val = strVal;
                return false;
            }
        }

        Thread rcvTrd = null;

        string ReceiveData = string.Empty;

        public string IDN()
        {
            string recvMsg = "";

            try
            {
                string str = "*IDN?";

                if (TrySend(str, out recvMsg))
                {
                    mw.LogState(LogType.RESPONSE, "Receive Data : " + recvMsg);
                }
                else
                {
                    recvMsg = "IDN_Fail";
                    mw.LogState(LogType.RESPONSE, "Receive Error " + recvMsg);
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "", ex);
            }

            return recvMsg;
        }

        public bool MeasVolt(out double data, string ch)
        {
            string recvMsg = "";

            try
            {
                var sendstr = "MEAS:VOLT:DC? (@" + ch.ToString() + ")";

                if (TrySend(sendstr, out recvMsg))
                {
                    if (double.TryParse(recvMsg, out data))
                        return true;
                    else
                        return false;
                }
                else
                {
                    mw.LogState(LogType.Fail, "Receive Error " + recvMsg);
                    data = 0.0;
                    return false;
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "", ex);
                data = 0.0;
                return false;
            }
        }

        public bool MeasVolt_Multi(out List<double> list, string channels, int channelCnt)
        {
            string recvMsg = "";

            try
            {
                //기존 사용하던 명령어 220926
                //var sendstr = "MEAS:VOLT:DC? (@" + channels.ToString() + ")";
                //20220926 nnkim 임피던스 오토로 사용하는 함수
                var sendstr = SetAutoImpedance(channels);

                //데이터 SHOW
                if (TrySend(sendstr, out recvMsg))
                {
                    var arrVal = recvMsg.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    List<double> voltList = new List<double>();
                    foreach (string value in arrVal)
                    {
                        voltList.Add(double.Parse(value));
                    }

                    if (voltList.Count == channelCnt)
                    {
                        list = voltList;
                        return true;
                    }
                    else
                    {
                        list = voltList;
                        return false;
                    }
                }
                else
                {
                    mw.LogState(LogType.Fail, "Receive Error " + recvMsg);
                    list = new List<double>();
                    return false;
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "", ex);
                list = new List<double>();
                return false;
            }
        }

        public bool MeasAuto_Impedance(out List<double> list, string channels, int channelCnt)
        {
            string recvMsg = "";

            try
            {
                var sendstr = "MEAS:VOLT:DC:APERture:ENABle? (@" + channels.ToString() + ")";

                if (TrySend(sendstr, out recvMsg))
                {
                    var arrVal = recvMsg.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    List<double> voltList = new List<double>();
                    foreach (string value in arrVal)
                    {
                        voltList.Add(double.Parse(value));
                    }

                    if (voltList.Count == channelCnt)
                    {
                        list = voltList;
                        return true;
                    }
                    else
                    {
                        list = voltList;
                        return false;
                    }
                }
                else
                {
                    mw.LogState(LogType.Fail, "Receive Error " + recvMsg);
                    list = new List<double>();
                    return false;
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "", ex);
                list = new List<double>();
                return false;
            }
        }

        public bool MeasRes(out double data, string ch)
        {
            string recvMsg = "";

            try
            {
                var sendstr = "MEAS:RES? (@" + ch.ToString() + ")";

                if (TrySend(sendstr, out recvMsg))
                {
                    if (double.TryParse(recvMsg, out data))
                        return true;
                    else
                        return false;
                }
                else
                {
                    mw.LogState(LogType.Fail, "Receive Error " + recvMsg);
                    data = 0.0;
                    return false;
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "", ex);
                data = 0.0;
                return false;
            }
        }

        public bool MeasTemp(out double data, string ch)
        {
            string recvMsg = "";

            try
            {
                var sendstr = "MEAS:RES? (@" + ch.ToString() + ")";

                if (TrySend(sendstr, out recvMsg))
                {
                    if (double.TryParse(recvMsg, out data))
                        return true;
                    else
                        return false;
                }
                else
                {
                    mw.LogState(LogType.Fail, "Receive Error " + recvMsg);
                    data = 0.0;
                    return false;
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "", ex);
                data = 0.0;
                return false;
            }
        }

        public bool MeasRes_Multi(out List<double> list, string channels, int channelCnt)
        {
            string recvMsg = "";

            try
            {
                var sendstr = "MEAS:RES? " + "(@" + channels.ToString() + ")";

                if (TrySend(sendstr, out recvMsg))
                {
                    var arrVal = recvMsg.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    List<double> resList = new List<double>();
                    foreach (string value in arrVal)
                    {
                        resList.Add(double.Parse(value));
                    }

                    if (resList.Count == channelCnt)
                    {
                        list = resList;
                        return true;
                    }
                    else
                    {
                        list = resList;
                        return false;
                    }
                }
                else
                {
                    mw.LogState(LogType.Fail, "Receive Error " + recvMsg);
                    list = new List<double>();
                    return false;
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "", ex);
                list = new List<double>();
                return false;
            }
        }

        public bool Open(string sendmsg, string channels)
        {
            try
            {
                var sendstr = "ROUT:OPEN (@" + channels.ToString() + ")";

                if (!tcpClient.Connected)
                {
                    mw.LogState(LogType.Info, "Check KeysightDAQ970 Connection");
                    return false;
                }
                else
                {
                    var body = System.Text.Encoding.ASCII.GetBytes(sendstr);
                    byte[] bt = new byte[body.Length + 2];

                    Array.Copy(body, 0, bt, 0, body.Length);

                    bt[bt.Length - 2] = 0x0D; // CR \r
                    bt[bt.Length - 1] = 0x0A; // LF \n

                    // Write
                    netStream.Write(bt, 0, bt.Length);

                    return true;
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "", ex);
                return false;
            }
        }

        public bool CONF_VOLT_DC_SET(string channels, int _1st = 10, string _2nd = "DEF")
        {
            try
            {
                rtString = "";
                var sendstr = string.Format("CONF:VOLT:DC {0},{1},(@{2}), ",_1st, _2nd, channels);

                if (!tcpClient.Connected)
                {
                    mw.LogState(LogType.Info, "Check KeysightDAQ970 Connection");
                    return false;
                }
                else
                {
                    var body = System.Text.Encoding.ASCII.GetBytes(sendstr);
                    byte[] bt = new byte[body.Length + 2];

                    Array.Copy(body, 0, bt, 0, body.Length);

                    bt[bt.Length - 2] = 0x0D; // CR \r
                    bt[bt.Length - 1] = 0x0A; // LF \n

                    // Write
                    netStream.Write(bt, 0, bt.Length);

                    mw.LogState(LogType.Info, string.Format("SEND MSG:{0}", sendstr));
                    return true;
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "", ex);
                return false;
            }
        }

        public bool ROUT_CLOS(string channels)
        {
            try
            {
                rtString = "";
                var sendstr = "ROUT:CLOS (@" + channels.ToString() + ")";

                if (!tcpClient.Connected)
                {
                    mw.LogState(LogType.Info, "Check KeysightDAQ970 Connection");
                    return false;
                }
                else
                {
                    var body = System.Text.Encoding.ASCII.GetBytes(sendstr);
                    byte[] bt = new byte[body.Length + 2];

                    Array.Copy(body, 0, bt, 0, body.Length);

                    bt[bt.Length - 2] = 0x0D; // CR \r
                    bt[bt.Length - 1] = 0x0A; // LF \n

                    // Write
                    netStream.Write(bt, 0, bt.Length);

                    mw.LogState(LogType.Info, string.Format("SEND MSG:{0}", sendstr));
                    return true;
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "", ex);
                return false;
            }
        }

        public bool RST()
        {
            try
            {
                rtString = "";
                var sendstr = "*RST";

                if (!tcpClient.Connected)
                {
                    mw.LogState(LogType.Info, "Check KeysightDAQ970 Connection");
                    return false;
                }
                else
                {
                    var body = System.Text.Encoding.ASCII.GetBytes(sendstr);
                    byte[] bt = new byte[body.Length + 2];

                    Array.Copy(body, 0, bt, 0, body.Length);

                    bt[bt.Length - 2] = 0x0D; // CR \r
                    bt[bt.Length - 1] = 0x0A; // LF \n

                    // Write
                    netStream.Write(bt, 0, bt.Length);

                    mw.LogState(LogType.Info, string.Format("SEND MSG:{0}",sendstr));

                    return true;
                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "", ex);
                return false;
            }
        }
        
        public bool READ(out string val)
        {
            string strVal = "";
            try
            {
                if (!tcpClient.Connected)
                {
                    mw.LogState(LogType.Info, "Check KeysightDAQ970 Connection");
                    val = strVal;
                    return false;
                }
                else
                {
                    rtString = "";
                    var body = System.Text.Encoding.ASCII.GetBytes("READ?");
                    byte[] bt = new byte[body.Length + 2];

                    Array.Copy(body, 0, bt, 0, body.Length);

                    bt[bt.Length - 2] = 0x0D; // CR \r
                    bt[bt.Length - 1] = 0x0A; // LF \n 

                    // Write
                    netStream.Write(bt, 0, bt.Length);
                    mw.LogState(LogType.Info, string.Format("SEND MSG:READ?"));

                    while (rtString == "")
                    {
                        if (mw.isStop || mw.ispause)
                        {
                            mw.LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");
                            val = "EMS";
                            return false;
                        }
                        Thread.Sleep(1);
                    }
                    val = rtString;
                    return true;
                }
            }
            catch (Exception)
            {
                val = strVal;
                return false;

            }
        }

        //임피던스 오토로 사용 하는 함수 impedance
        public string SetAutoImpedance(string strChannels)
        {
            //임피던스 설정
            var sendstr = "CONF:VOLT:DC 1,1,(@" + strChannels.ToString() + ");:"
            + "VOLT:DC:RANG:AUTO 1 , (@" + strChannels.ToString() + ");:"
            + "VOLT:DC:ZERO:AUTO 1 , (@" + strChannels.ToString() + ");:"
            + "VOLT:DC:NPLC 1,  (@" + strChannels.ToString() + ");:"
            + "VOLT:IMP:AUTO ON, (@" + strChannels.ToString() + ");:"
            + "ROUT:SCAN  (@" + strChannels.ToString() + ");:"
            + "INIT;:"
            + "FETC?";

            return sendstr;

            ////20220916
            ////DAQ 설정 값 해주는 부분
            //var sendstr = "CONF:VOLT:DC 1,1,(@" + channels.ToString() + ");:"
            //    + "VOLT:DC:RANG:AUTO 1 , (@" + channels.ToString() + ");:"
            //    + "VOLT:DC:ZERO:AUTO 1 , (@" + channels.ToString() + ");:"
            //    + "VOLT:DC:NPLC 1,  (@" + channels.ToString() + ");:"
            //    + "VOLT:IMP:AUTO ON, (@" + channels.ToString() + ")";
            //TrySend(sendstr, out recvMsg);

            ////데이터 스캔 해주고
            //var sendstr1 = "ROUT:SCAN (@" + channels.ToString() + ")";
            //TrySend(sendstr1, out recvMsg);

            ////몰라
            //var sendstr3= "INIT";
            //TrySend(sendstr3, out recvMsg);

            ////읽어오는 부분
            //var sendstr4 = "FETC?";
        }
    }
}