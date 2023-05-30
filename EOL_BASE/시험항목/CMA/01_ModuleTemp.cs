using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EOL_BASE
{
    public partial class MainWindow
    {
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
            return decT;
        }

        public double GetTempToResist(double resist)
        {
            double r_25 = 10;
            double r_t = resist;

            double R = r_t / r_25;
            double a = Math.Log(R) / 3435.0;
            double b = 1.0/298.0;
            double c = 1.0/(a+b);

            LogState(LogType.Info,
@"double r_25 = 10;
double r_t = resist;
double R = r_t / r_25;
double a = Math.Log(R) / 3435.0;
double b = 1.0/298.0;
double c = 1.0/(a+b);
return c-273.0;");
            return c-273.0;

            //return (1 / () + (1 / 298))) - 273;
            //var t = 1 / ((a + (b * Math.Log(R)) + (c * Math.Pow(Math.Log(R), 2)) + (d * Math.Pow(Math.Log(R), 3))));
            //var decT = t - 273.15;
            //return decT;
        }

        public bool TherResist1(TestItem ti)
        {
            isProcessingUI(ti);

            var atemp = temps.tempStr;

            if (keysight == null)
            {
                ti.Value_ = "NotConnected";
                return JudgementTestItem(ti);
            }

            keysight.rtstring = "";
            tempCnt = 1;
            keysight.MeasTemp("118");
            

            int rec = keysight.sp.BytesToRead;

            int cnt = 0;
            while (rec < 17)//33
            {
                Thread.Sleep(100);
                rec = keysight.sp.BytesToRead;
                cnt+=100;
                if (cnt == 2000)
                {
                    keysight.MeasTemp("118");                

                    rec = keysight.sp.BytesToRead;

                    cnt = 0;
                    while (rec < 17)//33
                    {
                        Thread.Sleep(100);
                        rec = keysight.sp.BytesToRead;
                        cnt += 100;
                        if (cnt == 2000)
                        {
                            ti.Value_ = _DEVICE_NOT_READY;
                            return JudgementTestItem(ti);
                        }
                    }
                    break;
                }
            }
            //받은 후에 데이터 파싱
            byte[] bt = new byte[rec];
            keysight.sp.Read(bt, 0, rec);

            keysight.rtstring = Encoding.Default.GetString(bt, 0, rec);
            LogState(LogType.RESPONSE, "KeysightDMM:" + keysight.rtstring);

            var vArr = keysight.rtstring.Replace("\r\n", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            tempList = new List<double>();
            foreach (var item in vArr)
            {
                double dv = 0;
                if (double.TryParse(item, out dv))
                {
                    tempList.Add(dv*0.001);
                }
            }

            var tempTher1 = 0.0;
            if (tempList != null && tempList.Count == tempCnt)
            {
                tempTher1 = tempList[0];
            }

            //LogState(LogType.Info, "Room Temp :" + atemp.ToString());
            //ti.refValues_.Clear();
            //ti.refValues_.Add(MakeDetailItem("TEST0001", atemp.ToString()));
            //LogState(LogType.Info, "Resistance :" + tempTher1.ToString());
            //temptoResist1 = GetTempToResist(tempTher1);
            //LogState(LogType.Info, "Resistance to Temperature :" + temptoResist1.ToString());
            //ti.refValues_.Add(MakeDetailItem("TEST0002", temptoResist1.ToString()));

            //ti.Value_ = temptoResist1 - atemp;

            ti.Value_ = tempTher1;

            return JudgementTestItem(ti);
        }

        public bool TherResist2(TestItem ti)
        {
            isProcessingUI(ti);

            var tempTher1 = 0.0;
            if (tempList != null && tempList.Count == tempCnt)
            {
                tempTher1 = tempList[1];
            }
            ti.Value_ = tempTher1;

            return JudgementTestItem(ti);
        }

    }
}