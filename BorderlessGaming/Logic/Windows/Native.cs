using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Windows.Win32;

using BorderlessGaming.Logic.Models;
using BorderlessGaming.Logic.Misc.Utilities;
using BorderlessGaming.Logic.Windows.Audio;
using BorderlessGaming.Logic.Extensions;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace BorderlessGaming.Logic.Windows
{
    public static class Native
    {
        #region Delegates

        public delegate bool EnumWindows_CallBackProc(IntPtr hwnd, uint lParam);

        #endregion

        internal static readonly HWND HWND_TOPMOST = new HWND(new IntPtr(-1));
        internal static readonly HWND HWND_NOTTOPMOST = new HWND(new IntPtr(-2));
        public const int INVALID_HANDLE_VALUE = -1;

        internal static List<WINDOW_STYLE> TargetStyles = new List<WINDOW_STYLE>
        {
            WINDOW_STYLE.WS_BORDER,
            WINDOW_STYLE.WS_DLGFRAME,
            WINDOW_STYLE.WS_THICKFRAME,
            WINDOW_STYLE.WS_SYSMENU,
            WINDOW_STYLE.WS_MAXIMIZEBOX,
            WINDOW_STYLE.WS_MINIMIZE
        };

        internal static List<WINDOW_EX_STYLE> ExtendedStyles = new List<WINDOW_EX_STYLE>
        {
            WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME,
            WINDOW_EX_STYLE.WS_EX_COMPOSITED,
            WINDOW_EX_STYLE.WS_EX_WINDOWEDGE,
            WINDOW_EX_STYLE.WS_EX_CLIENTEDGE,
            WINDOW_EX_STYLE.WS_EX_LAYERED,
            WINDOW_EX_STYLE.WS_EX_STATICEDGE,
            WINDOW_EX_STYLE.WS_EX_TOOLWINDOW,
            WINDOW_EX_STYLE.WS_EX_APPWINDOW
        };

        private static readonly object GetMainWindowForProcess_Locker = new object();

        private static HWND GetMainWindowForProcess_Value = HWND.Null;

        internal static bool HasTargetStyles(this WINDOW_STYLE flags)
        {
            return TargetStyles.Any(style => flags.HasFlag(style));
        }

        internal static bool HasExtendedStyles(this WINDOW_EX_STYLE flags)
        {
            return ExtendedStyles.Any(style => flags.HasFlag(style));
        }

        internal static string GetClassNameOfWindow(HWND hwnd)
        {
            var className = "";
            PWSTR classText = default;
            try
            {
                var cls_max_length = 1000;
                classText = new PWSTR();
                PInvoke.GetClassName(hwnd, classText, cls_max_length + 2);

                if (!string.IsNullOrEmpty(classText.ToString()) && !string.IsNullOrWhiteSpace(classText.ToString()))
                {
                    className = classText.ToString();
                }
            }
            catch (Exception ex)
            {
                className = ex.Message;
            }
            finally
            {
                classText = null;
            }
            return className;
        }

        internal static string GetWindowClassName(HWND hWnd)
        {
            int nRet;

            // Pre-allocate 256 characters, since this is the maximum class name length.
            var sbWindowClassName = new PWSTR();

            //Get the window class name
            nRet = PInvoke.GetClassName(hWnd, sbWindowClassName, 256);

            if (nRet != 0)
            {
                return sbWindowClassName.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        ///     Use this instead of GetWindowText.
        /// </summary>
        internal static string GetWindowTitle(HWND hWnd)
        {
            // Allocate correct string length first
            try
            {
                var length = (int)PInvoke.SendMessage(hWnd, PInvoke.WM_GETTEXTLENGTH, new WPARAM(), new LPARAM(IntPtr.Zero));
                var sbWindowTitle = new StringBuilder(length + 1);
                PInvoke.SendMessage(hWnd, PInvoke.WM_GETTEXT, new WPARAM((nuint)sbWindowTitle.Capacity), new LPARAM()); // sbWindowTitle.ToString());
                return sbWindowTitle.ToString();
            }
            catch (Exception)
            {
                return "<error>";
            }
        }

        internal static IntPtr FW(HWND hwndParent, string lpszClass)
        {
            return PInvoke.FindWindowEx(hwndParent, HWND.Null, lpszClass, string.Empty);
        }

        internal static IntPtr FW(IntPtr hwndParent, string lpszClass)
        {
            return FW(new HWND(hwndParent), lpszClass);
        }

        /// <summary>
        // This static method is required because legacy OSes do not support SetWindowLongPtr
        /// </summary>
        internal static WINDOW_STYLE GetWindowLong32(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex)
        {
            return (WINDOW_STYLE)PInvoke.GetWindowLongPtr(hWnd, nIndex);
        }

        /// <summary>
        // This static method is required because legacy OSes do not support SetWindowLongPtr
        /// </summary>
        internal static WINDOW_EX_STYLE GetWindowLong64(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex)
        {
            return (WINDOW_EX_STYLE)PInvoke.GetWindowLongPtr(hWnd, nIndex);
        }

        /// <summary>
        // This static method is required because legacy OSes do not support SetWindowLongPtr
        /// </summary>
        internal static WINDOW_STYLE SetWindowLong32(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, WINDOW_STYLE dwNewLong)
        {
            int _dwNewLong = (int)dwNewLong;
            return (WINDOW_STYLE)PInvoke.SetWindowLongPtr(hWnd, nIndex, _dwNewLong);
        }

        /// <summary>
        // This static method is required because legacy OSes do not support SetWindowLongPtr
        /// </summary>
        internal static WINDOW_EX_STYLE SetWindowLong64(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, WINDOW_EX_STYLE dwNewLong)
        {
            int _dwNewLong = (int)dwNewLong;
            return (WINDOW_EX_STYLE)PInvoke.SetWindowLongPtr(hWnd, nIndex, _dwNewLong);
        }

        // Do some preferential treatment to windows
        private static BOOL GetMainWindowForProcess_EnumWindows(HWND hWndEnumerated, LPARAM lParam)
        {
            if (GetMainWindowForProcess_Value == HWND.Null)
            {
                var styleCurrentWindow_standard = GetWindowLong32(hWndEnumerated, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

                if (lParam.Value == 0) // strict: windows that are visible and have a border
                {
                    if (PInvoke.IsWindowVisible(hWndEnumerated))
                    {
                        if
                        (
                            (styleCurrentWindow_standard & WINDOW_STYLE.WS_CAPTION) > 0
                            && (
                                (styleCurrentWindow_standard & WINDOW_STYLE.WS_BORDER) > 0
                                || (styleCurrentWindow_standard & WINDOW_STYLE.WS_THICKFRAME) > 0
                            )
                        )
                        {
                            GetMainWindowForProcess_Value = hWndEnumerated;
                            return false;
                        }
                    }
                }
                else if (lParam.Value == 1) // loose: windows that are visible
                {
                    if (PInvoke.IsWindowVisible(hWndEnumerated))
                    {
                        if ((uint)styleCurrentWindow_standard != 0)
                        {
                            GetMainWindowForProcess_Value = hWndEnumerated;
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     steveadoo32: I'd like to get rid of this method eventually. there was a big change
        ///     while i was working on the new stuff so I'm keeping this for now.
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        internal static async Task<HWND> GetMainWindowForProcess(Process process)
        {
            if (SettingsWrapper.Instance.Settings.SlowWindowDetection is true)
            {
                try
                {
                    var hMainWindow = HWND.Null;

                    GetMainWindowForProcess_Value = HWND.Null;
                    await TaskUtilities.StartTaskAndWait(() =>
                    {
                        for (uint i = 0; i <= 1; i++)
                        {
                            foreach (ProcessThread thread in process.Threads)
                            {
                                if (GetMainWindowForProcess_Value != HWND.Null)
                                {
                                    break;
                                }

                                PInvoke.EnumThreadWindows((uint)thread.Id, GetMainWindowForProcess_EnumWindows, new LPARAM((nint)i));
                            }
                        }
                    });
                    hMainWindow = GetMainWindowForProcess_Value;
                    if (hMainWindow != HWND.Null)
                    {
                        return hMainWindow;
                    }
                }
                catch
                {
                }
            }

            try
            {
                // Failsafe
                //process.Refresh();
                return new HWND(process.MainWindowHandle);
            }
            catch
            {
            }

            return HWND.Null;
        }


        /// <summary>
        ///     Query the windows
        /// </summary>
        /// <param name="callback">
        ///     A callback that's called when a new window is found. This way the functionality is the same as
        ///     before
        /// </param>
        /// <param name="windowPtrSet">A set of current window ptrs</param>
        internal static void QueryProcessesWithWindows(Action<ProcessDetails> callback, List<HWND> windowPtrSet)
        {
            var hWndList = new List<HWND>();

            BOOL Del(HWND hwnd, LPARAM lParam)
            {
                return GetMainWindowForProcess_EnumWindows(hWndList, hwnd, lParam);
            }

            PInvoke.EnumWindows(Del, 0);
            PInvoke.EnumWindows(Del, 1);
            foreach (var hWnd in hWndList)
            {
                if (PInvoke.GetWindowRect(hWnd, out RECT rect))
                {
                    if (((Rectangle)rect).IsEmpty)
                    {
                        continue;
                    }
                    if (windowPtrSet.Contains(hWnd))
                    {
                        continue;
                    }
                    // If GetWindowThreadProcessId fails the entire application crashes, so hopefully this will handle it safely.
                    try
                    {
                        unsafe
                        {
                            uint processId = default(uint);
                            PInvoke.GetWindowThreadProcessId(hWnd, &processId);
                            var process = ProcessExtensions.GetProcessById((int)processId);
                            if (process == null)
                            {
                                continue;
                            }
                            callback(new ProcessDetails(process, hWnd)
                            {
                                Manageable = true
                            });
                        }
                    }
                    catch
                    {
                        // Ignore or log (Counts as a continue).
                    }
                }
            }
        }

        private static bool GetMainWindowForProcess_EnumWindows(List<HWND> hWndList, HWND hWndEnumerated, LPARAM lParam)
        {
            var styleCurrentWindowStandard = GetWindowLong32(hWndEnumerated, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

            switch (lParam.Value)
            {
                case 0:
                    if (PInvoke.IsWindowVisible(hWndEnumerated))
                    {
                        if
                        (
                            (styleCurrentWindowStandard & WINDOW_STYLE.WS_CAPTION) > 0
                            && (
                                (styleCurrentWindowStandard & WINDOW_STYLE.WS_BORDER) > 0
                                || (styleCurrentWindowStandard & WINDOW_STYLE.WS_THICKFRAME) > 0
                            )
                        )
                        {
                            hWndList.Add(hWndEnumerated);
                        }
                    }
                    break;
                case 1:
                    if (PInvoke.IsWindowVisible(hWndEnumerated))
                    {
                        if ((uint)styleCurrentWindowStandard != 0)
                        {
                            hWndList.Add(hWndEnumerated);
                        }
                    }
                    break;
            }
            return true;
        }

        public static void UnMuteProcess(int pId)
        {
            if (IsMuted(pId))
            {
                VolumeMixer.SetApplicationMute(pId, false);
            }
        }

        public static bool IsMuted(int pId)
        {
            var applicationMute = VolumeMixer.GetApplicationMute(pId);
            var isMuted = applicationMute != null && (bool)applicationMute;
            return isMuted;
        }

        public static void MuteProcess(int pId)
        {
            if (!IsMuted(pId))
            {
                VolumeMixer.SetApplicationMute(pId, true);
            }
        }

        public static void RegisterHotKey(nint handle, int id, int fsModifiers, uint vk)
        {
            PInvoke.RegisterHotKey(new HWND(handle), id, (HOT_KEY_MODIFIERS)fsModifiers, vk);
        }

        public static void UnregisterHotKey(nint handle, int id)
        {
            PInvoke.UnregisterHotKey(new HWND(handle), id);
        }
    }
}