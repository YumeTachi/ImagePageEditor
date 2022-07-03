using System;
using System.Drawing;
using System.Xml.Serialization;
using static PageEditor.ImageOperation;

namespace PageEditor
{
    public class LayerListDrawEventArgs
    {
        public Graphics Graphics;
        public Rectangle Bounds;
        public Rectangle ThumbsBounds;
    }

    /// <summary>
    /// レイヤーのスーパークラス
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(LayerFill))]
    [XmlInclude(typeof(LayerImage))]
    [XmlInclude(typeof(LayerSpeechBaloon))]
    [XmlInclude(typeof(LayerImageList))]
    public abstract class Layer
    {
        /// <summary>表示非表示</summary>
        [XmlAttribute]
        public bool Visible;

        /// <summary>コンストラクタ</summary>
        public Layer()
        {
            Visible = true;
        }

        /// <summary>
        /// レイヤー種別を表す文字列の取得
        /// </summary>
        /// <returns></returns>
        public abstract string LayerType();

        /// <summary>
        /// メインキャンバスへの描画
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public abstract void Draw(Graphics g, int width, int height);

        /// <summary>
        /// レイヤリストへの描画（個別）
        /// </summary>
        /// <param name="e"></param>
        public abstract void DrawLayerListItem(LayerListDrawEventArgs e);

        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="e"></param>
        /// <param name="mouseControl"></param>
        /// <param name="document"></param>
        /// <param name="mainForm"></param>
        /// <returns></returns>
        internal abstract ThumbnailUpdateType OnMouseDown(JLMouseEventArgs e, MouseControlInfo mouseControl, Document document, MainForm mainForm);

        /// <summary>
        /// OnMouseMove
        /// </summary>
        /// <param name="e"></param>
        /// <param name="mouseControl"></param>
        /// <param name="document"></param>
        /// <param name="mainForm"></param>
        /// <returns></returns>
        internal abstract ThumbnailUpdateType OnMouseMove(JLMouseEventArgs e, MouseControlInfo mouseControl, Document document, MainForm mainForm);

        /// <summary>
        /// OnMouseUp
        /// </summary>
        /// <param name="e"></param>
        /// <param name="mouseControl"></param>
        /// <param name="document"></param>
        /// <param name="mainForm"></param>
        /// <returns></returns>
        internal abstract ThumbnailUpdateType OnMouseUp(JLMouseEventArgs e, MouseControlInfo mouseControl, Document document, MainForm mainForm);

        /// <summary>
        /// OnClick
        /// </summary>
        /// <param name="e"></param>
        /// <param name="mouseControl"></param>
        /// <param name="document"></param>
        /// <param name="mainForm"></param>
        /// <returns></returns>
        internal abstract ThumbnailUpdateType OnClick(JLMouseEventArgs e, MouseControlInfo mouseControl, Document document, MainForm mainForm);
    }
}
