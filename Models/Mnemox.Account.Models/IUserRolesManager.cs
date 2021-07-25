using System.Threading.Tasks;

namespace Mnemox.Account.Models
{
    public interface IUserRolesManager
    {
        Task AddUserRole(long userId, short roleId);
    }
}
