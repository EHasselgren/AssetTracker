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

        var assets = new List<Asset>
        {
            new Laptop("MacBook Pro M1", new DateTime(2023, 1, 15), 1299.99m, "New York"),
            new Laptop("Lenovo ThinkPad X1", new DateTime(2023, 3, 20), 1499.99m, "London"),
            new Laptop("Asus ROG", new DateTime(2023, 6, 10), 1799.99m, "New York"),
            new Laptop("MacBook Air", new DateTime(2023, 8, 5), 999.99m, "London"),
            new Laptop("Lenovo IdeaPad", new DateTime(2023, 9, 25), 799.99m, "New York"),

            new MobilePhone("iPhone 14 Pro", new DateTime(2023, 2, 1), 999.99m, "New York"),
            new MobilePhone("Samsung Galaxy S23", new DateTime(2023, 4, 15), 899.99m, "London"),
            new MobilePhone("iPhone 13", new DateTime(2023, 7, 1), 699.99m, "New York"),
            new MobilePhone("Nokia G50", new DateTime(2023, 10, 12), 299.99m, "London"),
            new MobilePhone("Samsung Galaxy A53", new DateTime(2023, 11, 30), 449.99m, "New York")
        };

        await _context.Assets.AddRangeAsync(assets);
        await _context.SaveChangesAsync();
    }
}