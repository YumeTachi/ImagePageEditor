using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace PageEditor
{
    /// <summary>
    /// シート
    /// </summary>
    [Serializable]
    public class Sheet
    {
        /// <summary>レイヤー名</summary>
        public string Name;

        /// <summary>レイヤーリスト(空でも非null)</summary>
        public List<Layer> Layers;

        /// <summary>選択中レイヤーのIndex</summary>
        [XmlAttribute]
        public int SelectIndex;

        /// <summary>現在選択中のレイヤ</summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Layer CurrentLayer
        {
            get
            {
                return Layers[SelectIndex];
            }
            set
            {
                SelectIndex = Layers.FindIndex(a => a == value);
                System.Diagnostics.Debug.Assert(SelectIndex != -1);
            }
        }

        /// <summary>サムネイル</summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Image Thumbnail;

        /// <summary>
        /// サムネイル
        /// </summary>
        public string ThumbnailData
        {
            get
            {
                if (Thumbnail == null)
                    return null;

                // バイト列に変換
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    Thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] bytes = ms.ToArray();

                    return Convert.ToBase64String(bytes);
                }
            }
            set
            {
                if (value == null || value.Length == 0)
                    Thumbnail = null;

                byte[] bytes = Convert.FromBase64String(value);

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes))
                {
                    Thumbnail = Image.FromStream(ms);
                }
            }
        }

        /// <summary>コンストラクタ</summary>
        public Sheet()
        {
            Name = "新規シート";
            Layers = null;
            SelectIndex = -1;
        }
    }
}
