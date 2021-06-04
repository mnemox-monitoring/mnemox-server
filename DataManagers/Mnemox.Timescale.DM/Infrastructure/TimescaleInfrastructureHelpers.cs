using Microsoft.AspNetCore.Http;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Timescale.DM.Dal;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Infrastructure
{
    public class TimescaleInfrastructureHelpers : ITimescaleInfrastructureHelpers
    {
        private readonly ILogsManager _logsManager;

        private const string SELECT_SERVER_SCHEMA_INFO = "select * from information_schema.schemata where schema_name = 'server';";

        private const string SELECT_DATABASE_STATE_TABLE_INFO = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'server' AND table_type = 'BASE TABLE' and table_name = 'database_states';";

        private const string GET_DATABASE_INITIALIZATION_STATE_FNC_NAME = "server.database_states_get_last";

        private const string SET_DATABASE_STATE_FNC_NAME = "server.database_states_set";

        private const string STATE_ID_PARAMETER_NAME = "p_state_id";

        public TimescaleInfrastructureHelpers(ILogsManager logsManager)
        {
            _logsManager = logsManager;
        }

        public async Task CreateSchema(IDbBase dbBase, string schemaName)
        {
            try
            {
                await dbBase.ExecuteNonQueryAsync($"create schema if not exists {schemaName};");
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new OutputException(
                    new Exception($"Cannot create schema {schemaName}"),
                    StatusCodes.Status500InternalServerError,
                    MnemoxStatusCodes.CANNOT_CREATE_SCHEMA);
            }
        }

        public async Task CreateExtension(IDbBase dbBase, string extensionName, string schema)
        {
            try
            {
                await dbBase.ExecuteNonQueryAsync($"create extension if not exists {extensionName} schema {schema};");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("has already been loaded with another version"))
                {
                    throw new OutputException(
                        new Exception($"Cannot create extension {extensionName} in schema {schema}, PostgreSQL service had to be restarted"),
                        StatusCodes.Status500InternalServerError,
                        MnemoxStatusCodes.CANNOT_CREATE_EXTENSION);
                }

                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new OutputException(
                    new Exception($"Cannot create extension {extensionName} in schema {schema}"),
                    StatusCodes.Status500InternalServerError,
                    MnemoxStatusCodes.CANNOT_CREATE_EXTENSION);
            }
        }

        public async Task DropSchema(IDbBase dbBase, string schema)
        {
            try
            {
                await dbBase.ExecuteNonQueryAsync($"drop schema if exists {schema};");
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new OutputException(
                    new Exception($"Cannot drop schema {schema}"),
                    StatusCodes.Status500InternalServerError,
                    MnemoxStatusCodes.CANNOT_DROP_SCHEMA);
            }
        }

        public string LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new OutputException(new Exception($"File {path} does not exists"), StatusCodes.Status404NotFound, MnemoxStatusCodes.NOT_FOUND);
            }

            var fileContent = File.ReadAllText(path);

            return fileContent;
        }

        public async Task RunNonQuery(IDbBase dbBase, string query)
        {
            try
            {
                await dbBase.ExecuteNonQueryAsync(query);
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new OutputException(
                    new Exception($"Cannot run query {query}"),
                    StatusCodes.Status500InternalServerError,
                    MnemoxStatusCodes.CANNOT_CREATE_EXTENSION);
            }
        }

        public async Task<DatabaseStatesEnums> DatabaseInitializationState(IDbBase dbBase)
        {
            try
            {
                using (var reader = await dbBase.ExecuteReaderQueryAsync(SELECT_SERVER_SCHEMA_INFO))
                {
                    if (!reader.HasRows)
                    {
                        return DatabaseStatesEnums.NOT_INITIALIZED;
                    }
                }

                using (var reader = await dbBase.ExecuteReaderQueryAsync(SELECT_DATABASE_STATE_TABLE_INFO))
                {
                    if (!reader.HasRows)
                    {
                        return DatabaseStatesEnums.NOT_INITIALIZED;
                    }
                }

                var state = await dbBase.ExecuteScalarAsync(GET_DATABASE_INITIALIZATION_STATE_FNC_NAME);

                if(state is DBNull)
                {
                    return DatabaseStatesEnums.NOT_INITIALIZED;
                }

                return (DatabaseStatesEnums)(short)state;
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new OutputException(
                    ex,
                    StatusCodes.Status500InternalServerError,
                    MnemoxStatusCodes.CANNOT_CREATE_EXTENSION);
            }
        }

        public async Task DatabaseStateSet(IDbBase dbBase, DatabaseStatesEnums databaseState)
        {
            try
            {
                var parameters = new List<TimescaleParameter> { 
                    new TimescaleParameter
                    {
                        NpgsqlValue = (short)databaseState,
                        ParameterName = STATE_ID_PARAMETER_NAME,
                        NpgsqlDbType = NpgsqlDbType.Smallint
                    }
                };

                await dbBase.ExecuteNonQueryFunctionAsync(SET_DATABASE_STATE_FNC_NAME, parameters);

            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new OutputException(
                    ex,
                    StatusCodes.Status500InternalServerError,
                    MnemoxStatusCodes.CANNOT_SET_DATABASE_STATE_PARAMETER);
            }
        }
    }
}
