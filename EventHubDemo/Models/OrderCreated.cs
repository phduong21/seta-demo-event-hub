using System.ComponentModel.DataAnnotations;

namespace EventHubDemo.Models;

public class OrderCreated
{
    [Required]
    public string OrderId { get; set; } = string.Empty;

    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public string Currency { get; set; } = string.Empty;
}
