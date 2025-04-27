using CardCatalogService.Application.Interfaces;
using CardCatalogService.Application.DTOs;
using CardCatalogService.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace CardCatalogService.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ICardRepository _cardRepository;

        public ReservationService(IReservationRepository reservationRepository, ICardRepository cardRepository)
        {
            _reservationRepository = reservationRepository;
            _cardRepository = cardRepository;
        }

        public async Task<CardReservation> ReserveAsync(int cardId, int userId, int cartId, int quantity)
        {
            var reservation = new CardReservation
            {
                CardId = cardId,
                UserId = userId,
                CartId = cartId,
                Quantity = quantity
            };

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveChangesAsync();
            return reservation;
        }

        public async Task<bool> ConfirmBatchAsync(ReservationBatchRequest request)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var item in request.Reservations)
                {
                    var reservation = await _reservationRepository.GetActiveReservationAsync(item.CardId, request.UserId, request.CartId);
                    if (reservation == null) continue;

                    var card = await _cardRepository.GetByIdAsync(item.CardId);
                    if (card == null || card.Stock < reservation.Quantity)
                        throw new Exception($"Not enough stock for CardId: {item.CardId}");

                    reservation.IsConfirmed = true;
                    card.Stock -= reservation.Quantity;
                }

                await _reservationRepository.SaveChangesAsync();
                scope.Complete(); // Hepsi başarılıysa transaction commit edilir
            }

            return true;
        }

        public async Task<bool> ReleaseBatchAsync(ReservationBatchRequest request)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var item in request.Reservations)
                {
                    var reservation = await _reservationRepository.GetActiveReservationAsync(item.CardId, request.UserId, request.CartId);
                    if (reservation == null) continue;

                    reservation.IsReleased = true;
                }

                await _reservationRepository.SaveChangesAsync();
                scope.Complete(); // Hepsi başarılıysa transaction commit edilir
            }

            return true;
        }
    }
}
