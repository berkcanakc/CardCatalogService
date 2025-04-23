using AutoMapper;
using CardCatalogService.Application.DTOs;
using CardCatalogService.Domain.Entities;

namespace CardCatalogService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity ↔ DTO
            CreateMap<Card, CardDto>();
            CreateMap<CardDto, Card>();
            CreateMap<CardImage, CardImageDto>();
            CreateMap<CardImageDto, CardImage>();
        }
    }
}
