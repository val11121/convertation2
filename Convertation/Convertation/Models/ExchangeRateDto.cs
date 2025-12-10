namespace Convertation.Models
{
    public class ExchangeRateDto
    {
        public int Id { get; set; }
        public string? CurrencyCode { get; set; }
        public decimal Rate { get; set; }
        public string? CurrencyName { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
