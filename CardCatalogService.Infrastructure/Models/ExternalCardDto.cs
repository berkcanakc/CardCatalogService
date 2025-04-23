using System.Text.Json.Serialization;

namespace CardCatalogService.Infrastructure.Models
{

    public class ExternalCardDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("desc")]
        public string Desc { get; set; }

        [JsonPropertyName("card_prices")]
        public List<CardPriceDto> CardPrices { get; set; }

        [JsonPropertyName("card_images")]
        public List<CardImageDto> CardImages { get; set; }
    }
}
