using System;

namespace AssetTracking.Helpers
{
    public static class AssetStatusHelper
    {
        public static AssetStatus GetStatusBasedOnDate(TimeSpan timeLeft)
        {
            return timeLeft.TotalDays switch
            {
                <= 90 => AssetStatus.Red,
                <= 180 => AssetStatus.Yellow,
                _ => AssetStatus.Green
            };
        }
    }
}
