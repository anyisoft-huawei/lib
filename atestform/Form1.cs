using lib.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using lib.file;

namespace atestform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 用于处理进度消息并通知UI
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="len"></param>
        /// <param name="lened"></param>
        void Rev(string msg, long len, long lened)
        {
            BeginInvoke(new Action(() => {
                lb_msg.Text = string.Format("[{0}/{1}]{2}", lened, len, msg);            
            }));
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

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (var o = new OpenFileDialog())
                {
                    if (o.ShowDialog() == DialogResult.OK)
                    {
                        new Thread(() => {
                            try
                            {
                                var list = new List<string>() { o.FileName };
                                lib.zip.ZipHelper.ZipCompress(list, "1", 6, Rev);//压缩测试
                            }
                            catch (Exception ex)
                            {
                                Rev(ex.Message, 0, 0);
                            }
                        }).Start();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                using (var o = new OpenFileDialog())
                {
                    if (o.ShowDialog() == DialogResult.OK)
                    {
                        lib.zip.ZipHelper.ZipDeCompress(o.FileName, @"D:\Code\out\adomeform\zip", Rev);//解压测试
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //var val = lib.zip.ZipHelper.ZipCompress("什么鬼什么鬼啊啊啊啊啊是啊大大大");//字符串测试
                //val = lib.zip.ZipHelper.ZipDeCompress(val);
                using (var o = new OpenFileDialog())
                {
                    if (o.ShowDialog() == DialogResult.OK)
                    {
                        var bs = System.IO.File.ReadAllBytes(o.FileName);
                        var val = Encoding.UTF8.GetString(lib.zip.ZipHelper.GZipDeCompress(bs));

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                using (var o = new OpenFileDialog())
                {
                    if (o.ShowDialog() == DialogResult.OK)
                    {
                        using (var bs = File.OpenRead(o.FileName))
                        {

                     
                        }
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
