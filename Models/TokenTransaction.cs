namespace netAPI.Models;

public class TokenTransaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? WalletAddress { get; set; }
    public Decimal Amount { get; set; }
    public int mbId { get; set; }
    public string? Status { get; set; }
    public string? TxnHash { get; set; }
}