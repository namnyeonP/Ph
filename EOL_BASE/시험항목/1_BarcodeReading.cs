using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EOL_BASE.Forms;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        private int DAQErrcnt = 0;

        public bool BarcodeReading(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                ti.Value_ = modelList[selectedIndex].LotId = lotTb.Text;
                //49. 바코드 리딩 값 적용 구문 및 로그 추가
                LogState(LogType.Info, "Value : " + ti.Value_);
            }));

            deviceStatus = "";
            List<string> ngDeviceList = new List<string>();

            #region Device Connecting Check
            var checkDeviceName = "[MES]";
            #region MES Check

            if (!isMESskip)
            {
                if (!MES.heartbeating())
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_mes.Background = System.Windows.Media.Brushes.Red;
                    }));
                    LogState(LogType.Fail, "[Fail] - " + checkDeviceName);
                    ngDeviceList.Add(checkDeviceName);

                    relays.RelayOn("IDO_0");
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_mes.Background = System.Windows.Media.Brushes.Green;
                    }));

                    relays.RelayOn("IDO_3");
                    LogState(LogType.Success, "[Success] - " + checkDeviceName);
                }
            }
            #endregion

            #region PLC Check
            checkDeviceName = "[PLC]";
            if (plc != null)
            {
                if (!plc.isAlive)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_plc.Background = System.Windows.Media.Brushes.Red;
                    }));
                    LogState(LogType.Fail, "[Fail] - " + checkDeviceName);
                    ngDeviceList.Add(checkDeviceName);

                    relays.RelayOn("IDO_0");
                }
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_plc.Background = System.Windows.Media.Brushes.Green;
                }));
                relays.RelayOn("IDO_3");
                LogState(LogType.Success, "[Success] - " + checkDeviceName);
            }
            #endregion

            #region Cycler Check
            checkDeviceName = "[CYCLER]";

            bool result = CyclerStatusCheck();

            if (result)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_cycler.Background = System.Windows.Media.Brushes.Green;
                }));
                LogState(LogType.Success, "[Success] - " + checkDeviceName); 
            }
            else
            {
                LogState(LogType.Fail, "[Fail] - " + checkDeviceName);
                 
                SetMainCState("NOT READY");

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_cycler.Background = System.Windows.Media.Brushes.Red;
                }));

                ngDeviceList.Add(checkDeviceName);
            }

            #endregion

            #region Chroma Check

            checkDeviceName = "[HIPOT]";
            if (chroma != null && chroma.ChromaAliveCheck() > 0)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_hipot.Background = System.Windows.Media.Brushes.Green;
                }));
                LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    contBt_hipot.Background = System.Windows.Media.Brushes.Red;
                }));
                LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                ngDeviceList.Add(checkDeviceName);
            }
            #endregion

            // 2019.08.27 jeonhj's comment
            // 온도계 연결성은 Watchdog Flag로 처리되고 있음.
            // 온도계 연결 끊어질 시 값을 -255로 표시하도록 변경(이건 Relay Background에 구현되어야함.)
            #region TEMP Check

            string regSubkey = "Software\\EOL_Trigger\\Relays";
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            checkDeviceName = "[TEMP]";
            string tempWd_Flag = rk.GetValue("NHT_RS232_WD_FLAG").ToString();
            var pro = System.Diagnostics.Process.GetProcessesByName("RelayController_Background");


            if (tempWd_Flag == "0" && pro.Length != 0)
            {
                LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                //new CNhtRS232_Receiver(this);
            }
            else
            {
                LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                ngDeviceList.Add(checkDeviceName);
            }
            #endregion

            #region DAQ Check

            int nDaqCnt = 0;
            checkDeviceName = "[DAQ]";
            if (daq != null && daq.sp.IsOpen)
            {
                var nowDaqDt = daq.localDt;
                //221101 DAQ 10s
                nowDaqDt = nowDaqDt.AddSeconds(10);
                //nowDaqDt = nowDaqDt.AddSeconds(5);


				//220926
                //20180808 wjs add daq retry connection
                if (DateTime.Now > nowDaqDt)
                {
                //연결이 되었는 지 재 확인 220926
                DaqReConnect:

                    //LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                    LogState(LogType.TEST, checkDeviceName + " Myunghwan Dispose resources"
                        + DateTime.Now + "/" + nowDaqDt);

                    daq.Close();

                    Thread.Sleep(100);
                    daq = null;
                    
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        daq = new EOL_BASE.모듈.CDAQ(this, this.daq_PortName);
                    }));
                    Thread.Sleep(1000);

                    nowDaqDt = daq.localDt;

                    //221101 DAQ 10s
                    nowDaqDt = nowDaqDt.AddSeconds(10);
                    //nowDaqDt = nowDaqDt.AddSeconds(5);

                    //220927
                    //연결이 되었는 지 재 확인 
                    if (daq.isAlive)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_daq1.Background = System.Windows.Media.Brushes.Green;
                        }));
                        LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                    }
                    else
                    {
                        if(nDaqCnt >= 5)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                contBt_daq1.Background = System.Windows.Media.Brushes.Red;
                            }));

                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                            ngDeviceList.Add(checkDeviceName);
                        }
                        else
                        {
                            nDaqCnt++;
                            goto DaqReConnect;
                        }                        
                    }                    
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_daq1.Background = System.Windows.Media.Brushes.Green;
                    }));

                    LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                }
            }
            else
            {
            //연결이 되었는 지 재 확인 220926
            DaqReConnect:

                LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! retry connection");

                if (daq != null && daq.sp != null)
                {
                    LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                    daq.sp.Close();
                    daq.sp.Dispose();
                    daq = null;
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    daq = new EOL_BASE.모듈.CDAQ(this, this.daq_PortName);
                }));

                Thread.Sleep(1000);

                if (daq != null && daq.sp.IsOpen)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_daq1.Background = System.Windows.Media.Brushes.Green;
                    }));

                    LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                }
                else
                {
                    if (nDaqCnt >= 5)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_daq1.Background = System.Windows.Media.Brushes.Red;
                        }));

                        LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                        ngDeviceList.Add(checkDeviceName);
                    }
                    else
                    {
                        nDaqCnt++;
                        goto DaqReConnect;
                    }
                }
            }

            #endregion

            if (isLineFlag)
            {
                #region DMM1 Check

                checkDeviceName = "[DMM1]";
                if (daq970_1 != null && daq970_1.isAlive)
                {
                    string tempstr = daq970_1.IDN();
                    if (tempstr == "IDN_Fail")
                    {
                        LogState(LogType.TEST, checkDeviceName + " Connection FAIL !!");

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_daq970_1.Background = System.Windows.Media.Brushes.Red;
                        }));

                        LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                        ngDeviceList.Add(checkDeviceName);
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_daq970_1.Background = System.Windows.Media.Brushes.Green;
                        }));
                        LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                    }
                }
                else
                {
                    LogState(LogType.TEST, checkDeviceName + " Connection FAIL !!");

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        contBt_daq970_1.Background = System.Windows.Media.Brushes.Red;
                    }));

                    LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                    ngDeviceList.Add(checkDeviceName);

                }
                #endregion

                //#region DMM2 Check

                //checkDeviceName = "[DMM2]";
                //if (daq970_2 != null && daq970_2.isAlive)
                //{
                //    string tempstr = daq970_2.IDN();
                //    if (tempstr == "IDN_Fail") 
                //    {
                //        LogState(LogType.TEST, checkDeviceName + " Connection FAIL !!");

                //        Dispatcher.BeginInvoke(new Action(() =>
                //        {
                //            contBt_daq970_2.Background = System.Windows.Media.Brushes.Red;
                //        }));

                //        LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                //        ngDeviceList.Add(checkDeviceName);
                //    }
                //    else
                //    {
                //        Dispatcher.BeginInvoke(new Action(() =>
                //        {
                //            contBt_daq970_2.Background = System.Windows.Media.Brushes.Green;
                //        }));
                //        LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                //    }
                //}
                //else
                //{
                //    LogState(LogType.TEST, checkDeviceName + " Connection FAIL !!");

                //    Dispatcher.BeginInvoke(new Action(() =>
                //    {
                //        contBt_daq970_2.Background = System.Windows.Media.Brushes.Red;
                //    }));

                //    LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                //    ngDeviceList.Add(checkDeviceName);
                //}
                //#endregion

                //#region DMM3 Check

                //checkDeviceName = "[DMM3]";
                //if (daq970_3 != null && daq970_3.isAlive)
                //{
                //    string tempstr = daq970_3.IDN();
                //    if (tempstr == "IDN_Fail")
                //    {
                //        LogState(LogType.TEST, checkDeviceName + " Connection FAIL !!");

                //        Dispatcher.BeginInvoke(new Action(() =>
                //        {
                //            contBt_daq970_3.Background = System.Windows.Media.Brushes.Red;
                //        }));

                //        LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                //        ngDeviceList.Add(checkDeviceName);
                //    }
                //    else
                //    {
                //        Dispatcher.BeginInvoke(new Action(() =>
                //        {
                //            contBt_daq970_3.Background = System.Windows.Media.Brushes.Green;
                //        }));
                //        LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                //    }
                //}
                //else
                //{
                //    LogState(LogType.TEST, checkDeviceName + " Connection FAIL !!");

                //    Dispatcher.BeginInvoke(new Action(() =>
                //    {
                //        contBt_daq970_3.Background = System.Windows.Media.Brushes.Red;
                //    }));

                //    LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                //    ngDeviceList.Add(checkDeviceName);
                //}
                //#endregion
            }
            else
            {
                #region DMM Check

                checkDeviceName = "[DMM]";
                if (keysight != null && keysight.sp.IsOpen)
                {
                    string tempstr = keysight.IDN();
                    if (tempstr == "IDN_Fail")
                    {
                        LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! retry connection");

                        if (keysight != null && keysight.sp != null)
                        {
                            LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                            keysight.sp.Close();
                            keysight.sp.Dispose();
                            keysight = null;
                        }

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            keysight = new CKeysightDMM(this, true, this.keysight_PortName);
                        }));

                        if (keysight != null && keysight.isAlive)
                        {
                            tempstr = keysight.IDN();
                            if (tempstr == "IDN_Fail")
                            {
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                                }));

                                LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                                ngDeviceList.Add(checkDeviceName);
                            }
                            else
                            {
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                                }));
                                LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                            }
                        }
                        else
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                            }));

                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                            ngDeviceList.Add(checkDeviceName);
                        }
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                        }));
                        LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                    }
                }
                else
                {
                    LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! retry connection");

                    if (keysight != null && keysight.sp != null)
                    {
                        LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                        keysight.sp.Close();
                        keysight.sp.Dispose();
                        keysight = null;
                    }

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        keysight = new CKeysightDMM(this, true, this.keysight_PortName);
                    }));

                    string tempstr = keysight.IDN();
                    if (keysight != null && keysight.isAlive)
                    {
                        tempstr = keysight.IDN();
                        if (tempstr == "IDN_Fail")
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                            }));

                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                            ngDeviceList.Add(checkDeviceName);
                        }
                        else
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                            }));
                            LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                        }
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                        }));

                        LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                        ngDeviceList.Add(checkDeviceName);
                    }

                }
                #endregion
            }

            //#region MES Check

            //checkDeviceName = "[MES]";

            //if (MES.heartbeating())
            //{
            //    Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        contBt_mes.Background = System.Windows.Media.Brushes.Green;
            //    }));

            //    LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
            //}
            //else
            //{
            //    Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        contBt_mes.Background = System.Windows.Media.Brushes.Red;
            //    }));
            //    LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
            //    ngDeviceList.Add(checkDeviceName);
            //}

            //#endregion

            if (ngDeviceList.Count > 0)
            {
                foreach (var device in ngDeviceList)
                {
                    deviceStatus += device + "/";
                }
                deviceStatus = deviceStatus.Remove(deviceStatus.Length - 1, 1);
                LogState(LogType.Fail, "[DEVICE_CHECK_RESULT] " + deviceStatus);
                ViewRetryWindow(deviceStatus, ti);
            }
            else
            {
                relays.RelayOff("IDO_0");
            }

            //2022.10.17소모품 카운트 하는 부분
            Protection_PartsCountStart = new Thread(() =>
            {
                FormPartsCountSetting PCS = new FormPartsCountSetting(this, "START");
                PCS.CountRealData();
                this.SetPartsCountData(ti, "StartRun");

            });
            Protection_PartsCountStart.Start();

            //230120
            var rtList = new List<string>();
            
            //try
            //{
            //     McResistanceMeasure(mcResistanceposition, out rtList);

            //    //221004 저항 측정 값 추가 Open 되었을 때
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_3_OPEN_CHECK_NEW, rtList[0].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_4_OPEN_CHECK_NEW, rtList[1].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_5_OPEN_CHECK_NEW, rtList[2].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_6_OPEN_CHECK_NEW, rtList[3].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_7_OPEN_CHECK_NEW, rtList[4].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_8_OPEN_CHECK_NEW, rtList[5].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_9_OPEN_CHECK_NEW, rtList[6].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_10_OPEN_CHECK_NEW, rtList[7].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_11_OPEN_CHECK_NEW, rtList[8].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_12_OPEN_CHECK_NEW, rtList[9].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_13_OPEN_CHECK_NEW, rtList[10].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_14_OPEN_CHECK_NEW, rtList[11].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_15_OPEN_CHECK_NEW, rtList[12].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_16_OPEN_CHECK_NEW, rtList[13].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_17_OPEN_CHECK_NEW, rtList[14].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_18_OPEN_CHECK_NEW, rtList[15].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_19_OPEN_CHECK_NEW, rtList[16].ToString()));
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MC_20_OPEN_CHECK_NEW, rtList[17].ToString()));

            //    rtList.Clear();
            //}
            //catch
            //{
            //    rtList.Clear();
            //}

            #endregion

            return JudgementTestItem(ti);
        }


        #region CyclerStatusCheck()
        private bool CyclerStatusCheck()
        {
            if (!cycler.isAlive1)
            {
                cycler._Dispose();
                cycler = null;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    cycler = new CCycler(this, this.can_cycler1);
                }));
                Thread.Sleep(5000);
            }

            if (cycler != null && cycler.isAlive1)
            {
                if (cycler.bmsList.Count > 0)
                {
                    if (cycler.bmsList.ContainsKey("110h"))
                    {
                        if (cycler.bmsList["110h"].Data.Contains("00 00 00 00 00 00"))
                        {
                            if (cycler.bmsList["110h"].Data.Contains("90 ")) { }
                            else if (cycler.bmsList["110h"].Data.Contains("91 ")) { } 
                            else
                            {
                                #region input mc on logic(191229)

                                LogState(LogType.Info, "Try to Cycler [INPUT_MC_ON]");

                                cycler.SendToDSP1("100", new byte[] { 0x80, 0x00, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 });

                                #region Changed logic 200107
                                LogState(LogType.Info, "Wait to INPUT MC ON ( Max 30sec )");
                                int cnt = 0;
                                while (cnt < 30000)
                                {
                                    cnt += 100;
                                    Thread.Sleep(100);

                                    if (cycler.bmsList["110h"].Data.Contains("90 10 00 00 00 00 00 00"))
                                    {
                                        LogState(LogType.Info, "INPUT MC ON in " + cnt.ToString() + "msec");
                                        break;
                                    }
                                    else if (cycler.bmsList["110h"].Data.Contains("90 11 00 00 00 00 00 00"))
                                    {
                                        LogState(LogType.Info, "INPUT MC ON in " + cnt.ToString() + "msec");
                                        break;
                                    }
                                }
                                #endregion

                                if (cycler.bmsList["110h"].Data.Contains("90 ")) { }
                                else if (cycler.bmsList["110h"].Data.Contains("91 ")) { }
                                else if (cycler.bmsList["110h"].Data.Contains("94 ")) { }
                                else
                                {
                                    LogState(LogType.Info, "Cycler is suspended(0x110)!");
                                    LogState(LogType.Info, "Receive Data: " + cycler.bmsList["110h"].Data.ToString());

                                    return false;
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            LogState(LogType.Info, "Cycler is suspended(0x110)!");
                            LogState(LogType.Info, "Receive Data: " + cycler.bmsList["110h"].Data.ToString());

                            return false;
                        }
                    }

                    if (cycler.bmsList.ContainsKey("120h"))
                    {
                        if (!(cycler.bmsList["120h"].Data.Remove(0, 6).Contains("00 00 00 00 00 00")))
                        {
                            LogState(LogType.Info, "Cycler is suspended(0x120)!");
                            LogState(LogType.Info, "Receive Data: " + cycler.bmsList["120h"].Data.ToString());

                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }

                var nowCycler = cycler.localDt;
                nowCycler = nowCycler.AddSeconds(5);

                if (DateTime.Now > nowCycler)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        private void TempCheck(TestItem ti)
        {
            string checkDeviceName = "[TEMP]";
            var now_plus_10s_dt = temps.localDt1;
            now_plus_10s_dt = now_plus_10s_dt.AddSeconds(10);

            if (DateTime.Now > now_plus_10s_dt)
            {
                LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! retry connection");

                temps = null;
                this.Dispatcher.Invoke(new Action(() =>
                {
                    temps = new CNhtRS232_Receiver(this);
                }));

                Thread.Sleep(100);

                now_plus_10s_dt = temps.localDt1;
                now_plus_10s_dt = now_plus_10s_dt.AddSeconds(10);
                if (DateTime.Now > now_plus_10s_dt)
                {
                    LogState(LogType.TEST, checkDeviceName + " ReConnection FAIL !! Restart EOL Program");
                    ViewRetryWindow(checkDeviceName, ti);
                }
                else
                    LogState(LogType.TEST, checkDeviceName + " TEST :" + temps.tempStr.ToString());
            }
            else
            {
                LogState(LogType.TEST, checkDeviceName + " TEST :" + temps.tempStr.ToString());
            }
        }

        private void DMMCheck(TestItem ti, int type)
        {
            string checkDeviceName = "[DMM]";
            //0이면 TCP, 1이면 Serial
            if (type == 0)
            {
                DMMTCPCheck(ti, checkDeviceName);
            }
            else if(type == 1)
            {
                DMMSerialCheck(ti, checkDeviceName);
            }
            
        }
        /// <summary>
        /// DMM TCP Type Connect Check
        /// </summary>
        /// <param name="ti"></param>
        /// <param name="deviceName"></param>
        private void DMMTCPCheck(TestItem ti, string deviceName)
        {
            if (contBt_daq970_1 != null && keysight.isAlive)
            {
                float res = keysight.TrySend("MEAS:VOLT:DC?\n");
                if (res != 0)
                    LogState(LogType.TEST, deviceName + " TEST :" + res);
                else
                {
                    LogState(LogType.TEST, deviceName + " Communication FAIL !! retry connection");                    
                    keysight = null;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        keysight = new CKeysightDMM(this, "169.254.4.61", 5025);
                    }));
                    Thread.Sleep(300);

                    res = keysight.TrySend("MEAS:VOLT:DC?\n");
                    if (res != 0)
                        LogState(LogType.TEST, deviceName + " TEST :" + res);
                    else
                    {
                        LogState(LogType.TEST, deviceName + " retry connection Communication FAIL !! Restart EOL Program");
                        ViewRetryWindow(deviceName, ti);
                    }
                }
            }
            else
            {
                LogState(LogType.TEST, deviceName + " Connection FAIL !! retry connection");

                if (keysight != null && keysight.isAlive != true)
                {
                    LogState(LogType.TEST, deviceName + " Dispose resources");

                    keysight.Dispose();
                    keysight = null;
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    keysight = new CKeysightDMM(this, "169.254.4.61", 5025);
                }));
                Thread.Sleep(300);

                if (keysight != null && keysight.isAlive)
                {
                    float res = keysight.TrySend("MEAS:VOLT:DC?\n");
                    if (res != 0)
                        LogState(LogType.TEST, deviceName + " TEST :" + res);
                    else
                    {
                        LogState(LogType.TEST, deviceName + " retry connection Communication FAIL !! Restart EOL Program");
                        ViewRetryWindow(deviceName, ti);
                    }
                }
                else
                {
                    ViewRetryWindow(deviceName, ti);
                }
            }
        }
        /// <summary>
        /// DMM Serial Connect Check
        /// </summary>
        /// <param name="ti"></param>
        /// <param name="deviceName"></param>
        private void DMMSerialCheck(TestItem ti, string deviceName)
        {
            if (keysight != null && keysight.isAlive)
            {
                string res = keysight.Read_Version();
                if (res.Length > 0)
                    LogState(LogType.TEST, deviceName + " TEST :" + res);
                else
                {
                    LogState(LogType.TEST, deviceName + " Communication FAIL !! retry connection");
                    keysight.Dispose();
                    keysight = null;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        keysight = new CKeysightDMM(this, true, this.keysight_PortName);
                    }));
                    Thread.Sleep(300);

                    res = keysight.Read_Version();
                    if (res.Length > 0)
                        LogState(LogType.TEST, deviceName + " TEST :" + res);
                    else
                    {
                        LogState(LogType.TEST, deviceName + " retry connection Communication FAIL !! Restart EOL Program");
                        ViewRetryWindow(deviceName, ti);
                    }
                }
            }
            else
            {
                LogState(LogType.TEST, deviceName + " Connection FAIL !! retry connection");

                if (keysight != null && keysight.isAlive != true)
                {
                    LogState(LogType.TEST, deviceName + " Dispose resources");

                    keysight.Dispose();
                    keysight = null;
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    keysight = new CKeysightDMM(this, true, this.keysight_PortName);
                }));
                Thread.Sleep(300);

                if (keysight != null && keysight.isAlive)
                {
                    string res = keysight.Read_Version();
                    if (res.Length > 0)
                        LogState(LogType.TEST, deviceName + " TEST :" + res);
                    else
                    {
                        LogState(LogType.TEST, deviceName + " retry connection Communication FAIL !! Restart EOL Program");
                        ViewRetryWindow(deviceName, ti);
                    }
                }
                else
                {
                    ViewRetryWindow(deviceName, ti);
                }
            }
        }

        /// <summary>
        /// DAQ Connect Check
        /// </summary>
        /// <param name="ti">Testitem</param>
        private void DAQCheck(TestItem ti)
        {
            string checkDeviceName = "[DAQ]";
            if (daq != null && daq.sp.IsOpen)
            {
                var nowDaqDt = daq.localDt;
                //nowDaqDt = nowDaqDt.AddSeconds(2);
                //221101 DAQ 10s
                nowDaqDt = nowDaqDt.AddSeconds(10);

                if (DateTime.Now > nowDaqDt)
                {
                    //LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                    LogState(LogType.TEST, checkDeviceName + " Myunghwan Dispose resources"
    + DateTime.Now + "/" + nowDaqDt);
                    daq.sp.Close();
                    daq.sp.Dispose();
                    daq = null;
                    Thread.Sleep(100);
                    DAQErrcnt++;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        daq = new CDAQ(this, daq_PortName);
                    }));

                    Thread.Sleep(200);

                    if (daq != null && daq.sp.IsOpen)
                    {
                        nowDaqDt = daq.localDt;
                        //221101 DAQ 10s
                        nowDaqDt = nowDaqDt.AddSeconds(10);
                        //nowDaqDt = nowDaqDt.AddSeconds(2);

                        if (DateTime.Now > nowDaqDt)
                        {
                            DAQErrcnt++;
                            ViewRetryWindow(checkDeviceName, ti);
                        }
                        else
                        {
                            LogState(LogType.TEST, checkDeviceName + " TEST :" + daq.sp.ToString());
                        }

                    }
                }

                LogState(LogType.TEST, checkDeviceName + " TEST :" + daq.sp.ToString());
            }
            else
            {
                LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! retry connection");

                if (daq != null && daq.sp.IsOpen)
                {
                    LogState(LogType.TEST, checkDeviceName + " Dispose resources");
                    daq.sp.Close();
                    daq.sp.Dispose();
                    daq = null;
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    daq = new CDAQ(this, daq_PortName);
                }));

                if (daq != null && daq.sp.IsOpen)
                {
                    LogState(LogType.TEST, checkDeviceName + " TEST :" + daq.sp.ToString());
                }
                else
                {
                    ViewRetryWindow(checkDeviceName, ti);
                }
            }
            LogState(LogType.Info, "DAQ Error Count : " + DAQErrcnt);
        }
        /// <summary>
        /// Cycler Connect Check
        /// </summary>
        /// <param name="ti">Testitem</param>
        private void CyclerCheck(TestItem ti)
        {
            string checkDeviceName = "[CYCLER]";

            if (cycler != null && cycler.isAlive1)
            {
                cycler.m_LastMsgsList1.Clear();
                cycler.bmsList.Clear();
                Thread.Sleep(500);

                var nowCycler = cycler.localDt;
                nowCycler = nowCycler.AddSeconds(5);

                if (DateTime.Now > nowCycler)
                {
                    SetMainCState("NOT READY");
                    ViewRetryWindow(checkDeviceName, ti);
                }
                LogState(LogType.TEST, checkDeviceName + " TEST :" + cycler.cycler1OP.ToString());
            }
            else
            {
                SetMainCState("NOT READY");
                ViewRetryWindow(checkDeviceName, ti);
            }
        }
        /// <summary>
        /// Chroma Connect Check
        /// </summary>
        /// <param name="ti">Testitem</param>
        private void ChromaCheck(TestItem ti)
        {
            string checkDeviceName = "[Hipot]";
            int res = chroma.ChromaAliveCheck();
            if (res > 0)
            {
                LogState(LogType.TEST, checkDeviceName + " Hipot Conneted.");
            }
            else
            {
                LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! Restart EOL Program");
                ViewRetryWindow(checkDeviceName, ti);
            }
        }

        public bool isDeviceFail = false;
        public bool isAlarmStop = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkDeviceName">Device Name</param>
        /// <param name="ti">Testitem</param>
        private void ViewRetryWindow(string checkDeviceName, TestItem ti)
        {
            ti.Result = "NG";
            StringBuilder sb = new StringBuilder();
            sb.Append("Test :");
            sb.Append(ti.Name);
            sb.Append(" End - NG [Min:");
            sb.Append(ti.Min.ToString());
            sb.Append("][Value:");
            sb.Append(ti.Value_);
            sb.Append("][Max:");
            sb.Append(ti.Max.ToString());
            sb.Append("]");
            this.LogState(LogType.NG, sb.ToString());

            //ng스킵일때 계측기에서 멈춰도 계속 진행함
            if (isSkipNG_)
            {
                return;
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                DeviceCheckWindow rt = new DeviceCheckWindow(this);
                rt.Height += 100;
                rt.Width += 530;
                rt.maintitle.FontSize += 20;
                rt.maintitle.Content = "DEVICE NOT READY";
                rt.reason.FontSize += 13;
                rt.reason.Content = checkDeviceName + "\nis NOT ready to TEST !!\nPlease restart EOL Program";
                rt.okbt.FontSize += 13;
                rt.Show();
            }));

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (isManual)
                    blinder.Visibility = System.Windows.Visibility.Hidden;
            }));
        }
    }
}