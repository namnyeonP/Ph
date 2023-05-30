using Renault_BT6.윈도우;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Renault_BT6.모듈
{
    public class CPLC
    {
        MainWindow mw;

        public string PLC_RECV_ADDRESS = "BA00";
        public string PLC_SEND_ADDRESS = "BA10";
        public string PLC_BCRS_ADDRESS = "WA0";
        public int PLC_LOGICAL_NUMBER = 0;

        int timeout = 5;

        public CPLC(MainWindow mw,
            string _PLC_RECV_ADDRESS,
            string _PLC_SEND_ADDRESS,
            string _PLC_BCRS_ADDRESS,
            int _PLC_LOGICAL_NUMBER = 0)
        {
            try
            {
                this.mw = mw;
                this.PLC_RECV_ADDRESS = _PLC_RECV_ADDRESS;
                this.PLC_SEND_ADDRESS = _PLC_SEND_ADDRESS;
                this.PLC_BCRS_ADDRESS = _PLC_BCRS_ADDRESS;
                this.PLC_LOGICAL_NUMBER = _PLC_LOGICAL_NUMBER;

                mw.LogState(LogType.Info, "PLC_RECV_ADDRESS:" + PLC_RECV_ADDRESS +
                    "/PLC_SEND_ADDRESS:" + PLC_SEND_ADDRESS +
                    "/PLC_BCRS_ADDRESS:" + PLC_BCRS_ADDRESS +
                    "/PLC_LOGICAL_NUMBER:" + PLC_LOGICAL_NUMBER.ToString());

                if (PLC_RECV_ADDRESS == "")
                    return;

                PLC_DATA.Add(PLC_RECV_ADDRESS, new bool[16]);
                PLC_DATA.Add(PLC_SEND_ADDRESS, new bool[16]);

                StartConnect();

                mw.Dispatcher.BeginInvoke(new Action(() =>
                {
                    rt = new DeviceCheckWindow(mw);
                    rt.m_nWndType = 1;
                    rt.Height += 100;
                    rt.Width += 400;
                    rt.maintitle.FontSize += 30;
                    rt.malb.FontSize += 10;
                    rt.malb.Content = "!!!";
                    rt.maintitle.Content = "EMERGENCY STOPPED!!";
                    rt.reason.FontSize += 13;
                    rt.reason.Content = "Device is Stopped";
                    rt.okbt.FontSize += 13;
                }));
            }
            catch (Exception ex)
            {
                mw.LogState(LogType.Info, "isTesting ON");
            }

        }

        public Dictionary<string, bool[]> PLC_DATA = new Dictionary<string, bool[]>();


        int PLC_SendData = 0;
        int PLC_RecvData = 0;

        public bool plc_isStart = false;
        public bool plc_Result_recv_ok = false;
        public bool plc_isPause = false;
        public bool plc_Pause_Continue = false;
        public bool plc_isStop = false;
        //public bool plc_model_4P8S = false;
        //public bool plc_model_4P8S_REV = false;
        //public bool plc_model_4P7S = false;
        //public bool plc_model_3P10S = false;
        //public bool plc_model_3P8S = false;
        //public bool plc_model_3P10S_REV = false;
        public bool plc_isAlive = false;
        public bool plc_isJigDown = false;

        public string plc_BCR = "";
        public bool isAlive { get; set; }


        ActUtlTypeLib.ActUtlType actType;
        List<string> szDeviceList = new List<string>();
        System.Windows.Forms.Timer recvTimer;
        Thread aliveThread, recvThread;

        #region Commands
        public bool isTesting_Flag = false;

        bool isOncePause = false;

        public void isTesting(bool on)
        {
            if (!isAlive)
                return;

            if (on)
            {
                isTesting_Flag = true;
                PLC_SendData += 1;
                mw.LogState(LogType.PLC_SEND, "isTesting ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                isTesting_Flag = false;
                PLC_SendData -= 1;
                mw.LogState(LogType.PLC_SEND, "isTesting OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
        }

        bool Result_Pass_Flag = false;
        public void Result_Pass(bool on)
        {
            if (!isAlive)
                return;

            if (on)
            {
                Result_Pass_Flag = true;
                PLC_SendData += 4;
                mw.LogState(LogType.PLC_SEND, "Result_Pass ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                Result_Pass_Flag = false;
                PLC_SendData -= 4;
                mw.LogState(LogType.PLC_SEND, "Result_Pass OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }

        }

        bool Result_NG_Flag = false;
        public void Result_NG(bool on)
        {
            if (!isAlive)
                return;

            if (on)
            {
                Result_NG_Flag = true;
                PLC_SendData += 8;
                mw.LogState(LogType.PLC_SEND, "Result_NG ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                Result_NG_Flag = false;
                PLC_SendData -= 8;
                mw.LogState(LogType.PLC_SEND, "Result_NG OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            //actType.WriteDeviceBlock(str_pc_NG, 1, 1);
        }

        public void CyclerAlarm(bool on)
        {
            if (!isAlive)
                return;

            if (on)
            {
                PLC_SendData += 512;
                mw.LogState(LogType.PLC_SEND, "CyclerAlarm");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                PLC_SendData -= 512;
                mw.LogState(LogType.PLC_SEND, "CyclerAlarm");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            //actType.WriteDeviceBlock(str_pc_Stop, 1, 1);
        }

        bool test_Stop_Flag { get; set; }
        public void test_Stop(bool on)
        {
            if (!isAlive)
                return;

            if (on)
            {
                test_Stop_Flag = true;
                PLC_SendData += 16;
                mw.LogState(LogType.PLC_SEND, "testStop_ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                test_Stop_Flag = false;
                PLC_SendData -= 16;
                mw.LogState(LogType.PLC_SEND, "testStop_OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            //actType.WriteDeviceBlock(str_pc_Stop, 1, 1);
        }


        public int AliveCheck(bool on)
        {
            int ret = 0;

            if (on)
            {
                PLC_SendData += 32768;
                mw.LogState(LogType.PLC_SEND, "PLC Alive On");
                ret = actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                PLC_SendData -= 32768;
                mw.LogState(LogType.PLC_SEND, "PLC Alive Off");
                ret = actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }

            PLC_SendData -= 32768;

            return ret;
        }

        //public void MES_NG()
        //{
        //    if (!isAlive)
        //        return;

        //    PLC_SendData += 2;
        //    mw.LogState(LogType.PLC_SEND, "MES_NG");
        //    actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
        //}


        #endregion


        public  void DisConnect()
        {
            try
            {
                isAlive = false;
                actType.Close();
            }
            catch
            {

            }
        }

        private void StartConnect()
        {
            actType = new ActUtlTypeLib.ActUtlType();
            actType.ActLogicalStationNumber = PLC_LOGICAL_NUMBER;

            int lSize = 1;
            int rtv = -1;
            int[] data = new int[lSize];
   
            if (actType.Open().Equals(0))
            {
                aliveThread = new Thread(() =>
                {
                    while (true)
                    {
                        PLC_SendData += 32768;
                        rtv = actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
                        if (rtv == 0)
                            isAlive = true;
                        else
                            isAlive = false;

                        Thread.Sleep(5000);

                        PLC_SendData -= 32768;

                        // 0 아니면 연결 상태 중지
                        rtv = actType.ReadDeviceBlock(PLC_SEND_ADDRESS, lSize, out data[0]);
                        if (rtv == 0)
                            isAlive = true;
                        else
                            isAlive = false;

                        Thread.Sleep(5000);
                    }
                });

                aliveThread.Start();

                mw.LogState(LogType.Info, "PLC Success");
                recvThread = new Thread(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(10);
                        recvTimer_Tick();
                    }
                });
                recvThread.Start();

                mw.LogState(LogType.Info, "Start PLC Alive Msg");

            }
            else
            {
                mw.LogState(LogType.Fail, "PLC OPEN FAIL!!");
                Device.Relay.On(RELAY.LIGHT_RED);
                
                isAlive = false;
            }
        }

        bool GetActtype(string device)
        {
            if (!isAlive)
                return false;

            int lSize = 1;
            bool rtv = false;
            int[] data = new int[lSize];
            actType.ReadDeviceBlock(device, lSize, out data[0]);

            string plcRecv = Convert.ToString(data[0], 2);
            int size = plcRecv.Length;
            while (size < 16)
            {
                plcRecv = "0" + plcRecv;

                size++;
            }

            int x = 0;
            for (int i = 15; i >= 0; i--)
            {
                if (plcRecv[i].Equals('0'))
                {
                    PLC_DATA[PLC_RECV_ADDRESS][x] = false;
                }
                else
                {
                    PLC_DATA[PLC_RECV_ADDRESS][x] = true;
                }
                x++;
            }

            //actType.ReadDeviceBlock("BA10", lSize, out data[0]);
            //if (data[0] > 32768)
            //    PLC_DATA[PLC_RECV_ADDRESS][15] = false;
            //else
            //    PLC_DATA[PLC_RECV_ADDRESS][15] = true;

            return rtv;
        }

        public byte[] getbytes1(string binary)
        {
            var list = new List<byte>();
            for (int i = 0; i < binary.Length; i = i + 8)
            {
                string t = binary.Substring(i, 8);
                list.Add(Convert.ToByte(t, 2));
            }
            return new byte[] { list[1], list[0] };
        }

        public string GetBCRS(string addr, int length)
        {
            ///<summary>
            /// modify by sjh 210812. Tact Time
            /// </summary>

            int[] val = new int[length];
            var rtval = "";
            int j = 0;
            var hex = j.ToString("X1");
            actType.ReadDeviceBlock(addr + hex, 16, out val[0]); //한번에 리딩 한다

            for (int i = 0; i < length; i++)
            {
                var plcRecv = Convert.ToString(val[i], 2);
                int size = plcRecv.Length;
                while (size < 16)
                {
                    plcRecv = "0" + plcRecv;

                    size++;
                }

                var data = this.getbytes1(plcRecv);
                rtval += Encoding.ASCII.GetString(data);
            }

            return rtval;

            /*
            int val = 0;
            var rtval = "";
            for (int i = 0; i < length; i++)
            {
                var hex = i.ToString("X1");
                actType.ReadDeviceBlock(addr + hex, 1, out val);

                var plcRecv = Convert.ToString(val, 2);
                int size = plcRecv.Length;
                while (size < 16)
                {
                    plcRecv = "0" + plcRecv;

                    size++;
                }

                var data = this.getbytes1(plcRecv);
                rtval += Encoding.ASCII.GetString(data);
            }
            return rtval;
            */
        }


        public void ModeChange(bool modeDummy = false)
        {
            return;
            if (modeDummy)
            {
                mw.AutoMode_change("dummy");
            }
            else
            {
                /*
                if (plc_model_4P8S)
                {
                    mw.AutoMode_change("type1lb");
                }
                else if (plc_model_4P8S_REV)
                {
                    mw.AutoMode_change("type2lb");
                }
                else if (plc_model_4P7S)
                {
                    mw.AutoMode_change("type3lb");
                }
                else if (plc_model_3P8S)
                {
                    mw.AutoMode_change("type4lb");
                }
                else if (plc_model_3P10S)
                {
                    mw.AutoMode_change("type5lb");
                }
                else if (plc_model_3P10S_REV)
                {
                    mw.AutoMode_change("type6lb");
                }
                */

                //int lSize = 1;
                ////bool rtv = false;
                //int[] data = new int[lSize];
                //actType.ReadDeviceBlock(PLC_RECV_ADDRESS, lSize, out data[0]);

                //var temp = data[0].ToString();

                ////180623
                //var bin7thDataStr = Convert.ToString(data[0], 2);

                //while (bin7thDataStr.Length < 16)
                //{
                //    bin7thDataStr = "0" + bin7thDataStr;
                //}

                //var bin7thData = bin7thDataStr.ToArray();

                //if (bin7thData[3] == '1') // 0001 0000 0000 0000, gen
                //{
                //    mw.AutoMode_change("type2lb");
                //}
                //else if (bin7thData[2] == '1') // 0010 0000 0000 0000, tob
                //{
                //    mw.AutoMode_change("type1lb");
                //}
                //else if (bin7thData[1] == '1') // 0100 0000 0000 0000, mqb
                //{
                //    mw.AutoMode_change("type3lb");
                //}
            }
        }

        void recvTimer_Tick()
        {
            try
            {
                GetActtype(PLC_RECV_ADDRESS);

                #region Start and  BCR Read

                if (plc_isStart && PLC_DATA[PLC_RECV_ADDRESS][0] == false)
                {
                    mw.LogState(LogType.PLC_RECV, "PLC START OFF.");
                    plc_isStart = PLC_DATA[PLC_RECV_ADDRESS][0] = false;
                }
                plc_isStart = PLC_DATA[PLC_RECV_ADDRESS][0];

                if (plc_isStart)
                {
                    mw.LogState(LogType.PLC_RECV, "PLC START ON.");

                    if (mw.IsManual)
                    {
                        mw.LogState(LogType.Info, "Please Change mode !!!!! Device is ready to start automatically..");
                        Thread.Sleep(5000);
                    }
                    //else if (Device.Cycler.ChgDisControl.CyclerIsAlarm() == true || Device.Cycler.ChgDisControl.GetCyclerAlarmStaus() == true)
                    //{
                    //    mw.LogState(LogType.Info, "Cannot start, Check the cycler alaram status");
                    //    Thread.Sleep(5000);
                    //}
                    else
                    {
                        if (mw.autoModeThread != null && mw.autoModeThread.IsAlive)
                        {
                            mw.Dispatcher.Invoke(new Action(() =>
                            {
                                mw.LogState(LogType.Info, "ALREADY_STARTED_BARCODE - " + mw.lotTb.Text);
                            }));
                        }
                        else
                        {
                            // ModeChange();

                            Thread.Sleep(100);       // Myunghwan choi,  add sleep 100 ms for stable BCR : verification

                            #region BCR Barcode Read
                            tempBarcode = GetBCRS(PLC_BCRS_ADDRESS, 15);
                            mw.LogState(LogType.PLC_RECV, "Origin BCR - " + tempBarcode);
                            tempBarcode = tempBarcode.Replace("\0", "").Replace("\n", "").Replace("\r", "").Replace(" ", "");
                            mw.LogState(LogType.PLC_RECV, "Parsed BCR - " + tempBarcode);

                            mw.Dispatcher.Invoke(new Action(() =>
                            {
                                mw.lotTb.Text = tempBarcode;
                            }));
                            #endregion

                            #region Test Start
                            isTesting(true);

                            mw.LogState(LogType.Info, "START_TO_BARCODE - " + tempBarcode);
                            
                            mw.Dispatcher.Invoke(new Action(() =>
                            {
                                mw.AutoMode();
                            }));
                           
                            tempBarcode = "";
                            #endregion
                        }
                    }
                }
                #endregion

                #region Result Recv OK
                plc_Result_recv_ok = PLC_DATA[PLC_RECV_ADDRESS][2];
                #endregion

                #region Pause
                if (plc_isPause && PLC_DATA[PLC_RECV_ADDRESS][4] == false)
                {
                    mw.LogState(LogType.PLC_RECV, "PLC PAUSE OFF.");
                    plc_isPause = PLC_DATA[PLC_RECV_ADDRESS][4] = false;
                }

                plc_isPause = PLC_DATA[PLC_RECV_ADDRESS][4];

                if (plc_isPause)
                {
                    if (mw.autoModeThread != null && mw.autoModeThread.IsAlive)
                    {
                        if (isOncePause == false)
                        {
                            isOncePause = true;

                            mw.Ispause = true;
                            mw.LogState(LogType.PLC_RECV, "PAUSE");

                            if (isTesting_Flag)
                                isTesting(false);

                            if (!test_Stop_Flag)
                                test_Stop(true);
                        }
                    }
                }
                #endregion

                #region PauseContinue
                if (plc_Pause_Continue && PLC_DATA[PLC_RECV_ADDRESS][5] == false)
                {
                    mw.LogState(LogType.PLC_RECV, "PLC PAUSE CONTINUE OFF.");
                    plc_Pause_Continue = PLC_DATA[PLC_RECV_ADDRESS][5] = false;
                }

                plc_Pause_Continue = PLC_DATA[PLC_RECV_ADDRESS][5];

                if (plc_Pause_Continue)
                {
                    if (mw.autoModeThread != null && mw.autoModeThread.IsAlive)
                    {
                        //isOncePause = false;
                        //mw.Ispause = false;

                        mw.LogState(LogType.PLC_RECV, "CONTINUE");

                        if (test_Stop_Flag)
                            test_Stop(false);

                        if (!isTesting_Flag)
                            isTesting(true);

                        isOncePause = false;
                        mw.Ispause = false;
                    }
                }
                #endregion

                GetActtype(PLC_RECV_ADDRESS);

                #region PLC Stop
                plc_isStop = PLC_DATA[PLC_RECV_ADDRESS][6];
                if (plc_isStop)
                {
                    if (mw.autoModeThread != null && mw.autoModeThread.IsAlive)
                    {
                        mw.LogState(LogType.PLC_RECV, "STOP");
                        mw.StopAuto();

                        try
                        {
                            mw.Dispatcher.Invoke(new Action(() =>
                            {
                                if (mw.mWindowPreCheck != null && mw.mWindowPreCheck.IsVisible == true)
                                {
                                    mw.mWindowPreCheck.Hide();
                                }

                                //if (!rt.IsActive)
                                if (rt != null & rt.IsVisible == false)
                                {
                                    rt.Show();
                                }
                            }));
                        }
                        catch (Exception ex)
                        {
                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                rt = new DeviceCheckWindow(mw);
                                rt.Height += 100;
                                rt.Width += 400;
                                rt.maintitle.FontSize += 30;
                                rt.malb.FontSize += 10;
                                rt.malb.Content = "!!!";
                                rt.maintitle.Content = "EMERGENCY STOPPED!!";
                                rt.reason.FontSize += 13;
                                rt.reason.Content = "Device is Stopped";
                                rt.okbt.FontSize += 13;
                            }));
                        }
                    }

                    //plc_isStop = PLC_DATA[PLC_RECV_ADDRESS][6] = false;
                }
                #endregion

                #region 사용 안함
                /*
                if (plc_isJigDown && PLC_DATA[PLC_RECV_ADDRESS][7] == false)
                {
                    mw.LogState(LogType.PLC_RECV, "PLC JIG REMOVED");
                    plc_isJigDown = PLC_DATA[PLC_RECV_ADDRESS][7] = false;
                }
                plc_isJigDown = PLC_DATA[PLC_RECV_ADDRESS][7];

                this.plc_model_4P8S = PLC_DATA[PLC_RECV_ADDRESS][10];
                this.plc_model_4P8S_REV = PLC_DATA[PLC_RECV_ADDRESS][11];
                this.plc_model_4P7S = PLC_DATA[PLC_RECV_ADDRESS][12];
                this.plc_model_3P10S = PLC_DATA[PLC_RECV_ADDRESS][13];
                this.plc_model_3P8S = PLC_DATA[PLC_RECV_ADDRESS][14];
                this.plc_model_3P10S_REV = PLC_DATA[PLC_RECV_ADDRESS][15];
                plc_isAlive = PLC_DATA[PLC_RECV_ADDRESS][15];

                */
                #endregion
            }
            catch
            {

            }
        }

        DeviceCheckWindow rt = null;
        public bool isSuccess { get; set; }

        /// <summary>
        /// 모든테스트 끝일때 날려
        /// 테스트 시작시 isSuc false해
        /// </summary>
        /// <param name="bbo"></param>
        /// <returns></returns>
        public bool TestResult(bool bbo)
        {
            bool rtv = false;

            if (!isAlive)
                return rtv;

            try
            {

                if (isTesting_Flag)
                    isTesting(false);

                if (bbo)
                {
                    Result_Pass(true);

                    isSuccess = rtv = true; 

                    int cnt = 0;
                    while (true)
                    {
                        cnt += 100;
                        if (cnt == timeout * 1000)
                            break;
                        Thread.Sleep(100);

                        if (plc_Result_recv_ok)
                        {
                            mw.LogState(LogType.Success, "Success To Result Flag at " + cnt.ToString() + "msc");
                            Result_Pass(false);

                            return rtv;
                        }
                    }
                    mw.LogState(LogType.Fail, "Fail To Result Flag in Time out 5sec");
                    Result_Pass(false);

                    return rtv;
                }
                else
                {
                    Result_NG(true);
                    isSuccess = false;
                    rtv = true;
                     
                    int cnt = 0;
                    while (true)
                    {
                        cnt += 100;
                        if (cnt == timeout * 1000)
                            break;
                        Thread.Sleep(100);

                        if (plc_Result_recv_ok)
                        {
                            mw.LogState(LogType.Success, "Success To Result Flag at " + cnt.ToString() + "msc");
                            Result_NG(false);

                            return rtv;
                        }
                    }
                    mw.LogState(LogType.Fail, "Fail To Result Flag in Time out 5sec");
                    Result_NG(false);

                    return rtv;
                }

            }
            catch (Exception ec)
            {
                mw.LogState(LogType.Info, "PLC", ec);
            }
            return rtv;
        }

        /// 시나리오 삭제로 인한 착공실패시 NG로 끝
        /// <summary>
        /// 착공실패		<<	BA11
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        //public bool StartJobResult(bool isSuccess)
        //{
        //    bool rtv = false;

        //    if (!isAlive)
        //        return rtv;

        //    try
        //    { 
        //        if (!isSuccess)                
        //        {   //mes ng ba11
        //            MES_NG();
        //            isSuc = false;

        //            rtv = true;

        //            mw.LogState(LogType.PLC_SEND, "START_JOB_FAIL");
        //            return rtv;
        //        }
        //    }
        //    catch (Exception ec)
        //    {
        //        mw.LogState(LogType.Info, "PLC", ec);
        //    }

        //    return rtv;
        //}

        string tempBarcode = "";
    }
}
