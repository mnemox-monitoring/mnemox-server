using Microsoft.AspNetCore.Mvc;

namespace Mnemox.Monitoring.Server.Controllers.WebClient
{
    [Route("web-client/settings")]
    [ApiController]
    public class WebClientSettings : MnemoxBaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
