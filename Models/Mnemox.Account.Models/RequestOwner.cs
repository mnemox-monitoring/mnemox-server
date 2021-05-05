using Mnemox.Shared.Models.Enums;
using System.Collections.Generic;

namespace Mnemox.Account.Models
{
    public class RequestOwner
    {
        public long OwnerId { get; set; }
        public MnemoxAccessObjectsTypesEnum MnemoxAccessObjectsType { get; set; }
        public List<Tenant> OwnerTenants { get; set; }
    }
}
