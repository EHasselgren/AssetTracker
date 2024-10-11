namespace AssetTracking.Helpers
{
    public static class CurrencyHelper
    {
        public static string GetCurrencySymbol(Office office)
        {
            return office.Location switch
            {
                "New York" => "$",
                "Malmö" => "kr",
                "London" => "£",
                _ => "$"
            };
        }
    }
}
