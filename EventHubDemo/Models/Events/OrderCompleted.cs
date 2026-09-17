using System.ComponentModel.DataAnnotations;

namespace EventHubDemo.Models.Events;

public class OrderCompleted
{
    public Guid OrderId { get; set; }
}
