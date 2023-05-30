using PhoenixonLibrary.Device;
using Renault_BT6.윈도우;
using Renault_BT6.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Renault_BT6.Forms;

namespace Renault_BT6
{
    public partial class MainWindow
    {
        public bool IR_Plus_Bushing(TestItem ti)
        {
            isProcessingUI(ti);

            int volt = EOL.IR_Recipe.BushingPlus.Volt;
            int time = EOL.IR_Recipe.BushingPlus.Time;

            int plusCH = EOL.IR_Recipe.PlusCH;
            int minusCH = EOL.IR_Recipe.BushingUpperCH;

            #region Temp Get and MES 

            double temp = 0.0;

            if (CONFIG.EolInspType == InspectionType.HIPOT)
            {
                temp = Device.Tempr.GetTemp;
            }
            else
            {
                temp = Device.TemprCT100.GetTemprature();
            }

            ti.refValues_.Add(MakeDetailItem(EOL.MesID.AmbientTempHiPot, temp.ToString() ));
            #endregion

            //221106
            FormPartsCountHipotSetting PCHS = new FormPartsCountHipotSetting(this, "START");
            PCHS.CountRealData();
            this.SetPartsCountData(ti, "StartRun");
            
            try
            {
                double res = 0.0;

                
                Judge judge_tpye = Judge.NULL;
                judge_tpye = Device.Chroma.GetResistance(plusCH,
                                                                          minusCH,
                                                                          volt,
                                                                          time,
                                                                          out res);

                switch (judge_tpye)
                {
                    case Judge.PASS:
                        if (res > 9.9E+30)
                            ti.Value_ = 999999;
                        else
                            ti.Value_ = res;
                        break;
                    case Judge.USER_STOP: ti.Value_ = _CASE_113; break;
                    case Judge.CAN_NOT_TEST: ti.Value_ = _CASE_114; break;
                    case Judge.TESTING: ti.Value_ = _CASE_115;  break;
                    case Judge.STOP: ti.Value_  = _CASE_112;  break;
                    case Judge.DC_HI: ti.Value_  = _CASE_33;break;
                    case Judge.DC_LO: ti.Value_  = _CASE_34; ; break;
                    case Judge.DC_ARC: ti.Value_  = _CASE_35;  break;
                    case Judge.DC_IO: ti.Value_  = _CASE_36;  break;
                    case Judge.DC_CHECK_LOW: ti.Value_  = _CASE_37;  break;
                    case Judge.DC_ADV_OVER: ti.Value_  = _CASE_38; break;
                    case Judge.DC_ADI_OVER: ti.Value_  = _CASE_39;  break;
                    case Judge.DC_IO_F: ti.Value_  = _CASE_43;  break;
                    case Judge.IR_HI: ti.Value_  = _CASE_49;  break;
                    case Judge.IR_LO: ti.Value_  = _CASE_50; break;
                    case Judge.IR_IO: ti.Value_  = _CASE_52; break;
                    case Judge.IR_ADV_OVER: ti.Value_  = _CASE_54; break;
                    case Judge.IR_ADI_OVER: ti.Value_  = _CASE_55;  break;
                    case Judge.DC_IR_GR_CONT: ti.Value_  = _CASE_120_IR; break;
                    case Judge.DC_IR_TRIPPED: ti.Value_ = _CASE_121_IR;  break;
                    default:
                        ti.Value_ = judge_tpye.ToString();
                        break;
                }

                SetContorlLimitData(2, ti.Value_.ToString());

                LogState(LogType.Info, string.Format("Hipot - {0}V {1}s Resistance - {2}, JudgeType-{3}",
                                                 volt,
                                                 time,
                                                 res,
                                                 judge_tpye.ToString()));

                #region  MES 상세 수집 항목
                if (judge_tpye == Judge.PASS)
                {
                    if (Device.Chroma.FetchList.Count == 60)
                        Device.Chroma.FetchList[59] = res.ToString();
                    else if (Device.Chroma.FetchList.Count == 59)
                        Device.Chroma.FetchList.Add(res.ToString());

                    int count = 5;
                    int index = 0;
                    int n = 0;

                    foreach (var value in Device.Chroma.FetchList)
                    {
                        index++;
                        if (count == index)
                        {
                            LogState(LogType.Info, string.Format("Hipot  FetchList,Resistance,{0}, {1}s",
                                                            value,
                                                            count));

                            ti.refValues_.Add(MakeDetailItem(EOL.MesID.BushingPlusResult[n],
                                                                        value));
                            count += 5;
                            n++;
                        }
                    }
                }
                #endregion

            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }

        public bool IR_Minus_Bushing(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                var res = 0.0;

                int volt = EOL.IR_Recipe.BushingMinus.Volt;
                int time = EOL.IR_Recipe.BushingMinus.Time;

                int plusCH = EOL.IR_Recipe.MinusCH;
                int minusCH = EOL.IR_Recipe.BushingUpperCH;
          

                Judge judge_tpye = Judge.NULL;
                judge_tpye = Device.Chroma.GetResistance(plusCH,
                                                                          minusCH,
                                                                          volt,
                                                                          time,
                                                                          out res);

                switch (judge_tpye)
                {
                    case Judge.PASS:
                        if (res > 9.9E+30)
                            ti.Value_ = 999999;
                        else
                            ti.Value_ = res;
                        break;
                    default:
                        ti.Value_ = judge_tpye.ToString();
                        break;
                }

                SetContorlLimitData(4, ti.Value_.ToString());

                LogState(LogType.Info, string.Format("Hipot - {0}V {1}s Resistance - {2}, JudgeType-{3}",
                                             volt,
                                             time,
                                             res,
                                             judge_tpye.ToString()));


                if(judge_tpye == Judge.PASS)
                {
                    if (Device.Chroma.FetchList.Count == 60)
                        Device.Chroma.FetchList[59] = res.ToString();
                    else if (Device.Chroma.FetchList.Count == 59)
                        Device.Chroma.FetchList.Add(res.ToString());

                    // MES 상세 수집 항목
                    int count = 5;
                    int index = 0;
                    int n = 0;

                    foreach (var value in Device.Chroma.FetchList)
                    {
                        index++;
                        if (count == index)
                        {
                            LogState(LogType.Info, string.Format("Hipot FetchList,Resistance,{0},{1}s",
                                                            value,
                                                            count));

                            ti.refValues_.Add(MakeDetailItem(EOL.MesID.BushingMinusResult[n],
                                                                        value));
                            count += 5;
                            n++;
                        }
                    }
                }
             
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }


        public bool IR_Plus_Cooling_Plate(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                var res = 0.0;

                int resVolt = EOL.IR_Recipe.CoolingPlatePlus.Volt;
                int resTime = EOL.IR_Recipe.CoolingPlatePlus.Time;

                int plusCH = EOL.IR_Recipe.PlusCH;
                int minusCH = EOL.IR_Recipe.CoolingPlateLeftCH;

                double Range = double.Parse(EOL.IR_Recipe.CoolingPlatePlus.Range);

                Judge judge_tpye = Judge.NULL;
                judge_tpye = Device.Chroma.GetResistance(plusCH,
                                                                      minusCH,
                                                                    resVolt,
                                                                    resTime,
                                                                    out res,
                                                                    Range,
                                                                    Range);

                switch (judge_tpye)
                {
                    case Judge.PASS:
                        if ( res > 3E+4) 
                        {
                            LogState(LogType.Info, string.Format("Hipot Measure Resistance, Hipot Fetch Resistance {0},{1}s",
                                res,
                                Device.Chroma.FetchList[Device.Chroma.FetchList.Count - 2]));

                            if (double.Parse(Device.Chroma.FetchList[Device.Chroma.FetchList.Count - 2]) > 9.9E+30)
                            {
                                ti.Value_ = 999999;
                            }
                            else
                            {
                                ti.Value_ = Device.Chroma.FetchList[Device.Chroma.FetchList.Count - 2]; 
                            }
                        }
                        else
                            ti.Value_ = res;
                        break;
                    case Judge.USER_STOP: ti.Value_ = _CASE_113; break;
                    case Judge.CAN_NOT_TEST: ti.Value_ = _CASE_114; break;
                    case Judge.TESTING: ti.Value_ = _CASE_115; break;
                    case Judge.STOP: ti.Value_ = _CASE_112; break;
                    case Judge.DC_HI: ti.Value_ = _CASE_33; break;
                    case Judge.DC_LO: ti.Value_ = _CASE_34; ; break;
                    case Judge.DC_ARC: ti.Value_ = _CASE_35; break;
                    case Judge.DC_IO: ti.Value_ = _CASE_36; break;
                    case Judge.DC_CHECK_LOW: ti.Value_ = _CASE_37; break;
                    case Judge.DC_ADV_OVER: ti.Value_ = _CASE_38; break;
                    case Judge.DC_ADI_OVER: ti.Value_ = _CASE_39; break;
                    case Judge.DC_IO_F: ti.Value_ = _CASE_43; break;
                    case Judge.IR_HI: ti.Value_ = _CASE_49; break;
                    case Judge.IR_LO: ti.Value_ = _CASE_50; break;
                    case Judge.IR_IO: ti.Value_ = _CASE_52; break;
                    case Judge.IR_ADV_OVER: ti.Value_ = _CASE_54; break;
                    case Judge.IR_ADI_OVER: ti.Value_ = _CASE_55; break;
                    case Judge.DC_IR_GR_CONT: ti.Value_ = _CASE_120_IR; break;
                    case Judge.DC_IR_TRIPPED: ti.Value_ = _CASE_121_IR; break;
                    default:
                        ti.Value_ = judge_tpye.ToString();
                        break;
                }


                LogState(LogType.Info, string.Format("Hipot - {0}V {1}s Resistance - {2}, JudgeType-{3}",
                                             resVolt,
                                             resTime,
                                             res,
                                             judge_tpye.ToString()));



                SetContorlLimitData(3, ti.Value_.ToString());

                if (judge_tpye == Judge.PASS)
                {
                    // MES 상세 수집 항목
                    int count = 5;
                    int index = 0;
                    int n = 0;

                    //if (Device.Chroma.FetchList.Count == 60)
                    //    Device.Chroma.FetchList[59] = res.ToString();
                    //else if (Device.Chroma.FetchList.Count == 59)
                    //    Device.Chroma.FetchList.Add(res.ToString());

                    foreach (var value in Device.Chroma.FetchList)
                    {
                        index++;
                        if (count == index)
                        {
                            LogState(LogType.Info, string.Format("Hipot FetchList,Resistance,{0},{1}s",
                                                            value,
                                                            count));

                            ti.refValues_.Add(MakeDetailItem(EOL.MesID.CoolingPlusResult[n],
                                                                        value));
                            count += 5;
                            n++;
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                LogState(LogType.Fail, ex.StackTrace);
            }

            return JudgementTestItem(ti);
        }

        public bool IR_Minus_Cooling_Plate(TestItem ti)
        {
            isProcessingUI(ti);

            try
            {
                var res = 0.0;

                int resVolt = EOL.IR_Recipe.CoolingPlateMinus.Volt;
                int resTime = EOL.IR_Recipe.CoolingPlateMinus.Time;

                int plusCH = EOL.IR_Recipe.MinusCH;
                int minusCH = EOL.IR_Recipe.CoolingPlateLeftCH;

         

                Judge judge_tpye = Judge.NULL;
                judge_tpye = Device.Chroma.GetResistance(plusCH,
                                                                      minusCH,
                                                                    resVolt,
                                                                    resTime,
                                                                    out res);
                                                                 
                switch (judge_tpye)
                {
                    case Judge.PASS:
                        if (res > 9.9E+30)
                            ti.Value_ = 999999;
                        else
                            ti.Value_ = res;
                        break;
                    default:
                        ti.Value_ = judge_tpye.ToString();
                        break;
                }


                LogState(LogType.Info, string.Format("Hipot - {0}V {1}s Resistance - {2}, JudgeType-{3}",
                                             resVolt,
                                             resTime,
                                             res,
                                             judge_tpye.ToString()));

                SetContorlLimitData(5, ti.Value_.ToString());

                if (judge_tpye == Judge.PASS)
                {
                    if (Device.Chroma.FetchList.Count == 60)
                        Device.Chroma.FetchList[59] = res.ToString();
                    else if (Device.Chroma.FetchList.Count == 59)
                        Device.Chroma.FetchList.Add(res.ToString());


                    // MES 상세 수집 항목
                    int count = 5;
                    int index = 0;
                    int n = 0;

                    foreach (var value in Device.Chroma.FetchList)
                    {
                        index++;
                        if (count == index)
                        {
                            LogState(LogType.Info, string.Format("Hipot FetchList,Resistance,{0},{1}s",
                                                            value,
                                                            count));

                            ti.refValues_.Add(MakeDetailItem(EOL.MesID.CoolingMinusResult[n],
                                                                        value));
                            count += 5;
                            n++;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LogState(LogType.Fail, ex.StackTrace);
            }

            return JudgementTestItem(ti);
        }

    }
}
