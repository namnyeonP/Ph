using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        public bool Plus_Probe_Contack(TestItem ti)
        {
            double minValue = -10; //제어항목으로 가져오는 스펙 Min값
            double maxValue = CONTACT_CHECK_UPPER_LIMIT; //제어항목으로 가져오는 스펙 Max값
            StringBuilder chList = new StringBuilder();
            List<string> resultList = new List<string>();
            List<double> resList = new List<double>();
            string resData = "";

            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            if (isLineFlag)
            {
                #region DAQ970A
                #region 문자열 채널 정렬
                var daqChInfo = new DAQChannelInfo(localTypes);
                var chStrList = daqChInfo.String_ContactCH.ToString().Split(',').ToList(); // Channel List

                if (chStrList.Contains("CC")) // 4P7S
                {
                    chStrList.Insert(0, chStrList[1]);
                    chStrList.RemoveAt(2);
                }
                #endregion

                #region MeasRes 측정부분
                if (!daq970_1.MeasRes_Multi(out resList, daqChInfo.ContactCH, daqChInfo.ContactCount))
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }
                #endregion

                #region 판정
                // 210803 JKD - 저항값 제어스펙으로 내부판정
                for (int i = 0; i < resList.Count; i++)
                {
                    double res = resList[i];

                    if (this.strChromaModelType == "NEW")
                    {
                        #region 221102 disable
                        if (minValue <= res && res <= maxValue)
                        {
                            resultList.Add("PASS");
                        }
                        else
                        {
                            resultList.Add("NG");

                            string chValue = chStrList[i]; // NG 발생 저항값의 채널

                            chList.Append(chValue);
                            chList.Append(":");
                            chList.Append(res);
                            chList.Append("/");
                        }
                        #endregion
                    }
                    else
                    {

                        //20221102
                        if (res <= maxValue)
                        {
                            resultList.Add("PASS");
                        }
                        else
                        {
                            resultList.Add("NG");

                            string chValue = chStrList[i]; // NG 발생 저항값의 채널

                            chList.Append(chValue);
                            chList.Append(":");
                            chList.Append(res);
                            chList.Append("/");
                        }
                    }
                }

                if (resultList.Contains("NG"))
                {
                    chList = chList.Remove(chList.Length - 1, 1);

                    ti.Value_ = chList.ToString(); // NG시 채널로 결과값 출력

                    LogState(LogType.Fail, string.Format("Channel :{0}", chList.ToString()));
                }
                else
                {
                    //저항 패스 일 경우 캡 검사 한번 더해서 NG,PASS 구분 하는 부분
                    //230221 cap 검사 기능 추가
                    var cap = 0.0;
                    int nIndexPlus = 0;
                    long lDecimalPoint = 100000000;
                    chroma.isPlusFlag = false;

                    if (this.strChromaModelType == "NEW")
                    {
                        nIndexPlus = 4;
                        lDecimalPoint = 10000000000;
                    }
                    else
                    {
                        nIndexPlus = 0;
                        lDecimalPoint = 100000000;
                    }

                    // CH-1(4P8S, 3P8S, 3P10S)  CH-2(4P8S_Rev, 3P10S_Rev) CH-3(4P7S)  CH-4(샤시) 
                    if (localTypes == 0)
                    {
                        //4P8S
                        chroma.GetCapacitance(1 + nIndexPlus, 4 + nIndexPlus, out cap);
                    }
                    else if (localTypes == 1)
                    {
                        //4P8S_rev
                        chroma.GetCapacitance(2 + nIndexPlus, 4 + nIndexPlus, out cap);
                    }
                    else if (localTypes == 2)
                    {
                        //4P7S
                        chroma.GetCapacitance(3 + nIndexPlus, 4 + nIndexPlus, out cap);
                    }
                    else if (localTypes == 3)
                    {
                        //3P8S
                        chroma.GetCapacitance(1 + nIndexPlus, 4 + nIndexPlus, out cap);
                    }
                    else if (localTypes == 4)
                    {
                        //3P10S
                        chroma.GetCapacitance(1 + nIndexPlus, 4 + nIndexPlus, out cap);

                    }
                    else if (localTypes == 5)
                    {
                        //3P10S_rer
                        chroma.GetCapacitance(2 + nIndexPlus, 4 + nIndexPlus, out cap);
                    }


                    //230221
                    if (cap * lDecimalPoint > 1)
                    {
                        ti.Value_ = "PASS";
                    }
                    else
                    {
                        ti.Value_ = "Side Pin NG";
                    }

                    LogState(LogType.Info, "Cap Data : ," + cap * lDecimalPoint);
                }
                #endregion

                

                // 전체 셀 저항값
                for (int i = 0; i < resList.Count; i++)
                {
                    if (i == resList.Count - 1)
                        resData += resList[i];
                    else
                        resData += resList[i] + "&";
                }                

                #region 상세수집 보고
                //JKD - 전체 셀 저항값 상세수집보고
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4072", resData));
                LogState(LogType.Info, "Cell Resistance," + resData);

                ti.refValues_.Add(MakeDetailItem("W2MLMTE4059", temps.humiStr.ToString()));
                LogState(LogType.Info, "Humidity," + temps.humiStr);
                #endregion

                #region 설비 IV실린더 Down 제어 부분
                if (!ModeSet_IvCylinder())
                {
                    ti.Value_ = _JIG_NOT_ACTIVE;

                    LogState(LogType.Fail, "IV Cylinder ModeSet Fail!");
                    return JudgementTestItem(ti);
                }
                #endregion

                //착공 결과 확인 및 대기
                // 211201 JKD - Manual Group Button 클릭시 착공 Flag On
                // 211206 JKD - MES Skip Flag 추가
                if (!isMESskip)
                {
                    int nTimeoutCount = 0;
                    while (true)
                    {
                        if (m_bIsStartJobFinish == true)
                        {
                            break;
                        }
                        Thread.Sleep(100);
                        nTimeoutCount++;

                        if (nTimeoutCount > 50) // 5초 대기 및 타임아웃
                        {
                            ti.Value_ = _START_JOB_NG;
                            break;
                        }
                    }

                    if (m_bStartJobResult == false)
                    {
                        ti.Value_ = _START_JOB_NG;
                    }
                }
                #endregion
            }
            else
            {
                #region Chroma
                var cap = 0.0;
                chroma.isPlusFlag = false;

                // CH-1(4P8S, 3P8S, 3P10S)  CH-2(4P8S_Rev, 3P10S_Rev) CH-3(4P7S)  CH-4(샤시) 
                if (localTypes == 0)
                {
                    //4P8S
                    chroma.GetCapacitance(1, 4, out cap);
                }
                else if (localTypes == 1)
                {
                    //4P8S_rev
                    chroma.GetCapacitance(2, 4, out cap);
                }
                else if (localTypes == 2)
                {
                    //4P7S
                    chroma.GetCapacitance(3, 4, out cap);
                }
                else if (localTypes == 3)
                {
                    //3P8S
                    chroma.GetCapacitance(1, 4, out cap);
                }
                else if (localTypes == 4)
                {
                    //3P10S
                    chroma.GetCapacitance(1, 4, out cap);

                }
                else if (localTypes == 5)
                {
                    //3P10S_rer
                    chroma.GetCapacitance(2, 4, out cap);
                }

                ti.refValues_.Add(MakeDetailItem("W2MLMTE4059", temps.humiStr.ToString()));

                //230221
                if((int)(cap * 1000000000) > 1)
                {
                    ti.Value_ = "PASS";
                }
                else
                {
                    ti.Value_ = "NG";
                }
                //ti.Value_ = (cap * 1000000000);//nF

                //착공 결과 확인 및 대기
                // 211201 JKD - Manual Group Button 클릭시 착공 Flag On
                // 211206 JKD - MES Skip Flag 추가
                if (!isMESskip)
                {
                    int nTimeoutCount = 0;
                    while (true)
                    {
                        if (m_bIsStartJobFinish == true)
                        {
                            break;
                        }
                        Thread.Sleep(100);
                        nTimeoutCount++;

                        if (nTimeoutCount > 50) // 5초 대기 및 타임아웃
                        {
                            ti.Value_ = _START_JOB_NG;
                            break;
                        }
                    }

                    if (m_bStartJobResult == false)
                    {
                        ti.Value_ = _START_JOB_NG;
                    }
                }
                #endregion
            }

            return JudgementTestItem(ti);
        }

        private bool ModeSet_IvCylinder()
        {
            if (plc.isTesting_Flag)
            {
                if (!plc.iv_Cylinder_Down_Flag) 
                {
                    plc.Iv_Cylinder_Down(true);
                    LogState(LogType.Success, "IV Cylinder Down Activated");

                    int cnt = 0;
                    while (true)
                    {
                        if (cnt >= 10000)
                        {
                            plc.Iv_Cylinder_Down(false);
                            LogState(LogType.Fail, "10s TimeOut. IV Cylinder Down Check Fail!");
                            return false;
                        }

                        if (plc.plc_isIV_Recv_Ok) // PLC IV Cylinder Down Bit Check
                        {
                            plc.Iv_Cylinder_Down(false);
                            LogState(LogType.Success, "IV Cylinder Down Check Success");
                            return true;
                        }

                        Thread.Sleep(100);
                        cnt += 100;
                    }

                }
            }

            return true;
        }

    }
}
