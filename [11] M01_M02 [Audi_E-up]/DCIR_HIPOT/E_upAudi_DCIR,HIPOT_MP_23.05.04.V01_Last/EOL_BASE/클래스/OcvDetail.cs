using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE.클래스
{
    public class OcvDetail : INotifyPropertyChanged
    {
        public OcvDetail()
        {
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string testName;

        public string TestName
        {
            get { return testName; }
            set { testName = value; OnPropertyChange("TestName"); }
        }

        double cellVolt_1;

        public double CellVolt_1
        {
            get { return cellVolt_1; }
            set { cellVolt_1 = value; OnPropertyChange("CellVolt_1"); }
        }
        double cellVolt_2;

        public double CellVolt_2
        {
            get { return cellVolt_2; }
            set { cellVolt_2 = value; OnPropertyChange("CellVolt_2"); }
        }
        double cellVolt_3;

        public double CellVolt_3
        {
            get { return cellVolt_3; }
            set { cellVolt_3 = value; OnPropertyChange("CellVolt_3"); }
        }
        double cellVolt_4;

        public double CellVolt_4
        {
            get { return cellVolt_4; }
            set { cellVolt_4 = value; OnPropertyChange("CellVolt_4"); }
        }
        double cellVolt_5;

        public double CellVolt_5
        {
            get { return cellVolt_5; }
            set { cellVolt_5 = value; OnPropertyChange("CellVolt_5"); }
        }
        double cellVolt_6;

        public double CellVolt_6
        {
            get { return cellVolt_6; }
            set { cellVolt_6 = value; OnPropertyChange("CellVolt_6"); }
        }
        double cellVolt_7;

        public double CellVolt_7
        {
            get { return cellVolt_7; }
            set { cellVolt_7 = value; OnPropertyChange("CellVolt_7"); }
        }
        double cellVolt_8;

        public double CellVolt_8
        {
            get { return cellVolt_8; }
            set { cellVolt_8 = value; OnPropertyChange("CellVolt_8"); }
        }
    }
}
