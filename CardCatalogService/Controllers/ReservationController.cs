using CardCatalogService.Application.Interfaces;
using CardCatalogService.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CardCatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // Kullanıcı sepete ürün eklediğinde kullanılır
        [HttpPost("reserve")]
        public async Task<IActionResult> Reserve([FromBody] ReserveRequest request)
        {
            var result = await _reservationService.ReserveAsync(
                request.CardId,
                request.UserId,
                request.CartId,
                request.Quantity
            );

            return Ok(result);
        }

        // Kullanıcı sepeti onayladığında veya sipariş verdiğinde kullanılır
        [HttpPost("confirm-batch")]
        public async Task<IActionResult> ConfirmBatch([FromBody] ReservationBatchRequest request)
        {
            await _reservationService.ConfirmBatchAsync(request);
            return Ok();
        }

        // Kullanıcı sepeti boşalttığında veya süresi dolduğunda kullanılır
        [HttpPost("release-batch")]
        public async Task<IActionResult> ReleaseBatch([FromBody] ReservationBatchRequest request)
        {
            await _reservationService.ReleaseBatchAsync(request);
            return Ok();
        }
    }
}
