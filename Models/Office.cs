using System;
using System.Threading.Tasks;

public class Office
{
    public string Location { get; }
    public string CurrencyCode { get; }

    public Office(string location, string currencyCode)
    {
        Location = location;
        CurrencyCode = currencyCode;
    }

    public static readonly Office NewYork = new Office("New York", "USD");
    public static readonly Office Malmö = new Office("Malmö", "SEK");
    public static readonly Office London = new Office("London", "GBP");

    public async Task<decimal> ConvertToLocalCurrency(decimal priceInUsd)
    {
        var currencyConverter = new CurrencyConverter();
        return await currencyConverter.ConvertCurrency(priceInUsd, "USD", CurrencyCode);
    }
}

