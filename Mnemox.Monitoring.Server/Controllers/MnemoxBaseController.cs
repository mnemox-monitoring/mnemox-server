using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mnemox.Shared.Models.Enums;
using System.Text.Json;

namespace Mnemox.Monitoring.Server.Controllers
{
    public class MnemoxBaseController : ControllerBase
    {
        [NonAction]
        protected ObjectResult InternalServerErrorResult()
        {
            return StatusCode(StatusCodes.Status500InternalServerError, CreateErrorDescription(MnemoxStatusCodes.INTERNAL_SERVER_ERROR));
        }

        private string CreateErrorDescription(MnemoxStatusCodes statusCode)
        {
            return JsonSerializer.Serialize(new { error_code = statusCode.ToString() });
        }
    }
}
