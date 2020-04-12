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
        AV管家 form;
        public InputForm(AV管家 form)
        {
            InitializeComponent();
            this.form = form;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            form.value = textBox1.Text;
        }
    }
}
