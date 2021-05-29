using Newtonsoft.Json;

namespace Mnemox.Shared.Models.Requests
{
    public class DatabaseDetails
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("port")]
        public int? Port { get; set; }
    }
}
