﻿using System;
using System.Windows.Forms;

namespace NorthwindTradersV3LinqToSql
{
    public partial class FrmNotaRemision0 : Form
    {

        public int Id;

        public FrmNotaRemision0()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmRptNotaRemision frmRptNotaRemision = new FrmRptNotaRemision();
            frmRptNotaRemision.Id = Id;
            frmRptNotaRemision.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FrmRptNotaRemision2 frmRptNotaRemision2 = new FrmRptNotaRemision2();
            frmRptNotaRemision2.Id = Id;
            frmRptNotaRemision2.ShowDialog();
            this.Close();
        }
    }
}
