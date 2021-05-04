using System;
using System.Threading.Tasks;

namespace Mnemox.Account.Models
{
    public interface ITokensManager
    {
        Task<Tokens> GetTokenDetailsFromDataStorgeAsync(string token);
        double GetTokenTtlMinutes(DateTime tokenValidUntilDateTimeUtc);
    }
}