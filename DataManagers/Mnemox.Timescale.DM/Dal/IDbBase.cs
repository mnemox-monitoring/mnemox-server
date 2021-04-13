using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Dal
{
    public interface IDbBase
    {
        Task ConnectAsync();
        Task DisconnectAsync();
    }
}