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

                        layer.Draw(g, image.Width, image.Height);
                    }
                }
            }
        }
    }
}
