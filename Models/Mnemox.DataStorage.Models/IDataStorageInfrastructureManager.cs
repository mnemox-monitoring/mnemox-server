using Mnemox.Shared.Models.Enums;
using System.Threading.Tasks;

namespace Mnemox.DataStorage.Models
{
    public interface IDataStorageInfrastructureManager
    {
        Task<InitializationStatesEnums> InitializeDataStorage(dynamic settings);
    }
}
