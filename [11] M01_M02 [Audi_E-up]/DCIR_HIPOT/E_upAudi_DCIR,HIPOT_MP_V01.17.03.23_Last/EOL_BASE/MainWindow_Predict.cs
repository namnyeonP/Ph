using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace EOL_BASE
{
    public class Predict : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
    
        public int ID;
        #region cell_dev
        private double cell_01_dev = 0.0;
        private double cell_02_dev = 0.0;
        private double cell_03_dev = 0.0;
        private double cell_04_dev = 0.0;
        private double cell_05_dev = 0.0;
        private double cell_06_dev = 0.0;
        private double cell_07_dev = 0.0;
        private double cell_08_dev = 0.0;
        private double cell_09_dev = 0.0;
        private double cell_10_dev = 0.0;
        private double cell_11_dev = 0.0;
        private double cell_12_dev = 0.0;
        private double cell_13_dev = 0.0;
        private double cell_14_dev = 0.0;
        private double cell_15_dev = 0.0;
        private double cell_16_dev = 0.0;
        private double cell_17_dev = 0.0;
        private double cell_18_dev = 0.0;
        private double cell_19_dev = 0.0;
        private double cell_20_dev = 0.0;
        private double cell_21_dev = 0.0;
        private double cell_22_dev = 0.0;
        private double cell_23_dev = 0.0;
        private double cell_24_dev = 0.0;
        private double cell_25_dev = 0.0;
        private double cell_26_dev = 0.0;
        private double cell_27_dev = 0.0;
        private double cell_28_dev = 0.0;
        private double cell_29_dev = 0.0;
        private double cell_30_dev = 0.0;
        private double cell_31_dev = 0.0;
        private double cell_32_dev = 0.0;
        private double cell_33_dev = 0.0;
        private double cell_34_dev = 0.0;
        private double cell_35_dev = 0.0;
        private double cell_36_dev = 0.0;
        private double cell_37_dev = 0.0;
        private double cell_38_dev = 0.0;
        private double cell_39_dev = 0.0;
        private double cell_40_dev = 0.0;
        public double Cell_01_dev { get { return cell_01_dev; } set { if (cell_01_dev != value) { cell_01_dev = value; OnPropertyChange("Cell_01_dev"); } } }
        public double Cell_02_dev { get { return cell_02_dev; } set { if (cell_02_dev != value) { cell_02_dev = value; OnPropertyChange("Cell_02_dev"); } } }
        public double Cell_03_dev { get { return cell_03_dev; } set { if (cell_03_dev != value) { cell_03_dev = value; OnPropertyChange("Cell_03_dev"); } } }
        public double Cell_04_dev { get { return cell_04_dev; } set { if (cell_04_dev != value) { cell_04_dev = value; OnPropertyChange("Cell_04_dev"); } } }
        public double Cell_05_dev { get { return cell_05_dev; } set { if (cell_05_dev != value) { cell_05_dev = value; OnPropertyChange("Cell_05_dev"); } } }
        public double Cell_06_dev { get { return cell_06_dev; } set { if (cell_06_dev != value) { cell_06_dev = value; OnPropertyChange("Cell_06_dev"); } } }
        public double Cell_07_dev { get { return cell_07_dev; } set { if (cell_07_dev != value) { cell_07_dev = value; OnPropertyChange("Cell_07_dev"); } } }
        public double Cell_08_dev { get { return cell_08_dev; } set { if (cell_08_dev != value) { cell_08_dev = value; OnPropertyChange("Cell_08_dev"); } } }
        public double Cell_09_dev { get { return cell_09_dev; } set { if (cell_09_dev != value) { cell_09_dev = value; OnPropertyChange("Cell_09_dev"); } } }
        public double Cell_10_dev { get { return cell_10_dev; } set { if (cell_10_dev != value) { cell_10_dev = value; OnPropertyChange("Cell_10_dev"); } } }
        public double Cell_11_dev { get { return cell_11_dev; } set { if (cell_11_dev != value) { cell_11_dev = value; OnPropertyChange("Cell_11_dev"); } } }
        public double Cell_12_dev { get { return cell_12_dev; } set { if (cell_12_dev != value) { cell_12_dev = value; OnPropertyChange("Cell_12_dev"); } } }
        public double Cell_13_dev { get { return cell_13_dev; } set { if (cell_13_dev != value) { cell_13_dev = value; OnPropertyChange("Cell_13_dev"); } } }
        public double Cell_14_dev { get { return cell_14_dev; } set { if (cell_14_dev != value) { cell_14_dev = value; OnPropertyChange("Cell_14_dev"); } } }
        public double Cell_15_dev { get { return cell_15_dev; } set { if (cell_15_dev != value) { cell_15_dev = value; OnPropertyChange("Cell_15_dev"); } } }
        public double Cell_16_dev { get { return cell_16_dev; } set { if (cell_16_dev != value) { cell_16_dev = value; OnPropertyChange("Cell_16_dev"); } } }
        public double Cell_17_dev { get { return cell_17_dev; } set { if (cell_17_dev != value) { cell_17_dev = value; OnPropertyChange("Cell_17_dev"); } } }
        public double Cell_18_dev { get { return cell_18_dev; } set { if (cell_18_dev != value) { cell_18_dev = value; OnPropertyChange("Cell_18_dev"); } } }
        public double Cell_19_dev { get { return cell_19_dev; } set { if (cell_19_dev != value) { cell_19_dev = value; OnPropertyChange("Cell_19_dev"); } } }
        public double Cell_20_dev { get { return cell_20_dev; } set { if (cell_20_dev != value) { cell_20_dev = value; OnPropertyChange("Cell_20_dev"); } } }
        public double Cell_21_dev { get { return cell_21_dev; } set { if (cell_21_dev != value) { cell_21_dev = value; OnPropertyChange("Cell_21_dev"); } } }
        public double Cell_22_dev { get { return cell_22_dev; } set { if (cell_22_dev != value) { cell_22_dev = value; OnPropertyChange("Cell_22_dev"); } } }
        public double Cell_23_dev { get { return cell_23_dev; } set { if (cell_23_dev != value) { cell_23_dev = value; OnPropertyChange("Cell_23_dev"); } } }
        public double Cell_24_dev { get { return cell_24_dev; } set { if (cell_24_dev != value) { cell_24_dev = value; OnPropertyChange("Cell_24_dev"); } } }
        public double Cell_25_dev { get { return cell_25_dev; } set { if (cell_25_dev != value) { cell_25_dev = value; OnPropertyChange("Cell_25_dev"); } } }
        public double Cell_26_dev { get { return cell_26_dev; } set { if (cell_26_dev != value) { cell_26_dev = value; OnPropertyChange("Cell_26_dev"); } } }
        public double Cell_27_dev { get { return cell_27_dev; } set { if (cell_27_dev != value) { cell_27_dev = value; OnPropertyChange("Cell_27_dev"); } } }
        public double Cell_28_dev { get { return cell_28_dev; } set { if (cell_28_dev != value) { cell_28_dev = value; OnPropertyChange("Cell_28_dev"); } } }
        public double Cell_29_dev { get { return cell_29_dev; } set { if (cell_29_dev != value) { cell_29_dev = value; OnPropertyChange("Cell_29_dev"); } } }
        public double Cell_30_dev { get { return cell_30_dev; } set { if (cell_30_dev != value) { cell_30_dev = value; OnPropertyChange("Cell_30_dev"); } } }
        public double Cell_31_dev { get { return cell_31_dev; } set { if (cell_31_dev != value) { cell_31_dev = value; OnPropertyChange("Cell_31_dev"); } } }
        public double Cell_32_dev { get { return cell_32_dev; } set { if (cell_32_dev != value) { cell_32_dev = value; OnPropertyChange("Cell_32_dev"); } } }
        public double Cell_33_dev { get { return cell_33_dev; } set { if (cell_33_dev != value) { cell_33_dev = value; OnPropertyChange("Cell_33_dev"); } } }
        public double Cell_34_dev { get { return cell_34_dev; } set { if (cell_34_dev != value) { cell_34_dev = value; OnPropertyChange("Cell_34_dev"); } } }
        public double Cell_35_dev { get { return cell_35_dev; } set { if (cell_35_dev != value) { cell_35_dev = value; OnPropertyChange("Cell_35_dev"); } } }
        public double Cell_36_dev { get { return cell_36_dev; } set { if (cell_36_dev != value) { cell_36_dev = value; OnPropertyChange("Cell_36_dev"); } } }
        public double Cell_37_dev { get { return cell_37_dev; } set { if (cell_37_dev != value) { cell_37_dev = value; OnPropertyChange("Cell_37_dev"); } } }
        public double Cell_38_dev { get { return cell_38_dev; } set { if (cell_38_dev != value) { cell_38_dev = value; OnPropertyChange("Cell_38_dev"); } } }
        public double Cell_39_dev { get { return cell_39_dev; } set { if (cell_39_dev != value) { cell_39_dev = value; OnPropertyChange("Cell_39_dev"); } } }
        public double Cell_40_dev { get { return cell_40_dev; } set { if (cell_40_dev != value) { cell_40_dev = value; OnPropertyChange("Cell_40_dev"); } } }
        #endregion
        private double module_dev = 0.0;
        public double Module_dev { get { return module_dev; } set { if (module_dev != value) { module_dev = value; OnPropertyChange("Module_dev"); } } }
        #region cell_34970
        public double Cell_01_34970= 0.0;
        public double Cell_02_34970= 0.0;
        public double Cell_03_34970= 0.0;
        public double Cell_04_34970= 0.0;
        public double Cell_05_34970= 0.0;
        public double Cell_06_34970= 0.0;
        public double Cell_07_34970= 0.0;
        public double Cell_08_34970= 0.0;
        public double Cell_09_34970= 0.0;
        public double Cell_10_34970= 0.0;
        public double Cell_11_34970= 0.0;
        public double Cell_12_34970= 0.0;
        public double Cell_13_34970= 0.0;
        public double Cell_14_34970= 0.0;
        public double Cell_15_34970= 0.0;
        public double Cell_16_34970= 0.0;
        public double Cell_17_34970= 0.0;
        public double Cell_18_34970= 0.0;
        public double Cell_19_34970= 0.0;
        public double Cell_20_34970= 0.0;
        public double Cell_21_34970= 0.0;
        public double Cell_22_34970= 0.0;
        public double Cell_23_34970= 0.0;
        public double Cell_24_34970= 0.0;
        public double Cell_25_34970= 0.0;
        public double Cell_26_34970= 0.0;
        public double Cell_27_34970= 0.0;
        public double Cell_28_34970= 0.0;
        public double Cell_29_34970= 0.0;
        public double Cell_30_34970= 0.0;
        public double Cell_31_34970= 0.0;
        public double Cell_32_34970= 0.0;
        public double Cell_33_34970= 0.0;
        public double Cell_34_34970= 0.0;
        public double Cell_35_34970= 0.0;
        public double Cell_36_34970= 0.0;
        public double Cell_37_34970= 0.0;
        public double Cell_38_34970= 0.0;
        public double Cell_39_34970= 0.0;
        public double Cell_40_34970 = 0.0;
        #endregion
        public double Module_34970 = 0.0;
        #region cell_daq
        public double Cell_01_daq= 0.0;
        public double Cell_02_daq= 0.0;
        public double Cell_03_daq= 0.0;
        public double Cell_04_daq= 0.0;
        public double Cell_05_daq= 0.0;
        public double Cell_06_daq= 0.0;
        public double Cell_07_daq= 0.0;
        public double Cell_08_daq= 0.0;
        public double Cell_09_daq= 0.0;
        public double Cell_10_daq= 0.0;
        public double Cell_11_daq= 0.0;
        public double Cell_12_daq= 0.0;
        public double Cell_13_daq= 0.0;
        public double Cell_14_daq= 0.0;
        public double Cell_15_daq= 0.0;
        public double Cell_16_daq= 0.0;
        public double Cell_17_daq= 0.0;
        public double Cell_18_daq= 0.0;
        public double Cell_19_daq= 0.0;
        public double Cell_20_daq= 0.0;
        public double Cell_21_daq= 0.0;
        public double Cell_22_daq= 0.0;
        public double Cell_23_daq= 0.0;
        public double Cell_24_daq= 0.0;
        public double Cell_25_daq= 0.0;
        public double Cell_26_daq= 0.0;
        public double Cell_27_daq= 0.0;
        public double Cell_28_daq= 0.0;
        public double Cell_29_daq= 0.0;
        public double Cell_30_daq= 0.0;
        public double Cell_31_daq= 0.0;
        public double Cell_32_daq= 0.0;
        public double Cell_33_daq= 0.0;
        public double Cell_34_daq= 0.0;
        public double Cell_35_daq= 0.0;
        public double Cell_36_daq= 0.0;
        public double Cell_37_daq= 0.0;
        public double Cell_38_daq= 0.0;
        public double Cell_39_daq= 0.0;
        public double Cell_40_daq = 0.0;
        #endregion
        public double Module_cycler = 0.0;

        private string lotID = "Empty";
        private string stationNum = "#0";
        private string modelName = "Empty";
        private DateTime finishedTime = new DateTime();
        private string isPass = "NO";
        private string isMESSkip = "Empty";
        private string isAlarmed = "Empty";
        private double bef_inclination = 0.0;
        private double now_inclination = 0.0;
        public string ModuleBCR = "Empty";

        public string LotID {
            get { return lotID; }
            set
            {
                if (lotID != value)
                {
                    lotID = value;
                    OnPropertyChange("LotID");
                }
            }
        }
        public DateTime FinishedTime
        {
            get { return finishedTime; }
            set
            {
                if (finishedTime != value)
                {
                    finishedTime = value;
                    OnPropertyChange("FinishedTime");
                }
            }
        }
        public string StationNum
        {
            get { return stationNum; }
            set
            {
                if (stationNum != value)
                {
                    stationNum = value;
                    OnPropertyChange("StationNum");
                }
            }
        }
        public string ModelName
        {
            get { return modelName; }
            set
            {
                if (modelName != value)
                {
                    modelName = value;
                    OnPropertyChange("ModelName");
                }
            }
        }
        public string IsPass
        {
            get { return isPass; }
            set
            {
                if (isPass != value)
                {
                    isPass = value;
                    OnPropertyChange("IsPass");
                }
            }
        }
        public string IsMESSkip
        {
            get { return isMESSkip; }
            set
            {
                if (isMESSkip != value)
                {
                    isMESSkip = value;
                    OnPropertyChange("IsMESSkip");
                }
            }
        }
        public string IsAlarmed
        {
            get { return isAlarmed; }
            set
            {
                if (isAlarmed != value)
                {
                    isAlarmed = value;
                    OnPropertyChange("IsAlarmed");
                }
            }
        }
        public double Bef_inclination
        {
            get { return bef_inclination; }
            set
            {
                if (bef_inclination != value)
                {
                    bef_inclination = value;
                    OnPropertyChange("Bef_inclination");
                }
            }
        }
        public double Now_inclination
        {
            get { return now_inclination; }
            set
            {
                if (now_inclination != value)
                {
                    now_inclination = value;
                    OnPropertyChange("Now_inclination");
                }
            }
        }
    }
    public partial class MainWindow
    {
        public string tableName = "PredictTable";
        public string predictFileName = "Predict_EOL.accdb";
        public bool AddPredictData(Predict pre)
        {
            try
            {
                string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + staticPath_PREDICT + "\\" + predictFileName;
                using (OleDbConnection conn = new OleDbConnection(connStr))
                {
                    #region Append all data
                    StringBuilder sb = new StringBuilder();

                    sb.Append("Insert into ");
                    sb.Append(tableName);
                    sb.Append("(");
                    //sb.Append("ID,");
                    sb.Append("FinishedTime," ); 
                    sb.Append("IsPass," ); 
                    sb.Append("StationNum," ); 
                    sb.Append("ModelName," ); 
                    sb.Append("LotID," ); 
                    sb.Append("ModuleBCR," ); 
                    sb.Append("IsMESSkip," ); 
                    sb.Append("IsAlarmed," );
                    sb.Append("Bef_inclination," ); 
                    sb.Append("Now_inclination," );
                    sb.Append("Cell_01_dev," ); 
                    sb.Append("Cell_02_dev," ); 
                    sb.Append("Cell_03_dev," ); 
                    sb.Append("Cell_04_dev," ); 
                    sb.Append("Cell_05_dev," );
                    sb.Append("Cell_06_dev," ); 
                    sb.Append("Cell_07_dev," ); 
                    sb.Append("Cell_08_dev," ); 
                    sb.Append("Cell_09_dev," ); 
                    sb.Append("Cell_10_dev," );
                    sb.Append("Cell_11_dev," ); 
                    sb.Append("Cell_12_dev," ); 
                    sb.Append("Cell_13_dev," );
                    sb.Append("Cell_14_dev," ); 
                    sb.Append("Cell_15_dev," );
                    sb.Append("Cell_16_dev," ); 
                    sb.Append("Cell_17_dev," ); 
                    sb.Append("Cell_18_dev," ); 
                    sb.Append("Cell_19_dev," ); 
                    sb.Append("Cell_20_dev," );
                    sb.Append("Cell_21_dev," ); 
                    sb.Append("Cell_22_dev," ); 
                    sb.Append("Cell_23_dev," ); 
                    sb.Append("Cell_24_dev," ); 
                    sb.Append("Cell_25_dev," );
                    sb.Append("Cell_26_dev," ); 
                    sb.Append("Cell_27_dev," ); 
                    sb.Append("Cell_28_dev," ); 
                    sb.Append("Cell_29_dev," ); 
                    sb.Append("Cell_30_dev," );
                    sb.Append("Cell_31_dev," ); 
                    sb.Append("Cell_32_dev," ); 
                    sb.Append("Cell_33_dev," ); 
                    sb.Append("Cell_34_dev," ); 
                    sb.Append("Cell_35_dev," );
                    sb.Append("Cell_36_dev," ); 
                    sb.Append("Cell_37_dev," ); 
                    sb.Append("Cell_38_dev," ); 
                    sb.Append("Cell_39_dev," ); 
                    sb.Append("Cell_40_dev," );
                    sb.Append("Module_dev," );
                    sb.Append("Cell_01_34970," ); 
                    sb.Append("Cell_02_34970," ); 
                    sb.Append("Cell_03_34970," ); 
                    sb.Append("Cell_04_34970," ); 
                    sb.Append("Cell_05_34970," );
                    sb.Append("Cell_06_34970," ); 
                    sb.Append("Cell_07_34970," ); 
                    sb.Append("Cell_08_34970," ); 
                    sb.Append("Cell_09_34970," ); 
                    sb.Append("Cell_10_34970," );
                    sb.Append("Cell_11_34970," ); 
                    sb.Append("Cell_12_34970," ); 
                    sb.Append("Cell_13_34970," ); 
                    sb.Append("Cell_14_34970," ); 
                    sb.Append("Cell_15_34970," );
                    sb.Append("Cell_16_34970," ); 
                    sb.Append("Cell_17_34970," ); 
                    sb.Append("Cell_18_34970," ); 
                    sb.Append("Cell_19_34970," ); 
                    sb.Append("Cell_20_34970," );
                    sb.Append("Cell_21_34970," ); 
                    sb.Append("Cell_22_34970," ); 
                    sb.Append("Cell_23_34970," ); 
                    sb.Append("Cell_24_34970," ); 
                    sb.Append("Cell_25_34970," );
                    sb.Append("Cell_26_34970," ); 
                    sb.Append("Cell_27_34970," ); 
                    sb.Append("Cell_28_34970," ); 
                    sb.Append("Cell_29_34970," ); 
                    sb.Append("Cell_30_34970," );
                    sb.Append("Cell_31_34970," ); 
                    sb.Append("Cell_32_34970," ); 
                    sb.Append("Cell_33_34970," ); 
                    sb.Append("Cell_34_34970," ); 
                    sb.Append("Cell_35_34970," );
                    sb.Append("Cell_36_34970," ); 
                    sb.Append("Cell_37_34970," ); 
                    sb.Append("Cell_38_34970," ); 
                    sb.Append("Cell_39_34970," ); 
                    sb.Append("Cell_40_34970," );
                    sb.Append("Module_34970," );
                    sb.Append("Cell_01_daq," ); 
                    sb.Append("Cell_02_daq," ); 
                    sb.Append("Cell_03_daq," ); 
                    sb.Append("Cell_04_daq," ); 
                    sb.Append("Cell_05_daq," );
                    sb.Append("Cell_06_daq," ); 
                    sb.Append("Cell_07_daq," ); 
                    sb.Append("Cell_08_daq," ); 
                    sb.Append("Cell_09_daq," ); 
                    sb.Append("Cell_10_daq," );
                    sb.Append("Cell_11_daq," ); 
                    sb.Append("Cell_12_daq," ); 
                    sb.Append("Cell_13_daq," ); 
                    sb.Append("Cell_14_daq," ); 
                    sb.Append("Cell_15_daq," );
                    sb.Append("Cell_16_daq," ); 
                    sb.Append("Cell_17_daq," ); 
                    sb.Append("Cell_18_daq," ); 
                    sb.Append("Cell_19_daq," ); 
                    sb.Append("Cell_20_daq," );
                    sb.Append("Cell_21_daq," ); 
                    sb.Append("Cell_22_daq," ); 
                    sb.Append("Cell_23_daq," ); 
                    sb.Append("Cell_24_daq," ); 
                    sb.Append("Cell_25_daq," );
                    sb.Append("Cell_26_daq," ); 
                    sb.Append("Cell_27_daq," ); 
                    sb.Append("Cell_28_daq," ); 
                    sb.Append("Cell_29_daq," ); 
                    sb.Append("Cell_30_daq," );
                    sb.Append("Cell_31_daq," ); 
                    sb.Append("Cell_32_daq," ); 
                    sb.Append("Cell_33_daq," ); 
                    sb.Append("Cell_34_daq," ); 
                    sb.Append("Cell_35_daq," );
                    sb.Append("Cell_36_daq," ); 
                    sb.Append("Cell_37_daq," ); 
                    sb.Append("Cell_38_daq," ); 
                    sb.Append("Cell_39_daq," ); 
                    sb.Append("Cell_40_daq," );
                    sb.Append("Module_cycler" );
                    sb.Append(")" );
                    sb.Append("Values (" );
                    //sb.Append("@ID," ); 
                    sb.Append("@FinishedTime," ); 
                    sb.Append("@IsPass," ); 
                    sb.Append("@StationNum," ); 
                    sb.Append("@ModelName," ); 
                    sb.Append("@LotID," ); 
                    sb.Append("@ModuleBCR," ); 
                    sb.Append("@IsMESSkip," ); 
                    sb.Append("@IsAlarmed," );
                    sb.Append("@Bef_inclination," ); 
                    sb.Append("@Now_inclination," );
                    sb.Append("@Cell_01_dev," ); 
                    sb.Append("@Cell_02_dev," ); 
                    sb.Append("@Cell_03_dev," ); 
                    sb.Append("@Cell_04_dev," ); 
                    sb.Append("@Cell_05_dev," );
                    sb.Append("@Cell_06_dev," ); 
                    sb.Append("@Cell_07_dev," ); 
                    sb.Append("@Cell_08_dev," ); 
                    sb.Append("@Cell_09_dev," ); 
                    sb.Append("@Cell_10_dev," );
                    sb.Append("@Cell_11_dev," ); 
                    sb.Append("@Cell_12_dev," ); 
                    sb.Append("@Cell_13_dev," ); 
                    sb.Append("@Cell_14_dev," ); 
                    sb.Append("@Cell_15_dev," );
                    sb.Append("@Cell_16_dev," ); 
                    sb.Append("@Cell_17_dev," ); 
                    sb.Append("@Cell_18_dev," ); 
                    sb.Append("@Cell_19_dev," ); 
                    sb.Append("@Cell_20_dev," );
                    sb.Append("@Cell_21_dev," ); 
                    sb.Append("@Cell_22_dev," ); 
                    sb.Append("@Cell_23_dev," ); 
                    sb.Append("@Cell_24_dev," ); 
                    sb.Append("@Cell_25_dev," );
                    sb.Append("@Cell_26_dev," ); 
                    sb.Append("@Cell_27_dev," ); 
                    sb.Append("@Cell_28_dev," ); 
                    sb.Append("@Cell_29_dev," ); 
                    sb.Append("@Cell_30_dev," );
                    sb.Append("@Cell_31_dev," ); 
                    sb.Append("@Cell_32_dev," ); 
                    sb.Append("@Cell_33_dev," ); 
                    sb.Append("@Cell_34_dev," ); 
                    sb.Append("@Cell_35_dev," );
                    sb.Append("@Cell_36_dev," ); 
                    sb.Append("@Cell_37_dev," ); 
                    sb.Append("@Cell_38_dev," ); 
                    sb.Append("@Cell_39_dev," ); 
                    sb.Append("@Cell_40_dev," );
                    sb.Append("@Module_dev," );
                    sb.Append("@Cell_01_34970," ); 
                    sb.Append("@Cell_02_34970," ); 
                    sb.Append("@Cell_03_34970," ); 
                    sb.Append("@Cell_04_34970," ); 
                    sb.Append("@Cell_05_34970," );
                    sb.Append("@Cell_06_34970," ); 
                    sb.Append("@Cell_07_34970," ); 
                    sb.Append("@Cell_08_34970," ); 
                    sb.Append("@Cell_09_34970," ); 
                    sb.Append("@Cell_10_34970," );
                    sb.Append("@Cell_11_34970," ); 
                    sb.Append("@Cell_12_34970," ); 
                    sb.Append("@Cell_13_34970," ); 
                    sb.Append("@Cell_14_34970," ); 
                    sb.Append("@Cell_15_34970," );
                    sb.Append("@Cell_16_34970," ); 
                    sb.Append("@Cell_17_34970," ); 
                    sb.Append("@Cell_18_34970," ); 
                    sb.Append("@Cell_19_34970," ); 
                    sb.Append("@Cell_20_34970," );
                    sb.Append("@Cell_21_34970," ); 
                    sb.Append("@Cell_22_34970," ); 
                    sb.Append("@Cell_23_34970," ); 
                    sb.Append("@Cell_24_34970," ); 
                    sb.Append("@Cell_25_34970," );
                    sb.Append("@Cell_26_34970," ); 
                    sb.Append("@Cell_27_34970," ); 
                    sb.Append("@Cell_28_34970," ); 
                    sb.Append("@Cell_29_34970," ); 
                    sb.Append("@Cell_30_34970," );
                    sb.Append("@Cell_31_34970," ); 
                    sb.Append("@Cell_32_34970," ); 
                    sb.Append("@Cell_33_34970," ); 
                    sb.Append("@Cell_34_34970," ); 
                    sb.Append("@Cell_35_34970," );
                    sb.Append("@Cell_36_34970," ); 
                    sb.Append("@Cell_37_34970," ); 
                    sb.Append("@Cell_38_34970," ); 
                    sb.Append("@Cell_39_34970," ); 
                    sb.Append("@Cell_40_34970," );
                    sb.Append("@Module_34970," );
                    sb.Append("@Cell_01_daq," ); 
                    sb.Append("@Cell_02_daq," ); 
                    sb.Append("@Cell_03_daq," ); 
                    sb.Append("@Cell_04_daq," ); 
                    sb.Append("@Cell_05_daq," );
                    sb.Append("@Cell_06_daq," ); 
                    sb.Append("@Cell_07_daq," ); 
                    sb.Append("@Cell_08_daq," ); 
                    sb.Append("@Cell_09_daq," ); 
                    sb.Append("@Cell_10_daq," );
                    sb.Append("@Cell_11_daq," ); 
                    sb.Append("@Cell_12_daq," ); 
                    sb.Append("@Cell_13_daq," ); 
                    sb.Append("@Cell_14_daq," ); 
                    sb.Append("@Cell_15_daq," );
                    sb.Append("@Cell_16_daq," ); 
                    sb.Append("@Cell_17_daq," ); 
                    sb.Append("@Cell_18_daq," ); 
                    sb.Append("@Cell_19_daq," ); 
                    sb.Append("@Cell_20_daq," );
                    sb.Append("@Cell_21_daq," ); 
                    sb.Append("@Cell_22_daq," ); 
                    sb.Append("@Cell_23_daq," ); 
                    sb.Append("@Cell_24_daq," ); 
                    sb.Append("@Cell_25_daq," );
                    sb.Append("@Cell_26_daq," ); 
                    sb.Append("@Cell_27_daq," ); 
                    sb.Append("@Cell_28_daq," ); 
                    sb.Append("@Cell_29_daq," ); 
                    sb.Append("@Cell_30_daq," );
                    sb.Append("@Cell_31_daq," ); 
                    sb.Append("@Cell_32_daq," ); 
                    sb.Append("@Cell_33_daq," ); 
                    sb.Append("@Cell_34_daq," ); 
                    sb.Append("@Cell_35_daq," );
                    sb.Append("@Cell_36_daq," ); 
                    sb.Append("@Cell_37_daq," ); 
                    sb.Append("@Cell_38_daq," ); 
                    sb.Append("@Cell_39_daq," ); 
                    sb.Append("@Cell_40_daq," );
                    sb.Append("@Module_cycler" );
                    sb.Append(")");
                    #endregion

                    OleDbCommand cmd = new OleDbCommand(sb.ToString());

                    cmd.Connection = conn;

                    conn.Open();

                    #region param add
                    //cmd.Parameters.Add("@ID", OleDbType.Integer).Value = pre.ID;
                    cmd.Parameters.Add("@FinishedTime", OleDbType.Date).Value = pre.FinishedTime;
                    cmd.Parameters.Add("@IsPass", OleDbType.VarChar).Value = pre.IsPass;
                    cmd.Parameters.Add("@StationNum", OleDbType.VarChar).Value = pre.StationNum;
                    cmd.Parameters.Add("@ModelName", OleDbType.VarChar).Value = pre.ModelName;
                    cmd.Parameters.Add("@LotID", OleDbType.VarChar).Value = pre.LotID;
                    cmd.Parameters.Add("@ModuleBCR", OleDbType.VarChar).Value = pre.ModuleBCR;
                    cmd.Parameters.Add("@IsMESSkip", OleDbType.VarChar).Value = pre.IsMESSkip;
                    cmd.Parameters.Add("@IsAlarmed", OleDbType.VarChar).Value = pre.IsAlarmed;
                    cmd.Parameters.Add("@Bef_inclination", OleDbType.Double).Value = pre.Bef_inclination;
                    cmd.Parameters.Add("@Now_inclination", OleDbType.Double).Value = pre.Now_inclination;
                    cmd.Parameters.Add("@Cell_01_dev", OleDbType.Double).Value = pre.Cell_01_dev;
                    cmd.Parameters.Add("@Cell_02_dev", OleDbType.Double).Value = pre.Cell_02_dev;
                    cmd.Parameters.Add("@Cell_03_dev", OleDbType.Double).Value = pre.Cell_03_dev;
                    cmd.Parameters.Add("@Cell_04_dev", OleDbType.Double).Value = pre.Cell_04_dev;
                    cmd.Parameters.Add("@Cell_05_dev", OleDbType.Double).Value = pre.Cell_05_dev;
                    cmd.Parameters.Add("@Cell_06_dev", OleDbType.Double).Value = pre.Cell_06_dev;
                    cmd.Parameters.Add("@Cell_07_dev", OleDbType.Double).Value = pre.Cell_07_dev;
                    cmd.Parameters.Add("@Cell_08_dev", OleDbType.Double).Value = pre.Cell_08_dev;
                    cmd.Parameters.Add("@Cell_09_dev", OleDbType.Double).Value = pre.Cell_09_dev;
                    cmd.Parameters.Add("@Cell_10_dev", OleDbType.Double).Value = pre.Cell_10_dev;
                    cmd.Parameters.Add("@Cell_11_dev", OleDbType.Double).Value = pre.Cell_11_dev;
                    cmd.Parameters.Add("@Cell_12_dev", OleDbType.Double).Value = pre.Cell_12_dev;
                    cmd.Parameters.Add("@Cell_13_dev", OleDbType.Double).Value = pre.Cell_13_dev;
                    cmd.Parameters.Add("@Cell_14_dev", OleDbType.Double).Value = pre.Cell_14_dev;
                    cmd.Parameters.Add("@Cell_15_dev", OleDbType.Double).Value = pre.Cell_15_dev;
                    cmd.Parameters.Add("@Cell_16_dev", OleDbType.Double).Value = pre.Cell_16_dev;
                    cmd.Parameters.Add("@Cell_17_dev", OleDbType.Double).Value = pre.Cell_17_dev;
                    cmd.Parameters.Add("@Cell_18_dev", OleDbType.Double).Value = pre.Cell_18_dev;
                    cmd.Parameters.Add("@Cell_19_dev", OleDbType.Double).Value = pre.Cell_19_dev;
                    cmd.Parameters.Add("@Cell_20_dev", OleDbType.Double).Value = pre.Cell_20_dev;
                    cmd.Parameters.Add("@Cell_21_dev", OleDbType.Double).Value = pre.Cell_21_dev;
                    cmd.Parameters.Add("@Cell_22_dev", OleDbType.Double).Value = pre.Cell_22_dev;
                    cmd.Parameters.Add("@Cell_23_dev", OleDbType.Double).Value = pre.Cell_23_dev;
                    cmd.Parameters.Add("@Cell_24_dev", OleDbType.Double).Value = pre.Cell_24_dev;
                    cmd.Parameters.Add("@Cell_25_dev", OleDbType.Double).Value = pre.Cell_25_dev;
                    cmd.Parameters.Add("@Cell_26_dev", OleDbType.Double).Value = pre.Cell_26_dev;
                    cmd.Parameters.Add("@Cell_27_dev", OleDbType.Double).Value = pre.Cell_27_dev;
                    cmd.Parameters.Add("@Cell_28_dev", OleDbType.Double).Value = pre.Cell_28_dev;
                    cmd.Parameters.Add("@Cell_29_dev", OleDbType.Double).Value = pre.Cell_29_dev;
                    cmd.Parameters.Add("@Cell_30_dev", OleDbType.Double).Value = pre.Cell_30_dev;
                    cmd.Parameters.Add("@Cell_31_dev", OleDbType.Double).Value = pre.Cell_31_dev;
                    cmd.Parameters.Add("@Cell_32_dev", OleDbType.Double).Value = pre.Cell_32_dev;
                    cmd.Parameters.Add("@Cell_33_dev", OleDbType.Double).Value = pre.Cell_33_dev;
                    cmd.Parameters.Add("@Cell_34_dev", OleDbType.Double).Value = pre.Cell_34_dev;
                    cmd.Parameters.Add("@Cell_35_dev", OleDbType.Double).Value = pre.Cell_35_dev;
                    cmd.Parameters.Add("@Cell_36_dev", OleDbType.Double).Value = pre.Cell_36_dev;
                    cmd.Parameters.Add("@Cell_37_dev", OleDbType.Double).Value = pre.Cell_37_dev;
                    cmd.Parameters.Add("@Cell_38_dev", OleDbType.Double).Value = pre.Cell_38_dev;
                    cmd.Parameters.Add("@Cell_39_dev", OleDbType.Double).Value = pre.Cell_39_dev;
                    cmd.Parameters.Add("@Cell_40_dev", OleDbType.Double).Value = pre.Cell_40_dev;
                    cmd.Parameters.Add("@Module_dev", OleDbType.Double).Value = pre.Module_dev;
                    cmd.Parameters.Add("@Cell_01_34970", OleDbType.Double).Value = pre.Cell_01_34970;
                    cmd.Parameters.Add("@Cell_02_34970", OleDbType.Double).Value = pre.Cell_02_34970;
                    cmd.Parameters.Add("@Cell_03_34970", OleDbType.Double).Value = pre.Cell_03_34970;
                    cmd.Parameters.Add("@Cell_04_34970", OleDbType.Double).Value = pre.Cell_04_34970;
                    cmd.Parameters.Add("@Cell_05_34970", OleDbType.Double).Value = pre.Cell_05_34970;
                    cmd.Parameters.Add("@Cell_06_34970", OleDbType.Double).Value = pre.Cell_06_34970;
                    cmd.Parameters.Add("@Cell_07_34970", OleDbType.Double).Value = pre.Cell_07_34970;
                    cmd.Parameters.Add("@Cell_08_34970", OleDbType.Double).Value = pre.Cell_08_34970;
                    cmd.Parameters.Add("@Cell_09_34970", OleDbType.Double).Value = pre.Cell_09_34970;
                    cmd.Parameters.Add("@Cell_10_34970", OleDbType.Double).Value = pre.Cell_10_34970;
                    cmd.Parameters.Add("@Cell_11_34970", OleDbType.Double).Value = pre.Cell_11_34970;
                    cmd.Parameters.Add("@Cell_12_34970", OleDbType.Double).Value = pre.Cell_12_34970;
                    cmd.Parameters.Add("@Cell_13_34970", OleDbType.Double).Value = pre.Cell_13_34970;
                    cmd.Parameters.Add("@Cell_14_34970", OleDbType.Double).Value = pre.Cell_14_34970;
                    cmd.Parameters.Add("@Cell_15_34970", OleDbType.Double).Value = pre.Cell_15_34970;
                    cmd.Parameters.Add("@Cell_16_34970", OleDbType.Double).Value = pre.Cell_16_34970;
                    cmd.Parameters.Add("@Cell_17_34970", OleDbType.Double).Value = pre.Cell_17_34970;
                    cmd.Parameters.Add("@Cell_18_34970", OleDbType.Double).Value = pre.Cell_18_34970;
                    cmd.Parameters.Add("@Cell_19_34970", OleDbType.Double).Value = pre.Cell_19_34970;
                    cmd.Parameters.Add("@Cell_20_34970", OleDbType.Double).Value = pre.Cell_20_34970;
                    cmd.Parameters.Add("@Cell_21_34970", OleDbType.Double).Value = pre.Cell_21_34970;
                    cmd.Parameters.Add("@Cell_22_34970", OleDbType.Double).Value = pre.Cell_22_34970;
                    cmd.Parameters.Add("@Cell_23_34970", OleDbType.Double).Value = pre.Cell_23_34970;
                    cmd.Parameters.Add("@Cell_24_34970", OleDbType.Double).Value = pre.Cell_24_34970;
                    cmd.Parameters.Add("@Cell_25_34970", OleDbType.Double).Value = pre.Cell_25_34970;
                    cmd.Parameters.Add("@Cell_26_34970", OleDbType.Double).Value = pre.Cell_26_34970;
                    cmd.Parameters.Add("@Cell_27_34970", OleDbType.Double).Value = pre.Cell_27_34970;
                    cmd.Parameters.Add("@Cell_28_34970", OleDbType.Double).Value = pre.Cell_28_34970;
                    cmd.Parameters.Add("@Cell_29_34970", OleDbType.Double).Value = pre.Cell_29_34970;
                    cmd.Parameters.Add("@Cell_30_34970", OleDbType.Double).Value = pre.Cell_30_34970;
                    cmd.Parameters.Add("@Cell_31_34970", OleDbType.Double).Value = pre.Cell_31_34970;
                    cmd.Parameters.Add("@Cell_32_34970", OleDbType.Double).Value = pre.Cell_32_34970;
                    cmd.Parameters.Add("@Cell_33_34970", OleDbType.Double).Value = pre.Cell_33_34970;
                    cmd.Parameters.Add("@Cell_34_34970", OleDbType.Double).Value = pre.Cell_34_34970;
                    cmd.Parameters.Add("@Cell_35_34970", OleDbType.Double).Value = pre.Cell_35_34970;
                    cmd.Parameters.Add("@Cell_36_34970", OleDbType.Double).Value = pre.Cell_36_34970;
                    cmd.Parameters.Add("@Cell_37_34970", OleDbType.Double).Value = pre.Cell_37_34970;
                    cmd.Parameters.Add("@Cell_38_34970", OleDbType.Double).Value = pre.Cell_38_34970;
                    cmd.Parameters.Add("@Cell_39_34970", OleDbType.Double).Value = pre.Cell_39_34970;
                    cmd.Parameters.Add("@Cell_40_34970", OleDbType.Double).Value = pre.Cell_40_34970;
                    cmd.Parameters.Add("@Module_34970", OleDbType.Double).Value = pre.Module_34970;
                    cmd.Parameters.Add("@Cell_01_daq", OleDbType.Double).Value = pre.Cell_01_daq;
                    cmd.Parameters.Add("@Cell_02_daq", OleDbType.Double).Value = pre.Cell_02_daq;
                    cmd.Parameters.Add("@Cell_03_daq", OleDbType.Double).Value = pre.Cell_03_daq;
                    cmd.Parameters.Add("@Cell_04_daq", OleDbType.Double).Value = pre.Cell_04_daq;
                    cmd.Parameters.Add("@Cell_05_daq", OleDbType.Double).Value = pre.Cell_05_daq;
                    cmd.Parameters.Add("@Cell_06_daq", OleDbType.Double).Value = pre.Cell_06_daq;
                    cmd.Parameters.Add("@Cell_07_daq", OleDbType.Double).Value = pre.Cell_07_daq;
                    cmd.Parameters.Add("@Cell_08_daq", OleDbType.Double).Value = pre.Cell_08_daq;
                    cmd.Parameters.Add("@Cell_09_daq", OleDbType.Double).Value = pre.Cell_09_daq;
                    cmd.Parameters.Add("@Cell_10_daq", OleDbType.Double).Value = pre.Cell_10_daq;
                    cmd.Parameters.Add("@Cell_11_daq", OleDbType.Double).Value = pre.Cell_11_daq;
                    cmd.Parameters.Add("@Cell_12_daq", OleDbType.Double).Value = pre.Cell_12_daq;
                    cmd.Parameters.Add("@Cell_13_daq", OleDbType.Double).Value = pre.Cell_13_daq;
                    cmd.Parameters.Add("@Cell_14_daq", OleDbType.Double).Value = pre.Cell_14_daq;
                    cmd.Parameters.Add("@Cell_15_daq", OleDbType.Double).Value = pre.Cell_15_daq;
                    cmd.Parameters.Add("@Cell_16_daq", OleDbType.Double).Value = pre.Cell_16_daq;
                    cmd.Parameters.Add("@Cell_17_daq", OleDbType.Double).Value = pre.Cell_17_daq;
                    cmd.Parameters.Add("@Cell_18_daq", OleDbType.Double).Value = pre.Cell_18_daq;
                    cmd.Parameters.Add("@Cell_19_daq", OleDbType.Double).Value = pre.Cell_19_daq;
                    cmd.Parameters.Add("@Cell_20_daq", OleDbType.Double).Value = pre.Cell_20_daq;
                    cmd.Parameters.Add("@Cell_21_daq", OleDbType.Double).Value = pre.Cell_21_daq;
                    cmd.Parameters.Add("@Cell_22_daq", OleDbType.Double).Value = pre.Cell_22_daq;
                    cmd.Parameters.Add("@Cell_23_daq", OleDbType.Double).Value = pre.Cell_23_daq;
                    cmd.Parameters.Add("@Cell_24_daq", OleDbType.Double).Value = pre.Cell_24_daq;
                    cmd.Parameters.Add("@Cell_25_daq", OleDbType.Double).Value = pre.Cell_25_daq;
                    cmd.Parameters.Add("@Cell_26_daq", OleDbType.Double).Value = pre.Cell_26_daq;
                    cmd.Parameters.Add("@Cell_27_daq", OleDbType.Double).Value = pre.Cell_27_daq;
                    cmd.Parameters.Add("@Cell_28_daq", OleDbType.Double).Value = pre.Cell_28_daq;
                    cmd.Parameters.Add("@Cell_29_daq", OleDbType.Double).Value = pre.Cell_29_daq;
                    cmd.Parameters.Add("@Cell_30_daq", OleDbType.Double).Value = pre.Cell_30_daq;
                    cmd.Parameters.Add("@Cell_31_daq", OleDbType.Double).Value = pre.Cell_31_daq;
                    cmd.Parameters.Add("@Cell_32_daq", OleDbType.Double).Value = pre.Cell_32_daq;
                    cmd.Parameters.Add("@Cell_33_daq", OleDbType.Double).Value = pre.Cell_33_daq;
                    cmd.Parameters.Add("@Cell_34_daq", OleDbType.Double).Value = pre.Cell_34_daq;
                    cmd.Parameters.Add("@Cell_35_daq", OleDbType.Double).Value = pre.Cell_35_daq;
                    cmd.Parameters.Add("@Cell_36_daq", OleDbType.Double).Value = pre.Cell_36_daq;
                    cmd.Parameters.Add("@Cell_37_daq", OleDbType.Double).Value = pre.Cell_37_daq;
                    cmd.Parameters.Add("@Cell_38_daq", OleDbType.Double).Value = pre.Cell_38_daq;
                    cmd.Parameters.Add("@Cell_39_daq", OleDbType.Double).Value = pre.Cell_39_daq;
                    cmd.Parameters.Add("@Cell_40_daq", OleDbType.Double).Value = pre.Cell_40_daq;
                    cmd.Parameters.Add("@Module_cycler", OleDbType.Double).Value = pre.Module_cycler;
                    #endregion

                    cmd.ExecuteNonQuery();

                    conn.Close();

                    return true;
                }
            }
            catch (Exception ec)
            {
                LogState(LogType.Fail, "AddPredictData", ec);
                return false;
            }
        }

        public void LoadPredictFileList()
        {
            //파일을 불러온다.
            //key값 / 합불판정 / 스테이션번호 / 모델이름 / 랏 / 모듈BCR / MES스킵여부 / 알람발생여부 / 이전기울기 / 현재기울기 / 970 셀전압 / 970 모듈전압 / DAQ 셀전압 / Cycler 모듈전압

            //200925 wjs add not use
            return;

            try
            {
                
                string newDB = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + staticPath_PREDICT + "\\" + predictFileName + ";" + "Jet OLEDB:Database Password=";
                Type objClassType = Type.GetTypeFromProgID("ADOX.Catalog");
                if (objClassType != null)
                {
                    //파일이 없다면 생성
                    object obj = Activator.CreateInstance(objClassType);

                    // Create MDB file
                    obj.GetType().InvokeMember("Create", System.Reflection.BindingFlags.InvokeMethod, null, obj,
                                        new object[] { "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + newDB + ";" });

                    // Clean up
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                    obj = null;

                    #region make table
                    string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + staticPath_PREDICT + "\\" + predictFileName;

                    using (OleDbConnection conn = new OleDbConnection(connStr))
                    {
                        conn.Open();

                        StringBuilder sb = new StringBuilder();
                        sb.Append("CREATE TABLE ");
                        sb.Append(tableName);
                        sb.Append(" (");
                        sb.Append("ID int identity primary key, ");
                        sb.Append("FinishedTime DateTime, ");
                        sb.Append("IsPass nvarchar(100), ");
                        sb.Append("StationNum nvarchar(100), ");
                        sb.Append("ModelName nvarchar(100), ");
                        sb.Append("LotID nvarchar(100), ");
                        sb.Append("ModuleBCR nvarchar(100), ");
                        sb.Append("IsMESSkip nvarchar(100), ");
                        sb.Append("IsAlarmed nvarchar(100), ");
                        sb.Append("Bef_inclination float, ");
                        sb.Append("Now_inclination float, ");
                        sb.Append("Cell_01_dev float, ");
                        sb.Append("Cell_02_dev float, ");
                        sb.Append("Cell_03_dev float, ");
                        sb.Append("Cell_04_dev float, ");
                        sb.Append("Cell_05_dev float, ");
                        sb.Append("Cell_06_dev float, ");
                        sb.Append("Cell_07_dev float, ");
                        sb.Append("Cell_08_dev float, ");
                        sb.Append("Cell_09_dev float, ");
                        sb.Append("Cell_10_dev float, ");
                        sb.Append("Cell_11_dev float, ");
                        sb.Append("Cell_12_dev float, ");
                        sb.Append("Cell_13_dev float, ");
                        sb.Append("Cell_14_dev float, ");
                        sb.Append("Cell_15_dev float, ");
                        sb.Append("Cell_16_dev float, ");
                        sb.Append("Cell_17_dev float, ");
                        sb.Append("Cell_18_dev float, ");
                        sb.Append("Cell_19_dev float, ");
                        sb.Append("Cell_20_dev float, ");
                        sb.Append("Cell_21_dev float, ");
                        sb.Append("Cell_22_dev float, ");
                        sb.Append("Cell_23_dev float, ");
                        sb.Append("Cell_24_dev float, ");
                        sb.Append("Cell_25_dev float, ");
                        sb.Append("Cell_26_dev float, ");
                        sb.Append("Cell_27_dev float, ");
                        sb.Append("Cell_28_dev float, ");
                        sb.Append("Cell_29_dev float, ");
                        sb.Append("Cell_30_dev float, ");
                        sb.Append("Cell_31_dev float, ");
                        sb.Append("Cell_32_dev float, ");
                        sb.Append("Cell_33_dev float, ");
                        sb.Append("Cell_34_dev float, ");
                        sb.Append("Cell_35_dev float, ");
                        sb.Append("Cell_36_dev float, ");
                        sb.Append("Cell_37_dev float, ");
                        sb.Append("Cell_38_dev float, ");
                        sb.Append("Cell_39_dev float, ");
                        sb.Append("Cell_40_dev float, ");
                        sb.Append("Module_dev float, ");
                        sb.Append("Cell_01_34970 float, ");
                        sb.Append("Cell_02_34970 float, ");
                        sb.Append("Cell_03_34970 float, ");
                        sb.Append("Cell_04_34970 float, ");
                        sb.Append("Cell_05_34970 float, ");
                        sb.Append("Cell_06_34970 float, ");
                        sb.Append("Cell_07_34970 float, ");
                        sb.Append("Cell_08_34970 float, ");
                        sb.Append("Cell_09_34970 float, ");
                        sb.Append("Cell_10_34970 float, ");
                        sb.Append("Cell_11_34970 float, ");
                        sb.Append("Cell_12_34970 float, ");
                        sb.Append("Cell_13_34970 float, ");
                        sb.Append("Cell_14_34970 float, ");
                        sb.Append("Cell_15_34970 float, ");
                        sb.Append("Cell_16_34970 float, ");
                        sb.Append("Cell_17_34970 float, ");
                        sb.Append("Cell_18_34970 float, ");
                        sb.Append("Cell_19_34970 float, ");
                        sb.Append("Cell_20_34970 float, ");
                        sb.Append("Cell_21_34970 float, ");
                        sb.Append("Cell_22_34970 float, ");
                        sb.Append("Cell_23_34970 float, ");
                        sb.Append("Cell_24_34970 float, ");
                        sb.Append("Cell_25_34970 float, ");
                        sb.Append("Cell_26_34970 float, ");
                        sb.Append("Cell_27_34970 float, ");
                        sb.Append("Cell_28_34970 float, ");
                        sb.Append("Cell_29_34970 float, ");
                        sb.Append("Cell_30_34970 float, ");
                        sb.Append("Cell_31_34970 float, ");
                        sb.Append("Cell_32_34970 float, ");
                        sb.Append("Cell_33_34970 float, ");
                        sb.Append("Cell_34_34970 float, ");
                        sb.Append("Cell_35_34970 float, ");
                        sb.Append("Cell_36_34970 float, ");
                        sb.Append("Cell_37_34970 float, ");
                        sb.Append("Cell_38_34970 float, ");
                        sb.Append("Cell_39_34970 float, ");
                        sb.Append("Cell_40_34970 float, ");
                        sb.Append("Module_34970 float, ");
                        sb.Append("Cell_01_daq float, ");
                        sb.Append("Cell_02_daq float, ");
                        sb.Append("Cell_03_daq float, ");
                        sb.Append("Cell_04_daq float, ");
                        sb.Append("Cell_05_daq float, ");
                        sb.Append("Cell_06_daq float, ");
                        sb.Append("Cell_07_daq float, ");
                        sb.Append("Cell_08_daq float, ");
                        sb.Append("Cell_09_daq float, ");
                        sb.Append("Cell_10_daq float, ");
                        sb.Append("Cell_11_daq float, ");
                        sb.Append("Cell_12_daq float, ");
                        sb.Append("Cell_13_daq float, ");
                        sb.Append("Cell_14_daq float, ");
                        sb.Append("Cell_15_daq float, ");
                        sb.Append("Cell_16_daq float, ");
                        sb.Append("Cell_17_daq float, ");
                        sb.Append("Cell_18_daq float, ");
                        sb.Append("Cell_19_daq float, ");
                        sb.Append("Cell_20_daq float, ");
                        sb.Append("Cell_21_daq float, ");
                        sb.Append("Cell_22_daq float, ");
                        sb.Append("Cell_23_daq float, ");
                        sb.Append("Cell_24_daq float, ");
                        sb.Append("Cell_25_daq float, ");
                        sb.Append("Cell_26_daq float, ");
                        sb.Append("Cell_27_daq float, ");
                        sb.Append("Cell_28_daq float, ");
                        sb.Append("Cell_29_daq float, ");
                        sb.Append("Cell_30_daq float, ");
                        sb.Append("Cell_31_daq float, ");
                        sb.Append("Cell_32_daq float, ");
                        sb.Append("Cell_33_daq float, ");
                        sb.Append("Cell_34_daq float, ");
                        sb.Append("Cell_35_daq float, ");
                        sb.Append("Cell_36_daq float, ");
                        sb.Append("Cell_37_daq float, ");
                        sb.Append("Cell_38_daq float, ");
                        sb.Append("Cell_39_daq float, ");
                        sb.Append("Cell_40_daq float, ");
                        sb.Append("Module_cycler float");
                        sb.Append(")");

                        OleDbCommand cmd = new OleDbCommand(sb.ToString(), conn);
                        cmd.ExecuteNonQuery();

                        //var sql = "INSERT INTO MyTable VALUES(1,'Lee')";
                        //cmd.CommandText = sql;
                        //cmd.ExecuteNonQuery();

                        conn.Close();

                    }


                }

                //파일이 없다면 테이블 생성

                #endregion
            }
            catch (Exception ex)
            {
                //LogState(LogType.Fail, "Create ACCDB File", ex);
                //MessageBox.Show("Could not create database file: " + staticPath_DB + "\\" + predictFileName + "\n\n" + ex.Message, "Database Creation Error");
            }
            finally
            {
            }

            #region 임시 DB 테스트용 자료 만들기
            //randomData();
            #endregion

        }
        public void randomData()
        {

            Random r = new Random();
            for (int i = 0; i < 500; i++)
            {
                var pr = new Predict();
                //pr.ID = i;
                pr.LotID = "TEST_LOT" + i.ToString("D4");
                pr.FinishedTime = DateTime.Now.AddDays(-1 * (r.Next(0, 150)));
                pr.ModelName = modelDataList[r.Next(0, 3)].ModelId;
                pr.StationNum = "#" + r.Next(1, 5);//1,2,3,4중에 나옴
                pr.Cell_01_dev = r.Next(1, 10);
                pr.Cell_02_dev = r.Next(1, 10);
                pr.Cell_03_dev = r.Next(1, 10);
                pr.Cell_04_dev = r.Next(1, 10);
                pr.Cell_05_dev = r.Next(1, 10);
                pr.Cell_06_dev = r.Next(1, 10);
                pr.Cell_07_dev = r.Next(1, 10);
                pr.Cell_08_dev = r.Next(1, 10);
                pr.Cell_09_dev = r.Next(1, 10);
                pr.Cell_10_dev = r.Next(1, 10);
                pr.Module_dev = r.Next(1, 10);
                pr.IsMESSkip = "NO";
                if (r.Next(0, 6) > 3)
                {
                    pr.IsAlarmed = "YES";
                }
                else
                {
                    pr.IsAlarmed = "NO";
                }

                if (r.Next(0, 6) > 3)//1,2,3,4,5 사이에서 나옴
                {
                    pr.IsPass = "YES";
                }

                AddPredictData(pr);
            }
        }

        public List<Predict> GetDataToSQL(object data)
        {
            var list = new List<Predict>(); 
            try
            {
                string connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + staticPath_PREDICT + "\\" + predictFileName;
                using (OleDbConnection conn = new OleDbConnection(connStr))
                {
                    conn.Open();

                    OleDbCommand cmd = new OleDbCommand();
                    if (data is OleDbCommand)
                    {
                        cmd = data as OleDbCommand;
                        cmd.Connection = conn;
                    }
                    else if (data is string)
                    {
                        var sql = data as string;
                        cmd = new OleDbCommand(sql, conn);
                        cmd.CommandText = sql;
                    }

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var pred = new Predict()
                        {
                            LotID = (string)reader["LotID"],
                            StationNum = (string)reader["StationNum"],
                            ModelName = (string)reader["ModelName"],
                            FinishedTime = (DateTime)reader["FinishedTime"],
                            IsPass = (string)reader["IsPass"],
                            IsMESSkip = (string)reader["IsMESSkip"],
                            IsAlarmed = (string)reader["IsAlarmed"],
                            Bef_inclination = (double)reader["Bef_inclination"],
                            Now_inclination = (double)reader["Now_inclination"],
                            Module_dev = (double)reader["Module_dev"],
                            Cell_01_dev = (double)reader["Cell_01_dev"],
                            Cell_02_dev = (double)reader["Cell_02_dev"],
                            Cell_03_dev = (double)reader["Cell_03_dev"],
                            Cell_04_dev = (double)reader["Cell_04_dev"],
                            Cell_05_dev = (double)reader["Cell_05_dev"],
                            Cell_06_dev = (double)reader["Cell_06_dev"],
                            Cell_07_dev = (double)reader["Cell_07_dev"],
                            Cell_08_dev = (double)reader["Cell_08_dev"],
                            Cell_09_dev = (double)reader["Cell_09_dev"],
                            Cell_10_dev = (double)reader["Cell_10_dev"],
                            Cell_11_dev = (double)reader["Cell_11_dev"],
                            Cell_12_dev = (double)reader["Cell_12_dev"],
                            Cell_13_dev = (double)reader["Cell_13_dev"],
                            Cell_14_dev = (double)reader["Cell_14_dev"],
                            Cell_15_dev = (double)reader["Cell_15_dev"],
                            Cell_16_dev = (double)reader["Cell_16_dev"],
                            Cell_17_dev = (double)reader["Cell_17_dev"],
                            Cell_18_dev = (double)reader["Cell_18_dev"],
                            Cell_19_dev = (double)reader["Cell_19_dev"],
                            Cell_20_dev = (double)reader["Cell_20_dev"],
                            Cell_21_dev = (double)reader["Cell_21_dev"],
                            Cell_22_dev = (double)reader["Cell_22_dev"],
                            Cell_23_dev = (double)reader["Cell_23_dev"],
                            Cell_24_dev = (double)reader["Cell_24_dev"],
                            Cell_25_dev = (double)reader["Cell_25_dev"],
                            Cell_26_dev = (double)reader["Cell_26_dev"],
                            Cell_27_dev = (double)reader["Cell_27_dev"],
                            Cell_28_dev = (double)reader["Cell_28_dev"],
                            Cell_29_dev = (double)reader["Cell_29_dev"],
                            Cell_30_dev = (double)reader["Cell_30_dev"],
                            Cell_31_dev = (double)reader["Cell_31_dev"],
                            Cell_32_dev = (double)reader["Cell_32_dev"],
                            Cell_33_dev = (double)reader["Cell_33_dev"],
                            Cell_34_dev = (double)reader["Cell_34_dev"],
                            Cell_35_dev = (double)reader["Cell_35_dev"],
                            Cell_36_dev = (double)reader["Cell_36_dev"],
                            Cell_37_dev = (double)reader["Cell_37_dev"],
                            Cell_38_dev = (double)reader["Cell_38_dev"],
                            Cell_39_dev = (double)reader["Cell_39_dev"],
                            Cell_40_dev = (double)reader["Cell_40_dev"],
                        };
                        list.Add(pred);
                    }

                }
            }
            catch (Exception ec)
            {
                LogState(LogType.Fail, "GetDataToSQL", ec);
            }
            return list;
        }
    }
}
