using EOL_BASE.클래스;
using EOL_BASE.윈도우;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        TestItem withTi;
        TestItem irTi;
        TestItem plusIvTi;

        List<double> isolationVoltageList;

        //211018 JKD - 신규라인에 IV Check 취소

        void SetTestItemCond()
        {
            //210802 JKD - 내전압, 절연저항 결과값 가져오기
            foreach (var item in modelList[selectedIndex].TestItemList)
            {
                if (item.Value.SingleMethodName.Contains("WithstandVoltage"))
                {
                    withTi = item.Value;
                }

                if (item.Value.SingleMethodName.Contains("Plus_IR"))
                {
                    irTi = item.Value;
                }

                if (item.Value.SingleMethodName.Contains("Plus_IV_Check"))
                {
                    plusIvTi = item.Value;
                }
            }
        }

        public bool Plus_IV_Check(TestItem ti)
        {
            isProcessingUI(ti);

            SetTestItemCond();

            ////메뉴얼테스트
            //withTi.Value_ = _CASE_35;
            //irTi.Result = "PASS";

            // 210914 JKD - 기존라인 IV 검사 시작 될 경우 IV_SKIP 처리
            if (!isLineFlag)
            {
                ti.Value_ = "IV_SKIP";
                ti.Result = "PASS";

                return JudgementTestItem(ti);
            }

            #region 단위검사는 무조건 도는거고, 자동검사일때는 아래 조건을 충족해야함

            //자동검사가 아닐때나
            var firstCond = !plc.isTesting_Flag;

            //내전압검사를 했고, 내전압이 아크일때
            var secondCond = withTi.Value_ != null && withTi.Value_.ToString() == _CASE_35;

            //절연저항 검사결과가 패스일때
            var thirdCond = irTi.Result == "PASS";

            #endregion

            if (firstCond || (secondCond && thirdCond))
            {
                #region 설비지그 제어 부분
                if (!ModeSet_IV())
                {
                    ti.Value_ = _JIG_NOT_ACTIVE;

                    LogState(LogType.Fail, "IV Check ModeSet Fail!");
                    return JudgementTestItem(ti);
                }
                #endregion

                #region 반복 취득 구문
                var specMax = double.Parse(ti.Max.ToString());
                var specMin = double.Parse(ti.Min.ToString());
                if (IV_Voltage(ti, specMax, specMin))
                {
                    //pass일때 내전압 결과값을 변환시킨다
                    foreach (var item in modelList[selectedIndex].TestItemList)
                    {
                        if (item.Value.SingleMethodName == "WithstandVoltage1")
                        {
                            item.Value.Result = "PASS";
                            break;
                        }
                    }
                }
                #endregion
                                
                DetailItems all_IV_Val = new DetailItems() { Key = "W2MLMTE4072" };
                DetailItems all_IV_Time = new DetailItems() { Key = "W2MLMTE4073" };

                #region 상세수집 설정 부분

                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();

                foreach (var iv in ivList)
                {
                    all_IV_Val.Reportitems.Add(iv.lastValue);
                    sb1.Append(iv.lastValue + "&");
                    all_IV_Time.Reportitems.Add(iv.mSec * 0.001);
                    sb2.Append((iv.mSec * 0.001) + "&");
                }

                LogState(LogType.Info, "All IV Value:" + sb1.ToString());
                LogState(LogType.Info, "All IV Time:" + sb2.ToString());

                ti.refValues_.Add(all_IV_Val);
                ti.refValues_.Add(all_IV_Time);
                #endregion

                #region 판정
                StringBuilder chList = new StringBuilder();
                foreach (var iv in ivList)
                {
                    if (iv.lastValue >= specMin && iv.lastValue <= specMax)
                    {

                    }
                    else
                    {
                        chList.Append(iv.Ch);
                        chList.Append(":");
                        chList.Append(iv.lastValue);
                        chList.Append("/");
                    }
                }
                 

                if (chList.Length != 0)
                {
                    chList = chList.Remove(chList.Length - 1, 1);
                    ti.Value_ = chList.ToString(); // NG시 채널로 결과값 출력
                }
                else
                {
                    if (ivList.Count != 0)
                        ti.Value_ = ivList.Select(x => x.lastValue).ToList().Max();
                    else
                        ti.Value_ = "NULL";
                }
                #endregion

            }
            else
            {
                //항상 PASS 처리
                ti.Value_ = "IV_SKIP";
                ti.Result = "PASS";

                ti.refValues_.Add(MakeDetailItem("W2MLMTE4072", "IV_SKIP"));
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4073", "IV_SKIP"));
            }

            return JudgementTestItem(ti);
        }

        public bool Minus_IV_Check(TestItem ti)
        {
            isProcessingUI(ti);

            SetTestItemCond();

            ////메뉴얼테스트
            //withTi.Value_ = _CASE_35;
            //irTi.Result = "PASS";

            // 210914 JKD - 기존라인 IV 검사 시작 될 경우 IV_SKIP 처리
            if (!isLineFlag)
            {
                ti.Value_ = "IV_SKIP";
                ti.Result = "PASS";

                return JudgementTestItem(ti);
            }

            #region 단위검사는 무조건 도는거고, 자동검사일때는 아래 조건을 충족해야함

            //자동검사가 아닐때나
            var firstCond = !plc.isTesting_Flag;

            //내전압검사를 했고, 내전압이 아크일때
            var secondCond = withTi.Value_ != null && withTi.Value_.ToString() == _CASE_35;

            //절연저항 검사결과가 패스일때
            var thirdCond = irTi.Result == "PASS";

            #endregion

            if (firstCond || (secondCond && thirdCond))
            {
                #region Plus IV 설비지그 제어 확인 부분
                if (plusIvTi.Value_.ToString() == _JIG_NOT_ACTIVE)
                {
                    ti.Value_ = _JIG_NOT_ACTIVE;
                    return JudgementTestItem(ti);
                }
                #endregion

                #region 반복 취득 구문
                var specMax = double.Parse(ti.Max.ToString());
                var specMin = double.Parse(ti.Min.ToString());
                if (IV_Voltage(ti, specMax, specMin))
                {
                    //pass일때 내전압 결과값을 변환시킨다
                    foreach (var item in modelList[selectedIndex].TestItemList)
                    {
                        if (item.Value.SingleMethodName == "WithstandVoltage1")
                        {
                            item.Value.Result = "PASS";
                            break;
                        }
                    }
                }
                #endregion

                DetailItems all_IV_Val = new DetailItems() { Key = "W2MLMTE4074" };
                DetailItems all_IV_Time = new DetailItems() { Key = "W2MLMTE4075" };

                #region 상세수집 설정 부분

                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();

                foreach (var iv in ivList)
                {
                    all_IV_Val.Reportitems.Add(iv.lastValue);
                    sb1.Append(iv.lastValue + "&");
                    all_IV_Time.Reportitems.Add(iv.mSec * 0.001);
                    sb2.Append((iv.mSec * 0.001) + "&");
                }

                LogState(LogType.Info, "All IV Value:" + sb1.ToString());
                LogState(LogType.Info, "All IV Time:" + sb2.ToString());

                ti.refValues_.Add(all_IV_Val);
                ti.refValues_.Add(all_IV_Time);
                #endregion

                #region 판정
                StringBuilder chList = new StringBuilder();
                foreach (var iv in ivList)
                { 
                    if (iv.lastValue >= specMin && iv.lastValue <= specMax)
                    {

                    }
                    else
                    {
                        chList.Append(iv.Ch);
                        chList.Append(":");
                        chList.Append(iv.lastValue);
                        chList.Append("/");
                    }
                }

                if(chList.Length != 0)
                {
                    chList = chList.Remove(chList.Length - 1, 1);
                    ti.Value_ = chList.ToString(); // NG시 채널로 결과값 출력
                }
                else
                {
                    if (ivList.Count != 0)
                        ti.Value_ = ivList.Select(x => x.lastValue).ToList().Max();
                    else
                        ti.Value_ = "NULL";
                }
                #endregion

            }
            else
            {
                //항상 PASS 처리
                ti.Value_ = "IV_SKIP";
                ti.Result = "PASS";

                ti.refValues_.Add(MakeDetailItem("W2MLMTE4074", "IV_SKIP"));
                ti.refValues_.Add(MakeDetailItem("W2MLMTE4075", "IV_SKIP"));
            }

            return JudgementTestItem(ti);
        }

        List<SingleIV> ivList = new List<SingleIV>();

        //모든 채널의 최대값(사용안함)
        double ivMAX = 0.0;
        //모든 채널의 최대값의 채널이름(사용안함)
        string ivMAXCh = "";

        public bool IV_Voltage(TestItem ti,double specMax,double specMin)
        { 
            ivList = new List<SingleIV>();
            ivMAX = 0.0;
            ivMAXCh = "";

            if (daq970_3 == null)
            {
                ti.Value_ = "NotConnected";
                 
                return false;
            }

            try
            {
                string ivChannel;
                string[] chStrList;
                string[] channelList;
                var daqChInfo = new DAQChannelInfo(localTypes);

                chStrList = daqChInfo.String_IVCH.ToString().Split(',');
                if (ti.SingleMethodName == "Plus_IV_Check")
                {
                    channelList = daqChInfo.Plus_IVCH.ToString().Split(',');
                    ivChannel = daqChInfo.Plus_IVCH.ToString();
                }
                else // Minus_IV_Check
                {
                    channelList = daqChInfo.Minus_IVCH.ToString().Split(',');
                    ivChannel = daqChInfo.Minus_IVCH.ToString();
                }

                //각 채널별로 최대 30초 기다리고, 
                //*RST
                //ROUT: CLOS(@101, 102, 103, 104, 105)
                //CONF: VOLT: DC 10,DEF,(@101,102,103,104,105)
                //READ?
                for (int i = 0; i < channelList.Length; i++)
                {
                    if (isStop || ispause)
                    {
                        LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");
                        return false;
                    }

                    List<double> singleChData = new List<double>();

                    LogState(LogType.Info, string.Format("CH Setting:{0}",channelList[i]));

                    if (daq970_3.RST())
                    {
                        Thread.Sleep(100);
                        if (daq970_3.ROUT_CLOS(channelList[i]))
                        {
                            Thread.Sleep(100);
                            if (daq970_3.CONF_VOLT_DC_SET(channelList[i]))
                            {
                                Thread.Sleep(300);
                            }
                        }
                    }

                    Stopwatch sp = new Stopwatch();
                    sp.Start();
                    while (true)
                    {
                        if (isStop || ispause)
                        {
                            LogState(LogType.EMERGENCY, "EMERGENCY STOPPED !!!");
                            return false;
                        }

                        string rtVal = "";
                        if (daq970_3.READ(out rtVal))
                        {
                            Thread.Sleep(200);
                            double val = 0.0;
                            double.TryParse(rtVal, out val);

                            if (ivMAX < val)
                            {
                                ivMAX = val;
                                ivMAXCh = chStrList[i];
                            }

                            singleChData.Add(val);

                            if (ti.SingleMethodName == "Plus_IV_Check")
                            {
                                LogState(LogType.Info, string.Format("Plus IV Check, CH:{0}, VAL:{1},mSec:{2}", chStrList[i], val, sp.ElapsedMilliseconds));
                            }
                            else
                            {
                                if (val < 0)
                                {
                                    val = Math.Abs(val);
                                    LogState(LogType.Success, string.Format("Minus IV Check, CH:{0}, VAL:{1},mSec:{2}",chStrList[i],val,sp.ElapsedMilliseconds));
                                }
                                else 
                                {
                                    val = val * -1;
                                    LogState(LogType.Fail, string.Format("Minus IV Check Fail!!, CH:{0}, VAL:{1},mSec:{2}",chStrList[i],val,sp.ElapsedMilliseconds));
                                }
                            }

                            //210813 유경석사원이 판정시에 절대값 처리 요청
                            //if ((val >= specMin && val <= specMax) || sp.ElapsedMilliseconds >= 30000)
                            if (Math.Abs(val) <= specMax || sp.ElapsedMilliseconds >= 30000)
                            {
                                ivList.Add(new SingleIV()
                                {
                                    Ch = chStrList[i],
                                    DataList = singleChData,
                                    mSec = (int)sp.ElapsedMilliseconds,
                                    lastValue = Math.Abs(val),
                                });
                                sp.Stop();

                                break;
                            }
                            else
                            {
                                //조건충족이 안되서 루프타는중
                            }
                        }
                        else
                        {
                            Thread.Sleep(200);
                            LogState(LogType.Fail, "DAQ970 READ FAIL");
                        }
                    }
                }

                //30000ms 초과라면 false
                if(ivList.Any(x=>x.mSec >= 30000))
                {
                    return false;
                }
                else
                {
                    return true;
                } 
            }
            catch (Exception ex)
            {

                LogState(LogType.Fail, string.Format("{0}", ex));
                return false;
            }
        }

        private bool ModeSet_IV()
        {
            //if (plc.isTesting_Flag) // 검사중인지 비트 확인
            //{
            //    if (!plc.iv_Check_Mode_Flag) // 초기 IV Flag 확인
            //    {
            //        plc.IV_Check_Mode(true); // IV Mode 비트 On
            //        LogState(LogType.Success, "IV Check Mode Activated");

            //        int cnt = 0;
            //        while (true)
            //        {
            //            if (cnt >= 10000)
            //            {
            //                plc.IV_Check_Mode(false); // Timeout, PLC IV Pin 비트 Off
            //                LogState(LogType.Fail, "10s TimeOut. IV Check Mode Fail!");

            //                return false;
            //            }

            //            if (plc.plc_isIV_Mode_Ok) // PLC측 IV Pin 비트 OK 확인
            //            {
            //                plc.IV_Check_Mode(false); // IV Mode 비트 Off
            //                LogState(LogType.Success, "IV Check Mode Success");

            //                return true;
            //            }

            //            Thread.Sleep(100);
            //            cnt += 100;
            //        }
                    
            //    }
            //}
            return true;
        }

    }
}
