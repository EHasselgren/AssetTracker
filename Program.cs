using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            using (var context = new AssetDbContext())
            {
                await context.Database.MigrateAsync();

                var seeder = new DbSeeder(context);
                await seeder.SeedAsync();
            }

            var assetManager = new AssetManager();

            Console.WriteLine("Displaying all assets from database:");
            await assetManager.PrintSortedAssetsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            }
        }
    }
}