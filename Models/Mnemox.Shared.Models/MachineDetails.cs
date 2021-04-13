using System;
using System.Text.Json.Serialization;

namespace Mnemox.Shared.Models
{
    public class MachineDetails
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
    }
}
