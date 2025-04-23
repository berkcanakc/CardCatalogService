using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using CardCatalogService.Infrastructure.External;
using CardCatalogService.Infrastructure.Interfaces;
using CardCatalogService.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace CardCatalogService.Infrastructure.External
{
    public class YgoProDeckClient : IYgoProDeckClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<YgoProDeckClient> _logger;

        private const string Endpoint = "https://db.ygoprodeck.com/api/v7/cardinfo.php";

        public YgoProDeckClient(HttpClient httpClient, ILogger<YgoProDeckClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ExternalCardDto>> GetAllCardsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(Endpoint);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadFromJsonAsync<YgoProDeckResponse>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return responseBody?.Data ?? new List<ExternalCardDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[YgoProDeckClient] Kartlar çekilirken hata oluştu.");
                return new List<ExternalCardDto>();
            }
        }
    }
}
