using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

public class JLUILocker : IDisposable
{
    private static int lockCount = 0;
    private static int lockAfter = 0;

    public JLUILocker()
    {
        lockCount++;
        lockAfter = Environment.TickCount & Int32.MaxValue;
    }
    public void Dispose()
    {
        lockCount--;
        lockAfter = Environment.TickCount & Int32.MaxValue;
    }

    public static void Lock()
    {
        lockCount++;
    }
    public static void Unlock()
    {
        lockCount--;
    }

    public static bool IsLocked()
    {
        int diff = ((Environment.TickCount & Int32.MaxValue) - lockAfter);
        return (lockCount > 0) || (diff < 50);
    }
    public static bool IsLocked(Object sender)
    {
        int diff = ((Environment.TickCount & Int32.MaxValue) - lockAfter);
        return (((System.Windows.Forms.Control)sender).Focused == false) || (lockCount > 0) || (diff < 50);
    }
}

public class JLMouseEventArgs
{
    /// <summary>
    /// マウスのどのボタンが押されたかを示す値を取得します。
    /// </summary>
    public MouseButtons Button { get; set; }

    /// <summary>
    /// マウス ボタンが押されて離された回数を取得します。
    /// </summary>
    public int Clicks { get; set; }

    /// <summary>
    /// マウス ホイールの回転回数を表す符号付きの数値に定数 WHEEL_DELTA の値を乗算した値を取得します。 マウス ホイールのノッチ 1 つ分が 1 移動量に相当します。
    /// </summary>
    public int Delta { get; set; }

    /// <summary>
    /// マウス イベント生成時のマウスの位置を取得します。
    /// Documentサイズに拡縮されています。
    /// </summary>
    public Point Location { get; set; }

    /// <summary>
    /// マウス イベント生成時のマウスの位置を取得します。
    /// 実際の画面上のサイズです。
    /// </summary>
    public Point LocationRaw { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="arg"></param>
    /// <param name="rate"></param>
    public JLMouseEventArgs(MouseEventArgs arg, float rate)
    {
        System.Diagnostics.Debug.Assert(arg.X == arg.Location.X && arg.Y == arg.Location.Y);

        Button = arg.Button;
        Clicks = arg.Clicks;
        Delta = arg.Delta;
        Location = new Point((int)Math.Round(arg.X / rate), (int)Math.Round(arg.Y / rate));
        LocationRaw = arg.Location;
    }
}

public class EventWrapper
{
    EventHandler eventHandler = null;

    public EventWrapper(EventHandler eventHandler)
    {
        this.eventHandler = eventHandler;
    }

    public void EventHandler(object sender, EventArgs e)
    {
        if (!JLUILocker.IsLocked(sender))
        {
            if (eventHandler != null)
                eventHandler(sender, e);
        }
    }
}
