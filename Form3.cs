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
        public Form3(string tgt, string fnd, string found)
        {
            InitializeComponent();
            string[] tgt_ary = tgt.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] fnd_ary = fnd.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tgt_ary.Length; ++i)
            {
                dataGridView1.Rows.Add(tgt_ary[i], fnd_ary[i]);
            }
            string[] founds = found.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Color[] color = { Color.WhiteSmoke, Color.Gainsboro };
            for (int i = 0; i < founds.Length; ++i)
            {
                dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = color[Convert.ToInt32(founds[i])];
            }
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            //for (int i = 1; i < dataGridView1.ColumnCount; ++i)
            //    dataGridView1.Columns[i].Width = this.Width / dataGridView1.ColumnCount;
        }

        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern void ILFree(IntPtr pidlList);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern IntPtr ILCreateFromPathW(string pszPath);

        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);

        public static void ExplorerFile(string filePath)
        {
            Console.WriteLine("try open" + filePath);
            if (!File.Exists(filePath) && !Directory.Exists(filePath))
            {
                MessageBox.Show(String.Format("Can not open {0}", filePath), "Open error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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

        private void openFile(string path)
        {
            if (!File.Exists(path)){
                MessageBox.Show(String.Format("Can not open {0}", path), "Open error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Process p = new Process();
            p.StartInfo.FileName = dataGridView1.SelectedCells[0].Value.ToString();
            p.Start();
            p.Close();
            p.Dispose();
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 1)
            {
                string path = dataGridView1.SelectedCells[0].Value.ToString();

                if (Form.ModifierKeys == Keys.Control)
                {
                    //System.Console.WriteLine(dataGridView1.SelectedCells[0].Value.ToString());
                    ExplorerFile(path);
                }
                else
                {
                    openFile(path);
                }
            }

        }

        private void dataGridView1_Key(object sender, KeyEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 1)
            {
                string file = dataGridView1.SelectedCells[0].Value.ToString();
                switch (e.KeyCode)
                {
                    case Keys.F2:
                        using (var form = new rename_form(file))
                        {
                            if (form.ShowDialog() != DialogResult.Yes) return;
                            dataGridView1.SelectedCells[0].Value = form.new_name;
                        }
                        break;
                    case Keys.C:
                        if (Control.ModifierKeys == Keys.Control)
                        {
                            Clipboard.SetText(file);
                        }
                        break;
                    case Keys.O:
                        if (Control.ModifierKeys == Keys.Control)
                        {
                            ExplorerFile(file);
                        }
                        else if(Control.ModifierKeys == Keys.Alt)
                        {
                            openFile(file);
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
                                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedCells[0].RowIndex);
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
