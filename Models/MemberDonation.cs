namespace netAPI.Models;

public class MemberDonation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public Decimal Amount { get; set; }
    public DateTime DonationDate { get; set; }
}