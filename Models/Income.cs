namespace netAPI.Models;

public class Income
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public Decimal Amount { get; set; }
}