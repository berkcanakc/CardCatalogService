namespace CardCatalogService.Domain.Entities
{
    public class Card
    {
        public int Id { get; set; }               // DB'deki primary key
        public int ExternalId { get; set; }       // Dış API'deki kart ID’si
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }        // Dış API'den gelen fiyat
        public int Stock { get; set; }            // Admin tarafından güncellenen stok
        public ICollection<CardImage> CardImages { get; set; }
    }
}
