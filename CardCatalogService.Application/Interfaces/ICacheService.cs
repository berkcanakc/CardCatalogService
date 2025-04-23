namespace CardCatalogService.Application.Interfaces
{
    public interface ICacheService
    {
        // Verilen anahtara karşılık gelen veriyi getir
        // Tipi generic <T> olarak alır çünkü her türlü veri olabilir
        Task<T?> GetAsync<T>(string key);

        // Veriyi belirli bir anahtar altında kaydet
        // İsteğe bağlı olarak süre (expiry) verilebilir
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        // Anahtara karşılık gelen veriyi sil
        Task RemoveAsync(string key);
    }
}
