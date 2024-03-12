using System;
using System.Text;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Steamworks.ServerList;
using Steamworks.Ugc;
using System.DirectoryServices.ActiveDirectory;
using BorderlessGaming.Logic.NekoBoiNick;

#nullable enable
namespace BorderlessGaming.Logic.Models.Json
{
    /// <summary>A static class which contains schema defined constants.</summary>
    public static class JsonConstants
    {
        public const string DefaultCulture = "en-US";
        public const MethodImplOptions HotPath = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
    }

    public partial class JsonProcessRectangle : IEquatable<JsonProcessRectangle>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        /// <summary>
        /// </summary>
        public JsonProcessRectangle() : base() { }

        public JsonProcessRectangle(int x, int y, int width, int height) => (X, Y, Width, Height) = (x, y, width, height);
        public JsonProcessRectangle(JsonProcessRectangle original) => (X, Y, Width, Height) = (original.X, original.Y, original.Width, original.Height);
        public JsonProcessRectangle(ProcessRectangle original) => (X, Y, Width, Height) = (original.X, original.Y, original.Width, original.Height);
        public void Deconstruct(out int x, out int y, out int width, out int height) => (x, y, width, height) = (X, Y, Width, Height);

        #region Equality
        public bool Equals(JsonProcessRectangle? other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return X == other.X
                && Y == other.Y
                && Width == other.Width
                && Height == other.Height;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj is JsonProcessRectangle baseType)
            {
                return Equals(baseType);
            }
            if (obj is ProcessRectangle other)
            {
                return X == other.X
                    && Y == other.Y
                    && Width == other.Width
                    && Height == other.Height;
            }
            return false;
        }

        public static bool StaticEquals(JsonProcessRectangle? instance, object? obj)
        {
            return instance?.Equals(obj: obj) ?? false;
        }

        public static bool Equals(JsonProcessRectangle? instance, JsonProcessRectangle? other)
        {
            return instance?.Equals(other) ?? false;
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash ^= X.GetHashCode();
            hash ^= Y.GetHashCode();
            hash ^= Width.GetHashCode();
            hash ^= Height.GetHashCode();
            return hash;
        }

        public static bool operator ==(JsonProcessRectangle left, JsonProcessRectangle right) => StaticEquals(left, right);
        public static bool operator !=(JsonProcessRectangle left, JsonProcessRectangle right) => !StaticEquals(left, right);

        public static bool operator ==(JsonProcessRectangle left, ProcessRectangle right) => StaticEquals(left, right);
        public static bool operator !=(JsonProcessRectangle left, ProcessRectangle right) => !StaticEquals(left, right);
        #endregion
    }

    public partial class JsonAppSettings : IEquatable<JsonAppSettings>
    {
        public bool SlowWindowDetection { get; set; }
        public bool ViewAllProcessDetails { get; set; }
        public bool RunOnStartup { get; set; }
        public bool UseGlobalHotkey { get; set; }
        public bool UseMouseLockHotKey { get; set; }
        public bool UseMouseHideHotKey { get; set; }
        public bool StartMinimized { get; set; }
        public bool HideBalloonTips { get; set; }
        public bool CloseToTray { get; set; }
        public bool CheckForUpdates { get; set; }
        public bool DisableSteamIntegration { get; set; }

        public string? Culture { get; set; }

        public JsonAppSettings() : base() { }
        public JsonAppSettings(bool? slowWindowDetection, bool? viewAllProcessDetails, bool? runOnStartup, bool? useGlobalHotkey, bool? useMouseLockHotKey, bool? useMouseHideHotKey, bool? startMinimized, bool? hideBalloonTips, bool? closeToTray, bool? checkForUpdates, bool? disableSteamIntegration, string? culture) => (SlowWindowDetection, ViewAllProcessDetails, RunOnStartup, UseGlobalHotkey, UseMouseLockHotKey, UseMouseHideHotKey, StartMinimized, HideBalloonTips, CloseToTray, CheckForUpdates, DisableSteamIntegration, Culture) = (slowWindowDetection ?? false, viewAllProcessDetails ?? false, runOnStartup ?? false, useGlobalHotkey ?? false, useMouseLockHotKey ?? false, useMouseHideHotKey ?? false, startMinimized ?? false, hideBalloonTips ?? false, closeToTray ?? false, checkForUpdates ?? false, disableSteamIntegration ?? false, culture);
        public JsonAppSettings(JsonAppSettings? original) => (SlowWindowDetection, ViewAllProcessDetails, RunOnStartup, UseGlobalHotkey, UseMouseLockHotKey, UseMouseHideHotKey, StartMinimized, HideBalloonTips, CloseToTray, CheckForUpdates, DisableSteamIntegration, Culture) = (original?.SlowWindowDetection ?? false, original?.ViewAllProcessDetails ?? false, original?.RunOnStartup ?? false, original?.UseGlobalHotkey ?? false, original?.UseMouseLockHotKey ?? false, original?.UseMouseHideHotKey ?? false, original?.StartMinimized ?? false, original?.HideBalloonTips ?? false, original?.CloseToTray ?? false, original?.CheckForUpdates ?? false, original?.DisableSteamIntegration ?? false, original?.Culture);
        public JsonAppSettings(AppSettings? original) => (SlowWindowDetection, ViewAllProcessDetails, RunOnStartup, UseGlobalHotkey, UseMouseLockHotKey, UseMouseHideHotKey, StartMinimized, HideBalloonTips, CloseToTray, CheckForUpdates, DisableSteamIntegration, Culture) = (original?.SlowWindowDetection ?? false, original?.ViewAllProcessDetails ?? false, original?.RunOnStartup ?? false, original?.UseGlobalHotkey ?? false, original?.UseMouseLockHotKey ?? false, original?.UseMouseHideHotKey ?? false, original?.StartMinimized ?? false, original?.HideBalloonTips ?? false, original?.CloseToTray ?? false, original?.CheckForUpdates ?? false, original?.DisableSteamIntegration ?? false, original?.Culture);
        public void Deconstruct(out bool? slowWindowDetection, out bool? viewAllProcessDetails, out bool? runOnStartup, out bool? useGlobalHotkey, out bool? useMouseLockHotKey, out bool? useMouseHideHotKey, out bool? startMinimized, out bool? hideBalloonTips, out bool? closeToTray, out bool? checkForUpdates, out bool? disableSteamIntegration, out string? culture) => (slowWindowDetection, viewAllProcessDetails, runOnStartup, useGlobalHotkey, useMouseLockHotKey, useMouseHideHotKey, startMinimized, hideBalloonTips, closeToTray, checkForUpdates, disableSteamIntegration, culture) = (SlowWindowDetection, ViewAllProcessDetails, RunOnStartup, UseGlobalHotkey, UseMouseLockHotKey, UseMouseHideHotKey, StartMinimized, HideBalloonTips, CloseToTray, CheckForUpdates, DisableSteamIntegration, Culture);

        #region Equality
        public static bool StaticEquals(JsonAppSettings? instance, object? obj)
        {
            return instance?.Equals(obj: obj) ?? false;
        }

        public bool Equals(JsonAppSettings? other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return SlowWindowDetection == other.SlowWindowDetection
                && ViewAllProcessDetails == other.ViewAllProcessDetails
                && RunOnStartup == other.RunOnStartup
                && UseGlobalHotkey == other.UseGlobalHotkey
                && UseMouseLockHotKey == other.UseMouseLockHotKey
                && UseMouseHideHotKey == other.UseMouseHideHotKey
                && StartMinimized == other.StartMinimized
                && HideBalloonTips == other.HideBalloonTips
                && CloseToTray == other.CloseToTray
                && CheckForUpdates == other.CheckForUpdates
                && DisableSteamIntegration == other.DisableSteamIntegration
                && Culture == other.Culture;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj is JsonAppSettings baseType)
            {
                return Equals(baseType);
            }
            if (obj is AppSettings other)
            {
                return SlowWindowDetection == other.SlowWindowDetection
                    && ViewAllProcessDetails == other.ViewAllProcessDetails
                    && RunOnStartup == other.RunOnStartup
                    && UseGlobalHotkey == other.UseGlobalHotkey
                    && UseMouseLockHotKey == other.UseMouseLockHotKey
                    && UseMouseHideHotKey == other.UseMouseHideHotKey
                    && StartMinimized == other.StartMinimized
                    && HideBalloonTips == other.HideBalloonTips
                    && CloseToTray == other.CloseToTray
                    && CheckForUpdates == other.CheckForUpdates
                    && DisableSteamIntegration == other.DisableSteamIntegration
                    && Culture == other.Culture;
            }
            return false;
        }

        public static bool Equals(JsonAppSettings? instance, JsonAppSettings? other)
        {
            return instance?.Equals(other) ?? false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                       HashCode.Combine(
                           SlowWindowDetection.GetHashCode(),
                           ViewAllProcessDetails.GetHashCode(),
                           RunOnStartup.GetHashCode(),
                           UseGlobalHotkey.GetHashCode(),
                           UseMouseLockHotKey.GetHashCode(),
                           UseMouseHideHotKey.GetHashCode(),
                           StartMinimized.GetHashCode(),
                           HideBalloonTips.GetHashCode()
                       ),
                       CloseToTray.GetHashCode(),
                       CheckForUpdates.GetHashCode(),
                       DisableSteamIntegration.GetHashCode(),
                       Culture?.GetHashCode()
                   );
        }

        public static bool operator ==(JsonAppSettings left, JsonAppSettings right) => StaticEquals(left, right);
        public static bool operator !=(JsonAppSettings left, JsonAppSettings right) => !StaticEquals(left, right);
        public static bool operator ==(JsonAppSettings left, AppSettings right) => StaticEquals(left, right);
        public static bool operator !=(JsonAppSettings left, AppSettings right) => !StaticEquals(left, right);
        #endregion

    }

    public partial class JsonUserPreferences : IEquatable<JsonUserPreferences>
    {

        public List<JsonFavorite> Favorites { get; set; } = [];

        public List<string> HiddenProcesses { get; set; } = [];

        public JsonAppSettings Settings { get; set; } = new();

        public JsonUserPreferences() : base() { }
        public JsonUserPreferences(List<JsonFavorite> favorites, List<string> hiddenProcesses, JsonAppSettings settings) => (Favorites, HiddenProcesses, Settings) = (favorites, hiddenProcesses, settings);
        public JsonUserPreferences(JsonUserPreferences original) => (Favorites, HiddenProcesses, Settings) = (original.Favorites, original.HiddenProcesses, original.Settings);
        public JsonUserPreferences(UserPreferences original)
        {
            foreach (var favorite in original.Favorites)
            {
                this.Favorites.Add(new JsonFavorite(favorite));
            }
            this.HiddenProcesses = [.. original.HiddenProcesses];
            this.Settings = new JsonAppSettings(original.Settings);
        }

        #region Equality
        public static bool StaticEquals(JsonUserPreferences? instance, object? obj)
        {
            return instance?.Equals(obj: obj) ?? false;
        }

        public bool Equals(JsonUserPreferences? other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return (Favorites is null ? other.Favorites is null : other.Favorites is not null && Enumerable.SequenceEqual(Favorites, other.Favorites))
                && (HiddenProcesses is null ? other.HiddenProcesses is null : other.HiddenProcesses is not null && Enumerable.SequenceEqual(HiddenProcesses, other.HiddenProcesses))
                && Settings == other.Settings;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj is JsonUserPreferences baseType)
            {
                return Equals(baseType);
            }
            if (obj is UserPreferences other)
            {
                return (Favorites is null ? other.Favorites is null : other.Favorites is not null && Favorites.SequenceEqual(other.Favorites))
                    && (HiddenProcesses is null ? other.HiddenProcesses is null : other.HiddenProcesses is not null && Enumerable.SequenceEqual(HiddenProcesses, other.HiddenProcesses))
                    && Settings == other.Settings;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Favorites.GetHashCode(), HiddenProcesses.GetHashCode(), Settings.GetHashCode());
        }

        public static bool operator ==(JsonUserPreferences left, JsonUserPreferences right) => StaticEquals(left, right);
        public static bool operator !=(JsonUserPreferences left, JsonUserPreferences right) => !StaticEquals(left, right);

        public static bool operator ==(JsonUserPreferences left, UserPreferences right) => StaticEquals(left, right);
        public static bool operator !=(JsonUserPreferences left, UserPreferences right) => !StaticEquals(left, right);
        #endregion
    }

    public partial class JsonFavorite : IEquatable<JsonFavorite>
    {
        public FavoriteType Type { get; set; } = new();
        public FavoriteSize Size { get; set; } = new();
        public string SearchText { get; set; } = string.Empty;
        public JsonProcessRectangle Screen { get; set; } = new();
        public int OffsetLeft { get; set; }
        public int OffsetTop { get; set; }
        public int OffsetRight { get; set; }
        public int OffsetBottom { get; set; }
        public bool ShouldMaximize { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int PositionWidth { get; set; }
        public int PositionHeight { get; set; }
        public bool RemoveMenus { get; set; }
        public bool TopMost { get; set; }
        public bool HideWindowsTaskbar { get; set; }
        public bool HideMouseCursor { get; set; }
        public bool DelayBorderless { get; set; }
        public bool MuteInBackground { get; set; }

        public JsonFavorite() : base() { }
        public JsonFavorite(FavoriteType type, FavoriteSize size, string searchText, JsonProcessRectangle screen, int offsetLeft, int offsetTop, int offsetRight, int offsetBottom, bool shouldMaximize, int positionX, int positionY, int positionWidth, int positionHeight, bool removeMenus, bool topMost, bool hideWindowsTaskbar, bool hideMouseCursor, bool delayBorderless, bool muteInBackground) => (Type, Size, SearchText, Screen, OffsetLeft, OffsetTop, OffsetRight, OffsetBottom, ShouldMaximize, PositionX, PositionY, PositionWidth, PositionHeight, RemoveMenus, TopMost, HideWindowsTaskbar, HideMouseCursor, DelayBorderless, MuteInBackground) = (type, size, searchText, screen, offsetLeft, offsetTop, offsetRight, offsetBottom, shouldMaximize, positionX, positionY, positionWidth, positionHeight, removeMenus, topMost, hideWindowsTaskbar, hideMouseCursor, delayBorderless, muteInBackground);
        public JsonFavorite(JsonFavorite original) => (Type, Size, SearchText, Screen, OffsetLeft, OffsetTop, OffsetRight, OffsetBottom, ShouldMaximize, PositionX, PositionY, PositionWidth, PositionHeight, RemoveMenus, TopMost, HideWindowsTaskbar, HideMouseCursor, DelayBorderless, MuteInBackground) = (original.Type, original.Size, original.SearchText, original.Screen, original.OffsetLeft, original.OffsetTop, original.OffsetRight, original.OffsetBottom, original.ShouldMaximize, original.PositionX, original.PositionY, original.PositionWidth, original.PositionHeight, original.RemoveMenus, original.TopMost, original.HideWindowsTaskbar, original.HideMouseCursor, original.DelayBorderless, original.MuteInBackground);
        public JsonFavorite(Favorite original) => (Type, Size, SearchText, Screen, OffsetLeft, OffsetTop, OffsetRight, OffsetBottom, ShouldMaximize, PositionX, PositionY, PositionWidth, PositionHeight, RemoveMenus, TopMost, HideWindowsTaskbar, HideMouseCursor, DelayBorderless, MuteInBackground) = (original.Type, original.Size, original.SearchText, new JsonProcessRectangle(original.Screen), original.OffsetLeft, original.OffsetTop, original.OffsetRight, original.OffsetBottom, original.ShouldMaximize, original.PositionX, original.PositionY, original.PositionWidth, original.PositionHeight, original.RemoveMenus, original.TopMost, original.HideWindowsTaskbar, original.HideMouseCursor, original.DelayBorderless, original.MuteInBackground);
        public void Deconstruct(out FavoriteType type, out FavoriteSize size, out string searchText, out JsonProcessRectangle screen, out int offsetLeft, out int offsetTop, out int offsetRight, out int offsetBottom, out bool shouldMaximize, out int positionX, out int positionY, out int positionWidth, out int positionHeight, out bool removeMenus, out bool topMost, out bool hideWindowsTaskbar, out bool hideMouseCursor, out bool delayBorderless, out bool muteInBackground) => (type, size, searchText, screen, offsetLeft, offsetTop, offsetRight, offsetBottom, shouldMaximize, positionX, positionY, positionWidth, positionHeight, removeMenus, topMost, hideWindowsTaskbar, hideMouseCursor, delayBorderless, muteInBackground) = (Type, Size, SearchText, Screen, OffsetLeft, OffsetTop, OffsetRight, OffsetBottom, ShouldMaximize, PositionX, PositionY, PositionWidth, PositionHeight, RemoveMenus, TopMost, HideWindowsTaskbar, HideMouseCursor, DelayBorderless, MuteInBackground);

        #region Equality
        public static bool StaticEquals(JsonFavorite? instance, object? obj)
        {
            return instance?.Equals(obj: obj) ?? false;
        }

        public bool Equals(JsonFavorite? other)
        {
            if (other is null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Type == other.Type
                && Size == other.Size
                && SearchText == other.SearchText
                && Screen == other.Screen
                && OffsetLeft == other.OffsetLeft
                && OffsetTop == other.OffsetTop
                && OffsetRight == other.OffsetRight
                && OffsetBottom == other.OffsetBottom
                && ShouldMaximize == other.ShouldMaximize
                && PositionX == other.PositionX
                && PositionY == other.PositionY
                && PositionWidth == other.PositionWidth
                && PositionHeight == other.PositionHeight
                && RemoveMenus == other.RemoveMenus
                && TopMost == other.TopMost
                && HideWindowsTaskbar == other.HideWindowsTaskbar
                && HideMouseCursor == other.HideMouseCursor
                && DelayBorderless == other.DelayBorderless
                && MuteInBackground == other.MuteInBackground;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj is JsonFavorite baseType)
            {
                return Equals(baseType);
            }
            if (obj is Favorite other)
            {
                return Type == other.Type
                    && Size == other.Size
                    && SearchText == other.SearchText
                    && Screen == other.Screen
                    && OffsetLeft == other.OffsetLeft
                    && OffsetTop == other.OffsetTop
                    && OffsetRight == other.OffsetRight
                    && OffsetBottom == other.OffsetBottom
                    && ShouldMaximize == other.ShouldMaximize
                    && PositionX == other.PositionX
                    && PositionY == other.PositionY
                    && PositionWidth == other.PositionWidth
                    && PositionHeight == other.PositionHeight
                    && RemoveMenus == other.RemoveMenus
                    && TopMost == other.TopMost
                    && HideWindowsTaskbar == other.HideWindowsTaskbar
                    && HideMouseCursor == other.HideMouseCursor
                    && DelayBorderless == other.DelayBorderless
                    && MuteInBackground == other.MuteInBackground;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                       HashCode.Combine(
                           Type.GetHashCode(),
                           Size.GetHashCode(),
                           SearchText.GetHashCode(),
                           Screen.GetHashCode(),
                           OffsetLeft.GetHashCode(),
                           OffsetTop.GetHashCode(),
                           OffsetRight.GetHashCode(),
                           OffsetBottom.GetHashCode()
                       ),
                       HashCode.Combine(
                           ShouldMaximize.GetHashCode(),
                           PositionX.GetHashCode(),
                           PositionY.GetHashCode(),
                           PositionWidth.GetHashCode(),
                           PositionHeight.GetHashCode(),
                           RemoveMenus.GetHashCode(),
                           TopMost.GetHashCode(),
                           HideWindowsTaskbar.GetHashCode()
                       ),
                       HideMouseCursor.GetHashCode(),
                       DelayBorderless.GetHashCode(),
                       MuteInBackground.GetHashCode()
                   );
        }

        public static bool operator ==(JsonFavorite left, JsonFavorite right) => StaticEquals(left, right);
        public static bool operator !=(JsonFavorite left, JsonFavorite right) => !StaticEquals(left, right);

        public static bool operator ==(JsonFavorite left, Favorite right) => StaticEquals(left, right);
        public static bool operator !=(JsonFavorite left, Favorite right) => !StaticEquals(left, right);
        #endregion
    }
}