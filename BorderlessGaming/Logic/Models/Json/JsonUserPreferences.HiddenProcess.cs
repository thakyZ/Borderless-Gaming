using System.Collections.Generic;

namespace BorderlessGaming.Logic.Models.Json
{
    public partial class JsonUserPreferences
    {
        /// <summary>
        ///     AlwaysHiddenProcesses is used to keep processes from showing up in the list no matter what
        /// </summary>
        public static readonly List<string> AlwaysHiddenProcesses = UserPreferences.AlwaysHiddenProcesses;
    }
}