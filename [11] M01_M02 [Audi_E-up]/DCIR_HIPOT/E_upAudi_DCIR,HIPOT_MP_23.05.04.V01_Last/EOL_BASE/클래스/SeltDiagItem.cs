using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE.클래스
{
    public class SelfDiagItem
    {
        string header;

        public string Header
        {
            get { return header; }
            set { header = value; }
        }

        string fileAddress;

        public string FileAddress
        {
            get { return fileAddress; }
            set { fileAddress = value; }
        }
    }
}
