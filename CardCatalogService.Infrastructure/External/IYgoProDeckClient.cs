using CardCatalogService.Infrastructure.Models;

namespace CardCatalogService.Infrastructure.Interfaces
{
    public interface IYgoProDeckClient
    {
        Task<List<ExternalCardDto>> GetAllCardsAsync();
    }
}
