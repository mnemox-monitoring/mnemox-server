using System.Text.Json.Serialization;

namespace Mnemox.Monitoring.Models
{
    public class MachineDetails
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
    }
}
