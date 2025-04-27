using CardCatalogService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardCatalogService.Application.Interfaces
{
    public interface IReservationRepository
    {
        Task<CardReservation?> GetActiveReservationAsync(int cardId, int userId, int cartId);
        Task<List<CardReservation>> GetActiveReservationsByCartIdAsync(int cartId);
        Task AddAsync(CardReservation reservation);
        Task SaveChangesAsync();
        Task<List<CardReservation>> GetActiveReservationsByCardIdAsync(int cardId);
    }
}
