using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            string arg = "";
            string file = @"C:\Windows\explorer.exe";
            if (control) arg = "/select, ";
            arg += listView1.SelectedItems[0].Text;
            p.StartInfo.FileName = file;
            p.StartInfo.Arguments = arg;
            p.Start();
        }

        private void control_press(object sender, KeyEventArgs e)
        {
            control = e.Control;
        }
    }
}
