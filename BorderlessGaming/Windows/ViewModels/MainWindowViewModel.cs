#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using BorderlessGaming.Forms;
using BorderlessGaming.Logic.Core;
using BorderlessGaming.Logic.Misc;
using BorderlessGaming.Logic.Models;
using BorderlessGaming.Logic.NekoBoiNick;
using BorderlessGaming.Properties;

namespace BorderlessGaming.Windows.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private MainWindowWPF? Parent { get; }

        /// <summary>
        /// The Borderless Toggle hotKey
        /// </summary>
        private int MakeBorderlessHotKey { get; } = (int) Key.F6;

        /// <summary>
        /// The Borderless Toggle hotKey modifier
        /// </summary>
        private int MakeBorderlessHotKeyModifier { get; } = (int) Key.LWin;

        /// <summary>
        /// The Mouse Lock hotKey
        /// </summary>
        private int MouseLockHotKey { get; } = (int) Key.Scroll;

        /// <summary>
        /// The Mouse Hide hotkey
        /// </summary>
        private int MouseHideHotKey { get; } = (int) Key.Scroll;

        /// <summary>
        /// The Mouse Hide hotkey modifier
        /// </summary>
        private int MouseHideHotKeyModifier { get; } = (int) Key.LWin;

        public MainWindowViewModel(MainWindowWPF parent)
        {
            this.Watcher = new ProcessWatcher(this);
            this.Parent = parent;
            this.FavoiteButtonIcon = Resources.add.GetBitmapSource();
            this.UnfavoiteButtonIcon = Resources.remove.GetBitmapSource();
            this.MakeBordelessButtonIcon = Resources.borderless.GetBitmapSource();
            this.RestoreWindowButtonIcon = Resources.bordered.GetBitmapSource();
            this.RunOnWindowsStartupMenuItemClicked = new CommandImpl<bool>(OnRunOnWindowsStartupMenuItemClicked);
            this.CheckForUpdatesMenuItemClicked = new CommandImpl<bool>(OnCheckForUpdatesMenuItemClicked);
            this.UseGlobalHotkeyMenuItemClicked = new CommandImpl<bool>(OnUseGlobalHotkeyMenuItemClicked);
            this.UseMouseLockHotkeyMenuItemClicked = new CommandImpl<bool>(OnUseMouseLockHotkeyMenuItemClicked);
            this.UseMouseHideHotkeyMenuItemClicked = new CommandImpl<bool>(OnUseMouseHideHotkeyMenuItemClicked);
            this.StartMinimizedToTrayMenuItemClicked = new CommandImpl<bool>(OnStartMinimizedToTrayMenuItemClicked);
            this.CloseToTrayMenuItemClicked = new CommandImpl<bool>(OnCloseToTrayMenuItemClicked);
            this.HideBalloonTipsMenuItemClicked = new CommandImpl<bool>(OnHideBalloonTipsMenuItemClicked);
            this.UseSlowerWindowDetectionMenuItemClicked = new CommandImpl<bool>(OnUseSlowerWindowDetectionMenuItemClicked);
            this.ViewFullProcessDetailsMenuItemClicked = new AsyncCommandImpl<bool>(OnViewFullProcessDetailsMenuItemClicked);
            this.ExitMenuItemClicked = new CommandImpl(OnExitMenuItemClicked);
            this.PauseAutomaticProcessingMenuItemClicked = new CommandImpl<bool>(OnPauseAutomaticProcessingMenuItemClicked);
            this.OpenDataFolderMenuItemClicked = new CommandImpl(OnOpenDataFolderMenuItemClicked);
            this.ToggleMouseCursorVisibilityMenuItemClicked = new CommandImpl<bool>(OnToggleMouseCursorVisibilityMenuItemClicked);
            this.ToggleWindowsTaskbarVisibilityMenuItemClicked = new CommandImpl<bool>(OnToggleWindowsTaskbarVisibilityMenuItemClicked);
            this.FullApplicationRefreshMenuItemClicked = new AsyncCommandImpl(OnFullApplicationRefreshMenuItemClicked);
            this.UsageGuideMenuItemClicked = new CommandImpl(OnUsageGuideMenuItemClicked);
            this.RegexReferenceMenuItemClicked = new CommandImpl(OnRegexReferenceMenuItemClicked);
            this.ReportABugMenuItemClicked = new CommandImpl(OnReportABugMenuItemClicked);
            this.SupportUsMenuItemClicked = new CommandImpl(OnSupportUsMenuItemClicked);
            this.AboutMenuItemClicked = new CommandImpl(OnAboutMenuItemClicked);
            this.FavoriteButtonClicked = new AsyncCommandImpl<string>(OnFavoriteButtonClicked);
            this.UnfavoriteButtonClicked = new AsyncCommandImpl<string>(OnUnfavoriteButtonClicked);
            this.MakeBordelessButtonClicked = new CommandImpl<string>(OnMakeBordelessButtonClicked);
            this.RestoreWindowButtonClicked = new CommandImpl<string>(OnRestoreWindowButtonClicked);
            this.SetLanguageButonClicked = new CommandImpl<string>(OnSetLanguageButtonClicked);
            LanguageManager.Setup(this.LanguagesMenuItemContextMenuItems, this.SetLanguageButonClicked);
        }

#pragma warning disable CS8618
        public MainWindowViewModel() { }
#pragma warning restore CS8618

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged<T>(ref T property, T value, [CallerMemberName] string callerName = "")
        {
            if (property is not null || value is not null || property?.Equals(value) == true)
            {
                property = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerName));
            }
        }

        #endregion INotifyPropertyChanged Implementation

        #region Bindings

        #region Localization Bindings

        public string Title { get; } = "Borderless Gaming";
        public string OptionsButtonLabel { get; } = "_Options";
        public string ToolsButtonLabel { get; } = "_Tools";
        public string HelpButtonLabel { get; } = "_Help";
        public string RunOnWindowsStartupMenuItemLabel { get; } = "Run On Windows Startup";
        public string LanguagesMenuItemLabel { get; } = "Languages";
        public string CheckForUpdatesMenuItemLabel { get; } = "Check For Updates";
        public string UseGlobalHotkeyMenuItemLabel { get; } = "Use Global Hotkey";
        public string UseMouseLockHotkeyMenuItemLabel { get; } = "Use Mouse Lock Hotkey";
        public string UseMouseHideHotkeyMenuItemLabel { get; } = "Use Mouse Hide Hotkey";
        public string StartMinimizedToTrayMenuItemLabel { get; } = "Start Minimized To Tray";
        public string CloseToTrayMenuItemLabel { get; } = "Close To Tray";
        public string HideBalloonTipsMenuItemLabel { get; } = "Hide Balloon Tips";
        public string UseSlowerWindowDetectionMenuItemLabel { get; } = "Use Slower Window Detection";
        public string ViewFullProcessDetailsMenuItemLabel { get; } = "View Full Process Details";
        public string ExitMenuItemLabel { get; } = "Exit";
        public string PauseAutomaticProcessingMenuItemLabel { get; } = "Pause Automatic Processing";
        public string OpenDataFolderMenuItemLabel { get; } = "Open Data Folder";
        public string ToggleMouseCursorVisibilityMenuItemLabel { get; } = "Toggle Mouse Cursor Visibility";
        public string ToggleWindowsTaskbarVisibilityMenuItemLabel { get; } = "Toggle Windows Taskbar Visibility";
        public string FullApplicationRefreshMenuItemLabel { get; } = "Full Application Refresh";
        public string UsageGuideMenuItemLabel { get; } = "Usage Guide";
        public string RegexReferenceMenuItemLabel { get; } = "Regex Reference";
        public string ReportABugMenuItemLabel { get; } = "Report A Bug";
        public string SupportUsMenuItemLabel { get; } = "Support Us";
        public string AboutMenuItemLabel { get; } = "About";
        public string ApplicationsLabel { get; } = "Applications";
        public string FavoritesAutomaticLabel { get; } = "Favorites (Automatic)";

        #endregion Localization Bindings

        #region Icon Bindings

        private BitmapSource? favoiteButtonIcon;
        public BitmapSource? FavoiteButtonIcon
        {
            get => favoiteButtonIcon;
            set => NotifyPropertyChanged<BitmapSource?>(ref favoiteButtonIcon, value);
        }

        private BitmapSource? unfavoiteButtonIcon;
        public BitmapSource? UnfavoiteButtonIcon
        {
            get => unfavoiteButtonIcon;
            set => NotifyPropertyChanged<BitmapSource?>(ref unfavoiteButtonIcon, value);
        }

        private BitmapSource? makeBordelessButtonIcon;
        public BitmapSource? MakeBordelessButtonIcon
        {
            get => makeBordelessButtonIcon;
            set => NotifyPropertyChanged<BitmapSource?>(ref makeBordelessButtonIcon, value);
        }

        private BitmapSource? restoreWindowButtonIcon;
        public BitmapSource? RestoreWindowButtonIcon
        {
            get => restoreWindowButtonIcon;
            set => NotifyPropertyChanged<BitmapSource?>(ref restoreWindowButtonIcon, value);
        }

        #endregion Icon Bindings

        #region Value Bindings

        private List<MenuItem> languagesMenuItemContextMenuItems = [];
        public List<MenuItem> LanguagesMenuItemContextMenuItems
        {
            get => languagesMenuItemContextMenuItems;
            set => NotifyPropertyChanged<List<MenuItem>>(ref languagesMenuItemContextMenuItems, value);
        }

        private List<ListBoxItem> applicationsListBoxItemsSource = [];
        public List<ListBoxItem> ApplicationsListBoxItemsSource
        {
            get => applicationsListBoxItemsSource;
            set => NotifyPropertyChanged<List<ListBoxItem>>(ref applicationsListBoxItemsSource, value);
        }

        private List<ListBoxItem> favoriteListBoxItemsSource = [];
        public List<ListBoxItem> FavoriteListBoxItemsSource
        {
            get => favoriteListBoxItemsSource;
            set => NotifyPropertyChanged<List<ListBoxItem>>(ref favoriteListBoxItemsSource, value);
        }

        private string statusLabel = "Loading...";
        public string StatusLabel
        {
            get => statusLabel;
            set => NotifyPropertyChanged<string>(ref statusLabel, value);
        }

        #endregion Value Bindings

        #region Command Bindings

        public ICommand RunOnWindowsStartupMenuItemClicked { get; }
        public ICommand CheckForUpdatesMenuItemClicked { get; }
        public ICommand UseGlobalHotkeyMenuItemClicked { get; }
        public ICommand UseMouseLockHotkeyMenuItemClicked { get; }
        public ICommand UseMouseHideHotkeyMenuItemClicked { get; }
        public ICommand StartMinimizedToTrayMenuItemClicked { get; }
        public ICommand CloseToTrayMenuItemClicked { get; }
        public ICommand HideBalloonTipsMenuItemClicked { get; }
        public ICommand UseSlowerWindowDetectionMenuItemClicked { get; }
        public ICommand ViewFullProcessDetailsMenuItemClicked { get; }
        public ICommand ExitMenuItemClicked { get; }
        public ICommand PauseAutomaticProcessingMenuItemClicked { get; }
        public ICommand OpenDataFolderMenuItemClicked { get; }
        public ICommand ToggleMouseCursorVisibilityMenuItemClicked { get; }
        public ICommand ToggleWindowsTaskbarVisibilityMenuItemClicked { get; }
        public ICommand FullApplicationRefreshMenuItemClicked { get; }
        public ICommand UsageGuideMenuItemClicked { get; }
        public ICommand RegexReferenceMenuItemClicked { get; }
        public ICommand ReportABugMenuItemClicked { get; }
        public ICommand SupportUsMenuItemClicked { get; }
        public ICommand AboutMenuItemClicked { get; }
        public ICommand FavoriteButtonClicked { get; }
        public ICommand UnfavoriteButtonClicked { get; }
        public ICommand MakeBordelessButtonClicked { get; }
        public ICommand RestoreWindowButtonClicked { get; }
        public ICommand SetLanguageButonClicked { get; }

        #endregion Command Bindings

        #endregion Bindings

        #region Methods

        #region Event Methods

        private void OnRunOnWindowsStartupMenuItemClicked(bool value)
        {
            AutoStart.Setup(value, "--silent --minimize");
            SettingsWrapper.Instance.Settings.RunOnStartup = value;
            SettingsWrapper.Save();
        }

        private void OnCheckForUpdatesMenuItemClicked(bool value)
        {
            SettingsWrapper.Instance.Settings.CheckForUpdates = value;
            SettingsWrapper.Save();
        }

        private void OnUseGlobalHotkeyMenuItemClicked(bool value)
        {
            SettingsWrapper.Instance.Settings.UseGlobalHotkey = value;
            SettingsWrapper.Save();
            RegisterHotkeys();
        }

        private void OnUseMouseLockHotkeyMenuItemClicked(bool value)
        {
            SettingsWrapper.Instance.Settings.UseMouseLockHotKey = value;
            SettingsWrapper.Save();
            RegisterHotkeys();
        }

        private void OnUseMouseHideHotkeyMenuItemClicked(bool value)
        {
            SettingsWrapper.Instance.Settings.UseMouseHideHotKey = value;
            SettingsWrapper.Save();
            RegisterHotkeys();
        }

        private void OnStartMinimizedToTrayMenuItemClicked(bool value)
        {
            SettingsWrapper.Instance.Settings.StartMinimized = value;
            SettingsWrapper.Save();
        }

        private void OnCloseToTrayMenuItemClicked(bool value)
        {
            SettingsWrapper.Instance.Settings.CloseToTray = value;
            SettingsWrapper.Save();
        }

        private void OnHideBalloonTipsMenuItemClicked(bool value)
        {
            SettingsWrapper.Instance.Settings.HideBalloonTips = value;
            SettingsWrapper.Save();
        }

        private void OnUseSlowerWindowDetectionMenuItemClicked(bool value)
        {
            SettingsWrapper.Instance.Settings.SlowWindowDetection = value;
            SettingsWrapper.Save();
        }

        private async Task OnViewFullProcessDetailsMenuItemClicked(bool value)
        {
            SettingsWrapper.Instance.Settings.ViewAllProcessDetails = value;
            SettingsWrapper.Save();
            await RefreshProcesses();
        }

        private void OnExitMenuItemClicked()
        {
            _closingFromExitMenu = true;
            this.Parent?.Close();
        }

        private void OnPauseAutomaticProcessingMenuItemClicked(bool value)
        {
            _watcher.AutoHandleFavorites = false;
        }

        private void OnOpenDataFolderMenuItemClicked()
        {
            try
            {
                Process.Start(new ProcessStartInfo("explorer.exe", "/e,\"" + AppEnvironment.DataPath + "\",\"" + AppEnvironment.DataPath + "\""));
            }
            catch
            {
                // ignored
            }
        }

        private void OnToggleMouseCursorVisibilityMenuItemClicked(bool value)
        {
            if (Manipulation.MouseCursorIsHidden ||
                MessageBox.Show(
                    LanguageManager.Data("toggleMouseCursorVisibilityPrompt"),
                    LanguageManager.Data("toggleMouseCursorVisibilityTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Manipulation.ToggleMouseCursorVisibility(this);
            }
        }

        private void OnToggleWindowsTaskbarVisibilityMenuItemClicked(bool value)
        {
            Manipulation.ToggleWindowsTaskbarVisibility();
        }

        private async Task OnResetHiddenProcessesMenuItemClicked()
        {
            SettingsWrapper.Instance.ResetHiddenProcesses();
            await RefreshProcesses();
        }

        private async Task OnFullApplicationRefreshMenuItemClicked()
        {
            await RefreshProcesses();
        }

        private void OnUsageGuideMenuItemClicked()
        {
            Tools.GotoSite("http://steamcommunity.com/app/388080/discussions/0/535151589899658778/");
        }

        private void OnRegexReferenceMenuItemClicked()
        {
            Tools.GotoSite("www.regular-expressions.info/reference.html");
        }

        private void OnReportABugMenuItemClicked()
        {
            Tools.GotoSite("https://github.com/Codeusa/Borderless-Gaming/issues");
        }

        private void OnSupportUsMenuItemClicked()
        {
            Tools.GotoSite("http://store.steampowered.com/app/388080");
        }

        private void OnAboutMenuItemClicked()
        {
            new AboutWPF().ShowDialog();
        }

        private void OnFavoriteButtonClicked(Favorite? fav)
        {
            this.FavoriteListBoxItemsSource.Remove(fav);
        }

        private void OnUnfavoriteButtonClicked(Favorite? fav)
        {
            this.FavoriteListBoxItemsSource.Remove(fav);
        }

        private void OnMakeBordelessButtonClicked(string? application)
        {

        }

        private void OnRestoreWindowButtonClicked(string? application)
        {

        }

        private void OnSetLanguageButtonClicked(string? lang)
        {
            foreach (MenuItem tt in this.LanguagesMenuItemContextMenuItems)
            {
                if (tt.Header.Equals(lang) && !tt.IsChecked)
                {
                    if (LanguageManager.IsDefault(tt.Header as string) && !LanguageManager.LanguageSelected(this.LanguagesMenuItemContextMenuItems))
                    {
                        tt.IsChecked = true;
                    }
                    return;
                }
                else if (!tt.Header.Equals(lang) && tt.IsChecked)
                {
                    tt.IsChecked = false;
                }
                LanguageManager.SetDefaultLanguage(tt.Header as string);
            }
        }

        #endregion Event Methods

        #endregion Methods
    }
}
