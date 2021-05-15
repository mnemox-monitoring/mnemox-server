using Microsoft.AspNetCore.Mvc;
using Mnemox.Logs.Models;
using Mnemox.Shared.Models.Settings;
using Mnemox.Web.Utils;
using System.IO;

namespace Mnemox.Monitoring.Server.Controllers.WebClient
{
    [Route("client/{*url}")]
    [ApiController]
    public class WebClientPagesController : ControllerBase
    {
        private readonly ILogsManager _logsManager;

        private readonly IServerSettings _serverSettings;

        private readonly IWebFilesManager _webFilesManager;

        public WebClientPagesController(ILogsManager logsManager, IServerSettings serverSettings, IWebFilesManager webFilesManager)
        {
            _logsManager = logsManager;

            _serverSettings = serverSettings;

            _webFilesManager = webFilesManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var file = _webFilesManager.GetFile(Request.Path);


            return File(file.File, file.ContentType);
        }
    }
}
