using Mnemox.Timescale.Models;

namespace Mnemox.Timescale.DM.Dal
{
    public class TimescaleDbFactory : IDbFactory
    {
        private readonly DbFactorySettings _factorySettings;

        public TimescaleDbFactory(DbFactorySettings factorySettings)
        {
            _factorySettings = factorySettings;
        }

        public IDbBase GetDbBase()
        {
            return new DbBase(_factorySettings.ConnectionString);
        }
    }
}
