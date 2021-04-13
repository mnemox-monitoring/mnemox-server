using Mnemox.Timescale.Models;

namespace Mnemox.Timescale.DM.Dal
{
    public class DbFactory : IDbFactory
    {
        private readonly DbFactorySettings _factorySettings;

        public DbFactory(DbFactorySettings factorySettings)
        {
            _factorySettings = factorySettings;
        }

        public DbBase GetDbBase()
        {
            return new DbBase(_factorySettings.ConnectionString);
        }
    }
}
