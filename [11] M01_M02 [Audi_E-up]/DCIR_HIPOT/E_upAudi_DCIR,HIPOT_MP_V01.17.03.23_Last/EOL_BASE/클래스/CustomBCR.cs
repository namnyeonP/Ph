using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE.클래스
{
    public class CustomBCR
    {
        public CustomBCR()
        {

        }

        int modelType;
        int mode;
        string prodID;
        string bcr;

        public string BCR
        {
            get
            {
                return bcr;
            }
            set
            {
                bcr = value;
            }
        }

        public string ProdID
        {
            get
            {
                return prodID;
            }
            set
            {
                prodID = value;
            }
        }

        /// <summary>
        /// (1:Diagnosis / 2:Master )
        /// </summary>
        public int Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
            }
        }

        /// <summary>
        /// (1:Audi_N / 2:Audi_M / 3:Porsche_N / 4:Porsche_M / 5:Maserati_N / 6:Maserati_M / 7:E-UP)
        /// </summary>
        public int ModelType
        {
            get
            {
                return modelType;
            }
            set
            {
                modelType = value;
            }
        }
    }
}
