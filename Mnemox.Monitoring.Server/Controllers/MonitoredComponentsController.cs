using Microsoft.AspNetCore.Mvc;
using Mnemox.Components.Models;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models;
using System;
using System.Threading.Tasks;

namespace Mnemox.Monitoring.Server.Controllers
{
    [Route("components")]
    [ApiController]
    public class MonitoredComponentsController : MnemoxBaseController
    {
        private readonly ILogsManager _logsManager;

        public MonitoredComponentsController(ILogsManager logsManager)
        {
            _logsManager = logsManager;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterComponent([FromBody]ComponentBaseModel componentBaseModel)
        {
            try
            {


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

            return Ok();
        }
    }
}
