using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BorderlessGaming.Logic.NekoBoiNick
{
    internal static class BitmapExtensions
    {
        public static BitmapSource GetBitmapSource(this Bitmap bitmap)
          => Imaging.CreateBitmapSourceFromHBitmap(
              bitmap.GetHbitmap(),
              IntPtr.Zero,
              Int32Rect.Empty,
              BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));
    }
}
