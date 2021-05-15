using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mnemox.Account.Models;
using Mnemox.Monitoring.Models;
using Mnemox.Resources.Models;
using Mnemox.Shared.Models.Settings;
using Mnemox.Timescale.DM;
using Mnemox.Timescale.DM.Account;
using Mnemox.Timescale.DM.Dal;
using Mnemox.Timescale.DM.HeartBeat;
using Mnemox.Timescale.DM.Resources;
using Mnemox.Timescale.DM.Tenants;
using Mnemox.Timescale.Models;
using System;

namespace Mnemox.Server.Utils
{
    public class ServerInitializationManagerHelpers : IServerInitializationManagerHelpers
    {
        private readonly IServerSettings _serverSettings;

        public ServerInitializationManagerHelpers(IServerSettings serverSettings)
        {
            _serverSettings = serverSettings;
        }

        public void SetTimescaleDataManagerTs()
        {
            const string DATABASE_FACTORY_SETTINGS_SECTION_NAME = "DbFactorySettings";

            var dbFactorySettings = new DbFactorySettings();

            _serverSettings.Configuration.GetSection(DATABASE_FACTORY_SETTINGS_SECTION_NAME).Bind(dbFactorySettings);

            _serverSettings.Services.AddTransient<IDbFactory>(c => new TimescaleDbFactory(dbFactorySettings));

            _serverSettings.Services.AddTransient<IHeartBeatDataManager, HeartBeatDataManagerTs>();

            _serverSettings.Services.AddTransient<IResourceDataManager, ResourcesDataManagerTs>();

            _serverSettings.Services.AddTransient<IUsersDataManagerTsHelpers, UsersDataManagerTsHelpers>();

            _serverSettings.Services.AddTransient<IUsersDataManager, UsersDataManagerTs>();

            _serverSettings.Services.AddTransient<IDataManagersHelpersTs, DataManagersHelpersTs>();

            _serverSettings.Services.AddTransient<IUsersDataManager, UsersDataManagerTs>();

            _serverSettings.Services.AddTransient<ITokensManager, TokensManagerTs>();

            _serverSettings.Services.AddTransient<ITenantObjectsManager, TenantsObjectsManagetTs>();
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
