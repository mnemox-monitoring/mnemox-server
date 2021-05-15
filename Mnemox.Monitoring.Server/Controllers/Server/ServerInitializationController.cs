using Microsoft.AspNetCore.Mvc;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using System;
using System.Threading.Tasks;

namespace Mnemox.Monitoring.Server.Controllers.Server
{
    [Route("server")]
    [ApiController]
    public class ServerInitializationController : MnemoxBaseController
    {
        private readonly ILogsManager _logsManager;

        public ServerInitializationController(ILogsManager logsManager)
        {
            _logsManager = logsManager;
        }

        /// <summary>
        /// Detect if server initialized already
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DetectServerUnitializationStatus()
        {
            try
            {
                

                return Ok();
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
