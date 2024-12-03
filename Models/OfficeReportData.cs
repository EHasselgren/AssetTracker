public class OfficeReportData
{
    public string Office { get; set; }
    public int TotalAssets { get; set; }
    public decimal TotalValueUSD { get; set; }
    public decimal TotalValueLocal { get; set; }
    public string LocalCurrencySymbol { get; set; }
    public int AssetsNearingEndOfLife { get; set; }
}