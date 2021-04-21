using Newtonsoft.Json;

namespace Mnemox.Shared.Models
{
    public class ResponseError
    {
        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty("message")]
        public string ErrorMessage { get; set; }
    }
}
