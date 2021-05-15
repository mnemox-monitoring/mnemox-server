using Mnemox.Logs.Models;
using Mnemox.Shared.Models.Settings;
using System;
using System.IO;

namespace Mnemox.Web.Utils
{
    public class WebFilesManager : IWebFilesManager
    {
        private readonly IServerSettings _serverSettings;

        private readonly ILogsManager _logsManager;

        private readonly IWebFilesManagerHelpers _webFilesManagerHelpers;

        private const string HTML_CONTENT_TYPE = "text/html";

        private const string BASE_URL = "client";

        public WebFilesManager(
            IServerSettings serverSettings,
            ILogsManager logsManager,
            IWebFilesManagerHelpers webFilesManagerHelpers)
        {
            _serverSettings = serverSettings;

            _logsManager = logsManager;

            _webFilesManagerHelpers = webFilesManagerHelpers;
        }

        public WebFile GetFile(string relativePath)
        {
            try
            {
                var webFile = new WebFile();

                if (relativePath.StartsWith("/"))
                {
                    relativePath = relativePath.Substring(1);
                }

                var extension = Path.GetExtension(relativePath);

                if (string.IsNullOrWhiteSpace(extension))
                {
                    var pathToFile = Path.Combine(_serverSettings.BasePath, BASE_URL);

                    webFile.File = _webFilesManagerHelpers.GetHtmlFile(pathToFile);

                    webFile.ContentType = HTML_CONTENT_TYPE;
                }
                else
                {
                    var pathToFile = Path.Combine(_serverSettings.BasePath, relativePath);

                    webFile.File = _webFilesManagerHelpers.ReadAllBytes(pathToFile);

                    webFile.ContentType = _webFilesManagerHelpers.GetContentType(extension);
                }

                return webFile;
            }
            catch (Exception ex)
            {
                _logsManager.ErrorAsync(new ErrorLogStructure(ex).WithErrorSource());

                throw;
            }
        }


    }
}
