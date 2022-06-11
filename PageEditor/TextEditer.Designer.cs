
namespace PageEditor
{
    partial class TextEditer
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
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.shapeKind1 = new System.Windows.Forms.RadioButton();
            this.shapeKind2 = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.sizeKind0 = new System.Windows.Forms.RadioButton();
            this.sizeKind1 = new System.Windows.Forms.RadioButton();
            this.sizeKind2 = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(353, 190);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // shapeKind1
            // 
            this.shapeKind1.AutoSize = true;
            this.shapeKind1.Checked = true;
            this.shapeKind1.Location = new System.Drawing.Point(6, 18);
            this.shapeKind1.Name = "shapeKind1";
            this.shapeKind1.Size = new System.Drawing.Size(46, 16);
            this.shapeKind1.TabIndex = 2;
            this.shapeKind1.TabStop = true;
            this.shapeKind1.Text = "BOX";
            this.shapeKind1.UseVisualStyleBackColor = true;
            this.shapeKind1.Click += new System.EventHandler(this.shapeRadioButton_Click);
            // 
            // shapeKind2
            // 
            this.shapeKind2.AutoSize = true;
            this.shapeKind2.Location = new System.Drawing.Point(6, 40);
            this.shapeKind2.Name = "shapeKind2";
            this.shapeKind2.Size = new System.Drawing.Size(72, 16);
            this.shapeKind2.TabIndex = 3;
            this.shapeKind2.Text = "ELLIPSE1";
            this.shapeKind2.UseVisualStyleBackColor = true;
            this.shapeKind2.Click += new System.EventHandler(this.shapeRadioButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(259, 230);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(51, 31);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.ColorSelect);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(314, 230);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(51, 31);
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.ColorSelect);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(257, 211);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "文字色";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(314, 211);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "背景色";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(259, 267);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 38);
            this.button1.TabIndex = 1;
            this.button1.Text = "決定";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.shapeKind1);
            this.groupBox1.Controls.Add(this.shapeKind2);
            this.groupBox1.Location = new System.Drawing.Point(12, 208);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(94, 97);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "形状";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.sizeKind0);
            this.groupBox2.Controls.Add(this.sizeKind1);
            this.groupBox2.Controls.Add(this.sizeKind2);
            this.groupBox2.Location = new System.Drawing.Point(112, 208);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(94, 97);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "文字サイズ";
            // 
            // sizeKind0
            // 
            this.sizeKind0.AutoSize = true;
            this.sizeKind0.Location = new System.Drawing.Point(6, 18);
            this.sizeKind0.Name = "sizeKind0";
            this.sizeKind0.Size = new System.Drawing.Size(35, 16);
            this.sizeKind0.TabIndex = 4;
            this.sizeKind0.Text = "小";
            this.sizeKind0.UseVisualStyleBackColor = true;
            this.sizeKind0.Click += new System.EventHandler(this.sizeRadioButton_Click);
            // 
            // sizeKind1
            // 
            this.sizeKind1.AutoSize = true;
            this.sizeKind1.Checked = true;
            this.sizeKind1.Location = new System.Drawing.Point(6, 40);
            this.sizeKind1.Name = "sizeKind1";
            this.sizeKind1.Size = new System.Drawing.Size(35, 16);
            this.sizeKind1.TabIndex = 2;
            this.sizeKind1.TabStop = true;
            this.sizeKind1.Text = "中";
            this.sizeKind1.UseVisualStyleBackColor = true;
            this.sizeKind1.Click += new System.EventHandler(this.sizeRadioButton_Click);
            // 
            // sizeKind2
            // 
            this.sizeKind2.AutoSize = true;
            this.sizeKind2.Location = new System.Drawing.Point(6, 62);
            this.sizeKind2.Name = "sizeKind2";
            this.sizeKind2.Size = new System.Drawing.Size(35, 16);
            this.sizeKind2.TabIndex = 3;
            this.sizeKind2.Text = "大";
            this.sizeKind2.UseVisualStyleBackColor = true;
            this.sizeKind2.Click += new System.EventHandler(this.sizeRadioButton_Click);
            // 
            // TextEditer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 317);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.KeyPreview = true;
            this.Name = "TextEditer";
            this.Text = "TextEditer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextEditer_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextEditer_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton shapeKind1;
        private System.Windows.Forms.RadioButton shapeKind2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton sizeKind0;
        private System.Windows.Forms.RadioButton sizeKind1;
        private System.Windows.Forms.RadioButton sizeKind2;
    }
}