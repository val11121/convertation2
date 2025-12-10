using Convertation.Models;
using System.Text.Json;

namespace Convertation.Data
{
    public class FileDbContext
    {
        private readonly string _filePath;
        private List<Conversion> _conversions = new();
        private int _nextId = 1;
        private readonly SemaphoreSlim _lock = new(1, 1);

        public FileDbContext(IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "wwwroot", "data");
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "conversions.json");
            LoadAsync().GetAwaiter().GetResult();
        }

        private async Task LoadAsync()
        {
            await _lock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                {
                    _conversions = new List<Conversion>();
                    _nextId = 1;
                    return;
                }

                var json = await File.ReadAllTextAsync(_filePath);
                var loaded = JsonSerializer.Deserialize<List<Conversion>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _conversions = loaded ?? new List<Conversion>();
                _nextId = _conversions.Any() ? _conversions.Max(c => c.Id) + 1 : 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading DB: {ex.Message}");
                _conversions = new List<Conversion>();
                _nextId = 1;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task SaveAsync()
        {
            await _lock.WaitAsync();
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_conversions, options);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving DB: {ex.Message}");
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<int> GetNextIdAsync()
        {
            var id = _nextId++;
            await Task.CompletedTask;
            return id;
        }

        public IReadOnlyList<Conversion> Conversions => _conversions.AsReadOnly();

        public async Task AddAsync(Conversion conversion)
        {
            _conversions.Add(conversion);
            await SaveAsync();
        }

        public async Task UpdateAsync(Conversion conversion)
        {
            var existing = _conversions.FirstOrDefault(c => c.Id == conversion.Id);
            if (existing != null)
            {
                var index = _conversions.IndexOf(existing);
                _conversions[index] = conversion;
                await SaveAsync();
            }
        }
    }
}
