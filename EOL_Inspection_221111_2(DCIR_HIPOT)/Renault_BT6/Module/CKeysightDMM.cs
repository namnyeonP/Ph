using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Renault_BT6.모듈
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
                    mw.LogState(LogType.Info, "KeysightDMM Connected");
                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.contBt_dmm.Background = System.Windows.Media.Brushes.Green;
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
                        mw.LogState(LogType.Info, "KeysightDMM Connected");
                        mw.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mw.contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                        }));
                    }
                }
                catch (Exception ecc)
                {
                    mw.LogState(LogType.Fail, "KeysightDMM - ", ecc);
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
            sp.BaudRate = 9600;
            sp.PortName = portname;
            sp.Parity = System.IO.Ports.Parity.None;

            isAlive = false;

            try
            {
                sp.Open();
                if (sp.IsOpen)
                {
                    
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
                            isAlive = false;
                            mw.LogState(LogType.Fail, sp.PortName + ", KeysightDMM : " + portname.ToString());
                            return;
                        }
                    } 
                    isAlive = true;
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


        public bool Close()
        {
            try
            {
                sp.DiscardInBuffer();
                sp.DiscardOutBuffer();
                sp.Close();
                sp.Dispose();
            }
            catch(Exception ec)
            {
                mw.LogState(LogType.Fail, "KeysightDMM", ec);
            }

            return true;
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
            Discard();
            var sendstr = "MEAS:VOLT:DC? (@" + ch.ToString() + ")";
            mw.LogState(LogType.Info, string.Format("MEAS:VOLT:DC?{0}", ch.ToString()));
            sp.WriteLine(sendstr);
            mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);
            return rtstring;
        }

        public string MeasVoltTrig(string ch)
        {
            Discard();
            //var sendstr = "INPUT:IMPEDANCE:AUTO ON,(@" + ch.ToString() + ")";
            //mw.LogState(LogType.Info, string.Format("INPUT:IMPEDANCE:AUTO ON,{0}", ch.ToString()));
            //sp.WriteLine(sendstr);
            //mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

            //var sendstr = "TRIG:SOURCE BUS";
            //sp.WriteLine(sendstr);
            //mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

            //sendstr = "TRIG:COUNT 10";
            //sp.WriteLine(sendstr);
            //mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

            //sendstr = "INIT";
            //sp.WriteLine(sendstr);
            //mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);
            return rtstring;
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
                if (comTimeout++ > 300)
                {
                    //throw new Exception("Keysight com check Communication Time out");
                    mw.LogState(LogType.Fail, "Keysight Read Timeout");
                    sp.DiscardOutBuffer();
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
            mw.LogState(LogType.Info, string.Format("MEAS:RES?{0}", ch.ToString()));
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

        public string MeasResManual(string ch)
        {

            Discard();
            //var sendstr = "MEAS:RES? (@" + ch.ToString() + ")";
            var sendstr = "MEAS:RES? 1000000, (@" + ch.ToString() + ")";
            mw.LogState(LogType.Info, string.Format("MEAS:RES?{0}", ch.ToString()));
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

        // 2018.11.09 jeonhj
        // MeasRes와 동일한 동작을 하는 메서드
        // 여러 프로젝트에 갈라져 있어 삭제는 어려우니 일단 추가 먼저
        public string MeasTemp(string ch)
        {
            Discard();
            var sendstr = "MEAS:RES? (@" + ch.ToString() + ")";
            mw.LogState(LogType.Info, string.Format("MEAS:RES?{0}", ch.ToString()));
            sp.WriteLine(sendstr);
            mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);
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


        #region 저항, 전압 계측 변경 버전

        public List<double> MeasVolt_MultiCh(string ch)
        {
            rtstring = "";
            sp.DiscardInBuffer();
            string cmd = "";

            string[] chArr = ch.Split(',');
            int rcvLen = chArr.Length * 16 + 1;

            cmd = "MEAS:VOLT:DC? (@" + ch.ToString() + ")";
            mw.LogState(LogType.Info, "send data : " + cmd);
            sp.WriteLine(cmd);
            Thread.Sleep(300);
            int comTimeout = 0;
            List<double> dd = new List<double>();

            while (sp.BytesToRead < rcvLen)
            {
                if (comTimeout++ > 30)
                {
                    mw.LogState(LogType.Fail, "Keysight Read Timeout");
                    return dd;
                }
                Thread.Sleep(100);
            }

            var lengh = sp.BytesToRead;

            //받은 후에 데이터 파싱
            byte[] bt = new byte[lengh];
            sp.Read(bt, 0, lengh);

            //rtstring = Encoding.Default.GetString(bt, 0, lengh).Replace(",", "&");
            rtstring = Encoding.Default.GetString(bt, 0, lengh);
            mw.LogState(LogType.Info, "Raw read data, " + rtstring);

            var val = rtstring.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var d1 in val)
            {
                var ds = double.Parse(d1).ToString();
                dd.Add(double.Parse(ds));
            }

            return dd;
        }

        public void MeasVoltMulti_Flow(string ch)
        {
            rtstring = "";
            sp.DiscardInBuffer();
            string cmd = "";

            string[] chArr = ch.Split(',');
            int rcvLen = chArr.Length * 16 + 1;

            for (int i = 0; i < 5; i++)
            {
                if (mw.IsEmgStop)
                {
                    mw.LogState(LogType.Info, "S/W Emergency Stop Click");
                    mw.StopAuto();
                    break;
                }
                sp.DiscardOutBuffer();
                cmd = "MEAS:VOLT:DC? (@" + ch.ToString() + ")";
                mw.LogState(LogType.Info, "send data : " + cmd);
                sp.DiscardInBuffer();
                sp.WriteLine(cmd);
                //Thread.Sleep(200);
                //while (sp.BytesToRead < 17)
                //{
                //    Thread.Sleep(100);
                //}
                string ret = sp.ReadLine().Replace(",", "&");
                mw.LogState(LogType.Info, "recv data : " + ret.ToString());
            }
        }

        public double MeasVolt_Single(string ch)
        {
            rtstring = "";
            string cmd = "";
            cmd = "MEAS:VOLT:DC? (@" + ch.ToString() + ")";
            mw.LogState(LogType.Info, "send data : " + cmd);
            sp.DiscardInBuffer();
            sp.WriteLine(cmd);
            Thread.Sleep(300);
            int comTimeout = 0;
            while (sp.BytesToRead < 17)
            {
                if (comTimeout++ > 10)
                {
                    mw.LogState(LogType.Fail, "Keysight Read Timeout");
                    return 0.0;
                }
                Thread.Sleep(100);
            }
            var lengh = sp.BytesToRead;

            //받은 후에 데이터 파싱
            byte[] bt = new byte[lengh];
            sp.Read(bt, 0, lengh);

            rtstring = Encoding.Default.GetString(bt, 0, lengh);
            mw.LogState(LogType.Info, "read data : " + rtstring);
            double val = double.Parse(rtstring);
            //val = Math.Round(val, 3);
            return val;
        }

        public void MeasVoltSingle_Flow(string ch)
        {
            rtstring = "";
            string cmd = "";

            for (int i = 0; i < 5; i++)
            {
                if (mw.IsEmgStop)
                {
                    mw.LogState(LogType.Info, "S/W Emergency Stop Click");
                    mw.StopAuto();
                    break;
                }
                sp.DiscardOutBuffer();
                cmd = "MEAS:VOLT:DC? (@" + ch.ToString() + ")";
                mw.LogState(LogType.Info, "send data : " + cmd);
                sp.DiscardInBuffer();
                sp.WriteLine(cmd);
                string ret = sp.ReadLine();
                mw.LogState(LogType.Info, "recv data : " + ret.ToString());
            }
            sp.DiscardInBuffer();
        }

        public List<double> MeasVolt_ResManual(string ch, bool isretry = false)
        {
            rtstring = "";
            sp.DiscardInBuffer();
            Thread.Sleep(100);
            string cmd = "";
            cmd = "MEAS:RES? " + (isretry ? "" : "1000000,") + "(@" + ch.ToString() + ")\r\n";
            mw.LogState(LogType.Info, "send data : " + cmd);
            sp.Write(cmd);
            int comTimeout = 0;
            List<double> dd = new List<double>();

            string[] chArr = ch.Split(',');
            int rcvLen = chArr.Length * 16 + 1;

            while (sp.BytesToRead < rcvLen)
            {
                if (comTimeout++ > 50)
                {
                    mw.LogState(LogType.Fail, "Keysight Read Timeout");
                    return dd;
                }
                Thread.Sleep(100);

            }

            var lengh = sp.BytesToRead;

            byte[] bt = new byte[lengh];
            sp.Read(bt, 0, lengh);

            rtstring = Encoding.Default.GetString(bt, 0, lengh).Replace(",", "&");
            mw.LogState(LogType.Info, "read data : " + rtstring);
            var val = rtstring.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var d1 in val)
            {
                var ds = double.Parse(d1).ToString();
                dd.Add(double.Parse(ds));
            }
            return dd;
        }
        public List<double> MeasVolt_Res(string ch, bool isretry = false)
        {
            rtstring = "";
            sp.DiscardInBuffer();
            Thread.Sleep(100);
            string cmd = "";
            cmd = "MEAS:RES? (@" + ch.ToString() + ")\r\n";
            mw.LogState(LogType.Info, "send data : " + cmd);
            sp.Write(cmd);
            int comTimeout = 0;
            List<double> dd = new List<double>();

            string[] chArr = ch.Split(',');
            int rcvLen = chArr.Length * 16 + 1;

            while (sp.BytesToRead < rcvLen)
            {
                if (comTimeout++ > 50)
                {
                    mw.LogState(LogType.Fail, "Keysight Read Timeout");
                    return dd;
                }
                Thread.Sleep(100);

            }

            var lengh = sp.BytesToRead;

            byte[] bt = new byte[lengh];
            sp.Read(bt, 0, lengh);

            rtstring = Encoding.Default.GetString(bt, 0, lengh).Replace(",", "&");
            mw.LogState(LogType.Info, "read data : " + rtstring);
            var val = rtstring.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var d1 in val)
            {
                var ds = double.Parse(d1).ToString();
                dd.Add(double.Parse(ds));
            }
            return dd;
        }

        #endregion
    }
}
