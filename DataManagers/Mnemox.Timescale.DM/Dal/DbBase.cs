using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Dal
{
    public class DbBase : IDbBase
    {
        private readonly string _connectionString;

        private NpgsqlConnection _connection;

        public DbBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task ConnectAsync()
        {
            _connection = new NpgsqlConnection(_connectionString);

            await _connection.OpenAsync();
        }

        public async Task DisconnectAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<DbDataReader> ExecuteReaderFunctionAsync(string command, List<TimescaleParameter> parameters = null)
        {
            DbDataReader reader = null;

            using (var cmd = new NpgsqlCommand(command, _connection)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                if (parameters != null && parameters.Count > 0) cmd.Parameters.AddRange(parameters.ToArray());

                reader = await cmd.ExecuteReaderAsync();
            }

            return reader;
        }

        public async Task<DbDataReader> ExecuteReaderQueryAsync(string command, List<TimescaleParameter> parameters = null)
        {
            DbDataReader reader = null;

            using (var cmd = new NpgsqlCommand(command, _connection)
            {
                CommandType = CommandType.Text
            })
            {
                if (parameters != null && parameters.Count > 0) cmd.Parameters.AddRange(parameters.ToArray());

                reader = await cmd.ExecuteReaderAsync();
            }

            return reader;
        }

        public async Task ExecuteNonQueryAsync(string command, List<TimescaleParameter> parameters = null)
        {
            using (var cmd = new NpgsqlCommand(command, _connection)
            {
                CommandType = CommandType.Text
            })
            {
                if (parameters != null && parameters.Count > 0) cmd.Parameters.AddRange(parameters.ToArray());

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task ExecuteNonQueryFunctionAsync(string command, List<TimescaleParameter> parameters = null)
        {
            using (var cmd = new NpgsqlCommand(command, _connection)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                if (parameters != null && parameters.Count > 0) cmd.Parameters.AddRange(parameters.ToArray());

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<object> ExecuteScalarAsync(string command, List<TimescaleParameter> parameters = null)
        {
            using (var cmd = new NpgsqlCommand(command, _connection)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                if (parameters != null && parameters.Count > 0) cmd.Parameters.AddRange(parameters.ToArray());

                return await cmd.ExecuteScalarAsync();
            }
        }
    }
}
