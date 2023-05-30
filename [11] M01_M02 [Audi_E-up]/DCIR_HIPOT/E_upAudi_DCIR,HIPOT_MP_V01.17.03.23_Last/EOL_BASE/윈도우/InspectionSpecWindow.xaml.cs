using EOL_BASE.클래스;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// InspectionSpecWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InspectionSpecWindow : Window
    {
        MainWindow mw;
        public InspectionSpecWindow()
        {
            InitializeComponent();

        }
        public InspectionSpecWindow(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;
            this.Loaded += InspectionSpecWindow_Loaded;
        }
        List<UserInfo> newlist,oldlist;

        void InspectionSpecWindow_Loaded(object sender, RoutedEventArgs e)
        {
            newlist = new List<UserInfo>();
            oldlist = new List<UserInfo>();

            if (mw.commonPermission < 3)
            {
                foreach (var user in mw.userInfoList)
                {
                    if (user.UserID == mw.loginUser.UserID)
                    {
                        //본인 포함 자기보다 권한 낮은넘들이 다 보인다
                        newlist.Add(user);
                    }
                    else
                    {
                        oldlist.Add(user);
                    }
                }
            }
            else
            {
                foreach (var user in mw.userInfoList)
                {
                    newlist.Add(user);
                }
            }
            this.testSpecDg.ItemsSource = newlist;
        }
         
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            var allnewList = new List<UserInfo>();

            foreach(var item in oldlist)
            {
                allnewList.Add(item);
            }

            foreach (var item in newlist)
            {
                allnewList.Add(item);
            }

            mw.userInfoList.Clear();

            foreach(var usr in allnewList)
            {
                mw.userInfoList.Add(usr);
            }

            mw.SetPermissionFile();

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void testSpecDg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.testSpecDg.SelectedItems.Count < 1)
                return;

            var user = this.testSpecDg.SelectedItems[0] as UserInfo;

            idTb.Text = user.UserID;
            pwTb.Text = user.UserPassWord;
            perCb.SelectedIndex = user.Permission-1;
            //stb.Text = p.Key;
            //maxtb.Text = p.Value.Max.ToString();
            //mintb.Text = p.Value.Min.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (idTb.Text != "" && pwTb.Text != "")
            {                
                try
                {
                    if (isAdd.IsChecked == true)
                    {
                        var usr = newlist.FirstOrDefault(x => x.UserID == idTb.Text);
                        if (usr != null)
                        {
                            MessageBox.Show("Already exist ID.\nPlease use an another ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }

                        var usr1 = oldlist.FirstOrDefault(x => x.UserID == idTb.Text);
                        if (usr1 != null)
                        {
                            MessageBox.Show("Already exist ID.\nPlease use an another ID.(another Permission)", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }

                        if(mw.commonPermission < perCb.SelectedIndex + 1)
                        {
                            MessageBox.Show("You don't have permission.\nNeed an additional authorization.", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }


                        newlist.Add(new UserInfo() { UserID = idTb.Text, UserPassWord = pwTb.Text, Permission = perCb.SelectedIndex + 1 });

                        this.testSpecDg.Items.Refresh();

                        mw.LogState(LogType.PERMISSION, string.Format("New user :{0} / LEVEL:{1}", idTb.Text, (perCb.SelectedIndex + 1).ToString()));

                    }
                    else if (isMod.IsChecked == true)
                    {
                        var usr = newlist.FirstOrDefault(x => x.UserID == idTb.Text);
                        if (usr != null)
                        {
                            if (mw.commonPermission < perCb.SelectedIndex + 1)
                            {
                                MessageBox.Show("You don't have permission.\nNeed an additional authorization.", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                                return;
                            }

                            usr.UserPassWord = pwTb.Text;
                            usr.Permission = perCb.SelectedIndex + 1;

                            mw.LogState(LogType.PERMISSION, string.Format("Modified user :{0} / LEVEL:{1}", idTb.Text, (perCb.SelectedIndex + 1).ToString()));
                        }
                        else
                        {
                            MessageBox.Show("Something is Wrong!!", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                    }
                    else if(isRem.IsChecked == true)
                    {
                        var usr = newlist.FirstOrDefault(x => x.UserID == idTb.Text);
                        if (usr != null)
                        {
                            newlist.Remove(usr);

                            this.testSpecDg.Items.Refresh();

                            mw.LogState(LogType.PERMISSION, string.Format("Removed user :{0} / LEVEL:{1}", idTb.Text, (perCb.SelectedIndex + 1).ToString()));
                        }
                        else
                        {
                            MessageBox.Show("Something is Wrong!!", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                    }
                }
                catch (Exception ec)
                {
                    mw.LogState(LogType.Fail, "Inspection Spec Change", ec);
                }
            }
        }
         

        // 2018.11.01 jeonhj's comment
        // 외기 온도에 따른 기준 저항값 변경을 위해 추가 윈도우 생성
        private void btnAmbTempSet_Click(object sender, RoutedEventArgs e)
        {
            AmbientTemperatureSpecWindow atsw = new AmbientTemperatureSpecWindow();

            atsw.ShowDialog();
        }
    }
}
