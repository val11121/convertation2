using KursV.Models;

namespace KursV.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        // СТАТИЧЕСКИЕ поля - сохраняются между запросами
        private static readonly List<ExchangeRate> _exchangeRates;
        private static int _nextId = 1;

        // Статический конструктор - выполняется один раз
        static ExchangeRateService()
        {
            _exchangeRates = new List<ExchangeRate>
            {
                new ExchangeRate {
                    Id = _nextId++,
                    CurrencyCode = "RUB",
                    CurrencyName = "Российский рубль",
                    Rate = 1.0m,
                    LastUpdated = DateTime.Now
                },
                new ExchangeRate {
                    Id = _nextId++,
                    CurrencyCode = "USD",
                    CurrencyName = "Доллар США",
                    Rate = 90.50m,
                    LastUpdated = DateTime.Now
                }
            };
        }

        public List<ExchangeRate> GetAllRates()
        {
            return _exchangeRates;
        }

        public ExchangeRate? GetRate(string currencyCode)
        {
            return _exchangeRates.FirstOrDefault(rate =>
                rate.CurrencyCode.Equals(currencyCode, StringComparison.OrdinalIgnoreCase));
        }

        public void AddRate(AddRateRequest request)
        {
            var existingRate = GetRate(request.CurrencyCode);
            if (existingRate != null)
            {
                throw new ArgumentException($"Валюта с кодом {request.CurrencyCode} уже существует");
            }

            var newRate = new ExchangeRate
            {
                Id = _nextId++,
                CurrencyCode = request.CurrencyCode.ToUpper(),
                CurrencyName = request.CurrencyName,
                Rate = request.Rate,
                LastUpdated = DateTime.Now
            };

            _exchangeRates.Add(newRate);
        }

        public void UpdateRate(string currencyCode, decimal newRate)
        {
            var existingRate = GetRate(currencyCode);
            if (existingRate == null)
            {
                throw new ArgumentException($"Валюта с кодом {currencyCode} не найдена");
            }

            existingRate.Rate = newRate;
            existingRate.LastUpdated = DateTime.Now;
        }

        public void DeleteRate(string currencyCode)
        {
            var existingRate = GetRate(currencyCode);
            if (existingRate == null)
            {
                throw new ArgumentException($"Валюта с кодом {currencyCode} не найдена");
            }

            _exchangeRates.Remove(existingRate);
        }
    }
}