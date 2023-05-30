using Peak.Can.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPCANHandle = System.UInt16;
using TPCANBitrateFD = System.String;
using TPCANTimestampFD = System.UInt64;
using Peak.Can.Basic;
using System.Threading;
using EOL_BASE.클래스;
using System.Diagnostics;

namespace EOL_BASE.모듈
{
    public class CBMS_CAN
    {
        MainWindow mw;

        private bool _isSendMsg = false;

        private bool _extendedMsg = false;

        public bool MOSFET_CLOSE = false;

        public bool ExtendedMsg
        {
            get { return _extendedMsg; }
            set
            {
                if (_extendedMsg != value)
                {
                    _extendedMsg = value;
                    if (value)
                    {
                        mw.LogState(LogType.Info, "Start Send Extended Msg");
                    }
                    else
                    {
                        mw.LogState(LogType.Info, "Stop Send Extended Msg");
                    }
                }
            }
        }

        public bool IsSendMsg
        {
            get { return _isSendMsg; }
            set
            {
                if (_isSendMsg != value)
                {
                    _isSendMsg = value;
                    if (value)
                    {
                        mw.LogState(LogType.Info, "Start Send TX Msg");
                    }
                    else
                    {
                        mw.LogState(LogType.Info, "Stop Send TX Msg");
                    }
                }
            }
        }

        Thread sendThread1, sendThread2;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        /// <param name="canID">PCAN_USB:FD 1 (51h) ~ 1번부터 연결된 갯수대로</param>
        /// <param name="isSend">TX메시지를 보낼것이 있다면, true</param>
        public CBMS_CAN(MainWindow mw, string canID, bool isSend)
        {
            this.mw = mw;
            InitializePCANComponents();
            SetPCAN_BMS1(canID);
            if (isSend)
            {
                //무언가 지속적으로 보내야 한다면, 해당부분을 쓸 것
                StartSendThread();
            }
        }

        /// <summary>
        /// 소멸자
        /// </summary>
        public void Dispose()
        {
            sendThread1.Abort();
            sendThread2.Abort();
            PCANBasic.Uninitialize(m_PcanHandle1);
        }

        //public double cellVolt1 = 0.0;
        //public double cellVolt2 = 0.0;
        //public double cellVolt3 = 0.0;
        //public double cellVolt4 = 0.0;
        //public double cellVolt5 = 0.0;
        //public double cellVolt6 = 0.0;
        /// <summary>
        /// tx thread starting
        /// </summary>
        void StartSendThread()
        {
            sendThread2 = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);//1ms 쉬면서 보냄
                    if (IsSendMsg)
                    {
                        if (bmsList.ContainsKey("665h") && bmsList.ContainsKey("666h"))
                        {
                            //Stopwatch st = new Stopwatch();
                            //st.Start();
                            //cellVolt1 = GetBMSData_int("665h", 0, 16) * 0.001;
                            //cellVolt2 = GetBMSData_int("665h", 16, 16) * 0.001;
                            //cellVolt3 = GetBMSData_int("665h", 32, 16) * 0.001;
                            //cellVolt4 = GetBMSData_int("665h", 48, 16) * 0.001;
                            //cellVolt5 = GetBMSData_int("666h", 0, 16) * 0.001;
                            //cellVolt6 = GetBMSData_int("666h", 16, 16) * 0.001;
                            
                            //mw.LogState(LogType.Info, string.Format("{0},{1},{2},{3},{4},{5}-{6}",
                            //    cellVolt1, cellVolt2, cellVolt3, cellVolt4, cellVolt5, cellVolt6, st.ElapsedTicks));
                            //st.Stop();
                        }

                    }
                }
            });
            //sendThread2.Start();
            sendThread1 = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);//1ms 쉬면서 보냄

                    if (IsSendMsg)
                    {

                        var time = double.Parse(System.DateTime.Now.ToString("fff"));

                        if (time % 10 == 0)//10ms주기로 보냄
                        {
                            //vehicle operation modes_HS1
                            SendToCAN("167", new byte[] { 0x10, 0x0, 0x0, 0x01, 0xFF, 0xE0, 0x0, 0x0 });

                            //powertrain_data_5_HS1
                            if (MOSFET_CLOSE)
                            {
                                SendToCAN("43F", new byte[] { 0x0, 0x50, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
                            }
                            else
                            {
                                SendToCAN("43F", new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
                            }

                            //bodyinfo_8
                            if (MOSFET_CLOSE)
                            {
                                SendToCAN("3B9", new byte[] { 0xFF, 0x0, 0x0, 0x05, 0x0, 0x0, 0x0, 0x0 });
                            }
                            else
                            {
                                SendToCAN("3B9", new byte[] { 0xFF, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });
                            }
                        }

                        if (time % 50 == 0)
                        {
                            //bodyinfo_3
                            SendToCAN("3B3", new byte[] { 0x44, 0x0, 0x0, 0x0C, 0xE6, 0x0, 0x0, 0x0 });
                        }

                        if (time % 999 == 0)
                        {
                            //별도 플래그
                            //SendToCAN("7FF", new byte[] { 0x02, 0x3E, 0x0, 0xFF, 0x0, 0x0, 0x0, 0x0 });
                        }

                        if (time % 200 == 0)
                        {
                        }

                        if (time % 500 == 0)
                        {
                        }
                    }
                }

            });
            sendThread1.Start();
        }


        /// <summary>
        /// id로 인텔방식의 데이터를 가져오는 메서드
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stbit">시작비트</param>
        /// <param name="length">길이</param>
        /// <param name="isLogging">로깅 여부(기본 true)</param>
        /// <returns></returns>
        public string GetBMSDataToIntel(object id, int stbit, int length, bool isLogging = true)
        {
            var sid = id.ToString() + "h";
            try
            {
                if (bmsList.ContainsKey(sid))
                {
                    var data = bmsList[sid].Data.Replace(" ", "");

                    var toint = ReverseDataAtIntelType(data, stbit, length).ToString();

                    //this.LogState(LogType.Info, "CAN_ID," + id + ",StartBit:"+stbit.ToString()+",Length:"+length.ToString()+",ToInt32:" + toint + ",binary:" + rtv);
                    if (isLogging)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("CAN_ID :");
                        sb.Append(id);
                        sb.Append(" - ");
                        sb.Append(toint);

                        mw.LogState(LogType.Info, sb.ToString());

                        sb = new StringBuilder();
                        sb.Append(id);
                        sb.Append(",");
                        sb.Append(bmsList[sid].Data.Replace(" ", ","));
                        mw.LogState(LogType.CAN, sb.ToString());
                    }

                    return toint;

                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Fail, "DetailDataSave", ex);
            }
            return "0";
        }


        /// <summary>
        /// id로 모토로라 방식의 데이터를 가져오는 메서드
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stbit">시작비트</param>
        /// <param name="length">길이</param>
        /// <param name="isLogging">로깅 여부(기본 true)</param>
        /// <returns></returns>
        public string GetBMSData(object id, int stbit, int length, bool isLogging = true)
        {
            if (length > 8)
            {
            }
            var sid = id.ToString() + "h";
            try
            {
                if (bmsList.ContainsKey(sid))
                {
                    var data = bmsList[sid].Data.Replace(" ", "");
                    var ret = string.Join(String.Empty, data.Select(c => Convert.ToString(Convert.ToInt64(c.ToString(), 16), 2).PadLeft(4, '0')));
                    var arr = ret.ToArray();
                    var rtv = "";
                    for (int i = stbit; i < stbit + length; i++)
                    {
                        rtv += arr[i];
                    }
                    var toint = Convert.ToInt32(rtv, 2).ToString();
                    //this.LogState(LogType.Info, "CAN_ID," + id + ",StartBit:"+stbit.ToString()+",Length:"+length.ToString()+",ToInt32:" + toint + ",binary:" + rtv);
                    if (isLogging)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("CAN_ID :");
                        sb.Append(id);
                        sb.Append(" - ");
                        sb.Append(toint);

                        mw.LogState(LogType.Info, sb.ToString());

                        sb = new StringBuilder();
                        sb.Append(id);
                        sb.Append(",");
                        sb.Append(bmsList[sid].Data.Replace(" ", ","));
                        mw.LogState(LogType.CAN, sb.ToString());
                    }

                    return toint;

                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Fail, "DetailDataSave", ex);
            }
            return "0";
        }


        /// <summary>
        /// id로 모토로라 방식의 데이터를 가져오는 메서드
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stbit">시작비트</param>
        /// <param name="length">길이</param>
        /// <param name="isLogging">로깅 여부(기본 true)</param>
        /// <returns></returns>
        public int GetBMSData_int(string id, int stbit, int length, bool isLogging = true)
        {  
            try
            {
                if (bmsList.ContainsKey(id))
                {
                    var data = bmsList[id].Data.Replace(" ", "");
                    var ret = string.Join(String.Empty, data.Select(c => Convert.ToString(Convert.ToInt64(c.ToString(), 16), 2).PadLeft(4, '0')));
                    var arr = ret.ToArray();
                    var rtv = "";
                    for (int i = stbit; i < stbit + length; i++)
                    {
                        rtv += arr[i];
                    }
                    var toint = Convert.ToInt32(rtv, 2);                     
                    return toint;

                }
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Fail, "DetailDataSave", ex);
            }
            return 0;
        }

        //public List<string> _17FE009C = new List<string>();
        public List<string> _635List = new List<string>();
        
        /// <summary>
        /// rx data threading
        /// </summary>
        /// <param name="theMsg"></param>
        /// <param name="itsTimeStamp"></param>
        private void ProcessMessage1(TPCANMsgFD theMsg, TPCANTimestampFD itsTimeStamp)
        {
            lock (m_LastMsgsList1.SyncRoot)
            {
                var msgStatus = new MessageStatus(theMsg, itsTimeStamp, this.bmsList.Count);

                ////특정시점에만 들어오는 바이트를 직접 얻을 때, 아래와 같이 필터를 건다.
                ////dtc 같은 것 읽을 때
                //if (msgStatus.IdString == "17FE009Ch")
                //{
                //    _17FE009C.Add(msgStatus.DataString);
                //}

                if (msgStatus.IdString == "635h")
                {
                    _635List.Add(msgStatus.DataString);
                }

                foreach (MessageStatus msg in m_LastMsgsList1)
                {
                    if ((msg.CANMsg.ID == theMsg.ID) && (msg.CANMsg.MSGTYPE == theMsg.MSGTYPE))
                    {
                        msg.Update(theMsg, itsTimeStamp);
                        return;
                    }
                }
                InsertMsgEntry1(theMsg, itsTimeStamp);
            }
        }


        #region methods
        public static string Reverse(string str)
        {
            char[] charArr = str.ToCharArray();
            Array.Reverse(charArr);
            return new string(charArr);
        }

        public static string LittEndian(string str)
        {
            long number = Convert.ToInt64(str, 16);
            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            foreach (byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }
        

        public int ReverseDataAtIntelType(string data, int stbit, int length)
        {
            var ret = string.Join(String.Empty, data.Select(c => Convert.ToString(Convert.ToInt64(c.ToString(), 16), 2).PadLeft(4, '0')));

            //원본 64개
            var arr = ret.ToArray();
            //7654 3210


            var rtv = "";
            for (int j = 7; j < arr.Length; j = j + 8)
            {
                //7,15,23(8차이)
                int innerCnt = 0;
                for (int i = j; ; i--)
                {
                    if (innerCnt == 8)
                        break;

                    rtv += arr[i];
                    innerCnt++;
                }
            }

            var setStLenVal = "";
            var SetArr = rtv.ToArray();
            for (int i = stbit; i < stbit + length; i++)
            {
                setStLenVal += SetArr[i];
            }
            var rvsArr = setStLenVal.ToArray();
            Array.Reverse(rvsArr);

            var resultStr = "";
            foreach (char a in rvsArr)
            {
                resultStr += a;
            }

            //8개짤라서 뒤에다 옴기자
            return Convert.ToInt32(resultStr, 2);
        }
        #endregion


        #region PCAN

        private delegate void ReadDelegateHandler();
        public System.Collections.ArrayList m_LastMsgsList1;
        private ReadDelegateHandler m_ReadDelegate1;
        private System.Threading.AutoResetEvent m_ReceiveEvent1;
        private System.Threading.Thread m_ReadThread1;
        private string pcanHandleName1;
        public Dictionary<string, ReceiveType> bmsList = new Dictionary<string, ReceiveType>();
        private TPCANHandle[] m_HandlesArray;
        private bool m_IsFD;
        private TPCANHandle m_PcanHandle1;
        private TPCANBaudrate m_Baudrate;
        private TPCANType m_HwType;

        private void InitializePCANComponents()
        {
            // Creates the list for received messages
            //
            m_LastMsgsList1 = new System.Collections.ArrayList();
            m_ReadDelegate1 = new ReadDelegateHandler(ReadMessages1);
            m_ReceiveEvent1 = new System.Threading.AutoResetEvent(false);

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
        /// 
        /// </summary>
        /// <param name="id">보낼 ID(hex string)</param>
        /// <param name="bts">보낼 byte array</param>
        /// <param name="length">byte array의 length를 명시적 표기</param>
        public void SendToCAN(string id, byte[] bts,int length = 8)
        {
            TPCANMsg CANMsg = new TPCANMsg();
            CANMsg.DATA = new byte[length];
            
            CANMsg.ID = Convert.ToUInt32(id, 16);//ID입력
            CANMsg.LEN = Convert.ToByte(length);//DLC
            if (id.Length == 8)
                CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_EXTENDED;
            else   
                CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;//스탠다드 고정
            

            for (int i = 0; i < bts.Length; i++)
            {
                CANMsg.DATA[i] = bts[i];
            }

            PCANBasic.Write(m_PcanHandle1, ref CANMsg);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="can"></param>
        public void SendToCAN(CanMsg can)
        {
            TPCANMsg CANMsg = new TPCANMsg();
            CANMsg.DATA = can.data;
            CANMsg.ID = can.id;
            CANMsg.LEN = Convert.ToByte(can.length);
            PCANBasic.Write(m_PcanHandle1, ref CANMsg);
        }

        private void ReadMessages1()
        {
            TPCANStatus stsResult;

            do
            {
                stsResult = m_IsFD ? ReadMessageFD1() : ReadMessage1();
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

        private TPCANStatus ReadMessage1()
        {
            TPCANMsg CANMsg;
            TPCANTimestamp CANTimeStamp;
            TPCANStatus stsResult;

            // We execute the "Read" function of the PCANBasic                
            //
            stsResult = PCANBasic.Read(m_PcanHandle1, out CANMsg, out CANTimeStamp);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                ProcessMessage1(CANMsg, CANTimeStamp);

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

                //LogState(LogType.Info, "CAN1 msg - " + msgStsCurrentMsg.IdString + " : " + msgStsCurrentMsg.DataString);
            }
        }

        /// <summary>
        /// PCAN 핸들을 고정으로 박아버린다.
        /// </summary>
        private void SetPCANHandle1()
        {
            bool bNonPnP;
            var strTemp = pcanHandleName1;
            if (strTemp == null)
                return;

            mw.LogState(LogType.Info, "PCAN Port Set : " + strTemp);

            strTemp = strTemp.Substring(strTemp.IndexOf('(') + 1, 3);

            strTemp = strTemp.Replace('h', ' ').Trim(' ');

            m_PcanHandle1 = Convert.ToUInt16(strTemp, 16);
            bNonPnP = m_PcanHandle1 <= PCANBasic.PCAN_DNGBUS1;
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

            //for (int i = 0; i < 20; i++)
            //{
            //iBuffer = (UInt32)i;
            iBuffer = PCANBasic.PCAN_PARAMETER_OFF;
            stsResult = PCANBasic.SetValue(m_PcanHandle1, TPCANParameter.PCAN_BUSOFF_AUTORESET, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                mw.LogState(LogType.Fail, GetFormatedError(stsResult));
            //}
        }

        private void SetConnectionStatus1(bool bConnected)
        {
            if (!bConnected)
            {
                bool bNonPnP;
                string strTemp;

                strTemp = pcanHandleName1;
                strTemp = strTemp.Substring(strTemp.IndexOf('(') + 1, 3);
                strTemp = strTemp.Replace('h', ' ').Trim(' ');
                m_PcanHandle1 = Convert.ToUInt16(strTemp, 16);
                bNonPnP = m_PcanHandle1 <= PCANBasic.PCAN_DNGBUS1;
            }
        }

        public bool isAlive = false;

        System.Windows.Forms.Timer tmrDisplay1 = new System.Windows.Forms.Timer();

        /// <summary>
        /// BMS(CAN) 초기화부분
        /// </summary>
        private void SetPCAN_BMS1(string fname)
        {
            UInt32 iBuffer = PCANBasic.LOG_FUNCTION_ALL;
            PCANBasic.SetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_LOG_CONFIGURE, ref iBuffer, sizeof(UInt32));

            pcanHandleName1 = fname;
            TPCANStatus stsResult;
            SetPCANHandle1();
            m_Baudrate = TPCANBaudrate.PCAN_BAUD_500K;
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
                    mw.LogState(LogType.Fail, "PCAN_BMS_" + GetFormatedError(stsResult));
                }
                else
                {
                    mw.LogState(LogType.Info, "The bitrate being used is different than the given one");
                    stsResult = TPCANStatus.PCAN_ERROR_OK;
                }

                PCANBasic.Uninitialize(m_PcanHandle1);
            }
            else
            {
                isAlive = true;
                ConfigureTraceFile1(); //Prepares the PCAN-Basic's PCAN-Trace file

                System.Threading.ThreadStart threadDelegate = new System.Threading.ThreadStart(this.CANReadThreadFunc1);
                if (m_ReadThread1 != null && m_ReadThread1.IsAlive)
                    m_ReadThread1.Abort();

                m_ReadThread1 = new System.Threading.Thread(threadDelegate);
                m_ReadThread1.IsBackground = true;
                m_ReadThread1.Start();

                tmrDisplayThread = new Thread(() =>
                    {
                        while (true)
                        {
                            DisplayMessages1();
                            Thread.Sleep(50);
                        }
                    });
                tmrDisplayThread.Start();
                //tmrDisplay1.Stop();
                //tmrDisplay1.Interval = 50;
                //tmrDisplay1.Tick += tmrDisplay_Tick1;
                //tmrDisplay1.Start();


                mw.LogState(LogType.Info, "CAN1 Timer Started", null, true, false);

                mw.LogState(LogType.Success, "PCAN1 Connected", null, true, false);

            }
            SetConnectionStatus1(stsResult == TPCANStatus.PCAN_ERROR_OK);

        }

        Thread tmrDisplayThread;
        private void tmrDisplay_Tick1(object sender, EventArgs e)
        {
            DisplayMessages1();
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
                mw.LogState(LogType.Info, GetFormatedError(stsResult));
                return;
            }

            while (true)
            {
                if (m_ReceiveEvent1.WaitOne(50))
                    mw.Dispatcher.Invoke(m_ReadDelegate1);
            }
        }


        #endregion
    }
}
