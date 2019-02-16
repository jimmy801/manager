using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class AV管家 : Form
    {
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        int tc = 0;
        ListViewItemComparer sorter = new ListViewItemComparer();
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string deskNames = "TOSHIBA_white SP_blk TOSHIBA_red TOSHIBA_blk ADATA_blue Seagate_4T_red Transcend_green Transcend_blk TOSHIBA_blue TOSHIBA_TBLK Seagate_BLK_8T";
        bool over = false;
        bool getDataexc = false;
        bool lastIsFolder = true;
        bool ValreadyRun = false;
        bool FalreadyRun = false;
        long VcmdT, VtotalT, FcmdT, FtotalT;
        List<int> lastSelect = new List<int>();
        List<ListViewItem> aryF = new List<ListViewItem>();
        List<ListViewItem> aryV = new List<ListViewItem>();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public AV管家()
        {
            this.Icon = Properties.Resources.favicon_20181126051103728;
            InitializeComponent();
            this.ShowIcon = true;
        }

        ///<summary>
        ///字串轉半形
        ///</summary>
        ///<paramname="input">任一字元串</param>
        ///<returns>半形字元串</returns>
        private static string ToNarrow(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        private string CommandOutput(string commandText)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true; //不跳出cmd視窗
            string strOutput = null;
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            try
            {
                p.Start();
                p.StandardInput.WriteLine("chcp 65001");
                p.StandardInput.WriteLine("@echo off");
                p.StandardInput.WriteLine(commandText);
                p.StandardInput.WriteLine("exit");
                strOutput = p.StandardOutput.ReadToEnd();//匯出整個執行過程
                string lastStr = "More? )";
                strOutput = strOutput.Substring(strOutput.LastIndexOf(lastStr) + lastStr.Length);
                strOutput = strOutput.Substring(0, strOutput.LastIndexOf("exit"));
                strOutput = new Regex(@"(\r\n)*.*Volume.+?\r\n|(\r\n)*.+File\(s\).+?\r\n|(\r\n)*.+Dir\(s\).+?\r\n").Replace(strOutput, "");
                p.WaitForExit();
                p.Close();
            }
            catch (Exception e)
            {
                strOutput = e.Message;
            }
            return strOutput;
        }

        private void setBtnSize()
        {
            int div = 2;
            if (!FolderR.Checked) div = 3;
            pictureBox1.SetBounds((listView1.Width - pictureBox1.Width - label1.Width - 6) / 2, 20, pictureBox1.Width, pictureBox1.Height);
            label1.SetBounds(pictureBox1.Right + 3, (pictureBox1.Height - label1.Height) / 2 + 20, label1.Width, label1.Height);
            int lstW = (listView1.Width - 20) / div - 3;
            for (int i = 0; i < listView1.Columns.Count; ++i)
                listView1.Columns[i].Width = lstW;
            BtnPanel.SetBounds(toolPanel.Width - BtnPanel.Width - 10, (toolPanel.Height - BtnPanel.Height) / 2, BtnPanel.Width, BtnPanel.Height);
            radioPanel.SetBounds(10, (toolPanel.Height - radioPanel.Height) / 2, radioPanel.Width, radioPanel.Height);
            int p5end = radioPanel.Location.X + radioPanel.Width;
            searchPanel.SetBounds((BtnPanel.Location.X - p5end - searchPanel.Width) / 2 + p5end, (toolPanel.Height - searchPanel.Height) / 2, searchPanel.Width, searchPanel.Height);
            this.Refresh();
        }

        private void setDefaultColor()
        {
            for (int i = 0; i < listView1.Items.Count; ++i)
            {
                if (i % 2 == 0) listView1.Items[i].BackColor = Color.WhiteSmoke;
                else listView1.Items[i].BackColor = Color.Gainsboro;
                listView1.Items[i].ForeColor = Color.Black;
            }
        }

        private void setUnFocusColor()
        {
            cancelUnFocusSelect();
            for (int i = 0; i < listView1.SelectedItems.Count; ++i)
            {
                listView1.SelectedItems[i].BackColor = SystemColors.Highlight;
                listView1.SelectedItems[i].ForeColor = Color.White;
                lastSelect.Add(listView1.SelectedItems[i].Index);
            }
        }

        private void cancelUnFocusSelect()
        {
            try
            {
                foreach (var i in lastSelect)
                {
                    if (i % 2 == 0) listView1.Items[i].BackColor = Color.WhiteSmoke;
                    else listView1.Items[i].BackColor = Color.Gainsboro;
                    listView1.Items[i].ForeColor = Color.Black;
                }
                lastSelect.Clear();
            }
            catch { }
        }

        private bool VaildSerialNum(string findstr)
        {
            string strpattern = @"^[a-zA-Z]+[0-9]+.\.\w+$";
            return new Regex(strpattern, RegexOptions.IgnoreCase).Match(findstr).Length > 0;
        }

        private void forFolder()
        {
            aryF.Clear();
            Stopwatch sw = new Stopwatch();//Stopwatch類別在System.Diagnostics命名空間裡
            sw.Reset();
            sw = Stopwatch.StartNew();
            string[] filter = { ".", "..", "白馬下載器", "新增資料夾", "Sp Service" };
            //string[] tmp = File.ReadAllLines(desktop + @"\o");
            string[] tmp = CommandOutput(String.Format(@"
for %p in (D E F G H I J K L M N O P Q R S T U V W X Y Z) do (
if exist %p:\ (
for /f ""tokens=4"" %i in ('vol %p: ^| findstr ""{0}""') do (
if ""%i"" NEQ """" (
cd /d %p:\
if exist %p:\Data ( 
cd /D %p:\Data 
)
dir
)
)
)
)", deskNames)).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            FcmdT = sw.ElapsedMilliseconds;
            string loc = "";
            foreach (var t in tmp)
            {
                if (t.Contains("<DIR>") && !loc.Contains("多人"))
                {
                    bool b = false;
                    foreach (var f in filter)
                        if (t.Contains(f)) { b = true; break; }
                    if (!b)
                        aryF.Add(new ListViewItem(new string[] { t.Substring(39), loc }));
                }
                else if (t.Contains("Directory of"))
                {
                    loc = String.Copy(t).Substring(14);
                    if (loc.Contains("多")) aryF.Add(new ListViewItem(new string[] { "多人/", loc.Substring(0, loc.IndexOf("多")) }));
                    if (!loc.EndsWith("\\")) loc += '\\';
                }
            }
            sw.Stop();
            FtotalT = sw.ElapsedMilliseconds;
            FalreadyRun = true;
        }

        private void getFolderLst()
        {
            listView1.Clear();
            searchText.Enabled = searchBTN.Enabled = rldBTN.Enabled = rdmBTN.Enabled = false;
            label1.Visible = true;
            pictureBox1.Visible = true;
            total.Text = "處理中";
            /*ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = desktop + @"\folder.bat";

            Process p = new Process();
            p.StartInfo = psi;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.WaitForExit();
            p.Close();
            p.Dispose();*/

            listView1.Columns.Add("Folder");
            listView1.Columns.Add("Location");
            setBtnSize();
            listView1.Items.AddRange(aryF.ToArray());
            label1.Visible = false;
            pictureBox1.Visible = false;
            total.Text = String.Format("共 {0} 個項目", listView1.Items.Count.ToString("#,0"));
            setDefaultColor();
            lastIsFolder = true;
            searchText.Enabled = searchBTN.Enabled = rldBTN.Enabled = rdmBTN.Enabled = true;
        }

        //StringBuilder sb = new StringBuilder();
        private void forVideo()
        {
            aryV.Clear();
            Stopwatch sw = new Stopwatch();//Stopwatch類別在System.Diagnostics命名空間裡
            sw.Reset();
            sw = Stopwatch.StartNew();
            string[] tmp = CommandOutput(String.Format(@"
for %p in (D E F G H I J K L M N O P Q R S T U V W X Y Z) do (
if exist %p:\ ( 
for /f ""tokens=4"" %i in ('vol %p: ^| findstr ""{0}""') do (
if ""%i"" NEQ """" (
%p:
if exist %p:\Data ( 
cd Data 
)
if exist %p:\多 (
cd 多
) 
dir /b /on /s *.mp4 *.rmvb *.avi *.mkv *.mpg *.flv *.wmv *.m4v *.3gp *.ts *.webm *.vob *.ovg *.ogg *.drc ^
*.mng *.mts *.m2ts *.mov *.qt *.yuv *.rm *.asf *.amv *.m4p *.mp2 *.mpeg *.mpe *.mpv *.m2v *.svi *.3g2 *.mxf *.roq *.nsv *.f4v *f4p *f4a *f4a
)
)
)
)", deskNames)).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            VcmdT = sw.ElapsedMilliseconds;
            string[] tmpstr;
            string actress = "";
            string loc = "";
            string[] actressLst = null;
            string tmpSerialNum = "";
            int backDash = 0;
            for (char i = 'D'; i <= 'Z'; ++i)
            {
                if (Directory.Exists(string.Format(@"{0}:\多人", i)))
                {
                    if (File.Exists(string.Format(@"{0}:\多人\人名.txt", i)))
                    {
                        actressLst = File.ReadAllLines(string.Format(@"{0}:\多人\人名.txt", i));
                        break;
                    }
                }
            }
            Dictionary<string, string> actressList = new Dictionary<string, string>();
            try
            {
                foreach (var a in actressLst)
                {
                    try
                    {
                        tmpstr = a.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        actressList.Add(tmpstr[0], tmpstr[1]);
                    }
                    catch { }
                }
            }
            catch { }
            foreach (var t in tmp)
            {
                tmpstr = t.Split("\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (t == "exit" || tmpstr.Length <= 1)
                    continue;
                backDash = t.LastIndexOf('\\') + 1;
                loc = t.Substring(0, backDash);
                for (int i = 2; i < tmpstr.Length; ++i)
                    if (!tmpstr[i].Contains("Data") && !tmpstr[i].Contains("多人") && !tmpstr[i].Contains("VR"))
                    {
                        actress = tmpstr[i];
                        break;
                    }
                if (loc.Contains(@"\多人\"))
                {
                    try
                    {
                        tmpSerialNum = t.Substring(backDash);
                        actressList.TryGetValue(tmpSerialNum.Substring(0, tmpSerialNum.IndexOf('.')), out actress);
                    }
                    catch { }
                }
                /* else if (VaildSerialNum(tmpstr[tmpstr.Length - 1]))
                 {
                     sb.Append(loc + tmpstr[tmpstr.Length - 1] + '\t');
                 }*/
                if (string.IsNullOrWhiteSpace(actress)) try { actress = tmpSerialNum.Substring(0, tmpSerialNum.IndexOf('.')); }
                    catch { }
                if (tmpstr[tmpstr.Length - 1] != "Data")
                    aryV.Add(new ListViewItem(new string[] { tmpstr[tmpstr.Length - 1], actress.Trim(), loc }));
            }
            sw.Stop();
            VtotalT = sw.ElapsedMilliseconds;
            ValreadyRun = true;
        }

        private void getVideoLst()
        {
            listView1.Clear();
            label1.Visible = pictureBox1.Visible = true;
            searchText.Enabled = searchBTN.Enabled = rldBTN.Enabled = rdmBTN.Enabled = false;
            total.Text = "處理中";
            /*ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = desktop + @"\video.bat";

            Process p = new Process();
            p.StartInfo = psi;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.WaitForExit();
            p.Close();
            p.Dispose();*/

            listView1.Columns.Add("Serial Number");
            listView1.Columns.Add("Actress");
            listView1.Columns.Add("Location");

            setBtnSize();
            listView1.Items.AddRange(aryV.ToArray());
            label1.Visible = pictureBox1.Visible = false;
            total.Text = String.Format("共 {0} 個項目", listView1.Items.Count.ToString("#,0"));
            setDefaultColor();
            lastIsFolder = false;
            searchText.Enabled = searchBTN.Enabled = rldBTN.Enabled = rdmBTN.Enabled = true;
        }

        private void waitForBatch()
        {
            while (!FalreadyRun || !ValreadyRun)
            {
                Application.DoEvents();
                searchText.Enabled = searchBTN.Enabled = rldBTN.Enabled = rdmBTN.Enabled = false;
                Thread.Sleep(10);
            }
            searchText.Enabled = searchBTN.Enabled = rldBTN.Enabled = rdmBTN.Enabled = true;
        }

        private void getData()
        {
            getDataexc = true;
            Thread t1 = new Thread(forVideo);
            t1.Start();

            Thread t2 = new Thread(forFolder);
            t2.Start();

            waitForBatch();
        }

        private void Initial()
        {
            lastSelect.Clear();
            waitForBatch();
            //new Form2(sb.ToString()).Show();

            if (FolderR.Checked && (!over || !lastIsFolder)) getFolderLst();
            else if (VideoR.Checked && (!over || lastIsFolder)) getVideoLst();

            if (getDataexc)
            {
                setDetail(String.Format("CMD: {0} sec, Other: {1} sec, Total: {2} sec", Convert.ToSingle(VcmdT + FcmdT) / 1000, Convert.ToSingle(VtotalT - VcmdT + FtotalT - FcmdT) / 1000, Convert.ToSingle(VtotalT + FtotalT) / 1000));
                getDataexc = false;
            }
            over = true;
        }

        private void reload()
        {
            FalreadyRun = false;
            ValreadyRun = false;
            over = false;
            List<string> last = new List<string>();
            for (int i = listView1.SelectedItems.Count - 1; i >= 0; --i)
                last.Add(listView1.SelectedItems[i].Text);
            listView1.Clear();
            label1.Visible = true;
            pictureBox1.Visible = true;
            total.Text = "處理中";
            getData();
            Initial();

            try
            {
                if (last.Count > 0)
                    listView1.EnsureVisible(listView1.FindItemWithText(last[0]).Index);
                foreach (var lvi in last)
                    listView1.FindItemWithText(lvi).Selected = true;
            }
            catch { }
        }

        private void rldBTN_Click(object sender, EventArgs e)
        {
            reload();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            setBtnSize();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyFirstColumnToClip();
        }

        private void t_Tick(object sender, EventArgs e)
        {
            if (tc++ == 50)
            {
                total.Text = String.Format("共 {0} 個項目", listView1.Items.Count.ToString("#,0"));
                t.Stop();
                tc = 0;
            }
        }

        private void setDetail(string msg)
        {
            total.Text = String.Format("共 {0} 個項目", listView1.Items.Count.ToString("#,0"));
            t.Start();
            total.Text += ", " + msg;
        }

        private void copyFirstColumnToClip()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                for (int i = 0; i < listView1.Items.Count; ++i)
                    if (listView1.Items[i].Selected)
                    {
                        int dotP = listView1.Items[i].Text.IndexOf('.');
                        if (dotP < 0) dotP = listView1.Items[i].Text.Length;
                        string copyStr = listView1.Items[i].Text.Substring(0, dotP);
                        Clipboard.SetText(copyStr);
                        setDetail(string.Format("已複製\"{0}\"", copyStr));
                        break;
                    }
            }
        }

        private void copySecondColumnToClip()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                for (int i = 0; i < listView1.Items.Count; ++i)
                    if (listView1.Items[i].Selected)
                    {
                        string copyStr = listView1.Items[i].SubItems[1].Text;
                        Clipboard.SetText(copyStr);
                        setDetail(string.Format("已複製\"{0}\"", copyStr));
                        break;
                    }
            }
        }

        private void search_task(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                searchText.Focus();
                searchText.SelectAll();
                e.SuppressKeyPress = true;
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Modifiers)
            {
                case Keys.Control:
                    switch (e.KeyCode)
                    {
                        case Keys.O: openFolder(); e.SuppressKeyPress = true; break;
                        case Keys.L: reload(); e.SuppressKeyPress = true; break;
                        case Keys.R: random_select(); e.SuppressKeyPress = true; break;
                        default: break;
                    }
                    break;
                case Keys.Alt:
                    if (e.KeyCode == Keys.O)
                    {
                        openFile(); e.SuppressKeyPress = true;
                    }
                    break;
                default:
                    switch (e.KeyCode)
                    {
                        case Keys.F1: copyFirstColumnToClip(); e.SuppressKeyPress = true; break;
                        case Keys.F2: copySecondColumnToClip(); e.SuppressKeyPress = true; break;
                        case Keys.Enter: openFile(); e.SuppressKeyPress = true; break;
                        default: break;
                    }
                    break;
            }
            search_task(sender, e);
        }

        private void cancelSelected()
        {
            for (int i = 0; i < listView1.SelectedItems.Count; ++i)
                listView1.SelectedItems[i].Selected = false;
        }

        private void search(int interval)
        {
            if (string.IsNullOrWhiteSpace(searchText.Text)) return;
            int startI = listView1.SelectedItems.Count > 0 ? listView1.SelectedItems[0].Index + interval < listView1.Items.Count ? listView1.SelectedItems[0].Index + interval >= 0 ? listView1.SelectedItems[0].Index + interval : listView1.Items.Count - 1 : 0 : 0;
            int endI = interval > 0 ? listView1.Items.Count : 0;
            int init_pos = startI;
            bool found = false;
            for (int i = startI; interval > 0 ? i < endI : i >= endI; i += interval)
            {
                for (int j = 0; j < listView1.Items[i].SubItems.Count; ++j)
                    if (listView1.Items[i].SubItems[j].Text.IndexOf(ToNarrow(searchText.Text), StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        cancelSelected();
                        listView1.Items[i].Selected = true;
                        listView1.EnsureVisible(i);
                        found = true;
                        break;
                    }
                if (found) break;
                else if (interval > 0 && i == listView1.Items.Count - 1 && startI != 0)
                {
                    i = -1;
                    endI = startI;
                }
                else if (interval < 0 && i == 0 && startI != listView1.Items.Count - 1)
                {
                    i = listView1.Items.Count;
                    endI = startI;
                }
            }
            if (!found) setDetail(string.Format("找不到\"{0}\"", ToNarrow(searchText.Text)));
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    searchText.Text = "";
                    listView1.Focus();
                    if (listView1.SelectedItems.Count == 1)
                        listView1.SelectedItems[0].Focused = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Enter:
                    if (Form.ModifierKeys == Keys.Alt) openFile();
                    else if (Control.ModifierKeys == Keys.Shift) search(-1);
                    else search(1);
                    e.SuppressKeyPress = true; break;
                default:
                    switch (e.KeyCode)
                    {
                        case Keys.F1: copyFirstColumnToClip(); e.SuppressKeyPress = true; break;
                        case Keys.F2: copySecondColumnToClip(); e.SuppressKeyPress = true; break;
                        case Keys.Enter: openFile(); e.SuppressKeyPress = true; break;
                        default: break;
                    }
                    break;
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (this.sorter.SortOrder == SortOrder.Ascending)
                this.sorter.SortOrder = SortOrder.Descending;
            else if (this.sorter.SortOrder == SortOrder.Descending)
                this.sorter.SortOrder = SortOrder.Ascending;
            if (sorter.SortColumn != e.Column) this.sorter.SortOrder = SortOrder.Ascending;
            sorter.SortColumn = e.Column;
            setDetail(string.Format("以\"{0}\"排序", listView1.Columns[e.Column].Text));
            listView1.Sort();
            setDefaultColor();
            if (listView1.SelectedItems.Count > 0)
                listView1.EnsureVisible(listView1.SelectedItems[0].Index);
        }

        private void findRepeat()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            string f1, f2;
            int pos = -1;
            bool add = false;
            int count = 0;
            for (int i = 1; i < listView1.Items.Count; i++)
            {
                pos = pos < 0 ? i - 1 : pos;
                f1 = listView1.Items[pos].Text.Substring(0, listView1.Items[pos].Text.LastIndexOf('.'));
                f2 = listView1.Items[i].Text.Substring(0, listView1.Items[i].Text.LastIndexOf('.'));
                if (f1.Contains(f2) || f2.Contains(f1))
                {
                    if (pos == i - 1)
                        sb.Append(listView1.Items[pos].SubItems[2].Text + listView1.Items[pos].SubItems[0].Text + '\t');
                    sb.Append(listView1.Items[i].SubItems[2].Text + listView1.Items[i].SubItems[0].Text + '\t');
                    if (add) count++;
                    else count += 2;
                    add = true;
                }
                else
                {
                    if (add)
                    {
                        sb2.Append(count.ToString() + '\t');
                        add = false;
                    }
                    pos = -1;
                }
            }
            if (sb.Length > 0) new Form2(sb.ToString(), sb2.ToString()).Show();
        }

        private void searchBTN_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift) search(-1);
            else search(1);
            if (string.IsNullOrWhiteSpace(searchText.Text) && VideoR.Checked)
                findRepeat();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //為ListViewItemSorter指定排序類
            sorter.SortOrder = SortOrder.Ascending;
            listView1.ListViewItemSorter = sorter;
            pictureBox1.SetBounds((listView1.Width - pictureBox1.Width - label1.Width - 6) / 2, 20, pictureBox1.Width, pictureBox1.Height);
            label1.SetBounds(pictureBox1.Right + 3, (pictureBox1.Height - label1.Height) / 2 + 20, label1.Width, label1.Height);
            getData();
            Initial();
            setBtnSize();
            this.MinimumSize = new Size(radioPanel.Location.X + radioPanel.Margin.Horizontal * 2 + radioPanel.Width + searchPanel.Width + searchPanel.Margin.Horizontal * 2 + BtnPanel.Width + BtnPanel.Margin.Horizontal * 2, this.MinimumSize.Height);
            t.Interval = 100;
            t.Tick += new EventHandler(t_Tick);
            setDefaultColor();
        }

        private void waitForExplorer(string selected)
        {
            Process p = new Process();
            string arg;
            string file = @"C:\Windows\explorer.exe";
            arg = "/select, " + selected;
            p.StartInfo.FileName = file;
            p.StartInfo.Arguments = arg;
            p.Start();
            p.WaitForExit();
            p.Close();
        }

        private void openFile()
        {
            string selected = "";
            string fileName = "";
            bool b = false;
            for (int i = 0; i < listView1.SelectedItems.Count; ++i)
            {
                for (int j = 0; j < listView1.Items.Count; ++j)
                {
                    if (listView1.Items[j] == listView1.SelectedItems[i])
                    {
                        if (listView1.Items[j].SubItems.Count == 2)
                            selected = "\"" + listView1.Items[j].SubItems[1].Text;
                        else selected = "\"" + listView1.Items[j].SubItems[2].Text;
                        fileName = listView1.SelectedItems[i].Text + "\"";
                        b = true;
                        if (listView1.Items[j].SubItems.Count == 2) setDetail("資料夾開啟中...");
                        else setDetail("檔案開啟中...");
                        break;
                    }
                }
                if (b) break;
            }
            Process p = new Process();
            string arg;
            string file = @"C:\Windows\explorer.exe";
            arg = selected + fileName;
            p.StartInfo.FileName = file;
            p.StartInfo.Arguments = arg;
            p.Start();
            p.WaitForExit();
            p.Close();
        }

        private void openFolder()
        {
            string selected;
            string fileName;
            bool b = false;
            for (int i = 0; i < listView1.SelectedItems.Count; ++i)
            {
                for (int j = 0; j < listView1.Items.Count; ++j)
                {
                    if (listView1.Items[j] == listView1.SelectedItems[i])
                    {
                        if (listView1.Items[j].SubItems.Count == 2)
                            selected = listView1.Items[j].SubItems[1].Text;
                        else selected = listView1.Items[j].SubItems[2].Text;
                        fileName = listView1.SelectedItems[i].Text;
                        new Thread(() => waitForExplorer(selected + fileName)).Start();
                        b = true;
                        setDetail("資料夾開啟中...");
                        break;
                    }
                }
                if (b) break;
            }
        }

        private void openWithFileManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFolder();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listView1.Focused) setUnFocusColor();
            else cancelUnFocusSelect();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (Form.ModifierKeys == Keys.Alt) openFile();
            else openFolder();
        }

        private void Condition_CheckedChanged(object sender, EventArgs e)
        {
            sorter.SortColumn = 0;
            listView1.ListViewItemSorter = sorter;

            Initial();
        }

        private void random_select()
        {
            if (listView1.Items.Count > 0)
            {
                cancelSelected();
                int r = new Random(Guid.NewGuid().GetHashCode()).Next(listView1.Items.Count);
                listView1.Items[r].Selected = true;
                listView1.EnsureVisible(r);
                listView1.Focus();
            }
        }

        private void rdm_Click(object sender, EventArgs e)
        {
            random_select();
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            searchText.SelectAll();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            setUnFocusColor();
        }

        private void listView1_Leave(object sender, EventArgs e)
        {
            setUnFocusColor();
        }

    }

    public class ListViewItemComparer : IComparer
    {
        private Comparer comparer;
        private int sortColumn;
        private SortOrder sortOrder;
        public ListViewItemComparer()
        {
            sortColumn = 0;
            sortOrder = SortOrder.None;
            comparer = Comparer.Default;
        }
        public ListViewItemComparer(int column)
        {
            sortColumn = column;
        }
        //指定進行排序的列
        public int SortColumn
        {
            get { return sortColumn; }
            set { sortColumn = value; }
        }
        //指定按升序或降序進行排序
        public SortOrder SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }
        public int Compare(object x, object y)
        {
            int CompareResult;
            ListViewItem itemX = (ListViewItem)x;
            ListViewItem itemY = (ListViewItem)y;
            //在這裡您可以提供自定義的排序
            CompareResult = comparer.Compare(itemX.SubItems[sortColumn].Text, itemY.SubItems[sortColumn].Text);
            if (this.SortOrder == SortOrder.Ascending)
                return CompareResult;
            else
                if (this.SortOrder == SortOrder.Descending)
                return (-CompareResult);
            else
                return 0;
        }
    }
}
