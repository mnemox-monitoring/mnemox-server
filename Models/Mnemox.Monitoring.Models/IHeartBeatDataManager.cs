using System.Threading.Tasks;

namespace Mnemox.Monitoring.Models
{
    public interface IHeartBeatDataManager
    {
        Task StoreHeartBeat(HeartBeatRequest heartBeatRequest);
    }
}
