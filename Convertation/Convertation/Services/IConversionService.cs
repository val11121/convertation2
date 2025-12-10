using Convertation.Models;

namespace Convertation.Services
{
    public interface IConversionService
    {       
        Task<Conversion> ConvertAsync(ConversionRequest request);
        Task<IEnumerable<Conversion>> GetAllAsync();
        Task<Conversion?> GetByIdAsync(int id);        
    }
}
