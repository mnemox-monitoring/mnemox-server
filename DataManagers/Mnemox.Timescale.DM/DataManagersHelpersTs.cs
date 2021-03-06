using Mnemox.Timescale.Models;
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

        public string CreateSelectPropertyName(string propertyName)
        {
            var tsPropertyName = $"{ConstantsTs.SELECT_PROPERTY_PREFIX}{propertyName}";

            return tsPropertyName;
        }

        public string CreateParameterName(string parameterName)
        {
            var tsParameterName = $"{ConstantsTs.FUNCTION_PARAMETER_PREFIX}{parameterName}";

            return tsParameterName;
        }
    }
}
