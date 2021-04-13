using System.Text.Json.Serialization;

namespace Mnemox.Shared.Models
{
    public class Metrics
    {
        [JsonPropertyName("total_cpu_usage_percentage")]
        public double TotalCpuUsagePercentage { get; set; }

        [JsonPropertyName("current_process_cpu_usage_percentage")]
        public double CurrentProcessCpuUsagePercentage { get; set; }

        [JsonPropertyName("memory_capacity_bytes")]
        public ulong MemoryCapacityBytes { get; set; }

        [JsonPropertyName("memory_in_use_bytes")]
        public ulong MemoryUsedBytes { get; set; }

        [JsonPropertyName("memory_in_use_percents")]
        public double MemoryUsedPercents { get; set; }
    }
}
