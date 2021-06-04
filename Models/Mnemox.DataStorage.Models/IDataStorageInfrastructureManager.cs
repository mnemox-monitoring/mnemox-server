using System.Threading.Tasks;

namespace Mnemox.DataStorage.Models
{
    public interface IDataStorageInfrastructureManager
    {
        Task InitializeDataStorage(dynamic settings);
    }
}
