using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Account.Models
{
    public interface ITokensManager
    {
        Task<Tokens> GetTokenDetails(string token);
    }
}