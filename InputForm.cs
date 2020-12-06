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
    public partial class InputForm : Form
    {
        Manager form;
        public InputForm(Manager form)
        {
            InitializeComponent();
            this.form = form;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            form.value = textBox1.Text;
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
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

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.UnicodeText, true))
            {
                var data = e.Data.GetData(DataFormats.UnicodeText, true);
                if (data != null)
                {
                    if (data is string)
                    {
                        textBox1.Text = data as string; // done!
                    }
                }
            }
        }
    }
}
