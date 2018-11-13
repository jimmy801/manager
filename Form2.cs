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

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            string arg;
            string file = @"C:\Windows\explorer.exe";
            arg = "/select, " + listView1.SelectedItems[0].Text;
            p.StartInfo.FileName = file;
            p.StartInfo.Arguments = arg;
            p.Start();
        }
    }
}
