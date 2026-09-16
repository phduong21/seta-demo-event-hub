using System.ComponentModel.DataAnnotations;

namespace EventHubDemo.Models.Requests;

public class CancelOrderRequest
{
    [Required]
    public string Reason { get; set; } = string.Empty;
}
