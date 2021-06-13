using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mnemox.Account.Models;
using Mnemox.Shared.Models;
using Mnemox.Shared.Models.Enums;
using System.Text.Json;

namespace Mnemox.Monitoring.Server.Controllers
{
    public class MnemoxBaseController : ControllerBase
    {
        [NonAction]
        protected ObjectResult InternalServerErrorResult(string message = null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError, 
                CreateErrorDescription(MnemoxStatusCodes.INTERNAL_SERVER_ERROR, message ?? "Internal server error"));
        }

        [NonAction]
        protected ObjectResult CreateErrorResultFromOutputException(OutputException outputException)
        {
            return StatusCode(outputException.HttpStatusCode, CreateErrorDescription(outputException.MnemoxStatusCode, outputException.Message));
        }

        [NonAction]
        protected ObjectResult CreateNotFound(string message)
        {
            return NotFound(CreateErrorDescription(MnemoxStatusCodes.NOT_FOUND, message));
        }

        private string CreateErrorDescription(MnemoxStatusCodes statusCode, string message = null)
        {
            return string.IsNullOrWhiteSpace(message) ?
                JsonSerializer.Serialize(new { errorCode = statusCode.ToString() }) :
                JsonSerializer.Serialize(new { errorCode = statusCode.ToString(), message });
        }

        public RequestOwner RequestOwner
        { 
            get
            {
                if (Request.HttpContext.Items.TryGetValue(UrlAndContextPropertiesNames.REQUEST_OWNER, out object requestUser))
                {
                    return (RequestOwner)requestUser;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
