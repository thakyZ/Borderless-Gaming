#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Windows.Win32;
using Windows.Win32.Foundation;
using Hardcodet.Wpf.TaskbarNotification;

using BorderlessGaming.Logic.Core;
using BorderlessGaming.Logic.Extensions;
using BorderlessGaming.Logic.Misc;
using BorderlessGaming.Logic.Models;
using BorderlessGaming.Logic.NekoBoiNick;
using BorderlessGaming.Logic.Steam;
using BorderlessGaming.Logic.Windows;
using Manipulation = BorderlessGaming.Logic.Windows.Manipulation;
using BorderlessGaming.Properties;
using BorderlessGaming.Windows.Components;
using BorderlessGaming.Windows.ViewModels;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace BorderlessGaming.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowWPF : Window
    {
        private MainWindowViewModel ViewModel => (this.DataContext as MainWindowViewModel)!;
        public MainWindowWPF()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(this);
            this._watcher = new ProcessWatcher(this);
        }

        private HWND Handle => new HWND();

        #region Local data

        #endregion


        #region Application Menu Events

        private void HandleProcessChange(ProcessDetails process, bool remove)
        {
            if (process == null)
            {
                return;
            }
            if (remove)
            {
                this.ViewModel.ApplicationsListBoxItemsSource.Remove(process);
            }
            else
            {
                this.ViewModel.ApplicationsListBoxItemsSource.Add(process);
            }

            this.ViewModel.StatusLabel = $@"{LanguageManager.Data("moreOptionsLabel")} {TimeProvider.System}";
        }

        internal async Task RefreshProcesses()
        {
            //clear the process list and repopulate it
            this.ViewModel.ApplicationsListBoxItemsSource.Clear();
            await _watcher.Refresh();
        }

        private void usageGuide_MenuItem_Click(object sender, EventArgs e)
        {
            Tools.GotoSite("http://steamcommunity.com/app/388080/discussions/0/535151589899658778/");
        }

        #endregion

        #region Application Form Events

        private void lstProcesses_SelectedIndexChanged(object sender, EventArgs e)
        {
            var validSelection = false;

            if (this.ViewModel.ApplicationsListBoxSelectedItem is ProcessDetails pd)
            {
                validSelection = pd.Manageable;
            }

            this.ViewModel.MakeBorderlessButtonIsEnabled = this.ViewModel.RestoreWindowIsEnabled =  this.ViewModel.FavoriteButtonIsEnabled = validSelection;
        }

        private void lstFavorites_SelectedIndexChanged(object sender, EventArgs e)
        {
           this.ViewModel.UnfavoriteButtonIsEnabled = this.ViewModel.FavoriteListBoxSelectedItem is not null;
        }

        private void setWindowTitle_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.ApplicationsListBoxSelectedItem is not ProcessDetails pd)
            {
                return;
            }

            if (!pd.Manageable)
            {
                return;
            }

            PInvoke.SetWindowText(pd.WindowHandle,
                InputText(LanguageManager.Data("setWindowTitleTitle"), LanguageManager.Data("setWindowTitlePrompt"),
                    Native.GetWindowTitle(pd.WindowHandle)));
        }

        private async void hideThisProcess_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.ApplicationsListBoxSelectedItem is not ProcessDetails pd)
            {
                return;
            }

            if (!pd.Manageable)
            {
                return;
            }

            SettingsWrapper.Instance.ExcludeProcess(pd.BinaryName.Trim().ToLower());
            await RefreshProcesses();
        }

        /// <summary>
        /// Makes the currently selected process borderless
        /// </summary>
        private async void btnMakeBorderless_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.ApplicationsListBoxSelectedItem is not ProcessDetails pd)
            {
                return;
            }

            if (!pd.Manageable)
            {
                return;
            }

            await _watcher.RemoveBorder(pd);
        }

        private void btnRestoreWindow_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.ApplicationsListBoxSelectedItem is not ProcessDetails pd)
            {
                return;
            }

            if (!pd.Manageable)
            {
                return;
            }

            Manipulation.RestoreWindow(pd);
        }

        /// <summary>
        /// adds the currently selected process to the favorites (by window title text)
        /// </summary>
        private void byTheWindowTitleText_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.ApplicationsListBoxSelectedItem is not ProcessDetails pd)
            {
                return;
            }


            if (!pd.Manageable)
            {
                return;
            }
            var favorite = new Favorite
            {
                Type = FavoriteType.Title,
                SearchText = pd.WindowTitle,
                Screen = ProcessRectangle.Empty
            };
            SettingsWrapper.Instance.AddFavorite(favorite, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Add(favorite);
            });

        }

        /// <summary>
        /// adds the currently selected process to the favorites (by process binary name)
        /// </summary>
        private void byTheProcessBinaryName_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.ApplicationsListBoxSelectedItem is not ProcessDetails pd)
            {
                return;
            }

            if (!pd.Manageable)
            {
                return;
            }
          var favorite = new Favorite
          {
              Type = FavoriteType.Process,
              SearchText = pd.BinaryName,
              Screen = ProcessRectangle.Empty
          };
            SettingsWrapper.Instance.AddFavorite(favorite, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Add(favorite);
            });
        }

        /// <summary>
        /// adds the currently selected process to the favorites (by window title text)
        /// </summary>
        private void byTheWindowTitleTextregex_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.ApplicationsListBoxSelectedItem is not ProcessDetails pd)
            {
                return;
            }

            if (!pd.Manageable)
            {
                return;
            }
            var res = InputText("Add to favorites by RegEx string",
                "Regex string (see the Help menu for reference)", pd.WindowTitle);
            if (!string.IsNullOrWhiteSpace(res?.Trim()))
            {
                var favorite = new Favorite
                {
                    Type = FavoriteType.Regex,
                    SearchText = res,
                    Screen = ProcessRectangle.Empty
                };
                SettingsWrapper.Instance.AddFavorite(favorite, () =>
                {
                    this.ViewModel.FavoriteListBoxItemsSource.Add(favorite);
                });
            }
        }
        private string? InputText(string sTitle, string sInstructions, string sDefaultValue = "")
        {
            try
            {
                using (var inputForm = new InputTextWPF())
                {
                    inputForm.SetTitle(sTitle);
                    inputForm.SetInstructions(sInstructions);
                    inputForm.SetInput(sDefaultValue);
                    if (inputForm.ShowDialog() == MessageBoxResult.OK)
                    {
                        return inputForm.GetCurrentValue();
                    }

                    return sDefaultValue;
                }
            }
            catch
            {
                // ignored
            }

            return string.Empty;
        }
        private void addSelectedItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.ApplicationsListBoxSelectedItem is not ProcessDetails pd)
            {
                return;
            }

            if (!pd.Manageable)
            {
                return;
            }

            if (!string.IsNullOrEmpty(pd.WindowTitle))
            {
                byTheWindowTitleText_MenuItem_Click(sender, e);
            }
            else
            {
                byTheProcessBinaryName_MenuItem_Click(sender, e);
            }
        }

        private void RefreshFavoritesList(Favorite fav)
        {
            //refreshing is done through observables so this method just readds the favorite
            //to make it look like it updated and because i dont want to change all that code
            SettingsWrapper.Instance.AddFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Add(fav);
            });
        }

        /// <summary>
        /// removes the currently selected entry from the favorites
        /// </summary>
        private void btnRemoveFavorite_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }
            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
              this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });
        }

        private void removeMenus_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }

            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });
            fav.RemoveMenus = this.ViewModel.RemoveMenusIsChecked;
            RefreshFavoritesList(fav);
        }

        private void alwaysOnTop_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }

            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });
            fav.TopMost = this.ViewModel.AlwaysOnTopIsChecked;
            RefreshFavoritesList(fav);
        }

        private void adjustWindowBounds_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }

            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });
            var (favOffsetL, favOffsetR, favOffsetT, favOffsetB) = InputSize(LanguageManager.Data("adjustWindowBoundsTitle"), LanguageManager.Data("adjustWindowBoundsLeft"), LanguageManager.Data("adjustWindowBoundsRight"), LanguageManager.Data("adjustWindowBoundsTop"), LanguageManager.Data("adjustWindowBoundsBottom"), false, null, fav.PositionX, fav.PositionY, fav.PositionWidth, fav.PositionHeight);
            /*
            int.TryParse(
InputText(LanguageManager.Data("adjustWindowBoundsTitle"),
string.Format(LanguageManager.Data("adjustWindowBoundsPrompt"), LanguageManager.Data("adjustWindowBoundsLeft")), fav.OffsetLeft.ToString()),
out int favOffsetL);
            int.TryParse(
               InputText(LanguageManager.Data("adjustWindowBoundsTitle"),
                   string.Format(LanguageManager.Data("adjustWindowBoundsPrompt"), LanguageManager.Data("adjustWindowBoundsRight")), fav.OffsetRight.ToString()),
                out int favOffsetR);
            int.TryParse(
                InputText(LanguageManager.Data("adjustWindowBoundsTitle"),
                    string.Format(LanguageManager.Data("adjustWindowBoundsPrompt"), LanguageManager.Data("adjustWindowBoundsTop")), fav.OffsetTop.ToString()),
                out int favOffsetT);
            int.TryParse(
                InputText(LanguageManager.Data("adjustWindowBoundsTitle"),
                    string.Format(LanguageManager.Data("adjustWindowBoundsPrompt"), LanguageManager.Data("adjustWindowBoundsBottom")), fav.OffsetBottom.ToString()),
                out int favOffsetB);
            */

            fav.OffsetLeft = favOffsetL;
            fav.OffsetRight = favOffsetR;
            fav.OffsetTop = favOffsetT;
            fav.OffsetBottom = favOffsetB;

            RefreshFavoritesList(fav);
        }

        private void automaximize_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }

            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });

            fav.ShouldMaximize = this.ViewModel.AutoMaximizeIsChecked;

            if (fav.ShouldMaximize)
            {
                fav.Size = FavoriteSize.FullScreen;
                fav.PositionX = 0;
                fav.PositionY = 0;
                fav.PositionWidth = 0;
                fav.PositionHeight = 0;
            }

            RefreshFavoritesList(fav);
        }

        private void hideMouseCursor_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }
            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });
            fav.HideMouseCursor = this.ViewModel.HideMouseCursorIsChecked;
            RefreshFavoritesList(fav);
        }

        private void hideWindowsTaskbar_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }
            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });

            fav.HideWindowsTaskbar = this.ViewModel.HideWindowsTaskbarIsChecked;

            RefreshFavoritesList(fav);
        }

        private void setWindowSize_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }


            var result =
                MessageBox.Show(
                   LanguageManager.Data("setWindowSizeMousePrompt"),
                   LanguageManager.Data("setWindowSizeMouseTitle"), MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result is MessageBoxResult.Cancel)
            {
                return;
            }

            if (result is MessageBoxResult.Yes)
            {
                using (var frmSelectArea = new DesktopAreaSelectorWPF())
                {
                    if (frmSelectArea.ShowDialog() is not MessageBoxResult.OK)
                    {
                        return;
                    }

                    // Temporarily disable compiler warning CS1690: http://msdn.microsoft.com/en-us/library/x524dkh4.aspx
                    //
                    // We know what we're doing: everything is safe here.
#pragma warning disable 1690
                    var rect = frmSelectArea.GetCurrentValue();
                    fav.PositionX = rect.X;
                    fav.PositionY = rect.Y;
                    fav.PositionWidth = rect.Width;
                    fav.PositionHeight = rect.Height;
#pragma warning restore 1690
                }
            }
            else // System.Windows.Forms.DialogResult.No
            {
                var (favPositionX, favPositionY, favPositionW, favPositionH) = InputSize(LanguageManager.Data("setWindowSizeTitle"), string.Format(LanguageManager.Data("setWindowSizePixelPrompt"), "X"), string.Format(LanguageManager.Data("setWindowSizePixelPrompt"), "Y"), LanguageManager.Data("setWindowSizeWidthPrompt"), LanguageManager.Data("setWindowSizeHeightPrompt"), false, null, fav.PositionX, fav.PositionY, fav.PositionWidth, fav.PositionHeight);
                /*
                int.TryParse(
                    InputText(LanguageManager.Data("setWindowSizeTitle"), string.Format(LanguageManager.Data("setWindowSizePixelPrompt"), "X"),
                        fav.PositionX.ToString()), out int favPositionX);
                int.TryParse(
                    InputText(LanguageManager.Data("setWindowSizeTitle"), string.Format(LanguageManager.Data("setWindowSizePixelPrompt"), "Y"),
                        fav.PositionY.ToString()), out int favPositionY);
                int.TryParse(
                    InputText(LanguageManager.Data("setWindowSizeTitle"), LanguageManager.Data("setWindowSizeWidthPrompt"),
                        fav.PositionWidth.ToString()), out int favPositionW);
                int.TryParse(
                    InputText(LanguageManager.Data("setWindowSizeTitle"), LanguageManager.Data("setWindowSizeHeightPrompt"),
                        fav.PositionHeight.ToString()), out int favPositionH);
                */
                fav.PositionX = favPositionX;
                fav.PositionHeight = favPositionH;
                fav.PositionWidth = favPositionW;
                fav.PositionY = favPositionY;
            }

            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });

            if (fav.PositionWidth == 0 || fav.PositionHeight == 0)
            {
                fav.Size = FavoriteSize.FullScreen;
            }
            else
            {
                fav.Size = FavoriteSize.SpecificSize;
                fav.ShouldMaximize = false;
            }
            RefreshFavoritesList(fav);
        }

        private void fullScreen_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }

            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });


            fav.Size = this.ViewModel.FullScreenIsChecked ? FavoriteSize.FullScreen : FavoriteSize.NoChange;

            if (fav.Size == FavoriteSize.FullScreen)
            {
                fav.PositionX = 0;
                fav.PositionY = 0;
                fav.PositionWidth = 0;
                fav.PositionHeight = 0;
            }
            else
            {
                fav.ShouldMaximize = false;
            }

            RefreshFavoritesList(fav);
        }


        private void noSizeChange_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }

            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });

            fav.Size = this.ViewModel.NoSizeChangeIsChecked ? FavoriteSize.NoChange : FavoriteSize.FullScreen;

            if (fav.Size == FavoriteSize.NoChange)
            {
                fav.ShouldMaximize = false;
                fav.OffsetLeft = 0;
                fav.OffsetRight = 0;
                fav.OffsetTop = 0;
                fav.OffsetBottom = 0;
                fav.PositionX = 0;
                fav.PositionY = 0;
                fav.PositionWidth = 0;
                fav.PositionHeight = 0;
            }

            RefreshFavoritesList(fav);
        }

        private void delayBorderless_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }

            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });

            fav.DelayBorderless = this.ViewModel.DelayBorderlessIsChecked;
            RefreshFavoritesList(fav);
        }

        /// <summary>
        /// Sets up the Favorite-ContextMenu according to the current state
        /// </summary>
        private void mnuFavoritesContext_Opening(object sender, CancelEventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                e.Cancel = true;
                return;
            }

            this.ViewModel.FullScreenIsChecked = fav.Size == FavoriteSize.FullScreen;

            this.ViewModel.MuteInBackgroundIsChecked = fav.MuteInBackground;
            this.ViewModel.AutoMaximizeIsChecked = fav.ShouldMaximize;
            this.ViewModel.AlwaysOnTopIsChecked = fav.TopMost;
            this.ViewModel.HideMouseCursorIsChecked = fav.HideMouseCursor;
            this.ViewModel.HideWindowsTaskbarIsChecked = fav.HideWindowsTaskbar;
            this.ViewModel.RemoveMenusIsChecked = fav.RemoveMenus;

            this.ViewModel.AutoMaximizeIsEnabled = fav.Size == FavoriteSize.FullScreen;
            this.ViewModel.AdjustWindowBoundsIsEnabled = fav.Size == FavoriteSize.FullScreen && !fav.ShouldMaximize;
            this.ViewModel.SetWindowSizeIsEnabled = fav.Size != FavoriteSize.FullScreen;
            this.ViewModel.SetWindowSizeIsChecked = fav.Size == FavoriteSize.SpecificSize;
            this.ViewModel.NoSizeChangeIsChecked = fav.Size == FavoriteSize.NoChange;

            this.ViewModel.EditRegexIsVisible = this.ViewModel.EditRegexIsEnabled = fav.Type == FavoriteType.Regex;
            this.ViewModel.SetWindowSizeKeepRatioIsChecked = fav.Size == FavoriteSize.SpecificSize;

            if (WpfScreen.AllScreens().Count() < 2)
            {
                this.ViewModel.FavScreenIsVisible = false;
            }
            else
            {
                this.ViewModel.FavScreenIsVisible = true;

                if (this.ViewModel.FavScreenHasDropDownItems)
                {
                    this.ViewModel.FavScreenDropDownItemsSource.Clear();
                }

                var superSize = WpfScreen.PrimaryScreen.Bounds;

                foreach (var screen in WpfScreen.AllScreens())
                {
                    superSize = Tools.GetContainingRectangle(superSize, screen.Bounds);

                    // fix for a .net-bug on Windows XP
                    var idx = screen.DeviceName.IndexOf('\0');
                    var fixedDeviceName = idx > 0 ? screen.DeviceName.Substring(0, idx) : screen.DeviceName;

                    var label = fixedDeviceName + (screen.IsPrimary ? " (P)" : string.Empty);
                    this.ViewModel.FavScreenDropDownItemsSource.Add(new MenuItem
                    {
                        Header =  label,
                        IsCheckable = true,
                        IsChecked = fav.Screen?.Equals(ProcessRectangle.ToProcessRectangle(screen.Bounds)) ?? false,
                        Command = new CommandImpl<bool>((bool isChecked) =>
                        {
                            fav.Screen = isChecked ? ProcessRectangle.ToProcessRectangle(screen.Bounds) : new ProcessRectangle();
                            SettingsWrapper.Save();
                        }),
                    });
                }
                // add supersize Option
                this.ViewModel.FavScreenDropDownItemsSource.Add(new MenuItem
                {
                    Header = LanguageManager.Data("superSize"),
                    IsCheckable = true,
                    IsChecked = fav.Screen?.Equals(ProcessRectangle.ToProcessRectangle(superSize)) ?? false,
                    Command = new CommandImpl(() =>
                    {
                        fav.Screen = ProcessRectangle.ToProcessRectangle(superSize);
                        SettingsWrapper.Save();
                    }),
                });
            }
        }

        /// <summary>
        /// Sets up the Process-ContextMenu according to the current state
        /// </summary>
        private void processContext_Opening(object sender, CancelEventArgs e)
        {
            if (this.ViewModel.ApplicationsListBoxSelectedItem is not ProcessDetails pd)
            {
                e.Cancel = true;
                return;
            }

            if (!pd.Manageable)
            {
                e.Cancel = true;
                return;
            }

            this.ViewModel.AddToFavsIsEnabled = SettingsWrapper.Instance.CanAddFavorite(pd.BinaryName) &&
                                       SettingsWrapper.Instance.CanAddFavorite(pd.WindowTitle);

            if (WpfScreen.AllScreens().Count() < 2)
            {
                this.ViewModel.BorderlessOnIsVisible = false;
            }
            else
            {
                this.ViewModel.BorderlessOnIsVisible = true;

                if (this.ViewModel.BorderlessOnHasDropDownItems)
                {
                    this.ViewModel.BorderlessOnDropDownItemsSource.Clear();
                }

                var superSize = WpfScreen.PrimaryScreen.Bounds;

                foreach (var screen in WpfScreen.AllScreens())
                {
                    superSize = Tools.GetContainingRectangle(superSize, screen.Bounds);

                    // fix for a .net-bug on Windows XP
                    var idx = screen.DeviceName.IndexOf('\0');
                    var fixedDeviceName = idx > 0 ? screen.DeviceName.Substring(0, idx) : screen.DeviceName;

                    this.ViewModel.BorderlessOnDropDownItemsSource.Add(new MenuItem() {
                        Header = fixedDeviceName + (screen.IsPrimary ? " (P)" : string.Empty),
                        Command = new AsyncCommandImpl(async () => await _watcher.RemoveBorder_ToSpecificScreen(pd, screen)),
                    });
                }

                // add super size Option
                this.ViewModel.BorderlessOnDropDownItemsSource.Add(new MenuItem {
                    Header = LanguageManager.Data("superSize"),
                    Command = new AsyncCommandImpl(async () => await _watcher.RemoveBorder_ToSpecificRect(pd, superSize)),
                });
            }
        }

        private MenuItem? _toolStripDisableSteamIntegration;

        /// <summary>
        /// Sets up the form
        /// </summary>
        private void MainWindow_Load(object sender, EventArgs e)
        {
            // set the title
            this.ViewModel.Title = "Borderless Gaming " + Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) + ((Uac.Elevated) ? " [Administrator]" : "");

            var settings = SettingsWrapper.Instance.Settings;
            // load up settings
            this.ViewModel.RunOnStartupIsChecked = settings.RunOnStartup.GetValueOrDefault();
            this.ViewModel.GlobalHotkeyIsChecked = settings.UseGlobalHotkey.GetValueOrDefault();
            this.ViewModel.CheckForUpdatesIsChecked = settings.CheckForUpdates.GetValueOrDefault();
            this.ViewModel.MouseLockIsChecked = settings.UseMouseLockHotKey.GetValueOrDefault();
            this.ViewModel.MouseHideIsChecked = settings.UseMouseHideHotKey.GetValueOrDefault();
            this.ViewModel.MinimizedToTrayIsChecked = settings.StartMinimized.GetValueOrDefault();
            this.ViewModel.HideBalloonTipsIsChecked = settings.HideBalloonTips.GetValueOrDefault();
            this.ViewModel.CloseToTrayIsChecked = settings.CloseToTray.GetValueOrDefault();
            this.ViewModel.ViewFullProcessDetailsIsChecked = settings.ViewAllProcessDetails.GetValueOrDefault();
            this.ViewModel.SlowWindowDetectionIsChecked = settings.SlowWindowDetection.GetValueOrDefault();

            // minimize the window if desired (hiding done in Shown)
            if (settings.StartMinimized.GetValueOrDefault() || SettingsWrapper.Instance.StartupOptions.Minimize)
            {
                WindowState = WindowState.Minimized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }

            if (SteamApi.IsLoaded && _toolStripDisableSteamIntegration == null)
            {
                _toolStripDisableSteamIntegration =
                    new MenuItem
                    {
                        Name = "toolStripDisableSteamIntegration",
                        RenderSize = new Size(254, 22),
                        Header = LanguageManager.Data("toolStripDisableSteamIntegration"),
                        ToolTip = new ToolTip {
                            Content = LanguageManager.Data("steamHint"),
                        },
                        IsChecked = settings.DisableSteamIntegration.GetValueOrDefault(),
                        IsCheckable = true,
                    };
                // let's do this before registering the CheckedChanged event
                _toolStripDisableSteamIntegration.Checked += ToolStripDisableSteamIntegrationCheckChanged;
                this.ViewModel.MenuItemDropDownItemsSource.Insert(0, _toolStripDisableSteamIntegration);
            }
        }

        private void ToolStripDisableSteamIntegrationCheckChanged(object sender, EventArgs e)
        {
            SettingsWrapper.Instance.Settings.DisableSteamIntegration = _toolStripDisableSteamIntegration?.IsChecked ?? true;
            SettingsWrapper.Save();
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            // hide the window if desired (this doesn't work well in Load)
            if (SettingsWrapper.Instance.Settings.StartMinimized.GetValueOrDefault() || SettingsWrapper.Instance.StartupOptions.Minimize)
            {
                Hide();
            }
            // initialize favorite list
            foreach (var ni in SettingsWrapper.Instance.Favorites)
            {
                this.ViewModel.FavoriteListBoxItemsSource.Add(ni);
            }

            // start Task API controller
            _watcher.Start(HandleProcessChange);

            // Update buttons' enabled/disabled state
            lstProcesses_SelectedIndexChanged(sender, e);
            lstFavorites_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// Cleans up when the application exits (main form closes)
        /// </summary>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // Not allowed to exit the application if we've hidden the Windows taskbar.
            //
            // Make them exit the game that triggered the taskbar to be hidden -- or -- use a global hotkey to restore it.
            if (Manipulation.WindowsTaskbarIsHidden)
            {
                _closingFromExitMenu = false;
                e.Cancel = true;
                return;
            }

            // If we're exiting -- or -- if we're closing-to-tray, then restore the mouse cursor.
            //
            // This prevents a scenario where the user can't (easily) get back to Borderless Gaming to undo the hidden mouse cursor.
            Manipulation.ToggleMouseCursorVisibility(this, Boolstate.True);

            // If the user didn't choose to exit from the tray icon context menu...
            if (!_closingFromExitMenu)
            {
                // ... and they have the preference set to close-to-tray ...
                if (SettingsWrapper.Instance.Settings.CloseToTray is true)
                {
                    // ... then minimize the app and do not exit (minimizing will trigger another event to hide the form)
                    WindowState = WindowState.Minimized;
                    e.Cancel = true;
                    return;
                }
            }

            // At this point, we're okay to exit the application

            // Unregister all global hotkeys
            UnregisterHotkeys();

            // Hide the tray icon.  If we don't do this, then Environment.Exit() can sometimes ghost the icon in the
            // Windows system tray area.
            this.ViewModel.TrayIconVisibility = Visibility.Hidden;

            // Overkill... the form should just close naturally.  Ideally we would just allow the form to close and
            // the remaining code in Program.cs would execute (if there were any), but this is how Borderless Gaming has
            // always exited and there may be a compatibility reason for it, so leaving it alone for now.
            Environment.Exit(0);
        }

        private void addSelectedItem_MouseHover(object sender, EventArgs e)
        {
            var ttTemp = new ToolTip();
            ttTemp.Content = LanguageManager.Data("addFavorite");
        }

        private void btnRemoveFavorite_MouseHover(object sender, EventArgs e)
        {
            var ttTemp = new ToolTip();
            ttTemp.Content = LanguageManager.Data("removeFavorite");
        }

        private void btnMakeBorderless_MouseHover(object sender, EventArgs e)
        {
            var ttTemp = new ToolTip();
            ttTemp.Content = LanguageManager.Data("makeBorderless");
        }

        private void btnRestoreWindow_MouseHover(object sender, EventArgs e)
        {
            var ttTemp = new ToolTip();
            ttTemp.Content = LanguageManager.Data("restoreBorders");
        }

        #endregion

        #region Tray Icon Events

        private bool _closingFromExitMenu;
        private readonly ProcessWatcher _watcher;
        internal ProcessWatcher Watcher => this._watcher;
        internal bool ClosingFromExitMenu { get; set; }

        private void Exit_MenuItem_Click(object sender, EventArgs e)
        {
            _closingFromExitMenu = true;
            Close();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.ViewModel.TrayIconVisibility = Visibility.Visible;

                if (SettingsWrapper.Instance.Settings.HideBalloonTips is true && SettingsWrapper.Instance.StartupOptions.Silent is false)
                {
                    // Display a balloon tooltip message for 2 seconds
                    this.TrayIcon.ShowBalloonTip(this.Title, string.Format(BorderlessGaming.Properties.Resources.TrayMinimized, "Borderless Gaming"), BalloonIcon.Info);
                    // original timeout 2000
                }

                if (!Manipulation.WindowsTaskbarIsHidden)
                {
                    Hide();
                }
            }
        }

        #endregion

        #region Global HotKeys

        /// <summary>
        /// registers the global hotkeys
        /// </summary>
        internal void RegisterHotkeys()
        {
            UnregisterHotkeys();

            if (SettingsWrapper.Instance.Settings.UseGlobalHotkey is true)
            {
                PInvoke.RegisterHotKey(this.Handle, GetType().GetHashCode(), (HOT_KEY_MODIFIERS)this.ViewModel.MakeBorderlessHotKeyModifier, this.ViewModel.MakeBorderlessHotKey);
            }

            if (SettingsWrapper.Instance.Settings.UseMouseLockHotKey is true)
            {
                PInvoke.RegisterHotKey(this.Handle, GetType().GetHashCode(), (HOT_KEY_MODIFIERS)0, this.ViewModel.MouseLockHotKey);
            }

            if (SettingsWrapper.Instance.Settings.UseMouseHideHotKey is true)
            {
                PInvoke.RegisterHotKey(this.Handle, GetType().GetHashCode(), (HOT_KEY_MODIFIERS)this.ViewModel.MouseHideHotKeyModifier, this.ViewModel.MouseHideHotKey);
            }
        }

        /// <summary>
        /// unregisters the global hotkeys
        /// </summary>
        private void UnregisterHotkeys()
        {
            PInvoke.UnregisterHotKey(this.Handle, GetType().GetHashCode());
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource? source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(WndProc);
        }

        /// <summary>
        /// Catches the Hotkeys
        /// </summary>
        protected IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == PInvoke.WM_HOTKEY)
            {
                var keystroke = ((uint) lParam >> 16) & 0x0000FFFF;
                var keystrokeModifier = (uint) lParam & 0x0000FFFF;

                // Global hotkey to make a window borderless
                if (keystroke == this.ViewModel.MakeBorderlessHotKey && keystrokeModifier == this.ViewModel.MakeBorderlessHotKeyModifier)
                {
                    // Find the currently-active window
                    var hCurrentActiveWindow = PInvoke.GetForegroundWindow();

                    // Only if that window isn't Borderless Windows itself
                    if (hCurrentActiveWindow != this.Handle)
                    {
                        // Figure out the process details based on the current window handle
                        var pd = _watcher.FromHandle(hCurrentActiveWindow);
                        if (pd == null)
                        {
                            Task.WaitAll(_watcher.Refresh());
                            pd = _watcher.FromHandle(hCurrentActiveWindow);
                            if (pd == null)
                            {
                                return IntPtr.Zero;
                            }
                        }
                        // If we have information about this process -and- we've already made it borderless, then reverse the process
                        if (pd.MadeBorderless)
                        {
                            Manipulation.RestoreWindow(pd);
                        }
                        // Otherwise, this is a fresh request to remove the border from the current window
                        else
                        {
                             _watcher.RemoveBorder(pd).GetAwaiter().GetResult();
                        }
                    }

                    return IntPtr.Zero; // handled the message, do not call base WndProc for this message
                }

                if (keystroke == this.ViewModel.MouseHideHotKey && keystrokeModifier == this.ViewModel.MouseHideHotKeyModifier)
                {
                    Manipulation.ToggleMouseCursorVisibility(this);

                    return IntPtr.Zero; // handled the message, do not call base WndProc for this message
                }

                if (keystroke == this.ViewModel.MouseLockHotKey && keystrokeModifier == 0)
                {
                    var hWnd = PInvoke.GetForegroundWindow();

                    // get size of client area
                    PInvoke.GetClientRect(hWnd, out RECT rect);

                    // get top,left point of client area
                    var p = new System.Drawing.Point { X = 0, Y = 0 };
                    PInvoke.ClientToScreen(hWnd, ref p);

                    var clipRect = new RECT(p.X, p.Y, rect.right - rect.left, rect.bottom - rect.top);
                    
                    Cursor.SetClip(Cursor.GetClip().Equals(clipRect) ? new RECT() : clipRect);

                    return IntPtr.Zero; // handled the message, do not call base WndProc for this message
                }
            }

            return IntPtr.Zero;
        }

        #endregion

        private void muteInBackground_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }
            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });
            fav.MuteInBackground = this.ViewModel.MuteInBackgroundIsChecked;
            if (!fav.MuteInBackground)
            {
                if (fav.IsRunning && Native.IsMuted(fav.RunningId))
                {
                    Native.UnMuteProcess(fav.RunningId);
                }
            } else if (fav.MuteInBackground)
            {
                if (fav.IsRunning && !Native.IsMuted(fav.RunningId))
                {
                    Native.MuteProcess(fav.RunningId);
                }
            }
            RefreshFavoritesList(fav);
        }

        private void editRegex_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }
            if (fav.Type == FavoriteType.Regex)
            {
                string? newRegex = InputText(LanguageManager.Data("setNewRegexTitle"), LanguageManager.Data("setNewRegexPrompt"), fav.SearchText);
                fav.SearchText = newRegex ?? string.Empty;
                RefreshFavoritesList(fav);
            }
        }

        private (int x, int y, int w, int h) InputSize(string sTitle, string xInstruction, string yInstruction, string wInstruction, string hInstruction, bool keepRatio, Favorite? fav = null, int xDefaultValue = 0, int yDefaultValue = 0, int wDefaultValue = 0, int hDefaultValue = 0)
        {
            Point? windowSize = null;
            try
            {
                keepRatio = keepRatio ? fav is not null : false;
                if (fav is not null) {
                    windowSize = fav.GetWindowSize();
                }
                using (var inputForm = new InputSizeWPF(keepRatio, windowSize))
                {
                    inputForm.SetTitle(sTitle);
                    inputForm.SetInstructionsX(xInstruction);
                    inputForm.SetInstructionsY(yInstruction);
                    inputForm.SetInstructionsW(wInstruction);
                    inputForm.SetInstructionsH(hInstruction);
                    inputForm.SetInputX(xDefaultValue);
                    inputForm.SetInputY(yDefaultValue);
                    inputForm.SetInputW(wDefaultValue);
                    inputForm.SetInputH(hDefaultValue);
                    if (inputForm.ShowDialog() == MessageBoxResult.Cancel)
                    {
                        return inputForm.GetCurrentValue();
                    }

                    return (xDefaultValue, yDefaultValue, wDefaultValue, hDefaultValue);
                }
            }
            catch
            {
                // ignored
            }

            return (0, 0, 0, 0);
        }

        private void toolStripSetSetWindowSizeKeepRatio_Click(object sender, EventArgs e)
        {
            if (this.ViewModel.FavoriteListBoxSelectedItem is not Favorite fav)
            {
                return;
            }

            var result =
                MessageBox.Show(
                   LanguageManager.Data("setWindowSizeMousePrompt"),
                   LanguageManager.Data("setWindowSizeMouseTitle"), MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
            {
                return;
            }

            if (result == MessageBoxResult.Yes)
            {
                using (var frmSelectArea = new DesktopAreaSelectorWPF(true, fav.GetWindowSize()))
                {
                    if (frmSelectArea.ShowDialog() != MessageBoxResult.OK)
                    {
                        return;
                    }

                    // Temporarily disable compiler warning CS1690: http://msdn.microsoft.com/en-us/library/x524dkh4.aspx
                    //
                    // We know what we're doing: everything is safe here.
#pragma warning disable 1690
                    var rect = frmSelectArea.GetCurrentValue();
                    fav.PositionX = rect.X;
                    fav.PositionY = rect.Y;
                    fav.PositionWidth = rect.Width;
                    fav.PositionHeight = rect.Height;
#pragma warning restore 1690
                }
            }
            else // System.Windows.Forms.DialogResult.No
            {
                var (favPositionX, favPositionY, favPositionW, favPositionH) = InputSize(LanguageManager.Data("setWindowSizeTitle"), string.Format(LanguageManager.Data("setWindowSizePixelPrompt"), "X"), string.Format(LanguageManager.Data("setWindowSizePixelPrompt"), "Y"), LanguageManager.Data("setWindowSizeWidthPrompt"), LanguageManager.Data("setWindowSizeHeightPrompt"), true, fav, fav.PositionX, fav.PositionY, fav.PositionWidth, fav.PositionHeight);
                fav.PositionX = favPositionX;
                fav.PositionHeight = favPositionH;
                fav.PositionWidth = favPositionW;
                fav.PositionY = favPositionY;
            }

            SettingsWrapper.Instance.RemoveFavorite(fav, () =>
            {
                this.ViewModel.FavoriteListBoxItemsSource.Remove(fav);
            });

            if (fav.PositionWidth == 0 || fav.PositionHeight == 0)
            {
                fav.Size = FavoriteSize.FullScreen;
            }
            else
            {
                fav.Size = FavoriteSize.SpecificSize;
                fav.ShouldMaximize = false;
            }
            RefreshFavoritesList(fav);
        }
    }
}
