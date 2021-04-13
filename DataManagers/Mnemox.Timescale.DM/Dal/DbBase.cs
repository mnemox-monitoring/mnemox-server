using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            await _connection?.CloseAsync();
        }
    }
}
