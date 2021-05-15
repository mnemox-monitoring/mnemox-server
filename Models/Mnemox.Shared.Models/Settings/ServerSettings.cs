using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mnemox.Shared.Models.Settings
{
    public class ServerSettings : IServerSettings
    {
        public string BasePath { get; set; }
        public string Database { get; set; }
        public bool Initialized { get; set; }
        public IServiceCollection Services { get; set; }
        public IConfiguration Configuration { get; set; }
    }
}
