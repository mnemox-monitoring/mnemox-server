using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mnemox.Shared.Models.Settings
{
    public class ApiConfiguration : IApiConfiguration
    {
        public IServiceCollection Services { get; set; }
        public IConfiguration Configuration { get; set; }
    }
}
