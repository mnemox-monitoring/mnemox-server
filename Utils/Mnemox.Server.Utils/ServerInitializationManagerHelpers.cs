using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mnemox.Account.Models;
using Mnemox.DataStorage.Models;
using Mnemox.Monitoring.Models;
using Mnemox.Resources.Models;
using Mnemox.Shared.Models.Settings;
using Mnemox.Timescale.DM;
using Mnemox.Timescale.DM.Account;
using Mnemox.Timescale.DM.Dal;
using Mnemox.Timescale.DM.HeartBeat;
using Mnemox.Timescale.DM.Infrastructure;
using Mnemox.Timescale.DM.Resources;
using Mnemox.Timescale.DM.Tenants;
using System;

namespace Mnemox.Server.Utils
{
    public class ServerInitializationManagerHelpers : IServerInitializationManagerHelpers
    {
        private readonly IServerSettings _serverSettings;

        private readonly IApiConfiguration _apiConfiguration;

        public ServerInitializationManagerHelpers(IServerSettings serverSettings, IApiConfiguration apiConfiguration)
        {
            _serverSettings = serverSettings;

            _apiConfiguration = apiConfiguration;
        }

        public void SetTimescaleDataManagerTs()
        {
            const string DATABASE_FACTORY_SETTINGS_SECTION_NAME = "DbFactorySettings";

            var dbFactorySettings = new DbFactorySettings();

            _apiConfiguration.Configuration.GetSection(DATABASE_FACTORY_SETTINGS_SECTION_NAME).Bind(dbFactorySettings);

            _apiConfiguration.Services.AddTransient<IDbFactory, TimescaleDbFactory>();

            _apiConfiguration.Services.AddTransient<IHeartBeatDataManager, HeartBeatDataManagerTs>();

            _apiConfiguration.Services.AddTransient<IResourceDataManager, ResourcesDataManagerTs>();

            _apiConfiguration.Services.AddTransient<IUsersDataManagerTsHelpers, UsersDataManagerTsHelpers>();

            _apiConfiguration.Services.AddTransient<IUsersDataManager, UsersDataManagerTs>();

            _apiConfiguration.Services.AddTransient<IDataManagersHelpersTs, DataManagersHelpersTs>();

            _apiConfiguration.Services.AddTransient<IUsersDataManager, UsersDataManagerTs>();

            _apiConfiguration.Services.AddTransient<ITokensManager, TokensManagerTs>();

            _apiConfiguration.Services.AddTransient<ITenantObjectsManager, TenantsObjectsManagetTs>();

            _apiConfiguration.Services.AddTransient<IDataStorageInfrastructureManager, TimescaleInfrastructure>();

            _apiConfiguration.Services.AddTransient<ITimescaleInfrastructureHelpers, TimescaleInfrastructureHelpers>();

            _apiConfiguration.Services.AddSingleton<IServersManager, ServersManagerTs>();
        }

        public string GetDatabaseType()
        {
            if (string.IsNullOrWhiteSpace(_serverSettings.Database))
            {
                throw new ArgumentNullException(nameof(_serverSettings.Database));
            }

            return _serverSettings.Database.Trim().ToLower();
        }
    }
}
