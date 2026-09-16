using System.ComponentModel.DataAnnotations;

namespace EventHubDemo.Models;

public class OrderCompleted
{
    [Required]
    public string OrderId { get; set; } = string.Empty;
}
