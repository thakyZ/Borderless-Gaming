using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using BorderlessGaming.Logic.Extensions;
using BorderlessGaming.Logic.Models;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace BorderlessGaming.Logic.Windows
{
    public class Windows
    {
        /// <summary>
        ///     Query the windows
        /// </summary>
        /// <param name="callback">
        ///     A callback that's called when a new window is found. This way the functionality is the same as
        ///     before
        /// </param>
        /// <param name="windowPtrSet">A set of current window ptrs</param>
        public void QueryProcessesWithWindows(Action<ProcessDetails> callback, HashSet<long> windowPtrSet)
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
                    //check if we already have this window in the list so we can avoid calling
                    //GetWindowThreadProcessId(its costly)
                    if (windowPtrSet.Contains(hWnd))
                    {
                        continue;
                    }
                    try
                    {
                        unsafe
                        {
                            uint processId = default(uint);
                            PInvoke.GetWindowThreadProcessId(hWnd, &processId);
                            var process = ProcessExtensions.GetProcessById(processId);
                            if (process is null)
                            {
                                continue;
                            }
                            callback(new ProcessDetails(process, hWnd)
                            {
                                Manageable = true
                            });
                        }
                    }
                    catch (Exception exception)
                    {
                        // TODO: Log error here...
                    }
                }
            }
        }

        private static bool GetMainWindowForProcess_EnumWindows(List<HWND> hwndList, HWND hWndEnumerated, LPARAM lParam)
        {
            var styleCurrentWindowStandard = Native.GetWindowLong32(hWndEnumerated, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

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
                            hwndList.Add(hWndEnumerated);
                        }
                    }
                    break;
                case 1:
                    if (PInvoke.IsWindowVisible(hWndEnumerated))
                    {
                        if ((uint)styleCurrentWindowStandard != 0)
                        {
                            hwndList.Add(hWndEnumerated);
                        }
                    }
                    break;
            }
            return true;
        }
    }
}