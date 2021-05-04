using Mnemox.Shared.Models.Enums;
using System;

namespace Mnemox.Account.Models
{
    public class Tokens
    {
        public long TokenId { get; set; }
        public string Token { get; set; }
        public long OwnerId { get; set; }
        public MnemoxAccessObjectsTypesEnum MnemoxAccessObjectsType { get; set; }
        public DateTime ValidUntilDateTimeUtc { get; set; }
    }
}
