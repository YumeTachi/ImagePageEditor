using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PageEditor
{
    class ImageListBox : ListBox
    {
        /// <summary>
        /// イメージ追加イベント発生時の引数
        /// </summary>
        public class OnImageAddedEventArgs : EventArgs
        {
            public int Index = -1;
            public ImageItem AddedImage = null;
        }

        [Browsable(true)]
        [Description("イメージが追加されたときに発生するイベントです")]
        [Category("動作")]
        public event EventHandler<OnImageAddedEventArgs> OnImageAdded;

        /// <summary>
        /// イメージ削除イベント発生時の引数
        /// </summary>
        public class OnImageDeleteEventArgs : EventArgs
        {
            public int Index = -1;
            public ImageItem RemovedImage = null;
        }

        [Browsable(true)]
        [Description("レイヤが削除されたときに発生するイベントです")]
        [Category("動作")]
        public event EventHandler<OnImageDeleteEventArgs> OnImageDeleted;



        // TODO:DefaultValueの設定



        // 透明ブラシ
        private Brush ClearBrush = null;

        // イベントの一時停止
        private class EventLocker : IDisposable { static public bool ForceLock = true; static int cnt = 0; public EventLocker() { cnt++; } public void Dispose() { cnt--; } public static bool IsLocked() { return ForceLock == false && cnt != 0; } }

        public ImageListBox()
            : base()
        {
            // ダブルバッファ有効
            DoubleBuffered = true;

            // 透明ブラシ
            Image clearBrushTexture = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(clearBrushTexture))
            {
                g.Clear(Color.White);
                g.FillRectangle(Brushes.LightGray, new Rectangle(0, 0, 8, 8));
                g.FillRectangle(Brushes.LightGray, new Rectangle(8, 8, 8, 8));
            }
            ClearBrush = new TextureBrush(clearBrushTexture);
        }

        // Itemの描画
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // 背景色
            e.DrawBackground();

            e.Graphics.DrawRectangle(Pens.Gray, e.Bounds);

            if (e.Index <= 0)
            {
                Pen pen = new Pen(Brushes.Gray, 2);

                int cx = (e.Bounds.Left + e.Bounds.Right) / 2;
                int cy = (e.Bounds.Top + e.Bounds.Bottom) / 2;
                int size = Math.Min(e.Bounds.Width, e.Bounds.Height) * 7 / 10;

                e.Graphics.DrawLine(pen, cx - size / 2, cy + size / 2, cx + size / 2, cy - size / 2);
                e.Graphics.DrawLine(pen, cx - size / 2, cy - size / 2, cx + size / 2, cy + size / 2);
            }
            else
            {
                ImageItem item = (0 <= e.Index && e.Index < Items.Count ? Items[e.Index] as ImageItem : null);

                if (item != null)
                {
                    PictureControl.PictureInfo pictureInfo = MainForm.GetInstance().Pictures.Load(item.FileName);
                    if (pictureInfo != null)
                        e.Graphics.DrawImage(pictureInfo.Picture, new Rectangle(e.Bounds.Left + 3, e.Bounds.Top + 3, e.Bounds.Width - 4, e.Bounds.Height - 4));
                }
            }
        }

        /// <summary>
        /// イメージの全作成
        /// </summary>
        internal void ClearImages()
        {
            // Lockしておく
            EventLocker.ForceLock = true;
            Items.Clear();
        }

        /// <summary>
        /// イメージの設定
        /// </summary>
        /// <param name="sheets">レイヤリスト</param>
        /// <param name="selectSheet">選択レイヤ</param>
        internal void SetImages(ImageItem[] imageItems, ImageItem selectImage)
        {
            // 描画更新開始
            BeginUpdate();

            // 全要素削除
            Items.Clear();

            // 一括追加
            Items.Add(ImageItem.Empty);
            if (imageItems != null && imageItems.Length > 0)
            {
                Items.AddRange(imageItems);
            }

            // Lock解除
            EventLocker.ForceLock = false;

            // 選択項目
            SelectedItem = selectImage;

            // 描画更新再開
            EndUpdate();
        }

        /// <summary>
        /// レイヤ配列の取得
        /// </summary>
        /// <returns></returns>
        internal ImageItem[] GetImages()
        {
            // 空の場合
            if (Items == null || Items.Count == 0)
                return null;

            // 戻り値用
            ImageItem[] retValue = new ImageItem[Items.Count - 1];
            for (int i = 1; i < Items.Count; i++)
                retValue[i - 1] = Items[i] as ImageItem;

            return retValue;
        }

        // マウスダウン
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        // マウス移動
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        // マウスアップ
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = IndexFromPoint(e.Location);
                if (0 <= index && index < Items.Count)
                {
                    if (GetItemRectangle(index).Contains(e.Location))
                    {
                        // 違う場合は設定変更
                        if (SelectedIndex != index)
                            SelectedIndex = index;

                        // メニュー
                        ToolStripMenuItem onDelete = new ToolStripMenuItem() { Text = "削除", Enabled = false };
                        onDelete.Click += OnDelete_Click;

                        ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                        contextMenuStrip.Items.Add(onDelete);

                        onDelete.Enabled = index != 0;
                        onDelete.Tag = index;
                        contextMenuStrip.Show(Cursor.Position);

                        base.OnMouseUp(e);
                        return;
                    }
                }

                // 範囲外クリック
                if (e.Button == MouseButtons.Right)
                {
                    // メニュー
                    ToolStripMenuItem onAdd = new ToolStripMenuItem() { Text = "イメージ追加", Enabled = true };
                    onAdd.Click += OnAdd_Click; ;

                    ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                    contextMenuStrip.Items.Add(onAdd);
                    contextMenuStrip.Show(Cursor.Position);
                }
            }

            base.OnMouseUp(e);
        }

        private void OnAdd_Click(object sender, EventArgs e)
        {
            if (MainForm.GetInstance().DataDirectory == null)
            {
                MessageBox.Show("画像ファイルを指定する際は先にワークスペースを保存してください。", "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // OpenFileDialog
            OpenFileDialog ofd = new OpenFileDialog();

            // はじめに表示されるフォルダを指定する
            ofd.InitialDirectory = MainForm.GetInstance().DataDirectory;

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
            string relative = MainForm.GetInstance().GetRelativePath(ofd.FileName);

            // 画像サイズを取得
            PictureControl.PictureInfo image = MainForm.GetInstance().Pictures.Load(relative);

            // 失敗
            if (image == null)
            {
                MessageBox.Show("画像ファイルを開けませんでした。", "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ImageItem item = new ImageItem();
            item.SetZoom(relative, MainForm.GetInstance().Document.Width, MainForm.GetInstance().Document.Height, image.Picture);

            // 追加本体
            Add(item);
        }

        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="item"></param>
        public void Add(ImageItem item)
        {
            using (new EventLocker())
            {
                Items.Add(item);
            }

            // 追加イベントを先に発行
            OnImageAdded(this, new OnImageAddedEventArgs() { Index = Items.Count - 1, AddedImage = item });

            // 選択シート変更
            SelectedItem = item;
        }

        private void OnDelete_Click(object sender, EventArgs e)
        {
            // シート特定
            ImageItem image = SelectedItem as ImageItem;

            // イベント発行
            OnImageDeleted(this, new OnImageDeleteEventArgs() { Index = SelectedIndex, RemovedImage = image });

            // 新レイヤの番号
            int newSelectedIndex = SelectedIndex == 0 ? 0 : SelectedIndex - 1;

            // 描画更新をいったん止めて要素を更新
            BeginUpdate();
            using (new EventLocker())
            {
                Items.RemoveAt(SelectedIndex);
            }
            SelectedIndex = newSelectedIndex;
            EndUpdate();
        }


        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (EventLocker.IsLocked())
                return;

            base.OnSelectedIndexChanged(e);
        }

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            if (EventLocker.IsLocked())
                return;

            base.OnSelectedValueChanged(e);
        }
    }
}
