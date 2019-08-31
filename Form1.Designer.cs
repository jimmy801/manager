namespace WindowsFormsApplication1
{
    partial class AV管家
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listView1 = new System.Windows.Forms.ListView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWithFileManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rldBTN = new System.Windows.Forms.Button();
            this.toolPanel = new System.Windows.Forms.Panel();
            this.BtnPanel = new System.Windows.Forms.Panel();
            this.rdmBTN = new System.Windows.Forms.Button();
            this.radioPanel = new System.Windows.Forms.Panel();
            this.VideoR = new System.Windows.Forms.RadioButton();
            this.FolderR = new System.Windows.Forms.RadioButton();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.searchText = new System.Windows.Forms.TextBox();
            this.searchBTN = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.listView2 = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.total = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1.SuspendLayout();
            this.toolPanel.SuspendLayout();
            this.BtnPanel.SuspendLayout();
            this.radioPanel.SuspendLayout();
            this.searchPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(535, 451);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            this.listView1.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
            this.listView1.Leave += new System.EventHandler(this.listView_Leave);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.openWithFileManagerToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(210, 48);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // openWithFileManagerToolStripMenuItem
            // 
            this.openWithFileManagerToolStripMenuItem.Name = "openWithFileManagerToolStripMenuItem";
            this.openWithFileManagerToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.openWithFileManagerToolStripMenuItem.Text = "Open with File Manager";
            this.openWithFileManagerToolStripMenuItem.Click += new System.EventHandler(this.openWithFileManagerToolStripMenuItem_Click);
            // 
            // rldBTN
            // 
            this.rldBTN.AutoSize = true;
            this.rldBTN.Location = new System.Drawing.Point(84, 3);
            this.rldBTN.Name = "rldBTN";
            this.rldBTN.Size = new System.Drawing.Size(56, 24);
            this.rldBTN.TabIndex = 5;
            this.rldBTN.Text = "Reload";
            this.rldBTN.UseVisualStyleBackColor = true;
            this.rldBTN.Click += new System.EventHandler(this.rldBTN_Click);
            this.rldBTN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.openAndCopyKeyDown);
            // 
            // toolPanel
            // 
            this.toolPanel.Controls.Add(this.BtnPanel);
            this.toolPanel.Controls.Add(this.radioPanel);
            this.toolPanel.Controls.Add(this.searchPanel);
            this.toolPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolPanel.Location = new System.Drawing.Point(0, 0);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new System.Drawing.Size(535, 43);
            this.toolPanel.TabIndex = 5;
            // 
            // BtnPanel
            // 
            this.BtnPanel.AutoSize = true;
            this.BtnPanel.Controls.Add(this.rdmBTN);
            this.BtnPanel.Controls.Add(this.rldBTN);
            this.BtnPanel.Location = new System.Drawing.Point(382, 4);
            this.BtnPanel.Name = "BtnPanel";
            this.BtnPanel.Size = new System.Drawing.Size(143, 31);
            this.BtnPanel.TabIndex = 7;
            // 
            // rdmBTN
            // 
            this.rdmBTN.Location = new System.Drawing.Point(3, 3);
            this.rdmBTN.Name = "rdmBTN";
            this.rdmBTN.Size = new System.Drawing.Size(75, 24);
            this.rdmBTN.TabIndex = 4;
            this.rdmBTN.Text = "Random";
            this.rdmBTN.UseVisualStyleBackColor = true;
            this.rdmBTN.Click += new System.EventHandler(this.rdm_Click);
            this.rdmBTN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.openAndCopyKeyDown);
            // 
            // radioPanel
            // 
            this.radioPanel.AutoSize = true;
            this.radioPanel.Controls.Add(this.VideoR);
            this.radioPanel.Controls.Add(this.FolderR);
            this.radioPanel.Location = new System.Drawing.Point(3, 10);
            this.radioPanel.Name = "radioPanel";
            this.radioPanel.Size = new System.Drawing.Size(113, 26);
            this.radioPanel.TabIndex = 7;
            // 
            // VideoR
            // 
            this.VideoR.AutoSize = true;
            this.VideoR.Location = new System.Drawing.Point(59, 4);
            this.VideoR.Name = "VideoR";
            this.VideoR.Size = new System.Drawing.Size(51, 16);
            this.VideoR.TabIndex = 1;
            this.VideoR.Text = "Video";
            this.VideoR.UseVisualStyleBackColor = true;
            this.VideoR.CheckedChanged += new System.EventHandler(this.Condition_CheckedChanged);
            this.VideoR.KeyDown += new System.Windows.Forms.KeyEventHandler(this.openAndCopyKeyDown);
            // 
            // FolderR
            // 
            this.FolderR.AutoSize = true;
            this.FolderR.Checked = true;
            this.FolderR.Location = new System.Drawing.Point(3, 4);
            this.FolderR.Name = "FolderR";
            this.FolderR.Size = new System.Drawing.Size(53, 16);
            this.FolderR.TabIndex = 0;
            this.FolderR.TabStop = true;
            this.FolderR.Text = "Folder";
            this.FolderR.UseVisualStyleBackColor = true;
            this.FolderR.CheckedChanged += new System.EventHandler(this.Condition_CheckedChanged);
            this.FolderR.KeyDown += new System.Windows.Forms.KeyEventHandler(this.openAndCopyKeyDown);
            // 
            // searchPanel
            // 
            this.searchPanel.AutoSize = true;
            this.searchPanel.Controls.Add(this.searchText);
            this.searchPanel.Controls.Add(this.searchBTN);
            this.searchPanel.Location = new System.Drawing.Point(197, 7);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(160, 31);
            this.searchPanel.TabIndex = 5;
            // 
            // searchText
            // 
            this.searchText.AllowDrop = true;
            this.searchText.ImeMode = System.Windows.Forms.ImeMode.On;
            this.searchText.Location = new System.Drawing.Point(3, 3);
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(100, 22);
            this.searchText.TabIndex = 2;
            this.searchText.TextChanged += new System.EventHandler(this.searchText_TextChanged);
            this.searchText.DragDrop += new System.Windows.Forms.DragEventHandler(this.searchText_DragDrop);
            this.searchText.DragEnter += new System.Windows.Forms.DragEventHandler(this.searchText_DragEnter);
            this.searchText.DoubleClick += new System.EventHandler(this.textBox1_DoubleClick);
            this.searchText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // searchBTN
            // 
            this.searchBTN.Location = new System.Drawing.Point(103, 2);
            this.searchBTN.Name = "searchBTN";
            this.searchBTN.Size = new System.Drawing.Size(52, 23);
            this.searchBTN.TabIndex = 3;
            this.searchBTN.Text = "search";
            this.searchBTN.UseVisualStyleBackColor = true;
            this.searchBTN.Click += new System.EventHandler(this.searchBTN_Click);
            this.searchBTN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.openAndCopyKeyDown);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.statusStrip1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 43);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(535, 473);
            this.panel2.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.listView2);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.pictureBox1);
            this.panel3.Controls.Add(this.listView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(535, 451);
            this.panel3.TabIndex = 3;
            // 
            // listView2
            // 
            this.listView2.ContextMenuStrip = this.contextMenuStrip1;
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.listView2.Location = new System.Drawing.Point(0, 0);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(535, 451);
            this.listView2.TabIndex = 8;
            this.listView2.TabStop = false;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.Visible = false;
            this.listView2.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            this.listView2.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            this.listView2.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            this.listView2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
            this.listView2.Leave += new System.EventHandler(this.listView_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(240, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "載入中...";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::WindowsFormsApplication1.Properties.Resources.loading;
            this.pictureBox1.Location = new System.Drawing.Point(220, 16);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(15, 15);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.total});
            this.statusStrip1.Location = new System.Drawing.Point(0, 451);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(535, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // total
            // 
            this.total.Name = "total";
            this.total.Size = new System.Drawing.Size(520, 17);
            this.total.Spring = true;
            this.total.Text = "處理中";
            this.total.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AV管家
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 516);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolPanel);
            this.MinimumSize = new System.Drawing.Size(450, 298);
            this.Name = "AV管家";
            this.Text = "AV管家";
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AV管家_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolPanel.ResumeLayout(false);
            this.toolPanel.PerformLayout();
            this.BtnPanel.ResumeLayout(false);
            this.BtnPanel.PerformLayout();
            this.radioPanel.ResumeLayout(false);
            this.radioPanel.PerformLayout();
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button rldBTN;
        private System.Windows.Forms.Panel toolPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel total;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.TextBox searchText;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.Button searchBTN;
        private System.Windows.Forms.ToolStripMenuItem openWithFileManagerToolStripMenuItem;
        private System.Windows.Forms.Panel radioPanel;
        private System.Windows.Forms.RadioButton VideoR;
        private System.Windows.Forms.RadioButton FolderR;
        private System.Windows.Forms.Panel BtnPanel;
        private System.Windows.Forms.Button rdmBTN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListView listView2;
    }
}

