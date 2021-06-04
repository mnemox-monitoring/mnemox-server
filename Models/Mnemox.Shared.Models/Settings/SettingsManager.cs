using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Mnemox.Shared.Models.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private readonly IFullSettings _fullSettings;

        private const string SETTINGS_FILE_NAME = "mnemox-server-settings.json";

        public SettingsManager(IFullSettings fullSettings)
        {
            _fullSettings = fullSettings;
        }

        public async Task ReloadSettingsAsync()
        {
            var settingsJsonString = JsonConvert.SerializeObject(_fullSettings, Formatting.Indented);

            await File.WriteAllTextAsync($"{_fullSettings.ServerSettings.BasePath}/{SETTINGS_FILE_NAME}", settingsJsonString);
        }

        public IFullSettings FullSettings
        {
            get
            {
                return _fullSettings;
            }
        }
    }
}
