using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Renault_BT6.클래스
{
    public class TestItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public TestItem()
        {
        }
        string _digitLength;

        public string DigitLength
        {
            get { return _digitLength; }
            set { _digitLength = value; }
        }

        private string methodName;
        private string groupMethodName;

        public string GroupMethodName
        {
            get { return groupMethodName; }
            set { groupMethodName = value; }
        }

        public string SingleMethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        private string groupName;

        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }
        private Button bt;

        public Button Bt
        {
            get { return bt; }
            set
            {
                bt = value;
                this.btname = bt.Content.ToString();
            }
        }
        int no;
        string name;
        object min;
        object max;
        object value_;
        List<object> refvalues_ = new List<object>();
        string result;
        public string btname;

        public List<TestItem> tempIdList = new List<TestItem>();

        public string Result
        {
            get { return result; }
            set
            {
                if (result != value)
                {
                    result = value;
                    OnPropertyChange("Result");
                }
            }
        }

        public object Value_
        {
            get { return this.value_; }
            set
            {
                if (value_ != value)
                {
                    value_ = value;
                    OnPropertyChange("Value_");
                }
            }
        }

        public List<object> refValues_
        {
            get { return this.refvalues_; }
            set
            {
                if (refvalues_ != value)
                {
                    refvalues_ = value;
                    OnPropertyChange("refValues_");
                }
            }
        }

        public object Max
        {
            get { return max; }
            set
            {
                if (max != value)
                {
                    max = value;
                    OnPropertyChange("Max");
                }
            }
        }

        public object Min
        {
            get { return min; }
            set
            {
                if (min != value)
                {
                    min = value;
                    OnPropertyChange("Min");
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChange("Name");
                }
            }
        }

        string clctitem;
        public string CLCTITEM
        {
            get { return clctitem; }
            set
            {
                if (name != value)
                {
                    clctitem = value;
                    OnPropertyChange("CLCTITEM");
                }
            }
        }

        public int No
        {
            get { return no; }
            set
            {
                if (no != value)
                {
                    no = value;
                    OnPropertyChange("No");
                }
            }
        }

        string unit;

        public string Unit
        {
            get { return unit; }
            set
            {
                if (unit != value)
                {
                    unit = value;
                    OnPropertyChange("Unit");
                }
            }
        }
    }
}
