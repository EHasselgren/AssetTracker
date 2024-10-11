namespace AssetTracking.Helpers
{
    public static class OfficeHelper
    {
        public static Office GetOfficeByLocation(string officeLocation)
        {
            return officeLocation switch
            {
                "Malmö" => Office.Malmö,
                "London" => Office.London,
                _ => Office.NewYork
            };
        }
    }
}
