public class Laptop : Asset
{
    public Laptop() : base() { }

    public Laptop(string modelName, DateTime purchaseDate, decimal priceInDollars, string officeLocation)
        : base(modelName, purchaseDate, priceInDollars, officeLocation)
    {
    }
}