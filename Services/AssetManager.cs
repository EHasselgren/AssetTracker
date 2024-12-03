using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    public async Task PrintSortedAssetsAsync()
    {
        var assets = await GetAllAssetsAsync();
        var sortedAssets = assets
            .OrderBy(a => a.GetType().Name)
            .ThenBy(a => a.PurchaseDate)
            .ToList();

        Console.WriteLine("\n| Asset Type   | Model        | Purchase Date | Price (Dollars) |");
        Console.WriteLine("|--------------|--------------|---------------|-----------------|");

        foreach (var asset in sortedAssets)
        {
            Console.WriteLine(
                $"| {asset.GetType().Name.PadRight(12)} " +
                $"| {asset.ModelName.PadRight(12)} " +
                $"| {asset.PurchaseDate.ToShortDateString().PadRight(13)} " +
                $"| ${asset.PriceInDollars.ToString("N2").PadRight(14)} |"
            );
        }
        Console.WriteLine();
    }
}