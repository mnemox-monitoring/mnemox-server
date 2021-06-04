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

        private readonly IApiConfiguration _apiConfiguration;

        public ServerInitializationManager(
            IServerSettings serverSettings, 
            ILogsManager logsManager,
            IServerInitializationManagerHelpers serverInitializationManagerHelpers,
            IApiConfiguration apiConfiguration)
        {
            _serverSettings = serverSettings;

            _logsManager = logsManager;

            _serverInitializationManagerHelpers = serverInitializationManagerHelpers;

            _apiConfiguration = apiConfiguration;
        }

        public bool IsServerInitialized()
        {
            return _serverSettings.Initialized;
        }

        public void Initialize()
        {
            _apiConfiguration.Services.AddTransient<IMemoryCacheFacade, MemoryCacheFacade>();

            _apiConfiguration.Services.AddTransient<ISecretsManager, SecretsManager>();

            _apiConfiguration.Services.AddTransient<AuthenticationFilter>();

            _apiConfiguration.Services.AddTransient<TenantContextValidationFilter>();

            var databaseTypeName = _serverInitializationManagerHelpers.GetDatabaseType();

            if (databaseTypeName == DatabasesConsts.TIMESCALEDB)
            {
                _serverInitializationManagerHelpers.SetTimescaleDataManagerTs();
            }
        }
    }
}
