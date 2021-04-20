using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mnemox.Shared.Models;
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

        [NonAction]
        protected ObjectResult CreateErrorResultFromOutputException(OutputException outputException)
        {
            return StatusCode(outputException.HttpStatusCode, CreateErrorDescription(MnemoxStatusCodes.INVALID_MODEL, outputException.Message));
        }

        private string CreateErrorDescription(MnemoxStatusCodes statusCode, string message = null)
        {
            return string.IsNullOrWhiteSpace(message) ?
                JsonSerializer.Serialize(new { error_code = statusCode.ToString() }) :
                JsonSerializer.Serialize(new { error_code = statusCode.ToString(), message });
        }
    }
}
