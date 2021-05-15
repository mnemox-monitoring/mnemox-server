using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mnemox.Shared.Models.Settings
{
    public interface IServerSettings
    {
        string BasePath { get; set; }
        string Database { get; set; }
        IServiceCollection Services { get; set; }
        IConfiguration Configuration { get; set; }
        bool Initialized { get; set; }
    }
}