using CardCatalogService.Application.Interfaces;

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
    }
}
