using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EOL_BASE
{
    public partial class MainWindow
    {        
        double preRes900 = 0.0;

        public bool Plus_IR500(TestItem ti)
        {
            isProcessingUI(ti);
            preRes900 = 0.0;
            chroma.isPlusFlag = false;


            //if (plc.isTesting_Flag)
            //{
            //    if (SensingVoltToMux())
            //    {
            //        ti.Value_ = -1;
            //        LogState(LogType.Info, "Over voltage sensing line(3V).");
            //        return JudgementTestItem(ti);
            //    }
            //}

            //ti.refValues_.Add(MakeDetailItem("W2MLMTE4025", INSULATION_RESISTANCE_VOLT.ToString()));
            //ti.refValues_.Add(MakeDetailItem("W2MLMTE4026", INSULATION_RESISTANCE_TIME.ToString()));

            #region 500V 10s
            int preResRtn900 = 0;
            // CH-1(4P8S, 3P8S, 3P10S)   CH-2(4P8S_Rev, 3P10S_Rev) CH-3(4P7S)  CH-4(??) 
            if (localTypes == 0)
            {
                //4P8S
                preResRtn900 = chroma.GetResistance500(1, 4, 500, 1, 20, out preRes900);
            }
            else if (localTypes == 1)
            {
                //4P8S_rev
                preResRtn900 = chroma.GetResistance500(2, 4, 500, 1, 20, out preRes900);
            }
            else if (localTypes == 2)
            {
                //4P7S ok
                preResRtn900 = chroma.GetResistance500(3, 4, 500, 1, 20, out preRes900);
            }
            else if (localTypes == 3)
            {
                //3P8S
                preResRtn900 = chroma.GetResistance500(1, 4, 500, 1, 20, out preRes900);
            }
            else if (localTypes == 4)
            {
                //3P10S
                preResRtn900 = chroma.GetResistance500(1, 4, 500, 1, 20, out preRes900);

            }
            else if (localTypes == 5)
            {
                //3P10S_rer
                preResRtn900 = chroma.GetResistance500(2, 4, 500, 1, 20, out preRes900);
            }

            if (preResRtn900 == -1) //수정필요
            {
                ti.Value_ = "Hipot Fail";
                LogState(LogType.Info, string.Format("Hipot Fail - {0}V {1}s", 500, 20));
                //return JudgementTestItem(ti);
            }
            // 200304 jgh Judgement Add
            else if (preResRtn900 == 49)
            {
                ti.Value_ = _HI_LIMIT;
                LogState(LogType.Fail, string.Format("Hipot - {0}V {1}s HI_LIMIT", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (preResRtn900 == 50)
            {
                ti.Value_ = _LOW_LIMIT;
                LogState(LogType.Fail, string.Format("Hipot - {0}V {1}s LOW_LIMIT", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (preResRtn900 == 52)
            {
                ti.Value_ = _IO;
                LogState(LogType.Fail, string.Format("Hipot - {0}V {1}s IO", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (preResRtn900 == 54)
            {
                ti.Value_ = _ADV_OVER;
                LogState(LogType.Fail, string.Format("Hipot - {0}V {1}s ADV_OVER", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (preResRtn900 == 55)
            {
                ti.Value_ = _ADI_OVER;
                LogState(LogType.Fail, string.Format("Hipot - {0}V {1}s ADI_OVER", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (preResRtn900 == 120)
            {
                ti.Value_ = _GR_CONT;
                LogState(LogType.Fail, string.Format("Hipot - {0}V {1}s GR_COUNT", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (preResRtn900 == 121)
            {
                ti.Value_ = _GFI_FAULT;
                LogState(LogType.Fail, string.Format("Hipot - {0}V {1}s GFI_FAULT", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (preResRtn900 == 112)
            {
                ti.Value_ = _STOP;
                LogState(LogType.Fail, string.Format("Hipot Fail - {0}V {1}s STOP", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (preResRtn900 == 113)
            {
                ti.Value_ = _USER_STOP;
                LogState(LogType.Fail, string.Format("Hipot Fail - {0}V {1}s USER_STOP", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (preResRtn900 == 114)
            {
                ti.Value_ = _CAN_NOT_TEST;
                LogState(LogType.Fail, string.Format("Hipot Fail - {0}V {1}s CAN_NOT_TEST", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            //else if (preResRtn900 == 115)
            //{
            //    ti.Value_ = _TESTING;
            //    LogState(LogType.Fail, string.Format("Hipot Fail - {0}V {1}s TESTING", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            //}

            else
            {
                ti.Value_ = preRes900; // 1000000;

                //ti.refValues_.Add(MakeDetailItem("W2MLMTE4060", preRes900.ToString()));
                LogState(LogType.Info, string.Format("Hipot - {0}V {1}s Resistance - {2}", 500, 20, preRes900));
            }
            Thread.Sleep(100);

            string fetchDetailData500 = "";

            for (int i = 0; i < chroma.fetchList500.Count; i++)
            {
                fetchDetailData500 += chroma.fetchList500[i].ToString();
                if (i < 20)
                {
                    fetchDetailData500 += "&";
                }
            }

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4060", fetchDetailData500));

            return JudgementTestItem(ti);
            #endregion
        }

        public bool Plus_IR(TestItem ti)
        {
            isProcessingUI(ti);

            if (isStop || ispause)
            {
                ti.Value_ = _EMG_STOPPED;
                LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");

                return JudgementTestItem(ti);
            }

            var res = 0.0;
            chroma.isPlusFlag = false;

            ti.refValues_.Clear();

            //Min. Max Add By KYJ 200703
            var d_max = 0.0;
            var d_min = 0.0;
            var rst1 = double.TryParse(ti.Max.ToString(), out d_max);
            var rst2 = double.TryParse(ti.Min.ToString(), out d_min);

            FETCH_STANDARD_MIN_VALUE = d_min;
            FETCH_STANDARD_MAX_VALUE = d_max;
            //////////////////////////////////////////////////////////

            //if (plc.isTesting_Flag)
            //{
            //    if (SensingVoltToMux())
            //    {
            //        ti.Value_ = -1;
            //        LogState(LogType.Info, "Over voltage sensing line(3V).");
            //        return JudgementTestItem(ti);
            //    }
            //}

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4025", INSULATION_RESISTANCE_VOLT.ToString()));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4026", INSULATION_RESISTANCE_TIME.ToString()));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4061", INSULATION_RESISTANCE_RANGE));
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4062", INSULATION_RESISTANCE_RAMP_TIME.ToString()));
            
            #region 기본 IR 검사

            int Channel_HI = 0;
            int Channel_LO = 0;

            if (!SetChannels(out Channel_HI, out Channel_LO))
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }

            int resRtn = 0;

            resRtn = chroma.GetResistance(Channel_HI, Channel_LO, out res);

            if (chroma.fetchCNTTime != 0)
            {
                //램프업 횟수(6)을 빼고 / 2
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4057", ((chroma.fetchCNTTime - 6) / 2).ToString()));
            }            	
            	
            if (resRtn == -1)
            {
                ti.Value_ = -1;
                LogState(LogType.Info, string.Format("Hipot Fail - {0}V {1}s (-1)", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            if (resRtn == -9)
            {
                ti.Value_ = chroma.judgementStr;
                LogState(LogType.Info, string.Format("Hipot Fail - {0}V {1}s (-1)", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else
            {
                // 20191031 Noah 검사시작 직후 무한대가 뜨면 리트라이 들어 갈수 있게 수정 요청(이지현 사원)
                if (this.strChromaModelType == "NEW")
                {
                    if (res > 60000)
                    {
                        ti.Value_ = 60000;
                    }
                    else
                    {
                        ti.Value_ = res; // resistance / 1000000;
                    }
                }
                else
                {
                    if (res > 9.9E+30)
                    {
                        ti.Value_ = 999999;
                    }
                    else
                    {
                        ti.Value_ = res; // resistance / 1000000;
                    }
                }

                //ti.Value_ = resistance;
                LogState(LogType.Info, string.Format("Retry Hipot - {0}V {1}s Resistance - {2}", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME, res));

                //res = 999999;                    
            }
            #endregion

            // 2019.08.27 jeonhj's comment
            // 60초동안 계측한 IR값 전부 기록
            string fetchDetailData = "";

            //200721 램프업시간 * 2 수정
            //1.     현재 Ramp up 3초 Data를 상세수집에 올리지 않게 되어 있습니다.
            //       -맨앞의 FETCH 3개를 안올리게 되어 있을 겁니다. 6개를 안올리도록 변경이 필요합니다.
            //       -상세수집 ID: W2MLMTE4054
            for (int i = INSULATION_RESISTANCE_RAMP_TIME * 2; i < chroma.fetchList.Count; i++)
            {
                fetchDetailData += chroma.fetchList[i].ToString();
                if (i != chroma.fetchList.Count)
                {
                    fetchDetailData += "&";
                }
            }

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4054", fetchDetailData));

            return JudgementTestItem(ti);
        }

        bool SetChannels(out int Channel_HI, out int Channel_LO)
        {
            if (this.strChromaModelType == "NEW")
            {
                switch (localTypes)
                {
                    case 0: { Channel_HI = 5; Channel_LO = 8; return true; }//4P8S
                    case 1: { Channel_HI = 6; Channel_LO = 8; return true; }//4P8S_rev
                    case 2: { Channel_HI = 7; Channel_LO = 8; return true; }//4P7S 
                    case 3: { Channel_HI = 5; Channel_LO = 8; return true; }//3P8S
                    case 4: { Channel_HI = 5; Channel_LO = 8; return true; }//3P10S
                    case 5: { Channel_HI = 6; Channel_LO = 8; return true; }//3P10S_rer
                    default: { Channel_HI = 0; Channel_LO = 0; return false; };
                }
            }
            else
            {
                switch (localTypes)
                {
                    case 0: { Channel_HI = 1; Channel_LO = 4; return true; }//4P8S
                    case 1: { Channel_HI = 2; Channel_LO = 4; return true; }//4P8S_rev
                    case 2: { Channel_HI = 3; Channel_LO = 4; return true; }//4P7S 
                    case 3: { Channel_HI = 1; Channel_LO = 4; return true; }//3P8S
                    case 4: { Channel_HI = 1; Channel_LO = 4; return true; }//3P10S
                    case 5: { Channel_HI = 2; Channel_LO = 4; return true; }//3P10S_rer
                    default: { Channel_HI = 0; Channel_LO = 0; return false; };
                }
            }
        }

        /// <summary>
        /// 내전압검사
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public bool WithstandVoltage1(TestItem ti)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            isProcessingUI(ti);

            ti.refValues_.Clear();

            ti.refValues_.Add(MakeDetailItem("W2MLMTE4063", chroma.CON_WITHSTANDING_VOLTAGE_V.ToString()));     //200723 ht 추가
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4064",chroma.CON_WITHSTANDING_RAMP_UP_TIME.ToString()));   //200723 ht 추가
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4065",chroma.CON_WITHSTANDING_VOLTAGE_T.ToString()));      //200723 ht 추가
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4066",chroma.CON_ARC_LIMIT.ToString()));                   //200723 ht 추가
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4067",chroma.CON_ARC_ENABLE.ToString()));                  //200723 ht 추가
            ti.refValues_.Add(MakeDetailItem("W2MLMTE4068", chroma.CON_WITHSTANDING_FALLDOWN_TIME.ToString())); //200723 ht 추가

            int Channel_HI = 0;
            int Channel_LO = 0;

            if (!SetChannels(out Channel_HI, out Channel_LO))
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }
            
            try
            {
                double current = -1;
                var rtv = chroma.GetVoltage(Channel_HI, Channel_LO, out current);

                if (chroma.judgementStr == _CASE_35 || chroma.judgementStr == _CASE_33) // 0517 kyounsuk 
                {
                    LogState(LogType.Info, string.Format("Channel setting error"));
                    current = -1;
                    rtv = chroma.GetVoltage(Channel_HI, Channel_LO, out current);
                }

                if (chroma.judgementStr == _CASE_35 || chroma.judgementStr == _CASE_33) // 0519 kyounsuk 
                {
                    LogState(LogType.Info, string.Format("Channel setting error"));
                    current = -1;
                    rtv = chroma.GetVoltage(Channel_HI, Channel_LO, out current);
                }
                //내전압 진행 중 DC-HI Or Arc 일 경우 Period(내전압 시작 시점으로부터 Error 발생시점) 상세수집 보고 200825
                if (chroma.judgementStr == _CASE_35 || chroma.judgementStr == _CASE_33)
                {
                    ti.refValues_.Add(MakeDetailItem("W2MLMTE4070", chroma.ARC_DC_HI_TIME.ToString())); 
                }
                else
                {
                    ti.refValues_.Add(MakeDetailItem("W2MLMTE4070", "-")); 
                }

                if (rtv == -9 && chroma.judgementStr == _CASE_35)
                {
                    ti.Value_ = chroma.judgementStr;
                }
                else if (rtv == 0)
                {
                    //정상
                    current = current * 1000;
                    ti.Value_ = current;
                }
                else
                {
                    //뭔지모르면 기존대로 -1
                    ti.Value_ = chroma.judgementStr;
                }
            }
            catch (Exception ec)
            {
                ti.Value_ = _DEVICE_NOT_READY;
                return JudgementTestItem(ti);
            }
            this.LogState(LogType.Info, ti.Name + " - Elapsed Time(msec) :" + (sp.ElapsedMilliseconds * 0.001).ToString());
            sp.Stop();
            sp = null;

            if (ispause)
            {
                this.LogState(LogType.Info, "Pause Stopped");
                ti.Value_ = _EMG_STOPPED;
            }

            return JudgementTestItem(ti);
        }

        #region 변경 EOL 문자열_ver1


        public string _CASE_113 = "REINPUT_USER STOP";          // 사용자가 계측기 전면부 정지 버튼 누를 시 발생
        public string _CASE_114 = "REINPUT_CAN NOT TEST";       // 원인미상(계측기에서 수신됨)
        public string _CASE_115 = "REINPUT_TESTING";            // 원인미상(계측기에서 수신됨)
        public string _CASE_112 = "REINPUT_STOP";               // 원인미상(계측기에서 수신됨)
        public string _CASE_33 = "-33";                         // 내전압 검사 시 High 채널 문제 발생 
        public string _CASE_49 = "-49";                         // 절연저항 검사 시 High 채널 문제 발생
        public string _CASE_34 = "-34";                         // 내전압 검사 시 Low 채널 문제 발생
        public string _CASE_50 = "-50";                         // 절연저항 검사 시 Low 채널 문제 발생
        public string _CASE_35 = "-35";                         // 내전압 검사 시 ARC 감지
        public string _CASE_36 = "-36";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_52 = "-52";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_37 = "-37";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_38 = "-38";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_54 = "-54";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_39 = "-39";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_55 = "-55";                         // 원인미상(계측기에서 수신됨)
        public string _CASE_120_DC = "REINPUT_DC_GR_CONT";         // 원인미상(계측기에서 수신됨)
        public string _CASE_120_IR = "REINPUT_IR_GR_CONT";       //문자열 공용
        public string _CASE_121_DC = "REINPUT_DC_TRIPPED";         // 스테이션 상에서 모듈과 설비 간 절연이 깨진 경우 발생(누설전류)
        public string _CASE_121_IR = "REINPUT_IR_TRIPPED";       //문자열 공용
        public string _CASE_43 = "REINPUT_DC_IO-F";             // 원인미상(계측기에서 수신됨)
        public string _HIPOT_EMO = "REINPUT_EMO";               // 200616 추가
        public string _HIPOT_OVERFLOW = "REINPUT_OVERFLOW";     // 200616 추가

        #endregion

        //200609 추가
        public string _CHAN_HI_FAIL = "SET_CH_FAIL(HI)";
        public string _CHAN_LO_FAIL = "SET_CH_FAIL(LO)";

        #region 안씀
        /// <summary>
        /// 무한대가 나오면 다시 검사
        /// </summary>
        /// <param name="volt"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        //private double RetryIRPlusTest(int volt, int time)
        //{
        //    double res = 0.0;
        //    int resRtn = 0;
        //    // CH-1(4P8S, 3P8S, 3P10S)   CH-2(4P8S_Rev, 3P10S_Rev) CH-3(4P7S)  CH-4(샤시) 
        //    if (localTypes == 0)
        //    {
        //        //4P8S
        //        resRtn = chroma.GetResistance500(1, 4, volt, time, out res);
        //    }
        //    else if (localTypes == 1)
        //    {
        //        //4P8S_rev
        //        resRtn = chroma.GetResistance500(2, 4, volt, time, out res);
        //    }
        //    else if (localTypes == 2)
        //    {
        //        //4P7S ok
        //        resRtn = chroma.GetResistance500(3, 4, volt, time, out res);
        //    }
        //    else if (localTypes == 3)
        //    {
        //        //3P8S
        //        resRtn = chroma.GetResistance500(1, 4, volt, time, out res);
        //    }
        //    else if (localTypes == 4)
        //    {
        //        //3P10S
        //        resRtn = chroma.GetResistance500(1, 4, volt, time, out res);

        //    }
        //    else if (localTypes == 5)
        //    {
        //        //3P10S_rer
        //        resRtn = chroma.GetResistance500(2, 4, volt, time, out res);
        //    }

        //    if (resRtn == -1)
        //    {
        //        res = -1;
        //        LogState(LogType.Info, string.Format("Retry Hipot Fail - {0}V {1}s (-1)", volt, time));
        //    }
        //    else
        //    {
        //        if (res > 9.9E+30)
        //        {
        //            res = 999999;
        //        }

        //        LogState(LogType.Info, string.Format("Retry Hipot - {0}V {1}s Res - ", volt, time, res));
        //    }
        //    return res;
        //}

        /// <summary>
        /// 무한대가 나오면 다시 한번 더 절연저항 측정
        /// </summary>
        /// <returns></returns>
        public double RetryIRPlus(out double res) //double로 변경 jgh
        {
            res = 0.0;
            double resRtn = 0;
            // CH-1(4P8S, 3P8S, 3P10S)   CH-2(4P8S_Rev, 3P10S_Rev) CH-3(4P7S)  CH-4(샤시) 
            if (localTypes == 0)
            {
                //4P8S
                resRtn = chroma.GetResistance(1, 4, out res);
            }
            else if (localTypes == 1)
            {
                //4P8S_rev
                resRtn = chroma.GetResistance(2, 4, out res);
            }
            else if (localTypes == 2)
            {
                //4P7S ok
                resRtn = chroma.GetResistance(3, 4, out res);
            }
            else if (localTypes == 3)
            {
                //3P8S
                resRtn = chroma.GetResistance(1, 4, out res);
            }
            else if (localTypes == 4)
            {
                //3P10S
                resRtn = chroma.GetResistance(1, 4, out res);

            }
            else if (localTypes == 5)
            {
                //3P10S_rer
                resRtn = chroma.GetResistance(2, 4, out res);
            }

            if (resRtn == -1)
            {
                resRtn = -1;
                LogState(LogType.Info, string.Format("Retry Hipot Fail - {0}V {1}s (-1)", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (resRtn == 49)
            {
                resRtn = 49;
                LogState(LogType.Fail, string.Format("Retry Hipot - {0}V {1}s HI_LIMIT", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (resRtn == 50)
            {
                resRtn = 50;
                LogState(LogType.Fail, string.Format("Retry Hipot - {0}V {1}s LOW_LIMIT", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (resRtn == 52)
            {
                resRtn = 52;
                LogState(LogType.Fail, string.Format("Retry Hipot - {0}V {1}s IO", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (resRtn == 54)
            {
                resRtn = 54;
                LogState(LogType.Fail, string.Format("Retry Hipot - {0}V {1}s ADV_OVER", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (resRtn == 55)
            {
                resRtn = 55;
                LogState(LogType.Fail, string.Format("Retry Hipot - {0}V {1}s ADI_OVER", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (resRtn == 120)
            {
                resRtn = 120;
                LogState(LogType.Fail, string.Format("Retry Hipot - {0}V {1}s GR_CONT", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (resRtn == 121)
            {
                resRtn = 121;
                LogState(LogType.Fail, string.Format("Retry Hipot - {0}V {1}s GFI_FAULT", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (resRtn == 112)
            {
                resRtn = 112;
                LogState(LogType.Fail, string.Format("Retry Hipot - {0}V {1}s STOP", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (resRtn == 113)
            {
                resRtn = 113;
                LogState(LogType.Fail, string.Format("Retry Hipot - {0}V {1}s USER_STOP", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (resRtn == 114)
            {
                resRtn = 114;
                LogState(LogType.Fail, string.Format("Retry Hipot - {0}V {1}s CAN_NOT_TEST", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else if (resRtn == 115)
            {
                resRtn = 115;
                LogState(LogType.Fail, string.Format("Retry Hipot - {0}V {1}s TESTING", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME));
            }
            else
            {
                if (res > 9.9E+30)
                {
                    resRtn = res;
                }

                LogState(LogType.Info, string.Format("Retry Hipot - {0}V {1}s Res - {2}", INSULATION_RESISTANCE_VOLT, INSULATION_RESISTANCE_TIME, res));
            }
            return resRtn;
        }
        #endregion
    }
}
