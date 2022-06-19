using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
        internal string DataDirectory
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
        private string MainFile { get; set; }

        /// <summary>
        /// ドキュメント本体
        /// </summary>
        internal Document Document { get; set; }

        /// <summary>
        /// PictureControl
        /// </summary>
        internal PictureControl Pictures { get; set; }



        Image image = null;

        float pictureRate = 1.0f;


        private MainForm()
        {
            Console.WriteLine("MainForm Initialize start");

            InitializeComponent();

            Pictures = new PictureControl();

            sheetListBox.OnSheetAdded += SheetListBox_OnSheetAdded;
            sheetListBox.OnSheetRenamed += SheetListBox_OnSheetRenamed;
            sheetListBox.OnSheetDeleted += SheetListBox_OnSheetDeleted;
            sheetListBox.OnSheetOrderChanged += SheetListBox_OnSheetOrderChanged;

            layerListBox.OnLayerVisibleChanged += LayerListBox_OnLayerVisibleChanged;
            layerListBox.OnLayerAdded += LayerListBox_OnLayerAdded;
            layerListBox.OnLayerDeleted += LayerListBox_OnLayerDeleted;
            layerListBox.OnLayerOrderChanged += LayerListBox_OnLayerOrderChanged;

            imageListListBox.OnImageAdded += ImageListListBox_OnImageAdded;
            imageListListBox.OnImageDeleted += ImageListListBox_OnImageDeleted;


            // ドキュメントの生成
            Document = CreateNewDocument();
            image = new Bitmap(Document.Width, Document.Height);

            // メインキャンバスの設定


            Console.WriteLine("MainForm Initialize finish");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Console.WriteLine("MainForm Load start");

            // ListBoxの更新
            sheetListBox.SetSheets(Document.Sheets.ToArray(), Document.CurrentSheet);
            layerListBox.SetLayers(Document.CurrentSheet.Layers.ToArray(), Document.CurrentSheet.CurrentLayer);

            // 描画更新を明示的に呼び出す。
            picturePanelSizeChanged(null, null);
            ImageUpdate(ImageOperation.ThumbnailUpdateType.IMMEDIATELY);

            Console.WriteLine("MainForm Load end");
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
        public string GetAbsolutePath(string fileName)
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
        public string GetRelativePath(string fileName)
        {
            if (DataDirectory == null)
                return null;

            if (fileName.StartsWith(DataDirectory))
                return fileName.Substring(DataDirectory.Length);

            return null;
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
                            Pictures.Add(fullPath, image);

                            // ImageList選択中の場合はそちらに張り付け
                            if (Document.CurrentSheet.CurrentLayer is LayerImageList)
                            {
                                // 設定は以下のシーケンスで行う。
                                // (1)ImageItemの生成
                                // (2)ImageListBoxに追加
                                // -------------------------ここまでがこの部分での処理
                                // (3)ImageListBoxからAddedイベントの発行→Documentに反映
                                // (4)ImageListBoxからIndexChangedイベントの発行→Document/描画に反映

                                ImageItem item = new ImageItem();
                                item.SetZoom(GetRelativePath(fullPath), Document.Width, Document.Height, image);

                                imageListListBox.Add(item);
                            }
                            else
                            {
                                // 設定は以下のシーケンスで行う。
                                // (1)LayerImageの生成
                                // (2)LayerListBoxに追加
                                // -------------------------ここまでがこの部分での処理
                                // (3)LayerListBoxからAddedイベントの発行→Documentに反映
                                // (4)LayerListBoxからIndexChangedイベントの発行→Document/描画に反映

                                LayerImage layerImage = new LayerImage();
                                layerImage.SetZoom(GetRelativePath(fullPath), Document.Width, Document.Height, image);

                                layerListBox.Insert(layerImage);
                            }
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
            if (Document == null)
                return;

            // --------------------------------------------------------------------------
            // メインキャンバスの再構築
            Size maxSize = new Size(splitContainer3.Panel1.Width, splitContainer3.Panel1.Height);

            if (maxSize.Width < 10 || maxSize.Height < 10)
                return;

            pictureRate = Math.Min((float)maxSize.Width / Document.Width,
                                   (float)maxSize.Height / Document.Height);

            if (pictureRate <= 0.1f)
                return;

            pictureBox1.Width = (int)(Document.Width * pictureRate);
            pictureBox1.Height = (int)(Document.Height * pictureRate);

            // Imageがない、あるいはサイズが違えば生成
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

            // 位置変更
            Point position = new Point((maxSize.Width - pictureBox1.Width) / 2, (maxSize.Height - pictureBox1.Height) / 2);
            pictureBox1.Location = position;

            // --------------------------------------------------------------------------
            // 描画更新
            ImageUpdate(ImageOperation.ThumbnailUpdateType.NONE);
        }

        /// <summary>
        /// メインキャンバスの描画更新
        /// </summary>
        /// <param name="sheet"></param>
        internal void ImageUpdate(ImageOperation.ThumbnailUpdateType thumbnailUpdateType)
        {
            // メインキャンバスの準備ができていない場合
            if (pictureBox1.Image == null)
                return;

            // 描画メイン
            // サムネイル更新が不要以外の場合は描画更新があったはずなので再描画する。
            if (thumbnailUpdateType != ImageOperation.ThumbnailUpdateType.NONE)
            {
                Sheet sheet = Document.CurrentSheet;
                ImageDraw.Draw(image, sheet.Layers);
            }

            // メインキャンバスに転送
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.FillRectangle(ImageDraw.GetClearBrush(), new Rectangle(0, 0, pictureBox1.Image.Width, pictureBox1.Image.Height));

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(image, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            }

            // サムネイル更新
            switch (thumbnailUpdateType)
            {
                case ImageOperation.ThumbnailUpdateType.IMMEDIATELY:
                    // 即時更新
                    timer1_Tick(null, null);
                    break;

                case ImageOperation.ThumbnailUpdateType.LATER:
                    // タイマーによる更新
                    timer1.Start();
                    break;

                default:
                case ImageOperation.ThumbnailUpdateType.NONE:
                    // 更新なし
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
            ImageOperation.ThumbnailUpdateType updateType = ImageOperation.MouseDown(new JLMouseEventArgs(e, pictureRate), Document.CurrentSheet.CurrentLayer, Document, this);
            if (updateType != ImageOperation.ThumbnailUpdateType.NONE)
                ImageUpdate(updateType);
        }

        /// <summary>
        /// MouseMove
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            ImageOperation.ThumbnailUpdateType updateType = ImageOperation.MouseMove(new JLMouseEventArgs(e, pictureRate), Document.CurrentSheet.CurrentLayer, Document, this);
            if (updateType != ImageOperation.ThumbnailUpdateType.NONE)
                ImageUpdate(updateType);
        }

        /// <summary>
        /// MouseUp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            ImageOperation.ThumbnailUpdateType updateType = ImageOperation.MouseUp(new JLMouseEventArgs(e, pictureRate), Document.CurrentSheet.CurrentLayer, Document, this);
            if (updateType != ImageOperation.ThumbnailUpdateType.NONE)
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
            Document.CurrentSheet.Thumbnail = thumbnail;

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
                Document = CreateNewDocument();
                image = new Bitmap(Document.Width, Document.Height);

                // ListBoxの更新
                sheetListBox.SetSheets(Document.Sheets.ToArray(), Document.CurrentSheet);
                layerListBox.SetLayers(Document.CurrentSheet.Layers.ToArray(), Document.CurrentSheet.CurrentLayer);

                // 描画更新を明示的に呼び出す。
                ImageUpdate(ImageOperation.ThumbnailUpdateType.IMMEDIATELY);

                // Documentを保存
                DataControl.SaveXML(MainFile, Document);

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
                Document = DataControl.LoadXML<Document>(MainFile);
                image = new Bitmap(Document.Width, Document.Height);

                // ListBoxの更新
                sheetListBox.SetSheets(Document.Sheets.ToArray(), Document.CurrentSheet);
                layerListBox.SetLayers(Document.CurrentSheet.Layers.ToArray(), Document.CurrentSheet.CurrentLayer);

                // 描画更新を明示的に呼び出す。
                ImageUpdate(ImageOperation.ThumbnailUpdateType.IMMEDIATELY);

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
                DataControl.SaveXML(MainFile, Document);
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
                    DataControl.SaveXML(MainFile, Document);

                    this.Text = "ワークスペース:" + MainFile;
                    保存ToolStripMenuItem.Text = "上書き保存";
                }
            }
        }

        #endregion

        //---------------------------------------------------------------------------------------------------------
        // シートリスト処理
        #region

        private void SheetListBox_OnSheetAdded(object sender, SheetListBox.OnSheetAddedEventArgs e)
        {
            Console.WriteLine("Sheet Added : " + e.Index.ToString());

            // Documentの更新
            Document.Sheets = sheetListBox.GetSeets().ToList();
        }

        private void SheetListBox_OnSheetRenamed(object sender, SheetListBox.OnSheetRenamedEventArgs e)
        {
            Console.WriteLine("Sheet Name Changed : " + e.Index.ToString() + "  " + e.BeforeName + " -> " + e.AfterName);
        }

        private void SheetListBox_OnSheetDeleted(object sender, SheetListBox.OnSheetDeleteEventArgs e)
        {
            Console.WriteLine("Sheet Deleted : " + e.Index.ToString());

            // Documentの更新
            Document.Sheets = sheetListBox.GetSeets().ToList();
        }

        private void SheetListBox_OnSheetOrderChanged(object sender, SheetListBox.OnSheetOrderChangedEventArgs e)
        {
            Console.WriteLine("Sheet Order Changed : " + e.OldIndex.ToString() + " -> " + e.NewIndex.ToString());

            // Documentの更新
            Document.Sheets = sheetListBox.GetSeets().ToList();
        }

        private void sheetListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer1.Stop();

            if (JLUILocker.IsLocked())
                return;

            // Documentの更新
            Document.CurrentSheet = sheetListBox.SelectedItem as Sheet;

            layerListBox.SetLayers(Document.CurrentSheet.Layers.ToArray(), Document.CurrentSheet.CurrentLayer);

            // 描画更新を明示的に呼び出す。
            ImageUpdate(ImageOperation.ThumbnailUpdateType.IMMEDIATELY);
        }

        #endregion

        //---------------------------------------------------------------------------------------------------------
        // レイヤーリスト処理
        #region

        private void LayerListBox_OnLayerVisibleChanged(object sender, LayerListBox.OnLayerVisibleChangedEventArgs e)
        {
             // 描画更新を明示的に呼び出す。
            ImageUpdate(ImageOperation.ThumbnailUpdateType.IMMEDIATELY);
        }

        private void LayerListBox_OnLayerAdded(object sender, LayerListBox.OnLayerAddedEventArgs e)
        {
            Console.WriteLine("Layer Added : " + e.Index.ToString());

            Document.CurrentSheet.Layers = layerListBox.GetLayers().ToList();
        }

        private void LayerListBox_OnLayerDeleted(object sender, LayerListBox.OnLayerDeleteEventArgs e)
        {
            Console.WriteLine("Layer Deleted : " + e.Index.ToString());

            Document.CurrentSheet.Layers = layerListBox.GetLayers().ToList();
        }

        private void LayerListBox_OnLayerOrderChanged(object sender, LayerListBox.OnLayerOrderChangedEventArgs e)
        {
            Console.WriteLine("Layer Order Changed : " + e.OldIndex.ToString() + " -> " + e.NewIndex.ToString());

            Document.CurrentSheet.Layers = layerListBox.GetLayers().ToList();

            // IndexChangedでは再描画されないのでここで再描画
            // 描画更新を明示的に呼び出す。
            ImageUpdate(ImageOperation.ThumbnailUpdateType.IMMEDIATELY);
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

            Document.CurrentSheet.CurrentLayer = layerListBox.SelectedItem as Layer;

            switch (layerListBox.SelectedItem.GetType().Name)
            {
                case "LayerFill":
                    helpLabel.Text = "tips:画面上左クリックで色選択ウィンドウが表示されます。";
                    imageListListBox.ClearImages();
                    imageListListBox.Enabled = false;
                    break;

                case "LayerImage":
                    helpLabel.Text = "tips:画面上左クリックでファイル選択ウィンドウが表示されます。";
                    imageListListBox.ClearImages();
                    imageListListBox.Enabled = false;
                    break;

                case "LayerSpeechBaloon":
                    helpLabel.Text = "tips:左クリックで新規作成、吹き出し左ドラッグで移動、吹き出し左クリックで編集、右クリックで一括追加用のメニューが表示されます。";
                    imageListListBox.ClearImages();
                    imageListListBox.Enabled = false;
                    break;

                case "LayerImageList":
                    helpLabel.Text = "tips:複数の画像から1枚の画像を選択できるレイヤーです。右下のImageViewから操作してください。";
                    {
                        LayerImageList layer = layerListBox.SelectedItem as LayerImageList;

                        imageListListBox.SetImages(layer.ImageItems.ToArray(), layer.CurrentImage);
                        imageListListBox.Enabled = true;
                    }
                    break;
            }
        }

        #endregion

        //---------------------------------------------------------------------------------------------------------
        // イメージリスト処理
        #region

        private void ImageListListBox_OnImageAdded(object sender, ImageListBox.OnImageAddedEventArgs e)
        {
            Console.WriteLine("Image Added : " + e.Index.ToString());

            // Documentの更新
            LayerImageList layer = Document.CurrentSheet.CurrentLayer as LayerImageList;
            layer.ImageItems = imageListListBox.GetImages().ToList();
        }

        private void ImageListListBox_OnImageDeleted(object sender, ImageListBox.OnImageDeleteEventArgs e)
        {
            Console.WriteLine("Image Deleted : " + e.Index.ToString());

            // Documentの更新
            LayerImageList layer = Document.CurrentSheet.CurrentLayer as LayerImageList;
            layer.ImageItems = imageListListBox.GetImages().ToList();
        }

        private void imageListListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LayerImageList layer = Document.CurrentSheet.CurrentLayer as LayerImageList;
            layer.CurrentImage = imageListListBox.SelectedItem as ImageItem;

            // 描画更新を明示的に呼び出す。
            ImageUpdate(ImageOperation.ThumbnailUpdateType.IMMEDIATELY);
            layerListBox.Invalidate(layerListBox.GetItemRectangle(layerListBox.SelectedIndex));
        }

        #endregion
    }
}
