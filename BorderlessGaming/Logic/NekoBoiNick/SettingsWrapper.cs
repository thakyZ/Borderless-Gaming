using System.IO;
using MessageBox = System.Windows.Forms.MessageBox;

using BorderlessGaming.Logic.Misc;
using BorderlessGaming.Logic.Models.Json;

#nullable enable
namespace BorderlessGaming.Logic.Models
{
    internal class SettingsWrapper
    {
        private static SettingsWrapper? _instance = null;

        private readonly UserPreferences _userPreferences;

        private readonly bool useJson = false;

        public static UserPreferences Instance
        {
            get
            {
                _instance ??= new();
                return _instance._userPreferences;
            }
        }

        private SettingsWrapper()
        {
            if (File.Exists(Path.Join(AppEnvironment.ExecutableDirectory, ".use_json")))
            {
                useJson = true;
            }
            _userPreferences = Load();
        }

        private UserPreferences Load()
        {
            if (useJson)
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
            _instance ??= new();
            if (_instance.useJson) {
                JsonTools.ConvertFromBebop(_instance._userPreferences).Save();
            } else {
                _instance._userPreferences.Save();
            }
        }
    }
}
