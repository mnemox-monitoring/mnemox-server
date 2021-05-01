using System;
using System.Data.Common;

namespace Mnemox.Timescale.DM
{
    public class DataManagersHelpersTs : IDataManagersHelpersTs
    {
        public string GetString(DbDataReader dbDataReader, string itemName)
        {
            var item = dbDataReader[itemName];

            return item is DBNull ? null : item.ToString();
        }

        public long? GetLong(DbDataReader dbDataReader, string itemName)
        {
            var item = dbDataReader[itemName];

            return item is DBNull ? null : (long)item;
        }

        public int? GetInt(DbDataReader dbDataReader, string itemName)
        {
            var item = dbDataReader[itemName];

            return item is DBNull ? null : (int)item;
        }

        public DateTime? GetDateTime(DbDataReader dbDataReader, string itemName)
        {
            var item = dbDataReader[itemName];

            return item is DBNull ? null : Convert.ToDateTime(item);
        }
    }
}
