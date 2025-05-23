using CurrencyMockService.Models;
using CurrencyMockService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyMockService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _currencyService;

    public CurrencyController(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    [HttpGet("rate")]
    public async Task<ActionResult<CurrencyRate>> GetRate(
        [FromQuery] string baseCurrency = "USD", 
        [FromQuery] string targetCurrency = "EUR")
    {
        try
        {
            var rate = await _currencyService.GetRateAsync(baseCurrency, targetCurrency);
            return Ok(rate);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}