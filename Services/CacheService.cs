using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

public class CacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);

        if (data == null)
            return default;

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, int minutes = 5)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes)
        };

        var jsonData = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(key, jsonData, options);
    }
    public async Task RemoveAsync(string key)
    {
     await _cache.RemoveAsync(key);
    }
}