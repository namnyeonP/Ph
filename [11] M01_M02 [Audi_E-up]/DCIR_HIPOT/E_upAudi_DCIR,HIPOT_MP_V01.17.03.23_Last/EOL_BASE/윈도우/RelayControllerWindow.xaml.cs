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
using EOL_BASE.클래스;

namespace EOL_BASE.윈도우
{
    /// <summary>
    /// RelayControllerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RelayControllerWindow : Window
    {
        MainWindow mw;
        string text;

        DIOrelays dioRelays = null;
        Dictionary<string, string> relayDic = new Dictionary<string, string>();

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

        void RelayControllerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string regSubkey = "Software\\EOL_Trigger\\Relays";
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

            dioRelays = new DIOrelays();
            relayDic = dioRelays.LoadRelays();
            unigrid.Columns = 4; // uniformgrid 컬럼 수를 정하는 곳

            try
            {
                for (int i = 0; i < 32; i++)
                {
                    var trig = rk.GetValue("Relay_" + i.ToString()).ToString();
                    ToggleButton tb = new ToggleButton();
                    string dioName = "IDO_" + i;
                    //tb.Content = "IDO_" + i;
                    string value = "";
                    relayDic.TryGetValue(dioName, out value);
                    tb.Content = string.Format("{0}-({1})", value, i);
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
            int index = (sender as ToggleButton).Content.ToString().IndexOf("-");
            int length = (sender as ToggleButton).Content.ToString().Length;
            var value = (sender as ToggleButton).Content.ToString().Remove(index, length - index);
            var id = relayDic.FirstOrDefault(x => x.Value == value).Key;

            //var id = (sender as ToggleButton).Content.ToString();

            mw.relays.RelayOff(id);
        }

        private void tb_Checked(object sender, RoutedEventArgs e)
        {
            int index = (sender as ToggleButton).Content.ToString().IndexOf("-");
            int length = (sender as ToggleButton).Content.ToString().Length;
            var value = (sender as ToggleButton).Content.ToString().Remove(index, length - index);
            var id = relayDic.FirstOrDefault(x => x.Value == value).Key;

            //var id = (sender as ToggleButton).Content.ToString();

            mw.relays.RelayOn(id);
        }
    }
}
