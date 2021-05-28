using Microsoft.AspNetCore.Mvc;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Settings;
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

        public ServerInitializationController(ILogsManager logsManager, IServerSettings serverSettings)
        {
            _logsManager = logsManager;

            _serverSettings = serverSettings;
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
    }
}
