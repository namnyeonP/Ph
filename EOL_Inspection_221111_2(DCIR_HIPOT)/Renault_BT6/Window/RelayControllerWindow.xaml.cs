using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

namespace Renault_BT6.윈도우
{
    /// <summary>
    /// RelayControllerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RelayControllerWindow : Window
    {
        StringBuilder _sbIOName;
        int _relayCount = 8;

        public RelayControllerWindow()
        {
            InitializeComponent();

            _sbIOName = new StringBuilder();

            this.Loaded += RelayControllerWindow_Loaded;
            this.Closed += RelayControllerWindow_Closed;
        }         

        void RelayControllerWindow_Closed(object sender, EventArgs e)
        {
            _sbIOName.Clear();
        }

        void RelayControllerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Device.Relay.ReadRelayStatus();

                for (int index = 0; index < _relayCount; index++)
                {
                    bool trig = Device.Relay.GetRelayStatus(index);

                    ToggleButton tb = new ToggleButton();
                    tb.Content = index;
                   
                    if (trig == true)
                        tb.IsChecked = true;

                    tb.Checked += tb_Checked;
                    tb.Unchecked += tb_Unchecked;

                    unigrid.Children.Add(tb);
                }
         
            }
            catch (Exception)
            {
            }

            SetIOName();

            descTb.Text = _sbIOName.ToString();

        }

        private void SetIOName()
        {
            try
            {
                _sbIOName.AppendLine("Light RED : 0 ");
                _sbIOName.AppendLine("Light YELLOW : 1");
                _sbIOName.AppendLine("Light GREEN : 2");
                _sbIOName.AppendLine("Light BLUE : 3");
            }
            catch
            {

            }
        
        }

        void tb_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = (sender as ToggleButton).Content.ToString();

           
                Device.Relay.Off(id);
            }
            catch
            {

            }
        }

        private void tb_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = (sender as ToggleButton).Content.ToString();
              
                Device.Relay.On(id);
            }
            catch
            {

            }
            
        }
    }
}
