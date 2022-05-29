using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PageEditor
{
    class SheetListDraw
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
        /// <param name="sheet"></param>
        /// <param name="e"></param>
        internal static void Draw(Sheet sheet, DrawItemEventArgs e)
        {
            // 背景色
            e.DrawBackground();

            // TODO:表示非表示状態

            // 枠線
            e.Graphics.DrawLine(GetLinePen(), 40, e.Bounds.Top, 40, e.Bounds.Bottom - 1);
            e.Graphics.DrawLine(GetLinePen(), 0, e.Bounds.Bottom - 1, e.Bounds.Width, e.Bounds.Bottom - 1);

            // シート番号
            Rectangle box1 = new Rectangle(0, e.Bounds.Top, 36, e.Bounds.Height - 2);
            e.Graphics.DrawString((e.Index + 1).ToString(), e.Font, Brushes.White, box1, new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });

            // サムネイル
            Rectangle box2 = new Rectangle(44, e.Bounds.Top + (e.Bounds.Height - 2 - 28) / 2, 48, 28);
            if (sheet.Thumbnail != null)
            {
                e.Graphics.DrawImage(sheet.Thumbnail, box2);
            }
            else
            {
                e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), box2);
            }
            e.Graphics.DrawRectangle(Pens.Black, box2);

            // タイトル
            Rectangle box3 = new Rectangle(100, e.Bounds.Top, 100, e.Bounds.Height - 2);
            e.Graphics.DrawString(sheet.Name, e.Font, Brushes.White, box3, new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
        }

    }
}
