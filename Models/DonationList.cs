namespace netAPI.Models;

public class DonationList
{
    public int id { get; set; }
    public int event_id { get; set; }
    public string name { get; set; }
    public decimal total_amount { get; set; }
    public string? description { get; set; }
    public string? ImageUrl { get; set; }
}