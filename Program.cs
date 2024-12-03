using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Console.Title = "Asset Management System";
            using (var context = new AssetDbContext())
            {
                await context.Database.MigrateAsync();
                var seeder = new DbSeeder(context);
                await seeder.SeedAsync();
            }

            var assetManager = new AssetManager();
            bool running = true;

            await DisplayHeader("Current Assets");
            await assetManager.PrintSortedAssetsAsync();
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            while (running)
            {
                await DisplayMainMenu();
                var keyInfo = Console.ReadKey(true);

                switch (keyInfo.KeyChar)
                {
                    case '1':
                        await DisplayHeader("All Assets");
                        await assetManager.PrintSortedAssetsAsync();
                        await PauseAndContinue();
                        break;

                    case '2':
                        await DisplayHeader("Add New Asset");
                        await AddNewAsset(assetManager);
                        await PauseAndContinue();
                        break;

                    case '3':
                        await DisplayHeader("Update Asset");
                        await UpdateExistingAsset(assetManager);
                        await PauseAndContinue();
                        break;

                    case '4':
                        await DisplayHeader("Delete Asset");
                        await DeleteExistingAsset(assetManager);
                        await PauseAndContinue();
                        break;

                    case '5':
                        await DisplayHeader("Asset Report");
                        await assetManager.GenerateReportAsync();
                        await PauseAndContinue();
                        break;

                    case '6':
                        await DisplayHeader("Exiting Application");
                        Console.WriteLine("Thank you for using Asset Management System!");
                        running = false;
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid option. Please try again.");
                        await Task.Delay(1500);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            }
            await PauseAndContinue();
        }
    }

    private static async Task DisplayMainMenu()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════╗");
        Console.WriteLine("║     Asset Management System     ║");
        Console.WriteLine("╠════════════════════════════════╣");
        Console.WriteLine("║ 1. View All Assets             ║");
        Console.WriteLine("║ 2. Add New Asset               ║");
        Console.WriteLine("║ 3. Update Asset                ║");
        Console.WriteLine("║ 4. Delete Asset                ║");
        Console.WriteLine("║ 5. Generate Report             ║");
        Console.WriteLine("║ 6. Exit                        ║");
        Console.WriteLine("╚════════════════════════════════╝");
        Console.Write("\nSelect an option: ");
    }

    private static async Task DisplayHeader(string title)
    {
        Console.Clear();
        Console.WriteLine("╔═" + new string('═', title.Length) + "═╗");
        Console.WriteLine("║ " + title + " ║");
        Console.WriteLine("╚═" + new string('═', title.Length) + "═╝\n");
    }

    private static async Task PauseAndContinue()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static async Task AddNewAsset(AssetManager assetManager)
    {
        Console.WriteLine("Select Asset Type:");
        Console.WriteLine("1. Laptop");
        Console.WriteLine("2. Mobile Phone");
        Console.Write("\nSelect type: ");

        string typeChoice = Console.ReadLine();

        Console.Write("\nEnter model name: ");
        string modelName = Console.ReadLine();

        Console.Write("Enter price in USD: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("\nInvalid price format.");
            return;
        }

        Console.WriteLine("\nSelect Office Location:");
        Console.WriteLine("1. New York (USD)");
        Console.WriteLine("2. Malmö (SEK)");
        Console.WriteLine("3. London (GBP)");
        Console.Write("\nSelect office: ");

        if (!int.TryParse(Console.ReadLine(), out int officeId) || officeId < 1 || officeId > 3)
        {
            Console.WriteLine("\nInvalid office selection.");
            return;
        }

        Asset newAsset = typeChoice switch
        {
            "1" => new Laptop(modelName, DateTime.Now, price, GetOfficeLocation(officeId)),
            "2" => new MobilePhone(modelName, DateTime.Now, price, GetOfficeLocation(officeId)),
            _ => throw new ArgumentException("Invalid asset type")
        };
        newAsset.OfficeId = officeId;

        await assetManager.AddAssetAsync(newAsset);
        Console.WriteLine("\n✓ Asset added successfully!");
    }

    private static async Task UpdateExistingAsset(AssetManager assetManager)
    {
        await assetManager.PrintSortedAssetsAsync();

        Console.Write("\nEnter the ID of the asset to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nInvalid ID format.");
            return;
        }

        var asset = await assetManager.GetAssetByIdAsync(id);
        if (asset == null)
        {
            Console.WriteLine("\nAsset not found.");
            return;
        }

        Console.Write("\nEnter new model name (or press Enter to keep current): ");
        string modelName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(modelName))
            asset.ModelName = modelName;

        Console.Write("Enter new price in USD (or press Enter to keep current): ");
        string priceInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(priceInput) && decimal.TryParse(priceInput, out decimal price))
            asset.PriceInDollars = price;

        Console.WriteLine("\nSelect new office (or press Enter to keep current):");
        Console.WriteLine("1. New York (USD)");
        Console.WriteLine("2. Malmö (SEK)");
        Console.WriteLine("3. London (GBP)");
        Console.Write("\nSelect office: ");

        string officeInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(officeInput) && int.TryParse(officeInput, out int officeId) && officeId >= 1 && officeId <= 3)
            asset.OfficeId = officeId;

        await assetManager.UpdateAssetAsync(asset);
        Console.WriteLine("\n✓ Asset updated successfully!");
    }

    private static async Task DeleteExistingAsset(AssetManager assetManager)
    {
        await assetManager.PrintSortedAssetsAsync();

        Console.Write("\nEnter the ID of the asset to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nInvalid ID format.");
            return;
        }

        try
        {
            await assetManager.DeleteAssetAsync(id);
            Console.WriteLine("\n✓ Asset deleted successfully!");
        }
        catch (KeyNotFoundException)
        {
            Console.WriteLine("\nAsset not found.");
        }
    }

    private static string GetOfficeLocation(int officeId)
    {
        return officeId switch
        {
            1 => "New York",
            2 => "Malmö",
            3 => "London",
            _ => "New York"
        };
    }
}