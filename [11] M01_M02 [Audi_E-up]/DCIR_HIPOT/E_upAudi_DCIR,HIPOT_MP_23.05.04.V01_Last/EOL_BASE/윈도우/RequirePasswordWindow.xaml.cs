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
    /// RequirePasswordWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RequirePasswordWindow : Window
    {
        public bool isOK = false;
        public bool isESC = false;
        MainWindow mw;
        public RequirePasswordWindow(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;
            this.letterCntLb.Content = mw.passWord.Length.ToString();
            this.KeyUp += RequirePasswordWindow_KeyUp;
            reason.Focus();
        }

        void RequirePasswordWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(this, null);
            }
            else if (e.Key == Key.Escape)
            {
                isESC = true;
                Button_Click1(this, null);
            }
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (reason.Password.ToString() == mw.passWord)
                isOK = true;

            this.Hide();
        }
    }
}
