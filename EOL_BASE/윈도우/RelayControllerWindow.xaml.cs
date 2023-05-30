using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EOL_BASE.윈도우
{
    /// <summary>
    /// RelayControllerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RelayControllerWindow : Window
    {
        string text;
        public RelayControllerWindow(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;
            this.Loaded += RelayControllerWindow_Loaded;
            this.Closed += RelayControllerWindow_Closed;

            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            //rk.SetValue("Access_DMM", "0");                        
            var regStr = rk.GetValue("Relay_Desc") as string;

            if (regStr == null)
            {
                rk.SetValue("Relay_Desc", "No Desc");
            }
            else
            {
                text = rk.GetValue("Relay_Desc").ToString();
            }
        }

        void RelayControllerWindow_Closed(object sender, EventArgs e)
        {
            string regSubkey = "Software\\EOL_Trigger";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
            rk.SetValue("Relay_Desc", descTb.Text.ToString());
        }
        MainWindow mw;
        void RelayControllerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string regSubkey = "Software\\EOL_Trigger\\Relays";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    var trig = rk.GetValue("Relay_" + i.ToString()).ToString();
                    ToggleButton tb = new ToggleButton();
                    tb.Content = "IDO_" + i;
                    if (trig == "1")
                        tb.IsChecked = true;

                    tb.Checked += tb_Checked;
                    tb.Unchecked += tb_Unchecked;
                    unigrid.Children.Add(tb);
                }
            }
            catch (Exception)
            {
            }

            descTb.Text = text;
        }


        void tb_Unchecked(object sender, RoutedEventArgs e)
        {
            var id = (sender as ToggleButton).Content.ToString();

            mw.relays.RelayOff(id);
        }

        private void tb_Checked(object sender, RoutedEventArgs e)
        {
            var id = (sender as ToggleButton).Content.ToString();

            mw.relays.RelayOn(id);
        }
    }
}
