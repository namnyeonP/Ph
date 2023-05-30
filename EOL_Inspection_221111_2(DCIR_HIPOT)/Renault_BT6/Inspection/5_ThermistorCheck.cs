using Renault_BT6.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Renault_BT6
{
    public partial class MainWindow
    {
        double thermistor1 = 0.0;
        double thermistor2 = 0.0;
        const int _SendCount = 5;

        public bool Module_Thermistor(TestItem ti)
        {
            isProcessingUI(ti);

            double moduleRes = 0;
            //double moduleRes = Device.KeysightDAQ.GetMeasRes(BatteryInfo.ModuleResCH, 5);

            try
            {
                if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    if (Device.KeysightDAQ.isAlive == false || Device.Tempr.GetTemp == -255 )
                    {
                        ti.Value_ = "DEVICE_NOT_READY";
                        return JudgementTestItem(ti);
                    }
                    moduleRes = double.Parse(Device.KeysightDAQ.MeasRes(BatteryInfo.ModuleResCH));
                   // moduleRes = Device.KeysightDAQ.GetMeasRes(BatteryInfo.ModuleResCH, 5);
                }
                else
                {
                    //if (Device.KeysightDAQ.isAlive == false || Device.Tempr.GetTemp == -255 )
                    if (Device.KeysightDAQ.isAlive == false || Device.TemprCT100.GetTemprature() == -255)
                    {
                        ti.Value_ = "DEVICE_NOT_READY";
                        return JudgementTestItem(ti);
                    }             
                }

                if (IsManual == true)
                {
                    moduleRes = double.Parse(Device.KeysightDAQ.MeasRes(BatteryInfo.ModuleResCH));
                }
                else
                {
                    moduleRes = mSecondPreCheckThermistorRes;
                }

                LogState(LogType.Info, 
                            string.Format("Res Temp,3435 / ( 3435 / 301.2 + Math.Log(({0} * 0.001) / 10.0))  - 276.2",
                            moduleRes));

                double moduleTemp = _DCIR.GetModuleTempCal(moduleRes);
                double atempX = 0.0;

                if (CONFIG.EolInspType == InspectionType.HIPOT)
                {
                    atempX = Device.Tempr.GetTemp;
                }
                else
                {
                    atempX = Device.TemprCT100.GetTemprature();
                }

                double tempoffset = 0.0d;
                Double.TryParse(Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\EOL_Trigger").GetValue("TEMPOFFSET").ToString()
                                                                                                      , out tempoffset);
                double atemp = atempX - tempoffset; // 220606 ks yoo 적외선 온도계에 방사율 제어기능 없어서 offset적용함.
                ti.Value_ = atemp - moduleTemp;

                // control limit
                SetContorlLimitData(0, ti.Value_.ToString());

                LogState(LogType.Info, "Ambient Temp, " + atemp);
                LogState(LogType.Info, "Module Temp, " + moduleTemp);
                LogState(LogType.Info, string.Format("{0} = {1} - {2}", ti.Value_, atemp, moduleTemp) );

                // MES 상세 수집
                // 21.08.29 : New Module Temp Check2 상세 수집 ID는 1개 배정 받음 (EOL.MesID.TempCheck2)
                // 함수 동작은 동일하나 상세 수집 형식이 다름으로 그의 따른 분기 추가

                if (ti.CLCTITEM == "WP2MMMTQ2001")
                {
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.AmbientTemp, atemp.ToString("N3")));
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.ModuleTemp, moduleTemp.ToString()));
                }

                if (ti.CLCTITEM == "WP2MMMTQ2061")
                {
                    string strRefData;
                    strRefData = string.Format("{0}&{1}", atemp.ToString("N3"), moduleTemp);
                    ti.refValues_.Add(MakeDetailItem(EOL.MesID.TempCheck2, strRefData.ToString()));
                }
            }
            catch
            {

            }

            return JudgementTestItem(ti);
        }

    
       
    }
}