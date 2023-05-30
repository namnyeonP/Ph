using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renault_BT6.Module
{
    public class DCIR_Process
    {
        public double ModuleTemp { get; set; }  // 충방 시작 외부 온도
                                                // 주의 : 충방 시작 전 외부 온도 값으로만 사용할 것

        public List<double> CellDcirList { get; set; }

        public DCIR_Process()
        {
           CellDcirList = new List<double>();

            Init();
        }

        public void Init()
        {
            try
            {
                ModuleTemp = 0;

                CellDcirList.Clear();
            }
            catch
            {

            }
        }

        /// <summary>
        /// BT6 온도 계산 식
        /// </summary>
        /// <param name="moduleRes"></param>
        /// <returns></returns>
        public double GetModuleTempCal(double moduleRes)
        {
            double temp =0;

            try
            {
                // 3435/ (3435/301.2+ln(R/10) )-276.2
                temp = 3435 / ( 3435 / 301.2 + Math.Log((moduleRes * 0.001) / 10.0))  - 276.2;
                //temp = 1.0 / ((1.0 / 301.2) + (1.0 / 3435) * Math.Log((moduleRes * 0.001) / 10.0)) - 276.2;
            }
            catch
            {
                temp = 0;
            }

            return temp;
        }


        /// <summary>
        /// Cell, Module DCIR 연산
        /// </summary>
        /// <param name="cellVolt1"></param>
        /// <param name="cellVolt2"></param>
        /// <param name="DisCurrent"></param>
        /// <returns></returns>
        public double GetOriginDCIR(double cellVolt1, double cellVolt2, double DisCurrent)
        {
            double oriDCIR = 0;

            try
            {
                oriDCIR = Math.Abs( ((cellVolt1 - cellVolt2) / DisCurrent) * 1000); //밀리옴
            }
            catch
            {
                oriDCIR = 0;
            }

            return oriDCIR;
        }

        public double GetDCIR_Cal(double originDCIR, double cellFomula1, double cellFomula2, double cellFomula3, double moduleTemp, out string log)
        {
            double cellDCIR = 0;
            log = "";
            int val = 25;

            try
            {
                // 기존
                //cellDCIR = originDCIR + (cellFomula1 * (moduleTemp - 23)) + cellFomula2;

                // 변경 20191227 
                // Cell DCIR = DCIR_Cell@T + 0.155*(T－25) – 0.0022*(T²-25²) [℃]
                cellDCIR = originDCIR + cellFomula1 * (moduleTemp - val) - cellFomula3 * (Math.Pow(ModuleTemp, 2) - Math.Pow(25, 2)) + cellFomula2;

                log = string.Format("{0} = {1} + {2} * ({3} - {4})  - {5} * ({6}^2  - 25^2) + {7}", 
                                            cellDCIR, 
                                            originDCIR, 
                                            cellFomula1, 
                                            moduleTemp, 
                                            val, 
                                            cellFomula3, 
                                            moduleTemp,
                                            cellFomula2);
            }
            catch
            {
                cellDCIR = 0;
            }

            return cellDCIR;
        }

        public double GetModuleDCIR_Cal(double originDCIR, 
                                                    double cellFomula1, 
                                                    double cellFomula2,
                                                    double cellFomula3,
                                                    double moduleTemp,
                                                    out string log)
        {
            double cellDCIR = 0;
            log = "";
            int val = 25;

            try
            {
                // 기존
                //cellDCIR = originDCIR + (cellFomula1 * (moduleTemp - 23)) + cellFomula2;

                // 변경 20191227 
                // Module DCIR = = DCIR_Module@T + ((0.155x12)*(T－25)) – ((0.0022x12)*(T²-25²)) [℃]  
                //cellDCIR = originDCIR + ((cellFomula1*12) * (moduleTemp - val)) - ((cellFomula2*12) * (Math.Pow(ModuleTemp, 2) - Math.Pow(25, 2)) );

                // 200103 KSM
                cellDCIR = originDCIR + ((cellFomula1) * (moduleTemp - val)) - ((cellFomula3) * (Math.Pow(ModuleTemp, 2) - Math.Pow(25, 2)) ) + cellFomula2;

                log = string.Format("{0} = {1} + ({2}) * ({3} - {4})  - ({5}) * ({6}^2  - {7}^2) + {8}",
                                            cellDCIR,
                                            originDCIR,
                                            cellFomula1,
                                            moduleTemp,
                                            val,
                                            cellFomula3,
                                            moduleTemp,
                                            val,
                                            cellFomula2);
            }
            catch
            {
                cellDCIR = 0;
            }

            return cellDCIR;
        }


        public double GetCellDeviation()
        {
            double deviation = 0;

            try
            {
                if (CellDcirList.Count > 0)
                {
                    deviation = CellDcirList.Max() - CellDcirList.Min();
                }
            }
            catch
            {
                deviation = 0;
            }
            return deviation;
        }

        public double GetCellRatio()
        {
            double ratio = 0;

            try
            {
                if (CellDcirList.Count > 0)
                {
                    ratio = (CellDcirList.Max() / CellDcirList.Min()) * 100;
                }
            }
            catch
            {
                ratio = 0;
            }
            return ratio;
        }


    } // end
}
