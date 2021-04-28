using System;
using System.Data.Common;

namespace Mnemox.Timescale.DM
{
    public class DataManagersHelpers : IDataManagersHelpers
    {
        public string GetString(DbDataReader dbDataReader, string itemName)
        {
            var item = dbDataReader[itemName];

            return item is DBNull ? null : item.ToString();
        }
    }
}
