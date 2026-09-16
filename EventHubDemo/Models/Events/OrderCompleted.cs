using System.ComponentModel.DataAnnotations;

namespace EventHubDemo.Models.Events;

public class OrderCompleted
{
    [Required]
    public string OrderId { get; set; } = string.Empty;
}
