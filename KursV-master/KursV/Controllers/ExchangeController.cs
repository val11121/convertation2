using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KursV.Models;
using KursV.Services;

namespace KursV.Controllers
{
    /// <summary>
    /// Контроллер для работы с курсами валют
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;

        public ExchangeController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        /// <summary>
        /// GET: api/exchange
        /// Получить все курсы валют
        /// </summary>
        [HttpGet]
        public IActionResult GetAllRates()
        {
            try
            {
                var rates = _exchangeRateService.GetAllRates();
                return Ok(rates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении курсов: {ex.Message}");
            }
        }

        /// <summary>
        /// GET: api/exchange/{currencyCode}
        /// Получить курс конкретной валюты
        /// </summary>
        [HttpGet("{currencyCode}")]
        public IActionResult GetRate(string currencyCode)
        {
            try
            {
                var rate = _exchangeRateService.GetRate(currencyCode);

                if (rate == null)
                    return NotFound($"Курс для валюты {currencyCode} не найден");

                return Ok(rate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении курса: {ex.Message}");
            }
        }

        /// <summary>
        /// POST: api/exchange
        /// Добавить новый курс валюты
        /// </summary>
        [HttpPost]
        public IActionResult AddRate([FromBody] AddRateRequest request)
        {
            try
            {
                // Проверяем валидность запроса
                if (request == null)
                    return BadRequest("Запрос не может быть пустым");

                if (string.IsNullOrEmpty(request.CurrencyCode))
                    return BadRequest("Код валюты обязателен для заполнения");

                if (request.Rate <= 0)
                    return BadRequest("Курс должен быть больше 0");

                _exchangeRateService.AddRate(request);
                return Ok("Курс валюты успешно добавлен");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при добавлении курса: {ex.Message}");
            }
        }

        /// <summary>
        /// PUT: api/exchange/{currencyCode}
        /// Обновить курс валюты
        /// </summary>
        [HttpPut("{currencyCode}")]
        public IActionResult UpdateRate(string currencyCode, [FromBody] decimal newRate)
        {
            try
            {
                if (newRate <= 0)
                    return BadRequest("Курс должен быть больше 0");

                _exchangeRateService.UpdateRate(currencyCode, newRate);
                return Ok("Курс валюты успешно обновлен");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при обновлении курса: {ex.Message}");
            }
        }
    }
}
