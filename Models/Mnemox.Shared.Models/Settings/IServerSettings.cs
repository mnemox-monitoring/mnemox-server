namespace Mnemox.Shared.Models.Settings
{
    public interface IServerSettings
    {
        string Database { get; set; }
        bool Initialized { get; set; }
        public string BasePath { get; set; }
    }
}