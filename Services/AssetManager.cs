using AssetTracking.Helpers;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

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

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn("ID").Centered())
            .AddColumn(new TableColumn("Office").LeftAligned())
            .AddColumn(new TableColumn("Asset Type").LeftAligned())
            .AddColumn(new TableColumn("Model").LeftAligned())
            .AddColumn(new TableColumn("Purchase Date").LeftAligned())
            .AddColumn(new TableColumn("USD Price").LeftAligned())
            .AddColumn(new TableColumn("Local Price").LeftAligned())
            .AddColumn(new TableColumn("Time Left").LeftAligned());

        foreach (var asset in assets)
        {
            var timeLeft = asset.TimeUntilEndOfLife();
            var status = AssetStatusHelper.GetStatusBasedOnDate(timeLeft);
            var localPrice = await asset.Office.ConvertToLocalCurrency(asset.PriceInDollars);
            string currencySymbol = CurrencyHelper.GetCurrencySymbol(asset.Office);

            string timeLeftColor = status switch
            {
                AssetStatus.Red => "red",
                AssetStatus.Yellow => "yellow",
                _ => "green"
            };

            table.AddRow(
                asset.Id.ToString(),
                asset.Office.Location,
                asset.GetType().Name,
                asset.ModelName,
                asset.PurchaseDate.ToShortDateString(),
                $"${asset.PriceInDollars:N2}",
                $"{currencySymbol}{localPrice:N2}",
                $"[{timeLeftColor}]{timeLeft.Days} days[/]"
            );
        }

        AnsiConsole.Write(table);
    }
    public async Task GenerateOfficeReportAsync()
    {
        var assets = await _context.Assets.Include(a => a.Office).ToListAsync();
        var officeGroups = assets.GroupBy(a => a.Office);
        var officeReport = new List<OfficeReportData>();

        foreach (var group in officeGroups)
        {
            var office = group.Key;
            var totalUSD = group.Sum(a => a.PriceInDollars);
            var localTotal = await office.ConvertToLocalCurrency(totalUSD);
            var currencySymbol = CurrencyHelper.GetCurrencySymbol(office);

            officeReport.Add(new OfficeReportData
            {
                Office = office.Location,
                TotalAssets = group.Count(),
                TotalValueUSD = totalUSD,
                TotalValueLocal = localTotal,
                LocalCurrencySymbol = currencySymbol,
                AssetsNearingEndOfLife = group.Count(a => a.TimeUntilEndOfLife().TotalDays <= 180)
            });
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[blue]Asset Report by Office[/]")
            .AddColumn(new TableColumn("Office").LeftAligned())
            .AddColumn(new TableColumn("Total Assets").RightAligned())
            .AddColumn(new TableColumn("Total Value (USD)").RightAligned())
            .AddColumn(new TableColumn("Total Value (Local)").RightAligned())
            .AddColumn(new TableColumn("Assets Near EOL").RightAligned());

        foreach (var office in officeReport)
        {
            table.AddRow(
                office.Office,
                office.TotalAssets.ToString(),
                $"${office.TotalValueUSD:N2}",
                $"{office.LocalCurrencySymbol}{office.TotalValueLocal:N2}",
                office.AssetsNearingEndOfLife.ToString()
            );
        }

        AnsiConsole.Write(table);
    }

    public async Task GenerateDeviceReportAsync()
    {
        var assets = await _context.Assets.Include(a => a.Office).ToListAsync();

        var deviceReport = assets
            .GroupBy(a => a.GetType().Name)
            .Select(g => new DeviceReportData
            {
                DeviceType = g.Key,
                Count = g.Count(),
                AverageAge = g.Average(a => (DateTime.Now - a.PurchaseDate).TotalDays),
                TotalValue = g.Sum(a => a.PriceInDollars)
            })
            .ToList();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[blue]Asset Report by Device Type[/]")
            .AddColumn("Device Type")
            .AddColumn(new TableColumn("Count").RightAligned())
            .AddColumn(new TableColumn("Average Age (days)").RightAligned())
            .AddColumn(new TableColumn("Total Value (USD)").RightAligned());

        foreach (var device in deviceReport)
        {
            table.AddRow(
                device.DeviceType,
                device.Count.ToString(),
                $"{device.AverageAge:N0}",
                $"${device.TotalValue:N2}"
            );
        }

        AnsiConsole.Write(table);
    }
}