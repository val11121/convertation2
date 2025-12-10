namespace KursV.Models
{
    /// <summary>
    /// модель для хранения курса валюты 
    /// </summary>
    public class ExchangeRate
    {
        public int Id { get; set; }

        /// <summary>
        /// Код валюты (например: USD, EUR, RUB)
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Курс к базовой валюте
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Название валюты
        /// </summary>
        public string CurrencyName { get; set; } = string.Empty;

        /// <summary>
        /// Дата обновления курса
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
