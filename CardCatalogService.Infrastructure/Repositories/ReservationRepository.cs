using CardCatalogService.Application.Interfaces;
using CardCatalogService.Domain.Entities;
using CardCatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardCatalogService.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CardReservation?> GetActiveReservationAsync(int cardId, int userId, int cartId)
        {
            return await _context.CardReservations
                .Where(r => r.CardId == cardId && r.UserId == userId && r.CartId == cartId)
                .Where(r => !r.IsConfirmed && !r.IsReleased)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CardReservation>> GetActiveReservationsByCartIdAsync(int cartId)
        {
            return await _context.CardReservations
                .Where(r => r.CartId == cartId)
                .Where(r => !r.IsConfirmed && !r.IsReleased)
                .ToListAsync();
        }
        public async Task<List<CardReservation>> GetActiveReservationsByCardIdAsync(int cardId)
        {
            return await _context.CardReservations
                .Where(r => r.CardId == cardId)
                .Where(r => !r.IsConfirmed && !r.IsReleased)
                .ToListAsync();
        }

        public async Task AddAsync(CardReservation reservation)
        {
            await _context.CardReservations.AddAsync(reservation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
