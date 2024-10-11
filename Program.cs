class Program
{
    static async Task Main(string[] args)
    {
        var assetManager = new AssetManager();

        assetManager.AddAsset(new Laptop("MacBook", new DateTime(2021, 10, 8), 1100m, "New York"));
        assetManager.AddAsset(new MobilePhone("iPhone", new DateTime(2020, 5, 12), 600m, "London"));
        assetManager.AddAsset(new Laptop("HP", new DateTime(2024, 1, 15), 320m, "Malmö"));
        assetManager.AddAsset(new Laptop("MacBook", new DateTime(2021, 10, 8), 2200m, "New York"));
        assetManager.AddAsset(new MobilePhone("iPhone", new DateTime(2020, 5, 12), 100m, "London"));
        assetManager.AddAsset(new Laptop("HP", new DateTime(2024, 1, 15), 220m, "Malmö"));
        assetManager.AddAsset(new Laptop("MacBook", new DateTime(2021, 10, 8), 3200m, "New York"));
        assetManager.AddAsset(new MobilePhone("iPhone", new DateTime(2020, 5, 12), 600m, "London"));
        assetManager.AddAsset(new Laptop("HP", new DateTime(2024, 1, 15), 120m, "Malmö"));
        assetManager.AddAsset(new Laptop("MacBook", new DateTime(2021, 10, 8), 200m, "New York"));
        assetManager.AddAsset(new MobilePhone("iPhone", new DateTime(2020, 5, 12), 90m, "London"));
        assetManager.AddAsset(new Laptop("HP", new DateTime(2024, 1, 15), 100m, "Malmö"));
        assetManager.AddAsset(new Laptop("MacBook", new DateTime(2021, 10, 8), 120m, "New York"));
        assetManager.AddAsset(new MobilePhone("iPhone", new DateTime(2022, 04, 01), 90m, "London"));
        assetManager.AddAsset(new Laptop("HP", new DateTime(2024, 1, 15), 1000m, "Malmö"));

        await assetManager.PrintSortedAssetsAsync();
    }
}
