using Newtonsoft.Json;

namespace Mnemox.Account.Models
{
    public class AuthResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
