using Newtonsoft.Json;
using System.Text.Json;

namespace Mnemox.Account.Models
{
    public class UserBase
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }
    }
}
