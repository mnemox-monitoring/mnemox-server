using Mnemox.Shared.Models.Settings;

namespace Mnemox.Server.Utils
{
    public interface IServerInitializationManagerHelpers
    {
        string GetDatabaseType();
        void SetTimescaleDataManagerTs();
    }
}