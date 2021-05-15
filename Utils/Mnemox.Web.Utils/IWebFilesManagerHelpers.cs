namespace Mnemox.Web.Utils
{
    public interface IWebFilesManagerHelpers
    {
        string GetContentType(string extension);
        byte[] GetHtmlFile(string relativePath);
        byte[] ReadAllBytes(string pathToFile);
    }
}