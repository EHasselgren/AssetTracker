public class MobilePhone : Asset
{
    public MobilePhone() : base() { }

    public MobilePhone(string modelName, DateTime purchaseDate, decimal priceInDollars, string officeLocation)
        : base(modelName, purchaseDate, priceInDollars, officeLocation)
    {
    }
}