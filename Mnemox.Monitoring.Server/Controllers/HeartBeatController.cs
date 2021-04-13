using Microsoft.AspNetCore.Mvc;
using Mnemox.HeartBeat.Models;
using Mnemox.Logs.Models;
using System;

namespace Mnemox.Monitoring.Server.Controllers
{
    [Route("heart-beat")]
    [ApiController]
    public class HeartBeatController : MnemoxBaseController
    {
        private readonly ILogsManager _logsManager;

        public HeartBeatController(ILogsManager logsManager)
        {
            _logsManager = logsManager;
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
        public IActionResult Index([FromBody]HeartBeatRequest heartBeatRequest)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logsManager.Error(new ErrorLogStructure(ex));

                return InternalServerErrorResult();
            }

            return Ok();
        }
    }
}
