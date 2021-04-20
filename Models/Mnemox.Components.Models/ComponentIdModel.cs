using Newtonsoft.Json;

namespace Mnemox.Components.Models
{
    public class ComponentIdModel
    {
        [JsonProperty("id")]
        public long ComponentId { get; set; }
    }
}
