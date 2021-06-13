using Mnemox.Shared.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mnemox.DataStorage.Models
{
    public class ServerModel
    {
        [JsonProperty("serverId")]
        public long? ServerId { get; set; }

        [JsonProperty("serverName")]
        public string ServerName { get; set; }

        [JsonProperty("serverState")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatesEnums ServerState { get; set; }
    }
}
