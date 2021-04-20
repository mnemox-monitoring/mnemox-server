using Newtonsoft.Json;

namespace Mnemox.Components.Models
{
    public class ComponentBaseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]

        public ComponentTypesEnum Type { get; set; }
    }
}
