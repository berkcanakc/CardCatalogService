using CardCatalogService.Application.DTOs;

namespace CardCatalogService.Application.Interfaces
{
    public interface ICardService
    {
        Task<IEnumerable<CardDto>> GetAllAsync();
        Task<CardDto?> GetByIdAsync(int id);
        Task<PagedList<CardDto>> SearchPagedAsync(CardSearchParameters parameters);
        Task UpdateStockAsync(int id, int newStock);

        Task ReserveStockAsync(int id, int quantity);

        Task ReleaseStockAsync(int id, int quantity);

        Task CommitStockAsync(int id, int quantity);
    }
}
