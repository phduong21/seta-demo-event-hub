using System.ComponentModel.DataAnnotations;

namespace EventHubDemo.Models.Events;

public class OrderProcessing
{
    [Required]
    public string OrderId { get; set; } = string.Empty;
}
