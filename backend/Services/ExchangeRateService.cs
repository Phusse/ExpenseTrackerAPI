using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace ExpenseTracker.Services;

/// <summary>
/// Service to fetch and cache exchange rates from Frankfurter API
/// </summary>
public interface IExchangeRateService
{
    Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency = "NGN");
    Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency);
}

public class ExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ExchangeRateService> _logger;
    private const string CacheKey = "exchange_rates";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public ExchangeRateService(HttpClient httpClient, IMemoryCache cache, ILogger<ExchangeRateService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency = "NGN")
    {
        var cacheKey = $"{CacheKey}_{baseCurrency}";

        if (_cache.TryGetValue(cacheKey, out Dictionary<string, decimal>? cachedRates) && cachedRates != null)
        {
            return cachedRates;
        }

        try
        {
            // Frankfurter API - free, no API key required
            var response = await _httpClient.GetAsync($"https://api.frankfurter.app/latest?from={baseCurrency}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch exchange rates: {status}", response.StatusCode);
                return GetFallbackRates(baseCurrency);
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<FrankfurterResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data?.Rates == null)
            {
                return GetFallbackRates(baseCurrency);
            }

            // Add base currency with rate 1
            var rates = new Dictionary<string, decimal>(data.Rates)
            {
                [baseCurrency] = 1m
            };

            _cache.Set(cacheKey, rates, CacheDuration);
            _logger.LogInformation("Exchange rates cached for {currency}", baseCurrency);

            return rates;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error fetching exchange rates: {message}", ex.Message);
            return GetFallbackRates(baseCurrency);
        }
    }

    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency) return amount;

        var rates = await GetExchangeRatesAsync(fromCurrency);
        
        if (rates.TryGetValue(toCurrency, out var rate))
        {
            return Math.Round(amount * rate, 2);
        }

        return amount;
    }

    /// <summary>
    /// Fallback rates in case API is unavailable (approximate rates as of Jan 2024)
    /// </summary>
    private static Dictionary<string, decimal> GetFallbackRates(string baseCurrency)
    {
        if (baseCurrency == "NGN")
        {
            return new Dictionary<string, decimal>
            {
                ["NGN"] = 1m,
                ["USD"] = 0.00063m,      // ~1590 NGN = 1 USD
                ["EUR"] = 0.00058m,      // ~1720 NGN = 1 EUR
                ["GBP"] = 0.00050m,      // ~2000 NGN = 1 GBP
                ["CAD"] = 0.00084m,
                ["AUD"] = 0.00096m,
                ["JPY"] = 0.092m,
                ["CNY"] = 0.0046m,
                ["INR"] = 0.053m,
                ["ZAR"] = 0.012m
            };
        }

        // Default: return 1:1 for unknown base
        return new Dictionary<string, decimal> { [baseCurrency] = 1m };
    }
}

public class FrankfurterResponse
{
    public string? Base { get; set; }
    public string? Date { get; set; }
    public Dictionary<string, decimal>? Rates { get; set; }
}
