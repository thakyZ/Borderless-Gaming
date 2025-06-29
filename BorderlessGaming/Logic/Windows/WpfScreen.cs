using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfPoint = System.Windows.Point;
using WpfWindow = System.Windows.Window;
using System.Windows.Forms;
using System.Windows.Interop;

namespace BorderlessGaming.Logic.Windows
{
    public class WpfScreen
    {
        public static IEnumerable<WpfScreen> AllScreens()
        {
            foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                yield return new WpfScreen(screen);
            }
        }

        public static WpfScreen GetScreenFrom(WpfWindow window)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(window);
            Screen screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
            WpfScreen wpfScreen = new WpfScreen(screen);
            return wpfScreen;
        }

        public static WpfScreen GetScreenFrom(WpfPoint point)
        {
            int x = (int) Math.Round(point.X);
            int y = (int) Math.Round(point.Y);

            // are x,y device-independent-pixels ??
            System.Drawing.Point drawingPoint = new System.Drawing.Point(x, y);
            Screen screen = System.Windows.Forms.Screen.FromPoint(drawingPoint);
            WpfScreen wpfScreen = new WpfScreen(screen);

            return wpfScreen;
        }

        public static WpfScreen PrimaryScreen => new WpfScreen(System.Windows.Forms.Screen.PrimaryScreen);

        private readonly Screen screen;

        private WpfScreen(System.Windows.Forms.Screen screen)
        {
            this.screen = screen;
        }

        public Int32Rect Bounds => this.GetRect(this.screen.Bounds);

        public Int32Rect WorkingArea => this.GetRect(this.screen.WorkingArea);

        private Int32Rect GetRect(Rectangle value)
        {
            // should x, y, width, height be device-independent-pixels ??
            return new Int32Rect
            {
                X = value.X,
                Y = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }

        public bool IsPrimary => this.screen.Primary;

        public string DeviceName => this.screen.DeviceName;

        public static WpfScreen FromHandle(IntPtr handle)
        {
            return new WpfScreen(System.Windows.Forms.Screen.FromHandle(handle));
        }
    }
}
