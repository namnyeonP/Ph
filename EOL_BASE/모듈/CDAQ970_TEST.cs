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
    public class CDAQ970
    {
        //MainWindow mw;
        public bool isAlive = false;
        public string addr = "";
        public Socket socket;
        System.Timers.Timer reConnectTimer;
        IPEndPoint serverEndPoint;

        Thread rcvTrd = null;

        string ReceiveData = string.Empty;

        private void ReceiveDataSocket()
        {
			while (true)
			{
                Thread.Sleep(10);

                if (socket.Connected == false)
                    continue;

                byte[] btRcvData = new byte[1024];

                if (socket.Receive(btRcvData) > 0)
                {
                    string Data = Convert.ToString(btRcvData);

                    if (string.IsNullOrEmpty(Data) == false)
                    {
                        string[] SplitData = Data.Split(',');

                        if (SplitData.Length > 1 && SplitData[1].IndexOf("DAQ970") >= 0)
                        {
                            ReceiveData = SplitData[0];
                        }
                    }
                }

            }
        }

        /// <summary>
        /// TCP
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="ipa">기본값 164.254.4.61</param>
        /// <param name="port">기본값 5025</param>
        public CDAQ970(string dmmIP, string port)
        {
            //this.mw = mw;
            serverEndPoint = new IPEndPoint(IPAddress.Parse(dmmIP), int.Parse(port));
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(serverEndPoint);//TCP   
                socket.ReceiveBufferSize = 1024;

                rcvTrd = new Thread(new ThreadStart(ReceiveDataSocket));
                rcvTrd.IsBackground = true;
                rcvTrd.Start();

                if (socket.Connected)
                {
                    reConnectTimer = new System.Timers.Timer();
                    reConnectTimer.Interval = 8000;
                    reConnectTimer.Elapsed += reConnectTimer_Tick;
                    reConnectTimer.Start();
                    isAlive = true;
                    //var Volt = TrySend("MEAS:VOLT:DC?\n");
                    //if (Volt != 0.0)
                    string version = IDN();
                    if (version != "")
                    {
                        //mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Open Success");
                        //mw.Dispatcher.BeginInvoke(new Action(() =>
                        //{
                        //    mw.contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                        //}));
                    }
                    else
                    {
                        //mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Fail");
                        //mw.Dispatcher.BeginInvoke(new Action(() =>
                        //{
                        //    mw.contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                        //}));
                    }
                }
                else
                {
                    //mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Fail");
                    //mw.Dispatcher.BeginInvoke(new Action(() =>
                    //{
                    //    mw.contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                    //}));
                }
            }
            catch (Exception ec)
            {
                //mw.LogState(LogType.Fail, "KeysightDMM - ", ec);
                try
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    socket.Connect(serverEndPoint);//TCP   
                    socket.ReceiveBufferSize = 1024;

                    if (socket.Connected)
                    {
                        reConnectTimer = new System.Timers.Timer();
                        reConnectTimer.Interval = 8000;
                        reConnectTimer.Elapsed += reConnectTimer_Tick;
                        reConnectTimer.Start();
                        isAlive = true;
                        //var Volt = TrySend("MEAS:VOLT:DC?\n");
                        //if (Volt != 0.0)
                        string version = IDN();
                        if (version != "")
                        {
                            //mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Open Success");
                            //mw.Dispatcher.BeginInvoke(new Action(() =>
                            //{
                            //    mw.contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                            //}));
                        }
                        else
                        {
                           //mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Fail");
                           //mw.Dispatcher.BeginInvoke(new Action(() =>
                           //{
                           //    mw.contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                           //}));
                        }
                    }
                    else
                    {
                        //mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Fail");
                        //mw.Dispatcher.BeginInvoke(new Action(() =>
                        //{
                        //    mw.contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                        //}));
                    }
                }
                catch (Exception ecc)
                {
                    //mw.LogState(LogType.DEVICE_CHECK, "KeysightDMM - " + port + " Exception", ecc);
                    //mw.Dispatcher.BeginInvoke(new Action(() =>
                    //{
                    //    mw.contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                    //}));
                }
            }
        }

        public void _Dispose()
        {
            if (socket != null && socket.Connected)
                socket.Disconnect(false);
        }

        void reConnectTimer_Tick(object sender, EventArgs e)
        {
            if (!socket.Connected)
            {
                socket.Disconnect(true);
                socket.Connect(serverEndPoint);
            }
        }

        public bool TrySend(string sendmsg, out string val)
        {
            string strVal = "";
            try
            {
                if (!socket.Connected)
                {
                    //mw.LogState(LogType.Info, "Check KeysightDMM Connection");
                    val = strVal;
                    return false;
                }
                else
                {
                    byte[] bt = new byte[1024];

                    socket.Send(Encoding.UTF8.GetBytes(sendmsg));
                    //mw.LogState(LogType.Info, "KeysightDMM Send - " + sendmsg.Replace("\n", ""));
                    Array.Clear(bt, 0, 1024);
                    int rec = socket.Receive(bt, SocketFlags.None);

                    if (rec > 0)
                    {
                        strVal = Encoding.Default.GetString(bt, 0, rec);
                        //mw.LogState(LogType.Info, "KeysightDMM Recv - " + strVal.Replace("\n", ""));
                    }
                    val = strVal;
                    return true;
                }
            }
            catch (Exception ec)
            {
                //mw.LogState(LogType.Fail, "KeySightDMM - ", ec);
                val = strVal;
                return false;
            }
        }

        public string IDN()
        {
            string recvMsg = "";

            try
            {
                string str = "*IDN?";
                //mw.LogState(LogType.Info, "Send Message : " + str);

                if (TrySend(str, out recvMsg))
                {
                    //mw.LogState(LogType.Info, "Receive Data : " + recvMsg);
                }
                else
                {
                    //mw.LogState(LogType.Fail, "Receive Error" + recvMsg);
                }
            }
            catch (Exception ex)
            {
                //mw.LogState(LogType.Info, "", ex);
            }

            return recvMsg;
        }

        public bool MeasVolt(out double data, string ch)
        {
            string recvMsg = "";

            data = 0;

            try
            {
                var sendstr = "MEAS:VOLT:DC? (@" + ch.ToString() + ")";
                //mw.LogState(LogType.Info, string.Format("MEAS:VOLT:DC? (@{0})", ch.ToString()));
                //mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

                //if (TrySend(sendstr, out recvMsg))
                //{
                //    if (double.TryParse(recvMsg, out data))
                //        return true;
                //    else
                //        return false;
                //}
                //else
                //{
                //    //mw.LogState(LogType.Fail, "Receive Error" + recvMsg);
                //    data = 0.0;
                //    return false;
                //}
                if (socket == null || socket.Connected == false) return false;
                ReceiveData = "";
                socket.Send(Encoding.UTF8.GetBytes(sendstr));

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

				while (sw.ElapsedMilliseconds < 2000)
				{
                    if (string.IsNullOrEmpty(ReceiveData) == false) break;
                    Thread.Sleep(100);
				}

                if (double.TryParse(ReceiveData.Split(',')[0], out data))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                //mw.LogState(LogType.Info, "", ex);
                data = 0.0;
                return false;
            }
        }

        public bool MeasVolt_Multi(out List<double> list, string channels, int channelCnt)
        {
            string recvMsg = "";

            try
            {
                var sendstr = "MEAS:VOLT:DC? (@" + channels.ToString() + ")";
                //mw.LogState(LogType.Info, string.Format("MEAS:VOLT:DC? (@{0})", channels.ToString()));
                //mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

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
                    //mw.LogState(LogType.Fail, "Receive Error" + recvMsg);
                    list = new List<double>();
                    return false;
                }
            }
            catch (Exception ex)
            {
                //mw.LogState(LogType.Info, "", ex);
                list = new List<double>();
                return false;
            }
        }

        public bool MeasRes(out double data, string ch)
        {
            string recvMsg = "";

            data = 0;

            try
            {
                var sendstr = "MEAS:RES? (@" + ch.ToString() + ")";
                //mw.LogState(LogType.Info, string.Format("MEAS:RES? ({0})", ch.ToString()));
                //mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

                //if (TrySend(sendstr, out recvMsg))
                //{
                //    if (double.TryParse(recvMsg, out data))
                //        return true;
                //    else
                //        return false;
                //}
                //else
                //{
                //    //mw.LogState(LogType.Fail, "Receive Error" + recvMsg);
                //    data = 0.0;
                //    return false;
                //}
                if (socket == null || socket.Connected == false) return false;
                ReceiveData = "";
                socket.Send(Encoding.UTF8.GetBytes(sendstr));

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                while (sw.ElapsedMilliseconds < 2000)
                {
                    if (string.IsNullOrEmpty(ReceiveData) == false) break;
                    Thread.Sleep(100);
                }

                if (double.TryParse(ReceiveData.Split(',')[0], out data))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                //mw.LogState(LogType.Info, "", ex);
                data = 0.0;
                return false;
            }
        }

        public bool MeasRes_Multi(out List<double> list, string channels, int channelCnt, bool retry = false)
        {
            string recvMsg = "";

            try
            {
                var sendstr = "MEAS:RES? " + (retry ? "" : "1000000,") + "(@" + channels.ToString() + ")";
                //mw.LogState(LogType.Info, string.Format("MEAS:RES? {0}(@{1})", retry ? "" : "1000000,", channels.ToString()));
                //mw.LogState(LogType.REQUEST, "KeysightDMM:" + sendstr);

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
                    //mw.LogState(LogType.Fail, "Receive Error" + recvMsg);
                    list = new List<double>();
                    return false;
                }
            }
            catch (Exception ex)
            {
                //mw.LogState(LogType.Info, "", ex);
                list = new List<double>();
                return false;
            }
        }
    }
}
