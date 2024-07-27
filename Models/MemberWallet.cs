namespace netAPI.Models;

public class MemberWallet
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string WalletAddress { get; set; }
    public string WalletId { get; set; }
}