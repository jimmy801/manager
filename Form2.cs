using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2(string str, string pos)
        {
            InitializeComponent();
            List<ListViewItem> ary = new List<ListViewItem>();
            foreach (var s in str.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                ary.Add(new ListViewItem(s));
            listView1.Items.AddRange(ary.ToArray());
            string[] colorPos = pos.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Color []color = { Color.WhiteSmoke, Color.Gainsboro};
            for (int i = 0, j = 0; i < listView1.Items.Count; ++i)
            {
                if (i == Convert.ToInt32(colorPos[j]))
                {
                    ++j;
                }
                listView1.Items[i].BackColor = color[j % 2];
                listView1.Items[i].ForeColor = Color.Black;
            }
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

            ExplorerFile(listView1.SelectedItems[0].Text);
        }
    }
}
