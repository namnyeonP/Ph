using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace EOL_BASE.윈도우
{
    /// <summary>
    /// MasterBcrWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MasterBcrWindow : Window
    {
        string BcrText;
        MainWindow mw;
        public bool ch = true;

        public MasterBcrWindow(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;
            this.Loaded += MasterBcrWindow_Loaded;
            this.Closed += MasterBcrWindow_Closed;

            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            //rk.SetValue("Access_DMM", "0");                        
            var regStr = rk.GetValue("Master_BCR") as string;

            if (regStr == null)
            {
                rk.SetValue("Master_BCR", "No BCR");
            }
            else
            {
                BcrText = rk.GetValue("Master_BCR").ToString();
            }
        }
        void MasterBcrWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
            bcrTb.Text = BcrText;
            
        }

        void MasterBcrWindow_Closed(object sender, EventArgs e)
        {
            ch = true;

            if (bcrTb.Text != BcrText)
            {
                ch = false;
            }
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
            rk.SetValue("Master_BCR", bcrTb.Text.ToString());
        }
    }
}
