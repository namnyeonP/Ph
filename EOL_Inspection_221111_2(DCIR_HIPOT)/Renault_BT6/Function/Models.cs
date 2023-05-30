using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renault_BT6.클래스
{
    public class Models : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public Models()
        {
        }

        Dictionary<string, TestItem> testItemList = new Dictionary<string, TestItem>();

        public Dictionary<string, TestItem> TestItemList
        {
            get { return testItemList; }
            set
            {
                testItemList = value;
                OnPropertyChange("TestItemList");
            }
        }

        string prodId;

        public string ProductCode
        {
            get { return prodId; }
            set
            {
                if (prodId != value)
                {
                    prodId = value;
                    OnPropertyChange("ProdId");
                }
            }

        }
        string procId;

        public string ProcessID
        {
            get { return procId; }
            set
            {
                if (procId != value)
                {
                    procId = value;
                    OnPropertyChange("ProcId");
                }
            }
        }
        string equipId;

        public string EquipId
        {
            get { return equipId; }
            set
            {
                if (equipId != value)
                {
                    equipId = value;
                    OnPropertyChange("EquipId");
                }
            }
        }
        string userId;

        public string UserId
        {
            get { return userId; }
            set
            {
                if (userId != value)
                {
                    userId = value;
                    OnPropertyChange("UserId");
                }
            }
        }

        string lotId;

        public string LotId
        {
            get { return lotId; }
            set
            {
                if (lotId != value)
                {
                    lotId = value;
                    OnPropertyChange("LotId");
                }
            }
        }

        string modelId;

        public string ModelId
        {
            get { return modelId; }
            set
            {
                if (modelId != value)
                {
                    modelId = value;
                    OnPropertyChange("ModelId");
                }
            }
        }

        string modelDesc;

        public string ModelDesc
        {
            get { return modelDesc; }
            set
            {
                if (modelDesc != value)
                {
                    modelId = value;
                    OnPropertyChange("ModelDesc");
                }
            }
        }


        string number;

        public string Number
        {
            get { return number; }
            set
            {
                if (number != value)
                {
                    number = value;
                    OnPropertyChange("Number");
                }
            }
        }

        string data1;

        public string Data1
        {
            get { return data1; }
            set
            {
                if (data1 != value)
                {
                    data1 = value;
                    OnPropertyChange("Data1");
                }
            }
        }

        string data2;

        public string Data2
        {
            get { return data2; }
            set
            {
                if (data2 != value)
                {
                    data2 = value;
                    OnPropertyChange("Data2");
                }
            }
        }

    }
}
