using Convertation.Data;
using Convertation.Models;
using System.Net.Http.Json;

namespace Convertation.Services
{
    public class ConversionService : IConversionService
    {
        private readonly FileDbContext _db;
        private readonly HttpClient _httpClient;

        public ConversionService(FileDbContext db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClient = httpClientFactory.CreateClient("ExchangeApi");
        }

        private async Task<Dictionary<string, decimal>> GetRatesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateDto[]>("api/Exchange");

            if (response == null)
                throw new InvalidOperationException("Не удалось загрузить курсы валют — пустой ответ от сервера.");

            return response
                .Where(r => !string.IsNullOrWhiteSpace(r.CurrencyCode))
                .ToDictionary(
                    r => r.CurrencyCode!,
                    r => r.Rate,
                    StringComparer.OrdinalIgnoreCase);
        }

        public async Task<Conversion> ConvertAsync(ConversionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FromCurrency) ||
                string.IsNullOrWhiteSpace(request.ToCurrency) ||
                request.Amount <= 0)
            {
                throw new ArgumentException("Invalid conversion request.");
            }

            var from = request.FromCurrency.ToUpperInvariant();
            var to = request.ToCurrency.ToUpperInvariant();

            // Каждый раз получаем свежие курсы
            var rates = await GetRatesAsync();

            if (!rates.TryGetValue(from, out decimal fromRate))
                throw new ArgumentException($"Валюта '{from}' не поддерживается или отсутствует в данных API.");

            if (!rates.TryGetValue(to, out decimal toRate))
                throw new ArgumentException($"Валюта '{to}' не поддерживается или отсутствует в данных API.");

            // Курсы в API — сколько RUB за 1 единицу валюты
            decimal exchangeRate = fromRate / toRate;
            decimal result = request.Amount * exchangeRate;

            var conversion = new Conversion
            {
                Id = await _db.GetNextIdAsync(),
                FromCurrency = from,
                ToCurrency = to,
                Amount = request.Amount,
                Result = Math.Round(result, 6),
                ExchangeRate = Math.Round(exchangeRate, 6),
                Timestamp = DateTime.UtcNow
            };

            await _db.AddAsync(conversion);
            return conversion;
        }

        public Task<IEnumerable<Conversion>> GetAllAsync() =>
            Task.FromResult(_db.Conversions.AsEnumerable());

        public Task<Conversion?> GetByIdAsync(int id) =>
            Task.FromResult(_db.Conversions.FirstOrDefault(c => c.Id == id));
    }
}