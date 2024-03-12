using BorderlessGaming.Logic.NekoBoiNick;

namespace BorderlessGaming.Logic.Models.Json
{
    public static class JsonTools
    {
        public static JsonUserPreferences ConvertFromBebop(UserPreferences userPreferences)
        {
            return new JsonUserPreferences(userPreferences);
        }

        public static UserPreferences ConvertToBebop(JsonUserPreferences userPreferences)
        {
            return new UserPreferences
            {
                Settings = new AppSettings
                {
                    CheckForUpdates = userPreferences.Settings.CheckForUpdates,
                    CloseToTray = userPreferences.Settings.CloseToTray,
                    Culture = userPreferences.Settings.Culture,
                    DisableSteamIntegration = userPreferences.Settings.DisableSteamIntegration,
                    HideBalloonTips = userPreferences.Settings.HideBalloonTips,
                    RunOnStartup = userPreferences.Settings.RunOnStartup,
                    SlowWindowDetection = userPreferences.Settings.SlowWindowDetection,
                    StartMinimized = userPreferences.Settings.StartMinimized,
                    UseGlobalHotkey = userPreferences.Settings.UseGlobalHotkey,
                    UseMouseHideHotKey = userPreferences.Settings.UseMouseHideHotKey,
                    UseMouseLockHotKey = userPreferences.Settings.UseMouseLockHotKey,
                    ViewAllProcessDetails = userPreferences.Settings.ViewAllProcessDetails,
                },
                Favorites = userPreferences.Favorites.ToBebopArray(),
                HiddenProcesses = userPreferences.HiddenProcesses.ToArray(),
                StartupOptions = userPreferences.StartupOptions,
            };
        }
    }
}