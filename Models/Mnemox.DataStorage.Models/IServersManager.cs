using Mnemox.Shared.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.DataStorage.Models
{
    public interface IServersManager
    {
        Task<int> Add(ServerModel serverModel);
        Task<StatesEnums?> GetLastInitializationState();
        Task<ServerModel> GetServerDetailsById(long serverId);
        Task<List<ServerModel>> GetServersListByStateOrAllAsync(StatesEnums? state = null);
        Task ServerInitizalizationStateSet(InitializationStatesEnums databaseState);
    }
}
