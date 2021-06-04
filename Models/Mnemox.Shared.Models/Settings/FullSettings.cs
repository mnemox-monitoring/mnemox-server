namespace Mnemox.Shared.Models.Settings
{
    public class FullSettings : IFullSettings
    {
        public string AllowedHosts { get; set; }
        public ServerSettings ServerSettings { get; set; }
        public DbFactorySettings DbFactorySettings { get; set; }
    }
}
