using Mnemox.Shared.Models;
using System;
using System.Text.Json.Serialization;

namespace Mnemox.HeartBeat.Models
{
    public class HeartBeatRequest
    {
        [JsonPropertyName("instance_id")]
        public long InstanceId { get; set; }

        /// <summary>
        /// Optional parameter
        /// Machine/Application date and time in UTC
        /// </summary>
        [JsonPropertyName("created_utc")]
        public DateTime CreatedUtc { get; set; }

        /// <summary>
        /// Optional parameter
        /// </summary>
        [JsonPropertyName("machine_details")]
        public MachineDetails MachineDetails { get; set; }

        /// <summary>
        /// Optional parameter
        /// </summary>
        [JsonPropertyName("metrics")]
        public Metrics Metrics { get; set; }
    }
}
