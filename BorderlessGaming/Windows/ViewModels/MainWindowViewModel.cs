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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using BorderlessGaming.Forms;
using BorderlessGaming.Logic.Core;
using BorderlessGaming.Logic.Misc;
using BorderlessGaming.Logic.Models;
using BorderlessGaming.Logic.NekoBoiNick;
using BorderlessGaming.Logic.Windows;
using Manipulation = BorderlessGaming.Logic.Windows.Manipulation;
using BorderlessGaming.Properties;
using BorderlessGaming.Windows.Components;

namespace BorderlessGaming.Windows.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private MainWindowWPF? Parent { get; }

        /// <summary>
        /// The Borderless Toggle hotKey
        /// </summary>
        internal uint MakeBorderlessHotKey { get; } = (uint) Key.F6;

        /// <summary>
        /// The Borderless Toggle hotKey modifier
        /// </summary>
        internal int MakeBorderlessHotKeyModifier { get; } = (int) Key.LWin;

        /// <summary>
        /// The Mouse Lock hotKey
        /// </summary>
        internal uint MouseLockHotKey { get; } = (uint) Key.Scroll;

        /// <summary>
        /// The Mouse Hide hotkey
        /// </summary>
        internal uint MouseHideHotKey { get; } = (uint) Key.Scroll;

        /// <summary>
        /// The Mouse Hide hotkey modifier
        /// </summary>
        internal int MouseHideHotKeyModifier { get; } = (int) Key.LWin;

        public MainWindowViewModel(MainWindowWPF parent)
        {
            this.Parent = parent;
            this.FavoriteButtonIcon = Resources.add.GetBitmapSource();
            this.UnfavoriteButtonIcon = Resources.remove.GetBitmapSource();
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
            this.FavoriteButtonClicked = new CommandImpl<Favorite>(OnFavoriteButtonClicked);
            this.UnfavoriteButtonClicked = new CommandImpl<Favorite>(OnUnfavoriteButtonClicked);
            this.MakeBordelessButtonClicked = new CommandImpl<string>(OnMakeBordelessButtonClicked);
            this.RestoreWindowButtonClicked = new CommandImpl<string>(OnRestoreWindowButtonClicked);
            this.SetLanguageButonClicked = new CommandImpl<string>(OnSetLanguageButtonClicked);
            this.TrayIconDoubleClickCommand = new CommandImpl(TrayIconDoubleClicked);
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

        private string? title;
        public string? Title { // "Borderless Gaming";
            get => title;
            set => this.NotifyPropertyChanged<string?>(ref title, value);
        }

        private string? optionsButtonLabel;
        public string? OptionsButtonLabel { // "_Options";
            get => optionsButtonLabel;
            set => this.NotifyPropertyChanged<string?>(ref optionsButtonLabel, value);
        }

        private string? toolsButtonLabel;
        public string? ToolsButtonLabel { // "_Tools";
            get => toolsButtonLabel;
            set => this.NotifyPropertyChanged<string?>(ref toolsButtonLabel, value);
        }

        private string? helpButtonLabel;
        public string? HelpButtonLabel { // "_Help";
            get => helpButtonLabel;
            set => this.NotifyPropertyChanged<string?>(ref helpButtonLabel, value);
        }

        private string? runOnWindowsStartupMenuItemLabel;
        public string? RunOnWindowsStartupMenuItemLabel { // "Run On Windows Startup";
            get => runOnWindowsStartupMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref runOnWindowsStartupMenuItemLabel, value);
        }

        private string? languagesMenuItemLabel;
        public string? LanguagesMenuItemLabel { // "Languages";
            get => languagesMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref languagesMenuItemLabel, value);
        }

        private string? checkForUpdatesMenuItemLabel;
        public string? CheckForUpdatesMenuItemLabel { // "Check For Updates";
            get => checkForUpdatesMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref checkForUpdatesMenuItemLabel, value);
        }

        private string? useGlobalHotkeyMenuItemLabel;
        public string? UseGlobalHotkeyMenuItemLabel { // "Use Global Hotkey";
            get => useGlobalHotkeyMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref useGlobalHotkeyMenuItemLabel, value);
        }

        private string? useMouseLockHotkeyMenuItemLabel;
        public string? UseMouseLockHotkeyMenuItemLabel { // "Use Mouse Lock Hotkey";
            get => useMouseLockHotkeyMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref useMouseLockHotkeyMenuItemLabel, value);
        }

        private string? useMouseHideHotkeyMenuItemLabel;
        public string? UseMouseHideHotkeyMenuItemLabel { // "Use Mouse Hide Hotkey";
            get => useMouseHideHotkeyMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref useMouseHideHotkeyMenuItemLabel, value);
        }

        private string? startMinimizedToTrayMenuItemLabel;
        public string? StartMinimizedToTrayMenuItemLabel { // "Start Minimized To Tray";
            get => startMinimizedToTrayMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref startMinimizedToTrayMenuItemLabel, value);
        }

        private string? closeToTrayMenuItemLabel;
        public string? CloseToTrayMenuItemLabel { // "Close To Tray";
            get => closeToTrayMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref closeToTrayMenuItemLabel, value);
        }

        private string? hideBalloonTipsMenuItemLabel;
        public string? HideBalloonTipsMenuItemLabel { // "Hide Balloon Tips";
            get => hideBalloonTipsMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref hideBalloonTipsMenuItemLabel, value);
        }

        private string? useSlowerWindowDetectionMenuItemLabel;
        public string? UseSlowerWindowDetectionMenuItemLabel { // "Use Slower Window Detection";
            get => useSlowerWindowDetectionMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref useSlowerWindowDetectionMenuItemLabel, value);
        }

        private string? viewFullProcessDetailsMenuItemLabel;
        public string? ViewFullProcessDetailsMenuItemLabel { // "View Full Process Details";
            get => viewFullProcessDetailsMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref viewFullProcessDetailsMenuItemLabel, value);
        }

        private string? exitMenuItemLabel;
        public string? ExitMenuItemLabel { // "Exit";
            get => exitMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref exitMenuItemLabel, value);
        }

        private string? pauseAutomaticProcessingMenuItemLabel;
        public string? PauseAutomaticProcessingMenuItemLabel { // "Pause Automatic Processing";
            get => pauseAutomaticProcessingMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref pauseAutomaticProcessingMenuItemLabel, value);
        }

        private string? openDataFolderMenuItemLabel;
        public string? OpenDataFolderMenuItemLabel { // "Open Data Folder";
            get => openDataFolderMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref openDataFolderMenuItemLabel, value);
        }

        private string? toggleMouseCursorVisibilityMenuItemLabel;
        public string? ToggleMouseCursorVisibilityMenuItemLabel { // "Toggle Mouse Cursor Visibility";
            get => toggleMouseCursorVisibilityMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref toggleMouseCursorVisibilityMenuItemLabel, value);
        }

        private string? toggleWindowsTaskbarVisibilityMenuItemLabel;
        public string? ToggleWindowsTaskbarVisibilityMenuItemLabel { // "Toggle Windows Taskbar Visibility";
            get => toggleWindowsTaskbarVisibilityMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref toggleWindowsTaskbarVisibilityMenuItemLabel, value);
        }

        private string? fullApplicationRefreshMenuItemLabel;
        public string? FullApplicationRefreshMenuItemLabel { // "Full Application Refresh";
            get => fullApplicationRefreshMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref fullApplicationRefreshMenuItemLabel, value);
        }

        private string? usageGuideMenuItemLabel;
        public string? UsageGuideMenuItemLabel { // "Usage Guide";
            get => usageGuideMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref usageGuideMenuItemLabel, value);
        }

        private string? regexReferenceMenuItemLabel;
        public string? RegexReferenceMenuItemLabel { // "Regex Reference";
            get => regexReferenceMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref regexReferenceMenuItemLabel, value);
        }

        private string? reportABugMenuItemLabel;
        public string? ReportABugMenuItemLabel { // "Report A Bug";
            get => reportABugMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref reportABugMenuItemLabel, value);
        }

        private string? supportUsMenuItemLabel;
        public string? SupportUsMenuItemLabel { // "Support Us";
            get => supportUsMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref supportUsMenuItemLabel, value);
        }

        private string? aboutMenuItemLabel;
        public string? AboutMenuItemLabel { // "About";
            get => aboutMenuItemLabel;
            set => this.NotifyPropertyChanged<string?>(ref aboutMenuItemLabel, value);
        }

        private string? applicationsLabel;
        public string? ApplicationsLabel { // "Applications";
            get => applicationsLabel;
            set => this.NotifyPropertyChanged<string?>(ref applicationsLabel, value);
        }

        private string? favoritesAutomaticLabel;
        public string? FavoritesAutomaticLabel { // "Favorites (Automatic)";
            get => favoritesAutomaticLabel;
            set => this.NotifyPropertyChanged<string?>(ref favoritesAutomaticLabel, value);
        }

        #endregion Localization Bindings

        #region Icon Bindings

        private BitmapSource? favoriteButtonIcon;
        public BitmapSource? FavoriteButtonIcon
        {
            get => favoriteButtonIcon;
            set => NotifyPropertyChanged<BitmapSource?>(ref favoriteButtonIcon, value);
        }

        private BitmapSource? unfavoriteButtonIcon;
        public BitmapSource? UnfavoriteButtonIcon
        {
            get => unfavoriteButtonIcon;
            set => NotifyPropertyChanged<BitmapSource?>(ref unfavoriteButtonIcon, value);
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

        private ProcessDetails? applicationsListBoxSelectedItem;
        public ProcessDetails? ApplicationsListBoxSelectedItem
        {
            get => applicationsListBoxSelectedItem;
            set => NotifyPropertyChanged<ProcessDetails?>(ref applicationsListBoxSelectedItem, value);
        }

        private List<ProcessDetails> applicationsListBoxItemsSource = [];
        public List<ProcessDetails> ApplicationsListBoxItemsSource
        {
            get => applicationsListBoxItemsSource;
            set => NotifyPropertyChanged<List<ProcessDetails>>(ref applicationsListBoxItemsSource, value);
        }

        private Favorite? favoriteListBoxSelectedItem;
        public Favorite? FavoriteListBoxSelectedItem
        {
            get => favoriteListBoxSelectedItem;
            set => NotifyPropertyChanged<Favorite?>(ref favoriteListBoxSelectedItem, value);
        }

        private List<Favorite> favoriteListBoxItemsSource = [];
        public List<Favorite> FavoriteListBoxItemsSource
        {
            get => favoriteListBoxItemsSource;
            set => NotifyPropertyChanged<List<Favorite>>(ref favoriteListBoxItemsSource, value);
        }

        private List<MenuItem> favScreenDropDownItemsSource = [];
        public List<MenuItem> FavScreenDropDownItemsSource
        {
            get => favScreenDropDownItemsSource;
            set => NotifyPropertyChanged<List<MenuItem>>(ref favScreenDropDownItemsSource, value);
        }
        public bool FavScreenHasDropDownItems
        {
            get => this.FavScreenDropDownItemsSource.Any();
        }

        private string statusLabel = "Loading...";
        public string StatusLabel
        {
            get => statusLabel;
            set => NotifyPropertyChanged<string>(ref statusLabel, value);
        }

        private bool makeBorderlessButtonIsEnabled;
        public bool MakeBorderlessButtonIsEnabled
        {
            get => makeBorderlessButtonIsEnabled;
            set => NotifyPropertyChanged<bool>(ref makeBorderlessButtonIsEnabled, value);
        }

        private bool restoreWindowIsEnabled;
        public bool RestoreWindowIsEnabled
        {
            get => restoreWindowIsEnabled;
            set => NotifyPropertyChanged<bool>(ref restoreWindowIsEnabled, value);
        }

        private bool favoriteButtonIsEnabled;
        public bool FavoriteButtonIsEnabled
        {
            get => favoriteButtonIsEnabled;
            set => NotifyPropertyChanged<bool>(ref favoriteButtonIsEnabled, value);
        }

        private bool unfavoriteButtonIsEnabled;
        public bool UnfavoriteButtonIsEnabled
        {
            get => unfavoriteButtonIsEnabled;
            set => NotifyPropertyChanged<bool>(ref unfavoriteButtonIsEnabled, value);
        }

        private bool muteInBackgroundIsChecked;
        public bool MuteInBackgroundIsChecked
        {
            get => muteInBackgroundIsChecked;
            set => NotifyPropertyChanged<bool>(ref muteInBackgroundIsChecked, value);
        }

        private bool removeMenusIsChecked;
        public bool RemoveMenusIsChecked
        {
            get => removeMenusIsChecked;
            set => NotifyPropertyChanged<bool>(ref removeMenusIsChecked, value);
        }

        private bool alwaysOnTopIsChecked;
        public bool AlwaysOnTopIsChecked
        {
            get => alwaysOnTopIsChecked;
            set => NotifyPropertyChanged<bool>(ref alwaysOnTopIsChecked, value);
        }

        private bool autoMaximizeIsChecked;
        public bool AutoMaximizeIsChecked
        {
            get => autoMaximizeIsChecked;
            set => NotifyPropertyChanged<bool>(ref autoMaximizeIsChecked, value);
        }

        private bool hideMouseCursorIsChecked;
        public bool HideMouseCursorIsChecked
        {
            get => hideMouseCursorIsChecked;
            set => NotifyPropertyChanged<bool>(ref hideMouseCursorIsChecked, value);
        }

        private bool hideWindowsTaskbarIsChecked;
        public bool HideWindowsTaskbarIsChecked
        {
            get => hideWindowsTaskbarIsChecked;
            set => NotifyPropertyChanged<bool>(ref hideWindowsTaskbarIsChecked, value);
        }

        private bool favScreenIsVisible;
        public bool FavScreenIsVisible
        {
            get => favScreenIsVisible;
            set => NotifyPropertyChanged<bool>(ref favScreenIsVisible, value);
        }

        private bool setWindowSizeKeepRatioIsChecked;
        public bool SetWindowSizeKeepRatioIsChecked
        {
            get => setWindowSizeKeepRatioIsChecked;
            set => NotifyPropertyChanged<bool>(ref setWindowSizeKeepRatioIsChecked, value);
        }

        private bool editRegexIsVisible;
        public bool EditRegexIsVisible
        {
            get => editRegexIsVisible;
            set => NotifyPropertyChanged<bool>(ref editRegexIsVisible, value);
        }

        private bool editRegexIsEnabled;
        public bool EditRegexIsEnabled
        {
            get => editRegexIsEnabled;
            set => NotifyPropertyChanged<bool>(ref editRegexIsEnabled, value);
        }

        private bool noSizeChangeIsChecked;
        public bool NoSizeChangeIsChecked
        {
            get => noSizeChangeIsChecked;
            set => NotifyPropertyChanged<bool>(ref noSizeChangeIsChecked, value);
        }

        private bool setWindowSizeIsChecked;
        public bool SetWindowSizeIsChecked
        {
            get => setWindowSizeIsChecked;
            set => NotifyPropertyChanged<bool>(ref setWindowSizeIsChecked, value);
        }

        private bool setWindowSizeIsEnabled;
        public bool SetWindowSizeIsEnabled
        {
            get => setWindowSizeIsEnabled;
            set => NotifyPropertyChanged<bool>(ref setWindowSizeIsEnabled, value);
        }

        private bool adjustWindowBoundsIsEnabled;
        public bool AdjustWindowBoundsIsEnabled
        {
            get => adjustWindowBoundsIsEnabled;
            set => NotifyPropertyChanged<bool>(ref adjustWindowBoundsIsEnabled, value);
        }

        private bool autoMaximizeIsEnabled;
        public bool AutoMaximizeIsEnabled
        {
            get => autoMaximizeIsEnabled;
            set => NotifyPropertyChanged<bool>(ref autoMaximizeIsEnabled, value);
        }

        private bool fullScreenIsChecked;
        public bool FullScreenIsChecked
        {
            get => fullScreenIsChecked;
            set => NotifyPropertyChanged<bool>(ref fullScreenIsChecked, value);
        }

        private bool addToFavsIsEnabled;
        public bool AddToFavsIsEnabled
        {
            get => addToFavsIsEnabled;
            set => NotifyPropertyChanged<bool>(ref addToFavsIsEnabled, value);
        }

        private bool borderlessOnIsVisible;
        public bool BorderlessOnIsVisible
        {
            get => borderlessOnIsVisible;
            set => NotifyPropertyChanged<bool>(ref borderlessOnIsVisible, value);
        }

        private List<MenuItem> borderlessOnDropDownItemsSource = [];
        public List<MenuItem> BorderlessOnDropDownItemsSource
        {
            get => borderlessOnDropDownItemsSource;
            set => NotifyPropertyChanged<List<MenuItem>>(ref borderlessOnDropDownItemsSource, value);
        }
        public bool BorderlessOnHasDropDownItems
        {
            get => borderlessOnDropDownItemsSource.Any();
        }

        private List<MenuItem> menuItemDropDownItemsSource = [];
        public List<MenuItem> MenuItemDropDownItemsSource
        {
            get => menuItemDropDownItemsSource;
            set => NotifyPropertyChanged<List<MenuItem>>(ref menuItemDropDownItemsSource, value);
        }

        private bool delayBorderlessIsChecked;
        public bool DelayBorderlessIsChecked
        {
            get => delayBorderlessIsChecked;
            set => NotifyPropertyChanged<bool>(ref delayBorderlessIsChecked, value);
        }

        private bool runOnStartupIsChecked;
        public bool RunOnStartupIsChecked
        {
            get => runOnStartupIsChecked;
            set => NotifyPropertyChanged<bool>(ref runOnStartupIsChecked, value);
        }

        private bool globalHotkeyIsChecked;
        public bool GlobalHotkeyIsChecked
        {
            get => globalHotkeyIsChecked;
            set => NotifyPropertyChanged<bool>(ref globalHotkeyIsChecked, value);
        }

        private bool checkForUpdatesIsChecked;
        public bool CheckForUpdatesIsChecked
        {
            get => checkForUpdatesIsChecked;
            set => NotifyPropertyChanged<bool>(ref checkForUpdatesIsChecked, value);
        }

        private bool mouseLockIsChecked;
        public bool MouseLockIsChecked
        {
            get => mouseLockIsChecked;
            set => NotifyPropertyChanged<bool>(ref mouseLockIsChecked, value);
        }

        private bool mouseHideIsChecked;
        public bool MouseHideIsChecked
        {
            get => mouseHideIsChecked;
            set => NotifyPropertyChanged<bool>(ref mouseHideIsChecked, value);
        }

        private bool minimizedToTrayIsChecked;
        public bool MinimizedToTrayIsChecked
        {
            get => minimizedToTrayIsChecked;
            set => NotifyPropertyChanged<bool>(ref minimizedToTrayIsChecked, value);
        }

        private bool hideBalloonTipsIsChecked;
        public bool HideBalloonTipsIsChecked
        {
            get => hideBalloonTipsIsChecked;
            set => NotifyPropertyChanged<bool>(ref hideBalloonTipsIsChecked, value);
        }

        private bool closeToTrayIsChecked;
        public bool CloseToTrayIsChecked
        {
            get => closeToTrayIsChecked;
            set => NotifyPropertyChanged<bool>(ref closeToTrayIsChecked, value);
        }

        private bool viewFullProcessDetailsIsChecked;
        public bool ViewFullProcessDetailsIsChecked
        {
            get => viewFullProcessDetailsIsChecked;
            set => NotifyPropertyChanged<bool>(ref viewFullProcessDetailsIsChecked, value);
        }

        private bool slowWindowDetectionIsChecked;
        public bool SlowWindowDetectionIsChecked
        {
            get => slowWindowDetectionIsChecked;
            set => NotifyPropertyChanged<bool>(ref slowWindowDetectionIsChecked, value);
        }

        private Visibility trayIconVisibility = Visibility.Hidden;
        public Visibility TrayIconVisibility
        {
            get => trayIconVisibility;
            set => NotifyPropertyChanged<Visibility>(ref trayIconVisibility, value);
        }

        private string? trayIconToolTipText;
        public string? TrayIconToolTipText
        {
            get => trayIconToolTipText;
            set => NotifyPropertyChanged<string?>(ref trayIconToolTipText, value);
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
        public ICommand TrayIconDoubleClickCommand { get; }

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
            if (this.Parent is null) return;
            SettingsWrapper.Instance.Settings.UseGlobalHotkey = value;
            SettingsWrapper.Save();
            this.Parent.RegisterHotkeys();
        }

        private void OnUseMouseLockHotkeyMenuItemClicked(bool value)
        {
            if (this.Parent is null) return;
            SettingsWrapper.Instance.Settings.UseMouseLockHotKey = value;
            SettingsWrapper.Save();
            this.Parent.RegisterHotkeys();
        }

        private void OnUseMouseHideHotkeyMenuItemClicked(bool value)
        {
            if (this.Parent is null) return;
            SettingsWrapper.Instance.Settings.UseMouseHideHotKey = value;
            SettingsWrapper.Save();
            this.Parent.RegisterHotkeys();
        }

        private void OnStartMinimizedToTrayMenuItemClicked(bool value)
        {
            if (this.Parent is null) return;
            SettingsWrapper.Instance.Settings.StartMinimized = value;
            SettingsWrapper.Save();
        }

        private void OnCloseToTrayMenuItemClicked(bool value)
        {
            if (this.Parent is null) return;
            SettingsWrapper.Instance.Settings.CloseToTray = value;
            SettingsWrapper.Save();
        }

        private void OnHideBalloonTipsMenuItemClicked(bool value)
        {
            if (this.Parent is null) return;
            SettingsWrapper.Instance.Settings.HideBalloonTips = value;
            SettingsWrapper.Save();
        }

        private void OnUseSlowerWindowDetectionMenuItemClicked(bool value)
        {
            if (this.Parent is null) return;
            SettingsWrapper.Instance.Settings.SlowWindowDetection = value;
            SettingsWrapper.Save();
        }

        private async Task OnViewFullProcessDetailsMenuItemClicked(bool value)
        {
            if (this.Parent is null) return;
            SettingsWrapper.Instance.Settings.ViewAllProcessDetails = value;
            SettingsWrapper.Save();
            await this.Parent.RefreshProcesses();
        }

        private void OnExitMenuItemClicked()
        {
            if (this.Parent is null) return;
            this.Parent.ClosingFromExitMenu = true;
            this.Parent.Close();
        }

        private void OnPauseAutomaticProcessingMenuItemClicked(bool value)
        {
            if (this.Parent is null) return;
            this.Parent.Watcher.AutoHandleFavorites = false;
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
            if (this.Parent is null) return;
            if (Manipulation.MouseCursorIsHidden ||
                MessageBox.Show(
                    LanguageManager.Data("toggleMouseCursorVisibilityPrompt"),
                    LanguageManager.Data("toggleMouseCursorVisibilityTitle"), MessageBoxButton.YesNo, MessageBoxImage.Warning,
                    MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Manipulation.ToggleMouseCursorVisibility(this.Parent);
            }
        }

        private void OnToggleWindowsTaskbarVisibilityMenuItemClicked(bool value)
        {
            Manipulation.ToggleWindowsTaskbarVisibility();
        }

        private async Task OnResetHiddenProcessesMenuItemClicked()
        {
            SettingsWrapper.Instance.ResetHiddenProcesses();
            if (this.Parent is null) return;
            await this.Parent.RefreshProcesses();
        }

        private async Task OnFullApplicationRefreshMenuItemClicked()
        {
            if (this.Parent is null) return;
            await this.Parent.RefreshProcesses();
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
            if (fav is null) return;
            this.FavoriteListBoxItemsSource.Remove(fav);
        }

        private void OnUnfavoriteButtonClicked(Favorite? fav)
        {
            if (fav is null) return;
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

        private void TrayIconDoubleClicked()
        {
            if (this.Parent is null) return;
            this.Parent.Show();
            this.Parent.WindowState = WindowState.Normal;
        }

        #endregion Event Methods

        #endregion Methods
    }
}
