using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using static PageEditor.ImageOperation;

namespace PageEditor
{
    [Serializable]
    public class LayerImageList : Layer
    {
        public List<ImageItem> ImageItems;

        [XmlAttribute]
        public int SelectedIndex;

        /// <summary>
        /// レイヤー種別を表す文字列の取得
        /// </summary>
        /// <returns></returns>
        public override string LayerType()
        {
            return "イメージリスト";
        }

        /// <summary>コンストラクタ</summary>
        public LayerImageList() : base()
        {
            ImageItems = new List<ImageItem>();
            SelectedIndex = -1;
        }

        // 情報取得
        [XmlIgnore]
        [IgnoreDataMember]
        internal ImageItem CurrentImage
        {
            get
            {
                if (SelectedIndex < 0)
                    return ImageItem.Empty;

                return ImageItems[SelectedIndex];
            }
            set
            {
                if (value == ImageItem.Empty)
                {
                    SelectedIndex = -1;
                    return;
                }

                SelectedIndex = ImageItems.FindIndex(a => a == value);
                System.Diagnostics.Debug.Assert(SelectedIndex != -1);
            }
        }

        /// <summary>
        /// メインキャンバスへの描画
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void Draw(Graphics g, int width, int height)
        {
            if (CurrentImage == null)
                return;

            CurrentImage.Draw(g, width, height);
        }

        /// <summary>
        /// レイヤリストへの描画（個別）
        /// </summary>
        /// <param name="e"></param>
        public override void DrawLayerListItem(LayerListDrawEventArgs e)
        {
            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), e.ThumbsBounds);
            if (CurrentImage != null && CurrentImage != ImageItem.Empty)
            {
                PictureControl.PictureInfo thumbImage = MainForm.GetInstance().Pictures.Load(CurrentImage.FileName);
                if (thumbImage != null)
                    e.Graphics.DrawImage(thumbImage.ThumbImage, e.ThumbsBounds);
            }
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
            if (CurrentImage != ImageItem.Empty)
            {
                mouseControl.Tag = CurrentImage;
                mouseControl.ObjectPoint = new Point(CurrentImage.X, CurrentImage.Y);
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
                ImageItem item = mouseControl.Tag as ImageItem;

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
            return ThumbnailUpdateType.NONE;
        }
    }
}
