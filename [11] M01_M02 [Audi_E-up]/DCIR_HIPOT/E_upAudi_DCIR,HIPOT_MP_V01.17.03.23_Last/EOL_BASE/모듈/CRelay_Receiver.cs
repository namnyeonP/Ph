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

        //List<string> relayList = new List<string>();
        Dictionary<string, string> relayDic = new Dictionary<string, string>();

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
            }
            else
            {
                mw.LogState(LogType.Fail, "RelayController_Background is NOT Activated");
            }  

            SetRelayList();
        }

        /// <summary>
        /// 해당 메소드에 Relay 상수 선언된 것들을 Dictionary에 넣는다.
        /// </summary>
        private void SetRelayList()
        {
            // ex)
            relayDic.Add("IDO_0", "RAMP_#1_RED");
            relayDic.Add("IDO_1", "RAMP_#1_YELLOW");
            relayDic.Add("IDO_2", "RAMP_#1_GREEN");
            relayDic.Add("IDO_3", "RAMP_#1_BLUE");

            relayDic.Add("IDO_4", "RAMP_#2_RED");
            relayDic.Add("IDO_5", "RAMP_#2_YELLOW");
            relayDic.Add("IDO_6", "RAMP_#2_GREEN");
            relayDic.Add("IDO_7", "RAMP_#2_BLUE");

            relayDic.Add("IDO_8", "RAMP_#3_RED");
            relayDic.Add("IDO_9", "RAMP_#3_YELLOW");
            relayDic.Add("IDO_10", "RAMP_#3_GREEN");
            relayDic.Add("IDO_11", "RAMP_#3_BLUE");
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
                string relay_name = "";

                if (relayDic.ContainsKey(id))
                {
                    relayDic.TryGetValue(id, out relay_name);
                }

                id = id.Replace("IDO_", "RELAY_");

                rk.SetValue(id, "1");
                Thread.Sleep(10);

                mw.LogState(LogType.Info, string.Format("CRelay ON - {0}, {1}", id, relay_name));

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
                string relay_name = "";

                if (relayDic.ContainsKey(id))
                {
                    relayDic.TryGetValue(id, out relay_name);
                }

                id = id.Replace("IDO_", "RELAY_");

                rk.SetValue(id, "0");
                Thread.Sleep(10);

                mw.LogState(LogType.Info, string.Format("CRelay OFF - {0}, {1}", id, relay_name));

                return true;
            }
            else
                return false;
        }
    }
}
