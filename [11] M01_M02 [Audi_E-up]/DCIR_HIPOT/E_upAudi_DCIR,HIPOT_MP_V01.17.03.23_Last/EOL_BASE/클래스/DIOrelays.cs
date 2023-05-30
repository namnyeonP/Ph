using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE.클래스
{
    public class DIOrelays
    {
        Dictionary<string, string> relayDic = null;
        private int relayCnt = 31;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public DIOrelays()
        {
            relayDic = new Dictionary<string, string>();
        }

        public void Dispose()
        {
            relayDic.Clear();
        }

        public Dictionary<string, string> LoadRelays()
        {
            // load config file.
            string path = AppDomain.CurrentDomain.BaseDirectory + "config.ini";
            StringBuilder sb = null;

            if (System.IO.File.Exists(path))
            {
                for (int i = 0; i < relayCnt; i++)
                {
                    string ido = string.Format("IDO_{0}", i);
                    sb = new StringBuilder();
                    GetPrivateProfileString("Relays", ido, "", sb, 255, path);
                    relayDic.Add(ido, sb.ToString());
                }
            }

            return relayDic;
        }
    }
}
