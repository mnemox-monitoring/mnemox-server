namespace Mnemox.Web.Utils
{
    public interface IWebFilesManager
    {
        WebFile GetFile(string relativePath);
    }
}