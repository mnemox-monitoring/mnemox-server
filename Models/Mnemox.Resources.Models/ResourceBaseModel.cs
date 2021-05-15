using Newtonsoft.Json;

namespace Mnemox.Resources.Models
{
    public class ResourceBaseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public ResourcesTypesEnum Type { get; set; }
    }
}
