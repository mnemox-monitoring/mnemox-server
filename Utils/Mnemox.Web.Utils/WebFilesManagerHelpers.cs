using System.IO;

namespace Mnemox.Web.Utils
{
    public class WebFilesManagerHelpers : IWebFilesManagerHelpers
    {
        private const string INDEX_HTML = "index.html";

        public string GetContentType(string extension)
        {
            const string CSS_EXTENSION = ".css";
            const string CSS_CONTENT_TYPE = "text/css";
            const string DEFAULT_CONTENT_TYPE = "text/plain";
            const string JS_EXTENSION = ".js";
            const string JS_CONTENT_TYPE = "text/javascript";

            return extension switch
            {
                CSS_EXTENSION => CSS_CONTENT_TYPE,
                JS_EXTENSION => JS_CONTENT_TYPE,
                _ => DEFAULT_CONTENT_TYPE,
            };
        }

        public byte[] GetHtmlFile(string pathToFile)
        {
            var pathToHtmlFile = Path.Combine(pathToFile, INDEX_HTML);

            return ReadAllBytes(pathToHtmlFile);
        }

        public byte[] ReadAllBytes(string pathToFile)
        {
            return File.ReadAllBytes(pathToFile);
        }
    }
}
