using CardCatalogService.Application.DTOs;
using CardCatalogService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CardCatalogService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        // GET: api/card
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cards = await _cardService.GetAllAsync();
            return Ok(cards);
        }

        // GET: api/card/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var card = await _cardService.GetByIdAsync(id);
            if (card == null)
                return NotFound();

            return Ok(card);
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedList<CardDto>), 200)]
        public async Task<IActionResult> GetPaged([FromQuery] CardSearchParameters parameters)
        {
            var result = await _cardService.SearchPagedAsync(parameters);
            return Ok(result);
        }

        [HttpGet("search-paged")]
        [ProducesResponseType(typeof(PagedList<CardDto>), 200)]
        public async Task<IActionResult> SearchPaged([FromQuery] CardSearchParameters parameters)
        {
            var result = await _cardService.SearchPagedAsync(parameters);
            return Ok(result);
        }

        // PATCH: api/card/5/stock
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int stock)
        {
            try
            {
                await _cardService.UpdateStockAsync(id, stock);
                return NoContent();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
