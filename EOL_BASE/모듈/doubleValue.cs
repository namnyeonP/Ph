using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE.모듈
{

    public class doubleValue : INotifyPropertyChanged
    {
        public doubleValue()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string name;
        double value_;

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
        public double Value_
        {
            get { return value_; }
            set
            {
                if (value_ != value)
                {
                    value_ = value;
                    OnPropertyChange("Value_");
                }
            }
        }
    }
}
