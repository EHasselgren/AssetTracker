using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracking.Helpers;

public class AssetManager
{
    private List<Asset> _assets;

    public AssetManager()
    {
        _assets = new List<Asset>();
    }

    public void AddAsset(Asset asset)
    {
        _assets.Add(asset);
    }

    public async Task PrintSortedAssetsAsync()
    {
        var sortedAssets = _assets
            .OrderBy(a => a.OfficeLocation)
            .ThenBy(a => a.PurchaseDate)
            .ToList();

        Console.WriteLine("\n| Office       | Asset Type   | Model        | Purchase Date | Price (Dollars) | Price (Local Currency) | Status  |");
        Console.WriteLine("|--------------|--------------|--------------|---------------|-----------------|------------------------|---------|");

        foreach (var asset in sortedAssets)
        {
            var timeLeft = asset.TimeUntilEndOfLife();
            AssetStatus status = AssetStatusHelper.GetStatusBasedOnDate(timeLeft);
            Office office = OfficeHelper.GetOfficeByLocation(asset.OfficeLocation);
            var localPrice = await office.ConvertToLocalCurrency(asset.PriceInDollars);
            string currencySymbol = CurrencyHelper.GetCurrencySymbol(office);
            string colorCode = ConsoleFormatter.GetAnsiColorCodeForStatus(status);

            Console.WriteLine(
                $"| {asset.OfficeLocation.PadRight(12)} " +
                $"| {asset.GetType().Name.PadRight(12)} " +
                $"| {asset.ModelName.PadRight(12)} " +
                $"| {asset.PurchaseDate.ToShortDateString().PadRight(13)} " +
                $"| {asset.PriceInDollars.ToString("N2").PadRight(15)} " +
                $"| {currencySymbol}{localPrice.ToString("N2").PadRight(23)} " +
                $"| {colorCode}{status.ToString().PadRight(7)}\u001b[0m |"
            );
        }

        Console.WriteLine();
    }

}
