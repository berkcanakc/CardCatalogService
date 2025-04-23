namespace CardCatalogService.Application.DTOs
{
    public class CardImageDto
    {
        public int Id { get; set; }             
        public int ExternalImageId { get; set; }
        public string Url { get; set; }         
        public string UrlSmall { get; set; }    
        public string UrlCropped { get; set; }  
    }
}
