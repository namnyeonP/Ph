using System.Windows;

namespace Renault_BT6.윈도우
{
    /// <summary>
    /// AlarmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmWindow : Window
    {
        public AlarmWindow(string str)
        {
            InitializeComponent();
            reason.Content = "Check the terminal and currnet probe surface";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
