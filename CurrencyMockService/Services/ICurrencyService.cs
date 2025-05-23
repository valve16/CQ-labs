using CurrencyMockService.Models;
namespace CurrencyMockService.Services;

public interface ICurrencyService
{
    Task<CurrencyRate> GetRateAsync(string baseCurrency, string targetCurrency);
}