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
        Task<DbDataReader> ExecuteReaderAsync(string command, List<TimescaleParameter> parameters = null);
    }
}