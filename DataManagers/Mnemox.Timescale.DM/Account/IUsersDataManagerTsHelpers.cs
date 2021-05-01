using Mnemox.Account.Models;
using System;
using System.Threading.Tasks;

namespace Mnemox.Timescale.DM.Account
{
    public interface IUsersDataManagerTsHelpers
    {
        Task<User> GetUserByUsernameAndPassword(string username, string password);
        Task<long> SetSignedInUserIntoStorage(User user, string signInToken, DateTime tokenValidUntilUtc);
    }
}