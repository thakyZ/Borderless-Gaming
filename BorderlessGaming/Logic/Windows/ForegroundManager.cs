using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.Input;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

using BorderlessGaming.Logic.Extensions;
using BorderlessGaming.Logic.Models;

namespace BorderlessGaming.Logic.Windows
{
    public static class ForegroundManager
    {
        static WINEVENTPROC _dele = null;
        private static UnhookWinEventSafeHandle _mHhook;

        public static void Subscribe()
        {
            _dele = WinEventProc;
            SafeHandle safeHandle = default(SafeHandle);
            _mHhook = PInvoke.SetWinEventHook(EventSystemForeground, EventSystemForeground, safeHandle, _dele, 0, 0, WineventOutofcontext);
        }

        private const uint WineventOutofcontext = 0;
        private const uint EventSystemForeground = 3;


        internal static void WinEventProc(HWINEVENTHOOK hWinEventHook, uint eventType, HWND hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (SettingsWrapper.Instance.Favorites is not null)
            {
                try
                {
                    unsafe
                    {
                        var handle = PInvoke.GetForegroundWindow();
                        uint processId = default(uint);
                        PInvoke.GetWindowThreadProcessId(handle, &processId);
                        var process = ProcessExtensions.GetProcessById((int)processId);
                        if (process is null)
                        {
                            return;
                        }
                        var details = new ProcessDetails(process, handle);
                        foreach (var fav in SettingsWrapper.Instance.Favorites.Where(favorite => favorite.IsRunning && favorite.MuteInBackground))
                        {

                            if (fav.Matches(details))
                            {
                                if (Native.IsMuted((int)processId))
                                {
                                    Native.UnMuteProcess((int)processId);
                                }
                            }
                            else
                            {
                                if (!Native.IsMuted(fav.RunningId))
                                {
                                    Native.MuteProcess(fav.RunningId);
                                }
                            }
                        }
                    }

                }
                catch (Exception)
                {
                    //
                }
            }

        }
    }
}