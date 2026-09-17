using System.ComponentModel.DataAnnotations;

namespace EventHubDemo.Models.Events;

public class OrderProcessing
{
    public Guid OrderId { get; set; }
}
