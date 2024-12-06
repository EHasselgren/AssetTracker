using Microsoft.EntityFrameworkCore;

public class DbSeeder
{
    private readonly AssetDbContext _context;

    public DbSeeder(AssetDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (await _context.Assets.AnyAsync())
        {
            return;
        }

        if (!await _context.Offices.AnyAsync())
        {
            var offices = new List<Office>
            {
                new Office { Id = 1, Location = "New York", CurrencyCode = "USD" },
                new Office { Id = 2, Location = "Malmö", CurrencyCode = "SEK" },
                new Office { Id = 3, Location = "London", CurrencyCode = "GBP" }
            };

            await _context.Offices.AddRangeAsync(offices);
            await _context.SaveChangesAsync();
        }

        var assets = new List<Asset>
        {
            new Laptop("MacBook Pro M1", DateTime.Now.AddDays(-1005), 1299.99m, "New York") { OfficeId = 1 },
            new MobilePhone("iPhone 14 Pro", DateTime.Now.AddDays(-1020), 999.99m, "London") { OfficeId = 3 },
            new Laptop("Lenovo ThinkPad X1", DateTime.Now.AddDays(-920), 1499.99m, "Malmö") { OfficeId = 2 },
            new MobilePhone("Samsung Galaxy S23", DateTime.Now.AddDays(-940), 899.99m, "London") { OfficeId = 3 },
            new Laptop("Asus ROG", new DateTime(2023, 6, 10), 1799.99m, "New York") { OfficeId = 1 },
            new Laptop("MacBook Air", new DateTime(2023, 8, 5), 999.99m, "Malmö") { OfficeId = 2 },
            new Laptop("Lenovo IdeaPad", new DateTime(2023, 9, 25), 799.99m, "London") { OfficeId = 3 },
            new MobilePhone("iPhone 13", new DateTime(2023, 7, 1), 699.99m, "New York") { OfficeId = 1 },
            new MobilePhone("Nokia G50", new DateTime(2023, 10, 12), 299.99m, "Malmö") { OfficeId = 2 },
            new MobilePhone("Samsung Galaxy A53", new DateTime(2023, 11, 30), 449.99m, "London") { OfficeId = 3 }
        };

        await _context.Assets.AddRangeAsync(assets);
        await _context.SaveChangesAsync();
    }
}