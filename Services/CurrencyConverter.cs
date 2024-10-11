using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class CurrencyConverter
{
    private const string ApiKey = "bd6de256f29ffb3f7d2969f9";
    private static readonly HttpClient _client = new HttpClient();

    public async Task<decimal> ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
    {
        string apiUrl = $"https://v6.exchangerate-api.com/v6/{ApiKey}/latest/{fromCurrency}";
        HttpResponseMessage response = await _client.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var exchangeRates = JObject.Parse(jsonResponse)["conversion_rates"];

            decimal conversionRate = exchangeRates.Value<decimal>(toCurrency);
            return amount * conversionRate;
        }
        else
        {
            throw new Exception("Unable to get currency conversion data.");
        }
    }
}
