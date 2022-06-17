
namespace PageEditor
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.新規作成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.開くToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.クリップボードにコピーToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.sheetListBox = new System.Windows.Forms.ListBox();
            this.layerListBox = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.sheetMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.新規シートの作成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.シート名の変更ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.シート削除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layerMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.新規吹き出しレイヤToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新規イメージレイヤToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新規塗り潰しレイヤToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.レイヤ削除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.helpLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.sheetMenu.SuspendLayout();
            this.layerMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1108, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新規作成ToolStripMenuItem,
            this.toolStripMenuItem3,
            this.開くToolStripMenuItem,
            this.toolStripMenuItem4,
            this.保存ToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(82, 22);
            this.toolStripDropDownButton1.Text = "ワークスペース";
            // 
            // 新規作成ToolStripMenuItem
            // 
            this.新規作成ToolStripMenuItem.Name = "新規作成ToolStripMenuItem";
            this.新規作成ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.新規作成ToolStripMenuItem.Text = "新規作成";
            this.新規作成ToolStripMenuItem.Click += new System.EventHandler(this.新規作成ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(119, 6);
            // 
            // 開くToolStripMenuItem
            // 
            this.開くToolStripMenuItem.Name = "開くToolStripMenuItem";
            this.開くToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.開くToolStripMenuItem.Text = "開く";
            this.開くToolStripMenuItem.Click += new System.EventHandler(this.開くToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(119, 6);
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.保存ToolStripMenuItem.Text = "保存";
            this.保存ToolStripMenuItem.Click += new System.EventHandler(this.保存ToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.クリップボードにコピーToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(55, 22);
            this.toolStripDropDownButton2.Text = "イメージ";
            // 
            // クリップボードにコピーToolStripMenuItem
            // 
            this.クリップボードにコピーToolStripMenuItem.Name = "クリップボードにコピーToolStripMenuItem";
            this.クリップボードにコピーToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.クリップボードにコピーToolStripMenuItem.Text = "クリップボードにコピー";
            this.クリップボードにコピーToolStripMenuItem.Click += new System.EventHandler(this.クリップボードにコピーToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel2.SizeChanged += new System.EventHandler(this.splitContainer1_Panel2_SizeChanged);
            this.splitContainer1.Size = new System.Drawing.Size(1108, 523);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.sheetListBox);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.layerListBox);
            this.splitContainer2.Size = new System.Drawing.Size(260, 523);
            this.splitContainer2.SplitterDistance = 316;
            this.splitContainer2.TabIndex = 0;
            // 
            // sheetListBox
            // 
            this.sheetListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sheetListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sheetListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.sheetListBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.sheetListBox.FormattingEnabled = true;
            this.sheetListBox.IntegralHeight = false;
            this.sheetListBox.ItemHeight = 40;
            this.sheetListBox.Location = new System.Drawing.Point(0, 0);
            this.sheetListBox.Name = "sheetListBox";
            this.sheetListBox.Size = new System.Drawing.Size(260, 316);
            this.sheetListBox.TabIndex = 0;
            this.sheetListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.sheetListBox_DrawItem);
            this.sheetListBox.SelectedIndexChanged += new System.EventHandler(this.sheetListBox_SelectedIndexChanged);
            this.sheetListBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sheetListBox_MouseUp);
            // 
            // layerListBox
            // 
            this.layerListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.layerListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layerListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.layerListBox.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.layerListBox.ForeColor = System.Drawing.Color.White;
            this.layerListBox.FormattingEnabled = true;
            this.layerListBox.IntegralHeight = false;
            this.layerListBox.ItemHeight = 40;
            this.layerListBox.Location = new System.Drawing.Point(0, 0);
            this.layerListBox.Name = "layerListBox";
            this.layerListBox.Size = new System.Drawing.Size(260, 203);
            this.layerListBox.TabIndex = 5;
            this.layerListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.layerListBox_DrawItem);
            this.layerListBox.SelectedIndexChanged += new System.EventHandler(this.layerListBox_SelectedIndexChanged);
            this.layerListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.layerListBox_MouseDown);
            this.layerListBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.layerListBox_MouseUp);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox1.Location = new System.Drawing.Point(14, 70);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(818, 423);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // sheetMenu
            // 
            this.sheetMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新規シートの作成ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.シート名の変更ToolStripMenuItem,
            this.toolStripMenuItem5,
            this.シート削除ToolStripMenuItem});
            this.sheetMenu.Name = "sheetMenu";
            this.sheetMenu.Size = new System.Drawing.Size(159, 82);
            // 
            // 新規シートの作成ToolStripMenuItem
            // 
            this.新規シートの作成ToolStripMenuItem.Name = "新規シートの作成ToolStripMenuItem";
            this.新規シートの作成ToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.新規シートの作成ToolStripMenuItem.Text = "新規シートの作成";
            this.新規シートの作成ToolStripMenuItem.Click += new System.EventHandler(this.新規シートの作成ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(155, 6);
            // 
            // シート名の変更ToolStripMenuItem
            // 
            this.シート名の変更ToolStripMenuItem.Name = "シート名の変更ToolStripMenuItem";
            this.シート名の変更ToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.シート名の変更ToolStripMenuItem.Text = "名前の変更";
            this.シート名の変更ToolStripMenuItem.Click += new System.EventHandler(this.シート名の変更ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(155, 6);
            // 
            // シート削除ToolStripMenuItem
            // 
            this.シート削除ToolStripMenuItem.Name = "シート削除ToolStripMenuItem";
            this.シート削除ToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.シート削除ToolStripMenuItem.Text = "削除";
            this.シート削除ToolStripMenuItem.Click += new System.EventHandler(this.シート削除ToolStripMenuItem_Click);
            // 
            // layerMenu
            // 
            this.layerMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新規吹き出しレイヤToolStripMenuItem,
            this.新規イメージレイヤToolStripMenuItem,
            this.新規塗り潰しレイヤToolStripMenuItem,
            this.toolStripMenuItem2,
            this.レイヤ削除ToolStripMenuItem});
            this.layerMenu.Name = "layerMenu";
            this.layerMenu.Size = new System.Drawing.Size(168, 98);
            // 
            // 新規吹き出しレイヤToolStripMenuItem
            // 
            this.新規吹き出しレイヤToolStripMenuItem.Name = "新規吹き出しレイヤToolStripMenuItem";
            this.新規吹き出しレイヤToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.新規吹き出しレイヤToolStripMenuItem.Text = "新規吹き出しレイヤ";
            this.新規吹き出しレイヤToolStripMenuItem.Click += new System.EventHandler(this.新規レイヤToolStripMenuItem_Click);
            // 
            // 新規イメージレイヤToolStripMenuItem
            // 
            this.新規イメージレイヤToolStripMenuItem.Name = "新規イメージレイヤToolStripMenuItem";
            this.新規イメージレイヤToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.新規イメージレイヤToolStripMenuItem.Text = "新規イメージレイヤ";
            this.新規イメージレイヤToolStripMenuItem.Click += new System.EventHandler(this.新規レイヤToolStripMenuItem_Click);
            // 
            // 新規塗り潰しレイヤToolStripMenuItem
            // 
            this.新規塗り潰しレイヤToolStripMenuItem.Name = "新規塗り潰しレイヤToolStripMenuItem";
            this.新規塗り潰しレイヤToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.新規塗り潰しレイヤToolStripMenuItem.Text = "新規塗り潰しレイヤ";
            this.新規塗り潰しレイヤToolStripMenuItem.Click += new System.EventHandler(this.新規レイヤToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(164, 6);
            // 
            // レイヤ削除ToolStripMenuItem
            // 
            this.レイヤ削除ToolStripMenuItem.Name = "レイヤ削除ToolStripMenuItem";
            this.レイヤ削除ToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.レイヤ削除ToolStripMenuItem.Text = "削除";
            this.レイヤ削除ToolStripMenuItem.Click += new System.EventHandler(this.レイヤ削除ToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 548);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1108, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // helpLabel
            // 
            this.helpLabel.Name = "helpLabel";
            this.helpLabel.Size = new System.Drawing.Size(35, 17);
            this.helpLabel.Text = "HELP";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1108, 570);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.sheetMenu.ResumeLayout(false);
            this.layerMenu.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox sheetListBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip sheetMenu;
        private System.Windows.Forms.ToolStripMenuItem 新規シートの作成ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem シート削除ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip layerMenu;
        private System.Windows.Forms.ToolStripMenuItem 新規吹き出しレイヤToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新規イメージレイヤToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新規塗り潰しレイヤToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem レイヤ削除ToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem 新規作成ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem 開くToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ListBox layerListBox;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem クリップボードにコピーToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem シート名の変更ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel helpLabel;
    }
}

