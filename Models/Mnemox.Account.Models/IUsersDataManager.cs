using System.Threading.Tasks;

namespace Mnemox.Account.Models
{
    public interface IUsersDataManager
    {
        Task<AuthResponse> SignIn(AuthRequest authRequest);
    }
}
