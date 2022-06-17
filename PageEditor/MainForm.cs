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
            新規イメージバッファレイヤToolStripMenuItem.Tag = typeof(LayerImageList);

            // ドキュメントの生成
            document = CreateNewDocument();
            image = new Bitmap(document.Width, document.Height);

            // ListBoxの更新
            // TODO:SelectedChangedEventを受けて更新が必要なので名称や引数は追って検討
            ApplyList();

            // 描画更新を明示的に呼び出す。
            picturePanelSizeChanged(null, null);
            ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);
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

        //
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Control+V
            if (e.KeyCode == Keys.V && e.Control)
            {
                // クリップボードの内容を適用する。

                // 画像形式
                if (Clipboard.ContainsImage())
                {
                    if (MainFile != null)
                    {
                        Image image = Clipboard.GetImage();
                        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                        {
                            // メモリに書き込み
                            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                            byte[] data = memoryStream.ToArray();

                            // ファイル名をMD5から生成
                            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                            byte[] bs = md5.ComputeHash(data);
                            md5.Clear();
                            string fullPath = GetSaveFileName(DataDirectory + BitConverter.ToString(bs).ToLower().Replace("-","") + ".jpg");

                            System.IO.File.WriteAllBytes(fullPath, data);

                            // PictureControlに追加しておく
                            PictureControl.Add(fullPath, image);

                            // ImageList選択中の場合はそちらに張り付け
                            if (document.CurrentSheet.CurrentLayer is LayerImageList)
                            {
                                ImageItem item = new ImageItem();
                                item.SetZoom(GetRelative(fullPath), document.Width, document.Height, image);

                                using (new JLUILocker())
                                {
                                    // 描画更新をいったん止めて要素を更新
                                    imageListListBox.BeginUpdate();
                                    LayerImageList layer = document.CurrentSheet.CurrentLayer as LayerImageList;
                                    layer.ImageItems.Add(item);
                                    layer.SelectedIndex = layer.ImageItems.Count - 1;
                                    imageListListBox.Items.Add("");
                                    imageListListBox.SelectedIndex = imageListListBox.Items.Count - 1;
                                    imageListListBox.EndUpdate();
                                }

                                // 描画更新を明示的に呼び出す。
                                ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);
                                layerListBox.Invalidate(layerListBox.GetItemRectangle(layerListBox.SelectedIndex));
                            }
                            else
                            {
                                LayerImage layerImage = new LayerImage();

                                // documentに追加
                                document.CurrentSheet.Layers.Insert(document.CurrentSheet.SelectIndex + 1, layerImage);
                                document.CurrentSheet.CurrentLayer = layerImage;

                                layerImage.SetZoom(GetRelative(fullPath), document.Width, document.Height, image);

                                using (new JLUILocker())
                                {
                                    // 描画更新をいったん止めて要素を更新
                                    layerListBox.BeginUpdate();
                                    layerListBox.Items.Insert(layerListBox.SelectedIndex, layerImage);
                                    layerListBox.SelectedItem = document.CurrentSheet.CurrentLayer;
                                    layerListBox.EndUpdate();
                                }
                            }

                            // 描画更新を明示的に呼び出す。
                            ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);
                        }
                    }
                }

                e.Handled = true;
            }
        }

        private string GetSaveFileName(string v)
        {
            while (System.IO.File.Exists(v) == true)
            {
                v = System.IO.Path.GetDirectoryName(v) + "\\" + System.IO.Path.GetFileNameWithoutExtension(v) + "_copy" + System.IO.Path.GetExtension(v);
            }

            return v;
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
        private void picturePanelSizeChanged(object sender, EventArgs e)
        {
            // ドキュメントが開かれていない（未初期化）の場合は何もしない
            if (document == null)
                return;

            // --------------------------------------------------------------------------
            // メインキャンバスの再構築
            Size maxSize = new Size(splitContainer3.Panel1.Width, splitContainer3.Panel1.Height);

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
            ImageUpdate(ImageOperation.UpdateType.NONE);
        }

        /// <summary>
        /// メインキャンバスの描画更新
        /// </summary>
        /// <param name="sheet"></param>
        internal void ImageUpdate(ImageOperation.UpdateType updateType)
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
                g.FillRectangle(ImageDraw.GetClearBrush(), new Rectangle(0, 0, pictureBox1.Image.Width, pictureBox1.Image.Height));

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(image, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            }

            // サムネイル更新タイマースタート
            switch (updateType)
            {
                case ImageOperation.UpdateType.IMMEDIATELY:
                    timer1_Tick(null, null);
                    break;

                case ImageOperation.UpdateType.LATER:
                    timer1.Start();
                    break;

                default:
                case ImageOperation.UpdateType.NONE:
                    break;
            }    

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
            ImageOperation.UpdateType updateType = ImageOperation.MouseDown(new JLMouseEventArgs(e, pictureRate), document.CurrentSheet.CurrentLayer, document, this);
            if (updateType != ImageOperation.UpdateType.NONE)
                ImageUpdate(updateType);
        }

        /// <summary>
        /// MouseMove
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            ImageOperation.UpdateType updateType = ImageOperation.MouseMove(new JLMouseEventArgs(e, pictureRate), document.CurrentSheet.CurrentLayer, document, this);
            if (updateType != ImageOperation.UpdateType.NONE)
                ImageUpdate(updateType);
        }

        /// <summary>
        /// MouseUp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            ImageOperation.UpdateType updateType = ImageOperation.MouseUp(new JLMouseEventArgs(e, pictureRate), document.CurrentSheet.CurrentLayer, document, this);
            if (updateType != ImageOperation.UpdateType.NONE)
                ImageUpdate(updateType);
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
            sheetListBox.Invalidate(sheetListBox.GetItemRectangle(sheetListBox.SelectedIndex));
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
                ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);

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
                ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);

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
            ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);
        }

        private void シート名の変更ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newName = Microsoft.VisualBasic.Interaction.InputBox("シート名を入力してください", "入力", document.CurrentSheet.Name, -1, -1);

            if (string.IsNullOrEmpty(newName))
                return;

            document.CurrentSheet.Name = newName;
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
            ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);
        }

        private void sheetListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer1.Stop();

            if (JLUILocker.IsLocked())
                return;

            document.CurrentSheet = sheetListBox.SelectedItem as Sheet;

            ApplySheet();

            // 描画更新を明示的に呼び出す。
            ImageUpdate(ImageOperation.UpdateType.NONE);
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

        private void layerListBox_MouseDown(object sender, MouseEventArgs e)
        {
            int index = layerListBox.IndexFromPoint(layerListBox.PointToClient(Control.MousePosition));

            if (0 <= index && index < layerListBox.Items.Count)
            {
                if (layerListBox.GetItemRectangle(index).Contains(e.Location))
                {
                    Layer toLayer = layerListBox.Items[index] as Layer;

                    if (e.Button == MouseButtons.Left && e.X < 29)
                    {
                        toLayer.Visible = !toLayer.Visible;

                        // 表示更新
                        layerListBox.Invalidate(layerListBox.GetItemRectangle(index));

                        // 描画更新を明示的に呼び出す。
                        ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);
                    }

                    if (toLayer != document.CurrentSheet.CurrentLayer)
                    {
                        layerListBox.SelectedIndex = index;
                        document.CurrentSheet.CurrentLayer = layerListBox.SelectedItem as Layer;
                    }
                }
            }
        }

        // レイヤーリストのMouseUp
        private void layerListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.X > 29)
            {
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
            ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);
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
            ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);
        }

        /// <summary>
        /// 選択レイヤの変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void layerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (layerListBox.SelectedItem == null)
                return;

            switch (layerListBox.SelectedItem.GetType().Name)
            {
                case "LayerFill":
                    helpLabel.Text = "tips:画面上左クリックで色選択ウィンドウが表示されます。";
                    imageListListBox.Items.Clear();
                    imageListListBox.Enabled = false;
                    break;

                case "LayerImage":
                    helpLabel.Text = "tips:画面上左クリックでファイル選択ウィンドウが表示されます。";
                    imageListListBox.Items.Clear();
                    imageListListBox.Enabled = false;
                    break;

                case "LayerSpeechBaloon":
                    helpLabel.Text = "tips:左クリックで新規作成、吹き出し左ドラッグで移動、吹き出し左クリックで編集、右クリックで一括追加用のメニューが表示されます。";
                    imageListListBox.Items.Clear();
                    imageListListBox.Enabled = false;
                    break;

                case "LayerImageList":
                    helpLabel.Text = "tips:複数の画像から1枚の画像を選択できるレイヤーです。右下のImageViewから操作してください。";
                    {
                        LayerImageList layer = layerListBox.SelectedItem as LayerImageList;

                        using (new JLUILocker())
                        {
                            imageListListBox.SuspendLayout();
                            imageListListBox.Items.Clear();
                            for (int i = -1; i < layer.ImageItems.Count; i++)
                            {
                                imageListListBox.Items.Add(i.ToString());
                            }
                            imageListListBox.SelectedIndex = layer.SelectedIndex + 1;
                            imageListListBox.ResumeLayout();
                            imageListListBox.Enabled = true;
                        }
                    }
                    break;
            }

            document.CurrentSheet.CurrentLayer = layerListBox.SelectedItem as Layer;
        }

        #endregion

        #region

        #endregion

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawRectangle(Pens.Gray, e.Bounds);

            if (e.Index == 0)
            {
                Pen pen = new Pen(Brushes.Gray, 2);

                e.Graphics.DrawLine(pen, e.Bounds.Left + 20, e.Bounds.Top + 20, e.Bounds.Right - 20, e.Bounds.Bottom - 20);
                e.Graphics.DrawLine(pen, e.Bounds.Left + 20, e.Bounds.Bottom - 20, e.Bounds.Right - 20, e.Bounds.Top + 20);
            }
            else
            {
                LayerImageList layer = document.CurrentSheet.CurrentLayer as LayerImageList;

                if (0 <= e.Index - 1 && e.Index - 1 < layer.ImageItems.Count)
                {
                    ImageItem item = layer.ImageItems[e.Index - 1];

                    Image image = PictureControl.GetThumbImage(item.FileName);
                    if (image != null)
                        e.Graphics.DrawImage(image, new Rectangle(e.Bounds.Left + 2, e.Bounds.Top + 2, e.Bounds.Width - 4, e.Bounds.Height - 4));
                }
            }
        }

        private void imageListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (JLUILocker.IsLocked())
                return;

            LayerImageList layer = document.CurrentSheet.CurrentLayer as LayerImageList;
            layer.SelectedIndex = imageListListBox.SelectedIndex - 1;

            // 描画更新を明示的に呼び出す。
            ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);
            layerListBox.Invalidate(layerListBox.GetItemRectangle(layerListBox.SelectedIndex));
        }

        private void imageListListBox_MouseUp(object sender, MouseEventArgs e)
        {
            int index = imageListListBox.IndexFromPoint(imageListListBox.PointToClient(Control.MousePosition));

            if (0 <= index && index < imageListListBox.Items.Count)
            {
                if (imageListListBox.GetItemRectangle(index).Contains(e.Location))
                {
                    LayerImageList layer = document.CurrentSheet.CurrentLayer as LayerImageList;

                    imageListListBox.SelectedIndex = index;
                    // Documentの更新はChangedEventの中で処理

                    if (e.Button == MouseButtons.Right)
                    {
                        ImageListItem削除ToolStripMenuItem.Enabled = index != 0;
                        ImageListItem削除ToolStripMenuItem.Tag = index;
                        imageListOnItemMenu.Show(Cursor.Position);
                    }

                    return;
                }
            }

            // 範囲外クリック
            if (e.Button == MouseButtons.Right)
            {
                imageListOnSpaceMenu.Show(Cursor.Position);
            }
        }

        private void ImageListItem削除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            using (new JLUILocker())
            {
                // 描画更新をいったん止めて要素を更新
                imageListListBox.BeginUpdate();
                imageListListBox.Items.RemoveAt((int)item.Tag);
                LayerImageList layer = document.CurrentSheet.CurrentLayer as LayerImageList;
                layer.SelectedIndex = -1;
                layer.ImageItems.RemoveAt((int)item.Tag - 1);
                imageListListBox.EndUpdate();
            }

            // 描画更新を明示的に呼び出す。
            ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);
            layerListBox.Invalidate(layerListBox.GetItemRectangle(layerListBox.SelectedIndex));
        }

        private void ImageListItem追加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainForm.DataDirectory == null)
            {
                MessageBox.Show("画像ファイルを指定する際は先にワークスペースを保存してください。", "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // OpenFileDialog
            OpenFileDialog ofd = new OpenFileDialog();

            // はじめに表示されるフォルダを指定する
            ofd.InitialDirectory = MainForm.DataDirectory;

            // フィルター
            ofd.Filter = "イメージファイル(*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|すべてのファイル(*.*)|*.*";

            // タイトル
            ofd.Title = "画像ファイルを選択してください";

            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;

            // ダイアログを表示する
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // 選択されたファイルを設定
            string relative = MainForm.GetRelative(ofd.FileName);

            // 画像サイズを取得
            Image image = PictureControl.Load(relative);

            // 失敗
            if (image == null)
            {
                MessageBox.Show("画像ファイルを開けませんでした。", "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ImageItem item = new ImageItem();
            item.SetZoom(relative, document.Width, document.Height, image);

            using (new JLUILocker())
            {
                // 描画更新をいったん止めて要素を更新
                imageListListBox.BeginUpdate();
                LayerImageList layer = document.CurrentSheet.CurrentLayer as LayerImageList;
                layer.ImageItems.Add(item);
                layer.SelectedIndex = layer.ImageItems.Count - 1;
                imageListListBox.Items.Add("");
                imageListListBox.SelectedIndex = imageListListBox.Items.Count - 1;
                imageListListBox.EndUpdate();
            }

            // 描画更新を明示的に呼び出す。
            ImageUpdate(ImageOperation.UpdateType.IMMEDIATELY);
            layerListBox.Invalidate(layerListBox.GetItemRectangle(layerListBox.SelectedIndex));
        }
    }
}
