using Microsoft.AspNetCore.Mvc;
using Convertation.Models;
using Convertation.Services;
using System.Net.Http.Json;

namespace CurrencyConverterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversionsController : ControllerBase
    {
        private readonly IConversionService _conversionService;
        private readonly HttpClient _httpClient;

        public ConversionsController(IConversionService conversionService, IHttpClientFactory httpClientFactory)
        {
            _conversionService = conversionService;
            _httpClient = httpClientFactory.CreateClient("ExchangeApi");
        }

        // GET: api/conversions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Conversion>>> GetAll()
        {
            var conversions = await _conversionService.GetAllAsync();
            return Ok(conversions);
        }

        // GET: api/conversions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Conversion>> GetById(int id)
        {
            var conversion = await _conversionService.GetByIdAsync(id);
            return conversion == null ? NotFound() : Ok(conversion);
        }

        // POST: api/conversions
        [HttpPost]
        public async Task<ActionResult<Conversion>> Convert([FromBody] ConversionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _conversionService.ConvertAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, "Внешний сервис курсов валют недоступен. Попробуйте позже.");
            }
            catch (TaskCanceledException) when (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                return StatusCode(504, "Превышено время ожидания ответа от сервиса курсов валют.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка при выполнении конвертации.");
            }
        }

        // GET: api/conversions/rates
        [HttpGet("rates")]
        public async Task<ActionResult<object>> GetRates()
        {
            try
            {
                var dtos = await _httpClient.GetFromJsonAsync<ExchangeRateDto[]>("api/Exchange");

                if (dtos == null || dtos.Length == 0)
                    return StatusCode(502, "Не удалось получить курсы валют: пустой ответ от внешнего сервиса.");

                var rates = dtos
                    .Where(x => !string.IsNullOrWhiteSpace(x.CurrencyCode))
                    .ToDictionary(
                        x => x.CurrencyCode!,
                        x => x.Rate,
                        StringComparer.OrdinalIgnoreCase);

                var latestUpdate = dtos.Max(x => x.LastUpdated);

                return Ok(new
                {
                    BaseCurrency = "RUB",
                    Note = "Курсы показывают, сколько RUB стоит 1 единица указанной валюты",
                    UpdatedAt = latestUpdate,
                    Rates = rates
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, "Внешний сервис курсов валют сейчас недоступен.");
            }
            catch (TaskCanceledException)
            {
                return StatusCode(504, "Таймаут при обращении к сервису курсов валют.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Ошибка при получении курсов валют.");
            }
        }
    }
}