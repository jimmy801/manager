using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form3 : Form
    {
        bool control;
        public Form3(string str, string found)
        {
            InitializeComponent();
            List<ListViewItem> ary = new List<ListViewItem>();
            foreach (var s in str.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                ary.Add(new ListViewItem(s));
            listView1.Items.AddRange(ary.ToArray());
            string[] founds = found.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Color[] color = { Color.WhiteSmoke, Color.Gainsboro };
            for (int i = 0; i < listView1.Items.Count; ++i)
            {
                listView1.Items[i].BackColor = color[Convert.ToInt32(founds[i])];
                listView1.Items[i].ForeColor = Color.Black;
            }
            control = false;
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


        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                if (control)
                {
                    ExplorerFile(listView1.SelectedItems[0].Text);
                }
                else
                {
                    Process p = new Process();
                    p.StartInfo.FileName = listView1.SelectedItems[0].Text;
                    p.Start();
                    p.Close();
                    p.Dispose();
                }
            }
        }

        private void listView1_Key(object sender, KeyEventArgs e)
        {
            control = e.Control;
            if(listView1.SelectedItems.Count == 1)
            {
                string file = listView1.SelectedItems[0].Text;
                switch (e.KeyCode)
                {
                    case Keys.F2:
                        using(var form = new rename_form(file))
                        {
                            if(form.ShowDialog() != DialogResult.Yes) return;
                            listView1.SelectedItems[0].Text = form.new_name;
                        }
                        break;
                    case Keys.Delete:
                        DialogResult dialogResult = MessageBox.Show(String.Format("Delete {0}?", file), "Delete file", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            if (File.Exists(file))
                            {
                                try
                                {
                                    File.Delete(file);
                                    listView1.Items.Remove(listView1.SelectedItems[0]);
                                }

                                catch (IOException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
