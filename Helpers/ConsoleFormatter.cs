namespace AssetTracking.Helpers
{
    public static class ConsoleFormatter
    {
        public static string GetAnsiColorCodeForStatus(AssetStatus status)
        {
            return status switch
            {
                AssetStatus.Red => "\u001b[31m",
                AssetStatus.Yellow => "\u001b[33m",
                AssetStatus.Green => "\u001b[32m",
                _ => "\u001b[32m"
            };
        }


    }
}
