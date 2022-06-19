using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PageEditor
{
    class SheetListBox : ListBox
    {
        /// <summary>
        /// シート追加イベント発生時の引数
        /// </summary>
        public class OnSheetAddedEventArgs : EventArgs
        {
            public int Index;
            public Sheet AddedSheet;
        }

        [Browsable(true)]
        [Description("シートが追加されたときに発生するイベントです")]
        [Category("動作")]
        public event EventHandler<OnSheetAddedEventArgs> OnSheetAdded;

        /// <summary>
        /// 名称変更イベント発生時の引数
        /// </summary>
        public class OnSheetRenamedEventArgs : EventArgs
        {
            public int Index;
            public string BeforeName;
            public string AfterName;
        }

        [Browsable(true)]
        [Description("シート名称が変更されたときに発生するイベントです")]
        [Category("動作")]
        public event EventHandler<OnSheetRenamedEventArgs> OnSheetRenamed;

        /// <summary>
        /// シート削除イベント発生時の引数
        /// </summary>
        public class OnSheetDeleteEventArgs : EventArgs
        {
            public int Index;
            public Sheet RemovedSheet;
        }

        [Browsable(true)]
        [Description("シートが削除されたときに発生するイベントです")]
        [Category("動作")]
        public event EventHandler<OnSheetDeleteEventArgs> OnSheetDeleted;

        /// <summary>
        /// シート並び替えイベント発生時の引数
        /// </summary>
        public class OnSheetOrderChangedEventArgs : EventArgs
        {
            public int OldIndex = -1;
            public int NewIndex = -1;
        }

        [Browsable(true)]
        [Description("シートの順序が変更されたときに発生するイベントです")]
        [Category("動作")]
        public event EventHandler<OnSheetOrderChangedEventArgs> OnSheetOrderChanged;



        // TODO:DefaultValueの設定



        // 枠線用ペン
        private Pen LinePen = null;

        // 透明ブラシ
        private Brush ClearBrush = null;

        // イベントの一時停止
        private class EventLocker : IDisposable { static int cnt = 0; public EventLocker() { cnt++; } public void Dispose() { cnt--; } public static bool IsLocked() { return cnt != 0; } }

        public SheetListBox()
            : base()
        {
            // ダブルバッファ有効
            DoubleBuffered = true;

            // DragDrop可能
            AllowDrop = true;

            // 枠線用ペン
            LinePen = new Pen(Brushes.Black, 2.0f);

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

            // 枠線
            e.Graphics.DrawLine(LinePen, 40, e.Bounds.Top, 40, e.Bounds.Bottom - 1);
            e.Graphics.DrawLine(LinePen, 0, e.Bounds.Bottom - 1, e.Bounds.Width, e.Bounds.Bottom - 1);

            // シート番号
            Rectangle box1 = new Rectangle(0, e.Bounds.Top, 36, e.Bounds.Height - 2);
            e.Graphics.DrawString((e.Index + 1).ToString(), e.Font, Brushes.White, box1, new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });

            // サムネイル背景表示
            Rectangle box2 = new Rectangle(44, e.Bounds.Top + (e.Bounds.Height - 2 - 28) / 2, 48, 28);
            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), box2);

            // シート特定
            Sheet sheet = (0 <= e.Index && e.Index < Items.Count ? Items[e.Index] as Sheet : null);

            // シートが有効の場合
            if (sheet != null)
            {
                // サムネイル表示
                if (sheet.Thumbnail != null)
                {
                    e.Graphics.DrawImage(sheet.Thumbnail, box2);
                }
            }

            // サムネイル枠表示
            e.Graphics.DrawRectangle(Pens.Black, box2);

            // タイトル
            string title = sheet != null ? sheet.Name : "no name";
            Rectangle box3 = new Rectangle(100, e.Bounds.Top, 100, e.Bounds.Height - 2);
            e.Graphics.DrawString(title, e.Font, Brushes.White, box3, new StringFormat(StringFormatFlags.NoWrap) { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
        }

        /// <summary>
        /// シートの設定
        /// </summary>
        /// <param name="sheets">シートリスト</param>
        /// <param name="selectSheet">選択シート</param>
        internal void SetSheets(Sheet[] sheets, Sheet selectSheet)
        {
            // 描画更新開始
            BeginUpdate();

            // 全要素削除
            Items.Clear();

            // 一括追加
            if (sheets != null && sheets.Length > 0)
                Items.AddRange(sheets);

            // 選択項目
            SelectedItem = selectSheet;

            // 描画更新再開
            EndUpdate();
        }

        /// <summary>
        /// シート配列の取得
        /// </summary>
        /// <returns></returns>
        internal Sheet[] GetSeets()
        {
            // 空の場合
            if (Items == null || Items.Count == 0)
                return null;

            // 戻り値用
            Sheet[] retValue = new Sheet[Items.Count];
            for (int i = 0; i < Items.Count; i++)
                retValue[i] = Items[i] as Sheet;

            return retValue;
        }

        private Point mouseLeftDownPoint = Point.Empty;
        private int mouseLeftDownIndex = -1;

        // マウスダウン
        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseLeftDownPoint = Point.Empty;
            mouseLeftDownIndex = -1;

            int index = IndexFromPoint(e.Location);

            if (0 <= index && index < Items.Count)
            {
                if (GetItemRectangle(index).Contains(e.Location))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        mouseLeftDownPoint = new Point(e.X, e.Y);
                        mouseLeftDownIndex = index;
                    }
                }
            }

            base.OnMouseDown(e);
        }

        // マウス移動
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseLeftDownPoint != Point.Empty && mouseLeftDownIndex != -1)
            {
                //ドラッグとしないマウスの移動範囲を取得する
                Rectangle moveRect = new Rectangle(
                    mouseLeftDownPoint.X - SystemInformation.DragSize.Width / 2,
                    mouseLeftDownPoint.Y - SystemInformation.DragSize.Height / 2,
                    SystemInformation.DragSize.Width,
                    SystemInformation.DragSize.Height);

                //ドラッグとする移動範囲を超えたか調べる
                if (!moveRect.Contains(e.X, e.Y))
                {
                    DoDragDrop(mouseLeftDownIndex, DragDropEffects.Move);    //ドラッグスタート

                    mouseLeftDownPoint = Point.Empty;
                    mouseLeftDownIndex = -1;
                }
            }

            base.OnMouseMove(e);
        }

        // ドラッグ開始
        protected override void OnDragEnter(DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;

            base.OnDragEnter(e);
        }

        // 並び替え
        protected override void OnDragDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(int)))
            {
                Point location = PointToClient(new Point(e.X, e.Y));

                // ドロップされたデータ
                int sourceIndex = (int)e.Data.GetData(typeof(int));

                // ドロップ先
                // これは行き先を決めるもので出力ではない。
                int targetIndex = IndexFromPoint(location);

                // -1になるのは範囲外なので一番後ろ
                if (targetIndex == -1)
                {
                    targetIndex = Items.Count;
                }
                else
                {
                    // 箱の下半分の場合は次の場所に配置する。
                    Rectangle itemRectangle = GetItemRectangle(targetIndex);
                    if (location.Y >= (itemRectangle.Top + itemRectangle.Bottom) / 2)
                    {
                        targetIndex++;
                    }
                }

                // 調整
                if (sourceIndex <= targetIndex)
                {
                    targetIndex--;
                }

                // 行き先と違う場合
                if (sourceIndex != targetIndex)
                {
                    object source = Items[sourceIndex];

                    using (new EventLocker())
                    {
                        // 元のツリーから削除
                        Items.Remove(source);

                        // Nodeを追加
                        Items.Insert(targetIndex, source);
                    }

                    // 移動イベント発行
                    OnSheetOrderChanged(this, new OnSheetOrderChangedEventArgs() { OldIndex = sourceIndex, NewIndex = targetIndex });

                    SelectedItem = source;
                }
            }

            base.OnDragDrop(e);
        }

        // マウスアップ
        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseLeftDownPoint = Point.Empty;
            mouseLeftDownIndex = -1;

            if (e.Button == MouseButtons.Right)
            {
                // メニュー
                ToolStripMenuItem onAdd = new ToolStripMenuItem() { Text = "シート追加", Enabled = true };
                onAdd.Click += OnAdd_Click;
                ToolStripMenuItem onRename = new ToolStripMenuItem() { Text = "名称変更", Enabled = false };
                onRename.Click += OnRename_Click;
                ToolStripMenuItem onDelete = new ToolStripMenuItem() { Text = "削除", Enabled = false };
                onDelete.Click += OnDelete_Click;

                ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                contextMenuStrip.Items.Add(onAdd);
                contextMenuStrip.Items.Add("-");
                contextMenuStrip.Items.Add(onRename);
                contextMenuStrip.Items.Add("-");
                contextMenuStrip.Items.Add(onDelete);

                int index = IndexFromPoint(e.Location);
                if (0 <= index && index < Items.Count)
                {
                    if (GetItemRectangle(index).Contains(e.Location))
                    {
                        // 違う場合は設定変更
                        if (SelectedIndex != index)
                            SelectedIndex = index;

                        // 名称変更は可能
                        onRename.Enabled = true;

                        // 削除は少なくとも２つ以上なければ不可
                        onDelete.Enabled = (Items.Count >= 2);
                    }
                }

                contextMenuStrip.Show(System.Windows.Forms.Cursor.Position);
            }

            base.OnMouseUp(e);
        }

        private void OnAdd_Click(object sender, EventArgs e)
        {
            Sheet sheet = new Sheet();
            sheet.Layers = new List<Layer>();
            sheet.Layers.Add(new LayerFill());
            sheet.Layers.Add(new LayerImage());
            sheet.Layers.Add(new LayerSpeechBaloon());
            sheet.SelectIndex = 0;

            using (new EventLocker())
            {
                Items.Insert(SelectedIndex + 1, sheet);
            }

            // 追加イベントを先に発行
            OnSheetAdded(this, new OnSheetAddedEventArgs() { Index = SelectedIndex + 1, AddedSheet = sheet });

            // 選択シート変更
            SelectedItem = sheet;
        }

        // ToolStripの名称変更押下
        private void OnRename_Click(object sender, EventArgs e)
        {
            // シート特定
            Sheet sheet = SelectedItem as Sheet;

            // 覚えておく
            string oldName = sheet.Name;

            // 新規名称取得
            string newName = Microsoft.VisualBasic.Interaction.InputBox("シート名を入力してください", "入力", oldName, -1, -1);
            if (string.IsNullOrEmpty(newName))
                return;

            // 名称更新
            sheet.Name = newName;

            // 再描画
            Invalidate(GetItemRectangle(SelectedIndex));

            // イベント発行
            OnSheetRenamed(this, new OnSheetRenamedEventArgs() { Index = SelectedIndex, BeforeName = oldName, AfterName = newName });
        }

        private void OnDelete_Click(object sender, EventArgs e)
        {
            // シート特定
            Sheet sheet = SelectedItem as Sheet;

            if (MessageBox.Show("シート[" + sheet.Name + "]を削除します。よろしいですか？", "メッセージ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;

            // イベント発行
            OnSheetDeleted(this, new OnSheetDeleteEventArgs() { Index = SelectedIndex, RemovedSheet = sheet });

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
