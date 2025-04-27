namespace CardCatalogService.Application.DTOs
{
    public class CardDto
    {
        public int Id { get; set; }
        public int ExternalId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CalculatedAvailableStock { get; set; }

        public List<CardImageDto> CardImages { get; set; }
    }
}
