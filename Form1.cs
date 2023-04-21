﻿using rm.Trie;
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
    public partial class Manager : Form
    {
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        int tc = 0;
        int firstFoundI;
        int firstFoundJ;
        ListViewItemComparer sorter1 = new ListViewItemComparer();
        ListViewItemComparer sorter2 = new ListViewItemComparer();
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        //string diskNames = "TOSHIBA_white ADATA_TC TOSHIBAEXT TOSHIBAEXT ADATA_blue Seagate_4T_red My_Passport Transcend_blk TOSHIBA_blue Seagate_5T Seagate_8T_2 TOSHIBA_4T_BLK ADATA_4T WD_4T_blk ADATA_4T_DB TOSHIBAl_4T_blk Seagate_8T_1 My_Book";
        //string diskUUID = "2C09-1B0E E27B-DFE5 2C4E-6CA7 5225-CD70 0C5C-0819 C870-5B24 76C5-8D86 B0A2-4A76 2E59-1FAD 9A8D-D6C1 A41A-385D 48C0-9292 BEEB-03E6 B81B-45DD 4E97-3281 2675-36D0 1A5A-E56E A464-4794 FC4A-E051";
        string diskUUID = "F765-E051";
        bool over = false;
        bool getDataexc = false;
        bool lastIsFolder = true;
        bool ValreadyRun = false;
        bool FalreadyRun = false;
        bool firstSetColor = false;
        long VcmdT, VtotalT, FcmdT, FtotalT;
        public string value;
        List<int> lastSelect;
        List<int> lastFSelect = new List<int>();
        List<int> lastVSelect = new List<int>();
        ITrieMap<string> trieF = new TrieMap<string>();
        ITrieMap<string> trieV = new TrieMap<string>();

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

        public Manager()
        {
            this.Icon = Properties.Resources.favicon_20181126051103728;
            InitializeComponent();
            listView1.OwnerDraw = true;
            listView2.OwnerDraw = true;
            listView1.BringToFront();
            label1.BringToFront();
            pictureBox1.BringToFront();
            listViewItem = listView1;
            lastSelect = lastFSelect;
            this.ShowIcon = true;
            statusStrip1.Renderer = new CustomRenderer();
        }

        private void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.SubItem.Bounds);

                //add a 2 pixel buffer the match default behavior
                Rectangle rec = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 4, e.Bounds.Height - 4);

                //TODO  Confirm combination of TextFormatFlags.EndEllipsis and TextFormatFlags.ExpandTabs works on all systems.  MSDN claims they're exclusive but on Win7-64 they work.
                TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.ExpandTabs | TextFormatFlags.SingleLine;

                //If a different tabstop than the default is needed, will have to p/invoke DrawTextEx from win32.
                TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.ListView.Font, rec, Color.White, flags);
            }
            else
            {
                e.DrawBackground();
                //add a 2 pixel buffer the match default behavior
                Rectangle rec = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 4, e.Bounds.Height - 4);

                //TODO  Confirm combination of TextFormatFlags.EndEllipsis and TextFormatFlags.ExpandTabs works on all systems.  MSDN claims they're exclusive but on Win7-64 they work.
                TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.ExpandTabs | TextFormatFlags.SingleLine;

                //If a different tabstop than the default is needed, will have to p/invoke DrawTextEx from win32.
                TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.ListView.Font, rec, Color.Black, flags);
            }
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

        private string CommandOutput(string commandText, string filename)
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
                /*p.WaitForExit();
                p.Close();
                if(!File.Exists(filename))
                {
                    return "";
                }
                strOutput = File.ReadAllText(filename, Encoding.UTF8);*/
                strOutput = p.StandardOutput.ReadToEnd();//匯出整個執行過程
                p.WaitForExit();
                p.Close();
                string lastStr = "More? )\r\n";
                int start = strOutput.LastIndexOf(lastStr);
                start = start >= 0? (start+ lastStr.Length) : 0;
                int len = strOutput.LastIndexOf("\r\nexit");
                len = len >= 0? (len - start) : strOutput.Length;
                strOutput = strOutput.Substring(start, len);
            }
            catch
            {
                p.Close();
                strOutput = "";
                //strOutput = e.Message;
            }
            return strOutput;
        }

        private void setListViewSize()
        {
            int x = 20;
            int div = 2;
            if (!FolderR.Checked) div = 3;
            int lstW = (listViewItem.Width - x) / div;
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
            if (firstSetColor &&
                (listViewItem.Items.Count > 0 && listViewItem.Items[0].SubItems[0].BackColor == Color.WhiteSmoke))
                return;
            Color[] back = { Color.WhiteSmoke, Color.Gainsboro };
            for (int i = 0; i < listViewItem.Items.Count; ++i)
            {
                Color c = back[i & 1];
                for (int j = 0; j < listViewItem.Items[i].SubItems.Count; ++j)
                    listViewItem.Items[i].SubItems[j].BackColor = c;
            }
            firstSetColor = true;
        }

        private void setLastFocus()
        {
            for (int i = 0; i < lastSelect.Count; ++i)
            {
                listViewItem.Items[lastSelect[i]].Selected = true;
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
            trieF.Clear();
            Stopwatch sw = new Stopwatch();//Stopwatch類別在System.Diagnostics命名空間裡
            sw.Reset();
            sw = Stopwatch.StartNew();
            string[] filter = { ".", "..", "白馬下載器", "新增資料夾", "Sp Service", "System" };
            string tmpFile = "tmpFolder.txt";
            //string[] tmp = File.ReadAllLines(desktop + @"\o");
/*            if (File.Exists(tmpFile))
            {
                File.Delete(tmpFile);
            }
            else
            {
                File.AppendText(tmpFile);
            }
            string[] tmp = CommandOutput(String.Format(@"
for %p in (D E F G H I J K L M N O P Q R S T U V W X Y Z) do (
if exist %p:\Data ( 
for /f %i in ('vol %p: ^| findstr ""{0}""') do (
if ""%i"" NEQ """" (
for /F ""tokens=*"" %A in ('dir /ad/b %p:\Data') do @echo %p:\Data\%A >> ""{1}""
)
)
)
)", diskUUID, tmpFile), tmpFile).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);*/
            string[] tmp = CommandOutput(String.Format(@"
for %p in (D E F G H I J K L M N O P Q R S T U V W X Y Z) do (
if exist %p:\Data ( 
for /f %i in ('vol %p: ^| findstr ""{0}""') do (
if ""%i"" NEQ """" (
for /F ""tokens=*"" %A in ('dir /ad/b %p:\Data') do @echo %p:\Data\%A
)
)
)
)", diskUUID), tmpFile).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

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
                    {
                        string f = t.Substring(t.LastIndexOf("\\") + 1);
                        aryF.Add(new ListViewItem(new string[] { f, loc }));
                        trieF.Add(f, loc + f);
                    }
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
            trieV.Clear();
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
            string main_cmd = "dir /b /s /a-d";
            string ignoreUnfind = " 2>nul";
            //string cmd = main_cmd + string.Join(ignoreUnfind + " & " + main_cmd, allTypes) + ignoreUnfind;
            string cmd = main_cmd + separate_pre + string.Join(separate_pre, videoTypes) + ignoreUnfind;
            string tmpFile = "tmpVedio.txt";
            //string[] tmp = File.ReadAllLines(desktop + @"\o");
/*            if (File.Exists(tmpFile))
            {
                File.Delete(tmpFile);
            }
            else
            {
                File.AppendText(tmpFile);
            }
            string[] tmp = CommandOutput(String.Format(@"
for %p in (D E F G H I J K L M N O P Q R S T U V W X Y Z) do ( 
if exist %p:\Data\ (
for /f %i in ('vol %p: ^| findstr ""{0}""') do ( 
if ""%i"" NEQ """" ( 
{1} >> {2}
)
)
)
)", diskUUID, cmd, tmpFile), tmpFile).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);*/
            string[] tmp = CommandOutput(String.Format(@"
for %p in (D E F G H I J K L M N O P Q R S T U V W X Y Z) do ( 
if exist %p:\Data\ (
for /f %i in ('vol %p: ^| findstr ""{0}""') do ( 
if ""%i"" NEQ """" ( 
{1}
)
)
)
)", diskUUID, cmd), tmpFile).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            VcmdT = sw.ElapsedMilliseconds;
            string[] tmpstr;
            string actress = "";
            string loc = "";
            List<string> actressLst = new List<string>();
            string tmpSerialNum = "";
            int backDash = 0;
            byte n = 0;
            for (char i = 'D'; i <= 'Z' && !n.Equals(2); ++i)
            {
                try
                {
                    actressLst.AddRange(File.ReadAllLines(string.Format(@"{0}:\Data\多人1\人名1.txt", i)));
                    n++;
                }
                catch { }
                try
                {
                    actressLst.AddRange(File.ReadAllLines(string.Format(@"{0}:\Data\多人2\人名2.txt", i)));
                    n++;
                }
                catch { }
                try
                {
                    actressLst.AddRange(File.ReadAllLines(string.Format(@"{0}:\Data\VR\多人\人名.txt", i)));
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
                if (loc.Contains(@"\多人1\") || loc.Contains(@"\多人2\"))
                {
                    try
                    {
                        tmpSerialNum = t.Substring(backDash);
                        actressList.TryGetValue(tmpSerialNum.Substring(0, tmpSerialNum.IndexOf('.')).TrimEnd('C'), out actress);
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
                {
                    string file = tmpstr[tmpstr.Length - 1];
                    aryV.Add(new ListViewItem(new string[] { file, actress.Trim(), loc }));
                    int dot = file.LastIndexOf('.');
                    string serial = file.Substring(0, dot > 0 ? dot : file.Length).Trim();
                    trieV.Add(serial, loc + file);
                    if (Regex.IsMatch(serial, @"^(\d+)"))
                        trieV.Add(Regex.Replace(serial, @"^(\d+)", ""), loc + file);
                }
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
                    string copyStr = "";
                    if (n > 0) copyStr = listViewItem.Items[i].SubItems[n].Text;
                    else
                    {
                        copyStr = listViewItem.Items[i].SubItems[listViewItem.Items[i].SubItems.Count - 1].Text + listViewItem.Items[i].SubItems[0].Text;
                    }
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
                        case Keys.M: multiFind(); e.SuppressKeyPress = true; break;
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
                        case Keys.F4: copyOtherColumnToClip(-1); e.SuppressKeyPress = true; break;
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
                    if (listViewItem.Items[i].SubItems[j].Text.IndexOf(ToNarrow(searchText.Text).Trim(new char[] { ' ', '\t', '\n' }), StringComparison.OrdinalIgnoreCase) >= 0)
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
            firstSetColor = false;
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

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;
            textBox.Multiline = true;
            // Add vertical scroll bars to the TextBox control.
            textBox.ScrollBars = ScrollBars.Vertical;
            // Allow the RETURN key to be entered in the TextBox control.
            textBox.AcceptsReturn = true;
            // Allow the TAB key to be entered in the TextBox control.
            textBox.AcceptsTab = true;
            // Set WordWrap to true to allow text to wrap to the next line.
            textBox.WordWrap = true;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            /*textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);*/

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            //form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            //form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void multiFind()
        {
            if (new InputForm(this).ShowDialog() == DialogResult.Cancel) return;
            StringBuilder tgt_sb = new StringBuilder();
            StringBuilder fnd_sb = new StringBuilder();
            StringBuilder clr_sb = new StringBuilder();
            ITrieMap<string> trieMap = null;

            if (VideoR.Checked) trieMap = trieV;
            else trieMap = trieF;

            foreach (string s in value.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                int backSlash = s.LastIndexOf('\\');
                string str = s.Substring(backSlash > 0 ? backSlash + 1 : 0);
                int dot = str.LastIndexOf('.');
                str = str.Substring(0, dot > 0 ? dot : str.Length).Trim();
                str = str.TrimStart("1234567890".ToCharArray());
                str = ToNarrow(str).Trim(new char[] { ' ', '\t', '\n' });
                string found = str;

                if (trieMap.HasKey(str))
                {
                    fnd_sb.Append(trieMap.ValueBy(str) + '\t');
                    clr_sb.Append("1\t");
                }
                else
                {
                    fnd_sb.Append(str + '\t');
                    clr_sb.Append("0\t");
                }

                tgt_sb.Append(s.Trim() + '\t');

                /*for (int i = 0; i < listViewItem.Items.Count; i++)
                {
                    for (int j = 0; j < listViewItem.Items[i].SubItems.Count; ++j)
                    {
                        //System.Console.WriteLine(listViewItem.Items[i].SubItems[j].Text);
                        if (listViewItem.Items[i].SubItems[j].Text.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            found = listViewItem.Items[i].SubItems[listViewItem.Items[i].SubItems.Count - 1].Text + listViewItem.Items[i].SubItems[0].Text;
                            //System.Console.WriteLine("found");
                            break;
                        }
                    }
                    if (found != str) break;
                }
                tgt_sb.Append(s.Trim() + '\t');
                //System.Console.WriteLine(found);
                if (found == str)
                {
                    fnd_sb.Append(found + '\t');
                    clr_sb.Append("0\t");
                }
                else
                {
                    fnd_sb.Append(found + '\t');
                    clr_sb.Append("1\t");
                }*/
            }
            /*for (int i = 1; i < listViewItem.Items.Count; i++)
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
            }*/
            new Form3(tgt_sb.ToString(), fnd_sb.ToString(), clr_sb.ToString()).Show();

        }

        private void findDeepDuplicate()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            HashSet<string> complete = new HashSet<string>();
            string f1, f2;
            string p1, p2;
            bool add = false;
            int count = 0;
            for (int i = 1; i < listViewItem.Items.Count; i++)
            {
                ListViewItem item = listViewItem.Items[i];
                f1 = item.Text.Substring(0, item.Text.LastIndexOf('.'));
                p1 = item.SubItems[item.SubItems.Count - 1].Text + item.SubItems[0].Text + '\t';
                if (complete.Contains(p1))
                    continue;
                for (int j = 0; j < i; ++j)
                {
                    ListViewItem item2 = listViewItem.Items[j];
                    f2 = item2.Text.Substring(0, item2.Text.LastIndexOf('.'));
                    if (f1.Contains(f2) || f2.Contains(f1))
                    {
                        p2 = item2.SubItems[item2.SubItems.Count - 1].Text + item2.SubItems[0].Text + '\t';
                        if (add)
                        {
                            sb.Append(p2);
                            complete.Add(p2);
                            count++;
                        }
                        else
                        {
                            sb.Append(p1);
                            sb.Append(p2);
                            complete.Add(p1);
                            complete.Add(p2);
                            count += 2;
                        }
                        add = true;
                    }
                    else
                    {
                        if (add)
                        {
                            sb2.Append(count.ToString() + '\t');
                            add = false;
                        }
                    }
                }
            }
            if (sb.Length > 0) new Form2(sb.ToString(), sb2.ToString()).Show();
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
                f1 = VideoR.Checked ? listViewItem.Items[pos].Text.Substring(0, listViewItem.Items[pos].Text.LastIndexOf('.')) : listViewItem.Items[pos].Text;
                f2 = VideoR.Checked ? listViewItem.Items[i].Text.Substring(0, listViewItem.Items[i].Text.LastIndexOf('.')) : listViewItem.Items[i].Text;
                if ((f1.Contains(f2) || f2.Contains(f1)) && f1 != (f2 + 'C') && f2 != (f1 + 'C'))
                {
                    ListViewItem item = listViewItem.Items[i];
                    if (pos == i - 1)
                    {
                        ListViewItem item_p = listViewItem.Items[pos];
                        sb.Append(item_p.SubItems[item_p.SubItems.Count - 1].Text + item_p.SubItems[0].Text + '\t');
                    }
                    sb.Append(item.SubItems[item.SubItems.Count - 1].Text + item.SubItems[0].Text + '\t');
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
            if (string.IsNullOrWhiteSpace(searchText.Text))
            {
                if (Control.ModifierKeys == Keys.Control) findDeepDuplicate();
                else findDuplicate();
            }

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

        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern void ILFree(IntPtr pidlList);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern IntPtr ILCreateFromPathW(string pszPath);

        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);

        public static void ExplorerFile(string filePath)
        {
            if (!File.Exists(filePath) && !Directory.Exists(filePath))
                return;

            IntPtr pidlList = ILCreateFromPathW(filePath);
            if (pidlList == IntPtr.Zero) return;
            try
            {
                Marshal.ThrowExceptionForHR(SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
            }
            finally
            {
                ILFree(pidlList);
            }
        }

        private void openFile()
        {
            string file = "";
            bool b = false;
            for (int i = 0; i < listViewItem.SelectedItems.Count; ++i)
            {
                for (int j = 0; j < listViewItem.Items.Count; ++j)
                {
                    if (listViewItem.Items[j] == listViewItem.SelectedItems[i])
                    {
                        b = true;
                        if (listViewItem.Items[j].SubItems.Count == 2)
                        {
                            file = String.Format("\"{0}{1}\"", listViewItem.Items[j].SubItems[1].Text, listViewItem.SelectedItems[i].Text);
                            Process p = new Process();
                            p.StartInfo.FileName = @"C:\Windows\explorer.exe";
                            p.StartInfo.Arguments = file;
                            p.Start();
                            p.WaitForExit();
                            p.Close();
                            p.Dispose();
                            setDetail("");
                            setDetail("資料夾開啟中...");
                        }
                        else
                        {
                            file = String.Format("\"{0}{1}\"", listViewItem.Items[j].SubItems[2].Text, listViewItem.SelectedItems[i].Text);
                            Process p = new Process();
                            p.StartInfo.FileName = file;
                            p.Start();
                            //p.WaitForExit();
                            p.Close();
                            p.Dispose();
                            setDetail("檔案開啟中...");
                        }
                        break;
                    }
                }
                if (b) break;
            }
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
                        ExplorerFile(selected + fileName);
                        setDetail("資料夾開啟中...");
                        //new Thread(() => waitForExplorer(selected + fileName)).Start();
                        b = true;
                        break;
                    }
                }
                if (b) break;
            }
            setDetail("");
            if (!b) return;
        }

        private void openWithFileManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFolder();
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            if (Form.ModifierKeys == Keys.Alt) openFolder();
            else openFile();
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
            if (new Regex(@"^[\s　\t\n]").IsMatch(searchText.Text) || new Regex(@"[\s　\t\n]$").IsMatch(searchText.Text))
            {
                searchText.Text = searchText.Text.Trim();
                searchText.Focus();
                searchText.SelectionStart = searchText.Text.Length;
            }
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
