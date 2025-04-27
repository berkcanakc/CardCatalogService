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
        private readonly ICardCacheService _cardCacheService;
        private readonly IReservationRepository _reservationRepository;

        public CardService(ICardRepository cardRepository, IMapper mapper, ICacheService cacheService, ICardCacheService cardCacheService, IReservationRepository reservationRepository)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _cardCacheService = cardCacheService;
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<CardDto>> GetAllAsync()
        {
            var cachedCards = await _cardCacheService.GetAsync<List<CardDto>>("AllCards");
            if (cachedCards is not null)
                return cachedCards.AsEnumerable(); // Dönüştürüyoruz

            var cards = await _cardRepository.GetAllAsync();
            var cardDtos = new List<CardDto>();

            foreach (var card in cards)
            {
                var activeReservations = await _reservationRepository.GetActiveReservationsByCardIdAsync(card.Id);
                var reservedQuantity = activeReservations.Sum(r => r.Quantity);

                var calculatedAvailableStock = card.Stock - reservedQuantity;

                cardDtos.Add(new CardDto
                {
                    Id = card.Id,
                    Name = card.Name,
                    Stock = card.Stock,
                    CalculatedAvailableStock = calculatedAvailableStock
                });
            }

            await _cardCacheService.SetAsync("AllCards", cardDtos, TimeSpan.FromMinutes(5));

            return cardDtos.AsEnumerable();
        }


        public async Task<PagedList<CardDto>> SearchPagedAsync(CardSearchParameters parameters)
        {
            string cacheKey = $"cards:search:{parameters.Page}:{parameters.PageSize}:{parameters.Query}:{parameters.MinPrice}:{parameters.MaxPrice}:{parameters.TypeFilter}";

            // Önce cache kontrolü
            var cached = await _cacheService.GetAsync<PagedList<CardDto>>(cacheKey);
            if (cached is not null)
                return cached;

            // Cache yoksa DB'den kartları çek
            var (cards, totalCount) = await _cardRepository.SearchPagedAsync(parameters);
            var dtos = new List<CardDto>();

            foreach (var card in cards)
            {
                var activeReservations = await _reservationRepository.GetActiveReservationsByCardIdAsync(card.Id);
                var reservedQuantity = activeReservations.Sum(r => r.Quantity);

                var calculatedAvailableStock = card.Stock - reservedQuantity;

                dtos.Add(new CardDto
                {
                    Id = card.Id,
                    Name = card.Name,
                    Stock = card.Stock,
                    CalculatedAvailableStock = calculatedAvailableStock
                });
            }

            var paged = new PagedList<CardDto>(dtos, parameters.Page, parameters.PageSize, totalCount);

            // Cache'e 10 dakikalığına at
            await _cacheService.SetAsync(cacheKey, paged, TimeSpan.FromMinutes(10));

            return paged;
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
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

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
