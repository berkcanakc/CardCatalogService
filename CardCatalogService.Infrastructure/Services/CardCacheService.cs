using CardCatalogService.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CardCatalogService.Infrastructure.Cache
{
    public class CardCacheService : ICardCacheService
    {
        private readonly ICacheService _cacheService;
        private const string CardPrefix = "Card_";

        public CardCacheService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public Task RemoveCardFromCache(int cardId)
        {
            // Belirli bir kartın cache'ini sil
            var cardCacheKey = $"{CardPrefix}{cardId}";
            var removeCardCache = _cacheService.RemoveAsync(cardCacheKey);

            // Tüm cache'leri temizle
            var removeAllCardCache = _cacheService.RemoveByPrefixAsync(CardPrefix);

            // Hem kart cache'ini hem de tüm cache'leri sil
            return Task.WhenAll(removeCardCache, removeAllCardCache);
        }

        public Task RemoveAllCardCache()
        {
            return _cacheService.RemoveByPrefixAsync(CardPrefix);
        }

        public Task InvalidateAllCards()
        {
            // Eğer Invalidate = RemoveAll ise, doğrudan reuse
            return RemoveAllCardCache();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            return await _cacheService.GetAsync<T>(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            await _cacheService.SetAsync(key, value, expiry);
        }

        public async Task RemoveAsync(string key)
        {
            await _cacheService.RemoveAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            await _cacheService.RemoveByPrefixAsync(prefix);
        }
    }
}
