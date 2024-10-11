using System;

public class Asset
{
    public string ModelName { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal PriceInDollars { get; set; }
    public string OfficeLocation { get; set; }

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