
using Newtonsoft.Json;

namespace Mnemox.Account.Models
{
    public class UserSignUp: UserBase
    {
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
