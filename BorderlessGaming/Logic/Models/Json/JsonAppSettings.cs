namespace BorderlessGaming.Logic.Models.Json {
    public partial class JsonAppSettings {
        public static JsonAppSettings CreateDefault() {
            return new JsonAppSettings {
                CheckForUpdates = true,
                Culture = JsonConstants.DefaultCulture
            };
        }
    }
}