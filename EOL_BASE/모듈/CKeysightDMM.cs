using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE.모듈
{
    public class CKeysightDMM
    {
        MainWindow mw;
        public bool isAlive = false;
        public string addr = "";
        public Socket socket;
        System.Windows.Forms.Timer reConnectTimer;
        IPEndPoint serverEndPoint;

        string rtString = "";
        string unrecvStrVal = "";
        string header = "\u0001";
        string header2 = "\u0003";
        string connectMsg = "Welcome";
        NetworkStream netStream;
        /// <summary>
        /// TCP
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="ipa">기본값 164.254.4.61</param>
        /// <param name="port">기본값 5025</param>
        public CKeysightDMM(MainWindow mw, string dmmIP, int port)
        {
            this.mw = mw;
            serverEndPoint = new IPEndPoint(IPAddress.Parse(dmmIP), port);
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IAsyncResult result = socket.BeginConnect(dmmIP, port, null, null);

                bool success = result.AsyncWaitHandle.WaitOne(2000, true);

                if (socket.Connected)
                {
                    socket.Connect(serverEndPoint);//TCP   
                    socket.ReceiveBufferSize = 1024;

                    reConnectTimer = new System.Windows.Forms.Timer();
                    reConnectTimer.Interval = 8000;
                    reConnectTimer.Tick += reConnectTimer_Tick;
                    reConnectTimer.Start();
                    isAlive = true;
                    var Volt = TrySend("MEAS:VOLT:DC?\n");
                    mw.LogState(LogType.Info, "KeysightDMM Connected");
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                    }));
                }
                else
                {
                    // NOTE, MUST CLOSE THE SOCKET
                    socket.Close();

                    mw.LogState(LogType.Fail, "KeysightDMM - Failed to connect server.");
                    //35. DMM 연결 실패 시 연결상태 체크 UI 변경 구문 추가
                    //190106 by grchoi
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                    }));
                }

            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "KeysightDMM - ", ec);
            }

        }
        
        public void _Dispose()
        {
            if (socket != null && socket.Connected)
                socket.Disconnect(false);

            if (sp != null)
                sp.Dispose();
        }

        void reConnectTimer_Tick(object sender, EventArgs e)
        {
            if (!socket.Connected)
            {
                socket.Disconnect(true);
                socket.Connect(serverEndPoint);
            }
        }

        public float TrySend(string str)
        {
            float rtf = 0.0f;
            try
            {
                if (socket.Connected)
                {
                    byte[] bt = new byte[1024];

                    socket.Send(Encoding.UTF8.GetBytes(str));
                    mw.LogState(LogType.Info, "KeysightDMM Send - " + str.Replace("\n", ""));
                    Array.Clear(bt, 0, 1024);
                    int rec = socket.Receive(bt, SocketFlags.None);

                    if (rec > 0)
                    {
                        string text = Encoding.Default.GetString(bt, 0, rec);
                        mw.LogState(LogType.Info, "KeysightDMM Recv - " + text.Replace("\n", ""));
                        float.TryParse(text, out rtf);
                    }
                }

            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "KeySightDMM - ", ec);
            }
            return rtf;

        }

        #region SerialPort Comm, 34970 대응

        public string IDN()
        {
            if (!sp.IsOpen)
                return "";

            Discard();
            var sendstr = "*IDN?";
            int idnLengthMax = 33;
            sp.WriteLine(sendstr);
            mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);
            //Thread.Sleep(1000); //jgh 택타임 축소 요청 이지현사원
            Thread.Sleep(500);
            int rec = this.sp.BytesToRead;

            int cnt = 0;
            while (rec < idnLengthMax)
            {
                Thread.Sleep(100);
                rec = this.sp.BytesToRead;
                cnt += 100;
                if (cnt == 2000)
                {
                    mw.LogState(LogType.Fail, "KeysightDMM Read IDN Time out");
                    return "IDN_Fail";
                }
            }
            byte[] bt = new byte[rec];
            this.sp.Read(bt, 0, rec);

            this.rtstring = Encoding.Default.GetString(bt, 0, rec);
            mw.LogState(LogType.Info, this.rtstring.ToString());

            return rtstring;
        }

        public SerialPort sp;

        /// <summary>
        /// serialport 타입, 34970 대응
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="isSerial">단순 구분용 파라미터</param>
        /// <param name="portname">포트 이름</param>
        public CKeysightDMM(MainWindow mw, bool isSerial, string portname)
        {
            this.mw = mw;
            sp = new System.IO.Ports.SerialPort();
            sp.DataBits = 8;
            sp.BaudRate = 9600;
            sp.PortName = portname;
            sp.Parity = System.IO.Ports.Parity.None;
            sp.StopBits = System.IO.Ports.StopBits.One;

            try
            {
                sp.Open();
                if (sp.IsOpen)
                {
                    isAlive = true;
                    sp.DiscardOutBuffer();
                    sp.DiscardInBuffer();
                    Thread.Sleep(1000);

                    //sp.DataReceived -= CKeysightDMM_DataReceived;  
                    //sp.DataReceived += CKeysightDMM_DataReceived;             


                    var sendstr = "*IDN?";
                    sp.WriteLine(sendstr);
                    mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

                    int rec = sp.BytesToRead;

                    int cnt = 0;
                    while (rec < 10)//33
                    {
                        Thread.Sleep(100);
                        rec = sp.BytesToRead;
                        cnt += 100;
                        //not received data
                        if (cnt == 2000)
                        {
                            mw.LogState(LogType.Fail, sp.PortName + ", KeysightDMM : " + portname.ToString());
                            return;
                        }
                    }

                    mw.LogState(LogType.Success, sp.PortName + ", KeysightDMM : " + portname.ToString());
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                    }));
                }
                else
                    mw.LogState(LogType.Fail, "KeysightDMM" + sp.PortName + " Open");
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "KeysightDMM", ec);
            }
        }

        public string rtstring = "";
        void CKeysightDMM_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);

            int rec = sp.BytesToRead;
            while (rec < 30)
            {
                Thread.Sleep(100);
                rec = sp.BytesToRead;
            }
            //받은 후에 데이터 파싱
            byte[] bt = new byte[rec];
            sp.Read(bt, 0, rec);

            rtstring = Encoding.Default.GetString(bt, 0, rec);
            mw.LogState(LogType.RESPONSE, "KeysightDMM:" + rtstring);
        }

        public string Open(string ch)
        {
            sp.DiscardInBuffer();
            sp.DiscardOutBuffer();
            sp.WriteLine("ROUT:OPEN (@" + ch.ToString() + ")");
            Thread.Sleep(100);
            return rtstring;
        }

        public string Close(string ch)
        {
            sp.DiscardInBuffer();
            sp.DiscardOutBuffer();
            sp.WriteLine("ROUT:OPEN (@" + ch.ToString() + ")");
            Thread.Sleep(100);
            return rtstring;
        }

        //20180717 wjs add version cmd
        public string Read_Version()
        {
            string cmd = "";
            sp.DiscardInBuffer();
            cmd = "*IDN?";
            mw.LogState(LogType.Info, "send data : " + cmd);
            sp.WriteLine(cmd);
            Thread.Sleep(300);
            int comTimeout = 0;
            while (sp.BytesToRead < 33)
            {
                if (comTimeout++ > 10)
                {
                    //throw new Exception("Keysight com check Communication Time out");
                    mw.LogState(LogType.Fail, "Keysight Read Timeout");
                    sp.DiscardOutBuffer();
                    return "";
                }
                Thread.Sleep(100);
            }
            rtstring = sp.ReadLine();
            mw.LogState(LogType.Info, "read data : " + rtstring);
            var p = rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            sp.DiscardOutBuffer();
            return rtstring;
        }

        /// <summary>
        /// 물리적 채널을 입력해서 전압을 읽어오는 메서드
        /// 채널 개수가 늘어나면 내부 슬립 1초로 부족할 수 있다.
        /// </summary>
        /// <param name="ch">101,102,103,104,109,110,111</param>
        /// <returns></returns>
        public string MeasVolt(string ch)
        {
            try
            {
                Discard();
                var sendstr = "MEAS:VOLT:DC? (@" + ch.ToString() + ")";
                sp.WriteLine(sendstr);
                mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);
                return rtstring;
            }
            catch (Exception ex)
            {
                return rtstring;
            }

        }

        /// <summary>
        /// 물리적 채널을 입력해서 저항을 읽어오는 메서드
        /// 채널 개수가 늘어나면 내부 슬립 1초로 부족할 수 있다.
        /// </summary>
        /// <param name="ch">101,102,103,104,109,110,111</param>
        /// <returns></returns>
        public string MeasRes(string ch)
        {
            Discard();
            var sendstr = "MEAS:RES? (@" + ch.ToString() + ")";
            sp.WriteLine(sendstr);
            mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);
            Thread.Sleep(100); //jgh 택타임 단축요청 이지현사원
            return rtstring;
        }

        // 2018.11.09 jeonhj
        // MeasRes와 동일한 동작을 하는 메서드
        // 여러 프로젝트에 갈라져 있어 삭제는 어려우니 일단 추가 먼저
        public string MeasTemp(string ch)
        {
            Discard();
            var sendstr = "MEAS:RES? (@" + ch.ToString() + ")";
            sp.WriteLine(sendstr);
            mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);
            return rtstring;
        }

        public void Discard()
        {
            rtstring = "";
            if (sp.IsOpen)
            {
                sp.DiscardInBuffer();
                sp.DiscardOutBuffer();
                Thread.Sleep(10);
            }
        }

        public void Dispose()
        {
            sp.Dispose();
        }


        #endregion


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

        public bool TrySend(string sendmsg, out string val)
        {
            string strVal = "";
            try
            {
                if (!socket.Connected)
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

    }
}
