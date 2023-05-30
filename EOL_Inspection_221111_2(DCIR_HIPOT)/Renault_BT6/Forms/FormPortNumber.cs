using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Renault_BT6.모듈;
using Renault_BT6.윈도우;
using Renault_BT6.클래스;
using Microsoft.Win32;

namespace Renault_BT6.Forms
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
