using AutoMapper;
using CardCatalogService.Application.DTOs;
using CardCatalogService.Application.Interfaces;

namespace CardCatalogService.Application.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public CardService(ICardRepository cardRepository, IMapper mapper, ICacheService cacheService)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<CardDto>> GetAllAsync()
        {
            const string cacheKey = "all-cards";

            // Önce cache'e bakalım
            var cachedData = await _cacheService.GetAsync<IEnumerable<CardDto>>(cacheKey);
            if (cachedData is not null)
                return cachedData;

            // Cache'te yoksa veritabanından al
            var cards = await _cardRepository.GetAllAsync();

            var mapped = _mapper.Map<IEnumerable<CardDto>>(cards);

            // Veriyi cache'e yaz (30 dakika sakla)
            await _cacheService.SetAsync(cacheKey, mapped, TimeSpan.FromMinutes(30));

            return mapped;
        }

        public async Task<PagedList<CardDto>> SearchPagedAsync(CardSearchParameters parameters)
        {
            var (cards, totalCount) = await _cardRepository.SearchPagedAsync(parameters);
            var dtos = _mapper.Map<List<CardDto>>(cards);
            return new PagedList<CardDto>(dtos, parameters.Page, parameters.PageSize, totalCount);
        }

        public async Task<PagedList<CardDto>> GetPagedAsync(int page, int pageSize)
        {
            var (cards, totalCount) = await _cardRepository.GetPagedAsync(page, pageSize);
            var dtos = _mapper.Map<List<CardDto>>(cards);
            return new PagedList<CardDto>(dtos, page, pageSize, totalCount);
        }

        public async Task<CardDto?> GetByIdAsync(int id)
        {
            string cacheKey = $"card-{id}";

            // 1. Önce cache'e bak
            var cachedCard = await _cacheService.GetAsync<CardDto>(cacheKey);
            if (cachedCard is not null)
                return cachedCard;

            // 2. DB'den çek
            var card = await _cardRepository.GetByIdAsync(id);
            if (card is null)
                return null;

            var dto = _mapper.Map<CardDto>(card);

            // 3. Cache'e yaz
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30));

            return dto;
        }


        public async Task UpdateStockAsync(int id, int newStock)
        {
            var card = await _cardRepository.GetByIdAsync(id);
            if (card == null)
                throw new Exception("Card not found");

            card.Stock = newStock;

            _cardRepository.UpdateAsync(card);
            await _cardRepository.SaveChangesAsync();
        }
    }
}
