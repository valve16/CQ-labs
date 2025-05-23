namespace CurrencyMockService.Models;

public class CurrencyRate
{
    public string BaseCurrency { get; set; } = "USD";
    public string TargetCurrency { get; set; } = "EUR";
    public decimal Rate { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
}