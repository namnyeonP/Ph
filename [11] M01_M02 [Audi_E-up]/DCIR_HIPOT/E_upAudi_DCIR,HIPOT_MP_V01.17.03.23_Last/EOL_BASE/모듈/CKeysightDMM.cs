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

        /// <summary>
        /// TCP
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="ipa">기본값 164.254.4.61</param>
        /// <param name="port">기본값 5025</param>
        public CKeysightDMM(MainWindow mw, string dmmIP, string port)
        {
            this.mw = mw;
            serverEndPoint = new IPEndPoint(IPAddress.Parse(dmmIP), int.Parse(port));
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(serverEndPoint);//TCP   
                socket.ReceiveBufferSize = 1024;

                if (socket.Connected)
                {
                    reConnectTimer = new System.Windows.Forms.Timer();
                    reConnectTimer.Interval = 8000;
                    reConnectTimer.Tick += reConnectTimer_Tick;
                    reConnectTimer.Start();
                    isAlive = true;
                    var Volt = TrySend("MEAS:VOLT:DC?\n");
                    if (Volt != 0.0)
                    {
                        mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Open Success");
                        mw.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mw.contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                        }));
                    }
                    else
                    {
                        mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Fail");
                        mw.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mw.contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                        }));
                    }
                }
                else
                {
                    mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Fail");
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                    }));
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Fail, "KeysightDMM - ", ec);
                try
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    socket.Connect(serverEndPoint);//TCP   
                    socket.ReceiveBufferSize = 1024;

                    if (socket.Connected)
                    {
                        reConnectTimer = new System.Windows.Forms.Timer();
                        reConnectTimer.Interval = 8000;
                        reConnectTimer.Tick += reConnectTimer_Tick;
                        reConnectTimer.Start();
                        isAlive = true;
                        var Volt = TrySend("MEAS:VOLT:DC?\n");
                        if (Volt != 0.0)
                        {
                            mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Open Success");
                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                            }));
                        }
                        else
                        {
                            mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Fail");
                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                            }));
                        }
                    }
                    else
                    {
                        mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Fail");
                        mw.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mw.contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                        }));
                    }
                }
                catch (Exception ecc)
                { 
                    mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Exception",ecc);
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                    }));
                }
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
            Thread.Sleep(1000);
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
            sp.BaudRate = 19200;
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

        public bool MeasVolt(out double data, string ch)
        {
            if (!isAlive)
            {
                data = 0.0;
                return false;
            }
            Discard();
            var sendstr = "MEAS:VOLT:DC? (@" + ch.ToString() + ")";
            mw.LogState(LogType.Info, string.Format("MEAS:VOLT:DC? (@{0})", ch.ToString()));
            sp.WriteLine(sendstr);
            mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

            int comTimeout = 0;
            while (sp.BytesToRead < 17)
            {
                if (comTimeout++ > 100)
                {
                    //throw new Exception("Keysight com check Communication Time out");
                    mw.LogState(LogType.Fail, "Keysight Read Timeout");
                    sp.DiscardOutBuffer();
                    data = 0.0;
                    return false;
                }

                if (mw.isStop)
                {
                    mw.LogState(LogType.Info, "EMERGENCY STOP!!");
                    data = 0.0;
                    return false;
                }

                Thread.Sleep(10);
            }
            var lengh = sp.BytesToRead;
            byte[] bt = new byte[lengh];
            sp.Read(bt, 0, lengh);

            rtstring = Encoding.Default.GetString(bt, 0, lengh);
            mw.LogState(LogType.RESPONSE, "KeysightDMM:" + rtstring);
            if (double.TryParse(rtstring, out data))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool MeasVolt_Multi(out List<double> list, string channels, int channelCnt)
        {
            if (!isAlive)
            {
                list = new List<double>();
                return false;
            }
            Discard();
            var sendstr = "MEAS:VOLT:DC? (@" + channels.ToString() + ")";
            mw.LogState(LogType.Info, string.Format("MEAS:VOLT:DC? (@{0})", channels.ToString()));
            sp.WriteLine(sendstr);
            mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

            int comTimeout = 0;
            while (sp.BytesToRead < ((16 * channelCnt) + 1))
            {
                if (comTimeout++ > 300)
                {
                    //throw new Exception("Keysight com check Communication Time out");
                    mw.LogState(LogType.Fail, "Keysight Read Timeout");
                    sp.DiscardOutBuffer();
                    list =new List<double>();
                    return false;
                }
                               
                if (mw.isStop)
                {
                    mw.LogState(LogType.Info, "EMERGENCY STOP!!");
                    list = new List<double>();
                    return false;
                }

                Thread.Sleep(10);
            }
            var lengh = sp.BytesToRead;
            byte[] bt = new byte[lengh];
            sp.Read(bt, 0, lengh);

            rtstring = Encoding.Default.GetString(bt, 0, lengh);
            mw.LogState(LogType.RESPONSE, "KeysightDMM:" + rtstring);
            var val = rtstring.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var tempList = new List<double> { };            
            foreach (var va in val)
            {
                tempList.Add(double.Parse(va));
            }
            list = tempList;
            return true;
        }

        public bool MeasRes_Multi(out List<double> list, string channels, int channelCnt, bool retry = false)
        {
            if (!isAlive)
            {
                list = new List<double>();
                return false;
            }
            Discard();
            var sendstr = "MEAS:RES? " + (retry ? "" : "1000000,") + "(@" + channels.ToString() + ")";
            mw.LogState(LogType.Info, string.Format("MEAS:RES? {0}(@{1})", retry ? "" : "1000000,", channels.ToString()));
            sp.WriteLine(sendstr);
            mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

            int comTimeout = 0;
            while (sp.BytesToRead < ((16 * channelCnt) + 1))
            {
                // 2020.02.10 Noah Choi AutoRange로 허공에서 측정시 8초 가까이 걸림(10회 테스트 결과 MAX : 7.737)
                // 박진호 선임에게 증상 설명 후 15초로 변경 진행(예전 우종선 과장님께서 수정하신 부분)
                if (comTimeout++ > 1500)
                {
                    //throw new Exception("Keysight com check Communication Time out");
                    mw.LogState(LogType.Fail, "Keysight Read Timeout");
                    sp.DiscardOutBuffer();
                    list = new List<double>();
                    return false;
                }

                if (mw.isStop)
                {
                    mw.LogState(LogType.Info, "EMERGENCY STOP!!");
                    list = new List<double>();
                    return false;
                }

                Thread.Sleep(10);
            }
            var lengh = sp.BytesToRead;
            byte[] bt = new byte[lengh];
            sp.Read(bt, 0, lengh);

            rtstring = Encoding.Default.GetString(bt, 0, lengh);
            mw.LogState(LogType.RESPONSE, "KeysightDMM:" + rtstring);
            var val = rtstring.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var tempList = new List<double> { };
            foreach (var va in val)
            {
                tempList.Add(double.Parse(va));
            }
            list = tempList;
            return true;
        }

        public string MeasRes(string ch)
        {
            Discard();
            var sendstr = "MEAS:RES? (@" + ch.ToString() + ")";
            mw.LogState(LogType.Info, string.Format("MEAS:RES? ({0})", ch.ToString()));
            if (sp != null && sp.IsOpen)
            {
                sp.WriteLine(sendstr);
                mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

                int rec = sp.BytesToRead;

                int cnt = 0;
                while (rec < 17)//33
                {
                    Thread.Sleep(100);
                    rec = sp.BytesToRead;
                    cnt += 100;
                    //not received data
                    if (cnt == 2000)
                    {
                        mw.LogState(LogType.Fail, "KeysightDMM - Timeout");
                        return "0.0";
                    }
                }

                byte[] bt = new byte[rec];
                sp.Read(bt, 0, rec);

                rtstring = Encoding.Default.GetString(bt, 0, rec);
                // 20190813 Noah Choi 113채널 응답부분 추가 (박진호선임)
                mw.LogState(LogType.RESPONSE, "KeysightDMM:" + rtstring);
            }
            else
            {
                rtstring = "0.0";
            }

            return rtstring;
        }
        
        public void Discard()
        {
            rtstring = "";
            if (sp != null && sp.IsOpen)
            {
                sp.DiscardInBuffer();
                sp.DiscardOutBuffer();
                Thread.Sleep(10);
                mw.LogState(LogType.Info, "KeysightDMM : Discard IN/OUT Buffer");
            }

            if (socket != null && socket.Connected)
            {
            }
        }

        public void Dispose()
        {
            sp.Dispose();
        }

        #endregion


    }
}
