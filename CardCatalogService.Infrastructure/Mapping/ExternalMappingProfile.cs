using AutoMapper;
using CardCatalogService.Domain.Entities;
using CardCatalogService.Infrastructure.Models;

namespace CardCatalogService.Infrastructure.Mapping
{
    public class ExternalMappingProfile : Profile
    {
        public ExternalMappingProfile()
        {
            // CardImageDto → CardImage
            CreateMap<CardImageDto, CardImage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalImageId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.UrlSmall, opt => opt.MapFrom(src => src.ImageUrlSmall))
                .ForMember(dest => dest.UrlCropped, opt => opt.MapFrom(src => src.ImageUrlCropped))
                .ForMember(dest => dest.CardId, opt => opt.Ignore())
                .ForMember(dest => dest.Card, opt => opt.Ignore());

            // ExternalCardDto → Card
            CreateMap<ExternalCardDto, Card>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // DB'nin belirlediği ID
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Desc))
                .ForMember(dest => dest.Price, opt => opt.MapFrom<CardPriceResolver>())
                .ForMember(dest => dest.Stock, opt => opt.Ignore()) // dış API'de yok
                .ForMember(dest => dest.CardImages, opt => opt.MapFrom(src => src.CardImages)); // nested liste map
        }
    }
}
