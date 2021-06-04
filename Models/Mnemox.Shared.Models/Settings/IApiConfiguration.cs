using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mnemox.Shared.Models.Settings
{
    public interface IApiConfiguration
    {
        IConfiguration Configuration { get; set; }
        IServiceCollection Services { get; set; }
    }
}