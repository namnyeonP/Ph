using Renault_BT6.모듈;
using Renault_BT6.윈도우;
using Renault_BT6.클래스;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Renault_BT6
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



    }
}