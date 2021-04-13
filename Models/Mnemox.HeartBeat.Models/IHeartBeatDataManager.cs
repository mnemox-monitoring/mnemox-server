using System.Threading.Tasks;

namespace Mnemox.HeartBeat.Models
{
    public interface IHeartBeatDataManager
    {
        Task StoreHeartBeat(HeartBeatRequest heartBeatRequest);
    }
}
