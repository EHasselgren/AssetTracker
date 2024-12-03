using AssetTracking.Helpers;
using Microsoft.EntityFrameworkCore;

public class AssetManager
{
    private readonly AssetDbContext _context;

    public AssetManager()
    {
        _context = new AssetDbContext();
    }

    public async Task AddAssetAsync(Asset asset)
    {
        await _context.Assets.AddAsync(asset);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Asset>> GetAllAssetsAsync()
    {
        return await _context.Assets.ToListAsync();
    }

    public async Task<Asset> GetAssetByIdAsync(int id)
    {
        return await _context.Assets.FindAsync(id);
    }

    public async Task UpdateAssetAsync(Asset asset)
    {
        var existingAsset = await _context.Assets.FindAsync(asset.Id);
        if (existingAsset == null)
            throw new KeyNotFoundException($"Asset with ID {asset.Id} not found.");

        _context.Entry(existingAsset).CurrentValues.SetValues(asset);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAssetAsync(int id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null)
            throw new KeyNotFoundException($"Asset with ID {id} not found.");

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();
    }

    public async Task PrintSortedAssetsAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Office)
            .OrderBy(a => a.Office.Location)
            .ThenBy(a => a.PurchaseDate)
            .ToListAsync();

        Console.WriteLine("\n| Office | Asset Type | Model | Purchase Date | USD Price | Local Price | Status |");
        Console.WriteLine("|---------|------------|-------|---------------|-----------|-------------|---------|");

        foreach (var asset in assets)
        {
            var timeLeft = asset.TimeUntilEndOfLife();
            var status = AssetStatusHelper.GetStatusBasedOnDate(timeLeft);
            string colorCode = ConsoleFormatter.GetAnsiColorCodeForStatus(status);
            var localPrice = await asset.Office.ConvertToLocalCurrency(asset.PriceInDollars);
            string currencySymbol = CurrencyHelper.GetCurrencySymbol(asset.Office);

            var line =
                $"| {asset.Office.Location.PadRight(7)} " +
                $"| {asset.GetType().Name.PadRight(10)} " +
                $"| {asset.ModelName.PadRight(5)} " +
                $"| {asset.PurchaseDate.ToShortDateString().PadRight(13)} " +
                $"| ${asset.PriceInDollars.ToString("N2").PadRight(9)} " +
                $"| {currencySymbol}{localPrice.ToString("N2").PadRight(11)} " +
                $"| {colorCode}{timeLeft.Days} days\u001b[0m |";

            Console.WriteLine(line);
        }
        Console.WriteLine();
    }

    public async Task GenerateReportAsync()
    {
        var assets = await _context.Assets.Include(a => a.Office).ToListAsync();

        var officeReport = assets
            .GroupBy(a => a.Office.Location)
            .Select(g => new
            {
                Office = g.Key,
                TotalAssets = g.Count(),
                TotalValueUSD = g.Sum(a => a.PriceInDollars),
                AssetsNearingEndOfLife = g.Count(a => a.TimeUntilEndOfLife().TotalDays <= 180)
            });

        Console.WriteLine("\nASSET REPORT BY OFFICE");
        Console.WriteLine("======================");

        foreach (var office in officeReport)
        {
            Console.WriteLine($"\nOffice: {office.Office}");
            Console.WriteLine($"Total Assets: {office.TotalAssets}");
            Console.WriteLine($"Total Value (USD): ${office.TotalValueUSD:N2}");
            Console.WriteLine($"Assets Nearing End of Life: {office.AssetsNearingEndOfLife}");
        }

        var typeReport = assets
            .GroupBy(a => a.GetType().Name)
            .Select(g => new
            {
                Type = g.Key,
                Count = g.Count(),
                AverageAge = g.Average(a => (DateTime.Now - a.PurchaseDate).TotalDays)
            });

        Console.WriteLine("\nASSET REPORT BY TYPE");
        Console.WriteLine("===================");

        foreach (var type in typeReport)
        {
            Console.WriteLine($"\nType: {type.Type}");
            Console.WriteLine($"Total Count: {type.Count}");
            Console.WriteLine($"Average Age: {type.AverageAge:N0} days");
        }
    }
}