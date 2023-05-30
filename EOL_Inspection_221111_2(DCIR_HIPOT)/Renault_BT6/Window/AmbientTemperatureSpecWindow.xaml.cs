using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Renault_BT6.클래스;

namespace Renault_BT6.윈도우
{
    /// <summary>
    /// AmbientTemperatureSpecWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AmbientTemperatureSpecWindow : Window
    {
        AmbientTempSetting setting = new AmbientTempSetting();

        string filepath;
        PGLB pglb = new PGLB();

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        public AmbientTemperatureSpecWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            filepath = pglb.FilePath;
            LoadSetting();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            setting.LowerTemp1 = tbLowerTemp1.Text;
            setting.HigherTemp1 = tbHigherTemp1.Text;
            setting.Resistance1 = tbResist1.Text;
            setting.CellResistance1 = tbCellResist1.Text;

            setting.LowerTemp2 = tbLowerTemp2.Text;
            setting.HigherTemp2 = tbHigherTemp2.Text;
            setting.Resistance2 = tbResist2.Text;
            setting.CellResistance2 = tbCellResist2.Text;

            setting.LowerTemp3 = tbLowerTemp3.Text;
            setting.HigherTemp3 = tbHigherTemp3.Text;
            setting.Resistance3 = tbResist3.Text;
            setting.CellResistance3 = tbCellResist3.Text;

            setting.LowerTemp4 = tbLowerTemp4.Text;
            setting.HigherTemp4 = tbHigherTemp4.Text;
            setting.Resistance4 = tbResist4.Text;
            setting.CellResistance4 = tbCellResist4.Text;

            setting.LowerTemp5 = tbLowerTemp5.Text;
            setting.HigherTemp5 = tbHigherTemp5.Text;
            setting.Resistance5 = tbResist5.Text;
            setting.CellResistance5 = tbCellResist5.Text;

            SaveSetting();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void LoadSpec(AmbientTempSetting setting)
        {
            if (!File.Exists(filepath)) return;

            StringBuilder sb = new StringBuilder();
            GetPrivateProfileString("Temp1", "LowerTemp1", "", sb, 20, filepath);
            setting.LowerTemp1 = sb.ToString();
            GetPrivateProfileString("Temp2", "LowerTemp2", "", sb, 20, filepath);
            setting.LowerTemp2 = sb.ToString();
            GetPrivateProfileString("Temp3", "LowerTemp3", "", sb, 20, filepath);
            setting.LowerTemp3 = sb.ToString();
            GetPrivateProfileString("Temp4", "LowerTemp4", "", sb, 20, filepath);
            setting.LowerTemp4 = sb.ToString();
            GetPrivateProfileString("Temp5", "LowerTemp5", "", sb, 20, filepath);
            setting.LowerTemp5 = sb.ToString();

            GetPrivateProfileString("Temp1", "HigherTemp1", "", sb, 20, filepath);
            setting.HigherTemp1 = sb.ToString();
            GetPrivateProfileString("Temp2", "HigherTemp2", "", sb, 20, filepath);
            setting.HigherTemp2 = sb.ToString();
            GetPrivateProfileString("Temp3", "HigherTemp3", "", sb, 20, filepath);
            setting.HigherTemp3 = sb.ToString();
            GetPrivateProfileString("Temp4", "HigherTemp4", "", sb, 20, filepath);
            setting.HigherTemp4 = sb.ToString();
            GetPrivateProfileString("Temp5", "HigherTemp5", "", sb, 20, filepath);
            setting.HigherTemp5 = sb.ToString();

            GetPrivateProfileString("Temp1", "Resistance1", "", sb, 20, filepath);
            setting.Resistance1 = sb.ToString();
            GetPrivateProfileString("Temp2", "Resistance2", "", sb, 20, filepath);
            setting.Resistance2 = sb.ToString();
            GetPrivateProfileString("Temp3", "Resistance3", "", sb, 20, filepath);
            setting.Resistance3 = sb.ToString();
            GetPrivateProfileString("Temp4", "Resistance4", "", sb, 20, filepath);
            setting.Resistance4 = sb.ToString();
            GetPrivateProfileString("Temp5", "Resistance5", "", sb, 20, filepath);
            setting.Resistance5 = sb.ToString();

            GetPrivateProfileString("Temp1", "CellResistance1", "", sb, 20, filepath);
            setting.CellResistance1 = sb.ToString();
            GetPrivateProfileString("Temp2", "CellResistance2", "", sb, 20, filepath);
            setting.CellResistance2 = sb.ToString();
            GetPrivateProfileString("Temp3", "CellResistance3", "", sb, 20, filepath);
            setting.CellResistance3 = sb.ToString();
            GetPrivateProfileString("Temp4", "CellResistance4", "", sb, 20, filepath);
            setting.CellResistance4 = sb.ToString();
            GetPrivateProfileString("Temp5", "CellResistance5", "", sb, 20, filepath);
            setting.CellResistance5 = sb.ToString();
        }

        public void LoadSetting()
        {
            if (!File.Exists(filepath)) return;

            StringBuilder sb = new StringBuilder();
            GetPrivateProfileString("Temp1", "LowerTemp1", "", sb, 20, filepath);
            tbLowerTemp1.Text = sb.ToString();
            GetPrivateProfileString("Temp1", "HigherTemp1", "", sb, 20, filepath);
            tbHigherTemp1.Text = sb.ToString();
            GetPrivateProfileString("Temp1", "Resistance1", "", sb, 20, filepath);
            tbResist1.Text = sb.ToString();
            GetPrivateProfileString("Temp1", "CellResistance1", "", sb, 20, filepath);
            tbCellResist1.Text = sb.ToString();

            GetPrivateProfileString("Temp2", "LowerTemp2", "", sb, 20, filepath);
            tbLowerTemp2.Text = sb.ToString();
            GetPrivateProfileString("Temp2", "HigherTemp2", "", sb, 20, filepath);
            tbHigherTemp2.Text = sb.ToString();
            GetPrivateProfileString("Temp2", "Resistance2", "", sb, 20, filepath);
            tbResist2.Text = sb.ToString();
            GetPrivateProfileString("Temp2", "CellResistance2", "", sb, 20, filepath);
            tbCellResist2.Text = sb.ToString();

            GetPrivateProfileString("Temp3", "LowerTemp3", "", sb, 20, filepath);
            tbLowerTemp3.Text = sb.ToString();
            GetPrivateProfileString("Temp3", "HigherTemp3", "", sb, 20, filepath);
            tbHigherTemp3.Text = sb.ToString();
            GetPrivateProfileString("Temp3", "Resistance3", "", sb, 20, filepath);
            tbResist3.Text = sb.ToString();
            GetPrivateProfileString("Temp3", "CellResistance3", "", sb, 20, filepath);
            tbCellResist3.Text = sb.ToString();

            GetPrivateProfileString("Temp4", "LowerTemp4", "", sb, 20, filepath);
            tbLowerTemp4.Text = sb.ToString();
            GetPrivateProfileString("Temp4", "HigherTemp4", "", sb, 20, filepath);
            tbHigherTemp4.Text = sb.ToString();
            GetPrivateProfileString("Temp4", "Resistance4", "", sb, 20, filepath);
            tbResist4.Text = sb.ToString();
            GetPrivateProfileString("Temp4", "CellResistance4", "", sb, 20, filepath);
            tbCellResist4.Text = sb.ToString();

            GetPrivateProfileString("Temp5", "LowerTemp5", "", sb, 20, filepath);
            tbLowerTemp5.Text = sb.ToString();
            GetPrivateProfileString("Temp5", "HigherTemp5", "", sb, 20, filepath);
            tbHigherTemp5.Text = sb.ToString();
            GetPrivateProfileString("Temp5", "Resistance5", "", sb, 20, filepath);
            tbResist5.Text = sb.ToString();
            GetPrivateProfileString("Temp5", "CellResistance5", "", sb, 20, filepath);
            tbCellResist5.Text = sb.ToString();
        }

        private void SaveSetting()
        {
            string folderpath = @"c:\\" + pglb.FolderName;
            DirectoryInfo dtif = new DirectoryInfo(folderpath);
            if (!dtif.Exists)
            {
                dtif.Create();
            }

            if (!File.Exists(filepath))
            {
                File.Create(filepath);
            }

            WritePrivateProfileString("Temp1", "LowerTemp1", setting.LowerTemp1, filepath);
            WritePrivateProfileString("Temp1", "HigherTemp1", setting.HigherTemp1, filepath);
            WritePrivateProfileString("Temp1", "Resistance1", setting.Resistance1, filepath);
            WritePrivateProfileString("Temp1", "CellResistance1", setting.CellResistance1, filepath);

            WritePrivateProfileString("Temp2", "LowerTemp2", setting.LowerTemp2, filepath);
            WritePrivateProfileString("Temp2", "HigherTemp2", setting.HigherTemp2, filepath);
            WritePrivateProfileString("Temp2", "Resistance2", setting.Resistance2, filepath);
            WritePrivateProfileString("Temp2", "CellResistance2", setting.CellResistance2, filepath);

            WritePrivateProfileString("Temp3", "LowerTemp3", setting.LowerTemp3, filepath);
            WritePrivateProfileString("Temp3", "HigherTemp3", setting.HigherTemp3, filepath);
            WritePrivateProfileString("Temp3", "Resistance3", setting.Resistance3, filepath);
            WritePrivateProfileString("Temp3", "CellResistance3", setting.CellResistance3, filepath);

            WritePrivateProfileString("Temp4", "LowerTemp4", setting.LowerTemp4, filepath);
            WritePrivateProfileString("Temp4", "HigherTemp4", setting.HigherTemp4, filepath);
            WritePrivateProfileString("Temp4", "Resistance4", setting.Resistance4, filepath);
            WritePrivateProfileString("Temp4", "CellResistance4", setting.CellResistance4, filepath);

            WritePrivateProfileString("Temp5", "LowerTemp5", setting.LowerTemp5, filepath);
            WritePrivateProfileString("Temp5", "HigherTemp5", setting.HigherTemp5, filepath);
            WritePrivateProfileString("Temp5", "Resistance5", setting.Resistance5, filepath);
            WritePrivateProfileString("Temp5", "CellResistance5", setting.CellResistance5, filepath);
        }

        private void tbAmbTempSet_GotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.ContextIdle,
                new Action(delegate{(sender as TextBox).SelectAll();}));
        }
    }
}
