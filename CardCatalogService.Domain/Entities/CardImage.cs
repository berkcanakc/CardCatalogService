namespace CardCatalogService.Domain.Entities
{
    public class CardImage
    {
        public int Id { get; set; } // PK
        public long ExternalImageId { get; set; } // API'den gelen ID
        public string Url { get; set; }
        public string UrlSmall { get; set; }
        public string UrlCropped { get; set; }

        public int CardId { get; set; } // FK
        public Card Card { get; set; }
    }
}
