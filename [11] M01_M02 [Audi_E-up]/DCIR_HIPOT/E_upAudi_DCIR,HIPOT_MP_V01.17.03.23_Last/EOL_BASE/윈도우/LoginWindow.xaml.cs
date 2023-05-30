using System;
using System.Collections.Generic;
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
using System.Windows.Interop;

namespace EOL_BASE.윈도우
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmExtendFrameIntoClientArea(System.IntPtr hWnd, ref Margins pMargins);

        public static bool ActiveAeroGlassEffect(Window win)
        {
            if (!DwmIsCompositionEnabled())
                return false;

            IntPtr hwnd = new WindowInteropHelper(win).Handle;

            if (hwnd == IntPtr.Zero)
                throw new InvalidProgramException("asdf");

            win.Background = Brushes.Transparent;

            HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);

            var margins = new Margins
            {
                cxLeftWidth = 0,
                cxRightWidth = Convert.ToInt32(win.Width) * Convert.ToInt32(win.Width),
                cyTopHeight = 0,
                cyBottomHeight = Convert.ToInt32(win.Height) * Convert.ToInt32(win.Height)
            };

            DwmExtendFrameIntoClientArea(hwnd, ref margins);

            return true;
        }

        MainWindow mw;
        public LoginWindow(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;
            this.KeyDown += LoginWindow_KeyDown;
            this.Loaded += LoginWindow_Loaded; 
            //this.SourceInitialized += new EventHandler(EventHandled);
        }

        private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter: Login(this, null); break;
                case Key.Escape:this.Hide();break;
            }
        }

        private void EventHandled(object sender, EventArgs e)
        {
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            idTb.Focus();
        }

        private void Exit(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }

        private void Login(object sender, MouseButtonEventArgs e)
        {
            if (idTb.Text == "" || pwTb.Password.ToString() == "")
            {
                mw.LogState(LogType.PERMISSION, string.Format("Login Fail : empty ID / PW"), null, false);
                MessageBox.Show("Please check the login Parameters.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            mw.GetPermissionFile();

            var id = idTb.Text;
            var pw = pwTb.Password.ToString();

            var user = mw.userInfoList.FirstOrDefault(x => x.UserID == id);

            if (user != null)
            {
                if (user.UserPassWord == pw)
                {

                     //< ToggleButton Visibility = "Collapsed" x: Name = "AdminTogBt" Grid.Row = "1" IsHitTestVisible = "False" Margin = "1" Content = "ADMINISTOR" FontSize = "12"
                     //                     BorderThickness = "1" BorderBrush = "Black"
                     //                     Background = "Green"
                     //                     Foreground = "White"  FontWeight = "Bold" Grid.ColumnSpan = "3" />
    
                     //   < ToggleButton Visibility = "Collapsed" x: Name = "MainTogBt" Grid.Row = "1" IsHitTestVisible = "False" Margin = "1" Content = "MAINTENANCE" FontSize = "12"
                     //                     BorderThickness = "1" BorderBrush = "Black"
                     //                     Background = "Orange"
                     //                     Foreground = "White"  FontWeight = "Bold" Grid.ColumnSpan = "3" />
    
                     //   < ToggleButton Visibility = "Collapsed" x: Name = "OperTogBt" Grid.Row = "1" IsHitTestVisible = "False" Margin = "1" Content = "OPERATOR" FontSize = "12"
                     //                     BorderThickness = "1" BorderBrush = "Black"
                     //                     Background = "Yellow"
                     //                     Foreground = "Black"  FontWeight = "Bold" Grid.ColumnSpan = "3" />


                    mw.commonPermission = user.Permission;
                    switch (mw.commonPermission)
                    {
                        case 1: this.permissionBd.Background = Brushes.SkyBlue; this.permissionLb.Content = "OPERATOR";this.permissionLb.Foreground = Brushes.White; break;
                        case 2: this.permissionBd.Background = Brushes.Red; this.permissionLb.Content = "MAINTENANCE"; this.permissionLb.Foreground = Brushes.White; break;
                        case 3: this.permissionBd.Background = Brushes.Green; this.permissionLb.Content = "ADMINISTOR"; this.permissionLb.Foreground = Brushes.White; break;
                    }
                    loginLb.Content = user.UserID;
                    outer_loginGrd.Visibility = Visibility.Collapsed;
                    inner_loginGrd.Visibility = Visibility.Visible;
                    lastDateLb.Content = user.LastLoginDate;

                    mw.LogoutTime = DateTime.Now;

                    mw.loginUser = user;

                    mw.LogState(LogType.PERMISSION, string.Format("Login Success : {0} / LEVEL:{1}", mw.loginUser.UserID, mw.loginUser.Permission.ToString()), null, false);
                }
                else
                {
                    mw.LogState(LogType.PERMISSION, string.Format("Login Fail : Wrong PW"), null, false);
                    MessageBox.Show("Please check the Password.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

            }
            else
            {
                mw.LogState(LogType.PERMISSION, string.Format("Login Fail : Wrong ID"), null, false);
                MessageBox.Show("Please check the ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }
        }

        public void Logout(object sender, RoutedEventArgs e)
        {
            mw.logoutSeq();

            if (e != null)
            {
                MessageBox.Show("User Logout.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            mw.Dispatcher.BeginInvoke(new Action(() =>
            {
                InitializeUserLogin();
            }));
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            var ins = new InspectionSpecWindow(mw);
            ins.Show();
        }

        public void InitializeUserLogin()
        {
            mw.commonPermission = 0;
            outer_loginGrd.Visibility = Visibility.Visible;
            inner_loginGrd.Visibility = Visibility.Collapsed;

            this.permissionBd.Background = Brushes.LightGray; 
            this.permissionLb.Content = ""; 
            this.permissionLb.Foreground = Brushes.Black; 

            mw.loginUser = null;
            idTb.Text = "";
            pwTb.Password = "";
        }
    }
}
