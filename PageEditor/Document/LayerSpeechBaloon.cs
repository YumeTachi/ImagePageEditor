using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;
using static PageEditor.ImageOperation;

namespace PageEditor
{
    [Serializable]
    public class SpeechBaloon
    {
        public enum BaloonKind
        {
            Box,
            RoundedCorner1,
        }

        public enum FontSizeKind
        {
            Middle,
            Small,
            Large,
        }

        /// <summary>種別</summary>
        [XmlAttribute]
        public BaloonKind Kind;

        /// <summary>文字サイズ</summary>
        [XmlAttribute]
        public FontSizeKind FontSize;

        /// <summary>表示位置右上X</summary>
        [XmlAttribute]
        public int X;

        /// <summary>表示位置右上Y</summary>
        [XmlAttribute]
        public int Y;

        /// <summary>テキスト</summary>
        public string Text;

        /// <summary>テキストの描画色</summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Color ForeColor;

        /// <summary>テキストの背景色</summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Color BackColor;

        /// <summary>テキストの描画色</summary>
        [XmlAttribute(AttributeName = "ForeColor")]
        public string _ForeColor
        {
            get { return ColorTranslator.ToHtml(ForeColor); }
            set { ForeColor = ColorTranslator.FromHtml(value); }
        }

        /// <summary>テキストの背景色</summary>
        [XmlAttribute(AttributeName = "BackColor")]
        public string _BackColor
        {
            get { return ColorTranslator.ToHtml(BackColor); }
            set { BackColor = ColorTranslator.FromHtml(value); }
        }

        /// <summary>フォント</summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Font Font
        {
            get
            {
                // サイズに応じて何を返すか決める。
                switch (FontSize)
                {
                    case FontSizeKind.Small:
                        return ImageDraw.DefaultFontSmall;

                    case FontSizeKind.Large:
                        return ImageDraw.DefaultFontLarge;

                    case FontSizeKind.Middle:
                        return ImageDraw.DefaultFontNormal;
                }

                return ImageDraw.DefaultFontNormal;
            }
        }

        /// <summary>
        /// ツール用テキスト範囲
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Rectangle TextRect;

        /// <summary>
        /// ツール用テキスト範囲（選択用）
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Rectangle SelectRect;

        public SpeechBaloon()
        {
            Kind = BaloonKind.RoundedCorner1;
            FontSize = FontSizeKind.Middle;
            Text = "";
            ForeColor = Color.DarkGray;
            BackColor = Color.White;
        }

        public SpeechBaloon(SpeechBaloon arg)
        {
            Kind = arg.Kind;
            FontSize = arg.FontSize;
            X = arg.X;
            Y = arg.Y;
            Text = arg.Text.Clone() as string;
            ForeColor = arg.ForeColor;
            BackColor = arg.BackColor;
            TextRect = arg.TextRect;
            SelectRect = arg.SelectRect;
        }

    }

    [Serializable]
    public class LayerSpeechBaloon : Layer
    {
        public List<SpeechBaloon> SpeechBaloons;

        /// <summary>
        /// レイヤー種別を表す文字列の取得
        /// </summary>
        /// <returns></returns>
        public override string LayerType()
        {
            return "吹き出し";
        }

        /// <summary>コンストラクタ</summary>
        public LayerSpeechBaloon() : base()
        {
            SpeechBaloons = new List<SpeechBaloon>();
        }

        /// <summary>
        /// 指定位置に吹き出しがあるか確認
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        internal int FindIndex(Point location)
        {
            for (int i = 0; i < SpeechBaloons.Count; i++)
            {
                if (SpeechBaloons[i].SelectRect.Contains(location))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 指定位置に吹き出しがあるか確認
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        internal SpeechBaloon Find(Point location)
        {
            foreach (SpeechBaloon speechBaloon in SpeechBaloons)
            {
                if (speechBaloon.SelectRect.Contains(location))
                {
                    return speechBaloon;
                }
            }
            return null;
        }

        // 描画に使用
        static StringFormat StringFormat = new StringFormat(StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft);
        static Pen WakuPen = new Pen(Brushes.Black, 8.0f);
        static Image WorkImage = null;

        /// <summary>
        /// 丸吹き出しロジック１
        /// </summary>
        /// <param name="speechBaloon"></param>
        /// <returns></returns>
        private static GraphicsPath CreateRoundedCorner1(SpeechBaloon speechBaloon)
        {
            GraphicsPath graphicsPath = new GraphicsPath();

            // オフセットで調整？要調整
            int offset = Math.Min(80, Math.Max(40, (speechBaloon.TextRect.Width + speechBaloon.TextRect.Height) / 15));

            graphicsPath.AddClosedCurve(new Point[]
            {
                new Point((speechBaloon.TextRect.Left + speechBaloon.TextRect.Right) / 2, speechBaloon.TextRect.Top - offset),
                new Point(speechBaloon.TextRect.Right + offset, (speechBaloon.TextRect.Top + speechBaloon.TextRect.Bottom) / 2),
                new Point((speechBaloon.TextRect.Left + speechBaloon.TextRect.Right) / 2, speechBaloon.TextRect.Bottom + offset),
                new Point(speechBaloon.TextRect.Left - offset, (speechBaloon.TextRect.Top + speechBaloon.TextRect.Bottom) / 2)
            }, 1.0f);

            return graphicsPath;
        }

        /// <summary>
        /// メインキャンバスへの描画
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void Draw(Graphics g, int width, int height)
        {
            if (WorkImage == null || WorkImage.Width != width || WorkImage.Height != height)
            {
                WorkImage = new Bitmap(width, height);
            }

            using (Graphics tg = Graphics.FromImage(WorkImage))
            {
                tg.Clear(Color.FromArgb(0, 0, 0, 0));
                tg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                tg.SmoothingMode = SmoothingMode.HighQuality;

                foreach (SpeechBaloon speechBaloon in SpeechBaloons)
                {
                    SizeF textSize = tg.MeasureString(speechBaloon.Text, speechBaloon.Font, new PointF(speechBaloon.X, speechBaloon.Y), StringFormat);

                    // 実サイズ
                    speechBaloon.TextRect = new Rectangle(speechBaloon.X - (int)textSize.Width / 2,
                                                          speechBaloon.Y - (int)textSize.Height / 2,
                                                          (int)textSize.Width,
                                                          (int)textSize.Height);

                    // 選択枠
                    speechBaloon.SelectRect = new Rectangle(speechBaloon.X - (int)textSize.Width / 2 - 20,
                                                            speechBaloon.Y - (int)textSize.Height / 2 - 20,
                                                            (int)textSize.Width + 40,
                                                            (int)textSize.Height + 40);

                    switch (speechBaloon.Kind)
                    {
                        case SpeechBaloon.BaloonKind.Box:
                            {
                                // コピーして拡張
                                Rectangle tmp = speechBaloon.TextRect;
                                tmp.Inflate(20, 20);
                                tg.DrawRectangle(WakuPen, tmp);
                            }
                            break;

                        case SpeechBaloon.BaloonKind.RoundedCorner1:
                            tg.DrawPath(WakuPen, CreateRoundedCorner1(speechBaloon));
                            break;
                    }
                }

                foreach (SpeechBaloon speechBaloon in SpeechBaloons)
                {
                    switch (speechBaloon.Kind)
                    {
                        case SpeechBaloon.BaloonKind.Box:
                            {
                                // コピーして拡張
                                Rectangle tmp = speechBaloon.TextRect;
                                tmp.Inflate(20, 20);
                                tg.FillRectangle(new SolidBrush(speechBaloon.BackColor), tmp);
                            }
                            break;

                        case SpeechBaloon.BaloonKind.RoundedCorner1:
                            tg.FillPath(new SolidBrush(speechBaloon.BackColor), CreateRoundedCorner1(speechBaloon));
                            break;
                    }
                }

                foreach (SpeechBaloon speechBaloon in SpeechBaloons)
                {
                    // DrawStringは左上の座標を指定する関数だが、縦書きRightToLeftを指定すると右上になるので位置を調整
                    PointF point = new PointF(speechBaloon.X + speechBaloon.TextRect.Width / 2, speechBaloon.Y - speechBaloon.TextRect.Height / 2);

                    tg.DrawString(speechBaloon.Text, speechBaloon.Font, new SolidBrush(speechBaloon.ForeColor), point, StringFormat);
                }
            }

            g.DrawImage(WorkImage, new Rectangle(0, 0, width, height));
        }

        /// <summary>
        /// レイヤリストへの描画（個別）
        /// </summary>
        /// <param name="e"></param>
        public override void DrawLayerListItem(LayerListDrawEventArgs e)
        {
            // 塗りつぶしの枠だけ。
            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), e.ThumbsBounds);
            e.Graphics.DrawRectangle(Pens.White, e.ThumbsBounds);

            Rectangle baloon = new Rectangle(e.ThumbsBounds.Left + 3, e.ThumbsBounds.Top + 2, 12, 14);
            e.Graphics.FillEllipse(Brushes.White, baloon);
            e.Graphics.DrawEllipse(Pens.DarkGray, baloon);
        }

        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="e"></param>
        /// <param name="mouseControl"></param>
        /// <param name="document"></param>
        /// <param name="mainForm"></param>
        /// <returns></returns>
        internal override ThumbnailUpdateType OnMouseDown(JLMouseEventArgs e, MouseControlInfo mouseControl, Document document, MainForm mainForm)
        {
            SpeechBaloon speechBaloon = Find(e.Location);

            if (speechBaloon != null)
            {
                mouseControl.Tag = speechBaloon;
                mouseControl.ObjectPoint = new Point(speechBaloon.X, speechBaloon.Y);
            }

            return ThumbnailUpdateType.NONE;
        }

        /// <summary>
        /// OnMouseMove
        /// </summary>
        /// <param name="e"></param>
        /// <param name="mouseControl"></param>
        /// <param name="document"></param>
        /// <param name="mainForm"></param>
        /// <returns></returns>
        internal override ThumbnailUpdateType OnMouseMove(JLMouseEventArgs e, MouseControlInfo mouseControl, Document document, MainForm mainForm)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                SpeechBaloon item = mouseControl.Tag as SpeechBaloon;

                if (item == null) return ThumbnailUpdateType.NONE;

                item.X = mouseControl.ObjectPoint.X + (e.Location.X - mouseControl.DownPoint.X);
                item.Y = mouseControl.ObjectPoint.Y + (e.Location.Y - mouseControl.DownPoint.Y);

                return ThumbnailUpdateType.LATER;
            }

            return ThumbnailUpdateType.NONE;
        }

        /// <summary>
        /// OnMouseUp
        /// </summary>
        /// <param name="e"></param>
        /// <param name="mouseControl"></param>
        /// <param name="document"></param>
        /// <param name="mainForm"></param>
        /// <returns></returns>
        internal override ThumbnailUpdateType OnMouseUp(JLMouseEventArgs e, MouseControlInfo mouseControl, Document document, MainForm mainForm)
        {
            return ThumbnailUpdateType.NONE;
        }

        /// <summary>
        /// OnClick
        /// </summary>
        /// <param name="e"></param>
        /// <param name="mouseControl"></param>
        /// <param name="document"></param>
        /// <param name="mainForm"></param>
        /// <returns></returns>
        internal override ThumbnailUpdateType OnClick(JLMouseEventArgs e, MouseControlInfo mouseControl, Document document, MainForm mainForm)
        {
            if (mouseControl.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // オブジェクト上でクリック
                if (mouseControl.Tag != null)
                {
                    SpeechBaloon speechBaloon = mouseControl.Tag as SpeechBaloon;

                    TextEditer textEditer = new TextEditer("編集");
                    textEditer.Set(speechBaloon);
                    textEditer.Show(mainForm);

                    return ThumbnailUpdateType.NONE;
                }
                else
                {
                    // 新規作成
                    TextEditer editer = new TextEditer("新規作成");
                    if (editer.ShowDialog(mainForm) == DialogResult.OK)
                    {
                        SpeechBaloon newSpeechBaloon = editer.CreateSpeechBaloon();
                        newSpeechBaloon.X = e.Location.X;
                        newSpeechBaloon.Y = e.Location.Y;

                        SpeechBaloons.Add(newSpeechBaloon);

                        return ThumbnailUpdateType.IMMEDIATELY;
                    }
                }
            }
            else if (mouseControl.Button == MouseButtons.Right)
            {
                if (mouseControl.Tag != null)
                {
                    SpeechBaloon speechBaloon = mouseControl.Tag as SpeechBaloon;

                    ContextMenuStrip c = new ContextMenuStrip();
                    c.Items.Add(new ToolStripMenuItem("削除", null, Delete_Click) { Tag = speechBaloon });
                    c.Show(Cursor.Position);
                }
                else
                {
                    ContextMenuStrip c = new ContextMenuStrip();
                    c.Items.Add(new ToolStripMenuItem("一括追加", null, AddSum_Click) { Tag = new int[] { document.Width, document.Height } });
                    c.Show(Cursor.Position);
                }
            }

            return ThumbnailUpdateType.NONE;
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            // 引数の受け取り
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            SpeechBaloon speechBaloon = toolStripMenuItem.Tag as SpeechBaloon;

            // 対象の吹き出しを削除
            SpeechBaloons.Remove(speechBaloon);

            // 描画更新
            MainForm.GetInstance().ImageUpdate(ThumbnailUpdateType.IMMEDIATELY);
        }

        /// <summary>
        /// 一括追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSum_Click(object sender, EventArgs e)
        {
            // 新規作成
            TextEditer editer = new TextEditer("新規作成：一括追加");
            if (editer.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // 引数の受け取り
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            int[] args = toolStripMenuItem.Tag as int[];

            // 結果の受け取り
            SpeechBaloon baseSpeechBaloon = editer.CreateSpeechBaloon();

            // 分割
            string[] rs = baseSpeechBaloon.Text.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n\n", "\r").Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // 分割されたデータごとに処理
            for (int i = 0; i < rs.Length; i++)
            {
                // Cloneの生成
                SpeechBaloon speechBaloon = new SpeechBaloon(baseSpeechBaloon);
                speechBaloon.Text = rs[i].Trim();

                speechBaloon.X = args[0] * (rs.Length - i) / (rs.Length + 1);
                speechBaloon.Y = args[1] / 2;

                SpeechBaloons.Add(speechBaloon);
            }

            // 描画更新
            MainForm.GetInstance().ImageUpdate(ThumbnailUpdateType.IMMEDIATELY);
        }
    }
}
