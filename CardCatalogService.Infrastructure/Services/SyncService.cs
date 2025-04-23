using AutoMapper;
using CardCatalogService.Application.Interfaces;
using CardCatalogService.Domain.Entities;
using CardCatalogService.Infrastructure.External;
using CardCatalogService.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace CardCatalogService.Infrastructure.Services
{
    public class SyncService : ISyncService
    {
        private readonly IYgoProDeckClient _client;
        private readonly ICardRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<SyncService> _logger;

        public SyncService(
            IYgoProDeckClient client,
            ICardRepository repository,
            IMapper mapper,
            ILogger<SyncService> logger)
        {
            _client = client;
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task SyncCardsAsync()
        {
            var externalCards = await _client.GetAllCardsAsync();

            foreach (var externalCard in externalCards)
            {
                try
                {
                    var existingCard = await _repository.GetByExternalIdAsync(externalCard.Id);

                    if (existingCard == null)
                    {
                        var newCard = _mapper.Map<Card>(externalCard);
                        await _repository.AddAsync(newCard);
                    }
                    else
                    {
                        _mapper.Map(externalCard, existingCard); // güncelleme yapılır

                        // Görselleri sıfırla ve yeniden ekle
                        existingCard.CardImages.Clear();

                        if (externalCard.CardImages != null)
                        {
                            foreach (var imageDto in externalCard.CardImages)
                            {
                                var image = _mapper.Map<CardImage>(imageDto);
                                existingCard.CardImages.Add(image);
                            }
                        }

                        // EF zaten existingCard'ı track ettiği için Update gerekmez
                        // await _repository.UpdateAsync(existingCard); // bunu KALDIR
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[SYNC ERROR] Kart senkronizasyonunda hata. ExternalId: {externalCard.Id}");
                }
            }

            await _repository.SaveChangesAsync(); // en son topluca kayıt
        }
    }
}
