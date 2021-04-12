using Microsoft.AspNetCore.Mvc;

namespace Mnemox.Monitoring.Server.Controllers
{
    public class HeartBeatController : Controller
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
