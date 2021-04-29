﻿using lib.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace atestform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PDFHelper pdf;

        private void btn_test_Click(object sender, EventArgs e)
        {
            try
            {
                using (var o = new OpenFileDialog())
                {
                    o.Filter = "PDF文件|*.pdf";
                    if(o.ShowDialog() == DialogResult.OK)
                    {
                        pdf = new PDFHelper();
                        pdf.Read(o.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}