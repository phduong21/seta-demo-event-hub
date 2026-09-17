using System.ComponentModel.DataAnnotations;

namespace EventHubDemo.Models.Events;

public class OrderCancelled
{
    public Guid OrderId { get; set; }

    [Required]
    public string Reason { get; set; } = string.Empty;
}
