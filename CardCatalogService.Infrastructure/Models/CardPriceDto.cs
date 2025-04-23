using System.Text.Json.Serialization;

namespace CardCatalogService.Infrastructure.Models
{
    public class CardPriceDto
    {
        [JsonPropertyName("cardmarket_price")]
        public string CardmarketPrice { get; set; }
    }
}