using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_BASE.모듈
{
    public class CRelay_Receiver
    {
        MainWindow mw;
        RegistryKey rk;
        public bool isAlive = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mw">로그를 찍기위한 메인윈도우와의 연결</param>
        public CRelay_Receiver(MainWindow mw)
        {
            this.mw = mw;
            var pro = System.Diagnostics.Process.GetProcessesByName("RelayController_Background");
            if (pro.Length > 0)
            {
                mw.LogState(LogType.Success, "RelayController_Background is Activated");
                isAlive = true;

                string regSubkey = "Software\\EOL_Trigger\\Relays";
                rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regSubkey);


                Reset();
                RelayOn("IDO_1");
            }
            else
            {
                mw.LogState(LogType.Fail, "RelayController_Background is NOT Activated");
            }

        }

        public bool RelayStatus(string id)
        {
            if (rk != null)
            {
                id = id.Replace("IDO_", "RELAY_");

                var data = rk.GetValue(id).ToString();

                return data == "0" ? false : true;
            }
            else
            {
                return false;

            }
        }
        /// <summary>
        /// 릴레이 ON
        /// </summary>
        /// <param name="id">ex) IDO_1</param>
        /// <returns></returns>
        public bool RelayOn(string id)
        {
            if (isAlive)
            {
                id = id.Replace("IDO_", "RELAY_");
                rk.SetValue(id, "1");
                Thread.Sleep(10);

                mw.LogState(LogType.Info, "CRelay ON - " + id);

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 모든 릴레이를 끈다.
        /// 2개이상의 프로그램이 동시에 뜨는 경우, 주석처리가 필요하다.
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            if (isAlive)
            {
                for (int i = 0; i < 64; i++)
                {
                    rk.SetValue("RELAY_" + i.ToString(), "0");
                }
                Thread.Sleep(50);

                mw.LogState(LogType.Info, "CRelay RESET");

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 릴레이 OFF
        /// </summary>
        /// <param name="id">ex) IDO_1</param>
        /// <returns></returns>
        public bool RelayOff(string id)
        {
            if (isAlive)
            {
                id = id.Replace("IDO_", "RELAY_");
                rk.SetValue(id, "0");
                Thread.Sleep(10);

                mw.LogState(LogType.Info, "CRelay OFF - " + id);

                return true;
            }
            else
                return false;
        }
    }
}
