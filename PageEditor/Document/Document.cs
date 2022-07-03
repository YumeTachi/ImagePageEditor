using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace PageEditor
{
    /// <summary>
    /// ドキュメント
    /// </summary>
    [Serializable]
    public class Document
    {
        /// <summary>画像サイズ</summary>
        [XmlAttribute]
        public int Width;

        /// <summary>画像サイズ</summary>
        [XmlAttribute]
        public int Height;

        /// <summary>シートリスト(空でも非null)</summary>
        public List<Sheet> Sheets;

        /// <summary>選択中シートのIndex</summary>
        [XmlAttribute]
        public int SelectIndex;

        /// <summary>現在選択中のシート</summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Sheet CurrentSheet
        {
            get
            {
                return Sheets[SelectIndex];
            }
            set
            {
                SelectIndex = Sheets.FindIndex(a => a == value);
                System.Diagnostics.Debug.Assert(SelectIndex != -1);
            }
        }

        /// <summary>コンストラクタ</summary>
        public Document()
        {
            Width = 1280 * 2;
            Height = 720 * 2;
            Sheets = null;
            SelectIndex = -1;
        }
    }
}
