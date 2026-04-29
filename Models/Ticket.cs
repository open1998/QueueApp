namespace QueueApp.Models;

public enum TicketStatus
{
    Waiting,
    Calling,
    Served
}

public class Ticket
{
    public int Id { get; set; }
    public int TicketNumber { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public TicketStatus Status { get; set; }
    public int? CounterId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CalledAt { get; set; }
}
