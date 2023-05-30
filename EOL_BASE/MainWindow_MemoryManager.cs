using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE
{
    public partial class MainWindow
    {
        #region 강제 메모리 회수

        public void MemoryRefresh()
        {
            var privateMemorySize64 = System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 * 0.000001;
            var workingset64 = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 * 0.000001;
            var gcMemory = GC.GetTotalMemory(true) * 0.000001;
            //if (workingset64 > 90)
            {
                FlushMemory();
            }
        }

        [DllImportAttribute("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);
        public static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }

        }

        #endregion


    }
}
