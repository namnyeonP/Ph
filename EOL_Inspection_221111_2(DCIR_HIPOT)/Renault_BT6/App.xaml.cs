using System.Diagnostics;
using System.Windows;

namespace Renault_BT6
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App() : base()
        {
            // moons 중복 방지 실행 적용 20190627
            Process[] process = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (process.Length > 1)
            {
                MessageBox.Show("This is program already running !!", "EOL Inspection", MessageBoxButton.OK);
                this.Shutdown();
            }
        }
    }
}
