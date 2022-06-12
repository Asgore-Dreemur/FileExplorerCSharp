using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ShellTestApp;

namespace FileExplorerCSharp
{
    
    public partial class Form1 : Form
    {
        public static Form1 fr1;
        public string curpath;
        public static string cpath, cpath2;
        public static bool isdict;
        AutoSizeFormClass asc = new AutoSizeFormClass();
        Dictionary<string, string> drivemap = new Dictionary<string, string>();
        List<string> filelist = new List<string>();
        List<string> dictlist = new List<string>();
        List<string> nexttimeline = new List<string>();
        List<string> agotimeline = new List<string>();
        int agoid = 0, nextid = 0;
        public Form1()
        {
            InitializeComponent();
            asc.controllInitializeSize(this);
            GetFileList(".");
            GetDrive();
        }

        private void listView1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                if (this.listView1.SelectedItems.Count > 0)
                {
                    string fpath = this.listView1.SelectedItems[0].Text;
                    ShellContextMenu scm = new ShellContextMenu();
                    DirectoryInfo[] dir = new DirectoryInfo[1];
                    string ffpath = curpath + "\\" + fpath;
                    dir[0] = new DirectoryInfo(ffpath);
                    scm.ShowContextMenu(dir, this.PointToScreen(new Point(e.X, e.Y)));
                }
                else
                {
                    string fpath = ".";
                    ShellContextMenu scm = new ShellContextMenu();
                    DirectoryInfo[] dir = new DirectoryInfo[1];
                    dir[0] = new DirectoryInfo(fpath);
                    scm.ShowContextMenu(dir, this.PointToScreen(new Point(e.X, e.Y)));
                }
            }
            else if(e.Button == MouseButtons.Left)
            {
                this.menuStrip1.Items[1].Enabled = true;
            }
        }

        private void Form1_AutoSizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }
        private void GetFileList(string path)
        {
            
                this.Cursor = Cursors.WaitCursor;
                DirectoryInfo directory = new DirectoryInfo(path);
                FileInfo[] flist = directory.GetFiles();
                DirectoryInfo[] dirinfo = directory.GetDirectories();
                this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
                this.imageList1.ColorDepth = ColorDepth.Depth32Bit;
                List<ListViewItem> itemlist = new List<ListViewItem>();
                foreach (DirectoryInfo dinfo in dirinfo)
                {
                    ListViewItem item = new ListViewItem(dinfo.Name);
                    SHGFI.SHFILEINFO shfi = new SHGFI.SHFILEINFO();
                    SHGFI.SHGetFileInfo(dinfo.FullName,
                        0,
                        ref shfi,
                        (uint)Marshal.SizeOf(shfi),
                        SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON);
                    this.imageList1.Images.Add(Icon.FromHandle(shfi.hIcon).Clone() as Icon);
                    int imgIndex = imageList1.Images.Count - 1;
                    string typename = dinfo.GetType().Namespace;
                    DateTime dt = dinfo.LastWriteTime;
                    string wdate = "";
                    wdate += dt.Year.ToString();
                    wdate += "/";
                    wdate += dt.Month.ToString();
                    wdate += "/";
                    wdate += dt.Day.ToString();
                    wdate += " ";
                    wdate += dt.Hour.ToString();
                    wdate += ":";
                    wdate += dt.Minute.ToString();
                    item.SubItems.Add(wdate);
                    item.SubItems.Add("文件夹");
                    item.ImageIndex = imgIndex;
                    itemlist.Add(item);
                    if (dictlist.Contains(dinfo.Name)) dictlist.Remove(dinfo.Name);
                    dictlist.Add(dinfo.Name);
                }
                foreach (FileInfo f in flist)
                {
                    ListViewItem item = new ListViewItem(f.Name);
                    imageList1.Images.Add(SHGFI.GetFileIcon(f.Name, false));
                    int imgIndex = imageList1.Images.Count - 1;
                    SHGFI.SHFILEINFO shfi = new SHGFI.SHFILEINFO();
                    SHGFI.SHGetFileInfo(f.FullName,
                        0,
                        ref shfi,
                        (uint)Marshal.SizeOf(shfi),
                        SHGFI.SHGFI_USEFILEATTRIBUTES | SHGFI.SHGFI_TYPENAME | SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON);
                    DateTime dt = f.LastWriteTime;
                    string wdate = "";
                    wdate += dt.Year.ToString();
                    wdate += "/";
                    wdate += dt.Month.ToString();
                    wdate += "/";
                    wdate += dt.Day.ToString();
                    wdate += " ";
                    wdate += dt.Hour.ToString();
                    wdate += ":";
                    wdate += dt.Minute.ToString();
                    int fsize = (int)f.Length;
                    int fcsize = (int)Math.Ceiling((double)fsize/1024);
                    string filesize = fcsize.ToString() + "KB";
                    item.SubItems.Add(wdate);
                    item.SubItems.Add(shfi.szTypeName);
                    item.SubItems.Add(filesize);
                    item.ImageIndex = imgIndex;
                    itemlist.Add(item);
                    if (filelist.Contains(f.Name)) dictlist.Remove(f.Name);
                    filelist.Add(f.Name);
                }
                foreach (ListViewItem item in itemlist)
                {
                    this.listView1.Items.Add(item);
                }
                curpath = Path.GetFullPath(path);
                this.textBox1.Text = curpath;
                this.Cursor = Cursors.Default;
            
            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {
                string cpath = this.textBox1.Text;
                this.listView1.Items.Clear();
                GetFileList(cpath);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string path = curpath;
            string upath = "";
            string[] lpath = path.Split('\\');
            if (lpath.Length == 2 && lpath[1] == "") return;
            for(int i=0;i<lpath.Length - 1; i++)
            {
                upath += lpath.GetValue(i);
                if(i < lpath.Length - 2)
                {
                    upath += "\\";
                }
            }
            string astr = curpath;
            this.listView1.Items.Clear();
            GetFileList(upath);
            if (agoid == 0)
            {
                this.button1.Enabled = true;
                agotimeline.Add(astr);
            }
            else
            {
                agotimeline.Insert(agotimeline.Count - agoid, astr);
                this.button1.Enabled = true;
                --agoid;
            }
        }
        private void GetDrive()
        {
            DriveInfo[] dinfo = DriveInfo.GetDrives();
            foreach (DriveInfo info in dinfo)
            {
                this.imageList2.ImageSize = new Size(16, 16);
                this.imageList2.ColorDepth = ColorDepth.Depth32Bit;
                SHGFI.SHFILEINFO shfi = new SHGFI.SHFILEINFO();
                SHGFI.SHGetFileInfo(info.Name,
                    0,
                    ref shfi,
                    (uint)Marshal.SizeOf(shfi),
                    SHGFI.SHGFI_USEFILEATTRIBUTES | SHGFI.SHGFI_TYPENAME | SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON | SHGFI.SHGFI_DISPLAYNAME);
                string dname = shfi.szDisplayName;
                this.imageList2.Images.Add(Icon.FromHandle(shfi.hIcon).Clone() as Icon);
                ListViewItem lvi = new ListViewItem(dname);
                lvi.ImageIndex = imageList2.Images.Count - 1;
                this.listView2.Items.Add(lvi);
                drivemap.Add(dname, info.Name);
            }
        }

        private void listView2_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView2.SelectedItems.Count > 0)
            {
                string dpath = listView2.SelectedItems[0].Text;
                string drname = drivemap[dpath];
                this.listView1.Items.Clear();
                GetFileList(drname);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                string dpath = listView1.SelectedItems[0].Text;
                string abspath = curpath + "\\" + dpath;
                if (dictlist.Contains(dpath))
                {
                    this.listView1.Items.Clear();
                    GetFileList(abspath);
                }
                else
                {
                    SHGFI.ShellExecute(IntPtr.Zero, "open", abspath, null, curpath, SHGFI.ShowWindowCommands.SW_SHOWNORMAL);
                }
            }
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listView1.Items.Clear();
            GetFileList(curpath);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int vit = nexttimeline.Count - nextid - 1;
            this.listView1.Items.Clear();
            string rpath = curpath;
            GetFileList(nexttimeline[vit]);
            if (nextid + 1 == nexttimeline.Count)
            {
                this.button2.Enabled = false;
                ++nextid;
            }
            else
            {
                ++nextid;
            }
            if (agoid - 1 == -1)
            {
                agotimeline.Insert(agotimeline.Count - agoid - 1, rpath);
                this.button2.Enabled = false;
            }
            else
            {
                --agoid;
                agotimeline.Insert(agotimeline.Count - agoid - 1, rpath);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string rpath = curpath;
            this.listView1.Items.Clear();
            GetFileList(agotimeline[agotimeline.Count - agoid - 1]);
            if (agoid + 1 == agotimeline.Count)
            {
                this.button1.Enabled = false;
                ++agoid;
            }
            else
            {
                ++agoid;
            }
            if (nextid == 0)
            {
                nexttimeline.Add(rpath);
            }
            else
            {
                --nextid;
                nexttimeline.Insert(nexttimeline.Count - nextid - 1, rpath);
            }
            this.button2.Enabled = true;
        }

        private void 重命名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dpath = listView1.SelectedItems[0].Text;
            cpath = curpath + "\\" + dpath;
            if (dictlist.Contains(dpath))
            {
                isdict = true;
            }
            else { isdict = false; }
            cpath2 = curpath;
            Form2 dlg = new Form2();
            dlg.Text = "重命名" + dpath;
            dlg.ShowDialog();
            this.listView1.Items.Clear();
            GetFileList(curpath);
        }
    }
}
