using Mnemox.Shared.Models.Settings;

namespace Mnemox.Timescale.DM.Dal
{
    public class TimescaleDbFactory : IDbFactory
    {
        private readonly ISettingsManager _settingsManager;

        private readonly DbFactorySettings _dbFactorySettings;

        public TimescaleDbFactory(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;

            _dbFactorySettings = _settingsManager.FullSettings.DbFactorySettings;
        }

        public IDbBase GetDbBase(string connectionString = null)
        {
            return new DbBase(connectionString ?? _dbFactorySettings.ConnectionString);
        }

        public string CreateConnectionString(string databaseAddress, string username, string password, int? port = 5432, string database = "mnemox")
        {
            var connectionString = string.Format(_dbFactorySettings.ConnectionStringTemplate, databaseAddress, port, username, password, database);

            return connectionString;
        }
    }
}
