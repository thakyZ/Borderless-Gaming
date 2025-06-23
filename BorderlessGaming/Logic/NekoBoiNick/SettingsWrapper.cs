using System;
using System.IO;
using BorderlessGaming.Logic.Misc;
using BorderlessGaming.Logic.Models.Json;
using BorderlessGaming.Logic.NekoBoiNick;

#nullable enable

namespace BorderlessGaming.Logic.Models
{
    /// <summary>
    /// Wrapper class for the original <see cref="Bebop" /> settings and the newer
    /// <see cref="BorderlessGaming.Logic.Models.Json" /> settings.
    /// </summary>
    internal class SettingsWrapper
    {
        /// <summary>
        /// Instance of this <see cref="SettingsWrapper" />
        /// </summary>
        private static Lazy<SettingsWrapper> _instance = new Lazy<SettingsWrapper>(() => new SettingsWrapper());

        /// <summary>
        /// Gets the instance of the user preferences.
        /// </summary>
        private UserPreferences UserPreferences { get; }
        
        /// <summary>
        /// Gets whether the application should use json or not.
        /// </summary>
        private bool UseJson { get; }

        /// <summary>
        /// Gets the instance of the user preference externally.
        /// </summary>
        public static UserPreferences Instance => _instance.Value.UserPreferences;

        /// <summary>
        /// Creates a new instance of <see cref="SettingsWrapper" />
        /// </summary>
        private SettingsWrapper()
        {
            if (File.Exists(Path.Join(AppEnvironment.ExecutableDirectory, ".use_json"))
                || Environment.GetEnvironmentVariables().Contains("BORDERLESS_WINDOWED_USE_JSON", "true"))
            {
                this.UseJson = true;
            }

            this.UserPreferences = Load();
        }

        private UserPreferences Load()
        {
            if (this.UseJson)
            {
                if (File.Exists(JsonUserPreferences.JsonConfigPath))
                {
                    return JsonTools.ConvertToBebop(JsonUserPreferences.Instance);
                }

                var output = UserPreferences.Instance;
                JsonTools.ConvertFromBebop(output).Save();
                return output;
            }
            else if (!File.Exists(AppEnvironment.ConfigPath) && File.Exists(JsonUserPreferences.JsonConfigPath))
            {
                return JsonTools.ConvertToBebop(JsonUserPreferences.Instance);
            }
            return UserPreferences.Instance;
        }

        public static void Save()
        {
            if (_instance.Value.UseJson) {
                JsonTools.ConvertFromBebop(_instance.Value.UserPreferences).Save();
            } else {
                _instance.Value.UserPreferences.Save();
            }
        }
    }
}
