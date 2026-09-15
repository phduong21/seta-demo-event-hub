using System.ComponentModel.DataAnnotations;

namespace EventHubDemo.Configuration;

public class EventHubOptions
{
    public const string SectionName = "EventHub";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    public string ConsumerGroup { get; set; } = "$Default";
}
