using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE.클래스
{
    public class UserInfo: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string userID;
        public string UserID
        {
            get { return userID; }
            set
            {
                if (userID != value)
                {
                    userID = value;
                    OnPropertyChange("UserID");
                }
            }
        }
        string userPassWord;
        public string UserPassWord
        {
            get { return userPassWord; }
            set
            {
                if (userPassWord != value)
                {
                    userPassWord = value;
                    OnPropertyChange("UserPassWord");
                }
            }
        }
        int permission;
        public int Permission
        {
            get { return permission; }
            set
            {
                if (permission != value)
                {
                    permission = value;
                    OnPropertyChange("Permission");
                }
            }
        }
        string lastLoginDate;
        public string LastLoginDate
        {
            get { return lastLoginDate; }
            set
            {
                if (lastLoginDate != value)
                {
                    lastLoginDate = value;
                    OnPropertyChange("LastLoginDate");
                }
            }
        }
    }
}
