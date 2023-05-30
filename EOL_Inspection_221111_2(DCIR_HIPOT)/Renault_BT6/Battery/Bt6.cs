using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renault_BT6.Battery
{
    public class Bt6
    {
        public static bool SchedulerModeChange = false;


        private List<double> cellList = new List<double>();
        public List<double> CellVoltList
        {
            get { return cellList; }
            set { cellList = value; }
        }
        public double ModuleVolt { get; set; }

        public Bt6()
        {

        }

    } // class end
}
