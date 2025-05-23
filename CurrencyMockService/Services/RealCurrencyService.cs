using CurrencyMockService.Models;
using CurrencyMockService.Services;

namespace CurrencyMockService.Services;

public class RealCurrencyService : ICurrencyService
{
    public async Task<CurrencyRate> GetRateAsync(string baseCurrency, string targetCurrency)
    {
        // В реальной реализации здесь был бы вызов внешнего API
        throw new NotImplementedException("This would call real API in production");
    }
}