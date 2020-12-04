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

namespace WindowsFormsApplication1
{
    public partial class rename_form : Form
    {
        public string new_name;
        string ori_file;
        public rename_form(string name)
        {
            InitializeComponent();
            ori_file = name;
            ext_lbl.Text = name.Substring(name.LastIndexOf('.'));
            int bk_slash = name.LastIndexOf('\\');
            folder_lbl.Text = name.Substring(0, bk_slash + 1);
            name_txtbx.Text = name.Substring(bk_slash + 1, name.LastIndexOf('.') - bk_slash - 1);
            name_txtbx.SelectAll();
        }

        private void rename_btn_Click(object sender, EventArgs e)
        {
            rename();
        }

        private void rename()
        {
            if (Directory.Exists(ori_file))
            {
                new_name = folder_lbl.Text + name_txtbx.Text;
                File.Move(ori_file, new_name);
            }
        }

        private void name_txtbx_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.Enter:
                    rename();
                    break;
                default:
                    break;
            }
        }
    }
}
