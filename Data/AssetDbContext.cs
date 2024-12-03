using Microsoft.EntityFrameworkCore;

public class AssetDbContext : DbContext
{
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Office> Offices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=Elias-Laptop;Database=AssetTrackingDB;Trusted_Connection=True;TrustServerCertificate=True;"
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>()
            .HasDiscriminator<string>("AssetType")
            .HasValue<Laptop>("Laptop")
            .HasValue<MobilePhone>("MobilePhone");

        modelBuilder.Entity<Asset>()
            .HasOne(a => a.Office)
            .WithMany(o => o.Assets)
            .HasForeignKey(a => a.OfficeId);

        modelBuilder.Entity<Office>().HasData(
            new Office { Id = 1, Location = "New York", CurrencyCode = "USD" },
            new Office { Id = 2, Location = "Malmö", CurrencyCode = "SEK" },
            new Office { Id = 3, Location = "London", CurrencyCode = "GBP" }
        );
    }
}