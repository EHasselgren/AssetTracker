using Microsoft.EntityFrameworkCore;
using Spectre.Console;

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
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[blue]Press any key to continue...[/]");
            Console.ReadKey(true);

            while (running)
            {
                var choice = await DisplayMainMenu();

                switch (choice)
                {
                    case "View All Assets":
                        await DisplayHeader("All Assets");
                        await assetManager.PrintSortedAssetsAsync();
                        await PauseAndContinue();
                        break;

                    case "Add New Asset":
                        await DisplayHeader("Add New Asset");
                        await AddNewAsset(assetManager);
                        await PauseAndContinue();
                        break;

                    case "Update Asset":
                        await DisplayHeader("Update Asset");
                        await UpdateExistingAsset(assetManager);
                        await PauseAndContinue();
                        break;

                    case "Delete Asset":
                        await DisplayHeader("Delete Asset");
                        await DeleteExistingAsset(assetManager);
                        await PauseAndContinue();
                        break;

                    case "Generate Report":
                        await DisplayHeader("Reports");
                        var reportType = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[blue]Select Report Type[/]")
                                .PageSize(3)
                                .AddChoices(new[] {
                "Asset Report by Office",
                "Asset Report by Device",
                "Back to Main Menu"
                                }));

                        switch (reportType)
                        {
                            case "Asset Report by Office":
                                await DisplayHeader("Asset Report by Office");
                                await assetManager.GenerateOfficeReportAsync();
                                break;

                            case "Asset Report by Device":
                                await DisplayHeader("Asset Report by Device");
                                await assetManager.GenerateDeviceReportAsync();
                                break;

                            case "Back to Main Menu":
                                continue;
                        }

                        await PauseAndContinue();
                        break;

                    case "Exit":
                        await DisplayHeader("Exiting Application");
                        AnsiConsole.MarkupLine("[green]Thank you for using Asset Management System![/]");
                        running = false;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            if (ex.InnerException != null)
            {
                AnsiConsole.MarkupLine($"[red]Inner Error: {ex.InnerException.Message}[/]");
            }
            await PauseAndContinue();
        }
    }

    private static async Task<string> DisplayMainMenu()
    {
        Console.Clear();
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Asset Management System[/]")
                .PageSize(7)
                .AddChoices(new[] {
                    "View All Assets",
                    "Add New Asset",
                    "Update Asset",
                    "Delete Asset",
                    "Generate Report",
                    "Exit"
                }));

        return selection;
    }

    private static async Task DisplayHeader(string title)
    {
        Console.Clear();
        var rule = new Rule($"[yellow]{title}[/]");
        rule.Style = Style.Parse("blue");
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
    }

    private static async Task PauseAndContinue()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[blue]Press any key to continue...[/]");
        Console.ReadKey(true);
    }

    private static async Task AddNewAsset(AssetManager assetManager)
    {
        var assetType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select Asset Type:")
                .AddChoices("Laptop", "Mobile Phone"));

        var modelName = AnsiConsole.Ask<string>("[green]Enter model name:[/]");

        var price = AnsiConsole.Prompt(
            new TextPrompt<decimal>("[green]Enter price in USD:[/]")
                .ValidationErrorMessage("[red]Please enter a valid price[/]")
                .Validate(price => price >= 0 ? ValidationResult.Success() : ValidationResult.Error("Price must be positive")));

        var office = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select Office Location:")
                .AddChoices(new[] {
                    "New York (USD)",
                    "Malmö (SEK)",
                    "London (GBP)"
                }));

        var officeId = office.StartsWith("New York") ? 1 : office.StartsWith("Malmö") ? 2 : 3;

        Asset newAsset = assetType switch
        {
            "Laptop" => new Laptop(modelName, DateTime.Now, price, GetOfficeLocation(officeId)),
            "Mobile Phone" => new MobilePhone(modelName, DateTime.Now, price, GetOfficeLocation(officeId)),
            _ => throw new ArgumentException("Invalid asset type")
        };
        newAsset.OfficeId = officeId;

        await assetManager.AddAssetAsync(newAsset);
        AnsiConsole.MarkupLine("\n[green] Asset added successfully![/]");
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
        Console.WriteLine("\n Asset updated successfully!");
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
            Console.WriteLine("\n Asset deleted successfully!");
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