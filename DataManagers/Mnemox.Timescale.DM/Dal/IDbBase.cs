using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Dal
{
    public interface IDbBase
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        Task ExecuteNonQueryAsync(string command, List<TimescaleParameter> parameters = null);
        Task ExecuteNonQueryFunctionAsync(string command, List<TimescaleParameter> parameters = null);
        Task<DbDataReader> ExecuteReaderFunctionAsync(string command, List<TimescaleParameter> parameters = null);
        Task<DbDataReader> ExecuteReaderQueryAsync(string command, List<TimescaleParameter> parameters = null);
        Task<object> ExecuteScalarAsync(string command, List<TimescaleParameter> parameters = null);
    }
}