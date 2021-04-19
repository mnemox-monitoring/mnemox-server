using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mnemox.Components.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ComponentTypesEnum
    {
        MACHINE,
        SERVICE
    }
}
