using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PageEditor
{
    public partial class MainForm : Form
    {
        private static MainForm m_Instance = null;
        public static MainForm GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new MainForm();

            return m_Instance;
        }

        /// <summary>
        /// Pictureファイルの探索基準となるデータディレクトリのルート
        /// MainFileが c:\main.jlw だとすると、
        /// DataDirectory は c:\main\ になる。
        /// </summary>
        static public string DataDirectory
        { 
            get
            {
                if (MainFile == null)
                    return null;

                return System.IO.Path.GetDirectoryName(MainFile) + "\\" + System.IO.Path.GetFileNameWithoutExtension(MainFile) + "\\";
            }
        }
        /// <summary>
        /// メインファイル
        /// </summary>
        static private string MainFile { get; set; } 


        Document document = null;
        Image image = null;

        float pictureRate = 1.0f;


        private MainForm()
        {
            InitializeComponent();

            新規塗り潰しレイヤToolStripMenuItem.Tag = typeof(LayerFill);
            新規イメージレイヤToolStripMenuItem.Tag = typeof(LayerImage);
            新規吹き出しレイヤToolStripMenuItem.Tag = typeof(LayerSpeechBaloon);

            // ドキュメントの生成
            document = CreateNewDocument();
            image = new Bitmap(document.Width, document.Height);

            // ListBoxの更新
            // TODO:SelectedChangedEventを受けて更新が必要なので名称や引数は追って検討
            ApplyList();

            // 描画更新を明示的に呼び出す。
            splitContainer1_Panel2_SizeChanged(null, null);
        }

        /// <summary>
        /// 空のドキュメントの生成
        /// </summary>
        /// <returns></returns>
        private Document CreateNewDocument()
        {
            Document retValue = new Document();

            Sheet sheet = new Sheet();
            sheet.Layers = new List<Layer>();
            sheet.Layers.Add(new LayerFill());
            sheet.Layers.Add(new LayerImage());
            sheet.Layers.Add(new LayerSpeechBaloon());
            sheet.SelectIndex = 0;

            retValue.Sheets = new List<Sheet>();
            retValue.Sheets.Add(sheet);
            retValue.SelectIndex = 0;

            return retValue;
        }

        /// <summary>
        /// WorkDirectoryを使用して絶対パスを取得します。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetAbsolute(string fileName)
        {
            if (DataDirectory == null)
                return null;

            return DataDirectory + fileName;
        }

        /// <summary>
        /// WorkDirectoryを使用して相対パスを取得します。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetRelative(string fileName)
        {
            if (DataDirectory == null)
                return null;

            if (fileName.StartsWith(DataDirectory))
                return fileName.Substring(DataDirectory.Length);

            return null;
        }

        /// <summary>
        /// リスト更新
        /// </summary>
        private void ApplyList()
        {
            using (new JLUILocker())
            {
                layerListBox.BeginUpdate();
                sheetListBox.BeginUpdate();

                sheetListBox.Items.Clear();
                layerListBox.Items.Clear();

                foreach (Sheet sheet in document.Sheets)
                {
                    sheetListBox.Items.Add(sheet);
                }
                sheetListBox.SelectedItem = document.CurrentSheet;

                List<Layer> rev = new List<Layer>(document.CurrentSheet.Layers);
                rev.Reverse();
                foreach (Layer layer in rev)
                {
                    layerListBox.Items.Add(layer);
                }
                layerListBox.SelectedItem = document.CurrentSheet.CurrentLayer;

                layerListBox.EndUpdate();
                sheetListBox.EndUpdate();
            }
        }

        /// <summary>
        /// リスト更新
        /// </summary>
        private void ApplySheet()
        {
            using (new JLUILocker())
            {
                layerListBox.BeginUpdate();

                layerListBox.Items.Clear();

                List<Layer> rev = new List<Layer>(document.CurrentSheet.Layers);
                rev.Reverse();
                foreach (Layer layer in rev)
                {
                    layerListBox.Items.Add(layer);
                }
                layerListBox.SelectedItem = document.CurrentSheet.CurrentLayer;

                layerListBox.EndUpdate();
            }
        }


        private void クリップボードにコピーToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
                return;

            Clipboard.SetImage(image);
        }

        //---------------------------------------------------------------------------------------------------------
        // PictureBox処理系
        #region

        /// <summary>
        /// 画像イメージのリサイズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
        {
            // ドキュメントが開かれていない（未初期化）の場合は何もしない
            if (document == null)
                return;

            // --------------------------------------------------------------------------
            // メインキャンバスの再構築
            Size maxSize = new Size(splitContainer1.Panel2.Width, splitContainer1.Panel2.Height);

            if (maxSize.Width < 10 || maxSize.Height < 10)
                return;

            pictureRate = Math.Min((float)maxSize.Width / document.Width,
                                   (float)maxSize.Height / document.Height);

            if (pictureRate <= 0.1f)
                return;

            pictureBox1.Width = (int)(document.Width * pictureRate);
            pictureBox1.Height = (int)(document.Height * pictureRate);

            Point position = new Point((maxSize.Width - pictureBox1.Width) / 2, (maxSize.Height - pictureBox1.Height) / 2);

            pictureBox1.Location = position;

            // --------------------------------------------------------------------------
            // 描画更新
            ImageUpdate();
        }

        /// <summary>
        /// メインキャンバスの描画更新
        /// </summary>
        /// <param name="sheet"></param>
        public void ImageUpdate()
        {
            Sheet sheet = document.CurrentSheet;

            // Imageがなければ生成
            if (pictureBox1.Image == null
                || pictureBox1.Image.Width != pictureBox1.Width
                || pictureBox1.Image.Height != pictureBox1.Height)
            {
                // 生成済みの場合は消去。これはどのくらい効果があるのかはよくわからない。
                if (pictureBox1.Image != null)
                    pictureBox1.Image.Dispose();

                // イメージの再生成。
                // ここで一度描画反映されてしまうが気にしない。
                pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            }

            // 描画メイン
            ImageDraw.Draw(image, sheet.Layers);

            // メインキャンバスに転送
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(image, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            }

            // サムネイル更新タイマースタート
            timer1.Start();

            // 描画更新
            pictureBox1.Refresh();
        }

        /// <summary>
        /// MouseDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ImageOperation.MouseDown(new JLMouseEventArgs(e, pictureRate), document.CurrentSheet.CurrentLayer, document, this))
                ImageUpdate();
        }

        /// <summary>
        /// MouseMove
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (ImageOperation.MouseMove(new JLMouseEventArgs(e, pictureRate), document.CurrentSheet.CurrentLayer, document, this))
                ImageUpdate();
        }

        /// <summary>
        /// MouseUp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (ImageOperation.MouseUp(new JLMouseEventArgs(e, pictureRate), document.CurrentSheet.CurrentLayer, document, this))
                ImageUpdate();
        }

        // サムネイル更新処理
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            if (image == null)
                return;

            // サムネイルの作成
            Image thumbnail = image.GetThumbnailImage(48, 28, delegate { return false; }, IntPtr.Zero);

            // 更新
            document.CurrentSheet.Thumbnail = thumbnail;

            // 描画更新
            sheetListBox.Refresh();
        }
        #endregion

        //---------------------------------------------------------------------------------------------------------
        // ファイル系
        #region

        /// <summary>
        /// ワークスペース→新規作成押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 新規作成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainFile != null)
            {
                if (MessageBox.Show("すでにワークスペースを開いています。新規作成しますか？", "メッセージ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }

            // OpenFileDialog
            SaveFileDialog sfd = new SaveFileDialog();

            // フィルター
            sfd.Filter = "ワークスペース(*.xml)|*.xml|すべてのファイル(*.*)|*.*";

            // タイトル
            sfd.Title = "ワークスペースファイルを選択してください";

            // 拡張子を自動付与
            sfd.AddExtension = true;

            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;

            // ダイアログを表示する
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                MainFile = sfd.FileName;
                System.IO.Directory.CreateDirectory(DataDirectory);

                // ドキュメントの生成
                document = CreateNewDocument();
                image = new Bitmap(document.Width, document.Height);

                // ListBoxの更新
                // TODO:SelectedChangedEventを受けて更新が必要なので名称や引数は追って検討
                ApplyList();

                // 描画更新を明示的に呼び出す。
                splitContainer1_Panel2_SizeChanged(null, null);

                // Documentを保存
                DataControl.SaveXML(MainFile, document);

                this.Text = "ワークスペース:" + MainFile;
                保存ToolStripMenuItem.Text = "上書き保存";
            }
        }

        /// <summary>
        /// 開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainFile != null)
            {
                if (MessageBox.Show("すでにワークスペースを開いています。新規作成しますか？", "メッセージ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }

            // OpenFileDialog
            OpenFileDialog ofd = new OpenFileDialog();

            // フィルター
            ofd.Filter = "ワークスペース(*.xml)|*.xml|すべてのファイル(*.*)|*.*";

            // タイトル
            ofd.Title = "ワークスペースファイルを選択してください";

            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;

            // ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                MainFile = ofd.FileName;

                // ドキュメントの読み込み
                document = DataControl.LoadXML<Document>(MainFile);
                image = new Bitmap(document.Width, document.Height);

                // ListBoxの更新
                // TODO:SelectedChangedEventを受けて更新が必要なので名称や引数は追って検討
                ApplyList();

                // 描画更新を明示的に呼び出す。
                splitContainer1_Panel2_SizeChanged(null, null);

                this.Text = "ワークスペース:" + MainFile;
                保存ToolStripMenuItem.Text = "上書き保存";
            }
        }

        /// <summary>
        /// 保存／上書き保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 上書き保存
            if (MainFile != null)
            {
                // Documentを保存
                DataControl.SaveXML(MainFile, document);
            }
            // 保存
            else
            {
                // OpenFileDialog
                SaveFileDialog sfd = new SaveFileDialog();

                // フィルター
                sfd.Filter = "ワークスペース(*.xml)|*.xml|すべてのファイル(*.*)|*.*";

                // タイトル
                sfd.Title = "ワークスペースファイルを選択してください";

                // 拡張子を自動付与
                sfd.AddExtension = true;

                // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                sfd.RestoreDirectory = true;

                // ダイアログを表示する
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    MainFile = sfd.FileName;
                    System.IO.Directory.CreateDirectory(DataDirectory);

                    // Documentを保存
                    DataControl.SaveXML(MainFile, document);

                    this.Text = "ワークスペース:" + MainFile;
                    保存ToolStripMenuItem.Text = "上書き保存";
                }
            }
        }

        #endregion

        //---------------------------------------------------------------------------------------------------------
        // シートリスト処理
        #region

        /// <summary>
        /// レイヤーリストの描画更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sheetListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            SheetListDraw.Draw(sheetListBox.Items[e.Index] as Sheet, e);
        }

        // シートリストのMouseUp
        private void sheetListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = sheetListBox.IndexFromPoint(sheetListBox.PointToClient(Control.MousePosition));

                if (0 <= index && index < sheetListBox.Items.Count)
                {
                    if (sheetListBox.GetItemRectangle(index).Contains(e.Location))
                    {
                        sheetListBox.SelectedIndex = index;
                        document.CurrentSheet = sheetListBox.SelectedItem as Sheet;
                    }

                    シート名の変更ToolStripMenuItem.Enabled = true;
                }
                else
                {
                    シート名の変更ToolStripMenuItem.Enabled = false;
                }

                シート削除ToolStripMenuItem.Enabled = document.Sheets.Count >= 2;
                sheetMenu.Show(System.Windows.Forms.Cursor.Position);
            }
        }

        private void 新規シートの作成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sheet sheet = new Sheet();
            sheet.Layers = new List<Layer>();
            sheet.Layers.Add(new LayerFill());
            sheet.Layers.Add(new LayerImage());
            sheet.Layers.Add(new LayerSpeechBaloon());
            sheet.SelectIndex = 0;

            // documentに追加
            document.Sheets.Insert(document.SelectIndex + 1, sheet);
            document.CurrentSheet = sheet;

            using (new JLUILocker())
            {
                // 描画更新をいったん止めて要素を更新
                sheetListBox.BeginUpdate();
                sheetListBox.Items.Insert(sheetListBox.SelectedIndex + 1, sheet);
                sheetListBox.SelectedItem = document.CurrentSheet;
                sheetListBox.EndUpdate();
            }

            ApplySheet();

            // 描画更新を明示的に呼び出す。
            splitContainer1_Panel2_SizeChanged(null, null);
        }

        private void シート名の変更ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newName = Microsoft.VisualBasic.Interaction.InputBox("シート名を入力してください", "入力", document.CurrentSheet.Name, -1, -1);

            if (string.IsNullOrEmpty(newName))
                return;

            document.CurrentSheet.Name = newName;

            ApplySheet();
        }

        private void シート削除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // document側の変更
            // 現在レイヤの削除
            document.Sheets.Remove(document.CurrentSheet);
            document.SelectIndex = document.SelectIndex == 0 ? 0 : document.SelectIndex - 1;

            using (new JLUILocker())
            {
                // 描画更新をいったん止めて要素を更新
                sheetListBox.BeginUpdate();
                sheetListBox.Items.RemoveAt(sheetListBox.SelectedIndex);
                sheetListBox.SelectedItem = document.CurrentSheet;
                sheetListBox.EndUpdate();
            }

            ApplySheet();

            // 描画更新を明示的に呼び出す。
            splitContainer1_Panel2_SizeChanged(null, null);
        }

        private void sheetListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer1.Stop();

            if (JLUILocker.IsLocked())
                return;

            document.CurrentSheet = sheetListBox.SelectedItem as Sheet;

            ApplySheet();

            // 描画更新を明示的に呼び出す。
            splitContainer1_Panel2_SizeChanged(null, null);
        }

        #endregion

        //---------------------------------------------------------------------------------------------------------
        // レイヤーリスト処理
        #region

        /// <summary>
        /// レイヤーリストの描画更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void layerListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            // 対象レイヤの描画処理を呼び出す。
            LayerListDraw.Draw(layerListBox.Items[e.Index] as Layer, e);
        }

        // レイヤーリストのMouseUp
        private void layerListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = layerListBox.IndexFromPoint(layerListBox.PointToClient(Control.MousePosition));

                if (0 <= index && index < layerListBox.Items.Count)
                {
                    if (layerListBox.GetItemRectangle(index).Contains(e.Location))
                    {
                        layerListBox.SelectedIndex = index;
                        document.CurrentSheet.CurrentLayer = layerListBox.SelectedItem as Layer;
                    }
                }

                レイヤ削除ToolStripMenuItem.Enabled = document.CurrentSheet.Layers.Count >= 2;
                layerMenu.Show(System.Windows.Forms.Cursor.Position);
            }
        }

        private void レイヤ削除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // document側の変更
            // 現在レイヤの削除
            document.CurrentSheet.Layers.Remove(document.CurrentSheet.CurrentLayer);
            document.CurrentSheet.SelectIndex = document.CurrentSheet.SelectIndex == 0 ? 0 : document.CurrentSheet.SelectIndex - 1;

            using (new JLUILocker())
            {
                // 描画更新をいったん止めて要素を更新
                layerListBox.BeginUpdate();
                layerListBox.Items.RemoveAt(layerListBox.SelectedIndex);
                layerListBox.SelectedItem = document.CurrentSheet.CurrentLayer;
                layerListBox.EndUpdate();
            }

            // 描画更新を明示的に呼び出す。
            splitContainer1_Panel2_SizeChanged(null, null);
        }

        private void 新規レイヤToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if (item.Tag == null)
                return;

            Layer layer = (Layer)Activator.CreateInstance((Type)item.Tag);

            if (layer == null)
                return;

            // documentに追加
            document.CurrentSheet.Layers.Insert(document.CurrentSheet.SelectIndex + 1, layer);
            document.CurrentSheet.CurrentLayer = layer;

            using (new JLUILocker())
            {
                // 描画更新をいったん止めて要素を更新
                layerListBox.BeginUpdate();
                layerListBox.Items.Insert(layerListBox.SelectedIndex, layer);
                layerListBox.SelectedItem = document.CurrentSheet.CurrentLayer;
                layerListBox.EndUpdate();
            }

            // 描画更新を明示的に呼び出す。
            splitContainer1_Panel2_SizeChanged(null, null);
        }

        /// <summary>
        /// 選択レイヤの変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void layerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (JLUILocker.IsLocked())
                return;

            document.CurrentSheet.CurrentLayer = layerListBox.SelectedItem as Layer;
        }

        #endregion
    }
}
