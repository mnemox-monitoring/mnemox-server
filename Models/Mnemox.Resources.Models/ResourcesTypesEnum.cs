using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mnemox.Resources.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResourcesTypesEnum
    {
        MACHINE,
        SERVICE
    }
}
