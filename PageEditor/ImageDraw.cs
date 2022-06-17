using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PageEditor
{
    static class ImageDraw
    {
        static Brush ClearBrush = null;

        static public Font DefaultFontSmall = new Font("源暎ラテゴ v2", 24);
        static public Font DefaultFontNormal = new Font("源暎ラテゴ v2", 36);
        static public Font DefaultFontLarge = new Font("源暎ラテゴ v2", 48);

        static StringFormat StringFormat = new StringFormat(StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft);
        static Pen WakuPen = new Pen(Brushes.Black, 8.0f);

        static Image WorkImage = null;

        /// <summary>
        /// 透明色を表現するハッチブラシの生成
        /// </summary>
        /// <returns>ハッチブラシ</returns>
        public static Brush GetClearBrush()
        {
            // 未生成
            if (ClearBrush == null)
            {
                Image image = new Bitmap(16, 16);
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.Clear(Color.White);
                    g.FillRectangle(Brushes.LightGray, new Rectangle(0, 0, 8, 8));
                    g.FillRectangle(Brushes.LightGray, new Rectangle(8, 8, 8, 8));
                }
                ClearBrush = new TextureBrush(image);
            }

            return ClearBrush;
        }

        /// <summary>
        /// 描画本体
        /// </summary>
        /// <param name="image"></param>
        /// <param name="layers"></param>
        internal static void Draw(Image image, List<Layer> layers)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.FromArgb(0, 0, 0, 0));

                if (layers != null)
                {
                    foreach (Layer layer in layers)
                    {
                        if (layer.Visible == false)
                            continue;

                        switch (layer.GetType().Name)
                        {
                            case "LayerSpeechBaloon":
                                DrawLayerSpeechBaloon(layer as LayerSpeechBaloon, g, image.Width, image.Height);
                                break;

                            case "LayerImage":
                                DrawLayerImage(layer as LayerImage, g, image.Width, image.Height);
                                break;

                            case "LayerFill":
                                DrawLayerFill(layer as LayerFill, g, image.Width, image.Height);
                                break;

                            case "LayerImageList":
                                DrawLayerImageList(layer as LayerImageList, g, image.Width, image.Height);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 吹き出しレイヤを描画します。
        /// </summary>
        /// <param name="layerSpeechBaloon"></param>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void DrawLayerSpeechBaloon(LayerSpeechBaloon layerSpeechBaloon, Graphics g, int width, int height)
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

                foreach (SpeechBaloon speechBaloon in layerSpeechBaloon.SpeechBaloons)
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

                foreach (SpeechBaloon speechBaloon in layerSpeechBaloon.SpeechBaloons)
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

                foreach (SpeechBaloon speechBaloon in layerSpeechBaloon.SpeechBaloons)
                {
                    // DrawStringは左上の座標を指定する関数だが、縦書きRightToLeftを指定すると右上になるので位置を調整
                    PointF point = new PointF(speechBaloon.X + speechBaloon.TextRect.Width / 2, speechBaloon.Y - speechBaloon.TextRect.Height / 2);

                    tg.DrawString(speechBaloon.Text, speechBaloon.Font, new SolidBrush(speechBaloon.ForeColor), point, StringFormat);
                }
            }

            g.DrawImage(WorkImage, new Rectangle(0, 0, width, height));
        }

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
        /// 塗りつぶしレイヤーを描画します。
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void DrawLayerFill(LayerFill layer, Graphics g, int width, int height)
        {
            Brush brush = new SolidBrush(layer.BackColor);
            g.FillRectangle(brush, new RectangleF(0, 0, width, height));
        }

        /// <summary>
        /// 画像レイヤーを描画します。
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void DrawLayerImage(LayerImage layer, Graphics g, int width, int height)
        {
            // 画像コントローラから画像データ取得
            Image image = PictureControl.Load(layer.FileName);

            // 画像データがあれば描画する。
            if (image != null)
            {
                if (layer.Angle == 0)
                {
                    g.DrawImage(image, new RectangleF(layer.X - image.Width * layer.Scale / 2.0f, layer.Y - image.Height * layer.Scale / 2.0f, image.Width * layer.Scale, image.Height * layer.Scale));
                }
                else
                {
                    PointF p0 = new PointF(-image.Width / 2.0f * layer.Scale, -image.Height / 2.0f * layer.Scale);
                    PointF p1 = new PointF(+image.Width / 2.0f * layer.Scale, -image.Height / 2.0f * layer.Scale);
                    PointF p2 = new PointF(-image.Width / 2.0f * layer.Scale, +image.Height / 2.0f * layer.Scale);

                    float cos = (float)Math.Cos(layer.Angle / 180.0 * Math.PI);
                    float sin = (float)Math.Sin(layer.Angle / 180.0 * Math.PI);

                    g.DrawImage(image, new PointF[] 
                    {
                        new PointF(p0.X * cos - p0.Y * sin + layer.X, p0.X * sin + p0.Y * cos + layer.Y),
                        new PointF(p1.X * cos - p1.Y * sin + layer.X, p1.X * sin + p1.Y * cos + layer.Y),
                        new PointF(p2.X * cos - p2.Y * sin + layer.X, p2.X * sin + p2.Y * cos + layer.Y),
                    });
                }
            }
        }

        /// <summary>
        /// イメージバッファレイヤーを描画します。
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void DrawLayerImageList(LayerImageList layer, Graphics g, int width, int height)
        {
            ImageItem info = layer.GetInfo();

            if (info == null)
                return;

            // 画像コントローラから画像データ取得
            Image image = PictureControl.Load(info.FileName);

            // 画像データがあれば描画する。
            if (image != null)
            {
                if (info.Angle == 0)
                {
                    g.DrawImage(image, new RectangleF(info.X - image.Width * info.Scale / 2.0f, info.Y - image.Height * info.Scale / 2.0f, image.Width * info.Scale, image.Height * info.Scale));
                }
                else
                {
                    PointF p0 = new PointF(-image.Width / 2.0f * info.Scale, -image.Height / 2.0f * info.Scale);
                    PointF p1 = new PointF(+image.Width / 2.0f * info.Scale, -image.Height / 2.0f * info.Scale);
                    PointF p2 = new PointF(-image.Width / 2.0f * info.Scale, +image.Height / 2.0f * info.Scale);

                    float cos = (float)Math.Cos(info.Angle / 180.0 * Math.PI);
                    float sin = (float)Math.Sin(info.Angle / 180.0 * Math.PI);

                    g.DrawImage(image, new PointF[]
                    {
                        new PointF(p0.X * cos - p0.Y * sin + info.X, p0.X * sin + p0.Y * cos + info.Y),
                        new PointF(p1.X * cos - p1.Y * sin + info.X, p1.X * sin + p1.Y * cos + info.Y),
                        new PointF(p2.X * cos - p2.Y * sin + info.X, p2.X * sin + p2.Y * cos + info.Y),
                    });
                }
            }
        }
    }
}
