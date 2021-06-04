namespace Mnemox.Shared.Models.Settings
{
    public interface IFullSettings
    {
        string AllowedHosts { get; set; }
        DbFactorySettings DbFactorySettings { get; set; }
        ServerSettings ServerSettings { get; set; }
    }
}