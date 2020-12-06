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
            ext_txt.TextChanged += txtContents_TextChanged;
            folder_txt.TextChanged += txtContents_TextChanged;
            ori_file = name;
            ext_txt.Text = name.Substring(name.LastIndexOf('.'));
            int bk_slash = name.LastIndexOf('\\');
            folder_txt.Text = name.Substring(0, bk_slash + 1);
            name_txtbx.Text = name.Substring(bk_slash + 1, name.LastIndexOf('.') - bk_slash - 1);
            name_txtbx.SelectAll();
        }

        private void rename_btn_Click(object sender, EventArgs e)
        {
            rename();
        }

        private void rename()
        {
            if (File.Exists(ori_file))
            {
                new_name = folder_txt.Text + name_txtbx.Text + ext_txt.Text;
                if(new_name != ori_file)
                    File.Move(ori_file, new_name);
            }
        }

        private void txtContents_TextChanged(object sender, EventArgs e)
        {
            AutoSizeTextBox(sender as TextBox);
        }

        // Make the TextBox fit its contents.
        private void AutoSizeTextBox(TextBox txtbx)
        {
            int left_margin = txtbx.Margin.Left;
            int top_margin = txtbx.Margin.Top;
            Size size = TextRenderer.MeasureText(txtbx.Text, txtbx.Font);
            txtbx.ClientSize =
                new Size(size.Width + left_margin, size.Height + top_margin);
        }

        private void name_txtbx_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    cancel_btn.PerformClick();
                    break;
                case Keys.Enter:
                    rename_btn.PerformClick();
                    break;
                default:
                    break;
            }
        }
    }
}
