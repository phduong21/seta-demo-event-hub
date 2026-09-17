using System.Text.Json.Serialization;

namespace EventHubDemo.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Currency
{
    USD,
    EUR,
    VND
}
