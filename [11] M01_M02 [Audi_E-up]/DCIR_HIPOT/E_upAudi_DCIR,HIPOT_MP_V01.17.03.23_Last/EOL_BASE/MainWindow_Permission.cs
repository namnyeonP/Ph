using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EOL_BASE
{
    //outer_loginGrd
    //idTb
    //pwTb
    //inner_loginGrd
    //loginLb
    //AdminTogBt
    //MainTogBt
    //OperTogBt
    //lastDateLb : 2021-01-11 21:32:44
    public partial class MainWindow
    {

        /// <summary>
        /// 3 : administor 
        /// 2 : maintenence
        /// 1 : operator
        /// 0 : logout
        /// </summary>
        public int commonPermission = 0;

        public Dictionary<string, int> Permissions = new Dictionary<string, int>();

        public bool PermissionResult = false;

        private void InitializePermissionList()
        {
            Permissions.Add("PermissionAddModify", 2);
            Permissions.Add("SingleItemButton", 3);
            Permissions.Add("GroupItemButton", 2);
            Permissions.Add("ModelButton", 2);
            Permissions.Add("Auto_Manual", 2);
            Permissions.Add("StartAuto", 2);
            Permissions.Add("Pause", 2);
            Permissions.Add("EMS", 0);
            Permissions.Add("RESET", 2);
            Permissions.Add("OpenRSTFolder", 1);
            Permissions.Add("OpenLogFolder", 1);
            Permissions.Add("OpenEOLLIST", 2);
            Permissions.Add("OpenCtrllist", 2);
            Permissions.Add("OpenModelList", 2);
            Permissions.Add("OpenBCRList", 2);
            Permissions.Add("RelayControl", 2);
            Permissions.Add("CyclerSetting", 2);
            Permissions.Add("ShowDAQ", 2);
            Permissions.Add("NGSkip", 2);
            Permissions.Add("MESSkip", 2);
            Permissions.Add("PREDICT", 3);
            Permissions.Add("MASTER", 3);
            Permissions.Add("EXIT", 1);
            Permissions.Add("SAVE_DATA", 2);
        }

        public enum PERMISSION
        {
            PermissionAddModify = 1,
            SingleItemButton = 2,
            GroupItemButton = 3,
            ModelButton = 4,
            Auto_Manual = 5,
            StartAuto = 6,
            Pause = 7,
            EMS = 8,
            RESET = 9,
            OpenRSTFolder = 10,
            OpenLogFolder = 11,
            OpenEOLLIST = 12,
            OpenCtrllist = 13,
            OpenModelList = 14,
            OpenBCRList = 15,
            RelayControl = 16,
            CyclerSetting = 17,
            ShowDAQ = 18,
            NGSkip = 19,
            MESSkip = 20,
            PREDICT = 21,
            MASTER = 22,
            EXIT = 23,
            SAVE_DATA = 24,
        }

        public List<UserInfo> userInfoList = new List<UserInfo>();

        public UserInfo loginUser;


        private bool CheckPermission(PERMISSION per)
        {
            if(PermissionResult)
            {
                return true;
                //int target = Permissions[per.ToString()];
                //if (commonPermission >= target)
                if(true)
                {
                    if (loginUser == null)
                    {
                        LogState(LogType.PERMISSION, string.Format("Login required - Success [{0}]", per.ToString()), null, false);
                    }
                    else
                    {
                        LogState(LogType.PERMISSION, string.Format("Check Permission - Success {0} / LEVEL:{1} / [{2}]",
                            loginUser.UserID,
                            loginUser.Permission.ToString(),
                            per.ToString()), null, false);
                        LogoutTime = DateTime.Now;
                    }
                    return true;
                }
                //else
                //{
                //    if (loginUser == null)
                //    {
                //        LogState(LogType.PERMISSION, string.Format("Login required - Fail [{0}]", per.ToString()), null, false);
                //    }
                //    else
                //    {
                //        LogState(LogType.PERMISSION, string.Format("Check Permission - Fail {0} / LEVEL:{1} / [{2}]",
                //            loginUser.UserID,
                //            loginUser.Permission.ToString(),
                //            per.ToString()), null, false);
                //    }
                //    return false;
                //}
            }
            else
            {
                return false;
            }

        }

        private void PermissionError()
        {
            MessageBox.Show("You don't have permission.\nNeed an additional authorization.", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
        }


        public void logoutSeq()
        {
            loginUser.LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            LogState(LogType.PERMISSION, string.Format("Logout success : {0},LEVEL:{1},{2}",
                loginUser.UserID,
                loginUser.Permission.ToString(),
                loginUser.LastLoginDate), null, false);
            SetPermissionFile();

        }


        public void GetPermissionFile()
        {
            userInfoList.Clear();
            try
            {
                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");

                FileStream readData = new FileStream(@"C:\info.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader streamReader = new StreamReader(readData, encode);

                //파일에서 ID와 값을 가져온다
                List<string> ctrlItems = new List<string>();
                while (streamReader.Peek() > -1)
                {
                    string[] arr = streamReader.ReadLine().Split(',');
                    var id = arr[0];
                    var pw = arr[1];
                    var per = arr[2];
                    var last = arr[3];

                    userInfoList.Add(new UserInfo() { UserID = id, UserPassWord = pw, Permission = int.Parse(per), LastLoginDate = last });
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.PERMISSION, "GetPermissionFile", ex);
            }
        }

        public void SetPermissionFile()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (StreamWriter sw = new StreamWriter(@"C:\info.txt", false))
                {
                    foreach (var user in userInfoList)
                    {
                        sb.Append(user.UserID + "," + user.UserPassWord + "," + user.Permission.ToString() + "," + user.LastLoginDate + "\n");

                    }
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                LogState(LogType.PERMISSION, "SetPermissionFile", ex);
            }
        }


        private void loginLb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //var rp = new RequirePasswordWindow(this);
            //rp.ShowDialog();
            //if (rp.isOK)
            //{
            var ins = new InspectionSpecWindow(this);
            ins.Show();
            //}
            //else
            //{
            //    MessageBox.Show("Not Matched Password", "Info", MessageBoxButton.OK, MessageBoxImage.Stop);
            //}
            //rp.Close();
        }


        Thread logoutTh;

        /// <summary>
        /// 로그인직후 몇분 기다릴껀지 변수
        /// </summary>
        int logoutAfterDelayMinute = 1;

        DateTime logoutTime;

        /// <summary>
        /// 값을 넣을때 자동으로 추가해줌
        /// </summary>
        public DateTime LogoutTime
        {
            get
            { return logoutTime; }
            set
            {
                logoutTime = value;
                logoutTime = logoutTime.AddMinutes(logoutAfterDelayMinute);
            }
        }

        public bool b_isExit = false;
        private void AutoLogoutFunc()
        {
            if (logoutTh == null)
            {
                logoutTh = new Thread(() =>
                {
                    LogoutTime = DateTime.Now;

                    while (true)
                    {
                        Thread.Sleep(1000);

                        //설정시간보다 현재시간이 커지면 

                        if (loginUser != null)
                        {
                            if (LogoutTime < DateTime.Now)
                            {
                                LogState(LogType.PERMISSION, string.Format("Logout by Timer(+{0} minute) : {1},LEVEL:{2},{3}",
                                    logoutAfterDelayMinute.ToString(),
                                    loginUser.UserID,
                                    loginUser.Permission.ToString(),
                                    loginUser.LastLoginDate));
                                lg.Logout(this, null);
                            }
                        }

                        if (b_isExit == true)
                        {
                            break;
                        }
                    }
                });
                logoutTh.Start();
            }
        }
    }
}
