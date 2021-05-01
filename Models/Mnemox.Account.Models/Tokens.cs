using System;

namespace Mnemox.Account.Models
{
    public class Tokens
    {
        public long TokenId { get; set; }
        public string Token { get; set; }
        public long OwnerId { get; set; }
        public int OwnerTypeId { get; set; }
        public DateTime ValidUntilDateTimeUtc { get; set; }
    }
}
