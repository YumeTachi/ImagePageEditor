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
            MouseControl.Tag = null;

            // マウスダウンを通知
            return layer.OnMouseDown(e, MouseControl, document, mainForm);
        }

        internal static ThumbnailUpdateType MouseMove(JLMouseEventArgs e, Layer layer, Document document, MainForm mainForm)
        {
            // 移動・クリック判定保留中
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
                    // MOVEモードに移行する
                    MouseControl.Status = MouseControlInfo.ControlStatus.Move;
                }
            }

            // 移動中
            if (MouseControl.Status == MouseControlInfo.ControlStatus.Move)
            {
                System.Diagnostics.Debug.Assert(MouseControl.Tag != null);

                // マウス移動を通知（処理中に限る）
                return layer.OnMouseMove(e, MouseControl, document, mainForm);
            }

            return ThumbnailUpdateType.NONE;
        }

        internal static ThumbnailUpdateType MouseUp(JLMouseEventArgs e, Layer layer, Document document, MainForm mainForm)
        {
            ThumbnailUpdateType retValue = ThumbnailUpdateType.NONE;

            // クリック確定
            if (MouseControl.Status == MouseControlInfo.ControlStatus.Deferment && MouseControl.Button == e.Button)
            {
                // クリック通知
                retValue = layer.OnClick(e, MouseControl, document, mainForm);
            }
            // マウスUP
            if (MouseControl.Status == MouseControlInfo.ControlStatus.Move && MouseControl.Button == e.Button)
            {
                // マウスUP通知
                retValue = layer.OnMouseUp(e, MouseControl, document, mainForm);
            }

            MouseControl.Status = MouseControlInfo.ControlStatus.None;

            return retValue;
        }
    }
}
