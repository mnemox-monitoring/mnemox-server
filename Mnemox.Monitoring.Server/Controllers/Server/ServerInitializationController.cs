using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Requests;
using Mnemox.Shared.Models.Settings;
using Mnemox.Timescale.DM.Dal;
using System;
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

        private const string SERVER_INITIALIZED_ALREADY = "Server initialized already";

        private const string INVALID_DATABASE_DETAILS = "Invalid database details all fields are mandatory";

        public ServerInitializationController(
            ILogsManager logsManager, 
            IServerSettings serverSettings,
            IDbFactory dbFactory)
        {
            _logsManager = logsManager;

            _serverSettings = serverSettings;

            _dbFactory = dbFactory;
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

                if(string.IsNullOrWhiteSpace(databaseDetails.Address) || 
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

                return Ok(new { databaseServerExists = true });
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
    }
}
