using Mnemox.Account.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Account.Models
{
    public interface IUsersDataManager
    {
        Task<long> CreateUser(UserSignUp createUserReques);
        Task<List<User>> GetUsersByRole(RolesEnum rolesEnum);
        Task<AuthResponse> SignIn(AuthRequest authRequest);
    }
}
