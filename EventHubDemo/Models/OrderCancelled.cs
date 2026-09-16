using System.ComponentModel.DataAnnotations;

namespace EventHubDemo.Models;

public class OrderCancelled
{
    [Required]
    public string OrderId { get; set; } = string.Empty;

    [Required]
    public string Reason { get; set; } = string.Empty;
}
