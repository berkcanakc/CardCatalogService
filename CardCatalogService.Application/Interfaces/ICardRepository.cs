using CardCatalogService.Application.DTOs;
using CardCatalogService.Domain.Entities;

namespace CardCatalogService.Application.Interfaces
{
    public interface ICardRepository
    {
        Task<IEnumerable<Card>> GetAllAsync();
        Task<Card?> GetByIdAsync(int id);
        Task<Card?> GetByExternalIdAsync(int externalId);
        Task<(List<Card>, int)> SearchPagedAsync(CardSearchParameters parameters);
        Task<(List<Card> Data, int TotalCount)> GetPagedAsync(int page, int pageSize);
        Task AddAsync(Card card);
        Task UpdateAsync(Card card);
        Task SaveChangesAsync();
        Task<List<CardReservation>> GetActiveReservationsByCardIdAsync(int cardId);
    }
}
