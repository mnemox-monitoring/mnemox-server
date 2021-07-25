using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mnemox.Account.Models;
using Mnemox.Account.Models.Enums;
using Mnemox.DataStorage.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using Mnemox.Shared.Models.Requests;
using Mnemox.Shared.Models.Settings;
using Mnemox.Timescale.DM.Dal;
using Mnemox.Timescale.DM.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Monitoring.Server.Controllers.Server
{
    [Route("server")]
    [ApiController]
    public class ServerInitializationController : MnemoxBaseController
    {
        private readonly ILogsManager _logsManager;

        private readonly IServerSettings _serverSettings;

        private readonly IDbFactory _dbFactory;

        private readonly IDataStorageInfrastructureManager _dataStorageInfrastructureManager;

        private readonly IServersManager _serversManager;

        private readonly IUsersDataManager _usersDataManager;

        private const string SERVER_INITIALIZED_ALREADY = "Server initialized already";

        private const string INVALID_DATABASE_DETAILS = "Invalid database details all fields are mandatory";

        private const string INVALID_SERVER_DETAILS = "Invalid server details all fields are mandatory";

        private const string SELECTED_SERVER_IS_ACTIVE = "Selected server is active, you can select only deactivated servers";

        public ServerInitializationController(
            ILogsManager logsManager, 
            IServerSettings serverSettings,
            IDbFactory dbFactory,
            IDataStorageInfrastructureManager dataStorageInfrastructureManager,
            IServersManager serversManager,
            IUsersDataManager usersDataManager)
        {
            _logsManager = logsManager;

            _serverSettings = serverSettings;

            _dbFactory = dbFactory;

            _dataStorageInfrastructureManager = dataStorageInfrastructureManager;

            _serversManager = serversManager;

            _usersDataManager = usersDataManager;
        }

        /// <summary>
        /// Detect if server initialized already
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("init-status")]
        public async Task<IActionResult> DetectServerInitializationStatus()
        {
            try
            {
                return Ok(new { isInitialized = _serverSettings.Initialized });
            }
            catch (OutputException ex)
            {
                return CreateErrorResultFromOutputException(ex);
            }
            catch (HandledException)
            {
                return InternalServerErrorResult();
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                return InternalServerErrorResult();
            }
        }

        /// <summary>
        /// Database existence validation
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("validate/database")]
        public async Task<IActionResult> ValidateDatabaseExistence([FromBody] DatabaseDetails databaseDetails)
        {
            IDbBase dbBase = null;

            try
            {
                if (_serverSettings.Initialized)
                {
                    throw new OutputException(
                        new Exception(SERVER_INITIALIZED_ALREADY),
                        StatusCodes.Status401Unauthorized,
                        MnemoxStatusCodes.UNAUTHORIZED);
                }

                if (string.IsNullOrWhiteSpace(databaseDetails.Address) ||
                   string.IsNullOrWhiteSpace(databaseDetails.Username) ||
                   string.IsNullOrWhiteSpace(databaseDetails.Password))
                {
                    throw new OutputException(
                        new Exception(INVALID_DATABASE_DETAILS),
                        StatusCodes.Status400BadRequest,
                        MnemoxStatusCodes.INVALID_MODEL);
                }

                var connectionString = _dbFactory.CreateConnectionString(
                    databaseDetails.Address,
                    databaseDetails.Username,
                    databaseDetails.Password,
                    databaseDetails.Port);

                dbBase = _dbFactory.GetDbBase(connectionString);

                try
                {
                    await dbBase.ConnectAsync();
                }
                catch (Exception ex)
                {
                    throw new OutputException(ex, StatusCodes.Status400BadRequest, MnemoxStatusCodes.CANNOT_CONNECT_TO_THE_DATABASE);
                }

                var serverInitializationState = await _dataStorageInfrastructureManager.InitializeDataStorage(
                    new InfrastructureSettings
                    {
                        ConnectonString = connectionString
                    }
                );

                var servers = await _serversManager.GetServersListByStateOrAllAsync(StatesEnums.INACTIVE);

                return Ok(
                    servers != null ?
                    new { serverInitializationState, servers } : 
                    new { serverInitializationState, servers = new List<ServerModel>() }) ;
            }
            catch (OutputException ex)
            {
                return CreateErrorResultFromOutputException(ex);
            }
            catch (HandledException)
            {
                return InternalServerErrorResult();
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                return InternalServerErrorResult();
            }
            finally
            {
                await dbBase?.DisconnectAsync();
            }
        }

        /// <summary>
        /// Database existence validation
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("init")]
        public async Task<IActionResult> InitServer([FromBody] ServerDetails serverDetails)
        {
            try
            {
                if (_serverSettings.Initialized)
                {
                    throw new OutputException(
                        new Exception(SERVER_INITIALIZED_ALREADY),
                        StatusCodes.Status401Unauthorized,
                        MnemoxStatusCodes.UNAUTHORIZED);
                }

                if (string.IsNullOrWhiteSpace(serverDetails.ServerName))
                {
                    throw new OutputException(
                        new Exception(INVALID_SERVER_DETAILS),
                        StatusCodes.Status400BadRequest,
                        MnemoxStatusCodes.INVALID_MODEL);
                };

                var server = new ServerModel();

                if (serverDetails.ServerId != null)
                {
                    server = await _serversManager.GetServerDetailsById(serverDetails.ServerId.Value);

                    if (server.ServerState == StatesEnums.ACTIVE)
                    {
                        throw new OutputException(
                            new Exception(SELECTED_SERVER_IS_ACTIVE),
                            StatusCodes.Status400BadRequest,
                            MnemoxStatusCodes.SELECTED_SERVER_ID_ACTIVE);
                    }
                }
                else
                {
                    server.ServerState = StatesEnums.INACTIVE;

                    server.ServerId = await _serversManager.Add(
                            new ServerModel
                            {
                                ServerName = serverDetails.ServerName,
                                ServerState = server.ServerState
                            }
                        );
                }

                var systemOwner = await _usersDataManager.GetUsersByRole(RolesEnum.SystemOwner);

                return Ok(new { 
                    serverId = server.ServerId, 
                    serverState = server.ServerState, 
                    systemOwnerExists = systemOwner != null });
            }
            catch (OutputException ex)
            {
                return CreateErrorResultFromOutputException(ex);
            }
            catch (HandledException)
            {
                return InternalServerErrorResult();
            }
            catch (Exception ex)
            {
                await _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                return InternalServerErrorResult();
            }
        }
    }
}
