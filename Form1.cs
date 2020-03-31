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
        int firstFoundI;
        int firstFoundJ;
        ListViewItemComparer sorter1 = new ListViewItemComparer();
        ListViewItemComparer sorter2 = new ListViewItemComparer();
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string deskNames = "TOSHIBA_white SP_blk TOSHIBA_red TOSHIBA_blk ADATA_blue Seagate_4T_red Transcend_green Transcend_blk TOSHIBA_blue TOSHIBA_TBLK Seagate_BLK_8T TOSHIBA_4T_BLK ADATA_4T WD_4T_blk ADATA_4T_DB";
        bool over = false;
        bool getDataexc = false;
        bool lastIsFolder = true;
        bool ValreadyRun = false;
        bool FalreadyRun = false;
        long VcmdT, VtotalT, FcmdT, FtotalT;
        List<int> lastSelect;
        List<int> lastFSelect = new List<int>();
        List<int> lastVSelect = new List<int>();
        List<ListViewItem> aryF = new List<ListViewItem>();
        List<ListViewItem> aryV = new List<ListViewItem>();
        ListView listViewItem;
        Thread t1 = null;
        Thread t2 = null;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public AV管家()
        {
            this.Icon = Properties.Resources.favicon_20181126051103728;
            InitializeComponent();
            listView1.BringToFront();
            label1.BringToFront();
            pictureBox1.BringToFront();
            listViewItem = listView1;
            lastSelect = lastFSelect;
            this.ShowIcon = true;
            statusStrip1.Renderer = new CustomRenderer();
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
            string strOutput = "";
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            try
            {
                p.Start();
                p.StandardInput.WriteLine("chcp 65001");
                p.StandardInput.WriteLine("@echo off");
                p.StandardInput.WriteLine(commandText);
                p.StandardInput.WriteLine("exit");
                strOutput = p.StandardOutput.ReadToEnd();//匯出整個執行過程
                string lastStr = "More? )\r\n";
                int start = strOutput.LastIndexOf(lastStr) + lastStr.Length;
                int len = strOutput.LastIndexOf("\r\nexit") - start;
                strOutput = strOutput.Substring(start, len);
                p.WaitForExit();
                p.Close();
            }
            catch
            {
                strOutput = "";
                //strOutput = e.Message;
            }
            return strOutput;
        }

        private void setListViewSize()
        {
            int y = 28;
            int div = 2;
            if (!FolderR.Checked) div = 3;
            int lstW = (listViewItem.Width - y) / div - 3;
            for (int i = 0; i < listViewItem.Columns.Count; ++i)
                listViewItem.Columns[i].Width = lstW;
        }

        private void setBtnSize()
        {
            int y = 0;
            pictureBox1.SetBounds((listViewItem.Width - pictureBox1.Width - label1.Width - 6) / 2, y, pictureBox1.Width, pictureBox1.Height);
            label1.SetBounds(pictureBox1.Right + 3, (pictureBox1.Height - label1.Height) / 2 + y, label1.Width, label1.Height);
            BtnPanel.SetBounds(toolPanel.Width - BtnPanel.Width - 10, (toolPanel.Height - BtnPanel.Height) / 2, BtnPanel.Width, BtnPanel.Height);
            radioPanel.SetBounds(10, (toolPanel.Height - radioPanel.Height) / 2, radioPanel.Width, radioPanel.Height);
            int p5end = radioPanel.Location.X + radioPanel.Width;
            searchPanel.SetBounds((BtnPanel.Location.X - p5end - searchPanel.Width) / 2 + p5end, (toolPanel.Height - searchPanel.Height) / 2, searchPanel.Width, searchPanel.Height);
        }

        private void setDefaultColor()
        {
            for (int i = 0; i < listViewItem.Items.Count; ++i)
            {
                if (i % 2 == 0) listViewItem.Items[i].BackColor = Color.WhiteSmoke;
                else listViewItem.Items[i].BackColor = Color.Gainsboro;
                listViewItem.Items[i].ForeColor = Color.Black;
            }
        }

        private void setOnFocusColor()
        {
            cancelOnFocusSelect();
            for (int i = 0; i < listViewItem.SelectedItems.Count; ++i)
            {
                listViewItem.SelectedItems[i].BackColor = SystemColors.Highlight;
                listViewItem.SelectedItems[i].ForeColor = Color.White;
            }
        }

        private void cancelOnFocusSelect()
        {
            try
            {
                foreach (var i in lastSelect)
                {
                    if (i % 2 == 0) listViewItem.Items[i].BackColor = Color.WhiteSmoke;
                    else listViewItem.Items[i].BackColor = Color.Gainsboro;
                    listViewItem.Items[i].ForeColor = Color.Black;
                }
            }
            catch { }
            finally
            {
                lastSelect.Clear();
                for (int i = 0; i < listViewItem.SelectedItems.Count; ++i)
                    lastSelect.Add(listViewItem.SelectedItems[i].Index);
            }
        }

        private void setLastFocus()
        {
            for (int i = 0; i < lastSelect.Count; ++i)
            {
                listViewItem.Items[lastSelect[i]].BackColor = SystemColors.Highlight;
                listViewItem.Items[lastSelect[i]].ForeColor = Color.White;
            }
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
            string[] filter = { ".", "..", "白馬下載器", "新增資料夾", "Sp Service", "System" };
            //string[] tmp = File.ReadAllLines(desktop + @"\o");
            string[] tmp = CommandOutput(String.Format(@"
for %p in (D E F G H I J K L M N O P Q R S T U V W X Y Z) do (
if exist %p:\ (
if exist %p:\Data ( 
for /f %i in ('dir %p: ^| findstr ""{0}""') do (
if ""%i"" NEQ """" (
cd /D %p:\Data 
for /F ""tokens=*"" %A in ('dir /ad/b') do @echo %~dpnxA
)
)
)
)
)", deskNames)).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            FcmdT = sw.ElapsedMilliseconds;
            string loc = "";
            bool b;
            foreach (var t in tmp)
            {
                loc = t.Substring(0, t.LastIndexOf("\\") + 1);
                if (!loc.Contains("多人"))
                {
                    b = false;
                    foreach (var f in filter)
                        if (t.Contains(f)) { b = true; break; }
                    if (!b)
                        aryF.Add(new ListViewItem(new string[] { t.Substring(t.LastIndexOf("\\") + 1), loc }));
                }
            }
            sw.Stop();
            FtotalT = sw.ElapsedMilliseconds;
            FalreadyRun = true;
            t2.Abort();
        }

        private void getFolderLst()
        {
            searchText.Enabled = searchBTN.Enabled = rldBTN.Enabled = rdmBTN.Enabled = false;
            label1.Visible = pictureBox1.Visible = true;
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

            if (listView1.Columns.Count == 0)
            {
                listView1.Columns.Add("Folder");
                listView1.Columns.Add("Location");
                setListViewSize();
                listView1.Items.AddRange(aryF.ToArray());
            }
            else
                setListViewSize();
            label1.Visible = pictureBox1.Visible = false;
            searchText.Enabled = searchBTN.Enabled = rldBTN.Enabled = rdmBTN.Enabled = true;
            total.Text = String.Format("共 {0} 個項目", listView1.Items.Count.ToString("#,0"));
            //Refresh();
            lastIsFolder = true;
        }

        //StringBuilder sb = new StringBuilder();
        private void forVideo()
        {
            aryV.Clear();
            Stopwatch sw = new Stopwatch();//Stopwatch類別在System.Diagnostics命名空間裡
            sw.Reset();
            sw = Stopwatch.StartNew();
            string path = "%p:\\Data\\";
            string allNames = "*.";
            string separate_pre = " " + path + allNames;
            string[] videoTypes = {"mp4", "rmvb", "avi", "mkv", "mpg", "flv", "wmv", "m4v", "3gp", "ts", "webm", "vob", "ovg", "ogg", "drc",
                "mng", "mts", "m2ts", "mov", "qt", "yuv", "rm", "asf", "amv", "m4p", "mp2", "mpeg", "mpe", "mpv", "m2v", "svi", "3g2",
                "mxf", "roq", "nsv", "f4v", "f4p", "f4a" };
            string[] allTypes = (allNames + string.Join(allNames, videoTypes)).Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string main_cmd = "dir /b /s";
            string ignoreUnfind = " 2>nul";
            //string cmd = main_cmd + string.Join(ignoreUnfind + " & " + main_cmd, allTypes) + ignoreUnfind;
            string cmd = main_cmd + separate_pre + string.Join(separate_pre, videoTypes) + ignoreUnfind;
            string[] tmp = CommandOutput(String.Format(@"
for %p in (D E F G H I J K L M N O P Q R S T U V W X Y Z) do ( 
if exist %p:\ ( 
if exist %p:\Data\ (
for /f %i in ('dir %p: ^| findstr ""{0}""') do ( 
if ""%i"" NEQ """" ( 
{1} 
)
)
)
)
)", deskNames, cmd)).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            VcmdT = sw.ElapsedMilliseconds;
            string[] tmpstr;
            string actress = "";
            string loc = "";
            string[] actressLst = null;
            string[] VRActressLst = null;
            string tmpSerialNum = "";
            int backDash = 0;
            byte n = 0;
            for (char i = 'D'; i <= 'Z' && !n.Equals(2); ++i)
            {
                try
                {
                    actressLst = File.ReadAllLines(string.Format(@"{0}:\Data\多人\人名.txt", i));
                    n++;
                }
                catch { }
                try
                {
                    VRActressLst = File.ReadAllLines(string.Format(@"{0}:\Data\VR\多人\人名.txt", i));
                    n++;
                }
                catch { }
                //if (Directory.Exists(string.Format(@"{0}:\多人", i)))
                //{
                //    if (File.Exists(string.Format(@"{0}:\多人\人名.txt", i)))
                //    {
                //        actressLst = File.ReadAllLines(string.Format(@"{0}:\多人\人名.txt", i));
                //        break;
                //    }
                //}
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
                foreach (var a in VRActressLst)
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
                if (!t.Contains("Data") && !t.Contains("多人") && !t.Contains("VR")) continue;
                tmpstr = t.Split("\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (t == "exit" || tmpstr.Length <= 1)
                    continue;
                backDash = t.LastIndexOf('\\') + 1;
                loc = t.Substring(0, backDash);
                for (int i = 2; i < tmpstr.Length; ++i)
                {
                    if (!tmpstr[i].Contains("Data") && !tmpstr[i].Contains("多人") && !tmpstr[i].Contains("VR"))
                    {
                        actress = tmpstr[i];
                        break;
                    }
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
            t1.Abort();
        }

        private void getVideoLst()
        {
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

            if (listView2.Columns.Count == 0)
            {
                listView2.Columns.Add("Serial Number");
                listView2.Columns.Add("Actress");
                listView2.Columns.Add("Location");
                setListViewSize();
                listView2.Items.AddRange(aryV.ToArray());
            }
            else
                setListViewSize();
            label1.Visible = pictureBox1.Visible = false;
            searchText.Enabled = searchBTN.Enabled = rldBTN.Enabled = rdmBTN.Enabled = true;
            total.Text = String.Format("共 {0} 個項目", listView2.Items.Count.ToString("#,0"));
            //Refresh();
            lastIsFolder = false;
        }

        private void waitForBatch()
        {
            while (!FalreadyRun || !ValreadyRun)
            {
                Application.DoEvents();
                searchText.Enabled = searchBTN.Enabled = rldBTN.Enabled = rdmBTN.Enabled = false;
                Thread.Sleep(10);
            }
            if (listView2.Columns.Count == 0)
            {
                listView2.Columns.Add("Serial Number");
                listView2.Columns.Add("Actress");
                listView2.Columns.Add("Location");
                listView2.BeginUpdate();
                listView2.Items.AddRange(aryV.ToArray());
                listView2.EndUpdate();
            }
            if (listView1.Columns.Count == 0)
            {
                listView1.Columns.Add("Folder");
                listView1.Columns.Add("Location");
                listView1.BeginUpdate();
                listView1.Items.AddRange(aryF.ToArray());
                listView1.EndUpdate();
            }

            searchText.Enabled = searchBTN.Enabled = rldBTN.Enabled = rdmBTN.Enabled = true;
        }

        private void getData()
        {
            getDataexc = true;
            t1 = new Thread(forVideo);
            t1.IsBackground = true;
            t1.Start();

            t2 = new Thread(forFolder);
            t2.IsBackground = true;
            t2.Start();

            waitForBatch();
        }

        private void Initial()
        {
            waitForBatch();
            //new Form2(sb.ToString()).Show();

            bool tmp = lastIsFolder;
            if (FolderR.Checked && (!over || !lastIsFolder)) getFolderLst();
            else if (VideoR.Checked && (!over || lastIsFolder)) getVideoLst();
            try
            {
                setDefaultColor();
                listView1.SetSortIcon(sorter1.SortColumn, sorter1.SortOrder);
                listView2.SetSortIcon(sorter2.SortColumn, sorter2.SortOrder);
                //Refresh();
                setLastFocus();
            }
            catch { }

            if (getDataexc)
            {
                setDetail(String.Format("CMD: {0} sec, Other: {1} sec, Total: {2} sec", Convert.ToSingle(VcmdT + FcmdT) / 1000, Convert.ToSingle(VtotalT - VcmdT + FtotalT - FcmdT) / 1000, Convert.ToSingle(VtotalT + FtotalT) / 1000), 3);
                getDataexc = false;
            }
            over = true;
        }

        private void reload()
        {
            FalreadyRun = false;
            ValreadyRun = false;
            over = false;
            listView1.Hide();
            listView2.Hide();
            List<string> last = new List<string>();
            for (int i = listViewItem.SelectedItems.Count - 1; i >= 0; --i)
                last.Add(listViewItem.SelectedItems[i].Text);
            listView1.Clear();
            listView2.Clear();
            label1.Visible = true;
            pictureBox1.Visible = true;
            total.Text = "處理中";
            getData();
            Initial();

            try
            {
                if (last.Count > 0)
                    listViewItem.EnsureVisible(listViewItem.FindItemWithText(last[0]).Index);
                foreach (var lvi in last)
                    listViewItem.FindItemWithText(lvi).Selected = true;
            }
            catch { }
            finally { listViewItem.Show(); }
        }

        private void rldBTN_Click(object sender, EventArgs e)
        {
            reload();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            setBtnSize();
            setListViewSize();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyFirstColumnToClip();
        }

        private void t_Tick(object sender, EventArgs e)
        {
            if (tc++ == 10)
            {
                if (!label1.Visible)
                    total.Text = String.Format("共 {0} 個項目", listViewItem.Items.Count.ToString("#,0"));
                t.Stop();
                tc = 0;
            }
        }

        private void setDetail(string msg)
        {
            setDetail(msg, 1);
        }

        private void setDetail(string msg, double sec)
        {
            if (!label1.Visible)
                total.Text = String.Format("共 {0} 個項目", listViewItem.Items.Count.ToString("#,0"));
            t.Stop();
            tc = 0 - (int)((sec - 1) * 10);
            t.Start();
            if (!String.IsNullOrWhiteSpace(msg))
                total.Text += ", " + msg;
        }

        private void copyFirstColumnToClip()
        {
            if (listViewItem.SelectedItems.Count <= 0) return;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < listViewItem.Items.Count; ++i)
                if (listViewItem.Items[i].Selected)
                {
                    int dotP = listViewItem.Items[i].Text.IndexOf('.');
                    if (dotP < 0) dotP = listViewItem.Items[i].Text.Length;
                    if (sb.Length > 0) sb.Append(' ');
                    sb.Append(listViewItem.Items[i].Text.Substring(0, dotP));
                }
            Clipboard.SetText(sb.ToString());
            setDetail(string.Format("已複製\"{0}\"", sb.ToString()), 1.5);
        }

        private void copyOtherColumnToClip(int n)
        {
            if (listViewItem.SelectedItems.Count != 1 || listViewItem.Items[0].SubItems.Count <= n) return;
            for (int i = 0; i < listViewItem.Items.Count; ++i)
            {
                if (listViewItem.Items[i].Selected)
                {
                    string copyStr = listViewItem.Items[i].SubItems[n].Text;
                    Clipboard.SetText(copyStr);
                    setDetail(string.Format("已複製\"{0}\"", copyStr), 1.5);
                    break;
                }
            }
        }

        private void copyAllActress()
        {
            List<string> names = new List<string>();
            int nameIndex = listViewItem == listView2 ? 1 : 0;
            for (int i = 0; i < listViewItem.Items.Count; ++i)
            {
                foreach (string str in listViewItem.Items[i].SubItems[nameIndex].Text.Split(" （）()".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    if (!names.Contains(str) && str != "多人" && str != "素人") names.Add(str);
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str in names)
                stringBuilder.Append(String.Format("\"{0}\",", str));
            Clipboard.SetText(stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString());
        }

        private void change_radio()
        {
            if (!FolderR.Checked) { FolderR.Focus(); FolderR.Checked = true; }
            else { VideoR.Focus(); VideoR.Checked = true; }
        }

        private void openAndCopyKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.Control && e.KeyCode == Keys.Tab)
            {
                change_radio();
                return;
            }
            switch (e.Modifiers)
            {
                case Keys.Control:
                    switch (e.KeyCode)
                    {
                        case Keys.O: openFolder(); e.SuppressKeyPress = true; break;
                        case Keys.L: reload(); e.SuppressKeyPress = true; break;
                        case Keys.R: random_select(); e.SuppressKeyPress = true; break;
                        case Keys.F: searchText.Focus(); searchText.SelectAll(); e.SuppressKeyPress = true; break;
                        case Keys.Tab: change_radio(); break;
                        default: break;
                    }
                    break;
                case Keys.Alt:
                    if (e.KeyCode == Keys.Enter)
                    {
                        openFile(); e.SuppressKeyPress = true;
                    }
                    break;
                default:
                    switch (e.KeyCode)
                    {
                        case Keys.F1: copyFirstColumnToClip(); e.SuppressKeyPress = true; break;
                        case Keys.F2: copyOtherColumnToClip(1); e.SuppressKeyPress = true; break;
                        case Keys.F3: copyOtherColumnToClip(2); e.SuppressKeyPress = true; break;
                        case Keys.F12: copyAllActress(); e.SuppressKeyPress = true; break;
                    }
                    break;
            }
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            openAndCopyKeyDown(sender, e);
            switch (e.Modifiers)
            {
                case Keys.Control:
                    break;
                case Keys.Alt:
                    break;
                default:
                    switch (e.KeyCode)
                    {
                        case Keys.Enter: openFile(); e.SuppressKeyPress = true; break;
                        default: break;
                    }
                    break;
            }
        }

        private void cancelSelected()
        {
            for (int i = 0; i < listViewItem.SelectedItems.Count; ++i)
                listViewItem.SelectedItems[i].Selected = false;
        }

        private void search(int interval)
        {
            if (string.IsNullOrWhiteSpace(searchText.Text)) return;
            int startI = listViewItem.SelectedItems.Count > 0 ? listViewItem.SelectedItems[0].Index + interval < listViewItem.Items.Count ? listViewItem.SelectedItems[0].Index + interval >= 0 ? listViewItem.SelectedItems[0].Index + interval : listViewItem.Items.Count - 1 : 0 : 0;
            int endI = interval > 0 ? listViewItem.Items.Count : 0;
            bool lastFound = firstFoundI >= 0;
            int lastI = firstFoundI;
            int lastJ = firstFoundJ;
            bool found = false;
            for (int i = startI; interval > 0 ? i < endI : i >= endI; i += interval)
            {
                for (int j = 0; j < listViewItem.Items[i].SubItems.Count; ++j)
                    if (listViewItem.Items[i].SubItems[j].Text.IndexOf(ToNarrow(searchText.Text).Trim(new char[]{ ' ', '\t', '\n'}), StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (!lastFound) { firstFoundI = i; firstFoundJ = j; }
                        else { lastI = i; lastJ = j; }
                        cancelSelected();
                        listViewItem.Items[i].Selected = true;
                        listViewItem.EnsureVisible(i);
                        found = true;
                        break;
                    }
                if (found) break;
                else if (interval > 0 && i == listViewItem.Items.Count - 1 && startI != 0)
                {
                    i = -1;
                    endI = startI;
                }
                else if (interval < 0 && i == 0 && startI != listViewItem.Items.Count - 1)
                {
                    i = listViewItem.Items.Count;
                    endI = startI;
                }
            }
            if (!found) setDetail(string.Format("找不到\"{0}\"", ToNarrow(searchText.Text)));
            else if (lastI == firstFoundI)
                setDetail(string.Format("已經回到搜尋起點\"{0}\"", listViewItem.Items[firstFoundI].SubItems[firstFoundJ].Text));
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            openAndCopyKeyDown(sender, e);
            switch (e.Modifiers)
            {
                case Keys.Control:
                    break;
                case Keys.Alt:
                    break;
                default:
                    switch (e.KeyCode)
                    {
                        case Keys.Escape:
                            searchText.Text = "";
                            listViewItem.Focus();
                            if (listViewItem.SelectedItems.Count == 1)
                                listViewItem.SelectedItems[0].Focused = true;
                            searchText.Focus();
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.Enter:
                            if (e.Modifiers == Keys.Shift) search(-1);
                            else search(1);
                            e.SuppressKeyPress = true; break;
                        default:
                            break;
                    }
                    break;
            }
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewItemComparer sorter = VideoR.Checked ? sorter2 : sorter1;
            if (sorter.SortOrder == SortOrder.Ascending)
                sorter.SortOrder = SortOrder.Descending;
            else if (sorter.SortOrder == SortOrder.Descending)
                sorter.SortOrder = SortOrder.Ascending;
            if (sorter.SortColumn != e.Column) sorter.SortOrder = SortOrder.Ascending;
            sorter.SortColumn = e.Column;
            setDetail(string.Format("以\"{0}\"排序", listViewItem.Columns[e.Column].Text), 2);
            listViewItem.Sort();
            setDefaultColor();
            if (listViewItem.SelectedItems.Count > 0)
                listViewItem.EnsureVisible(listViewItem.SelectedItems[0].Index);
            listViewItem.SetSortIcon(e.Column, sorter.SortOrder);
        }

        private void findDuplicate()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            string f1, f2;
            int pos = -1;
            bool add = false;
            int count = 0;
            for (int i = 1; i < listViewItem.Items.Count; i++)
            {
                pos = pos < 0 ? i - 1 : pos;
                f1 = listViewItem.Items[pos].Text.Substring(0, listViewItem.Items[pos].Text.LastIndexOf('.'));
                f2 = listViewItem.Items[i].Text.Substring(0, listViewItem.Items[i].Text.LastIndexOf('.'));
                if (f1.Contains(f2) || f2.Contains(f1))
                {
                    if (pos == i - 1)
                        sb.Append(listViewItem.Items[pos].SubItems[2].Text + listViewItem.Items[pos].SubItems[0].Text + '\t');
                    sb.Append(listViewItem.Items[i].SubItems[2].Text + listViewItem.Items[i].SubItems[0].Text + '\t');
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
            if (sb.Length > 0) new Form2(sb.ToString(), sb2.ToString()).ShowDialog(this);
        }

        private void searchBTN_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift) search(-1);
            else search(1);
            if (string.IsNullOrWhiteSpace(searchText.Text) && VideoR.Checked)
                findDuplicate();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            setBtnSize();
            //為ListViewItemSorter指定排序類
            sorter1.SortOrder = SortOrder.Ascending;
            sorter2.SortOrder = SortOrder.Ascending;
            listView1.ListViewItemSorter = sorter1;
            listView2.ListViewItemSorter = sorter2;
            listView1.Visible = listView2.Visible = false;
            getData();
            Initial();
            listView1.Visible = !VideoR.Checked;
            listView2.Visible = VideoR.Checked;
            this.MinimumSize = new Size(radioPanel.Location.X + radioPanel.Margin.Horizontal * 2 + radioPanel.Width + searchPanel.Width + searchPanel.Margin.Horizontal * 2 + BtnPanel.Width + BtnPanel.Margin.Horizontal * 2, this.MinimumSize.Height);
            t.Interval = 100;
            t.Tick += new EventHandler(t_Tick);
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
            for (int i = 0; i < listViewItem.SelectedItems.Count; ++i)
            {
                for (int j = 0; j < listViewItem.Items.Count; ++j)
                {
                    if (listViewItem.Items[j] == listViewItem.SelectedItems[i])
                    {
                        if (listViewItem.Items[j].SubItems.Count == 2)
                            selected = "\"" + listViewItem.Items[j].SubItems[1].Text;
                        else selected = "\"" + listViewItem.Items[j].SubItems[2].Text;
                        fileName = listViewItem.SelectedItems[i].Text + "\"";
                        b = true;
                        if (listViewItem.Items[j].SubItems.Count == 2) setDetail("資料夾開啟中...", int.MaxValue / 10);
                        else setDetail("檔案開啟中...", int.MaxValue / 10);
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
            p.Dispose();
            setDetail("");
        }

        private void openFolder()
        {
            string selected = "";
            string fileName = "";
            bool b = false;
            for (int i = 0; i < listViewItem.SelectedItems.Count; ++i)
            {
                for (int j = 0; j < listViewItem.Items.Count; ++j)
                {
                    if (listViewItem.Items[j] == listViewItem.SelectedItems[i])
                    {
                        if (listViewItem.Items[j].SubItems.Count == 2)
                            selected = listViewItem.Items[j].SubItems[1].Text;
                        else selected = listViewItem.Items[j].SubItems[2].Text;
                        fileName = listViewItem.SelectedItems[i].Text;
                        setDetail("資料夾開啟中...", 1.5);
                        //new Thread(() => waitForExplorer(selected + fileName)).Start();
                        b = true;
                        break;
                    }
                }
                if (b) break;
            }
            if (!b) return;
            //Process.Start("explorer.exe", "/select, " + selected + fileName);
            Process p = new Process();
            string arg;
            string file = @"C:\Windows\explorer.exe";
            arg = "/select, " + selected + fileName;
            p.StartInfo.FileName = file;
            p.StartInfo.Arguments = arg;
            p.Start();
            p.WaitForExit();
            p.Close();
            p.Dispose();
        }

        private void openWithFileManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFolder();
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listViewItem.Focused) setOnFocusColor();
            else cancelOnFocusSelect();
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            if (Form.ModifierKeys == Keys.Alt) openFile();
            else openFolder();
        }

        private void Condition_CheckedChanged(object sender, EventArgs e)
        {
            //sorter.SortColumn = 0;
            lastSelect = VideoR.Checked ? lastVSelect : lastFSelect;
            listViewItem = VideoR.Checked ? listView2 : listView1;
            listViewItem.ListViewItemSorter = VideoR.Checked ? sorter2 : sorter1;
            setListViewSize();
            Initial();
            listView1.Visible = !VideoR.Checked;
            listView2.Visible = VideoR.Checked;
        }

        private void random_select()
        {
            if (listViewItem.Items.Count > 0)
            {
                cancelSelected();
                int r = new Random(Guid.NewGuid().GetHashCode()).Next(listViewItem.Items.Count);
                listViewItem.Items[r].Selected = true;
                listViewItem.EnsureVisible(r);
                listViewItem.Focus();
            }
        }

        private void rdm_Click(object sender, EventArgs e)
        {
            random_select();
        }

        private void searchText_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.UnicodeText, true))
            {
                var data = e.Data.GetData(DataFormats.UnicodeText, true);
                if (data != null)
                {
                    if (data is string)
                    {
                        searchText.Text = data as string; // done!
                    }
                }
            }
        }

        private void searchText_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void searchText_TextChanged(object sender, EventArgs e)
        {
            firstFoundI = firstFoundJ = -1;
        }

        private void AV管家_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { t1.Abort(); }
            catch { }
            try { t2.Abort(); }
            catch { }
            FalreadyRun = ValreadyRun = true;
            System.Environment.Exit(0);
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            searchText.SelectAll();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            setOnFocusColor();
        }

        private void listView_Leave(object sender, EventArgs e)
        {
            setOnFocusColor();
        }

    }

    #region sorting by column header listview
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class ListViewExtensions
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct HDITEM
        {
            public Mask mask;
            public int cxy;
            [MarshalAs(UnmanagedType.LPTStr)] public string pszText;
            public IntPtr hbm;
            public int cchTextMax;
            public Format fmt;
            public IntPtr lParam;
            // _WIN32_IE >= 0x0300 
            public int iImage;
            public int iOrder;
            // _WIN32_IE >= 0x0500
            public uint type;
            public IntPtr pvFilter;
            // _WIN32_WINNT >= 0x0600
            public uint state;

            [Flags]
            public enum Mask
            {
                Format = 0x4,       // HDI_FORMAT
            };

            [Flags]
            public enum Format
            {
                SortDown = 0x200,   // HDF_SORTDOWN
                SortUp = 0x400,     // HDF_SORTUP
            };
        };

        public const int LVM_FIRST = 0x1000;
        public const int LVM_GETHEADER = LVM_FIRST + 31;

        public const int HDM_FIRST = 0x1200;
        public const int HDM_GETITEM = HDM_FIRST + 11;
        public const int HDM_SETITEM = HDM_FIRST + 12;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, ref HDITEM lParam);

        public static void SetSortIcon(this ListView listViewControl, int columnIndex, SortOrder order)
        {
            IntPtr columnHeader = SendMessage(listViewControl.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            for (int columnNumber = 0; columnNumber <= listViewControl.Columns.Count - 1; columnNumber++)
            {
                var columnPtr = new IntPtr(columnNumber);
                var item = new HDITEM
                {
                    mask = HDITEM.Mask.Format
                };

                if (SendMessage(columnHeader, HDM_GETITEM, columnPtr, ref item) == IntPtr.Zero)
                {
                    throw new System.ComponentModel.Win32Exception();
                }

                if (order != SortOrder.None && columnNumber == columnIndex)
                {
                    switch (order)
                    {
                        case SortOrder.Ascending:
                            item.fmt &= ~HDITEM.Format.SortDown;
                            item.fmt |= HDITEM.Format.SortUp;
                            break;
                        case SortOrder.Descending:
                            item.fmt &= ~HDITEM.Format.SortUp;
                            item.fmt |= HDITEM.Format.SortDown;
                            break;
                    }
                }
                else
                {
                    item.fmt &= ~HDITEM.Format.SortDown & ~HDITEM.Format.SortUp;
                }

                if (SendMessage(columnHeader, HDM_SETITEM, columnPtr, ref item) == IntPtr.Zero)
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }
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
    #endregion

    #region staus strip text too long make them become to "..."
    public class CustomRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item is ToolStripStatusLabel)
                TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont,
                    e.TextRectangle, e.TextColor, Color.Transparent,
                    e.TextFormat | TextFormatFlags.EndEllipsis);
            else
                base.OnRenderItemText(e);
        }
    }
    #endregion
}
