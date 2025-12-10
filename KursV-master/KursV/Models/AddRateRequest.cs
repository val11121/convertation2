namespace KursV.Models
{
    /// <summary>
    /// Модель для добавления нового курса валюты
    /// </summary>
    public class AddRateRequest
    {
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public string CurrencyName { get; set; } = string.Empty;
    }
}
