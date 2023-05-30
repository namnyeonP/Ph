using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE.모듈
{
    public class CNhtRS232_Receiver
    {
        MainWindow mw;

        public bool isAlive = false;
        RegistryKey rk;
        System.Windows.Forms.Timer ti;
        Thread tickThread;
        /// <summary>
        /// port=COM3 / baud=9600 / parity=none / data=8 / stop=1
        /// </summary>
        /// <param name = "mw" > 로그를 찍기위한 메인윈도우와의 연결</param>
        public CNhtRS232_Receiver(MainWindow mw)
        {
            this.mw = mw;
            this.mw = mw;
            var pro = System.Diagnostics.Process.GetProcessesByName("RelayController_Background");
            if (pro.Length > 0)
            {
                mw.LogState(LogType.Success, "RelayController_Background is Activated");
                isAlive = true;

                string regSubkey = "Software\\EOL_Trigger\\Relays";
                rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);

                tickThread = new Thread(() =>
                {
                    while (true)
                    {
                        var str = rk.GetValue("NHT_RS232_TEMP").ToString();
                        float.TryParse(str, out tempStr);
                        mw.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mw.templb.Content = str;
                        }));
                        localDt1 = DateTime.Now;
                        Thread.Sleep(500);
                    }
                });
                tickThread.Start();
                //ti = new System.Windows.Forms.Timer();
                //ti.Interval = 100;
                //ti.Tick += ti_Tick;
                //ti.Start();
            }
            else
            {
                mw.LogState(LogType.Fail, "RelayController_Background is NOT Activated");
            }


        }
        public DateTime localDt1 = new DateTime();
        void ti_Tick(object sender, EventArgs e)
        {
            if (mw.position == "#1")
            {
                var str = rk.GetValue("NHT_RS232_TEMP1").ToString();
                float.TryParse(str, out tempStr);
                mw.templb.Content = str;
            }
            else if (mw.position == "#2")
            {
                var str = rk.GetValue("NHT_RS232_TEMP2").ToString();
                float.TryParse(str, out tempStr);
                mw.templb.Content = str;
            }
            
        }

        public float tempStr = 0.0f;

        Object obj = new Object();
    }
}
