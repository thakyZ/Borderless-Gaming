using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;

using BorderlessGaming.Logic.Misc.Utilities;
using BorderlessGaming.Logic.Models;
using BorderlessGaming.Logic.NekoBoiNick;
using BorderlessGaming.Logic.Steam;
using BorderlessGaming.Properties;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace BorderlessGaming.Logic.Windows
{
    public static class Manipulation
    {
        // Cursor swap data
        private static Cursor curInvisibleCursorForms;

        private static HCURSOR hCursorOriginal = HCURSOR.Null;

        // List of original screens prior to Windows taskbar hidden
        private static readonly List<OriginalScreenInfo> OriginalScreens = new List<OriginalScreenInfo>();

        // Windows taskbar hidden data
        public static bool WindowsTaskbarIsHidden;

        // Mouse cursor hidden data
        public static bool MouseCursorIsHidden;

        /// <summary>
        ///     remove the menu, resize the window, remove border, and maximize
        /// </summary>
        public static async Task MakeWindowBorderless(ProcessDetails processDetails, Form frmMain, IntPtr targetWindow,
            Rectangle targetFrame, Favorite favDetails)
        {
            var _targetWindow = new HWND(targetWindow);
            if (NeedsDelay(_targetWindow))
            {
                await MakeWindowBorderlessDelayed(processDetails, frmMain, targetWindow, targetFrame, favDetails);
            }
            else
            {
                // Automatically match a window to favorite details, if that information is available.
                // Note: if one is not available, the default settings will be used as a new Favorite() object.

                // Automatically match this window to a process

                // Failsafe to prevent rapid switching, but also allow a few changes to the window handle (to be persistent)
                if (processDetails != null)
                {
                    if (processDetails.MadeBorderless)
                    {
                        if (processDetails.MadeBorderlessAttempts > 3 || ! await processDetails.WindowHasTargetableStyles())
                        {
                            return;
                        }
                    }
                }

                // If no target frame was specified, assume the entire space on the primary screen
                if (targetFrame.Width == 0 || targetFrame.Height == 0)
                {
                    targetFrame = Screen.FromHandle(_targetWindow).Bounds;
                }

                // Get window styles
                var styleCurrentWindowStandard = Native.GetWindowLong32(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
                var styleCurrentWindowExtended = Native.GetWindowLong64(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

                // Compute new styles (XOR of the inverse of all the bits to filter)
                var styleNewWindowStandard =
                    styleCurrentWindowStandard
                    & ~(
                        WINDOW_STYLE.WS_CAPTION // composite of Border and DialogFrame
                        //   | WINDOW_STYLE.WS_BORDER
                        //   | WINDOW_STYLE.WS_DLGFRAME                  
                        | WINDOW_STYLE.WS_THICKFRAME
                        | WINDOW_STYLE.WS_SYSMENU
                        | WINDOW_STYLE.WS_MAXIMIZEBOX // same as TabStop
                        | WINDOW_STYLE.WS_MINIMIZEBOX // same as Group
                    );

                var styleNewWindowExtended =
                    styleCurrentWindowExtended
                    & ~(
                        WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME
                        | WINDOW_EX_STYLE.WS_EX_COMPOSITED
                        | WINDOW_EX_STYLE.WS_EX_WINDOWEDGE
                        | WINDOW_EX_STYLE.WS_EX_CLIENTEDGE
                        | WINDOW_EX_STYLE.WS_EX_LAYERED
                        | WINDOW_EX_STYLE.WS_EX_STATICEDGE
                        | WINDOW_EX_STYLE.WS_EX_TOOLWINDOW
                        | WINDOW_EX_STYLE.WS_EX_APPWINDOW
                    );

                // Should have process details by now
                if (processDetails != null)
                {
                    // Save original details on this window so that we have a chance at undoing the process
                    processDetails.OriginalStyleFlagsStandard = styleCurrentWindowStandard;
                    processDetails.OriginalStyleFlagsExtended = styleCurrentWindowExtended;
                    PInvoke.GetWindowRect(processDetails.WindowHandle, out RECT rectTemp);
                    processDetails.OriginalLocation = new Rectangle(rectTemp.left, rectTemp.top,
                        rectTemp.right - rectTemp.left, rectTemp.bottom - rectTemp.top);
                }

                // remove the menu and menuitems and force a redraw
                if (favDetails.RemoveMenus)
                {
                    // unfortunately, menus can't be re-added easily so they aren't removed by default anymore
                    var menuHandle = PInvoke.GetMenu(_targetWindow);
                    if (menuHandle != HMENU.Null)
                    {
                        var menuItemCount = PInvoke.GetMenuItemCount(menuHandle);

                        for (var i = 0; i < menuItemCount; i++)
                        {
                            PInvoke.RemoveMenu(menuHandle, 0, MENU_ITEM_FLAGS.MF_BYPOSITION | MENU_ITEM_FLAGS.MF_REMOVE);
                        }

                        PInvoke.DrawMenuBar(_targetWindow);
                    }
                }

                // auto-hide the Windows taskbar (do this before resizing the window)
                if (favDetails.HideWindowsTaskbar)
                {
                    PInvoke.ShowWindow(new HWND(frmMain.Handle), SHOW_WINDOW_CMD.SW_SHOWNOACTIVATE);
                    if (frmMain.WindowState == FormWindowState.Minimized)
                    {
                        frmMain.WindowState = FormWindowState.Normal;
                    }

                    ToggleWindowsTaskbarVisibility(Boolstate.False);
                }

                // auto-hide the mouse cursor
                if (favDetails.HideMouseCursor)
                {
                    ToggleMouseCursorVisibility(frmMain, Boolstate.False);
                }

                // update window styles
                Native.SetWindowLong32(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_STYLE, styleNewWindowStandard);
                Native.SetWindowLong64(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, styleNewWindowExtended);

                // update window position
                if (favDetails.Size != FavoriteSize.NoChange)
                {
                    if (favDetails.Size == FavoriteSize.FullScreen || favDetails.PositionWidth == 0 ||
                        favDetails.PositionHeight == 0)
                    {
                        // Set the window size to the biggest possible, using bounding adjustments
                        PInvoke.SetWindowPos
                        (
                            _targetWindow,
                            HWND.Null,
                            targetFrame.X + favDetails.OffsetLeft,
                            targetFrame.Y + favDetails.OffsetTop,
                            targetFrame.Width - favDetails.OffsetLeft + favDetails.OffsetRight,
                            targetFrame.Height - favDetails.OffsetTop + favDetails.OffsetBottom,
                            SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER |
                            SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                        );

                        // And auto-maximize
                        if (favDetails.ShouldMaximize)
                        {
                            PInvoke.ShowWindow(_targetWindow, SHOW_WINDOW_CMD.SW_MAXIMIZE);
                        }
                    }
                    else
                    {
                        // Set the window size to the exact position specified by the user
                        PInvoke.SetWindowPos
                        (
                            _targetWindow,
                            HWND.Null,
                            favDetails.PositionX,
                            favDetails.PositionY,
                            favDetails.PositionWidth,
                            favDetails.PositionHeight,
                            SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER |
                            SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                        );
                    }
                }

                // Set topmost
                if (favDetails.TopMost)
                {
                    PInvoke.SetWindowPos
                    (
                        _targetWindow,
                        Native.HWND_TOPMOST,
                        0,
                        0,
                        0,
                        0,
                        SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE |
                        SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                    );
                }
            }

            // Make a note that we attempted to make the window borderless
            if (processDetails != null)
            {
                processDetails.MadeBorderless = true;
                processDetails.MadeBorderlessAttempts++;
            }
            if (SteamApi.IsLoaded)
            {
                if (SteamApi.UnlockAchievement("FIRST_TIME_BORDERLESS"))
                {
                    Console.WriteLine("Great!");
                }
            }
        }

        /// <summary>
        ///     remove the menu, resize the window, remove border, and maximize
        /// </summary>
        public static async Task MakeWindowBorderless(ProcessDetails processDetails, System.Windows.Window frmMain, IntPtr targetWindow,
            System.Windows.Int32Rect targetFrame, Favorite favDetails)
        {
            var _targetWindow = new HWND(targetWindow);
            var _windowHandle = new WindowInteropHelper(frmMain).Handle;
            if (NeedsDelay(_targetWindow))
            {
                await MakeWindowBorderlessDelayed(processDetails, frmMain, targetWindow, targetFrame, favDetails);
            }
            else
            {
                // Automatically match a window to favorite details, if that information is available.
                // Note: if one is not available, the default settings will be used as a new Favorite() object.

                // Automatically match this window to a process

                // Failsafe to prevent rapid switching, but also allow a few changes to the window handle (to be persistent)
                if (processDetails != null)
                {
                    if (processDetails.MadeBorderless)
                    {
                        if (processDetails.MadeBorderlessAttempts > 3 || ! await processDetails.WindowHasTargetableStyles())
                        {
                            return;
                        }
                    }
                }

                // If no target frame was specified, assume the entire space on the primary screen
                if (targetFrame.Width == 0 || targetFrame.Height == 0)
                {
                    targetFrame = WpfScreen.FromHandle(targetWindow).Bounds;
                }

                // Get window styles
                var styleCurrentWindowStandard = Native.GetWindowLong32(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
                var styleCurrentWindowExtended = Native.GetWindowLong64(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

                // Compute new styles (XOR of the inverse of all the bits to filter)
                var styleNewWindowStandard =
                    styleCurrentWindowStandard
                    & ~(
                        WINDOW_STYLE.WS_CAPTION // composite of Border and DialogFrame
                        //   | WINDOW_STYLE.WS_BORDER
                        //   | WINDOW_STYLE.WS_DLGFRAME                  
                        | WINDOW_STYLE.WS_THICKFRAME
                        | WINDOW_STYLE.WS_SYSMENU
                        | WINDOW_STYLE.WS_MAXIMIZEBOX // same as TabStop
                        | WINDOW_STYLE.WS_MINIMIZEBOX // same as Group
                    );

                var styleNewWindowExtended =
                    styleCurrentWindowExtended
                    & ~(
                        WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME
                        | WINDOW_EX_STYLE.WS_EX_COMPOSITED
                        | WINDOW_EX_STYLE.WS_EX_WINDOWEDGE
                        | WINDOW_EX_STYLE.WS_EX_CLIENTEDGE
                        | WINDOW_EX_STYLE.WS_EX_LAYERED
                        | WINDOW_EX_STYLE.WS_EX_STATICEDGE
                        | WINDOW_EX_STYLE.WS_EX_TOOLWINDOW
                        | WINDOW_EX_STYLE.WS_EX_APPWINDOW
                    );

                // Should have process details by now
                if (processDetails != null)
                {
                    // Save original details on this window so that we have a chance at undoing the process
                    processDetails.OriginalStyleFlagsStandard = styleCurrentWindowStandard;
                    processDetails.OriginalStyleFlagsExtended = styleCurrentWindowExtended;
                    PInvoke.GetWindowRect(processDetails.WindowHandle, out RECT rectTemp);
                    processDetails.OriginalLocation = new Rectangle(rectTemp.left, rectTemp.top,
                        rectTemp.right - rectTemp.left, rectTemp.bottom - rectTemp.top);
                }

                // remove the menu and menuitems and force a redraw
                if (favDetails.RemoveMenus)
                {
                    // unfortunately, menus can't be re-added easily so they aren't removed by default anymore
                    var menuHandle = PInvoke.GetMenu(new HWND(targetWindow));
                    if (menuHandle != HMENU.Null)
                    {
                        var menuItemCount = PInvoke.GetMenuItemCount(menuHandle);

                        for (var i = 0; i < menuItemCount; i++)
                        {
                            PInvoke.RemoveMenu(menuHandle, 0, MENU_ITEM_FLAGS.MF_BYPOSITION | MENU_ITEM_FLAGS.MF_REMOVE);
                        }

                        PInvoke.DrawMenuBar(new HWND(targetWindow));
                    }
                }

                // auto-hide the Windows taskbar (do this before resizing the window)
                if (favDetails.HideWindowsTaskbar)
                {
                    PInvoke.ShowWindow(new HWND(_windowHandle), SHOW_WINDOW_CMD.SW_SHOWNOACTIVATE);
                    if (frmMain.WindowState is System.Windows.WindowState.Minimized)
                    {
                        frmMain.WindowState = System.Windows.WindowState.Normal;
                    }

                    ToggleWindowsTaskbarVisibility(Boolstate.False);
                }

                // auto-hide the mouse cursor
                if (favDetails.HideMouseCursor)
                {
                    ToggleMouseCursorVisibility(frmMain, Boolstate.False);
                }

                // update window styles
                Native.SetWindowLong32(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_STYLE, styleNewWindowStandard);
                Native.SetWindowLong64(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, styleNewWindowExtended);

                // update window position
                if (favDetails.Size != FavoriteSize.NoChange)
                {
                    if (favDetails.Size == FavoriteSize.FullScreen || favDetails.PositionWidth == 0 ||
                        favDetails.PositionHeight == 0)
                    {
                        // Set the window size to the biggest possible, using bounding adjustments
                        PInvoke.SetWindowPos
                        (
                            _targetWindow,
                            HWND.Null,
                            targetFrame.X + favDetails.OffsetLeft,
                            targetFrame.Y + favDetails.OffsetTop,
                            targetFrame.Width - favDetails.OffsetLeft + favDetails.OffsetRight,
                            targetFrame.Height - favDetails.OffsetTop + favDetails.OffsetBottom,
                            SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER |
                            SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                        );

                        // And auto-maximize
                        if (favDetails.ShouldMaximize)
                        {
                            PInvoke.ShowWindow(_targetWindow, SHOW_WINDOW_CMD.SW_MAXIMIZE);
                        }
                    }
                    else
                    {
                        // Set the window size to the exact position specified by the user
                        PInvoke.SetWindowPos
                        (
                            _targetWindow,
                            HWND.Null,
                            favDetails.PositionX,
                            favDetails.PositionY,
                            favDetails.PositionWidth,
                            favDetails.PositionHeight,
                            SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER |
                            SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                        );
                    }
                }

                // Set topmost
                if (favDetails.TopMost)
                {
                    PInvoke.SetWindowPos
                    (
                        _targetWindow,
                        Native.HWND_TOPMOST,
                        0,
                        0,
                        0,
                        0,
                        SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE |
                        SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                    );
                }
            }

            // Make a note that we attempted to make the window borderless
            if (processDetails != null)
            {
                processDetails.MadeBorderless = true;
                processDetails.MadeBorderlessAttempts++;
            }
            if (SteamApi.IsLoaded)
            {
                if (SteamApi.UnlockAchievement("FIRST_TIME_BORDERLESS"))
                {
                    Console.WriteLine("Great!");
                }
            }
        }

        private static async Task MakeWindowBorderlessDelayed(ProcessDetails processDetails, Form frmMain,
            IntPtr targetWindow, Rectangle targetFrame, Favorite favDetails)
        {
            var _targetWindow = new HWND(targetWindow);
            // Automatically match a window to favorite details, if that information is available.
            // Note: if one is not available, the default settings will be used as a new Favorite() object.

            // Automatically match this window to a process

            // Failsafe to prevent rapid switching, but also allow a few changes to the window handle (to be persistent)
            if (processDetails != null)
            {
                if (processDetails.MadeBorderless)
                {
                    if (processDetails.MadeBorderlessAttempts > 3 || ! await processDetails.WindowHasTargetableStyles())
                    {
                        return;
                    }
                }
            }

            // If no target frame was specified, assume the entire space on the primary screen
            if (targetFrame.Width == 0 || targetFrame.Height == 0)
            {
                targetFrame = Screen.FromHandle(targetWindow).Bounds;
            }

            // Get window styles
            var styleCurrentWindowStandard = Native.GetWindowLong32(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            var styleCurrentWindowExtended = Native.GetWindowLong64(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

            // Compute new styles (XOR of the inverse of all the bits to filter)
            var styleNewWindowStandard =
                styleCurrentWindowStandard
                & ~(
                    WINDOW_STYLE.WS_CAPTION // composite of Border and DialogFrame
                    // | WINDOW_STYLE.WS_BORDER
                    //| WINDOW_STYLE.WS_DLGFRAME                  
                    | WINDOW_STYLE.WS_THICKFRAME
                    | WINDOW_STYLE.WS_OVERLAPPEDWINDOW
                    | WINDOW_STYLE.WS_SYSMENU
                    | WINDOW_STYLE.WS_MAXIMIZEBOX // same as TabStop
                    | WINDOW_STYLE.WS_MINIMIZEBOX // same as Group
                );

            var styleNewWindowExtended =
                styleCurrentWindowExtended
                & ~(
                    WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME
                    | WINDOW_EX_STYLE.WS_EX_COMPOSITED
                    | WINDOW_EX_STYLE.WS_EX_WINDOWEDGE
                    | WINDOW_EX_STYLE.WS_EX_CLIENTEDGE
                    | WINDOW_EX_STYLE.WS_EX_LAYERED
                    | WINDOW_EX_STYLE.WS_EX_STATICEDGE
                    | WINDOW_EX_STYLE.WS_EX_TOOLWINDOW
                    | WINDOW_EX_STYLE.WS_EX_APPWINDOW
                );

            // Should have process details by now
            if (processDetails != null)
            {
                // Save original details on this window so that we have a chance at undoing the process
                processDetails.OriginalStyleFlagsStandard = styleCurrentWindowStandard;
                processDetails.OriginalStyleFlagsExtended = styleCurrentWindowExtended;
                PInvoke.GetWindowRect(processDetails.WindowHandle, out RECT rect_temp);
                processDetails.OriginalLocation = new Rectangle(rect_temp.left, rect_temp.top,
                    rect_temp.right - rect_temp.left, rect_temp.bottom - rect_temp.top);
            }

            // remove the menu and menuitems and force a redraw

            // unfortunately, menus can't be re-added easily so they aren't removed by default anymore
            var menuHandle = PInvoke.GetMenu(_targetWindow);
            if (menuHandle != HMENU.Null)
            {
                var menuItemCount = PInvoke.GetMenuItemCount(menuHandle);

                for (var i = 0; i < menuItemCount; i++)
                {
                    PInvoke.RemoveMenu(menuHandle, 0, MENU_ITEM_FLAGS.MF_BYPOSITION | MENU_ITEM_FLAGS.MF_REMOVE);
                }

                PInvoke.DrawMenuBar(_targetWindow);
            }


            // auto-hide the Windows taskbar (do this before resizing the window)
            if (favDetails.HideWindowsTaskbar)
            {
                PInvoke.ShowWindow(new HWND(frmMain.Handle), SHOW_WINDOW_CMD.SW_SHOWMINNOACTIVE);
                if (frmMain.WindowState == FormWindowState.Minimized)
                {
                    frmMain.WindowState = FormWindowState.Normal;
                }

                ToggleWindowsTaskbarVisibility(Boolstate.False);
            }

            // auto-hide the mouse cursor
            if (favDetails.HideMouseCursor)
            {
                ToggleMouseCursorVisibility(frmMain, Boolstate.False);
            }


            // update window position
            if (favDetails.Size != FavoriteSize.NoChange)
            {
                if (favDetails.Size == FavoriteSize.FullScreen || favDetails.PositionWidth == 0 ||
                    favDetails.PositionHeight == 0)
                {
                    // Set the window size to the biggest possible, using bounding adjustments
                    PInvoke.SetWindowPos
                    (
                        _targetWindow,
                        HWND.Null,
                        targetFrame.X + favDetails.OffsetLeft,
                        targetFrame.Y + favDetails.OffsetTop,
                        targetFrame.Width - favDetails.OffsetLeft + favDetails.OffsetRight,
                        targetFrame.Height - favDetails.OffsetTop + favDetails.OffsetBottom,
                        SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW |
                        SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER | SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                    );
                }
                else
                {
                    // Set the window size to the exact position specified by the user
                    PInvoke.SetWindowPos
                    (
                        _targetWindow,
                        HWND.Null,
                        favDetails.PositionX,
                        favDetails.PositionY,
                        favDetails.PositionWidth,
                        favDetails.PositionHeight,
                        SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW |
                        SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER | SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                    );
                }
            }

            // Set topmost
            if (favDetails.TopMost)
            {
                PInvoke.SetWindowPos
                (
                    _targetWindow,
                    Native.HWND_TOPMOST,
                    0,
                    0,
                    0,
                    0,
                    SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOMOVE |
                    SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                );
            }
            //wait before applying styles
            await TaskUtilities.WaitAndStartTaskAsync(() =>
            {
                Native.SetWindowLong32(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_STYLE, styleNewWindowStandard);
                Native.SetWindowLong64(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, styleNewWindowExtended);
            }, 4);
        }

        private static async Task MakeWindowBorderlessDelayed(ProcessDetails processDetails, System.Windows.Window frmMain,
            IntPtr targetWindow, System.Windows.Int32Rect targetFrame, Favorite favDetails)
        {
            var _targetWindow = new HWND(targetWindow);
            var _windowHandle = new WindowInteropHelper(frmMain).Handle;
            // Automatically match a window to favorite details, if that information is available.
            // Note: if one is not available, the default settings will be used as a new Favorite() object.

            // Automatically match this window to a process

            // Failsafe to prevent rapid switching, but also allow a few changes to the window handle (to be persistent)
            if (processDetails != null)
            {
                if (processDetails.MadeBorderless)
                {
                    if (processDetails.MadeBorderlessAttempts > 3 || ! await processDetails.WindowHasTargetableStyles())
                    {
                        return;
                    }
                }
            }

            // If no target frame was specified, assume the entire space on the primary screen
            if (targetFrame.Width == 0 || targetFrame.Height == 0)
            {
                targetFrame = WpfScreen.FromHandle(_targetWindow).Bounds;
            }

            // Get window styles
            var styleCurrentWindowStandard = Native.GetWindowLong32(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            var styleCurrentWindowExtended = Native.GetWindowLong64(_targetWindow, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

            // Compute new styles (XOR of the inverse of all the bits to filter)
            var styleNewWindowStandard =
                styleCurrentWindowStandard
                & ~(
                    WINDOW_STYLE.WS_CAPTION // composite of Border and DialogFrame
                    // | WINDOW_STYLE.WS_BORDER
                    //| WINDOW_STYLE.WS_DLGFRAME                  
                    | WINDOW_STYLE.WS_THICKFRAME
                    | WINDOW_STYLE.WS_OVERLAPPEDWINDOW
                    | WINDOW_STYLE.WS_SYSMENU
                    | WINDOW_STYLE.WS_MAXIMIZEBOX // same as TabStop
                    | WINDOW_STYLE.WS_MINIMIZEBOX // same as Group
                );

            var styleNewWindowExtended =
                styleCurrentWindowExtended
                & ~(
                    WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME
                    | WINDOW_EX_STYLE.WS_EX_COMPOSITED
                    | WINDOW_EX_STYLE.WS_EX_WINDOWEDGE
                    | WINDOW_EX_STYLE.WS_EX_CLIENTEDGE
                    | WINDOW_EX_STYLE.WS_EX_LAYERED
                    | WINDOW_EX_STYLE.WS_EX_STATICEDGE
                    | WINDOW_EX_STYLE.WS_EX_TOOLWINDOW
                    | WINDOW_EX_STYLE.WS_EX_APPWINDOW
                );

            // Should have process details by now
            if (processDetails != null)
            {
                // Save original details on this window so that we have a chance at undoing the process
                processDetails.OriginalStyleFlagsStandard = styleCurrentWindowStandard;
                processDetails.OriginalStyleFlagsExtended = styleCurrentWindowExtended;
                PInvoke.GetWindowRect(processDetails.WindowHandle, out RECT rect_temp);
                processDetails.OriginalLocation = new Rectangle(rect_temp.left, rect_temp.top,
                    rect_temp.right - rect_temp.left, rect_temp.bottom - rect_temp.top);
            }

            // remove the menu and menuitems and force a redraw

            // unfortunately, menus can't be re-added easily so they aren't removed by default anymore
            var menuHandle = PInvoke.GetMenu(_targetWindow);
            if (menuHandle != HWND.Null)
            {
                var menuItemCount = PInvoke.GetMenuItemCount(menuHandle);

                for (var i = 0; i < menuItemCount; i++)
                {
                    PInvoke.RemoveMenu(menuHandle, 0, MENU_ITEM_FLAGS.MF_BYPOSITION | MENU_ITEM_FLAGS.MF_REMOVE);
                }

                PInvoke.DrawMenuBar(_targetWindow);
            }


            // auto-hide the Windows taskbar (do this before resizing the window)
            if (favDetails.HideWindowsTaskbar)
            {
                PInvoke.ShowWindow(new HWND(_windowHandle), SHOW_WINDOW_CMD.SW_SHOWNOACTIVATE);
                if (frmMain.WindowState == System.Windows.WindowState.Minimized)
                {
                    frmMain.WindowState = System.Windows.WindowState.Normal;
                }

                ToggleWindowsTaskbarVisibility(Boolstate.False);
            }

            // auto-hide the mouse cursor
            if (favDetails.HideMouseCursor)
            {
                ToggleMouseCursorVisibility(frmMain, Boolstate.False);
            }


            // update window position
            if (favDetails.Size != FavoriteSize.NoChange)
            {
                if (favDetails.Size == FavoriteSize.FullScreen || favDetails.PositionWidth == 0 ||
                    favDetails.PositionHeight == 0)
                {
                    // Set the window size to the biggest possible, using bounding adjustments
                    PInvoke.SetWindowPos
                    (
                        _targetWindow,
                        HWND.Null,
                        targetFrame.X + favDetails.OffsetLeft,
                        targetFrame.Y + favDetails.OffsetTop,
                        targetFrame.Width - favDetails.OffsetLeft + favDetails.OffsetRight,
                        targetFrame.Height - favDetails.OffsetTop + favDetails.OffsetBottom,
                        SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW |
                        SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER | SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                    );
                }
                else
                {
                    // Set the window size to the exact position specified by the user
                    PInvoke.SetWindowPos
                    (
                        _targetWindow,
                        HWND.Null,
                        favDetails.PositionX,
                        favDetails.PositionY,
                        favDetails.PositionWidth,
                        favDetails.PositionHeight,
                        SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW |
                        SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER | SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                    );
                }
            }

            // Set topmost
            if (favDetails.TopMost)
            {
                PInvoke.SetWindowPos
                (
                    _targetWindow,
                    Native.HWND_TOPMOST,
                    0,
                    0,
                    0,
                    0,
                    SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOMOVE |
                    SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                );
            }
            //wait before applying styles
            await TaskUtilities.WaitAndStartTaskAsync(() =>
            {
                Native.SetWindowLong32(new HWND(targetWindow), WINDOW_LONG_PTR_INDEX.GWL_STYLE, styleNewWindowStandard);
                Native.SetWindowLong64(new HWND(targetWindow), WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, styleNewWindowExtended);
            }, 4);
        }


        /// <summary>
        ///     Check if a window class contains Unreal because it differs per game.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private static bool IsUnreal(IntPtr handle)
        {
            return IsUnreal(new HWND(handle));
        }


        /// <summary>
        ///     Check if a window class contains Unreal because it differs per game.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private static bool IsUnreal(HWND hWnd)
        {
            return Native.GetWindowClassName(hWnd).ToLower().Contains("unreal");
        }

        private static bool NeedsDelay(IntPtr handle)
        {
            return NeedsDelay(new HWND(handle));
        }

        private static bool NeedsDelay(HWND hWnd)
        {
            //other game engines
            var classNames = new List<string> {"YYGameMakerYY"};
            var className = Native.GetWindowClassName(hWnd);
            return IsUnreal(hWnd) || classNames.Any(name => name.Equals(className));
        }

        public static void RestoreWindow(ProcessDetails pd)
        {
            if (pd == null || !pd.MadeBorderless || pd.OriginalStyleFlagsStandard == 0)
            {
                return;
            }

            Native.SetWindowLong32(pd.WindowHandle, WINDOW_LONG_PTR_INDEX.GWL_STYLE, pd.OriginalStyleFlagsStandard);
            Native.SetWindowLong64(pd.WindowHandle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, pd.OriginalStyleFlagsExtended);
            PInvoke.SetWindowPos(pd.WindowHandle, HWND.Null, pd.OriginalLocation.X, pd.OriginalLocation.Y,
                pd.OriginalLocation.Width, pd.OriginalLocation.Height,
                SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
            PInvoke.SetWindowPos(pd.WindowHandle, Native.HWND_NOTTOPMOST, 0, 0, 0, 0,
                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE);
            pd.MadeBorderless = false;
            pd.MadeBorderlessAttempts = 0;
        }

        public static void ToggleWindowsTaskbarVisibility(Boolstate forced = Boolstate.Indeterminate)
        {
            try
            {
                var hTaskBar = PInvoke.FindWindow("Shell_TrayWnd", null);

                if (hTaskBar == Native.INVALID_HANDLE_VALUE || hTaskBar == IntPtr.Zero)
                {
                    return;
                }

                var TaskBarIsCurrentlyVisible = PInvoke.IsWindowVisible(hTaskBar);
                var wantToMakeWindowsTaskbarVisible = forced == Boolstate.True
                    ? true
                    : forced ==Boolstate.False
                        ? false
                        : !TaskBarIsCurrentlyVisible;

                // For forced modes, if the taskbar is already visible and we're requesting to show it, then do nothing
                if (wantToMakeWindowsTaskbarVisible && TaskBarIsCurrentlyVisible)
                {
                    return;
                }

                // For forced modes, if the taskbar is already hidden and we're requesting to hide it, then do nothing
                if (!wantToMakeWindowsTaskbarVisible && !TaskBarIsCurrentlyVisible)
                {
                    return;
                }

                // If we're hiding the taskbar, let's take some notes on the original screen desktop work areas
                if (!wantToMakeWindowsTaskbarVisible)
                {
                    foreach (var screen in Screen.AllScreens)
                    {
                        var osi = new OriginalScreenInfo();
                        osi.Screen = screen;
                        osi.Workarea = (RECT)screen.WorkingArea;
                        OriginalScreens.Add(osi);
                    }
                }

                // Show or hide the Windows taskbar
                PInvoke.ShowWindow(hTaskBar,
                    wantToMakeWindowsTaskbarVisible ? SHOW_WINDOW_CMD.SW_SHOWNOACTIVATE : SHOW_WINDOW_CMD.SW_HIDE);

                // Keep track of the taskbar state so we don't let the user accidentally close Borderless Gaming
                WindowsTaskbarIsHidden = !wantToMakeWindowsTaskbarVisible;

                if (wantToMakeWindowsTaskbarVisible)
                {
                    // If we're showing the taskbar, let's restore the original screen desktop work areas...
                    foreach (var osi in OriginalScreens)
                    {
                        try
                        {
                            unsafe
                            {
                                RECT workArea = new RECT();
                                PInvoke.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_SETWORKAREA, 0, &workArea, SYSTEM_PARAMETERS_INFO_UPDATE_FLAGS.SPIF_SENDCHANGE);
                                osi.Workarea = workArea;
                            }
                        }
                        catch (Exception exception)
                        {
                            // TODO: Log here...
                        }
                    }

                    // ...and then forget them (we don't need them anymore)
                    OriginalScreens.Clear();

                    // And we need to redraw the system tray in case tray icons from other applications did something while the
                    // taskbar was hidden.  Simulating mouse movement over the system tray seems to be the best way to get this
                    // done.
                    RedrawWindowsSystemTrayArea();
                }
                else
                {
                    // If we're hiding the taskbar, let's set the screen desktop work area over the entire screen so that 
                    // maximizing windows works as expected.
                    foreach (var osi in OriginalScreens)
                    {
                        RECT rect = osi.Screen.Bounds;
                        try
                        {
                            unsafe
                            {
                                RECT _rect = default(RECT);
                                PInvoke.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_SETWORKAREA, 0, &_rect, SYSTEM_PARAMETERS_INFO_UPDATE_FLAGS.SPIF_SENDCHANGE);
                                rect = _rect;
                            }
                        }
                        catch (Exception exception)
                        {
                            // TODO: Log here...
                        }

                        // Note: WinAPI SystemParametersInfo() will automatically determine which screen by the rectangle we pass in.
                        //       (it's not possible to specify which screen we're referring to directly)
                    }
                }
            }
            catch
            {
            }
        }

        public static void ToggleMouseCursorVisibility(Form frmMain,
            Boolstate forced = Boolstate.Indeterminate)
        {
            if (forced == Boolstate.True && !MouseCursorIsHidden ||
                forced == Boolstate.False && MouseCursorIsHidden)
            {
                return;
            }

            if (forced == Boolstate.True || MouseCursorIsHidden)
            {
                PInvoke.SetSystemCursor(hCursorOriginal, SYSTEM_CURSOR_ID.OCR_NORMAL);
                PInvoke.DestroyIcon(hCursorOriginal);
                hCursorOriginal = HCURSOR.Null;

                MouseCursorIsHidden = false;
            }
            else
            {
                string fileName = null;

                try
                {
                    hCursorOriginal = new HCURSOR(frmMain.Cursor.CopyHandle());

                    if (curInvisibleCursorForms == null)
                    {
                        // Can't load from a memory stream because the constructor new Cursor() does not accept animated or non-monochrome cursors
                        fileName = Path.GetTempPath() + Guid.NewGuid() + ".cur";

                        using (var fileStream = File.Open(fileName, FileMode.Create))
                        {
                            using (var ms = new MemoryStream(Resources.blank))
                            {
                                ms.WriteTo(fileStream);
                            }

                            fileStream.Flush();
                            fileStream.Close();
                        }

                        var file = PInvoke.LoadCursorFromFile(fileName);
                        if (!file.IsInvalid && !file.IsClosed) {
                            curInvisibleCursorForms = new Cursor(file.DangerousGetHandle());
                        }
                    }

                    PInvoke.SetSystemCursor(new HCURSOR(curInvisibleCursorForms.CopyHandle()), SYSTEM_CURSOR_ID.OCR_NORMAL);

                    MouseCursorIsHidden = true;
                }
                catch
                {
                    // swallow exception and assume cursor set failed
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            if (File.Exists(fileName))
                            {
                                File.Delete(fileName);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

#nullable enable
        public static void ToggleMouseCursorVisibility(System.Windows.Window windowMain, Boolstate forced = Boolstate.Indeterminate)
        {
            if (forced == Boolstate.True && !MouseCursorIsHidden ||
                forced == Boolstate.False && MouseCursorIsHidden)
            {
                return;
            }

            if (forced == Boolstate.True || MouseCursorIsHidden)
            {
                PInvoke.SetSystemCursor(hCursorOriginal, SYSTEM_CURSOR_ID.OCR_NORMAL);
                PInvoke.DestroyIcon(hCursorOriginal);
                hCursorOriginal = default(HCURSOR);

                MouseCursorIsHidden = false;
            }
            else
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.None;
            }
        }

        /// <summary>
        ///     remove the menu, resize the window, remove border, and maximize
        /// </summary>
        internal static async Task MakeWindowBorderless(ProcessDetails processDetails, System.Windows.Window windowMain, HWND targetWindow,
            Rectangle targetFrame, Favorite favDetails)
        {
            var _windowHandle = new WindowInteropHelper(windowMain).Handle;
            if (NeedsDelay(targetWindow))
            {
                await MakeWindowBorderlessDelayed(processDetails, windowMain, targetWindow, targetFrame, favDetails);
            }
            else
            {
                // Automatically match a window to favorite details, if that information is available.
                // Note: if one is not available, the default settings will be used as a new Favorite() object.

                // Automatically match this window to a process

                // Failsafe to prevent rapid switching, but also allow a few changes to the window handle (to be persistent)
                if (processDetails != null)
                {
                    if (processDetails.MadeBorderless)
                    {
                        if (processDetails.MadeBorderlessAttempts > 3 || ! await processDetails.WindowHasTargetableStyles())
                        {
                            return;
                        }
                    }
                }

                // If no target frame was specified, assume the entire space on the primary screen
                if (targetFrame.Width == 0 || targetFrame.Height == 0)
                {
                    targetFrame = Screen.FromHandle(targetWindow).Bounds;
                }

                // Get window styles
            var styleCurrentWindowStandard = Native.GetWindowLong32(targetWindow, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            var styleCurrentWindowExtended = Native.GetWindowLong64(targetWindow, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

                // Compute new styles (XOR of the inverse of all the bits to filter)
                var styleNewWindowStandard =
                    styleCurrentWindowStandard
                    & ~(
                        WINDOW_STYLE.WS_CAPTION // composite of Border and DialogFrame
                        // | WINDOW_STYLE.WS_BORDER
                        //| WINDOW_STYLE.WS_DLGFRAME                  
                        | WINDOW_STYLE.WS_THICKFRAME
                        | WINDOW_STYLE.WS_OVERLAPPEDWINDOW
                        | WINDOW_STYLE.WS_SYSMENU
                        | WINDOW_STYLE.WS_MAXIMIZEBOX // same as TabStop
                        | WINDOW_STYLE.WS_MINIMIZEBOX // same as Group
                    );

                var styleNewWindowExtended =
                    styleCurrentWindowExtended
                    & ~(
                        WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME
                        | WINDOW_EX_STYLE.WS_EX_COMPOSITED
                        | WINDOW_EX_STYLE.WS_EX_WINDOWEDGE
                        | WINDOW_EX_STYLE.WS_EX_CLIENTEDGE
                        | WINDOW_EX_STYLE.WS_EX_LAYERED
                        | WINDOW_EX_STYLE.WS_EX_STATICEDGE
                        | WINDOW_EX_STYLE.WS_EX_TOOLWINDOW
                        | WINDOW_EX_STYLE.WS_EX_APPWINDOW
                    );

                // Should have process details by now
                if (processDetails != null)
                {
                    // Save original details on this window so that we have a chance at undoing the process
                    processDetails.OriginalStyleFlagsStandard = styleCurrentWindowStandard;
                    processDetails.OriginalStyleFlagsExtended = styleCurrentWindowExtended;
                    PInvoke.GetWindowRect(processDetails.WindowHandle, out RECT rectTemp);
                    processDetails.OriginalLocation = new Rectangle(rectTemp.left, rectTemp.top,
                        rectTemp.right - rectTemp.left, rectTemp.bottom - rectTemp.top);
                }

                // remove the menu and menu items and force a redraw
                if (favDetails.RemoveMenus)
                {
                    // unfortunately, menus can't be re-added easily so they aren't removed by default anymore
                    var menuHandle = PInvoke.GetMenu(targetWindow);
                    if (menuHandle != HWND.Null)
                    {
                        var menuItemCount = PInvoke.GetMenuItemCount(menuHandle);

                        for (var i = 0; i < menuItemCount; i++)
                        {
                            PInvoke.RemoveMenu(menuHandle, 0, MENU_ITEM_FLAGS.MF_BYPOSITION | MENU_ITEM_FLAGS.MF_REMOVE);
                        }

                        PInvoke.DrawMenuBar(targetWindow);
                    }
                }

                // auto-hide the Windows task bar (do this before resizing the window)
                if (favDetails.HideWindowsTaskbar)
                {
                    PInvoke.ShowWindow(new HWND(_windowHandle), SHOW_WINDOW_CMD.SW_SHOWNOACTIVATE);
                    if (windowMain.WindowState == System.Windows.WindowState.Minimized)
                    {
                        windowMain.WindowState = System.Windows.WindowState.Normal;
                    }

                    ToggleWindowsTaskbarVisibility(Boolstate.False);
                }

                // auto-hide the mouse cursor
                if (favDetails.HideMouseCursor)
                {
                    ToggleMouseCursorVisibility(windowMain, Boolstate.False);
                }

                // update window styles
                Native.SetWindowLong32(targetWindow, WINDOW_LONG_PTR_INDEX.GWL_STYLE, styleNewWindowStandard);
                Native.SetWindowLong64(targetWindow, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, styleNewWindowExtended);

                // update window position
                if (favDetails.Size != FavoriteSize.NoChange)
                {
                    if (favDetails.Size == FavoriteSize.FullScreen || favDetails.PositionWidth == 0 ||
                        favDetails.PositionHeight == 0)
                    {
                        // Set the window size to the biggest possible, using bounding adjustments
                        PInvoke.SetWindowPos
                        (
                            targetWindow,
                            HWND.Null,
                            targetFrame.X + favDetails.OffsetLeft,
                            targetFrame.Y + favDetails.OffsetTop,
                            targetFrame.Width - favDetails.OffsetLeft + favDetails.OffsetRight,
                            targetFrame.Height - favDetails.OffsetTop + favDetails.OffsetBottom,
                            SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER |
                            SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                        );

                        // And auto-maximize
                        if (favDetails.ShouldMaximize)
                        {
                            PInvoke.ShowWindow(targetWindow, SHOW_WINDOW_CMD.SW_MAXIMIZE);
                        }
                    }
                    else
                    {
                        // Set the window size to the exact position specified by the user
                        PInvoke.SetWindowPos
                        (
                            targetWindow,
                            HWND.Null,
                            favDetails.PositionX,
                            favDetails.PositionY,
                            favDetails.PositionWidth,
                            favDetails.PositionHeight,
                            SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER |
                            SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                        );
                    }
                }

                // Set topmost
                if (favDetails.TopMost)
                {
                    PInvoke.SetWindowPos
                    (
                        targetWindow,
                        Native.HWND_TOPMOST,
                        0,
                        0,
                        0,
                        0,
                        SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE |
                        SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                    );
                }
            }

            // Make a note that we attempted to make the window borderless
            if (processDetails != null)
            {
                processDetails.MadeBorderless = true;
                processDetails.MadeBorderlessAttempts++;
            }
            if (SteamApi.IsLoaded)
            {
                if (SteamApi.UnlockAchievement("FIRST_TIME_BORDERLESS"))
                {
                    Console.WriteLine("Great!");
                }
            }
        }

        private static async Task MakeWindowBorderlessDelayed(ProcessDetails processDetails, System.Windows.Window windowMain, HWND targetWindow, Rectangle targetFrame, Favorite favDetails)
        {
            var _windowHandle = new WindowInteropHelper(windowMain).Handle;
            // Automatically match a window to favorite details, if that information is available.
            // Note: if one is not available, the default settings will be used as a new Favorite() object.

            // Automatically match this window to a process

            // Failsafe to prevent rapid switching, but also allow a few changes to the window handle (to be persistent)
            if (processDetails != null)
            {
                if (processDetails.MadeBorderless)
                {
                    if (processDetails.MadeBorderlessAttempts > 3 || ! await processDetails.WindowHasTargetableStyles())
                    {
                        return;
                    }
                }
            }

            // If no target frame was specified, assume the entire space on the primary screen
            if (targetFrame.Width == 0 || targetFrame.Height == 0)
            {
                targetFrame = Screen.FromHandle(targetWindow).Bounds;
            }

            // Get window styles
            var styleCurrentWindowStandard = Native.GetWindowLong32(targetWindow, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            var styleCurrentWindowExtended = Native.GetWindowLong64(targetWindow, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

            // Compute new styles (XOR of the inverse of all the bits to filter)
            var styleNewWindowStandard =
                styleCurrentWindowStandard
                & ~(
                    WINDOW_STYLE.WS_CAPTION // composite of Border and DialogFrame
                    // | WINDOW_STYLE.WS_BORDER
                    //| WINDOW_STYLE.WS_DLGFRAME                  
                    | WINDOW_STYLE.WS_THICKFRAME
                    | WINDOW_STYLE.WS_OVERLAPPEDWINDOW
                    | WINDOW_STYLE.WS_SYSMENU
                    | WINDOW_STYLE.WS_MAXIMIZEBOX // same as TabStop
                    | WINDOW_STYLE.WS_MINIMIZEBOX // same as Group
                );

            var styleNewWindowExtended =
                styleCurrentWindowExtended
                & ~(
                    WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME
                    | WINDOW_EX_STYLE.WS_EX_COMPOSITED
                    | WINDOW_EX_STYLE.WS_EX_WINDOWEDGE
                    | WINDOW_EX_STYLE.WS_EX_CLIENTEDGE
                    | WINDOW_EX_STYLE.WS_EX_LAYERED
                    | WINDOW_EX_STYLE.WS_EX_STATICEDGE
                    | WINDOW_EX_STYLE.WS_EX_TOOLWINDOW
                    | WINDOW_EX_STYLE.WS_EX_APPWINDOW
                );

            // Should have process details by now
            if (processDetails != null)
            {
                // Save original details on this window so that we have a chance at undoing the process
                processDetails.OriginalStyleFlagsStandard = styleCurrentWindowStandard;
                processDetails.OriginalStyleFlagsExtended = styleCurrentWindowExtended;
                PInvoke.GetWindowRect(processDetails.WindowHandle, out RECT rect_temp);
                processDetails.OriginalLocation = new Rectangle(rect_temp.left, rect_temp.top,
                    rect_temp.right - rect_temp.left, rect_temp.bottom - rect_temp.top);
            }

            // remove the menu and menu items and force a redraw

            // unfortunately, menus can't be re-added easily so they aren't removed by default anymore
            var menuHandle = PInvoke.GetMenu(targetWindow);
            if (menuHandle != HMENU.Null)
            {
                var menuItemCount = PInvoke.GetMenuItemCount(menuHandle);

                for (var i = 0; i < menuItemCount; i++)
                {
                    PInvoke.RemoveMenu(menuHandle, 0, MENU_ITEM_FLAGS.MF_BYPOSITION | MENU_ITEM_FLAGS.MF_REMOVE);
                }

                PInvoke.DrawMenuBar(targetWindow);
            }


            // auto-hide the Windows task bar (do this before resizing the window)
            if (favDetails.HideWindowsTaskbar)
            {
                PInvoke.ShowWindow(new HWND(_windowHandle), SHOW_WINDOW_CMD.SW_SHOWNOACTIVATE);
                if (windowMain.WindowState == System.Windows.WindowState.Minimized)
                {
                    windowMain.WindowState = System.Windows.WindowState.Normal;
                }

                ToggleWindowsTaskbarVisibility(Boolstate.False);
            }

            // auto-hide the mouse cursor
            if (favDetails.HideMouseCursor)
            {
                ToggleMouseCursorVisibility(windowMain, Boolstate.False);
            }


            // update window position
            if (favDetails.Size != FavoriteSize.NoChange)
            {
                if (favDetails.Size == FavoriteSize.FullScreen || favDetails.PositionWidth == 0 ||
                    favDetails.PositionHeight == 0)
                {
                    // Set the window size to the biggest possible, using bounding adjustments
                    PInvoke.SetWindowPos
                    (
                        targetWindow,
                        HWND.Null,
                        targetFrame.X + favDetails.OffsetLeft,
                        targetFrame.Y + favDetails.OffsetTop,
                        targetFrame.Width - favDetails.OffsetLeft + favDetails.OffsetRight,
                        targetFrame.Height - favDetails.OffsetTop + favDetails.OffsetBottom,
                        SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW |
                        SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER | SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                    );
                }
                else
                {
                    // Set the window size to the exact position specified by the user
                    PInvoke.SetWindowPos
                    (
                        targetWindow,
                        HWND.Null,
                        favDetails.PositionX,
                        favDetails.PositionY,
                        favDetails.PositionWidth,
                        favDetails.PositionHeight,
                        SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW |
                        SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER | SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                    );
                }
            }

            // Set topmost
            if (favDetails.TopMost)
            {
                PInvoke.SetWindowPos
                (
                    targetWindow,
                    Native.HWND_TOPMOST,
                    0,
                    0,
                    0,
                    0,
                    SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOMOVE |
                    SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOSENDCHANGING
                );
            }
            //wait before applying styles
            await TaskUtilities.WaitAndStartTaskAsync(() =>
            {
                Native.SetWindowLong32(targetWindow, WINDOW_LONG_PTR_INDEX.GWL_STYLE, styleNewWindowStandard);
                Native.SetWindowLong64(targetWindow, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, styleNewWindowExtended);
            }, 4);
        }
        #nullable restore

        private static void RedrawWindowsSystemTrayArea()
        {
            try
            {
                // Windows XP and earlier
                var hNotificationArea = PInvoke.FindWindowEx
                (
                    new HWND(Native.FW(Native.FW(Native.FW(HWND.Null, "Shell_TrayWnd"), "TrayNotifyWnd"), "SysPager")),
                    HWND.Null,
                    "ToolbarWindow32",
                    "User Promoted Notification Area"
                );

                if (hNotificationArea == IntPtr.Zero || hNotificationArea == Native.INVALID_HANDLE_VALUE)
                {
                    return;
                }

                // Get the notification bounds
                PInvoke.GetClientRect(hNotificationArea, out RECT rect);

                // Wiggle the mouse over the notification area
                // Note: this doesn't actually move the mouse cursor on the screen -- this just sends a message to the system tray window
                //       that mouse movement occurred over it, forcing it to refresh.  Sending messages asking for a repaint or invalidated
                //       area don't work, but this does.
                for (int x = 0; x < rect.right; x += 5)
                for (int y = 0; y < rect.bottom; y += 5)
                {
                    PInvoke.SendMessage(hNotificationArea, PInvoke.WM_MOUSEMOVE, 0, new LPARAM((y << 16) | x));
                }
            }
            catch
            {
                // ignored
            }
        }

        private class OriginalScreenInfo
        {
            public Screen Screen;
            public RECT Workarea; // with Windows task bar
        }
    }
}