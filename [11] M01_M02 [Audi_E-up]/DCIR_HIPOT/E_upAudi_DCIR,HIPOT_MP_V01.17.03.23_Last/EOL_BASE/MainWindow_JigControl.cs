using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        public void PreCheck_with_LV()
        {
            //진행중 켜졌을때만 한다.
            if (!plc.judgementFlag)
            {
                return;
            }


            var lvCnt = GetLV_RETRY_COUNTER();
            lvRetryCount = "0";

            LogState(LogType.Info, string.Format("PreCheck_with_LV Start cnt:{0}---------------------------------------", lvCnt));

            int rtcnt = 1;
            for (int cnt = 0; cnt < lvCnt; cnt++)
            {
                //200710 고정수치 사용안함
                //double spec_MV = 24.0;
                double spec_CV = 3;
                double spec_RES = 100000.0;
                double spec_CR = 1000000.0;

                //210113 wjs 포르쉐/마세라티 LV Pass/Fail 판정조건은 HW Version 및 Normal Sleep 함수 내부에 들어있다.
                //48 48 48 은 ASCII 상 '0' 에 해당하는 Decimal(48) 이며 만약 각 버전에 대한 저항값이 측정되는 경우 48 48 48 이 아닌 다른값이 반드시 측정된다.
                //Sleep 에 대한 판정은 리트라이 시 Reboot 가 실패했을 시에 false, 
                //CMC 통신보드의 state 주소인 000A 번지의 state가 04(sleep state)가 아닌 경우 false이다.

                bool result_MV = true;
                bool result_CV = true;
                bool result_RES = true;
                bool result_CR = true;

                //210112 wjs 포르쉐 마세라티 LV 연결성 점검 결과 플래그
                bool result_HWV = true;
                bool ressult_Sleep = true;

                var dmmChInfo = new DMMChannelInfo(localTypes);
                double spec_MV = dmmChInfo.CellCount * spec_CV; //채널개수 * 3v(cell voltage spec)

                //201222 wjs add LV Retry about Mase, Pors N/M
                if (localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                    || localTypes == ModelType.MASERATI_M183_NORMAL  //221101 wjs add mase m183
                    || localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    isCMCRebootFail = false;
                    //cmc.CMC_WakeUp();
                    //Thread.Sleep(CMC_WakeUp_after_sleep);//웨이크업 후 측정시간까지 대기

                    #region CMC Check
                    int retryCount; 
                    bool result = ConnectionCheck_CMC(out retryCount);
                    if(!result)
                    {
                        //210112 연결성검사가 fail인 경우는 무조건 슬립 후 LV fail로 나간다.
                        ressult_Sleep = NormalSleep_pre();
                        result_HWV = false;
                        
                        isMESskip = true; // 0804

                        lvRetryCount = _DEVICE_NOT_READY;
                        LogState(LogType.Info, "PreCheck_with_LV FAIL---------------------------------------------");
                        return;
                    }
                    #endregion

                    //정상적이면 true 실패하면 false 반환
                    result_HWV = HW_Version_Pre();

                    ressult_Sleep = NormalSleep_pre();//201222 이 Sleep 은 단위검사항목과 구분하여 CMC의 Sleep 여부를 성공/실패로만 구분한다. 실패 시 리트라이하던지 NG 배출해야함

                    //이하 아래의 리트라이 및 동작여부에 대한 판단은 CMC_CV 와 res_Sleep 여부를 토대로 추가 작성하면 될 듯하다.

                    //조건은 아마도 나중에 결정되면 추가함 된다.// 210112 전압은 미사용
                    //if (CMClist_Cell[0] > 3.0 && CMClist_Cell[1] > 3.0 && CMClist_Cell[2] > 3.0 && CMClist_Cell[3] > 3.0 && CMClist_Cell[4] > 3.0 && CMClist_Cell[5] > 3.0)
                    //{
                    //    //LV가 정상으로 삽입되었다고 판단
                    //    LogState(LogType.Fail, "CMC Cell Voltage All OK");
                    //    CMC_CV = true;
                    //}
                    //else
                    //{
                    //    //LV가 정상으로 삽입되지 않았거나 셀 전압이 3 V 미만으로 불량으로 판단
                    //    LogState(LogType.Fail, "CMC Cell Voltage Less than 3.0");
                    //    CMC_CV = false;
                    //}
                    //res_Sleep 이 false 면 Sleep 이 정상적으로 동작하지 않은거고 True 이면 정상적 상태로 판단

                }
                else  // AUDI & EUP
                {
                    #region Thermistor
                    var resList = new List<double>();
                    if (!keysight.MeasRes_Multi(out resList, dmmChInfo.ModuleRes_LV, 2, true))
                    {
                        lvRetryCount = _DEVICE_NOT_READY;
                        LogState(LogType.Info, "PreCheck_with_LV FAIL---------------------------------------------");
                        return;
                    }

                    for (int i = 0; i < resList.Count; i++)
                    {
                        if (resList[i] > spec_RES)
                            result_RES = false;
                    }
                    #endregion

                    #region Cell Voltage
                    var cvList = new List<double>();
                    if (!keysight.MeasVolt_Multi(out cvList, dmmChInfo.CellCH, dmmChInfo.ChannelCount))
                    {
                        lvRetryCount = _DEVICE_NOT_READY;
                        LogState(LogType.Info, "PreCheck_with_LV FAIL---------------------------------------------");
                        return;
                    }

                    for (int i = 0; i < cvList.Count; i++)
                    {
                        if (cvList[i] < spec_CV)
                            result_CV = false;
                    }
                    #endregion

                    #region Cell Line resistance
                    var crList = new List<double>();
                    if (!keysight.MeasRes_Multi(out crList, dmmChInfo.CellCH, dmmChInfo.ChannelCount))
                    {
                        lvRetryCount = _DEVICE_NOT_READY;
                        LogState(LogType.Info, "PreCheck_with_LV FAIL---------------------------------------------");
                        return;
                    }

                    for (int i = 0; i > crList.Count; i++)
                    {
                        if (crList[i] < spec_CV)
                            result_CR = false;
                    }
                    #endregion


                    #region Module Voltage
                    var tempMv1 = 0.0;
                    if (!keysight.MeasVolt(out tempMv1, dmmChInfo.ModuleCH))
                    {
                        lvRetryCount = _DEVICE_NOT_READY;
                        LogState(LogType.Info, "PreCheck_with_LV FAIL---------------------------------------------");
                        return;
                    }

                    if (tempMv1 < spec_MV)
                        result_MV = false;
                    #endregion
                }

                #region Judgement
                if (isStop || ispause)
                {
                    lvRetryCount = _EMG_STOPPED;
                    LogState(LogType.Info, "PreCheck_with_LV FAIL(Emergency)---------------------------------------------");
                    return;
                }
                if (localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                    || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                    || localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    if (result_HWV && ressult_Sleep)
                    {
                        LogState(LogType.Info, "PreCheck_with_LV PASS---------------------------------------------");
                        return;
                    }
                }
                else  // AUDI & EUP
                {
                    if (result_MV && result_CV && result_RES && result_CR)
                    {
                        LogState(LogType.Info, "PreCheck_with_LV PASS---------------------------------------------");
                        return;
                    }
                    
                    if (!result_MV && !result_CV && !result_RES && !result_CR)
                    {
                        LogState(LogType.Info, "Connection NG"); // 0804 ks yoo
                        isMESskip = true;   
                        return;
                    }

                }
                #endregion

                #region Jig control
                if (!LV_RetryTest())
                {
                    LogState(LogType.Info, "PreCheck_with_LV FAIL---------------------------------------------");
                    return;
                }
                #endregion

                //이도저도 아니고 NG라면 리트라이
                if (localTypes == ModelType.MASERATI_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                    || localTypes == ModelType.MASERATI_M183_NORMAL //221101 wjs add mase m183
                    || localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    if (isCMCRebootFail)
                    {
                        //CMC Reboot가 한번이라도 실패하는 경우에는 합불 여부를 떠나 해당 문자열을 리트라이 카운트에 적용(PJH)
                        lvRetryCount = _CMC_OPEN_FAIL;
                    }
                    else
                    {
                        lvRetryCount = rtcnt.ToString();
                    }
                }
                else
                {
                    lvRetryCount = rtcnt.ToString();
                }
                rtcnt++;
            }
            LogState(LogType.Info, "PreCheck_with_LV END---------------------------------------------");
        }

        public bool LV_RetryTest()
        {
            LogState(LogType.Info, "RetryTest Start---------------------------------------------");

            if (!plc.isAlive)
            {
                LogState(LogType.Info, "RetryTest END---------------------------------------------");
                return false;
            }

            try
            {
                if (!plc.LV_Retry_Flag)
                {
                    plc.LV_Retry(true);

                    int cnt = 0;
                    while (true)
                    {
                        Thread.Sleep(100);
                        cnt += 100;//5s

                        if (cnt > 20000)
                        {
                            lvRetryCount = "RETRY_NOT_ACTIVE";
                            LogState(LogType.Fail, "Not Activated in 10sec");
                            plc.LV_Retry(false);
                            LogState(LogType.Info, "RetryTest END---------------------------------------------");
                            return false;
                        }

                        if (isStop || ispause)
                        {
                            plc.LV_Retry(false);
                            lvRetryCount = _EMG_STOPPED;
                            LogState(LogType.Info, "RetryTest END---------------------------------------------");
                            return false;
                        }

                        if (plc.plc_lv_retry_recv_ok)
                        {
                            LogState(LogType.Info, "Success To RetryTest Flag in Time out 10sec");
                            plc.LV_Retry(false);
                            LogState(LogType.Info, "RetryTest END---------------------------------------------");
                            return true;
                        }
                    }
                }
                else
                {
                    LogState(LogType.Info, "RetryTest END---------------------------------------------");
                    return false;
                }

            }
            catch (Exception ec)
            {
                LogState(LogType.Info, "RetryTest", ec);
                LogState(LogType.Info, "RetryTest END---------------------------------------------");
                return false;
            }
        }

    }
}
