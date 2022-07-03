using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;
using static PageEditor.ImageOperation;

namespace PageEditor
{
    [Serializable]
    public class ImageItem
    {
        [XmlIgnore]
        [IgnoreDataMember]
        static public ImageItem Empty = new ImageItem();

        /// <summary>ファイル名</summary>
        [XmlAttribute]
        public string FileName;

        // 画像の表示位置中央X
        [XmlAttribute]
        public int X;

        // 画像の表示位置中央Y
        [XmlAttribute]
        public int Y;

        [XmlAttribute]
        public float Angle;

        [XmlAttribute]
        public float Scale;

        /// <summary>コンストラクタ</summary>
        public ImageItem() : base()
        {
            FileName = null;
            X = 0;
            Y = 0;
            Angle = 0;
            Scale = 1;
        }

        internal void SetZoom(string relativeFileName, int documentWidth, int documentHeight, Image image)
        {
            FileName = relativeFileName;
            X = documentWidth / 2;
            Y = documentHeight / 2;
            Angle = 0;
            Scale = Math.Min((float)documentWidth / image.Width, (float)documentHeight / image.Height);
        }

        /// <summary>
        /// メインキャンバスへの描画
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Draw(Graphics g, int width, int height)
        {
            // 画像コントローラから画像データ取得
            PictureControl.PictureInfo image = MainForm.GetInstance().Pictures.Load(FileName);

            // 画像データがあれば描画する。
            if (image != null)
            {
                if (Angle == 0)
                {
                    g.DrawImage(image.Picture, new RectangleF(X - image.Picture.Width * Scale / 2.0f, Y - image.Picture.Height * Scale / 2.0f, image.Picture.Width * Scale, image.Picture.Height * Scale));
                }
                else
                {
                    PointF p0 = new PointF(-image.Picture.Width / 2.0f * Scale, -image.Picture.Height / 2.0f * Scale);
                    PointF p1 = new PointF(+image.Picture.Width / 2.0f * Scale, -image.Picture.Height / 2.0f * Scale);
                    PointF p2 = new PointF(-image.Picture.Width / 2.0f * Scale, +image.Picture.Height / 2.0f * Scale);

                    float cos = (float)Math.Cos(Angle / 180.0 * Math.PI);
                    float sin = (float)Math.Sin(Angle / 180.0 * Math.PI);

                    g.DrawImage(image.Picture, new PointF[]
                    {
                        new PointF(p0.X * cos - p0.Y * sin + X, p0.X * sin + p0.Y * cos + Y),
                        new PointF(p1.X * cos - p1.Y * sin + X, p1.X * sin + p1.Y * cos + Y),
                        new PointF(p2.X * cos - p2.Y * sin + X, p2.X * sin + p2.Y * cos + Y),
                    });
                }
            }
        }
    }

    /// <summary>
    /// 画像レイヤ
    /// </summary>
    [Serializable]
    public class LayerImage : Layer
    {
        [XmlIgnore]
        [IgnoreDataMember]
        public ImageItem ImageItem;

        /// <summary>ファイル名</summary>
        [XmlAttribute]
        public string FileName { get => ImageItem.FileName; set => ImageItem.FileName = value; }

        // 画像の表示位置中央X
        [XmlAttribute]
        public int X { get => ImageItem.X; set => ImageItem.X = value; }

        // 画像の表示位置中央Y
        [XmlAttribute]
        public int Y { get => ImageItem.Y; set => ImageItem.Y = value; }

        [XmlAttribute]
        public float Angle { get => ImageItem.Angle; set => ImageItem.Angle = value; }

        [XmlAttribute]
        public float Scale { get => ImageItem.Scale; set => ImageItem.Scale = value; }

        /// <summary>
        /// レイヤー種別を表す文字列の取得
        /// </summary>
        /// <returns></returns>
        public override string LayerType()
        {
            return "画像";
        }

        /// <summary>コンストラクタ</summary>
        public LayerImage() : base()
        {
            ImageItem = new ImageItem();
        }

        /// <summary>
        /// メインキャンバスへの描画
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void Draw(Graphics g, int width, int height)
        {
            ImageItem.Draw(g, width, height);
        }

        /// <summary>
        /// レイヤリストへの描画（個別）
        /// </summary>
        /// <param name="e"></param>
        public override void DrawLayerListItem(LayerListDrawEventArgs e)
        {
            PictureControl.PictureInfo thumbImage = MainForm.GetInstance().Pictures.Load(ImageItem.FileName);

            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), e.ThumbsBounds);
            if (thumbImage != null)
                e.Graphics.DrawImage(thumbImage.ThumbImage, e.ThumbsBounds);
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
            mouseControl.Tag = this;
            mouseControl.ObjectPoint = new Point(X, Y);

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
                LayerImage item = mouseControl.Tag as LayerImage;

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
            if (mouseControl.Button == MouseButtons.Right)
            {
                if (MainForm.GetInstance().DataDirectory == null)
                {
                    MessageBox.Show("画像ファイルを指定する際は先にワークスペースを保存してください。", "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return ThumbnailUpdateType.NONE;
                }

                // OpenFileDialog
                OpenFileDialog ofd = new OpenFileDialog();

                // はじめのファイル名を指定する
                ofd.FileName = FileName;

                // はじめに表示されるフォルダを指定する
                ofd.InitialDirectory = MainForm.GetInstance().DataDirectory;

                // フィルター
                ofd.Filter = "イメージファイル(*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|すべてのファイル(*.*)|*.*";

                // タイトル
                ofd.Title = "画像ファイルを選択してください";

                // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                ofd.RestoreDirectory = true;

                // ダイアログを表示する
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // 選択されたファイルを設定
                    string relative = MainForm.GetInstance().GetRelativePathWithCopy(ofd.FileName);

                    // 画像サイズを取得
                    PictureControl.PictureInfo image = MainForm.GetInstance().Pictures.Load(relative);

                    // 失敗
                    if (image == null)
                    {
                        MessageBox.Show("画像ファイルを開けませんでした。", "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return ThumbnailUpdateType.NONE;
                    }

                    ImageItem.SetZoom(relative, document.Width, document.Height, image.Picture);

                    return ThumbnailUpdateType.IMMEDIATELY;
                }
            }

            return ThumbnailUpdateType.NONE;
        }
    }
}
