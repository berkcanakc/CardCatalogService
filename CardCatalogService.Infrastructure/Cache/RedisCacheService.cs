using CardCatalogService.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace CardCatalogService.Infrastructure.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IDatabase _redisDb;
        private readonly IServer _redisServer;

        public RedisCacheService(
            IDistributedCache distributedCache,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _redisDb = connectionMultiplexer.GetDatabase();

            // Bu host ve port config'den alınmalı (şimdilik hardcoded örnek)
            _redisServer = connectionMultiplexer.GetServer("localhost", 6379);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cachedData = await _distributedCache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cachedData))
                return default;

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var serialized = JsonSerializer.Serialize(value);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(30)
            };

            await _distributedCache.SetStringAsync(key, serialized, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            var keys = _redisServer.Keys(pattern: $"{prefix}*").ToList();

            foreach (var key in keys)
            {
                await _redisDb.KeyDeleteAsync(key);
            }
        }
    }
}
