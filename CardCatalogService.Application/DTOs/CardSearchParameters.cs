namespace CardCatalogService.Application.DTOs
{
    public class CardSearchParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? Query { get; set; } = string.Empty;
        public string? TypeFilter { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }

}
