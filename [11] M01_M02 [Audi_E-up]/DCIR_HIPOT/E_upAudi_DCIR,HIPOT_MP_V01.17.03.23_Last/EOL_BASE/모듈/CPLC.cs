using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using EOL_BASE.윈도우;

namespace EOL_BASE.모듈
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
        }

        public Dictionary<string, bool[]> PLC_DATA = new Dictionary<string, bool[]>();


        int PLC_SendData = 0;
        int PLC_RecvData = 0;

        public bool plc_isStart = false;
        public bool plc_Result_recv_ok = false;
        public bool plc_isPause = false;
        public bool plc_Pause_Continue = false;
        public bool plc_isStop = false;
        public bool plc_model_A = false;
        public bool plc_model_B = false;
        public bool plc_model_C = false;
        public bool plc_model_D = false;
        public bool plc_model_E = false;
        public bool plc_model_F = false;
        public bool plc_isAlive = false;
        public bool plc_isJigDown = false;
        public bool plc_lv_retry_recv_ok = false;

        public string plc_BCR = "";
        public bool isAlive = false;

        ActUtlTypeLib.ActUtlType actType;
        List<string> szDeviceList = new List<string>();
        System.Windows.Forms.Timer recvTimer;
        Thread aliveThread, recvThread;
        string plc_write_fail = " - PLC_WRITE_FAIL";

        #region Commands
        public bool isTesting_Flag = false;

        bool isOncePause = false;
        bool isOnceStop = false;
        public void isTesting(bool on)
        {
            if (on)
            {
                isTesting_Flag = true;
                PLC_SendData += 1;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "isTesting ON"+ plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "isTesting ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                isTesting_Flag = false;
                PLC_SendData -= 1;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "isTesting OFF" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "isTesting OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
        }

        bool Result_Pass_Flag = false;
        public void Result_Pass(bool on)
        {
            if (on)
            {
                Result_Pass_Flag = true;
                PLC_SendData += 4;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "Result_Pass ON" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "Result_Pass ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                Result_Pass_Flag = false;
                PLC_SendData -= 4;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "Result_Pass OFF" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "Result_Pass OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }

        }

        bool Result_NG_Flag = false;
        public void Result_NG(bool on)
        {
            if (on)
            {
                Result_NG_Flag = true;
                PLC_SendData += 8;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "Result_NG ON" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "Result_NG ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                Result_NG_Flag = false;
                PLC_SendData -= 8;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "Result_NG OFF" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "Result_NG OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            //actType.WriteDeviceBlock(str_pc_NG, 1, 1);
        }

        public bool LV_Retry_Flag = false;
        public void LV_Retry(bool on)
        { 
            if (on)
            {
                LV_Retry_Flag = true;
                PLC_SendData += 256;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "LV_Retry ON" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "LV_Retry ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                LV_Retry_Flag = false;
                PLC_SendData -= 256;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "LV_Retry OFF" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "LV_Retry OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            //actType.WriteDeviceBlock(str_pc_NG, 1, 1);
        }

        public void CyclerAlarm(bool on)
        {
            if (on)
            {
                PLC_SendData += 512;

				if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "CyclerAlarm ON" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "CyclerAlarm ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                PLC_SendData -= 512;

				if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "CyclerAlarm OFF" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "CyclerAlarm OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            //actType.WriteDeviceBlock(str_pc_Stop, 1, 1);
        }

        bool test_Stop_Flag = false;
        public void test_Stop(bool on)
        { 
            if (on)
            {
                test_Stop_Flag = true;
                PLC_SendData += 16;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "Test_Stop ON" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "Test_Stop ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                test_Stop_Flag = false;
                PLC_SendData -= 16;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "Test_Stop OFF" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "Test_Stop OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            //actType.WriteDeviceBlock(str_pc_Stop, 1, 1);
        }

        public bool jig_Down_Flag = false;
        public void Jig_Down(bool on)
        { 
            if (on)
            {
                jig_Down_Flag = true;
                PLC_SendData += 128;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "Jig_Down ON" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "Jig_Down ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                jig_Down_Flag = false;
                PLC_SendData -= 128;

                if (!isAlive)
                {
                    mw.LogState(LogType.PLC_SEND, "Jig_Down OFF" + plc_write_fail);
                    return;
                }

                mw.LogState(LogType.PLC_SEND, "Jig_Down OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            //actType.WriteDeviceBlock(str_pc_Stop, 1, 1);
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

        Thread instanceTh1, instanceTh2;
        private void StartConnect()
        {
            actType = new ActUtlTypeLib.ActUtlType();
            actType.ActLogicalStationNumber = PLC_LOGICAL_NUMBER;
            if (actType.Open().Equals(0))
            {
                int timeoutTick = 1000;//msec

                aliveThread = new Thread(() =>
                {
                    while (true)
                    {
                        #region WriteDeviceBlock
                        PLC_SendData += 32768;

                        int rtv = -1; 
                        var instanceTh1 = new Thread(() =>
                        {
                            try
                            {
                                rtv = actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
                            }
                            catch (Exception)
                            {
                                rtv = -1;
                            } 
                        });
                        instanceTh1.Start();

                        #region response timeout
                        int cnt = 0;
                        // 2020.02.11 Noah Choi PLC 연결이 도중에 끊기는 순간 프로그램 터지는 증상이 발생
                        // Try-Catch문 추가
                        try
                        {
                            while (rtv == -1)
                            {
                                Thread.Sleep(100);
                                cnt += 100;
                                if (cnt >= timeoutTick)
                                {
                                    mw.LogState(LogType.DEVICE_CHECK, "PLC_RESPONSE_FAIL_IN_" + (timeoutTick / 1000).ToString() + "SEC");
                                    isAlive = false;
                                    mw.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        mw.contBt_plc.Background = System.Windows.Media.Brushes.Red;
                                    }));
                                    break;
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            mw.LogState(LogType.Fail, "WriteDeviceBlock_response timeout", ex);
                        }


                        #endregion
                        instanceTh1 = null;

                        if (rtv == 0)
                        {
                            isAlive = true;

                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_plc.Background = System.Windows.Media.Brushes.Green;
                            }));
                        }
                        else
                        {
                            isAlive = false;

                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_plc.Background = System.Windows.Media.Brushes.Red;
                            }));
                        }
                        #endregion

                        Thread.Sleep(5000);

                        #region ReadDeviceBlock
                         
                        PLC_SendData -= 32768;
                        rtv = -1;

                        int lSize = 1;
                        int[] data = new int[lSize];

                        var instanceTh2 = new Thread(() =>
                        {
                            try
                            {
                                rtv = actType.ReadDeviceBlock(PLC_SEND_ADDRESS, lSize, out data[0]);
                            }
                            catch (Exception)
                            {
                                rtv = -1;
                            } 
                        });
                        instanceTh2.Start();

                        #region response timeout
                        cnt = 0;

                        // 2020.02.11 Noah Choi PLC 연결이 도중에 끊기는 순간 프로그램 터지는 증상이 발생
                        // Try-Catch문 추가
                        try
                        {
                            while (rtv == -1)
                            {
                                Thread.Sleep(100);
                                cnt += 100;
                                if (cnt >= timeoutTick)
                                {
                                    mw.LogState(LogType.DEVICE_CHECK, "PLC_RESPONSE_FAIL_IN_" + (timeoutTick / 1000).ToString() + "SEC");
                                    isAlive = false;
                                    mw.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        mw.contBt_plc.Background = System.Windows.Media.Brushes.Red;
                                    }));
                                    break;
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            mw.LogState(LogType.Fail, "ReadDeviceBlock_response timeout", ex);
                        }

                        #endregion
                        instanceTh2 = null;


                        if (rtv == 0)
                        {
                            isAlive = true;

                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_plc.Background = System.Windows.Media.Brushes.Green;
                            }));
                        }
                        else
                        {
                            isAlive = false;

                            mw.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                mw.contBt_plc.Background = System.Windows.Media.Brushes.Red;
                            }));
                        }

                        #endregion

                        Thread.Sleep(5000);
                    }
                });
                aliveThread.Start();

                mw.LogState(LogType.DEVICE_CHECK, "PLC OPEN Success");
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
                mw.LogState(LogType.DEVICE_CHECK, "PLC OPEN FAIL!!");
                mw.Dispatcher.BeginInvoke(new Action(() =>
                {
                    mw.contBt_plc.Background = System.Windows.Media.Brushes.Red;
                    //mw.relays.RelayOn("IDO_0");
                    mw.tlamp.SetTLampInstrumentOff(true);
                }));
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
        }


        public void ModeChange()
        {
            mw.Dispatcher.Invoke(new Action(() =>
            {
                    //PLC_DATA[PLC_RECV_ADDRESS][10]=false;
                    //PLC_DATA[PLC_RECV_ADDRESS][11] == model.PLCBit_B &&
                    //PLC_DATA[PLC_RECV_ADDRESS][12] == model.PLCBit_C &&
                    //PLC_DATA[PLC_RECV_ADDRESS][13] == model.PLCBit_D &&
                    //PLC_DATA[PLC_RECV_ADDRESS][14] == model.PLCBit_E &&
                    //PLC_DATA[PLC_RECV_ADDRESS][15] == model.PLCBit_F)

                foreach (var model in mw.modelDataList)
                {
                    if (
                    PLC_DATA[PLC_RECV_ADDRESS][10] == model.PLCBit_A &&
                    PLC_DATA[PLC_RECV_ADDRESS][11] == model.PLCBit_B &&
                    PLC_DATA[PLC_RECV_ADDRESS][12] == model.PLCBit_C &&
                    PLC_DATA[PLC_RECV_ADDRESS][13] == model.PLCBit_D &&
                    PLC_DATA[PLC_RECV_ADDRESS][14] == model.PLCBit_E &&
                    PLC_DATA[PLC_RECV_ADDRESS][15] == model.PLCBit_F)
                    {
                        var modelName = model.ModelId.Replace("_", "-");
                        //mw.isPLC_MODE_CHABGED = true;
                        foreach (var rb in mw.modelNameGrid.Children)
                        {
                            var rbs = rb as RadioButton;
                            if (modelName == rbs.Content.ToString())
                            {
                                rbs.IsChecked = true;
                                //mw.stwthfshdshf.Start();
                                break;
                            }
                        }

                        //wait for ModelRb_Checked Event
                        //mw.isPLC_MODE_CHABGED = false;

                        //mw.SetViewModel_To_Event(model.ModelId);

                        //mw.LoadList();

                        //mw.testItemListDg.Items.Refresh();

                        break;
                    }
                    
                }
            }));
            Thread.Sleep(100); 
        }

        //200401 판정값을 설비로 보내기 위한 플래그
        public bool judgementFlag = false;

        void recvTimer_Tick()
        {
            GetActtype(PLC_RECV_ADDRESS);

            #region Start N BCRS

            if (plc_isStart && PLC_DATA[PLC_RECV_ADDRESS][0] == false)
            {
                mw.LogState(LogType.PLC_RECV, "PLC START OFF.");
                plc_isStart = PLC_DATA[PLC_RECV_ADDRESS][0] = false;
            }
            plc_isStart = PLC_DATA[PLC_RECV_ADDRESS][0];

            if (plc_isStart)
            {
                mw.LogState(LogType.PLC_RECV, "PLC START ON.");
                 
                if (mw.isManual)
                {
                    mw.LogState(LogType.Info, "Please change mode !!!!! Device is ready to start automayically..");
                    Thread.Sleep(5000); 
                }                  
                else if (mw.isDeviceFail)
                {
                    mw.LogState(LogType.Info, "Please check [DEVICE CHECK WINDOW] !!!!! [DEVICE CHECK WINDOW] is Activated NOW");
                    Thread.Sleep(5000); 
                }
                else if (mw.isCyclerFail)
                {
                    mw.LogState(LogType.Info, "Please check [CYCLER ALARM WINDOW] !!!!! [CYCLER ALARM WINDOW] is Activated NOW");
                    Thread.Sleep(5000);
                }
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
                        mw.StartTimer();

                        //200401 설비서 시작했으니 플래그 켬
                        judgementFlag = true;

                        ModeChange();

                        tempBarcode = GetBCRS(PLC_BCRS_ADDRESS, 15);

                        mw.LogState(LogType.PLC_RECV, "Origin BCR - " + tempBarcode);
                        tempBarcode = tempBarcode.Replace("\0", "").Replace("\n", "").Replace("\r", "").Replace(" ", "");
                        mw.LogState(LogType.PLC_RECV, "Parsed BCR - " + tempBarcode);

                        mw.Dispatcher.Invoke(new Action(() =>
                        {
                            mw.lotTb.Text = tempBarcode;
                        }));

                        //200401 시작부분 주석처리
                        //isTesting(true);

                        mw.LogState(LogType.Info, "START_TO_BARCODE - " + tempBarcode);

                        mw.Dispatcher.Invoke(new Action(() =>
                        {
                            mw.AutoMode();
                        }));

                        tempBarcode = "";
                    }
                }
            }
            #endregion

            #region RecvOK
            if (plc_Result_recv_ok && PLC_DATA[PLC_RECV_ADDRESS][2] == false)
            {
                mw.LogState(LogType.PLC_RECV, "PLC RECV OK OFF.");
                plc_Pause_Continue = PLC_DATA[PLC_RECV_ADDRESS][2] = false;
            }
            plc_Result_recv_ok = PLC_DATA[PLC_RECV_ADDRESS][2];

            if (plc_Result_recv_ok)
            {
                mw.LogState(LogType.PLC_RECV, "PLC RECV OK ON.");
            }
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

                        mw.ispause = true;
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
            if (plc_Pause_Continue && PLC_DATA[PLC_RECV_ADDRESS][9] == false)  // 5 is puase 9 is blank
            {
                mw.LogState(LogType.PLC_RECV, "PLC PAUSE CONTINUE OFF.");
                plc_Pause_Continue = PLC_DATA[PLC_RECV_ADDRESS][9] = false;
            }
            plc_Pause_Continue = PLC_DATA[PLC_RECV_ADDRESS][9];

            if (plc_Pause_Continue)
            {
                if (mw.autoModeThread != null && mw.autoModeThread.IsAlive)
                {
                    mw.LogState(LogType.PLC_RECV, "CONTINUE");

                    if (test_Stop_Flag)
                        test_Stop(false);

                    if (!isTesting_Flag)
                        isTesting(true);
                    
                    isOncePause = false;

                    mw.ispause = false;
                }
            }
            #endregion

            GetActtype(PLC_RECV_ADDRESS);
            #region Stop
            plc_isStop = PLC_DATA[PLC_RECV_ADDRESS][9];  //  6 is stop , 9 is blank
            if (plc_isStop)
            {
                if (mw.autoModeThread != null && mw.autoModeThread.IsAlive)
                {
                    mw.LogState(LogType.PLC_RECV, "STOP");
                    mw.StopAuto();

                    if (isOnceStop == false)
                    {
                        isOnceStop = true;
                        
                        mw.Dispatcher.Invoke(new Action(() =>
                        {
                            DeviceCheckWindow dcw = new DeviceCheckWindow(mw);
                            mw.isDeviceFail = false;
                            dcw.Height += 100;
                            dcw.Width += 200;
                            dcw.maintitle.FontSize += 20;
                            dcw.maintitle.Content = "EMERGENCY STOP";
                            dcw.reason.FontSize += 13;
                            dcw.reason.Content = "Received [STOP SIGNAL] by PLC";
                            dcw.okbt.FontSize += 13;
                            dcw.Show();
                        }));
                    }
                }
                mw.LogState(LogType.PLC_RECV, "STOP");
                //plc_isStop = PLC_DATA[PLC_RECV_ADDRESS][6] = false;
            }
            else
            {
                isOnceStop = false;
            }
            #endregion

            #region PLC Jig Down
            if (plc_isJigDown && PLC_DATA[PLC_RECV_ADDRESS][7] == false)
            {
                mw.LogState(LogType.PLC_RECV, "PLC JIG DOWN OFF.");
                plc_isJigDown = PLC_DATA[PLC_RECV_ADDRESS][7] = false;
            }
            plc_isJigDown = PLC_DATA[PLC_RECV_ADDRESS][7];

            if (plc_isJigDown)
            {
                //mw.LogState(LogType.PLC_RECV, "PLC JIG DOWN ON.");
            }
            #endregion

            plc_lv_retry_recv_ok = PLC_DATA[PLC_RECV_ADDRESS][8];
            this.plc_model_A = PLC_DATA[PLC_RECV_ADDRESS][10];
            this.plc_model_B = PLC_DATA[PLC_RECV_ADDRESS][11];
            this.plc_model_C = PLC_DATA[PLC_RECV_ADDRESS][12];
            this.plc_model_D = PLC_DATA[PLC_RECV_ADDRESS][13];
            this.plc_model_E = PLC_DATA[PLC_RECV_ADDRESS][14];
            this.plc_model_F = PLC_DATA[PLC_RECV_ADDRESS][15];
            plc_isAlive = PLC_DATA[PLC_RECV_ADDRESS][15];
        }
         

        public bool isSuc = false;
        /// <summary>
        /// 모든테스트 끝일때 날려
        /// 테스트 시작시 isSuc false해
        /// </summary>
        /// <param name="bbo"></param>
        /// <returns></returns>
        public bool TestResult(bool passFail)
        {
            bool rtv = false;

            //if (!isAlive)
            //    return rtv;

            try
            {

                if (isTesting_Flag)
                    isTesting(false);

                //200401 플래그가 켜져있으면 설비에서 시작했다 판단하고 결과를 보낸다.
                if (judgementFlag)
                {
                    judgementFlag = false;

                    if (passFail)
                    {
                        Result_Pass(true);

                        isSuc = rtv = true;

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
                        isSuc = false;
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
