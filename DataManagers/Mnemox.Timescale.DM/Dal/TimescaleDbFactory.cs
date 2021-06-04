using Mnemox.Shared.Models.Settings;

namespace Mnemox.Timescale.DM.Dal
{
    public class TimescaleDbFactory : IDbFactory
    {
        private readonly DbFactorySettings _factorySettings;

        public TimescaleDbFactory(DbFactorySettings factorySettings)
        {
            _factorySettings = factorySettings;
        }

        public IDbBase GetDbBase(string connectionString = null)
        {
            return new DbBase(connectionString ?? _factorySettings.ConnectionString);
        }

        public string CreateConnectionString(string databaseAddress, string username, string password, int? port = 5432, string database = "mnemox")
        {
            var connectionString = string.Format(_factorySettings.ConnectionStringTemplate, databaseAddress, port, username, password, database);

            return connectionString;
        }
    }
}
