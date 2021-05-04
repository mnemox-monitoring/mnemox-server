using Mnemox.Shared.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnemox.Account.Models
{
    public interface ITenantObjectsManager
    {
        Task<List<Tenant>> GetObjectTenantsAsync(long objectId, MnemoxAccessObjectsTypesEnum mnemoxAccessObjectsTypes);
    }
}
