using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Renault_BT6.클래스
{
    public class PGLB
    {
        string foldername = "eolconfig";
        string filename = "eol_config.ini";
        string filepath;

        public string FolderName
        {
            get { return foldername; }
        }

        public string FilePath
        {
            get { return filepath; }
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public PGLB()
        {
            filepath = @"c:\" + foldername + "\\" + filename;
        }

        // config 설정을 추가하고 싶다면 항상 이 함수를 사용한다.
        // config 파일 경로는 고정적으로 c:\\config 폴더에 저장된다.
        // config 파일을 생성할 경로가 없다면 이 함수 내부에서 만들어주도록 CreateConfigFile을 호출한다.
        public void AddConfigData(string section, string key, string val)
        {
            // 무조건 호출한다.
            // 어차피 파일이 있으면 다시 생성하지 않고 false를 리턴한다.
            // 조건을 확인할 필요도 없다.
            CreateConfigFile();

            WritePrivateProfileString(section, key, val, filepath);
        }

        public void GetConfigData(string section, string key, string def, StringBuilder retVal, int size)
        {
            GetPrivateProfileString(section, key, def, retVal, size, filepath);
        }

        public bool CreateConfigFile()
        {
            string folderpath = @"c:\\" + foldername;
            DirectoryInfo dtif = new DirectoryInfo(folderpath);

            if (!dtif.Exists)
            {
                dtif.Create();
            }

            if (!File.Exists(filepath))
            {
                File.Create(filepath);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
