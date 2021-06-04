namespace Mnemox.Shared.Models.Settings
{
    public class ServerSettings : IServerSettings
    {
        public string Database { get; set; }
        public bool Initialized { get; set; }
        public string BasePath { get; set; }
    }
}
