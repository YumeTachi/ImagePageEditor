using System.Drawing;
using System.Windows.Forms;

namespace PageEditor
{
    static class LayerListDraw
    {
        static Pen LinePen = null;

        /// <summary>
        /// 枠線用ペン
        /// </summary>
        /// <returns></returns>
        public static Pen GetLinePen()
        {
            if (LinePen == null)
            {
                LinePen = new Pen(Brushes.Black, 2.0f);
            }
            return LinePen;
        }

        /// <summary>
        /// 描画本体
        /// </summary>
        /// <param name="image"></param>
        /// <param name="layers"></param>
        internal static void Draw(Layer layer, DrawItemEventArgs e)
        {
            Common(layer, e);

            switch (layer.GetType().Name)
            {
                case "LayerSpeechBaloon":
                    DrawLayerSpeechBaloon(layer as LayerSpeechBaloon, e);
                    break;
                case "LayerFill":
                    DrawLayerFill(layer as LayerFill, e);
                    break;
                case "LayerImage":
                    DrawLayerImage(layer as LayerImage, e);
                    break;
            }
        }

        /// <summary>
        /// レイヤービュー用の描画
        /// </summary>
        private static void Common(Layer layer, DrawItemEventArgs e)
        {
            // 背景色
            e.DrawBackground();

            // TODO:表示非表示状態
            
            // 枠線
            e.Graphics.DrawLine(GetLinePen(), 29, e.Bounds.Top, 29, e.Bounds.Bottom - 1);
            e.Graphics.DrawLine(GetLinePen(), 0, e.Bounds.Bottom - 1, e.Bounds.Width, e.Bounds.Bottom - 1);

            // レイヤの文字列
            Rectangle box = new Rectangle(60, e.Bounds.Top, 100, e.Bounds.Height - 2);
            e.Graphics.DrawString(layer.LayerType(), e.Font, Brushes.White, box, new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
        }

        /// <summary>
        /// 塗りつぶしレイヤーを描画します。
        /// </summary>
        private static void DrawLayerFill(LayerFill layer, DrawItemEventArgs e)
        {
            // 塗りつぶしの枠だけ。
            Rectangle box = new Rectangle(35, e.Bounds.Top + (e.Bounds.Height - 18 - 2) / 2, 18, 18);
            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), box);
            e.Graphics.FillRectangle(new SolidBrush(layer.BackColor), box);
            e.Graphics.DrawRectangle(Pens.White, box);
        }

        /// <summary>
        /// イメージレイヤーを描画します。
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="e"></param>
        private static void DrawLayerImage(LayerImage layer, DrawItemEventArgs e)
        {
            // 塗りつぶしの枠だけ。
            Rectangle box = new Rectangle(35, e.Bounds.Top + (e.Bounds.Height - 20) / 2, 18, 18);
            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), box);
            e.Graphics.DrawRectangle(Pens.White, box);
        }

        /// <summary>
        /// 吹き出しレイヤーを描画します。
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="e"></param>
        private static void DrawLayerSpeechBaloon(LayerSpeechBaloon layer, DrawItemEventArgs e)
        {
            // 塗りつぶしの枠だけ。
            Rectangle box = new Rectangle(35, e.Bounds.Top + (e.Bounds.Height - 20) / 2, 18, 18);
            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), box);
            e.Graphics.DrawRectangle(Pens.White, box);

            Rectangle baloon = new Rectangle(box.Left + 3, box.Top + 2, 12, 14);
            e.Graphics.FillEllipse(Brushes.White, baloon);
            e.Graphics.DrawEllipse(Pens.DarkGray, baloon);
        }
    }
}
