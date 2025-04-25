using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardCatalogService.Application.Interfaces
{
    public interface ICardCacheService
    {
        Task RemoveCardFromCache(int cardId);

        Task InvalidateAllCards();
    }
}
