using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE.클래스
{
    public class DetailItems
    {
        string key;
        List<object> reportitems = new List<object>();

        public List<object> Reportitems
        {
            get { return reportitems; }
            set { reportitems = value; }
        }


        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                if (key != "")
                {
                    int.TryParse(key.Substring(key.Length-4, 4), out order);
                }
            }
        }

        public int order;
    }
}
