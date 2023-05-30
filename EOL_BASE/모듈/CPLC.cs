using EOL_BASE.윈도우;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace EOL_BASE.모듈
{
    public class CPLC
    {
        MainWindow mw;

        public string PLC_RECV_ADDRESS = "BA00";
        public string PLC_SEND_ADDRESS = "BA10";
        public string PLC_BCRS_ADDRESS = "W840";
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

            //mw.Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        rt = new DeviceCheckWindow(mw);
            //        rt.Height += 100;
            //        rt.Width += 400;
            //        rt.maintitle.FontSize += 30;
            //        rt.malb.FontSize += 10;
            //        rt.malb.Content = "!!!";
            //        rt.maintitle.Content = "EMERGENCY STOPPED!!";
            //        rt.reason.FontSize += 13;
            //        rt.reason.Content = "Device is Stopped";
            //        rt.okbt.FontSize += 13;
            //    }));
        }

        public Dictionary<string, bool[]> PLC_DATA = new Dictionary<string, bool[]>();


        int PLC_SendData = 0;
        int PLC_RecvData = 0;

        public bool plc_isStart = false;
        public bool plc_Result_recv_ok = false;
        public bool plc_isPause = false;
        public bool plc_Pause_Continue = false;
        public bool plc_isStop = false;
        public bool plc_model_4P8S = false;
        public bool plc_model_4P8S_REV = false;
        public bool plc_model_4P7S = false;
        public bool plc_model_3P10S = false;
        public bool plc_model_3P8S = false;
        public bool plc_model_3P10S_REV = false;
        public bool plc_isAlive = false;
        public bool plc_isJigDown = false;
        //public bool plc_isIV_Mode_Ok = false;
        public bool plc_isIV_Recv_Ok = false;
        public bool plc_lv_retry_recv_ok = false;

        public string plc_BCR = "";
        public bool isAlive = false;

        public string masterTempBcr = "";

        ActUtlTypeLib.ActUtlType actType;
        List<string> szDeviceList = new List<string>();
        System.Windows.Forms.Timer recvTimer;
        Thread aliveThread, recvThread;
        string plc_write_fail = " - PLC_WRITE_FAIL";

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

        bool test_Stop_Flag = false;
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

        public bool jig_Down_Flag = false;
        public void Jig_Down(bool on)
        {
            if (!isAlive)
                return;

            if (on)
            {
                jig_Down_Flag = true;
                PLC_SendData += 128;
                mw.LogState(LogType.PLC_SEND, "JigDown_ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                jig_Down_Flag = false;
                PLC_SendData -= 128;
                mw.LogState(LogType.PLC_SEND, "JigDown_OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            //actType.WriteDeviceBlock(str_pc_Stop, 1, 1);
        }

        public bool iv_Cylinder_Down_Flag = false;
        public void Iv_Cylinder_Down(bool on)
        {
            if (!isAlive)
                return;

            if (on)
            {
                iv_Cylinder_Down_Flag = true;
                PLC_SendData += 256;
                mw.LogState(LogType.PLC_SEND, "IVCylinderDown_ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                iv_Cylinder_Down_Flag = false;
                PLC_SendData -= 256;
                mw.LogState(LogType.PLC_SEND, "IVCylinderDown_OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            //actType.WriteDeviceBlock(str_pc_Stop, 1, 1);
        }

        ////JKD 신규라인 9번 비트
        //public bool iv_Check_Mode_Flag = false;
        //public void IV_Check_Mode(bool on)
        //{
        //    if (!isAlive)
        //        return;

        //    if (on)
        //    {
        //        iv_Check_Mode_Flag = true;
        //        PLC_SendData += 512;
        //        mw.LogState(LogType.PLC_SEND, "IVCheckMode_ON");
        //        actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
        //    }
        //    else
        //    {
        //        iv_Check_Mode_Flag = false;
        //        PLC_SendData -= 512;
        //        mw.LogState(LogType.PLC_SEND, "IVCheckMode_OFF");
        //        actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
        //    }
        //}

        //신규라인 6번 비트(PreCheck)
        public bool check_Cool_New_Flag = false;
        public void CoolingPinWireNew(bool on)
        {
            if (!isAlive)
                return;

            if (on)
            {
                check_Cool_New_Flag = true;
                PLC_SendData += 64;
                mw.LogState(LogType.PLC_SEND, "CoolingPinWire_ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                check_Cool_New_Flag = false;
                PLC_SendData -= 64;
                mw.LogState(LogType.PLC_SEND, "CoolingPinWire_OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
        }

        //기존라인 9번 비트(PreCheck)
        public bool check_Cool_Old_Flag = false;
        public void CoolingPinWireOld(bool on)
        {
            if (!isAlive)
                return;

            if (on)
            {
                check_Cool_Old_Flag = true;
                PLC_SendData += 512;
                mw.LogState(LogType.PLC_SEND, "CoolingPinWire_ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                check_Cool_Old_Flag = false;
                PLC_SendData -= 512;
                mw.LogState(LogType.PLC_SEND, "CoolingPinWire_OFF");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
        }

        public bool check_Cool_Flag = false;
        public void PreUnloadingFlag(bool on)
        {
            if (!isAlive)
                return;

            if (on)
            {
                check_Cool_Flag = true;
                PLC_SendData += 2;
                mw.LogState(LogType.PLC_SEND, "PreUnloadingFlag_ON");
                actType.WriteDeviceBlock(PLC_SEND_ADDRESS, 1, PLC_SendData);
            }
            else
            {
                check_Cool_Flag = false;
                PLC_SendData -= 2;
                mw.LogState(LogType.PLC_SEND, "PreUnloadingFlag_OFF");
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


        Thread instanceTh1, instanceTh2, recThread;

        private void StartConnect()
        {
            bool isOPEN = false;
            actType = new ActUtlTypeLib.ActUtlType();
            actType.ActLogicalStationNumber = PLC_LOGICAL_NUMBER;

            //210923 JKD - MX COMPONENT Open Receive Timeout
            System.Diagnostics.Stopwatch sp = new System.Diagnostics.Stopwatch();
            sp.Start();
            recThread = new Thread(() =>
            {
                isOPEN = actType.Open().Equals(0);
            });
            recThread.Start();

            while (true)
            {
                if (isOPEN)
                {
                    sp.Stop();
                    break;
                }

                if (sp.ElapsedMilliseconds >= 3000 && isOPEN == false)
                {
                    sp.Stop();
                    break;
                }
            }

            // JKD - MX 컴포넌트 설정 O, PLC 연결 X -> 통신 연결 지연시간 발생
            //if (actType.Open().Equals(0))
            if (isOPEN)
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
                        while (rtv == -1)
                        {
                            Thread.Sleep(100);
                            cnt += 100;
                            if (cnt >= timeoutTick)
                            {
                                mw.relays.RelayOn("IDO_0"); 
                                mw.LogState(LogType.Fail, "PLC_RESPONSE_FAIL_IN_" + (timeoutTick / 1000).ToString() + "SEC");
                                isAlive = false;
                                mw.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    mw.contBt_plc.Background = System.Windows.Media.Brushes.Red;
                                }));
                                break;
                            }
                        }
                        #endregion
                        instanceTh1 = null;

                        if (rtv == 0)
                        {
                            isAlive = true;

                            var isComDied = mw.relays.RelayStatus("IDO_0");
                            if (isComDied)
                            {
                                mw.relays.RelayOff("IDO_0"); // 211208 JKD - PLC 재연결시 IDO_0 경광등 Off
                            }

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
                        while (rtv == -1)
                        {
                            Thread.Sleep(100);
                            cnt += 100;
                            if (cnt >= timeoutTick)
                            {
                                mw.relays.RelayOn("IDO_0"); 
                                mw.LogState(LogType.Fail, "PLC_RESPONSE_FAIL_IN_" + (timeoutTick / 1000).ToString() + "SEC");
                                isAlive = false;
                                mw.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    mw.contBt_plc.Background = System.Windows.Media.Brushes.Red;
                                }));
                                break;
                            }
                        }
                        #endregion
                        instanceTh2 = null;


                        if (rtv == 0)
                        {
                            isAlive = true;

                            var isComDied = mw.relays.RelayStatus("IDO_0");
                            if (isComDied)
                            {
                                mw.relays.RelayOff("IDO_0"); // 211208 JKD - PLC 재연결시 IDO_0 경광등 Off
                            }

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

                mw.LogState(LogType.Success, "PLC OPEN Success");
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
                mw.Dispatcher.BeginInvoke(new Action(() =>
                {
                    mw.contBt_plc.Background = System.Windows.Media.Brushes.Red;
                    mw.relays.RelayOn("IDO_0");  
                    //mw.tlamp.SetTLampInstrumentOff(true);
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


        //public string testGetBCRS(string addr, int length)
        //{
        //    string deviceNames = "";
        //    int[] val = new int[length];
        //    var rtval = "";
        //    int j = 0;
        //    var hex = j.ToString("X1");
        //    actType.ReadDeviceBlock(addr + hex, 16, out val[0]);

        //    for (int i = 0; i < length; i++)
        //    {
                
        //        actType.ReadDeviceBlock(addr + hex, 1, out val);

        //        var plcRecv = Convert.ToString(val, 2);
        //        int size = plcRecv.Length;
        //        while (size < 16)
        //        {
        //            plcRecv = "0" + plcRecv;

        //            size++;
        //        }

        //        var data = this.getbytes1(plcRecv);
        //        rtval += Encoding.ASCII.GetString(data);
        //    }
        //    return rtval;
        //}


        public string GetBCRS(string addr, int length)
        {
            int[] val = new int[length];
            var rtval = "";
            int j = 0;
            var hex = j.ToString("X1");
            actType.ReadDeviceBlock(addr + hex, 16, out val[0]);

            for (int i = 0; i < length; i++)
            {
                //var hex = i.ToString("X1");
                //actType.ReadDeviceBlock(addr + hex, 1, out val);

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
        }


        string GEN_Mode = "4096";
        //int GEN65_Mode = 8192;
        string TOB_Mode = "8192";
        string MQB_Mode = "16384";
        //int MQB_Mode = 32768;

        public void ModeChange(bool modeDummy = false)
        {
            if (modeDummy)
            {
                mw.AutoMode_change("dummy");
            }
            else
            {
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

        int errorClearWaitCnt = 0;
        bool isConnFail = false;

        //200401 판정값을 설비로 보내기 위한 플래그
        public bool judgementFlag = false;

        void recvTimer_Tick()
        {
            double data = 0.0;
            GetActtype(PLC_RECV_ADDRESS);

            if (mw.cycler != null)
            {
                if (mw.cycler.isAlaramStatusCheck == false)
                {
                    if (errorClearWaitCnt++ > 100)
                    {
                        errorClearWaitCnt = 0;
                        mw.LogState(LogType.Info, "Cycler Fault!!! Please Click Error Clear Button!!!!!!!!!!!");
                    }
                    return;
                }
            }
            else
            {
                return;
            }

            this.plc_model_4P8S = PLC_DATA[PLC_RECV_ADDRESS][10];
            this.plc_model_4P8S_REV = PLC_DATA[PLC_RECV_ADDRESS][11];
            this.plc_model_4P7S = PLC_DATA[PLC_RECV_ADDRESS][12];
            this.plc_model_3P10S = PLC_DATA[PLC_RECV_ADDRESS][13];
            this.plc_model_3P8S = PLC_DATA[PLC_RECV_ADDRESS][14];
            this.plc_model_3P10S_REV = PLC_DATA[PLC_RECV_ADDRESS][15];
            plc_lv_retry_recv_ok = PLC_DATA[PLC_RECV_ADDRESS][9];

            ////210803 JKD - IV Pin Bit
            //#region IV Mode
            //if (plc_isIV_Mode_Ok && PLC_DATA[PLC_RECV_ADDRESS][9] == false)
            //{
            //    plc_isIV_Mode_Ok = PLC_DATA[PLC_RECV_ADDRESS][9] = false;
            //}
            //plc_isIV_Mode_Ok = PLC_DATA[PLC_RECV_ADDRESS][9];
            //#endregion

            #region Start N BCRS
            if (plc_isStart && PLC_DATA[PLC_RECV_ADDRESS][0] == false)
            {
                mw.LogState(LogType.PLC_RECV, "PLC START OFF.");
                plc_isStart = PLC_DATA[PLC_RECV_ADDRESS][0] = false;
            }
            plc_isStart = PLC_DATA[PLC_RECV_ADDRESS][0];

             if (plc_isStart)
            {
                mw.LogState(LogType.PLC_RECV, "START");

                #region 210108 유사원 요청

                if (mw.isLineFlag)
                {
                    #region DAQ970A
                    if (mw.daq970_1 != null)
                    {
                        isConnFail = false;
                        mw.daq970_1.MeasRes(out data, mw.connTargetCh);

                        double resResult = data;
                        double target = double.Parse(mw.connTargetOhm);
                        if (resResult > target)
                        {
                            isConnFail = true;
                        }

                        mw.LogState(LogType.Info, string.Format("resResult:{0} / Spec:{1}", resResult, target));
                    }
                    else
                    {
                        return;
                    }
                    #endregion
                }
                else
                {
                    #region DMM34970

                    if (mw.keysight != null)
                    {
                        isConnFail = false;
                        mw.keysight.MeasTemp(mw.connTargetCh);

                        int rec = mw.keysight.sp.BytesToRead;

                        int cnt = 0;
                        //채널한개값 16 + etx
                        while (rec < 17)
                        {
                            Thread.Sleep(100);
                            rec = mw.keysight.sp.BytesToRead;
                            cnt += 100;
                            //not received data
                            if (cnt == 5000)
                            {
                                mw.keysight.MeasTemp(mw.connTargetCh);

                                rec = mw.keysight.sp.BytesToRead;

                                cnt = 0;
                                while (rec < 17)
                                {
                                    Thread.Sleep(100);
                                    rec = mw.keysight.sp.BytesToRead;
                                    cnt += 100;
                                    if (cnt == 5000)
                                    {
                                        mw.LogState(LogType.Info, "Cooling pin connection check limit - Connection Check FAIL ");
                                    }
                                }
                                break;
                            }
                        }
                        if (cnt != 5000)
                        {
                            //받은 후에 데이터 파싱
                            byte[] bt = new byte[rec];
                            mw.keysight.sp.Read(bt, 0, rec);

                            mw.keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);


                            //LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

                            var vArr = mw.keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            double resResult = double.Parse(vArr[0]);
                            double target = double.Parse(mw.connTargetOhm);
                            if (resResult > target)
                            {
                                isConnFail = true;
                            }

                            mw.LogState(LogType.Info, string.Format("resResult:{0} / Spec:{1}", resResult, target));

                        }
                    }
                    else
                    {
                        return;
                    }
                    #endregion
                }

                #endregion            

                if (mw.isManual)
                {
                    mw.LogState(LogType.Info, "Please Change mode !!!!! Device is ready to start automayically..");
                    Thread.Sleep(5000);
                }
                else if (mw.isDeviceFail)
                {
                    mw.LogState(LogType.Info, "Please check [DEVICE CHECK WINDOW] !!!!! [DEVICE CHECK WINDOW] is Activated NOW");
                    Thread.Sleep(5000);
                }
                else if (mw.cycler.isAlarmOpen)
                {
                    mw.LogState(LogType.Info, "Please check [CYCLER ALARM WINDOW] !!!!! [CYCLER ALARM WINDOW] is Activated NOW");
                    Thread.Sleep(5000);
                }
                else if(isConnFail)
                {
                    isTesting(true);
                    Thread.Sleep(1000);
                    TestResult(false);
                    Thread.Sleep(100);
                    if (mw.isLineFlag)
                    {
                        CoolingPinWireNew(true);
                        Thread.Sleep(1000);
                        CoolingPinWireNew(false);
                    }
                    else
                    {
                        //CoolingPinWireOld(true);
                        //Thread.Sleep(1000);
                        //CoolingPinWireOld(false);
                    }
                  
                    mw.Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBox.Show("Please check cooling pin wiring", "Warning", MessageBoxButton.OK,MessageBoxImage.Warning);
                    }));
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
                        judgementFlag = true;

                        ModeChange();

                        tempBarcode = GetBCRS(PLC_BCRS_ADDRESS, 16);

                        mw.LogState(LogType.PLC_RECV, "Origin BCR - " + tempBarcode);
                        tempBarcode = tempBarcode.Replace("\0", "").Replace("\n", "").Replace("\r", "");

                        // 20191121 Noah Choi PLC에게 바코드를 받을 시 마지막 문자 삭제 진행
                        string tempSubstringBarcode = tempBarcode;
                        tempSubstringBarcode = tempSubstringBarcode.Substring(tempBarcode.Length - 1);

                        if (tempSubstringBarcode == " ")
                        {
                            tempBarcode = tempBarcode.Trim();
                            mw.LogState(LogType.PLC_RECV, "Parsed BCR_TrimEnd - " + tempBarcode);
                        }

                        // 20191208 Noah Choi 바코드에 MASTER라는게 붙을 시 NG처리(강인혁 사원님 요청) 
                        masterTempBcr = tempBarcode;

                        if (masterTempBcr.Length > 6)
                        {
                            masterTempBcr = masterTempBcr.Substring(0, 6);

                            if (masterTempBcr == "MASTER")
                            {
                                mw.LogState(LogType.PLC_RECV, "Recevice MASTER_BCR - " + tempBarcode);
                                mw.Dispatcher.Invoke(new Action(() =>
                                {
                                    mw.isMasterBcr = true;
                                    //mw.isSkipNG.IsChecked = true;
                                    Thread.Sleep(500);
                                }));

                            }
                        }
                        

                        mw.LogState(LogType.PLC_RECV, "Parsed BCR - " + tempBarcode);
                        
                        mw.Dispatcher.Invoke(new Action(() =>
                        {
                            mw.lotTb.Text = tempBarcode;
                        }));

                        isTesting(true);

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

            ////210803 JKD - IV Pin Bit
            //#region IV Mode
            //if (plc_isIV_Mode_Ok && PLC_DATA[PLC_RECV_ADDRESS][9] == false)
            //{
            //    plc_isIV_Mode_Ok = PLC_DATA[PLC_RECV_ADDRESS][9] = false;
            //}
            //plc_isIV_Mode_Ok = PLC_DATA[PLC_RECV_ADDRESS][9];
            //#endregion

            //210803 JKD - IV Cylinder Down Bit 
            #region IV Cylinder Down
            if (plc_isIV_Recv_Ok && PLC_DATA[PLC_RECV_ADDRESS][8] == false)
            {
                plc_isIV_Recv_Ok = PLC_DATA[PLC_RECV_ADDRESS][8] = false;
            }
            plc_isIV_Recv_Ok = PLC_DATA[PLC_RECV_ADDRESS][8];
            #endregion

            plc_Result_recv_ok = PLC_DATA[PLC_RECV_ADDRESS][2];

            #region Pause
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
            plc_Pause_Continue = PLC_DATA[PLC_RECV_ADDRESS][5];
            if (plc_Pause_Continue)
            {
                if (mw.autoModeThread != null && mw.autoModeThread.IsAlive)
                {
                    //190612
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
            plc_isStop = PLC_DATA[PLC_RECV_ADDRESS][6];
            if (plc_isStop)
            {
                if (mw.autoModeThread != null && mw.autoModeThread.IsAlive)
                {
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

                        if (!mw.isAlarmStop)
                        {
                            dcw.Show();
                            mw.isAlarmStop = true;
                        }
                    }));

                    mw.LogState(LogType.PLC_RECV, "STOP");
                    mw.StopAuto();
                }

                //plc_isStop = PLC_DATA[PLC_RECV_ADDRESS][6] = false;
            }
            else
            {
                mw.isAlarmStop = false;
            }
            #endregion

            if (plc_isJigDown && PLC_DATA[PLC_RECV_ADDRESS][7] == false)
            {
                mw.LogState(LogType.PLC_RECV, "PLC JIG REMOVED");
                plc_isJigDown = PLC_DATA[PLC_RECV_ADDRESS][7] = false;
            }
            plc_isJigDown = PLC_DATA[PLC_RECV_ADDRESS][7];
            //plc_isAlive = PLC_DATA[PLC_RECV_ADDRESS][15];
        }
        DeviceCheckWindow rt = null;
        public bool isSuc = false;
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
                    judgementFlag = false;

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
                    judgementFlag = false;

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
            catch (Exception ec)
            {
                mw.LogState(LogType.Info, "PLC", ec);
            }
            return rtv;
        }

        public bool LV_Retry_Flag = false;
        public void SendLV_Retry(bool on)
        {
            if (on)
            {
                LV_Retry_Flag = true;
                PLC_SendData += 512;

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
                PLC_SendData -= 512;

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

        public void SendInspectionStartOnOff(bool on)
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
