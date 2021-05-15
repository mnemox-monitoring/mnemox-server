using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Settings;

namespace Mnemox.Server.Utils
{
    public class ServerInitializationManager
    {
        private readonly IServerSettings _serverSettings;

        private readonly ILogsManager _logsManager;

        public ServerInitializationManager(IServerSettings serverSettings, ILogsManager logsManager)
        {
            _serverSettings = serverSettings;

            _logsManager = logsManager;
        }

        public bool IsServerInitialized()
        {
            try
            {

            }
            catch(HandledException ex)
            {
                _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw;
            }

            return false;
        }
    }
}
