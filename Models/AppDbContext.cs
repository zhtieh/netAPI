using Microsoft.EntityFrameworkCore;

namespace netAPI.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Item { get; set; } = null!;

    public DbSet<Member> Member { get; set; } = null!;

    public DbSet<MemberDonation> MemberDonation { get; set; } = null!;

    public DbSet<MemberWallet> MemberWallet { get; set; } = null!;
}