using System;
using System.Data.Common;

namespace Mnemox.Timescale.DM
{
    public interface IDataManagersHelpersTs
    {
        string CreateParameterName(string parameterName);
        string CreateSelectPropertyName(string propertyName);
        DateTime? GetDateTime(DbDataReader dbDataReader, string itemName);
        
        int? GetInt(DbDataReader dbDataReader, string itemName);
        
        long? GetLong(DbDataReader dbDataReader, string itemName);
        
        string GetString(DbDataReader dbDataReader, string itemName);
    }
}