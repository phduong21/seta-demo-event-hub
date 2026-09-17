using System.ComponentModel.DataAnnotations;
using EventHubDemo.Models.Enums;

namespace EventHubDemo.Models.Events;

public class OrderCreated
{
    public Guid OrderId { get; set; }

    public Guid CustomerId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public Currency Currency { get; set; }
}
