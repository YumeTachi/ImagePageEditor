using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PageEditor
{
    /// <summary>
    /// レイヤーのスーパークラス
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(LayerFill))]
    [XmlInclude(typeof(LayerImage))]
    [XmlInclude(typeof(LayerSpeechBaloon))]
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
    }

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
    }

    /// <summary>
    /// 画像レイヤ
    /// </summary>
    [Serializable]
    public class LayerImage : Layer
    {
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
            FileName = null;
            X = 0;
            Y = 0;
            Angle = 0;
            Scale = 1;
        }
    }

    [Serializable]
    public class SpeechBaloon
    {
        public enum BaloonKind
        {
            Box,
            RoundedCorner1,
        }

        public enum FontSizeKind
        {
            Middle,
            Small,
            Large,
        }

        /// <summary>種別</summary>
        [XmlAttribute]
        public BaloonKind Kind;

        /// <summary>文字サイズ</summary>
        [XmlAttribute]
        public FontSizeKind FontSize;

        /// <summary>表示位置右上X</summary>
        [XmlAttribute]
        public int X;

        /// <summary>表示位置右上Y</summary>
        [XmlAttribute]
        public int Y;

        /// <summary>テキスト</summary>
        public string Text;

        /// <summary>テキストの描画色</summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Color ForeColor;

        /// <summary>テキストの背景色</summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Color BackColor;

        /// <summary>テキストの描画色</summary>
        [XmlAttribute(AttributeName = "ForeColor")]
        public string _ForeColor
        {
            get { return ColorTranslator.ToHtml(ForeColor); }
            set { ForeColor = ColorTranslator.FromHtml(value); }
        }

        /// <summary>テキストの背景色</summary>
        [XmlAttribute(AttributeName = "BackColor")]
        public string _BackColor
        {
            get { return ColorTranslator.ToHtml(BackColor); }
            set { BackColor = ColorTranslator.FromHtml(value); }
        }

        /// <summary>フォント</summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Font Font 
        {
            get
            {
                // サイズに応じて何を返すか決める。
                switch (FontSize)
                {
                    case FontSizeKind.Small:
                        return ImageDraw.DefaultFontSmall;

                    case FontSizeKind.Large:
                        return ImageDraw.DefaultFontLarge;

                    case FontSizeKind.Middle:
                        return ImageDraw.DefaultFontNormal;
                }

                return ImageDraw.DefaultFontNormal;
            }
        }

        /// <summary>
        /// ツール用テキスト範囲
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Rectangle TextRect;

        /// <summary>
        /// ツール用テキスト範囲（選択用）
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public Rectangle SelectRect;

        public SpeechBaloon()
        {
            Kind = BaloonKind.RoundedCorner1;
            FontSize = FontSizeKind.Middle;
            Text = "";
            ForeColor = Color.DarkGray;
            BackColor = Color.White;
        }

        public SpeechBaloon(SpeechBaloon arg)
        {
            Kind       = arg.Kind;
            FontSize   = arg.FontSize;
            X          = arg.X;
            Y          = arg.Y;
            Text       = arg.Text.Clone() as string;
            ForeColor  = arg.ForeColor;
            BackColor  = arg.BackColor;
            TextRect   = arg.TextRect;
            SelectRect = arg.SelectRect;
        }

    }

    [Serializable]
    public class LayerSpeechBaloon : Layer
    {
        public List<SpeechBaloon> SpeechBaloons;

        /// <summary>
        /// レイヤー種別を表す文字列の取得
        /// </summary>
        /// <returns></returns>
        public override string LayerType()
        {
            return "吹き出し";
        }

        /// <summary>コンストラクタ</summary>
        public LayerSpeechBaloon() : base()
        {
            SpeechBaloons = new List<SpeechBaloon>();
        }

        /// <summary>
        /// 指定位置に吹き出しがあるか確認
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        internal int FindIndex(Point location)
        {
            for (int i = 0; i < SpeechBaloons.Count; i++)
            {
                if (SpeechBaloons[i].SelectRect.Contains(location))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 指定位置に吹き出しがあるか確認
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        internal SpeechBaloon Find(Point location)
        {
            foreach (SpeechBaloon speechBaloon in SpeechBaloons)
            {
                if (speechBaloon.SelectRect.Contains(location))
                {
                    return speechBaloon;
                }
            }
            return null;
        }
    }

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
