using Mnemox.Account.Models;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Account
{
    public interface IUsersDataManagerTsHelpers
    {
        Task<User> GetUserByUsernameAndPassword(string username, string password);
        Task SetSignedInUser(User user, string signInToken);
    }
}