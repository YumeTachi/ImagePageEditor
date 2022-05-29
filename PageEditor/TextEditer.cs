using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PageEditor
{
    public partial class TextEditer : Form
    {
        SpeechBaloon m_SpeechBaloon = null;

        public TextEditer()
        {
            InitializeComponent();
        }

        public void Set(SpeechBaloon speechBaloon = null)
        {
            timer1.Stop();

            m_SpeechBaloon = speechBaloon;

            // 新規作成
            if (m_SpeechBaloon == null)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;

                using (new JLUILocker())
                {
                    textBox1.Text = speechBaloon.Text;

                    switch (speechBaloon.Kind)
                    {
                        case SpeechBaloon.BaloonKind.Box:
                            radioButton1.Checked = true;
                            break;
                        case SpeechBaloon.BaloonKind.RoundedCorner1:
                            radioButton2.Checked = true;
                            break;
                    }

                    pictureBox1.BackColor = speechBaloon.ForeColor;
                    pictureBox2.BackColor = speechBaloon.BackColor;
                }
            }
        }

        internal SpeechBaloon CreateSpeechBaloon()
        {
            SpeechBaloon speechBaloon = new SpeechBaloon();
            speechBaloon.ForeColor = pictureBox1.BackColor;
            speechBaloon.BackColor = pictureBox2.BackColor;
            if (radioButton1.Checked) speechBaloon.Kind = SpeechBaloon.BaloonKind.Box;
            if (radioButton2.Checked) speechBaloon.Kind = SpeechBaloon.BaloonKind.RoundedCorner1;
            speechBaloon.Text = textBox1.Text;

            return speechBaloon;
        }


        private void ColorSelect(object sender, EventArgs e)
        {
            PictureBox pictureBox = sender as PictureBox;

            // ColorDialog
            ColorDialog cd = new ColorDialog();

            // 初期値
            cd.Color = pictureBox.BackColor;

            // よく使う色？
            cd.CustomColors = new int[] { 0x2E2EBF, 0xE57A3C };

            //ダイアログを表示する
            if (cd.ShowDialog() == DialogResult.OK)
            {
                //選択された色の取得
                pictureBox.BackColor = cd.Color;
            }

            if (m_SpeechBaloon != null)
            {
                m_SpeechBaloon.ForeColor = pictureBox1.BackColor;
                m_SpeechBaloon.BackColor = pictureBox2.BackColor;
                timer1.Start();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (m_SpeechBaloon != null)
            {
                m_SpeechBaloon.Text = textBox1.Text;
                timer1.Start();
            }
        }

        private void radioButton_Click(object sender, EventArgs e)
        {
            if (m_SpeechBaloon != null)
            {
                if (radioButton1.Checked) m_SpeechBaloon.Kind = SpeechBaloon.BaloonKind.Box;
                if (radioButton2.Checked) m_SpeechBaloon.Kind = SpeechBaloon.BaloonKind.RoundedCorner1;
                timer1.Start();
            }
        }

        private void TextEditer_FormClosing(object sender, FormClosingEventArgs e)
        {
            // タイマー停止
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MainForm.GetInstance().ImageUpdate();
        }
    }
}
