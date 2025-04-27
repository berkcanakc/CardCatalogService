using CardCatalogService.Application.DTOs;
using CardCatalogService.Domain.Entities;
using System.Threading.Tasks;

namespace CardCatalogService.Application.Interfaces
{
    public interface IReservationService
    {
        // Kullanıcı sepete bir ürün eklerken
        Task<CardReservation> ReserveAsync(int cardId, int userId, int cartId, int quantity);

        // Kullanıcı sepeti onaylayıp sipariş verirken
        Task<bool> ConfirmBatchAsync(ReservationBatchRequest request);

        // Sepetin süresi dolarsa veya kullanıcı sepeti boşaltırsa
        Task<bool> ReleaseBatchAsync(ReservationBatchRequest request);
    }
}
