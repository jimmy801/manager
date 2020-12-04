namespace WindowsFormsApplication1
{
    partial class rename_form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rename_btn = new System.Windows.Forms.Button();
            this.cancel_btn = new System.Windows.Forms.Button();
            this.name_txtbx = new System.Windows.Forms.TextBox();
            this.ext_lbl = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.folder_lbl = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rename_btn
            // 
            this.rename_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rename_btn.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.rename_btn.Location = new System.Drawing.Point(3, 3);
            this.rename_btn.Name = "rename_btn";
            this.rename_btn.Size = new System.Drawing.Size(257, 29);
            this.rename_btn.TabIndex = 0;
            this.rename_btn.Text = "rename";
            this.rename_btn.UseVisualStyleBackColor = true;
            this.rename_btn.Click += new System.EventHandler(this.rename_btn_Click);
            // 
            // cancel_btn
            // 
            this.cancel_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel_btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel_btn.Location = new System.Drawing.Point(266, 3);
            this.cancel_btn.Name = "cancel_btn";
            this.cancel_btn.Size = new System.Drawing.Size(257, 29);
            this.cancel_btn.TabIndex = 1;
            this.cancel_btn.Text = "cancel";
            this.cancel_btn.UseVisualStyleBackColor = true;
            // 
            // name_txtbx
            // 
            this.name_txtbx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.name_txtbx.Location = new System.Drawing.Point(9, 3);
            this.name_txtbx.Name = "name_txtbx";
            this.name_txtbx.Size = new System.Drawing.Size(508, 22);
            this.name_txtbx.TabIndex = 2;
            this.name_txtbx.KeyDown += new System.Windows.Forms.KeyEventHandler(this.name_txtbx_KeyDown);
            // 
            // ext_lbl
            // 
            this.ext_lbl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ext_lbl.AutoSize = true;
            this.ext_lbl.Location = new System.Drawing.Point(523, 0);
            this.ext_lbl.Name = "ext_lbl";
            this.ext_lbl.Size = new System.Drawing.Size(1, 32);
            this.ext_lbl.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ext_lbl, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.name_txtbx, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.folder_lbl, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(526, 32);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // folder_lbl
            // 
            this.folder_lbl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.folder_lbl.AutoSize = true;
            this.folder_lbl.Location = new System.Drawing.Point(3, 0);
            this.folder_lbl.Name = "folder_lbl";
            this.folder_lbl.Size = new System.Drawing.Size(1, 32);
            this.folder_lbl.TabIndex = 4;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.cancel_btn, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.rename_btn, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 32);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(526, 35);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // rename_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 67);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "rename_form";
            this.Text = "rename_form";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button rename_btn;
        private System.Windows.Forms.Button cancel_btn;
        private System.Windows.Forms.TextBox name_txtbx;
        private System.Windows.Forms.Label ext_lbl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label folder_lbl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}