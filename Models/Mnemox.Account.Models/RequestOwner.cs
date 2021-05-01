using System.Collections.Generic;

namespace Mnemox.Account.Models
{
    public class RequestOwner
    {
        public long OwnerId { get; set; }
        public int OwnerTypeId { get; set; }
        public List<long> OwnerTenants { get; set; }
    }
}
