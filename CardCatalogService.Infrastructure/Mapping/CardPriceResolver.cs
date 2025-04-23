using AutoMapper;
using CardCatalogService.Domain.Entities;
using CardCatalogService.Infrastructure.Models;

namespace CardCatalogService.Infrastructure.Mapping
{
    public class CardPriceResolver : IValueResolver<ExternalCardDto, Card, decimal>
    {
        public decimal Resolve(ExternalCardDto source, Card destination, decimal destMember, ResolutionContext context)
        {
            if (decimal.TryParse(source.CardPrices?.FirstOrDefault()?.CardmarketPrice, out var price))
                return price;

            return 0;
        }
    }
}
