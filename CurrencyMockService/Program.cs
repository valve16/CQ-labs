using CurrencyMockService.Services;
using CurrencyMockService.Models;
using Moq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var mockCurrencyService = new Mock<ICurrencyService>();

mockCurrencyService.Setup(x => x.GetRateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string baseCurrency, string targetCurrency) => 
        new CurrencyRate
        {
            BaseCurrency = baseCurrency,
            TargetCurrency = targetCurrency,
            Rate = baseCurrency switch
            {
                "USD" when targetCurrency == "EUR" => 0.92m,
                "USD" when targetCurrency == "RUB" => 0.79m,
                "EUR" when targetCurrency == "USD" => 1.09m,
                "RUB" when targetCurrency == "USD" => 1.27m,
                _ => 1.0m
            }
        });

builder.Services.AddSingleton(mockCurrencyService.Object);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();