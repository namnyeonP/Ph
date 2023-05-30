using Renault_BT6.모듈;
using Renault_BT6.윈도우;
using Renault_BT6.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renault_BT6.Forms;

namespace Renault_BT6
{
    public partial class MainWindow
    {
        List<string> ngDeviceList = new List<string>();

        string[] _clResultData = new string[30];

        // MES 상세 수집
        private void MesContorolValue(TestItem ti)
        {
            try
            {
                if (CONFIG.EolInspType == InspectionType.EOL)
                {
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.RestTimeBeforeDischarge,
                                                                befDiscRestTime));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.DischargeCurrent, discCur));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.DischargeTime, discTime));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.DischargeCurrentUpperLimit, discCurLimit));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.RestTimeAfterDischarge, aftDiscRestTime));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ChargeCurrent, charCur));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ChargeTime, charTime));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ChargeCurrentUpperLimit, aftCharRestTime));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.RestTimeafterCharge, charCurLimit));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.SafetyVoltageUpperLimit, safeVoltHighLimit));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.SafetyVoltageLowLimit, safeVoltLowLimit));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.CellDCIR1, cellFomula1.ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.CellDCIR2, cellFomula2.ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.CellDCIR3, cellFomula3.ToString()));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ModuleDCIR1, moduleFomula1.ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ModuleDCIR2, moduleFomula2.ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ModuleDCIR3, moduleFomula3.ToString()));

                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.CoumtCylcerMC, counter_Cycler.ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.PreCheckCyclerVolt, mFisrtPreCheckReportCyclerVolt.ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.PreCheckDMMSensingVolt, mFisrtPreCheckReportDMMSensingVolt.ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.PreCheckDMMProbeVolt, mFisrtPreCheckReportDMMProbeVolt.ToString()));

                    //MC 저항 기능 nnkim
                    //221017 바코드 리딩할때 저항 한번 찍음
                    var rtList = new List<string>();
                    McResistanceMeasure(mcResistanceposition, out rtList);

                    try
                    {
                        //221004 저항 측정 값 추가 Open 되었을 때
                        ti.refValues_.Add(MakeDetailItem(EOL.MesID.KEY_EOL_MC_3_OPEN_CHECK, rtList[0].ToString()));
                        ti.refValues_.Add(MakeDetailItem(EOL.MesID.KEY_EOL_MC_4_OPEN_CHECK, rtList[1].ToString()));
                        ti.refValues_.Add(MakeDetailItem(EOL.MesID.KEY_EOL_MC_5_OPEN_CHECK, rtList[2].ToString()));
                        ti.refValues_.Add(MakeDetailItem(EOL.MesID.KEY_EOL_MC_6_OPEN_CHECK, rtList[3].ToString()));
                        ti.refValues_.Add(MakeDetailItem(EOL.MesID.KEY_EOL_MC_7_OPEN_CHECK, rtList[4].ToString()));
                        ti.refValues_.Add(MakeDetailItem(EOL.MesID.KEY_EOL_MC_8_OPEN_CHECK, rtList[5].ToString()));
                        rtList.Clear();
                    }
                    catch
                    {
                        rtList.Clear();
                        LogState(LogType.Info, "Please MC ResistanceCable Check");
                    }
                }
                else if (CONFIG.EolInspType == InspectionType.HIPOT)
                {

                    // IR Plus Bushing 
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.BushingPlusVolt, 
                                                                EOL.IR_Recipe.BushingPlus.Volt.ToString() ));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.BushingPlusTime,
                                                                EOL.IR_Recipe.BushingPlus.Time.ToString() ));

                    // IR Plus Cooling 
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.CoolingPlusVolt,
                                                            EOL.IR_Recipe.CoolingPlatePlus.Volt.ToString()));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.CoolingPlusTime,
                                                                EOL.IR_Recipe.CoolingPlatePlus.Time.ToString()));

                    foreach (string id in EOL.IR_CtrlItem)
                    {
                        if(MesCollectionID.IR_Minus_Bushing == id)
                        {
                            ti.refValues_.Add(MakeDetailItem(EOL.MesID.BushingMinusVolt,
                                                  EOL.IR_Recipe.BushingMinus.Volt.ToString()));
                            ti.refValues_.Add(MakeDetailItem(EOL.MesID.BushingMinusTime,
                                                                        EOL.IR_Recipe.BushingMinus.Time.ToString()));
                        }
                        else if (MesCollectionID.IR_Minus_Cooling_Plate == id)
                        {
                            ti.refValues_.Add(MakeDetailItem(EOL.MesID.CoolingMinusVolt,
                                                 EOL.IR_Recipe.CoolingPlateMinus.Volt.ToString()));
                            ti.refValues_.Add(MakeDetailItem(EOL.MesID.CoolingMinusTime,
                                                                        EOL.IR_Recipe.CoolingPlateMinus.Time.ToString()));
                        }
                    }

                }
            }
            catch
            {

            }
        }

        public void SetContorlLimitData(int index, string val)
        {
            try
            {
                _clResultData[index] = val;
            }
            catch
            {

            }
        }

        public bool BarcodeReading(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    ti.Value_ = modelList[selectedIndex].LotId = lotTb.Text;
                    //49. 바코드 리딩 값 적용 구문 및 로그 추가
                    LogState(LogType.Info, "Value : " + ti.Value_);
                }));

                //2022.10.17소모품 카운트 하는 부분 nnkim
                Protection_PartsCountStart = new Thread(() =>
                {
                    if ((int)CONFIG.EolInspType == (int)InspectionType.EOL)
                    {
                        FormPartsCountSetting PCS = new FormPartsCountSetting(this, "START");
                        PCS.CountRealData();
                        this.SetPartsCountData(ti, "StartRun");
                    }
                });
                Protection_PartsCountStart.Start();

                deviceStatus = "";
                var checkDeviceName = "";
                
                // 배열 초기화
                _clResultData = Enumerable.Repeat("", 30).ToArray();

                #region Device Connecting Check

                ngDeviceList.Clear();

                #region 공통 부분 
                #region PLC Check
                checkDeviceName = "[PLC]";
                if (Device.PLC != null && !Device.PLC.isAlive)
                {
                    LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                    ngDeviceList.Add(checkDeviceName);
                }
                else
                {
                    LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                }
                #endregion

                #region MES Check
                if(IsMESskip != true)
                {
                    MesConnectCheck();

                    checkDeviceName = "[MES]";
                    if (Device.MES != null && !Device.MES.IsMESConnected)
                    {
                        LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                        ngDeviceList.Add(checkDeviceName);
                    }
                    else
                    {
                        if (Device.MES.SendHeartbeating() == true)
                        {
                            LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                        }
                        else
                        {
                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                            ngDeviceList.Add(checkDeviceName);
                        }
                    }
                }
                #endregion
                
                #endregion

                #region Chroma Check - HIPOT 전용
                if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    checkDeviceName = "[HIPOT]";
                    if (Device.Chroma.ReadVersion() != "")
                    {
                        LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                    }
                    else
                    {
                        Device.Chroma.Close();
                        Thread.Sleep(200);
                        Device.Chroma.Open();

                        if (Device.Chroma.Alive == true)
                        {
                            LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                        }
                        else
                        {
                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                            ngDeviceList.Add(checkDeviceName);
                        }
                    }

                    #region TEMP Check
                    checkDeviceName = "[TEMP]";
                    if (Device.Tempr.GetTemp != -255)
                    //if(Device.TemprCT100.GetTemprature() != -255)
                    {
                        LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                    }
                    else
                    {
                        //try
                        //{
                        //    Device.TemprCT100.ResetDevice(EventUITemp);

                        //    if (Device.TemprCT100.GetOpenState() == true /* && Device.TemprCT100.GetTemprature() != -255*/)
                        //    {
                        //        DateTime waitTime = DateTime.Now.AddSeconds(3);

                        //        while (true)
                        //        {
                        //            if (DateTime.Now.Ticks > waitTime.Ticks)
                        //            {
                        //                throw new Exception();
                        //            }

                        //            if (Device.TemprCT100.GetTemprature() != -255)
                        //            {
                        //                LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);

                        //                break;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        throw new Exception();
                        //    }
                        //}
                        //catch
                        //{
                        //    LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);

                        //    ngDeviceList.Add(checkDeviceName);
                        //}


                        Device.Tempr.Close();
                        Thread.Sleep(100);

                        Device.Tempr.Open();
                        int time = 0;
                        while (true)
                        {
                            time += 100;
                            Thread.Sleep(100);

                            if (Device.Tempr.GetTemp > -255)
                                break;
                            else if (time > 4000)
                                break;
                        }

                        Thread.Sleep(100);
                        if (Device.Tempr.GetTemp == -255)
                        {
                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                            ngDeviceList.Add(checkDeviceName);
                        }
                        else
                            LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);

                    }
                    #endregion
                }
                #endregion
                else if (CONFIG.EolInspType == InspectionType.EOL)
                {
                    #region Cycler Check
                    checkDeviceName = "[CYCLER]";
                    if (Device.Cycler.GetDspStatus() != true)
                    {
                        SetMainCState("NOT READY");

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            contBt_cycler.Background = System.Windows.Media.Brushes.Red;
                        }));

                        LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                        ngDeviceList.Add(checkDeviceName);
                    }
                    else
                    {
                        if(Device.Cycler.OnInputMCAfterStatusCheck() == false)
                        {
                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName + "Fail On Input MC");

                            ngDeviceList.Add(checkDeviceName);
                        }
                        else
                        {
                            // OK

                            LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                        }
                    }
                    #endregion

                    #region TEMP Check
                    checkDeviceName = "[TEMP]";
                    //if (Device.Tempr.GetTemp != -255)
                    if (Device.TemprCT100.GetTemprature() != -255)
                    {
                        LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                    }
                    else 
                    {
                        try
                        {
                            Device.TemprCT100.ResetDevice(EventUITemp);

                            if (Device.TemprCT100.GetOpenState() == true /* && Device.TemprCT100.GetTemprature() != -255*/)
                            {
                                DateTime waitTime = DateTime.Now.AddSeconds(3);

                                while(true)
                                {
                                    if(DateTime.Now.Ticks > waitTime.Ticks)
                                    {
                                        throw new Exception();
                                    }

                                    if(Device.TemprCT100.GetTemprature() != -255)
                                    {
                                        LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);

                                        break;
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        catch
                        {
                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);

                            ngDeviceList.Add(checkDeviceName);
                        }

                        /*
                        Device.Tempr.Close();
                        Thread.Sleep(100);

                        Device.Tempr.Open();
                        int time = 0;
                        while(true) {
                            time += 100;
                            Thread.Sleep(100);

                            if (Device.Tempr.GetTemp > -255)
                                break;
                            else if (time > 3000)
                                break;
                        }

                        Thread.Sleep(3000);
                        if (Device.Tempr.GetTemp == -255)
                        {
                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                            ngDeviceList.Add(checkDeviceName);
                        }
                        else
                            LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                        */
                    }
                    #endregion

                    #region DAQ Check
                    checkDeviceName = "[DAQ]";
                    if (Device.Cycler.DaqAlive)
                    {
                        if (Device.Cycler.DaqCellRead() == true)
                        {
                            LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                        }
                        else
                        {
                            Device.Cycler.DAQClose();
                            Thread.Sleep(200);

                            Device.Cycler.DAQOpen();
                            Thread.Sleep(500);

                            if (Device.Cycler.DaqAlive == true)
                            {
                                LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                            }
                            else
                            {
                                LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                                ngDeviceList.Add(checkDeviceName);
                            }
                        }
                    }
                    else // not open
                    {
                        Device.Cycler.DAQClose();
                        Thread.Sleep(200);

                        Device.Cycler.DAQOpen();
                        Thread.Sleep(500);

                        if (Device.Cycler.DaqCellRead() == true)
                        {
                            LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                        }
                        else
                        {
                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                            ngDeviceList.Add(checkDeviceName);
                        }
                    }
                    #endregion

                    #region Keysight34970A Check


                    checkDeviceName = "[DMM]";
                    if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                    {
                        string ret = Device.KeysightDAQ.IDN();

                        if (ret.Length > 10)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                            }));

                            LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
                        }
                        else
                        {
                            LogState(LogType.TEST, checkDeviceName + " Communication FAIL !! retry connection");
                            Device.KeysightDAQ.Dispose();
                            Device.KeysightDAQ = null;

                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                Device.KeysightDAQ = new CKeysightDMM(this, true, CONFIG.KeysightDAQ);
                            }));
                            Thread.Sleep(300);

                            ret = Device.KeysightDAQ.IDN();
                            if (ret.Length > 10)
                            {
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                                }));

                                LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
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
                    }
                    else
                    {
                        LogState(LogType.TEST, checkDeviceName + " Connection FAIL !! retry connection");

                        if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive != true)
                        {
                            LogState(LogType.TEST, checkDeviceName + " Dispose resources");

                            Device.KeysightDAQ.Dispose();
                            Device.KeysightDAQ = null;
                        }

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            Device.KeysightDAQ = new CKeysightDMM(this, true, CONFIG.KeysightDAQ);
                        }));
                        Thread.Sleep(300);

                        if (Device.KeysightDAQ != null && Device.KeysightDAQ.isAlive)
                        {
                            string ret = Device.KeysightDAQ.IDN();

                            if (ret.Length > 10)
                            {
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    contBt_dmm.Background = System.Windows.Media.Brushes.Green;
                                }));

                                LogState(LogType.Success, "[DEVICE_CHECK] " + checkDeviceName);
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
                                contBt_dmm.Background = System.Windows.Media.Brushes.Red;
                            }));
                            LogState(LogType.Fail, "[DEVICE_CHECK] " + checkDeviceName);
                            ngDeviceList.Add(checkDeviceName);
                        }
                    }

                    #endregion
                    #endregion
                }

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
            }
            catch
            {

            }

            MesContorolValue(ti);

            return JudgementTestItem(ti);
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
            int res = Device.Chroma.ReadVersion().Length;
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
                rt.Width += 200;
                rt.maintitle.FontSize += 20;
                rt.maintitle.Content = "DEVICE NOT READY";
                rt.reason.FontSize += 13;
                rt.reason.Content = checkDeviceName + " is NOT ready to TEST !!\nPlease restart EOL Program";
                rt.okbt.FontSize += 13;
                rt.Show();
            }));

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (IsManual)
                    blinder.Visibility = System.Windows.Visibility.Hidden;
            }));
        }
    }
}