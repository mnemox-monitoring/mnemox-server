namespace Mnemox.Server.Utils
{
    public interface IServerInitializationManager
    {
        void Initialize();
        bool IsServerInitialized();
    }
}