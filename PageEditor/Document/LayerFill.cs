using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;
using static PageEditor.ImageOperation;

namespace PageEditor
{
    /// <summary>
    /// 塗りつぶしレイヤー
    /// </summary>
    [Serializable]
    public class LayerFill : Layer
    {
        // 持っているのは塗りつぶしの色だけ

        /// <summary>塗りつぶしRGB</summary>
        [XmlAttribute(AttributeName = "BackColor")]
        public string _BackColor
        {
            get { return ColorTranslator.ToHtml(BackColor); }
            set { BackColor = ColorTranslator.FromHtml(value); }
        }

        /// <summary>塗りつぶしRGB</summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Color BackColor;

        /// <summary>
        /// レイヤー種別を表す文字列の取得
        /// </summary>
        /// <returns></returns>
        public override string LayerType()
        {
            return "塗りつぶし";
        }

        /// <summary>コンストラクタ</summary>
        public LayerFill() : base()
        {
            BackColor = Color.FromArgb(255, 122, 138, 132);
        }

        /// <summary>
        /// メインキャンバスへの描画
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void Draw(Graphics g, int width, int height)
        {
            Brush brush = new SolidBrush(BackColor);
            g.FillRectangle(brush, new RectangleF(0, 0, width, height));
        }

        /// <summary>
        /// レイヤリストへの描画（個別）
        /// </summary>
        /// <param name="e"></param>
        public override void DrawLayerListItem(LayerListDrawEventArgs e)
        {
            // 塗りつぶしの枠だけ。
            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), e.ThumbsBounds);
            e.Graphics.FillRectangle(new SolidBrush(BackColor), e.ThumbsBounds);
            e.Graphics.DrawRectangle(Pens.White, e.ThumbsBounds);
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
            if (mouseControl.Button == MouseButtons.Right)
            {
                

                // ColorDialog
                ColorDialog cd = new ColorDialog();

                // 初期値
                cd.Color = BackColor;

                // よく使う色？
                // BGR
                cd.CustomColors = General.CustomColors;

                //ダイアログを表示する
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    //選択された色の取得
                    BackColor = cd.Color;
                    return ThumbnailUpdateType.IMMEDIATELY;
                }

                return ThumbnailUpdateType.NONE;
            }

            return ThumbnailUpdateType.NONE;
        }
    }
}
