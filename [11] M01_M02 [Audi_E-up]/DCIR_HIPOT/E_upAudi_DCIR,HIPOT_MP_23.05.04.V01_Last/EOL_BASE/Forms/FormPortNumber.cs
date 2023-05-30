using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using EOL_BASE.모듈;
using EOL_BASE.윈도우;
using EOL_BASE.클래스;
using Microsoft.Win32;

namespace EOL_BASE.Forms
{
    public partial class FormPortNumber : Form
    {
        public FormPortNumber(MainWindow mw)
        {
            InitializeComponent();

            this.Text = "Channel " + mw.position;
        }
    }
}
