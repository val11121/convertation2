using KursV.Models;

namespace KursV.Services
{
    /// <summary>
    /// Интерфейс для сервиса работы с курсами валют
    /// </summary>
    public interface IExchangeRateService
    {
        /// <summary>
        /// Получить все курсы валют
        /// </summary>
        List<ExchangeRate> GetAllRates();

        /// <summary>
        /// Получить курс конкретной валюты
        /// </summary>
        ExchangeRate? GetRate(string currencyCode);

        /// <summary>
        /// Добавить новый курс валюты
        /// </summary>
        void AddRate(AddRateRequest request);

        /// <summary>
        /// Обновить существующий курс
        /// </summary>
        void UpdateRate(string currencyCode, decimal newRate);

        /// <summary>
        /// Удалить валюту
        /// </summary>
        void DeleteRate(string currencyCode);
    }
}