using CardCatalogService.Application.DTOs;
using CardCatalogService.Application.Interfaces;
using CardCatalogService.Domain.Entities;
using CardCatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CardCatalogService.Infrastructure.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly ApplicationDbContext _context;

        public CardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Card>> GetAllAsync()
        {
            return await _context.Cards.ToListAsync();
        }

        public async Task<Card?> GetByIdAsync(int id)
        {
            return await _context.Cards.FindAsync(id);
        }

        public async Task<Card?> GetByExternalIdAsync(int externalId)
        {
            return await _context.Cards
                .FirstOrDefaultAsync(c => c.ExternalId == externalId);
        }

        public async Task<(List<Card>, int)> SearchPagedAsync(CardSearchParameters parameters)
        {
            var query = _context.Cards.Include(c => c.CardImages).AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.Query))
            {
                var keyword = parameters.Query.Trim().ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(keyword) ||
                    c.Description.ToLower().Contains(keyword) ||
                    c.Type.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(parameters.TypeFilter))
            {
                var type = parameters.TypeFilter.Trim().ToLower();
                query = query.Where(c => c.Type.ToLower() == type);
            }

            if (parameters.MinPrice.HasValue)
            {
                query = query.Where(c => c.Price >= parameters.MinPrice.Value);
            }

            if (parameters.MaxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= parameters.MaxPrice.Value);
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(c => c.Id)
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (data, totalCount);
        }


        public async Task<(List<Card>, int)> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Cards.Include(c => c.CardImages);
            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }


        public async Task AddAsync(Card card)
        {
            await _context.Cards.AddAsync(card);
        }

        public async Task UpdateAsync(Card card)
        {
            _context.Cards.Update(card);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
