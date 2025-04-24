using CardCatalogService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CardCatalogService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly ISyncService _syncService;

        public SyncController(ISyncService syncService)
        {
            _syncService = syncService;
        }

        // POST: api/sync
        [HttpPost]
        public async Task<IActionResult> Sync()
        {
            throw new Exception("Test patlattık!");
            await _syncService.SyncCardsAsync();
            return Ok("Kartlar başarıyla senkronize edildi.");
        }
    }
}
