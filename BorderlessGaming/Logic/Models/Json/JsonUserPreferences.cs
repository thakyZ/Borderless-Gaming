using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MessageBox = System.Windows.Forms.MessageBox;
using BorderlessGaming.Logic.Misc;
using BorderlessGaming.Logic.Misc.Utilities;
using CommandLine;


namespace BorderlessGaming.Logic.Models.Json
{
    public partial class JsonUserPreferences
    {
        private static readonly Lazy<JsonUserPreferences> lazy = new(Load);
        public static JsonUserPreferences Instance { get { return lazy.Value; } }
        public StartupOptions StartupOptions { get; set; } = new StartupOptions();

        internal static string JsonConfigPath => string.Join(".", AppEnvironment.ConfigPath.Split(".").SkipLast(1).ToArray()) + ".json";

        private static JsonSerializerOptions _jsonSerializerOptions;

        static JsonUserPreferences()
        {
            var output = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = false,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                IgnoreReadOnlyProperties = true,
                PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
                IncludeFields = false
            };
            // output.Converters.Add();
            _jsonSerializerOptions = output;
        }

        private static JsonUserPreferences Load()
        {
            JsonUserPreferences preferences = new JsonUserPreferences()
            {
                Favorites = [],
                HiddenProcesses = [],
                Settings = JsonAppSettings.CreateDefault()
            };
            try
            {
                if (!File.Exists(JsonConfigPath))
                {
                    InternalSave(preferences);
                    return preferences;
                }
                using var fileStream = new FileStream(JsonConfigPath, FileMode.Open);
                preferences = JsonSerializer.Deserialize<JsonUserPreferences>(fileStream, _jsonSerializerOptions);
                var parseResults = Parser.Default.ParseArguments<StartupOptions>(Environment.GetCommandLineArgs());
                preferences.StartupOptions = parseResults.Errors.Any() ? new StartupOptions() : parseResults.Value;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e);
                Environment.FailFast("Failed to save user preferences", e);
            }
            return preferences;
        }

        private static void InternalSave(JsonUserPreferences instance)
        {
            try
            {
                if (!File.Exists(JsonConfigPath)) {
                    File.Create(JsonConfigPath).Close();
                }
                using var fs = new FileStream(JsonConfigPath, FileMode.Truncate);
                using var sw = new StreamWriter(fs, Encoding.UTF8);
                var jsonString = JsonSerializer.Serialize(instance, _jsonSerializerOptions);
                sw.Write(jsonString);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e);
                Environment.FailFast("Failed to save user preferences", e);
            }
        }

        public void Save()
        {
            InternalSave(this);
        }

        public bool CanAddFavorite(string item)
        {
            return !Favorites.Exists(fav => fav.SearchText.Equals(item));
        }

        public void AddFavorite(JsonFavorite favorite, Action callback)
        {
            if (!Favorites.Exists(fav => fav.SearchText.Equals(favorite.SearchText)))
            {
                var tmp = Favorites.ToArray();
                Extensions.CollectionExtensions.Add(ref tmp, favorite);
                Favorites = [..tmp];
                Save();
                callback();
            }
        }

        public void RemoveFavorite(JsonFavorite favorite, Action callback)
        {
            if (Favorites.Exists(fav => fav.SearchText.Equals(favorite.SearchText)))
            {
                var tmp = Favorites.ToList();
                tmp.Remove(Favorites.Find(fav => fav.SearchText.Equals(favorite.SearchText)));
                Favorites = tmp;
                Save();
                callback();
            }
        }

        public void ExcludeProcess(string processName)
        {
            if (!IsHidden(processName) && !string.IsNullOrWhiteSpace(processName))
            {
                var tmp = HiddenProcesses.ToArray();
                Extensions.CollectionExtensions.Add(ref tmp, processName);
                HiddenProcesses = [.. tmp];
                Save();
            }
        }

        public void ResetHiddenProcesses()
        {
            HiddenProcesses = [];
            Save();
        }
        public bool IsHidden(Process process)
        {
            return IsHidden(process.ProcessName);
        }

        public bool IsHidden(string processName)
        {
            return AlwaysHiddenProcesses.Exists(process => process.Equals(processName.ToLower())) || HiddenProcesses.Exists(process => process.Equals(processName.ToLower()));
        }

        public int DetectionDelay => Settings.SlowWindowDetection ? 10 : 2;
    }
}