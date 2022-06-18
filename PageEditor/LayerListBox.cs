using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PageEditor
{
    class LayerListBox : ListBox
    {
        /// <summary>
        /// レイヤ表示状態変更イベント発生時の引数
        /// </summary>
        public class OnLayerVisibleChangedEventArgs : EventArgs
        {
            public int Index = -1;
            public bool beforeVisible = false;
        }

        [Browsable(true)]
        [Description("レイヤが追加されたときに発生するイベントです")]
        [Category("動作")]
        public event EventHandler<OnLayerVisibleChangedEventArgs> OnLayerVisibleChanged;

        /// <summary>
        /// レイヤ追加イベント発生時の引数
        /// </summary>
        public class OnLayerAddedEventArgs : EventArgs
        {
            public int Index = -1;
            public Layer AddedLayer = null;
        }

        [Browsable(true)]
        [Description("レイヤが追加されたときに発生するイベントです")]
        [Category("動作")]
        public event EventHandler<OnLayerAddedEventArgs> OnLayerAdded;

        /// <summary>
        /// レイヤ削除イベント発生時の引数
        /// </summary>
        public class OnLayerDeleteEventArgs : EventArgs
        {
            public int Index = -1;
            public Layer RemovedLayer = null;
        }

        [Browsable(true)]
        [Description("レイヤが削除されたときに発生するイベントです")]
        [Category("動作")]
        public event EventHandler<OnLayerDeleteEventArgs> OnLayerDeleted;



        // TODO:DefaultValueの設定



        // 枠線用ペン
        private Pen LinePen = null;

        // 透明ブラシ
        private Brush ClearBrush = null;

        // イベントの一時停止
        private class EventLocker : IDisposable { static int cnt = 0; public EventLocker() { cnt++; } public void Dispose() { cnt--; } public static bool IsLocked() { return cnt != 0; } }

        public LayerListBox()
            : base()
        {
            // ダブルバッファ有効
            DoubleBuffered = true;

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
            // 何してるんだろうね？
            base.OnDrawItem(e);

            // 背景色
            e.DrawBackground();

            // レイヤー特定
            Layer layer = (0 <= e.Index && e.Index < Items.Count ? Items[e.Index] as Layer : null);

            // 表示非表示状態
            if (layer != null && layer.Visible)
            {
                Rectangle boxV = new Rectangle(0, e.Bounds.Top, 29, e.Bounds.Height - 2);
                e.Graphics.DrawString("目", e.Font, Brushes.LightGray, boxV, new StringFormat(StringFormatFlags.NoWrap) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            // 枠線
            e.Graphics.DrawLine(LinePen, 29, e.Bounds.Top, 29, e.Bounds.Bottom - 1);
            e.Graphics.DrawLine(LinePen, 0, e.Bounds.Bottom - 1, e.Bounds.Width, e.Bounds.Bottom - 1);

            // レイヤの文字列
            if (layer != null)
            {
                Rectangle boxS = new Rectangle(60, e.Bounds.Top, 100, e.Bounds.Height - 2);
                e.Graphics.DrawString(layer.LayerType(), e.Font, Brushes.White, boxS, new StringFormat(StringFormatFlags.NoWrap) { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });

                switch (layer.GetType().Name)
                {
                    case "LayerSpeechBaloon":
                        DrawLayerSpeechBaloon(layer as LayerSpeechBaloon, e);
                        break;
                    case "LayerFill":
                        DrawLayerFill(layer as LayerFill, e);
                        break;
                    case "LayerImage":
                        DrawLayerImage(layer as LayerImage, e);
                        break;
                    case "LayerImageList":
                        DrawLayerImageList(layer as LayerImageList, e);
                        break;
                }
            }
        }

        /// <summary>
        /// 塗りつぶしレイヤーを描画します。
        /// </summary>
        private static void DrawLayerFill(LayerFill layer, DrawItemEventArgs e)
        {
            // 塗りつぶしの枠だけ。
            Rectangle box = new Rectangle(35, e.Bounds.Top + (e.Bounds.Height - 18 - 2) / 2, 18, 18);
            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), box);
            e.Graphics.FillRectangle(new SolidBrush(layer.BackColor), box);
            e.Graphics.DrawRectangle(Pens.White, box);
        }

        /// <summary>
        /// イメージレイヤーを描画します。
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="e"></param>
        private static void DrawLayerImage(LayerImage layer, DrawItemEventArgs e)
        {
            Image thumbImage = PictureControl.GetThumbImage(layer.FileName);

            Rectangle box = new Rectangle(35, e.Bounds.Top + (e.Bounds.Height - 20) / 2, 18, 18);
            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), box);
            if (thumbImage != null)
                e.Graphics.DrawImage(thumbImage, box);
            e.Graphics.DrawRectangle(Pens.White, box);
        }

        /// <summary>
        /// 吹き出しレイヤーを描画します。
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="e"></param>
        private static void DrawLayerSpeechBaloon(LayerSpeechBaloon layer, DrawItemEventArgs e)
        {
            // 塗りつぶしの枠だけ。
            Rectangle box = new Rectangle(35, e.Bounds.Top + (e.Bounds.Height - 20) / 2, 18, 18);
            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), box);
            e.Graphics.DrawRectangle(Pens.White, box);

            Rectangle baloon = new Rectangle(box.Left + 3, box.Top + 2, 12, 14);
            e.Graphics.FillEllipse(Brushes.White, baloon);
            e.Graphics.DrawEllipse(Pens.DarkGray, baloon);
        }

        /// <summary>
        /// イメージリストレイヤーを描画します。
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="e"></param>
        private static void DrawLayerImageList(LayerImageList layer, DrawItemEventArgs e)
        {
            Rectangle box = new Rectangle(35, e.Bounds.Top + (e.Bounds.Height - 20) / 2, 18, 18);
            e.Graphics.FillRectangle(ImageDraw.GetClearBrush(), box);
            ImageItem info = layer.CurrentImage;
            if (info != null)
            {
                Image thumbImage = PictureControl.GetThumbImage(info.FileName);
                if (thumbImage != null)
                    e.Graphics.DrawImage(thumbImage, box);
            }
            e.Graphics.DrawRectangle(Pens.White, box);
        }

        /// <summary>
        /// レイヤの設定
        /// </summary>
        /// <param name="sheets">レイヤリスト</param>
        /// <param name="selectSheet">選択レイヤ</param>
        internal void SetLayers(Layer[] layers, Layer selectLayer)
        {
            // 描画更新開始
            BeginUpdate();

            // 全要素削除
            Items.Clear();

            // 一括追加
            if (layers != null && layers.Length > 0)
            {
                List<Layer> revLayers = new List<Layer>(layers);
                revLayers.Reverse();

                Items.AddRange(revLayers.ToArray());
            }

            // 選択項目
            SelectedItem = selectLayer;

            // 描画更新再開
            EndUpdate();
        }

        /// <summary>
        /// レイヤ配列の取得
        /// </summary>
        /// <returns></returns>
        internal Layer[] GetLayers()
        {
            // 空の場合
            if (Items == null || Items.Count == 0)
                return null;

            // 戻り値用
            Layer[] retValue = new Layer[Items.Count];
            for (int i = Items.Count - 1; i >= 0; i--)
                retValue[i] = Items[i] as Layer;

            return retValue;
        }
        
        // マウスダウン
        protected override void OnMouseDown(MouseEventArgs e)
        {
            int index = IndexFromPoint(PointToClient(Control.MousePosition));

            if (0 <= index && index < Items.Count)
            {
                if (GetItemRectangle(index).Contains(e.Location))
                {
                    Layer toLayer = Items[index] as Layer;

                    if (e.Button == MouseButtons.Left && e.X < 29)
                    {
                        toLayer.Visible = !toLayer.Visible;

                        // 表示更新
                        Invalidate(GetItemRectangle(index));

                        // イベント発行
                        OnLayerVisibleChanged(this, new OnLayerVisibleChangedEventArgs() { Index = index, beforeVisible = !toLayer.Visible });
                    }
                }
            }

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
                // メニュー
                ToolStripMenuItem onAddLayerFill = new ToolStripMenuItem() { Text = "新規塗り潰しレイヤ", Enabled = true, Tag = typeof(LayerFill) };
                ToolStripMenuItem onAddLayerImage = new ToolStripMenuItem() { Text = "新規イメージレイヤ", Enabled = true, Tag = typeof(LayerImage) };
                ToolStripMenuItem onAddLayerSpeechBaloon = new ToolStripMenuItem() { Text = "新規吹き出しレイヤ", Enabled = true, Tag = typeof(LayerSpeechBaloon) };
                ToolStripMenuItem onAddLayerImageList = new ToolStripMenuItem() { Text = "新規イメージリストレイヤ", Enabled = true, Tag = typeof(LayerImageList) };
                onAddLayerFill.Click += OnAddClick;
                onAddLayerImage.Click += OnAddClick;
                onAddLayerSpeechBaloon.Click += OnAddClick;
                onAddLayerImageList.Click += OnAddClick;

                ToolStripMenuItem onDelete = new ToolStripMenuItem() { Text = "削除", Enabled = false };
                onDelete.Click += OnDelete_Click;

                ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                contextMenuStrip.Items.Add(onAddLayerFill);
                contextMenuStrip.Items.Add(onAddLayerImage);
                contextMenuStrip.Items.Add(onAddLayerSpeechBaloon);
                contextMenuStrip.Items.Add(onAddLayerImageList);
                contextMenuStrip.Items.Add("-");
                contextMenuStrip.Items.Add(onDelete);

                int index = IndexFromPoint(PointToClient(Control.MousePosition));
                if (0 <= index && index < Items.Count)
                {
                    if (GetItemRectangle(index).Contains(e.Location))
                    {
                        // 違う場合は設定変更
                        if (SelectedIndex != index)
                            SelectedIndex = index;

                        // 削除は可能
                        onDelete.Enabled = true;
                    }
                }

                contextMenuStrip.Show(System.Windows.Forms.Cursor.Position);
            }

            base.OnMouseUp(e);
        }

        private void OnAddClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if (item.Tag == null)
                return;

            Layer layer = (Layer)Activator.CreateInstance((Type)item.Tag);

            if (layer == null)
                return;

            using (new EventLocker())
            {
                Items.Insert(SelectedIndex, layer);
            }

            // 追加イベントを先に発行
            OnLayerAdded(this, new OnLayerAddedEventArgs() { Index = SelectedIndex, AddedLayer = layer });

            // 選択シート変更
            SelectedItem = layer;
        }

        private void OnDelete_Click(object sender, EventArgs e)
        {
            // シート特定
            Layer layer = SelectedItem as Layer;

            if (MessageBox.Show("[" + layer.LayerType() + "]レイヤを削除します。よろしいですか？", "メッセージ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;

            // イベント発行
            OnLayerDeleted(this, new OnLayerDeleteEventArgs() { Index = SelectedIndex, RemovedLayer = layer });

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
