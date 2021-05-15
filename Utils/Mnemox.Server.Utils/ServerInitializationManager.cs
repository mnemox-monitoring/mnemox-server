using Microsoft.Extensions.DependencyInjection;
using Mnemox.Api.Security.Utils;
using Mnemox.Logs.Models;
using Mnemox.Security.Utils;
using Mnemox.Shared.Models.Consts;
using Mnemox.Shared.Models.Settings;
using Mnemox.Shared.Utils;

namespace Mnemox.Server.Utils
{
    public class ServerInitializationManager : IServerInitializationManager
    {
        private readonly IServerSettings _serverSettings;

        private readonly ILogsManager _logsManager;

        private readonly IServerInitializationManagerHelpers _serverInitializationManagerHelpers;

        public ServerInitializationManager(
            IServerSettings serverSettings, 
            ILogsManager logsManager,
            IServerInitializationManagerHelpers serverInitializationManagerHelpers)
        {
            _serverSettings = serverSettings;

            _logsManager = logsManager;

            _serverInitializationManagerHelpers = serverInitializationManagerHelpers;
        }

        public bool IsServerInitialized()
        {
            return _serverSettings.Initialized;
        }

        public void Initialize()
        {
            _serverSettings.Services.AddTransient<IMemoryCacheFacade, MemoryCacheFacade>();

            _serverSettings.Services.AddTransient<ISecretsManager, SecretsManager>();

            _serverSettings.Services.AddTransient<AuthenticationFilter>();

            _serverSettings.Services.AddTransient<TenantContextValidationFilter>();

            var databaseTypeName = _serverInitializationManagerHelpers.GetDatabaseType();

            if (databaseTypeName == DatabasesConsts.TIMESCALEDB)
            {
                _serverInitializationManagerHelpers.SetTimescaleDataManagerTs();
            }
        }
    }
}
