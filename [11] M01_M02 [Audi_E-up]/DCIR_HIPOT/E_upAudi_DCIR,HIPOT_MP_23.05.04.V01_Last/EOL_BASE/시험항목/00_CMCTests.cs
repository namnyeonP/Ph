using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE
{

    public partial class MainWindow
    {

        public string sleepValue1 = "0015";
        public string sleepValue2 = "0016";
        public string sleepValue3 = "0017";
        public string sleepValue4 = "001A";
        public string sleepValue5 = "001B";


        public bool NormalSleep(TestItem ti)
        {
            isProcessingUI(ti);

            //210323 wjs add port & relay num info log
            LogState(LogType.Info, "CMC Port : " + cmc_PortName + " CMC Relay Number : " + CMC_Relay);

            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            if (isDoSleep)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (isManual)
                    {
                        blinder3.Visibility = blinder2.Visibility = blinder.Visibility = System.Windows.Visibility.Hidden;
                    }
                }));

                return false;
            }

            if (!isDoSleep)
                isDoSleep = true;

            cmc.CMC_Sleep(); //슬립날리고

            Thread.Sleep(500); //190417 수정됨 1000>500

            cmc.raw_cmcList.Clear(); //리스트 비운다음에

            Thread.Sleep(2000); //190417 수정됨 2000>1000 190419 수정됨 1000>2000

            var templist = new Dictionary<string, string>();
            var obj = new Object();
            // hsg 180912
            try
            {
                foreach (var ITEM in cmc.raw_cmcList)
                {
                    templist.Add(ITEM.Key, ITEM.Value);
                }
            }
            catch (Exception ex)
            {
                if (cmc.raw_cmcList.Count() == 0)
                {
                    LogState(LogType.Fail, "Exception!!!!!Not Received CMC Data");
                }
                LogState(LogType.Fail, ex.ToString());
            }

            try
            {
                lock (templist)
                {
                    foreach (var ITEM in templist)
                    {
                        LogState(LogType.Info, ITEM.Key + " - " + ITEM.Value);
                    }

                    string temp;
                    if (templist.TryGetValue("000A", out temp))
                    {
                        if (temp == "04")
                        {
                            ti.Value_ = 0;
                        }
                        else
                        {
                            ti.Value_ = 1;
                        }
                    }
                    else
                    {
                        LogState(LogType.Info, "NG!! REBOOT");

                        if (Reboot_CMC())
                        {
                            LogState(LogType.Info, "OK REBOOT");

                            cmc.CMC_Sleep(); //슬립날리고

                            Thread.Sleep(500); //190417 수정됨 1000>500

                            cmc.raw_cmcList.Clear(); //리스트 비운다음에

                            Thread.Sleep(2000); //190417 수정됨 2000>1000 190419 수정됨 1000>2000

                            if (templist.Count != 0)
                            {
                                templist.Clear();
                            }

                            try
                            {
                                foreach (var ITEM in cmc.raw_cmcList)
                                {
                                    templist.Add(ITEM.Key, ITEM.Value);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (cmc.raw_cmcList.Count() == 0)
                                {
                                    LogState(LogType.Fail, "Exception!!!!! Not Received CMC Data");
                                }
                                LogState(LogType.Fail, ex.ToString());
                            }

                            try
                            {
                                lock (templist)
                                {
                                    foreach (var ITEM in templist)
                                    {
                                        LogState(LogType.Info, ITEM.Key + " - " + ITEM.Value);
                                    }

                                    if (templist.TryGetValue("000A", out temp))
                                    {
                                        if (temp == "04")
                                        {
                                            ti.Value_ = 0;
                                        }
                                        else
                                        {
                                            ti.Value_ = 1;
                                        }
                                    }
                                    else
                                    {
                                        ti.Value_ = 1;
                                    }
                                }
                            }
                            catch (Exception ecc)
                            {
                                LogState(LogType.Fail, ecc.ToString());
                            }
                        }
                        else
                        {
                            LogState(LogType.Info, "FAIL REBOOT");

                            ti.Value_ = 1;
                        }
                    }
                    LogState(LogType.Info, "CMC Status : 000A - " + temp);
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.ToString());
            }
            return JudgementTestItem(ti);
        }
        //201222 wjs add for LV Retry about Mase, Pors N/M
        public bool NormalSleep_pre()
        {
            bool isSleep = false;

            //210323 wjs add port & relay num info log
            LogState(LogType.Info, "CMC Port : " + cmc_PortName + " CMC Relay Number : " + CMC_Relay);

            cmc.CMC_Sleep(); //슬립날리고

            Thread.Sleep(500); //190417 수정됨 1000>500

            cmc.raw_cmcList.Clear(); //리스트 비운다음에

            Thread.Sleep(2000); //190417 수정됨 2000>1000 190419 수정됨 1000>2000

            var templist = new Dictionary<string, string>();
            var obj = new Object();
            // hsg 180912
            try
            {
                foreach (var ITEM in cmc.raw_cmcList)
                {
                    templist.Add(ITEM.Key, ITEM.Value);
                }
            }
            catch (Exception ex)
            {
                if (cmc.raw_cmcList.Count() == 0)
                {
                    LogState(LogType.Fail, "Not Received CMC Data");
                }
                LogState(LogType.Fail, ex.ToString());
            }

            try
            {
                lock (templist)
                {
                    foreach (var ITEM in templist)
                    {
                        LogState(LogType.Info, ITEM.Key + " - " + ITEM.Value);
                    }

                    string temp;
                    if (templist.TryGetValue("000A", out temp))
                    {
                        if (temp == "04")
                        {
                            isSleep = true;
                        }
                        else
                        {
                            isSleep = false;
                        }
                    }
                    else
                    {
                        //ti.Value_ = 1;
                        LogState(LogType.Info, "NG!! REBOOT");


                        if (!Reboot_CMC())
                        {
                            cmc.CMC_Sleep(); //슬립날리고

                            Thread.Sleep(500); //190417 수정됨 1000>500

                            cmc.raw_cmcList.Clear(); //리스트 비운다음에

                            Thread.Sleep(2000); //190417 수정됨 2000>1000 190419 수정됨 1000>2000

                            templist.Clear();
                            try
                            {
                                foreach (var ITEM in cmc.raw_cmcList)
                                {
                                    templist.Add(ITEM.Key, ITEM.Value);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (cmc.raw_cmcList.Count() == 0)
                                {
                                    LogState(LogType.Fail, "Not Received CMC Data");
                                }
                                LogState(LogType.Fail, ex.ToString());
                            }

                            try
                            {
                                lock (templist)
                                {
                                    foreach (var ITEM in templist)
                                    {
                                        LogState(LogType.Info, ITEM.Key + " - " + ITEM.Value);
                                    }

                                    if (templist.TryGetValue("000A", out temp))
                                    {
                                        if (temp == "04")
                                        {
                                            isSleep = true;
                                        }
                                        else
                                        {
                                            isSleep = false;
                                        }
                                    }
                                    else
                                    {
                                        isSleep = false;
                                    }
                                }
                            }
                            catch (Exception ecc)
                            {
                                LogState(LogType.Fail, ecc.ToString());
                            }
                        }
                        else
                        {
                            isSleep = false;
                        }

                    }
                    LogState(LogType.Info, "CMC Status : 000A - " + temp);
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.Fail, ex.ToString());
            }
            return isSleep;
        }
        public string CalcurateHWVersion(double version)
        {
            var rtv = "0";
            if (version > 2719 && version < 3261)
            {
                rtv = "L";
            }
            else if (version > 15565 && version < 17203)
            {
                rtv = "M";
            }
            else if (version > 29507 && version < 30049)
            {
                rtv = "H";
            }
            if (rtv == "0")
            {
            }
            return rtv;
        }

        //190417 수정됨 1700>1200
        int CMC_WakeUp_after_sleep = 1200;
        //190417 수정됨 2500>500
        int CMC_V4328_after_sleep = 500;
        //190417 수정됨 800>500
        int CMC_GUID_after_sleep = 500;

        /// <summary>
        /// PreLV Check용 HW Version 함수
        /// </summary>
        /// <returns>
        /// true = Success / false = Fail
        /// </returns>
        public bool HW_Version_Pre()
        {
            string HW_Version = "484848";

            cmc.CMC_WakeUp();
            Thread.Sleep(CMC_WakeUp_after_sleep);
            cmc.CMC_V4328();
            Thread.Sleep(CMC_V4328_after_sleep);

            publicWatch = new Stopwatch();
            publicWatch.Start();

            //var data1 = CMClist[9];
            //var data2 = CMClist[10];
            //var data3 = CMClist[11];

            //210312 wjs change var to double
            double data1 = 0.0;
            double data2 = 0.0;
            double data3 = 0.0;

            //220314 Porsche FL 사용안함
            ////210312 wjs add PORSCHE_FACELIFT
            //if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            //{
            //    //AN0~AN2
            //    data1 = CMClist[8];
            //    data2 = CMClist[9];
            //    data3 = CMClist[10];
            //}
            //else
            {
                //AN1~AN3
                data1 = CMClist[9];
                data2 = CMClist[10];
                data3 = CMClist[11];
            }
            LogState(LogType.Info, "HW Value 1:" + data1 + "/HW Value 2:" + data2 + "/HW Value 3:" + data3);
            LogState(LogType.Info, "Calcurated Values:" + CalcurateHWVersion(data1) + CalcurateHWVersion(data2) + CalcurateHWVersion(data3));
            HW_Version = Encoding.ASCII.GetBytes(CalcurateHWVersion(data1))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data2))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data3))[0].ToString();

            string cmcversion;
            if (cmc.raw_cmcList.TryGetValue("0000", out cmcversion))
            {
                LogState(LogType.CMC_RECV, "CMC Version : " + cmcversion);
            }
            else
            {
                LogState(LogType.Fail, "CMC Version Read Fail");
            }
            //hwver ng, cmc reset
            if (HW_Version == "484848")
            {
                LogState(LogType.Fail, "CMC REBOOT 1st");
                if (Reboot_CMC())
                {
                    //220314 Porsche FL 사용안함
                    ////210312 wjs add PORSCHE_FACELIFT
                    //if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                    //{
                    //    //AN0~AN2
                    //    data1 = CMClist[8];
                    //    data2 = CMClist[9];
                    //    data3 = CMClist[10];
                    //}
                    //else
                    {
                        //AN1~AN3
                        data1 = CMClist[9];
                        data2 = CMClist[10];
                        data3 = CMClist[11];
                    }
                    LogState(LogType.Info, "HW Value 1:" + data1 + "/HW Value 2:" + data2 + "/HW Value 3:" + data3);
                    LogState(LogType.Info, "Calcurated Values:" + CalcurateHWVersion(data1) + CalcurateHWVersion(data2) + CalcurateHWVersion(data3));
                    HW_Version = Encoding.ASCII.GetBytes(CalcurateHWVersion(data1))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data2))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data3))[0].ToString();

                    cmcversion = "";
                    if (cmc.raw_cmcList.TryGetValue("0000", out cmcversion))
                    {
                        LogState(LogType.CMC_RECV, "CMC Version : " + cmcversion);
                    }
                    else
                    {
                        LogState(LogType.Fail, "CMC Version Read Fail");
                    }

                    if (HW_Version == "484848")
                    {
                        LogState(LogType.Fail, "CMC REBOOT 2nd");
                        if (Reboot_CMC())
                        {
                            //220314 Porsche FL 사용안함
                            ////210312 wjs add PORSCHE_FACELIFT
                            //if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                            //{
                            //    //AN0~AN2
                            //    data1 = CMClist[8];
                            //    data2 = CMClist[9];
                            //    data3 = CMClist[10];
                            //}
                            //else
                            {
                                //AN1~AN3
                                data1 = CMClist[9];
                                data2 = CMClist[10];
                                data3 = CMClist[11];
                            }
                            LogState(LogType.Info, "HW Value 1:" + data1 + "/HW Value 2:" + data2 + "/HW Value 3:" + data3);
                            LogState(LogType.Info, "Calcurated Values:" + CalcurateHWVersion(data1) + CalcurateHWVersion(data2) + CalcurateHWVersion(data3));
                            HW_Version = Encoding.ASCII.GetBytes(CalcurateHWVersion(data1))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data2))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data3))[0].ToString();

                            cmcversion = "";
                            if (cmc.raw_cmcList.TryGetValue("0000", out cmcversion))
                            {
                                LogState(LogType.CMC_RECV, "CMC Version : " + cmcversion);
                            }
                            else
                            {
                                LogState(LogType.Fail, "CMC Version Read Fail");
                            }

                            if (HW_Version == "484848")
                            {
                                return false;
                            }
                        }
                        else
                        {
                            isCMCRebootFail = true;
                            return false;
                        }
                    }
                }
                else
                {
                    isCMCRebootFail = true;
                    return false;
                }
            }
            return true;
        }

        public bool HW_Version(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            cmc.CMC_WakeUp();
            Thread.Sleep(CMC_WakeUp_after_sleep);
            cmc.CMC_V4328();
            Thread.Sleep(CMC_V4328_after_sleep);

            publicWatch = new Stopwatch();
            publicWatch.Start();

            #region GET GUID
            this.cmcGuid = "";
            //try 3 times
            cmc.CMC_GUID();
            Thread.Sleep(CMC_GUID_after_sleep);
            LogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);
            if (this.cmcGuid.Length != 16)
            {
                cmc.CMC_GUID();
                Thread.Sleep(CMC_GUID_after_sleep);
                LogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);
            }
            if (this.cmcGuid.Length != 16)
            {
                cmc.CMC_GUID();
                Thread.Sleep(CMC_GUID_after_sleep);
                LogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);
            }
            //Thread.Sleep(1800); //190417 수정됨 1800>0
            //LogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);

            ti.refValues_.Clear();
            //201217 wjs add maserati
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DATA__CMC_GUID, this.cmcGuid));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_DATA__CMC_GUID, this.cmcGuid));
            }
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DATA__CMC_GUID, this.cmcGuid));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_DATA__CMC_GUID, this.cmcGuid));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DATA__CMC_GUID, this.cmcGuid));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_DATA__CMC_GUID, this.cmcGuid));
            }
            //221101 wjs add mase m183
            else if(localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DATA__CMC_GUID, this.cmcGuid));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_DATA__CMC_GUID, this.cmcGuid));
            }
            #endregion

            //cmc.CMC_WakeUp();

            //var data1 = CMClist[9];
            //var data2 = CMClist[10];
            //var data3 = CMClist[11];

            //210312 wjs change var to double
            double data1 = 0.0;
            double data2 = 0.0;
            double data3 = 0.0;

            //220314 Porsche FL 사용안함
            ////210312 wjs add PORSCHE_FACELIFT
            //if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            //{
            //    //AN0~AN2
            //    data1 = CMClist[8];
            //    data2 = CMClist[9];
            //    data3 = CMClist[10];
            //}
            //else
            {
                //AN1~AN3
                data1 = CMClist[9];
                data2 = CMClist[10];
                data3 = CMClist[11];
            }
            
            LogState(LogType.Info, "HW Value 1:" + data1 + "/HW Value 2:" + data2 + "/HW Value 3:" + data3);
            LogState(LogType.Info, "Calcurated Values:" + CalcurateHWVersion(data1) + CalcurateHWVersion(data2) + CalcurateHWVersion(data3));
            ti.Value_ = Encoding.ASCII.GetBytes(CalcurateHWVersion(data1))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data2))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data3))[0].ToString();

            string cmcversion;
            if (cmc.raw_cmcList.TryGetValue("0000", out cmcversion))
            {
                LogState(LogType.CMC_RECV, "CMC Version : " + cmcversion);
                //201217 wjs add maserati
                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CMC____VERSION, cmcversion));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_CMC____VERSION, cmcversion));
                }
                else if (localTypes == ModelType.MASERATI_NORMAL)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_CMC____VERSION, cmcversion));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_CMC____VERSION, cmcversion));
                }
                //210312 WJS ADD PORS FL
                else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_CMC____VERSION, cmcversion));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_CMC____VERSION, cmcversion));
                }
                //if (pro_Type == ProgramType.EOLInspector)
                //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CMC____VERSION, cmcversion));
                //else if (pro_Type == ProgramType.VoltageInspector)
                //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_CMC____VERSION, cmcversion));

                //221101 wjs add mase m183
                else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_CMC____VERSION, cmcversion));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_CMC____VERSION, cmcversion));
                }
            }
            else
            {
                LogState(LogType.Fail, "CMC Version Read Fail");
            }

            //hwver ng, cmc reset
            if (ti.Value_.ToString() == "484848")
            {
                LogState(LogType.Fail, "CMC REBOOT 1st");
                if (Reboot_CMC())
                {
                    #region GET GUID
                    this.cmcGuid = "";
                    //try 3 times
                    cmc.CMC_GUID();
                    Thread.Sleep(CMC_GUID_after_sleep);
                    LogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);
                    if (this.cmcGuid.Length != 16)
                    {
                        cmc.CMC_GUID();
                        Thread.Sleep(CMC_GUID_after_sleep);
                        LogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);
                    }
                    if (this.cmcGuid.Length != 16)
                    {
                        cmc.CMC_GUID();
                        Thread.Sleep(CMC_GUID_after_sleep);
                        LogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);
                    }
                    //Thread.Sleep(1800); //190417 수정됨 1800>0
                    //ogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);

                    ti.refValues_.Clear();
                    //201217 wjs add maserati
                    if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                    {
                        if (pro_Type == ProgramType.EOLInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DATA__CMC_GUID, this.cmcGuid));
                        else if (pro_Type == ProgramType.VoltageInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_DATA__CMC_GUID, this.cmcGuid));
                    }
                    else if (localTypes == ModelType.MASERATI_NORMAL)
                    {
                        if (pro_Type == ProgramType.EOLInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DATA__CMC_GUID, this.cmcGuid));
                        else if (pro_Type == ProgramType.VoltageInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_DATA__CMC_GUID, this.cmcGuid));
                    }
                    //210312 WJS ADD PORS FL
                    else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                    {
                        if (pro_Type == ProgramType.EOLInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DATA__CMC_GUID, this.cmcGuid));
                        else if (pro_Type == ProgramType.VoltageInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_DATA__CMC_GUID, this.cmcGuid));
                    }
                    //221101 wjs add mase m183
                    else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                    {
                        if (pro_Type == ProgramType.EOLInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DATA__CMC_GUID, this.cmcGuid));
                        else if (pro_Type == ProgramType.VoltageInspector)
                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_DATA__CMC_GUID, this.cmcGuid));
                    }
                    #endregion

                    //data1 = CMClist[9];
                    //data2 = CMClist[10];
                    //data3 = CMClist[11];

                    //220314 Porsche FL 사용안함
                    ////210312 wjs add PORSCHE_FACELIFT
                    //if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                    //{
                    //    //AN0~AN2
                    //    data1 = CMClist[8];
                    //    data2 = CMClist[9];
                    //    data3 = CMClist[10];
                    //}
                    //else
                    {
                        //AN1~AN3
                        data1 = CMClist[9];
                        data2 = CMClist[10];
                        data3 = CMClist[11];
                    }
                    LogState(LogType.Info, "HW Value 1:" + data1 + "/HW Value 2:" + data2 + "/HW Value 3:" + data3);
                    LogState(LogType.Info, "Calcurated Values:" + CalcurateHWVersion(data1) + CalcurateHWVersion(data2) + CalcurateHWVersion(data3));
                    ti.Value_ = Encoding.ASCII.GetBytes(CalcurateHWVersion(data1))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data2))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data3))[0].ToString();

                    cmcversion = "";
                    if (cmc.raw_cmcList.TryGetValue("0000", out cmcversion))
                    {
                        LogState(LogType.CMC_RECV, "CMC Version : " + cmcversion);
                        //201217 wjs add maserati
                        if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                        {
                            if (pro_Type == ProgramType.EOLInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CMC____VERSION, cmcversion));
                            else if (pro_Type == ProgramType.VoltageInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_CMC____VERSION, cmcversion));
                        }
                        else if (localTypes == ModelType.MASERATI_NORMAL)
                        {
                            if (pro_Type == ProgramType.EOLInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_CMC____VERSION, cmcversion));
                            else if (pro_Type == ProgramType.VoltageInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_CMC____VERSION, cmcversion));
                        }
                        //210312 WJS ADD PORS FL
                        else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                        {
                            if (pro_Type == ProgramType.EOLInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_CMC____VERSION, cmcversion));
                            else if (pro_Type == ProgramType.VoltageInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_CMC____VERSION, cmcversion));
                        }
                        //221101 wjs add mase m183
                        else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                        {
                            if (pro_Type == ProgramType.EOLInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_CMC____VERSION, cmcversion));
                            else if (pro_Type == ProgramType.VoltageInspector)
                                ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_CMC____VERSION, cmcversion));
                        }
                        //if (pro_Type == ProgramType.EOLInspector)
                        //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CMC____VERSION, cmcversion));
                        //else if (pro_Type == ProgramType.VoltageInspector)
                        //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_CMC____VERSION, cmcversion));
                    }
                    else
                    {
                        LogState(LogType.Fail, "CMC Version Read Fail");
                    }

                    if (ti.Value_.ToString() == "484848")
                    {
                        LogState(LogType.Fail, "CMC REBOOT 2nd");
                        if (Reboot_CMC())
                        {
                            #region GET GUID
                            this.cmcGuid = "";
                            //try 3 times
                            cmc.CMC_GUID();
                            Thread.Sleep(CMC_GUID_after_sleep);
                            LogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);
                            if (this.cmcGuid.Length != 16)
                            {
                                cmc.CMC_GUID();
                                Thread.Sleep(CMC_GUID_after_sleep);
                                LogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);
                            }
                            if (this.cmcGuid.Length != 16)
                            {
                                cmc.CMC_GUID();
                                Thread.Sleep(CMC_GUID_after_sleep);
                                LogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);
                            }
                            //Thread.Sleep(1800); //190417 수정됨 1800>0
                            //LogState(LogType.Info, "GUID :(STRING)" + this.cmcGuid);

                            ti.refValues_.Clear();
                            //201217 wjs add maserati
                            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                            {
                                if (pro_Type == ProgramType.EOLInspector)
                                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_DATA__CMC_GUID, this.cmcGuid));
                                else if (pro_Type == ProgramType.VoltageInspector)
                                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_DATA__CMC_GUID, this.cmcGuid));
                            }
                            else if (localTypes == ModelType.MASERATI_NORMAL)
                            {
                                if (pro_Type == ProgramType.EOLInspector)
                                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_DATA__CMC_GUID, this.cmcGuid));
                                else if (pro_Type == ProgramType.VoltageInspector)
                                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_DATA__CMC_GUID, this.cmcGuid));
                            }
                            //210312 WJS ADD PORS FL
                            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                            {
                                if (pro_Type == ProgramType.EOLInspector)
                                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_DATA__CMC_GUID, this.cmcGuid));
                                else if (pro_Type == ProgramType.VoltageInspector)
                                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_DATA__CMC_GUID, this.cmcGuid));
                            }
                            //221101 wjs add mase m183
                            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                            {
                                if (pro_Type == ProgramType.EOLInspector)
                                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_DATA__CMC_GUID, this.cmcGuid));
                                else if (pro_Type == ProgramType.VoltageInspector)
                                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_DATA__CMC_GUID, this.cmcGuid));
                            }
                            #endregion

                            //data1 = CMClist[9];
                            //data2 = CMClist[10];
                            //data3 = CMClist[11];

                            //220314 Porsche FL 사용안함
                            ////210312 wjs add PORSCHE_FACELIFT
                            //if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                            //{
                            //    //AN0~AN2
                            //    data1 = CMClist[8];
                            //    data2 = CMClist[9];
                            //    data3 = CMClist[10];
                            //}
                            //else
                            {
                                //AN1~AN3
                                data1 = CMClist[9];
                                data2 = CMClist[10];
                                data3 = CMClist[11];
                            }

                            LogState(LogType.Info, "HW Value 1:" + data1 + "/HW Value 2:" + data2 + "/HW Value 3:" + data3);
                            LogState(LogType.Info, "Calcurated Values:" + CalcurateHWVersion(data1) + CalcurateHWVersion(data2) + CalcurateHWVersion(data3));
                            ti.Value_ = Encoding.ASCII.GetBytes(CalcurateHWVersion(data1))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data2))[0].ToString() + Encoding.ASCII.GetBytes(CalcurateHWVersion(data3))[0].ToString();

                            cmcversion = "";
                            if (cmc.raw_cmcList.TryGetValue("0000", out cmcversion))
                            {
                                LogState(LogType.CMC_RECV, "CMC Version : " + cmcversion);
                                //201217 wjs add maserati
                                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                                {
                                    if (pro_Type == ProgramType.EOLInspector)
                                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CMC____VERSION, cmcversion));
                                    else if (pro_Type == ProgramType.VoltageInspector)
                                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_CMC____VERSION, cmcversion));
                                }
                                else if (localTypes == ModelType.MASERATI_NORMAL)
                                {
                                    if (pro_Type == ProgramType.EOLInspector)
                                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_CMC____VERSION, cmcversion));
                                    else if (pro_Type == ProgramType.VoltageInspector)
                                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_CMC____VERSION, cmcversion));
                                }
                                //210312 WJS ADD PORS FL
                                else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                                {
                                    if (pro_Type == ProgramType.EOLInspector)
                                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_CMC____VERSION, cmcversion));
                                    else if (pro_Type == ProgramType.VoltageInspector)
                                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_CMC____VERSION, cmcversion));
                                }
                                //221101 wjs add mase m183
                                else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                                {
                                    if (pro_Type == ProgramType.EOLInspector)
                                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_CMC____VERSION, cmcversion));
                                    else if (pro_Type == ProgramType.VoltageInspector)
                                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_CMC____VERSION, cmcversion));
                                }
                                //if (pro_Type == ProgramType.EOLInspector)
                                //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_CMC____VERSION, cmcversion));
                                //else if (pro_Type == ProgramType.VoltageInspector)
                                //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_CMC____VERSION, cmcversion));
                            }
                            else
                            {
                                LogState(LogType.Fail, "CMC Version Read Fail");
                            }
                        }
                        else
                        {
                            ti.Value_ = "CMC_OPEN_FAIL";
                            return false;
                        }
                    }
                }
                else
                {
                    ti.Value_ = "CMC_OPEN_FAIL";
                    return false;
                }
            }

            return JudgementTestItem(ti);
        }

        public bool BalanceSwitchOpen(TestItem ti)
        {
            isProcessingUI(ti);

            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            #region 190417 없앰 
            //cmc.CMC_WATCHDOG_RESET();
            //Thread.Sleep(3500);
            #endregion
            cmc.CMC_OPEN();
            Thread.Sleep(2000);

            ti.refValues_.Clear();
            int ocnt = 0;
            string ostr = "";
            if (cmc.CB1_OPEN_FLT) { ocnt += 1; LogState(LogType.Info, "CB1_OPEN_FLT:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CB2_OPEN_FLT) { ocnt += 1; LogState(LogType.Info, "CB2_OPEN_FLT:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CB3_OPEN_FLT) { ocnt += 1; LogState(LogType.Info, "CB3_OPEN_FLT:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CB4_OPEN_FLT) { ocnt += 1; LogState(LogType.Info, "CB4_OPEN_FLT:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CB5_OPEN_FLT) { ocnt += 1; LogState(LogType.Info, "CB5_OPEN_FLT:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CB6_OPEN_FLT) { ocnt += 1; LogState(LogType.Info, "CB6_OPEN_FLT:1"); ostr += "1"; } else { ostr += "0"; };


            //201217 wjs add maserati
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_BAL__OP_SWITCH, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_BAL__OP_SWITCH, ostr.ToString()));
            }
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_BAL__OP_SWITCH, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_BAL__OP_SWITCH, ostr.ToString()));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_BAL__OP_SWITCH, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_BAL__OP_SWITCH, ostr.ToString()));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_BAL__OP_SWITCH, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_BAL__OP_SWITCH, ostr.ToString()));
            }
            //if (pro_Type == ProgramType.EOLInspector)
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_BAL__OP_SWITCH, ostr.ToString()));
            //else if (pro_Type == ProgramType.VoltageInspector)
            //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_BAL__OP_SWITCH, ostr.ToString()));

            LogState(LogType.Info, "balanceSwitch_open :" + ostr.ToString());// + "balanceSwitch_short :" + scnt.ToString());

            ti.Value_ = ocnt;// +scnt;            

            return JudgementTestItem(ti);
        }


        double cb1;
        double cb2;
        double cb3;
        double cb4;
        double cb5;
        double cb6;
         
        public bool BalanceSwitchShort1(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            cbshort1Volt = cbshort2Volt = cbshort3Volt = cbshort4Volt = cbshort5Volt = cbshort6Volt = -1;

            cmc.CMC_Short(true);
            publicWatch = new Stopwatch();
            publicWatch.Start();
            Thread.Sleep(300);
            cmc.CMC_Short(true);
            Thread.Sleep(1000);

            // 180422 Add Retry
            if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
            {
                Thread.Sleep(100);
                cmc.CMC_Short(true);
                Thread.Sleep(1000);

                if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                {
                    Thread.Sleep(100);
                    cmc.CMC_Short(true);
                    Thread.Sleep(1000);

                    if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                    {
                        Thread.Sleep(100);
                        cmc.CMC_Short(true);
                        Thread.Sleep(1000);
                        if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                        {
                            Thread.Sleep(100);
                            cmc.CMC_Short(true);
                            Thread.Sleep(1000);
                            if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                            {
                                Thread.Sleep(100);
                                cmc.CMC_Short(true);
                                Thread.Sleep(1000);
                            }
                        }
                    }
                }
            }

            //short ng cmc reset
            if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
            {
                LogState(LogType.Fail, "CMC REBOOT 1st");
                if (!Reboot_CMC())
                {
                    ti.Value_ = "CMC_OPEN_FAIL";
                    ti.Result = "NG";
                    this.LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                    if (isSkipNG_)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    //cmc.CMC_WakeUp();
                    //Thread.Sleep(CMC_WakeUp_after_sleep);
                    //cmc.CMC_V4328();
                    //Thread.Sleep(CMC_V4328_after_sleep);

                    cmc.CMC_Short(true);
                    publicWatch = new Stopwatch();
                    publicWatch.Start();
                    Thread.Sleep(300);
                    cmc.CMC_Short(true);
                    Thread.Sleep(1000);

                    // 180422 Add Retry
                    if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                    {
                        Thread.Sleep(100);
                        cmc.CMC_Short(true);
                        Thread.Sleep(1000);

                        if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                        {
                            Thread.Sleep(100);
                            cmc.CMC_Short(true);
                            Thread.Sleep(1000);

                            if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                            {
                                Thread.Sleep(100);
                                cmc.CMC_Short(true);
                                Thread.Sleep(1000);
                                if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                                {
                                    Thread.Sleep(100);
                                    cmc.CMC_Short(true);
                                    Thread.Sleep(1000);
                                    if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                                    {
                                        Thread.Sleep(100);
                                        cmc.CMC_Short(true);
                                        Thread.Sleep(1000);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
            {
                LogState(LogType.Fail, "CMC REBOOT 2nd");
                if (!Reboot_CMC())
                {
                    ti.Value_ = "CMC_OPEN_FAIL";
                    ti.Result = "NG";
                    this.LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                    if (isSkipNG_)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    //cmc.CMC_WakeUp();
                    //Thread.Sleep(CMC_WakeUp_after_sleep);
                    //cmc.CMC_V4328();
                    //Thread.Sleep(CMC_V4328_after_sleep);

                    cmc.CMC_Short(true);
                    publicWatch = new Stopwatch();
                    publicWatch.Start();
                    Thread.Sleep(300);
                    cmc.CMC_Short(true);
                    Thread.Sleep(1000);

                    // 180422 Add Retry
                    if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                    {
                        Thread.Sleep(100);
                        cmc.CMC_Short(true);
                        Thread.Sleep(1000);

                        if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                        {
                            Thread.Sleep(100);
                            cmc.CMC_Short(true);
                            Thread.Sleep(1000);

                            if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                            {
                                Thread.Sleep(100);
                                cmc.CMC_Short(true);
                                Thread.Sleep(1000);
                                if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                                {
                                    Thread.Sleep(100);
                                    cmc.CMC_Short(true);
                                    Thread.Sleep(1000);
                                    if (cbshort1Volt <= 0 || cbshort2Volt <= 0 || cbshort3Volt <= 0 || cbshort4Volt <= 0 || cbshort5Volt <= 0 || cbshort6Volt <= 0)
                                    {
                                        Thread.Sleep(100);
                                        cmc.CMC_Short(true);
                                        Thread.Sleep(1000);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            publicWatch.Stop();

            cb1 = cbshort1Volt * 0.1525925;
            LogState(LogType.Info, ti.Name + " - (" + cbshort1Volt + " * 0.1525925) = " + cb1);

            cb2 = cbshort2Volt * 0.1525925;
            cb3 = cbshort3Volt * 0.1525925;
            cb4 = cbshort4Volt * 0.1525925;
            cb5 = cbshort5Volt * 0.1525925;
            cb6 = cbshort6Volt * 0.1525925;

            ti.Value_ = cb1;
            return JudgementTestItem(ti);
        }

        public bool BalanceSwitchShort2(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cb2;
            LogState(LogType.Info, ti.Name + " - (" + cbshort2Volt + " * 0.1525925) = " + cb2);
            return JudgementTestItem(ti);
        }

        public bool BalanceSwitchShort3(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cb3;
            LogState(LogType.Info, ti.Name + " - (" + cbshort3Volt + " * 0.1525925) = " + cb3);
            return JudgementTestItem(ti);
        }

        public bool BalanceSwitchShort4(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cb4;
            LogState(LogType.Info, ti.Name + " - (" + cbshort4Volt + " * 0.1525925) = " + cb4);
            return JudgementTestItem(ti);
        }

        public bool BalanceSwitchShort5(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cb5;
            LogState(LogType.Info, ti.Name + " - (" + cbshort5Volt + " * 0.1525925) = " + cb5);
            return JudgementTestItem(ti);
        }

        public bool BalanceSwitchShort6(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = cb6;
            LogState(LogType.Info, ti.Name + " - (" + cbshort6Volt + " * 0.1525925) = " + cb6);
            return JudgementTestItem(ti);
        }

        public bool DaisyChain(TestItem ti)
        {
            isProcessingUI(ti);

            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            const int nTryCount = 3;
            
            for(int i=0; i<nTryCount; ++i)
            {
                LogState(LogType.Info, "TryCount : " + (i+1).ToString());

                cmc2cv1 = 0;
                
                cmc.CMC_ModeChange();

                int cnt = 0;

                while (cmc2cv1 == 0)
                {
                    Thread.Sleep(100);

                    cnt++;

                    if (cnt == 20)
                    {
                        break;
                    }
                }

                if (cmc2cv1 < 10000)
                {
                    Thread.Sleep(2000);
                }

                if (cmc2cv1 != 0)
                {
                    var raw = cmc2cv1;

                    ti.Value_ = raw * 0.0001525925;

                    LogState(LogType.Info, ti.Name + " - " + raw + " * 0.0001525925 = " + ti.Value_);
                }
                else
                {
                    ti.Value_ = "Not Changed";
                }
                
                cmc.CMC_ModeChange();

                Thread.Sleep(500);

                cmc1mv = 0;
                
                if(JudgementTestItem(ti) == true)
                {
                    LogState(LogType.Info, "Retry Judge Pass!");

                    break;
                }
                else
                {
                    LogState(LogType.Info, "Retry Judge NG!");
                    
                    Reboot_CMC();
                }
            }
            
            return JudgementTestItem(ti);
        }

        public bool OpenWireDetect1(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            ow1Volt = ow2Volt = ow3Volt = ow4Volt = ow5Volt = ow6Volt = 0;

            cmc.CMC_Wire(true);
            publicWatch = new Stopwatch();
            publicWatch.Start();
            Thread.Sleep(500);
            cmc.CMC_Wire(true);
            Thread.Sleep(500);

            // 180422 Add Retry
            if (ow1Volt < 100 || ow2Volt < 100 || ow3Volt < 100 || ow4Volt < 100 || ow5Volt < 100 || ow6Volt < 100)
            {
                cmc.CMC_Wire(true);
                Thread.Sleep(500);

                if (ow1Volt < 100 || ow2Volt < 100 || ow3Volt < 100 || ow4Volt < 100 || ow5Volt < 100 || ow6Volt < 100)
                {
                    cmc.CMC_Wire(true);
                    Thread.Sleep(500);

                    if (ow1Volt < 100 || ow2Volt < 100 || ow3Volt < 100 || ow4Volt < 100 || ow5Volt < 100 || ow6Volt < 100)
                    {
                        cmc.CMC_Wire(true);
                        Thread.Sleep(500);

                        if (ow1Volt < 100 || ow2Volt < 100 || ow3Volt < 100 || ow4Volt < 100 || ow5Volt < 100 || ow6Volt < 100)
                        {
                            LogState(LogType.Fail, "CMC REBOOT 1st");
                            if (!Reboot_CMC())
                            {
                                ti.Value_ = "CMC_OPEN_FAIL";
                                ti.Result = "NG";
                                this.LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                                if (isSkipNG_)
                                {
                                    return true;
                                }
                                return false;
                            }
                            else
                            {
                                //cmc.CMC_WakeUp();
                                //Thread.Sleep(CMC_WakeUp_after_sleep);
                                //cmc.CMC_V4328();
                                //Thread.Sleep(CMC_V4328_after_sleep);

                                if (ow1Volt < 100 || ow2Volt < 100 || ow3Volt < 100 || ow4Volt < 100 || ow5Volt < 100 || ow6Volt < 100)
                                {
                                    cmc.CMC_Wire(true);
                                    Thread.Sleep(500);

                                    if (ow1Volt < 100 || ow2Volt < 100 || ow3Volt < 100 || ow4Volt < 100 || ow5Volt < 100 || ow6Volt < 100)
                                    {
                                        cmc.CMC_Wire(true);
                                        Thread.Sleep(500);

                                        if (ow1Volt < 100 || ow2Volt < 100 || ow3Volt < 100 || ow4Volt < 100 || ow5Volt < 100 || ow6Volt < 100)
                                        {
                                            cmc.CMC_Wire(true);
                                            Thread.Sleep(500);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            publicWatch.Stop();


            ti.Value_ = ow1Volt * 0.1525925;
            LogState(LogType.Info, ti.Name + " - " + ow1Volt + " * 0.1525925 = " + ti.Value_);
            return JudgementTestItem(ti);
        }

        public bool OpenWireDetect2(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = ow2Volt * 0.1525925;
            LogState(LogType.Info, ti.Name + " - " + ow2Volt + " * 0.1525925 = " + ti.Value_);
            return JudgementTestItem(ti);
        }

        public bool OpenWireDetect3(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = ow3Volt * 0.1525925;
            LogState(LogType.Info, ti.Name + " - " + ow3Volt + " * 0.1525925 = " + ti.Value_);
            return JudgementTestItem(ti);
        }

        public bool OpenWireDetect4(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = ow4Volt * 0.1525925;
            LogState(LogType.Info, ti.Name + " - " + ow4Volt + " * 0.1525925 = " + ti.Value_);
            return JudgementTestItem(ti);
        }

        public bool OpenWireDetect5(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = ow5Volt * 0.1525925;
            LogState(LogType.Info, ti.Name + " - " + ow5Volt + " * 0.1525925 = " + ti.Value_);
            return JudgementTestItem(ti);
        }

        public bool OpenWireDetect6(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = ow6Volt * 0.1525925;
            LogState(LogType.Info, ti.Name + " - " + ow6Volt + " * 0.1525925 = " + ti.Value_);
            return JudgementTestItem(ti);
        }

        public bool OverVoltFault(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.refValues_.Clear();

            int ocnt = 0;
            string ostr = "";
            //if (cmc.CT0_OV) { ocnt += 1; LogState(LogType.Info, "CT0_OV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT1_OV) { ocnt += 1; LogState(LogType.Info, "CT1_OV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT2_OV) { ocnt += 1; LogState(LogType.Info, "CT2_OV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT3_OV) { ocnt += 1; LogState(LogType.Info, "CT3_OV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT4_OV) { ocnt += 1; LogState(LogType.Info, "CT4_OV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT5_OV) { ocnt += 1; LogState(LogType.Info, "CT5_OV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT6_OV) { ocnt += 1; LogState(LogType.Info, "CT6_OV:1"); ostr += "1"; } else { ostr += "0"; };

            //201217 wjs add maserati
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_OVER___V_FAULT, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_OVER___V_FAULT, ostr.ToString()));
            }
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_OVER___V_FAULT, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_OVER___V_FAULT, ostr.ToString()));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_OVER___V_FAULT, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_OVER___V_FAULT, ostr.ToString()));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_OVER___V_FAULT, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_OVER___V_FAULT, ostr.ToString()));
            }
            //if (pro_Type == ProgramType.EOLInspector)
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_OVER___V_FAULT, ostr.ToString()));
            //else if (pro_Type == ProgramType.VoltageInspector)
            //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_OVER___V_FAULT, ostr.ToString()));

            LogState(LogType.Info, "OverVoltageFault :" + ostr.ToString());
            ti.Value_ = ocnt;

            return JudgementTestItem(ti);
        }

        public bool UnderVoltFault(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.refValues_.Clear();

            int ocnt = 0;
            string ostr = "";
            //if (cmc.CT0_UV) { ocnt += 1; LogState(LogType.Info, "CT0_UV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT1_UV) { ocnt += 1; LogState(LogType.Info, "CT1_UV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT2_UV) { ocnt += 1; LogState(LogType.Info, "CT2_UV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT3_UV) { ocnt += 1; LogState(LogType.Info, "CT3_UV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT4_UV) { ocnt += 1; LogState(LogType.Info, "CT4_UV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT5_UV) { ocnt += 1; LogState(LogType.Info, "CT5_UV:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.CT6_UV) { ocnt += 1; LogState(LogType.Info, "CT6_UV:1"); ostr += "1"; } else { ostr += "0"; };

            //201217 wjs add maserati
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_UNDER__V_FAULT, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_UNDER__V_FAULT, ostr.ToString()));
            }
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_UNDER__V_FAULT, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_UNDER__V_FAULT, ostr.ToString()));
            }
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_UNDER__V_FAULT, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_UNDER__V_FAULT, ostr.ToString()));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_UNDER__V_FAULT, ostr.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_UNDER__V_FAULT, ostr.ToString()));
            }
            //if (pro_Type == ProgramType.EOLInspector)
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_UNDER__V_FAULT, ostr.ToString()));
            //else if (pro_Type == ProgramType.VoltageInspector)
            //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_UNDER__V_FAULT, ostr.ToString()));

            LogState(LogType.Info, "UnderVoltageFault :" + ostr.ToString());
            ti.Value_ = ocnt;

            return JudgementTestItem(ti);
        }

        string EvenOn = "999999";
        string EvenOff = "999999";
        string OddOn = "999999";
        string OddOff = "999999";

        double beforeCV1 = 0.0;
        double beforeCV2 = 0.0;
        double beforeCV3 = 0.0;
        double beforeCV4 = 0.0;
        double beforeCV5 = 0.0;
        double beforeCV6 = 0.0;


        double beforeCV1_origin = 0.0;
        double beforeCV2_origin = 0.0;
        double beforeCV3_origin = 0.0;
        double beforeCV4_origin = 0.0;
        double beforeCV5_origin = 0.0;
        double beforeCV6_origin = 0.0;

        double afterCV1 = 0.0;
        double afterCV2 = 0.0;
        double afterCV3 = 0.0;
        double afterCV4 = 0.0;
        double afterCV5 = 0.0;
        double afterCV6 = 0.0;


        double afterCV1_origin = 0.0;
        double afterCV2_origin = 0.0;
        double afterCV3_origin = 0.0;
        double afterCV4_origin = 0.0;
        double afterCV5_origin = 0.0;
        double afterCV6_origin = 0.0;

        public string EVEN_ON()
        {
            cmc.CV2_CB = 0.0;
            cmc.CV4_CB = 0.0;
            cmc.CV6_CB = 0.0;

            cmc.CMC_CEN();
            publicWatch = new Stopwatch();
            publicWatch.Start();
            LogState(LogType.CMC_RECV, "START/Time elapsed: " + publicWatch.Elapsed);
            int cnt = 0;
            while (!cmc.cbFlag)
            {
                cnt += 1;
                Thread.Sleep(1);
                if (cnt > commonTimeOut)
                {
                    LogState(LogType.Fail, "TIME OUT!! in ODD_OFF");
                    publicWatch.Stop();
                    return "999999";
                }
            }
            LogState(LogType.CMC_RECV, "END  /Time elapsed: " + publicWatch.Elapsed);
            publicWatch.Stop();
            Thread.Sleep(commonSleep);

            afterCV2_origin = cmc.CV2_CB;
            afterCV4_origin = cmc.CV4_CB;
            afterCV6_origin = cmc.CV6_CB;

            afterCV2 = afterCV2_origin * 0.0001525925;
            afterCV4 = afterCV4_origin * 0.0001525925;
            afterCV6 = afterCV6_origin * 0.0001525925;

            LogState(LogType.Info, "[AFTER]" + afterCV2_origin.ToString() + "(CMC_CV2) * 0.0001525925 = " + afterCV2.ToString());
            LogState(LogType.Info, "[AFTER]" + afterCV4_origin.ToString() + "(CMC_CV4) * 0.0001525925 = " + afterCV4.ToString());
            LogState(LogType.Info, "[AFTER]" + afterCV6_origin.ToString() + "(CMC_CV6) * 0.0001525925 = " + afterCV6.ToString());

            cmc.raw_cmcList.Remove("00BF");

            int ocnt = 0;
            string ostr = "";
            if (cmc.CT1_BAL) { ocnt += 1; LogState(LogType.Info, "CT1_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT1_BAL:0"); ostr += "0"; };
            if (cmc.CT2_BAL) { ocnt += 1; LogState(LogType.Info, "CT2_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT2_BAL:0"); ostr += "0"; };
            if (cmc.CT3_BAL) { ocnt += 1; LogState(LogType.Info, "CT3_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT3_BAL:0"); ostr += "0"; };
            if (cmc.CT4_BAL) { ocnt += 1; LogState(LogType.Info, "CT4_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT4_BAL:0"); ostr += "0"; };
            if (cmc.CT5_BAL) { ocnt += 1; LogState(LogType.Info, "CT5_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT5_BAL:0"); ostr += "0"; };
            if (cmc.CT6_BAL) { ocnt += 1; LogState(LogType.Info, "CT6_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT6_BAL:0"); ostr += "0"; };

            EvenOn = ostr;

            return ostr;
        }

        public string EVEN_OFF()
        {
            cmc.CMC_CEF();

            publicWatch = new Stopwatch();
            publicWatch.Start();
            LogState(LogType.CMC_RECV, "START/Time elapsed: " + publicWatch.Elapsed);
            int cnt = 0;
            while (cmc.cbFlag)
            {
                cnt += 1;
                Thread.Sleep(1);
                if (cnt > commonTimeOut)
                {
                    LogState(LogType.Fail, "TIME OUT!! in ODD_OFF");
                    publicWatch.Stop();
                    return "999999";
                }
            }
            LogState(LogType.CMC_RECV, "END  /Time elapsed: " + publicWatch.Elapsed);
            publicWatch.Stop();

            Thread.Sleep(commonSleep);

            int ocnt = 0;
            string ostr = "";
            if (cmc.CT1_BAL) { ocnt += 1; LogState(LogType.Info, "CT1_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT1_BAL:0"); ostr += "0"; };
            if (cmc.CT2_BAL) { ocnt += 1; LogState(LogType.Info, "CT2_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT2_BAL:0"); ostr += "0"; };
            if (cmc.CT3_BAL) { ocnt += 1; LogState(LogType.Info, "CT3_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT3_BAL:0"); ostr += "0"; };
            if (cmc.CT4_BAL) { ocnt += 1; LogState(LogType.Info, "CT4_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT4_BAL:0"); ostr += "0"; };
            if (cmc.CT5_BAL) { ocnt += 1; LogState(LogType.Info, "CT5_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT5_BAL:0"); ostr += "0"; };
            if (cmc.CT6_BAL) { ocnt += 1; LogState(LogType.Info, "CT6_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT6_BAL:0"); ostr += "0"; };

            EvenOff = ostr;

            return ostr;
        }

        public string ODD_ON()
        {
            cmc.CV1_CB = 0.0;
            cmc.CV3_CB = 0.0;
            cmc.CV5_CB = 0.0;

            cmc.CMC_CON();

            publicWatch = new Stopwatch();
            publicWatch.Start();
            LogState(LogType.CMC_RECV, "START/Time elapsed: " + publicWatch.Elapsed);
            int cnt = 0;
            while (!cmc.cbFlag)
            {
                cnt += 1;
                Thread.Sleep(1);
                if (cnt > commonTimeOut)
                {
                    LogState(LogType.Fail, "TIME OUT!! in ODD_OFF");
                    publicWatch.Stop();
                    return "999999";
                }
            }
            LogState(LogType.CMC_RECV, "END  /Time elapsed: " + publicWatch.Elapsed);
            publicWatch.Stop();

            Thread.Sleep(commonSleep);

            afterCV1_origin = cmc.CV1_CB;
            afterCV3_origin = cmc.CV3_CB;
            afterCV5_origin = cmc.CV5_CB;

            afterCV1 = afterCV1_origin * 0.0001525925;
            afterCV3 = afterCV3_origin * 0.0001525925;
            afterCV5 = afterCV5_origin * 0.0001525925;

            LogState(LogType.Info, "[AFTER]" + afterCV1_origin.ToString() + "(CMC_CV1) * 0.0001525925 = " + afterCV1.ToString());
            LogState(LogType.Info, "[AFTER]" + afterCV3_origin.ToString() + "(CMC_CV3) * 0.0001525925 = " + afterCV3.ToString());
            LogState(LogType.Info, "[AFTER]" + afterCV5_origin.ToString() + "(CMC_CV5) * 0.0001525925 = " + afterCV5.ToString());


            cmc.raw_cmcList.Remove("00BF");

            int ocnt = 0;
            string ostr = "";
            if (cmc.CT1_BAL) { ocnt += 1; LogState(LogType.Info, "CT1_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT1_BAL:0"); ostr += "0"; };
            if (cmc.CT2_BAL) { ocnt += 1; LogState(LogType.Info, "CT2_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT2_BAL:0"); ostr += "0"; };
            if (cmc.CT3_BAL) { ocnt += 1; LogState(LogType.Info, "CT3_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT3_BAL:0"); ostr += "0"; };
            if (cmc.CT4_BAL) { ocnt += 1; LogState(LogType.Info, "CT4_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT4_BAL:0"); ostr += "0"; };
            if (cmc.CT5_BAL) { ocnt += 1; LogState(LogType.Info, "CT5_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT5_BAL:0"); ostr += "0"; };
            if (cmc.CT6_BAL) { ocnt += 1; LogState(LogType.Info, "CT6_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT6_BAL:0"); ostr += "0"; };

            OddOn = ostr;

            return ostr;
        }

        public string ODD_OFF()
        {
            cmc.CMC_COF();
            publicWatch = new Stopwatch();
            publicWatch.Start();
            LogState(LogType.CMC_RECV, "START/Time elapsed: " + publicWatch.Elapsed);
            int cnt = 0;
            while (cmc.cbFlag)
            {
                cnt += 1;
                Thread.Sleep(1);
                if (cnt > commonTimeOut)
                {
                    LogState(LogType.Fail, "TIME OUT!! in ODD_OFF");
                    publicWatch.Stop();
                    return "999999";
                }
            }
            LogState(LogType.CMC_RECV, "END  /Time elapsed: " + publicWatch.Elapsed);
            publicWatch.Stop();

            Thread.Sleep(commonSleep);

            int ocnt = 0;
            string ostr = "";
            if (cmc.CT1_BAL) { ocnt += 1; LogState(LogType.Info, "CT1_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT1_BAL:0"); ostr += "0"; };
            if (cmc.CT2_BAL) { ocnt += 1; LogState(LogType.Info, "CT2_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT2_BAL:0"); ostr += "0"; };
            if (cmc.CT3_BAL) { ocnt += 1; LogState(LogType.Info, "CT3_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT3_BAL:0"); ostr += "0"; };
            if (cmc.CT4_BAL) { ocnt += 1; LogState(LogType.Info, "CT4_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT4_BAL:0"); ostr += "0"; };
            if (cmc.CT5_BAL) { ocnt += 1; LogState(LogType.Info, "CT5_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT5_BAL:0"); ostr += "0"; };
            if (cmc.CT6_BAL) { ocnt += 1; LogState(LogType.Info, "CT6_BAL:1"); ostr += "1"; } else { LogState(LogType.Info, "CT6_BAL:0"); ostr += "0"; };

            OddOff = ostr;

            return ostr;
        }

        int commonSleep = 200;
        int commonTimeOut = 3000;
        public bool CellEvenBalancingOn(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            #region Init Variables
            afterCV1 = afterCV1_origin = beforeCV1 = beforeCV1_origin = 0.0;
            afterCV2 = afterCV2_origin = beforeCV2 = beforeCV2_origin = 0.0;
            afterCV3 = afterCV3_origin = beforeCV3 = beforeCV3_origin = 0.0;
            afterCV4 = afterCV4_origin = beforeCV4 = beforeCV4_origin = 0.0;
            afterCV5 = afterCV5_origin = beforeCV5 = beforeCV5_origin = 0.0;
            afterCV6 = afterCV6_origin = beforeCV6 = beforeCV6_origin = 0.0;

            EvenOn = "999999";
            EvenOff = "999999";
            OddOn = "999999";
            OddOff = "999999";
            #endregion

            #region Get Before Voltages N Logging
            beforeCV1_origin = CMClist[1];
            beforeCV2_origin = CMClist[2];
            beforeCV3_origin = CMClist[3];
            beforeCV4_origin = CMClist[4];
            beforeCV5_origin = CMClist[5];
            beforeCV6_origin = CMClist[6];

            beforeCV1 = beforeCV1_origin * 0.0001525925;
            beforeCV2 = beforeCV2_origin * 0.0001525925;
            beforeCV3 = beforeCV3_origin * 0.0001525925;
            beforeCV4 = beforeCV4_origin * 0.0001525925;
            beforeCV5 = beforeCV5_origin * 0.0001525925;
            beforeCV6 = beforeCV6_origin * 0.0001525925;

            LogState(LogType.Info, "[BEFORE]" + beforeCV1_origin.ToString() + "(CMC_CV1) * 0.0001525925 = " + beforeCV1.ToString());
            LogState(LogType.Info, "[BEFORE]" + beforeCV2_origin.ToString() + "(CMC_CV2) * 0.0001525925 = " + beforeCV2.ToString());
            LogState(LogType.Info, "[BEFORE]" + beforeCV3_origin.ToString() + "(CMC_CV3) * 0.0001525925 = " + beforeCV3.ToString());
            LogState(LogType.Info, "[BEFORE]" + beforeCV4_origin.ToString() + "(CMC_CV4) * 0.0001525925 = " + beforeCV4.ToString());
            LogState(LogType.Info, "[BEFORE]" + beforeCV5_origin.ToString() + "(CMC_CV5) * 0.0001525925 = " + beforeCV5.ToString());
            LogState(LogType.Info, "[BEFORE]" + beforeCV6_origin.ToString() + "(CMC_CV6) * 0.0001525925 = " + beforeCV6.ToString());
            #endregion

            #region Even ON
            if (EVEN_ON() != "010101")
            {
                if (!Reboot_CMC())
                {
                    ti.Value_ = "CMC_OPEN_FAIL";
                    ti.Result = "NG";
                    this.LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                    if (isSkipNG_)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    if (EVEN_ON() != "010101")
                    {
                        if (!Reboot_CMC())
                        {
                            ti.Value_ = "CMC_OPEN_FAIL";
                            ti.Result = "NG";
                            this.LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                            if (isSkipNG_)
                            {
                                return true;
                            }
                            return false;
                        }
                        else
                        {
                            if (EVEN_ON() != "010101")
                            {
                                ti.Value_ = EvenOn;
                                ti.Result = "NG";
                                this.LogState(LogType.NG, "Test :" + ti.Name + " EVEN_ON");

                                if (isSkipNG_)
                                {
                                    return true;
                                }
                                return false;
                            }
                        }
                    }
                }
            }
            LogState(LogType.CMC_RECV, "00BF(RAW):" + cmc.rawbyte_bal);
            LogState(LogType.CMC_RECV, "00BF(REVERSED):" + cmc.rawbyte_bal_reverse);

            if (EVEN_OFF() != "000000")
            {
                if (!Reboot_CMC())
                {
                    ti.Value_ = "CMC_OPEN_FAIL";
                    ti.Result = "NG";
                    this.LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                    if (isSkipNG_)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    if (EVEN_OFF() != "000000")
                    {
                        if (!Reboot_CMC())
                        {
                            ti.Value_ = "CMC_OPEN_FAIL";
                            ti.Result = "NG";
                            this.LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                            if (isSkipNG_)
                            {
                                return true;
                            }
                            return false;
                        }
                        else
                        {
                            if (EVEN_OFF() != "000000")
                            {
                                ti.Value_ = EvenOff;
                                ti.Result = "NG";
                                this.LogState(LogType.NG, "Test :" + ti.Name + " EVEN_OFF");

                                if (isSkipNG_)
                                {
                                    return true;
                                }
                                return false;
                            }
                        }
                    }
                }
            }
            LogState(LogType.CMC_RECV, "00BF(RAW):" + cmc.rawbyte_bal);
            LogState(LogType.CMC_RECV, "00BF(REVERSED):" + cmc.rawbyte_bal_reverse);
            #endregion

            #region Odd ON
            if (ODD_ON() != "101010")
            {
                if (!Reboot_CMC())
                {
                    ti.Value_ = "CMC_OPEN_FAIL";
                    ti.Result = "NG";
                    this.LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                    if (isSkipNG_)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    if (ODD_ON() != "101010")
                    {
                        if (!Reboot_CMC())
                        {
                            ti.Value_ = "CMC_OPEN_FAIL";
                            ti.Result = "NG";
                            this.LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                            if (isSkipNG_)
                            {
                                return true;
                            }
                            return false;
                        }
                        else
                        {
                            if (ODD_ON() != "101010")
                            {
                                ti.Value_ = OddOn;
                                ti.Result = "NG";
                                this.LogState(LogType.NG, "Test :" + ti.Name + " OddOn");

                                if (isSkipNG_)
                                {
                                    return true;
                                }
                                return false;
                            }
                        }
                    }
                }
            }
            LogState(LogType.CMC_RECV, "00BF(RAW):" + cmc.rawbyte_bal);
            LogState(LogType.CMC_RECV, "00BF(REVERSED):" + cmc.rawbyte_bal_reverse);

            if (ODD_OFF() != "000000")
            {
                if (!Reboot_CMC())
                {
                    ti.Value_ = "CMC_OPEN_FAIL";
                    ti.Result = "NG";
                    this.LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                    if (isSkipNG_)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    if (ODD_OFF() != "000000")
                    {
                        if (!Reboot_CMC())
                        {
                            ti.Value_ = "CMC_OPEN_FAIL";
                            ti.Result = "NG";
                            this.LogState(LogType.NG, "Test :" + ti.Name + " CMC_OPEN_FAIL");

                            if (isSkipNG_)
                            {
                                return true;
                            }
                            return false;
                        }
                        else
                        {
                            if (ODD_OFF() != "000000")
                            {
                                ti.Value_ = OddOff;
                                ti.Result = "NG";
                                this.LogState(LogType.NG, "Test :" + ti.Name + " OddOff");

                                if (isSkipNG_)
                                {
                                    return true;
                                }
                                return false;
                            }
                        }
                    }
                }
            }
            LogState(LogType.CMC_RECV, "00BF(RAW):" + cmc.rawbyte_bal);
            LogState(LogType.CMC_RECV, "00BF(REVERSED):" + cmc.rawbyte_bal_reverse);
            #endregion

            ti.Value_ = EvenOn;

            #region Report before/after CV
            ti.refValues_.Clear();

            //201217 wjs add maserati
            string detailKeyCvBef = "";
            string detailKeyCvafr = "";

            if (pro_Type == ProgramType.EOLInspector)
            {
                //201217 wjs add maserati
                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    detailKeyCvBef = KEY_EOL_POR_NOR_HI__CONT_S_BEF;
                    detailKeyCvafr = KEY_EOL_POR_NOR_HI__CONT_S_AFT;
                }
                else if (localTypes == ModelType.MASERATI_NORMAL)
                {
                    detailKeyCvBef = KEY_EOL_MAS_NOR_HI__CONT_S_BEF;
                    detailKeyCvafr = KEY_EOL_MAS_NOR_HI__CONT_S_AFT;
                }
                else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    detailKeyCvBef = KEY_EOL_POR_F_L_HI__CONT_S_BEF;
                    detailKeyCvafr = KEY_EOL_POR_F_L_HI__CONT_S_AFT;
                }
                //221101 wjs add mase m183
                else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    detailKeyCvBef = KEY_EOL_MAS_M183_HI__CONT_S_BEF;
                    detailKeyCvafr = KEY_EOL_MAS_M183_HI__CONT_S_AFT;
                }
                var dti1 = new DetailItems() { Key = detailKeyCvBef };
                //var dti1 = new DetailItems() { Key = KEY_EOL_POR_NOR_HI__CONT_S_BEF };
                dti1.Reportitems.Add(beforeCV1);
                dti1.Reportitems.Add(beforeCV2);
                dti1.Reportitems.Add(beforeCV3);
                dti1.Reportitems.Add(beforeCV4);
                dti1.Reportitems.Add(beforeCV5);
                dti1.Reportitems.Add(beforeCV6);

                var dti2 = new DetailItems() { Key = detailKeyCvafr };
                //var dti2 = new DetailItems() { Key = KEY_EOL_POR_NOR_HI__CONT_S_AFT };
                dti2.Reportitems.Add(afterCV1);
                dti2.Reportitems.Add(afterCV2);
                dti2.Reportitems.Add(afterCV3);
                dti2.Reportitems.Add(afterCV4);
                dti2.Reportitems.Add(afterCV5);
                dti2.Reportitems.Add(afterCV6);

                ti.refValues_.Add(dti1);
                ti.refValues_.Add(dti2);
            }
            else if (pro_Type == ProgramType.VoltageInspector)
            {
                //201217 wjs add maserati
                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    detailKeyCvBef = KEY_VOL_POR_NOR_HI__CONT_S_BEF;
                    detailKeyCvafr = KEY_VOL_POR_NOR_HI__CONT_S_AFT;
                }
                else if (localTypes == ModelType.MASERATI_NORMAL)
                {
                    detailKeyCvBef = KEY_VOL_MAS_NOR_HI__CONT_S_BEF;
                    detailKeyCvafr = KEY_VOL_MAS_NOR_HI__CONT_S_AFT;
                }
                else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    detailKeyCvBef = KEY_VOL_POR_F_L_HI__CONT_S_BEF;
                    detailKeyCvafr = KEY_VOL_POR_F_L_HI__CONT_S_AFT;
                }
                //221101 wjs add mase m183
                else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    detailKeyCvBef = KEY_VOL_MAS_M183_HI__CONT_S_BEF;
                    detailKeyCvafr = KEY_VOL_MAS_M183_HI__CONT_S_AFT;
                }
                var dti1 = new DetailItems() { Key = detailKeyCvBef };
                //var dti1 = new DetailItems() { Key = KEY_VOL_POR_NOR_HI__CONT_S_BEF };
                dti1.Reportitems.Add(beforeCV1);
                dti1.Reportitems.Add(beforeCV2);
                dti1.Reportitems.Add(beforeCV3);
                dti1.Reportitems.Add(beforeCV4);
                dti1.Reportitems.Add(beforeCV5);
                dti1.Reportitems.Add(beforeCV6);

                var dti2 = new DetailItems() { Key = detailKeyCvafr };
                //var dti2 = new DetailItems() { Key = KEY_VOL_POR_NOR_HI__CONT_S_AFT };
                dti2.Reportitems.Add(afterCV1);
                dti2.Reportitems.Add(afterCV2);
                dti2.Reportitems.Add(afterCV3);
                dti2.Reportitems.Add(afterCV4);
                dti2.Reportitems.Add(afterCV5);
                dti2.Reportitems.Add(afterCV6);

                ti.refValues_.Add(dti1);
                ti.refValues_.Add(dti2);

            }
            #endregion

            return JudgementTestItem(ti);
        }

        public bool CellEvenBalancingOff(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            ti.Value_ = EvenOff;

            return JudgementTestItem(ti);
        }

        public bool CellOddBalancingOn(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = OddOn;

            return JudgementTestItem(ti);
        }

        public bool CellOddBalancingOff(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = OddOff;

            return JudgementTestItem(ti);
        }

        public bool HighContactResist1(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            LogState(LogType.Info, "[BEFORE]" + beforeCV1_origin.ToString() + "(CMC_CV1) * 0.0001525925 = " + beforeCV1.ToString());
            LogState(LogType.Info, "[AFTER]" + afterCV1_origin.ToString() + "(CMC_CV1) * 0.0001525925 = " + afterCV1.ToString());
            var originVal = (beforeCV1 - afterCV1) * 1000;
            ti.Value_ = originVal;
            LogState(LogType.Info, beforeCV1.ToString() + "(BEFORE) - " + afterCV1.ToString() + "(AFTER) = " + ti.Value_.ToString() + "(" + originVal.ToString() + ")mV");

            return JudgementTestItem(ti);
        }

        public bool HighContactResist2(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            LogState(LogType.Info, "[BEFORE]" + beforeCV2_origin.ToString() + "(CMC_CV2) * 0.0001525925 = " + beforeCV2.ToString());
            LogState(LogType.Info, "[AFTER]" + afterCV2_origin.ToString() + "(CMC_CV2) * 0.0001525925 = " + afterCV2.ToString());
            var originVal = (beforeCV2 - afterCV2) * 1000;
            ti.Value_ = originVal;
            LogState(LogType.Info, beforeCV2.ToString() + "(BEFORE) - " + afterCV2.ToString() + "(AFTER) = " + ti.Value_.ToString() + "(" + originVal.ToString() + ")mV");
            return JudgementTestItem(ti);
        }
        public bool HighContactResist3(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            LogState(LogType.Info, "[BEFORE]" + beforeCV3_origin.ToString() + "(CMC_CV3) * 0.0001525925 = " + beforeCV3.ToString());
            LogState(LogType.Info, "[AFTER]" + afterCV3_origin.ToString() + "(CMC_CV3) * 0.0001525925 = " + afterCV3.ToString());
            var originVal = (beforeCV3 - afterCV3) * 1000;
            ti.Value_ = originVal;
            LogState(LogType.Info, beforeCV3.ToString() + "(BEFORE) - " + afterCV3.ToString() + "(AFTER) = " + ti.Value_.ToString() + "(" + originVal.ToString() + ")mV");
            return JudgementTestItem(ti);
        }
        public bool HighContactResist4(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            LogState(LogType.Info, "[BEFORE]" + beforeCV4_origin.ToString() + "(CMC_CV4) * 0.0001525925 = " + beforeCV4.ToString());
            LogState(LogType.Info, "[AFTER]" + afterCV4_origin.ToString() + "(CMC_CV4) * 0.0001525925 = " + afterCV4.ToString());
            var originVal = (beforeCV4 - afterCV4) * 1000;
            ti.Value_ = originVal;
            LogState(LogType.Info, beforeCV4.ToString() + "(BEFORE) - " + afterCV4.ToString() + "(AFTER) = " + ti.Value_.ToString() + "(" + originVal.ToString() + ")mV");
            return JudgementTestItem(ti);
        }
        public bool HighContactResist5(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            LogState(LogType.Info, "[BEFORE]" + beforeCV5_origin.ToString() + "(CMC_CV5) * 0.0001525925 = " + beforeCV5.ToString());
            LogState(LogType.Info, "[AFTER]" + afterCV5_origin.ToString() + "(CMC_CV5) * 0.0001525925 = " + afterCV5.ToString());
            var originVal = (beforeCV5 - afterCV5) * 1000;
            ti.Value_ = originVal;
            LogState(LogType.Info, beforeCV5.ToString() + "(BEFORE) - " + afterCV5.ToString() + "(AFTER) = " + ti.Value_.ToString() + "(" + originVal.ToString() + ")mV");
            return JudgementTestItem(ti);
        }
        public bool HighContactResist6(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            LogState(LogType.Info, "[BEFORE]" + beforeCV6_origin.ToString() + "(CMC_CV6) * 0.0001525925 = " + beforeCV6.ToString());
            LogState(LogType.Info, "[AFTER]" + afterCV6_origin.ToString() + "(CMC_CV6) * 0.0001525925 = " + afterCV6.ToString());
            var originVal = (beforeCV6 - afterCV6) * 1000;
            ti.Value_ = originVal;
            LogState(LogType.Info, beforeCV6.ToString() + "(BEFORE) - " + afterCV6.ToString() + "(AFTER) = " + ti.Value_.ToString() + "(" + originVal.ToString() + ")mV");
            return JudgementTestItem(ti);
        }
        #region NOT USED
        public bool OverTempFault(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.refValues_.Clear();

            int ocnt = 0;
            string ostr = "";
            //if (cmc.AN0_OT) { ocnt += 1; LogState(LogType.Info, "AN0_OT:1"); ostr += "1"; } else { ostr += "0"; };
            //if (cmc.AN1_OT) { ocnt += 1; LogState(LogType.Info, "AN1_OT:1"); ostr += "1"; } else { ostr += "0"; };
            //if (cmc.AN2_OT) { ocnt += 1; LogState(LogType.Info, "AN2_OT:1"); ostr += "1"; } else { ostr += "0"; };
            //if (cmc.AN3_OT) { ocnt += 1; LogState(LogType.Info, "AN3_OT:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.AN4_OT) { ocnt += 1; LogState(LogType.Info, "AN4_OT:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.AN5_OT) { ocnt += 1; LogState(LogType.Info, "AN5_OT:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.AN6_OT) { ocnt += 1; LogState(LogType.Info, "AN6_OT:1"); ostr += "1"; } else { ostr += "0"; };

            ti.refValues_.Add(MakeDetailItem("CTEW3203", ostr.ToString()));

            ti.Value_ = ocnt;

            return JudgementTestItem(ti);
        }

        public bool UnderTempFault(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.refValues_.Clear();

            int ocnt = 0;
            string ostr = "";
            //if (cmc.AN0_UT) { ocnt += 1; LogState(LogType.Info, "AN0_UT:1"); ostr += "1"; } else { ostr += "0"; };
            //if (cmc.AN1_UT) { ocnt += 1; LogState(LogType.Info, "AN1_UT:1"); ostr += "1"; } else { ostr += "0"; };
            //if (cmc.AN2_UT) { ocnt += 1; LogState(LogType.Info, "AN2_UT:1"); ostr += "1"; } else { ostr += "0"; };
            //if (cmc.AN3_UT) { ocnt += 1; LogState(LogType.Info, "AN3_UT:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.AN4_UT) { ocnt += 1; LogState(LogType.Info, "AN4_UT:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.AN5_UT) { ocnt += 1; LogState(LogType.Info, "AN5_UT:1"); ostr += "1"; } else { ostr += "0"; };
            if (cmc.AN6_UT) { ocnt += 1; LogState(LogType.Info, "AN6_UT:1"); ostr += "1"; } else { ostr += "0"; };

            ti.refValues_.Add(MakeDetailItem("CTEW3203", ostr.ToString()));

            ti.Value_ = ocnt;

            return JudgementTestItem(ti);
        }
        #endregion
    }
}
