using Newtonsoft.Json;

namespace Mnemox.Shared.Models.Requests
{
    public class ServerDetails
    {
        [JsonProperty("serverId")]
        public int? ServerId { get; set; }

        [JsonProperty("serverName")]
        public string ServerName { get; set; }

        [JsonProperty("port")]
        public int? Port { get; set; }
    }
}
