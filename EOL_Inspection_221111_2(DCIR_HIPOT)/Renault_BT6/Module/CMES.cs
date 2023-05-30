using Renault_BT6.윈도우;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
//221104
using System.Runtime.InteropServices;

namespace Renault_BT6.모듈
{
    public enum MES_RESP
    {
        OK,
        NG,
        READY,
        HEARTBEAT_CONFIRM,
        JOB_START_REPORT,
        JOB_START_CONFIRM,
        JOB_COMPLETE_CONFIRM,
        PROCESS_CONTROL_PARAMETER_SEND,
        PROCCESING_SPEC_SEND,
        PROCESSING_DATA_CONFIRM,
        PROCESSING_DATA_REPORT,
        REQUEST_TIME_CONFIRM,

    }

    public class CMES
    {
        MainWindow mw;

        public CMES(MainWindow mw)
        {
            this.mw = mw;
        }

        //MES_Web_Service.ServiceSoapClient mesService = null;
        Dictionary<string, double> procSpecList = new Dictionary<string, double>();
        Dictionary<string, string> procSpecStringList = new Dictionary<string, string>();
        public bool IsMESConnected { get; set; }
        public bool isMES_SYS_Disconnected = true;//190109 플래그 추가
        MES_RESP mres = MES_RESP.READY;

        NetworkStream mes_stream;
        System.Windows.Forms.Timer heartbeatTimer = new System.Windows.Forms.Timer();
        TcpClient tc;
        Thread rcvg;
        public Socket socket;
        public int retryConntectcnt = 3;

        //221104 nnkim
        public bool bIsRequestTime = false;

        public Dictionary<string, double> mes_CellList = new Dictionary<string, double>();
        public Dictionary<string, double> m_listMesCellVoltage = new Dictionary<string, double>();

        public string Open()
        {
            IsMESConnected = false;

            string rts = string.Empty;

            if (mes_CellList.Count == 0)
            {
                for (int i = 0; i < 12; i++)
                {
                    mes_CellList.Add("TEMP" + (i + 1).ToString(), 0.0);
                }
            }

            if (CONFIG.MesIP == "")
            {
                return rts = "MES IP EMPTY!";
            }
            //return rts;
            try
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(CONFIG.MesIP), int.Parse(CONFIG.MesPort));

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(serverEndPoint);//TCP   
                socket.ReceiveBufferSize = 1024;

                rcvg = new Thread(new ThreadStart(Reciving));
                rcvg.Start();

                IsMESConnected = true;
                isMES_SYS_Disconnected = false;
                mw.Dispatcher.BeginInvoke(new Action(() =>
                {
                    mw.lotTb.Background =  mw.prodTb.Background = mw.procTb.Background = mw.EquipIDTb.Background = Brushes.SkyBlue;
                    mw.contBt_mes.Background = System.Windows.Media.Brushes.Green;
                    mw.isMESSkipCb.IsEnabled = true;   //200706 wjs change false to true
                }));

                mw.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ldtime = DateTime.Now;
                    ldtime = ldtime.AddSeconds(30);

                    heartbeatTimer.Tick -= new EventHandler(heartbeatTimer_Tick);
                    heartbeatTimer.Tick += new EventHandler(heartbeatTimer_Tick);
                    heartbeatTimer.Interval = 100;
                    heartbeatTimer.Start();
                }));

                mw.LogState(LogType.Info, "MES Connected");

                Device.Relay.Off(RELAY.LIGHT_RED);
                Device.Relay.On(RELAY.LIGHT_BLUE);

                mw.GetMesData();
            }
            catch (Exception ec)
            {
                rts = ec.Message;

                if (!IsMESConnected)
                    mw.LogState(LogType.Fail, "MES Connect : " + rts);

                if (!IsMESConnected && retryConntectcnt != 0)
                {
                    Thread.Sleep(3000);
                    retryConntectcnt--;
                    mw.LogState(LogType.Info, "MES Retry Connection..");
                    Open();
                }

                mw.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //MessageBox.Show("Please Check MES Server", "MES NOT CONNECTED", MessageBoxButton.OK, MessageBoxImage.Information);
                    mw.LogState(LogType.Fail, "Mes not connected");

                    IsMESConnected = false;

                    mw.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mw.isMESSkipCb.IsEnabled = true;
                        mw.lotTb.Background = mw.EquipIDTb.Background = mw.prodTb.Background = mw.procTb.Background = Brushes.Red;
                        mw.contBt_mes.Background = Brushes.Red;
                    }));

                    Device.Relay.On(RELAY.LIGHT_RED);
                    Device.Relay.Off(RELAY.LIGHT_BLUE);
                    isMES_SYS_Disconnected = true;
                }));

            }
            return rts;
        }

        public void Close()
        {
            try
            {
                IsMESConnected = false;

                socket.Disconnect(false);
                socket.Close();
                mw.LogState(LogType.Info, "MES Close....");
            }
            catch
            {

            }
        }

        //221104 nnkim
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }
        [DllImport("kernel32")]
        public static extern int SetSystemTime([In] SystemTime st);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetLocalTime(ref SystemTime st);

        /// <summary>
        /// 루프돌면서 소켓통신으로 받은 값으로 동작
        /// 
        /// </summary>
        private void Reciving()
        {
            try
            {
                byte[] bt = new byte[ushort.MaxValue];
                while (true)
                {
                    if (socket.Connected)
                    {
                        Array.Clear(bt, 0, ushort.MaxValue);
                        int rec = socket.Receive(bt, SocketFlags.None);

                        if (rec > 0)
                        {
                            //190121 추가됨!
                            //190315 변경됨!!
                            mw.LogState(LogType.RESPONSE, "[RAW_DATA]," + Encoding.Default.GetString(bt).Replace("\0", ""));

                            var msgID = System.Text.Encoding.UTF8.GetString(new byte[] { bt[4], bt[5], bt[6] });

                            var barr = new byte[bt.Length - 9];
                            int cnt = 0;
                            for (int i = 11; i < bt.Length; i++)
                            {
                                barr[cnt] = bt[i];
                                cnt++;
                            }
                            var str = System.Text.Encoding.UTF8.GetString(barr);
                            str = str.Replace("\0", "");

                            switch (msgID)
                            {
                                #region "202"  Job Start Confirm
                                case "202": // Job Start Confirm
                                    {
                                        var strarr = str.Split(',');

                                        if (strarr[0].Contains("NG"))
                                        {
                                            mres = MES_RESP.NG;
                                            mw.LogState(LogType.RESPONSE, "StartJobInsp - " + str);
                                        }
                                        else
                                        {
                                            mres = MES_RESP.JOB_START_CONFIRM;
                                            mw.LogState(LogType.RESPONSE, "StartJobInsp - " + str);

                                            #region   //220906 Sigma Level 기능 추가 NNKIM
                                            m_listMesCellVoltage.Clear();
                                            int nIndex = 0;
                                            //wjs
                                            foreach (var item in strarr)
                                            {
                                                /*
                                                if (item.Contains("CMAID"))
                                                {
                                                    var mono = item.Replace("CMAID=", "");
                                                    mono = mono.Replace("\n", "");

                                                    mw.Dispatcher.Invoke(new Action(() =>
                                                    {
                                                        mw.MonoFrame = mw.monoTb.Text = mono;
                                                    }));

                                                    mw.LogState(LogType.RESPONSE, "StartJobInsp - Module BCR:" + mono);
                                                } */
                                                if (nIndex == 3) // MES 어떤식으로 내려오는지 확인 필요 (이건 전압만 내려줄때)
                                                {
                                                    var strArrCellVoltage = item.Split('^');

                                                    for (int i = 0; i < strArrCellVoltage.Length; i++)
                                                    {
                                                        var strID = strArrCellVoltage[i].Split('=')[0];
                                                        var strVoltage = double.Parse(strArrCellVoltage[i].Split('=')[1]);
                                                        m_listMesCellVoltage.Add(strID, strVoltage);
                                                    }

                                                    for (int i = 0; i < m_listMesCellVoltage.Count; ++i)
                                                    {
                                                        mw.LogState(LogType.Info, "MES Voltage : " +
                                                            m_listMesCellVoltage.ToList()[i].Key.ToString() +
                                                            m_listMesCellVoltage.ToList()[i].Value.ToString());
                                                    }
                                                }
                                                ++nIndex;
                                            }//wjs
                                            #endregion

                                            if (CONFIG.EolInspType == InspectionType.EOL)
                                            {
                                                mw.LogState(LogType.Info, "Initialize ID/CV Dictionary");

                                                mes_CellList.Clear();

                                                if (strarr.Length > 3)
                                                {

                                                    var lot_cv_Arr = strarr[3].Split('^');

                                                    for (int i = 0; i < lot_cv_Arr.Length; i++)
                                                    {
                                                        var id = lot_cv_Arr[i].Split('=')[0];
                                                        var cv = double.Parse(lot_cv_Arr[i].Split('=')[1]);
                                                        mes_CellList.Add(id, cv);
                                                        mw.LogState(LogType.Info, "ID:" + id + " / Cell Voltage :" + cv.ToString());
                                                    }
                                                }
                                            }
                                            //mw.LogState(LogType.Info, mres.ToString());
                                        }
                                    };
                                    break;
                                #endregion
                                #region "204" Job Complete Comfirm
                                case "204": 
                                    {
                                        var strarr = str.Split(',');

                                        if (strarr[0].Contains("NG"))
                                        {
                                            mres = MES_RESP.NG;
                                            mw.LogState(LogType.RESPONSE, "EndJobInsp - " + str);
                                        }
                                        else
                                        {
                                            mres = MES_RESP.JOB_COMPLETE_CONFIRM;
                                            //mw.LogState(LogType.Info, mres.ToString());
                                            mw.LogState(LogType.RESPONSE, "EndJobInsp - " + str);
                                        }
                                    }; break;
                                #endregion
                                #region "302" Process Contorl Parameter Send
                                case "302":
                                    {
                                        var strarr = str.Split(',');

                                        if (strarr[0].Contains("NG"))
                                        {
                                            mres = MES_RESP.NG;
                                            mw.LogState(LogType.RESPONSE, "GetProcessControlParameterRequest - " + str);
                                        }
                                        else
                                        {
                                            lastSpec = str;
                                            mres = MES_RESP.PROCESS_CONTROL_PARAMETER_SEND;
                                            mw.LogState(LogType.RESPONSE, "GetProcessControlParameterResponse - " + lastSpec);
                                        }

                                    }; break;
                                #endregion
                                #region "402" Process Spec Send
                                case "402":
                                    {
                                        var strarr = str.Split(',');

                                        if (strarr[0].Contains("NG"))
                                        {
                                            mres = MES_RESP.NG;
                                            mw.LogState(LogType.RESPONSE, "GetProcessingSpecRequest - " + str);
                                        }
                                        else
                                        {
                                            lastSpec = str;
                                            mres = MES_RESP.PROCCESING_SPEC_SEND;
                                            mw.LogState(LogType.RESPONSE, "GetProcessingSpecResponse - " + lastSpec);
                                        }
                                    }; break;
                                #endregion
                                #region "404" Process Data Confirm
                                case "404":
                                    {
                                        var strarr = str.Split(',');

                                        if (strarr[0].Contains("NG"))
                                        {
                                            mres = MES_RESP.NG;
                                            mw.LogState(LogType.RESPONSE, "GetProcessDataReport - " + str);
                                        }
                                        else
                                        {
                                            mres = MES_RESP.PROCESSING_DATA_REPORT;
                                            mw.LogState(LogType.RESPONSE, "GetProcessDataReport - " + str);
                                        }

                                    }; break;
                                #endregion
                                #region "901~904"
                                case "901":   // Unknown message
                                    {
                                        mres = MES_RESP.NG;
                                        mw.LogState(LogType.RESPONSE, "901" + str);
                                    }; break;
                                case "902":
                                    {
                                        mres = MES_RESP.NG;
                                        mw.LogState(LogType.RESPONSE, "902" + str);
                                    }; break;
                                case "903": // Transaction Timeout
                                    {
                                        mres = MES_RESP.NG;
                                        mw.LogState(LogType.RESPONSE, "903" + str);
                                    }; break;
                                case "904":
                                    {
                                        mres = MES_RESP.NG;
                                        mw.LogState(LogType.RESPONSE, "904" + str);
                                    }; break;
                                case "104":
                                    {
                                        //221104
                                        var strarr = str.Split(',');

                                        if (strarr[0].Contains("NG"))
                                        {
                                            mres = MES_RESP.NG;
                                            mw.LogState(LogType.RESPONSE, "Recieve Server Time - " + str);
                                            //mw.LogState(LogType.RESPONSE, "StartJobInsp - " + str2);
                                            //mw.LogState(LogType.RESPONSE, "StartJobInsp - " + str3);
                                            //mw.LogState(LogType.RESPONSE, "StartJobInsp - " + str4);

                                            bIsRequestTime = false;
                                        }
                                        else
                                        {
                                            mw.LogState(LogType.RESPONSE, "Recieve Server Time - " + str);

                                            foreach (var item in strarr)
                                            {
                                                if (item.Contains("SYSTIME"))
                                                {
                                                    var mono = item.Replace("SYSTIME=", "");
                                                    mono = mono.Replace("\n", "");
                                                    mono = mono.Substring(0, mono.Length);

                                                    string strDate = string.Format("{0}-{1}-{2} {3}:{4}:{5}", mono.Substring(0, 4),
                                                        mono.Substring(4, 2), mono.Substring(6, 2), mono.Substring(8, 2), mono.Substring(10, 2),
                                                        mono.Substring(12, 2));

                                                    System.DateTime dtDateTime;
                                                    dtDateTime = Convert.ToDateTime(strDate); // 서버로 부터 받아온 시간.yyyy-mm-dd hh:mm:ss

                                                    // 우리나라의 표준 시간대로 변경하기 위해서

                                                    // 컴퓨터의 시간을 변경한다.
                                                    SystemTime ServerTime = new SystemTime();

                                                    ServerTime.wYear = (ushort)dtDateTime.Year;

                                                    ServerTime.wMonth = (ushort)dtDateTime.Month;
                                                    ServerTime.wDay = (ushort)dtDateTime.Day;
                                                    ServerTime.wHour = (ushort)dtDateTime.Hour;
                                                    ServerTime.wMinute = (ushort)dtDateTime.Minute;
                                                    ServerTime.wSecond = (ushort)dtDateTime.Second;

                                                    bool result = SetLocalTime(ref ServerTime);
                                                    if (result == false)
                                                    {
                                                        int lastError = Marshal.GetLastWin32Error();
                                                        Console.WriteLine(lastError);
                                                        bIsRequestTime = false;
                                                    }
                                                    else
                                                    {
                                                        bIsRequestTime = true;
                                                    }
                                                }
                                            }

                                            mres = MES_RESP.REQUEST_TIME_CONFIRM;
                                        }
                                    }; break;
                                    #endregion
                                    // 2020.01.12 KSM Remove
                                    //default:
                                    //    {
                                    //        mres = MES_RESP.NG;
                                    //        mw.LogState(LogType.RESPONSE, msgID + str);
                                    //    }; break;
                            }
                        }
                    }
                }
            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Info, "MES Disconnect");
                mw.LogState(LogType.Info, "Reciving", ec);

                Close();
                mw.SetMESSkipCheck();
            }
        }

        public string lastSpec = "";
        DateTime ldtime;

        int heartbeatSno = 1;
        void heartbeatTimer_Tick(object sender, EventArgs e)
        {
            //마지막으로 보낸 시간을 측정해서.. 30초가 흘렀으면 
            //해당 메시지를 보내라

            if (DateTime.Now.Ticks > ldtime.Ticks)
            {
                SendHeartbeating();
            }
        }

        public bool SendHeartbeating()
        {
            var sno = heartbeatSno.ToString("D4");

            if (heartbeatSno == 9999)
                heartbeatSno = 1;

            heartbeatSno++;

            return GetToStreamMES('E', 'S', 'R', "101", sno, "");
        }


        Object sendlock = new Object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction">E</param>
        /// <param name="type">S</param>
        /// <param name="reply">R</param>
        /// <param name="msgID">401</param>
        /// <param name="seqNo">0001</param>
        /// <param name="prodID">EVEVPBMHREVA0</param>
        /// <param name="procID">BTA02</param>
        /// <returns></returns>
        bool GetToStreamMES(char direction, char type, char reply, string msgID, string seqNo, string content)
        {
            lock (sendlock)
            {
                var str = "";
                var header = new byte[]{
                02,
                Convert.ToByte(direction),
                Convert.ToByte(type),
                Convert.ToByte(reply),
                Convert.ToByte(msgID.ToCharArray()[0]),
                Convert.ToByte(msgID.ToCharArray()[1]),
                Convert.ToByte(msgID.ToCharArray()[2]),
                Convert.ToByte(seqNo.ToCharArray()[0]),
                Convert.ToByte(seqNo.ToCharArray()[1]),
                Convert.ToByte(seqNo.ToCharArray()[2]),
                Convert.ToByte(seqNo.ToCharArray()[3])};
                var body = System.Text.Encoding.ASCII.GetBytes(content);
                byte[] bt = new byte[header.Length + body.Length + 3];

                Array.Copy(header, bt, header.Length);

                Array.Copy(body, 0, bt, header.Length, body.Length);
                int checksum = 0;
                for (int i = 1; i < bt.Length; i++)
                {
                    checksum += (byte)bt[i];
                }
                //foreach (byte chData in bt)
                //{
                //    checksum += chData;
                //}
                bt[bt.Length - 3] = Convert.ToByte((checksum.ToString().Remove(0, 1)).ToCharArray()[(checksum.ToString().Remove(0, 1)).ToCharArray().Length - 2]);
                bt[bt.Length - 2] = Convert.ToByte((checksum.ToString().Remove(0, 1)).ToCharArray()[(checksum.ToString().Remove(0, 1)).ToCharArray().Length - 1]);
                bt[bt.Length - 1] = 0x03;
                try
                {
                    if (this.IsMESConnected)
                    {
                        socket.Send(bt);
                        mw.LogState(LogType.REQUEST, "[RAW_DATA]," + System.Text.Encoding.Default.GetString(bt));
                        ldtime = DateTime.Now;
                        ldtime = ldtime.AddSeconds(30);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (SocketException soc)
                {
                    mw.LogState(LogType.Fail, "MES Disconnected", soc);
                    this.IsMESConnected = false;
                    tossedMES = new Thread(new ThreadStart(RetryMES));
                    tossedMES.Start();

                }
                return false;
            }
        }

        Thread tossedMES;

        public void RetryMES()
        {
            retryConntectcnt = 3;
            mw.LogState(LogType.Info, "MES Retry Connection..");
            Open();
        }

        int startJopInspSno = 1;
        public string StartJobInsp(string LotID, string procID, string EqpID, string UserID)
        {
            string rts = string.Empty;
            var mid = "201";
            var sno = startJopInspSno.ToString("D4");

            if (startJopInspSno == 9999)
                startJopInspSno = 1;

            try
            {
                //mesService = new MES_Web_Service.ServiceSoapClient();

                if (!IsMESConnected)
                    return rts;

                StringBuilder body = new StringBuilder();

                body.Append("LOTID=");
                body.Append(LotID);
                body.Append(",");

                body.Append("PROCID=");
                body.Append(procID);
                body.Append(",");

                body.Append("EQPID=");
                body.Append(EqpID);
                body.Append(",");

                body.Append("USERID=");
                body.Append(UserID);

                mw.LogState(LogType.REQUEST, "StartJobInsp:" + body.ToString());

                mres = MES_RESP.READY;
                GetToStreamMES('E', 'S', 'R', mid, sno, body.ToString());

                //착공하고 리스폰스가 있을때까지 대기
                //10초후에는 903치고 리턴
                var oldTime = DateTime.Now;
                var ltime = oldTime;
                var after10s = oldTime.AddSeconds(10);
                while (mres == MES_RESP.READY)
                {
                    ltime = DateTime.Now;
                    Thread.Sleep(100);
                    if (ltime.Ticks >= after10s.Ticks)
                    {
                        SendToTimeout(mid, sno);
                        rts = "NG";
                        return rts;
                    }
                }

                switch (mres)
                {
                    case MES_RESP.NG: rts = "NG"; break;
                    case MES_RESP.JOB_START_CONFIRM:; break;
                }

                mw.LogState(LogType.RESPONSE, "MES StartJobInsp - " + mres.ToString() + rts);

                startJopInspSno++;
            }
            catch (Exception ec)
            {
            }
            return rts;
        }

        int endJobInspSno = 1;
        /// <summary>
        /// 설비에서측정한검사결과Data로완공보고(Job Complete Report) 시판정값과함께보고되며항목은사전에정의함.
        /// </summary>
        /// <param name="LotID"></param>
        /// <param name="procID"></param>
        /// <param name="EqpID"></param>
        /// <param name="UserID"></param>
        /// <param name="rsncode"></param>
        /// <param name="strbody"></param>
        /// <returns></returns>
        public string EndJobInsp(string LotID, string procID, string EqpID, string UserID, string rsncode, string strbody)
        {
            string rts = string.Empty;
            var mid = "203";
            var sno = endJobInspSno.ToString("D4");

            if (endJobInspSno == 9999)
                endJobInspSno = 1;

            try
            {
                //mesService = new MES_Web_Service.ServiceSoapClient();

                if (!IsMESConnected)
                    return rts;

                StringBuilder body = new StringBuilder();

                body.Append("LOTID=");
                body.Append(LotID);
                body.Append(",");

                body.Append("PROCID=");
                body.Append(procID);
                body.Append(",");

                body.Append("EQPID=");
                body.Append(EqpID);
                body.Append(",");

                body.Append("USERID=");
                body.Append(UserID);
                body.Append(",");

                body.Append("RESNCODE=");
                body.Append(rsncode);
                body.Append(",");

                body.Append(strbody);

                mw.LogState(LogType.REQUEST, "EndJobInsp:" + body.ToString());

                mres = MES_RESP.READY;
                GetToStreamMES('E', 'S', 'R', mid, sno, body.ToString());

                //착공하고 리스폰스가 있을때까지 대기
                //10초후에는 903치고 리턴
                var oldTime = DateTime.Now;
                var ltime = oldTime;
                var after10s = oldTime.AddSeconds(10);
                while (mres == MES_RESP.READY)
                {
                    ltime = DateTime.Now;
                    Thread.Sleep(100);
                    if (ltime.Ticks >= after10s.Ticks)
                    {
                        SendToTimeout(mid, sno);
                        rts = "NG";
                        return rts;
                    }
                }

                switch (mres)
                {
                    case MES_RESP.NG:
                        rts = "NG"; break;
                    case MES_RESP.JOB_COMPLETE_CONFIRM: 
                        rts = "OK"; break;
                }

                mw.LogState(LogType.RESPONSE, "MES EndJobInsp - " + mres.ToString());

                endJobInspSno++;
            }
            catch (Exception ec)
            {
                rts = "NG";
                return rts;
            }
            return rts;
        }
        int processControlParameterSno = 1;
        /// <summary>
        /// 설비에서해당공정진행을위해Host에서전송하는Setting 값으로항목은사전에정의함.
        /// </summary>
        /// <param name="ProdID"></param>
        /// <param name="ProcID"></param>
        /// <returns></returns>
        public string GetProcessControlParameterRequest(string equipId, string ProdID, string ProcID)
        {
            var rts = string.Empty;
            var mid = "301";
            var sno = processControlParameterSno.ToString("D4");

            if (processControlParameterSno == 9999)
                processControlParameterSno = 1;


            if (IsMESConnected)
            {
                StringBuilder body = new StringBuilder();

                body.Append("EQPID=");
                body.Append(equipId);
                body.Append(",");

                body.Append("PRODID=");
                body.Append(ProdID);
                body.Append(",");

                body.Append("PROCID=");
                body.Append(ProcID);

                mw.LogState(LogType.REQUEST, "GetProcessControlParameterRequest:" + body.ToString());

                mres = MES_RESP.READY;

                GetToStreamMES('E', 'S', 'R', mid, sno, body.ToString());

                //착공하고 리스폰스가 있을때까지 대기
                //10초후에는 903치고 리턴
                var oldTime = DateTime.Now;
                var ltime = oldTime;
                var after10s = oldTime.AddSeconds(10);
                while (mres == MES_RESP.READY)
                {
                    ltime = DateTime.Now;
                    Thread.Sleep(100);
                    if (ltime.Ticks >= after10s.Ticks)
                    {
                        SendToTimeout(mid, sno);
                        rts = "NG";
                        return rts;
                    }
                }

                Thread.Sleep(100);
                if (mres == MES_RESP.PROCESS_CONTROL_PARAMETER_SEND)
                {
                    rts = lastSpec;
                }
                else if (mres == MES_RESP.NG)
                {
                    rts = "NG";
                }
                processControlParameterSno++;
                return rts;
            }
            else
            {
                return rts;
            }
        }

        int processingSpecSno = 1;
        /// </summary>
        /// <param name="ProdID"></param>
        /// <param name="ProcID"></param>
        /// <returns></returns>
        public string GetProcessingSpecRequest(string equipId, string ProdID, string ProcID)
        {
            var rts = string.Empty;
            var mid = "401";
            var sno = processingSpecSno.ToString("D4");

            if (processingSpecSno == 9999)
                processingSpecSno = 1;


            if (IsMESConnected)
            {
                StringBuilder body = new StringBuilder();

                body.Append("EQPID=");
                body.Append(equipId);
                body.Append(",");

                body.Append("PRODID=");
                body.Append(ProdID);
                body.Append(",");

                body.Append("PROCID=");
                body.Append(ProcID);

                mw.LogState(LogType.REQUEST, "GetProcessingSpecRequest:" + body.ToString());

                mres = MES_RESP.READY;

                GetToStreamMES('E', 'S', 'R', mid, sno, body.ToString());

                //착공하고 리스폰스가 있을때까지 대기
                //10초후에는 903치고 리턴
                var oldTime = DateTime.Now;
                var ltime = oldTime;
                var after10s = oldTime.AddSeconds(10);
                while (mres == MES_RESP.READY)
                {
                    ltime = DateTime.Now;
                    Thread.Sleep(100);
                    if (ltime.Ticks >= after10s.Ticks)
                    {
                        SendToTimeout(mid, sno);
                        rts = "NG";
                        return rts;
                    }
                }

                Thread.Sleep(100);
                if (mres == MES_RESP.PROCCESING_SPEC_SEND)
                {
                    rts = lastSpec;
                }
                else if (mres == MES_RESP.NG)
                {
                    rts = "NG";
                }
                processingSpecSno++;
                return rts;
            }
            else
            {
                return rts;
            }
        }

        int timeoutSno = 1;
        public void SendToTimeout(string mid, string sno)
        {
            try
            {
                var thisSno = timeoutSno.ToString("D4");
                if (timeoutSno == 9999)
                    timeoutSno = 1;

                mw.LogState(LogType.Fail, "Transaction TimeOut!!");
                GetToStreamMES('E', 'S', 'N', "903", thisSno, "NONRVMSGID=" + mid + ",SDSQNO=" + sno);

                timeoutSno++;
            }
            catch
            {

            }
        }


        int processDataReportSno = 1;

        /// <summary>
        /// 상세수집항목보고
        /// </summary>
        /// <param name="LotID"></param>
        /// <param name="procID"></param>
        /// <param name="EqpID"></param>
        /// <param name="CLCTDITEM"></param>
        /// <returns></returns>
        public string GetProcessDataReport(string LotID, string procID, string EqpID, string userID, string CLCTDITEM)
        {
            var rts = string.Empty;
            var mid = "403";
            var sno = processDataReportSno.ToString("D4");

            if (processDataReportSno == 9999)
                processDataReportSno = 1;


            if (IsMESConnected)
            {
                StringBuilder body = new StringBuilder();

                body.Append("LOTID=");
                body.Append(LotID);
                body.Append(",");

                body.Append("PROCID=");
                body.Append(procID);
                body.Append(",");

                body.Append("EQPID=");
                body.Append(EqpID);
                body.Append(",");
                body.Append("USERID=");
                body.Append(userID);
                body.Append(",");
                body.Append(CLCTDITEM);

                mw.LogState(LogType.REQUEST, "GetProcessDataReport:" + body.ToString());

                mres = MES_RESP.READY;

                GetToStreamMES('E', 'S', 'R', mid, sno, body.ToString());

                //착공하고 리스폰스가 있을때까지 대기
                //10초후에는 903치고 리턴
                var oldTime = DateTime.Now;
                var ltime = oldTime;
                var after10s = oldTime.AddSeconds(10);
                while (mres == MES_RESP.READY)
                {
                    ltime = DateTime.Now;
                    Thread.Sleep(100);
                    if (ltime.Ticks >= after10s.Ticks)
                    {
                        SendToTimeout(mid, sno);
                        rts = "NG";
                        return rts;
                    }
                }
                switch (mres)
                {
                    case MES_RESP.NG: rts = "NG"; break;
                    case MES_RESP.PROCESSING_DATA_REPORT:; break;
                }

                if (rts == string.Empty)
                {
                    mw.LogState(LogType.Info, "MES ProcessDataReport");
                }
                processDataReportSno++;
                return rts;
            }
            else
            {
                return rts;
            }
        }

        //221104 ServerTime nnkim
        int ServerTimeSno = 1;
        public string ServerTime_Request(string EqpID)
        {
            string rts = string.Empty;
            var sno = ServerTimeSno.ToString("D4");
            var mid = "103";

            if (ServerTimeSno == 9999)
                ServerTimeSno = 1;

            try
            {
                ServerTimeSno++;

                StringBuilder body = new StringBuilder();

                body.Append("EQPID=");
                body.Append(EqpID);
                body.Append(",");

                GetToStreamMES('E', 'S', 'R', mid, sno, body.ToString());

                //착공하고 리스폰스가 있을때까지 대기
                //10초후에는 903치고 리턴 // kyoungsuk 30s 220324
                mres = MES_RESP.READY;

                var oldTime = DateTime.Now;
                var ltime = oldTime;
                var after10s = oldTime.AddSeconds(30);
                while (mres == MES_RESP.READY)
                {
                    ltime = DateTime.Now;
                    Thread.Sleep(100);
                    if (ltime.Ticks >= after10s.Ticks)
                    {
                        SendToTimeout(mid, sno);
                        rts = "NG";
                        return rts;
                    }
                }

                switch (mres)
                {
                    case MES_RESP.NG: rts = "NG"; break;
                    case MES_RESP.REQUEST_TIME_CONFIRM:; break;
                }

                mw.LogState(LogType.RESPONSE, "MES ServerTime_Request - " + mres.ToString());
            }
            catch (Exception ec)
            {
            }

            return rts;
        }
    }
}
