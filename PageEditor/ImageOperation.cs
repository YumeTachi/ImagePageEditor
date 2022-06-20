using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PageEditor
{
    static class ImageOperation
    {
        internal enum ThumbnailUpdateType
        {
            NONE,
            IMMEDIATELY,
            LATER
        }

        private class MouseControlInfo
        {
            public enum ControlStatus
            {
                None,
                Deferment,
                Move
            }

            public ControlStatus Status;
            public MouseButtons Button;
            public Point DownPoint;
            public Point DownPointRaw;
            public Point ObjectPoint;
            public object Object;

            public MouseControlInfo()
            {
                Status = ControlStatus.None;
            }

            public MouseControlInfo(MouseControlInfo arg)
            {
                Status        = arg.Status;
                Button        = arg.Button;
                DownPoint     = arg.DownPoint;
                DownPointRaw  = arg.DownPointRaw;
                ObjectPoint   = arg.ObjectPoint;
                Object        = arg.Object;
            }
        }

        static MouseControlInfo MouseControl = new MouseControlInfo();


        internal static ThumbnailUpdateType MouseDown(JLMouseEventArgs e, Layer layer, Document document, MainForm mainForm)
        {
            // 処理中なら何もしない
            if (MouseControl.Status != MouseControlInfo.ControlStatus.None)
                return ThumbnailUpdateType.NONE;

            // コントロールを開始
            MouseControl.Status = MouseControlInfo.ControlStatus.Deferment;
            MouseControl.Button = e.Button;
            MouseControl.DownPoint = e.Location;
            MouseControl.DownPointRaw = e.LocationRaw;
            MouseControl.Object = null;

            switch (layer.GetType().Name)
            {
                case "LayerSpeechBaloon":

                    {
                        LayerSpeechBaloon layerSpeechBaloon = layer as LayerSpeechBaloon;

                        SpeechBaloon speechBaloon = layerSpeechBaloon.Find(e.Location);

                        if (speechBaloon != null)
                        {
                            MouseControl.Object = speechBaloon;
                            MouseControl.ObjectPoint = new Point(speechBaloon.X, speechBaloon.Y);
                        }
                    }
                    break;

                case "LayerImage":

                    {
                        LayerImage layerImage = layer as LayerImage;

                        MouseControl.ObjectPoint = new Point(layerImage.X, layerImage.Y);
                    }
                    break;
            }

            return ThumbnailUpdateType.NONE;
        }

        internal static ThumbnailUpdateType MouseMove(JLMouseEventArgs e, Layer layer, Document document, MainForm mainForm)
        {
            if (MouseControl.Status == MouseControlInfo.ControlStatus.Deferment)
            {
                //ドラッグとしないマウスの移動範囲を取得する
                Rectangle moveRect = new Rectangle(
                    MouseControl.DownPointRaw.X - SystemInformation.DragSize.Width / 2,
                    MouseControl.DownPointRaw.Y - SystemInformation.DragSize.Height / 2,
                    SystemInformation.DragSize.Width,
                    SystemInformation.DragSize.Height);

                //ドラッグとする移動範囲を超えたか調べる
                if (!moveRect.Contains(e.LocationRaw.X, e.LocationRaw.Y))
                {
                    MouseControl.Status = MouseControlInfo.ControlStatus.Move;
                }
            }

            if (MouseControl.Status == MouseControlInfo.ControlStatus.Move)
            {
                switch (layer.GetType().Name)
                {
                    case "LayerSpeechBaloon":

                        if (e.Button == MouseButtons.Left && MouseControl.Object != null)
                        {
                            SpeechBaloon speechBaloon = MouseControl.Object as SpeechBaloon;

                            speechBaloon.X = MouseControl.ObjectPoint.X + (e.Location.X - MouseControl.DownPoint.X);
                            speechBaloon.Y = MouseControl.ObjectPoint.Y + (e.Location.Y - MouseControl.DownPoint.Y);

                            return ThumbnailUpdateType.LATER;
                        }
                        break;

                    case "LayerImage":

                        if (e.Button == MouseButtons.Left)
                        {
                            LayerImage layerImage = layer as LayerImage;

                            layerImage.X = MouseControl.ObjectPoint.X + (e.Location.X - MouseControl.DownPoint.X);
                            layerImage.Y = MouseControl.ObjectPoint.Y + (e.Location.Y - MouseControl.DownPoint.Y);

                            return ThumbnailUpdateType.LATER;
                        }
                        break;
                }
            }

            return ThumbnailUpdateType.NONE;
        }

        internal static ThumbnailUpdateType MouseUp(JLMouseEventArgs e, Layer layer, Document document, MainForm mainForm)
        {
            MouseControlInfo mouseControlTmp = new MouseControlInfo(MouseControl);
            MouseControl.Status = MouseControlInfo.ControlStatus.None;

            // クリック確定
            if (mouseControlTmp.Status == MouseControlInfo.ControlStatus.Deferment && mouseControlTmp.Button == e.Button)
            {
                switch (layer.GetType().Name)
                {
                    case "LayerSpeechBaloon":

                        if (mouseControlTmp.Button == MouseButtons.Left)
                        {
                            // オブジェクト上でクリック
                            if (mouseControlTmp.Object != null)
                            {
                                SpeechBaloon speechBaloon = mouseControlTmp.Object as SpeechBaloon;

                                // 開いているものは閉じる
                                CloseTextEditor();

                                // テキストエディタの表示
                                m_TextEditor = new TextEditer("編集");
                                if (m_TextEditorLocation != Point.Empty)
                                    m_TextEditor.Location = m_TextEditorLocation;
                                m_TextEditor.Set(speechBaloon);
                                m_TextEditor.Show(mainForm);

                                return ThumbnailUpdateType.IMMEDIATELY;
                            }
                            else
                            {
                                // 新規作成
                                TextEditer editer = new TextEditer("新規作成");
                                if (editer.ShowDialog() == DialogResult.OK)
                                {
                                    LayerSpeechBaloon layerSpeechBaloon = layer as LayerSpeechBaloon;

                                    SpeechBaloon newSpeechBaloon = editer.CreateSpeechBaloon();
                                    newSpeechBaloon.X = e.Location.X;
                                    newSpeechBaloon.Y = e.Location.Y;

                                    layerSpeechBaloon.SpeechBaloons.Add(newSpeechBaloon);

                                    return ThumbnailUpdateType.IMMEDIATELY;
                                }
                            }
                        }
                        else if (mouseControlTmp.Button == MouseButtons.Right)
                        {
                            if (mouseControlTmp.Object != null)
                            {
                                LayerSpeechBaloon layerSpeechBaloon = layer as LayerSpeechBaloon;
                                SpeechBaloon speechBaloon = mouseControlTmp.Object as SpeechBaloon;

                                ContextMenuStrip c = new ContextMenuStrip();
                                c.Items.Add(new ToolStripMenuItem("削除", null, Delete_Click) { Tag = new object[] { layerSpeechBaloon, speechBaloon } });
                                c.Show(Cursor.Position);

                                // ここでは変化しないのでfalse応答
                                return ThumbnailUpdateType.NONE;
                            }
                            else
                            {
                                LayerSpeechBaloon layerSpeechBaloon = layer as LayerSpeechBaloon;

                                ContextMenuStrip c = new ContextMenuStrip();
                                c.Items.Add(new ToolStripMenuItem("一括追加", null, AddSum_Click) { Tag = new object[] { layerSpeechBaloon, document.Width, document.Height } });
                                c.Show(Cursor.Position);
                            }
                        }

                        break;

                    case "LayerFill":

                        if (mouseControlTmp.Button == MouseButtons.Left)
                        {
                            return ClickLayerFill(layer as LayerFill);
                        }
                        break;

                    case "LayerImage":

                        if (mouseControlTmp.Button == MouseButtons.Left)
                        {
                            return ClickLayerImage(layer as LayerImage, document);
                        }
                        break;
                }

                return ThumbnailUpdateType.NONE;
            }

            MouseControl.Status = MouseControlInfo.ControlStatus.None;
            return ThumbnailUpdateType.NONE;
        }

        /// <summary>
        /// 塗りつぶしレイヤー選択時のメインイメージクリック
        /// </summary>
        /// <param name="layerFill"></param>
        /// <returns></returns>
        private static ThumbnailUpdateType ClickLayerFill(LayerFill layerFill)
        {
            // ColorDialog
            ColorDialog cd = new ColorDialog();

            // 初期値
            cd.Color = layerFill.BackColor;

            // よく使う色？
            // BGR
            cd.CustomColors = new int[] { 0x2E2EBF, 0xE57A3C };

            //ダイアログを表示する
            if (cd.ShowDialog() == DialogResult.OK)
            {
                //選択された色の取得
                layerFill.BackColor = cd.Color;
                return ThumbnailUpdateType.IMMEDIATELY;
            }

            return ThumbnailUpdateType.NONE;
        }

        /// <summary>
        /// イメージレイヤー選択時のメインイメージクリック
        /// </summary>
        /// <param name="layerImage"></param>
        /// <returns></returns>
        private static ThumbnailUpdateType ClickLayerImage(LayerImage layerImage, Document document)
        {
            if (MainForm.GetInstance().DataDirectory == null)
            {
                MessageBox.Show("画像ファイルを指定する際は先にワークスペースを保存してください。", "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ThumbnailUpdateType.NONE;
            }

            // OpenFileDialog
            OpenFileDialog ofd = new OpenFileDialog();

            // はじめのファイル名を指定する
            ofd.FileName = layerImage.FileName;

            // はじめに表示されるフォルダを指定する
            ofd.InitialDirectory = MainForm.GetInstance().DataDirectory;

            // フィルター
            ofd.Filter = "イメージファイル(*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|すべてのファイル(*.*)|*.*";

            // タイトル
            ofd.Title = "画像ファイルを選択してください";

            // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;

            // ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // 選択されたファイルを設定
                string relative = MainForm.GetInstance().GetRelativePathWithCopy(ofd.FileName);

                // 画像サイズを取得
                PictureControl.PictureInfo image = MainForm.GetInstance().Pictures.Load(relative);

                // 失敗
                if (image == null)
                {
                    MessageBox.Show("画像ファイルを開けませんでした。", "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return ThumbnailUpdateType.NONE;
                }

                layerImage.SetZoom(relative, document.Width, document.Height, image.Picture);

                return ThumbnailUpdateType.IMMEDIATELY;
            }

            return ThumbnailUpdateType.NONE;
        }

        static TextEditer m_TextEditor = null;
        static Point m_TextEditorLocation = Point.Empty;

        public static void CloseTextEditor()
        {
            // 表示されていれば閉じる
            if (m_TextEditor != null)
            {
                m_TextEditorLocation = m_TextEditor.Location;
                m_TextEditor.Dispose();
                m_TextEditor = null;
            }
        }

        private static void Delete_Click(object sender, EventArgs e)
        {
            // 引数の受け取り
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;

            object[] args = toolStripMenuItem.Tag as object[];

            LayerSpeechBaloon layerSpeechBaloon = args[0] as LayerSpeechBaloon;
            SpeechBaloon speechBaloon = args[1] as SpeechBaloon;

            // 対象の吹き出しを削除
            layerSpeechBaloon.SpeechBaloons.Remove(speechBaloon);

            // 描画更新
            MainForm.GetInstance().ImageUpdate(ThumbnailUpdateType.IMMEDIATELY);
        }

        /// <summary>
        /// 一括追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AddSum_Click(object sender, EventArgs e)
        {
            // 新規作成
            TextEditer editer = new TextEditer("新規作成：一括追加");
            if (editer.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // 引数の受け取り
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;

            object[] args = toolStripMenuItem.Tag as object[];

            LayerSpeechBaloon layerSpeechBaloon = args[0] as LayerSpeechBaloon;
            int width = (int)args[1];
            int height = (int)args[2];


            // 結果の受け取り
            SpeechBaloon baseSpeechBaloon = editer.CreateSpeechBaloon();

            // 分割
            string[] rs = baseSpeechBaloon.Text.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n\n", "\r").Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // 分割されたデータごとに処理
            for (int i = 0; i < rs.Length; i++)
            {
                // Cloneの生成
                SpeechBaloon speechBaloon = new SpeechBaloon(baseSpeechBaloon);
                speechBaloon.Text = rs[i].Trim();

                speechBaloon.X = width * (rs.Length - i) / (rs.Length + 1);
                speechBaloon.Y = height / 2;

                layerSpeechBaloon.SpeechBaloons.Add(speechBaloon);
            }

            // 描画更新
            MainForm.GetInstance().ImageUpdate(ThumbnailUpdateType.IMMEDIATELY);
        }

    }
}
