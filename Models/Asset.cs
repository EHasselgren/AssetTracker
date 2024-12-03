using System;
using System.ComponentModel.DataAnnotations;

public class Asset
{
    [Key]
    public int Id { get; set; }
    public string ModelName { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal PriceInDollars { get; set; }
    public string OfficeLocation { get; set; }
    public string AssetType { get; set; }

    protected Asset() { }

    protected Asset(string modelName, DateTime purchaseDate, decimal priceInDollars, string officeLocation)
    {
        ModelName = modelName;
        PurchaseDate = purchaseDate;
        PriceInDollars = priceInDollars;
        OfficeLocation = officeLocation;
    }

    public TimeSpan TimeUntilEndOfLife()
    {
        return (PurchaseDate.AddYears(3) - DateTime.Now);
    }
}