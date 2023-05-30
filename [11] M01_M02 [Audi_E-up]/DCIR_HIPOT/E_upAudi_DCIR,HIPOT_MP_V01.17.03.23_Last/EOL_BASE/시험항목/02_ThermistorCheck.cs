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
        double tempTher1 = 0.0;
        double tempTher2 = 0.0;
        //210312 WJS ADD 서미스터3
        double tempTher3 = 0.0;

        double tempRes1 = 0.0;
        double tempRes2 = 0.0;
        double tempshortcheck = 0.0; // 220609 ks yoo 

        int ambientMin = 19;
        int ambientMax = 28;

        public double GetTempToResist_Audi(double resist)
        {
            double r_25 = 10;
            double r_t = resist;

            var a = 0.003354016;
            var b = 0.0002550561;
            var c = 1.627639E-06;
            var d = 2.215254E-07;
            double R = r_t / r_25;

            var t = 1 / ((a + (b * Math.Log(R)) + (c * Math.Pow(Math.Log(R), 2)) + (d * Math.Pow(Math.Log(R), 3))));

            var decT = t - 273.15;

            string log = string.Format("1 / ({0} + {1} * LN({2}) + {3} * (LN({4})^2) + {5} *LN({6})^3)",
                                      a,
                                      b,
                                      R,
                                      c,
                                      R,
                                      d,
                                      R);
            
            log += "\r\n";
            log += string.Format("{0} -273.15 = {1}", t, decT);

            LogState(LogType.Info, log);

            return decT;
        }

        float ambientTemp = 0.0f;

        public bool ThermistorCheck1(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            tempTher1 = 0.0;
            tempTher2 = 0.0;
            //210312 WJS ADD PORS FL THER3
            tempTher3 = 0.0;
            tempRes1 = 0.0;
            tempRes2 = 0.0;
            tempshortcheck = 0.0; //220609 thermsitor short check - ks yoo

            ambientTemp = 0.0f;

            ti.refValues_.Clear();
            ambientTemp = temps.tempStr;

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR) /* ||
                localTypes == ModelType.E_UP)*/ //220609 ks yoo 아우디와 이업 구분 나눔
            {
                var dmmChInfo = new DMMChannelInfo(localTypes);

                var resList = new List<double>();

                //이업, 아우디는 써미스터1 시점에 2까지 그냥 들고 있으라함

                if (!keysight.MeasRes_Multi(out resList, dmmChInfo.ModuleRes, 2))
                {
                    ti.Value_ = _DEVICE_NOT_READY;

                    return JudgementTestItem(ti);
                }

                if (resList[0] > 9.9E+30)
                {
                    resList[0] = 99999;
                }
                if (resList[1] > 9.9E+30)
                {
                    resList[1] = 99999;
                }

                tempRes1 = resList[0] * 0.001;

                tempRes2 = resList[1] * 0.001;

                //이업, 아우디는 온도 환산은 temp dev할때만 가져다 쓰고 저항값으로 Judge, 상세보고함

                //근데 ThermistorDeviation() 같이 편차구할때는 온도값으로 환산해서 계산 되어야 한다고함, 때문에 미리 구해놓음

                tempTher1 = GetTempToResist_Audi(tempRes1);

                tempTher2 = GetTempToResist_Audi(tempRes2);

                ti.Value_ = tempRes1; 

                LogState(LogType.Info, "Thermistor Check Ambient:" + ambientTemp.ToString());

                LogState(LogType.Info, string.Format("rs 1 : Resistance:{0} / Temp:{1}", tempRes1, tempTher1));

                LogState(LogType.Info, string.Format("rs 2 : Resistance:{0} / Temp:{1}", tempRes2, tempTher2));

                //상세수집 ㄱ

                if(localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_AMBIENT___TEMP, ambientTemp.ToString()));

                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_TEMP1_____TEMP, tempTher1.ToString()));
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_AUD_NOR_AMBIENT___TEMP, ambientTemp.ToString()));

                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_AUD_NOR_TEMP1_____TEMP, tempTher1.ToString()));
                    }
                }
                else if (localTypes == ModelType.E_UP)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_AMBIENT___TEMP, ambientTemp.ToString()));

                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_TEMP1_____TEMP, tempTher1.ToString()));
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_E_UP_AMBIENT___TEMP, ambientTemp.ToString()));

                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_E_UP_TEMP1_____TEMP, tempTher1.ToString()));
                    }
                }
            }
            else if (localTypes == ModelType.E_UP)   //20220608 E-UP검사 변경으로인해 추가 BY 경석
            {
                {
                    var dmmChInfo = new DMMChannelInfo(localTypes);

                    var resList = new List<double>();

                    //이업, 아우디는 써미스터1 시점에 2까지 그냥 들고 있으라함

                    if (!keysight.MeasRes_Multi(out resList, dmmChInfo.ModuleRes, 3))
                    {
                        ti.Value_ = _DEVICE_NOT_READY;

                        return JudgementTestItem(ti);
                    }

                    if (resList[0] > 9.9E+30)
                    {
                        resList[0] = 99999;
                    }
                    if (resList[1] > 9.9E+30)
                    {
                        resList[1] = 99999;
                    }

                    tempRes1 = resList[0] * 0.001;

                    tempRes2 = resList[1] * 0.001;

                    tempshortcheck = resList[2] * 0.001;


                    //이업, 아우디는 온도 환산은 temp dev할때만 가져다 쓰고 저항값으로 Judge, 상세보고함

                    //근데 ThermistorDeviation() 같이 편차구할때는 온도값으로 환산해서 계산 되어야 한다고함, 때문에 미리 구해놓음

                    tempTher1 = GetTempToResist_Audi(tempRes1);

                    tempTher2 = GetTempToResist_Audi(tempRes2);

                    ti.Value_ = tempRes1;

                    LogState(LogType.Info, "Thermistor Check Ambient:" + ambientTemp.ToString());

                    LogState(LogType.Info, string.Format("rs 1 : Resistance:{0} / Temp:{1}", tempRes1, tempTher1));

                    LogState(LogType.Info, string.Format("rs 2 : Resistance:{0} / Temp:{1}", tempRes2, tempTher2));

                    //상세수집 ㄱ

                    if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
                    {
                        if (pro_Type == ProgramType.EOLInspector)
                        {
                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_AMBIENT___TEMP, ambientTemp.ToString()));

                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_TEMP1_____TEMP, tempTher1.ToString()));
                        }
                        else if (pro_Type == ProgramType.VoltageInspector)
                        {
                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_AUD_NOR_AMBIENT___TEMP, ambientTemp.ToString()));

                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_AUD_NOR_TEMP1_____TEMP, tempTher1.ToString()));
                        }
                    }
                    else if (localTypes == ModelType.E_UP)
                    {
                        if (pro_Type == ProgramType.EOLInspector)
                        {
                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_AMBIENT___TEMP, ambientTemp.ToString()));

                            ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_TEMP1_____TEMP, tempTher1.ToString()));
                        }
                        else if (pro_Type == ProgramType.VoltageInspector)
                        {
                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_E_UP_AMBIENT___TEMP, ambientTemp.ToString()));

                            ti.refValues_.Add(MakeDetailItem(KEY_VOL_E_UP_TEMP1_____TEMP, tempTher1.ToString()));
                        }
                    }
                }
            }

            else if (localTypes == ModelType.PORSCHE_NORMAL
               || localTypes == ModelType.PORSCHE_MIRROR
               || localTypes == ModelType.MASERATI_NORMAL
               || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL 
               || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
               || localTypes == ModelType.MASERATI_M183_NORMAL  //221101 wjs add mase m183
               )
            {

                //201217 wjs add maserati
                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    LogState(LogType.Info, "Thermistor Check Ambient:" + ambientTemp.ToString());
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_AMBIENT___TEMP, ambientTemp.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_AMBIENT___TEMP, ambientTemp.ToString()));

                    var rs12 = 0.0;
                    var adc = CMClist[12];
                    cmc.CalculateToMC33772ModuleTemp(adc, out rs12);
                    LogState(LogType.Info, "Thermistor Check 1 - ADC:" + adc.ToString());
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_TEMP1______ADC, rs12.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_TEMP1______ADC, rs12.ToString()));

                    ti.Value_ = tempTher1 = rs12;
                }
                else if (localTypes == ModelType.MASERATI_NORMAL)
                {
                    LogState(LogType.Info, "Thermistor Check Ambient:" + ambientTemp.ToString());
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_AMBIENT___TEMP, ambientTemp.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_AMBIENT___TEMP, ambientTemp.ToString()));

                    var rs12 = 0.0;
                    var adc = CMClist[12];
                    cmc.CalculateToMC33772ModuleTemp(adc, out rs12);
                    LogState(LogType.Info, "Thermistor Check 1 - ADC:" + adc.ToString());
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_TEMP1______ADC, rs12.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_TEMP1______ADC, rs12.ToString()));

                    ti.Value_ = tempTher1 = rs12;
                }
                else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    LogState(LogType.Info, "Thermistor Check Ambient:" + ambientTemp.ToString());
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_AMBIENT___TEMP, ambientTemp.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_AMBIENT___TEMP, ambientTemp.ToString()));

                    var rs12 = 0.0;
                    var adc = CMClist[12];
                    cmc.CalculateToMC33772ModuleTemp(adc, out rs12);
                    LogState(LogType.Info, "Thermistor Check 1 - ADC:" + adc.ToString());
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_TEMP1______ADC, rs12.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_TEMP1______ADC, rs12.ToString()));

                    ti.Value_ = tempTher1 = rs12;
                }
                //221101 wjs add mase m183
                else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    LogState(LogType.Info, "Thermistor Check Ambient:" + ambientTemp.ToString());
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_AMBIENT___TEMP, ambientTemp.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_AMBIENT___TEMP, ambientTemp.ToString()));

                    var rs12 = 0.0;
                    var adc = CMClist[12];
                    cmc.CalculateToMC33772ModuleTemp(adc, out rs12);
                    LogState(LogType.Info, "Thermistor Check 1 - ADC:" + adc.ToString());
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_TEMP1______ADC, rs12.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_TEMP1______ADC, rs12.ToString()));

                    ti.Value_ = tempTher1 = rs12;
                }
            }

            return JudgementTestItem(ti);
        }

        public bool Thermsitorshortcheck(TestItem ti) //220609 thermsitor short check - ks yoo
        {
            isProcessingUI(ti);
            ti.refValues_.Clear();
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR ||
                localTypes == ModelType.E_UP)
            {
                if (tempshortcheck > 10000)
                {
                    ti.Value_ = "OPEN";
                    LogState(LogType.Info, "thermistor Short Check : " + tempshortcheck.ToString());
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_LINE_THERMSITORSHORTCHECK, tempshortcheck.ToString()));
                }
                else
                {
                    ti.Value_ = tempshortcheck;
                    LogState(LogType.Info, "thermistor Short Check : " + tempshortcheck.ToString());
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_LINE_THERMSITORSHORTCHECK, tempshortcheck.ToString()));
                }
            }

            return JudgementTestItem(ti);
        }

        

        public bool ThermistorCheck2(TestItem ti)
        {
            isProcessingUI(ti);
            ti.refValues_.Clear();
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR ||
                localTypes == ModelType.E_UP)
            {
                ti.Value_ = tempRes2;

                if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_NOR_TEMP2_____TEMP, tempTher2.ToString()));
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_AUD_NOR_TEMP2_____TEMP, tempTher2.ToString()));
                    }
                }
                else if (localTypes == ModelType.E_UP)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_TEMP2_____TEMP, tempTher2.ToString()));
                    }
                    else if (pro_Type == ProgramType.VoltageInspector)
                    {
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_E_UP_TEMP2_____TEMP, tempTher2.ToString()));
                    }
                }
                else{}
            }
            else if (localTypes == ModelType.PORSCHE_NORMAL
               || localTypes == ModelType.PORSCHE_MIRROR
               || localTypes == ModelType.MASERATI_NORMAL
               || localTypes == ModelType.PORSCHE_FACELIFT_NORMAL 
               || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
               || localTypes == ModelType.MASERATI_M183_NORMAL  //221101 wjs add mase m183
               )

            { 
                var rs13 = 0.0;
                var adc = CMClist[13];
                cmc.CalculateToMC33772ModuleTemp(adc, out rs13);
                LogState(LogType.Info, "Thermistor Check 2 - ADC:" + adc.ToString());
                ti.Value_ = tempTher2 = rs13;

                //201217 wjs add maserati
                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_TEMP2______ADC, rs13.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_TEMP2______ADC, rs13.ToString()));
                }
                else if (localTypes == ModelType.MASERATI_NORMAL)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_TEMP2______ADC, rs13.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_TEMP2______ADC, rs13.ToString()));
                }
                //210312 WJS ADD PORS FL
                else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_TEMP2______ADC, rs13.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_TEMP2______ADC, rs13.ToString()));
                }
                //221101 wjs add mase m183
                else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    if (pro_Type == ProgramType.EOLInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_TEMP2______ADC, rs13.ToString()));
                    else if (pro_Type == ProgramType.VoltageInspector)
                        ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_TEMP2______ADC, rs13.ToString()));
                }
                //if (pro_Type == ProgramType.EOLInspector)
                //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_TEMP2______ADC, rs13.ToString()));
                //else if (pro_Type == ProgramType.VoltageInspector)
                //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_TEMP2______ADC, rs13.ToString()));
            }

            return JudgementTestItem(ti);
        }
       
        //210312 WJS PORS FL 은 서미스터가 3개
        public bool ThermistorCheck3(TestItem ti)
        {
            isProcessingUI(ti);
            ti.refValues_.Clear();
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }

            var rs14 = 0.0;
            var adc = CMClist[11];
            cmc.CalculateToMC33772ModuleTemp(adc, out rs14);
            LogState(LogType.Info, "Thermistor Check 3 - ADC:" + adc.ToString());
            ti.Value_ = tempTher3 = rs14;

            if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_TEMP3______ADC, rs14.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_TEMP3______ADC, rs14.ToString()));
            }
            //221101 wjs add mase m183
            //       일단 넣긴 넣는데 IF 상 미 사용이라 eollist 에서 미 사용인가... 차후 확인 wjs 체크필요
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_TEMP3______ADC, rs14.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_TEMP3______ADC, rs14.ToString()));
            }
            return JudgementTestItem(ti);
        }

        public bool ThermistorDeviation(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            //220314 Porsche FL 사용안함 
            ////210312 WJS ADD PORS FL
            //if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            //{
            //    List<double> Temps = new List<double>();
            //    Temps.Add(tempTher1);
            //    Temps.Add(tempTher2);
            //    Temps.Add(tempTher3);
            //    ti.Value_ = Temps.Max() - Temps.Min();
            //    LogState(LogType.Info, string.Format("Thermistor 1 : {0}, Thermistor 2 : {1}, Thermistor 3 : {2} ", Temps[0], Temps[1], Temps[2]));
            //    LogState(LogType.Info, string.Format("Thermistor Max : {0}, Thermistor Min : {1} ", Temps.Max(), Temps.Min()));
            //    this.LogState(LogType.Info, string.Format("{0}(Thermistor Max) - {1}(Thermistor Min) = {2}", Temps.Max(), Temps.Min(), ti.Value_.ToString()));
            //}
            //else
            {
                //List<double> Temps = new List<double>();
                //Temps.Add(tempTher1);
                //Temps.Add(tempTher2);
                //Temps.Max() - Temps.Min();

                ti.Value_ = tempTher1 - tempTher2;
                this.LogState(LogType.Info, string.Format("{0}(Thermistor 1) - {1}(Thermistor 2) = {2}", tempTher1, tempTher2, ti.Value_.ToString()));
            }
            return JudgementTestItem(ti);
        }



        public bool DCIR_TempNewCheck2(TestItem ti)
        {
            isProcessingUI(ti);

            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            tempTher1 = 0.0;
            tempTher2 = 0.0;
            tempTher3 = 0.0;
            tempRes1 = 0.0;
            tempRes2 = 0.0;

            ti.refValues_.Clear();

            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR ||
                localTypes == ModelType.MASERATI_NORMAL ||
                localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR
                || localTypes == ModelType.MASERATI_M183_NORMAL ////221101 wjs add mase m183
                )
            {
                //210709 : 포르쉐 노멀, 포르쉐 미러, 포르쉐 페이스리프트 노멀, 포르쉐 페이스리프트 미러, 마세라티 노멀 에서 사용 되도록 구분 되어야 한다
                //Thermistor3 함수는 있는데 실제로 사용은 1,2만 하는듯 (어쨌든 팩1동과 동일하게 가져 가야 한다고 한다)

                //1. Get Thermistor1
                var rs12 = 0.0;
                var adc1 = CMClist[12];
                cmc.CalculateToMC33772ModuleTemp(adc1, out rs12);
                LogState(LogType.Info, "Thermistor Check 1 - ADC:" + adc1.ToString());
                tempTher1 = rs12;
                Thread.Sleep(1);

                //2. Get Thermistor2
                var rs13 = 0.0;
                var adc2 = CMClist[13];
                cmc.CalculateToMC33772ModuleTemp(adc2, out rs13);
                LogState(LogType.Info, "Thermistor Check 2 - ADC:" + adc2.ToString());
                tempTher2 = rs13;

                //3. Calc Thermistor1&2, Dev
                //List<double> dList = new List<double>();
                //dList.Add(rs12);
                //dList.Add(rs13);
                //double dMax = dList.Max();
                //double dMin = dList.Min();
                //double dDev = dMax - dMin;
                //ti.Value_ = dDev;
                //LogState(LogType.Info, "MeasureMax:" + dMax.ToString() + "  MeasureMin:" + dMin.ToString() + "  DEV:" + dDev.ToString());

                ti.Value_ = tempTher1 - tempTher2;
                this.LogState(LogType.Info, string.Format("{0}(Thermistor 1) - {1}(Thermistor 2) = {2}", tempTher1, tempTher2, ti.Value_.ToString()));


                string strRefData = string.Empty;
                strRefData = string.Format("{0}&{1}", tempTher1, tempTher2);

                if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_TEMP_NEW_CHECK, strRefData.ToString()));
                }
                else if (localTypes == ModelType.MASERATI_NORMAL)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_TEMP_NEW_CHECK, strRefData.ToString()));
                }
                else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_TEMP_NEW_CHECK, strRefData.ToString()));
                }
                //221101 wjs add mase m183
                else if (localTypes == ModelType.MASERATI_M183_NORMAL)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_TEMP_NEW_CHECK, strRefData.ToString()));
                }
            }
            else if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR ||
                     localTypes == ModelType.E_UP)
            {
                //1. Get Thermistor1&2
                var dmmChInfo = new DMMChannelInfo(localTypes);

                var resList = new List<double>();

                if (!keysight.MeasRes_Multi(out resList, dmmChInfo.ModuleRes, 2))
                {
                    ti.Value_ = _DEVICE_NOT_READY;
                    return JudgementTestItem(ti);
                }

                if (resList[0] > 9.9E+30)
                {
                    resList[0] = 99999;
                }
                if (resList[1] > 9.9E+30)
                {
                    resList[1] = 99999;
                }

                tempRes1 = resList[0] * 0.001;

                tempRes2 = resList[1] * 0.001;

                //이업, 아우디는 온도 환산은 temp dev할때만 가져다 쓰고 저항값으로 Judge, 상세보고함

                //근데 ThermistorDeviation() 같이 편차구할때는 온도값으로 환산해서 계산 되어야 한다고함, 때문에 미리 구해놓음

                tempTher1 = GetTempToResist_Audi(tempRes1);

                tempTher2 = GetTempToResist_Audi(tempRes2);

                LogState(LogType.Info, string.Format("rs 1 : Resistance:{0} / Temp:{1}", tempRes1, tempTher1));

                LogState(LogType.Info, string.Format("rs 2 : Resistance:{0} / Temp:{1}", tempRes2, tempTher2));

                //2. Calc Thermistor1&2, Dev
                //List<double> dList = new List<double>();
                //dList.Add(dTemp1);
                //dList.Add(dTemp2);
                //double dMax = dList.Max();
                //double dMin = dList.Min();
                //double dDev = dMax - dMin;
                //ti.Value_ = dDev;
                //LogState(LogType.Info, "MeasureMax:" + dMax.ToString() + "  MeasureMin:" + dMin.ToString() + "  DEV:" + dDev.ToString());  
                ti.Value_ = tempTher1 - tempTher2;

                this.LogState(LogType.Info, string.Format("{0}(Thermistor 1) - {1}(Thermistor 2) = {2}", tempTher1, tempTher2, ti.Value_.ToString()));

                string strRefData = string.Empty;
                strRefData = string.Format("{0}&{1}", tempTher1, tempTher2);

                if (localTypes == ModelType.AUDI_NORMAL || localTypes == ModelType.AUDI_MIRROR)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_AUD_TEMP_NEW_CHECK2, strRefData.ToString()));
                }
                else if (localTypes == ModelType.E_UP)
                {
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_E_UP_TEMP_NEW_CHECK2, strRefData.ToString()));
                }
            }

            return JudgementTestItem(ti);
        }
        
        public bool PCBTemp(TestItem ti)
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
            var rs14 = 0.0;
            var adc = CMClist[14];
            cmc.CalculateToMC33772PCB(adc, out rs14);
            LogState(LogType.Info, "PCB TEMP Check - ADC:" + adc.ToString());
            ti.Value_ = rs14;

            //201217 wjs add maserati
            if (localTypes == ModelType.PORSCHE_NORMAL || localTypes == ModelType.PORSCHE_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_PCB___TEMP_ADC, rs14.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_PCB___TEMP_ADC, rs14.ToString()));
            }
            else if (localTypes == ModelType.MASERATI_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_NOR_PCB___TEMP_ADC, rs14.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_NOR_PCB___TEMP_ADC, rs14.ToString()));
            }
            //210312 WJS ADD PORS FL
            else if (localTypes == ModelType.PORSCHE_FACELIFT_NORMAL || localTypes == ModelType.PORSCHE_FACELIFT_MIRROR)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_F_L_PCB___TEMP_ADC, rs14.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_F_L_PCB___TEMP_ADC, rs14.ToString()));
            }
            //221101 wjs add mase m183
            else if (localTypes == ModelType.MASERATI_M183_NORMAL)
            {
                if (pro_Type == ProgramType.EOLInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_EOL_MAS_M183_PCB___TEMP_ADC, rs14.ToString()));
                else if (pro_Type == ProgramType.VoltageInspector)
                    ti.refValues_.Add(MakeDetailItem(KEY_VOL_MAS_M183_PCB___TEMP_ADC, rs14.ToString()));
            }
            //if (pro_Type == ProgramType.EOLInspector)
            //    ti.refValues_.Add(MakeDetailItem(KEY_EOL_POR_NOR_PCB___TEMP_ADC, rs14.ToString()));
            //else if (pro_Type == ProgramType.VoltageInspector)
            //    ti.refValues_.Add(MakeDetailItem(KEY_VOL_POR_NOR_PCB___TEMP_ADC, rs14.ToString()));

            return JudgementTestItem(ti);
        }

        public bool AmbientTemperature(TestItem ti)
        {
            isProcessingUI(ti);
            if (isDummy)
            {
                ti.Value_ = 0;
                ti.Result = "PASS";
                this.LogState(LogType.NG, "Test :" + ti.Name + " End - DUMMY (VALUE IS 0)");
                return true;
            }
            ti.Value_ = ambientTemp;

            return JudgementTestItem(ti);
        }
    }
}
