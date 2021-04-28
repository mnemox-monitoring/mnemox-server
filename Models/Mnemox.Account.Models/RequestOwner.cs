using Mnemox.Shared.Models.Enums;
using System.Collections.Generic;

namespace Mnemox.Account.Models
{
    public class RequestOwner
    {
        public long OwnerId { get; set; }
        public List<long> OwnerTenants { get; set; }
        public MnemoxAccessObjectsTypes OwnerType { get; set; }
    }
}
