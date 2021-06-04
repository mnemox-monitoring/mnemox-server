using Microsoft.AspNetCore.Http;
using Mnemox.DataStorage.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Shared.Models.Settings;
using Mnemox.Timescale.DM.Dal;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Infrastructure
{
    public class TimescaleInfrastructure : IDataStorageInfrastructureManager
    {
        private readonly ILogsManager _logsManager;

        private readonly IDbFactory _dbFactory;

        private readonly ITimescaleInfrastructureHelpers _timescaleInfrastructureHelpers;

        private readonly ISettingsManager _settingsManager;

        private const string PATH_TO_DATABASE_INITIALIZATION_FILES_FOLDER = "Files/Database/Timescaledb";

        private const string TABLES_AND_FUNCTIONS_FILE = "core-tables-and-functions-initialization.sql";

        public TimescaleInfrastructure(
            ILogsManager logsManager, 
            IDbFactory dbFactory, 
            ITimescaleInfrastructureHelpers timescaleInfrastructureHelpers,
            ISettingsManager settingsManager)
        {
            _logsManager = logsManager;

            _dbFactory = dbFactory;

            _timescaleInfrastructureHelpers = timescaleInfrastructureHelpers;

            _settingsManager = settingsManager;
        }


        public async Task InitializeDataStorage(dynamic settings)
        {
            IDbBase db = null;

            try
            {
                InfrastructureSettings infrastructureSettings = settings;

                db = _dbFactory.GetDbBase(infrastructureSettings.ConnectonString);

                await db.ConnectAsync();

                var databaseInitializetionState = await _timescaleInfrastructureHelpers.DatabaseInitializationState(db);

                if(databaseInitializetionState == DatabaseStatesEnums.INITIALIZED)
                {
                    return;
                }
                
                await _timescaleInfrastructureHelpers.CreateSchema(db, "server");

                await _timescaleInfrastructureHelpers.CreateSchema(db, "monitoring");

                await _timescaleInfrastructureHelpers.CreateExtension(db, "timescaledb", "monitoring");

                await _timescaleInfrastructureHelpers.CreateSchema(db, "resources");

                await _timescaleInfrastructureHelpers.CreateSchema(db, "tenants");

                await _timescaleInfrastructureHelpers.DropSchema(db, "public");

                await _timescaleInfrastructureHelpers.CreateExtension(db, "pgcrypto", "tenants");

                var pathToInitializationFile = Path.Combine(
                    _settingsManager.FullSettings.ServerSettings.BasePath, 
                    PATH_TO_DATABASE_INITIALIZATION_FILES_FOLDER,
                    TABLES_AND_FUNCTIONS_FILE);

                var tablesAndFunctionsInitialization = _timescaleInfrastructureHelpers.LoadFile(
                        pathToInitializationFile
                    );

                await _timescaleInfrastructureHelpers.RunNonQuery(db, tablesAndFunctionsInitialization);

                _settingsManager.FullSettings.DbFactorySettings.ConnectionString = infrastructureSettings.ConnectonString;

                await _settingsManager.ReloadSettingsAsync();

                await _timescaleInfrastructureHelpers.DatabaseStateSet(db, DatabaseStatesEnums.INITIALIZED);
            }
            catch (OutputException)
            {
                throw;
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw new OutputException(ex, StatusCodes.Status500InternalServerError, MnemoxStatusCodes.INTERNAL_SERVER_ERROR);
            }
            finally
            {
                await db?.DisconnectAsync();
            }
        }
    }
}
