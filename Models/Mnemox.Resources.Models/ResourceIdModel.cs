using Newtonsoft.Json;

namespace Mnemox.Resources.Models
{
    public class ResourceIdModel
    {
        [JsonProperty("resource_id")]
        public long ResourceId { get; set; }
    }
}
