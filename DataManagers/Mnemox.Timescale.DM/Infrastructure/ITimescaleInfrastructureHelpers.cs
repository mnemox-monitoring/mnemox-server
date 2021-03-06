using Mnemox.Shared.Models.Enums;
using Mnemox.Timescale.DM.Dal;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Infrastructure
{
    public interface ITimescaleInfrastructureHelpers
    {
        Task CreateExtension(IDbBase dbBase, string extensionName, string schema);
        Task CreateSchema(IDbBase dbBase, string schemaName);
        Task<InitializationStatesEnums> InitializationState(IDbBase dbBase);
        Task DropSchema(IDbBase dbBase, string schema);
        string LoadFile(string path);
        Task RunNonQuery(IDbBase dbBase, string query);
        void ReInitConnectionString(InfrastructureSettings infrastructureSettings);
    }
}