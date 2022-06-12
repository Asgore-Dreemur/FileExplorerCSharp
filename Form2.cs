using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;

namespace FileExplorerCSharp
{
    public partial class Form2 : Form
    {
        string cpfilepath;
        bool isdict;
        public Form2()
        {
            InitializeComponent();
            cpfilepath = Form1.cpath;
            isdict = Form1.isdict;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string rname = this.textBox1.Text;
            string curpath = Form1.cpath2;
            try
            {
                Computer cp = new Computer();
                if (isdict)
                {
                    cp.FileSystem.RenameDirectory(cpfilepath, rname);
                }
                else
                {
                    cp.FileSystem.RenameFile(cpfilepath, rname);
                }
                MessageBox.Show("操作成功完成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("错误:" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
