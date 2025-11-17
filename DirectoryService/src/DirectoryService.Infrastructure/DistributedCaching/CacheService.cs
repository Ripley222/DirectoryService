using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Core.Abstractions.Caching;

namespace DirectoryService.Infrastructure.DistributedCaching;

public class CacheService(IDistributedCache cache) : ICacheService
{
    private readonly ConcurrentDictionary<string, bool> _cacheKeys = new();
    
    public async Task<T?> GetOrSetAsync<T>(
        string key,
        DistributedCacheEntryOptions options,
        Func<Task<T>> factory,
        CancellationToken cancellationToken = default) where T : class
    {
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue is null)
        {
            var factoryResult = await factory();
            await SetAsync<T>(key, factoryResult, options, cancellationToken);

            return factoryResult;
        }
        
        return cachedValue;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var cachedValue = await cache.GetStringAsync(key, cancellationToken);
        
        return cachedValue is null 
            ? null 
            : JsonSerializer.Deserialize<T>(cachedValue);
    }
    
    public async Task SetAsync<T>(
        string key, 
        T value,
        DistributedCacheEntryOptions options, 
        CancellationToken cancellationToken = default) where T : class
    {
        var serializedValue = JsonSerializer.Serialize(value);
        
        await cache.SetStringAsync(key, serializedValue, options, cancellationToken);
        
        _cacheKeys.TryAdd(key, true);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await cache.RemoveAsync(key, cancellationToken);
        
        _cacheKeys.TryRemove(key, out _);
    }

    public Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        var tasks = _cacheKeys
            .Keys
            .Where(k => k.StartsWith(prefixKey))
            .Select(k => RemoveAsync(k, cancellationToken));
        
        return Task.WhenAll(tasks);
    }
}