using CardCatalogService.Application.DTOs;

namespace CardCatalogService.Application.Interfaces
{
    public interface ICardService
    {
        Task<IEnumerable<CardDto>> GetAllAsync();
        Task<CardDto?> GetByIdAsync(int id);
        Task<PagedList<CardDto>> SearchPagedAsync(CardSearchParameters parameters);
        Task UpdateStockAsync(int id, int newStock);
    }
}
