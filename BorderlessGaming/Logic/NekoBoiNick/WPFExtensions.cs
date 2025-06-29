using System;
using System.Windows;
using System.Windows.Input;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Size = System.Windows.Size;

namespace BorderlessGaming.Logic.NekoBoiNick
{
    public static class WPFExtensions
    {
        public static Int32Rect GetClip(this Cursor cursor)
        {
            PInvoke.GetClipCursor(out RECT rect);
            return new Int32Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }
        public static void SetClip(this Cursor cursor, Int32Rect rect)
        {
            PInvoke.ClipCursor(new RECT(rect.X, rect.Y, rect.Width, rect.Height));
        }
        internal static void SetClip(this Cursor cursor, RECT rect)
        {
            PInvoke.ClipCursor(rect);
        }
    }
}