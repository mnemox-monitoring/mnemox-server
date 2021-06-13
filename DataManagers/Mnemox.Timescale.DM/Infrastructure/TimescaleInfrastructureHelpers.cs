using Microsoft.AspNetCore.Http;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Shared.Models.Settings;
using Mnemox.Timescale.DM.Dal;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Infrastructure
{
    public class TimescaleInfrastructureHelpers : ITimescaleInfrastructureHelpers
    {
        private readonly ILogsManager _logsManager;

        private readonly ISettingsManager _settingsManager;

        private const string SELECT_SERVER_SCHEMA_INFO = "select * from information_schema.schemata where schema_name = 'server';";

        private const string SELECT_DATABASE_STATE_TABLE_INFO = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'server' AND table_type = 'BASE TABLE' and table_name = 'database_states';";

        private const string GET_INITIALIZATION_STATE_FNC_NAME = "server.initialization_states_get_last";

        public TimescaleInfrastructureHelpers(ILogsManager logsManager, ISettingsManager settingsManager)
        {
            _logsManager = logsManager;

            _settingsManager = settingsManager;
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

        public async Task<InitializationStatesEnums> InitializationState(IDbBase dbBase)
        {
            try
            {
                using (var reader = await dbBase.ExecuteReaderQueryAsync(SELECT_SERVER_SCHEMA_INFO))
                {
                    if (!reader.HasRows)
                    {
                        return InitializationStatesEnums.DATABASE_NOT_INITIALIZED;
                    }
                }

                using (var reader = await dbBase.ExecuteReaderQueryAsync(SELECT_DATABASE_STATE_TABLE_INFO))
                {
                    if (!reader.HasRows)
                    {
                        return InitializationStatesEnums.DATABASE_NOT_INITIALIZED;
                    }
                }

                var state = await dbBase.ExecuteScalarAsync(GET_INITIALIZATION_STATE_FNC_NAME);

                if(state is DBNull)
                {
                    return InitializationStatesEnums.DATABASE_NOT_INITIALIZED;
                }

                return (InitializationStatesEnums)(short)state;
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

        public void ReInitConnectionString(InfrastructureSettings infrastructureSettings)
        {
            _settingsManager.FullSettings.DbFactorySettings.ConnectionString = infrastructureSettings.ConnectonString;
        }
    }
}
