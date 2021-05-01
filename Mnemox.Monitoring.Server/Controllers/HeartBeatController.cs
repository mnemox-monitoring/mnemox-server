using Microsoft.AspNetCore.Mvc;
using Mnemox.Api.Security.Utils;
using Mnemox.Logs.Models;
using Mnemox.Monitoring.Models;
using Mnemox.Shared.Models;
using System;
using System.Threading.Tasks;

namespace Mnemox.Monitoring.Server.Controllers
{
    [ServiceFilter(typeof(AuthenticationFilter))]
    [TenantContextValidationFilter]
    [Route("tenant/{tenantId:long}/heart-beat")]
    [ApiController]
    public class HeartBeatController : MnemoxBaseController
    {
        private readonly ILogsManager _logsManager;

        private readonly IHeartBeatDataManager _heartBeatDataManager;

        public HeartBeatController(ILogsManager logsManager, IHeartBeatDataManager heartBeatDataManager)
        {
            _logsManager = logsManager;

            _heartBeatDataManager = heartBeatDataManager;
        }

        /// <summary>
        /// Inserts heart beat
        /// </summary>
        /// <remarks>
        /// ### Json Properties
        /// - machine_details -> optional
        /// - metrics -> optional
        /// </remarks>
        /// <param name="heartBeatRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> StoreHeartBeat([FromBody]HeartBeatRequest heartBeatRequest)
        {
            try
            {
                await _heartBeatDataManager.StoreHeartBeat(heartBeatRequest);
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
