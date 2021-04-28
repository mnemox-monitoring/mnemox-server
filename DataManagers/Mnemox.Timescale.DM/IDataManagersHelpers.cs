using System.Data.Common;

namespace Mnemox.Timescale.DM
{
    public interface IDataManagersHelpers
    {
        string GetString(DbDataReader dbDataReader, string itemName);
    }
}